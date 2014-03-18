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

    public partial class Entity : IEntityDto
    {
        public Entity()
        {
            this.AcessRight = new HashSet<AcessRight>();
            this.EntityInfo = new HashSet<EntityInfo>();
        }
    
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int ClientId { get; set; }
        public Nullable<int> TypeId { get; set; }
    
        public virtual ICollection<AcessRight> AcessRight { get; set; }
        public virtual Client Client { get; set; }
        public virtual EntityType EntityType { get; set; }
        public virtual ICollection<EntityInfo> EntityInfo { get; set; }
    }
}