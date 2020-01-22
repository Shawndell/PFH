﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ProjectForHealing.Models;
using ProjectForHealing.ViewModels;

namespace ProjectForHealing.Controllers
{
    public class ResourceController : Controller
    {
       private CRMSContext context;
        private readonly IHostingEnvironment hostingEnvironment;
        //asking to get instance of CRMS context
        public ResourceController(CRMSContext dbContext, IHostingEnvironment hostingEnvironment)
        {
            context = dbContext;
            this.hostingEnvironment = hostingEnvironment;
        }


        public IActionResult Submitted()
        {
            return View();
        }

        public IActionResult Add()
        {
            AddResourceViewModel addResourceViewModel = new AddResourceViewModel();
            return View(addResourceViewModel);
        }


        [HttpPost]
        [Route("/Resource/Add")]
        public IActionResult Add(AddResourceViewModel addResourceViewModel)
        {
        
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                //if the actual file is not null
                if(addResourceViewModel.UploadedFile != null)
                {
                  string uploadsFolder =  Path.Combine(hostingEnvironment.WebRootPath, "ResourceFiles");
                  uniqueFileName = Guid.NewGuid().ToString() + "_" + addResourceViewModel.UploadedFile.FileName;
                   string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    addResourceViewModel.UploadedFile.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                Resource newResource = new Resource
                {
                    //  ResourceID = addResourceViewModel.ResourceID,
                    OrgName = addResourceViewModel.OrgName,
                    OrgNumber = addResourceViewModel.OrgNumber,
                    OrgEmail = addResourceViewModel.OrgEmail,
                    OrgAddress = addResourceViewModel.OrgAddress,
                    OrgZip = addResourceViewModel.OrgZip,
                    OrgSte = addResourceViewModel.OrgSte,
                    OrgDescription = addResourceViewModel.OrgDescription,
                    WebsiteUrl = addResourceViewModel.WebsiteUrl,
                    Fname = addResourceViewModel.Fname,
                    Lname = addResourceViewModel.Lname,
                    RepNumber = addResourceViewModel.RepNumber,
                    RepEmail = addResourceViewModel.RepEmail,
                   PhotoPath = uniqueFileName
                };
                context.Resource.Add(newResource);
                context.SaveChanges();
                return Redirect("/Resource/Submitted");
            }
            else
            {
                return View(addResourceViewModel);
            }
            
        }

     

    }
}