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
	public class SuppliersController : Controller
	{
		private IEntityService<Supplier> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: Suppliers
		public SuppliersController()  
		   {  
				this.repository = new EntityService<Supplier>(db);  
		   }  
		public SuppliersController(IEntityService<Supplier> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<Supplier>();
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
		var records = new PagedList<Supplier>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.SupplierName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.SupplierDetails.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Email.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.SupplierName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.SupplierDetails.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Email.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: Suppliers/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Supplier supplier = repository.GetById(id);
			if (supplier == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",supplier);
		}

		// GET: Suppliers/Create
		[HttpGet]
		public ActionResult Create()
		{
			Supplier supplier = new Supplier();
			return PartialView("Create",supplier);
		}

		// POST: Suppliers/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,SupplierName,SupplierDetails,Email,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Supplier supplier)
		{
			if (ModelState.IsValid)
			{
				repository.Add(supplier);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(supplier);
		}

		// GET: Suppliers/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Supplier supplier = repository.GetById(id);
			if (supplier == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",supplier);
		}

		// POST: Suppliers/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,SupplierName,SupplierDetails,Email,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Supplier supplier)
		{
			if (ModelState.IsValid)
			{
				repository.Update(supplier);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", supplier);
		}

		// GET: Suppliers/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Supplier supplier = repository.GetById(id);
			if (supplier == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",supplier);
		}

		// POST: Suppliers/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			Supplier supplier = repository.GetById(id);
			repository.Remove(supplier);
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
						Supplier supplier = repository.GetById(key);
						repository.Remove(supplier);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<Supplier>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<Supplier>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "Supplier.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<Supplier>(records.AsQueryable(), "Supplier.csv");
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
