//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.Data;
//using KalaERPApi.Service;
//using KalaERPApi.Models.Request.Corporate.Trans;
//using KalaERPApi.Service.Corporate.Trans;
//using  System.Web.Http.Cors;
//using System.IO;
//namespace KalaERPApi.Controllers.Corporate.Trans
//{

//   // [EnableCors(origins: "http://www.kalapms.com:8282", headers: "*", methods: "*")]
//    public class CorporateReqFeedbackController : ApiController
//    {

//        [HttpGet]
//        [Route("Corporate/CorporateReqAction/GetCPRTFeedbackPending")]
//        public DataTable GetCPRTFeedbackPending(string Type, string ReqCode, string P_PCCode, string P_EmpCode, string P_FromDt, string P_ToDt)
//        {
//            DataTable ds = new DataTable();
//            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
//            ds = dc.GetCPRTFeedbackPending(Type, ReqCode, P_PCCode, P_EmpCode, P_FromDt, P_ToDt); 
//            return ds;
//        }

//        [HttpGet]
//        [Route("Corporate/CorporateReqAction/GetCPRTActionView")]
//        public DataTable GetCPRTActionView( string Type, string ReqCode, string UserId)
//        {
//            DataTable ds = new DataTable();
//            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
//            ds = dc.GetCPRTActionView(Type,ReqCode, UserId);
//            return ds;
//        }

//        [Route("Corporate/CorporateReqAction/GetCorpReqActionPrvDtls")]
//        public DataTable GetCorpReqActionPrvDtls(string Code)
//        {
//            DataTable ds = new DataTable();
//            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
//            ds = dc.GetCorpReqActionPrvDtls(Code);
//            return ds;
//        }


//        [HttpGet]
//        [Route("Corporate/CorporateReqAction/GetCPRTReqFile")]
//        public DataTable GetCPRTReqFile(string Type, string ReqCode, string UserId)
//        {
//            DataTable ds = new DataTable();
//            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
//            ds = dc.GetCPRTReqFile(Type, ReqCode, UserId);
//            return ds;
//        }

//        [HttpGet]
//        [Route("Corporate/CorporateReqFeedback/GetCPRTActionFile")]
//        public DataTable GetCPRTActionFile(string Type, string ReqCode, string UserId)
//        {
//            DataTable ds = new DataTable();
//            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
//            ds = dc.GetCPRTActionFile(Type, ReqCode, UserId);
//            return ds;
//        }


//        [Route("Corporate/CorporateReqFeedback/GetCopReqProblemHead")]
//        public DataTable GetCopReqProblemHead(string PCCode)
//        {
//            DataTable ds = new DataTable();
//            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
//            ds = dc.GetCopReqProblemHead(PCCode);
//            return ds;
//        }

//        [HttpGet]
//        [Route("Corporate/CorporateReqFeedback/GetCPRTActionAssignToEmp")]
//        public DataTable GetCPRTActionAssignToEmp()
//        {
//            DataTable ds = new DataTable();
//            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
//            ds = dc.GetCPRTActionAssignToEmp();
//            return ds;
//        }


//        [Route("Corporate/CorporateReqFeedback/GetExpEmpOrSupp")]
//        public DataTable GetExpEmpOrSupp(string Type)
//        {
//            DataTable ds = new DataTable();
//            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
//            ds = dc.GetExpEmpOrSupp(Type);
//            return ds;
//        }


//        [HttpGet]
//        [Route("Corporate/CorporateReqFeedback/GetToProfitCenter")]
//        public DataTable GetToProfitCenter()
//        {
//            DataTable ds = new DataTable();
//            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
//            ds = dc.GetToProfitCenter();
//            return ds;
//        }

//        [HttpPost()]
//        [Route("Corporate/CorporateReqFeedback/UploadFiles")]
//        public string UploadFiles()
//        {
//            string StrTempPath = "";
//            int iUploadedCnt = 0;
//            // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
//            // sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");
//            var request = System.Web.HttpContext.Current.Request;
//            // StrTempPath = "C:/TempERPFile/TempCRPTReq" + "/" + request["FrmEcode"].ToString();
//            StrTempPath = request["FileUploadTempPath"].ToString() + "/" + request["FrmEcode"].ToString();

//            if (Directory.Exists(StrTempPath))
//            {
//                Directory.GetAccessControl(StrTempPath);
//            }
//            else
//            {
//                Directory.CreateDirectory(StrTempPath);
//                Directory.GetAccessControl(StrTempPath);
//            }

//            if (request["FileUploadType"].ToString().Trim() == "Delete")
//            {
//                String filePath = StrTempPath + "/" + request["fileUpload"].ToString().Trim();
//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//             return " file Deleted";
//            }
//            else
//            //Save
//            { 
//            //if (Directory.Exists(StrTempPath))
//            //{
//            //    Directory.GetAccessControl(StrTempPath);

//            //    //foreach (string strFile in System.IO.Directory.GetFiles(StrTempPath, "*.*"))
//            //    //{
//            //    //    File.Delete(strFile);
//            //    //}
//            //    // Directory.Delete(StrTempPath);
//            //}
//            //else
//            //{
//            //    Directory.CreateDirectory(StrTempPath);
//            //    Directory.GetAccessControl(StrTempPath);
//            //}

//            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
//            var FrmEcode = request["FrmEcode"].ToString();
//            //  var fileUpload = request["fileUpload"].ToString();
//            // CHECK THE FILE COUNT.
//            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
//            {
//                System.Web.HttpPostedFile hpf = hfc[iCnt];
//                if (hpf.ContentLength > 0)
//                {
//                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (Delete)
//                        if (File.Exists(StrTempPath + "/" + Path.GetFileName(hpf.FileName)))
//                        {
//                            File.Delete(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
//                            // SAVE THE FILES IN THE FOLDER.
//                        }

//                        // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
//                        if (!File.Exists(StrTempPath + "/" + Path.GetFileName(hpf.FileName)))
//                        {
//                            // SAVE THE FILES IN THE FOLDER.
//                            // hpf.SaveAs(StrTempPath + "/" +  "1@@" + Path.GetFileName(hpf.FileName) );
//                            hpf.SaveAs(StrTempPath + "/" + Path.GetFileName(hpf.FileName));
//                            iUploadedCnt = iUploadedCnt + 1;
//                        }

//                }
//            }
//            // RETURN A MESSAGE.
//            if (iUploadedCnt > 0)
//            {
//                return iUploadedCnt + " Files Uploaded Successfully";
//            }
//            else
//            {
//                return "Upload Failed";
//            }
//            }
//        }

//        [HttpPost]
//        [Route("Corporate/CorporateReqFeedback/Submit")]
//        public string Submit([FromBody] CorporateReqFeedbackRequest CorporateReqFeedback)
//        {
//            string strMsg = "";
//            CorporateReqFeedbackCon dc = new CorporateReqFeedbackCon();
//            strMsg = dc.Submit(CorporateReqFeedback);
//            return strMsg;
//        }
//    }
//}


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
using System.Threading.Tasks;

//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;

namespace KalaERPApi.Controllers.Corporate.Trans
{

    // [EnableCors(origins: "http://www.kalapms.com:8282", headers: "*", methods: "*")]
    public class CorporateReqActionController : ApiController
    {

        [HttpGet]
        [Route("Corporate/CorporateReqAction/GetCPRTActionPending")]
        public DataTable GetCrptReqActionPending(string Type, string ReqCode, string P_PCCode, string P_EmpCode, string P_FromDt, string P_ToDt)
        {
            DataTable ds = new DataTable();
            CorporateReqActionCon dc = new CorporateReqActionCon();
            ds = dc.GetCPRTActionPending(Type, ReqCode, P_PCCode, P_EmpCode, P_FromDt, P_ToDt);
            return ds;
        }

        [HttpGet]
        [Route("Corporate/CorporateReqAction/GetCPRTActionView")]
        public DataTable GetCPRTActionView(string Type, string ReqCode, string UserId)
        {
            DataTable ds = new DataTable();
            CorporateReqActionCon dc = new CorporateReqActionCon();
            ds = dc.GetCPRTActionView(Type, ReqCode, UserId);
            return ds;
        }

        [Route("Corporate/CorporateReqAction/GetCorpReqActionPrvDtls")]
        public DataTable GetCorpReqActionPrvDtls(string Code)
        {
            DataTable ds = new DataTable();
            CorporateReqActionCon dc = new CorporateReqActionCon();
            ds = dc.GetCorpReqActionPrvDtls(Code);
            return ds;
        }

        [HttpGet]
        [Route("Corporate/CorporateReqAction/GetCPRTReqFile")]
        public DataTable GetCPRTReqFile(string Type, string ReqCode, string UserId)
        {
            DataTable ds = new DataTable();
            CorporateReqActionCon dc = new CorporateReqActionCon();
            ds = dc.GetCPRTReqFile(Type, ReqCode, UserId);
            return ds;
        }

        [HttpGet]
        [Route("Corporate/CorporateReqAction/GetCPRTActionFile")]
        public DataTable GetCPRTActionFile(string Type, string ReqCode, string UserId)
        {
            DataTable ds = new DataTable();
            CorporateReqActionCon dc = new CorporateReqActionCon();
            ds = dc.GetCPRTActionFile(Type, ReqCode, UserId);
            return ds;
        }


        [Route("Corporate/CorporateReqAction/GetCopReqProblemHead")]
        public DataTable GetCopReqProblemHead(string PCCode)
        {
            DataTable ds = new DataTable();
            CorporateReqActionCon dc = new CorporateReqActionCon();
            ds = dc.GetCopReqProblemHead(PCCode);
            return ds;
        }

        [HttpGet]
        [Route("Corporate/CorporateReqAction/GetCPRTActionAssignToEmp")]
        public DataTable GetCPRTActionAssignToEmp()
        {
            DataTable ds = new DataTable();
            CorporateReqActionCon dc = new CorporateReqActionCon();
            ds = dc.GetCPRTActionAssignToEmp();
            return ds;
        }


        [Route("Corporate/CorporateReqAction/GetExpEmpOrSupp")]
        public DataTable GetExpEmpOrSupp(string Type)
        {
            DataTable ds = new DataTable();
            CorporateReqActionCon dc = new CorporateReqActionCon();
            ds = dc.GetExpEmpOrSupp(Type);
            return ds;
        }


        [HttpGet]
        [Route("Corporate/CorporateReqAction/GetToProfitCenter")]
        public DataTable GetToProfitCenter()
        {
            DataTable ds = new DataTable();
            CorporateReqActionCon dc = new CorporateReqActionCon();
            ds = dc.GetToProfitCenter();
            return ds;
        }

        [HttpPost()]
        [Route("Corporate/CorporateReqAction/UploadFiles")]
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

        [HttpPost()]
        [Route("Corporate/CorporateReqAction/DownloadFile")]
        public async Task<FileStream> DownloadFile(string fileName)
        {
            MemoryStream ms = new MemoryStream();

            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            currentDirectory = currentDirectory + "\\src\\assets";
            var file = Path.Combine(Path.Combine(currentDirectory, "attachments"), fileName);


            file = "E:\\ERP\\2020\\05 May\\CRPTReq" + "\\" + fileName;

            using (var stream1 = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                // await stream;
            }

            file = "E:/ERP/2020/05 May/CRPTReq" + "/" + fileName;
            using (var stream2 = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                // await stream;
            }

            return new FileStream(file, FileMode.Open, FileAccess.Read);
        }
    

        [HttpGet]//http get as it return file 
        [Route("Corporate/CorporateReqAction/GetDownloadFile")]
        public HttpResponseMessage GetTestFile(string FilePath)
        {
            //below code locate physcial file on server 
           // var localFilePath = System.Web.HttpContext.Current.Server.MapPath("~/timetable.zip");
            //  localFilePath = "E:\\ERP\\2020\\05 May\\CRPTReq" + "\\" + fileName;
            var localFilePath = FilePath;
            HttpResponseMessage response = null;
            if (!File.Exists(localFilePath))
            {
                //if file not found than return response as resource not present 
                response = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                //if file present than read file 
                var fStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);
                //compose response and include file as content in it
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(fStream)
                };
                //set content header of reponse as file attached in reponse
                response.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(fStream.Name)
                };
                //set the content header content type as application/octet-stream as it returning file as reponse 
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            }
            return response;
        }
  
    /// public async System.Threading.Tasks.Task<FileStream> DownloadFile (string fileName)
    //public async Task<FileStream> DownloadFile(string fileName)
    //{
    //    var currentDirectory = System.IO.Directory.GetCurrentDirectory();
    //    currentDirectory = currentDirectory + "\\src\\assets";
    //    var file1 = Path.Combine(Path.Combine(currentDirectory, "attachments"), fileName);

    //   // String file = ("E:/ERP/2020/05 May/CRPTReq" + "/" + fileName);

    //    String file = ("E:\\ERP\\2020\\05 May\\CRPTReq" + "\\" + fileName);

    //    if (File.Exists(file))
    //    {
    //    }

    //    //  return await Task.FromResult(1);
    //    await Task.Delay(5000);
    //  //  return await Task.FromResult(new FileStream(file, FileMode.Open, FileAccess.Read));

    //   return new FileStream(file, FileMode.Open, FileAccess.Read);
    //}


    //[HttpGet]
    //[Route("Corporate/CorporateReqAction/DownloadFile")]
    // [HttpGet("DownloadFile/{fileName}")]
    //public async Task<IActionResult> DownloadFile1(string fileName)
    //{
    //    MemoryStream ms = new MemoryStream();
    //    if (CloudStorageAccount.TryParse(config.Value.StorageConnection, out CloudStorageAccount storageAccount))
    //    {
    //        CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
    //        CloudBlobContainer container = BlobClient.GetContainerReference(config.Value.Container);

    //        if (await container.ExistsAsync())
    //        {
    //            CloudBlob file = container.GetBlobReference(fileName);

    //            if (await file.ExistsAsync())
    //            {
    //                await file.DownloadToStreamAsync(ms);
    //                Stream blobStream = file.OpenReadAsync().Result;
    //                return File(blobStream, file.Properties.ContentType, file.Name);
    //            }
    //            else
    //            {
    //                return Content("File does not exist");
    //            }
    //        }
    //        else
    //        {
    //            return Content("Container does not exist");
    //        }
    //    }
    //    else
    //    {
    //        return Content("Error opening storage");
    //    }
    //}

    [HttpPost]
        [Route("Corporate/CorporateReqAction/Submit")]
        public string Submit([FromBody] CorporateReqActionRequest CorporateReqAction)
        {
            string strMsg = "";
            CorporateReqActionCon dc = new CorporateReqActionCon();
            strMsg = dc.Submit(CorporateReqAction);
            return strMsg;
        }

        [HttpGet]
        [Route("Corporate/CorporateReqAction/getEPSPendingAuthList")]
        public DataTable getEPSPendingAuthList(string EmpCode, string LoginType)
        {
            DataTable ds = new DataTable();
            CorporateReqActionCon dc = new CorporateReqActionCon();
            ds = dc.getEPSPendingAuthList(EmpCode, LoginType);
            return ds;
        }
    }
}
