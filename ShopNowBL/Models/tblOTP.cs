namespace ShopNowBL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblOTP")]
    public partial class tblOTP
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EmailId { get; set; }

        [Required]
        [StringLength(50)]
        public string OTP { get; set; }

        public DateTime Created_DateTime { get; set; }

        public int IsUsed { get; set; }
    }
}
