//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QUANLYTIEC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBL_MATERIAL
    {
        public int MaterialID { get; set; }
        public string MaterialName { get; set; }
        public string Notes { get; set; }
        public Nullable<int> UOMID { get; set; }
        public int UnitPrie { get; set; }
        public Nullable<int> MaterialGroupID { get; set; }
    }
}
