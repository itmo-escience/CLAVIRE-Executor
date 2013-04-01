﻿using System;
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
        [DataMember] public string Type { get; private set; }
        [DataMember] public string Url { get; private set; }
    }

    [DataContract]
    public class Resource
    {
        [DataMember] public string ResourceName { get; private set; } 
        [DataMember] public string ResourceDescription { get; private set; }
        [DataMember] public IEnumerable<string> SupportedArchitectures { get; private set; }

        [DataMember] public string Location { get; private set; }

        [DataMember] public ControllerDescription Controller { get; private set; }
        
        //[DataMember] public string ProviderName { get; private set; } // todo: Resource.ProviderName -> ResourceType
        //[DataMember] public string ProviderUrl;

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
                    string json = File.ReadAllText(filePath);
                    int startPos = json.IndexOf('{');
                    json = json.Substring(startPos);

                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Resource));
                    var memStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
                    var res = (Resource) serializer.ReadObject(memStream);

                    foreach (var node in res.Nodes)
                    {
                        node.ResourceName = res.ResourceName;
                        node.OverrideNulls(res._nodeDefaults);
                    }

                    resources.Add(res);
                }
                catch (Exception e)
                {
                    Log.Warn(String.Format("Could not deserialize resource file {0}: {1}\n{2}", filePath, e.Message, e.StackTrace));
                }
            }

            return resources;
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


