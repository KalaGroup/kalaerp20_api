using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using KalaERPApi.Service.KalaSpan.Trans;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using KalaERPApi.Service;
using System.Text;

namespace KalaERPApi.Controllers.KalaSpan.Trans
{
    public class GRInreturnableController : ApiController
    {
        #region
        GRInreturnableCon dc = new GRInreturnableCon();
        SqlCommand cmd;
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        CommonCon cf = new CommonCon();
        StringBuilder sb = new StringBuilder();
        #endregion

        [HttpGet]
        [Route("KalaSpan/GRInreturnable/GetVehicleDtls")]
        public DataTable GetVehicleDtls(string VNo)
        {
            return dc.GetVehicleDtls(VNo);
        }

        [HttpPost]
        [Route("KalaSpan/GRInreturnable/Submit")]
        public HttpResponseMessage PostFiles()
        {
            var message = "File Not Found";
            int SrNo = 0;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;

                //FileStream fs = new FileStream("C:/Error/ApiError_GRInreturnableSubmit.txt", FileMode.OpenOrCreate, FileAccess.Write);
                //StreamWriter m_streamWriter = new StreamWriter(fs);
                //m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                //m_streamWriter.WriteLine();
                //m_streamWriter.WriteLine("***************** " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + "*****************");
                //m_streamWriter.WriteLine("GrCode: " + httpRequest["GrCode"].Trim());
                //m_streamWriter.WriteLine("SerialNo: " + httpRequest["SerialNo"].Trim());
                //m_streamWriter.WriteLine("PartCode: " + httpRequest["PartCode"].Trim());
                //m_streamWriter.WriteLine("AttachmentType: " + httpRequest["AttachmentType"].Trim());
                //m_streamWriter.WriteLine("StageName: " + httpRequest["StageName"].Trim());
                //m_streamWriter.Flush();
                //m_streamWriter.Close();

                if (con.State == ConnectionState.Open) { con.Close(); }
                con.Open();

                //update status
                sb.Remove(0, sb.Length);
                sb.Append("Update GateReceiptINReturnable set VehInStatus='D' where GRCode='" + httpRequest["GrCode"].Trim() + "' and ");
                sb.Append("VehicleNo='" + httpRequest["SerialNo"].Trim() + "'");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                sb.Append(" values('20.005','" + httpRequest["PartCode"].Trim() + "','" + httpRequest["GrCode"].Trim() + "',GetDate(),'1','20.005','0','0','StageI')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                #region
                /*
                HttpFileCollection hfc = HttpContext.Current.Request.Files;               
                if (hfc.Count > 0)
                {  
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        HttpPostedFile hpf = hfc[iCnt];
                        if (hpf.ContentLength > 0)
                        {
                            SrNo +=1;
                            var ext = hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));
                            var extension = ext.ToLower();

                            var filename = DateTime.Now.ToString("yyyyMMdd_hhmmssfff_") +SrNo + ext;
                            var filePath = cf.getMainFilePath(httpRequest["StageName"].Trim()) + "\\" + filename;
                            if (!File.Exists(filePath))
                            {                               
                                hpf.SaveAs(filePath);

                                sb.Remove(0, sb.Length);
                                sb.Append("insert into GateReceiptINReturnableAttachDetails(GRCode,PartCode,SerialNo,AttachmentType,SrNo,AttachmentPath,StageName) ");
                                sb.Append("Values('" + httpRequest["GrCode"].Trim() + "','" + httpRequest["PartCode"].Trim() + "','" + httpRequest["SerialNo"].Trim() + "',");
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
                                sb.Append("'" + SrNo + "','" + filename + "','" + httpRequest["StageName"].Trim() + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }
                    } 
                }*/
                #endregion

                message = string.Format("Vehicle Scanned Successfully.");
                dict.Add("Message", message);
                return Request.CreateResponse(HttpStatusCode.Created, dict);
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

    }
}