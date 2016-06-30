using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auth.Models;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Models
{
    public class AccountController : Controller
    {
        BenutzerdatenverwaltungEntities _db = new BenutzerdatenverwaltungEntities();

        public ActionResult Logout()
        {
            Session.Remove("username");
            Session.Remove("role");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if(EmailExist(username))
            {
                var usernameQuery = from d in _db.users
                                    where d.email == username
                                    select d;

                foreach(var item in usernameQuery)
                {
                    username = item.username;
                }
            }

            //Wenn Email oder username in der db vorkommen:
            if(UsernameExist(username))
            {
                string saltFromDB = null;
                string passwordFromDB = null;
                string roleFromDB = null;

                var loginQuery = from d in _db.users
                                 where d.username == username
                                 select d;

                foreach(var item in loginQuery)
                {
                    saltFromDB = item.salt;
                    passwordFromDB = item.password;
                    roleFromDB = item.role;
                }

                //Überprüft das Passwort:
                if(CreateHash(password, saltFromDB) == passwordFromDB)
                {
                    Session["username"] = username.ToLower();
                    if(!String.IsNullOrWhiteSpace(roleFromDB))
                    {
                        Session["role"] = roleFromDB;
                    }
                    Session.Timeout = 30;
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string email, string username, string password1, string password2)
        {
            //Überprüft ob die Email in der DB existiert:
            if(EmailExist(email))
            {
                //Stimmen die Passworter überein wird die Registrierung ausgeführt
                if (password1 == password2 && !String.IsNullOrEmpty(password2))
                {

                    users u = new users();

                    //Erstellt ein zufälligen Salt in Hex 24 bytes lang
                    string salt = CreateSalt();

                    //Erstellt einen hash aus Salt + Password 16 Bytes lang
                    string generatedPassword = CreateHash(password1, salt);

                    var query = from d in _db.users
                                where d.email == email
                                select d;

                    foreach(var item in query)
                    {
                        item.username = username;
                        item.email = email;
                        item.salt = salt;
                        item.password = generatedPassword;
                    }

                    //Object in Datenbank speichern
                    _db.SaveChanges();
                }
            }

            return View();
        }

        public const int salt_byte_size = 24;

        public string CreateSalt()
        {
            //Random Salt generieren
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[salt_byte_size];
            csprng.GetBytes(salt);

            //Byte[] als String in Hex wieder zusammenführen
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < salt.Length; i++)
            {
                sb.Append(salt[i].ToString("x2"));
            }

            return sb.ToString();
        }

        public string CreateHash(string password, string str_salt)
        {
            //Passwort hashen md5(Salt + Passwort)
            byte[] hash;

            using (MD5 md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str_salt + password));
            }

            //Byte[] als String in Hex wieder zusammenführen
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        public Boolean EmailExist(string email)
        {

            //Überprüft ob email in der DB vorhanden ist
            if (_db.users.Any(o => o.email == email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean UsernameExist(string username)
        {

            //Überprüft ob email in der DB vorhanden ist
            if (_db.users.Any(o => o.username == username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetRole(string username)
        {
            string role = null;

            var roleQuery = from d in _db.users
                            where d.username == username
                            select d;

            if(roleQuery != null)
            {
                foreach (var item in roleQuery)
                {
                    role = item.role;
                }
            }

            return role;
        }
	}
}