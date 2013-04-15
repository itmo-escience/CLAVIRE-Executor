using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Attributes;
using PFX = System.Threading.Tasks;

namespace MITP
{
    internal class TaskCache
    {
        private static readonly TimeSpan UPDATE_DURATION_TO_WARN = TimeSpan.FromSeconds(0.3);
        private static readonly TimeSpan UPDATE_INTERVAL = TimeSpan.FromMilliseconds(200);

        private static readonly Dictionary<ulong, TaskCache> _cache = new Dictionary<ulong, TaskCache>();
        private static readonly object _globalLock = new object();

        public readonly object StateLock = new object();

        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId _id { get; set; }

        public TaskRunContext Context { get; private set; }
        public TaskStateInfo StateInfo { get; private set; }       // mutable

        private bool _isUpdating { get; set; }                     // mutable
        private DateTime _lastUpdateTime { get; set; }             // mutable

        /*
        private TaskCache(TaskRunContext context)//, TaskState state = TaskState.Started, string stateComment = "")
        {
            Context = context;
            //StateInfo = new TaskStateInfo(state, stateComment);

            _isUpdating = false;
            _lastUpdateTime = DateTime.Now - UPDATE_INTERVAL - TimeSpan.FromMilliseconds(50);
        }
        */

        private TaskCache(TaskRunContext context, TaskStateInfo state)
        {
            Context = context;
            StateInfo = state;

            _isUpdating = false;
            _lastUpdateTime = DateTime.Now - UPDATE_INTERVAL - TimeSpan.FromMilliseconds(50);
        }

        private static TaskCache LoadTask(ulong taskId)
        {
            return null; // todo : deserialize struct

            var collection = Mongo.GetCollection<TaskCache>();
            var task = collection.AsQueryable<TaskCache>().Where(t => t.Context.TaskId == taskId).SingleOrDefault();
            return task;
        }

        private static TaskCache[] LoadTasks(IEnumerable<string> resourceNames)
        {
            return new TaskCache[0]; // todo : deserialize struct

            var collection = Mongo.GetCollection<TaskCache>();
            var tasks = collection.AsQueryable<TaskCache>().Where(t => t.Context.Resource.ResourceName.In(resourceNames)).ToArray();

            return new TaskCache[0];

            return tasks;
        }

        public void Save()
        {
            var collection = Mongo.GetCollection<TaskCache>();
            lock (StateLock)
            {
                collection.Save(this);
            }
        }

        public static void ReloadTasks(IEnumerable<string> resourceNames)
        {
            lock (_globalLock)
            {
                while (_cache.Any())
                {
                    var elem = _cache.First();
                    lock (elem.Value.StateLock)
                    {
                        elem.Value.Save();
                        _cache.Remove(elem.Key);
                    }
                }

                var tasks = LoadTasks(resourceNames);
                foreach (var task in tasks)
                {
                    _cache[task.Context.TaskId] = task;
                }
            }
        }

        public static void AddTask(TaskRunContext context, TaskStateInfo state)//, TaskState state = TaskState.Started, string stateComment = "")
        {
            var taskCache = new TaskCache(context, state);

            lock (_globalLock)
            {
                taskCache.Save(); // todo : move out of lock?
                _cache[context.TaskId] = taskCache;
            }
        }

        public static TaskCache GetById(ulong taskId)
        {
            TaskCache taskCache;
            lock (_globalLock)
            {
                if (!_cache.ContainsKey(taskId))
                {
                    var task = LoadTask(taskId);

                    if (task != null)
                    {
                        _cache[taskId] = task;
                    }
                    else
                    {
                        Log.Error("No cache for task " + taskId.ToString());
                        throw new ArgumentException("Unknown task for this farm");
                    }
                }

                taskCache = _cache[taskId];
            }
            return taskCache;
        }

        //public static void UpdateTaskState(TaskCache cache)
        public void UpdateState()
        {
            //var cache = GetById(taskId);

            // todo : PFX.Task.Run in .Net 4.5
            PFX.Task.Factory.StartNew(() =>
            {
                if (!this._isUpdating && (this._lastUpdateTime + UPDATE_INTERVAL < DateTime.Now || DateTime.Now < this._lastUpdateTime))
                {
                    bool wasFinished = false;
                    bool isThisThreadUpdating = false;
                    lock (this.StateLock)
                    {
                        if (!this._isUpdating && (this._lastUpdateTime + UPDATE_INTERVAL < DateTime.Now || DateTime.Now < this._lastUpdateTime))
                        {
                            this._isUpdating = true;
                            isThisThreadUpdating = true;

                            wasFinished = this.StateInfo.IsFinished();
                        }
                    }

                    if (isThisThreadUpdating)
                    {
                        try
                        {
                            //throw new Exception("test");

                            if (!wasFinished)
                            {
                                var timer = System.Diagnostics.Stopwatch.StartNew();

                                var newState = this.Context.Controller.GetTaskStateInfo(this.Context);

                                timer.Stop();
                                if (timer.Elapsed > UPDATE_DURATION_TO_WARN)
                                    Log.Warn(String.Format("Task {0} update took {1} seconds", this.Context.TaskId, timer.Elapsed.TotalSeconds)); // Context.TaskId is immutable

                                lock (this.StateLock)
                                {
                                    this.StateInfo = newState;
                                    this.Save();
                                }

                                if (!wasFinished && this.StateInfo.IsFinished())
                                {
                                    // todo : import to Storage
                                    var resource = ResourceCache.GetByName(this.Context.Resource.ResourceName);    // Context.Resource.ResourceName is immutable
                                    resource.Release(this.Context.NodesConfig);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            lock (this.StateLock)
                            {
                                this.StateInfo = new TaskStateInfo(TaskState.Failed, e.Message); // todo : retries
                                this.Save();
                            }

                            Log.Error(String.Format("Exception on update task {0}: {1}", this.Context.TaskId, e)); // Context.TaskId is immutable
                            throw;
                        }
                        finally
                        {
                            lock (this.StateLock)
                            {
                                this._lastUpdateTime = DateTime.Now;
                                this._isUpdating = false;
                            }
                        }
                    }
                }
            });
        }

        public static ulong[] GetActiveTaskIds()
        {
            lock (_globalLock)
            {
                var ids = new List<ulong>();
                foreach (var task in _cache.Values)
                {
                    lock (task.StateLock)
                    {
                        if (task.StateInfo.State == TaskState.Started)
                            ids.Add(task.Context.TaskId);
                    }
                }

                //ulong[] ids = _cache.Where(pair => pair.Value.StateInfo.State == TaskState.Started).Select(pair => pair.Key).ToArray(); // todo : WARNING! UNSAFE!
                ulong[] idsArray = ids.ToArray();
                return idsArray;
            }
        }
    }
}
