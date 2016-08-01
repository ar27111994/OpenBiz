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
	public class AttributesController : Controller
	{
		private IEntityService<BLL.Entities.Inventory.Attribute> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: Attributes
		public AttributesController()  
		   {  
				this.repository = new EntityService<BLL.Entities.Inventory.Attribute>(db);  
		   }  
		public AttributesController(IEntityService<BLL.Entities.Inventory.Attribute> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<BLL.Entities.Inventory.Attribute>();
		records.Content = repository.GetAll(a => a.Product).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(a => a.Product).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<BLL.Entities.Inventory.Attribute>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Product.ProductName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Name.ToString().ToLower().Contains(filter.ToLower()))
		,a => a.Product)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Product.ProductName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Name.ToString().ToLower().Contains(filter.ToLower()))
		,a => a.Product).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: Attributes/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			BLL.Entities.Inventory.Attribute attribute = repository.GetById(id, a => a.Product);
			if (attribute == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",attribute);
		}

		// GET: Attributes/Create
		[HttpGet]
		public ActionResult Create()
		{
			BLL.Entities.Inventory.Attribute attribute = new BLL.Entities.Inventory.Attribute();
			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName");
			return PartialView("Create",attribute);
		}

		// POST: Attributes/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,Name,ProductID")] BLL.Entities.Inventory.Attribute attribute)
		{
			if (ModelState.IsValid)
			{
				repository.Add(attribute);
				repository.Commit();
				return Json(new { success = true });
			}

			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName", attribute.ProductID);
			return PartialView(attribute);
		}

		// GET: BLL.Entities.Inventory.Attributes/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			BLL.Entities.Inventory.Attribute attribute = repository.GetById(id);
			if (attribute == null)
			{
				return HttpNotFound();
			}
			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName", attribute.ProductID);
			return PartialView("Edit",attribute);
		}

		// POST: Attributes/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Name,ProductID")] BLL.Entities.Inventory.Attribute attribute)
		{
			if (ModelState.IsValid)
			{
				repository.Update(attribute);
				repository.Commit();
				return Json(new { success = true });
			}
			IEntityService<BLL.Entities.Inventory.Product> Productrepository = new EntityService<BLL.Entities.Inventory.Product>(db);
			ViewData["ProductID"] = new SelectList(Productrepository.GetAll(), "Id", "ProductName", attribute.ProductID);
			return PartialView("Edit", attribute);
		}

		// GET: Attributes/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			BLL.Entities.Inventory.Attribute attribute = repository.GetById(id, a => a.Product);
			if (attribute == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",attribute);
		}

		// POST: Attributes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			BLL.Entities.Inventory.Attribute attribute = repository.GetById(id);
			repository.Remove(attribute);
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
						BLL.Entities.Inventory.Attribute attribute = repository.GetById(key);
						repository.Remove(attribute);
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
			var records = repository.GetAll(a => a.Product).OrderBy("Id DESC").AsEnumerable<BLL.Entities.Inventory.Attribute>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<BLL.Entities.Inventory.Attribute>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "Attribute.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<BLL.Entities.Inventory.Attribute>(records.AsQueryable(), "Attribute.csv");
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
