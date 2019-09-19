using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Taxtation.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(email));
                mail.Subject = subject;
                string EmailMsg = message;
                mail.Body = EmailMsg;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                mail.From = new MailAddress("datacogl@gmail.com", "Accountrak™ - ERP Alerts");
                smtp.Host = "smtp.gmail.com";
                smtp.UseDefaultCredentials = true;

            try
            {
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = "datacogl@gmail.com";
                    NetworkCred.Password = "Sirsyed45";
                

                smtp.Credentials = NetworkCred;
                smtp.EnableSsl = true;
                //smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("datacogl@gmail.com", "Sirsyed45");
                smtp.Port = 587;
                smtp.Timeout = 200000;
                //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; smtp.Send(mail);
                smtp.Send(mail);

                return Task.CompletedTask;


            }

            catch (Exception ex)
            {
                throw ex;
            }

        }


    }
}
