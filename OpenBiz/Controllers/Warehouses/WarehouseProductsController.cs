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
using BLL.Entities.Warehouse;
using DAL;

namespace SCMS.Controllers.Warehouses
{
	public class WarehouseProductsController : Controller
	{
		private IEntityService<WarehouseProducts> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: WarehouseProducts
		public WarehouseProductsController()  
		   {  
				this.repository = new EntityService<WarehouseProducts>(db);  
		   }  
		public WarehouseProductsController(IEntityService<WarehouseProducts> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<WarehouseProducts>();
		records.Content = repository.GetAll(w => w.Product,w => w.Warehouse).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(w => w.Product,w => w.Warehouse).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<WarehouseProducts>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Product.ProductName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Warehouse.WarehouseName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.EntryTitle.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Purpose.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Quantity.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.PostingTime.ToString().ToLower().Contains(filter.ToLower()))
		,w => w.Product,w => w.Warehouse)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Product.ProductName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Warehouse.WarehouseName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.EntryTitle.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Purpose.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Quantity.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.PostingTime.ToString().ToLower().Contains(filter.ToLower()))
		,w => w.Product,w => w.Warehouse).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: WarehouseProducts/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			WarehouseProducts warehouseProducts = repository.GetById(id,w => w.Product,w => w.Warehouse);
			if (warehouseProducts == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",warehouseProducts);
		}

		// GET: WarehouseProducts/Create
		[HttpGet]
		public ActionResult Create()
		{
			WarehouseProducts warehouseProducts = new WarehouseProducts();
			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName");
			IEntityService<BLL.Entities.Warehouse.Warehouse> Warehouserepository = new EntityService<BLL.Entities.Warehouse.Warehouse>(db);
			ViewData["WarehouseID"] = new SelectList(Warehouserepository.GetAll(), "Id", "WarehouseName");
			return PartialView("Create",warehouseProducts);
		}

		// POST: WarehouseProducts/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,EntryTitle,Purpose,ProductID,WarehouseID,Quantity,PostingTime,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] WarehouseProducts warehouseProducts)
		{
			if (ModelState.IsValid)
			{
				repository.Add(warehouseProducts);
				repository.Commit();
				return Json(new { success = true });
			}

			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName", warehouseProducts.ProductID);
			IEntityService<BLL.Entities.Warehouse.Warehouse> Warehouserepository = new EntityService<BLL.Entities.Warehouse.Warehouse>(db);
			ViewData["WarehouseID"] = new SelectList(Warehouserepository.GetAll(), "Id", "WarehouseName", warehouseProducts.WarehouseID);
			return PartialView(warehouseProducts);
		}

		// GET: WarehouseProducts/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			WarehouseProducts warehouseProducts = repository.GetById(id);
			if (warehouseProducts == null)
			{
				return HttpNotFound();
			}
			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName", warehouseProducts.ProductID);
			IEntityService<BLL.Entities.Warehouse.Warehouse> Warehouserepository = new EntityService<BLL.Entities.Warehouse.Warehouse>(db);
			ViewData["WarehouseID"] = new SelectList(Warehouserepository.GetAll(), "Id", "WarehouseName", warehouseProducts.WarehouseID);
			return PartialView("Edit",warehouseProducts);
		}

		// POST: WarehouseProducts/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,EntryTitle,Purpose,ProductID,WarehouseID,Quantity,PostingTime,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] WarehouseProducts warehouseProducts)
		{
			if (ModelState.IsValid)
			{
				repository.Update(warehouseProducts);
				repository.Commit();
				return Json(new { success = true });
			}
			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName", warehouseProducts.ProductID);
			IEntityService<BLL.Entities.Warehouse.Warehouse> Warehouserepository = new EntityService<BLL.Entities.Warehouse.Warehouse>(db);
			ViewData["WarehouseID"] = new SelectList(Warehouserepository.GetAll(), "Id", "WarehouseName", warehouseProducts.WarehouseID);
			return PartialView("Edit", warehouseProducts);
		}

		// GET: WarehouseProducts/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			WarehouseProducts warehouseProducts = repository.GetById(id,w => w.Product,w => w.Warehouse);
			if (warehouseProducts == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",warehouseProducts);
		}

		// POST: WarehouseProducts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			WarehouseProducts warehouseProducts = repository.GetById(id);
			repository.Remove(warehouseProducts);
			repository.Commit();
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
						WarehouseProducts warehouseProducts = repository.GetById(key);
						repository.Remove(warehouseProducts);
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

        public ActionResult ProductStockLegder()
        {
            return View();
        }


        public JsonResult DataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<WarehouseProducts>();

                dtsource = repository.GetAll(w => w.Product, w => w.Warehouse).ToList();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<WarehouseProducts> data = new ProductsStockResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = new ProductsStockResultSet().Count(param.Search.Value, dtsource, columnSearch);
                List<WarehouseProducts> filteredResults = new List<WarehouseProducts>();
                foreach (var d in data)
                {
                    var tmp = new WarehouseProducts();
                    tmp.EntryTitle = d.EntryTitle;
                    tmp.EntryType = d.EntryType;
                    tmp.PostingTime = d.PostingTime;
                    tmp.Purpose = d.Purpose;
                    tmp.Quantity = d.Quantity;
                    IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
                    d.Product = Productrepository.GetById(d.ProductID);
                    tmp.Product = d.Product;
                    tmp.Product.ProductName = d.Product.ProductName;

                    IEntityService<BLL.Entities.Warehouse.Warehouse> Warehouserepository = new EntityService<BLL.Entities.Warehouse.Warehouse>(db);
                    d.Warehouse = Warehouserepository.GetById(d.WarehouseID);
                    tmp.Warehouse = d.Warehouse;
                    tmp.Warehouse.WarehouseName = d.Warehouse.WarehouseName;

                    filteredResults.Add(tmp);
                }
                DTResult<WarehouseProducts> result = new DTResult<WarehouseProducts>
                {
                    draw = param.Draw,
                    data = filteredResults,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                //var content = JsonConvert.SerializeObject(result,
                //                Formatting.None,
                //                new JsonSerializerSettings()
                //                {
                //                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                //                });
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
		public ActionResult Export(ExportParameters model)
		{
			var records = repository.GetAll(w => w.Product,w => w.Warehouse).OrderBy("Id DESC").AsEnumerable<WarehouseProducts>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<WarehouseProducts>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "WarehouseProducts.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<WarehouseProducts>(records.AsQueryable(), "WarehouseProducts.csv");
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
