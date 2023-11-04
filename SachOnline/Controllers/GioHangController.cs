using SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace SachOnline.Controllers
{
    public class GioHangController : Controller
    {
        private string connection;
        private dbSachOnlineDataContext data;

        public GioHangController()
        {
            // Khởi tạo chuỗi kết nối
            connection = "Data Source=DESKTOP-JGJPIQA\\SQLEXPRESS01;Initial Catalog=QL_SachOnlineltw;Integrated Security=True";
            data = new dbSachOnlineDataContext(connection);
        }

        // LayGioHang
        public List<GioHang> LayGioHang()
        {
            var gioHang = Session["GioHang"] as List<GioHang>;
            if (Session["GioHang"] == null)
            {
                gioHang = new List<GioHang>();
                Session["GioHang"] = gioHang;
            }
            else
            {
                gioHang = Session["GioHang"] as List<GioHang>;
            }

            return gioHang;
        }

        // ThemGioHang
        public ActionResult ThemGioHang(int ms, string url)
        {
            var gioHang = LayGioHang();
            var sanPham = gioHang.Find(n => n.iMaSach == ms);
            if (sanPham == null)
            {
                sanPham = new GioHang(ms);
                gioHang.Add(sanPham);
            }
            else
            {
                sanPham.iSoLuong++;
            }
            return Redirect(url);
        }

        // TongSoLuong
        private int TongSoLuong(List<GioHang> gioHang)
        {
            return gioHang.Sum(n => n.iSoLuong);
        }

        // TinhTongTien
        private double TongTien(List<GioHang> gioHang)
        {
            return gioHang.Sum(n => n.dThanhTien);
        }

        // GioHang
        public ActionResult GioHang()
        {
            var gioHang = LayGioHang();
            if (gioHang.Count == 0)
            {
                return RedirectToAction("Index", "SachOnLine");
            }

            ViewBag.TongSoLuong = TongSoLuong(gioHang);
            ViewBag.TongTien = TongTien(gioHang);
            return View(gioHang);
        }

        // GioHangPartial
        public ActionResult GioHangPartial()
        {
            var gioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong(gioHang);
            ViewBag.TongTien = TongTien(gioHang);
            return PartialView();
        }
        // XoaSPKhoiGioHang
        public ActionResult XoaSPKhoiGioHang(int iMaSach)
        {
            var gioHang = LayGioHang();
            var sanPham = gioHang.SingleOrDefault(n => n.iMaSach == iMaSach);
            if (sanPham != null)
            {
                gioHang.Remove(sanPham);
                if (gioHang.Count == 0)
                {
                    return RedirectToAction("Index", "SachOnline");
                }
            }
            return RedirectToAction("GioHang");
        }

        // XoaGioHang
        public ActionResult XoaGioHang()
        {
            var gioHang = LayGioHang();
            gioHang.Clear();
            return RedirectToAction("Index", "SachOnline");
        }
        // CapNhatGioHang
        [HttpPost]
        public ActionResult CapNhatGioHang(int iMaSach, FormCollection f)
        {
            var gioHang = LayGioHang();
            var sanPham = gioHang.SingleOrDefault(n => n.iMaSach == iMaSach);
            if (sanPham != null)
            {
                sanPham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "User");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "SachOnline");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong(lstGioHang);
            ViewBag.TongTien = TongTien(lstGioHang);
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            List<GioHang> lstGioHang = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            var NgayGiao = String.Format("{0:MM/dd/yyyy}", f["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(NgayGiao);
            ddh.TinhTrangGiaoHang = 1;
            ddh.DaThanhToan = false;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            // Thêm chi tiết đơn hàng
            foreach (var item in lstGioHang)
            {
                CHITIETDATHANG ctdh = new CHITIETDATHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSach = item.iMaSach;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dDonGia;
                data.CHITIETDATHANGs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }
        // XacNhanDonHang
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }
}
