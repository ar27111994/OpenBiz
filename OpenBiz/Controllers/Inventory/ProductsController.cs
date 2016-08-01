using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using DAL.Repository.Persistence;
using SCMS.ViewModels;
using System.Web;
using System.Web.Mvc;
using SCMS.DataExport;
using SCMS.Actions;
using BLL.Entities.Inventory;
using DAL;
using System.IO;

namespace SCMS.Controllers.Inventory
{
    public class ProductsController : Controller
    {
        private IEntityService<Product> repository = null;
        private SCMSContext db = new SCMSContext();
        // GET: Products
        public ProductsController()
        {
            this.repository = new EntityService<Product>(db);
        }
        public ProductsController(IEntityService<Product> repository)
        {
            this.repository = repository;
        }
        public ActionResult Index()
        {
            var records = new PagedList<Product>();
            records.Content = repository.GetAll(p => p.Brand, p => p.Category, p => p.UnitOfMeasurement).OrderBy("Id DESC")
                            .Take(20).ToList();

            // Count
            records.TotalRecords = repository.GetAll(p => p.Brand, p => p.Category, p => p.UnitOfMeasurement).Count();

            records.CurrentPage = 1;
            records.PageSize = 20;
            return View(records);
        }



        [HttpGet]
        public ActionResult Grid(string filter = null, int page = 1,
         int pageSize = 20, string sort = "Id", string sortdir = "DESC")
        {
            var records = new PagedList<Product>();
            ViewBag.filter = filter;
            records.Content = repository.GetList(x => filter == null
                                        || (x.Brand.BrandName.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Category.CategoryName.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.UnitOfMeasurement.Unit.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.ProductName.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.SKU.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.ProductBasePrice.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.ProductDescription.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Barcode.ToString().ToLower().Contains(filter.ToLower()))
            , p => p.Brand, p => p.Category, p => p.UnitOfMeasurement)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize).ToList();

            // Count
            records.TotalRecords = repository.GetList(x => filter == null
                                        || (x.Brand.BrandName.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Category.CategoryName.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.UnitOfMeasurement.Unit.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.ProductName.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.SKU.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.ProductBasePrice.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.ProductDescription.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Barcode.ToString().ToLower().Contains(filter.ToLower()))
            , p => p.Brand, p => p.Category, p => p.UnitOfMeasurement).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            return PartialView("Grid", records);
        }


        // GET: Products/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = repository.GetById(id, p => p.Brand, p => p.Category, p => p.UnitOfMeasurement);
            if (product == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", product);
        }

        // GET: Products/Create
        [HttpGet]
        public ActionResult Create()
        {
            Product product = new Product();
            IEntityService<BLL.Entities.Inventory.Brand> Brandrepository = new EntityService<BLL.Entities.Inventory.Brand>(db);
            ViewData["BrandID"] = new SelectList(Brandrepository.GetAll(), "Id", "BrandName");
            IEntityService<BLL.Entities.Inventory.ProductCategory> ProductCategoryrepository = new EntityService<BLL.Entities.Inventory.ProductCategory>(db);
            ViewData["CategoryID"] = new SelectList(ProductCategoryrepository.GetAll(), "Id", "CategoryName");
            IEntityService<BLL.Entities.Inventory.UnitOfMeasurement> UnitOfMeasurementrepository = new EntityService<BLL.Entities.Inventory.UnitOfMeasurement>(db);
            ViewData["UnitOfMeasurementID"] = new SelectList(UnitOfMeasurementrepository.GetAll(), "Id", "Unit");
            return PartialView("Create", product);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductName,SKU,ProductBasePrice,ProductDescription,CategoryID,Barcode,UnitOfMeasurementID,BrandID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase upload = Request.Files[0];

                    if (upload == null)
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }
                    else if (upload.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 3; //3 MB
                        string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png" };

                        if (!AllowedFileExtensions.Contains(upload.FileName.Substring(upload.FileName.LastIndexOf('.'))))
                        {
                            ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                        }

                        else if (upload.ContentLength > MaxContentLength)
                        {
                            ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
                        }
                        else
                        {
                            var fileName = product.ProductName;
                            var path = Path.Combine(Server.MapPath("~/Content/Images/ProductImages/"), fileName + ".png");
                            upload.SaveAs(path);
                            ModelState.Clear();
                            repository.Add(product);
                            repository.Commit();
                            return Json(new { success = true });
                        }
                    }


                }
            }

            IEntityService<BLL.Entities.Inventory.Brand> Brandrepository = new EntityService<BLL.Entities.Inventory.Brand>(db);
            ViewData["BrandID"] = new SelectList(Brandrepository.GetAll(), "Id", "BrandName", product.BrandID);
            IEntityService<BLL.Entities.Inventory.ProductCategory> ProductCategoryrepository = new EntityService<BLL.Entities.Inventory.ProductCategory>(db);
            ViewData["CategoryID"] = new SelectList(ProductCategoryrepository.GetAll(), "Id", "CategoryName", product.CategoryID);
            IEntityService<BLL.Entities.Inventory.UnitOfMeasurement> UnitOfMeasurementrepository = new EntityService<BLL.Entities.Inventory.UnitOfMeasurement>(db);
            ViewData["UnitOfMeasurementID"] = new SelectList(UnitOfMeasurementrepository.GetAll(), "Id", "Unit", product.UnitOfMeasurementID);
            return PartialView(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = repository.GetById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            IEntityService<BLL.Entities.Inventory.Brand> Brandrepository = new EntityService<BLL.Entities.Inventory.Brand>(db);
            ViewData["BrandID"] = new SelectList(Brandrepository.GetAll(), "Id", "BrandName", product.BrandID);
            IEntityService<BLL.Entities.Inventory.ProductCategory> ProductCategoryrepository = new EntityService<BLL.Entities.Inventory.ProductCategory>(db);
            ViewData["CategoryID"] = new SelectList(ProductCategoryrepository.GetAll(), "Id", "CategoryName", product.CategoryID);
            IEntityService<BLL.Entities.Inventory.UnitOfMeasurement> UnitOfMeasurementrepository = new EntityService<BLL.Entities.Inventory.UnitOfMeasurement>(db);
            ViewData["UnitOfMeasurementID"] = new SelectList(UnitOfMeasurementrepository.GetAll(), "Id", "Unit", product.UnitOfMeasurementID);
            return PartialView("Edit", product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductName,SKU,ProductBasePrice,ProductDescription,CategoryID,Barcode,UnitOfMeasurementID,BrandID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase upload = Request.Files[0];

                    if (upload == null)
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }
                    else if (upload.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 3; //3 MB
                        string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png" };

                        if (!AllowedFileExtensions.Contains(upload.FileName.Substring(upload.FileName.LastIndexOf('.'))))
                        {
                            ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
                        }

                        else if (upload.ContentLength > MaxContentLength)
                        {
                            ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
                        }
                        else
                        {
                            var fileName = product.ProductName;
                            var path = Path.Combine(Server.MapPath("~/Content/Images/ProductImages/"), fileName + ".png");
                            upload.SaveAs(path);
                            ModelState.Clear();
                        }
                    }


                }
                repository.Update(product);
                repository.Commit();
                return Json(new { success = true });
            }
            IEntityService<BLL.Entities.Inventory.Brand> Brandrepository = new EntityService<BLL.Entities.Inventory.Brand>(db);
            ViewData["BrandID"] = new SelectList(Brandrepository.GetAll(), "Id", "BrandName", product.BrandID);
            IEntityService<BLL.Entities.Inventory.ProductCategory> ProductCategoryrepository = new EntityService<BLL.Entities.Inventory.ProductCategory>(db);
            ViewData["CategoryID"] = new SelectList(ProductCategoryrepository.GetAll(), "Id", "CategoryName", product.CategoryID);
            IEntityService<BLL.Entities.Inventory.UnitOfMeasurement> UnitOfMeasurementrepository = new EntityService<BLL.Entities.Inventory.UnitOfMeasurement>(db);
            ViewData["UnitOfMeasurementID"] = new SelectList(UnitOfMeasurementrepository.GetAll(), "Id", "Unit", product.UnitOfMeasurementID);
            return PartialView("Edit", product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = repository.GetById(id, p => p.Brand, p => p.Category, p => p.UnitOfMeasurement);
            if (product == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Product product = repository.GetById(id);
            repository.Remove(product);
            repository.Commit();
            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Images/ProductImages/"), product.ProductName + ".png"));
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult DeleteSelected(string[] ids)
        {
            if (ids != null)
            {
                long key = 0;
                foreach (string id in ids)
                {
                    if (long.TryParse(id, out key))
                    {
                        Product product = repository.GetById(key);
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Images/ProductImages/"), product.ProductName + ".png"));
                        repository.Remove(product);
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                repository.Commit();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult Export(ExportParameters model)
        {
            var records = repository.GetAll(p => p.Brand, p => p.Category, p => p.UnitOfMeasurement).OrderBy("Id DESC").AsEnumerable<Product>();
            if (model.PagingEnabled && model.Range.ToString() != "All")
            {
                records = records.Skip((model.CurrentPage - 1) * model.PageSize)
                   .Take(model.PageSize);
            }
            if (model.OutputType.Equals(Output.Excel))
            {
                var excelFormatter = new ExcelFormatter<Product>(records);
                return new ExcelResult(excelFormatter.WriteHtmlTable(), "Product.xls");
            }
            if (model.OutputType.Equals(Output.Csv))
            {
                return new CsvResult<Product>(records.AsQueryable(), "Product.csv");
            }
            return Json(new { success = false });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
