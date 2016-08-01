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
	public class SupplierAddressesController : Controller
	{
		private IEntityService<SupplierAddress> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: SupplierAddresses
		public SupplierAddressesController()  
		   {  
				this.repository = new EntityService<SupplierAddress>(db);  
		   }  
		public SupplierAddressesController(IEntityService<SupplierAddress> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<SupplierAddress>();
		records.Content = repository.GetAll(s => s.AddressType,s => s.Supplier).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(s => s.AddressType,s => s.Supplier).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<SupplierAddress>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.AddressType.AddressTypeName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Supplier.SupplierName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Address.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.PhoneNo.ToString().ToLower().Contains(filter.ToLower()))
		,s => s.AddressType,s => s.Supplier)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.AddressType.AddressTypeName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Supplier.SupplierName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Address.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.PhoneNo.ToString().ToLower().Contains(filter.ToLower()))
		,s => s.AddressType,s => s.Supplier).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: SupplierAddresses/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SupplierAddress supplierAddress = repository.GetById(id,s => s.AddressType,s => s.Supplier);
			if (supplierAddress == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",supplierAddress);
		}

		// GET: SupplierAddresses/Create
		[HttpGet]
		public ActionResult Create()
		{
			SupplierAddress supplierAddress = new SupplierAddress();
			IEntityService<BLL.Entities.Procurement.AddressType> AddressTyperepository = new EntityService<BLL.Entities.Procurement.AddressType>(db);
			ViewData["AddressTypeID"] = new SelectList(AddressTyperepository.GetAll(), "Id", "AddressTypeName");
			IEntityService<BLL.Entities.Procurement.Supplier> Supplierrepository = new EntityService<BLL.Entities.Procurement.Supplier>(db);
			ViewData["SupplierID"] = new SelectList(Supplierrepository.GetAll(), "Id", "SupplierName");
			return PartialView("Create",supplierAddress);
		}

		// POST: SupplierAddresses/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,Address,Latitude,Longitude,AddressTypeID,PhoneNo,SupplierID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] SupplierAddress supplierAddress)
		{
			if (ModelState.IsValid)
			{
				repository.Add(supplierAddress);
				repository.Commit();
				return Json(new { success = true });
			}

			IEntityService<BLL.Entities.Procurement.AddressType> AddressTyperepository = new EntityService<BLL.Entities.Procurement.AddressType>(db);
			ViewData["AddressTypeID"] = new SelectList(AddressTyperepository.GetAll(), "Id", "AddressTypeName", supplierAddress.AddressTypeID);
			IEntityService<BLL.Entities.Procurement.Supplier> Supplierrepository = new EntityService<BLL.Entities.Procurement.Supplier>(db);
			ViewData["SupplierID"] = new SelectList(Supplierrepository.GetAll(), "Id", "SupplierName", supplierAddress.SupplierID);
			return PartialView(supplierAddress);
		}

		// GET: SupplierAddresses/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SupplierAddress supplierAddress = repository.GetById(id);
			if (supplierAddress == null)
			{
				return HttpNotFound();
			}
			IEntityService<BLL.Entities.Procurement.AddressType> AddressTyperepository = new EntityService<BLL.Entities.Procurement.AddressType>(db);
			ViewData["AddressTypeID"] = new SelectList(AddressTyperepository.GetAll(), "Id", "AddressTypeName", supplierAddress.AddressTypeID);
			IEntityService<BLL.Entities.Procurement.Supplier> Supplierrepository = new EntityService<BLL.Entities.Procurement.Supplier>(db);
			ViewData["SupplierID"] = new SelectList(Supplierrepository.GetAll(), "Id", "SupplierName", supplierAddress.SupplierID);
			return PartialView("Edit",supplierAddress);
		}

		// POST: SupplierAddresses/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Address,Latitude,Longitude,AddressTypeID,PhoneNo,SupplierID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] SupplierAddress supplierAddress)
		{
			if (ModelState.IsValid)
			{
				repository.Update(supplierAddress);
				repository.Commit();
				return Json(new { success = true });
			}
			IEntityService<BLL.Entities.Procurement.AddressType> AddressTyperepository = new EntityService<BLL.Entities.Procurement.AddressType>(db);
			ViewData["AddressTypeID"] = new SelectList(AddressTyperepository.GetAll(), "Id", "AddressTypeName", supplierAddress.AddressTypeID);
			IEntityService<BLL.Entities.Procurement.Supplier> Supplierrepository = new EntityService<BLL.Entities.Procurement.Supplier>(db);
			ViewData["SupplierID"] = new SelectList(Supplierrepository.GetAll(), "Id", "SupplierName", supplierAddress.SupplierID);
			return PartialView("Edit", supplierAddress);
		}

		// GET: SupplierAddresses/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SupplierAddress supplierAddress = repository.GetById(id,s => s.AddressType,s => s.Supplier);
			if (supplierAddress == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",supplierAddress);
		}

		// POST: SupplierAddresses/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			SupplierAddress supplierAddress = repository.GetById(id);
			repository.Remove(supplierAddress);
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
						SupplierAddress supplierAddress = repository.GetById(key);
						repository.Remove(supplierAddress);
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
			var records = repository.GetAll(s => s.AddressType,s => s.Supplier).OrderBy("Id DESC").AsEnumerable<SupplierAddress>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<SupplierAddress>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "SupplierAddress.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<SupplierAddress>(records.AsQueryable(), "SupplierAddress.csv");
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
