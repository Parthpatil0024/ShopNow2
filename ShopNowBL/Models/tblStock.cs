namespace ShopNowBL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblStock")]
    public partial class tblStock
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblStock()
        {
            tblTransactionItems = new HashSet<tblTransactionItem>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        public int ProductQty { get; set; }

        public decimal BasePrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal? Discount { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblTransactionItem> tblTransactionItems { get; set; }
    }
}
