using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
//using System.Reflection;
using ControllerFarmService.ResourceBaseService;
using Config = System.Configuration.ConfigurationManager;
using PFX = System.Threading.Tasks;
//using NLog;


namespace MITP
{
    [ServiceBehavior(Namespace = "http://escience.ifmo.ru/nano/services/", InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[ServiceBehavior(Namespace = "http://escience.ifmo.ru/nano/services/", InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ControllerFarmService : IControllerFarmService
    {
        public const string FARMID_PARAM_NAME = "FarmId";

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static ReaderWriterLockSlim _resourcesLock = new ReaderWriterLockSlim();

        private void CheckNodeConfigConsistency(ulong taskId, IEnumerable<NodeRunConfig> config, Resource resource)
        {
            if (config.Any(node => node.ResourceName != resource.ResourceName))
            {
                logger.Error("Node configs have different resources: " + String.Join(", ", config.Select(c => c.ResourceName)));
                throw new ArgumentException("All node configs should have the same resource name");
            }

            var unknownNodes = config.Select(n => n.NodeName).Except(resource.Nodes.Select(n => n.NodeName));
            if (unknownNodes.Any())
            {
                logger.Error("Task {0} has unknown nodes for resource {1}: {2}",
                    taskId, resource.ResourceName, String.Join(", ", unknownNodes)
                );

                throw new Exception("Wrong node config for task " + taskId.ToString() + ": " + String.Join(", ", unknownNodes));
            }
        }

        public void Run(TaskRunContext task)
        {
            _resourcesLock.EnterReadLock();

            try
            {
                logger.Info("Running task " + task.ToString());

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

                    logger.Info("Trying to run task {0} on resource {1}", task.TaskId, task.Resource.ResourceName);

                    task.LocalId = task.Controller.Run(task);

                    logger.Info("Task {0} ({1}) started on resource {2} with localId = {3}",
                        task.TaskId, task.PackageName, task.Resource.ResourceName, task.LocalId
                    );

                    var state = new TaskStateInfo(TaskState.Started, task.LocalId.ToString());
                    TaskCache.AddTask(task, state);
                }
                catch (Exception e)
                {
                    resourceCache.Release(task.NodesConfig);

                    logger.ErrorException("Unable to run task " + task.TaskId.ToString(), e);
                    throw;
                }
            }
            catch (Exception e)
            {
                logger.ErrorException(e, "Exception on Farm.Run(task {0})", task.TaskId);
                throw;
            }
            finally
            {
                _resourcesLock.ExitReadLock();
            }
        }

        public void Abort(ulong taskId)
        {
            _resourcesLock.EnterReadLock();

            try
            {
                logger.Info("Aborting task " + taskId.ToString());

                var task = TaskCache.GetById(taskId);
                task.UpdateStateAsync();

                lock (task.StateLock)
                {
                    if (task.StateInfo.State == TaskState.Started)
                    {
                        task.Context.Controller.Abort(task.Context);
                        logger.Info("Task aborted: " + taskId.ToString());

                        task.SetState(TaskState.Aborted, "Aborted by request"); // autorelease resources
                    }
                    else
                    {
                        logger.Warn("Task was not started: " + taskId.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Error on aborting task " + taskId.ToString(), e);
                throw;
            }
            finally
            {
                _resourcesLock.ExitReadLock();
            }
        }

        public TaskStateInfo GetTaskStateInfo(ulong taskId)
        {
            _resourcesLock.EnterReadLock();

            try
            {
                var task = TaskCache.GetById(taskId);
                task.UpdateStateAsync();

                lock (task.StateInfo)
                {
                    var taskState = new TaskStateInfo(task.StateInfo);
                    return taskState;
                }
            }
            catch (Exception e)
            {
                logger.ErrorException(e, "Error on getting task {0} state info", taskId);
                throw;
            }
            finally
            {
                _resourcesLock.ExitReadLock();
            }
        }

        public NodeStateInfo[] GetNodesState(string resourceName)
        {
            _resourcesLock.EnterReadLock();

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
                logger.ErrorException(e, "Error on getting resource '{0}' state info", resourceName);
                throw;
            }
            finally
            {
                _resourcesLock.ExitReadLock();
            }
        }

        public ulong[] GetActiveTaskIds()
        {
            _resourcesLock.EnterReadLock();

            try
            {
                ulong[] ids = TaskCache.GetActiveTaskIds();
                return ids;
            }
            catch (Exception e)
            {
                logger.ErrorException("Error on getting active tasks ids", e);
                throw;
            }
            finally
            {
                _resourcesLock.ExitReadLock();
            }
        }

        public string[] GetActiveResourceNames()
        {
            _resourcesLock.EnterReadLock();

            try
            {
                string[] names = ResourceCache.GetActiveResourceNames();
                return names;
            }
            catch (Exception e)
            {
                logger.ErrorException("Error on getting active resource names", e);
                throw;
            }
            finally
            {
                _resourcesLock.ExitReadLock();
            }
        }

        public void ReloadAllResources(string dumpingKey = null)
        {
            PFX.Task.Factory.StartNew(() =>
            {
                _resourcesLock.EnterWriteLock(); // the only update (i.e. write) to resources

                try
                {
                    logger.Info("Reloading resources for controller, key = '{0}'", dumpingKey ?? "");
                    Console.WriteLine("Reloading resources for controller");

                    TaskCache.DumpAllTasks();

                    var resourceBase = new ResourceBaseServiceClient();

                    try
                    {
                        string farmId = Config.AppSettings[FARMID_PARAM_NAME];

                        var resources = resourceBase.GetResourcesForFarm(farmId, dumpingKey); // waits all other dumps
                        resourceBase.Close();

                        string[] resourceNames = resources.Select(r => r.ResourceName).ToArray();
                        logger.Info("Resources to reload for farm {0}: {1}", farmId, String.Join(", ", resourceNames));

                        ResourceCache.ReloadResources(resources);
                        TaskCache.RestoreTasks(resourceNames);
                        //TaskCache.ReloadTasks(resourceNames);

                        PFX.Parallel.ForEach(resourceNames, (name) =>
                        {
                            ResourceCache.UpdateNodesState(name);
                        });

                        logger.Info("Resource reloading done for farm " + farmId);
                    }
                    catch (Exception e)
                    {
                        resourceBase.Abort();
                        logger.ErrorException("Exception on reloading resources", e);
                        throw;
                    }
                }
                finally
                {
                    _resourcesLock.ExitWriteLock();
                }
            });
        }

        ControllerFarmService()
        {
            bool loaded = false;
            //for (int retries = 0; !loaded && retries < 3; retries++)
            {
                try
                {
                    ReloadAllResources();
                    loaded = true;
                }
                catch (Exception e)
                {
                    logger.ErrorException("Could not load resources on server start. Retying.", e);
                    System.Threading.Thread.Sleep(200);
                }
            }

            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                logger.ErrorException("Exception in some TPL's task", e.Exception);
                e.SetObserved();
                //throw e.Exception;
            };  
        }
    }
}


