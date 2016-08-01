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
using BLL.Entities.DistOrders;
using DAL;

namespace SCMS.Controllers.Orders
{
	public class OrdersController : Controller
	{
		private IEntityService<Order> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: Orders
		public OrdersController()  
		   {  
				this.repository = new EntityService<Order>(db);  
		   }  
		public OrdersController(IEntityService<Order> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<Order>();
		records.Content = repository.GetAll(o => o.Distributor).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(o => o.Distributor).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<Order>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Distributor.Name.ToString().ToLower().Contains(filter.ToLower()))
		,o => o.Distributor)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Distributor.Name.ToString().ToLower().Contains(filter.ToLower()))
		,o => o.Distributor).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: Orders/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Order order = repository.GetById(id,o => o.Distributor);
			if (order == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",order);
		}

		// GET: Orders/Create
		[HttpGet]
		public ActionResult Create()
		{
			Order order = new Order();
			IEntityService<BLL.Entities.Distribution.Distributor> Distributorrepository = new EntityService<BLL.Entities.Distribution.Distributor>(db);
			ViewData["DistributorID"] = new SelectList(Distributorrepository.GetAll(), "Id", "Name");
			return PartialView("Create",order);
		}

		// POST: Orders/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,DistributorID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Order order)
		{
			if (ModelState.IsValid)
			{
				repository.Add(order);
				repository.Commit();
				return Json(new { success = true });
			}

			IEntityService<BLL.Entities.Distribution.Distributor> Distributorrepository = new EntityService<BLL.Entities.Distribution.Distributor>(db);
			ViewData["DistributorID"] = new SelectList(Distributorrepository.GetAll(), "Id", "Name", order.DistributorID);
			return PartialView(order);
		}

		// GET: Orders/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Order order = repository.GetById(id);
			if (order == null)
			{
				return HttpNotFound();
			}
			IEntityService<BLL.Entities.Distribution.Distributor> Distributorrepository = new EntityService<BLL.Entities.Distribution.Distributor>(db);
			ViewData["DistributorID"] = new SelectList(Distributorrepository.GetAll(), "Id", "Name", order.DistributorID);
			return PartialView("Edit",order);
		}

		// POST: Orders/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,DistributorID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Order order)
		{
			if (ModelState.IsValid)
			{
				repository.Update(order);
				repository.Commit();
				return Json(new { success = true });
			}
			IEntityService<BLL.Entities.Distribution.Distributor> Distributorrepository = new EntityService<BLL.Entities.Distribution.Distributor>(db);
			ViewData["DistributorID"] = new SelectList(Distributorrepository.GetAll(), "Id", "Name", order.DistributorID);
			return PartialView("Edit", order);
		}

		// GET: Orders/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Order order = repository.GetById(id,o => o.Distributor);
			if (order == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",order);
		}

		// POST: Orders/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			Order order = repository.GetById(id);
			repository.Remove(order);
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
						Order order = repository.GetById(key);
						repository.Remove(order);
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
			var records = repository.GetAll(o => o.Distributor).OrderBy("Id DESC").AsEnumerable<Order>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<Order>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "Order.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<Order>(records.AsQueryable(), "Order.csv");
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
