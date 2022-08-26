namespace ShopNowBL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblStore")]
    public partial class tblStore
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string StoreName { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string ContactNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
