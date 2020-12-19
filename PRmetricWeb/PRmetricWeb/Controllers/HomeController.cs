using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRmetricWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PRmetricWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Auth()
        {
            return View();
        }

        public IActionResult Index(string url, string project, string repos, string token)
        {
            var userList = new UserList(url,project,repos,token);
            List<User> users = userList.GetUsers();
            return View(users);
        }

        public IActionResult Info()
        {
            var InfoList = new InfoList();
            var info = InfoList.GetIterationTablesInfo();
            return View(info);
        }
    }
}
