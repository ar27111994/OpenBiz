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
using BLL.Entities.Distribution;
using DAL;

namespace SCMS.Controllers.Distribution
{
	public class DistributorsController : Controller
	{
		private IEntityService<Distributor> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: Distributors
		public DistributorsController()  
		   {  
				this.repository = new EntityService<Distributor>(db);  
		   }  
		public DistributorsController(IEntityService<Distributor> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<Distributor>();
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
		var records = new PagedList<Distributor>();
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

		
		// GET: Distributors/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Distributor distributor = repository.GetById(id);
			if (distributor == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",distributor);
		}

		// GET: Distributors/Create
		[HttpGet]
		public ActionResult Create()
		{
			Distributor distributor = new Distributor();
			return PartialView("Create",distributor);
		}

		// POST: Distributors/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,Name,Address,Latitude,Longitude,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Distributor distributor)
		{
			if (ModelState.IsValid)
			{
				repository.Add(distributor);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(distributor);
		}

		// GET: Distributors/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Distributor distributor = repository.GetById(id);
			if (distributor == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",distributor);
		}

		// POST: Distributors/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Name,Address,Latitude,Longitude,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Distributor distributor)
		{
			if (ModelState.IsValid)
			{
				repository.Update(distributor);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", distributor);
		}

		// GET: Distributors/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Distributor distributor = repository.GetById(id);
			if (distributor == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",distributor);
		}

		// POST: Distributors/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			Distributor distributor = repository.GetById(id);
			repository.Remove(distributor);
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
						Distributor distributor = repository.GetById(key);
						repository.Remove(distributor);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<Distributor>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<Distributor>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "Distributor.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<Distributor>(records.AsQueryable(), "Distributor.csv");
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
