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
using BLL.Entities.Procurement;
using DAL;

namespace SCMS.Controllers.Procurement
{
	public class PaymentTermsController : Controller
	{
		private IEntityService<PaymentTerm> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: PaymentTerms
		public PaymentTermsController()  
		   {  
				this.repository = new EntityService<PaymentTerm>(db);  
		   }  
		public PaymentTermsController(IEntityService<PaymentTerm> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<PaymentTerm>();
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
		var records = new PagedList<PaymentTerm>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Title.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Description.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Days.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Title.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Description.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Days.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: PaymentTerms/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			PaymentTerm paymentTerm = repository.GetById(id);
			if (paymentTerm == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",paymentTerm);
		}

		// GET: PaymentTerms/Create
		[HttpGet]
		public ActionResult Create()
		{
			PaymentTerm paymentTerm = new PaymentTerm();
			return PartialView("Create",paymentTerm);
		}

		// POST: PaymentTerms/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "PaymentTermID,Title,Description,Days")] PaymentTerm paymentTerm)
		{
			if (ModelState.IsValid)
			{
				repository.Add(paymentTerm);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(paymentTerm);
		}

		// GET: PaymentTerms/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			PaymentTerm paymentTerm = repository.GetById(id);
			if (paymentTerm == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",paymentTerm);
		}

		// POST: PaymentTerms/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "PaymentTermID,Title,Description,Days")] PaymentTerm paymentTerm)
		{
			if (ModelState.IsValid)
			{
				repository.Update(paymentTerm);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", paymentTerm);
		}

		// GET: PaymentTerms/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			PaymentTerm paymentTerm = repository.GetById(id);
			if (paymentTerm == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",paymentTerm);
		}

		// POST: PaymentTerms/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			PaymentTerm paymentTerm = repository.GetById(id);
			repository.Remove(paymentTerm);
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
						PaymentTerm paymentTerm = repository.GetById(key);
						repository.Remove(paymentTerm);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<PaymentTerm>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<PaymentTerm>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "PaymentTerm.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<PaymentTerm>(records.AsQueryable(), "PaymentTerm.csv");
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
