namespace ShopNowBL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string UserName { get; set; }

        [StringLength(150)]
        public string EmailId { get; set; }

        [StringLength(50)]
        public string MobileNo { get; set; }

        public int RoleId { get; set; }

        [Required]
        [StringLength(150)]
        public string Password { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        public int StoreId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
