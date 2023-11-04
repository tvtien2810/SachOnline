using SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SachOnline.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private string connection;
        private dbSachOnlineDataContext db;

        public AdminController()
        {
            // Khởi tạo chuỗi kết nối
            connection = "Data Source=DESKTOP-JGJPIQA\\SQLEXPRESS01;Initial Catalog=QL_SachOnlineltw;Integrated Security=True";
            db = new dbSachOnlineDataContext(connection);
        }

        // GET: Admin/Admin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            var sTenDN = f["your_name"];
            var sMatKhau = f["your_pass"];

            ADMIN ad = db.ADMINs.SingleOrDefault(n => n.TenDN == sTenDN && n.MatKhau == sMatKhau);
            if (ad != null)
            {
                Session["Admin"] = ad;

                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }
        }

    }
}