using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Xml.Linq;

namespace MITP
{
    [ServiceBehavior(Namespace = "http://escience.ifmo.ru/nano/services/", InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]    
    public class VirtualProviderService : IVirtualProviderService
    {
        const string AMAZON_PROVIDER_NAME = "Amazon EC2";

        private static byte[] GetHmacSha256(byte[] messageBytes, byte[] keyBytes)
        {
            /*
            String algorithmName = "HmacSHA256";
            var algo = KeyedHashAlgorithm.Create(algorithmName);
            algo.Key = keyBytes;/**/

            var algo = new HMACSHA256(keyBytes);
            byte[] hash = algo.ComputeHash(messageBytes);

            return hash;
        }

        private static string SignMessage(string keySecret, string baseUrl, string method, string message)
        {
            var uri = new Uri(baseUrl);

            string toSign =
                method + "\n" +
                uri.Host.ToLowerInvariant() /* todo : port added if non-standart (80, 443) */ + "\n" +
                uri.AbsolutePath /*"/"*/ + "\n" +
                message;

            byte[] keyBytes = Encoding.UTF8.GetBytes(keySecret);
            byte[] toSignBytes = Encoding.UTF8.GetBytes(toSign);
            byte[] signatureBytes = GetHmacSha256(toSignBytes, keyBytes);
            string signatureBase64 = Convert.ToBase64String(signatureBytes, Base64FormattingOptions.None);

            string signedMessage = message + "&Signature=" + Uri.EscapeDataString(signatureBase64); // todo : url-encode
            return signedMessage;
        }

        static string MakeExtendedMessage(string key, string action, Dictionary<string, string> options)
        {
            var messageParams = new Dictionary<string, string>(options)
            {
                { "Action", action },
                { "AWSAccessKeyId", key },
                { "Timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") },    // : -> \%3a
                { "SignatureVersion", "2" },
                { "SignatureMethod", "HmacSHA256" },
                { "Version", "2013-10-01" }
            };

            string extMessage = String.Join("&",
                messageParams
                    .Select(pair => pair.Key + "=" + Uri.EscapeDataString(pair.Value))
                    .OrderBy(st => st, StringComparer.Ordinal));

            return extMessage;
        }

        private static string SendActionRequest(VirtualPool pool, string method, string action, Dictionary<string, string> options = null)
        {
            if (options == null)
                options = new Dictionary<string, string>();

            string messageToSign = MakeExtendedMessage(pool.Credentials.Id, action, options);
            string signedMessage = SignMessage(pool.Credentials.Password, pool.ProviderUrl, method, messageToSign);

            string requestUrl = pool.ProviderUrl + "?" + signedMessage;

            using (var client = new System.Net.WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string response = client.DownloadString(requestUrl);

                return response;
            }
        }

        public void StartInstance(VirtualPool pool, string instanceId)
        {
            if (pool.ProviderType.ToLowerInvariant() == AMAZON_PROVIDER_NAME.ToLowerInvariant())
            {
                try
                {
                    string response = SendActionRequest(pool, "GET", "StartInstances",
                        new Dictionary<string, string>()
                        {
                            { "InstanceId.1", instanceId }
                        }
                    );

                    // todo: log response
                }
                catch // todo : log exception
                {
                }
            }
            else
                throw new Exception("Unknown virtual resource provider type");
        }

        public void StopInstance(VirtualPool pool, string instanceId)
        {
            if (pool.ProviderType.ToLowerInvariant() == AMAZON_PROVIDER_NAME.ToLowerInvariant())
            {
                try
                {
                    string response = SendActionRequest(pool, "GET", "StopInstances",
                        new Dictionary<string, string>()
                        {
                            { "InstanceId.1", instanceId }
                        }
                    );
    
                    // todo: log response
                }
                catch // todo : log exception
                {
                }
            }
            else
                throw new Exception("Unknown virtual resource provider type");
        }

        public Dictionary<string, VirtualNodeStateInfo> GetVirtualNodesState(VirtualPool pool)
        {
            if (pool.ProviderType.ToLowerInvariant() == AMAZON_PROVIDER_NAME.ToLowerInvariant())
            {
                string response = SendActionRequest(pool, "GET", "DescribeInstances"
                    /*
                     *  DescribeInstanceStatus
                     *  DescribeRegions
                     *  DescribeAvailabilityZones
                     *  DescribeInstances
                     * 
                     * */
                    /*
             new Dictionary<string, string> {
                 { "Filter.1.Name",  "region-name" },
                 { "Filter.1.Value", "eu-west-1" }

                 // {"InstanceId", "i-d096739d"}
             }  */
                );

                var responseXml = XElement.Parse(response);
                //string nameSpace = "{" + responseXml.GetDefaultNamespace().NamespaceName + "}";
                var nameSpace = responseXml.GetDefaultNamespace();                

                Func<string, VirtualNodeState> parseState = (st) => 
                { 
                    switch (st.ToLowerInvariant())
                    {
                        case "running": return VirtualNodeState.Started;
                        case "stopped": return VirtualNodeState.Stopped;
                        default: return VirtualNodeState.Pending;
                    }                    
                };

                var allInstances = responseXml
                        .Descendants(nameSpace + "instancesSet")
                            .Elements(nameSpace + "item").Select(it => 
                                new VirtualNodeStateInfo
                                (
                                    instanceId : it.Element(nameSpace + "instanceId").Value,
                                    imageId    : it.Element(nameSpace + "imageId").Value,
                                    //dnsName    : it.Element(nameSpace + "dnsName").Value,
                                    address  : (it.Elements(nameSpace + "ipAddress").SingleOrDefault() ?? new XElement("ipAddress")).Value,

                                    state      : parseState(it.Element(nameSpace + "instanceState").Element(nameSpace + "name").Value)
                                ));

                //var virtualNodes = allInstances.Where(inst => resource.Nodes.Any(n => n.NodeName == inst.instanceId));
                //var res = virtualNodes.ToDictionary(vn => vn.instanceId, vn => parseState(vn.state));

                var res = allInstances.ToDictionary(i => i.InstanceId, i => i);
                return res;
            }
            else
                throw new Exception("Unknown virtual resource provider type");            
        }
    }
}



