using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using KalaERPApi.Service.Maintenance.Trans;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using KalaERPApi.Service;
using System.Text;
using System.IO;

namespace KalaERPApi.Controllers.Maintenance
{
    public class MaintenanceController : ApiController
    {
        #region
        MaintenanceCon dc = new MaintenanceCon();
        SqlCommand cmd;
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        CommonCon cf = new CommonCon();
        StringBuilder sb = new StringBuilder();


        DataSet dsChkStage = new DataSet();
        CommonCon ComCon = new CommonCon();
        string strSql = "";
        SqlTransaction tran = null;

        #endregion

        [HttpGet]
        [Route("Maintenance/MaintenanceComplaint/getMachineScanDtls")]
        public DataTable GetMachineDtls(string MNo)
        {
            return dc.GetMachineDtls(MNo);
        }

        [HttpPost]
        [Route("Maintenance/MaintenanceComplaint/Submit")]
        public HttpResponseMessage Submit()
        {
            var message = "File Not Found";
            int SrNo = 0;
            string PrcNo = "";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;  
                HttpFileCollection hfc = HttpContext.Current.Request.Files;
              
                if (hfc.Count > 0)
                {
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    PrcNo = GetmaxPrc("MaintenanceComplaint", "MCCode", ComCon.yearEnd(con, tran), httpRequest["FromPCCode"].Trim().Trim().Substring(0, 2), con, tran);
                                        
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        HttpPostedFile hpf = hfc[iCnt];
                        if (hpf.ContentLength > 0)
                        {
                            SrNo += 1;
                            var ext = hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));
                            var extension = ext.ToLower();
                                                       
                            var filename = DateTime.Now.ToString("yyyyMMdd_hhmmssfff_") + SrNo + ext;
                            var filePath = cf.getMainFilePath("MNTReq") + "\\" + filename;
                            if (!File.Exists(filePath))
                            {
                                hpf.SaveAs(filePath);

                                sb.Remove(0, sb.Length);
                                sb.Append("INSERT INTO MaintenanceComplaintFiles");
                                sb.Append("(MCCode,SrNo,Attachfile,FileName)");
                                sb.Append(" VALUES('" + PrcNo.Trim() + "' ,'" + SrNo + "','" + filename + "','" + filename + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }                            
                        }
                    }
                    tran.Commit();
                } 
                message = dc.Submit(PrcNo.Trim(), httpRequest["PartCode"].Trim(), httpRequest["MachineSrNo"].Trim(), httpRequest["ForPCCode"].Trim(),
                httpRequest["FromPCCode"].Trim(), httpRequest["Reason"].Trim(), httpRequest["EmpCode"].Trim(), httpRequest["Remark"].Trim());

                dict.Add("Message", message);
                return Request.CreateResponse(HttpStatusCode.OK, dict);
            }
            catch (Exception ex)
            {                
                FileStream fs = new FileStream("C:/Error/MaintenanceComplaintLog.txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter m_streamWriter = new StreamWriter(fs);
                m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                m_streamWriter.WriteLine();
                m_streamWriter.WriteLine("***************** " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + "*****************");
                m_streamWriter.WriteLine("StackTrace: " + ex.StackTrace);
                m_streamWriter.WriteLine("Message: " + ex.Message);
                m_streamWriter.Flush();
                m_streamWriter.Close();

                tran.Rollback();
                message = string.Format("Error Occured ");
                dict.Add("Message", message + ex.Message);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            finally
            {
                con.Close();
            }
        }

        public string GetmaxPrc(string tablename, string fieldname, string Yr, string CompCode, SqlConnection con, SqlTransaction tran)
        {

            string PrcCode = "";
            string Max = "0";
            SqlCommand cmd = new SqlCommand("select max(substring(" + fieldname + ",13,7)) as MX from " + tablename.Trim() + " where yr='" + Yr.Trim() + "' and CompanyCode='" + CompCode.Trim() + "'", con, tran);
            cmd.CommandTimeout = 0;
            cmd.Transaction = tran;
            if ((cmd.ExecuteScalar().ToString() == System.DBNull.Value.ToString()))
            {
                Max = (CompCode + "000001");
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 9))
            {
                Max = (CompCode + ("00000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 99))
            {
                Max = (CompCode + ("0000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 999))
            {
                Max = (CompCode + ("000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 9999))
            {
                Max = (CompCode + ("00" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 99999))
            {
                Max = (CompCode + ("0" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else
            {
                Max = (CompCode + (Convert.ToInt32(cmd.ExecuteScalar()) + 1));
            }
            PrcCode = "MCF" + "/" + Yr + "/" + Max;

            cmd.Dispose();
            return PrcCode;
        }

    }
}