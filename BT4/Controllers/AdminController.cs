using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using Demo.Models.Authentication;
using X.PagedList.Extensions;

namespace Demo.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        QlbanVaLiContext db = new QlbanVaLiContext();
     
        [Route("index")]
        [Authentication]
        public IActionResult Index()
        {
            return View();
        }

        [Authentication]
        [Route("danhmucsanpham")]
        public IActionResult DanhMucSanPham(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstsanpham = db.TDanhMucSps.AsNoTracking().OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(lstsanpham, pageNumber, pageSize);
            return View(lst);
        }


        [Route("ThemSanPhamMoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            ViewBag.MaChatLieu = new SelectList(db.TChatLieus.ToList(), "MaChatLieu", "ChatLieu");
            ViewBag.MaHangSx = new SelectList(db.THangSxes.ToList(), "MaHangSx", "HangSx");
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc");
            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai");
            ViewBag.MaDt = new SelectList(db.TLoaiDts.ToList(), "MaDt", "TenLoai");
            return View();
        }

        [Route("ThemSanPhamMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPhamMoi(TDanhMucSp sanPham)
        {
            if (ModelState.IsValid)
            {
                db.TDanhMucSps.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham");
            }
            return View(sanPham);
        }


        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(string maSamPham)
        {
            ViewBag.MaChatLieu = new SelectList(db.TChatLieus.ToList(), "MaChatLieu", "ChatLieu");
            ViewBag.MaHangSx = new SelectList(db.THangSxes.ToList(), "MaHangSx", "HangSx");
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc");
            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai");
            ViewBag.MaDt = new SelectList(db.TLoaiDts.ToList(), "MaDt", "TenLoai");
            var sanPham = db.TDanhMucSps.Find(maSamPham);
            return View(sanPham);
        }

        [Route("SuaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(TDanhMucSp sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham", "Admin");
            }
            return View(sanPham);
        }

        [Route("XoaSanPham")]
        [HttpGet]
        public IActionResult XoaSanPham(string maSanPham)
        {
            TempData["Message"] = "";
            var chiTietSanPham = db.TChiTietSanPhams.Where(x => x.MaSp == maSanPham).ToList();
            if (chiTietSanPham.Count > 0)
            {
                TempData["Message"] = "Không xóa đươc sản phẩm này";
                return RedirectToAction("DanhMucSanPham", "Admin");
            }
            var anhSanPham = db.TAnhSps.Where(x => x.MaSp == maSanPham);
            if (anhSanPham.Any()) db.RemoveRange(anhSanPham);
            db.Remove(db.TDanhMucSps.Find(maSanPham));
            db.SaveChanges();
            TempData["Message"] = "Sản phẩm đã được xóa";
            return RedirectToAction("DanhMucSanPham", "Admin");
        }













        [Authentication]
        [Route("danhmuckhachhang")]
        public IActionResult DanhMucKhachHang(int? page)
        {
            int pageSize = 10; // S? lu?ng khách hàng m?i trang
            int pageNumber = page ?? 1; // Trang hi?n t?i

            // L?y t?t c? khách hàng, s?p x?p và phân trang
            var lstKhachHang = db.TKhachHangs.OrderBy(kh => kh.TenKhachHang).ToPagedList(pageNumber, pageSize);

            return View(lstKhachHang);
        }
        // GET: Xác nh?n xóa khách hàng (ch? c?n n?u b?n mu?n hi?n th? m?t trang xác nh?n)
        [Route("XoaKhachHang")]
        [HttpGet]
        public IActionResult XoaKhachHang(string maKhachHang)
        {
            var khachHang = db.TKhachHangs.FirstOrDefault(kh => kh.MaKhanhHang == maKhachHang);
            if (khachHang == null)
            {
                return NotFound(); // Không tìm th?y khách hàng
            }

            return View(khachHang); // Tr? v? trang xác nh?n xóa
        }

        // POST: Xóa khách hàng
        [Route("XoaKhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanXoaKhachHang(string maKhachHang)
        {
            var khachHang = db.TKhachHangs.FirstOrDefault(kh => kh.MaKhanhHang == maKhachHang);
            if (khachHang == null)
            {
                return NotFound(); // Không tìm th?y khách hàng
            }

            db.TKhachHangs.Remove(khachHang); // Xóa khách hàng kh?i co s? d? li?u
            db.SaveChanges(); // Luu thay d?i

            TempData["Message"] = "Xóa khách hàng thành công.";
            return RedirectToAction("DanhMucKhachHang"); // Quay v? trang danh sách khách hàng
        }
        // GET: Hi?n th? form ch?nh s?a
        [Route("SuaKhachHang")]
        [HttpGet]
        public IActionResult SuaKhachHang(string maKhachHang)
        {
            // Tìm khách hàng theo mã
            var khachHang = db.TKhachHangs.FirstOrDefault(kh => kh.MaKhanhHang == maKhachHang);
            if (khachHang == null)
            {
                return NotFound(); // Không tìm th?y khách hàng
            }

            return View(khachHang); // Tr? v? form v?i thông tin khách hàng
        }

        // POST: Xác nh?n và luu thay d?i
        [Route("SuaKhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaKhachHang(TKhachHang model)
        {
            if (ModelState.IsValid)
            {
                // Tìm khách hàng trong DB
                var khachHang = db.TKhachHangs.FirstOrDefault(kh => kh.MaKhanhHang == model.MaKhanhHang);
                if (khachHang == null)
                {
                    return NotFound();
                }

                // C?p nh?t thông tin khách hàng
                khachHang.TenKhachHang = model.TenKhachHang;
                khachHang.NgaySinh = model.NgaySinh;
                khachHang.SoDienThoai = model.SoDienThoai;
                khachHang.DiaChi = model.DiaChi;
                khachHang.LoaiKhachHang = model.LoaiKhachHang;
                khachHang.GhiChu = model.GhiChu;

                // Luu thay d?i vào co s? d? li?u
                db.SaveChanges();

                TempData["Message"] = "C?p nh?t thông tin khách hàng thành công.";
                return RedirectToAction("DanhMucKhachHang");
            }

            return View(model); // N?u d? li?u không h?p l?, tr? l?i form v?i thông tin l?i
        }
        [Route("DanhMucNhanVien")]
        [HttpGet]
        public IActionResult DanhMucNhanVien(int? page)
        {
            int pageSize = 10; // s? lu?ng nhân viên m?i trang
            int pageNumber = page ?? 1;

            // L?y danh sách nhân viên t? co s? d? li?u và s?p x?p theo MaNhanVien
            var nhanViens = db.TNhanViens
                .OrderBy(nv => nv.MaNhanVien)
                .ToPagedList(pageNumber, pageSize);

            return View(nhanViens);
        }
        // GET: Xác nh?n xóa nhân viên (n?u mu?n hi?n th? m?t trang xác nh?n)
        [Route("XoaNhanVien")]
        [HttpGet]
        public IActionResult XoaNhanVien(string maNhanVien)
        {
            var nhanVien = db.TNhanViens.FirstOrDefault(nv => nv.MaNhanVien == maNhanVien);
            if (nhanVien == null)
            {
                return NotFound(); // Không tìm th?y nhân viên
            }

            return View(nhanVien); // Tr? v? trang xác nh?n xóa
        }

        // POST: Xóa nhân viên
        [Route("XoaNhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanXoaNhanVien(string maNhanVien)
        {
            var nhanVien = db.TNhanViens.FirstOrDefault(nv => nv.MaNhanVien == maNhanVien);
            if (nhanVien == null)
            {
                return NotFound(); // Không tìm th?y nhân viên
            }

            // Ki?m tra xem nhân viên có hóa don nào không
            bool hasHoaDon = db.THoaDonBans.Any(hd => hd.MaNhanVien == maNhanVien);
            if (hasHoaDon)
            {
                TempData["Message"] = "Không th? xóa vì nhân viên này có liên k?t v?i hóa don.";
                return RedirectToAction("DanhMucNhanVien"); // Quay v? trang danh sách nhân viên
            }

            // L?y username c?a nhân viên d? xóa b?n ghi trong b?ng TUser n?u có
            var username = nhanVien.Username;

            // Xóa nhân viên kh?i b?ng TNhanVien
            db.TNhanViens.Remove(nhanVien);

            // N?u có username, xóa user tuong ?ng trong b?ng TUser
            if (!string.IsNullOrEmpty(username))
            {
                var user = db.TUsers.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    db.TUsers.Remove(user);
                }
            }

            // Th?c hi?n luu thay d?i vào co s? d? li?u
            try
            {
                db.SaveChanges();
                TempData["Message"] = "Xóa nhân viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Ðã x?y ra l?i khi xóa nhân viên: " + ex.Message;
            }

            return RedirectToAction("DanhMucNhanVien"); // Quay v? trang danh sách nhân viên
        }

        [Route("ThemNhanVienMoi")]
        [HttpGet]
        public IActionResult ThemNhanVienMoi()
        {
            // D? li?u cho các dropdown n?u c?n
            ViewBag.ChucVu = new SelectList(new List<string> { "Sales", "Manager", "Admin" }, "ChucVu"); // Ví d? cho ch?c v?

            return View();
        }
        [Route("ThemNhanVienMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemNhanVienMoi(TNhanVien nhanVien, string password)
        {
            if (ModelState.IsValid)
            {
                using var transaction = db.Database.BeginTransaction();
                try
                {
                    // Ki?m tra xem MaNhanVien có trùng không
                    if (db.TNhanViens.Any(nv => nv.MaNhanVien == nhanVien.MaNhanVien))
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên dã t?n t?i.");
                        return View(nhanVien);
                    }

                    // Ki?m tra xem Username có trùng không
                    if (db.TUsers.Any(u => u.Username == nhanVien.Username))
                    {
                        ModelState.AddModelError("Username", "Username dã t?n t?i.");
                        return View(nhanVien);
                    }

                    // T?o tài kho?n ngu?i dùng m?i
                    var user = new TUser
                    {
                        Username = nhanVien.Username,
                        Password = password, // Nên mã hóa m?t kh?u tru?c khi luu
                        LoaiUser = 1 // 1 = Nhân viên, 0 = Khách hàng, có th? tùy ch?nh theo logic c?a b?n
                    };
                    db.TUsers.Add(user);

                    // Thêm nhân viên m?i vào co s? d? li?u
                    db.TNhanViens.Add(nhanVien);

                    // Luu thay d?i vào database
                    db.SaveChanges();

                    // Commit transaction
                    transaction.Commit();

                    TempData["Message"] = "Thêm nhân viên m?i thành công!";
                    return RedirectToAction("DanhMucNhanVien");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Ðã có l?i x?y ra: " + ex.Message);
                }
            }

            // N?u không h?p l?, hi?n th? l?i form v?i các l?i
            return View(nhanVien);
        }
        // GET: Hi?n th? form s?a thông tin nhân viên
        [Route("SuaNhanVien")]
        [HttpGet]
        public IActionResult SuaNhanVien(string maNhanVien)
        {
            var nhanVien = db.TNhanViens.FirstOrDefault(nv => nv.MaNhanVien == maNhanVien);
            if (nhanVien == null)
            {
                return NotFound(); // N?u không tìm th?y nhân viên
            }
            return View(nhanVien); // Tr? v? View v?i thông tin c?a nhân viên
        }

        // POST: C?p nh?t thông tin nhân viên
        [Route("SuaNhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaNhanVien(TNhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                var nhanVienToUpdate = db.TNhanViens.FirstOrDefault(nv => nv.MaNhanVien == nhanVien.MaNhanVien);
                if (nhanVienToUpdate == null)
                {
                    return NotFound(); // Không tìm th?y nhân viên
                }

                // Ch? c?p nh?t các thông tin khác
                nhanVienToUpdate.TenNhanVien = nhanVien.TenNhanVien;
                nhanVienToUpdate.NgaySinh = nhanVien.NgaySinh;
                nhanVienToUpdate.SoDienThoai1 = nhanVien.SoDienThoai1;
                nhanVienToUpdate.SoDienThoai2 = nhanVien.SoDienThoai2;
                nhanVienToUpdate.DiaChi = nhanVien.DiaChi;
                nhanVienToUpdate.ChucVu = nhanVien.ChucVu;
                nhanVienToUpdate.GhiChu = nhanVien.GhiChu;

                db.SaveChanges(); // Luu thay d?i vào co s? d? li?u
                TempData["Message"] = "C?p nh?t thông tin nhân viên thành công!";
                return RedirectToAction("DanhMucNhanVien"); // Quay l?i trang danh sách nhân viên
            }
            return View(nhanVien); // N?u d? li?u không h?p l?, hi?n th? l?i form
        }


    }
}
