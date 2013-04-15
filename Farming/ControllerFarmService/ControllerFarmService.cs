using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Reflection;
using ControllerFarmService.ResourceBaseService;
using Config = System.Configuration.ConfigurationManager;
//using NLog;

namespace MITP
{
    [ServiceBehavior(Namespace = "http://escience.ifmo.ru/nano/services/", InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[ServiceBehavior(Namespace = "http://escience.ifmo.ru/nano/services/", InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ControllerFarmService : IControllerFarmService
    {
        public const string FARMID_PARAM_NAME = "FarmId";

        //private static Logger Log = LogManager.GetCurrentClassLogger();

        private void CheckNodeConfigConsistency(ulong taskId, IEnumerable<NodeRunConfig> config, Resource resource)
        {
            if (config.Any(node => node.ResourceName != resource.ResourceName))
            {
                Log.Error("Node configs have different resources: " + String.Join(", ", config.Select(c => c.ResourceName)));
                throw new ArgumentException("All node configs should have the same resource name");
            }

            var unknownNodes = config.Select(n => n.NodeName).Except(resource.Nodes.Select(n => n.NodeName));
            if (unknownNodes.Any())
            {
                Log.Error(String.Format(
                    "Task {0} has unknown nodes for resource {1}: {2}",
                    taskId, resource.ResourceName, String.Join(", ", unknownNodes)
                ));
                throw new Exception("Wrong node config for task " + taskId.ToString() + ": " + String.Join(", ", unknownNodes));
            }
        }

        public void Run(TaskRunContext task)
        {
            try
            {
                Log.Info("Running task " + task.ToString());

                string resourceName = task.NodesConfig.First().ResourceName;
                var resourceCache = ResourceCache.GetByName(resourceName);

                lock (resourceCache.StateLock)
                {
                    CheckNodeConfigConsistency(task.TaskId, task.NodesConfig, resourceCache.Resource);

                    task.Resource = resourceCache.Resource;
                    task.Controller = resourceCache.Controller;
                }

                try
                {
                    resourceCache.Acquire(task.NodesConfig);  // todo : m.b. move under resourceCache.StateLock?

                    Log.Info(String.Format("Trying to run task {0} on resource {1}", task.TaskId, task.Resource.ResourceName));

                    task.LocalId = task.Controller.Run(task);

                    Log.Info(String.Format("Task {0} ({1}) started on resource {2} with localId = {3}",
                        task.TaskId, task.Incarnation.PackageName, task.Resource.ResourceName, task.LocalId
                    ));

                    var state = new TaskStateInfo(TaskState.Started, task.LocalId.ToString());
                    TaskCache.AddTask(task, state);
                }
                catch (Exception e)
                {
                    resourceCache.Release(task.NodesConfig);

                    Log.Error(String.Format("Unable to run task {0}: {1}", task.TaskId, e));
                    throw;
                }
            }
            catch (Exception e)
            {
                Log.Error(String.Format("Exception on Farm.Run(task {0}): {1}", task.TaskId, e));
                throw;
            }
        }

        public void Abort(ulong taskId)
        {
            try
            {
                Log.Info("Aborting task " + taskId.ToString());

                var task = TaskCache.GetById(taskId);
                task.UpdateState();

                lock (task.StateLock)
                {
                    if (task.StateInfo.State == TaskState.Started)
                    {
                        task.Context.Controller.Abort(task.Context);
                        Log.Info("Task aborted: " + taskId.ToString());

                        task.StateInfo.State = TaskState.Aborted;
                        task.StateInfo.StateComment = "Aborted by request";
                        task.Save();

                        var resourceCache = ResourceCache.GetByName(task.Context.Resource.ResourceName);
                        resourceCache.Release(task.Context.NodesConfig);
                    }
                    else
                    {
                        Log.Warn("Task was not started: " + taskId.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(String.Format("Error on aborting task {0}: {1}}", taskId, e));
                throw;
            }
        }

        public TaskStateInfo GetTaskStateInfo(ulong taskId)
        {
            try
            {
                // todo : m.b. load from Mongo?
                var task = TaskCache.GetById(taskId);
                task.UpdateState();

                lock (task.StateInfo)
                {
                    var taskState = new TaskStateInfo(task.StateInfo);
                    return taskState;
                }
            }
            catch (Exception e)
            {
                Log.Error(String.Format("Error on getting task {0} state info: {1}", taskId, e));
                throw;
            }
        }

        public NodeStateInfo[] GetNodesState(string resourceName)
        {
            try
            {
                ResourceCache.UpdateNodesState(resourceName); // todo : unify. TaskCache.UpdateState(id) OR task.UpdateState()
                var resourceCache = ResourceCache.GetByName(resourceName);

                lock (resourceCache.StateLock)
                {
                    var nodesState = resourceCache.NodeStateInfo.Select(state => new NodeStateInfo(state)).ToArray();
                    return nodesState;
                }
            }
            catch (Exception e)
            {
                Log.Error(String.Format("Error on getting resource '{0}' state info: {1}", resourceName, e));
                throw;
            }
        }

        public ulong[] GetActiveTaskIds()
        {
            try
            {
                ulong[] ids = TaskCache.GetActiveTaskIds();
                return ids;
            }
            catch (Exception e)
            {
                Log.Error("Error on getting active tasks ids: " + e.ToString());
                throw;
            }
        }

        public string[] GetActiveResourceNames()
        {
            try
            {
                string[] names = ResourceCache.GetActiveResourceNames();
                return names;
            }
            catch (Exception e)
            {
                Log.Error("Error on getting active resource names: " + e.ToString());
                throw;
            }
        }

        public void ReloadAllResources()
        {
            Log.Info("Reloading resources for controller");
            var resourceBase = new ResourceBaseServiceClient();

            try
            {
                string farmId = Config.AppSettings[FARMID_PARAM_NAME];

                var resources = resourceBase.GetResourcesForFarm(farmId);
                resourceBase.Close();

                string[] resourceNames = resources.Select(r => r.ResourceName).ToArray();
                Log.Info("Resources to reload for farm " + farmId + ": " + String.Join(", ", resourceNames));

                ResourceCache.ReloadResources(resources);
                TaskCache.ReloadTasks(resourceNames);

                Log.Info("Resource reloading done for farm " + farmId);
            }
            catch (Exception e)
            {
                resourceBase.Abort();
                Log.Error("Exception on reloading resources: " + e.ToString());
                throw;
            }
        }

        ControllerFarmService()
        {
            bool loaded = false;
            for (int retries = 0; !loaded && retries < 3; retries++)
            {
                try
                {
                    ReloadAllResources();
                    loaded = true;
                }
                catch (Exception e)
                {
                    Log.Error("Could not load resources on server start. Retying. " + e.ToString());
                    System.Threading.Thread.Sleep(200);
                }
            }

            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Log.Error("Exception in some TPL's task: " + e.ToString());
                e.SetObserved();
                //throw e.Exception;
            };  
        }
    }
}


