using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopNowBL.Models
{
    public class EmployeeModel
    {
        public int Id
        {
            get;
            set;
        }
        [Required]
        public string EmpName
        {
            get;
            set;
        }
        [Required]
        public string Address
        {
            get;
            set;
        }
        [Required]
        public decimal Salary
        {
            get;
            set;
        }
        public bool IsEdit
        {
            get;
            set;
        }
    }
}
