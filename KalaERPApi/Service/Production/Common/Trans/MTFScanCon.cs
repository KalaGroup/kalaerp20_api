using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using MTFScanRequest = KalaERPApi.Models.Request.Production.Common.Trans.MTFScanRequest;

namespace KalaERPApi.Service.Production.Common.Trans
{
    public class MTFScanCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        public SqlTransaction tran = null;
        DataSet dsDetailsSub = new DataSet();
    
        public DataTable GetMTFCode(string FPCCode,string TPCCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetMTFCode", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@FPCCode", SqlDbType.Char).Value = FPCCode;
            dAd.SelectCommand.Parameters.Add("@TPCCode", SqlDbType.Char).Value = TPCCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
    
        public DataTable GetMTFDts(string MTFCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetMtfSrNoDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@MTFCode", SqlDbType.Char).Value = MTFCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit([FromBody] MTFScanRequest MTFscanReq)
        {            
            CommonCon ComCon = new CommonCon();
            try
            {               
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                int recCount = ComCon.CountChars(MTFscanReq.SerialNoDts, ",");
                string[] strMTFDts = Regex.Split(MTFscanReq.SerialNoDts, ",");

                for (int cSub = 0; cSub <= recCount; cSub++)                
                {
                    string[] Dts = Regex.Split(strMTFDts[cSub].ToString().Trim(), "-->");
                                    
                    sb.Remove(0, sb.Length);
                    sb.Append("Update MTFDEtailsSub set TRFStatus='M' where  MTFCode='" + MTFscanReq.MtfCode.Trim() + "' and Partcode='" + Dts[0].Trim() + "' and SerialNo='" + Dts[1].Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                                       
                    sb.Remove(0, sb.Length);
                    sb.Append("Update JobCardDetailsSub set TransferStatus='D',TransferCode='" + MTFscanReq.MtfCode.Trim() + "' where SerialNo='" + Dts[1].Trim() + "' and SrNoPartCode='" + Dts[0].Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();                  

                }

                //Added by FS for DG MTF Scan
                sb.Remove(0, sb.Length);
                sb.Append("Update MemoExciseMfg set MTFScanStatus='D' where MMTFCode='" + MTFscanReq.MtfCode.Trim() + "' ");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();

                dsDetailsSub = ComCon.procTranDS("exec GetMTFDts '" + MTFscanReq.MtfCode.Trim() + "'", "tbl_MTFSrNo", con, tran);
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_MTFSrNo"].Rows.Count > 0)
                {
                    for (int k = 0; k < dsDetailsSub.Tables["tbl_MTFSrNo"].Rows.Count; k++)
                    {
                        string alreadySave = ComCon.getTranName("select ReceivedCode from StockWIP where FromProfitCenterCode='" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["FPCCode"].ToString().Trim() + "' and PartCode='" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["Partcode"].ToString().Trim() + "' and ReceivedCode='"+ MTFscanReq.MtfCode.Trim() + "' and ReceivedQty>0 and ToProfitCenterCode='"+ dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["TPCCode"].ToString().Trim() + "'", "tbl_WIPStock", "ReceivedCode", con,tran);
                        if (alreadySave.Trim() == "0")
                        {
                            if (dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["Partcode"].ToString().Trim().Substring(0, 3) == "001" || dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["Partcode"].ToString().Trim().Substring(0, 3) == "002")
                            {
                                //sb.Remove(0, sb.Length);
                                //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,StageName)");
                                //sb.Append(" values('" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["FPCCode"].ToString().Trim() + "','" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["Partcode"].ToString().Trim() + "' ,");
                                //sb.Append("'" + MTFscanReq.MtfCode.Trim() + "','" + DateTime.Now + "','" + double.Parse(dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["IssueQty"].ToString().Trim()) + "','" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["TPCCode"].ToString().Trim() + "','0','StageI')");
                                //cmd = new SqlCommand(sb.ToString(), con);
                                //cmd.Transaction = tran;
                                //cmd.ExecuteNonQuery();
                                //cmd.Dispose();
                            }
                            else if (dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["Partcode"].ToString().Trim().Substring(0, 3) == "010")
                            {
                                //sb.Remove(0, sb.Length);
                                //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,StageName)");
                                //sb.Append(" values('" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["FPCCode"].ToString().Trim() + "','" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["Partcode"].ToString().Trim() + "' ,");
                                //sb.Append("'" + MTFscanReq.MtfCode.Trim() + "','" + DateTime.Now + "','" + double.Parse(dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["IssueQty"].ToString().Trim()) + "','" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["TPCCode"].ToString().Trim() + "','0','StageIII')");
                                //cmd = new SqlCommand(sb.ToString(), con);
                                //cmd.Transaction = tran;
                                //cmd.ExecuteNonQuery();
                                //cmd.Dispose();
                            }
                            else
                            {
                                //sb.Remove(0, sb.Length);
                                //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                                //sb.Append(" values('" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["FPCCode"].ToString().Trim() + "','" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["Partcode"].ToString().Trim() + "' ,");
                                //sb.Append("'" + MTFscanReq.MtfCode.Trim() + "','" + DateTime.Now + "','" + double.Parse(dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["IssueQty"].ToString().Trim()) + "','" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["TPCCode"].ToString().Trim() + "','0')");
                                //cmd = new SqlCommand(sb.ToString(), con);
                                //cmd.Transaction = tran;
                                //cmd.ExecuteNonQuery();
                                //cmd.Dispose();
                            }
                        }                                               
                    }
                }                               
                tran.Commit();
                return MTFscanReq.MtfCode.Trim();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());
            }

            finally
            {
                con.Close();
            }
        }

    }
}