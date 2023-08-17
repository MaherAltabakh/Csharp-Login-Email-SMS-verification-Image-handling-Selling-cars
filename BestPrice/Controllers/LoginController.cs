using BestPrice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text;
using Vonage;
using Vonage.Request;


namespace BestPrice.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login(User user)
        {
            ViewBag.LoginErrorMessage = TempData["LoginErrorMessage"];
            ViewBag.Email = user.Email;
            return View();
        }

        public IActionResult LoginAction(string email, string password)
        {
            if (email != null)
            {
                UserDB userDB = new UserDB();
                User user = userDB.signIn(email, password);
                if (user.UserID == 0)
                {
                    user.Email = email;
                    TempData["LoginErrorMessage"] = "Email and password did not match any user";
                    return RedirectToAction("Login", user);
                }
                else
                {
                    HttpContext.Session.SetString("UserID", user.UserID.ToString());
                    HttpContext.Session.SetString("FistName", user.FirstName);
                    HttpContext.Session.SetString("LastName", user.LastName);
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("Phone", user.Phone);
                    return RedirectToAction("TFAChoice", user);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult TFAChoice(User user)
        {
            ViewBag.User = user;
            return View();
        }

        public IActionResult TFA(string TFAMethod)
        {
            if (TempData["TFAerrorMessage"] != null)
            {
                ViewBag.TFANotMatch = TempData["TFAerrorMessage"];
            }
            else
            {
                Random random = new Random();
                string TFA = random.Next(111111, 999999).ToString();
                HttpContext.Session.SetString("code", TFA);
                if (TFAMethod == "Email")
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("Best.Price357951@gmail.com", "Best Price Dealership");
                        mail.To.Add(HttpContext.Session.GetString("Email"));
                        mail.Subject = "Secure two-step verification notification";
                        mail.Body = string.Format("<html><head></head><body>" +
                                               "You have requested a secure verification code to log into your Best Price Account. <br>" +
                                               "Please enter this secure verification code: <strong>" + TFA + "</strong> <br>" +
                                               "<br> <strong> PLEASE DO NOT REPLY TO THIS MESSAGE </strong>" +
                                               "</body>");
                        mail.IsBodyHtml = true;
                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential("Best.Price357951@gmail.com", "jexksucjzopedfjp");
                            smtp.EnableSsl = true;
                            try
                            {
                                smtp.Send(mail);
                                HttpContext.Session.SetString("TFATimer", DateTime.Now.ToString());
                            }
                            catch (SmtpException e)
                            {
                                ViewBag.TFAEmailNotSentError = e.ToString();
                            }
                        }
                    }
                }
                else if (TFAMethod == "SMS")
                {
                    var credentials = Credentials.FromApiKeyAndSecret("3b087c2d", "8xvCoP1XQ8T8vswg");
                    var VonageClient = new VonageClient(credentials);
                    var phone = HttpContext.Session.GetString("Phone");
                    var response = VonageClient.SmsClient.SendAnSms(new Vonage.Messaging.SendSmsRequest()
                    {
                        To = phone,
                        From = "16084030003",
                        Text = "Best Priced text Code : " + TFA + " Valid for 2 min"
                    });
                }
            }
            return View();
        }

        public IActionResult TFAValidation(string Code)
        {
            TimeSpan span = DateTime.Now.Subtract(DateTime.Parse(HttpContext.Session.GetString("TFATimer")));
            if (span.TotalMinutes > 2)
            {
                TempData["TFAerrorMessage"] = "Verification code is EXPIRED. Please request a new verification code.";
                return RedirectToAction("TFA");
            }
            else if (HttpContext.Session.GetString("code") == Code)
            {
                HttpContext.Session.SetString("isLoggedIn", "True");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["TFAerrorMessage"] = "Verification code is INVALID. Please try again or request a new verification code.";
                return RedirectToAction("TFA");
            }
        }


        public IActionResult ForgetPassword()
        {
            if (TempData["NewPasswordExpired"] != null)
                ViewBag.NewPasswordExpired = TempData["NewPasswordExpired"];
            if (TempData["NoMatching"] != null)
                ViewBag.NoMatching = TempData["NoMatching"];
            return View();
        }


        public IActionResult ResetPassword(string EmailResetPassword)
        {

            UserDB userDB = new UserDB();
            User user = userDB.LookForUserByEmail(EmailResetPassword);
            if (user.UserID == 0 && TempData["NewPasswordInvalid"] != null)
            {
                ViewBag.NewPasswordInvalid = TempData["NewPasswordInvalid"];
            }
            else if (user.UserID == 0)
            {
                TempData["NoMatching"] = "Please enter a correct Email address!!!";
                return RedirectToAction("ForgetPassword");
            }
            else
            {
                HttpContext.Session.SetString("thisUser", user.Email);
                const string valid = "!@#$?_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder resetPassword = new StringBuilder();
                Random random = new Random();
                int length = 15;
                while (0 < length--)
                {
                    resetPassword.Append(valid[random.Next(valid.Length)]);
                }
                HttpContext.Session.SetString("ResetPass", resetPassword.ToString());

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("Best.Price357951@gmail.com", "Best Price Dealership");
                    mail.To.Add(user.Email);
                    mail.Subject = "Reset the Password";
                    mail.Body = string.Format("<html><head></head><body>" +
                                           "The temporary password is <strong>" + resetPassword.ToString() + "</strong>" +
                                           "</body>");
                    mail.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("Best.Price357951@gmail.com", "jexksucjzopedfjp");
                        smtp.EnableSsl = true;
                        try
                        {
                            smtp.Send(mail);
                            HttpContext.Session.SetString("ResetPasswordTimer", DateTime.Now.ToString());
                        }
                        catch (SmtpException e)
                        {
                            ViewBag.TFAEmailNotSentError = e.ToString();
                        }
                    }
                }
            }
            return View();
        }


        public IActionResult ResetPasswordValidation(string TempPassword)
        {
            TimeSpan span = DateTime.Now.Subtract(DateTime.Parse(HttpContext.Session.GetString("ResetPasswordTimer")));
            if (span.TotalMinutes > 2)
            {
                TempData["NewPasswordExpired"] = "Temporary password is EXPIRED. Please request a new one!!!";
                return RedirectToAction("ForgetPassword");
            }
            else if (HttpContext.Session.GetString("ResetPass") == TempPassword)
            {
                return RedirectToAction("SetNewPAssword");
            }
            else
            {
                TempData["NewPasswordInvalid"] = "Temporary password is INVALID. Please try again or request a new one!!!";
                return RedirectToAction("ResetPassword");
            }
        }

        public IActionResult SetNewPAssword()
        {
            if (TempData["NewPasswordNotMatch"] != null)
                ViewBag.NewPasswordNotMatch = TempData["NewPasswordNotMatch"];
            return View();
        }


        public IActionResult UpdateNewPassword(string NewPassword, string ConfirmNewPassword)
        {
            if (NewPassword != ConfirmNewPassword)
            {
                TempData["NewPasswordNotMatch"] = "Please make sure to enter matches passwords!!!";
                return RedirectToAction("SetNewPAssword");
            }
            else
            {
                UserDB userDB = new UserDB();
                userDB.UpdatePassword(NewPassword, HttpContext.Session.GetString("thisUser"));
            }
            return RedirectToAction("Login");
        }


        public IActionResult LogoutAction()
        {
            HttpContext.Session.SetString("isLoggedIn", "False");
            return View("Login");
        }

        public IActionResult CreateUpdateUser(User user)
        {
            if (HttpContext.Session.GetString("UserID") != null)//For update the user
            {
                UserDB userDB = new UserDB();
                user = userDB.LookForUserByEmail(HttpContext.Session.GetString("Email"));
            }
            ViewBag.errorMessage = TempData["errorMessage"];
            ViewBag.User = user;
            return View();
        }

        public IActionResult SaveUpdateUser(User user)
        {
            List<string> errorMessages = new List<string>();
            UserDB userDB = new UserDB();

            if (user.FirstName == null)
                errorMessages.Add("First Name is required");
            if (user.LastName == null)
                errorMessages.Add("Last Name is required");
            if (user.AddressLine1 == null)
                errorMessages.Add("Address Line 1 is required");
            if (user.City == null)
                errorMessages.Add("City is required");
            if (user.State == null)
                errorMessages.Add("State is required");
            if (user.ZipCode == null)
                errorMessages.Add("ZipCode is required");
            if (user.Phone == null)
                errorMessages.Add("Phone is required");
            if (user.Email == null)
                errorMessages.Add("Email is required");
            if (user.Password == null)
                errorMessages.Add("Password is required");
            //if((userDB.LookForUserByEmail(user.Email).Email.ToLower() == user.Email.ToLower()))
            if (HttpContext.Session.GetString("UserID") == null)//To make sure there is No login as new user
            {
                if (userDB.LookForUserByEmail(user.Email).UserID != 0)//To make sure this email is Not existed as a new user
                    errorMessages.Add("This Email is already registered. Please choose another Email");
            }
            else
            {
                user.UserID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
                //The user want to use the same email
                User testUser = userDB.LookForUserByEmail(user.Email);
                if (testUser.UserID != 0)//To make sure this email is Not existed as an updated email
                    if (testUser.UserID != user.UserID)//To make sure that the existed email is not belong to this user, and he want to keep his email as it is
                        errorMessages.Add("This Email is already registered. Please choose another Email");
            }
            if (errorMessages.Count > 0)
            {
                TempData["errorMessage"] = errorMessages;
                //here we are redirecting to the above action and pass the mistaken object to send it back to the HTML page to be edited
                return RedirectToAction("CreateUpdateUser", user);
            }
            else
                userDB.UserEditSave(user);
            if (HttpContext.Session.GetString("UserID") != null)//To make sure there is No login as new user
                return RedirectToAction("Index", "Home");
            else
                return View("Login", user);
        }

        public IActionResult ConfirmDeletAccount()
        {
            ViewBag.Delete = "1";
            return View("Login");
        }


        public IActionResult DeletAccount(string email, string password)
        {

            if (email != null)
            {
                UserDB userDB = new UserDB();
                User user = userDB.signIn(email, password);
                if (user.UserID == 0)
                {
                    TempData["LoginErrorMessage"] = "Email and password did not match any user";
                    HttpContext.Session.SetString("isLoggedIn", "False");
                    return RedirectToAction("Login");
                }
                else
                {
                    if (HttpContext.Session.GetString("UserID") != null)//To make sure there is No login as new user
                    {
                        UserDB userDb = new UserDB();
                        userDb.DeletAccount(HttpContext.Session.GetString("UserID"));
                    }
                    return View("Login");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}
