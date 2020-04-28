using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace meldboek.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateAccount()
        {
            return View();
        }

        public IActionResult LogIn(string email, string password)
        {

            {
                if (email == "admin" && password == "admin")
                {
                    Response.Redirect("LogIn");
                }
            }



            return View();

        }

    }
}