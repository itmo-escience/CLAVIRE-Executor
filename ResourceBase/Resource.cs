using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Config = System.Web.Configuration.WebConfigurationManager;

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
    public partial class Resource
    {
        public string Json { get; private set; }

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

                    if (!template.TrimStart().StartsWith("<#@"))
                    {
                        string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
                        string assemblyDir = Path.GetDirectoryName(assemblyLocation);

                        var used_assemblies_location = Directory.GetFiles(assemblyDir, "*.dll").Where(name => !name.ToLowerInvariant().Contains("mono.texttemplating"));

                        string new_template = @"<#@ template debug=""false"" hostspecific=""false"" language=""C#"" #>" + Environment.NewLine; // inherits=""MITP.Resource""
                        foreach (string location in used_assemblies_location)
                            new_template += String.Format(@"<#@ assembly name=""{0}"" #>{1}", location, Environment.NewLine);

                        new_template += @"<#@ import namespace=""System.Collections.Generic"" #>" + Environment.NewLine;
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

                    if (!res.IsInScheduledDowntime())
                        resources.Add(res);
                }
                catch (Exception e)
                {
                    Log.Warn(String.Format("Could not deserialize resource file {0}: {1}\n{2}", filePath, e.Message, e.StackTrace));
                }
            }

            return resources;
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


