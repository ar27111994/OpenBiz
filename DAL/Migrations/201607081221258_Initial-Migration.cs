namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddressTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AddressTypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Attributes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                        ProductID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProductName = c.String(maxLength: 50),
                        SKU = c.String(maxLength: 50),
                        ProductBasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductDescription = c.String(maxLength: 500),
                        CategoryID = c.Long(nullable: false),
                        Barcode = c.String(),
                        UnitOfMeasurementID = c.Long(nullable: false),
                        BrandID = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brands", t => t.BrandID, cascadeDelete: true)
                .ForeignKey("dbo.ProductCategories", t => t.CategoryID, cascadeDelete: true)
                .ForeignKey("dbo.UnitOfMeasurements", t => t.UnitOfMeasurementID)
                .Index(t => t.SKU, unique: true, name: "AlteranteKey_SKU")
                .Index(t => t.CategoryID)
                .Index(t => t.UnitOfMeasurementID)
                .Index(t => t.BrandID);
            
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BrandName = c.String(nullable: false, maxLength: 40),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 29),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductParts",
                c => new
                    {
                        ProductID = c.Long(nullable: false),
                        PartID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductID, t.PartID })
                .ForeignKey("dbo.Parts", t => t.PartID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID)
                .Index(t => t.PartID);
            
            CreateTable(
                "dbo.Parts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PartName = c.String(nullable: false, maxLength: 30),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductMaterials",
                c => new
                    {
                        ProductID = c.Long(nullable: false),
                        MaterialID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductID, t.MaterialID })
                .ForeignKey("dbo.RawMaterials", t => t.MaterialID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID)
                .Index(t => t.MaterialID);
            
            CreateTable(
                "dbo.RawMaterials",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MaterialName = c.String(nullable: false, maxLength: 40),
                        Manufacturer = c.String(nullable: false, maxLength: 40),
                        SupplierID = c.Long(nullable: false),
                        MaterialPrice = c.Long(nullable: false),
                        Barcode = c.String(),
                        UnitOfMeasurementID = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .ForeignKey("dbo.UnitOfMeasurements", t => t.UnitOfMeasurementID)
                .Index(t => t.SupplierID)
                .Index(t => t.UnitOfMeasurementID);
            
            CreateTable(
                "dbo.RQItems",
                c => new
                    {
                        ItemID = c.Long(nullable: false),
                        RFQID = c.Long(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ItemID, t.RFQID })
                .ForeignKey("dbo.RawMaterials", t => t.ItemID, cascadeDelete: true)
                .ForeignKey("dbo.RequestForQuotations", t => t.RFQID, cascadeDelete: true)
                .Index(t => t.ItemID)
                .Index(t => t.RFQID);
            
            CreateTable(
                "dbo.RequestForQuotations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ScheduledDate = c.DateTime(nullable: false),
                        PaymentTermID = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                        PaymentTerm_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PaymentTerms", t => t.PaymentTerm_Id)
                .Index(t => t.PaymentTerm_Id);
            
            CreateTable(
                "dbo.PaymentTerms",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Days = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RQSuppliers",
                c => new
                    {
                        SupplierID = c.Long(nullable: false),
                        RFQID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SupplierID, t.RFQID })
                .ForeignKey("dbo.RequestForQuotations", t => t.RFQID, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .Index(t => t.SupplierID)
                .Index(t => t.RFQID);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SupplierName = c.String(),
                        SupplierDetails = c.String(),
                        SupplierAddress = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SupplierAddresses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Address = c.String(nullable: false, maxLength: 170),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        AddressTypeID = c.Long(nullable: false),
                        PhoneNo = c.String(),
                        Email = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                        Supplier_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AddressTypes", t => t.AddressTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_Id)
                .Index(t => t.AddressTypeID)
                .Index(t => t.Supplier_Id);
            
            CreateTable(
                "dbo.Quotes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        QuoteDescription = c.String(),
                        QuotationDate = c.DateTime(nullable: false),
                        MaterialID = c.Long(nullable: false),
                        RFQID = c.Long(nullable: false),
                        PaymentTermID = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                        RawMaterial_Id = c.Long(),
                        Supplier_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PaymentTerms", t => t.PaymentTermID, cascadeDelete: true)
                .ForeignKey("dbo.RawMaterials", t => t.RawMaterial_Id)
                .ForeignKey("dbo.RequestForQuotations", t => t.RFQID, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_Id)
                .Index(t => t.RFQID)
                .Index(t => t.PaymentTermID)
                .Index(t => t.RawMaterial_Id)
                .Index(t => t.Supplier_Id);
            
            CreateTable(
                "dbo.MaterialTaxes",
                c => new
                    {
                        MaterialID = c.Long(nullable: false),
                        TaxID = c.Long(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => new { t.MaterialID, t.TaxID })
                .ForeignKey("dbo.RawMaterials", t => t.MaterialID, cascadeDelete: true)
                .ForeignKey("dbo.Taxes", t => t.TaxID, cascadeDelete: true)
                .Index(t => t.MaterialID)
                .Index(t => t.TaxID);
            
            CreateTable(
                "dbo.Taxes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 40),
                        Percentage = c.Double(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductTaxes",
                c => new
                    {
                        ProductID = c.Long(nullable: false),
                        TaxID = c.Long(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => new { t.ProductID, t.TaxID })
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .ForeignKey("dbo.Taxes", t => t.TaxID, cascadeDelete: true)
                .Index(t => t.ProductID)
                .Index(t => t.TaxID);
            
            CreateTable(
                "dbo.UnitOfMeasurements",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Unit = c.String(nullable: false, maxLength: 30),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WarehouseMaterials",
                c => new
                    {
                        RawMaterialID = c.Long(nullable: false),
                        WarehouseID = c.Long(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        EntryTitle = c.String(maxLength: 50),
                        Purpose = c.String(maxLength: 90),
                        Quantity = c.Int(nullable: false),
                        PostingTime = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => new { t.RawMaterialID, t.WarehouseID })
                .ForeignKey("dbo.RawMaterials", t => t.RawMaterialID, cascadeDelete: true)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseID, cascadeDelete: true)
                .Index(t => t.RawMaterialID)
                .Index(t => t.WarehouseID);
            
            CreateTable(
                "dbo.Warehouses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        WarehouseName = c.String(maxLength: 50),
                        WarehouseLocation = c.String(maxLength: 150),
                        Longitude = c.Double(nullable: false),
                        Latitude = c.Double(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WarehouseProducts",
                c => new
                    {
                        ProductID = c.Long(nullable: false),
                        WarehouseID = c.Long(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
                        EntryTitle = c.String(maxLength: 50),
                        Purpose = c.String(maxLength: 90),
                        Quantity = c.Int(nullable: false),
                        PostingTime = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => new { t.ProductID, t.WarehouseID })
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseID, cascadeDelete: true)
                .Index(t => t.ProductID)
                .Index(t => t.WarehouseID);
            
            CreateTable(
                "dbo.AttributeValues",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Value = c.String(nullable: false, maxLength: 50),
                        AttributeID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attributes", t => t.AttributeID, cascadeDelete: true)
                .Index(t => t.AttributeID);
            
            CreateTable(
                "dbo.Contracts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ContractTitle = c.String(nullable: false, maxLength: 33),
                        ContractDescription = c.String(nullable: false, maxLength: 1000),
                        ContractDate = c.DateTime(nullable: false),
                        Renewable = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PurchaseOrders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        QuoteID = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        WarehouseID = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                        Quote_Id = c.Long(),
                        Warehouse_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quotes", t => t.Quote_Id)
                .ForeignKey("dbo.Warehouses", t => t.Warehouse_Id)
                .Index(t => t.Quote_Id)
                .Index(t => t.Warehouse_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.WarehouseAdmins",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 256),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PurchaseOrders", "Warehouse_Id", "dbo.Warehouses");
            DropForeignKey("dbo.PurchaseOrders", "Quote_Id", "dbo.Quotes");
            DropForeignKey("dbo.AttributeValues", "AttributeID", "dbo.Attributes");
            DropForeignKey("dbo.Attributes", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Products", "UnitOfMeasurementID", "dbo.UnitOfMeasurements");
            DropForeignKey("dbo.ProductMaterials", "ProductID", "dbo.Products");
            DropForeignKey("dbo.ProductMaterials", "MaterialID", "dbo.RawMaterials");
            DropForeignKey("dbo.WarehouseMaterials", "WarehouseID", "dbo.Warehouses");
            DropForeignKey("dbo.WarehouseProducts", "WarehouseID", "dbo.Warehouses");
            DropForeignKey("dbo.WarehouseProducts", "ProductID", "dbo.Products");
            DropForeignKey("dbo.WarehouseMaterials", "RawMaterialID", "dbo.RawMaterials");
            DropForeignKey("dbo.RawMaterials", "UnitOfMeasurementID", "dbo.UnitOfMeasurements");
            DropForeignKey("dbo.MaterialTaxes", "TaxID", "dbo.Taxes");
            DropForeignKey("dbo.ProductTaxes", "TaxID", "dbo.Taxes");
            DropForeignKey("dbo.ProductTaxes", "ProductID", "dbo.Products");
            DropForeignKey("dbo.MaterialTaxes", "MaterialID", "dbo.RawMaterials");
            DropForeignKey("dbo.RawMaterials", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.RQItems", "RFQID", "dbo.RequestForQuotations");
            DropForeignKey("dbo.RQSuppliers", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.Quotes", "Supplier_Id", "dbo.Suppliers");
            DropForeignKey("dbo.Quotes", "RFQID", "dbo.RequestForQuotations");
            DropForeignKey("dbo.Quotes", "RawMaterial_Id", "dbo.RawMaterials");
            DropForeignKey("dbo.Quotes", "PaymentTermID", "dbo.PaymentTerms");
            DropForeignKey("dbo.SupplierAddresses", "Supplier_Id", "dbo.Suppliers");
            DropForeignKey("dbo.SupplierAddresses", "AddressTypeID", "dbo.AddressTypes");
            DropForeignKey("dbo.RQSuppliers", "RFQID", "dbo.RequestForQuotations");
            DropForeignKey("dbo.RequestForQuotations", "PaymentTerm_Id", "dbo.PaymentTerms");
            DropForeignKey("dbo.RQItems", "ItemID", "dbo.RawMaterials");
            DropForeignKey("dbo.ProductParts", "ProductID", "dbo.Products");
            DropForeignKey("dbo.ProductParts", "PartID", "dbo.Parts");
            DropForeignKey("dbo.Products", "CategoryID", "dbo.ProductCategories");
            DropForeignKey("dbo.Products", "BrandID", "dbo.Brands");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.PurchaseOrders", new[] { "Warehouse_Id" });
            DropIndex("dbo.PurchaseOrders", new[] { "Quote_Id" });
            DropIndex("dbo.AttributeValues", new[] { "AttributeID" });
            DropIndex("dbo.WarehouseProducts", new[] { "WarehouseID" });
            DropIndex("dbo.WarehouseProducts", new[] { "ProductID" });
            DropIndex("dbo.WarehouseMaterials", new[] { "WarehouseID" });
            DropIndex("dbo.WarehouseMaterials", new[] { "RawMaterialID" });
            DropIndex("dbo.ProductTaxes", new[] { "TaxID" });
            DropIndex("dbo.ProductTaxes", new[] { "ProductID" });
            DropIndex("dbo.MaterialTaxes", new[] { "TaxID" });
            DropIndex("dbo.MaterialTaxes", new[] { "MaterialID" });
            DropIndex("dbo.Quotes", new[] { "Supplier_Id" });
            DropIndex("dbo.Quotes", new[] { "RawMaterial_Id" });
            DropIndex("dbo.Quotes", new[] { "PaymentTermID" });
            DropIndex("dbo.Quotes", new[] { "RFQID" });
            DropIndex("dbo.SupplierAddresses", new[] { "Supplier_Id" });
            DropIndex("dbo.SupplierAddresses", new[] { "AddressTypeID" });
            DropIndex("dbo.RQSuppliers", new[] { "RFQID" });
            DropIndex("dbo.RQSuppliers", new[] { "SupplierID" });
            DropIndex("dbo.RequestForQuotations", new[] { "PaymentTerm_Id" });
            DropIndex("dbo.RQItems", new[] { "RFQID" });
            DropIndex("dbo.RQItems", new[] { "ItemID" });
            DropIndex("dbo.RawMaterials", new[] { "UnitOfMeasurementID" });
            DropIndex("dbo.RawMaterials", new[] { "SupplierID" });
            DropIndex("dbo.ProductMaterials", new[] { "MaterialID" });
            DropIndex("dbo.ProductMaterials", new[] { "ProductID" });
            DropIndex("dbo.ProductParts", new[] { "PartID" });
            DropIndex("dbo.ProductParts", new[] { "ProductID" });
            DropIndex("dbo.Products", new[] { "BrandID" });
            DropIndex("dbo.Products", new[] { "UnitOfMeasurementID" });
            DropIndex("dbo.Products", new[] { "CategoryID" });
            DropIndex("dbo.Products", "AlteranteKey_SKU");
            DropIndex("dbo.Attributes", new[] { "ProductID" });
            DropTable("dbo.WarehouseAdmins");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.PurchaseOrders");
            DropTable("dbo.Contracts");
            DropTable("dbo.AttributeValues");
            DropTable("dbo.WarehouseProducts");
            DropTable("dbo.Warehouses");
            DropTable("dbo.WarehouseMaterials");
            DropTable("dbo.UnitOfMeasurements");
            DropTable("dbo.ProductTaxes");
            DropTable("dbo.Taxes");
            DropTable("dbo.MaterialTaxes");
            DropTable("dbo.Quotes");
            DropTable("dbo.SupplierAddresses");
            DropTable("dbo.Suppliers");
            DropTable("dbo.RQSuppliers");
            DropTable("dbo.PaymentTerms");
            DropTable("dbo.RequestForQuotations");
            DropTable("dbo.RQItems");
            DropTable("dbo.RawMaterials");
            DropTable("dbo.ProductMaterials");
            DropTable("dbo.Parts");
            DropTable("dbo.ProductParts");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.Brands");
            DropTable("dbo.Products");
            DropTable("dbo.Attributes");
            DropTable("dbo.AddressTypes");
        }
    }
}
