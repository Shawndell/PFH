﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ProjectForHealing.Models;
using ProjectForHealing.ViewModels;

namespace ProjectForHealing.Controllers
{
    public class AdminController : Controller
    {

        private CRMSContext context;
        private readonly IHostingEnvironment hostingEnvironment;
        public AdminController(CRMSContext dbContext, IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            context = dbContext;
        }
        
        //checks if logged in before being able to interact with any function in this controller
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpContext.Session.TryGetValue("username", out var bytes);
            if (bytes == null)
            {
                context.Result = new RedirectResult("/");
                return;
            }
            var username = Encoding.UTF8.GetString(bytes);

            if (string.IsNullOrWhiteSpace(username))
            {
                context.Result = new RedirectResult("/");
                return;
            }

            
            base.OnActionExecuting(context);
        }
        public IActionResult Index()
        {
           ViewBag.name = HttpContext.Session.GetString("fname") + " " + HttpContext.Session.GetString("lname");
           ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.number = HttpContext.Session.GetString("pnumber");
            ViewBag.role = HttpContext.Session.GetString("role");
            if (HttpContext.Session.GetString("pic") != "null")
            {
                ViewBag.pic = HttpContext.Session.GetString("pic");
            }
           
            return View();
        }
        public IActionResult AddAdmin()
        {
            return View();
        }
        

        [HttpPost]
        [Route("/Admin/AddAdmin")]
        public IActionResult AddAdmin(AdminViewModel model)
        {
        
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                //if the actual file is not null
                if (model.UploadedFile != null)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "ProfilePictures");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.UploadedFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    model.UploadedFile.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                Admin newAdmin = new Admin
                {
                    //  ResourceID = addEditorViewModel.ResourceID,
                    UserName = model.UserName,
                    Fname = model.Fname,
                    Lname = model.Lname,
                    Email = model.Email,
                    Pnumber = model.Pnumber,
                    AdminPassWord = model.AdminPassWord,
                    ProfilePic = model.ProfilePic,
                    SuperUser = model.SuperUser,
                    PhotoPath = uniqueFileName
                };
                context.Admin.Add(newAdmin);
                context.SaveChanges();
                return Redirect("/Admin/Created");
            }
            else
            {
                return View(model);
            }

        }
        public IActionResult Created()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            // todo: add validation for editing a resource
            var source = context.Resource.SingleOrDefault(x => x.ResourceID == id);
            return View(source);
        }
        [HttpPost]
        public IActionResult Edit(Resource source)
        {
            if (ModelState.IsValid)
            {
                context.Entry(source).State = EntityState.Modified;
                context.SaveChanges();
                ViewBag.Title = "Changes Saved";
                return View(source);
            }
            ViewBag.Title = "Changes not saved";
            return View(source);
        }
        public IActionResult ResourceManage()
        {

            List<Resource> resources = context.Resource.Where(x => x.IsApproved == true).ToList();

            return View(resources);
        }

        [HttpGet]
        public IActionResult Unapproved() {

            List<Resource> resources = context.Resource.Where(x => x.IsApproved == false ).ToList();

            return View(resources);
        }




        [HttpGet]
        public IActionResult Remove(int id)
        {

            Resource theResource = context.Resource.SingleOrDefault(x => x.ResourceID == id);
            context.Resource.Remove(theResource);

            context.SaveChanges();
            return Redirect("/Admin");
        }


        [HttpGet]
        public IActionResult Approve(int id )
        {
          
                Resource theResource = context.Resource.SingleOrDefault(x => x.ResourceID == id);
                theResource.IsApproved = true;
            
            context.SaveChanges();
            return Redirect("/Admin/Unapproved");
        }

           [HttpGet]
        public IActionResult ViewSource(int id)
        {
            var source = context.Resource.SingleOrDefault(x => x.ResourceID == id);
            return View(source);
        }
    }
}