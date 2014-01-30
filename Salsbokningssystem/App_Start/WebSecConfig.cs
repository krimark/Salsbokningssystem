using System.Web.Security;
using WebMatrix.WebData;

namespace Salsbokningssystem
{
    public static class WebSecConfig
    {
        public static void RegisterWebSec()
        {
            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "User", "Id", "UserName", true);

            if (!Roles.RoleExists("Administratör"))
            {
                Roles.CreateRole("Administratör");
            }

            if (!Roles.RoleExists("Lärare"))
            {
                Roles.CreateRole("Lärare");
            }

            if (!Roles.RoleExists("Användare"))
            {
                Roles.CreateRole("Användare");
            }

            if (!WebSecurity.UserExists("admin"))
            {
                WebSecurity.CreateUserAndAccount("admin", "password");
                Roles.AddUserToRole("admin", "Administratör");
            }
        }
    }
}