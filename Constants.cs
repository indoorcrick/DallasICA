using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DallasICA
{
    public class Constants
    {
        public static List<SelectListItem> GENDER = new List<SelectListItem>() 
        { 
            //new SelectListItem { Text = "Not Set", Value = "Not Set" }, 
            new SelectListItem { Text = "MALE", Value = "MALE" }, 
            new SelectListItem { Text = "FEMALE", Value = "FEMALE" } 
        };
    }
}
