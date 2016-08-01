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
using BLL.Entities.Warehouse;
using DAL;
using Newtonsoft.Json;

namespace SCMS.Controllers.Warehouses
{
    public class WarehouseMaterialsController : Controller
    {
        private IEntityService<WarehouseMaterials> repository = null;
        private SCMSContext db = new SCMSContext();
        // GET: WarehouseMaterials
        public WarehouseMaterialsController()
        {
            this.repository = new EntityService<WarehouseMaterials>(db);
        }
        public WarehouseMaterialsController(IEntityService<WarehouseMaterials> repository)
        {
            this.repository = repository;
        }
        public ActionResult Index()
        {
            var records = new PagedList<WarehouseMaterials>();
            records.Content = repository.GetAll(w => w.RawMaterial, w => w.Warehouse).OrderBy("Id DESC")
                            .Take(20).ToList();

            // Count
            records.TotalRecords = repository.GetAll(w => w.RawMaterial, w => w.Warehouse).Count();

            records.CurrentPage = 1;
            records.PageSize = 20;
            return View(records);
        }



        [HttpGet]
        public ActionResult Grid(string filter = null, int page = 1,
         int pageSize = 20, string sort = "Id", string sortdir = "DESC")
        {
            var records = new PagedList<WarehouseMaterials>();
            ViewBag.filter = filter;
            records.Content = repository.GetList(x => filter == null
                                        || (x.RawMaterial.MaterialName.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Warehouse.WarehouseName.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.EntryTitle.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Purpose.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Quantity.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.PostingTime.ToString().ToLower().Contains(filter.ToLower()))
            , w => w.RawMaterial, w => w.Warehouse)
                        .OrderBy(sort + " " + sortdir)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize).ToList();

            // Count
            records.TotalRecords = repository.GetList(x => filter == null
                                        || (x.RawMaterial.MaterialName.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Warehouse.WarehouseName.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.EntryTitle.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Purpose.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.Quantity.ToString().ToLower().Contains(filter.ToLower()))
                                        || (x.PostingTime.ToString().ToLower().Contains(filter.ToLower()))
            , w => w.RawMaterial, w => w.Warehouse).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            return PartialView("Grid", records);
        }


        // GET: WarehouseMaterials/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseMaterials warehouseMaterials = repository.GetById(id, w => w.RawMaterial, w => w.Warehouse);
            if (warehouseMaterials == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", warehouseMaterials);
        }

        // GET: WarehouseMaterials/Create
        [HttpGet]
        public ActionResult Create()
        {
            WarehouseMaterials warehouseMaterials = new WarehouseMaterials();
            IEntityService<BLL.Entities.Inventory.RawMaterial> RawMaterialrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
            ViewData["RawMaterialID"] = new SelectList(RawMaterialrepository.GetAll(), "Id", "MaterialName");
            IEntityService<BLL.Entities.Warehouse.Warehouse> Warehouserepository = new EntityService<BLL.Entities.Warehouse.Warehouse>(db);
            ViewData["WarehouseID"] = new SelectList(Warehouserepository.GetAll(), "Id", "WarehouseName");
            return PartialView("Create", warehouseMaterials);
        }

        // POST: WarehouseMaterials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EntryTitle,EntryType,Purpose,RawMaterialID,WarehouseID,Quantity,PostingTime,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] WarehouseMaterials warehouseMaterials)
        {
            if (ModelState.IsValid)
            {
                repository.Add(warehouseMaterials);
                repository.Commit();
                return Json(new { success = true });
            }

            IEntityService<BLL.Entities.Inventory.RawMaterial> RawMaterialrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
            ViewData["RawMaterialID"] = new SelectList(RawMaterialrepository.GetAll(), "Id", "MaterialName", warehouseMaterials.RawMaterialID);
            IEntityService<BLL.Entities.Warehouse.Warehouse> Warehouserepository = new EntityService<BLL.Entities.Warehouse.Warehouse>(db);
            ViewData["WarehouseID"] = new SelectList(Warehouserepository.GetAll(), "Id", "WarehouseName", warehouseMaterials.WarehouseID);
            return PartialView(warehouseMaterials);
        }

        // GET: WarehouseMaterials/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseMaterials warehouseMaterials = repository.GetById(id);
            if (warehouseMaterials == null)
            {
                return HttpNotFound();
            }
            IEntityService<BLL.Entities.Inventory.RawMaterial> RawMaterialrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
            ViewData["RawMaterialID"] = new SelectList(RawMaterialrepository.GetAll(), "Id", "MaterialName", warehouseMaterials.RawMaterialID);
            IEntityService<BLL.Entities.Warehouse.Warehouse> Warehouserepository = new EntityService<BLL.Entities.Warehouse.Warehouse>(db);
            ViewData["WarehouseID"] = new SelectList(Warehouserepository.GetAll(), "Id", "WarehouseName", warehouseMaterials.WarehouseID);
            return PartialView("Edit", warehouseMaterials);
        }

        // POST: WarehouseMaterials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EntryTitle,Purpose,RawMaterialID,WarehouseID,Quantity,PostingTime,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] WarehouseMaterials warehouseMaterials)
        {
            if (ModelState.IsValid)
            {
                repository.Update(warehouseMaterials);
                repository.Commit();
                return Json(new { success = true });
            }
            IEntityService<BLL.Entities.Inventory.RawMaterial> RawMaterialrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
            ViewData["RawMaterialID"] = new SelectList(RawMaterialrepository.GetAll(), "Id", "MaterialName", warehouseMaterials.RawMaterialID);
            IEntityService<BLL.Entities.Warehouse.Warehouse> Warehouserepository = new EntityService<BLL.Entities.Warehouse.Warehouse>(db);
            ViewData["WarehouseID"] = new SelectList(Warehouserepository.GetAll(), "Id", "WarehouseName", warehouseMaterials.WarehouseID);
            return PartialView("Edit", warehouseMaterials);
        }

        // GET: WarehouseMaterials/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseMaterials warehouseMaterials = repository.GetById(id, w => w.RawMaterial, w => w.Warehouse);
            if (warehouseMaterials == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", warehouseMaterials);
        }

        // POST: WarehouseMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            WarehouseMaterials warehouseMaterials = repository.GetById(id);
            repository.Remove(warehouseMaterials);
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
                        WarehouseMaterials warehouseMaterials = repository.GetById(key);
                        repository.Remove(warehouseMaterials);
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
            var records = repository.GetAll(w => w.RawMaterial, w => w.Warehouse).OrderBy("Id DESC").AsEnumerable<WarehouseMaterials>();
            if (model.PagingEnabled && model.Range.ToString() != "All")
            {
                records = records.Skip((model.CurrentPage - 1) * model.PageSize)
                   .Take(model.PageSize);
            }
            if (model.OutputType.Equals(Output.Excel))
            {
                var excelFormatter = new ExcelFormatter<WarehouseMaterials>(records);
                return new ExcelResult(excelFormatter.WriteHtmlTable(), "WarehouseMaterials.xls");
            }
            if (model.OutputType.Equals(Output.Csv))
            {
                return new CsvResult<WarehouseMaterials>(records.AsQueryable(), "WarehouseMaterials.csv");
            }
            return Json(new { success = false });
        }

        public ActionResult MaterialStockLegder()
        {
            return View();
        }

        public JsonResult DataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<WarehouseMaterials>();

                dtsource = repository.GetAll(w => w.RawMaterial, w => w.Warehouse).ToList();

                List<String> columnSearch = new List<string>();

                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<WarehouseMaterials> data = new MaterialsStockResultSet().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
                int count = new MaterialsStockResultSet().Count(param.Search.Value, dtsource, columnSearch);
                List<WarehouseMaterials> filteredResults = new List<WarehouseMaterials>();
                foreach(var d in data)
                {
                    var tmp = new WarehouseMaterials();
                    tmp.EntryTitle = d.EntryTitle;
                    tmp.EntryType = d.EntryType;
                    tmp.PostingTime = d.PostingTime;
                    tmp.Purpose = d.Purpose;
                    tmp.Quantity = d.Quantity;
                    IEntityService<BLL.Entities.Inventory.RawMaterial> RawMaterialrepository = new EntityService<BLL.Entities.Inventory.RawMaterial>(db);
                    d.RawMaterial = RawMaterialrepository.GetById(d.RawMaterialID);
                    tmp.RawMaterial = d.RawMaterial;
                    tmp.RawMaterial.MaterialName = d.RawMaterial.MaterialName;

                    IEntityService<BLL.Entities.Warehouse.Warehouse> Warehouserepository = new EntityService<BLL.Entities.Warehouse.Warehouse>(db);
                    d.Warehouse = Warehouserepository.GetById(d.WarehouseID);
                    tmp.Warehouse = d.Warehouse;
                    tmp.Warehouse.WarehouseName = d.Warehouse.WarehouseName;

                    filteredResults.Add(tmp);
                }
                DTResult<WarehouseMaterials> result = new DTResult<WarehouseMaterials>
                {
                    draw = param.Draw,
                    data = filteredResults,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                //var content = JsonConvert.SerializeObject(result,
                //                Formatting.None,
                //                new JsonSerializerSettings()
                //                {
                //                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                //                });
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
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
