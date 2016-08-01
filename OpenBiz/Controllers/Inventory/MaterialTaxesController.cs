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
	public class MaterialTaxesController : Controller
	{
		private IEntityService<MaterialTaxes> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: MaterialTaxes
		public MaterialTaxesController()  
		   {  
				this.repository = new EntityService<MaterialTaxes>(db);  
		   }  
		public MaterialTaxesController(IEntityService<MaterialTaxes> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<MaterialTaxes>();
		records.Content = repository.GetAll(m => m.Material,m => m.Tax).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(m => m.Material,m => m.Tax).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<MaterialTaxes>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Material.MaterialName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Tax.Name.ToString().ToLower().Contains(filter.ToLower()))
		,m => m.Material,m => m.Tax)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Material.MaterialName.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Tax.Name.ToString().ToLower().Contains(filter.ToLower()))
		,m => m.Material,m => m.Tax).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: MaterialTaxes/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			MaterialTaxes materialTaxes = repository.GetById(id,m => m.Material,m => m.Tax);
			if (materialTaxes == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",materialTaxes);
		}

		// GET: MaterialTaxes/Create
		[HttpGet]
		public ActionResult Create()
		{
			MaterialTaxes materialTaxes = new MaterialTaxes();
			IEntityService<BLL.Entities.Inventory.RawMaterial> RawMaterialrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
			ViewData["MaterialID"] = new SelectList(RawMaterialrepository.GetAll(), "Id", "MaterialName");
			IEntityService<BLL.Entities.Inventory.Tax> Taxrepository = new EntityService<BLL.Entities.Inventory.Tax>(db);
			ViewData["TaxID"] = new SelectList(Taxrepository.GetAll(), "Id", "Name");
			return PartialView("Create",materialTaxes);
		}

		// POST: MaterialTaxes/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "MaterialID,TaxID,Id,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] MaterialTaxes materialTaxes)
		{
			if (ModelState.IsValid)
			{
				repository.Add(materialTaxes);
				repository.Commit();
				return Json(new { success = true });
			}

			IEntityService<BLL.Entities.Inventory.RawMaterial> RawMaterialrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
			ViewData["MaterialID"] = new SelectList(RawMaterialrepository.GetAll(), "Id", "MaterialName", materialTaxes.MaterialID);
			IEntityService<BLL.Entities.Inventory.Tax> Taxrepository = new EntityService<BLL.Entities.Inventory.Tax>(db);
			ViewData["TaxID"] = new SelectList(Taxrepository.GetAll(), "Id", "Name", materialTaxes.TaxID);
			return PartialView(materialTaxes);
		}

		// GET: MaterialTaxes/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			MaterialTaxes materialTaxes = repository.GetById(id);
			if (materialTaxes == null)
			{
				return HttpNotFound();
			}
			IEntityService<BLL.Entities.Inventory.RawMaterial> RawMaterialrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
			ViewData["MaterialID"] = new SelectList(RawMaterialrepository.GetAll(), "Id", "MaterialName", materialTaxes.MaterialID);
			IEntityService<BLL.Entities.Inventory.Tax> Taxrepository = new EntityService<BLL.Entities.Inventory.Tax>(db);
			ViewData["TaxID"] = new SelectList(Taxrepository.GetAll(), "Id", "Name", materialTaxes.TaxID);
			return PartialView("Edit",materialTaxes);
		}

		// POST: MaterialTaxes/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "MaterialID,TaxID,Id,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] MaterialTaxes materialTaxes)
		{
			if (ModelState.IsValid)
			{
				repository.Update(materialTaxes);
				repository.Commit();
				return Json(new { success = true });
			}
			IEntityService<BLL.Entities.Inventory.RawMaterial> RawMaterialrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
			ViewData["MaterialID"] = new SelectList(RawMaterialrepository.GetAll(), "Id", "MaterialName", materialTaxes.MaterialID);
			IEntityService<BLL.Entities.Inventory.Tax> Taxrepository = new EntityService<BLL.Entities.Inventory.Tax>(db);
			ViewData["TaxID"] = new SelectList(Taxrepository.GetAll(), "Id", "Name", materialTaxes.TaxID);
			return PartialView("Edit", materialTaxes);
		}

		// GET: MaterialTaxes/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			MaterialTaxes materialTaxes = repository.GetById(id,m => m.Material,m => m.Tax);
			if (materialTaxes == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",materialTaxes);
		}

		// POST: MaterialTaxes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			MaterialTaxes materialTaxes = repository.GetById(id);
			repository.Remove(materialTaxes);
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
						MaterialTaxes materialTaxes = repository.GetById(key);
						repository.Remove(materialTaxes);
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
			var records = repository.GetAll(m => m.Material,m => m.Tax).OrderBy("Id DESC").AsEnumerable<MaterialTaxes>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<MaterialTaxes>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "MaterialTaxes.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<MaterialTaxes>(records.AsQueryable(), "MaterialTaxes.csv");
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
