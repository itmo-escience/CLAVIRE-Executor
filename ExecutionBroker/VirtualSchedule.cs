using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceProxies.ResourceBaseService;

namespace MITP
{
    public static class VirtualSchedule
    {
        public static void ReconfigureVirtualMachines(
            IEnumerable<Task> tasks, IEnumerable<Resource> resources,
            IEnumerable<TaskDependency> dependencies,
            IDictionary<ulong, Dictionary<NodeConfig, Estimation>> estimationsForTask,
            IEnumerable<TaskSchedule> schedule)
        {
            foreach (var res in resources)
            {
                if (res.VirtualPool != null)
                {
                    foreach (var node in res.Nodes)
                    {
                        if (node.VirtualState != null && // todo: лишнее?
                            node.VirtualState.HasValue &&
                            node.VirtualState.Value == VirtualNodeState.Stopped)
                        {
                            if (tasks.Any(t => 
                                t.State == TaskState.ReadyToExecute &&
                                estimationsForTask[t.TaskId].Any(e => e.Value.CalcDuration > res.VirtualPool.AvgStartTime) &&
                                !schedule.Any(s => s.TaskId == t.TaskId && s.Action == ScheduledAction.Run)))
                            {
                                StartInstance(res.VirtualPool, node.NodeName);
                            }
                        }
                    }
                }
            }
        }

        private static void StartInstance(VirtualPool pool, string instanceId)
        {
            Log.Info("Starting instance " + instanceId);

            var service = Discovery.GetVirtualProviders();

            try
            {
                service.StartInstance(pool, instanceId);
                service.Close();
            }
            catch (Exception e)
            {
                Log.Error(String.Format("Excepiton on starting instance {0}: {1}",
                    instanceId, e));

                service.Abort();
            }
        }
    }
}

