﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Salsbokningssystem.Models;
using SendMail.Controllers;
namespace SendMail.Controllers
{
    public class SendMailerController : Controller
    {
        //
        // GET: /SendMailer/  
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ViewResult Index(Salsbokningssystem.Models.MailModel _objModelMail)
        {

            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(_objModelMail.To);
                mail.From = new MailAddress(_objModelMail.From);
                mail.Subject = _objModelMail.Subject;
                string Body = _objModelMail.Body;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                ("teknikhogskolan@gmail.com", "plushogskolan");// Enter your User name and password
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return View("Index", _objModelMail);

            }
            else
            {
                return View();
            }
        }
    }
}