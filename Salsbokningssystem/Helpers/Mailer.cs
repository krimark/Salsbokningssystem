using System.Net.Mail;

namespace Salsbokningssystem.Helpers
{
    public static class Mailer
    {
        public static void SendMail(string recipient, string body, string subject)
        {
            if (recipient != null)
            {
                var mail = new MailMessage();
                mail.To.Add(recipient);
                mail.From = new MailAddress("teknikhogskolan@gmail.com");
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential
                        ("teknikhogskolan@gmail.com", "plushogskolan"),
                    EnableSsl = true
                };
                smtp.Send(mail);
            }
        }
    }
}