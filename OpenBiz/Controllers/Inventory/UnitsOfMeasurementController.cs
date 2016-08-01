

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
	public class UnitsOfMeasurementController : Controller
	{
		private IEntityService<UnitOfMeasurement> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: UnitOfMeasurements
		public UnitsOfMeasurementController()  
		   {  
				this.repository = new EntityService<UnitOfMeasurement>(db);  
		   }  
		public UnitsOfMeasurementController(IEntityService<UnitOfMeasurement> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<UnitOfMeasurement>();
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
		var records = new PagedList<UnitOfMeasurement>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Unit.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.IsActive.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Unit.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.IsActive.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: UnitOfMeasurements/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			UnitOfMeasurement unitOfMeasurement = repository.GetById(id);
			if (unitOfMeasurement == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",unitOfMeasurement);
		}

		// GET: UnitOfMeasurements/Create
		[HttpGet]
		public ActionResult Create()
		{
			UnitOfMeasurement unitOfMeasurement = new UnitOfMeasurement();
			return PartialView("Create",unitOfMeasurement);
		}

		// POST: UnitOfMeasurements/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,Unit,IsActive,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] UnitOfMeasurement unitOfMeasurement)
		{
			if (ModelState.IsValid)
			{
				repository.Add(unitOfMeasurement);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(unitOfMeasurement);
		}

		// GET: UnitOfMeasurements/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			UnitOfMeasurement unitOfMeasurement = repository.GetById(id);
			if (unitOfMeasurement == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",unitOfMeasurement);
		}

		// POST: UnitOfMeasurements/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Unit,IsActive,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] UnitOfMeasurement unitOfMeasurement)
		{
			if (ModelState.IsValid)
			{
				repository.Update(unitOfMeasurement);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", unitOfMeasurement);
		}

		// GET: UnitOfMeasurements/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			UnitOfMeasurement unitOfMeasurement = repository.GetById(id);
			if (unitOfMeasurement == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",unitOfMeasurement);
		}

		// POST: UnitOfMeasurements/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			UnitOfMeasurement unitOfMeasurement = repository.GetById(id);
			repository.Remove(unitOfMeasurement);
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
						UnitOfMeasurement unitOfMeasurement = repository.GetById(key);
						repository.Remove(unitOfMeasurement);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<UnitOfMeasurement>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<UnitOfMeasurement>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "UnitOfMeasurement.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<UnitOfMeasurement>(records.AsQueryable(), "UnitOfMeasurement.csv");
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
