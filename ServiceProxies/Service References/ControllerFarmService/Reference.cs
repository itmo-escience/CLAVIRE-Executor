﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiceProxies.ControllerFarmService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TaskRunContext", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class TaskRunContext : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CommandLineField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] ExpectedOutputFileNamesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ServiceProxies.ControllerFarmService.FileContext[] InputFilesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ServiceProxies.ControllerFarmService.NodeRunConfig[] NodesConfigField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PackageNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ulong TaskIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UserCertField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CommandLine {
            get {
                return this.CommandLineField;
            }
            set {
                if ((object.ReferenceEquals(this.CommandLineField, value) != true)) {
                    this.CommandLineField = value;
                    this.RaisePropertyChanged("CommandLine");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] ExpectedOutputFileNames {
            get {
                return this.ExpectedOutputFileNamesField;
            }
            set {
                if ((object.ReferenceEquals(this.ExpectedOutputFileNamesField, value) != true)) {
                    this.ExpectedOutputFileNamesField = value;
                    this.RaisePropertyChanged("ExpectedOutputFileNames");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ServiceProxies.ControllerFarmService.FileContext[] InputFiles {
            get {
                return this.InputFilesField;
            }
            set {
                if ((object.ReferenceEquals(this.InputFilesField, value) != true)) {
                    this.InputFilesField = value;
                    this.RaisePropertyChanged("InputFiles");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ServiceProxies.ControllerFarmService.NodeRunConfig[] NodesConfig {
            get {
                return this.NodesConfigField;
            }
            set {
                if ((object.ReferenceEquals(this.NodesConfigField, value) != true)) {
                    this.NodesConfigField = value;
                    this.RaisePropertyChanged("NodesConfig");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PackageName {
            get {
                return this.PackageNameField;
            }
            set {
                if ((object.ReferenceEquals(this.PackageNameField, value) != true)) {
                    this.PackageNameField = value;
                    this.RaisePropertyChanged("PackageName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ulong TaskId {
            get {
                return this.TaskIdField;
            }
            set {
                if ((this.TaskIdField.Equals(value) != true)) {
                    this.TaskIdField = value;
                    this.RaisePropertyChanged("TaskId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UserCert {
            get {
                return this.UserCertField;
            }
            set {
                if ((object.ReferenceEquals(this.UserCertField, value) != true)) {
                    this.UserCertField = value;
                    this.RaisePropertyChanged("UserCert");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FileContext", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class FileContext : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FileNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StorageIdField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FileName {
            get {
                return this.FileNameField;
            }
            set {
                if ((object.ReferenceEquals(this.FileNameField, value) != true)) {
                    this.FileNameField = value;
                    this.RaisePropertyChanged("FileName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StorageId {
            get {
                return this.StorageIdField;
            }
            set {
                if ((object.ReferenceEquals(this.StorageIdField, value) != true)) {
                    this.StorageIdField = value;
                    this.RaisePropertyChanged("StorageId");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="NodeRunConfig", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class NodeRunConfig : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CoresField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NodeNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ResourceNameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint Cores {
            get {
                return this.CoresField;
            }
            set {
                if ((this.CoresField.Equals(value) != true)) {
                    this.CoresField = value;
                    this.RaisePropertyChanged("Cores");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NodeName {
            get {
                return this.NodeNameField;
            }
            set {
                if ((object.ReferenceEquals(this.NodeNameField, value) != true)) {
                    this.NodeNameField = value;
                    this.RaisePropertyChanged("NodeName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ResourceName {
            get {
                return this.ResourceNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ResourceNameField, value) != true)) {
                    this.ResourceNameField = value;
                    this.RaisePropertyChanged("ResourceName");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TaskStateInfo", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class TaskStateInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<string, System.TimeSpan> ActionsDurationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<string, double> ResourceConsuptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private MITP.TaskState StateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StateCommentField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, System.TimeSpan> ActionsDuration {
            get {
                return this.ActionsDurationField;
            }
            set {
                if ((object.ReferenceEquals(this.ActionsDurationField, value) != true)) {
                    this.ActionsDurationField = value;
                    this.RaisePropertyChanged("ActionsDuration");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, double> ResourceConsuption {
            get {
                return this.ResourceConsuptionField;
            }
            set {
                if ((object.ReferenceEquals(this.ResourceConsuptionField, value) != true)) {
                    this.ResourceConsuptionField = value;
                    this.RaisePropertyChanged("ResourceConsuption");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public MITP.TaskState State {
            get {
                return this.StateField;
            }
            set {
                if ((this.StateField.Equals(value) != true)) {
                    this.StateField = value;
                    this.RaisePropertyChanged("State");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StateComment {
            get {
                return this.StateCommentField;
            }
            set {
                if ((object.ReferenceEquals(this.StateCommentField, value) != true)) {
                    this.StateCommentField = value;
                    this.RaisePropertyChanged("StateComment");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="NodeStateInfo", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class NodeStateInfo : ServiceProxies.ControllerFarmService.NodeStateResponse {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CoresAvailableField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CoresCountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CoresReservedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ResourceNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint TasksSubmissionLimitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint TasksSubmittedField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CoresAvailable {
            get {
                return this.CoresAvailableField;
            }
            set {
                if ((this.CoresAvailableField.Equals(value) != true)) {
                    this.CoresAvailableField = value;
                    this.RaisePropertyChanged("CoresAvailable");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CoresCount {
            get {
                return this.CoresCountField;
            }
            set {
                if ((this.CoresCountField.Equals(value) != true)) {
                    this.CoresCountField = value;
                    this.RaisePropertyChanged("CoresCount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CoresReserved {
            get {
                return this.CoresReservedField;
            }
            set {
                if ((this.CoresReservedField.Equals(value) != true)) {
                    this.CoresReservedField = value;
                    this.RaisePropertyChanged("CoresReserved");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ResourceName {
            get {
                return this.ResourceNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ResourceNameField, value) != true)) {
                    this.ResourceNameField = value;
                    this.RaisePropertyChanged("ResourceName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint TasksSubmissionLimit {
            get {
                return this.TasksSubmissionLimitField;
            }
            set {
                if ((this.TasksSubmissionLimitField.Equals(value) != true)) {
                    this.TasksSubmissionLimitField = value;
                    this.RaisePropertyChanged("TasksSubmissionLimit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint TasksSubmitted {
            get {
                return this.TasksSubmittedField;
            }
            set {
                if ((this.TasksSubmittedField.Equals(value) != true)) {
                    this.TasksSubmittedField = value;
                    this.RaisePropertyChanged("TasksSubmitted");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="NodeStateResponse", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ServiceProxies.ControllerFarmService.NodeStateInfo))]
    public partial class NodeStateResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CoresUsedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<string, string> DynamicHardwareParamsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NodeNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ServiceProxies.ControllerFarmService.NodeState StateField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public uint CoresUsed {
            get {
                return this.CoresUsedField;
            }
            set {
                if ((this.CoresUsedField.Equals(value) != true)) {
                    this.CoresUsedField = value;
                    this.RaisePropertyChanged("CoresUsed");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, string> DynamicHardwareParams {
            get {
                return this.DynamicHardwareParamsField;
            }
            set {
                if ((object.ReferenceEquals(this.DynamicHardwareParamsField, value) != true)) {
                    this.DynamicHardwareParamsField = value;
                    this.RaisePropertyChanged("DynamicHardwareParams");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NodeName {
            get {
                return this.NodeNameField;
            }
            set {
                if ((object.ReferenceEquals(this.NodeNameField, value) != true)) {
                    this.NodeNameField = value;
                    this.RaisePropertyChanged("NodeName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ServiceProxies.ControllerFarmService.NodeState State {
            get {
                return this.StateField;
            }
            set {
                if ((this.StateField.Equals(value) != true)) {
                    this.StateField = value;
                    this.RaisePropertyChanged("State");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="NodeState", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    public enum NodeState : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Available = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Busy = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Down = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ControllerFarmService.IControllerFarmService")]
    public interface IControllerFarmService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IControllerFarmService/Run", ReplyAction="http://tempuri.org/IControllerFarmService/RunResponse")]
        void Run(ServiceProxies.ControllerFarmService.TaskRunContext task);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IControllerFarmService/Abort", ReplyAction="http://tempuri.org/IControllerFarmService/AbortResponse")]
        void Abort(ulong taskId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IControllerFarmService/GetTaskStateInfo", ReplyAction="http://tempuri.org/IControllerFarmService/GetTaskStateInfoResponse")]
        ServiceProxies.ControllerFarmService.TaskStateInfo GetTaskStateInfo(ulong taskId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IControllerFarmService/GetActiveTaskIds", ReplyAction="http://tempuri.org/IControllerFarmService/GetActiveTaskIdsResponse")]
        ulong[] GetActiveTaskIds();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IControllerFarmService/GetActiveResourceNames", ReplyAction="http://tempuri.org/IControllerFarmService/GetActiveResourceNamesResponse")]
        string[] GetActiveResourceNames();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IControllerFarmService/GetNodesState", ReplyAction="http://tempuri.org/IControllerFarmService/GetNodesStateResponse")]
        ServiceProxies.ControllerFarmService.NodeStateInfo[] GetNodesState(string resourceName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IControllerFarmService/ReloadAllResources", ReplyAction="http://tempuri.org/IControllerFarmService/ReloadAllResourcesResponse")]
        void ReloadAllResources();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IControllerFarmServiceChannel : ServiceProxies.ControllerFarmService.IControllerFarmService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ControllerFarmServiceClient : System.ServiceModel.ClientBase<ServiceProxies.ControllerFarmService.IControllerFarmService>, ServiceProxies.ControllerFarmService.IControllerFarmService {
        
        public ControllerFarmServiceClient() {
        }
        
        public ControllerFarmServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ControllerFarmServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ControllerFarmServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ControllerFarmServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void Run(ServiceProxies.ControllerFarmService.TaskRunContext task) {
            base.Channel.Run(task);
        }
        
        public void Abort(ulong taskId) {
            base.Channel.Abort(taskId);
        }
        
        public ServiceProxies.ControllerFarmService.TaskStateInfo GetTaskStateInfo(ulong taskId) {
            return base.Channel.GetTaskStateInfo(taskId);
        }
        
        public ulong[] GetActiveTaskIds() {
            return base.Channel.GetActiveTaskIds();
        }
        
        public string[] GetActiveResourceNames() {
            return base.Channel.GetActiveResourceNames();
        }
        
        public ServiceProxies.ControllerFarmService.NodeStateInfo[] GetNodesState(string resourceName) {
            return base.Channel.GetNodesState(resourceName);
        }
        
        public void ReloadAllResources() {
            base.Channel.ReloadAllResources();
        }
    }
}
