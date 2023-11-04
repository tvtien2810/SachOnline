using SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SachOnline.Controllers
{
    public class UserController : Controller
    {
        private String connection;
        private dbSachOnlineDataContext db;

        public UserController()
        {
            // Khởi tạo chuỗi kết nối
            connection = "Data Source=DESKTOP-JGJPIQA\\SQLEXPRESS01;Initial Catalog=QL_SachOnlineltw;Integrated Security=True";
            db = new dbSachOnlineDataContext(connection);
        }

        // GET: User
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KHACHHANG kh)
        {
            var sHoTen = collection["HoTen"];
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["MatKhau"];
            var sMatKhauNhapLai = collection["MatKhauNL"];
            var sDiaChi = collection["DiaChi"];
            var sEmail = collection["Email"];
            var sDienThoai = collection["DienThoai"];
            var dNgaySinh = String.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);
            if (String.IsNullOrEmpty(sHoTen))
            {
                TempData["HoTen"] = sHoTen;
                ViewData["err1"] = "Họ tên không được rỗng";
            }
            else if (String.IsNullOrEmpty(sTenDN))
            {
                TempData["TenDN"] = sTenDN;
                ViewData["err2"] = "Tên đăng nhập không được rỗng";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                TempData["MatKhau"] = sMatKhau;
                ViewData["err3"] = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(sMatKhauNhapLai))
            {
                TempData["MatKhauNL"] = sMatKhauNhapLai;
                ViewData["err4"] = "Phải nhập lại mật khẩu";
            }
            else if (sMatKhau != sMatKhauNhapLai)
            {
                ViewData["err4"] = "Mật khẩu nhập lại không khớp";
            }
            else if (String.IsNullOrEmpty(sEmail))
            {
                TempData["Email"] = sEmail;
                ViewData["err5"] = "Email không được rỗng";
            }
            else if (String.IsNullOrEmpty(sDienThoai))
            {
                ViewData["err6"] = "Số điện thoại không được rỗng";
            }
            else if (String.IsNullOrEmpty(dNgaySinh))
            {
                ViewData["err7"] = "Ngày sinh không được rỗng";
            }
            else if (db.KHACHHANGs.SingleOrDefault(n => n.Email == sEmail) != null)
            {
                ViewBag.ThongBao = "Email đã được sử dụng";
            }
            else if (db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN) != null)
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại";
            }
            else
            {
                kh.HoTen = sHoTen;
                kh.TaiKhoan = sTenDN;
                kh.MatKhau = sMatKhau;
                kh.Email = sEmail;
                kh.DiaChi = sDiaChi;
                kh.DienThoai = sDienThoai;
                kh.NgaySinh = DateTime.Parse(dNgaySinh);
                db.KHACHHANGs.InsertOnSubmit(kh);
                db.SubmitChanges();
                return RedirectToAction("DangNhap");
            }
            return this.DangKy();
        }


        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["MatKhau"];

            if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["Err2"] = "Phải nhập mật khẩu";
            }
            else
            {
                if (sTenDN == "admin" && sMatKhau == "admin") // Thay "admin" và "admin_password" bằng thông tin đăng nhập thực tế của Admin
                {
                    // Đăng nhập thành công cho Admin
                    Session["Admin"] = "admin"; // Lưu thông tin đăng nhập Admin vào Session
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                else
                {
                    KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN && n.MatKhau == sMatKhau);
                    if (kh != null)
                    {
                        ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                        Session["TaiKhoan"] = kh;

                        // Kiểm tra giỏ hàng
                        var gioHang = Session["GioHang"] as List<GioHang>;
                        if (gioHang != null && gioHang.Any())
                        {
                            // Giỏ hàng không trống, chuyển hướng đến trang DatHang
                            return RedirectToAction("DatHang", "GioHang");
                        }
                        else
                        {
                            // Giỏ hàng trống, chuyển hướng đến trang Index
                            return RedirectToAction("Index", "SachOnline");
                        }
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                    }
                }
            }

            return View();
        }


        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "SachOnline");
        }



    }
}
