using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Config = System.Web.Configuration.WebConfigurationManager;

using ResourceBase.VirtualProviderService;

namespace MITP
{
    [DataContract]
    public class ControllerDescription
    {
        [DataMember] public string FarmId { get; private set; }
        [DataMember] public string Type   { get; private set; }
        [DataMember] public string Url    { get; private set; }
    }

    [DataContract]
    public class Resource
    {
        public string Json { get; internal set; }

        [DataMember] public string ResourceName { get; private set; } 
        [DataMember] public string ResourceDescription { get; private set; }
        [DataMember] public IEnumerable<string> SupportedArchitectures { get; private set; }

        [DataMember] public string Location { get; private set; }

        [DataMember] public ControllerDescription Controller { get; private set; }
        
        //[DataMember] public string ProviderName { get; private set; } // todo: Resource.ProviderName -> ResourceType
        //[DataMember] public string ProviderUrl;

        [DataMember] public string[][] ScheduledDowntime { get; private set; }

        [DataMember]
        public IDictionary<string, string> HardwareParams { get; private set; }


        [DataMember(IsRequired = false)]
        public VirtualPool VirtualPool { get; private set; }

        public bool IsVirtual { get { return VirtualPool != null; } }

        
        [DataMember(Name="NodeDefaults")] 
        private ResourceNode _nodeDefaults; // Used for node initialization

        [DataMember(Name="Nodes", IsRequired=true)]
        private IList<ResourceNode> _nodes { get; set; }
        public ReadOnlyCollection<ResourceNode> Nodes { get; private set; }

        private void Init()
        {
            if (Nodes == null)
                Nodes = new ReadOnlyCollection<ResourceNode>(_nodes);

            if (HardwareParams == null)
                HardwareParams = new Dictionary<string, string>();

            if (ScheduledDowntime == null)
                ScheduledDowntime = new string[0][];
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Init();
        }

        private Resource()
        {
            // Constructor is private. Resources can only be loaded from base
        }

        public static Resource BuildFromDescription(string json)
        {
            int startPos = json.IndexOf('{');
            json = json.Substring(startPos);

            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Resource));
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var res = (Resource)serializer.ReadObject(memStream);

            foreach (var node in res.Nodes)
            {
                node.ResourceName = res.ResourceName;
                node.OverrideNulls(res._nodeDefaults);
            }

            res.Json = json;
            return res;
        }

        public static IEnumerable<Resource> Load(IEnumerable<string> configurationFiles = null) // todo: Resource.configurationFiles -> urls
        {
            if (configurationFiles == null)
                configurationFiles = System.IO.Directory.GetFiles(Config.AppSettings["Resources"]);
                // todo : ./ and .\\ -> CONST.HomeFolder

            var resources = new List<Resource>();

            foreach (var filePath in configurationFiles)
            {
                try
                {
                    string template = File.ReadAllText(filePath);

                    if (!template.TrimStart().StartsWith("<#@")) // todo : measure performance of this
                    {
                        string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
                        string assemblyDir = Path.GetDirectoryName(assemblyLocation);

                        var used_assemblies_location = Directory.GetFiles(assemblyDir, "*.dll").Where(name => !name.ToLowerInvariant().Contains("mono.texttemplating"));

                        var refAssemblies = System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies();
                        var refAssembliesNames = refAssemblies
                            .Where(a => a.Name.StartsWith("System"))
                            .Select(a => System.Reflection.Assembly.Load(a.FullName).Location); // will not load it again
                        
                        string new_template = @"<#@ template debug=""false"" hostspecific=""false"" language=""C#"" #>" + Environment.NewLine; // inherits=""MITP.Resource""

                        foreach (string name in refAssembliesNames)
                            new_template += String.Format(@"<#@ assembly name=""{0}"" #>{1}", name, Environment.NewLine);

                        foreach (string location in used_assemblies_location)
                            new_template += String.Format(@"<#@ assembly name=""{0}"" #>{1}", location, Environment.NewLine);

                        new_template += @"<#@ import namespace=""System"" #>" + Environment.NewLine;
                        new_template += @"<#@ import namespace=""System.Collections.Generic"" #>" + Environment.NewLine;
                        new_template += @"<#@ import namespace=""System.Linq"" #>" + Environment.NewLine;
                        new_template += @"<#@ import namespace=""System.Text"" #>" + Environment.NewLine;
                        new_template += @"<#@ import namespace=""System.IO"" #>" + Environment.NewLine;
                        new_template += @"<#@ import namespace=""MITP"" #>" + Environment.NewLine;

                        template = new_template + template;


                        
                        /*template =
@"<#@ template debug=""false"" hostspecific=""false"" language=""C#"" #>
<#@ assembly name=""D:\Clavire\Executor\ResourceBase\bin\Debug\ResourceBase.dll"" #>
<#@ import namespace=""System.Collections.Generic"" #>" + Environment.NewLine + template;
//<#@ import namespace=""MITP.Resource"" #>" + Environment.NewLine + template;
//<#@ assembly name=""ResourceBase.dll"" #>
//<#@ import namespace=""System.Linq"" #>" + Environment.NewLine + template;
//<#@ output extension="".js"" #>" + Environment.NewLine + template;
                         */
                    }

                    var generator = new Mono.TextTemplating.TemplateGenerator();
                    //generator.AddParameter("", "parameter", "WorldName", "5");

                    var engine = new Mono.TextTemplating.TemplatingEngine();
                    string json = engine.ProcessTemplate(template, generator);                   
                    
                    
                    //string json = File.ReadAllText(filePath);
                    var res = Resource.BuildFromDescription(json);

                    res.UpdateVirtualNodesState();
                    res.InsertAddressValueInParams();

                    resources.Add(res);
                }
                catch (Exception e)
                {
                    Log.Warn(String.Format("Could not deserialize resource file {0}: {1}\n{2}", filePath, e.Message, e.StackTrace));
                }
            }

            return resources;
        }

        private void InsertAddressValueInParams()
        {
            foreach (var n in this.Nodes)
            {
                if (n.Services.ExecutionUrl.Contains("{address}") && !String.IsNullOrEmpty(n.Services.ExecutionUrl)) // todo: something wrong here
                    n.Services = new NodeServices(n.Services.ExecutionUrl
                        .Replace("{address}", n.NodeAddress)
                        .Replace("{Address}", n.NodeAddress)
                        .Replace("{nodeaddress}", n.NodeAddress)
                        .Replace("{nodeAddress}", n.NodeAddress));
            }
        }

        private void UpdateVirtualNodesState()
        {
            if (this.IsVirtual)
            {
                var service = new VirtualProviderServiceClient();

                try
                {
                    var virtualNodesState = service.GetVirtualNodesState(this.VirtualPool);

                    this.Json +=
                        "<# /* " +
                            String.Join("; ", virtualNodesState.Select(ns => 
                                String.Format("{0}, {1} = {2}",
                                    ns.Value.InstanceId,
                                    ns.Value.Address,
                                    ns.Value.State))) +
                        " */ #>";


                        
                    /*    
                        new ResourceBase.VirtualProviderService.Resource()
                        {
                            ResourceName = this.ResourceName,

                            VirtualPool = new ResourceBase.VirtualProviderService.VirtualPool()
                            {
                                BaseImage = this.VirtualPool.BaseImage,

                                Credentials = new ResourceBase.VirtualProviderService.VirtualPoolProviderCredentials()
                                {
                                    Id = this.VirtualPool.Credentials.Id,
                                    Password = this.VirtualPool.Credentials.Password
                                },

                                InstancesLimitMax = this.VirtualPool.InstancesLimitMax,
                                InstancesLimitMin = this.VirtualPool.InstancesLimitMax,

                                ProviderType = this.VirtualPool.ProviderType,
                                ProviderUrl  = this.VirtualPool.ProviderUrl,                                
                            },

                            Nodes = this.Nodes.Select(n => new ResourceBase.VirtualProviderService.ResourceNode()
                            {
                                ResourceName = n.ResourceName,
                                NodeName = n.NodeName,
                                NodeAddress = n.NodeAddress,                                
                            }).ToArray(),                            
                        }
                    );   */

                    foreach (var node in this.Nodes)
                    {
                        node.VirtualState = null;

                        if (virtualNodesState.ContainsKey(node.NodeName))
                        {
                            node.NodeAddress  = virtualNodesState[node.NodeName].Address;
                            node.VirtualState = virtualNodesState[node.NodeName].State;

                            switch (virtualNodesState[node.NodeName].State)
                            {                            
                                case VirtualNodeState.Started: /* OK */
                                    break;

                                default:
                                    node.TasksSubmissionLimit = 0;
                                    break;
                            }
                        }
                        else
                        {
                            node.TasksSubmissionLimit = 0;
                        }
                    }

                    service.Close();
                }
                catch (Exception e)
                {
                    service.Abort();

                    Log.Error("Exception while updating virtual nodes state: " + e.ToString());
                }
            }
        }



        public static string InstalledPackages(string resourceName, string nodeName)
        {
            return "";
            return @"			{
                ""Name"": ""TestP"",
                ""Version"": ""v1"",
                ""AppPath"": ""D:\\CLAVIRE\\_testp\\testp.exe""
			}";
        }

        public static Dictionary<string, string> GetInstalledPackagesForNode(string nodeName)
        {
            return new Dictionary<string, string>
            {
                { "testp", @"D:\\CLAVIRE\\_testp\\testp.exe" },
                //{ "testp2", @"D:\\CLAVIRE\\_testp\\testp.exe" }
            };
        }


        /*
        public IEnumerable<Tuple<ResourceNode, int>> NodesInConfig(ResourceConfig config)
        {
            return Nodes.Zip(config.Cores, (node, coresUsed) => Tuple.Create((coresUsed > 0)? node: null, coresUsed));
        }

        public ResourceNode FirstUsedNode(ResourceConfig config)
        {
            var nodeUsedWithCores = this.NodesInConfig(config).First(tup => tup.Item1 != null);
            var node = nodeUsedWithCores.Item1;
            return node;
        }

        public ResourceNode FirstUsedNodeCores(ResourceConfig config, out int cores)
        {
            var nodeUsedWithCores = this.NodesInConfig(config).First(tup => tup.Item1 != null);
            var node = nodeUsedWithCores.Item1;
            cores = nodeUsedWithCores.Item2;
            return node;
        }
        */
    }
}


