using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using ControllerFarmService.Controllers.Entity;
using ControllerFarmService.Controllers.Error;
using MITP;
using MITP.Entity;
using Renci.SshNet;
using Ssh = Tamir.SharpSsh;
using Ssh2 = Renci.SshNet;
using ControllerFarmService.ResourceBaseService;

namespace ControllerFarmService
{
    public class ASshBasedController : AbstractResourceController
    {
        private const string SSH_FIND_COMMAND = "find -type f -size -500M"; 
            // '-500M' == 'less than 500 MB'
            // todo : REMOVE RESTRICTION FOR FILES MORE THAN 500 MB IN SIZE

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private SshConnectionPool _sshPool = new SshConnectionPool();
        private CopierConnectionPool _copierPool = new CopierConnectionPool();
        public static String ClavireStartFileName = "clavire_start_time.txt";
        public static String ClavireFinishFileName = "clavire_finish_time.txt";
        
        protected string SshExec(ResourceNode node, string command, string args = "", AErrorResolver errorResolver = null)
        {
            int sshResult = 0;
            string sshOut = "";
            string sshErr = "";
            string sshCommand = command + " " + args;

            var sshCommands = new[] {sshCommand};

            if (sshCommand.Contains('\n'))
            {
                sshCommands = sshCommand.Split('\n');
            }
            /*
            var connectionInfo = new Ssh2.ConnectionInfo(node.Services.ExecutionUrl, node.Credentials.Username, new Ssh2.AuthenticationMethod[] { 
                new Ssh2.KeyboardInteractiveAuthenticationMethod(node.Credentials.Username), 
                new Ssh2.PasswordAuthenticationMethod(node.Credentials.Username, node.Credentials.Password)
            });
            */


            var sshExec = _sshPool.GetSshSession(true, node);
            
                //Ssh.SshExec(node.NodeAddress, node.Credentials.Username, node.Credentials.Password);
            //Log.Info("Ssh command to execute : " + command + " ");
            try
            {
                foreach (var s in sshCommands)
                {
                    if (!s.Contains("pbsnodes") && !s.Contains("qstat"))
                        logger.Trace("Ssh command to execute: " + s);

                    /*
                    sshExec.Connect();
                    sshResult = sshExec.RunCommand(sshCommand, ref sshOut, ref sshErr);
                    /**/
                    
                    var ssh = sshExec.RunCommand(s); // todo : using (var ssh = new...)
                    //ssh.Execute();

                    sshResult = ssh.ExitStatus;
                    sshErr = ssh.Error;
                    sshOut = ssh.Result;

                    if (!String.IsNullOrWhiteSpace(sshErr))
                    {
                        break;
                    }
                }

                /**/
            }
            catch (Exception e)
            {
                logger.WarnException("SSH exec caused exception", e);
                throw;
            }
            finally
            {
                /*/*
                sshExec.Close();
                /*
                if (sshExec.IsConnected)
                    sshExec.Disconnect();
                /***/
                _sshPool.PushSession(sshExec);
            }

            //sshErr = sshErr.Replace('.', ' '); // Cert creation emits many dots
            if (errorResolver == null && sshResult != 0 /*!String.IsNullOrWhiteSpace(sshErr)*/)
            {
                throw new Exception(String.Format("Ssh execution error. Command: \"{0}\". Code: {1}, StdOut: {2}, StdErr: {3}", sshCommand, sshResult, sshOut, sshErr));
            }
            if (errorResolver != null && !String.IsNullOrWhiteSpace(sshErr))
            {
                var resIn = new Dictionary<String, Object>();
                resIn[AErrorResolver.SSH_RESULT] = sshOut;
                resIn[AErrorResolver.SSH_EXIT_CODE] = sshResult;
                resIn[AErrorResolver.SSH_COMMAND] = sshCommand;
                resIn[AErrorResolver.SSH_ERROR] = sshErr;

                errorResolver.Resolve(resIn);
            }

            if (!command.Contains("pbsnodes") && !command.Contains("qstat"))
                logger.Trace("ssh execution result : " + sshOut);

            return sshOut;
        }

        protected void UploadFile(ResourceNode node, string remotePath, string localPath)
        {
            SecureCopier copier = null;

            try
            {
                copier = _copierPool.GetSshSession(true, node);
                copier.UploadFile(node, remotePath, localPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (copier != null)
                    _copierPool.PushSession(copier);
            }
        }

        protected void DownloadFile(ResourceNode node, string remotePath, string localPath)
        {
            SecureCopier copier = null;

            try
            {
                copier = _copierPool.GetSshSession(true, node);
                copier.DownloadFile(node, remotePath, localPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (copier != null)
                    _copierPool.PushSession(copier);
            }
        }

        public string CopyInputFiles(TaskRunContext task, out string fileNames)
        {
            var node = GetNode(task);

            //string ftpFolder = IncarnationParams.IncarnatePath(node.DataFolders.ExchangeUrlFromSystem, taskId, CopyPhase.In);
            //string jobFtpFolder = IncarnationParams.IncarnatePath(node.DataFolders.ExchangeUrlFromSystem, taskId, CopyPhase.None);
            //string ftpInputFolder = IncarnationParams.IncarnatePath(node.DataFolders.ExchangeUrlFromResource, taskId, CopyPhase.In);
            //string ftpOutputFolder = IncarnationParams.IncarnatePath(node.DataFolders.ExchangeUrlFromResource, taskId, CopyPhase.Out);
            string clusterHomeFolder = IncarnationParams.IncarnatePath(node.DataFolders.LocalFolder, task.TaskId, CopyPhase.None);

            //IOProxy.Ftp.MakePath(ftpInputFolder);
            //IOProxy.Ftp.MakePath(ftpOutputFolder);

            try
            {
                logger.Trace(Thread.CurrentThread.ManagedThreadId + " entered.");

                SshExec(node, "mkdir " + clusterHomeFolder);

                logger.Trace(Thread.CurrentThread.ManagedThreadId + " exited.");
            }
            catch (Exception e)
            {
                logger.WarnException("SSH exec in CopyInputFiles caused exception", e);
            }

            logger.Info("Copying input files for task {0}", task.TaskId);
            fileNames = ""; //String.Join(" ", incarnation.FilesToCopy.Select(f => f.FileName));
            foreach (var file in task.InputFiles)
            {
                string tmpFile = Path.GetTempFileName();
                IOProxy.Storage.Download(file.StorageId, tmpFile);

                string fileOnCluster = clusterHomeFolder.TrimEnd(new[] { '/', '\\' }) + "/" + file.FileName;
                fileNames += " " + fileOnCluster;

                logger.Debug("Copying file " + fileOnCluster);
                //ScpCopy(node, fileOnCluster, tmpFile);
                UploadFile(node, fileOnCluster, tmpFile);
                File.Delete(tmpFile);
            }

            return clusterHomeFolder;
        }


        public void CopyOutputsToExchange(TaskRunContext task)
        {
            ulong taskId = task.TaskId;
            var node = GetNode(task);
            var pack = PackageByName(node, task.PackageName);

            // temporary hack: files are not pushed from resource => using path from resource for scp copying
            string outFolderFromSystem = IncarnationParams.IncarnatePath(node.DataFolders.ExchangeUrlFromResource, taskId, CopyPhase.Out);
            //string outFolderFromSystem = IncarnationParams.IncarnatePath(node.DataFolders.ExchangeUrlFromSystem, taskId, CopyPhase.Out);
            bool copyingOutsToFtp = outFolderFromSystem.StartsWith("ftp://");
            if (copyingOutsToFtp && !outFolderFromSystem.EndsWith("/"))
                outFolderFromSystem += '/';
            if (!copyingOutsToFtp && !outFolderFromSystem.EndsWith("\\"))
                outFolderFromSystem += '\\';

            string clusterFolder = IncarnationParams.IncarnatePath((!String.IsNullOrEmpty(pack.LocalDir)) ? String.Format(pack.LocalDir, task.TaskId) : node.DataFolders.LocalFolder, taskId, CopyPhase.Out);
            if (!clusterFolder.EndsWith("/"))
                clusterFolder += "/";

            //var files = ImproveFiles(task.Incarnation.ExpectedOutputFileNames);
            /*                var fileNames =
                                SshExec(node, SshPbsCommands.Find, clusterFolder)
                                    .Split(new[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries)
                                    .Where(st => !st.Contains("/"))
                                    .Select(st => st.Replace("*", "").Replace("|", "").Replace("\n",""))
                                    .Where(st => !st.Contains(".rst") && !st.Contains(".err") && !st.Contains(".esav"));*/

            var fileNames = SshExec(node, "cd " + clusterFolder + "; " + SSH_FIND_COMMAND, "")
                            .Replace("./", "/").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(st => !st.Contains(".rst") /*&& !st.Contains(".err")*/ && !st.Contains(".esav"))
                            .Select(st => st.Trim(new[] { '/', '\\' }));

            //IOProxy.Ftp.MakePath(ftpOutFolderFromSystem);
            var dirStructure = fileNames
                .Where(name => name.Contains('/') || name.Contains('\\')) // inside subdir
                .Select(name => name.Remove(name.LastIndexOfAny(new[] { '\\', '/' })))
                .Distinct()
                .Select(file => outFolderFromSystem + file)
                .Union(new[] { outFolderFromSystem });
            foreach (string dir in dirStructure)
            {
                if (copyingOutsToFtp)
                    IOProxy.Ftp.MakePath(dir);
                else
                {
                    logger.Debug("Creating dir " + dir);
                    Directory.CreateDirectory(dir);
                }
            }


            logger.Info("Copying output files for task {0}", task.TaskId);
            //System.Threading.Tasks.Parallel.ForEach(fileNames, (fileName) =>
            foreach (string fileName in fileNames)
            {
                //if (files.Contains(fileName)) 
                {
                    string tmpFile = Path.GetTempFileName();
                    try
                    {
                        logger.Debug("Copying file " + clusterFolder + fileName);
                        //ScpGet(node, clusterFolder + fileName, tmpFile, false);
                        DownloadFile(node, clusterFolder + fileName, tmpFile);

                        if (copyingOutsToFtp)
                            IOProxy.Ftp.UploadLocalFile(tmpFile, outFolderFromSystem, fileName, shouldCreatePath: false);
                        else
                            File.Copy(tmpFile, outFolderFromSystem + fileName);

                        File.Delete(tmpFile);
                        logger.Debug("File copied " + fileName);
                    }
                    catch (Ssh.SshTransferException e)
                    {
                        logger.WarnException(e, "SshTransferException on file '{0}' copy for task {1}", clusterFolder + fileName, task.TaskId);
                    }
                    catch (Exception e)
                    {
                        logger.WarnException(e, "Exception on file '{0}' copy for task {1}", clusterFolder + fileName, task.TaskId);
                    }
                }
            }//);
        }
    }
}

