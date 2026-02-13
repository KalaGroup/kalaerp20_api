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
using KalaERPApi.Models.Request.Biotech.Trans;
using System.IO;

namespace KalaERPApi.Service.Biotech.Trans
{
    public class InvoiceBioSerCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        SqlTransaction tran = null;
        SqlCommand cmd = null;
        
        public DataTable GetMtlLblProcuct(string Type)
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("ServiceInvoiceMtlLblListBio_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable GetExpenditure()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("GetExpenditureBio_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable getCGST()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("getCGST_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable getSGST()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("getSGST_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable getIGST()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("getIGST_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable getTDS()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("getTDS_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }


        public DataTable GetProjectHead_Tech_Dealer(string Code, string Type)
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("LoadAssignEmpTec_Bio_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Code;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        //public DataTable GetQtnPerandExpAmt(string Code)
        //{
        //    // string[] Dts;
        //    SqlDataAdapter dAd = new SqlDataAdapter("LoadQtnPerandExpAmt_Bio_Sp", con);
        //    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
        //    dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Code;
        //    dAd.SelectCommand.CommandTimeout = 0;
        //    DataSet dSet = new DataSet();
        //    dAd.Fill(dSet);
        //    return dSet.Tables[0];
        //}

        public DataTable GetInvoicePendingComp(string Type)
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("LoadBiotechSrvInvoiceDetail_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            //if (Type != "0")
            //{
            //    Dts = Regex.Split(Type.ToString().Trim(), "-->");
            //    dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Dts[1].ToString().Trim();
            //    dAd.SelectCommand.Parameters.Add("@PartCode", SqlDbType.Char).Value = Dts[2].ToString().Trim();
            //}
            // dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = "0";
            //  dAd.SelectCommand.Parameters.Add("@PartCode", SqlDbType.Char).Value = "0";
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable GetSelectedCompType(string Type)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("[LoadSelectedCompTypeBio_Sp]", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetPrevActionDetails(string Code)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("[LoadPrvActDtlsBio]", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Code;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }


        public string Submit(InvoiceBioSerRequest InvoiceBioSerreq)
        {
            string[] strPlanDts;
            int SrNo;
            string[] Dts;
            string StrDispCode = "";
            string StrDisplayMsg = "";
            CommonCon ComCon = new CommonCon();
            DateTime sysdate = new DateTime(Convert.ToInt16(DateTime.Now.Year), Convert.ToInt16(DateTime.Now.Month), Convert.ToInt16(DateTime.Now.Day));

            try
            {
                String TallyNarration = "";
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //Save
                if (InvoiceBioSerreq.StrType.Trim() == "Save")
                {
                    StrDispCode = ComCon.GetMaxNo("InvoiceSales", "INV", InvoiceBioSerreq.CompCode.Trim(), con, tran);
                    TallyNarration = "InvNo:" + Convert.ToInt32((StrDispCode.Substring(12, 6)).ToString().Trim()) + " Dt:" + System.DateTime.Now.ToString("dd/MM/yyyy");

                    // Invoice Material Details    
                    #region

                    if (!string.IsNullOrEmpty(InvoiceBioSerreq.MtlDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(InvoiceBioSerreq.MtlDts, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            sb.Remove(0, sb.Length);
                            sb.Append("Insert into InvoicesalesDetails ");
                            sb.Append("(InvID,SrNo,PartCode,DPUOM,Qty,Rate,DCType,DCGSTPer,DSGSTPer,DIGSTPer,DHSNCode)");
                            sb.Append(" VALUES ('" + StrDispCode.Trim() + "','" + SrNo + "', ");
                            sb.Append("'" + Dts[1].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[2].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[3].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[4].ToString().Trim() + "', ");
                            //sb.Append("'" + Dts[5].ToString().Trim() + "', ");
                            sb.Append(" 'M', ");
                            sb.Append("'" + Dts[6].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[7].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[8].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[9].ToString().Trim() + "' )");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            TallyNarration += " Desc:" + Dts[10].ToString().Trim() + " Qty:" + Dts[3].ToString().Trim();
                        }
                    }
                    #endregion

                    // Invoice Labour Details    
                    #region
                    if (!string.IsNullOrEmpty(InvoiceBioSerreq.LblDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(InvoiceBioSerreq.LblDts, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            sb.Remove(0, sb.Length);
                            sb.Append("Insert into InvoicesalesDetails ");
                            sb.Append("(InvID,SrNo,PartCode,DPUOM,Qty,Rate,DCType,DCGSTPer,DSGSTPer,DIGSTPer,DHSNCode)");
                            sb.Append(" VALUES ('" + StrDispCode.Trim() + "','" + SrNo + "', ");
                            sb.Append("'" + Dts[1].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[2].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[3].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[4].ToString().Trim() + "', ");
                            //sb.Append("'" + Dts[5].ToString().Trim() + "', ");
                            sb.Append(" 'L', ");
                            sb.Append("'" + Dts[6].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[7].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[8].ToString().Trim() + "', ");
                            sb.Append("'" + Dts[9].ToString().Trim() + "' )");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            TallyNarration += " Desc:" + Dts[10].ToString().Trim() + " Qty:" + Dts[3].ToString().Trim();
                        }
                    }
                    #endregion                

                    // Master InvoiceCommercial
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO InvoiceSales ");
                    sb.Append("(InvId, MaxSrNo, Dt, Yr, InvCode,MECode, SOFCode, DICode, CustomerCode, IndentorCode,Qty,PONo, ");
                    sb.Append("PODate, ChapterNo, TMCode, ActOff,TransportName,TMobileNo,VehicleNo,NatureOfRemoval, ");
                    sb.Append("IssueDate, IssueTime, RemovalDate, RemovalTime, CSTPer, Frieght,Other, PrintCount, ");
                    sb.Append("CompanyCode, TCSPer, ExciseDutyInWords, CESSInwords, HEDCESSInwords, ");
                    sb.Append("AmountInWords, Remark, INVType, BasicInWords, OnAcParty, DescriptionManual, ");
                    sb.Append("NetWeight, GrossWeight, PortOfDischarge, DeliveryTerms, TransportRoute, ");
                    sb.Append("PortOfLoading, VesselFlightNo, TallyNarration, StockTransferStatus, ");
                    sb.Append("CGSTPer,SGSTPer,IGSTPer,CGSTInWords,SGSTInWords,IGSTInWords, MatAmt,LabAmt, InvDesc,InvUOM)");

                    sb.Append(" VALUES ('" + StrDispCode.Trim() + "','" + (StrDispCode.Substring(10, 8)) + "',");
                    sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + (StrDispCode.Substring(4, 5)) + "' ,");
                    sb.Append("'" + Convert.ToInt32((StrDispCode.Substring(12, 6)).ToString().Trim()) + "', ");
                    sb.Append("'" + InvoiceBioSerreq.SiteID.Trim() + "', ");
                    sb.Append("'" + InvoiceBioSerreq.CompNo.Trim() + "', ");
                    sb.Append("'" + InvoiceBioSerreq.PCACode.Trim() + "', ");
                    sb.Append("'" + InvoiceBioSerreq.CustomerCode.Trim() + "', ");
                    sb.Append("'" + InvoiceBioSerreq.IndentorCode.Trim() + "', ");
                    sb.Append("'" + InvoiceBioSerreq.InvQty.Trim() + "', ");
                    sb.Append("'" + InvoiceBioSerreq.PONo.Trim() + "' , ");
                    sb.Append("'" + ComCon.dateinyyyymmdd(InvoiceBioSerreq.PODate.Trim()) + "' , "); 
                    sb.Append("'0','01','-','-','NA','-','-', ");
                    sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',"); 
                    sb.Append(" '0', '0','0','0','" + InvoiceBioSerreq.CompCode.Trim() + "','0','',");
                    sb.Append(" '' ,'','" + InvoiceBioSerreq.InvoiceTotalInWord.Trim() + "','" + InvoiceBioSerreq.Remark.Trim() + "','CM',");
                    sb.Append("'Zero Only','NIL','NIL','0','0','NIL','NIL','NIL','NIL','NIL','" + TallyNarration.ToString().Trim() + "','0',");
                    sb.Append("'0','0',");
                    sb.Append("'0','Zero Only','Zero Only','Zero Only','" + InvoiceBioSerreq.MatAmt.Trim() + "','" + InvoiceBioSerreq.LabAmt.Trim() + "','" + InvoiceBioSerreq.InvDesc.Trim() + "','" + InvoiceBioSerreq.InvUOM.Trim() + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                  

                    //Action Taken Dtls
                    #region
                    if (!string.IsNullOrEmpty(InvoiceBioSerreq.ActDtls.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(InvoiceBioSerreq.ActDtls, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO InvoiceSalesDetailsSub(INVID, SrNo, PSCode, Status, WsStatusKalaToPms)");
                            sb.Append(" VALUES ('" + StrDispCode.Trim() + "','" + SrNo + "', ");
                            sb.Append(" '" + Dts[0].ToString().Trim() + "','0','0' )");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                    #endregion
                    // Issue Against Bill
                    //RecieptNote Transaction
                   // if (StrDispCode.Trim().Substring(0, 3) == "CIV")
                   // {
                        #region
                        String OnAccOf = ComCon.getTranName("Select GroupCode from Indentor where Icode ='" + InvoiceBioSerreq.IndentorCode.Trim() + "'", "Indentor", "GroupCode", con, tran);
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO ReceiptNoteTransaction");
                        sb.Append("(ONAccountOF,trnsIndcode,TrnsCustCode,TransactionCode,TrnsPCCode, IssueCode, IssueDt, IssueAmount, Type, DueDt, Status,TransCompCode)");
                        sb.Append(" VALUES('" + OnAccOf.ToString().Trim() + "','" + InvoiceBioSerreq.IndentorCode.Trim() + "','" + InvoiceBioSerreq.CustomerCode.Trim() + "', ");
                        sb.Append(" '" + StrDispCode.Trim() + "','08.023','" + InvoiceBioSerreq.CompCode.Trim() + "',");
                        sb.Append(" '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                        sb.Append(" '" + Convert.ToDouble(InvoiceBioSerreq.InvoiceTotalAmt.Trim()) + "','IV',");
                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                        sb.Append("'0','" + InvoiceBioSerreq.CompCode.Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion
                   // }

                    //DateTime.Now.ToString("yyyy-MM-dd")
                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@EmpID", InvoiceBioSerreq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "InvoiceServiceBio");
                    cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", InvoiceBioSerreq.CompCode.Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Ask ADG
                    sb.Remove(0, sb.Length);
                    sb.Append("Update CustomerComplaintBio SET INVStatus='C' where CompNo='" + InvoiceBioSerreq.CompNo.Trim() + "'  ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    sb.Remove(0, sb.Length);

                    tran.Commit();

                    StrDisplayMsg = "";
                    StrDisplayMsg = "Commercial Invoice  save successfully with no : " + StrDispCode.Trim() + " ";
                }
                //Update 
                else if (InvoiceBioSerreq.StrType.Trim() == "Update")
                {

                }
                return StrDisplayMsg;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace" + ex.StackTrace.ToString() + "\n" + "Message" + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }


        //public string Submit(InvoiceBioSerRequest InvoiceBioSerreq)
        //{
        //    string[] strPlanDts;
        //    int SrNo;
        //    string[] Dts;
        //    string StrDispCode = "";
        //    string StrDisplayMsg = "";
        //    CommonCon ComCon = new CommonCon();
        //    DateTime sysdate = new DateTime(Convert.ToInt16(DateTime.Now.Year), Convert.ToInt16(DateTime.Now.Month), Convert.ToInt16(DateTime.Now.Day));

        //    try
        //    {
        //        String TallyNarration = "";
        //        if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
        //        tran = con.BeginTransaction();
        //        //Save
        //        if (InvoiceBioSerreq.StrType.Trim() == "Save")
        //        {

        //            StrDispCode = ComCon.GetMaxNo("InvoiceCommercial", "CIV", InvoiceBioSerreq.CompCode.Trim(), con, tran);
        //            TallyNarration = "InvNo:" + Convert.ToInt32((StrDispCode.Substring(12, 6)).ToString().Trim()) + " Dt:" + System.DateTime.Now.ToString("dd/MM/yyyy");

        //            // Invoice Material Details    
        //            #region
        //            strPlanDts = null;
        //            strPlanDts = Regex.Split(InvoiceBioSerreq.MtlDts, "@#@");
        //            SrNo = 0;
        //            foreach (String StrSub in strPlanDts)
        //            {
        //                SrNo += 1;
        //                Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
        //                sb.Remove(0, sb.Length);
        //                sb.Append("Insert into InvoiceCommercialDetails ");
        //                sb.Append("(CCode,SrNo,PartCode,PUOM,Qty,Rate,Amount,CType,CGSTPer,SGSTPer,IGSTPer,HSNCode)");
        //                sb.Append(" VALUES ('" + StrDispCode.Trim() + "','" + SrNo + "', ");
        //                sb.Append("'" + Dts[1].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[2].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[3].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[4].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[5].ToString().Trim() + "', ");
        //                sb.Append(" 'M', ");
        //                sb.Append("'" + Dts[6].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[7].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[8].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[9].ToString().Trim() + "' )");
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.Transaction = tran;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();
        //                TallyNarration += " Desc:" + Dts[10].ToString().Trim() + " Qty:" + Dts[3].ToString().Trim();
        //            }
        //            #endregion

        //            // Invoice Labour Details    
        //            #region
        //            strPlanDts = null;
        //            strPlanDts = Regex.Split(InvoiceBioSerreq.LblDts, "@#@");
        //            SrNo = 0;
        //            foreach (String StrSub in strPlanDts)
        //            {
        //                SrNo += 1;
        //                Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
        //                sb.Remove(0, sb.Length);
        //                sb.Append("Insert into InvoiceCommercialDetails ");
        //                sb.Append("(CCode,SrNo,PartCode,PUOM,Qty,Rate,Amount,CType,CGSTPer,SGSTPer,IGSTPer,HSNCode)");
        //                sb.Append(" VALUES ('" + StrDispCode.Trim() + "','" + SrNo + "', ");
        //                sb.Append("'" + Dts[1].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[2].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[3].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[4].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[5].ToString().Trim() + "', ");
        //                sb.Append(" 'L', ");
        //                sb.Append("'" + Dts[6].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[7].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[8].ToString().Trim() + "', ");
        //                sb.Append("'" + Dts[9].ToString().Trim() + "' )");
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.Transaction = tran;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();
        //                TallyNarration += " Desc:" + Dts[10].ToString().Trim() + " Qty:" + Dts[3].ToString().Trim();
        //            }
        //            #endregion                

        //            // Master InvoiceCommercial
        //            sb.Remove(0, sb.Length);
        //            sb.Append("Insert into InvoiceCommercial ");
        //            sb.Append("(CCode,MaxSrNo,Dt,Yr,INVCode,MemoCode,CompNo,IndentorCode,IndentorChk,SiteID,VAT,CST,SrvTax,SBCess,KKCess,CGSTPer,SGSTPer,IGSTPer,MatAmt,LabAmt,InvoiceTotal,CompanyCode,Remark,TallyNarration,InvDesc,InvQty,InvUOM,INVType)");
        //            sb.Append(" VALUES ('" + StrDispCode.Trim() + "','" + (StrDispCode.Substring(10, 8)) + "',");
        //            sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + (StrDispCode.Substring(4, 5)) + "' ,");
        //            sb.Append("'" + Convert.ToInt32((StrDispCode.Substring(12, 6)).ToString().Trim()) + "','" + InvoiceBioSerreq.PCACode.Trim() + "','" + InvoiceBioSerreq.CompNo.Trim() + "', ");
        //            sb.Append(" '" + InvoiceBioSerreq.IndentorCode.Trim() + "',  '" + InvoiceBioSerreq.IndentorChk.Trim() + "', ");
        //            sb.Append(" '" + InvoiceBioSerreq.SiteID.Trim() + "', ");
        //            sb.Append(" '0','0','0','0','0','0','0','0', ");
        //            sb.Append(" '" + InvoiceBioSerreq.MatAmt.Trim() + "','" + InvoiceBioSerreq.LabAmt.Trim() + "','" + InvoiceBioSerreq.InvoiceTotalInWord.Trim() + "', ");
        //            sb.Append("  '" + InvoiceBioSerreq.CompCode.Trim() + "' , ");
        //            sb.Append(" '" + InvoiceBioSerreq.Remark.Trim() + "','" + TallyNarration.ToString().Trim() + "','" + InvoiceBioSerreq.InvDesc.Trim() + "','" + InvoiceBioSerreq.InvQty.Trim() + "','" + InvoiceBioSerreq.InvUOM.Trim() + "','CM')");
        //            cmd = new SqlCommand(sb.ToString(), con);
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();

        //            //Action Taken Dtls
        //            #region
        //            if (!string.IsNullOrEmpty(InvoiceBioSerreq.ActDtls.ToString().Trim()))
        //            {
        //                strPlanDts = null;
        //                strPlanDts = Regex.Split(InvoiceBioSerreq.ActDtls, "@#@");
        //                SrNo = 0;
        //                foreach (String StrSub in strPlanDts)
        //                {
        //                    SrNo += 1;
        //                    Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("Insert into InvoiceCommercialActDetails ");
        //                    sb.Append(" (CCode,SrNo,ActNo) ");
        //                    sb.Append(" VALUES ('" + StrDispCode.Trim() + "','" + SrNo + "', ");
        //                    sb.Append(" '" + Dts[0].ToString().Trim() + "' )");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();
        //                }
        //            }
        //            #endregion

        //            //RecieptNote Transaction
        //            if (StrDispCode.Trim().Substring(0, 3) == "CIV")
        //            {
        //                #region
        //                String OnAccOf = ComCon.getTranName("Select GroupCode from Indentor where Icode ='" + InvoiceBioSerreq.IndentorCode.Trim() + "'", "Indentor", "GroupCode", con, tran);
        //                sb.Remove(0, sb.Length);
        //                sb.Append("INSERT INTO ReceiptNoteTransaction");
        //                sb.Append("(ONAccountOF,trnsIndcode,TrnsCustCode,TransactionCode,TrnsPCCode, IssueCode, IssueDt, IssueAmount, Type, DueDt, Status,TransCompCode)");
        //                sb.Append(" VALUES('" + OnAccOf.ToString().Trim() + "','" + InvoiceBioSerreq.IndentorCode.Trim() + "','" + InvoiceBioSerreq.CustomerCode.Trim() + "', ");
        //                sb.Append(" '" + StrDispCode.Trim() + "','08.023','" + InvoiceBioSerreq.CompCode.Trim() + "',");
        //                sb.Append(" '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");                       
        //                sb.Append(" '" + Convert.ToDouble(InvoiceBioSerreq.InvoiceTotalAmt.Trim()) + "','IV',");
        //                sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");                      
        //                sb.Append("'0','" + InvoiceBioSerreq.CompCode.Trim() + "')");                       
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.Transaction = tran;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();  
        //                #endregion
        //            }

        //            //DateTime.Now.ToString("yyyy-MM-dd")
        //            //****************User Acivity****************
        //            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //            cmd.Parameters.AddWithValue("@EmpID", InvoiceBioSerreq.EmpCode.Trim());
        //            cmd.Parameters.AddWithValue("@TransactionType", "S");
        //            cmd.Parameters.AddWithValue("@TransactionFrom", "InvoiceServiceBio");
        //            cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode.Trim());
        //            cmd.Parameters.AddWithValue("@CompanyCode", InvoiceBioSerreq.CompCode.Trim());
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();

        //            //Ask ADG
        //            sb.Remove(0, sb.Length);
        //            sb.Append("Update CustomerComplaintBio SET INVStatus='C' where CompNo='" + InvoiceBioSerreq.CompNo.Trim() + "'  ");
        //            cmd = new SqlCommand(sb.ToString(), con);
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();
        //            sb.Remove(0, sb.Length);

        //          //  tran.Commit();

        //            StrDisplayMsg = "";
        //            StrDisplayMsg = "Commercial Invoice  save successfully with no : " + StrDispCode.Trim() + " ";
        //        }
        //        //Update 
        //        else if (InvoiceBioSerreq.StrType.Trim() == "Update")
        //        {

        //        }
        //        return StrDisplayMsg;
        //    }
        //    catch (Exception ex)
        //    {
        //        tran.Rollback();               
        //        return ("StackTrace" + ex.StackTrace.ToString() + "\n" + "Message" + ex.Message.ToString());
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //}
    }
}