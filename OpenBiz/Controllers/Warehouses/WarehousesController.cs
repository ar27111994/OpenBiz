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
	public class WarehousesController : Controller
	{
		private IEntityService<Warehouse> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: Warehouses
		public WarehousesController()  
		   {  
				this.repository = new EntityService<Warehouse>(db);  
		   }  
		public WarehousesController(IEntityService<Warehouse> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<Warehouse>();
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
		var records = new PagedList<Warehouse>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.WarehouseName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.WarehouseLocation.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.WarehouseName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.WarehouseLocation.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: Warehouses/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Warehouse warehouse = repository.GetById(id);
			if (warehouse == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",warehouse);
		}

		// GET: Warehouses/Create
		[HttpGet]
		public ActionResult Create()
		{
			Warehouse warehouse = new Warehouse();
			return PartialView("Create",warehouse);
		}

		// POST: Warehouses/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,WarehouseName,WarehouseLocation,Longitude,Latitude,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Warehouse warehouse)
		{
			if (ModelState.IsValid)
			{
				repository.Add(warehouse);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(warehouse);
		}

		// GET: Warehouses/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Warehouse warehouse = repository.GetById(id);
			if (warehouse == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",warehouse);
		}

		// POST: Warehouses/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,WarehouseName,WarehouseLocation,Longitude,Latitude,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Warehouse warehouse)
		{
			if (ModelState.IsValid)
			{
				repository.Update(warehouse);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", warehouse);
		}

		// GET: Warehouses/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Warehouse warehouse = repository.GetById(id);
			if (warehouse == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",warehouse);
		}

		// POST: Warehouses/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			Warehouse warehouse = repository.GetById(id);
			repository.Remove(warehouse);
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
						Warehouse warehouse = repository.GetById(key);
						repository.Remove(warehouse);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<Warehouse>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<Warehouse>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "Warehouse.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<Warehouse>(records.AsQueryable(), "Warehouse.csv");
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
