//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HeroLauncher
{
    using System;
    using System.Collections.Generic;
    
    public partial class ServerList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Mode { get; set; }
        public string IpV4 { get; set; }
        public string IpText { get; set; }
        public Nullable<int> Port { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
    }
}
