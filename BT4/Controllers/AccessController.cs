using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Controllers
{
    public class AccessController : Controller
    { 
        QlbanVaLiContext db = new QlbanVaLiContext();




        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
           return View();
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult Login(TUser user)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                var u1 = db.TUsers.Where(x => x.Username.Equals(user.Username) && x.Password.Equals(user.Password) && x.LoaiUser == 0).FirstOrDefault();
                if (u1 != null)
                {
                    HttpContext.Session.SetString("UserName", u1.Username.ToString());
                    return RedirectToAction("Index", "Home");
                }
                var u2 = db.TUsers.Where(x => x.Username.Equals(user.Username) && x.Password.Equals(user.Password) && x.LoaiUser == 1).FirstOrDefault();
                if (u2 != null)
                {
                    HttpContext.Session.SetString("UserName", u2.Username.ToString());
                    return RedirectToAction("DanhMucSanPham", "Admin");
                }
            }
            return View();
        }










        [Route("Signup")]
        [HttpGet]
        public IActionResult Signup()
        {
           
            var model = new RegisterViewModel
            {
                KhachHang = new TKhachHang(), 
                User = new TUser() 
            };
            return View(model); 
        }

        [Route("Signup")]
        [HttpPost]
        public IActionResult Signup(RegisterViewModel model)
        {
            var existingUser = db.TUsers.SingleOrDefault(u => u.Username == model.User.Username);

            if (existingUser != null)
            {
                model.UsernameError = "Tên người dùng đã tồn tại. Vui lòng chọn tên khác.";
                ModelState.AddModelError("User.Username", model.UsernameError);
                return View(model);
            }
            model.User.LoaiUser = 0;
            db.TUsers.Add(model.User);
            model.KhachHang.MaKhanhHang = model.User.Username;
            model.KhachHang.Username = model.User.Username;
            db.TKhachHangs.Add(model.KhachHang);
            db.SaveChanges();
            return RedirectToAction("Login");

        }






       

        [Route("Logout")]

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login","Access");
        }
    }
}
