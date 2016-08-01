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
	public class ContractsController : Controller
	{
		private IEntityService<Contract> repository = null; 
		private SCMSContext db = new SCMSContext();
		// GET: Contracts
		public ContractsController()  
		   {  
				this.repository = new EntityService<Contract>(db);  
		   }  
		public ContractsController(IEntityService<Contract> repository)  
		   {  
				this.repository = repository;  
		   }  
		public ActionResult Index()
		{
		var records = new PagedList<Contract>();
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
		var records = new PagedList<Contract>();
		ViewBag.filter = filter;
		records.Content = repository.GetList(x => filter == null
									|| (x.ContractTitle.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.ContractDescription.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.ContractDate.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Renewable.ToString().ToLower().Contains(filter.ToLower()))
		)
					.OrderBy(sort + " " + sortdir)
					.Skip((page - 1) * pageSize)
					.Take(pageSize).ToList();
		// Count
		records.TotalRecords = repository.GetList(x => filter == null 
									|| (x.ContractTitle.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.ContractDescription.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.ContractDate.ToString().ToLower().Contains(filter.ToLower()))
									|| (x.Renewable.ToString().ToLower().Contains(filter.ToLower()))
		).Count();

			records.CurrentPage = page;
			records.PageSize = pageSize;
			return PartialView("Grid",records);
		}

		
		// GET: Contracts/Details/5
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Contract contract = repository.GetById(id);
			if (contract == null)
			{
				return HttpNotFound();
			}
			return PartialView("Details",contract);
		}

		// GET: Contracts/Create
		[HttpGet]
		public ActionResult Create()
		{
			Contract contract = new Contract();
			return PartialView("Create",contract);
		}

		// POST: Contracts/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,ContractTitle,ContractDescription,ContractDate,Renewable,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Contract contract)
		{
			if (ModelState.IsValid)
			{
				repository.Add(contract);
				repository.Commit();
				return Json(new { success = true });
			}

			return PartialView(contract);
		}

		// GET: Contracts/Edit/5
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Contract contract = repository.GetById(id);
			if (contract == null)
			{
				return HttpNotFound();
			}
			return PartialView("Edit",contract);
		}

		// POST: Contracts/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,ContractTitle,ContractDescription,ContractDate,Renewable,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Contract contract)
		{
			if (ModelState.IsValid)
			{
				repository.Update(contract);
				repository.Commit();
				return Json(new { success = true });
			}
			return PartialView("Edit", contract);
		}

		// GET: Contracts/Delete/5
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Contract contract = repository.GetById(id);
			if (contract == null)
			{
				return HttpNotFound();
			}
			return PartialView("Delete",contract);
		}

		// POST: Contracts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			Contract contract = repository.GetById(id);
			repository.Remove(contract);
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
						Contract contract = repository.GetById(key);
						repository.Remove(contract);
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
			var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<Contract>();
			if (model.PagingEnabled && model.Range.ToString() != "All")
			{
				records = records.Skip((model.CurrentPage - 1) * model.PageSize)
				   .Take(model.PageSize);
			}
			if (model.OutputType.Equals(Output.Excel))
			{
				var excelFormatter = new ExcelFormatter<Contract>(records);
				return new ExcelResult(excelFormatter.WriteHtmlTable(), "Contract.xls");
			}
			if (model.OutputType.Equals(Output.Csv))
			{
				return new CsvResult<Contract>(records.AsQueryable(), "Contract.csv");
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
