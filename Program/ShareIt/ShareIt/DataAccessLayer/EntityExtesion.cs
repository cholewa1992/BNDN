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

    public partial class EntityExtesion : IEntityDto
    {
        public EntityExtesion()
        {
            this.Entity = new HashSet<Entity>();
        }
    
        public int Id { get; set; }
        public string Extension { get; set; }
    
        public virtual ICollection<Entity> Entity { get; set; }
    }
}
