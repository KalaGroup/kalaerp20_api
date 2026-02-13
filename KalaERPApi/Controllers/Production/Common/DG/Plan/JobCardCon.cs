using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using Swashbuckle.Swagger;
using System.Text.RegularExpressions;
using KalaERPApi.Models.Request;

namespace KalaERPApi.Service
{
    public class JobCardCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        public SqlTransaction tran = null;
        DataSet dsDetailsSub = new DataSet();

        //SqlCommand cmd = null;

        public object SqlDbTypeChar { get; private set; }

        public DataTable GetDG()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetJobCardDGDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public string Submit(JobCardRequest jobreq)
        {
            string strmax = "";
            string JobCardNo = "";
            CommonCon ComCon = new CommonCon();
            // string yearEnd = clsCommonFunctions.yearEnd();
            //int intmax = 0;

            try
            {
              if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                JobCardNo = ComCon.GetMaxNo("JobCard", "JCD", jobreq.PCCode.Trim().Substring(0, 2), con, tran);

                SqlCommand cmd = new SqlCommand();
                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO JobCard(JobCode,Dt,Yr,MaxSrNo,PCCode,Remark,CompanyCode,Active,Auth)");
                sb.Append(" VALUES('" + JobCardNo.Trim() + "','" + DateTime.Now + "','" + ComCon.yearEnd(con,tran) + "','" + (JobCardNo.Substring(10, 8)) + "',");
                sb.Append("'03.051','" + jobreq.Remark.Trim() + "','" + jobreq.PCCode.Trim().Substring(0, 2) + "','1','1')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                int recCount =ComCon.CountChars(jobreq.JobCardDts, ",");
                string[] strJCDDts = Regex.Split(jobreq.JobCardDts, ",");
                int SrNo = 0;
                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo += 1;
                    string[] Dts = Regex.Split(strJCDDts[cSub].ToString().Trim(), "-->");
                    
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO JobCardDetails(JobCode,SrNo,BOMCode,PartCode,Qty,Stage1Status,Stage2Status,Stage3Status)");
                    sb.Append(" VALUES('" + JobCardNo.Trim() + "','" + SrNo + "','" + Dts[0].Trim() + "','" + Dts[1].Trim() + "'," + Dts[2].Trim() + ",");
                    sb.Append("'P','P','P')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    dsDetailsSub = ComCon.procTranDS("exec GetJobCardSrNo '" + Dts[1] + "'," + Dts[2].Trim() + "", "tbl_JoBCardSerialNo", con, tran);
                    if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows.Count > 0)
                    {

                        int SrNok = 0;
                        for (int k = 0; k < dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows.Count; k++)
                        {
                            SrNok += 1;
                            sb.Remove(0, sb.Length);
                            sb.Append("insert into JobCardDetailsSub(JobCode,SrNo,PartCode,SrNoPartCode,SerialNo) ");
                            sb.Append("values('" + JobCardNo.Trim() + "','" + SrNok + "','" + Dts[1].Trim() + "',");
                            sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim() + "',");
                            sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                       
                }

                    tran.Commit();
                return JobCardNo;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return null;
               
            }
            finally
            {
                con.Close();
            }
        }

      
        
    }
}