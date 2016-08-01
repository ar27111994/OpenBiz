

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
	public class AttributeValuesController : Controller
	{
		private IEntityService<AttributeValue> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: AttributeValues
		public AttributeValuesController()  
		   {  
				this.repository = new EntityService<AttributeValue>(db);  
		   }  
		public AttributeValuesController(IEntityService<AttributeValue> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<AttributeValue>();
		records.Content = repository.GetAll(a => a.Attribute).OrderBy("Id DESC")
						.Take(20).ToList();

		// Count
		records.TotalRecords = repository.GetAll(a => a.Attribute).Count();

			records.CurrentPage = 1;
			records.PageSize = 20;
			return View(records);
		}


		
		[HttpGet]
		public ActionResult Grid(string filter = null, int page = 1, 
		 int pageSize = 20, string sort = "Id", string sortdir = "DESC")
		{
		var records = new PagedList<AttributeValue>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.Attribute.Name.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Value.ToString().ToLower().Contains(filter.ToLower()))
		,a => a.Attribute)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();

		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.Attribute.Name.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Value.ToString().ToLower().Contains(filter.ToLower()))
		,a => a.Attribute).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: AttributeValues/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AttributeValue attributeValue = repository.GetById(id, a => a.Attribute);
			if (attributeValue == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",attributeValue);
		}

		// GET: AttributeValues/Create
		[HttpGet]
		public ActionResult Create()
		{
			AttributeValue attributeValue = new AttributeValue();
			IEntityService<BLL.Entities.Inventory.Attribute> Attributerepository = new EntityService<BLL.Entities.Inventory.Attribute>(db);
			ViewData["AttributeID"] = new SelectList(Attributerepository.GetAll(), "Id", "Name");
			return PartialView("Create",attributeValue);
		}

		// POST: AttributeValues/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,Value,AttributeID")] AttributeValue attributeValue)
		{
			if (ModelState.IsValid)
			{
				repository.Add(attributeValue);
				repository.Commit();
				return Json(new { success = true });
			}

			IEntityService<BLL.Entities.Inventory.Attribute> Attributerepository = new EntityService<BLL.Entities.Inventory.Attribute>(db);
			ViewData["AttributeID"] = new SelectList(Attributerepository.GetAll(), "Id", "Name", attributeValue.AttributeID);
			return PartialView(attributeValue);
		}

		// GET: AttributeValues/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AttributeValue attributeValue = repository.GetById(id);
			if (attributeValue == null)
			{
				return HttpNotFound();
			}
			IEntityService<BLL.Entities.Inventory.Attribute> Attributerepository = new EntityService<BLL.Entities.Inventory.Attribute>(db);
			ViewData["AttributeID"] = new SelectList(Attributerepository.GetAll(), "Id", "Name", attributeValue.AttributeID);
			return PartialView("Edit",attributeValue);
		}

		// POST: AttributeValues/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,Value,AttributeID")] AttributeValue attributeValue)
		{
			if (ModelState.IsValid)
			{
				repository.Update(attributeValue);
				repository.Commit();
				return Json(new { success = true });
			}
			IEntityService<BLL.Entities.Inventory.Attribute> Attributerepository = new EntityService<BLL.Entities.Inventory.Attribute>(db);
			ViewData["AttributeID"] = new SelectList(Attributerepository.GetAll(), "Id", "Name", attributeValue.AttributeID);
			return PartialView("Edit", attributeValue);
		}

		// GET: AttributeValues/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AttributeValue attributeValue = repository.GetById(id, a => a.Attribute);
			if (attributeValue == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",attributeValue);
		}

		// POST: AttributeValues/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			AttributeValue attributeValue = repository.GetById(id);
			repository.Remove(attributeValue);
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
						AttributeValue attributeValue = repository.GetById(key);
						repository.Remove(attributeValue);
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
			var records = repository.GetAll(a => a.Attribute).OrderBy("Id DESC").AsEnumerable<AttributeValue>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<AttributeValue>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "AttributeValue.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<AttributeValue>(records.AsQueryable(), "AttributeValue.csv");
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
