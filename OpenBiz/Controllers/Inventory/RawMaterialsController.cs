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
using System.IO;

namespace SCMS.Controllers.Inventory
{
	public class RawMaterialsController : Controller
	{
		private IEntityService<RawMaterial> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: RawMaterials
		public RawMaterialsController()  
		   {  
				this.repository = new EntityService<RawMaterial>(db);  
		   }  
		public RawMaterialsController(IEntityService<RawMaterial> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<RawMaterial>();
		records.Content = repository.GetAll(r => r.Supplier,r => r.UnitOfMeasurement).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(r => r.Supplier,r => r.UnitOfMeasurement).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<RawMaterial>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Supplier.SupplierName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.UnitOfMeasurement.Unit.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.MaterialName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Manufacturer.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.MaterialPrice.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Barcode.ToString().ToLower().Contains(filter.ToLower()))
		,r => r.Supplier,r => r.UnitOfMeasurement)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Supplier.SupplierName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.UnitOfMeasurement.Unit.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.MaterialName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Manufacturer.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.MaterialPrice.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Barcode.ToString().ToLower().Contains(filter.ToLower()))
		,r => r.Supplier,r => r.UnitOfMeasurement).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: RawMaterials/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RawMaterial rawMaterial = repository.GetById(id, r => r.Supplier, r => r.UnitOfMeasurement);
			if (rawMaterial == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",rawMaterial);
		}

		// GET: RawMaterials/Create
		[HttpGet]
		public ActionResult Create()
		{
			RawMaterial rawMaterial = new RawMaterial();
			IEntityService<BLL.Entities.Procurement.Supplier> Supplierrepository = new EntityService<BLL.Entities.Procurement.Supplier>(db);
			ViewData["SupplierID"] = new SelectList(Supplierrepository.GetAll(), "Id", "SupplierName");
			IEntityService<BLL.Entities.Inventory.UnitOfMeasurement> UnitOfMeasurementrepository = new EntityService<BLL.Entities.Inventory.UnitOfMeasurement>(db);
			ViewData["UnitOfMeasurementID"] = new SelectList(UnitOfMeasurementrepository.GetAll(), "Id", "Unit");
			return PartialView("Create",rawMaterial);
		}

		// POST: RawMaterials/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,MaterialName,Manufacturer,SupplierID,MaterialPrice,Barcode,UnitOfMeasurementID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] RawMaterial rawMaterial)
		{
			if (ModelState.IsValid)
			{
				if (Request.Files.Count > 0)
				{
					HttpPostedFileBase upload = Request.Files[0];

					if (upload == null)
					{
						ModelState.AddModelError("File", "Please Upload Your file");
					}
					else if (upload.ContentLength > 0)
					{
						int MaxContentLength = 1024 * 1024 * 3; //3 MB
						string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png" };

						if (!AllowedFileExtensions.Contains(upload.FileName.Substring(upload.FileName.LastIndexOf('.'))))
						{
							ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
						}

						else if (upload.ContentLength > MaxContentLength)
						{
							ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
						}
						else
						{
							var fileName = rawMaterial.MaterialName;
							var path = Path.Combine(Server.MapPath("~/Content/Images/MaterialImages/"), fileName + ".png");
							upload.SaveAs(path);
							ModelState.Clear();
							repository.Add(rawMaterial);
							repository.Commit();
							return Json(new { success = true });
						}
					}


				}
			}

			IEntityService<BLL.Entities.Procurement.Supplier> Supplierrepository = new EntityService<BLL.Entities.Procurement.Supplier>(db);
			ViewData["SupplierID"] = new SelectList(Supplierrepository.GetAll(), "Id", "SupplierName", rawMaterial.SupplierID);
			IEntityService<BLL.Entities.Inventory.UnitOfMeasurement> UnitOfMeasurementrepository = new EntityService<BLL.Entities.Inventory.UnitOfMeasurement>(db);
			ViewData["UnitOfMeasurementID"] = new SelectList(UnitOfMeasurementrepository.GetAll(), "Id", "Unit", rawMaterial.UnitOfMeasurementID);
			return PartialView(rawMaterial);
		}

		// GET: RawMaterials/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RawMaterial rawMaterial = repository.GetById(id);
			if (rawMaterial == null)
			{
				return HttpNotFound();
			}
			IEntityService<BLL.Entities.Procurement.Supplier> Supplierrepository = new EntityService<BLL.Entities.Procurement.Supplier>(db);
			ViewData["SupplierID"] = new SelectList(Supplierrepository.GetAll(), "Id", "SupplierName", rawMaterial.SupplierID);
			IEntityService<BLL.Entities.Inventory.UnitOfMeasurement> UnitOfMeasurementrepository = new EntityService<BLL.Entities.Inventory.UnitOfMeasurement>(db);
			ViewData["UnitOfMeasurementID"] = new SelectList(UnitOfMeasurementrepository.GetAll(), "Id", "Unit", rawMaterial.UnitOfMeasurementID);
			return PartialView("Edit",rawMaterial);
		}

		// POST: RawMaterials/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,MaterialName,Manufacturer,SupplierID,MaterialPrice,Barcode,UnitOfMeasurementID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] RawMaterial rawMaterial)
		{
			if (ModelState.IsValid)
			{
				if (Request.Files.Count > 0)
				{
					HttpPostedFileBase upload = Request.Files[0];

					if (upload == null)
					{
						ModelState.AddModelError("File", "Please Upload Your file");
					}
					else if (upload.ContentLength > 0)
					{
						int MaxContentLength = 1024 * 1024 * 3; //3 MB
						string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png" };

						if (!AllowedFileExtensions.Contains(upload.FileName.Substring(upload.FileName.LastIndexOf('.'))))
						{
							ModelState.AddModelError("File", "Please file of type: " + string.Join(", ", AllowedFileExtensions));
						}

						else if (upload.ContentLength > MaxContentLength)
						{
							ModelState.AddModelError("File", "Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
						}
						else
						{
							var fileName = rawMaterial.MaterialName;
							var path = Path.Combine(Server.MapPath("~/Content/Images/MaterialImages/"), fileName + ".png");
							upload.SaveAs(path);
							ModelState.Clear();
						}
					}


				}
				repository.Update(rawMaterial);
				repository.Commit();
				return Json(new { success = true });
			}
			IEntityService<BLL.Entities.Procurement.Supplier> Supplierrepository = new EntityService<BLL.Entities.Procurement.Supplier>(db);
			ViewData["SupplierID"] = new SelectList(Supplierrepository.GetAll(), "Id", "SupplierName", rawMaterial.SupplierID);
			IEntityService<BLL.Entities.Inventory.UnitOfMeasurement> UnitOfMeasurementrepository = new EntityService<BLL.Entities.Inventory.UnitOfMeasurement>(db);
			ViewData["UnitOfMeasurementID"] = new SelectList(UnitOfMeasurementrepository.GetAll(), "Id", "Unit", rawMaterial.UnitOfMeasurementID);
			return PartialView("Edit", rawMaterial);
		}

		// GET: RawMaterials/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RawMaterial rawMaterial = repository.GetById(id, r => r.Supplier, r => r.UnitOfMeasurement);
			if (rawMaterial == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",rawMaterial);
		}

		// POST: RawMaterials/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			RawMaterial rawMaterial = repository.GetById(id);
			repository.Remove(rawMaterial);
			System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Images/MaterialImages/"), rawMaterial.MaterialName + ".png"));
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
						RawMaterial rawMaterial = repository.GetById(key);
						repository.Remove(rawMaterial);
						System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/Images/MaterialImages/"), rawMaterial.MaterialName + ".png"));

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
			var records = repository.GetAll(r => r.Supplier,r => r.UnitOfMeasurement).OrderBy("Id DESC").AsEnumerable<RawMaterial>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<RawMaterial>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "RawMaterial.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<RawMaterial>(records.AsQueryable(), "RawMaterial.csv");
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
