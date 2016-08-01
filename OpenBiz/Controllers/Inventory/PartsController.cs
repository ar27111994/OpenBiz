

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
	public class PartsController : Controller
	{
		private IEntityService<Part> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: Parts
		public PartsController()  
		   {  
				this.repository = new EntityService<Part>(db);  
		   }  
		public PartsController(IEntityService<Part> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<Part>();
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
		var records = new PagedList<Part>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.PartName.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.PartName.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: Parts/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Part part = repository.GetById(id);
			if (part == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",part);
		}

		// GET: Parts/Create
		[HttpGet]
		public ActionResult Create()
		{
			Part part = new Part();
			return PartialView("Create",part);
		}

		// POST: Parts/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,PartName,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Part part)
		{
			if (ModelState.IsValid)
			{
				repository.Add(part);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(part);
		}

		// GET: Parts/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Part part = repository.GetById(id);
			if (part == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",part);
		}

		// POST: Parts/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,PartName,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Part part)
		{
			if (ModelState.IsValid)
			{
				repository.Update(part);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", part);
		}

		// GET: Parts/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Part part = repository.GetById(id);
			if (part == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",part);
		}

		// POST: Parts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			Part part = repository.GetById(id);
			repository.Remove(part);
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
						Part part = repository.GetById(key);
						repository.Remove(part);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<Part>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<Part>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "Part.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<Part>(records.AsQueryable(), "Part.csv");
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
