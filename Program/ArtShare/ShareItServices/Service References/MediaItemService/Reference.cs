﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShareItServices.MediaItemService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MediaItemDTO", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.DTO")]
    [System.SerializableAttribute()]
    public partial class MediaItemDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FileExtensionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ShareItServices.MediaItemService.MediaItemInformationDTO[] InformationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ShareItServices.MediaItemService.UserDTO OwnerField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ShareItServices.MediaItemService.MediaItemTypeDTO TypeField;
        
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
        public string FileExtension {
            get {
                return this.FileExtensionField;
            }
            set {
                if ((object.ReferenceEquals(this.FileExtensionField, value) != true)) {
                    this.FileExtensionField = value;
                    this.RaisePropertyChanged("FileExtension");
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
        public ShareItServices.MediaItemService.MediaItemInformationDTO[] Information {
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
        public ShareItServices.MediaItemService.UserDTO Owner {
            get {
                return this.OwnerField;
            }
            set {
                if ((object.ReferenceEquals(this.OwnerField, value) != true)) {
                    this.OwnerField = value;
                    this.RaisePropertyChanged("Owner");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ShareItServices.MediaItemService.MediaItemTypeDTO Type {
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
        private ShareItServices.MediaItemService.UserInformationDTO[] InformationField;
        
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
        public ShareItServices.MediaItemService.UserInformationDTO[] Information {
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
    [System.Runtime.Serialization.DataContractAttribute(Name="MediaItemInformationDTO", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.DTO")]
    [System.SerializableAttribute()]
    public partial class MediaItemInformationDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ShareItServices.MediaItemService.InformationTypeDTO TypeField;
        
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
        public ShareItServices.MediaItemService.InformationTypeDTO Type {
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
    [System.Runtime.Serialization.DataContractAttribute(Name="MediaItemTypeDTO", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.DTO")]
    public enum MediaItemTypeDTO : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Movie = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Book = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Music = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="InformationTypeDTO", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.DTO")]
    public enum InformationTypeDTO : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Title = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Description = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Price = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Picture = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        KeywordTag = 5,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Genre = 6,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        TrackLength = 7,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Runtime = 8,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        NumberOfPages = 9,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Author = 10,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Director = 11,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Artist = 12,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        CastMember = 13,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ReleaseDate = 14,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Language = 15,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ExpirationDate = 16,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        AverageRating = 17,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Thumbnail = 18,
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
        private ShareItServices.MediaItemService.UserInformationTypeDTO TypeField;
        
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
        public ShareItServices.MediaItemService.UserInformationTypeDTO Type {
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
        Firstname = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Lastname = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Email = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Location = 4,
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
    [System.Runtime.Serialization.DataContractAttribute(Name="MediaItemSearchResultDTO", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.DTO")]
    [System.SerializableAttribute()]
    public partial class MediaItemSearchResultDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ShareItServices.MediaItemService.MediaItemDTO[] MediaItemListField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int NumberOfSearchResultsField;
        
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
        public ShareItServices.MediaItemService.MediaItemDTO[] MediaItemList {
            get {
                return this.MediaItemListField;
            }
            set {
                if ((object.ReferenceEquals(this.MediaItemListField, value) != true)) {
                    this.MediaItemListField = value;
                    this.RaisePropertyChanged("MediaItemList");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumberOfSearchResults {
            get {
                return this.NumberOfSearchResultsField;
            }
            set {
                if ((this.NumberOfSearchResultsField.Equals(value) != true)) {
                    this.NumberOfSearchResultsField = value;
                    this.RaisePropertyChanged("NumberOfSearchResults");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="MediaItemService.IMediaItemService")]
    public interface IMediaItemService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/GetMediaItemInformation", ReplyAction="http://tempuri.org/IMediaItemService/GetMediaItemInformationResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="http://tempuri.org/IMediaItemService/GetMediaItemInformationFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        [System.ServiceModel.FaultContractAttribute(typeof(ShareItServices.MediaItemService.ArgumentFault), Action="http://tempuri.org/IMediaItemService/GetMediaItemInformationArgumentFaultFault", Name="ArgumentFault", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
        ShareItServices.MediaItemService.MediaItemDTO GetMediaItemInformation(int mediaItemId, System.Nullable<int> userId, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/GetMediaItemInformation", ReplyAction="http://tempuri.org/IMediaItemService/GetMediaItemInformationResponse")]
        System.Threading.Tasks.Task<ShareItServices.MediaItemService.MediaItemDTO> GetMediaItemInformationAsync(int mediaItemId, System.Nullable<int> userId, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/GetMediaItems", ReplyAction="http://tempuri.org/IMediaItemService/GetMediaItemsResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ShareItServices.MediaItemService.ArgumentFault), Action="http://tempuri.org/IMediaItemService/GetMediaItemsArgumentFaultFault", Name="ArgumentFault", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="http://tempuri.org/IMediaItemService/GetMediaItemsFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO> GetMediaItems(int from, int to, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/GetMediaItems", ReplyAction="http://tempuri.org/IMediaItemService/GetMediaItemsResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO>> GetMediaItemsAsync(int from, int to, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/GetMediaItemsByType", ReplyAction="http://tempuri.org/IMediaItemService/GetMediaItemsByTypeResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="http://tempuri.org/IMediaItemService/GetMediaItemsByTypeFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        [System.ServiceModel.FaultContractAttribute(typeof(ShareItServices.MediaItemService.ArgumentFault), Action="http://tempuri.org/IMediaItemService/GetMediaItemsByTypeArgumentFaultFault", Name="ArgumentFault", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
        System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO> GetMediaItemsByType(int from, int to, ShareItServices.MediaItemService.MediaItemTypeDTO mediaType, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/GetMediaItemsByType", ReplyAction="http://tempuri.org/IMediaItemService/GetMediaItemsByTypeResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO>> GetMediaItemsByTypeAsync(int from, int to, ShareItServices.MediaItemService.MediaItemTypeDTO mediaType, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/SearchMediaItems", ReplyAction="http://tempuri.org/IMediaItemService/SearchMediaItemsResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ShareItServices.MediaItemService.ArgumentFault), Action="http://tempuri.org/IMediaItemService/SearchMediaItemsArgumentFaultFault", Name="ArgumentFault", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="http://tempuri.org/IMediaItemService/SearchMediaItemsFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO> SearchMediaItems(int from, int to, string searchKey, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/SearchMediaItems", ReplyAction="http://tempuri.org/IMediaItemService/SearchMediaItemsResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO>> SearchMediaItemsAsync(int from, int to, string searchKey, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/SearchMediaItemsByType", ReplyAction="http://tempuri.org/IMediaItemService/SearchMediaItemsByTypeResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ShareItServices.MediaItemService.ArgumentFault), Action="http://tempuri.org/IMediaItemService/SearchMediaItemsByTypeArgumentFaultFault", Name="ArgumentFault", Namespace="http://schemas.datacontract.org/2004/07/BusinessLogicLayer.FaultDataContracts")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="http://tempuri.org/IMediaItemService/SearchMediaItemsByTypeFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO> SearchMediaItemsByType(int from, int to, System.Nullable<ShareItServices.MediaItemService.MediaItemTypeDTO> mediaType, string searchKey, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/SearchMediaItemsByType", ReplyAction="http://tempuri.org/IMediaItemService/SearchMediaItemsByTypeResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO>> SearchMediaItemsByTypeAsync(int from, int to, System.Nullable<ShareItServices.MediaItemService.MediaItemTypeDTO> mediaType, string searchKey, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/RateMediaItem", ReplyAction="http://tempuri.org/IMediaItemService/RateMediaItemResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(System.ServiceModel.FaultException), Action="http://tempuri.org/IMediaItemService/RateMediaItemFaultExceptionFault", Name="FaultException", Namespace="http://schemas.datacontract.org/2004/07/System.ServiceModel")]
        void RateMediaItem(int userId, int mediaItemId, int rating, string clientToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMediaItemService/RateMediaItem", ReplyAction="http://tempuri.org/IMediaItemService/RateMediaItemResponse")]
        System.Threading.Tasks.Task RateMediaItemAsync(int userId, int mediaItemId, int rating, string clientToken);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMediaItemServiceChannel : ShareItServices.MediaItemService.IMediaItemService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MediaItemServiceClient : System.ServiceModel.ClientBase<ShareItServices.MediaItemService.IMediaItemService>, ShareItServices.MediaItemService.IMediaItemService {
        
        public MediaItemServiceClient() {
        }
        
        public MediaItemServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MediaItemServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MediaItemServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MediaItemServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public ShareItServices.MediaItemService.MediaItemDTO GetMediaItemInformation(int mediaItemId, System.Nullable<int> userId, string clientToken) {
            return base.Channel.GetMediaItemInformation(mediaItemId, userId, clientToken);
        }
        
        public System.Threading.Tasks.Task<ShareItServices.MediaItemService.MediaItemDTO> GetMediaItemInformationAsync(int mediaItemId, System.Nullable<int> userId, string clientToken) {
            return base.Channel.GetMediaItemInformationAsync(mediaItemId, userId, clientToken);
        }
        
        public System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO> GetMediaItems(int from, int to, string clientToken) {
            return base.Channel.GetMediaItems(from, to, clientToken);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO>> GetMediaItemsAsync(int from, int to, string clientToken) {
            return base.Channel.GetMediaItemsAsync(from, to, clientToken);
        }
        
        public System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO> GetMediaItemsByType(int from, int to, ShareItServices.MediaItemService.MediaItemTypeDTO mediaType, string clientToken) {
            return base.Channel.GetMediaItemsByType(from, to, mediaType, clientToken);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO>> GetMediaItemsByTypeAsync(int from, int to, ShareItServices.MediaItemService.MediaItemTypeDTO mediaType, string clientToken) {
            return base.Channel.GetMediaItemsByTypeAsync(from, to, mediaType, clientToken);
        }
        
        public System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO> SearchMediaItems(int from, int to, string searchKey, string clientToken) {
            return base.Channel.SearchMediaItems(from, to, searchKey, clientToken);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO>> SearchMediaItemsAsync(int from, int to, string searchKey, string clientToken) {
            return base.Channel.SearchMediaItemsAsync(from, to, searchKey, clientToken);
        }
        
        public System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO> SearchMediaItemsByType(int from, int to, System.Nullable<ShareItServices.MediaItemService.MediaItemTypeDTO> mediaType, string searchKey, string clientToken) {
            return base.Channel.SearchMediaItemsByType(from, to, mediaType, searchKey, clientToken);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<ShareItServices.MediaItemService.MediaItemTypeDTO, ShareItServices.MediaItemService.MediaItemSearchResultDTO>> SearchMediaItemsByTypeAsync(int from, int to, System.Nullable<ShareItServices.MediaItemService.MediaItemTypeDTO> mediaType, string searchKey, string clientToken) {
            return base.Channel.SearchMediaItemsByTypeAsync(from, to, mediaType, searchKey, clientToken);
        }
        
        public void RateMediaItem(int userId, int mediaItemId, int rating, string clientToken) {
            base.Channel.RateMediaItem(userId, mediaItemId, rating, clientToken);
        }
        
        public System.Threading.Tasks.Task RateMediaItemAsync(int userId, int mediaItemId, int rating, string clientToken) {
            return base.Channel.RateMediaItemAsync(userId, mediaItemId, rating, clientToken);
        }
    }
}
