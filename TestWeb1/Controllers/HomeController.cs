using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestWeb1.Models;

namespace TestWeb1.Controllers
{
    public class HomeController : Controller
    {
        DBClassDataContext _context = new DBClassDataContext("Data Source=aegold;Initial Catalog=ProductDB;Integrated Security=True;TrustServerCertificate=True");
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public JsonResult GetList()
        {
            var list = _context.SanPhams.ToList();
            return Json(list,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}