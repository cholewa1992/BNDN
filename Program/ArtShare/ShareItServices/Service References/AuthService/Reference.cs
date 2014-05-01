﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShareItServices.AuthService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserDTO", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.DTO")]
    [System.SerializableAttribute()]
    public partial class UserDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ShareItServices.AuthService.UserInformationDTO[] InformationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PasswordField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UsernameField;
        
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
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ShareItServices.AuthService.UserInformationDTO[] Information {
            get {
                return this.InformationField;
            }
            set {
                if ((object.ReferenceEquals(this.InformationField, value) != true)) {
                    this.InformationField = value;
                    this.RaisePropertyChanged("Information");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
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
        
        [System.Runtime.Serialization.DataMemberAttribute()]
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
    [System.Runtime.Serialization.DataContractAttribute(Name="UserInformationDTO", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.DTO")]
    [System.SerializableAttribute()]
    public partial class UserInformationDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ShareItServices.AuthService.UserInformationTypeDTO TypeField;
        
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
        public string Data {
            get {
                return this.DataField;
            }
            set {
                if ((object.ReferenceEquals(this.DataField, value) != true)) {
                    this.DataField = value;
                    this.RaisePropertyChanged("Data");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ShareItServices.AuthService.UserInformationTypeDTO Type {
            get {
                return this.TypeField;
            }
            set {
                if ((this.TypeField.Equals(value) != true)) {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="UserInformationTypeDTO", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.DTO")]
    public enum UserInformationTypeDTO : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Email = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Firstname = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Lastname = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Location = 4,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UnauthorizedUser", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
    [System.SerializableAttribute()]
    public partial class UnauthorizedUser : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
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
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="ArgumentFault", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
    [System.SerializableAttribute()]
    public partial class ArgumentFault : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
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
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="ClientDTO", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.DTO")]
    [System.SerializableAttribute()]
    public partial class ClientDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TokenField;
        
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
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Token {
            get {
                return this.TokenField;
            }
            set {
                if ((object.ReferenceEquals(this.TokenField, value) != true)) {
                    this.TokenField = value;
                    this.RaisePropertyChanged("Token");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AuthService.IAuthService")]
    public interface IAuthService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthService/ValidateUser", ReplyAction="http://tempuri.org/IAuthService/ValidateUserResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="http://tempuri.org/IAuthService/ValidateUserFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        [System.ServiceModel.FaultContractAttribute(typeof(ShareItServices.AuthService.UnauthorizedUser), Action="http://tempuri.org/IAuthService/ValidateUserUnauthorizedUserFault", Name="UnauthorizedUser", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(ShareItServices.AuthService.ArgumentFault), Action="http://tempuri.org/IAuthService/ValidateUserArgumentFaultFault", Name="ArgumentFault", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
        int ValidateUser(ShareItServices.AuthService.UserDTO user, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthService/ValidateUser", ReplyAction="http://tempuri.org/IAuthService/ValidateUserResponse")]
        System.Threading.Tasks.Task<int> ValidateUserAsync(ShareItServices.AuthService.UserDTO user, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthService/CheckClientExists", ReplyAction="http://tempuri.org/IAuthService/CheckClientExistsResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ShareItServices.AuthService.ArgumentFault), Action="http://tempuri.org/IAuthService/CheckClientExistsArgumentFaultFault", Name="ArgumentFault", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="http://tempuri.org/IAuthService/CheckClientExistsFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        bool CheckClientExists(ShareItServices.AuthService.ClientDTO client);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthService/CheckClientExists", ReplyAction="http://tempuri.org/IAuthService/CheckClientExistsResponse")]
        System.Threading.Tasks.Task<bool> CheckClientExistsAsync(ShareItServices.AuthService.ClientDTO client);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthService/IsUserAdminOnClient", ReplyAction="http://tempuri.org/IAuthService/IsUserAdminOnClientResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ShareItServices.AuthService.ArgumentFault), Action="http://tempuri.org/IAuthService/IsUserAdminOnClientArgumentFaultFault", Name="ArgumentFault", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="http://tempuri.org/IAuthService/IsUserAdminOnClientFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        bool IsUserAdminOnClient(ShareItServices.AuthService.UserDTO user, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthService/IsUserAdminOnClient", ReplyAction="http://tempuri.org/IAuthService/IsUserAdminOnClientResponse")]
        System.Threading.Tasks.Task<bool> IsUserAdminOnClientAsync(ShareItServices.AuthService.UserDTO user, string clientToken);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAuthServiceChannel : ShareItServices.AuthService.IAuthService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AuthServiceClient : System.ServiceModel.ClientBase<ShareItServices.AuthService.IAuthService>, ShareItServices.AuthService.IAuthService {
        
        public AuthServiceClient() {
        }
        
        public AuthServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AuthServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int ValidateUser(ShareItServices.AuthService.UserDTO user, string clientToken) {
            return base.Channel.ValidateUser(user, clientToken);
        }
        
        public System.Threading.Tasks.Task<int> ValidateUserAsync(ShareItServices.AuthService.UserDTO user, string clientToken) {
            return base.Channel.ValidateUserAsync(user, clientToken);
        }
        
        public bool CheckClientExists(ShareItServices.AuthService.ClientDTO client) {
            return base.Channel.CheckClientExists(client);
        }
        
        public System.Threading.Tasks.Task<bool> CheckClientExistsAsync(ShareItServices.AuthService.ClientDTO client) {
            return base.Channel.CheckClientExistsAsync(client);
        }
        
        public bool IsUserAdminOnClient(ShareItServices.AuthService.UserDTO user, string clientToken) {
            return base.Channel.IsUserAdminOnClient(user, clientToken);
        }
        
        public System.Threading.Tasks.Task<bool> IsUserAdminOnClientAsync(ShareItServices.AuthService.UserDTO user, string clientToken) {
            return base.Channel.IsUserAdminOnClientAsync(user, clientToken);
        }
    }
}
