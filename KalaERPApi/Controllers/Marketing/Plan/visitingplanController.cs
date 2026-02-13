using KalaERPApi.Models.Marketing.Plan;
using KalaERPApi.Models.Request.Marketing.Plan;
using KalaERPApi.Service;
using KalaERPApi.Service.Marketing.Plan;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace KalaERPApi.Controllers.Marketing.Plan
{
    // [EnableCors(origins: "http://www.kalapms.com:8282", headers: "*", methods: "*")]
    public class VisitingPlanController : ApiController
    {
        #region
        CommonCon cf = new CommonCon();
        VisitingPlanCon dc = new VisitingPlanCon();
        DataTable ds = new DataTable();
        StringBuilder sb = new StringBuilder();
        string strMsg = "";
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        SqlCommand cmd = null;
        #endregion

        [HttpGet]
        [Route("VisitingPlan/GetProfitcenter")]
        public DataTable GetVistingPlanEmpInfo(string Code)
        {
            ds = cf.GetEmpPCINFO("EmpPCINFO", Code);
            return ds;
        }

        [HttpGet]
        [Route("VisitingPlan/GetProfitcenterVP")]
        public DataTable GetVistingPlanEmpInfoVP(string Code)
        {
            if (Code.Substring(0, 3) == "VPM")
            {
                ds = cf.GetEmpPCINFO("EmpPCINFOVP", Code);
            }
            else { 
                ds = cf.GetEmpPCINFO("EmpPCINFO", Code); 
            }
            return ds;
        }

        [HttpGet]
        [Route("VisitingPlan/GetVisitPlanLocation")]
        public DataTable GetVisitPlanLocation(string Type, string VPPlanNo, string P_PCCode, string P_EmpCode, string P_FromDt, string P_ToDt)
        {
            ds = dc.GetVisitPlanLocation(Type, VPPlanNo, P_PCCode, P_EmpCode, P_FromDt, P_ToDt);
            return ds;
        }

        [HttpGet]
        [Route("VisitingPlan/GetAllVisitPlan")]
        public DataTable GetAllVisitPlanRpt(string Type, string VPPlanNo, string P_PCCode, string P_EmpCode, string P_FromDt, string P_ToDt)
        {
            ds = dc.GetAllVisitPlanRpt(Type, VPPlanNo, P_PCCode, P_EmpCode, P_FromDt, P_ToDt);
            return ds;
        }

        [HttpPost]
        [Route("VisitingPlan/Submit")]
        public string Submit([FromBody] VisitingPlanRequest VisitPlanreq)
        {
            strMsg = dc.Submit(VisitPlanreq);
            return strMsg;
        }

        [HttpPost]
        [Route("VisitingPlan/VPExpenseSubmit")]
        public string VPExpenseSubmit([FromBody] VisitingPlanExpRequest VisitPlanExpreq)
        {
            strMsg = dc.VPExpenseSubmit(VisitPlanExpreq);
            return strMsg;
        }

        [HttpPost]
        [Route("VisitingPlan/LocationSave")]
        public string SaveLocation([FromBody] ILocation location)
        {
            strMsg = dc.SaveLocation(location);
            return strMsg;
        }

        [HttpPost()]
        [Route("VisitingPlan/UploadFiles")]
        public string UploadFiles()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;                             
                HttpFileCollection hfc = HttpContext.Current.Request.Files;
                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    HttpPostedFile hpf = hfc[iCnt];
                    if (hpf.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 500; //Size = 100 MB  
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".wav", ".mp3", ".mp4", ".m4a", ".aac" };
                        var ext = hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        
                        if (hpf.ContentLength > MaxContentLength)
                        {
                            return "File Size is Max";
                        }
                        else
                        {
                            var filePath = cf.getMainFilePath("VisitPlanMktfile") + "\\" + hpf.FileName;
                            if (!File.Exists(filePath))
                            {
                                hpf.SaveAs(filePath);

                                if (con.State == ConnectionState.Open) { con.Close(); }
                                con.Open();

                                sb.Remove(0, sb.Length);
                                sb.Append("Update VisitPlanMKTDetails SET CurrentTime=getdate(),CurrentLatitude='" + httpRequest["latitude"].Trim() + "',CurrentLongitude='" + httpRequest["longitude"].Trim() + "',");
                                sb.Append("Feedback='" + httpRequest["feedback"].Trim() + "' WHERE VPCode ='" + httpRequest["vpcode"].Trim() + "' and ClientName='" + httpRequest["customer"].Trim() + "' and  SrNo=  '" + httpRequest["vpdsrno"].Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                cmd = new SqlCommand("InsertVisitPlanMKTFile", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@VPCode", httpRequest["vpcode"].Trim());
                                cmd.Parameters.AddWithValue("@SrNo", httpRequest["vpdsrno"].Trim() );                                
                                cmd.Parameters.AddWithValue("@FileName", hpf.FileName);
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                                con.Close();
                            }
                        }                      
                    }
                }
                return "File Not Found";
            }
            catch (Exception ex)
            {
                return "Upload Failed StackTrace " + ex.StackTrace + " Message " + ex.Message;
            }
            finally
            {
                con.Close();
            }
        }              

         [HttpGet]
        [Route("VisitingPlan/GetDownloadFile")]
        public HttpResponseMessage GetDownloadFile(string FilePath)
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

    }
}
