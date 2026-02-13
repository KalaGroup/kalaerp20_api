using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using KalaERPApi.Service;
using KalaERPApi.Models.Request.Corporate.Trans;
using KalaERPApi.Service.Corporate.Trans;
using  System.Web.Http.Cors;
using System.IO;
namespace KalaERPApi.Controllers.Corporate.Trans
{
   
   // [EnableCors(origins: "http://www.kalapms.com:8282", headers: "*", methods: "*")]
    public class CorporateReqFeedbackController : ApiController
    {

        [HttpGet]
        [Route("Corporate/CorporateReqFeedback/GetCPRTFeedbackPending")]
        public DataTable GetCrptReqActionPending(string Type, string ReqCode, string P_PCCode, string P_EmpCode, string P_FromDt, string P_ToDt)
        {
            DataTable ds = new DataTable();
            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
            ds = dc.GetCPRTFeedbackPending(Type, ReqCode, P_PCCode, P_EmpCode, P_FromDt, P_ToDt); 
            return ds;
        }

        [HttpGet]
        [Route("Corporate/CorporateReqFeedback/GetCPRTView")]
        public DataTable GetCPRTView(string Type, string ReqCode, string UserId)
        {
            DataTable ds = new DataTable();
            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
            ds = dc.GetCPRTView(Type, ReqCode, UserId);
            return ds;
        }

        //[Route("Corporate/CorporateReqFeedback/GetCorpReqActionPrvDtls")]
        //public DataTable GetCorpReqActionPrvDtls(string Code)
        //{
        //    DataTable ds = new DataTable();
        //    CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
        //    ds = dc.GetCorpReqActionPrvDtls(Code);
        //    return ds;
        //}


        [HttpGet]
        [Route("Corporate/CorporateReqFeedback/GetCorpReqFeedbackPrvDtls")]
        public DataTable GetCorpReqFeedbackPrvDtls(string Type, string ReqCode, string UserId)
        {
            DataTable ds = new DataTable();
            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
            ds = dc.GetCorpReqFeedbackPrvDtls(Type, ReqCode, UserId);
            return ds;
        }


        [HttpGet]
        [Route("Corporate/CorporateReqFeedback/GetCPRTFeedbackFile")]
        public DataTable GetCPRTReqFile(string Type, string ReqCode, string UserId)
        {
            DataTable ds = new DataTable();
            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
            ds = dc.GetCPRTFeedbackFile(Type, ReqCode, UserId);
            return ds;
        }

             
        [HttpPost()]
        [Route("Corporate/CorporateReqFeedback/UploadFiles")]
        public string UploadFiles()
        {
            string StrTempPath = "";
            int iUploadedCnt = 0;
            // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
            // sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");
            var request = System.Web.HttpContext.Current.Request;
            // StrTempPath = "C:/TempERPFile/TempCRPTReq" + "/" + request["FrmEcode"].ToString();
            StrTempPath = request["FileUploadTempPath"].ToString() + "/" + request["FrmEcode"].ToString();

            if (Directory.Exists(StrTempPath))
            {
                Directory.GetAccessControl(StrTempPath);
            }
            else
            {
                Directory.CreateDirectory(StrTempPath);
                Directory.GetAccessControl(StrTempPath);
            }

            if (request["FileUploadType"].ToString().Trim() == "Delete")
            {
                String filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
             return " file Deleted";
            }
            else
            //Save
            { 
            //if (Directory.Exists(StrTempPath))
            //{
            //    Directory.GetAccessControl(StrTempPath);

            //    //foreach (string strFile in System.IO.Directory.GetFiles(StrTempPath, "*.*"))
            //    //{
            //    //    File.Delete(strFile);
            //    //}
            //    // Directory.Delete(StrTempPath);
            //}
            //else
            //{
            //    Directory.CreateDirectory(StrTempPath);
            //    Directory.GetAccessControl(StrTempPath);
            //}

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            var FrmEcode = request["FrmEcode"].ToString();
            //  var fileUpload = request["fileUpload"].ToString();
            // CHECK THE FILE COUNT.
            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];
                if (hpf.ContentLength > 0)
                {
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (Delete)
                        if (File.Exists(StrTempPath + "/" + Path.GetFileName(hpf.FileName)))
                        {
                            File.Delete(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
                            // SAVE THE FILES IN THE FOLDER.
                        }

                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(StrTempPath + "/" + Path.GetFileName(hpf.FileName)))
                        {
                            // SAVE THE FILES IN THE FOLDER.
                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
                            iUploadedCnt = iUploadedCnt + 1;
                        }
                       
                }
            }
            // RETURN A MESSAGE.
            if (iUploadedCnt > 0)
            {
                return iUploadedCnt + " Files Uploaded Successfully";
            }
            else
            {
                return "Upload Failed";
            }
            }
        }

        [HttpPost]
        [Route("Corporate/CorporateReqFeedback/Submit")]
        public string Submit([FromBody] CorporateReqFeedbackRequest CorporateReqFeedback)
        {
            string strMsg = "";
            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
            strMsg = dc.Submit(CorporateReqFeedback);
            return strMsg;
        }
    }
}
