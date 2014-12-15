using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Net;
namespace DallasICA.Controllers
{
    public class PaymentController : Controller
    {
        #region Payment Misc
        [Authorize]
        public ActionResult PaymentMisc(string packageName, double price, string packageDesc)
        {
            Session["PackageName"] = packageName;
            Session["Price"] = price.ToString("N2");
            Session["PackageDesc"] = packageDesc;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult PaymentMisc()
        {
            Guid regID = Guid.NewGuid();
            string orderID = "0001";//PayPalHelper.GetOrderID();

            string packageName = Session["PackageName"].ToString();
            string price = Session["Price"].ToString();

            string message = string.Format("{0} | {1}", packageName, Session["PackageDesc"].ToString());

            Session["RegistrationID"] = regID;
            Session["Paypal_PackageName"] = message;
            Session["Paypal_Price"] = price;
            Session["Paypal_OrderId"] = orderID;
            return View("Payment");
        }
        #endregion

        public ActionResult Payment()
        {
            return View();
        }

        #region Payment Success

        public string Status, TransID, OrderID, Email, CurrencyType, Amount, ProfileId, TransDate, plannumber, planName;

        public ActionResult PaymentSuccess()
        {
            StringBuilder sb = new StringBuilder();
            if (Session["RegistrationID"] != null)
                sb.Append(" [RegistrationID: " + Session["RegistrationID"].ToString() + "]");
            if (Session["Paypal_OrderId"] != null)
                sb.Append(" [Paypal_OrderId: " + Session["Paypal_OrderId"].ToString() + "]");
            if (Session["Paypal_PackageName"] != null)
                sb.Append(" [Paypal_PackageName: " + Session["Paypal_PackageName"].ToString() + "]");


            string Payment_Status = string.Empty;
            if (Request.QueryString["payment_status"] != null)
                Payment_Status = Request.QueryString["payment_status"].ToString();

            if (Payment_Status != string.Empty)
            {
                sb.Append(" [payment_status: " + Payment_Status + "]");

                StringBuilder strReturnQstring = new StringBuilder();
                strReturnQstring.Append(Request.QueryString.ToString());
                strReturnQstring.Append("&cmd=_notify-validate");


                string PostMode = "2";
                string WebURL = string.Empty;
                string SdHost = string.Empty;
                
                if (PostMode == "1")
                {
                    WebURL = "http://www.paypal.com/cgi-bin/webscr";
                    SdHost = "www.paypal.com";
                }
                else if (PostMode == "2")
                {
                    WebURL = "https://www.paypal.com/cgi-bin/webscr";
                    SdHost = "www.paypal.com";
                }
                else
                {
                    Response.Write("PostMode: " + (PostMode) + "is invalid!");
                }

                
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(WebURL);
                myRequest.AllowAutoRedirect = false;
                myRequest.Method = "POST";
                myRequest.ContentType = "application/x-www-form-urlencoded";

                //Create post stream 
                Stream RequestStream = myRequest.GetRequestStream();
                byte[] SomeBytes = Encoding.UTF8.GetBytes(strReturnQstring.ToString());
                RequestStream.Write(SomeBytes, 0, SomeBytes.Length);
                RequestStream.Close();

                sb.Append(" [request sent]");

                //Send request and get response 
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    sb.Append(" [response: OK]");

                    //Get the stream. 
                    Stream ReceiveStream = myResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    //send the stream to a reader. 
                    StreamReader readStream = new StreamReader(ReceiveStream, encode);
                    //Read the result 
                    string Result = readStream.ReadLine();
                    if (Result == "INVALID")
                    {
                        sb.Append(" [result: INVALID]");

                        return View("PaymentCancel");
                    }
                    // The result was invalid so send a failure notice or some other handling. 
                    else if (Result == "VERIFIED")
                    {
                        sb.Append(" [result: VERIFIED]");

                        switch ((Payment_Status))
                        {
                            case "Completed":

                                if (Session["RegistrationID"] != null)
                                {
                                    //Guid regID = new Guid(Session["RegistrationID"].ToString());
                                   // m_bl.AddPaymentDetails(regID, Session["Paypal_OrderId"].ToString(), Session["Paypal_TeamName"].ToString(), Payment_Status, DateTime.Now);
                                }
                                else
                                {
                                    //Guid regID = Guid.Empty;
                                   // m_bl.AddPaymentDetails(regID, Session["Paypal_OrderId"].ToString(), Session["Paypal_TeamName"].ToString(), Payment_Status, DateTime.Now);
                                }

                                break;
                        }
                    }
                }
            }

           // m_bl.LogInfo("PayPalSuccess", sb.ToString());

            return View();

        }

        #endregion

        public ActionResult PaymentCancel()
        {
            StringBuilder sb = new StringBuilder();
            if (Session["RegistrationID"] != null)
                sb.Append(" [RegistrationID: " + Session["RegistrationID"].ToString() + "]");
            if (Session["Paypal_OrderId"] != null)
                sb.Append(" [Paypal_OrderId: " + Session["Paypal_OrderId"].ToString() + "]");
            if (Session["Paypal_PackageName"] != null)
                sb.Append(" [Paypal_PackageName: " + Session["Paypal_PackageName"].ToString() + "]");
            if (Session["Paypal_Price"] != null)
                sb.Append(" [Paypal_Price: " + Session["Paypal_Price"].ToString() + "]");

            if (sb.Length == 0)
                sb.Append("No Session data found!");

           // m_bl.LogInfo("PayPalCancel", sb.ToString());
            return View();
        }
    }
}