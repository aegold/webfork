using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestWeb1.Models;
using System.Diagnostics;
using System.IO;

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
        [HttpPost]
        public JsonResult GetList(int pageNumber,int pageSize)
        {
            Debug.WriteLine(pageNumber +" " + pageSize);
            var list = _context.SanPhams
                             .OrderBy(sp => sp.Ten)
                             .Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            var tongSoSP = _context.SanPhams.Count();
            return Json(new {list,tongSoSP },JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            ViewBag.Message = "Your Create page.";

            return View();
        }
        [HttpPost]
        public JsonResult PostCreate()
        {
            try
            {
                string ten = Request.Form["Ten"];
                string moTa = Request.Form["MoTa"];
                int Price = Convert.ToInt32(Request.Form["Price"]);
                var imageFile = Request.Files["imageFile"];
                Debug.WriteLine(ten + " " + moTa + " " + Price + " ");

                SanPham newProductObj = new SanPham();
                newProductObj.ID = Guid.NewGuid().ToString();
                newProductObj.Ten = ten;
                newProductObj.Mota = moTa;
                newProductObj.Price = Price;

                //IMAGE
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    Debug.WriteLine("Image Recieved");
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/Images"), fileName);

                    imageFile.SaveAs(filePath);
                    newProductObj.ImagePath = "/Images/" + fileName;
                }

                _context.SanPhams.InsertOnSubmit(newProductObj);
                _context.SubmitChanges();
                return Json(new { success = true, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetProductByID(string id)
        {
            var productObj = _context.SanPhams.FirstOrDefault(sp => sp.ID == id);
            if (productObj == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            //Debug.WriteLine(productObj.ID);
            return Json(new { success = true, product = productObj }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult EditProduct()
        {
            try
            {
                var id = Request.Form["ID"];
                Debug.WriteLine(id);
                var productObj = _context.SanPhams.FirstOrDefault(product => product.ID == id);
                //Debug.WriteLine(productObj.ID + "/" + sp.Ten + "/" + sp.Mota + "/" + sp.Price);

                productObj.Ten = Request.Form["Ten"];
                productObj.Mota = Request.Form["MoTa"];
                productObj.Price = Convert.ToInt32(Request.Form["Price"]);
                var imageFile = Request.Files["imageFile"];

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    Debug.WriteLine("Image Recieved");
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/Images"), fileName);

                    imageFile.SaveAs(filePath);
                    productObj.ImagePath = "/Images/" + fileName;
                }

                _context.SubmitChanges();
                return Json(new { success = true,message = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                var productObj = _context.SanPhams.FirstOrDefault(sp => sp.ID == id);
                _context.SanPhams.DeleteOnSubmit(productObj);
                _context.SubmitChanges();
                return Json(new { success = true, message = "Xoá thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public JsonResult SearchProductsByName(string name)
        {
            {
                     var productList = _context.SanPhams
                    .Where(sp => sp.Ten.Contains(name)) // Tìm kiếm theo tên sản phẩm
                    .Select(sp => new
                    {
                        sp.ID,
                        sp.Ten,
                        sp.Mota,
                        sp.Price,
                        ImagePath = Url.Content(sp.ImagePath) // Đảm bảo đường dẫn ảnh hợp lệ
                    })
                    .ToList();
                    Debug.WriteLine(productList);
                return Json(productList, JsonRequestBehavior.AllowGet);
            }
        }
    }
}