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
using BLL.Entities.Production;
using DAL;

namespace SCMS.Controllers.Manufacturing
{
	public class PlantsController : Controller
	{
		private IEntityService<Plant> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: Plants
		public PlantsController()  
		   {  
				this.repository = new EntityService<Plant>(db);  
		   }  
		public PlantsController(IEntityService<Plant> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<Plant>();
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
		var records = new PagedList<Plant>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Name.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Address.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Name.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Address.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: Plants/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Plant plant = repository.GetById(id);
			if (plant == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",plant);
		}

		// GET: Plants/Create
		[HttpGet]
		public ActionResult Create()
		{
			Plant plant = new Plant();
			return PartialView("Create",plant);
		}

		// POST: Plants/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,Name,Address,Latitude,Longitude,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Plant plant)
		{
			if (ModelState.IsValid)
			{
				repository.Add(plant);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(plant);
		}

		// GET: Plants/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Plant plant = repository.GetById(id);
			if (plant == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",plant);
		}

		// POST: Plants/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Name,Address,Latitude,Longitude,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Plant plant)
		{
			if (ModelState.IsValid)
			{
				repository.Update(plant);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", plant);
		}

		// GET: Plants/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Plant plant = repository.GetById(id);
			if (plant == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",plant);
		}

		// POST: Plants/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			Plant plant = repository.GetById(id);
			repository.Remove(plant);
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
						Plant plant = repository.GetById(key);
						repository.Remove(plant);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<Plant>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<Plant>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "Plant.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<Plant>(records.AsQueryable(), "Plant.csv");
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
