﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QUANLYTIEC.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Errors
        public ActionResult Error()
        {
            Response.StatusCode = 500;
            return View();
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
        public ActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View();
        }
        public ActionResult Forbidden()
        {
            Response.StatusCode = 401;
            return View();
        }

    }
}