using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MITP
{
    [DataContract]
    public class VirtualPool
    {
        [DataMember(IsRequired=true)] public string ProviderType { get; private set; }
        [DataMember(IsRequired=true)] public string ProviderUrl  { get; private set; }

        [DataMember(IsRequired=true)] public string BaseImage    { get; private set; }

        [DataMember] public TimeSpan AvgStartTime { get { return TimeSpan.FromSeconds(20); } private set { } }

        
        [DataMember] 
        public int InstancesLimitMin { get; private set; }

        [DataMember(IsRequired=true)] 
        public int InstancesLimitMax { get; private set; }

        [DataMember(IsRequired = true)]
        public VirtualPoolProviderCredentials Credentials { get; private set; }
    }

    [DataContract]
    public class VirtualPoolProviderCredentials
    {
        [DataMember(IsRequired=true)]
        public string Id { get; private set; }

        [DataMember(IsRequired=true)]
        public string Password { get; private set; }
    }
}
