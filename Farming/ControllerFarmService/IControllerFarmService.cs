using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MongoDB.Bson;
using ControllerFarmService.ResourceBaseService;

namespace MITP
{
    [DataContract]
    public class TaskRunContext// todo : compare with Task and TaskDescription
    {
        public object LocalId { get; set; }

        [DataMember] public ulong TaskId { get; private set; }
        [DataMember] public IncarnationParams Incarnation { get; private set; } // todo : embed
        [DataMember] public IEnumerable<NodeConfig> NodesConfig { get; private set; }

        public Resource Resource { get; set; }
        public IStatelessResourceController Controller { get; set; }
    }

    [DataContract]
    public class TaskStateInfo
    {
        [DataMember] public TaskState State { get; set; }
        [DataMember] public string StateComment { get; set; }

        [DataMember] public Dictionary<string, TimeSpan> ActionsDuration { get; private set; }

        [DataMember] public Dictionary<string, double> ResourceConsuption { get; private set; }

        public bool IsFinished()
        {
            if (State != TaskState.Started)
                return true;

            return false;
        }

        public TaskStateInfo(TaskState currentState = TaskState.Started, string stateComment = "")
        {
            State = currentState;
            StateComment = stateComment;

            ActionsDuration = new Dictionary<string, TimeSpan>();
            ResourceConsuption = new Dictionary<string, double>();
        }

        public TaskStateInfo(TaskStateInfo otherTask)
        {
            State = otherTask.State;
            StateComment = otherTask.StateComment;

            ActionsDuration = new Dictionary<string, TimeSpan>(otherTask.ActionsDuration);
            ResourceConsuption = new Dictionary<string, double>(otherTask.ResourceConsuption);
        }
    }


    [ServiceContract]
    public interface IControllerFarmService
    {
        [OperationContract]
        void Run(TaskRunContext task);

        [OperationContract]
        void Abort(ulong taskId);

        [OperationContract]
        TaskStateInfo GetTaskStateInfo(ulong taskId); // should remember string providedTaskId, Resource resource, IEnumerable<NodeConfig> nodesConfig

        //public TaskHistory GetTaskHistory(ulong taskId); // todo : Task history

        [OperationContract]
        ulong[] GetActiveTaskIds();

        [OperationContract]
        string[] GetActiveResourceNames();

        [OperationContract]
        NodeStateInfo[] GetNodesState(string resourceName);

        [OperationContract]
        void ReloadAllResources();
    }

    /*
    struct TimeSlice<T>
    {
        DateTime Time;
        T Value;
    }

    class TaskHistory  // todo : Task history
    {
        [DataMember] TaskStateInfo[] States { get; set; }
        [DataMember] TaskStateInfo CurrentState { get { return States.LastOrDefault(); } }

        [DataMember] Dictionary<string, TimeSpan> ActionTimes;
        [DataMember] Dictionary<string, List<TimeSlice<double>>> ResourceConsuption;
    }
    */
}
