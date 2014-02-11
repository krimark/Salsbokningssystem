using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Salsbokningssystem.Filters;
using Salsbokningssystem.Models;
using System.Security.Principal;
using System.Web;
using System.IO;

namespace Salsbokningssystem.Controllers
{
    [Authorize(Roles = "Administratör")]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {


        public ActionResult Index()
        {
            List<Models.IndexViewModel> modelList = new List<IndexViewModel>();

            Models.DataClasses1DataContext db = new DataClasses1DataContext();
            var users = db.Users.ToList();

            foreach(var user in users)
            {
                Models.IndexViewModel model = new IndexViewModel();
                model.ID = user.ID;
                model.UserName = user.UserName;
                model.Role = Roles.GetRolesForUser(user.UserName).FirstOrDefault();
                model.Email = user.Email;
                model.Active = user.Active;
                modelList.Add(model);
            }
 
            return View(modelList);
        }

        public ActionResult Deactivate(int id)
        {
            Models.DataClasses1DataContext db = new DataClasses1DataContext();

            Models.User user = db.Users.FirstOrDefault(u => u.ID == id);
            user.Active = false;

            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Activate(int id)
        {
            Models.DataClasses1DataContext db = new DataClasses1DataContext();

            Models.User user = db.Users.FirstOrDefault(u => u.ID == id);
            user.Active = true;

            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Models.DataClasses1DataContext db = new DataClasses1DataContext();

            Models.EditUserModel user = db.Users.Where(i => i.ID == id).Select(u => new Models.EditUserModel
            {
                UserId = u.ID,
                UserName = u.UserName,
                Email = u.Email,
                Active = u.Active,
                NewPassword = "",
                ConfirmPassword = ""
            }).FirstOrDefault();

            user.Role = Roles.GetRolesForUser(user.UserName).FirstOrDefault();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserModel model)
        {
            if (ModelState.IsValid)
            {

                Models.DataClasses1DataContext db = new DataClasses1DataContext();

                Models.User user = db.Users.FirstOrDefault(f => f.ID == model.UserId);

                user.Email = model.Email;
                user.Active = model.Active;

                db.SubmitChanges();

                string currentRole = Roles.GetRolesForUser(user.UserName).FirstOrDefault();

                if (currentRole != model.Role)
                {
                    Roles.RemoveUserFromRole(user.UserName, currentRole);
                    Roles.AddUserToRole(user.UserName, model.Role);
                }

                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                if (model.NewPassword != null)
                {
                    var token = WebSecurity.GeneratePasswordResetToken(model.UserName);
                    try
                    {
                        //Reset password using the reset token and the new password
                        WebSecurity.ResetPassword(token, model.NewPassword);
                    }
                    catch (Exception)
                    {

                        ModelState.AddModelError("", "Det nuvarande lösenordet är fel eller det nya lösenordet är ogiltigt.");
                        return View(model);
                    }

                }

            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index");
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToAction("Index", "Booking");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Användarnamnet eller lösenordet är fel.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Login", "Account");
        }


        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            Models.RegisterModel model = new RegisterModel();
            return View(model);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email });

                    Roles.AddUserToRole(model.UserName, model.Role);

                    return RedirectToAction("Index");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Delete(string userName)
        {
            try
            {
                // TODO: Add delete logic here
                if (Roles.GetRolesForUser(userName).Count() > 0)
                {
                    Roles.RemoveUserFromRoles(userName, Roles.GetRolesForUser(userName));
                }

                ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(userName); // deletes record from webpages_Membership table
                ((SimpleMembershipProvider)Membership.Provider).DeleteUser(userName, true); // deletes record from UserProfile table

            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

        public ActionResult BatchFile()
        {


            return View();
        }

        [HttpPost]
        public ActionResult BatchFile(HttpPostedFileBase file)
        {
            //string selectedRole = form["Role"];
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                using (StreamReader sr = new StreamReader(file.InputStream))
                {
                    string fileText = sr.ReadToEnd();
                    //string[] lineValues = fileText.Trim().Split(',');
                    //string[] lineValues = fileText.Split(new[]{","}, StringSplitOptions.RemoveEmptyEntries);
                    string[] lineValues = fileText.Split(',').Select(a => a.Trim()).ToArray();
                    Models.BatchRegisterViewModel model = new Models.BatchRegisterViewModel();
                    for (int i = 0; i < lineValues.Length; i+=3)
                    {
                        Models.BatchRegisterModel user = new Models.BatchRegisterModel();
                        user.UserName = lineValues[i];
                        user.Password = lineValues[i+1];
                        user.Email = lineValues[i+2];

                        model.registerList.Add(user);
                    }

                    return View(model);
                    //do what you want with the file-text...
                }

            }

            return View();
        }

        [HttpPost]
        public ActionResult BatchRegister(Models.BatchRegisterViewModel model)
        {
            foreach (var user in model.registerList)
            {
                //if (ModelState.IsValid)
                //{
                    // Attempt to register the user
                    try
                    {

                        WebSecurity.CreateUserAndAccount(user.UserName, user.Password, new { Email = user.Email });

                        Roles.AddUserToRole(user.UserName, model.role);

                    }
                    catch (MembershipCreateUserException e)
                    {
                        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    }
                //}
            }

            return RedirectToAction("Index");
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Användarnmanet existerar redan. Ange ett annat användarnamn.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "En användare med den e-postadressen existerar redan. Ange en annan e-postadress.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Det angivna lösenordet är ogiltigt. Ange ett giltigt lösenord.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Den angivna e-postadressen är ogiltig. Kontrollera och försök igen.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Svaret på lösenordsfrågan är inkorrekt. Kontrollera och försök igen.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Den angivna lösenordsfrågan är inkorrekt. Kontrollera och försök igen.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Det angivna användarnamnet är fel. Kontrollera och försök igen.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
