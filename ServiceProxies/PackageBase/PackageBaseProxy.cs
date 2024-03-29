﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;
using Easis.PackageBase;
using Easis.PackageBase.Client;
using Easis.PackageBase.Engine;
using Easis.PackageBase.Definition;
using Easis.PackageBase.Types;
using Config = System.Web.Configuration.WebConfigurationManager;

namespace MITP
{
    public partial class PackageBaseProxy
    {
        private static readonly TimeSpan PB_PROCESS_TIME_LIMIT = TimeSpan.FromMinutes(3);

        private static ScriptedRepository GetRepository()
        {
            string repositoryUrl = Config.AppSettings["PackageBaseRepoUrl"];
            ScriptedRepository repo;

            if (repositoryUrl.ToLower().StartsWith("http://"))
                repo = new ScriptedRepository(new HttpFileLoader(repositoryUrl), new RubyScriptEngine());
            else
                repo = new ScriptedRepository(new FileSystemFileLoader(repositoryUrl), new RubyScriptEngine());

            return repo;
        }

        public static IEnumerable<string> GetSupportedPackageNames()
        {
            var repo = GetRepository();
            var names = repo.GetPackageList().Select(p => p.Name.Trim());
            return names;
        }

        internal static CompiledModeDef GetCompiledDef(string packageName, ScriptedRepository repository = null)
        {
            CompiledModeDef compiledDef = null;
            var repo = repository ?? GetRepository();

            ModeCompiler compiler = new ModeCompiler();
            var result = compiler.Compile(new ModeQName(packageName), repo, out compiledDef);
            CheckEngineResult(result);

            return compiledDef;
        }

        public static IDictionary<string, string> UpdateInputs(
            /* ref */ PackageEngineState engineState, 
            IEnumerable<TaskFileDescription> moreFiles)
        {
            var fileParams = moreFiles.ToDictionary(
                f => f.SlotName,
                f => (f.FileName ?? f.SlotName) + "|" + f.StorageId
            );

            return UpdateInputs(engineState, fileParams);
        }

        public static IDictionary<string, string> UpdateInputs(
            /* ref */ PackageEngineState engineState,
            IDictionary<string, string> moreParams)
        {
            var engine = new PackageEngine(engineState.CompiledDef, engineState.EngineContext);
            engine.UpdateContractParams(moreParams);

            var processedInputs = engine.GetEnabledInsValues().Where(inp => inp.IsSet);
            var newInputParams = engine.Stringify(processedInputs, engine.CompiledMode.Defs.Values);

            //var newInputParams = processedInputs.ToDictionary(
            //    inp => inp.VarName,
            //    inp => (inp.Value == null)? null: inp.Value.ToString()
            //);

            return newInputParams;
        }

        public static IncarnationParams ProcessInputFiles(/* ref */ PackageEngineState engineState, out TimeSpan inputFilesTime)
        {
            var engine = new PackageEngine(engineState.CompiledDef, engineState.EngineContext);

            var incarnation = new IncarnationParams();
            var fileManager = new InputFileManager(engineState.StoragePathBase);

            inputFilesTime = TimeSpanExt.Measure(() =>
            {
                TimeSpan timeLimit = PB_PROCESS_TIME_LIMIT;

                try
                {
                    Tools.ProcessWithTimeLimit(timeLimit, () =>
                    {
                        engine.PrepareInputs(fileManager);
                        CheckEngineResult(engine);
                    });
                }
                catch (TimeLimitException tle)
                {
                    throw new PackageBaseException("Inputs processing in Package base exceeded time limit (" + timeLimit.TotalSeconds + " seconds).", tle);
                }
                finally
                {
                    incarnation.FilesToCopy = incarnation.FilesToCopy.Concat(fileManager.GetInputFiles()).ToArray();
                    fileManager.Cleanup();
                }
            });

            object cmdLine = engine.Ctx.Memory[CmdLineDef.DefName].Value;
            //if (cmdLine == null)
                //throw new PackageBaseException("CommandLine generated by PackageBase is empty");
            incarnation.CommandLine = cmdLine.ToString();
            Log.Debug(String.Format(
                "Command line for pack {0}.{1} is '{2}' (task {3}). Cmdline object from PB is {4}",
                engine.CompiledMode.ModeQName.PackageName ?? "''", engine.CompiledMode.ModeQName.ModeName ?? "''",
                incarnation.CommandLine ?? "", engineState._taskDescription.TaskId,
                cmdLine == null ? "null" : "not null"
            ));

            if (String.IsNullOrWhiteSpace(incarnation.CommandLine))
                throw new PackageBaseException("CommandLine generated by PackageBase is empty");

            if (!engine.IsReadyToRun())
            {
                Log.Warn("PackageBase: task is not ready to run after PrepareInputs");

                Log.Error("PackageBase: task is not ready to run after PrepareInputs");
                throw new PackageBaseException("PackageBase: task is not ready to run after processing inputs");
            }

            return incarnation;
        }

        public static IEnumerable<string> ListExpectedOutputs(PackageEngineState engineState, out bool hasGroups)
        {
            var fileList = new List<string>();
            hasGroups = false;

            var engine = new PackageEngine(engineState.CompiledDef, engineState.EngineContext);
            var expectedOutFilesAndGroups = engine.GetEnabledOutDefs()
                .Where(def => def is IOutFileDef || def is OutFileGroupDefBase)
                .Where(def => engine.Ctx.Memory[def.Name].IsRequiredOrExpected);

            if (expectedOutFilesAndGroups.Any(def => def is OutFileGroupDefBase))
                hasGroups = true;

            foreach (var def in expectedOutFilesAndGroups)
            {
                if (def is IOutFileDef)
                {
                    var fileDef = def as IOutFileDef;
                    string filePath = 
                        (fileDef.Path.Trim(new[] { '/', '\\' }) + "/" + fileDef.ExpectedName)
                        .Trim(new[] { '/', '\\' })
                    ;

                    fileList.Add(filePath);
                }
            }

            return fileList;
        }

        public static Dictionary<string, string> ProcessOutputs(PackageEngineState engineState, string ftpRoot, out IEnumerable<TaskFileDescription> outFiles, out TimeSpan outputFilesTime)
        {
            var forbiddenFileNamesRx = (new string[] { 
                    @"\.\./", @"torque", @"\.dataList\.src$", @"\.hosts$", @"\.sh$", @"\.exe$"
                }).Select(st => new Regex(st, RegexOptions.IgnoreCase));

            string ftpRootWithoutSlash = ftpRoot.TrimEnd(new[] { '/', '\\' });
            var allFilePaths = IOProxy.Ftp.GetFileNamesInAllTree(ftpRootWithoutSlash);
            var filteredFilePathsWithRoot = allFilePaths.Where(st => !forbiddenFileNamesRx.Any(rx => rx.IsMatch(st)));
            var filteredFilePaths = filteredFilePathsWithRoot.Select(st => st.Substring(ftpRoot.Length));


            var fileManager = new OutputFileManager(ftpRoot, engineState.StoragePathBase);
            var engine = new PackageEngine(engineState.CompiledDef, engineState.EngineContext);

            var actionStarted = DateTime.Now;
            {
                TimeSpan timeLimit = PB_PROCESS_TIME_LIMIT;

                try
                {
                    Tools.ProcessWithTimeLimit(timeLimit, () =>
                    {
                        //engine.ProcessOutputs(outputFileNames, fileManager);
                        engine.ProcessOutputs(filteredFilePaths, fileManager);
                    });
                }
                catch (TimeLimitException tle)
                {
                    throw new PackageBaseException("Outputs processing in Package base exceeded time limit (" + timeLimit.TotalSeconds + " seconds).", tle);
                }
                finally
                {
                    outFiles = fileManager.GetOutputFiles(); // includes Cleanup();
                }
            }
            var actionFinished = DateTime.Now;
            outputFilesTime = actionFinished - actionStarted; // todo : fix ugliness

            var processedOuts = engine.GetEnabledOutsValues();
            var outFilesAndGroups = engine.GetEnabledOutDefs().Where(def => def is IOutFileDef || def is OutFileGroupDefBase);

            foreach (var def in outFilesAndGroups)
            {
                var memoryEntry = processedOuts.First(me => me.VarName == def.Name);

                if (memoryEntry.IsSet) // file was there
                {
                    if (def is IOutFileDef)
                    {
                        var extFileDef = memoryEntry.Value as ExternalFileDef;

                        extFileDef.Locator = outFiles.FirstOrDefault(fd => fd.SlotName == def.Name).StorageId;

                        if (extFileDef.Locator == null) // todo : null.StorageId will fail earlier
                        {
                            Log.Warn("No output file processed for param " + memoryEntry.VarName + ", and memoryEntry.IsSet == true");
                        }
                    }
                    else
                        if (def is OutFileGroupDefBase)
                        {
                            var group = def as OutFileGroupDefBase;
                            var extFileDefs = memoryEntry.Value as IEnumerable<ExternalFileDef>;

                            foreach (var extFileDef in extFileDefs)
                            {
                                extFileDef.Locator = outFiles.FirstOrDefault(file =>
                                    file.SlotName == def.Name &&
                                    file.FileName == extFileDef.FileName
                                ).StorageId;

                                if (extFileDef.Locator == null) // todo : null.StorageId will fail earlier
                                {
                                    Log.Warn(String.Format(
                                        "File '{0}' wasn't processed for param {1}, and memoryEntry.IsSet == true",
                                        extFileDef.FileName, memoryEntry.VarName
                                    ));
                                }
                            }
                        }
                }
            }

            //var outputParams = engine.Stringify(processedOuts, engine.CompiledMode.Outs.Values);

            var outputParams = PackageBase.Serializer.PackageBaseSerializer.Serialize(processedOuts, engine.CompiledMode.Outs.Values);
            Log.Debug(String.Format("Task's {0} output params are: {1}", engineState._taskDescription.TaskId,
                String.Join(", ", outputParams.Select(pair => pair.Key + " = " + pair.Value))
            ));

            return outputParams;
        }

        private static void CheckEngineResult(PackageEngine engine)
        {
            CheckEngineResult(engine.Ctx.Result);
        }

        private static void CheckEngineResult(PackageEngineResult engineResult)
        {
            if (!engineResult.Succeeded)
            {
                throw new PackageBaseException(
                    String.Join(
                        Environment.NewLine,
                        engineResult.Messages.Reverse().Select(mess => mess.Type.ToString() + " " + mess.Message)
                    )
                );
            }
        }
    }
}

