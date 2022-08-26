namespace ShopNowBL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblErrorLog")]
    public partial class tblErrorLog
    {
        public int Id { get; set; }

        [Required]
        public string ExcepUrl { get; set; }

        [Required]
        public string ExcepMsg { get; set; }

        [Required]
        public string ExcepType { get; set; }

        [Required]
        public string ExcepSource { get; set; }

        public DateTime LogDate { get; set; }
    }
}
