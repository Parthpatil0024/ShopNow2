namespace ShopNowBL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblTransactionItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceId { get; set; }

        public int ProductId { get; set; }

        public decimal Qty { get; set; }

        public decimal? Price { get; set; }

        public virtual tblStock tblStock { get; set; }
    }
}
