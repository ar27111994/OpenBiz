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

namespace SCMS.Procurement.Controllers
{
	public class AddressTypesController : Controller
	{
		private IEntityService<AddressType> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: AddressTypes
		public AddressTypesController()  
		   {  
				this.repository = new EntityService<AddressType>(db);  
		   }  
		public AddressTypesController(IEntityService<AddressType> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<AddressType>();
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
		var records = new PagedList<AddressType>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.AddressTypeName.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.AddressTypeName.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: AddressTypes/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AddressType addressType = repository.GetById(id);
			if (addressType == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",addressType);
		}

		// GET: AddressTypes/Create
		[HttpGet]
		public ActionResult Create()
		{
			AddressType addressType = new AddressType();
			return PartialView("Create",addressType);
		}

		// POST: AddressTypes/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,AddressTypeName")] AddressType addressType)
		{
			if (ModelState.IsValid)
			{
				repository.Add(addressType);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(addressType);
		}

		// GET: AddressTypes/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AddressType addressType = repository.GetById(id);
			if (addressType == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",addressType);
		}

		// POST: AddressTypes/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,AddressTypeName")] AddressType addressType)
		{
			if (ModelState.IsValid)
			{
				repository.Update(addressType);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", addressType);
		}

		// GET: AddressTypes/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AddressType addressType = repository.GetById(id);
			if (addressType == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",addressType);
		}

		// POST: AddressTypes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			AddressType addressType = repository.GetById(id);
			repository.Remove(addressType);
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
						AddressType addressType = repository.GetById(key);
						repository.Remove(addressType);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<AddressType>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<AddressType>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "AddressType.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<AddressType>(records.AsQueryable(), "AddressType.csv");
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
