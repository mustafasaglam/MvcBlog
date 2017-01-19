using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;
using System.Web.Helpers;
using System.IO;

namespace MvcBlog.Controllers
{
    public class UyeController : Controller
    {
        mvcblogDB db = new mvcblogDB();

        // GET: Uye
        public ActionResult Index(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.UyeId)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Uye uye)
        {
            var login = db.Uyes.Where(u => u.KullaniciAdi == uye.KullaniciAdi).SingleOrDefault();
            if (login.KullaniciAdi == uye.KullaniciAdi && login.Email == uye.Email && login.Sifre == uye.Sifre)
            {
                Session["uyeid"] = login.UyeId;
                Session["kullaniciadi"] = login.KullaniciAdi;
                Session["yetkiid"] = login.YetkiId;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }

        }
        public ActionResult Logout()
        {
            Session["uyeid"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Uye uye, HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);

                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newfoto);
                    uye.Foto = "/Uploads/UyeFoto/" + newfoto;
                    uye.YetkiId = 2;
                    db.Uyes.Add(uye);
                    db.SaveChanges();
                    Session["uyeid"] = uye.UyeId;
                    Session["kullaniciadi"] = uye.KullaniciAdi;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Fotoğraf", "Fotoğraf Seçiniz");
                }
            }
            return View(uye);
        }
        public ActionResult Edit(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.UyeId)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        [HttpPost]
        public ActionResult Edit(Uye uye, int id, HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                var uyes = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
                if (Foto != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(uye.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(uyes.Foto));
                    }
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);

                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newfoto);
                    uyes.Foto = "/Uploads/UyeFoto/" + newfoto;
                }
                uyes.AdSoyad = uye.AdSoyad;
                uyes.KullaniciAdi = uye.KullaniciAdi;
                uyes.Sifre = uye.Sifre;
                uyes.Email = uye.Email;
                db.SaveChanges();
                Session["kullaniciadi"] = uye.KullaniciAdi;
                return RedirectToAction("Index", "Home", new { id = uyes.UyeId });


            }
            return View();
        }

        public ActionResult UyeProfil(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
            return View(uye);
        }
    }
}