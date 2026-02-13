using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using KalaERPApi.Service;
using KalaERPApi.Models.Request.Biotech.Trans;
using KalaERPApi.Service.Biotech.Trans;
using System.Web.Http.Cors;
using System.IO;
namespace KalaERPApi.Controllers.Biotech.Trans
{

    // [EnableCors(origins: "http://www.kalapms.com:8282", headers: "*", methods: "*")]
    public class ComplaintActiontakenController : ApiController
    {
        [HttpPost()]
        [Route("Biotech/ComplaintActionTaken/UploadActionAttachmnetFiles")]
        public string UploadActionAttachmnetFiles()
        {
            string StrTempPath = "";
            int iUploadedCnt = 0;

            // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.

            // sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");
            var request = System.Web.HttpContext.Current.Request;
            StrTempPath = "C:/TempERPFile/TempActionTakenfileBio" + "/" + request["FrmEcode"].ToString();

            if (Directory.Exists(StrTempPath))
            {
                foreach (string strFile in System.IO.Directory.GetFiles(StrTempPath, "*.*"))
                {
                    File.Delete(strFile);
                }
                // Directory.Delete(StrTempPath);
            }
            else
            {
                Directory.CreateDirectory(StrTempPath);
                Directory.GetAccessControl(StrTempPath);

            }

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


        [HttpPost()]
        [Route("Biotech/ComplaintActionTaken/UploadExpAttachmnetFiles")]
        public string UploadExpAttachmnetFiles()
        {
            string StrTempPath = "";
            int iUploadedCnt = 0;

            // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.

            // sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");
            var request = System.Web.HttpContext.Current.Request;
            StrTempPath = "C:/TempERPFile/TempActionTakenExpfileBio" + "/" + request["FrmExpEcode"].ToString();

            if (Directory.Exists(StrTempPath))
            {
                foreach (string strFile in System.IO.Directory.GetFiles(StrTempPath, "*.*"))
                {
                    File.Delete(strFile);
                }
                // Directory.Delete(StrTempPath);
            }
            else
            {
                Directory.CreateDirectory(StrTempPath);
                Directory.GetAccessControl(StrTempPath);
            }

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            var FrmExpEcode = request["FrmExpEcode"].ToString();
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

        [HttpGet]
        [Route("Biotech/ComplaintActionTaken/GetExpenditure")]
        public DataTable GetExpenditure()
        {
            DataTable ds = new DataTable();
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            ds = dc.GetExpenditure();
            return ds;
        }


        [HttpGet]
        [Route("Biotech/ComplaintActionTaken/GetMtlProcuct")]
        public DataTable GetMtlProcuct()
        {
            DataTable ds = new DataTable();
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            ds = dc.GetMtlProcuct();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/ComplaintActionTaken/getCGST")]
        public DataTable getCGST()
        {
            DataTable ds = new DataTable();
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            ds = dc.getCGST();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/ComplaintActionTaken/getSGST")]
        public DataTable getSGST()
        {
            DataTable ds = new DataTable();
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            ds = dc.getSGST();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/ComplaintActionTaken/getIGST")]
        public DataTable getIGST()
        {
            DataTable ds = new DataTable();
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            ds = dc.getIGST();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/ComplaintActionTaken/getTDS")]
        public DataTable getTDS()
        {
            DataTable ds = new DataTable();
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            ds = dc.getTDS();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/ComplaintActionTaken/GetProjectHead_Tech_Dealer")]
        public DataTable GetProjectHead(string Code, string Type)
        {
            DataTable ds = new DataTable();
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            ds = dc.GetProjectHead_Tech_Dealer(Code, Type);
            return ds;
        }

        [HttpGet]
        [Route("Biotech/ComplaintActionTaken/GetQtnPerandExpAmt")]
        public DataTable GetQtnPerandExpAmt(string Code)
        {
            DataTable ds = new DataTable();
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            ds = dc.GetQtnPerandExpAmt(Code);
            return ds;
        }


        [HttpGet]
        [Route("Biotech/ComplaintActionTaken/GetActionPendingComp")]
        public DataTable GetActionPendingComp(string Code)
        {
            DataTable ds = new DataTable();
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            ds = dc.GetActionPendingComp(Code);
            return ds;
        }

        [HttpGet]
        [Route("Biotech/ComplaintActionTaken/GetSelectedCompType")]
        public DataTable GetSelectedCompType(string Code)
        {
            DataTable ds = new DataTable();
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            ds = dc.GetSelectedCompType(Code);
            return ds;
        }

        [HttpPost]
        [Route("Biotech/ComplaintActionTaken/Submit")]
        public string Submit([FromBody] ComplaintActiontakenRequest ComplaintActiontakenreq)
        {
            string strMsg = "";
            ComplaintActiontakenCon dc = new ComplaintActiontakenCon();
            strMsg = dc.Submit(ComplaintActiontakenreq);
            return strMsg;
        }
    }
}
