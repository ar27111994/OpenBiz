using System;
using System.Linq.Dynamic;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using DAL.Repository.Persistence;
using System.Web;
using System.Web.Mvc;
using BLL.Entities.Inventory;
using DAL;
using SCMS.ViewModels;
using System.Text;
using SCMS.DataExport;
using SCMS.Actions;

namespace SCMS.Controllers.Inventory
{
    public class BrandsController : Controller
    {
        private IEntityService<Brand> repository = null;
        private SCMSContext db = new SCMSContext();
        // GET: Brands
        public BrandsController()
        {
            this.repository = new EntityService<Brand>(db);
        }
        public BrandsController(IEntityService<Brand> repository)
        {
            this.repository = repository;
        }

        public ActionResult Index()
        {
            var records = new PagedList<Brand>();
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
            var records = new PagedList<Brand>();
            ViewBag.filter = filter;
            records.Content = repository.GetList(x => filter == null

                                || (x.BrandName.ToString().ToLower().Contains(filter.ToLower()))
            )
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize).ToList();

            // Count
            records.TotalRecords = repository.GetList(x => filter == null

                                || (x.BrandName.ToString().ToLower().Contains(filter.ToLower()))
            ).Count();



            records.CurrentPage = page;
            records.PageSize = pageSize;
            return PartialView("Grid",records);
        }

        // GET: Brands/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = repository.GetById(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", brand);
        }

        // GET: Brands/Create
        [HttpGet]
        public ActionResult Create()
        {
            Brand brand = new Brand();
            return PartialView("Create", brand);
        }

        // POST: Brands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BrandName,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                repository.Add(brand);
                repository.Commit();
                return Json(new { success = true });
            }
            return PartialView("Create", brand);
        }

        // GET: Brands/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = repository.GetById(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", brand);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BrandName,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                repository.Update(brand);
                repository.Commit();
                return Json(new { success = true });
            }
            return PartialView("Edit", brand);
        }

        // GET: Brands/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = repository.GetById(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Brand brand = repository.GetById(id);
            repository.Remove(brand);
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
                        Brand brand = repository.GetById(key);
                        repository.Remove(brand);
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
            var records = repository.GetAll().OrderBy("Id DESC").AsEnumerable<Brand>();
            if (model.PagingEnabled && model.Range.ToString() != "All")
            {
                records = records.Skip((model.CurrentPage - 1) * model.PageSize)
                   .Take(model.PageSize);
            }
            if (model.OutputType.Equals(Output.Excel))
            {
                var excelFormatter = new ExcelFormatter<Brand>(records);
                return new ExcelResult(excelFormatter.WriteHtmlTable(), "Brands.xls");
            }
            if (model.OutputType.Equals(Output.Csv))
            {
                return new CsvResult<Brand>(records.AsQueryable(), "Brands.csv");
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
