using System.Data;
using System.Web.Http;
using KalaERPApi.Service.KalaSpan.Trans;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using KalaERPApi.Service;
using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.IO;

namespace KalaERPApi.Controllers.KalaSpan.Trans
{
    public class KalaSpanStageController : ApiController
    {
        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        CommonCon cf = new CommonCon();
        KalaSpanStageCon dc = new KalaSpanStageCon();
        CommonCon dc1 = new CommonCon();
        SqlCommand cmd;
        #endregion

        [HttpGet]
        [Route("KalaSpan/Stage/GetScanDts")]
        public DataTable GetScanDts1(string strSrNo, string strPartCode, string strCat, string strStage)
        {
            return dc.GetScanDts(strSrNo, strPartCode, strCat, strStage);
        }

        [HttpGet]
        [Route("KalaSpan/Stage/GetTRProdDts")]
        public DataTable GetProdDts(string strPartCode, string StrDGSrNo, string strPfbCode)
        {
            return dc.GetProdDts(strPartCode, StrDGSrNo, strPfbCode);
        }

        [HttpGet]
        [Route("KalaSpan/GetPrcChkDts")]
        public DataTable GetPrcChkDts(string strStageNo, string PrcStatusName)
        {
            return dc.GetPrcChkDts(strStageNo, PrcStatusName);
        }

        [HttpGet]
        [Route("KalaSpan/Get6M")]
        public DataTable Get6M()
        {
            return dc.Get6M();
        }

        [HttpGet]
        [Route("KalaSpan/Stage/GetProdDts")]
        public DataTable GetPodprcDts(string strPartCode, string strPCCode)
        {
            return dc.GetPodprcDts(strPartCode, strPCCode);
        }

        [HttpGet]
        [Route("KalaSpan/GetPrcStatus")]
        public DataTable GetPrcStatus()
        {
            return dc.GetPrcStatus();
        }

        [HttpGet]
        [Route("KalaSpan/GetAccChkDts")]
        public DataTable GetAccChkDts()
        {
            return dc.GetAccChkDts();
        }

        [HttpGet]
        [Route("KalaSpan/GetAccStageVChkDts")]
        public DataTable GetAccStageVChkDts(string SerialNo, string Type)
        {
            return dc.GetAccStageVChkDts(SerialNo, Type);
        }
              
        [HttpPost]
        [Route("KalaSpan/KalaSpanStage/Submit")]
        public HttpResponseMessage Submit()
        {
            var message = "File Not Found";
            int SrNo = 0;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;

                //FileStream fs = new FileStream("C:/Error/ApiError_KalaSpanStageSubmit.txt", FileMode.OpenOrCreate, FileAccess.Write);
                //StreamWriter m_streamWriter = new StreamWriter(fs);
                //m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                //m_streamWriter.WriteLine();
                //m_streamWriter.WriteLine("***************** " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + "*****************");
                //m_streamWriter.WriteLine("StackTrace " + httpRequest["JBCode"].Trim());
                //m_streamWriter.WriteLine("StageNo " + Convert.ToInt32(httpRequest["StageNo"].Trim()));
                //m_streamWriter.WriteLine("ProductCode " + httpRequest["ProductCode"].Trim());
                //m_streamWriter.WriteLine("VehPartCode " + httpRequest["VehPartCode"].Trim());
                //m_streamWriter.WriteLine("VehSrNo " + httpRequest["VehSrNo"].Trim());
                //m_streamWriter.WriteLine("AltPartcode " + httpRequest["AltPartcode"].Trim());
                //m_streamWriter.WriteLine("AltSrno " + httpRequest["AltSrno"].Trim());
                //m_streamWriter.WriteLine("CpyPartcode " + httpRequest["CpyPartcode"].Trim());
                //m_streamWriter.WriteLine("CpySrno " + httpRequest["CpySrno"].Trim());
                //m_streamWriter.WriteLine("BatPartcode " + httpRequest["BatPartcode"].Trim());
                //m_streamWriter.WriteLine("BatSrno " + httpRequest["BatSrno"].Trim());
                //m_streamWriter.WriteLine("Bat2Partcode " + httpRequest["Bat2Partcode"].Trim());
                //m_streamWriter.WriteLine("Bat2Srno " + httpRequest["Bat2Srno"].Trim());
                //m_streamWriter.WriteLine("QA6M " + httpRequest["QA6M"].Trim());
                //m_streamWriter.WriteLine("PrcStatus " + httpRequest["PrcStatus"].Trim());
                //m_streamWriter.WriteLine("PrcChkDts " + httpRequest["PrcChkDts"].Trim());
                //m_streamWriter.WriteLine("AttachmentType " + httpRequest["AttachmentType"].Trim());
                //m_streamWriter.WriteLine("TeamName " + httpRequest["TeamName"].Trim());
                //m_streamWriter.Flush();
                //m_streamWriter.Close();

                if (Convert.ToInt32(httpRequest["StageNo"].Trim()) != 0)
                {
                    HttpFileCollection hfc = HttpContext.Current.Request.Files;
                    if (hfc.Count > 0)
                    {
                        if (con.State == ConnectionState.Open) { con.Close(); }
                        con.Open();

                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            HttpPostedFile hpf = hfc[iCnt];
                            if (hpf.ContentLength > 0)
                            {
                                SrNo += 1;
                                var ext = hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));
                                var extension = ext.ToLower();

                                var filename = DateTime.Now.ToString("yyyyMMdd_hhmmssfff_") + SrNo + ext;
                                var filePath = cf.getMainFilePath("StageNo") + "\\" + filename;
                                if (!File.Exists(filePath))
                                {
                                    hpf.SaveAs(filePath);

                                    sb.Remove(0, sb.Length);
                                    sb.Append("insert into GateReceiptINReturnableAttachDetails(GRCode,PartCode,SerialNo,AttachmentType,SrNo,AttachmentPath,StageName) ");
                                    sb.Append("Values('" + httpRequest["JBCode"].Trim() + "','" + httpRequest["VehPartCode"].Trim() + "','" + httpRequest["VehSrNo"].Trim() + "',");
                                    if (extension.Trim() == ".jpg" || extension.Trim() == ".jpeg")
                                    {
                                        sb.Append("'Photo',");
                                    }
                                    else if (extension.Trim() == ".mp3")
                                    {
                                        sb.Append("'Audio',");
                                    }
                                    else if (extension.Trim() == ".mp4")
                                    {
                                        sb.Append("'Video',");
                                    }
                                    sb.Append("'" + SrNo + "','" + filename + "','" + httpRequest["StageNo"].Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }
                        }
                    }
                }
               
                message = dc.Submit(httpRequest["JBCode"].Trim(), Convert.ToInt32(httpRequest["StageNo"].Trim()), httpRequest["ProductCode"].Trim(), httpRequest["VehPartCode"].Trim(),
                httpRequest["VehSrNo"].Trim(), httpRequest["AltPartcode"].Trim(), httpRequest["AltSrno"].Trim(), httpRequest["CpyPartcode"].Trim(),
                httpRequest["CpySrno"].Trim(), httpRequest["BatPartcode"].Trim(), httpRequest["BatSrno"].Trim(), httpRequest["Bat2Partcode"].Trim(),
                httpRequest["Bat2Srno"].Trim(), httpRequest["QA6M"].Trim(), httpRequest["PrcStatus"].Trim(), httpRequest["PrcChkDts"].Trim(),
                httpRequest["AttachmentType"].Trim(), httpRequest["TeamName"].Trim());

                dict.Add("Message", message);
                return Request.CreateResponse(HttpStatusCode.OK, dict);                               
            }
            catch (Exception ex)
            {
                message = string.Format("Error Occured ");
                dict.Add("Message", message + ex.Message);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            finally
            {
                con.Close();
            }
        }

        [HttpPost]
        [Route("KalaSpan/JointInspectionReport/JIRSubmit")]
        public HttpResponseMessage JIRSubmit()
        {
            var JIRCode = "File Not Found";
            int SrNo = 0;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;

                //FileStream fs = new FileStream("C:/Error/ApiError_JIRSubmit.txt", FileMode.OpenOrCreate, FileAccess.Write);
                //StreamWriter m_streamWriter = new StreamWriter(fs);
                //m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                //m_streamWriter.WriteLine();
                //m_streamWriter.WriteLine("***************** " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + "*****************");
                //m_streamWriter.WriteLine("TRCode: " + httpRequest["TRCode"].Trim());
                //m_streamWriter.WriteLine("SerialNo: " + httpRequest["SerialNo"].Trim());
                //m_streamWriter.WriteLine("UserID: " + httpRequest["UserID"].Trim());
                //m_streamWriter.WriteLine("PCCode: " + httpRequest["PCCode"].Trim());               
                //m_streamWriter.Flush();
                //m_streamWriter.Close();

                JIRCode = dc.JIRSubmit(httpRequest["TRCode"].Trim(), httpRequest["SerialNo"].Trim(), 
                    httpRequest["UserID"].Trim(), httpRequest["PCCode"].Trim());

                #region
                /*
                HttpFileCollection hfc = HttpContext.Current.Request.Files;
                if (hfc.Count > 0)
                {
                    if (con.State == ConnectionState.Open) { con.Close(); }
                    con.Open();

                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        HttpPostedFile hpf = hfc[iCnt];
                        if (hpf.ContentLength > 0)
                        {
                            SrNo += 1;
                            var ext = hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));
                            var extension = ext.ToLower();

                            var filename = DateTime.Now.ToString("yyyyMMdd_hhmmssfff_") + SrNo + ext;
                            var filePath = cf.getMainFilePath(httpRequest["StageNo"].Trim()) + "\\" + filename;
                            if (!File.Exists(filePath))
                            {
                                hpf.SaveAs(filePath);

                                sb.Remove(0, sb.Length);
                                sb.Append("insert into GateReceiptINReturnableAttachDetails(GRCode,PartCode,SerialNo,AttachmentType,SrNo,AttachmentPath,StageName) ");
                                sb.Append("Values('" + JIRCode.Trim() + "','1032185000000000000','ALL',");
                                if (extension.Trim() == ".jpg" || extension.Trim() == ".jpeg")
                                {
                                    sb.Append("'Photo',");
                                }
                                else if (extension.Trim() == ".mp3" || hpf.FileName.Trim().Substring(0, 5) == "AUDIO")
                                {
                                    sb.Append("'Audio',");
                                }
                                else if (extension.Trim() == ".mp4")
                                {
                                    sb.Append("'Video',");
                                }
                                sb.Append("'" + SrNo + "','" + filename + "','" + httpRequest["StageNo"].Trim() + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }
                    }
                }*/
                #endregion

                dict.Add("Message", JIRCode);
                return Request.CreateResponse(HttpStatusCode.OK, dict);
            }
            catch (Exception ex)
            {
                JIRCode = string.Format("Error Occured, ");
                dict.Add("Message", JIRCode + ex.Message);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            finally
            {
                con.Close();
            }
        }

        [HttpGet]
        [Route("KalaSpan/GetKSpanMTTR")]
        public DataTable getKSpanMTTRReport(string FromDt, string ToDt, string VehicleNo, string StageName, string Type)
        {
            return dc.GetKSpanMTTRReport(DateTime.Now.AddYears(-2).ToString("yyyy-MM-dd 00:00:00"), DateTime.Now.ToString("yyyy-MM-dd 23:59:59"), VehicleNo, StageName, Type);
        }

        [HttpGet]
        [Route("KalaSpan/GetKSpanMTTRAttachments")]
        public DataTable getKSpanMTTRAttachments(string VehicleNo, string StageName, string Type)
        {
            return dc.GetKSpanMTTRAttachments(VehicleNo, StageName, Type);
        }

    }
}
