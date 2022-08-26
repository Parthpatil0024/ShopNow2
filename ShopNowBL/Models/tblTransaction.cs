namespace ShopNowBL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblTransaction")]
    public partial class tblTransaction
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int TotalQty { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNo { get; set; }

        public decimal InvoiceAmount { get; set; }

        public decimal? GST { get; set; }

        public decimal? TotalDiscount { get; set; }

        [Column(TypeName = "date")]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual tblCustomer tblCustomer { get; set; }
    }
}
