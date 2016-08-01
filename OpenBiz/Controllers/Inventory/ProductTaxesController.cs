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

namespace SCMS.Controllers.Inventory
{
	public class ProductTaxesController : Controller
	{
		private IEntityService<ProductTaxes> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: ProductTaxes
		public ProductTaxesController()  
		   {  
				this.repository = new EntityService<ProductTaxes>(db);  
		   }  
		public ProductTaxesController(IEntityService<ProductTaxes> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<ProductTaxes>();
		records.Content = repository.GetAll(p => p.Product,p => p.Tax).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(p => p.Product,p => p.Tax).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<ProductTaxes>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Product.ProductName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Tax.Name.ToString().ToLower().Contains(filter.ToLower()))
		,p => p.Product,p => p.Tax)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Product.ProductName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Tax.Name.ToString().ToLower().Contains(filter.ToLower()))
		,p => p.Product,p => p.Tax).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: ProductTaxes/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ProductTaxes productTaxes = repository.GetById(id,p => p.Product,p => p.Tax);
			if (productTaxes == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",productTaxes);
		}

		// GET: ProductTaxes/Create
		[HttpGet]
		public ActionResult Create()
		{
			ProductTaxes productTaxes = new ProductTaxes();
			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName");
			IEntityService<BLL.Entities.Inventory.Tax> Taxrepository = new EntityService<BLL.Entities.Inventory.Tax>(db);
			ViewData["TaxID"] = new SelectList(Taxrepository.GetAll(), "Id", "Name");
			return PartialView("Create",productTaxes);
		}

		// POST: ProductTaxes/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ProductID,TaxID,Id,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] ProductTaxes productTaxes)
		{
			if (ModelState.IsValid)
			{
				repository.Add(productTaxes);
				repository.Commit();
				return Json(new { success = true });
			}

			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName", productTaxes.ProductID);
			IEntityService<BLL.Entities.Inventory.Tax> Taxrepository = new EntityService<BLL.Entities.Inventory.Tax>(db);
			ViewData["TaxID"] = new SelectList(Taxrepository.GetAll(), "Id", "Name", productTaxes.TaxID);
			return PartialView(productTaxes);
		}

		// GET: ProductTaxes/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ProductTaxes productTaxes = repository.GetById(id);
			if (productTaxes == null)
			{
				return HttpNotFound();
			}
			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName", productTaxes.ProductID);
			IEntityService<BLL.Entities.Inventory.Tax> Taxrepository = new EntityService<BLL.Entities.Inventory.Tax>(db);
			ViewData["TaxID"] = new SelectList(Taxrepository.GetAll(), "Id", "Name", productTaxes.TaxID);
			return PartialView("Edit",productTaxes);
		}

		// POST: ProductTaxes/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ProductID,TaxID,Id,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] ProductTaxes productTaxes)
		{
			if (ModelState.IsValid)
			{
				repository.Update(productTaxes);
				repository.Commit();
				return Json(new { success = true });
			}
			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName", productTaxes.ProductID);
			IEntityService<BLL.Entities.Inventory.Tax> Taxrepository = new EntityService<BLL.Entities.Inventory.Tax>(db);
			ViewData["TaxID"] = new SelectList(Taxrepository.GetAll(), "Id", "Name", productTaxes.TaxID);
			return PartialView("Edit", productTaxes);
		}

		// GET: ProductTaxes/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ProductTaxes productTaxes = repository.GetById(id,p => p.Product,p => p.Tax);
			if (productTaxes == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",productTaxes);
		}

		// POST: ProductTaxes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			ProductTaxes productTaxes = repository.GetById(id);
			repository.Remove(productTaxes);
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
						ProductTaxes productTaxes = repository.GetById(key);
						repository.Remove(productTaxes);
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
			var records = repository.GetAll(p => p.Product,p => p.Tax).OrderBy("Id DESC").AsEnumerable<ProductTaxes>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<ProductTaxes>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "ProductTaxes.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<ProductTaxes>(records.AsQueryable(), "ProductTaxes.csv");
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
