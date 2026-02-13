using System.Web.Http;
using System.Data;
using KalaERPApi.Service.Biotech.Trans;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.IO;
using KalaERPApi.Service;
using System;
using System.Net;
using System.Net.Http;

namespace KalaERPApi.Controllers.Biotech.Trans
{
    // [EnableCors(origins: "http://www.kalapms.com:8282", headers: "*", methods: "*")]
    public class PaymentRequisitionController : ApiController
    {
        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        PaymentRequisitionCon dc = new PaymentRequisitionCon();
        CommonCon cf = new CommonCon();
        SqlCommand cmd = null;
        SqlTransaction tran = null;
        #endregion

        [HttpGet]
        [Route("Biotech/KalaFresh/PayReqExistingSuppList")]
        public DataTable getPayReqExistingSuppList(string supp_name)
        {
            try
            {
                sb.Remove(0, sb.Length);
                sb.Append("select s.SName+'->'+ContactPerson+'->'+CPPhNo+'->'+WAdd+'->'+s.SCode+'->'+CT.CName+'(' +ST.SAliseName + ')->'+ cast(CT.CID as varchar) as SuppDtls from ");
                sb.Append("Supplier s inner join City ct on ct.CID = s.CityID inner join State ST on CT.StateID = ST.SID where s.Active = '1' and s.Auth = '1' and s.SName like '" + supp_name.Trim() + "%' order by s.SName");
                return cf.procDT(sb.ToString(), "tbl_ExistingSupplierList");
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        [Route("Biotech/KalaFresh/PayReqDataList")]
        public DataTable getPayReqDataList(string from_date, string to_date, string pc_code, string user_id, string type)
        {
            try
            {
                sb.Remove(0, sb.Length);
                sb.Append("exec getKFreshPayReqDetails '" + from_date.Trim() + "','" + to_date.Trim() + "','" + pc_code.Trim() + "','" + user_id.Trim() + "','" + type.Trim() + "'");
                return cf.procDT(sb.ToString(), "tbl_ExistingSupplierList");
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [Route("Biotech/KalaFresh/PayReqSubmit")]
        public HttpResponseMessage Submit()
        {
            var message = "File Not Found";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;

                //FileStream fs = new FileStream("C:/Error/ApiError.txt", FileMode.OpenOrCreate, FileAccess.Write);
                //StreamWriter m_streamWriter = new StreamWriter(fs);
                //m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                //m_streamWriter.WriteLine();
                //m_streamWriter.WriteLine("***************** " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + "*****************");
                //m_streamWriter.WriteLine("supplier_name " + httpRequest["supplier_name"].Trim());
                //m_streamWriter.WriteLine("supplier_code " + httpRequest["supplier_code"].Trim());
                //m_streamWriter.WriteLine("supplier_address " + httpRequest["supplier_address"].Trim());
                //m_streamWriter.WriteLine("supplier_city " + httpRequest["supplier_city"].Trim());
                //m_streamWriter.WriteLine("contact_person " + httpRequest["contact_person"].Trim());
                //m_streamWriter.WriteLine("contact_number " + httpRequest["contact_number"].Trim());
                //m_streamWriter.WriteLine("bill_no " + httpRequest["bill_no"].Trim());
                //m_streamWriter.WriteLine("bill_date " + httpRequest["bill_date"].Trim());
                //m_streamWriter.WriteLine("bill_amt " + httpRequest["bill_amt"].Trim());
                //m_streamWriter.WriteLine("trans_type " + httpRequest["trans_type"].Trim());
                //m_streamWriter.WriteLine("trans_amt " + httpRequest["trans_amt"].Trim());
                //m_streamWriter.WriteLine("user_id " + httpRequest["user_id"].Trim());
                //m_streamWriter.WriteLine("pc_code " + httpRequest["pc_code"].Trim());
                //m_streamWriter.Flush();
                //m_streamWriter.Close();

                message = dc.Submit(httpRequest["supplier_name"].Trim(), httpRequest["supplier_code"].Trim(),
                     httpRequest["supplier_address"].Trim(), httpRequest["supplier_city"].Trim(),
                     httpRequest["contact_person"].Trim(), httpRequest["contact_number"].Trim(),
                     httpRequest["bill_no"].Trim(), httpRequest["bill_date"].Trim(),
                     httpRequest["bill_amt"].Trim(), httpRequest["trans_type"].Trim(),
                     httpRequest["trans_amt"].Trim(), httpRequest["remark"].Trim(),
                     httpRequest["beneficiary_name"].Trim(), httpRequest["bank_name"].Trim(),
                     httpRequest["bank_address"].Trim(), httpRequest["bank_account_no"].Trim(),
                     httpRequest["ifsc_no"].Trim(), httpRequest["user_id"].Trim(), httpRequest["pc_code"].Trim());

                HttpFileCollection hfc = HttpContext.Current.Request.Files;
                if (hfc.Count > 0)
                {
                    if (con.State == ConnectionState.Open) { con.Close(); }
                    con.Open();

                    int SrNo = 0;
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        HttpPostedFile hpf = hfc[iCnt];
                        if (hpf.ContentLength > 0)
                        {
                            SrNo += 1;
                            var ext = hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));
                            var extension = ext.ToLower();

                            //var filename = DateTime.Now.ToString("yyyyMMdd_hhmmssfff_") + SrNo + ext;
                            var filename = message.Substring(4, 5).Trim() + message.Substring(10, 8).Trim() + SrNo + ext;
                            var destPath = cf.getMainFilePath("ExpRequisitionfile") + "\\" + filename;
                            if (!File.Exists(destPath))
                            {
                                hpf.SaveAs(destPath);

                                sb.Remove(0, sb.Length);
                                sb.Append("insert into ExpenseRequisitionFileDetails(REQCode,SrNo,FileType,FileName) ");
                                sb.Append("Values('" + message.Trim() + "','" + SrNo + "',");
                                if (extension.Trim() == ".jpg" || extension.Trim() == ".jpeg")
                                {
                                    sb.Append("'Image',");
                                }
                                else if (extension.Trim() == ".mp3" || hpf.FileName.Trim().Substring(0, 5) == "AUDIO")
                                {
                                    sb.Append("'Audio',");
                                }
                                else if (extension.Trim() == ".mp4" || hpf.FileName.Trim().Substring(0, 5) == "VIDEO")
                                {
                                    sb.Append("'Video',");
                                }
                                sb.Append("'" + filename + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }
                    }
                }

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
        [Route("Biotech/KalaFresh/PayReqAuth")]
        public string Auth(string req_no, string supp_code, string bill_amt, string auth_remark, string user_id, string pc_code)
        {
            string message = "Failed";
            try
            {
                //FileStream fs = new FileStream("C:/Error/ApiError.txt", FileMode.OpenOrCreate, FileAccess.Write);
                //StreamWriter m_streamWriter = new StreamWriter(fs);
                //m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                //m_streamWriter.WriteLine();
                //m_streamWriter.WriteLine("***************** " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + "*****************");
                //m_streamWriter.WriteLine("req_no " + req_no.Trim());
                //m_streamWriter.WriteLine("supp_code " + supp_code.Trim());
                //m_streamWriter.WriteLine("bill_amt " + bill_amt.Trim());
                //m_streamWriter.WriteLine("auth_remark " + auth_remark.Trim());
                //m_streamWriter.WriteLine("user_id " + user_id.Trim());
                //m_streamWriter.WriteLine("pc_code " + pc_code.Trim());
                //m_streamWriter.Flush();
                //m_streamWriter.Close();

                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                sb.Remove(0, sb.Length);
                sb.Append("Update ExpenceRequisitionWithPlan set Auth='1',AuthRemark='" + auth_remark.Trim() + "',");
                sb.Append("AuthBy='" + user_id.Trim() + "',AuthDtTime='" + DateTime.Now + "' where ");
                sb.Append("REQCode ='" + req_no.Trim() + "' and Auth='0'");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                sb.Remove(0, sb.Length);
                sb.Append("insert into EmpFundTransaction(EmpCode,DocType,TransCode,IssueCode,IssueDt,IssAmount,ReceivedCode,ReceivedDt,RecAmount) ");
                sb.Append("Values('" + user_id.Trim() + "','ERW','" + req_no.Trim() + "','0',NULL,'0.00',");
                sb.Append("'" + req_no.Trim() + "','" + DateTime.Now + "','" + bill_amt.Trim() + "')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                //****************User Acivity**************** 
                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO AuthorizationDetails");
                sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                sb.Append(" VALUES('" + DateTime.Now + "' ,'" + user_id.Trim() + "','ExpenseRequisition Authorize',");
                sb.Append("'" + req_no.Trim() + "','" + pc_code.Trim().Substring(0, 2) + "')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                tran.Commit();
                message = req_no.Trim();
                return message;
            }
            catch (Exception ex)
            {
                message = "Error Occured ";
                tran.Rollback();
                return message + " StackTrace " + ex.StackTrace + " Message " + ex.Message;
            }
            finally
            {
                con.Close();
            }
        }

    }
}
