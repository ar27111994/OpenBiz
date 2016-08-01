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
using BLL.Entities.Payment;
using DAL;

namespace SCMS.Controllers.Payment
{
    public class AccountEntriesController : Controller
    {
        private IEntityService<AccountEntry> repository = null;
        private SCMSContext db = new SCMSContext();
        // GET: AccountEntries
        public AccountEntriesController()
        {
            this.repository = new EntityService<AccountEntry>(db);
        }
        public AccountEntriesController(IEntityService<AccountEntry> repository)
        {
            this.repository = repository;
        }
        public ActionResult Index()
        {
            var records = new PagedList<AccountEntry>();
            records.Content = repository.GetAll(a => a.Account).OrderBy("Id DESC")
                            .Take(20).ToList();

            // Count
            records.TotalRecords = repository.GetAll(a => a.Account).Count();

            records.CurrentPage = 1;
            records.PageSize = 20;
            return View(records);
        }



        [HttpGet]
        public ActionResult Grid(string filter = null, int page = 1,
         int pageSize = 20, string sort = "Id", string sortdir = "DESC")
        {
            var records = new PagedList<AccountEntry>();
            ViewBag.filter = filter;
            records.Content = repository.GetList(x => filter == null
                                        || (x.Account.Name.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Amount.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.EntryType.ToString().ToLower().Contains(filter.ToLower()))
            , a => a.Account)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize).ToList();

            // Count
            records.TotalRecords = repository.GetList(x => filter == null
                                        || (x.Account.Name.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Amount.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.EntryType.ToString().ToLower().Contains(filter.ToLower()))
            , a => a.Account).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            return PartialView("Grid", records);
        }


        // GET: AccountEntries/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountEntry accountEntry = repository.GetById(id, a => a.Account);
            if (accountEntry == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", accountEntry);
        }

        // GET: AccountEntries/Create
        [HttpGet]
        public ActionResult Create()
        {
            AccountEntry accountEntry = new AccountEntry();
            IEntityService<BLL.Entities.Payment.Account> Accountrepository = new EntityService<BLL.Entities.Payment.Account>(db);
            ViewData["AccountID"] = new SelectList(Accountrepository.GetAll(), "Id", "Name");
            return PartialView("Create", accountEntry);
        }

        // POST: AccountEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Amount,EntryType,AccountID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] AccountEntry accountEntry)
        {
            if (ModelState.IsValid)
            {
                repository.Add(accountEntry);
                repository.Commit();
                return Json(new { success = true });
            }

            IEntityService<BLL.Entities.Payment.Account> Accountrepository = new EntityService<BLL.Entities.Payment.Account>(db);
            ViewData["AccountID"] = new SelectList(Accountrepository.GetAll(), "Id", "Name", accountEntry.AccountID);
            return PartialView(accountEntry);
        }

        // GET: AccountEntries/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountEntry accountEntry = repository.GetById(id);
            if (accountEntry == null)
            {
                return HttpNotFound();
            }
            IEntityService<BLL.Entities.Payment.Account> Accountrepository = new EntityService<BLL.Entities.Payment.Account>(db);
            ViewData["AccountID"] = new SelectList(Accountrepository.GetAll(), "Id", "Name", accountEntry.AccountID);
            return PartialView("Edit", accountEntry);
        }

        // POST: AccountEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Amount,EntryType,AccountID,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] AccountEntry accountEntry)
        {
            if (ModelState.IsValid)
            {
                repository.Update(accountEntry);
                repository.Commit();
                return Json(new { success = true });
            }
            IEntityService<BLL.Entities.Payment.Account> Accountrepository = new EntityService<BLL.Entities.Payment.Account>(db);
            ViewData["AccountID"] = new SelectList(Accountrepository.GetAll(), "Id", "Name", accountEntry.AccountID);
            return PartialView("Edit", accountEntry);
        }

        // GET: AccountEntries/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountEntry accountEntry = repository.GetById(id, a => a.Account);
            if (accountEntry == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", accountEntry);
        }

        // POST: AccountEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            AccountEntry accountEntry = repository.GetById(id);
            repository.Remove(accountEntry);
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
                        AccountEntry accountEntry = repository.GetById(key);
                        repository.Remove(accountEntry);
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
            var records = repository.GetAll(a => a.Account).OrderBy("Id DESC").AsEnumerable<AccountEntry>();
            if (model.PagingEnabled && model.Range.ToString() != "All")
            {
                records = records.Skip((model.CurrentPage - 1) * model.PageSize)
                   .Take(model.PageSize);
            }
            if (model.OutputType.Equals(Output.Excel))
            {
                var excelFormatter = new ExcelFormatter<AccountEntry>(records);
                return new ExcelResult(excelFormatter.WriteHtmlTable(), "AccountEntry.xls");
            }
            if (model.OutputType.Equals(Output.Csv))
            {
                return new CsvResult<AccountEntry>(records.AsQueryable(), "AccountEntry.csv");
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
