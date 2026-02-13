using System.Data;
using System.Web.Http;
using KalaERPApi.Models.Request.Accounts.Trans;
using KalaERPApi.Service.Accounts.Invoice.Trans;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using System.Net.Security;
using KalaERPApi.Service;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

namespace KalaERPApi.Controllers.Accounts.Invoice.Trans
{
    public class InvoiceScanController : ApiController
    {
        CommonCon ComCon = new CommonCon();
        InvScanCon dc = new InvScanCon();
        string strProc = "";

        [HttpGet]
        [Route("InvoiceScan/GetScanDts")]
        public DataTable GetScanDts1(string strSrNo)
        {
            return dc.GetScanDtsInv(strSrNo);
        }

        [HttpPost]
        [Route("InvoiceScan/Submit")]
        public string Submit(InvoiceScanRequest InvStageReq)
        {
            string status = dc.Submit(InvStageReq);
            SendEmail(InvStageReq.InvId.Trim());
            return status;
        }

        protected string SendEmail(string InvID)
        {
            string Status = "False", ToMailID = "", StrReplyTo = "", CCMailID = "", Body = "";
            DataSet dsDetails = null;
            DataSet dstemp = null;
            try
            {
                string query = "exec getGateOutDetails '" + InvID.Trim() + "'";
                dsDetails = ComCon.procDS(query, "tbl_GateOut");
                if (dsDetails.Tables["tbl_GateOut"].Rows.Count > 0)
                {
                    ToMailID = dsDetails.Tables["tbl_GateOut"].Rows[0]["HODMailID"].ToString().Trim();
                    CCMailID = dsDetails.Tables["tbl_GateOut"].Rows[0]["CCMailID"].ToString().Trim();
                    StrReplyTo = dsDetails.Tables["tbl_GateOut"].Rows[0]["ReplyToMailID"].ToString().Trim();

                    strProc = "";
                    strProc += "Select Fname+' '+Lname as AssignToEmpName, CmobileNo as AssignToMobileNo,CompmailID as AssignToMailID  ";
                    strProc += "from employee where ecode='" + dsDetails.Tables["tbl_GateOut"].Rows[0]["OrderBy"].ToString().Trim() + "' ";

                    dstemp = ComCon.procDS(strProc, "e");
                    if (dstemp.Tables["e"].Rows.Count > 0)
                    {

                        ToMailID = "";
                        ////For BLR
                        //if (dsDetails.Tables["tbl_GateOut"].Rows[0]["HODMailID"].ToString().Trim() == "dinesh.vyawahare@kalabiz.com" || dstemp.Tables["e"].Rows[0]["AssignToMailID"].ToString().Trim() == "dinesh.vyawahare@kalabiz.com")
                        //{
                        //    ToMailID = "harisha.sn@kalabiz.com";
                        //}
                        //else
                        //{
                        if (dsDetails.Tables["tbl_GateOut"].Rows[0]["HODMailID"].ToString().Trim() == dstemp.Tables["e"].Rows[0]["AssignToMailID"].ToString().Trim())
                        {
                            ToMailID = dsDetails.Tables["tbl_GateOut"].Rows[0]["HODMailID"].ToString().Trim();
                        }
                        else
                        {
                            ToMailID = dsDetails.Tables["tbl_GateOut"].Rows[0]["HODMailID"].ToString().Trim() + "," + dstemp.Tables["e"].Rows[0]["AssignToMailID"].ToString().Trim();
                        }
                        // }
                    }

                    //BLR
                    if (dsDetails.Tables["tbl_GateOut"].Rows[0]["HODMailID"].ToString().Trim() == "dinesh.vyawahare@kalabiz.com" || dstemp.Tables["e"].Rows[0]["AssignToMailID"].ToString().Trim() == "dinesh.vyawahare@kalabiz.com")
                    {
                        ToMailID = "";
                        ToMailID = "harisha.sn@kalabiz.com";
                    }

                    if (string.IsNullOrEmpty(ToMailID.ToString()))
                    {
                        ToMailID = "fs@kalabiz.com";
                    }

                    if (dsDetails.Tables["tbl_GateOut"].Rows[0]["PCName"].ToString().Trim() == "Akurdi - HO" ||
                    dsDetails.Tables["tbl_GateOut"].Rows[0]["PCName"].ToString().Trim() == "Goa" ||
                    dsDetails.Tables["tbl_GateOut"].Rows[0]["PCName"].ToString().Trim() == "Kolhapur" ||
                    dsDetails.Tables["tbl_GateOut"].Rows[0]["PCName"].ToString().Trim() == "Solapur")
                    {
                        ToMailID = ToMailID + ",br@kalabiz.com";
                    }
                    else if (dsDetails.Tables["tbl_GateOut"].Rows[0]["PCName"].ToString().Trim() == "Mkt Corporate")
                    {
                        ToMailID = ToMailID + ",aw@kalabiz.com";
                    }
                    else if (dsDetails.Tables["tbl_GateOut"].Rows[0]["PCName"].ToString().Trim() == "Bhopal")
                    {
                        ToMailID = ToMailID + ",sales.bhopal@kalabiz.com";
                    }
                    else if (dsDetails.Tables["tbl_GateOut"].Rows[0]["PCName"].ToString().Trim() == "Indore")
                    {
                        ToMailID = ToMailID + ",sales.indore@kalabiz.com";
                    }
                    else if (dsDetails.Tables["tbl_GateOut"].Rows[0]["PCName"].ToString().Trim() == "Pune")
                    {
                        ToMailID = ToMailID + ",pulse@kalabiz.com";
                    }

                    MailMessage m = new MailMessage();
                    SmtpClient sc = new SmtpClient();

                    m.From = new MailAddress("erp@kalabiz.com", "Kala Genset Pvt. Ltd. - ERP System");
                    //ToMail
                    string[] ToMailIDitems = Regex.Split(ToMailID.Trim(), ",");
                    HashSet<string> uniqueValues = new HashSet<string>();
                    foreach (string strTo in ToMailIDitems)
                    {
                        if (!uniqueValues.Contains(strTo))
                        {
                            uniqueValues.Add(strTo);
                            m.To.Add(new MailAddress(strTo));
                        }
                    }
                    //ToMail END

                    //CCMail 
                    string[] CCMailIDitems = Regex.Split(CCMailID.Trim(), ",");
                    foreach (string strCC in CCMailIDitems)
                    {
                        m.CC.Add(new MailAddress(strCC));
                    }
                    //CCMail End

                    //BCCMailID
                    //string BCCMailID = "praveenkumar@kalabiz.com,sanjaykumar@kalabiz.com"; Commented on 24/04/2025 by KB as per Discussion with FS.
                    string BCCMailID = "fs@kalabiz.com";
                    if (!string.IsNullOrEmpty(BCCMailID))
                    {
                        string[] arrCCMailID = Regex.Split(BCCMailID, ",");
                        foreach (string strBCC in arrCCMailID)
                        {
                            m.Bcc.Add(new MailAddress(strBCC));
                        }
                    }

                    Body = "<p style='color:#1F497D;'>Dear Sir/Madam,</p>";
                    Body += "<span style='color:#1F497D;'>Dispatch Details as Follows: </span><BR><BR>";

                    string htmlTableStartIndividual = "<table style=\"border-collapse:collapse; text-align:left;width:700px;\" >";
                    string htmlTableEndIndividual = "</table>";

                    string htmlTrStartIndividual = "<tr style =\"color:#555555;\">";
                    string htmlTrEndIndividual = "</tr>";
                    string htmlTdStartIndividual = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin;white-space: nowrap;padding-left:5px;\">";
                    string htmlTdEndIndividual = "</td>";

                    Body += htmlTableStartIndividual;
                    Body += htmlTrStartIndividual + htmlTdStartIndividual + "<b>MOF No: </b>" + dsDetails.Tables["tbl_GateOut"].Rows[0]["MOFCode"].ToString().Trim() + htmlTdEndIndividual + htmlTrEndIndividual;
                    Body += htmlTrStartIndividual + htmlTdStartIndividual + "<b>Invoice No: </b>" + dsDetails.Tables["tbl_GateOut"].Rows[0]["INVID"].ToString().Trim() + htmlTdEndIndividual + htmlTrEndIndividual;
                    Body += htmlTrStartIndividual + htmlTdStartIndividual + "<b>Invoice Date: </b>" + dsDetails.Tables["tbl_GateOut"].Rows[0]["Dt"].ToString().Trim() + htmlTdEndIndividual + htmlTrEndIndividual;
                    Body += htmlTrStartIndividual + htmlTdStartIndividual + "<b>Customer Name: </b>" + dsDetails.Tables["tbl_GateOut"].Rows[0]["CustName"].ToString().Trim() + htmlTdEndIndividual + htmlTrEndIndividual;
                    Body += htmlTrStartIndividual + htmlTdStartIndividual + "<b>Delivery Address: </b>" + dsDetails.Tables["tbl_GateOut"].Rows[0]["CustAddress"].ToString().Trim() + htmlTdEndIndividual + htmlTrEndIndividual;
                    Body += htmlTrStartIndividual + htmlTdStartIndividual + "<b>Product Description: </b>" + dsDetails.Tables["tbl_GateOut"].Rows[0]["PartDesc"].ToString().Trim() + htmlTdEndIndividual + htmlTrEndIndividual;
                    Body += htmlTrStartIndividual + htmlTdStartIndividual + "<b>Branch Name: </b>" + dsDetails.Tables["tbl_GateOut"].Rows[0]["PCName"].ToString().Trim() + htmlTdEndIndividual + htmlTrEndIndividual;
                    Body += htmlTrStartIndividual + htmlTdStartIndividual + "<b>Vehicle No: </b>" + dsDetails.Tables["tbl_GateOut"].Rows[0]["VehicleNo"].ToString().Trim() + htmlTdEndIndividual + htmlTrEndIndividual;
                    Body += htmlTrStartIndividual + htmlTdStartIndividual + "<b>Driver Name: </b>" + dsDetails.Tables["tbl_GateOut"].Rows[0]["DriverName"].ToString().Trim() + htmlTdEndIndividual + htmlTrEndIndividual;
                    Body += htmlTrStartIndividual + htmlTdStartIndividual + "<b>Driver Mobile No: </b>" + dsDetails.Tables["tbl_GateOut"].Rows[0]["DriverMobileNo"].ToString().Trim() + htmlTdEndIndividual + htmlTrEndIndividual;
                    Body += htmlTableEndIndividual;

                    Body += "<BR><span style='text-align: justify'><span style='color:#1F497D'>Thank You/Regards.</span></span><BR>";
                    Body += "<BR><span style='color:#1F497D'><b>Kala Genset Pvt. Ltd.</b></span>";
                    m.Subject = dsDetails.Tables["tbl_GateOut"].Rows[0]["MOFCode"].ToString().Trim() + " has been Dispatched on " + DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");

                    m.IsBodyHtml = true;
                    m.Body = Body;
                    m.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    if (!string.IsNullOrEmpty(StrReplyTo.ToString()))
                    {
                        string[] arrReplyToMailID = Regex.Split(StrReplyTo.ToString(), ",");
                        foreach (string strReplyTo in arrReplyToMailID)
                        {
                            m.ReplyToList.Add(new MailAddress(strReplyTo));
                        }
                    }

                    if (sc != null)
                    {
                        sc.UseDefaultCredentials = true;
                        sc.Credentials = new System.Net.NetworkCredential("erp@kalabiz.com", "wanv ftwc dobq blrr");
                        sc.Port = 587;
                        sc.Host = "smtp.gmail.com";
                        sc.EnableSsl = true;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        sc.Timeout = 999999999;
                        sc.ServicePoint.MaxIdleTime = 1;
                        sc.Send(m);
                    }
                    m.Dispose();
                    sc = null;
                    Status = "True";
                }
            }
            catch (Exception ex)
            {
                Status = "False, StackTrace " + ex.StackTrace + ", Message: " + ex.Message;
            }
            return Status;
        }

    }
}
