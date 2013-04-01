using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MITP
{
    public class ResourceBaseService : IResourceBaseService
    {
        public Resource[] GetAllResources()
        {
            var resources = Resource.Load().ToArray();
            return resources;
        }

        public string[] GetResourceNames()
        {
            var resources = GetAllResources();
            string[] names = resources.Select(r => r.ResourceName).ToArray();
            return names;
        }

        public Resource GetResourceByName(string resourceName)
        {
            var resources = GetAllResources();
            var res = resources.Single(r => r.ResourceName == resourceName);
            return res;
        }

        public string[] GetNodeNames(string resourceName)
        {
            var resources = GetAllResources();
            var res = resources.Single(r => r.ResourceName == resourceName);
            var nodeNames = res.Nodes.Select(n => n.NodeName).ToArray();
            return nodeNames;
        }

        public ResourceNode GetResourceNodeByName(string resourceName, string nodeName)
        {
            var resource = GetResourceByName(resourceName);
            var node = resource.Nodes.Single(n => n.NodeName == nodeName);
            return node;
        }

        public Resource[] GetResourcesForFarm(string farmId)
        {
            var allResources = GetAllResources();
            var resourcesForFarm = allResources.Where(r => r.Controller.FarmId.ToLowerInvariant() == farmId.ToLowerInvariant()).ToArray();

            return resourcesForFarm;
        }
    }
}
