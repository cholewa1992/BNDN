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

    public partial class UserAcc : IEntityDto
    {
        public UserAcc()
        {
            this.AccessRight = new HashSet<AccessRight>();
            this.ClientAdmin = new HashSet<ClientAdmin>();
            this.UserInfo = new HashSet<UserInfo>();
            this.Rating = new HashSet<Rating>();
        }
    
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    
        public virtual ICollection<AccessRight> AccessRight { get; set; }
        public virtual ICollection<ClientAdmin> ClientAdmin { get; set; }
        public virtual ICollection<UserInfo> UserInfo { get; set; }
        public virtual ICollection<Rating> Rating { get; set; }
    }
}