using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Salsbokningssystem.Filters;
using Salsbokningssystem.Models;
using System.Security.Principal;

namespace Salsbokningssystem.Controllers
{
    [Authorize(Roles = "Administratör")]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {


        public ActionResult Index()
        {
            Models.DataClasses1DataContext db = new DataClasses1DataContext();
            var users = db.Users.ToList();

            return View(users);
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

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserModel model)
        {
            if (ModelState.IsValid)
            {

                //MembershipUser mUser = Membership.GetUser(model.UserName);

                Models.DataClasses1DataContext db = new DataClasses1DataContext();

                Models.User user = db.Users.FirstOrDefault(f => f.ID == model.UserId);

                user.Email = model.Email;
                user.Active = model.Active;

                db.SubmitChanges();

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
            return View();
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

                    Roles.AddUserToRole(model.UserName, "Användare");
                    //WebSecurity.Login(model.UserName, model.Password);
                    //MembershipUser mUser = Membership.GetUser(model.UserName);

                    //int userID = (int)mUser.ProviderUserKey;

                    //if (model.Email != null)
                    //{
                    //    Models.DataClasses1DataContext db = new DataClasses1DataContext();

                    //    Models.User user = db.Users.FirstOrDefault(f => f.ID == userID);

                    //    user.Email = model.Email;

                    //    db.SubmitChanges();
                    //}

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
