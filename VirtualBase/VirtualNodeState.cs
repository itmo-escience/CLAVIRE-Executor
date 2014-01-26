using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MITP
{
    [DataContract]
    public enum VirtualNodeState
    {
        [EnumMember] Starting,
        [EnumMember] Started,
        [EnumMember] Stopping,
        [EnumMember] Stopped,
        [EnumMember] Pending
    }
}

