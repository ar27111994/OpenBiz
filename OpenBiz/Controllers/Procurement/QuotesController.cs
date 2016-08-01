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
	public class QuotesController : Controller
	{
		private IEntityService<Quote> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: Quotes
		public QuotesController()  
		   {  
				this.repository = new EntityService<Quote>(db);  
		   }  
		public QuotesController(IEntityService<Quote> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<Quote>();
		records.Content = repository.GetAll(q => q.BuyingTerms,q => q.RFQ).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(q => q.BuyingTerms,q => q.RFQ).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<Quote>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.BuyingTerms.Title.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.RFQ.CreatedBy.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.QuoteDescription.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.QuotationDate.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.MaterialID.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.TotalCharges.ToString().ToLower().Contains(filter.ToLower()))
		,q => q.BuyingTerms,q => q.RFQ)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.BuyingTerms.Title.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.RFQ.CreatedBy.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.QuoteDescription.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.QuotationDate.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.MaterialID.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.TotalCharges.ToString().ToLower().Contains(filter.ToLower()))
		,q => q.BuyingTerms,q => q.RFQ).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: Quotes/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Quote quote = repository.GetById(id,q => q.BuyingTerms,q => q.RFQ);
			if (quote == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",quote);
		}

		// GET: Quotes/Create
		[HttpGet]
		public ActionResult Create()
		{
			Quote quote = new Quote();
			IEntityService<BLL.Entities.Procurement.PaymentTerm> PaymentTermrepository = new EntityService<BLL.Entities.Procurement.PaymentTerm>(db);
			ViewData["PaymentTermID"] = new SelectList(PaymentTermrepository.GetAll(), "Id", "Title");
			IEntityService<BLL.Entities.Procurement.RequestForQuotation> RequestForQuotationrepository = new EntityService<BLL.Entities.Procurement.RequestForQuotation>(db);
			ViewData["RFQID"] = new SelectList(RequestForQuotationrepository.GetAll(), "Id", "CreatedBy");
			return PartialView("Create",quote);
		}

		// POST: Quotes/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,QuoteDescription,QuotationDate,MaterialID,RFQID,PaymentTermID,TotalCharges,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Quote quote)
		{
			if (ModelState.IsValid)
			{
				repository.Add(quote);
				repository.Commit();
				return Json(new { success = true });
			}

			IEntityService<BLL.Entities.Procurement.PaymentTerm> PaymentTermrepository = new EntityService<BLL.Entities.Procurement.PaymentTerm>(db);
			ViewData["PaymentTermID"] = new SelectList(PaymentTermrepository.GetAll(), "Id", "Title", quote.PaymentTermID);
			IEntityService<BLL.Entities.Procurement.RequestForQuotation> RequestForQuotationrepository = new EntityService<BLL.Entities.Procurement.RequestForQuotation>(db);
			ViewData["RFQID"] = new SelectList(RequestForQuotationrepository.GetAll(), "Id", "CreatedBy", quote.RFQID);
			return PartialView(quote);
		}

		// GET: Quotes/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Quote quote = repository.GetById(id);
			if (quote == null)
			{
				return HttpNotFound();
			}
			IEntityService<BLL.Entities.Procurement.PaymentTerm> PaymentTermrepository = new EntityService<BLL.Entities.Procurement.PaymentTerm>(db);
			ViewData["PaymentTermID"] = new SelectList(PaymentTermrepository.GetAll(), "Id", "Title", quote.PaymentTermID);
			IEntityService<BLL.Entities.Procurement.RequestForQuotation> RequestForQuotationrepository = new EntityService<BLL.Entities.Procurement.RequestForQuotation>(db);
			ViewData["RFQID"] = new SelectList(RequestForQuotationrepository.GetAll(), "Id", "CreatedBy", quote.RFQID);
			return PartialView("Edit",quote);
		}

		// POST: Quotes/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,QuoteDescription,QuotationDate,MaterialID,RFQID,PaymentTermID,TotalCharges,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Quote quote)
		{
			if (ModelState.IsValid)
			{
				repository.Update(quote);
				repository.Commit();
				return Json(new { success = true });
			}
			IEntityService<BLL.Entities.Procurement.PaymentTerm> PaymentTermrepository = new EntityService<BLL.Entities.Procurement.PaymentTerm>(db);
			ViewData["PaymentTermID"] = new SelectList(PaymentTermrepository.GetAll(), "Id", "Title", quote.PaymentTermID);
			IEntityService<BLL.Entities.Procurement.RequestForQuotation> RequestForQuotationrepository = new EntityService<BLL.Entities.Procurement.RequestForQuotation>(db);
			ViewData["RFQID"] = new SelectList(RequestForQuotationrepository.GetAll(), "Id", "CreatedBy", quote.RFQID);
			return PartialView("Edit", quote);
		}

		// GET: Quotes/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Quote quote = repository.GetById(id,q => q.BuyingTerms,q => q.RFQ);
			if (quote == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",quote);
		}

		// POST: Quotes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			Quote quote = repository.GetById(id);
			repository.Remove(quote);
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
						Quote quote = repository.GetById(key);
						repository.Remove(quote);
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
			var records = repository.GetAll(q => q.BuyingTerms,q => q.RFQ).OrderBy("Id DESC").AsEnumerable<Quote>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<Quote>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "Quote.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<Quote>(records.AsQueryable(), "Quote.csv");
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
