﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1008
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiceProxies.ResourceBaseService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Resource", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class Resource : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ServiceProxies.ResourceBaseService.ControllerDescription ControllerField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<string, string> HardwareParamsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LocationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ServiceProxies.ResourceBaseService.ResourceNode NodeDefaultsField;
        
        private ServiceProxies.ResourceBaseService.ResourceNode[] NodesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ResourceDescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ResourceNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[][] ScheduledDowntimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] SupportedArchitecturesField;
        
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
        public ServiceProxies.ResourceBaseService.ControllerDescription Controller {
            get {
                return this.ControllerField;
            }
            set {
                if ((object.ReferenceEquals(this.ControllerField, value) != true)) {
                    this.ControllerField = value;
                    this.RaisePropertyChanged("Controller");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, string> HardwareParams {
            get {
                return this.HardwareParamsField;
            }
            set {
                if ((object.ReferenceEquals(this.HardwareParamsField, value) != true)) {
                    this.HardwareParamsField = value;
                    this.RaisePropertyChanged("HardwareParams");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Location {
            get {
                return this.LocationField;
            }
            set {
                if ((object.ReferenceEquals(this.LocationField, value) != true)) {
                    this.LocationField = value;
                    this.RaisePropertyChanged("Location");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ServiceProxies.ResourceBaseService.ResourceNode NodeDefaults {
            get {
                return this.NodeDefaultsField;
            }
            set {
                if ((object.ReferenceEquals(this.NodeDefaultsField, value) != true)) {
                    this.NodeDefaultsField = value;
                    this.RaisePropertyChanged("NodeDefaults");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public ServiceProxies.ResourceBaseService.ResourceNode[] Nodes {
            get {
                return this.NodesField;
            }
            set {
                if ((object.ReferenceEquals(this.NodesField, value) != true)) {
                    this.NodesField = value;
                    this.RaisePropertyChanged("Nodes");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ResourceDescription {
            get {
                return this.ResourceDescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.ResourceDescriptionField, value) != true)) {
                    this.ResourceDescriptionField = value;
                    this.RaisePropertyChanged("ResourceDescription");
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
        public string[][] ScheduledDowntime {
            get {
                return this.ScheduledDowntimeField;
            }
            set {
                if ((object.ReferenceEquals(this.ScheduledDowntimeField, value) != true)) {
                    this.ScheduledDowntimeField = value;
                    this.RaisePropertyChanged("ScheduledDowntime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] SupportedArchitectures {
            get {
                return this.SupportedArchitecturesField;
            }
            set {
                if ((object.ReferenceEquals(this.SupportedArchitecturesField, value) != true)) {
                    this.SupportedArchitecturesField = value;
                    this.RaisePropertyChanged("SupportedArchitectures");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="ControllerDescription", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class ControllerDescription : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FarmIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UrlField;
        
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
        public string FarmId {
            get {
                return this.FarmIdField;
            }
            set {
                if ((object.ReferenceEquals(this.FarmIdField, value) != true)) {
                    this.FarmIdField = value;
                    this.RaisePropertyChanged("FarmId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Type {
            get {
                return this.TypeField;
            }
            set {
                if ((object.ReferenceEquals(this.TypeField, value) != true)) {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Url {
            get {
                return this.UrlField;
            }
            set {
                if ((object.ReferenceEquals(this.UrlField, value) != true)) {
                    this.UrlField = value;
                    this.RaisePropertyChanged("Url");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="ResourceNode", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class ResourceNode : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint CoresCountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ServiceProxies.ResourceBaseService.NodeCredentials CredentialsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ServiceProxies.ResourceBaseService.NodeDataFolders DataFoldersField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<string, string> HardwareParamsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] OtherSoftwareField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ServiceProxies.ResourceBaseService.PackageOnNode[] PackagesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ResourceNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ServiceProxies.ResourceBaseService.NodeServices ServicesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private uint TasksSubmissionLimitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NodeNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NodeAddressField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] SupportedArchitecturesField;
        
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
        public ServiceProxies.ResourceBaseService.NodeCredentials Credentials {
            get {
                return this.CredentialsField;
            }
            set {
                if ((object.ReferenceEquals(this.CredentialsField, value) != true)) {
                    this.CredentialsField = value;
                    this.RaisePropertyChanged("Credentials");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ServiceProxies.ResourceBaseService.NodeDataFolders DataFolders {
            get {
                return this.DataFoldersField;
            }
            set {
                if ((object.ReferenceEquals(this.DataFoldersField, value) != true)) {
                    this.DataFoldersField = value;
                    this.RaisePropertyChanged("DataFolders");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, string> HardwareParams {
            get {
                return this.HardwareParamsField;
            }
            set {
                if ((object.ReferenceEquals(this.HardwareParamsField, value) != true)) {
                    this.HardwareParamsField = value;
                    this.RaisePropertyChanged("HardwareParams");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] OtherSoftware {
            get {
                return this.OtherSoftwareField;
            }
            set {
                if ((object.ReferenceEquals(this.OtherSoftwareField, value) != true)) {
                    this.OtherSoftwareField = value;
                    this.RaisePropertyChanged("OtherSoftware");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ServiceProxies.ResourceBaseService.PackageOnNode[] Packages {
            get {
                return this.PackagesField;
            }
            set {
                if ((object.ReferenceEquals(this.PackagesField, value) != true)) {
                    this.PackagesField = value;
                    this.RaisePropertyChanged("Packages");
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
        public ServiceProxies.ResourceBaseService.NodeServices Services {
            get {
                return this.ServicesField;
            }
            set {
                if ((object.ReferenceEquals(this.ServicesField, value) != true)) {
                    this.ServicesField = value;
                    this.RaisePropertyChanged("Services");
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
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=9)]
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
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=10)]
        public string NodeAddress {
            get {
                return this.NodeAddressField;
            }
            set {
                if ((object.ReferenceEquals(this.NodeAddressField, value) != true)) {
                    this.NodeAddressField = value;
                    this.RaisePropertyChanged("NodeAddress");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=11)]
        public string[] SupportedArchitectures {
            get {
                return this.SupportedArchitecturesField;
            }
            set {
                if ((object.ReferenceEquals(this.SupportedArchitecturesField, value) != true)) {
                    this.SupportedArchitecturesField = value;
                    this.RaisePropertyChanged("SupportedArchitectures");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="NodeCredentials", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class NodeCredentials : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string UsernameField;
        
        private string PasswordField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CertFileField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string Username {
            get {
                return this.UsernameField;
            }
            set {
                if ((object.ReferenceEquals(this.UsernameField, value) != true)) {
                    this.UsernameField = value;
                    this.RaisePropertyChanged("Username");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=1)]
        public string Password {
            get {
                return this.PasswordField;
            }
            set {
                if ((object.ReferenceEquals(this.PasswordField, value) != true)) {
                    this.PasswordField = value;
                    this.RaisePropertyChanged("Password");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public string CertFile {
            get {
                return this.CertFileField;
            }
            set {
                if ((object.ReferenceEquals(this.CertFileField, value) != true)) {
                    this.CertFileField = value;
                    this.RaisePropertyChanged("CertFile");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="NodeDataFolders", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class NodeDataFolders : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExchangeUrlFromSystemField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExchangeUrlFromResourceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool CopyLocalField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LocalFolderField;
        
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
        public string ExchangeUrlFromSystem {
            get {
                return this.ExchangeUrlFromSystemField;
            }
            set {
                if ((object.ReferenceEquals(this.ExchangeUrlFromSystemField, value) != true)) {
                    this.ExchangeUrlFromSystemField = value;
                    this.RaisePropertyChanged("ExchangeUrlFromSystem");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public string ExchangeUrlFromResource {
            get {
                return this.ExchangeUrlFromResourceField;
            }
            set {
                if ((object.ReferenceEquals(this.ExchangeUrlFromResourceField, value) != true)) {
                    this.ExchangeUrlFromResourceField = value;
                    this.RaisePropertyChanged("ExchangeUrlFromResource");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public bool CopyLocal {
            get {
                return this.CopyLocalField;
            }
            set {
                if ((this.CopyLocalField.Equals(value) != true)) {
                    this.CopyLocalField = value;
                    this.RaisePropertyChanged("CopyLocal");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public string LocalFolder {
            get {
                return this.LocalFolderField;
            }
            set {
                if ((object.ReferenceEquals(this.LocalFolderField, value) != true)) {
                    this.LocalFolderField = value;
                    this.RaisePropertyChanged("LocalFolder");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="NodeServices", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class NodeServices : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string ExecutionUrlField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string ExecutionUrl {
            get {
                return this.ExecutionUrlField;
            }
            set {
                if ((object.ReferenceEquals(this.ExecutionUrlField, value) != true)) {
                    this.ExecutionUrlField = value;
                    this.RaisePropertyChanged("ExecutionUrl");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="PackageOnNode", Namespace="http://schemas.datacontract.org/2004/07/MITP")]
    [System.SerializableAttribute()]
    public partial class PackageOnNode : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] CleanupField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] CleanupIgnoreField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] CopyOnStartupField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<string, string> EnvVarsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.Dictionary<string, string> ParamsField;
        
        private string NameField;
        
        private string VersionField;
        
        private string AppPathField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LocalDirField;
        
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
        public string[] Cleanup {
            get {
                return this.CleanupField;
            }
            set {
                if ((object.ReferenceEquals(this.CleanupField, value) != true)) {
                    this.CleanupField = value;
                    this.RaisePropertyChanged("Cleanup");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] CleanupIgnore {
            get {
                return this.CleanupIgnoreField;
            }
            set {
                if ((object.ReferenceEquals(this.CleanupIgnoreField, value) != true)) {
                    this.CleanupIgnoreField = value;
                    this.RaisePropertyChanged("CleanupIgnore");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] CopyOnStartup {
            get {
                return this.CopyOnStartupField;
            }
            set {
                if ((object.ReferenceEquals(this.CopyOnStartupField, value) != true)) {
                    this.CopyOnStartupField = value;
                    this.RaisePropertyChanged("CopyOnStartup");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, string> EnvVars {
            get {
                return this.EnvVarsField;
            }
            set {
                if ((object.ReferenceEquals(this.EnvVarsField, value) != true)) {
                    this.EnvVarsField = value;
                    this.RaisePropertyChanged("EnvVars");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.Dictionary<string, string> Params {
            get {
                return this.ParamsField;
            }
            set {
                if ((object.ReferenceEquals(this.ParamsField, value) != true)) {
                    this.ParamsField = value;
                    this.RaisePropertyChanged("Params");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=5)]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=6)]
        public string Version {
            get {
                return this.VersionField;
            }
            set {
                if ((object.ReferenceEquals(this.VersionField, value) != true)) {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=7)]
        public string AppPath {
            get {
                return this.AppPathField;
            }
            set {
                if ((object.ReferenceEquals(this.AppPathField, value) != true)) {
                    this.AppPathField = value;
                    this.RaisePropertyChanged("AppPath");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=8)]
        public string LocalDir {
            get {
                return this.LocalDirField;
            }
            set {
                if ((object.ReferenceEquals(this.LocalDirField, value) != true)) {
                    this.LocalDirField = value;
                    this.RaisePropertyChanged("LocalDir");
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ResourceBaseService.IResourceBaseService")]
    public interface IResourceBaseService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IResourceBaseService/GetAllResources", ReplyAction="http://tempuri.org/IResourceBaseService/GetAllResourcesResponse")]
        ServiceProxies.ResourceBaseService.Resource[] GetAllResources();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IResourceBaseService/GetResourceNames", ReplyAction="http://tempuri.org/IResourceBaseService/GetResourceNamesResponse")]
        string[] GetResourceNames();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IResourceBaseService/GetResourceByName", ReplyAction="http://tempuri.org/IResourceBaseService/GetResourceByNameResponse")]
        ServiceProxies.ResourceBaseService.Resource GetResourceByName(string resourceName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IResourceBaseService/GetNodeNames", ReplyAction="http://tempuri.org/IResourceBaseService/GetNodeNamesResponse")]
        string[] GetNodeNames(string resourceName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IResourceBaseService/GetResourceNodeByName", ReplyAction="http://tempuri.org/IResourceBaseService/GetResourceNodeByNameResponse")]
        ServiceProxies.ResourceBaseService.ResourceNode GetResourceNodeByName(string resourceName, string nodeName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IResourceBaseService/GetResourcesForFarm", ReplyAction="http://tempuri.org/IResourceBaseService/GetResourcesForFarmResponse")]
        ServiceProxies.ResourceBaseService.Resource[] GetResourcesForFarm(string farmId, string dumpingKey);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IResourceBaseService/AddInstalledPackage", ReplyAction="http://tempuri.org/IResourceBaseService/AddInstalledPackageResponse")]
        void AddInstalledPackage(string resourceName, string nodeName, ServiceProxies.ResourceBaseService.PackageOnNode pack);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IResourceBaseService/RemoveInstalledPackage", ReplyAction="http://tempuri.org/IResourceBaseService/RemoveInstalledPackageResponse")]
        void RemoveInstalledPackage(string resourceName, string nodeName, string packName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IResourceBaseServiceChannel : ServiceProxies.ResourceBaseService.IResourceBaseService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ResourceBaseServiceClient : System.ServiceModel.ClientBase<ServiceProxies.ResourceBaseService.IResourceBaseService>, ServiceProxies.ResourceBaseService.IResourceBaseService {
        
        public ResourceBaseServiceClient() {
        }
        
        public ResourceBaseServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ResourceBaseServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ResourceBaseServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ResourceBaseServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public ServiceProxies.ResourceBaseService.Resource[] GetAllResources() {
            return base.Channel.GetAllResources();
        }
        
        public string[] GetResourceNames() {
            return base.Channel.GetResourceNames();
        }
        
        public ServiceProxies.ResourceBaseService.Resource GetResourceByName(string resourceName) {
            return base.Channel.GetResourceByName(resourceName);
        }
        
        public string[] GetNodeNames(string resourceName) {
            return base.Channel.GetNodeNames(resourceName);
        }
        
        public ServiceProxies.ResourceBaseService.ResourceNode GetResourceNodeByName(string resourceName, string nodeName) {
            return base.Channel.GetResourceNodeByName(resourceName, nodeName);
        }
        
        public ServiceProxies.ResourceBaseService.Resource[] GetResourcesForFarm(string farmId, string dumpingKey) {
            return base.Channel.GetResourcesForFarm(farmId, dumpingKey);
        }
        
        public void AddInstalledPackage(string resourceName, string nodeName, ServiceProxies.ResourceBaseService.PackageOnNode pack) {
            base.Channel.AddInstalledPackage(resourceName, nodeName, pack);
        }
        
        public void RemoveInstalledPackage(string resourceName, string nodeName, string packName) {
            base.Channel.RemoveInstalledPackage(resourceName, nodeName, packName);
        }
    }
}
