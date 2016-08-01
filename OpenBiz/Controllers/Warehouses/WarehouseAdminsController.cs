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
	public class WarehouseAdminsController : Controller
	{
		private IEntityService<WarehouseAdmin> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: WarehouseAdmins
		public WarehouseAdminsController()  
		   {  
				this.repository = new EntityService<WarehouseAdmin>(db);  
		   }  
		public WarehouseAdminsController(IEntityService<WarehouseAdmin> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<WarehouseAdmin>();
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
		var records = new PagedList<WarehouseAdmin>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Username.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Password.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Email.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Username.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Password.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Email.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: WarehouseAdmins/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			WarehouseAdmin warehouseAdmin = repository.GetById(id);
			if (warehouseAdmin == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",warehouseAdmin);
		}

		// GET: WarehouseAdmins/Create
		[HttpGet]
		public ActionResult Create()
		{
			WarehouseAdmin warehouseAdmin = new WarehouseAdmin();
			return PartialView("Create",warehouseAdmin);
		}

		// POST: WarehouseAdmins/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,Username,Password,Email,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] WarehouseAdmin warehouseAdmin)
		{
			if (ModelState.IsValid)
			{
				repository.Add(warehouseAdmin);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(warehouseAdmin);
		}

		// GET: WarehouseAdmins/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			WarehouseAdmin warehouseAdmin = repository.GetById(id);
			if (warehouseAdmin == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",warehouseAdmin);
		}

		// POST: WarehouseAdmins/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Username,Password,Email,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] WarehouseAdmin warehouseAdmin)
		{
			if (ModelState.IsValid)
			{
				repository.Update(warehouseAdmin);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", warehouseAdmin);
		}

		// GET: WarehouseAdmins/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			WarehouseAdmin warehouseAdmin = repository.GetById(id);
			if (warehouseAdmin == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",warehouseAdmin);
		}

		// POST: WarehouseAdmins/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			WarehouseAdmin warehouseAdmin = repository.GetById(id);
			repository.Remove(warehouseAdmin);
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
						WarehouseAdmin warehouseAdmin = repository.GetById(key);
						repository.Remove(warehouseAdmin);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<WarehouseAdmin>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<WarehouseAdmin>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "WarehouseAdmin.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<WarehouseAdmin>(records.AsQueryable(), "WarehouseAdmin.csv");
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
