using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Http;
using GRInreturnableRequest = KalaERPApi.Models.KalaSpan.GRInreturnableRequest;

namespace KalaERPApi.Service.KalaSpan.Trans
{
    public class GRInreturnableCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder();
        DataSet dsChkStage = new DataSet();
        CommonCon ComCon = new CommonCon();
        string strSql = "";
        SqlTransaction tran = null;
        SqlCommand cmd;

        public DataTable GetVehicleDtls(string VNo)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleGRInreturnableDetails", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@VehicleNo", SqlDbType.Char).Value = VNo;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public Boolean ChkStage(string StageNo, string GRICode, string VehNo)
        {
            Boolean strChkStage = false;
            CommonCon ComCon = new CommonCon();
            
           dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntSRNo From GateReceiptINReturnableAttachDetails where GRCode='" + GRICode + "' and SerialNo='" + VehNo + "'  and StageName='" + StageNo + "' ", "tbl_ChkStgDone");
            
            if (dsChkStage != null && dsChkStage.Tables["tbl_ChkStgDone"].Rows.Count == 1)
            {
                if (int.Parse(dsChkStage.Tables["tbl_ChkStgDone"].Rows[0]["CntSRNo"].ToString().Trim()) > 0)

                {
                    strChkStage = true;
                }
                else
                {
                    strChkStage = false;
                }
            }
            else
            {
                strChkStage = false;

            }
            return strChkStage;

        }
            
        /*
        public string Submit([FromBody] GRInreturnableRequest GRInreturnableReq)
        {
            string strkVA = "";
            strkVA = ComCon.getName("SELECT KVA FROM Part WHERE PartCode='" + GRInreturnableReq.VehPartCode.Trim() + "'", "tblPart", "KVA");

            if (ChkStage(GRInreturnableReq.StageNo, GRInreturnableReq.GRICode, GRInreturnableReq.VehicleNo) == true)
            {

                return "This Stage for Vehicle No " + GRInreturnableReq.VehicleNo.Trim() + " alredy Completed!";
            }

            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                SqlCommand cmd = new SqlCommand();
                // string cntStageDtsStatus = "0";
                string cntStageStatus = "0";
                int recCount1 = ComCon.CountChars(GRInreturnableReq.PhotoDtS, ",");
                string[] strPrcChkDts = Regex.Split(GRInreturnableReq.PhotoDtS, ",");
                int SrNo = 0;
                if (GRInreturnableReq.StageNo == "VehIn")//S1 Start
                {

                    sb.Remove(0, sb.Length);
                    sb.Append("Update GateReceiptINReturnableDetailssub set VehInStatus='D' where GRCode='" + GRInreturnableReq.GRICode.Trim() + "' and ");
                    sb.Append("Serialno='" + GRInreturnableReq.VehicleNo.Trim() + "' and  Partcode='" + GRInreturnableReq.VehPartCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #region 
                    for (int cSub1 = 0; cSub1 <= recCount1; cSub1++)
                    {
                        SrNo += 1;
                        string[] PrcChkDts = Regex.Split(strPrcChkDts[cSub1].ToString().Trim(), "-->");
                        string GetDGStartTime = "";

                        sb.Remove(0, sb.Length);
                        sb.Append("insert into GateReceiptINReturnableAttachDetails(GRCode,SysDt,PartCode,SerialNo,AttachmentType,SrNo,AttachmentPath,StageName)");
                        sb.Append("values('" + GRInreturnableReq.GRICode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                        sb.Append("'" + GRInreturnableReq.VehPartCode.Trim() + "','" + GRInreturnableReq.VehicleNo.Trim() + "','Photo',");
                        sb.Append("'1','" + PrcChkDts[0].Trim() + "',");
                        sb.Append("'" + GRInreturnableReq.StageNo.Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }
                   
                    #endregion
                }
                tran.Commit();
                // tran.Rollback();
                return GRInreturnableReq.GRICode.Trim();
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
        */
    }
}