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
    
    public partial class TBL_PARTY_PRODUCT
    {
        public int PartyProductID { get; set; }
        public int PartyID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public decimal UnitCost { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
