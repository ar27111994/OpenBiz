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
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Net.Mail;

namespace SCMS.Controllers.Procurement
{
	public class RequestForQuotationsController : Controller
	{
		private IEntityService<RequestForQuotation> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: RequestForQuotations
		public RequestForQuotationsController()  
		   {  
				this.repository = new EntityService<RequestForQuotation>(db);  
		   }  
		public RequestForQuotationsController(IEntityService<RequestForQuotation> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<RequestForQuotation>();
		records.Content = repository.GetAll(r => r.PaymentTerm).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(r => r.PaymentTerm).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<RequestForQuotation>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.PaymentTerm.Title.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.ScheduledDate.ToString().ToLower().Contains(filter.ToLower()))
		,r => r.PaymentTerm)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.PaymentTerm.Title.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.ScheduledDate.ToString().ToLower().Contains(filter.ToLower()))
		,r => r.PaymentTerm).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: RequestForQuotations/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RequestForQuotation requestForQuotation = repository.GetById(id,r => r.PaymentTerm);
			if (requestForQuotation == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",requestForQuotation);
		}

		// GET: RequestForQuotations/Create
		[HttpGet]
		public ActionResult Create()
		{
			RequestForQuotation requestForQuotation = new RequestForQuotation();
			IEntityService<BLL.Entities.Procurement.Supplier> Supplierrepository = new EntityService<BLL.Entities.Procurement.Supplier>(db);
			ViewData["Suppliers"] = new SelectList(Supplierrepository.GetAll(), "Id", "SupplierName");
            IEntityService<BLL.Entities.Inventory.RawMaterial> Materialsrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
            ViewData["Items"] = new SelectList(Materialsrepository.GetAll(), "Id", "MaterialName");
            IEntityService<BLL.Entities.Procurement.PaymentTerm> PaymentTermrepository = new EntityService<BLL.Entities.Procurement.PaymentTerm>(db);
			ViewData["PaymentTermID"] = new SelectList(PaymentTermrepository.GetAll(), "Id", "Title");
			return PartialView("Create",requestForQuotation);
		}

		// POST: RequestForQuotations/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,ScheduledDate,PaymentTermID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] RequestForQuotation requestForQuotation, string[] Suppliers, string[] Items, string[] Quantity)
        {
            IEntityService<BLL.Entities.Procurement.PaymentTerm> PaymentTermrepository = new EntityService<BLL.Entities.Procurement.PaymentTerm>(db);

            if (ModelState.IsValid)
			{
                
				repository.Add(requestForQuotation);
				repository.Commit();

                FileStream fs = new FileStream(Server.MapPath("~/Content/RFQs/RFQ_" + DateTime.Now.ToString().Replace(':', '-') + ".pdf"), FileMode.Create, FileAccess.Write, FileShare.None);
                Document doc = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                PdfPTable table = new PdfPTable(2);
                table.TotalWidth = 400f;
                table.LockedWidth = true;

                float[] widths = new float[] { 6f, 6f };
                table.SetWidths(widths);
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 30f;
                PdfPCell cell = new PdfPCell(new Phrase("Requested Raw Materials"));
                cell.Colspan = 3;
                cell.HorizontalAlignment = 1;
                table.AddCell(cell);
                var q = Quantity.GetEnumerator();
                q.MoveNext();
                foreach (string i in Items)
                {
                    long temp;
                    long.TryParse(i, out temp);
                    var RQItm = new RQItem();
                    RQItm.RFQ = requestForQuotation;
                    RQItm.RFQID = requestForQuotation.Id;
                    RQItm.ItemID = temp;
                    IEntityService<BLL.Entities.Inventory.RawMaterial> Materialsrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
                    RQItm.Material = Materialsrepository.GetById(temp);
                    table.AddCell(RQItm.Material.MaterialName);
                    int itmq = int.Parse(q.Current.ToString());
                    RQItm.Quantity = itmq;
                    IEntityService<BLL.Entities.Procurement.RQItem> RQIrepository = new EntityService<BLL.Entities.Procurement.RQItem>(db);
                    RQIrepository.Add(RQItm);
                    q.MoveNext();
                }
                doc.Add(table);
                string html = PaymentTermrepository.GetById(requestForQuotation.PaymentTermID).Description;
                TextReader reader = new StringReader(html);

                doc.Add(new Phrase("Payment Terms:  "));

                HTMLWorker worker = new HTMLWorker(doc);


                worker.StartDocument();

                worker.Parse(reader);

                worker.EndDocument();
                worker.Close();
                writer.CloseStream = false;
                doc.Close();
                IEntityService<BLL.Entities.Procurement.Supplier> Supplierrepository = new EntityService<BLL.Entities.Procurement.Supplier>(db);

                foreach (var s in Suppliers)
                {
                    long tmp;
                    long.TryParse(s, out tmp);
                    var RQSup = new RQSupplier();
                    RQSup.RFQ = requestForQuotation;
                    RQSup.RFQID = requestForQuotation.Id;
                    RQSup.SupplierID = tmp;
                    RQSup.Supplier = Supplierrepository.GetById(tmp);
                    IEntityService<BLL.Entities.Procurement.RQSupplier> RQSrepository = new EntityService<BLL.Entities.Procurement.RQSupplier>(db);
                    RQSrepository.Add(RQSup);
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                    mail.From = new MailAddress("ar27111994@gmail.com");
                    mail.To.Add(RQSup.Supplier.Email);
                    mail.Subject = "Request For Quotation";
                    mail.Body = "Request For Quotation for Raw Materials (See Attachment). Scheduled Date:" + requestForQuotation.ScheduledDate.ToShortDateString();
                    fs.Position = 0;
                    mail.Attachments.Add(new Attachment(fs,"RFQ.pdf"));
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("ar27111994@gmail.com", "MyIDIs1994");
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                }


                return Json(new { success = true });
			}

			ViewData["PaymentTermID"] = new SelectList(PaymentTermrepository.GetAll(), "Id", "Title", requestForQuotation.PaymentTermID);
			return PartialView(requestForQuotation);
		}

		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RequestForQuotation requestForQuotation = repository.GetById(id,r => r.PaymentTerm);
			if (requestForQuotation == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",requestForQuotation);
		}

		// POST: RequestForQuotations/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			RequestForQuotation requestForQuotation = repository.GetById(id);
			repository.Remove(requestForQuotation);
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
						RequestForQuotation requestForQuotation = repository.GetById(key);
						repository.Remove(requestForQuotation);
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
			var records = repository.GetAll(r => r.PaymentTerm).OrderBy("Id DESC").AsEnumerable<RequestForQuotation>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<RequestForQuotation>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "RequestForQuotation.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<RequestForQuotation>(records.AsQueryable(), "RequestForQuotation.csv");
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
