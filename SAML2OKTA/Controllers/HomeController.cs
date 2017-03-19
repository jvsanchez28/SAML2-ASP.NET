using SAML2OKTA.Authorize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAML2OKTA.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Error()
        {
            return View("~/Views/Error.cshtml");
        }

        public ActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }
   
        public ActionResult Login()
        {
            return Redirect("~/Login.ashx");
        }

        [Authorize]
        public ActionResult Private()
        {

            return View("~/Views/Private.cshtml");
        }

        [Authorize]
        public ActionResult Logout()
        {
            return Redirect("~/Logout.ashx");
        }
    }
}