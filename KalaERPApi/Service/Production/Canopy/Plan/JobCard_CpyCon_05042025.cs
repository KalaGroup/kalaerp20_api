using KalaERPApi.Models.Request;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace KalaERPApi.Service
{
    public class JobCard_CpyCon
    {

        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        CommonCon ComCon = new CommonCon();
        public SqlTransaction tran = null;
      
        SqlCommand cmd = null;
        #endregion       

        public DataTable GetCpyPlan(string strJobCardType, string strcompID)
        {
            if (strcompID == "28")
            {
                SqlDataAdapter dAd = new SqlDataAdapter("GetJobCard_Cpy_Bangalore_PlanDts", con);
                //SqlDataAdapter dAd = new SqlDataAdapter("GetJobCardDGDts_fs", con);
                dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = strJobCardType;
                dAd.SelectCommand.Parameters.Add("@CompId", SqlDbType.Char).Value = strcompID;
                dAd.SelectCommand.CommandTimeout = 0;
                DataSet dSet = new DataSet();
                dAd.Fill(dSet);
                return dSet.Tables[0];
            }
            else
            {
                SqlDataAdapter dAd = new SqlDataAdapter("GetJobCard_Cpy_PlanDts", con);
                //SqlDataAdapter dAd = new SqlDataAdapter("GetJobCardDGDts_fs", con);
                dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = strJobCardType;
                dAd.SelectCommand.Parameters.Add("@CompId", SqlDbType.Char).Value = strcompID;
                dAd.SelectCommand.CommandTimeout = 0;
                DataSet dSet = new DataSet();
                dAd.Fill(dSet);
                return dSet.Tables[0];
            }
        }

        public string Submit(JobCard_CpyRequest job_Cpyreq)
        {

            DataSet dsDetailsSub;
            DataSet dsCanopyPlanDtsSub;
         

            string StrDisplayMsg = "";
            string StrDispCode_CPYPlan = "";
            String StrDispCode_MaterialReq_CNC_ALL_msg = "";
            String StrDispCode_MaterialReq_WH_ALL_msg = "";
            String StrDispCode_MaterialReq_FAB_ALL_msg = "";
            String StrDispCode_MaterialReq_POC_ALL_msg = "";
            string StrDispCode_MaterialReq_CNC = "";

            // String StrCompCode = "";
            string[] strPlanDts;
            string[] Dts;
            //StrCompCode = job_Cpyreq.PCCode.Trim().Substring(0, 2).Trim();
            int SrNo;
            string ParentPart = "";

            if (string.IsNullOrEmpty(job_Cpyreq.JobCard_CpyDts))
            {
                StrDisplayMsg = "Please Check Record !";
                return StrDisplayMsg;
            }


            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //Save Cpy Plan
                StrDispCode_CPYPlan = ComCon.GetMaxNo("CanopyPlan", "CPY", job_Cpyreq.CompCode.Trim(), con, tran);


                //For Mst Save
                #region
                cmd = new SqlCommand("InsertCanopyPlan", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CPCode", StrDispCode_CPYPlan.Trim());
                cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                cmd.Parameters.AddWithValue("@MaxSrNo", StrDispCode_CPYPlan.Substring(10, 8).Trim());
                cmd.Parameters.AddWithValue("@Yr", StrDispCode_CPYPlan.Substring(4, 5).Trim());
                cmd.Parameters.AddWithValue("@FromDt", DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                cmd.Parameters.AddWithValue("@ToDt", DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                cmd.Parameters.AddWithValue("@PlanPCCode", job_Cpyreq.PCCode.Trim()); //"01.041"
                cmd.Parameters.AddWithValue("@CompanyCode", job_Cpyreq.CompCode.Trim());
                cmd.Parameters.AddWithValue("@PlanType", "G");
                cmd.Parameters.AddWithValue("@AutoFlg", "Yes"); //ASK FS
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                #endregion


                //For CPY Plan Details  Save --Start
                #region

                strPlanDts = Regex.Split(job_Cpyreq.JobCard_CpyDts, "@@#@@");
                SrNo = 0;


                foreach (string StrSub in strPlanDts) //For Loop Start
                { // Main For Loop Start Bracket 

                    SrNo += 1;
                    Dts = Regex.Split(StrSub.ToString().Trim(), "@#@");

                    //     0               1                       2                   3                       4                   5                       6                       7                       8                       9                       10
                    //item.KVA + "@#@" + item.Model + "@#@" + item.Partcode + "@#@" + item.FNorm + "@#@" + item.TotStk + "@#@" + item.WIPStk + "@#@" + item.PenPlanQty + "@#@" + item.PReq + "@#@" + item.PlanQty + "@#@" + item.BatchQty + "@#@" + item.Bomcode

                    //For Dts Save
                    #region
                    cmd = new SqlCommand("InsertCanopyPlanDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CPCode", StrDispCode_CPYPlan.Trim());
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                    cmd.Parameters.AddWithValue("@PartCode", Dts[2].ToString().Trim());
                    cmd.Parameters.AddWithValue("@BomCode", Dts[10].ToString().Trim());

                    string PartCodeWOP = "";
                    PartCodeWOP = ComCon.getTranName("SELECT Partcode FROM BOMdetails where BOMCode='" + Dts[10].ToString().Trim() + "' " +
                    "AND KitCode='" + Dts[2].ToString().Trim() + "' AND Partcode like '004%' ", "tblBP", "Partcode", con, tran);

                    cmd.Parameters.AddWithValue("@PartCodeWOP", PartCodeWOP.Trim());
                    cmd.Parameters.AddWithValue("@Qty", int.Parse(Dts[8].ToString().Trim()));
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    #endregion
                    //For Dts Save

                    //Insert Temp Turret Kit

                    //sb.Append("select '" + StrDispCode_CPYPlan.Trim() + "' as CPCode ,'" + Dts[10].ToString().Trim() + "'  as BomCode ,");
                    //sb.Append("TKITID,'" + Dts[2].ToString().Trim() + "'  as CanopyPartCode,CanopyPartCode as TurretKitPartcode,SheetPartCode,KITType,TLength,TWidth,TThickness,SerialNo,SerialQty ");
                    //sb.Append("from TurretKit where Auth='1' and Canopypartcode in ( select Partcode from BomDetails where BOMCode='" + Dts[10].ToString().Trim() + "' ");
                    //sb.Append("and substring(partcode,1,4) in ('0121','0122') and substring(partcode,12,2) in ('15'))  order By TKITid");

                    #region
                    // Mst
                    sb.Remove(0, sb.Length);

                    //sb.Append("insert Into TurretKitForPrc(CPCode,BOMCode,TKITID,CanopyPartCode,TurretKitPartcode,SheetPartCode,KITType,TLength,TWidth,TThickness,SerialNo,SerialQty,CompCode) ");
                    //sb.Append("select '" + StrDispCode_CPYPlan.Trim() + "' as CPCode ,'" + Dts[10].ToString().Trim() + "'  as BomCode ,");
                    //sb.Append("TKITID,'" + Dts[2].ToString().Trim() + "'  as CanopyPartCode,CanopyPartCode as TurretKitPartcode,SheetPartCode,KITType,TLength,TWidth,TThickness,SerialNo,SerialQty,CatID ");
                    //sb.Append("from TurretKit where Auth='1' and Canopypartcode in ( select Partcode from BomDetails where BOMCode='" + Dts[10].ToString().Trim() + "' ");
                    //sb.Append("and substring(partcode,1,4) in ('0121','0122') and substring(partcode,12,2) in ('15'))  order By TKITid");

                    sb.Append("insert Into TurretKitForPrc(CPCode,BOMCode,TKITID,CanopyPartCode,TurretKitPartcode,SheetPartCode,KITType,TLength,TWidth,TThickness,SerialNo,SerialQty,CompCode,CatID) ");
                    sb.Append("select S.CPCode,S.BomCode,S.TKITID,S.CanopyPartCode,S.TurretKitPartcode,S.SheetPartCode,S.KITType,S.TLength	,S.TWidth,S.TThickness,	S.SerialNo,	S.SerialQty,S1.CompCode,s1.CatID  from (select '" + StrDispCode_CPYPlan.Trim() + "' as CPCode ,'" + Dts[10].ToString().Trim() + "'  as BomCode ,");
                    sb.Append("TKITID,'" + Dts[2].ToString().Trim() + "'  as CanopyPartCode,CanopyPartCode as TurretKitPartcode,SheetPartCode,KITType,TLength,TWidth,TThickness,SerialNo,SerialQty,CatID ");
                    sb.Append("from TurretKit where Auth='1' and Canopypartcode in ( select Partcode from BomDetails where BOMCode='" + Dts[10].ToString().Trim() + "' ");
                    sb.Append("and substring(partcode,1,4) in ('0121','0122') and substring(partcode,12,2) in ('15','25','35','45'))) as S" +
                        " inner join ( select  count(PPM.BracketID) as B ,PPM.CatID ,PPM.Location as CompCode  from ProductionPlanMaster PPM  where PPM.Active='1' " +
                        "group by PPM.Location,PPM.CatID) as S1 on S.CatID =S1.CatID  order By TKITid ");



                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.CommandTimeout = 0;
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    // Dts

                    sb.Remove(0, sb.Length);
                    sb.Append("insert Into TurretKitForPrcDts(CPCode,TKITID,SrNo,PartCode,Qty,TLength,TWidth,THeight,TTHickness,TLossWt,TLength1,TLength2,TWidth1,TWidth2,TLosssqft,TCatagorycode) ");
                    sb.Append("select '" + StrDispCode_CPYPlan.Trim() + "' as CPCode,TKITID,SrNo,PartCode,Qty,TLength,TWidth,THeight,TTHickness,TLossWt,TLength1,TLength2,");
                    sb.Append("TWidth1,TWidth2,TLosssqft,TCatagorycode from TurretKitdetails where TKITid in ( select TKITid ");
                    sb.Append("from TurretKit where Auth='1' and Canopypartcode in ( select Partcode from BomDetails where BOMCode='" + Dts[10].ToString().Trim() + "' and substring(partcode,1,4) in ('0121','0122') and substring(partcode,12,2) in ('15','25','35','45')))  order By TKITid");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.CommandTimeout = 0;
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion
                    //Insert Temp Turret Kit

                    //For DtsSub Save
                    #region
                    //dsDetailsSub = ComCon.procTranDS("select bd.Partcode,PartDesc,(select Rate from ProfitcenterPLDetails where Partcode=bd.Partcode and ProfitcenterCode = '01.007') as Rate, " +
                    //"isNull((select isNull(Max(MarAMT),0) as Strokes from ProfitcenterPLDetails where Partcode=bd.Partcode and ProfitcenterCode = '01.002'),0) as Strokes " +
                    //" from BOM B Inner Join BOMDetails Bd on B.BOMCode = Bd.BOMCode " +
                    //" inner Join Part P on bd.Partcode = P.Partcode  where B.BOMCode = '" + Dts[10].ToString().Trim() + "' and " +
                    //" B.Active = '1' and B.Auth = '1' and Kit = '1' and Bd.MOB = 'M' and  Bd.KitCode like '004%' and substring(Bd.KitCode,11,1) in ('4','5') and Bd.Partcode like '004%' ", "tbl_RaiseReqDtsSub", con, tran);

                    dsDetailsSub = ComCon.procTranDS(" select s.Partcode, s.PartDesc, s.Rate, s.KVA, s.Strokes,s1.CompCode,S1.CatID from " +
                        "(select DISTINCT  bd.Partcode,p.PartDesc," +
                        "(select Rate from ProfitcenterPLDetails where Partcode=bd.Partcode and ProfitcenterCode = '01.007') as Rate,  P1.KVA ," +
                           "isNull((select isNull(Max(MarAMT),0) as Strokes from ProfitcenterPLDetails where Partcode=bd.Partcode and ProfitcenterCode = '01.002'),0) as Strokes" +
                           " ,BM.BID as BracketId " +
                           " , CASE WHEN SUBSTRING(bd.Partcode, 12, 1) = '1' THEN '029' " +
                           " WHEN SUBSTRING(bd.Partcode, 12, 1) = '2' THEN '084'" +
                           " WHEN SUBSTRING(bd.Partcode, 12, 1) = '3' THEN '038'" +
                           " ELSE 'other'END AS CatCode" +
                           ",CASE WHEN SUBSTRING(bd.Partcode, 12, 1) = '1' THEN 'cpy'" +
                           " WHEN SUBSTRING(bd.Partcode, 12, 1) = '2' THEN 'BF'" +
                           " WHEN SUBSTRING(bd.Partcode, 12, 1) = '3' THEN 'FT' " +
                           " ELSE 'other'END AS CatName " +
                           " from BOM B Inner Join BOMDetails Bd on B.BOMCode = Bd.BOMCode " +
                           " inner Join Part P on bd.Partcode = P.Partcode " +
                           " inner Join Part P1 on b.Partcode = P1.Partcode	 " +
                           " inner Join BracketMst BM   ON p1.kva BETWEEN BM.fromkva AND BM.tokva " +
                           " where B.BOMCode = '" + Dts[10].ToString().Trim() + "' and " +
                           " B.Active = '1' and B.Auth = '1' and p.Kit = '1' and Bd.MOB = 'M' and  Bd.KitCode like '004%' and substring(Bd.KitCode,11,1) in ('4','5') and Bd.Partcode like '004%' ) as S  " +
                           " inner join ( select  count(PPM.BracketID) as B ,PPM.BracketID,PPM.CatID ,PPM.Location as CompCode from ProductionPlanMaster PPM" +
                           " where PPM.Active = '1' group by PPM.Location, PPM.BracketID, PPM.CatID) as S1 on S.CatCode = S1.CatID and S.BracketId = S1.BracketID", "tbl_RaiseReqDtsSub", con, tran);



                    if (dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows.Count > 0)
                    {
                        for (int m = 0; m < dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows.Count; m++)
                        {
                            if (double.Parse(dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Rate"].ToString().Trim()) >= 1000) //25
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("insert Into CanopyPlanDtsSub(CPCode,CpyPartCode,SrNo,PartCode,CPQty,Rate,Strokes,CompCode,CatID)");
                                sb.Append(" values ('" + StrDispCode_CPYPlan.Trim() + "','" + Dts[2].ToString().Trim() + "'," + (m + 1) + " ,");
                                sb.Append(" '" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "','" + int.Parse(Dts[8].ToString().Trim()) + "',");
                                sb.Append("'" + double.Parse(dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Rate"].ToString().Trim()) + "','" + double.Parse(dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Strokes"].ToString().Trim()) + "' ,'" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["CompCode"].ToString().Trim() + "' ,'" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["CatID"].ToString().Trim() + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.CommandTimeout = 0;
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                                //OS Fab Plan
                                #region
                                if (int.Parse(Dts[8].ToString().Trim()) >= 1) //Plan Qty
                                {
                                    if (dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim().Substring(11, 1) == "1" ||
                                            dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim().Substring(11, 1) == "0")//CPY
                                    {

                                        #region
                                        ParentPart = "";
                                        ParentPart = ComCon.getTranName("select PartCode  as ParentPart from BOMDetails where KitCode='" + PartCodeWOP.Trim() + "' " +
                                         " and BOMCode = '" + Dts[10].ToString().Trim() + "'  " +
                                         " and Partcode like '004%' and substring(Partcode, 11, 1) = '4' ", "tblParentPart", "ParentPart", con, tran);

                                        sb.Remove(0, sb.Length);
                                        sb.Append("insert Into CanopyPlanOSDetails(CPCode,CpyPartCode,SrNo,PartCode,Scode,Qty,OSFqty,OSFStatus,ParentPart)");
                                        sb.Append(" values ('" + StrDispCode_CPYPlan.Trim() + "','" + Dts[2].ToString().Trim() + "'," + (m + 1) + " ,");
                                        sb.Append(" '" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "','02.13.01.01.23.0001','" + (double.Parse(Dts[8].ToString().Trim()) / 2) + "',");
                                        sb.Append("'0','P','" + ParentPart.Trim() + "' )");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.CommandTimeout = 0;
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        // Commented by KB on 27/12/2024 as Supplier change Required from Human to GSEC
                                        //sb.Remove(0, sb.Length);
                                        //sb.Append("insert Into CanopyPlanOSDetails(CPCode,CpyPartCode,SrNo,PartCode,Scode,Qty,OSFqty,OSFStatus,ParentPart)");
                                        //sb.Append(" values ('" + StrDispCode_CPYPlan.Trim() + "','" + Dts[2].ToString().Trim() + "'," + (m + 1) + " ,");
                                        //sb.Append(" '" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "','02.08.01.01.01.0039','" + (double.Parse(Dts[8].ToString().Trim()) / 2) + "',");
                                        //sb.Append("'0','P','" + ParentPart.Trim() + "' )");
                                        //cmd = new SqlCommand(sb.ToString(), con);
                                        //cmd.CommandTimeout = 0;
                                        //cmd.Transaction = tran;
                                        //cmd.ExecuteNonQuery();
                                        //cmd.Dispose();

                                        // Added GSEC Against Human
                                        sb.Remove(0, sb.Length);
                                        sb.Append("insert Into CanopyPlanOSDetails(CPCode,CpyPartCode,SrNo,PartCode,Scode,Qty,OSFqty,OSFStatus,ParentPart)");
                                        sb.Append(" values ('" + StrDispCode_CPYPlan.Trim() + "','" + Dts[2].ToString().Trim() + "'," + (m + 1) + " ,");
                                        sb.Append(" '" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "','02.04.01.01.01.0573','" + (double.Parse(Dts[8].ToString().Trim()) / 2) + "',");
                                        sb.Append("'0','P','" + ParentPart.Trim() + "' )");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.CommandTimeout = 0;
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                        #endregion

                                    }
                                    else if (dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim().Substring(11, 1) == "2") //BF
                                    {
                                        #region
                                        sb.Remove(0, sb.Length);
                                        sb.Append("insert Into CanopyPlanOSDetails(CPCode,CpyPartCode,SrNo,PartCode,Scode,Qty,OSFqty,OSFStatus,ParentPart)");
                                        sb.Append(" values ('" + StrDispCode_CPYPlan.Trim() + "','" + Dts[2].ToString().Trim() + "'," + (m + 1) + " ,");
                                        sb.Append(" '" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "','02.01.01.01.01.0305','" + (double.Parse(Dts[8].ToString().Trim()) / 2) + "',");
                                        sb.Append("'0','P','" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "' )");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.CommandTimeout = 0;
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        sb.Remove(0, sb.Length);
                                        sb.Append("insert Into CanopyPlanOSDetails(CPCode,CpyPartCode,SrNo,PartCode,Scode,Qty,OSFqty,OSFStatus,ParentPart)");
                                        sb.Append(" values ('" + StrDispCode_CPYPlan.Trim() + "','" + Dts[2].ToString().Trim() + "'," + (m + 1) + " ,");
                                        sb.Append(" '" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "','02.01.01.01.23.0009','" + (double.Parse(Dts[8].ToString().Trim()) / 2) + "',");
                                        sb.Append("'0','P','" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "' )");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.CommandTimeout = 0;
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                        #endregion

                                    }
                                    else if (dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim().Substring(11, 1) == "3")//FT
                                    {
                                        #region
                                        sb.Remove(0, sb.Length);
                                        sb.Append("insert Into CanopyPlanOSDetails(CPCode,CpyPartCode,SrNo,PartCode,Scode,Qty,OSFqty,OSFStatus,ParentPart)");
                                        sb.Append(" values ('" + StrDispCode_CPYPlan.Trim() + "','" + Dts[2].ToString().Trim() + "'," + (m + 1) + " ,");
                                        sb.Append(" '" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "','02.01.01.01.01.0305','" + (double.Parse(Dts[8].ToString().Trim())) + "',");
                                        sb.Append("'0','P','" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "' )");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.CommandTimeout = 0;
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                        #endregion
                                    }

                                }
                                #endregion
                                //OS Fab Plan
                            }
                            else if (double.Parse(dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Rate"].ToString().Trim()) < 1000)
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("insert Into CanopyPlanDtsSubBelowStdRate(CPCode,CpyPartCode,SrNo,PartCode,CPQty,Rate,Strokes,CompCode,CatID)");
                                sb.Append(" values ('" + StrDispCode_CPYPlan.Trim() + "','" + Dts[2].ToString().Trim() + "'," + (m + 1) + " ,");
                                sb.Append(" '" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Partcode"].ToString().Trim() + "','" + int.Parse(Dts[8].ToString().Trim()) + "',");
                                sb.Append("'" + double.Parse(dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Rate"].ToString().Trim()) + "','" + double.Parse(dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["Strokes"].ToString().Trim()) + "','" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["CompCode"].ToString().Trim() + "','" + dsDetailsSub.Tables["tbl_RaiseReqDtsSub"].Rows[m]["CatID"].ToString().Trim() + "' )");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.CommandTimeout = 0;
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }




                        }

                    }

                    #endregion
                    //For DtsSub Save

                    //All Requisition Start
                    #region
                    //Auto WH Req --START --******************************
                    #region

                    string StrDispCode_WHReq = "";

                    String StrWH_PCCode = "0";
                    if (job_Cpyreq.PCCode.Trim() == "01.041")
                    {
                        StrWH_PCCode = "01.005"; //CNC
                    }
                    else if (job_Cpyreq.PCCode.Trim() == "03.064")
                    {
                        StrWH_PCCode = "03.040"; ////CNC
                    }
                    else if (job_Cpyreq.PCCode.Trim() == "28.012")
                    {
                        StrWH_PCCode = "28.017"; ////CNC
                    }


                    StrDispCode_WHReq = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", job_Cpyreq.CompCode.Trim(), con, tran);


                    if (string.IsNullOrEmpty(StrDispCode_MaterialReq_WH_ALL_msg.Trim()))
                    {
                        StrDispCode_MaterialReq_WH_ALL_msg = StrDispCode_WHReq.Trim();
                    }
                    else
                    {
                        StrDispCode_MaterialReq_WH_ALL_msg += ", " + StrDispCode_WHReq.Trim();
                    }


                    sb.Remove(0, sb.Length);
                    sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                        " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                    sb.Append("values('" + StrDispCode_WHReq.Trim() + "','" + StrDispCode_WHReq.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    sb.Append("'" + StrDispCode_WHReq.Substring(4, 5).Trim() + "','" + StrWH_PCCode.Trim() + "','01.091','" + Dts[2].ToString().Trim() + "','" + job_Cpyreq.CompCode.Trim() + "','" + Dts[8].ToString().Trim() + "','P','WIP',");
                    sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + " ','1','1','1','" + StrDispCode_CPYPlan.Trim() + "','0' )");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Details Req STRAT ********************

                    DataSet dsReqDts_WHReq = ComCon.procTranDS("exec InternalReqLogisticsdetailsWHKIT_Canopy '" + Dts[2].ToString().Trim() + "'", "tbl_ReqDts_WHReq", con, tran);
                    if (dsReqDts_WHReq != null && dsReqDts_WHReq.Tables["tbl_ReqDts_WHReq"].Rows.Count > 0)
                    {
                        int SrNoReq_WHReq = 0;

                        for (int cntd = 0; cntd < dsReqDts_WHReq.Tables["tbl_ReqDts_WHReq"].Rows.Count; cntd++)
                        {
                            SrNoReq_WHReq = SrNoReq_WHReq + 1;
                            cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@REQCode", StrDispCode_WHReq.Trim());
                            cmd.Parameters.AddWithValue("@SrNo", SrNoReq_WHReq);
                            cmd.Parameters.AddWithValue("@PartCode", dsReqDts_WHReq.Tables["tbl_ReqDts_WHReq"].Rows[cntd]["Partcode"].ToString().Trim());
                            cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_WHReq.Tables["tbl_ReqDts_WHReq"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(Dts[8].ToString().Trim()));
                            cmd.Parameters.AddWithValue("@REQStatus", "P");
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //GetReqDetailsSub(StrDispCode_CNCReq.Trim(), dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows[cntd]["Partcode"].ToString().Trim(), 0, double.Parse(Dts[8].ToString().Trim()));
                        }
                    }

                    //Details Req  END ***************************

                    //}

                    #endregion
                    //Auto WH Req --END


                    //Auto CNC Req --START --******************************
                    #region  CNC
                    #region  old code
                    string StrDispCode_CNCReq = "";

                    String StrCNC_PCCode = "0";
                    //if (job_Cpyreq.PCCode.Trim() == "01.041")
                    //{
                    //    StrCNC_PCCode = "01.009"; //CNC
                    //}
                    //else if (job_Cpyreq.PCCode.Trim() == "03.064")
                    //{
                    //    StrCNC_PCCode = "03.061"; ////CNC
                    //}
                    //else if (job_Cpyreq.PCCode.Trim() == "28.012")
                    //{
                    //    StrWH_PCCode = "28.013"; ////CNC
                    //}

                    #endregion

                    string RequisitionForPartCode = "";
                    string CatID = "";
                    string CompCode = "";


                    dsCanopyPlanDtsSub = ComCon.procTranDS("select CatID,CompCode from CanopyPlanDtsSub where CPCode='" + StrDispCode_CPYPlan.ToString().Trim() + "'  group by CatID, CompCode", "tbl_CanopyPlanDtsSub", con, tran);

                    if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows.Count > 0)
                    {

                        for (int m1 = 0; m1 < dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows.Count; m1++)
                        {
                            if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m1]["CompCode"].ToString().Trim() == "01" &&
                                 dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m1]["CatID"].ToString().Trim() == "029")
                            {
                                StrCNC_PCCode = "01.009"; //CNC
                                CatID = "029"; ///Canopy
                                CompCode = "01";

                                // CPY Without BaseFrame & fuel Tank
                                RequisitionForPartCode = ComCon.getTranName(
                                                                              "SELECT Partcode FROM BOMdetails " +
                                                                              "WHERE BOMCode='" + Dts[10].ToString().Trim() + "' " +
                                                                              //"AND KitCode='" + Dts[2].ToString().Trim() + "' " +
                                                                              "AND SUBSTRING(Partcode, 11, 1) IN ('4') " +
                                                                              "AND Partcode LIKE '004%'",
                                                                              "tblBP", "Partcode", con, tran);
                            }
                            else if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m1]["CompCode"].ToString().Trim() == "03" &&
                                 dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m1]["CatID"].ToString() == "084")

                            {
                                StrCNC_PCCode = "03.061"; ////CNC
                                CatID = "084";  //Base Frame
                                CompCode = "03";
                                //BaseFrame
                                RequisitionForPartCode = ComCon.getTranName(
                                                                             "SELECT Partcode FROM BOMdetails " +
                                                                             "WHERE BOMCode='" + Dts[10].ToString().Trim() + "' " +
                                                                             "AND SUBSTRING(Partcode, 12, 1) IN ('2') " +
                                                                             "AND SUBSTRING(kitcode, 11, 1) IN ('5') " +
                                                                             "AND KITCode LIKE '004%'",
                                                                             "tblBP", "Partcode", con, tran);
                            }

                            else if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m1]["CompCode"].ToString().Trim() == "03" &&

                             dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m1]["CatID"].ToString().Trim() == "038")
                            {
                                StrCNC_PCCode = "03.061"; ////CNC
                                CatID = "038"; //Fuel Tank
                                CompCode = "03";

                                RequisitionForPartCode = ComCon.getTranName(
                                                                           "SELECT Partcode FROM BOMdetails " +
                                                                           "WHERE BOMCode='" + Dts[10].ToString().Trim() + "' " +
                                                                           "AND SUBSTRING(Partcode, 12, 1) IN ('3') " +
                                                                           "AND SUBSTRING(kitcode, 11, 1) IN ('5') " +
                                                                           "AND KITCode LIKE '004%'",
                                                                           "tblBP", "Partcode", con, tran);
                            }
                            else if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m1]["CompCode"].ToString().Trim() == "28")
                            {
                                StrWH_PCCode = "28.013"; ////CNC
                                CatID = "";
                            }


                            #region  //For CNC Req Product Wise and Company Wise
                            StrDispCode_CNCReq = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m1]["CompCode"].ToString().Trim(), con, tran);


                            if (string.IsNullOrEmpty(StrDispCode_MaterialReq_CNC_ALL_msg.Trim()))
                            {
                                StrDispCode_MaterialReq_CNC_ALL_msg = StrDispCode_CNCReq.Trim();
                            }
                            else
                            {
                                StrDispCode_MaterialReq_CNC_ALL_msg += ", " + StrDispCode_CNCReq.Trim();
                            }


                            sb.Remove(0, sb.Length);
                            sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                            sb.Append("values('" + StrDispCode_CNCReq.Trim() + "','" + StrDispCode_CNCReq.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                            sb.Append("'" + StrDispCode_CNCReq.Substring(4, 5).Trim() + "','" + StrCNC_PCCode.Trim() + "','23.001','" + Dts[2].ToString().Trim() + "','" + dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m1]["CompCode"].ToString().Trim() + "','" + Dts[8].ToString().Trim() + "','P','WIP',");
                            sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + " ','1','1','1','" + StrDispCode_CPYPlan.Trim() + "','" + RequisitionForPartCode.Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //Details Req STRAT ********************



                            DataSet dsReqDts_CNCReq = ComCon.procTranDS(
                                "exec InternalReqLogisticsKit '" + Dts[2].ToString().Trim() + "' ,0 ,'" + dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m1]["CatID"].ToString().Trim() + "'", "tbl_ReqDts_CNCReq", con, tran);


                            // DataSet dsReqDts_CNCReq = ComCon.procTranDS("exec InternalReqLogisticsKit '" + Dts[2].ToString().Trim() + "' ,0 ,'" + CatID.ToString().Trim() + "'", "tbl_ReqDts_CNCReq", con, tran);
                            if (dsReqDts_CNCReq != null && dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows.Count > 0)
                            {
                                int SrNoReq_CNCReq = 0;

                                for (int cntd = 0; cntd < dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows.Count; cntd++)
                                {
                                    SrNoReq_CNCReq = SrNoReq_CNCReq + 1;
                                    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@REQCode", StrDispCode_CNCReq.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNoReq_CNCReq);
                                    cmd.Parameters.AddWithValue("@PartCode", dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows[cntd]["Partcode"].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(Dts[8].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@REQStatus", "P");
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    GetReqDetailsSub(StrDispCode_CNCReq.Trim(), dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows[cntd]["Partcode"].ToString().Trim(), 0, double.Parse(Dts[8].ToString().Trim()));
                                }
                            }

                            //Details Req  END * **************************

                            //}

                            #endregion
                        }


                    }




                    #endregion

                    //Auto CNC Req --END

                    //Auto FAB Req --START *****************************


                    //#region  old code
                    ////if (job_Cpyreq.PCCode.Trim() == "01.041")
                    ////{
                    ////    StrFAB_PCCode = "01.008"; //FAB
                    ////}
                    ////else if (job_Cpyreq.PCCode.Trim() == "03.064")
                    ////{
                    ////    StrFAB_PCCode = "03.002"; ////FAB
                    ////}
                    ////else if (job_Cpyreq.PCCode.Trim() == "28.012")
                    ////{
                    ////    StrWH_PCCode = "28.015"; ////CNC
                    ////}
                    //#endregion

                    #region FAB

                    string StrDispCode_FABReq = "";

                    String StrFAB_PCCode = "0";

                    //Added Rb

                    dsCanopyPlanDtsSub = ComCon.procTranDS("select CatID,CompCode from CanopyPlanDtsSub where CPCode='" + StrDispCode_CPYPlan.ToString().Trim() + "'  group by CatID, CompCode", "tbl_CanopyPlanDtsSub", con, tran);

                    if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows.Count > 0)
                    {

                        for (int m2 = 0; m2 < dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows.Count; m2++)
                        {
                            if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m2]["CompCode"].ToString().Trim() == "01" &&
                                 dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m2]["CatID"].ToString().Trim() == "029")
                            {
                                StrFAB_PCCode = "01.008"; //FAB
                                CatID = "029"; //Canopy
                                CompCode = "01";

                                // CPY Without BaseFrame & fuel Tank  
                                RequisitionForPartCode = ComCon.getTranName(
                                                                              "SELECT Partcode FROM BOMdetails " +
                                                                              "WHERE BOMCode='" + Dts[10].ToString().Trim() + "' " +
                                                                              "AND SUBSTRING(Partcode, 11, 1) IN ('4') " +
                                                                              "AND Partcode LIKE '004%'",
                                                                              "tblBP", "Partcode", con, tran);
                            }
                            else if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m2]["CompCode"].ToString().Trim() == "03" &&
                                 dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m2]["CatID"].ToString() == "084")

                            {
                                StrFAB_PCCode = "03.002"; ////FAB
                                CatID = "084"; //Base Frame
                                CompCode = "03";
                                //BaseFrame
                                RequisitionForPartCode = ComCon.getTranName(
                                                                             "SELECT Partcode FROM BOMdetails " +
                                                                             "WHERE BOMCode='" + Dts[10].ToString().Trim() + "' " +
                                                                             "AND SUBSTRING(Partcode, 12, 1) IN ('2') " +
                                                                             "AND SUBSTRING(kitcode, 11, 1) IN ('5') " +
                                                                             "AND KITCode LIKE '004%'",
                                                                             "tblBP", "Partcode", con, tran);
                            }

                            else if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m2]["CompCode"].ToString().Trim() == "03" &&

                             dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m2]["CatID"].ToString().Trim() == "038")
                            {
                                StrFAB_PCCode = "03.002"; ////FAB
                                CatID = "038"; //Fuel Tank
                                CompCode = "03";
                                //fuel Tank
                                RequisitionForPartCode = ComCon.getTranName(
                                                                           "SELECT Partcode FROM BOMdetails " +
                                                                           "WHERE BOMCode='" + Dts[10].ToString().Trim() + "' " +
                                                                           "AND SUBSTRING(Partcode, 12, 1) IN ('3') " +
                                                                           "AND SUBSTRING(kitcode, 11, 1) IN ('5') " +
                                                                           "AND KITCode LIKE '004%'",
                                                                           "tblBP", "Partcode", con, tran);
                            }
                            else if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m2]["CompCode"].ToString().Trim() == "28")
                            {
                                StrWH_PCCode = "28.015";
                                CatID = "";
                                CompCode = "28";
                            }

                            #region  For FAB Req Product Wise and Company Wise

                            StrDispCode_FABReq = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m2]["CompCode"].ToString().Trim(), con, tran);


                            if (string.IsNullOrEmpty(StrDispCode_MaterialReq_FAB_ALL_msg.Trim()))
                            {
                                StrDispCode_MaterialReq_FAB_ALL_msg = StrDispCode_FABReq.Trim();
                            }
                            else
                            {
                                StrDispCode_MaterialReq_FAB_ALL_msg += ", " + StrDispCode_FABReq.Trim();
                            }


                            sb.Remove(0, sb.Length);
                            sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                            sb.Append("values('" + StrDispCode_FABReq.Trim() + "','" + StrDispCode_FABReq.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                            sb.Append("'" + StrDispCode_FABReq.Substring(4, 5).Trim() + "','" + StrFAB_PCCode.Trim() + "','23.001','" + Dts[2].ToString().Trim() + "','" + dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m2]["CompCode"].ToString().Trim() + "','" + Dts[8].ToString().Trim() + "','P','WIP',");
                            sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + " ','1','1','1','" + StrDispCode_CPYPlan.Trim() + "','" + RequisitionForPartCode.Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //Details Req STRAT ********************


                            DataSet dsReqDts_FABReq = ComCon.procTranDS("exec InternalReqLogisticsKit '" + Dts[2].ToString().Trim() + "',1 ,'" + dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m2]["CatID"].ToString() + "'", "tbl_ReqDts_FABReq", con, tran);
                            if (dsReqDts_FABReq != null && dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows.Count > 0)
                            {
                                int SrNoReq_FABReq = 0;

                                for (int cnt_FAB = 0; cnt_FAB < dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows.Count; cnt_FAB++)
                                {
                                    SrNoReq_FABReq = SrNoReq_FABReq + 1;
                                    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@REQCode", StrDispCode_FABReq.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNoReq_FABReq);
                                    cmd.Parameters.AddWithValue("@PartCode", dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows[cnt_FAB]["Partcode"].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows[cnt_FAB]["RaiseReqQty"].ToString().Trim()) * double.Parse(Dts[8].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@REQStatus", "P");
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    GetReqDetailsSub(StrDispCode_FABReq.Trim(), dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows[cnt_FAB]["Partcode"].ToString().Trim(), 1, double.Parse(Dts[8].ToString().Trim()));
                                }
                            }

                            #endregion
                        }

                    }




                    //StrDispCode_FABReq = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CompCode.Trim(), con, tran);


                    //if (string.IsNullOrEmpty(StrDispCode_MaterialReq_FAB_ALL_msg.Trim()))
                    //{
                    //    StrDispCode_MaterialReq_FAB_ALL_msg = StrDispCode_FABReq.Trim();
                    //}
                    //else
                    //{
                    //    StrDispCode_MaterialReq_FAB_ALL_msg += ", " + StrDispCode_FABReq.Trim();
                    //}


                    //sb.Remove(0, sb.Length);
                    //sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                    //    " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                    //sb.Append("values('" + StrDispCode_FABReq.Trim() + "','" + StrDispCode_FABReq.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    //sb.Append("'" + StrDispCode_FABReq.Substring(4, 5).Trim() + "','" + StrFAB_PCCode.Trim() + "','23.001','" + Dts[2].ToString().Trim() + "','" + CompCode.Trim() + "','" + Dts[8].ToString().Trim() + "','P','WIP',");
                    //sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + " ','1','1','1','" + StrDispCode_CPYPlan.Trim() + "','" + RequisitionForPartCode.Trim() + "')");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    ////Details Req STRAT ********************


                    //DataSet dsReqDts_FABReq = ComCon.procTranDS("exec InternalReqLogisticsKit '" + Dts[2].ToString().Trim() + "',1 ,'" + CatID.ToString().Trim() + "'", "tbl_ReqDts_FABReq", con, tran);
                    //if (dsReqDts_FABReq != null && dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows.Count > 0)
                    //{
                    //    int SrNoReq_FABReq = 0;

                    //    for (int cnt_FAB = 0; cnt_FAB < dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows.Count; cnt_FAB++)
                    //    {
                    //        SrNoReq_FABReq = SrNoReq_FABReq + 1;
                    //        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                    //        cmd.CommandType = CommandType.StoredProcedure;
                    //        cmd.Parameters.AddWithValue("@REQCode", StrDispCode_FABReq.Trim());
                    //        cmd.Parameters.AddWithValue("@SrNo", SrNoReq_FABReq);
                    //        cmd.Parameters.AddWithValue("@PartCode", dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows[cnt_FAB]["Partcode"].ToString().Trim());
                    //        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows[cnt_FAB]["RaiseReqQty"].ToString().Trim()) * double.Parse(Dts[8].ToString().Trim()));
                    //        cmd.Parameters.AddWithValue("@REQStatus", "P");
                    //        cmd.Transaction = tran;
                    //        cmd.ExecuteNonQuery();
                    //        cmd.Dispose();

                    //        GetReqDetailsSub(StrDispCode_FABReq.Trim(), dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows[cnt_FAB]["Partcode"].ToString().Trim(), 1, double.Parse(Dts[8].ToString().Trim()));
                    //    }
                    //}

                    //Details Req  END ***************************

                    //}

                    #endregion

                    //Auto FAB Req --END


                    //Auto U1 Powder C to U- FBB Req --START *****************************
                    #region Powder Coting

                    String StrPC_PCCode = "0";

                    //if (job_Cpyreq.CompCode.Trim() == "03")
                    //{

                    //Added Rohan 

                    dsCanopyPlanDtsSub = ComCon.procTranDS("select CatID,CompCode from CanopyPlanDtsSub where CPCode='" + StrDispCode_CPYPlan.ToString().Trim() + "' and CompCode='03'   group by CatID, CompCode", "tbl_CanopyPlanDtsSub", con, tran);

                    if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows.Count > 0)
                    {

                        for (int m3 = 0; m3 < dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows.Count; m3++)
                        {
                            if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CompCode"].ToString().Trim() == "03" &&
                                 dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CatID"].ToString() == "084")

                            {
                                StrPC_PCCode = "01.007"; ////PC
                                CatID = "084"; //Base Frame

                                //BaseFrame
                                RequisitionForPartCode = ComCon.getTranName(
                                                                             "SELECT Partcode FROM BOMdetails " +
                                                                             "WHERE BOMCode='" + Dts[10].ToString().Trim() + "' " +
                                                                             "AND SUBSTRING(Partcode, 12, 1) IN ('2') " +
                                                                             "AND SUBSTRING(kitcode, 11, 1) IN ('5') " +
                                                                             "AND KITCode LIKE '004%'",
                                                                             "tblBP", "Partcode", con, tran);
                            }

                            else if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CompCode"].ToString().Trim() == "03" &&

                             dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CatID"].ToString().Trim() == "038")
                            {
                                StrPC_PCCode = "01.007"; ////PC
                                CatID = "038"; //Fuel Tank

                                //fuel Tank
                                RequisitionForPartCode = ComCon.getTranName(
                                                                           "SELECT Partcode FROM BOMdetails " +
                                                                           "WHERE BOMCode='" + Dts[10].ToString().Trim() + "' " +
                                                                           "AND SUBSTRING(Partcode, 12, 1) IN ('3') " +
                                                                           "AND SUBSTRING(kitcode, 11, 1) IN ('5') " +
                                                                           "AND KITCode LIKE '004%'",
                                                                           "tblBP", "Partcode", con, tran);
                            }

                            string StrDispCode_PCReq = "";

                            StrDispCode_PCReq = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", "01", con, tran);

                            if (string.IsNullOrEmpty(StrDispCode_MaterialReq_POC_ALL_msg.Trim()))
                            {
                                StrDispCode_MaterialReq_POC_ALL_msg = StrDispCode_PCReq.Trim();
                            }
                            else
                            {
                                StrDispCode_MaterialReq_POC_ALL_msg += ", " + StrDispCode_PCReq.Trim();
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                            sb.Append("values('" + StrDispCode_PCReq.Trim() + "','" + StrDispCode_PCReq.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                            sb.Append("'" + StrDispCode_PCReq.Substring(4, 5).Trim() + "','01.007','03.002','" + Dts[2].ToString().Trim() + "','01','" + Dts[8].ToString().Trim() + "','P','WIP',");
                            sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + "','1','1','1','" + StrDispCode_CPYPlan.Trim() + "','" + RequisitionForPartCode.Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //Details Req STRAT ********************
                             DataSet dsReqDts_PCReq;
                            if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CompCode"].ToString().Trim() == "03" &&
                                 dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CatID"].ToString() == "084")
                            {
                                dsReqDts_PCReq = ComCon.procTranDS("Select CPCode,CpyPartCode,SrNo,PartCode,CPQty as RaiseReqQty,Rate  from CanopyPlanDtsSub  where CPCode='" + StrDispCode_CPYPlan.Trim() + "' and CpyPartCode ='" + Dts[2].ToString().Trim() + "' and CatID='" + dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CatID"].ToString().Trim() + "' " +
                                      " Union ALL Select CPCode,CpyPartCode,SrNo,PartCode,CPQty as RaiseReqQty ,Rate from CanopyPlanDtsSubBelowStdRate  where CPCode='" + StrDispCode_CPYPlan.Trim() + "' and CpyPartCode ='" + Dts[2].ToString().Trim() + "' and CatID='" + dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CatID"].ToString().Trim() + "' Order by Srno ", "tbl_ReqDts_PCReq", con, tran);
       


                                if (dsReqDts_PCReq != null && dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count > 0)
                                //if ( dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count > 0)
                                {
                                    int SrNoReq_PCReq = 0;

                                    for (int cnt_PC = 0; cnt_PC < dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count; cnt_PC++)
                                    {
                                        SrNoReq_PCReq = SrNoReq_PCReq + 1;
                                        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@REQCode", StrDispCode_PCReq.Trim());
                                        cmd.Parameters.AddWithValue("@SrNo", SrNoReq_PCReq);
                                        cmd.Parameters.AddWithValue("@PartCode", dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows[cnt_PC]["Partcode"].ToString().Trim());
                                        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows[cnt_PC]["RaiseReqQty"].ToString().Trim()));
                                        cmd.Parameters.AddWithValue("@REQStatus", "P");
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();


                                    }
                                }
                            }
                            else if (dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CompCode"].ToString().Trim() == "03" &&

                             dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CatID"].ToString().Trim() == "038")
                            {
                                dsReqDts_PCReq = ComCon.procTranDS("Select CPCode,CpyPartCode,SrNo,PartCode,CPQty as RaiseReqQty,Rate  from CanopyPlanDtsSub  where CPCode='" + StrDispCode_CPYPlan.Trim() + "' and CpyPartCode ='" + Dts[2].ToString().Trim() + "' and CatID='" + dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CatID"].ToString().Trim() + "' " +
                                    " Union ALL Select CPCode,CpyPartCode,SrNo,PartCode,CPQty as RaiseReqQty ,Rate from CanopyPlanDtsSubBelowStdRate  where CPCode='" + StrDispCode_CPYPlan.Trim() + "' and CpyPartCode ='" + Dts[2].ToString().Trim() + "' and CatID='" + dsCanopyPlanDtsSub.Tables["tbl_CanopyPlanDtsSub"].Rows[m3]["CatID"].ToString().Trim() + "' Order by Srno ", "tbl_ReqDts_PCReq", con, tran);


                                if (dsReqDts_PCReq != null && dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count > 0)
                                //if ( dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count > 0)
                                {
                                    int SrNoReq_PCReq = 0;

                                    for (int cnt_PC = 0; cnt_PC < dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count; cnt_PC++)
                                    {
                                        SrNoReq_PCReq = SrNoReq_PCReq + 1;
                                        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@REQCode", StrDispCode_PCReq.Trim());
                                        cmd.Parameters.AddWithValue("@SrNo", SrNoReq_PCReq);
                                        cmd.Parameters.AddWithValue("@PartCode", dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows[cnt_PC]["Partcode"].ToString().Trim());
                                        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows[cnt_PC]["RaiseReqQty"].ToString().Trim()));
                                        cmd.Parameters.AddWithValue("@REQStatus", "P");
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();


                                    }
                                }


                            }





                           // DataSet dsReqDts_PCReq;
                            //dsReqDts_PCReq = ComCon.procTranDS("Select CPCode,CpyPartCode,SrNo,PartCode,CPQty as RaiseReqQty,Rate  from CanopyPlanDtsSub  where CPCode='" + StrDispCode_CPYPlan.Trim() + "' and CpyPartCode ='" + Dts[2].ToString().Trim() + "' " +
                            //    " Union ALL Select CPCode,CpyPartCode,SrNo,PartCode,CPQty as RaiseReqQty ,Rate from CanopyPlanDtsSubBelowStdRate  where CPCode='" + StrDispCode_CPYPlan.Trim() + "' and CpyPartCode ='" + Dts[2].ToString().Trim() + "' Order by Srno ", "tbl_ReqDts_PCReq", con, tran);


                            //if (dsReqDts_PCReq != null && dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count > 0)
                            ////if ( dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count > 0)
                            //{
                            //    int SrNoReq_PCReq = 0;

                            //    for (int cnt_PC = 0; cnt_PC < dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count; cnt_PC++)
                            //    {
                            //        SrNoReq_PCReq = SrNoReq_PCReq + 1;
                            //        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                            //        cmd.CommandType = CommandType.StoredProcedure;
                            //        cmd.Parameters.AddWithValue("@REQCode", StrDispCode_PCReq.Trim());
                            //        cmd.Parameters.AddWithValue("@SrNo", SrNoReq_PCReq);
                            //        cmd.Parameters.AddWithValue("@PartCode", dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows[cnt_PC]["Partcode"].ToString().Trim());
                            //        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows[cnt_PC]["RaiseReqQty"].ToString().Trim()));
                            //        cmd.Parameters.AddWithValue("@REQStatus", "P");
                            //        cmd.Transaction = tran;
                            //        cmd.ExecuteNonQuery();
                            //        cmd.Dispose();


                            //    }
                            //}



                        }

                    }




                    //string StrDispCode_PCReq = "";

                    //StrDispCode_PCReq = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", "01", con, tran);

                    //if (string.IsNullOrEmpty(StrDispCode_MaterialReq_POC_ALL_msg.Trim()))
                    //{
                    //    StrDispCode_MaterialReq_POC_ALL_msg = StrDispCode_PCReq.Trim();
                    //}
                    //else
                    //{
                    //    StrDispCode_MaterialReq_POC_ALL_msg += ", " + StrDispCode_PCReq.Trim();
                    //}

                    //sb.Remove(0, sb.Length);
                    //sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                    //    " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                    //sb.Append("values('" + StrDispCode_PCReq.Trim() + "','" + StrDispCode_PCReq.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    //sb.Append("'" + StrDispCode_PCReq.Substring(4, 5).Trim() + "','01.007','03.002','" + Dts[2].ToString().Trim() + "','01','" + Dts[8].ToString().Trim() + "','P','WIP',");
                    //sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + "','1','1','1','" + StrDispCode_CPYPlan.Trim() + "','0')");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    ////Details Req STRAT ********************


                    //DataSet dsReqDts_PCReq = ComCon.procTranDS("Select CPCode,CpyPartCode,SrNo,PartCode,CPQty as RaiseReqQty,Rate  from CanopyPlanDtsSub  where CPCode='" + StrDispCode_CPYPlan.Trim() + "' and CpyPartCode ='" + Dts[2].ToString().Trim() + "' " +
                    //    " Union ALL Select CPCode,CpyPartCode,SrNo,PartCode,CPQty as RaiseReqQty ,Rate from CanopyPlanDtsSubBelowStdRate  where CPCode='" + StrDispCode_CPYPlan.Trim() + "' and CpyPartCode ='" + Dts[2].ToString().Trim() + "' Order by Srno ", "tbl_ReqDts_PCReq", con, tran);
                    //if (dsReqDts_PCReq != null && dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count > 0)
                    //{
                    //    int SrNoReq_PCReq = 0;

                    //    for (int cnt_PC = 0; cnt_PC < dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count; cnt_PC++)
                    //    {
                    //        SrNoReq_PCReq = SrNoReq_PCReq + 1;
                    //        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                    //        cmd.CommandType = CommandType.StoredProcedure;
                    //        cmd.Parameters.AddWithValue("@REQCode", StrDispCode_PCReq.Trim());
                    //        cmd.Parameters.AddWithValue("@SrNo", SrNoReq_PCReq);
                    //        cmd.Parameters.AddWithValue("@PartCode", dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows[cnt_PC]["Partcode"].ToString().Trim());
                    //        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows[cnt_PC]["RaiseReqQty"].ToString().Trim()));
                    //        cmd.Parameters.AddWithValue("@REQStatus", "P");
                    //        cmd.Transaction = tran;
                    //        cmd.ExecuteNonQuery();
                    //        cmd.Dispose();


                    //    }
                    //}
                    //}

                    //    //Details Req  END ***************************
                    //}

                    #endregion
                    //Auto FAB Req --END

                    #endregion
                    //All Requisition End

                } // Main For Loop END Bracket 


                //****************User Acivity****************
                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@EmpID", job_Cpyreq.EmpCode.Trim());
                cmd.Parameters.AddWithValue("@TransactionType", "S");
                cmd.Parameters.AddWithValue("@TransactionFrom", "CpyPlan(Manual)");
                cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode_CPYPlan.Trim());
                cmd.Parameters.AddWithValue("@CompanyCode", job_Cpyreq.CompCode.Trim());
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                tran.Commit();
                //tran.Rollback();

                //StrDisplayMsg = "";
                //
                if (!string.IsNullOrEmpty(StrDispCode_CPYPlan.Trim()))
                {
                    StrDisplayMsg = "Saved Successfully With Canopy Plan: " + StrDispCode_CPYPlan.Trim() + "";
                }

                if (!string.IsNullOrEmpty(StrDispCode_MaterialReq_CNC_ALL_msg.Trim()))
                {
                    StrDisplayMsg += " & CNC Req: " + StrDispCode_MaterialReq_CNC_ALL_msg.Trim() + "";
                }

                if (!string.IsNullOrEmpty(StrDispCode_MaterialReq_FAB_ALL_msg.Trim()))
                {
                    StrDisplayMsg += " & Fab Req: " + StrDispCode_MaterialReq_FAB_ALL_msg.Trim() + "";
                }

                if (!string.IsNullOrEmpty(StrDispCode_MaterialReq_POC_ALL_msg.Trim()))
                {
                    StrDisplayMsg += " & PC_Unit_1 to Fab_U4 : " + StrDispCode_MaterialReq_POC_ALL_msg.Trim() + "";
                }

                return StrDisplayMsg;

                #endregion
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace " + ex.StackTrace.ToString() + ", Message " + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }

        protected void GetReqDetailsSub(string ReqCode, string strKitCode, int strPCwise, double KitQty)
        {
            DataSet dsReqDtsSub = ComCon.procTranDS("exec InternalReqLogisticsdetails '" + strKitCode + "'," + strPCwise + "", "tbl_RaiseReqDtsSub", con, tran);
            if (dsReqDtsSub != null && dsReqDtsSub.Tables["tbl_RaiseReqDtsSub"].Rows.Count > 0)
            {
                int SrNok = 0;
                for (int k = 0; k < dsReqDtsSub.Tables["tbl_RaiseReqDtsSub"].Rows.Count; k++)
                {
                    if (Convert.ToDouble(dsReqDtsSub.Tables["tbl_RaiseReqDtsSub"].Rows[k]["RaiseReqQty"].ToString().Trim()) > 0)
                    {
                        SrNok += 1;
                    }
                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into MaterialReqDetailsSub(REQCode,SrNo,RKitCode,PartCode,Qty,REQStatus) ");
                    sb.Append("values('" + ReqCode.Trim() + "','" + SrNok + "','" + strKitCode + "',");
                    sb.Append("'" + dsReqDtsSub.Tables["tbl_RaiseReqDtsSub"].Rows[k]["PartCode"].ToString().Trim() + "',");
                    sb.Append("'" + double.Parse(dsReqDtsSub.Tables["tbl_RaiseReqDtsSub"].Rows[k]["RaiseReqQty"].ToString().Trim()) * KitQty + "','P')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
        }
    }
}