using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MITP
{
    [DataContract]
    public class VirtualNodeStateInfo
    {
        [DataMember] public string InstanceId { get; private set; }
        [DataMember] public string ImageId    { get; private set; }
        [DataMember] public string Address    { get; private set; }

        [DataMember] public VirtualNodeState State { get; private set; }

        public VirtualNodeStateInfo(string instanceId, string imageId, string address, VirtualNodeState state)
        {
            InstanceId = instanceId;
            ImageId    = imageId;
            Address    = address;
            State      = state;
        }
    }
}

