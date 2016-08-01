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
using BLL.Entities.Transportation;
using DAL;

namespace SCMS.Controllers.Transportation
{
	public class ShippingSlipsController : Controller
	{
		private IEntityService<ShippingSlip> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: ShippingSlips
		public ShippingSlipsController()  
		   {  
				this.repository = new EntityService<ShippingSlip>(db);  
		   }  
		public ShippingSlipsController(IEntityService<ShippingSlip> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<ShippingSlip>();
		records.Content = repository.GetAll(s => s.Order).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(s => s.Order).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<ShippingSlip>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Order.CreatedBy.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Title.ToString().ToLower().Contains(filter.ToLower()))
		,s => s.Order)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Order.CreatedBy.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Title.ToString().ToLower().Contains(filter.ToLower()))
		,s => s.Order).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: ShippingSlips/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ShippingSlip shippingSlip = repository.GetById(id,s => s.Order);
			if (shippingSlip == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",shippingSlip);
		}

		// GET: ShippingSlips/Create
		[HttpGet]
		public ActionResult Create()
		{
			ShippingSlip shippingSlip = new ShippingSlip();
			IEntityService<BLL.Entities.DistOrders.Order> Orderrepository = new EntityService<BLL.Entities.DistOrders.Order>(db);
			ViewData["OrderID"] = new SelectList(Orderrepository.GetAll(), "Id", "CreatedBy");
			return PartialView("Create",shippingSlip);
		}

		// POST: ShippingSlips/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,Title,OrderID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] ShippingSlip shippingSlip)
		{
			if (ModelState.IsValid)
			{
				repository.Add(shippingSlip);
				repository.Commit();
				return Json(new { success = true });
			}

			IEntityService<BLL.Entities.DistOrders.Order> Orderrepository = new EntityService<BLL.Entities.DistOrders.Order>(db);
			ViewData["OrderID"] = new SelectList(Orderrepository.GetAll(), "Id", "CreatedBy", shippingSlip.OrderID);
			return PartialView(shippingSlip);
		}

		// GET: ShippingSlips/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ShippingSlip shippingSlip = repository.GetById(id);
			if (shippingSlip == null)
			{
				return HttpNotFound();
			}
			IEntityService<BLL.Entities.DistOrders.Order> Orderrepository = new EntityService<BLL.Entities.DistOrders.Order>(db);
			ViewData["OrderID"] = new SelectList(Orderrepository.GetAll(), "Id", "CreatedBy", shippingSlip.OrderID);
			return PartialView("Edit",shippingSlip);
		}

		// POST: ShippingSlips/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Title,OrderID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] ShippingSlip shippingSlip)
		{
			if (ModelState.IsValid)
			{
				repository.Update(shippingSlip);
				repository.Commit();
				return Json(new { success = true });
			}
			IEntityService<BLL.Entities.DistOrders.Order> Orderrepository = new EntityService<BLL.Entities.DistOrders.Order>(db);
			ViewData["OrderID"] = new SelectList(Orderrepository.GetAll(), "Id", "CreatedBy", shippingSlip.OrderID);
			return PartialView("Edit", shippingSlip);
		}

		// GET: ShippingSlips/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ShippingSlip shippingSlip = repository.GetById(id,s => s.Order);
			if (shippingSlip == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",shippingSlip);
		}

		// POST: ShippingSlips/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			ShippingSlip shippingSlip = repository.GetById(id);
			repository.Remove(shippingSlip);
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
						ShippingSlip shippingSlip = repository.GetById(key);
						repository.Remove(shippingSlip);
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
			var records = repository.GetAll(s => s.Order).OrderBy("Id DESC").AsEnumerable<ShippingSlip>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<ShippingSlip>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "ShippingSlip.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<ShippingSlip>(records.AsQueryable(), "ShippingSlip.csv");
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
