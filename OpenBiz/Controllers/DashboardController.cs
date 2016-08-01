using DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCMS.Controllers
{
    public class DashboardController : Controller
    {
        private SCMSContext db = new SCMSContext();
        public ActionResult Index()
        {
            ViewBag.Title = "Dashboard";
            //var Products = (from a in db.Products select new { a.ProductName, a.Quantity }).ToList();
            //var json = JsonConvert.SerializeObject(Products, Formatting.Indented);
            //ViewBag.json = json;
            return View();
        }

        public ActionResult Inventory()
        {
            //var Products = (from a in db.Products select new { a.ProductName, a.Quantity }).ToList();
            //var json = JsonConvert.SerializeObject(Products, Formatting.Indented);
            //ViewBag.json = json;
            return View();
        }
    }
}