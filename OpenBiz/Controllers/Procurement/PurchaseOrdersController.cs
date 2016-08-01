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
	public class PurchaseOrdersController : Controller
	{
		private IEntityService<PurchaseOrder> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: PurchaseOrders
		public PurchaseOrdersController()  
		   {  
				this.repository = new EntityService<PurchaseOrder>(db);  
		   }  
		public PurchaseOrdersController(IEntityService<PurchaseOrder> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<PurchaseOrder>();
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
		var records = new PagedList<PurchaseOrder>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.QuoteID.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.OrderDate.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.WarehouseID.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.QuoteID.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.OrderDate.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.WarehouseID.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: PurchaseOrders/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			PurchaseOrder purchaseOrder = repository.GetById(id);
			if (purchaseOrder == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",purchaseOrder);
		}

		// GET: PurchaseOrders/Create
		[HttpGet]
		public ActionResult Create()
		{
			PurchaseOrder purchaseOrder = new PurchaseOrder();
			return PartialView("Create",purchaseOrder);
		}

		// POST: PurchaseOrders/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,QuoteID,OrderDate,WarehouseID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] PurchaseOrder purchaseOrder)
		{
			if (ModelState.IsValid)
			{
				repository.Add(purchaseOrder);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(purchaseOrder);
		}

		// GET: PurchaseOrders/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			PurchaseOrder purchaseOrder = repository.GetById(id);
			if (purchaseOrder == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",purchaseOrder);
		}

		// POST: PurchaseOrders/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,QuoteID,OrderDate,WarehouseID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] PurchaseOrder purchaseOrder)
		{
			if (ModelState.IsValid)
			{
				repository.Update(purchaseOrder);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", purchaseOrder);
		}

		// GET: PurchaseOrders/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			PurchaseOrder purchaseOrder = repository.GetById(id);
			if (purchaseOrder == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",purchaseOrder);
		}

		// POST: PurchaseOrders/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			PurchaseOrder purchaseOrder = repository.GetById(id);
			repository.Remove(purchaseOrder);
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
						PurchaseOrder purchaseOrder = repository.GetById(key);
						repository.Remove(purchaseOrder);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<PurchaseOrder>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<PurchaseOrder>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "PurchaseOrder.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<PurchaseOrder>(records.AsQueryable(), "PurchaseOrder.csv");
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
