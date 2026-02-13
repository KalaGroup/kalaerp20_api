using System.Data;
using System.Web.Http;
using KalaERPApi.Models.Request.Production.DG.Trans;
using KalaERPApi.Service.Production.DG.Trans;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using KalaERPApi.Service;
using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

namespace KalaERPApi.Controllers.Production.DG.Trans
{
    public class DGStageController : ApiController
    {
        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        CommonCon cf = new CommonCon();
        DGStageCon dc = new DGStageCon();
        CommonCon dc1 = new CommonCon();
        SqlCommand cmd;
        #endregion

        [HttpGet]
        [Route("DGStage/Stage/GetScanDts")]
        public DataTable GetScanDts1(string strSrNo, string strPartCode, string strCat, string strStage,string StrPCCode)
        {
            return dc.GetScanDts(strSrNo, strPartCode, strCat, strStage, StrPCCode);
        }

        [HttpPost]
        [Route("DGStage/Stage/Submit")]
        public string Submit1(DGStageRequest DGStageReq)
        {
            return dc.Submit(DGStageReq);
        }

        [HttpGet]
        [Route("DGStage/GetPrcChkDts")]
        public DataTable GetPrcChkDts(string strStageNo, string PrcStatusName)
        {
            return dc1.GetPrcChkDts(strStageNo, PrcStatusName);
        }

        [HttpPost]
        [Route("DGStage/Stage/Upload")]
        public HttpResponseMessage Post()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var message = "";
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 50; //Size = 50 MB  
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".wav", ".mp3", ".mp4", ".aac" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            message = string.Format("Invalid File Format");
                            dict.Add("Message", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {
                            message = string.Format("File Size is Max");
                            dict.Add("Message", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            var filePath = cf.getMainFilePath(httpRequest["StageNo"].Trim()) + "\\" + postedFile.FileName;
                            postedFile.SaveAs(filePath);

                            if (con.State == ConnectionState.Open) { con.Close(); }                           
                            con.Open();    
                            sb.Remove(0, sb.Length);
                            sb.Append("Insert Into DGStageWiseFiles(TransCode,TransType,TransPartCode,TransSerialNo,TransFileName) ");
                            sb.Append("Values('" + httpRequest["JobCode"].Trim() + "','" + httpRequest["StageNo"].Trim() + "',");
                            sb.Append("'" + httpRequest["SrNoPartcode"].Trim() + "','" + httpRequest["Serialno"].Trim() + "','" + postedFile.FileName + "')");
                            cmd = new SqlCommand(sb.ToString(), con);                            
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            con.Close();
                                                     
                            message = string.Format("File Updated Successfully.");
                            dict.Add("Message", message);
                            return Request.CreateResponse(HttpStatusCode.Created, dict);
                        }
                    }
                }
                var res = string.Format("File Not Found");
                dict.Add("Message", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format("Error Occured ");
                dict.Add("Message", res + ex.Message);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            finally
            {
                con.Close();
            }
        }
                              
    }
}
