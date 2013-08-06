extern alias ModelAdjust;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easis.PackageBase;
using Easis.PackageBase.Client;
using Easis.PackageBase.Engine;
using Easis.PackageBase.Definition;
using Easis.PackageBase.Types;
using ServiceProxies.ResourceBaseService;

using estimationAdjuster = ModelAdjust.ModelEstimation.MLEstimator;

namespace MITP
{
    // todo : remove this hack. History have to be read from database
    public class HistorySample
    {
        public string Package { get; private set; }
        public string ResourceName { get; private set; }
        public NodeConfig[] NodesConfig { get; private set; }

        public Dictionary<string, string> PackParams { get; private set; }
        public Dictionary<string, double> ModelCoefs { get; private set; }

        public TimeSpan CalcTime { get; private set; }
        
        public PackageEngine EstimatorEngine { get; private set; }

        public HistorySample(
            string package, string resourceName, NodeConfig[] nodesConfig, 
            Dictionary<string, string> packParams, Dictionary<string, double> modelCoefs,
            TimeSpan calcTime, PackageEngine estimatorEngine)
        {
            Package = package;
            ResourceName = resourceName;
            NodesConfig = nodesConfig;

            PackParams = packParams;
            ModelCoefs = modelCoefs;

            CalcTime = calcTime;

            EstimatorEngine = estimatorEngine;
        }
    }

    public partial class PackageBaseProxy
    {
        private const string FIXED_COEF_PREFIX = "model.";
        private const string ADJUSTABLE_COEF_PREFIX = "model*.";

        #region Coefs cache
        
        private const int NUM_RUNS_TO_RECOMPUTE_COEFS_CACHE = 10;
        private static readonly TimeSpan TIME_LIMIT_TO_COMPUTE_ADJUSTED_COEFS = TimeSpan.FromSeconds(1);

        private static Dictionary<string, Dictionary<string, double>> _cachedCoefValues = new Dictionary<string, Dictionary<string, double>>();
        private static Dictionary<string, int> _cachedSamplesCount = new Dictionary<string, int>();

        private static void SetRecomputedCoefsCache(string key, Dictionary<string, double> newCache, int samplesCount)
        {
            _cachedCoefValues[key]   = new Dictionary<string, double>(newCache);
            _cachedSamplesCount[key] = samplesCount;
        }

        private static Dictionary<string, double> GetCoefsCahce(string key, out int samplesCount)
        {
            if (!_cachedSamplesCount.ContainsKey(key))
                _cachedSamplesCount[key] = 0;

            if (!_cachedCoefValues.ContainsKey(key))
                _cachedCoefValues[key] = new Dictionary<string, double>();

            samplesCount = _cachedSamplesCount[key];
            var cacheCopy = new Dictionary<string, double>(_cachedCoefValues[key]);
            return cacheCopy;
        }
        
        #endregion

        #region History
        // todo : remove this hack. History have to be read from database

        private static object _historyLock = new object();
        private static List<HistorySample> _history = new List<HistorySample>();

        public static void AddHistorySample(HistorySample historySample)
        {
            lock (_historyLock)
            {
                _history.Add(historySample);
            }
        }

        public static HistorySample[] GetHistorySamples(string package, string resourceName, string nodeName)
        {
            lock (_historyLock)
            {
                return _history.Where(hs =>
                    hs.Package.ToLowerInvariant() == package &&
                    hs.ResourceName == resourceName &&
                    hs.NodesConfig.Single().NodeName == nodeName)
                .ToArray();
            }
        }
        #endregion

        private static Dictionary<string, double> AutoAdjustCoefsByHistory(PackageEngine engine, IEnumerable<Resource> resources, ResourceNode node, Dictionary<string, object> fixedCoefs, Dictionary<string, double> adjustableCoefs)
        {
            lock (_historyLock)
            {   
                string packageName = engine.CompiledMode.ModeQName.PackageName.ToLowerInvariant();
                var history = GetHistorySamples(packageName, node.ResourceName, node.NodeName);

                string cacheKey = String.Format("{0}.{1}.{2}", node.ResourceName, node.NodeName, packageName);                
                int cachedSamplesCount;
                var cachedCoefs = GetCoefsCahce(cacheKey, out cachedSamplesCount);

                if (history.Length < cachedSamplesCount + NUM_RUNS_TO_RECOMPUTE_COEFS_CACHE)
                {
                    //Log.Debug("Not enough history samples to adjust coefs");
                    //return null;

                    return cachedCoefs;
                }

                Log.Debug("Recomputing model coefs for " + cacheKey);

                List<string> coefNames = history.SelectMany(h => h.ModelCoefs.Keys).Distinct()
                    .Union(fixedCoefs.Keys.Distinct())
                    .Union(adjustableCoefs.Keys.Distinct())
                    .OrderBy(name => name)
                    .ToList();

                double[,] historySampleNum = new double[history.Length, 2];
                for (int i = 0; i < historySampleNum.GetLength(0); i++)
                {
                    historySampleNum[i, 0] = i;
                    historySampleNum[i, 1] = history[i].CalcTime.TotalSeconds;
                }

                Func<double[], Dictionary<string, double>> coefVectorToDict = (double[] c) =>
                {
                    var dict = new Dictionary<string, double>();
                    for (int i = 0; i < c.Length; i++)
                        dict[coefNames[i]] = c[i];
                    return dict;
                };

                double[] lastCoefs = null;
                ModelEstimation lastEstim = null;

                Func<double[], double[], ModelEstimation> getEstim = (c, x) =>
                {
                    try
                    {
                        if (lastEstim != null && lastCoefs != null &&
                            lastCoefs.Zip(c, (c1, c2) => Math.Abs(c1 - c2)).All(diff => diff < 1e-12))
                            return lastEstim;

                        var h = history[(int) x[0]];

                        Dictionary<string, object> coefs = coefVectorToDict(c).ToDictionary(
                            pair => pair.Key,
                            pair => (object) pair.Value
                        );

                        var estimNode = resources.Single(r => r.ResourceName == h.NodesConfig.Single().ResourceName)
                            .Nodes.Single(n => n.NodeName == h.NodesConfig.Single().NodeName);

                        lastEstim = EstimateOnNode(h.EstimatorEngine, coefs, estimNode);
                        lastCoefs = (double[]) c.Clone();
                        return lastEstim;
                    }
                    catch (Exception e)
                    {
                        Log.Warn("Error on model adjust: " + e.ToString());
                        return null;
                    }
                };
                
                estimationAdjuster.meanFunction  getMean  = (c, x) => 
                {
                    var estim = getEstim(c, x);
                    return (estim != null)? estim.CalculationTime.Value: 0.0;
                };

                estimationAdjuster.sigmaFunction getSigma = (c, x) =>
                {
                    var estim = getEstim(c, x);
                    return (estim != null)? estim.CalculationTime.Dispersion: 0.0;
                };

                var estimator = new estimationAdjuster(historySampleNum, coefNames.Count, getMean, getSigma, 1.0e-4, false);

                var coefsStartingVector = new double[coefNames.Count];
                var isAdjustable        = new bool[coefNames.Count];

                foreach (var coefPair in adjustableCoefs)
                {
                    int pos = coefNames.IndexOf(coefPair.Key);
                    coefsStartingVector[pos] = coefPair.Value;
                    isAdjustable[pos] = true;
                }

                foreach (var coefPair in fixedCoefs)
                    coefsStartingVector[coefNames.IndexOf(coefPair.Key)] = (coefPair.Value is Double) ? (double)coefPair.Value : 0.0;

                if (isAdjustable.Any(b => b))
                {
                    Dictionary<string, double> adjusted = null;

                    try
                    {
                        Tools.ProcessWithTimeLimit(TIME_LIMIT_TO_COMPUTE_ADJUSTED_COEFS, () =>
                        {
                            var coefsAdjustedVector = estimator.estimate(coefsStartingVector, isAdjustable);
                            adjusted = coefVectorToDict(coefsAdjustedVector);
                        });
                    }
                    catch (TimeLimitException tle)
                    {
                        Log.Warn("Failed to recompute coefs in time: " + tle.ToString());
                        adjusted = cachedCoefs; // will recompute only if history changes => no repeated fails
                    }

                    SetRecomputedCoefsCache(cacheKey, adjusted, history.Length);

                    return adjusted;
                }
                else
                {
                    //return new Dictionary<string, double>(); // todo: return null?
                    return cachedCoefs;
                }
            }
        }

        private static Dictionary<string, object> GetModelCoefs(PackageEngine engine, IEnumerable<Resource> resources, ResourceNode node)
        {
            var fixedCoefs = new Dictionary<string, object>(engine.CompiledMode.Models.DefaultCoeffs);
            var adjustableCoefs  = new Dictionary<string, double>();

            string packageName = engine.CompiledMode.ModeQName.PackageName; // engineState._taskDescription.Package
            var packParams = node.PackageByName(packageName).Params;
            foreach (string paramKey in packParams.Keys)
            {
                string paramValue = packParams[paramKey];
                double paramValueAsDouble;
                bool isDouble = double.TryParse(paramValue,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture.NumberFormat,
                    out paramValueAsDouble);

                if (paramKey.ToLowerInvariant().StartsWith(FIXED_COEF_PREFIX))
                {
                    string coefName = paramKey.Remove(0, FIXED_COEF_PREFIX.Length);
                    if (isDouble)
                        fixedCoefs[coefName] = paramValueAsDouble;
                    else
                        fixedCoefs[coefName] = paramValue;
                }
                else
                if (paramKey.ToLowerInvariant().StartsWith(ADJUSTABLE_COEF_PREFIX))
                {
                    string coefName = paramKey.Remove(0, ADJUSTABLE_COEF_PREFIX.Length);
                    if (isDouble)
                        adjustableCoefs[coefName] = paramValueAsDouble;
                    else
                    {
                        fixedCoefs[coefName] = paramValue;
                        Log.Warn(String.Format(
                            "Cannot adjust param '{0}' for pack '{1}' on resource node '{2}.{3}' because it's value is not number",
                            paramKey, packageName,
                            node.ResourceName, node.NodeName
                        ));
                    }
                }
            }

            var adjustedCoefs = AutoAdjustCoefsByHistory(engine, resources, node, fixedCoefs, adjustableCoefs) ?? new Dictionary<string, double>();
            if (adjustableCoefs.Any())
                Log.Info("Model coefs were adjusted");

            var newCoefNames = adjustedCoefs.Keys.Except(adjustableCoefs.Keys);
            if (newCoefNames.Any())
                Log.Warn("Autoadjust created new coefs (ignoring them): " + String.Join(", ", newCoefNames));

            var modelCoefs = new Dictionary<string, object>(fixedCoefs);
            foreach (var coefName in adjustableCoefs.Keys)
            {
                if (adjustedCoefs.ContainsKey(coefName))
                    modelCoefs[coefName] = adjustedCoefs[coefName];
                else
                {
                    modelCoefs[coefName] = adjustableCoefs[coefName];
                    Log.Warn("Coef " + coefName + " was not returned as adjusted. Using non-adjusted value.");
                }
            }
            return modelCoefs;
        }

        public static Dictionary<NodeConfig, Estimation> GetEstimationsByModel(PackageEngineState engineState, IEnumerable<Resource> resources, IEnumerable<ResourceNode> permittedNodes)
        {
            var estims = new Dictionary<NodeConfig, Estimation>();
            Log.Debug("Estimating by model...");

            var engine = new PackageEngine(engineState.CompiledDef, engineState.EngineContext);
            if (!engine.CanEstimate())
            {
                Log.Debug("Can't estimate by model");
                return estims;
            }

            try
            {
                var allRes = resources.Select(r => new Common.Resource()
                {
                    Name = r.ResourceName,
                    Nodes = r.Nodes.Select(n => new Common.Node()
                    {
                        ResourceName = n.ResourceName,
                        DNSName = n.NodeName,
                        Parameters = new Dictionary<string, string>(n.StaticHardwareParams),
                        CoresAvailable = n.CoresAvailable,
                        CoresTotal = (int)n.CoresCount,
                    }).ToArray(),
                    Parameters = new Dictionary<string, string>(r.HardwareParams),
                });

                Log.Debug("Permitted nodes: " + String.Join(", ", permittedNodes.Select(n => n.ResourceName + "." + n.NodeName)));

                foreach (var node in permittedNodes)
                {
                    try
                    {
                        var res = allRes.Single(r => r.Name == node.ResourceName);
                        var modelCoefs = GetModelCoefs(engine, resources, node);

                        var modelEstimation = EstimateOnNode(engine, modelCoefs, node, res);

                        //double estimationInSeconds = engine.Estimate(modelExecParams, modelCoefs);
                        if (modelEstimation != null /* == no errors */ && modelEstimation.CalculationTime.IsSet &&
                            !Double.IsInfinity(modelEstimation.CalculationTime.Value) && !Double.IsNaN(modelEstimation.CalculationTime.Value))
                        {
                            var modelCoeffsToRemember = new Dictionary<string, double>();
                            foreach (var pair in modelCoefs)
                            {
                                if (pair.Value is double)
                                    modelCoeffsToRemember[pair.Key] = (double)pair.Value;
                            }

                            estims.Add
                            (
                                new NodeConfig() 
                                {
                                    ResourceName = node.ResourceName,
                                    NodeName = node.NodeName,
                                    Cores = (uint) 1, // was node.CoresAvailable
                                        // todo : node.pack.MinCores or node.pack.MaxCores
                                        // todo : estimation from model -> cores
                                },

                                new Estimation(modelEstimation)
                                {
                                    ModelCoeffs = modelCoeffsToRemember
                                }
                            );

                            /*
                            Log.Debug(String.Format("Estim by model on {0}.{1}: {2}",
                                node.ResourceName, node.NodeName,
                                (modelEstimation.CalculationTime.IsSet ? modelEstimation.CalculationTime.Value.ToString() : "not set")
                            ));
                            */
                        }
                        else
                        {
                            // todo : else Log.Trace estimation by model is NaN or Infinity
                            Log.Warn("Model estimation failed for task " + engineState._taskDescription.TaskId.ToString());
                        }
                    }
                    catch (Exception estimEx)
                    {
                        Log.Warn(String.Format(
                            "Exception while estimating task {1} on node '{2}' by models in PackageBase : {0}",
                            estimEx, engineState._taskDescription.TaskId, node.NodeName
                        ));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(String.Format(
                    "Exception while getting estimations from models in PackageBase for task {2}: {0}\n{1}",
                    e.Message, e.StackTrace,
                    engineState._taskDescription.TaskId
                ));
            }

            return estims;
        }    

        private static ModelEstimation EstimateOnNode(PackageEngine engine, Dictionary<string, object> modelCoefs, ResourceNode node, Common.Resource res = null)
        {
            var modelExecParams = new Dictionary<string, object>();
            foreach (string paramId in TimeMeter.ClusterParameterReader.GetAvailableParameterIds())   // from scheduler. Why?
            {
                try 
                {
                    if (res == null)
                    {
                        var n = new Common.Node()
                        {
                            ResourceName   = node.ResourceName,
                            DNSName        = node.NodeName,

                            CoresAvailable = node.CoresAvailable,
                            CoresTotal     = (int) node.CoresCount,

                            Parameters     = new Dictionary<string,string>(node.HardwareParams)
                        };

                        modelExecParams[paramId] = TimeMeter.ClusterParameterReader.GetValue(paramId, n);
                    }
                    else
                    {
                        var dest = new Common.LaunchDestination()
                        {
                            ResourceName = node.ResourceName,
                            NodeNames = new[] { node.NodeName },
                        };

                        modelExecParams[paramId] = TimeMeter.ClusterParameterReader.GetValue(paramId, res, dest);
                    }
                }
                catch (Exception) { /* it's ok not to extract all possible params */ }
            }

            var modelEstimation = engine.Estimate(modelExecParams, modelCoefs);
            if (engine.Ctx.Result.Messages.Any())
            {
                Log.Warn("Messages on estimation: " + String.Join("\n", engine.Ctx.Result.Messages.Select(mess => mess.Message)));
            }

            return modelEstimation;
        }
    }
}