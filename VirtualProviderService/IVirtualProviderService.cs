using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MITP
{
    [ServiceContract]
    public interface IVirtualProviderService
    {
        [OperationContract]
        Dictionary<string, VirtualNodeStateInfo> GetVirtualNodesState(VirtualPool pool);   // todo: dictionary -> array?

        [OperationContract]
        void StartInstance(VirtualPool pool, string instanceId);

        [OperationContract]
        void StopInstance(VirtualPool pool, string instanceId);
    }

}


