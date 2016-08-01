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
	public class TaxesController : Controller
	{
		private IEntityService<Tax> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: Taxes
		public TaxesController()  
		   {  
				this.repository = new EntityService<Tax>(db);  
		   }  
		public TaxesController(IEntityService<Tax> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<Tax>();
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
		var records = new PagedList<Tax>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Name.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Percentage.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Name.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Percentage.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: Taxes/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tax tax = repository.GetById(id);
			if (tax == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",tax);
		}

		// GET: Taxes/Create
		[HttpGet]
		public ActionResult Create()
		{
			Tax tax = new Tax();
			return PartialView("Create",tax);
		}

		// POST: Taxes/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,Name,Percentage,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Tax tax)
		{
			if (ModelState.IsValid)
			{
				repository.Add(tax);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(tax);
		}

		// GET: Taxes/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tax tax = repository.GetById(id);
			if (tax == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",tax);
		}

		// POST: Taxes/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Name,Percentage,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Tax tax)
		{
			if (ModelState.IsValid)
			{
				repository.Update(tax);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", tax);
		}

		// GET: Taxes/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tax tax = repository.GetById(id);
			if (tax == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",tax);
		}

		// POST: Taxes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			Tax tax = repository.GetById(id);
			repository.Remove(tax);
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
						Tax tax = repository.GetById(key);
						repository.Remove(tax);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<Tax>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<Tax>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "Tax.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<Tax>(records.AsQueryable(), "Tax.csv");
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
