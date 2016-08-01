using System;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using OpenBiz.Data.Configuration;
using System.Data.Entity;
using BLL.Entities.Inventory;
using BLL.Entities.Procurement;
using BLL.Entities.UserAccounts;
using BLL.Entities;
using DAL.Configuration;
using BLL.Entities.Warehouse;
using BLL.Entities.Production;
using BLL.Entities.Transportation;
using BLL.Entities.DistOrders;
using BLL.Entities.Distribution;
using System.Threading;

namespace DAL
{
    public class SCMSContext : IdentityDbContext<ApplicationUser>
    {
        public SCMSContext() : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RawMaterial>()
                .HasRequired(c => c.UnitOfMeasurement)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasRequired(s => s.UnitOfMeasurement)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RequestForQuotation>()
                .HasRequired(s => s.PaymentTerm)
                .WithMany()
                .WillCascadeOnDelete(false);


            modelBuilder.Configurations.Add(new ProductPartsConfiguration());

            modelBuilder.Configurations.Add(new ProductMaterialsConfiguration());

            modelBuilder.Configurations.Add(new WarehouseProductsConfiguration());

            modelBuilder.Configurations.Add(new WarehouseMaterialsConfiguration());

            modelBuilder.Configurations.Add(new RQItemConfiguration());

            modelBuilder.Configurations.Add(new RQSupplierConfiguration());

            modelBuilder.Configurations.Add(new MaterialTaxesConfiguration());

            modelBuilder.Configurations.Add(new ProductTaxesConfiguration());
        }

        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity
                    && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                IAuditableEntity entity = entry.Entity as IAuditableEntity;
                if (entity != null)
                {
                    string identityName = Thread.CurrentPrincipal.Identity.Name;
                    DateTime now = DateTime.UtcNow;

                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedBy = identityName;
                        entity.CreatedDate = now;
                    }
                    else
                    {
                        base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                        base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                    }

                    entity.UpdatedBy = identityName;
                    entity.UpdatedDate = now;
                }
            }

            return base.SaveChanges();
        }


        public IDbSet<BLL.Entities.Inventory.Attribute> Attributes { get; set; }
        public IDbSet<AttributeValue> AttributeValues { get; set; }
        public IDbSet<Product> Products { get; set; }
        public IDbSet<ProductCategory> ProductCategories { get; set; }
        public IDbSet<Part> Parts { get; set; }
        public IDbSet<ProductParts> ProductParts { get; set; }
        public IDbSet<RawMaterial> RawMaterials { get; set; }
        public IDbSet<ProductMaterials> ProductMaterials { get; set; }
        public IDbSet<UnitOfMeasurement> UnitsOfMeasurement { get; set; }
        public IDbSet<Brand> Brands { get; set; }
        public IDbSet<Warehouse> Warehouses { get; set; }
        public IDbSet<WarehouseAdmin> WarehouseAdmins { get; set; }
        public IDbSet<WarehouseProducts> WarehouseProducts { get; set; }
        public IDbSet<WarehouseMaterials> WarehouseMaterials { get; set; }
        public IDbSet<Supplier> Suppliers { get; set; }
        public IDbSet<Quote> Quotes { get; set; }
        public IDbSet<RequestForQuotation> RFQs { get; set; }
        public IDbSet<RQItem> RQItems { get; set; }
        public IDbSet<RQSupplier> RQSuppliers { get; set; }
        public IDbSet<AddressType> AddressTypes { get; set; }
        public IDbSet<Contract> Contracts { get; set; }
        public IDbSet<PaymentTerm> PaymentTerms { get; set; }
        public IDbSet<SupplierAddress> SupplierAddresses { get; set; }
        public IDbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public IDbSet<Tax> Taxes { get; set; }
        public IDbSet<ProductTaxes> ProductTaxes { get; set; }
        public IDbSet<MaterialTaxes> MaterialTaxes { get; set; }
        public IDbSet<Plant> Plants { get; set; }
        public IDbSet<ShippingSlip> ShippingSlips { get; set; }
        public IDbSet<Order> Orders { get; set; }
        public IDbSet<ProductionPlan> ProductionPlans { get; set; }
        public IDbSet<Distributor> Distributors { get; set; }
        public System.Data.Entity.DbSet<BLL.Entities.Payment.AccountEntry> AccountEntries { get; set; }
        public System.Data.Entity.DbSet<BLL.Entities.Payment.Account> Accounts { get; set; }
    }


}
