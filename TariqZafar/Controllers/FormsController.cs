using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TariqZafar.Models;

namespace TariqZafar.Controllers
{
    public class FormsController : Controller
    {
        private TariqZafarEntities db = new TariqZafarEntities();

        public ActionResult Index()
        {
            return View(db.Forms.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = db.Forms.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        public ActionResult Create()
        {
            ViewBag.CountryList = GetCountryList();
            ViewBag.StateList = new List<SelectListItem>();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FormID,Name,Age,Gender,Email,PhoneNumber,Country,State")] Form form)
        {

            string selectedCountry = Request["Country"];

            ViewBag.CountryList = GetCountryList();

            ViewBag.StateList = GetStateList(selectedCountry);

            if (ModelState.IsValid)
            {
                db.Forms.Add(form);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(form);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = db.Forms.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }

            ViewBag.CountryList = GetCountryList();

            string selectedCountry = form.Country;

            ViewBag.StateList = GetStateList(selectedCountry);

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FormID,Name,Age,Gender,Email,PhoneNumber,Country,State")] Form form)
        {
            string selectedCountry = Request["Country"];

            ViewBag.CountryList = GetCountryList();

            ViewBag.StateList = GetStateList(selectedCountry);

            if (ModelState.IsValid)
            {
                db.Entry(form).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(form);
        }

        private List<SelectListItem> GetCountryList()
        {
            var countries = new List<SelectListItem>
            {
              new SelectListItem { Text = "USA", Value = "USA" },
               new SelectListItem { Text = "India", Value = "India" },
               new SelectListItem { Text = "UK", Value = "UK" },
            };

            return countries;
        }

        [HttpGet]
        public JsonResult GetState(string country)
        {
            List<SelectListItem> _lst = GetStateList(country);

            var _StateList = new SelectList(_lst, "Value", "Text");
            return Json(_StateList, JsonRequestBehavior.AllowGet);
        }


        private List<SelectListItem> GetStateList(string country)
        {

            var states = new List<SelectListItem>();

            if (country == "USA")
            {
                states.Add(new SelectListItem { Text = "Alaska", Value = "Alaska" });
                states.Add(new SelectListItem { Text = "Arizona", Value = "Arizona" });
                states.Add(new SelectListItem { Text = "California", Value = "California" });
                states.Add(new SelectListItem { Text = "Illinois", Value = "Illinois" });
            }
            else if (country == "India")
            {
                states.Add(new SelectListItem { Text = "Uttar Pradesh", Value = "Uttar Pradesh" });
                states.Add(new SelectListItem { Text = "Punjab", Value = "Punjab" });
                states.Add(new SelectListItem { Text = "Bihar", Value = "Bihar" });
                states.Add(new SelectListItem { Text = "Punjab", Value = "Punjab" });
            }
            else if (country == "UK")
            {
                states.Add(new SelectListItem { Text = "England", Value = "England" });
                states.Add(new SelectListItem { Text = "Scotland", Value = "Scotland" });
            }

            return states;
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form form = db.Forms.Find(id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Form form = db.Forms.Find(id);
            db.Forms.Remove(form);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult FormForMachineTest1()
        {
            var lst = db.Forms.ToList();

            if (lst.Count > 0)
            {
                using (ReportDocument crReport = new ReportDocument())
                {
                    var response = System.Web.HttpContext.Current.Response;

                    var reportPath = Server.MapPath("~/Models/FormCrystal.rpt");
                    crReport.Load(reportPath);
                    crReport.SetDataSource(lst);

                    Stream stream = crReport.ExportToStream(ExportFormatType.PortableDocFormat);

                    stream.Seek(0, SeekOrigin.Begin);
                    crReport.Close();
                    crReport.Dispose();
                    return File(stream, "application/pdf");
                }
            }
            return RedirectToAction("Index");
        }
    }
}