using Npgsql;
using System.Net;
using System.Net.Mail;

namespace Amnex_Project_Resource_Mapping_System.Repo.Classes
{
    public class Account
    {
        public void logout()
        {

        }

        private string GenerateOTP()
        {
            const string characters = "0123456789";
            Random random = new Random();
            char[] otp = new char[6];
            for (int i = 0; i < 6; i++)
            {
                otp[i] = characters[random.Next(characters.Length)];
            }
            return new string(otp);
        }

        internal void sendOTP(string to, HttpContext httpContext)
        {
            string otp = GenerateOTP();
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("connect.nextgentechnology@gmail.com", "tcllfvvxodcydksv");
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("connect.nextgentechnology@gmail.com");
                mailMessage.To.Add(to);
                mailMessage.Subject = "PRMS OTP verification";
                mailMessage.Body = otp;

                smtpClient.Send(mailMessage);

                storeOTP(otp, httpContext);
            }
        }

        private void storeOTP(string otp, HttpContext httpContext)
        {
            httpContext.Session.SetString("OTP", otp);
            httpContext.Session.SetString("OTPTime", DateTime.Now.ToString());
        }
        public void forgotPassword(string password, HttpContext httpContext, NpgsqlConnection connection)
        {
            using (var cmd = new NpgsqlCommand($"select changePassword({Convert.ToInt32(httpContext.Session.GetString("userId"))},'{password}')", connection))
            {
                cmd.ExecuteReader();
            }
        }

        public void login(Models.Login login)
        {
            throw new NotImplementedException();
        }

        public bool changePassword(string currentPassword, string newPassword, HttpContext httpContext, NpgsqlConnection connection)
        {
            using (var cmd = new NpgsqlCommand($"SELECT changePassword({Convert.ToInt32(httpContext.Session.GetString("userId"))},'{currentPassword}','{newPassword}');", connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetBoolean(0);

                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}
