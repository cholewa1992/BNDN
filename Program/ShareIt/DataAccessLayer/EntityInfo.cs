//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccessLayer
{
    using System;
    using System.Collections.Generic;

    public partial class EntityInfo : IEntityDto
    {
        public string Data { get; set; }
        public int EntityId { get; set; }
        public int EntityInfoTypeId { get; set; }
        public int Id { get; set; }
    
        public virtual Entity Entity { get; set; }
        public virtual EntityInfoType EntityInfoType { get; set; }
    }
}
