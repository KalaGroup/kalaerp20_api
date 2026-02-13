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
using System.Web.Http.Cors;
using System.IO;
namespace KalaERPApi.Controllers.Corporate.Trans
{

    // [EnableCors(origins: "http://www.kalapms.com:8282", headers: "*", methods: "*")]
    public class CorporateReqController : ApiController
    {


        //[HttpGet]
        //[Route("Corporate/CorporateReqAction/GetCPRTActionPending")]
        //public DataTable GetCrptReqActionPending(string Type, string ReqCode, string P_PCCode, string P_EmpCode, string P_FromDt, string P_ToDt)
        //{
        //    DataTable ds = new DataTable();
        //    CorporateReqCon dc = new CorporateReqCon();
        //    ds = dc.GetCPRTActionPending(Type, ReqCode, P_PCCode, P_EmpCode, P_FromDt, P_ToDt); 
        //    return ds;
        //}


        [HttpGet]
        [Route("Corporate/CorporateReq/GetToProfitCenter")]
        public DataTable GetToProfitCenter()
        {
            DataTable ds = new DataTable();
            CorporateReqCon dc = new CorporateReqCon();
            ds = dc.GetToProfitCenter();
            return ds;
        }

     



        [HttpPost()]
        [Route("Corporate/CorporateReq/UploadFiles")]
        public string UploadFiles()
        {
            string StrTempPath = "";
            int iUploadedCnt = 0;
            // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
            // sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");
            var request = System.Web.HttpContext.Current.Request;
            StrTempPath = "C:/TempERPFile/TempCRPTReq" + "/" + request["FrmEcode"].ToString();

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
            {
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                var FrmEcode = request["FrmEcode"].ToString();
                //  var fileUpload = request["fileUpload"].ToString();
                // CHECK THE FILE COUNT.
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    if (hpf.ContentLength > 0)
                    {
                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                        if (!File.Exists(StrTempPath + Path.GetFileName(hpf.FileName)))
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
        [Route("Corporate/CorporateReq/Submit")]
        public string Submit([FromBody] CorporateReqRequest CorporateReqreq)
        {
            string strMsg = "";
            CorporateReqCon dc = new CorporateReqCon();
            strMsg = dc.Submit(CorporateReqreq);
            return strMsg;
        }

        //Add new 28/10/25

        [HttpGet]
        [Route("Corporate/CorporateReq/GetToEmpNamePCCode")]
        public DataTable GetToEmpNamePCCode()
        {
            DataTable ds = new DataTable();
            CorporateReqCon dc = new CorporateReqCon();
            ds = dc.GetToEmpNamePCCode();
            return ds;
        }
        //End

    }
}
