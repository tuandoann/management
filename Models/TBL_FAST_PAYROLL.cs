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
    
    public partial class TBL_FAST_PAYROLL
    {
        public int FastPayRollID { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime CheckIn { get; set; }
        public System.DateTime CheckOut { get; set; }
        public int Amount { get; set; }
    }
}
