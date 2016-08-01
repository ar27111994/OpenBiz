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
	public class ProductCategoriesController : Controller
	{
		private IEntityService<ProductCategory> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: ProductCategories
		public ProductCategoriesController()  
		   {  
				this.repository = new EntityService<ProductCategory>(db);  
		   }  
		public ProductCategoriesController(IEntityService<ProductCategory> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<ProductCategory>();
		records.Content = repository.GetAll()
						.OrderBy("Id DESC")
						.Take(20).ToList();
		// Count
		records.TotalRecords = repository.GetAll().Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<ProductCategory>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.CategoryName.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.CategoryName.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: ProductCategories/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ProductCategory productCategory = repository.GetById(id);
			if (productCategory == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",productCategory);
		}

		// GET: ProductCategories/Create
		[HttpGet]
		public ActionResult Create()
		{
			ProductCategory productCategory = new ProductCategory();
			return PartialView("Create",productCategory);
		}

		// POST: ProductCategories/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,CategoryName,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] ProductCategory productCategory)
		{
			if (ModelState.IsValid)
			{
				repository.Add(productCategory);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(productCategory);
		}

		// GET: ProductCategories/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ProductCategory productCategory = repository.GetById(id);
			if (productCategory == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",productCategory);
		}

		// POST: ProductCategories/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,CategoryName,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] ProductCategory productCategory)
		{
			if (ModelState.IsValid)
			{
				repository.Update(productCategory);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", productCategory);
		}

		// GET: ProductCategories/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ProductCategory productCategory = repository.GetById(id);
			if (productCategory == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",productCategory);
		}

		// POST: ProductCategories/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			ProductCategory productCategory = repository.GetById(id);
			repository.Remove(productCategory);
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
						ProductCategory productCategory = repository.GetById(key);
						repository.Remove(productCategory);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<ProductCategory>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<ProductCategory>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "ProductCategory.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<ProductCategory>(records.AsQueryable(), "ProductCategory.csv");
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
