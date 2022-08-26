using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ShopNowBL.Models
{
    public partial class DBTContext : DbContext
    {
        public DBTContext()
            : base("name=DBTContext") 
        {
        }

        public virtual DbSet<tblCustomer> tblCustomers { get; set; }
        public virtual DbSet<tblOTP> tblOTPs { get; set; }
        public virtual DbSet<tblStock> tblStocks { get; set; }
        public virtual DbSet<tblStore> tblStores { get; set; }
        public virtual DbSet<tblTransaction> tblTransactions { get; set; }
        public virtual DbSet<tblTransactionItem> tblTransactionItems { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblErrorLog> tblErrorLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblCustomer>()
                .HasMany(e => e.tblTransactions)
                .WithRequired(e => e.tblCustomer)
                .HasForeignKey(e => e.CustomerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblStock>()
                .HasMany(e => e.tblTransactionItems)
                .WithRequired(e => e.tblStock)
                .HasForeignKey(e => e.ProductId)
                .WillCascadeOnDelete(false);
        }
    }
}
