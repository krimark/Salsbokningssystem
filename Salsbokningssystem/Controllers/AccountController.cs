using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Salsbokningssystem.Helpers;
using WebMatrix.WebData;
using Salsbokningssystem.Filters;
using Salsbokningssystem.Models;
using System.Web;
using System.IO;

namespace Salsbokningssystem.Controllers
{

    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        readonly Random rand = new Random();
        DataClasses1DataContext db = new DataClasses1DataContext();

        [Authorize(Roles = "Administratör")]
        public ActionResult Index(string searchString, string orderBy)
        {
            var modelList = new List<AccountIndexViewModel>();

            var users = db.Users.Where(u => u.UserName.Contains(searchString) || u.Email.Contains(searchString) || searchString == null).ToList();

            foreach (var user in users)
            {
                var model = new AccountIndexViewModel();
                model.ID = user.ID;
                model.UserName = user.UserName;
                model.Role = Roles.GetRolesForUser(user.UserName).FirstOrDefault();
                model.Email = user.Email;
                model.UserGroup = user.UserGroup;
                model.Active = user.Active;
                modelList.Add(model);
            }

            var sortedList = new List<AccountIndexViewModel>();

            if (orderBy != null)
            {
                switch (orderBy)
                {
                    case "Användarnamn":
                        sortedList = modelList.OrderBy(u => u.UserName).ToList();
                        break;
                    case "Behörighet":
                        sortedList = modelList.OrderBy(u => u.Role).ToList();
                        break;
                    case "Epost":
                        sortedList = modelList.OrderBy(u => u.Email).ToList();
                        break;
                    case "Grupp":
                        sortedList = modelList.OrderBy(u => u.UserGroup).ToList();
                        break;
                }
            }
            else
            {
                sortedList = modelList;
            }

            if (searchString != null || orderBy != null)
                return PartialView("_UsersPartial", sortedList);

            return View(sortedList);
        }

        [Authorize(Roles = "Administratör")]
        public ActionResult Deactivate(int id)
        {
            var user = db.Users.FirstOrDefault(u => u.ID == id);
            if (user != null) user.Active = false;

            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administratör")]
        public ActionResult Activate(int id)
        {

            var user = db.Users.FirstOrDefault(u => u.ID == id);
            if (user != null) user.Active = true;

            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administratör")]
        public ActionResult Edit(int id)
        {
            var user = db.Users.Where(i => i.ID == id).Select(u => new EditUserModel
            {
                UserId = u.ID,
                UserName = u.UserName,
                Email = u.Email,
                Active = u.Active,
                NewPassword = "",
                ConfirmPassword = "",
                UserGroup = u.UserGroup

            }).FirstOrDefault();

            if (user != null)
            {
                user.Role = Roles.GetRolesForUser(user.UserName).FirstOrDefault();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administratör")]
        public ActionResult Edit(EditUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(f => f.ID == model.UserId);

                if (user != null)
                {
                    user.Email = model.Email;
                    user.Active = model.Active;
                    user.UserGroup = model.UserGroup;

                    db.SubmitChanges();

                    string currentRole = Roles.GetRolesForUser(user.UserName).FirstOrDefault();

                    if (currentRole != model.Role)
                    {
                        Roles.RemoveUserFromRole(user.UserName, currentRole);
                        Roles.AddUserToRole(user.UserName, model.Role);
                    }
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
        [Authorize(Roles = "Administratör")]
        public ActionResult Register()
        {
            var model = new RegisterModel();
            return View(model);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administratör")]
        public ActionResult Register(RegisterModel model)
        {

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email, UserGroup = model.UserGroup });

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

        [Authorize(Roles = "Administratör")]
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
                Membership.Provider.DeleteUser(userName, true); // deletes record from UserProfile table

            }
            catch (Exception)
            {
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administratör")]
        public ActionResult BatchFile()
        {

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administratör")]
        public ActionResult BatchFile(HttpPostedFileBase file)
        {

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                using (var sr = new StreamReader(file.InputStream, System.Text.Encoding.Default))
                {
                    string fileText = sr.ReadToEnd();
                    string[] lineValues = fileText.Split(new[] { ';', '\r' })
                        .Select(a => a.Trim()).ToArray();

                    // Remove last line if its empty
                    int nrOfLines = lineValues.Length;
                    if (lineValues[lineValues.Length - 1] == "")
                        nrOfLines--;

                    var model = new BatchRegisterViewModel();
                    for (int i = 0; i < nrOfLines; i += 6)
                    {
                        var user = new BatchRegisterModel();
                        user.UserName = GenerateUserName(lineValues[i], lineValues[i + 1]);
                        user.Password = GeneratePassword(8);
                        user.Email = lineValues[i + 4];
                        user.UserGroup = lineValues[i + 5];
                        model.registerList.Add(user);
                    }

                    return View(model);
                }
            }

            return View();
        }

        private string GeneratePassword(int length)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var chars = new char[length];
            //RandomGen2 random = new RandomGen2();
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rand.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }

        private string GenerateUserName(string lName, string fName)
        {
            int n = fName.IndexOf(" ", StringComparison.Ordinal);

            if (n > 0)
                fName = fName.Substring(0, n);

            if (lName.Length > 6)
                lName = lName.Substring(0, 6);

            return fName + "." + lName;
        }
        [HttpPost]
        [Authorize(Roles = "Administratör")]
        public ActionResult BatchRegister(BatchRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var user in model.registerList)
                {
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

                }
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RecoverPassword(string userName)
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == userName);
            if (user == null)
            {
                TempData["Message"] = "Användarnamnet du angav existerar inte.";
            }
            else
            {
                var token = WebSecurity.GeneratePasswordResetToken(userName);

                var resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { un = userName, rt = token }, "http") + "'>Återställ lösenord</a>";

                var body = "<b>Någon (du?) har angett att du vill få ett nytt lösenord.<br />Om du inte vill ha ett nytt lösenord så ignorerar du länken nedan, annars klickar du på den.</b><br/>" + resetLink;

                Mailer.SendMail(user.Email, body, "Återställning av lösenord.");

                TempData["Message"] = "Meddelande skickat.";
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string un, string rt)
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == un);

            if (user != null)
            {
                var newpassword = GeneratePassword(8);
                var response = WebSecurity.ResetPassword(rt, newpassword);
                if (response)
                {
                    var body = "<b>Ditt nya lösenord</b><br/>" + newpassword;
                    Mailer.SendMail(user.Email, body, "Ditt lösenord har ändrats");
                    TempData["Message"] = "Ditt nya lösenord har skickats till din e-postadress.";
                }
                else
                {
                    TempData["Message"] = "Nothing to see here...";
                }
            }
            return View();
        }

        //
        // GET: /Account/Manage
        [Authorize]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Ditt lösenord har ändrats"
                : "";
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Manage(LocalPasswordModel model)
        {

            ViewBag.ReturnUrl = Url.Action("Manage");
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                ModelState.AddModelError("", "Det nuvarande eller det nya lösenordet är felaktigt.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #region Helpers

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
