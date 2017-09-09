using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

using QUANLYTIEC.Models;
using QUANLYTIEC.Models.BUS;

namespace QUANLYTIEC.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/
        public bool CheckPermission()
        {
            return (Session["UserID"] == null) ? false : true;

        }

       
        public bool Role(int ActionType)
        {
            SYS_USER item = new SYS_USER
            {
                IsConfig = false,
                IsRegisterParty = false,
                IsMaterial = false,
                IsAttendance = false,
                IsList = false,
                IsReport = false

            };
            if (Session["UserID"] != null)
            {
                int UserID = int.Parse(Session["UserID"].ToString());
                bool IsAdmin = false;
                bool.TryParse(Session["IsAdmin"].ToString(), out IsAdmin);

                item = DA_User.Instance.GetPermissionByFunction(UserID, IsAdmin);

            }
            ViewBag.View = item.IsConfig;
            ViewBag.Add = item.IsRegisterParty;
            ViewBag.Edit = item.IsMaterial;
            ViewBag.Delete = item.IsAttendance;
            ViewBag.Print = item.IsList;
            ViewBag.Print = item.IsReport;

            switch (ActionType)
            {
                case 1:
                    {
                        return item.IsConfig;
                    }
                case 2:
                    {
                        return item.IsRegisterParty;
                    }
                case 3:
                    {
                        return item.IsMaterial;
                    }
                case 4:
                    {
                        return item.IsAttendance;
                    }
                case 5:
                    {
                        return item.IsList;
                    }
                case 6:
                    {
                        return item.IsReport;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public string Encrypte(string pString)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(pString));
            string sEncryptPass = Convert.ToBase64String(hashedDataBytes);
            return sEncryptPass;
        }



    }

}
