using KalaERPApi.Models.Production.Canopy.Trans;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace KalaERPApi.Service.Production.Canopy.Trans
{
    public class CpyPrcCon
    {
        #region
        CommonCon ComCon = new CommonCon();
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        Boolean ChkforStart = false; Boolean ChkforStartCPY = false;
        public SqlTransaction tran = null;
        string strReqCode = "", strReqCodeCPYAssly = "", strKanBan = "";
        DataSet dsReqMstCPYAssly = new DataSet();
        DataSet dsKitbelowRate = new DataSet();
        DataSet dsKitbelowRatedts = new DataSet();
        DataSet dsReqDtsCPYAssly = new DataSet();
        DataSet dsKanBan = new DataSet();
        DataSet dsCNCCons = new DataSet();
        DataSet dsReqMst = new DataSet();
        DataSet dsReqDts = new DataSet();
        DataSet dsReqDtsSub = new DataSet();
        DataSet dsDetailsSub = new DataSet();
        DataSet dsCPYSerialNo = new DataSet();
        DataSet dsDetailsSub1 = new DataSet();
        DataSet dsDetailsSubV = new DataSet();
        DataSet dsChkforStart = new DataSet();
        DataSet dsckhDoubleEntry = new DataSet();
        string[] strPlanDts;
        int SrNoA;
        string[] DtsA;
        #endregion

        public object SqlDbTypeChar { get; private set; }
        //,string Product
        public DataTable getCpyPrcddl(string PCCode, string MachineCode, string KVA, string Model, string PlanCode, string CatID)
        {
            string[] MachineDts = Regex.Split(MachineCode.ToString().Trim(), "-->");
            //if (PCCode == "01.009" && KVA == "0" &&  Model == "0")
            if (KVA == "0" && Model == "0" && PlanCode == "0" && CatID == "0")
            {
                //if (Product == "CPY")
                //{
                if (PCCode == "01.005")//Canopy Assembly U1
                {
                    dsDetailsSub = ComCon.procDS("select P.KVA,P.KVA as KVA1 from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "' and P.kva<'82.5' and " +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "' and Edt is null and Pf.Active='1' and ProductCode like '401%' and Pf.Dt>='2020-07-10 00:00:00' Group By P.KVA", "tbl_processfeedback");

                }
                else if (PCCode == "03.038") //Canopy Assembly U4
                {
                    dsDetailsSub = ComCon.procDS("select P.KVA,P.KVA as KVA1 from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "' and P.kva>='82.5' and" +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "' and Edt is null and Pf.Active='1' and ProductCode like '401%' and Pf.Dt>='2020-07-10 00:00:00' Group By P.KVA", "tbl_processfeedback");

                }
                else if (PCCode == "28.017") //Canopy Assembly Banglore
                {
                    dsDetailsSub = ComCon.procDS("select P.KVA,P.KVA as KVA1 from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and" +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "' and Edt is null and Pf.Active='1' and ProductCode like '401%' and Pf.Dt>='2020-07-10 00:00:00' Group By P.KVA", "tbl_processfeedback");

                }
                else
                {
                    dsDetailsSub = ComCon.procDS("select P.KVA,P.KVA as KVA1 from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "' and " +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "' and Edt is null and Pf.Active='1' and ProductCode like '401%' and Pf.Dt>='2020-07-10 00:00:00' Group By P.KVA", "tbl_processfeedback");

                }
                //}
                //else if (Product == "CPL")
                //{
                //    dsDetailsSub = ComCon.procDS("select P.KVA,P.KVA as KVA1 from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "' and " +
                //    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and Pf.Active='1'  and ProductCode like '003%' and Pf.Dt>='2020-07-10 00:00:00' Group By P.KVA", "tbl_processfeedback");
                //}

                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("GetddlCpyPrc", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@KVA", SqlDbType.Char).Value = KVA;
                    dAd.SelectCommand.Parameters.Add("@Model", SqlDbType.Char).Value = Model;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@CatID", SqlDbType.Char).Value = CatID;
                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);

                    return dsDetailsSub.Tables[0];
                }
            }
            //else if (PCCode == "01.009" && KVA != "0" && Model == "0")
            else if (KVA != "0" && Model == "0" && PlanCode == "0" && CatID == "0")
            {
                if (PCCode == "01.005") //Canopy Assembly U1
                {
                    dsDetailsSub = ComCon.procDS("select P.Model,P.Model as Model1 from processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and P.kva<'82.5' and " +
                " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and P.KVA='" + KVA.Trim() + "'  and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' Group By P.Model", "tbl_processfeedback");
                }
                else if (PCCode == "03.038") //Canopy Assembly U4
                {
                    dsDetailsSub = ComCon.procDS("select P.Model,P.Model as Model1 from processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and P.kva>='82.5' and " +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and P.KVA='" + KVA.Trim() + "'  and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' Group By P.Model", "tbl_processfeedback");

                }
                else if (PCCode == "28.017") //Canopy Assembly Banglore 
                {
                    dsDetailsSub = ComCon.procDS("select P.Model,P.Model as Model1 from processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and " +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and P.KVA='" + KVA.Trim() + "'  and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' Group By P.Model", "tbl_processfeedback");

                }
                else
                {
                    dsDetailsSub = ComCon.procDS("select P.Model,P.Model as Model1 from processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'   and " +
                " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and P.KVA='" + KVA.Trim() + "'  and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' Group By P.Model", "tbl_processfeedback");
                }
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("GetddlCpyPrc", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@KVA", SqlDbType.Char).Value = KVA;
                    dAd.SelectCommand.Parameters.Add("@Model", SqlDbType.Char).Value = Model;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@CatID", SqlDbType.Char).Value = CatID;
                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);

                    return dsDetailsSub.Tables[0];

                }
            }
            //else if (PCCode == "01.009" && KVA != "0" && Model != "0")
            else if (KVA != "0" && Model != "0" && PlanCode == "0" && CatID == "0")
            {
                if (PCCode.Trim() == "01.005" || PCCode.Trim() == "03.038" || PCCode.Trim() == "28.017") //Canopy Assembly U1 and U4
                {
                    //dsDetailsSub = ComCon.procDS(" select Top 1 Convert(varchar(10),P.KVA)+'-->'+P.Model as KVAMod,KVA,Model,CanopyPlanCode as CPCode,PF.Dt,PF.ProductCode as Partcode,Partdesc+'-->'+PF.Partcode as  Part,cd.Qty as CPQty,(cd.Qty-CpyWipQty) as PlanQtyBal,ProcessQty as PrcQty,isnull(PFBCode,0) as PFBCode,EDt,TurretKitCode as BOMCode,SupplierCode as SCode  from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode " +
                    //     "Inner join CanopyPlanDetails Cd On Pf.CanopyPlanCode=Cd.CPCode " +
                    //     " where ProfitcenterCode='" + PCCode.Trim() + "'  and MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null " +
                    // "   and P.KVA='" + KVA.Trim() + "'  and P.Model='" + Model.Trim() + "'  and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' order By Dt desc ", "tbl_processfeedback");

                    dsDetailsSub = ComCon.procDS("select Top 1 Convert(varchar(10),P.KVA)+'-->'+P.Model as KVAMod,KVA,Model,CanopyPlanCode as CPCode,PF.Dt,PF.ProductCode as Partcode,Partdesc+'-->'+PF.Partcode as  Part," +
                      "ProcessQty as CPQty,(ProcessQty-(select Count(PFBCode) From ProcessFeedbackDetailsSub where PFbCode=pf.PFBCode and EdtD is Not Null)) as PlanQtyBal," +
                      "(ProcessQty-(select Count(PFBCode) From ProcessFeedbackDetailsSub where PFbCode=pf.PFBCode and Edt is Not Null)) as PrcQty," +
                      "isnull(PFBCode,0) as PFBCode,EDt,TurretKitCode as BOMCode,SupplierCode as SCode from processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode " +
                      //"Inner join CanopyPlanDetails Cd On Pf.CanopyPlanCode=Cd.CPCode and PF.ProductCode=CD.Partcode " +BOM
                      " where ProfitcenterCode='" + PCCode.Trim() + "'  and MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null " +
                  "   and P.KVA='" + KVA.Trim() + "'  and P.Model='" + Model.Trim() + "' and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' order By Dt desc ", "tbl_processfeedback");
                }
                else
                {
                    dsDetailsSub = ComCon.procDS(" select Top 1 Convert(varchar(10),P.KVA)+'-->'+P.Model as KVAMod,KVA,Model,CanopyPlanCode as CPCode,PF.Dt,PF.ProductCode as Partcode,Partdesc+'-->'+PF.Partcode as  Part,ProcessQty as CPQty,isnull(PFBCode,0) as PFBCode,EDt,TurretKitCode as Code,SupplierCode as SCode  from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and " +
                  " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and P.KVA='" + KVA.Trim() + "'  and P.Model='" + Model.Trim() + "'  and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' order By Dt desc ", "tbl_processfeedback");
                }
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("GetddlCpyPrc", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@KVA", SqlDbType.Char).Value = KVA;
                    dAd.SelectCommand.Parameters.Add("@Model ", SqlDbType.Char).Value = Model;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@CatID", SqlDbType.Char).Value = CatID;
                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);

                    return dsDetailsSub.Tables[0];
                }
            }
            else if (KVA != "0" && Model != "0" && PlanCode != "0" && CatID == "0")
            {

                if (PCCode.Trim() == "01.005" || PCCode.Trim() == "03.038" || PCCode.Trim() == "28.017") //Canopy Assembly U1 and U4
                {
                    //  //dsDetailsSub = ComCon.procDS(" select Top 1 Convert(varchar(10),P.KVA)+'-->'+P.Model as KVAMod,KVA,Model,CanopyPlanCode as CPCode,PF.Dt,PF.ProductCode as Partcode,Partdesc+'-->'+PF.Partcode as  Part,cd.Qty as CPQty,(cd.Qty-CpyWipQty) as PlanQtyBal,ProcessQty as PrcQty,isnull(PFBCode,0) as PFBCode,EDt,TurretKitCode as BOMCode,SupplierCode as SCode  from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode " +
                    //  //     "Inner join CanopyPlanDetails Cd On Pf.CanopyPlanCode=Cd.CPCode " +
                    //  //     " where ProfitcenterCode='" + PCCode.Trim() + "'  and MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null " +
                    //  // "   and P.KVA='" + KVA.Trim() + "'  and P.Model='" + Model.Trim() + "'  and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' order By Dt desc ", "tbl_processfeedback");

                    //  dsDetailsSub = ComCon.procDS("select Top 1 Convert(varchar(10),P.KVA)+'-->'+P.Model as KVAMod,KVA,Model,CanopyPlanCode as CPCode,PF.Dt,PF.ProductCode as Partcode,Partdesc+'-->'+PF.Partcode as  Part," +
                    //    "ProcessQty as CPQty,(ProcessQty-(select Count(PFBCode) From ProcessFeedbackDetailsSub where PFbCode=pf.PFBCode and EdtD is Not Null)) as PlanQtyBal," +
                    //    "(ProcessQty-(select Count(PFBCode) From ProcessFeedbackDetailsSub where PFbCode=pf.PFBCode and Edt is Not Null)) as PrcQty," +
                    //    "isnull(PFBCode,0) as PFBCode,EDt,TurretKitCode as BOMCode,SupplierCode as SCode from processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode " +
                    //    //"Inner join CanopyPlanDetails Cd On Pf.CanopyPlanCode=Cd.CPCode and PF.ProductCode=CD.Partcode " +BOM
                    //    " where ProfitcenterCode='" + PCCode.Trim() + "'  and MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null " +
                    //"   and P.KVA='" + KVA.Trim() + "'  and P.Model='" + Model.Trim() + "' and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' order By Dt desc ", "tbl_processfeedback");
                }
                else
                {
                    dsDetailsSub = ComCon.procDS("select Pf.CatID,ct.CatagoryName  from processfeedback pf " +
                         "inner join Catagory ct on pf.CatID=ct.CatagoryCode " +
                        "Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'   and " +
                " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and P.KVA='" + KVA.Trim() + "'  and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' Group By Pf.CatID,ct.CatagoryName ", "tbl_processfeedback");
                }
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("GetddlCpyPrc", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@KVA", SqlDbType.Char).Value = KVA;
                    dAd.SelectCommand.Parameters.Add("@Model ", SqlDbType.Char).Value = Model;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@CatID", SqlDbType.Char).Value = CatID[0];
                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);

                    return dsDetailsSub.Tables[0];
                }

            }

            return dsDetailsSub.Tables[0];
        }
        public DataTable getCpyPrcddlFab(string PCCode, string MachineCode, string KVA, string Model, string SuppCode)

        {
            string[] MachineDts = Regex.Split(MachineCode.ToString().Trim(), "-->");

            if (KVA == "0" && Model == "0")
            {

                dsDetailsSub = ComCon.procDS("select P.KVA,P.KVA as KVA1 from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "' and " +
            " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and Pf.Active='1'  and SupplierCode='" + SuppCode.Trim() + "' and ProductCode like '401%' and Pf.Dt>='2020-07-10 00:00:00' Group By P.KVA", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("GetddlCpyPrcFab", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@KVA", SqlDbType.Char).Value = KVA;
                    dAd.SelectCommand.Parameters.Add("@Model", SqlDbType.Char).Value = Model;
                    dAd.SelectCommand.Parameters.Add("@SuppCode", SqlDbType.Char).Value = SuppCode;
                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);

                    return dsDetailsSub.Tables[0];
                }

            }
            //else if (PCCode == "01.009" && KVA != "0" && Model == "0")
            else if (KVA != "0" && Model == "0")
            {
                dsDetailsSub = ComCon.procDS("select P.Model,P.Model as Model1 from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and " +
                " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null  and P.KVA='" + KVA.Trim() + "'  and SupplierCode='" + SuppCode.Trim() + "'  and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' Group By P.Model", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("GetddlCpyPrcFab", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@KVA", SqlDbType.Char).Value = KVA;
                    dAd.SelectCommand.Parameters.Add("@Model", SqlDbType.Char).Value = Model;
                    dAd.SelectCommand.Parameters.Add("@SuppCode", SqlDbType.Char).Value = SuppCode;
                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);

                    return dsDetailsSub.Tables[0];

                }
            }
            //else if (PCCode == "01.009" && KVA != "0" && Model != "0")
            else if (KVA != "0" && Model != "0")
            {


                dsDetailsSub = ComCon.procDS(" select Top 1 Convert(varchar(10),P.KVA)+'-->'+P.Model as KVAMod,KVA,Model,CanopyPlanCode as CPCode,PF.Dt,PF.ProductCode as Partcode,Partdesc+'-->'+PF.Partcode as  Part,ProcessQty as CPQty,isnull(PFBCode,0) as PFBCode,EDt,TurretKitCode as BOMCode,SupplierCode as SCode  from  processfeedback pf Inner Join Part P On Pf.ProductCode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and " +
              " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null  and P.KVA='" + KVA.Trim() + "'  and P.Model='" + Model.Trim() + "'  and SupplierCode='" + SuppCode.Trim() + "' and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' order By Dt desc ", "tbl_processfeedback");

                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("GetddlCpyPrcFab", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@KVA", SqlDbType.Char).Value = KVA;
                    dAd.SelectCommand.Parameters.Add("@Model", SqlDbType.Char).Value = Model;
                    dAd.SelectCommand.Parameters.Add("@SuppCode", SqlDbType.Char).Value = SuppCode;
                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);

                    return dsDetailsSub.Tables[0];
                }
            }

            return dsDetailsSub.Tables[0];
        }
        public DataTable GetCpyKitFab(string PCCode, string MachineCode, string PlanCode, string Partcode, string CpyKit, string SuppCode)
        {
            string[] MachineDts = Regex.Split(MachineCode.ToString().Trim(), "-->");

            if (CpyKit == "0")
            {

                dsDetailsSub = ComCon.procDS(" select  AliseName as KitDesc,Pf.Partcode+'-->'+PartDesc as KitCode,PfbCode,EDt  from  processfeedback pf Inner Join Part P On Pf.partcode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and " +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null  and CanopyPlanCode='" + PlanCode + "'  and Productcode='" + Partcode + "' and SupplierCode='" + SuppCode.Trim() + "'  and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' order By Pf.Dt desc ", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {

                    SqlDataAdapter dAd = new SqlDataAdapter("GetCpyKitFab", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;
                    dAd.SelectCommand.Parameters.Add("@CpyKit", SqlDbType.Char).Value = CpyKit;
                    dAd.SelectCommand.Parameters.Add("@SuppCode", SqlDbType.Char).Value = SuppCode;

                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);
                    return dsDetailsSub.Tables[0];
                }

            }
            if (CpyKit != "0")
            {
                dsDetailsSub = ComCon.procDS(" select isnull(ProcessQty,0) as Bal  from  processfeedback pf Inner Join Part P On Pf.partcode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and " +
                   " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null  and CanopyPlanCode='" + PlanCode + "'  and Productcode='" + Partcode + "' and SupplierCode='" + SuppCode.Trim() + "' and  Pf.Partcode='" + CpyKit + "' and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' order By Pf.Dt desc ", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("GetCpyKitFab", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;
                    dAd.SelectCommand.Parameters.Add("@CpyKit", SqlDbType.Char).Value = CpyKit;
                    dAd.SelectCommand.Parameters.Add("@SuppCode", SqlDbType.Char).Value = SuppCode;

                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);
                    return dsDetailsSub.Tables[0];
                }
            }
            return dsDetailsSub.Tables[0];
        }
        public DataTable GetSheetPartDts(string PCCode, int SheetSrNo, string MachineCode, string SheetPartcode, string PlanCode, string Partcode, string CatID)
        {

            string[] MachineDts = Regex.Split(MachineCode.ToString().Trim(), "-->");
            if (SheetPartcode == "0" && SheetSrNo == 0)
            {
                //dsDetailsSub = ComCon.procDS("Select PartDesc+'-->'+Pf.Partcode as Sheet,Pf.Partcode as SheetCode from  processfeedback pf Inner Join Part P On Pf.Partcode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "' and " +
                dsDetailsSub = ComCon.procDS("Select PartDesc as Sheet,Pf.Partcode as SheetCode from  processfeedback pf Inner Join Part P On Pf.Partcode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "' and " +
                   " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "' and CatID='" + CatID.Trim() + "'  and Edt is null and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' " +
                    " and CanopyPlanCode='" + PlanCode + "' and Productcode='" + Partcode + "' ", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("SheetPartDts", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@SheetSrno", SqlDbType.Char).Value = SheetSrNo;
                    dAd.SelectCommand.Parameters.Add("@SheetPartcode", SqlDbType.Char).Value = SheetPartcode;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;
                    //ADDED RB
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@CatID", SqlDbType.Char).Value = CatID;

                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);
                    return dsDetailsSub.Tables[0];
                }
            }

            else if (SheetPartcode != "0" && SheetSrNo == 0)
            {
                dsDetailsSub = ComCon.procDS("Select  VersionCode as SerialNo ,VersionCode as SerialNo1  from  processfeedback pf Inner Join Part P On Pf.Partcode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "' and " +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "' and CatID='" + CatID.Trim() + "'  and Edt is null and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' " +
                    " and CanopyPlanCode='" + PlanCode + "' and Productcode='" + Partcode + "' and PF.partcode='" + SheetPartcode + "' ", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("SheetPartDts", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@SheetSrno", SqlDbType.Char).Value = SheetSrNo;
                    dAd.SelectCommand.Parameters.Add("@SheetPartcode", SqlDbType.Char).Value = SheetPartcode;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;
                    //ADDED RB
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@CatID", SqlDbType.Char).Value = CatID;

                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);
                    return dsDetailsSub.Tables[0];
                }
            }

            else if (SheetPartcode != "0" && SheetSrNo != 0)
            {
                dsDetailsSub = ComCon.procDS("Select PKitQty as QtyPerSet,WtPerUt as WtPerUts,Round(WtPerUt*PKitQty,2) as  WtPerSet,PFBCode as TKITID  from  processfeedback pf Inner Join Part P On Pf.Partcode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "' and " +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "' and CatID='" + CatID.Trim() + "'  and Edt is null and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' " +
                    " and CanopyPlanCode='" + PlanCode + "' and Productcode='" + Partcode + "' and PF.partcode='" + SheetPartcode + "'  and VersionCode= " + SheetSrNo + "", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("SheetPartDts", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@SheetSrno", SqlDbType.Char).Value = SheetSrNo;
                    dAd.SelectCommand.Parameters.Add("@SheetPartcode", SqlDbType.Char).Value = SheetPartcode;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;
                    //ADDED RB
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@CatID", SqlDbType.Char).Value = CatID;

                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);
                    return dsDetailsSub.Tables[0];
                }
            }
            return dsDetailsSub.Tables[0];
        }
        public DataTable GetSheetPartDtsPartCut(int SheetSrNo, string MachineCode, string SheetPartcode, string PlanCode, string Partcode)
        {

            string[] MachineDts = Regex.Split(MachineCode.ToString().Trim(), "-->");
            if (SheetPartcode == "0" && SheetSrNo == 0)
            {
                //dsDetailsSub = ComCon.procDS("Select PartDesc+'-->'+Pf.Partcode as Sheet,Pf.Partcode as SheetCode from  processfeedback pf Inner Join Part P On Pf.Partcode=P.partcode where ProfitcenterCode='01.009' and " +
                dsDetailsSub = ComCon.procDS("Select PartDesc as Sheet,Pf.Partcode as SheetCode from  processfeedback pf Inner Join Part P On Pf.Partcode=P.partcode where ProfitcenterCode='01.076' and " +
                   " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' " +
                    " and CanopyPlanCode='" + PlanCode + "' and Productcode='" + Partcode + "' ", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("SheetPartDtsPartCut", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@SheetSrno", SqlDbType.Char).Value = SheetSrNo;
                    dAd.SelectCommand.Parameters.Add("@SheetPartcode", SqlDbType.Char).Value = SheetPartcode;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;

                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);
                    return dsDetailsSub.Tables[0];
                }
            }

            else if (SheetPartcode != "0" && SheetSrNo == 0)
            {
                dsDetailsSub = ComCon.procDS("Select  VersionCode as SerialNo ,VersionCode as SerialNo1  from  processfeedback pf Inner Join Part P On Pf.Partcode=P.partcode where ProfitcenterCode='01.076' and " +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' " +
                    " and CanopyPlanCode='" + PlanCode + "' and Productcode='" + Partcode + "' and PF.partcode='" + SheetPartcode + "' ", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("SheetPartDtsPartCut", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@SheetSrno", SqlDbType.Char).Value = SheetSrNo;
                    dAd.SelectCommand.Parameters.Add("@SheetPartcode", SqlDbType.Char).Value = SheetPartcode;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;

                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);
                    return dsDetailsSub.Tables[0];
                }
            }

            else if (SheetPartcode != "0" && SheetSrNo != 0)
            {
                dsDetailsSub = ComCon.procDS("Select PKitQty as QtyPerSet,WtPerUt as WtPerUts,Round(WtPerUt*PKitQty,2) as  WtPerSet,PFBCode as TKITID  from  processfeedback pf Inner Join Part P On Pf.Partcode=P.partcode where ProfitcenterCode='01.076' and " +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' " +
                    " and CanopyPlanCode='" + PlanCode + "' and Productcode='" + Partcode + "' and PF.partcode='" + SheetPartcode + "'  and VersionCode= " + SheetSrNo + "", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("SheetPartDtsPartCut", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@SheetSrno", SqlDbType.Char).Value = SheetSrNo;
                    dAd.SelectCommand.Parameters.Add("@SheetPartcode", SqlDbType.Char).Value = SheetPartcode;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;

                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);
                    return dsDetailsSub.Tables[0];
                }
            }
            return dsDetailsSub.Tables[0];
        }
        public DataTable GetTKitDts(string PCCode, string TKitID, int BatchQty, string TrnsType, string Plancode, string ProdCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetTKitDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;

            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
            dAd.SelectCommand.Parameters.Add("@TKitID", SqlDbType.Char).Value = TKitID;
            dAd.SelectCommand.Parameters.Add("@BatchQty", SqlDbType.Char).Value = BatchQty;
            dAd.SelectCommand.Parameters.Add("@TrnsType", SqlDbType.Char).Value = TrnsType;
            dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = Plancode;
            dAd.SelectCommand.Parameters.Add("@ProdCode", SqlDbType.Char).Value = ProdCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable GetTKitDtsPartCut(string TKitID, int BatchQty, string TrnsType, string Plancode, string ProdCode)
        {

            SqlDataAdapter dAd1 = new SqlDataAdapter("GetTKitDtsPartCut", con);
            dAd1.SelectCommand.CommandType = CommandType.StoredProcedure;

            dAd1.SelectCommand.Parameters.Add("@TKitID", SqlDbType.Char).Value = TKitID;
            dAd1.SelectCommand.Parameters.Add("@BatchQty", SqlDbType.Char).Value = BatchQty;
            dAd1.SelectCommand.Parameters.Add("@TrnsType", SqlDbType.Char).Value = TrnsType;
            dAd1.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = Plancode;
            dAd1.SelectCommand.Parameters.Add("@ProdCode", SqlDbType.Char).Value = ProdCode;
            dAd1.SelectCommand.CommandTimeout = 0;
            DataSet dSet1 = new DataSet();
            dSet1.Clear();
            dAd1.Fill(dSet1);
            return dSet1.Tables[0];

        }
        public DataTable GetCpyKit(string PCCode, string MachineCode, string PlanCode, string Partcode, string CpyKit)
        {
            string[] MachineDts = Regex.Split(MachineCode.ToString().Trim(), "-->");

            if (CpyKit == "0")
            {

                dsDetailsSub = ComCon.procDS(" select  AliseName as KitDesc,Pf.Partcode+'-->'+PartDesc as KitCode,PfbCode,EDt  from  processfeedback pf Inner Join Part P On Pf.partcode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and " +
                    " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null  and CanopyPlanCode='" + PlanCode + "'  and Productcode='" + Partcode + "' and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' order By Pf.Dt desc ", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {

                    SqlDataAdapter dAd = new SqlDataAdapter("GetCpyKit", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;
                    dAd.SelectCommand.Parameters.Add("@CpyKit", SqlDbType.Char).Value = CpyKit;

                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);
                    return dsDetailsSub.Tables[0];
                }

            }
            if (CpyKit != "0")
            {
                dsDetailsSub = ComCon.procDS(" select isnull(ProcessQty,0) as Bal  from  processfeedback pf Inner Join Part P On Pf.partcode=P.partcode where ProfitcenterCode='" + PCCode.Trim() + "'  and " +
                   " MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null  and CanopyPlanCode='" + PlanCode + "'  and Productcode='" + Partcode + "'  and  Pf.Partcode='" + CpyKit + "' and Pf.Active='1' and Pf.Dt>='2020-07-10 00:00:00' order By Pf.Dt desc ", "tbl_processfeedback");
                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
                {
                    return dsDetailsSub.Tables[0];
                }
                else
                {
                    SqlDataAdapter dAd = new SqlDataAdapter("GetCpyKit", con);
                    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                    dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                    dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;
                    dAd.SelectCommand.Parameters.Add("@CpyKit", SqlDbType.Char).Value = CpyKit;

                    dAd.SelectCommand.CommandTimeout = 0;
                    DataSet dsDetailsSub = new DataSet();
                    dAd.Fill(dsDetailsSub);
                    return dsDetailsSub.Tables[0];
                }
            }
            return dsDetailsSub.Tables[0];
        }
        public DataTable GetCpyKitPC(string PCCode, string MachineCode, string PlanCode, string Partcode, string CpyKit)
        {
            string[] MachineDts = Regex.Split(MachineCode.ToString().Trim(), "-->");

            //if (CpyKit == "0")
            //{

            dsDetailsSub = ComCon.procDS("select P.AliseName as  KitDesc,'1' as SelectPC,P1.KVA,P1.Model as ModelCPType,	'0.00-->0.00-->0.00-->0.00' as Sqft,NestingForQty as BatchQty,0 as BatchBalQty " +
                    ",ProcessQty as PrcQty,0.00 as PrcSqft,	0.00 as TotSqft,CanopyPlanCode as PlanCode,convert(varchar(10),C.dt,103) as PlanDt,pf.CatID,Pf.PartCode as KitCode,PfbCode,GroupPfbCode,EDt,	TurretKitCode as BOMCode,ProductCode " +
                    " from processfeedback pf Inner Join Part P On Pf.partcode=P.partcode Inner Join Part P1 On Pf.Productcode=P1.Partcode inner join CanopyPlan C on Pf.CanopyPlanCode=C.CPCode " +
                    " where ProfitcenterCode='" + PCCode.Trim() + "'  and  MachineCode='" + MachineDts[0].Trim() + "' and SerialNo='" + MachineDts[1].Trim() + "'  and Edt is null  and Pf.Active='1' and Pf.Partcode like '004%' and Pf.Dt>='2020-07-10 00:00:00' order By Pf.Dt desc ", "tbl_processfeedback");
            if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
            {
                return dsDetailsSub.Tables[0];
            }
            else
            {

                SqlDataAdapter dAd = new SqlDataAdapter("GetCpyKit", con);
                dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;
                dAd.SelectCommand.Parameters.Add("@CpyKit", SqlDbType.Char).Value = CpyKit;

                dAd.SelectCommand.CommandTimeout = 0;
                DataSet dsDetailsSub = new DataSet();
                dAd.Fill(dsDetailsSub);
                return dsDetailsSub.Tables[0];
            }
        }
        public DataTable CpyKitDts(string PCCode, int BatchQty, string CpyKitcode, string BOMCode, string PFBCode)
        {


            SqlDataAdapter dAd = new SqlDataAdapter("CpyKitDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;

            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
            dAd.SelectCommand.Parameters.Add("@BatchQty", SqlDbType.Char).Value = BatchQty;
            dAd.SelectCommand.Parameters.Add("@CpyKitcode", SqlDbType.Char).Value = CpyKitcode;
            dAd.SelectCommand.Parameters.Add("@BOMCode", SqlDbType.Char).Value = BOMCode;
            dAd.SelectCommand.Parameters.Add("@PFBCode", SqlDbType.Char).Value = PFBCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dsDetailsSub = new DataSet();
            dAd.Fill(dsDetailsSub);
            return dsDetailsSub.Tables[0];


        }
        public DataTable CpyKitDtsCPYAssly(string PCCode, int PrcQty, string CpyPartCode, string PlanCode, string BOMCode, string PFBCode)
        {
            dsDetailsSub = ComCon.procDS(" select P.AliseName as  Part,'0.00-->0.00-->0.00-->0.00' as Sqft " +
                 ",TotQty as PrcQty,0.00 as PrcSqft,	0.00 as TotSqft,Pf.PartCode as KitCode " +
                 " from processfeedbackDetails pf Inner Join Part P On Pf.partcode=P.partcode  " +
                 " where PFBCode='" + PFBCode.Trim() + "' and Pf.Partcode like '004%' ", "tbl_processfeedback");
            if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
            {
                return dsDetailsSub.Tables[0];
            }
            else
            {

                SqlDataAdapter dAd = new SqlDataAdapter("CpyKitDtsCPYAssly", con);
                dAd.SelectCommand.CommandType = CommandType.StoredProcedure;

                dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                dAd.SelectCommand.Parameters.Add("@PrcQty", SqlDbType.Char).Value = PrcQty;
                dAd.SelectCommand.Parameters.Add("@CpyPartCode", SqlDbType.Char).Value = CpyPartCode;
                dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;
                dAd.SelectCommand.Parameters.Add("@BOMCode", SqlDbType.Char).Value = BOMCode;
                dAd.SelectCommand.Parameters.Add("@PFBCode", SqlDbType.Char).Value = PFBCode;
                dAd.SelectCommand.CommandTimeout = 0;
                DataSet dsDetailsSub = new DataSet();
                dAd.Fill(dsDetailsSub);
                return dsDetailsSub.Tables[0];
            }

        }
        public DataTable CpyKitDtsCPYAsslyKit(string PCCode, int PrcQty, string CpyPartCode, string PlanCode, string BOMCode, string PFBCode)
        {
            dsDetailsSub = ComCon.procDS(" select Partdesc+'-->'+Pf.PartCode as  Part,KitQty as Qty,TotQty as PrcQty,StockQty as StkQty,Pf.PartCode " +
                 " from processfeedbackDetails pf Inner Join Part P On Pf.partcode=P.partcode  " +
                 " where PFBCode='" + PFBCode.Trim() + "' and MOB='B' ", "tbl_processfeedback");
            if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_processfeedback"].Rows.Count > 0)
            {
                return dsDetailsSub.Tables[0];
            }
            else
            {

                SqlDataAdapter dAd = new SqlDataAdapter("GetPCKit", con);
                dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
                dAd.SelectCommand.Parameters.Add("@BOMCode", SqlDbType.Char).Value = BOMCode;
                //Added RB
                dAd.SelectCommand.Parameters.Add("@CatID", SqlDbType.Char).Value = 0;
                dAd.SelectCommand.Parameters.Add("@PrcQty", SqlDbType.Char).Value = PrcQty;
                dAd.SelectCommand.CommandTimeout = 0;
                DataSet dsDetailsSub = new DataSet();
                dAd.Fill(dsDetailsSub);
                return dsDetailsSub.Tables[0];
            }

        }
        public DataTable CpyEndPrcDts(string PCCode, string PlanCode, string Partcode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("CpyEndPrcDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
            dAd.SelectCommand.Parameters.Add("@CPCode", SqlDbType.Char).Value = PlanCode;
            dAd.SelectCommand.Parameters.Add("@ProductCode", SqlDbType.Char).Value = Partcode;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable LoadMachine(string PCCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("LoadMachine", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable LoadProduct(string PCCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("LoadProduct", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable LoadOSSupplier()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("LoadOSSupplier", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            //dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }



        public DataTable LoadCatID(string PCCode, string PlanCode)
        {


            SqlDataAdapter dAd = new SqlDataAdapter("LoadCatagory", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
            dAd.SelectCommand.Parameters.Add("@PlanCode", SqlDbType.Char).Value = PlanCode;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];

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
            PrcCode = "PSH" + "/" + Yr + "/" + Max;

            cmd.Dispose();
            return PrcCode;
        }
        public Boolean getChkforStart(string PCCode, string PlanCode, string productcode, string strMachineNo, string catID)
        {
            Boolean ChkforStart = false;
            CommonCon ComCon = new CommonCon();
            dsChkforStart = ComCon.procDS("Select  isNull(Count(Productcode),0)  as CntStart From processfeedback where ProfitcenterCode='" + PCCode + "' and  CanopyPlanCode='" + PlanCode + "' and Productcode='" + productcode + "' and serialNo='" + strMachineNo + "'and CatID='" + catID + "' and Active='1'", "tbl_TurretKitForPrc");
            if (dsChkforStart != null && dsChkforStart.Tables["tbl_TurretKitForPrc"].Rows.Count == 1)
            {
                if (int.Parse(dsChkforStart.Tables["tbl_TurretKitForPrc"].Rows[0]["CntStart"].ToString().Trim()) == 0)

                {
                    ChkforStart = true;
                }
                else
                {
                    ChkforStart = false;
                }
            }
            else
            {
                ChkforStart = false;

            }
            return ChkforStart;

        }
        public Boolean getChkforStartCpy(string PCCode, string PlanCode, string productcode, string CatID)
        {
            Boolean ChkforStartCpy = false;
            CommonCon ComCon = new CommonCon();
            dsChkforStart = ComCon.procDS("Select  isNull(Count(Productcode),0)  as CntStart From processfeedback where ProfitcenterCode='" + PCCode + "' and  CanopyPlanCode='" + PlanCode + "' and Productcode='" + productcode + "' and CatID='" + CatID + "' and Active='1' ", "tbl_TurretKitForPrcCpy");
            if (dsChkforStart != null && dsChkforStart.Tables["tbl_TurretKitForPrcCpy"].Rows.Count == 1)
            {
                if (int.Parse(dsChkforStart.Tables["tbl_TurretKitForPrcCpy"].Rows[0]["CntStart"].ToString().Trim()) == 0)

                {
                    ChkforStartCpy = true;
                }
                else
                {
                    ChkforStartCpy = false;
                }
            }
            else
            {
                ChkforStartCpy = false;

            }
            return ChkforStartCpy;

        }
        public string GetPrevPrcTime(string PCCode, string PlanCode, string productcode, string strMachineNo, SqlConnection con, SqlTransaction tran)
        {
            string ChkforStart = "Null";
            CommonCon ComCon = new CommonCon();
            dsChkforStart = ComCon.procTranDS("Select Top 1 EDt From processfeedback where ProfitcenterCode='" + PCCode + "' and CanopyPlanCode='" + PlanCode + "' and Productcode='" + productcode + "' and SerialNo='" + strMachineNo + "' and Active='1' Order By Dt Desc ", "tbl_processfeedback", con, tran);
            if (dsChkforStart != null && dsChkforStart.Tables["tbl_processfeedback"].Rows.Count > 0)
            {

                ChkforStart = dsChkforStart.Tables["tbl_processfeedback"].Rows[0]["EDt"].ToString().Trim();

            }
            else
            {
                ChkforStart = "Null";

            }
            return ChkforStart;

        }

        //public double ChkForMaxRate( string PlanCode, string productcode, SqlConnection con, SqlTransaction tran)
        //{
        //    double ChkForMaxRate = 0;
        //    CommonCon ComCon = new CommonCon();
        //    ChkForMaxRate = ComCon.getTranName("Select Isnull(Max(Rate),0) as MRate From CanopyDtsSub where CanopyPlanCode='" + PlanCode + "' and Productcode='" + productcode + "' ", "tbl_ChkForMaxRate", "MRate", con, tran);

        //    return ChkForMaxRate;

        //}

        //OLd CNC
        /*
        public string SubmitCNC(CpyPrcCNCRequest CpyPrcCNCReq)
        {
            //int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;
            string PrcNo = "";
            string strTKitCode = "";
            string ShRate = "0";
            string strBOMCode = "0";
            strReqCode = "";
            try
            {
                if (CpyPrcCNCReq.TkitId.Substring(0, 3) == "TK/")
                {
                    string NstWtsqft = "";
                    string[] strMachineNo = Regex.Split(CpyPrcCNCReq.MachineCodeSrNo, "-->");
                    ChkforStartCPY = getChkforStartCpy(CpyPrcCNCReq.PCCode.Trim(), CpyPrcCNCReq.PlanCode, CpyPrcCNCReq.ProductCode);
                    ChkforStart = getChkforStart(CpyPrcCNCReq.PCCode.Trim(), CpyPrcCNCReq.PlanCode, CpyPrcCNCReq.ProductCode, strMachineNo[1].ToString());
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    NstWtsqft = ComCon.getTranName("Select convert(varchar(10),Pwt)+'-->'+convert(varchar(10),PSqft ) as PwtSqft from ProfitcenterPlDetails where ProfitcenterCode='01.005' and Partcode='" + CpyPrcCNCReq.ProductCode + "'", "TblPwtSqft", "PwtSqft", con, tran);
                    string[] strNstWtsqft = Regex.Split(NstWtsqft.Trim(), "-->");
                    PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcCNCReq.PCCode.Trim().Substring(0, 2), con, tran);
                    strTKitCode = ComCon.getTranName("select TurretKitPartcode+'-->'+convert(nvarchar(10),TLength)+'-->'+convert(nvarchar(10),TWidth)+'-->'+convert(nvarchar(10),TThickness) as TurretKitPartcode From TurretKitForPrc where TKitId='" + CpyPrcCNCReq.TkitId + "'", "tblTKitCode", "TurretKitPartcode", con, tran);
                    string[] strTKitCodeDts = Regex.Split(strTKitCode, "-->");
                    if (double.Parse(strTKitCodeDts[3].Trim()) <= 1.5)
                    {
                        ShRate = ComCon.getTranName("select convert(nvarchar(10),PRate) as PRate From Process where PCode='01.032' ", "tblProcess", "PRate", con, tran);
                    }
                    else if (double.Parse(strTKitCodeDts[3].Trim()) > 1.5)
                    {
                        ShRate = ComCon.getTranName("select convert(nvarchar(10),PRate) as PRate From Process where PCode='01.003' ", "tblProcess", "PRate", con, tran);
                    }

                    strBOMCode = ComCon.getTranName("select Max(Bd.BOMCode) as BOMCode From BOM B Inner Join BOMDetails BD on B.BomCode=Bd.BomCode  where B.Active='1' and Bd.KitCode='" + strTKitCodeDts[0].Trim() + "'", "tblBOM", "BOMCode", con, tran);

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,SupplierCode,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                    sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,");
                    sb.Append("PartCode,VersionCode,ProcessQty,PKitQty,PLength,PWidth,PThickness, CompanyCode,PFBRate,PPWCode,Remark)");
                    sb.Append(" values('" + PrcNo.Trim() + "','" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "', ");
                    if (ChkforStart == true)
                    {
                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                    }
                    else if (ChkforStart == false)
                    {
                        //ComCon.dateinyyyymmdd
                        //sb.Append("'" + ComCon.dateinyyyymmdd(GetPrevPrcTime(CpyPrcCNCReq.PCCode, CpyPrcCNCReq.PlanCode, CpyPrcCNCReq.ProductCode, strMachineNo[1].ToString(), con, tran)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                        sb.Append("'" + GetPrevPrcTime(CpyPrcCNCReq.PCCode, CpyPrcCNCReq.PlanCode, CpyPrcCNCReq.ProductCode, strMachineNo[1].ToString(), con, tran) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                    }
                    sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcCNCReq.OSSupplierCode.Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "',");
                    sb.Append("'" + CpyPrcCNCReq.ProductCode.Trim() + "','" + CpyPrcCNCReq.PlanCode.Trim() + "','" + strTKitCodeDts[0].Trim() + "',");
                    sb.Append("'" + CpyPrcCNCReq.ProductCode + "','" + CpyPrcCNCReq.BatchQty + "','" + double.Parse(strNstWtsqft[0].Trim()) + "','" + double.Parse(strNstWtsqft[1].Trim()) + "','" + CpyPrcCNCReq.ShWtperUts + "',");
                    sb.Append("'" + CpyPrcCNCReq.SheetPartcode + "','" + CpyPrcCNCReq.SerialNo + "','" + CpyPrcCNCReq.BatchQty + "','" + CpyPrcCNCReq.ShQtyPerset + "','" + strTKitCodeDts[1].Trim() + "', ");
                    sb.Append("'" + strTKitCodeDts[2].Trim() + "', '" + strTKitCodeDts[3].Trim() + "','01', ");
                    sb.Append("'" + ShRate.Trim() + "','" + CpyPrcCNCReq.EmpCode + "','Nil')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Action Taken File Attachment
                    #region
                    if (!string.IsNullOrEmpty(CpyPrcCNCReq.AttachFileDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(CpyPrcCNCReq.AttachFileDts, "@#@");
                        SrNoA = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNoA += 1;
                            DtsA = Regex.Split(StrSub.ToString().Trim(), "-->");

                            string FileName = PrcNo.ToString().Trim().Substring(4, 5).Trim() + PrcNo.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNoA) + Path.GetExtension(DtsA[1].ToString().Trim());
                            String StrMpath = ComCon.getMainFilePath("PrcCNC") + "/" + FileName.ToString().Trim();
                            string StrTpath = "C:/TempERPFile/TempPrcCNC/" + CpyPrcCNCReq.EmpCode.Trim() + "/" + DtsA[1].ToString().Trim();

                            string StrTempPath = "C:/TempERPFile/TempPrcCNC/" + CpyPrcCNCReq.EmpCode.Trim();
                            if (Directory.Exists(StrTempPath))
                            {
                                Directory.GetAccessControl(StrTpath);
                                File.Copy(StrTpath, StrMpath);
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessFeedbackFiles");
                            sb.Append("(GroupPFBCode,SrNo,FileName)");
                            sb.Append(" VALUES('" + PrcNo.Trim() + "' ,'" + SrNoA + "','" + FileName.ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }
                    #endregion
                    //Action Taken File Attachment

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,issueDate,issueQty,ToProfitCenterCode,StockType,StageName)");
                    sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + CpyPrcCNCReq.SheetPartcode.Trim() + "',");
                    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + CpyPrcCNCReq.ShWtperBatch + "','" + CpyPrcCNCReq.PCCode.Trim() + "',0,'0')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    if (ChkforStart == false)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,StageName)");
                        sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + CpyPrcCNCReq.SheetPartcode.Trim() + "',");
                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + CpyPrcCNCReq.ShWtperBatch + "','01.076',0,'0')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    if (ChkforStartCPY == true)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                        sb.Append(" values('" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "',");
                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    sb.Remove(0, sb.Length);
                    sb.Append("Update TurretKitForPrc set PrcStatus='D' where TKitId='" + CpyPrcCNCReq.TkitId + "'  and CPCode='" + CpyPrcCNCReq.PlanCode + "' and CanopyPartcode='" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    int recCount = ComCon.CountChars(CpyPrcCNCReq.PrcDts, ",");
                    string[] strPrcDts = Regex.Split(CpyPrcCNCReq.PrcDts, ",");
                    int SrNo = 0;

                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {

                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                        sb.Append("PFBRate,PLength,PWidth,PThickness,PLossWt,PHeight,PLength1,PLength2,");
                        sb.Append("PWidth1,PWidth2,PLossSqft,PCatagoryCode)");
                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + Dts[0].Trim() + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[1].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[2].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[3].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[4].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[5].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[6].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[7].Trim()) + "',");
                        sb.Append("'0','0','0','0','0','0','" + Dts[8].Trim() + "')");
                        //Partcode,KitQty,TotQty,PfbRate,Plen,Pwidth,PThk,PLossWt,WtPeruts,SqftPerUts,catCode
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        if (ChkforStart == false)
                        {
                            //if (Dts[0].ToString().Trim() == "0000030103000001000")
                            //{
                            //    sb.Remove(0, sb.Length);
                            //    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                            //    sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                            //    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcCNCReq.PCCode.Trim() + "',0)");
                            //    cmd = new SqlCommand(sb.ToString(), con);
                            //    cmd.Transaction = tran;
                            //    cmd.ExecuteNonQuery();
                            //    cmd.Dispose();
                            //}
                            //else
                            //{
                            //    sb.Remove(0, sb.Length);
                            //    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                            //    sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                            //    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','01.076',1)");
                            //    cmd = new SqlCommand(sb.ToString(), con);
                            //    cmd.Transaction = tran;
                            //    cmd.ExecuteNonQuery();
                            //    cmd.Dispose();
                            //}

                        }

                    }

                    string cntSheet = "0";
                    cntSheet = ComCon.getTranName("select isnull(Count(PrcStatus),0) as PrcStatus from TurretKitForPrc where PrcStatus='P'  and CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CanopyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' ", "TurretKitForPrc", "PrcStatus", con, tran);
                    if (cntSheet == "0")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDtsSub set CPTQty='" + CpyPrcCNCReq.BatchQty + "' ,CPTStatus='D'  where CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                        sb.Append(" values('" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "','01.076',");
                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                        sb.Append(" values('" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "','01.076',");
                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //Consumable Prc
                        #region
                        //string GetMaxRate = "0";
                        string PrcConsumable = "";
                        string ConsumableRate = "";
                        ConsumableRate = ComCon.getTranName("Select Isnull(Rate,0) as Rate  from ProfitcenterPlDetails where ProfitcenterCode='03.059' and Partcode='" + strTKitCodeDts[0].Trim() + "'", "TblPCPL", "Rate", con, tran);

                        PrcConsumable = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcCNCReq.PCCode.Trim().Substring(0, 2), con, tran);
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,SupplierCode,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                        sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,");
                        sb.Append("PartCode,VersionCode,ProcessQty,PKitQty,PLength,PWidth,PThickness, CompanyCode,PFBRate,PPWCode,Remark)");
                        sb.Append(" values('" + PrcNo.Trim() + "','" + PrcConsumable.Trim() + "','" + (PrcConsumable.Substring(10, 8)) + "', ");
                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                        sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcCNCReq.OSSupplierCode.Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "',");
                        sb.Append("'" + CpyPrcCNCReq.ProductCode.Trim() + "','" + CpyPrcCNCReq.PlanCode.Trim() + "','" + strTKitCodeDts[0].Trim() + "',");
                        //NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt
                        sb.Append("'" + CpyPrcCNCReq.ProductCode + "','" + CpyPrcCNCReq.BatchQty + "','" + double.Parse(strNstWtsqft[0].Trim()) + "','" + double.Parse(strNstWtsqft[1].Trim()) + "','0',");
                        //PartCode,VersionCode,ProcessQty,PKitQty,
                        sb.Append("'" + strTKitCodeDts[0].Trim() + "','0','" + CpyPrcCNCReq.BatchQty + "','0', ");
                        //PLength,PWidth,PThickness, CompanyCode
                        sb.Append("'0','0', '0','01', ");
                        //PFBRate,PPWCode,Remark
                        sb.Append("'" + ConsumableRate.Trim() + "','" + CpyPrcCNCReq.EmpCode + "','Nil')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        int ChkStk = 0;
                        dsCNCCons = ComCon.procTranDS("select Partdesc,Bd.Partcode,Qty as KitQty," + CpyPrcCNCReq.BatchQty + " * Qty as TotQty , (select Round(Isnull(Sum(Recqty) - sum(IssueQty), 0), 00) as Stk From (select Sum(ReceivedQty) as Recqty," +
                        "0.00 as IssueQty from stockwip where ToProfitcenterCode = '01.009' and StockType = '0' " +
                        " and Partcode = Bd.Partcode and  ReceivedQty > 0   Union all " +
                        " select 0.00 as Recqty, sum(IssueQty) as IssueQty from stockwip where FromProfitcenterCode = '01.009' and StockType = '0' " +
                        " and Partcode = Bd.Partcode and  IssueQty > 0) as stk) as StockQty,Bd.SuppRate,Bd.categoryID  " +
                         " from BOMDetails Bd Inner Join Part P On Bd.PartCode = P.Partcode " +
                        " where BOMCode ='" + strBOMCode + "'  and  kITCode = '" + strTKitCodeDts[0].Trim() + "' " +
                         " and Bd.Partcode Not like '006%' ", "tbl_dsCNCCons", con, tran);

                        if (dsCNCCons != null && dsCNCCons.Tables["tbl_dsCNCCons"].Rows.Count > 0)
                        {
                            SrNo = 0;

                            for (int brd = 0; brd < dsCNCCons.Tables["tbl_dsCNCCons"].Rows.Count; brd++)

                            {
                                if (Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["TotQty"].ToString().Trim()) >
                                Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["StockQty"].ToString().Trim()))
                                {
                                    if (ChkStk == 0)
                                    {
                                        PrcNo = dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["Partdesc"].ToString().Trim();
                                        ChkStk = 1;

                                    }
                                    else
                                    {
                                        PrcNo = PrcNo + "," + dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["Partdesc"].ToString().Trim();
                                    }
                                }

                                else if (ChkStk == 0)
                                {
                                    #region
                                    SrNo += 1;
                                    sb.Remove(0, sb.Length);
                                    sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                                    sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt,PLength,PWidth,PThickness,PLossWt,PCatagoryCode)");
                                    sb.Append("values('" + PrcConsumable.Trim() + "','" + SrNo + "',");
                                    sb.Append("'" + dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["Partcode"].ToString().Trim().Trim() + "',");
                                    sb.Append("'" + Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["KitQty"].ToString().Trim()) + "',");
                                    sb.Append("'" + Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["TotQty"].ToString().Trim()) + "',");
                                    sb.Append("'" + Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["SuppRate"].ToString().Trim()) + "',");
                                    sb.Append("'" + Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["SuppRate"].ToString().Trim()) + "',");
                                    sb.Append("'0','0','0','0','0','0',");
                                    sb.Append("'" + dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["categoryID"].ToString().Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();


                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                    sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["Partcode"].ToString().Trim() + "',");
                                    sb.Append("'" + PrcConsumable.Trim() + "',GetDate(),'" + Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["TotQty"].ToString()) + "','" + CpyPrcCNCReq.PCCode.Trim() + "',0)");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    #endregion

                                }
                            }
                            if (ChkStk > 0)
                            {
                                PrcNo = "Insufficient Stock For Consumable: " + PrcNo;
                                return PrcNo;
                            }
                        }
                        #endregion
                        //Consumable Prc

                        string GetMaxValue = "";

                        //Auto Req fab
                        dsReqMst = ComCon.procTranDS("exec CPYToLogPrcReq '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','01.008','01.008,01.007,01.005' ", "tbl_RaiseReqMst", con, tran);
                        if (dsReqMst != null && dsReqMst.Tables["tbl_RaiseReqMst"].Rows.Count > 0)
                        {
                            //Master Req
                            #region

                            GetMaxValue = "";
                            GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcCNCReq.PCCode.Trim().Substring(0, 2), con, tran);
                            // strReqCode = Convert.ToString("REQ/" + ComCon.yearEnd(con, tran) + "/" + CpyPrcCNCReq.PCCode.Trim().Substring(0, 2) + GetMaxValue);
                            strReqCode = GetMaxValue;
                            //PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcCNCReq.PCCode.Trim().Substring(0, 2), con, tran);

                            //sb.Remove(0, sb.Length);
                            //sb.Append("UPDATE GetMaxCode ");
                            //sb.Append("SET MaxValue='" + GetMaxValue + "' ");
                            //sb.Append("WHERE Prefix='REQ' and TblName='MaterialRequisitionWithOutPlan' and ");
                            //sb.Append("CompCode='" + CpyPrcCNCReq.PCCode.Trim().Substring(0, 2) + "' AND Yr='" + ComCon.yearEnd(con, tran) + "'");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();


                            cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanProcess", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@REQCode", strReqCode);
                            cmd.Parameters.AddWithValue("@MaxSrNo", GetMaxValue.Substring(10, 8).ToString());
                            cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                            cmd.Parameters.AddWithValue("@Yr", ComCon.yearEnd(con, tran));
                            cmd.Parameters.AddWithValue("@ProfitCenterCode", "01.008");
                            cmd.Parameters.AddWithValue("@ToProfitCenterCode", "03.059");
                            cmd.Parameters.AddWithValue("@ClassCode", CpyPrcCNCReq.ProductCode.ToString().Trim());
                            cmd.Parameters.AddWithValue("@ActNo", dsReqMst.Tables["tbl_RaiseReqMst"].Rows[0]["RaiseReqQty"].ToString().Trim());
                            cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcCNCReq.PCCode.Trim().Substring(0, 2));
                            cmd.Parameters.AddWithValue("@REQStatus", "P");
                            cmd.Parameters.AddWithValue("@REQType", "WIP");
                            cmd.Parameters.AddWithValue("@Remark", "Auto Req For Plan No " + CpyPrcCNCReq.PlanCode.Trim() + " and PrCode " + PrcNo.Trim());
                            cmd.Parameters.AddWithValue("@Discard", 1);
                            cmd.Parameters.AddWithValue("@Active", 1);
                            cmd.Parameters.AddWithValue("@Auth", 1);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            #endregion
                            //Master Req

                            dsReqDts = ComCon.procTranDS("exec InternalReqLogisticsKit '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "',1 ", "tbl_RaiseReqDts", con, tran);

                            if (dsReqDts != null && dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count > 0)
                            {
                                int SrNoReq = 0;

                                for (int cntd = 0; cntd < dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count; cntd++)
                                {

                                    SrNoReq = SrNoReq + 1;
                                    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@REQCode", strReqCode);
                                    cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                    //string[] DtlsPartCodeReq = null;
                                    //DtlsPartCodeReq = Regex.Split(dataItemDtls["PartDesc"].ToString().Trim(), "-->");
                                    cmd.Parameters.AddWithValue("@PartCode", dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(dsReqMst.Tables["tbl_RaiseReqMst"].Rows[0]["RaiseReqQty"].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@REQStatus", "P");
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    GetReqDetailsSub(strReqCode.Trim(), CpyPrcCNCReq.ProductCode.ToString().Trim(), 1, double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));


                                }
                            }

                        }
                        //Auto Req fab


                        //Auto PC Req
                        #region

                        dsReqMst = ComCon.procTranDS("exec CPYToLogPrcReq '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','01.007','01.007,01.005' ", "tbl_RaiseReqMst", con, tran);
                        if (dsReqMst != null && dsReqMst.Tables["tbl_RaiseReqMst"].Rows.Count > 0)
                        {

                            //Master Req
                            #region
                            GetMaxValue = "";
                            GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcCNCReq.PCCode.Trim().Substring(0, 2), con, tran);
                            //strReqCode = Convert.ToString("REQ/" + ComCon.yearEnd(con, tran) + "/" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + GetMaxValue);
                            strReqCode = GetMaxValue;
                            //sb.Remove(0, sb.Length);
                            //sb.Append("UPDATE GetMaxCode ");
                            //sb.Append("SET MaxValue='" + GetMaxValue + "' ");
                            //sb.Append("WHERE Prefix='REQ' and TblName='MaterialRequisitionWithOutPlan' and ");
                            //sb.Append("CompCode='" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + "' AND Yr='" + ComCon.yearEnd(con, tran) + "'");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();

                            cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanProcess", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@REQCode", strReqCode);
                            cmd.Parameters.AddWithValue("@MaxSrNo", GetMaxValue.Substring(10, 8).ToString());
                            cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@Yr", ComCon.yearEnd(con, tran));
                            cmd.Parameters.AddWithValue("@ProfitCenterCode", "01.007");
                            cmd.Parameters.AddWithValue("@ToProfitCenterCode", "03.059");
                            cmd.Parameters.AddWithValue("@ClassCode", CpyPrcCNCReq.ProductCode);
                            cmd.Parameters.AddWithValue("@ActNo", dsReqMst.Tables["tbl_RaiseReqMst"].Rows[0]["RaiseReqQty"].ToString().Trim());
                            cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcCNCReq.PCCode.Trim().Substring(0, 2));
                            cmd.Parameters.AddWithValue("@REQStatus", "P");
                            cmd.Parameters.AddWithValue("@REQType", "WIP");
                            cmd.Parameters.AddWithValue("@Remark", "Auto Req For Plan No: " + CpyPrcCNCReq.ProductCode + " and Prc No: " + PrcNo);
                            cmd.Parameters.AddWithValue("@Discard", 1);
                            cmd.Parameters.AddWithValue("@Active", 1);
                            cmd.Parameters.AddWithValue("@Auth", 1);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            #endregion
                            //Master Req

                            //Details Req
                            #region


                            dsReqDts = ComCon.procTranDS("exec InternalReqLogisticsKit '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "',2 ", "tbl_RaiseReqDts", con, tran);
                            if (dsReqDts != null && dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count > 0)
                            {
                                int SrNoReq = 0;

                                for (int cntd = 0; cntd < dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count; cntd++)
                                {
                                    SrNoReq = SrNoReq + 1;
                                    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@REQCode", strReqCode);
                                    cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                    cmd.Parameters.AddWithValue("@PartCode", dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(dsReqMst.Tables["tbl_RaiseReqMst"].Rows[0]["RaiseReqQty"].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@REQStatus", "P");
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    GetReqDetailsSub(strReqCode.Trim(), dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim(), 2, double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));


                                }
                            }
                            #endregion
                            //Details Req
                            //DateTime.Now.ToString("yyyy-MM-dd")
                            //*********************User Acivity ***************************
                            cmd = new SqlCommand("insertLoginTransactionDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@EmpID", CpyPrcCNCReq.EmpCode);
                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                            cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                            cmd.Parameters.AddWithValue("@TransactionNo", strReqCode);
                            cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcCNCReq.PCCode.Trim().Substring(0, 2));
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }

                        #endregion
                        //Auto PC Req

                        //Auto Canopy Assembly Req
                        #region
                        dsReqMstCPYAssly = ComCon.procTranDS("exec CPYToLogPrcReq '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','01.005','01.005' ", "tbl_RaiseReqMstCPYAssly", con, tran);
                        if (dsReqMstCPYAssly != null && dsReqMstCPYAssly.Tables["tbl_RaiseReqMstCPYAssly"].Rows.Count > 0)
                        {

                            // Master Req
                            #region
                            GetMaxValue = "";

                            GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcCNCReq.PCCode.Trim().Substring(0, 2), con, tran);
                            //strReqCodeCPYAssly = Convert.ToString("REQ/" + ComCon.yearEnd(con, tran) + "/" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + GetMaxValue);
                            //PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcBendReq.PCCode.Trim().Substring(0, 2), con, tran);
                            strReqCodeCPYAssly = GetMaxValue;

                            //sb.Remove(0, sb.Length);
                            //sb.Append("UPDATE GetMaxCode ");
                            //sb.Append("SET MaxValue='" + GetMaxValue + "' ");
                            //sb.Append("WHERE Prefix='REQ' and TblName='MaterialRequisitionWithOutPlan' and ");
                            //sb.Append("CompCode='" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + "' AND Yr='" + ComCon.yearEnd(con, tran) + "'");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();

                            cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanProcess", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@REQCode", strReqCodeCPYAssly);
                            cmd.Parameters.AddWithValue("@MaxSrNo", GetMaxValue.Substring(10, 8).ToString());
                            cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@Yr", ComCon.yearEnd(con, tran));
                            cmd.Parameters.AddWithValue("@ProfitCenterCode", "01.005");
                            cmd.Parameters.AddWithValue("@ToProfitCenterCode", "03.059");


                            cmd.Parameters.AddWithValue("@ClassCode", CpyPrcCNCReq.ProductCode);
                            cmd.Parameters.AddWithValue("@ActNo", dsReqMstCPYAssly.Tables["tbl_RaiseReqMstCPYAssly"].Rows[0]["RaiseReqQty"].ToString().Trim());
                            cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcCNCReq.PCCode.Trim().Substring(0, 2));
                            cmd.Parameters.AddWithValue("@REQStatus", "P");
                            cmd.Parameters.AddWithValue("@REQType", "WIP");
                            cmd.Parameters.AddWithValue("@Remark", "Auto Req For Plan No: " + CpyPrcCNCReq.ProductCode + " and Prc No: " + PrcNo);
                            cmd.Parameters.AddWithValue("@Discard", 1);
                            cmd.Parameters.AddWithValue("@Active", 1);
                            cmd.Parameters.AddWithValue("@Auth", 1);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            #endregion
                            //Master Req

                            //Details Req
                            #region



                            dsReqDtsCPYAssly = ComCon.procTranDS("exec InternalReqLogisticsKit '" + CpyPrcCNCReq.ProductCode + "',3 ", "tbl_RaiseReqDtsCPYAssly", con, tran);
                            if (dsReqDtsCPYAssly != null && dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows.Count > 0)
                            {
                                int SrNoReq = 0;

                                for (int cntd = 0; cntd < dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows.Count; cntd++)
                                {
                                    SrNoReq = SrNoReq + 1;
                                    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@REQCode", strReqCodeCPYAssly);
                                    cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                    cmd.Parameters.AddWithValue("@PartCode", dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["Partcode"].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(dsReqMstCPYAssly.Tables["tbl_RaiseReqMstCPYAssly"].Rows[0]["RaiseReqQty"].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@REQStatus", "P");
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    GetReqDetailsSub(strReqCodeCPYAssly.Trim(), dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["Partcode"].ToString().Trim(), 3, double.Parse(dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));
                                }
                            }
                            #endregion
                            //Details Req
                            //DateTime.Now.ToString("yyyy-MM-dd")
                            //*********************User Acivity ***************************
                            cmd = new SqlCommand("insertLoginTransactionDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@EmpID", CpyPrcCNCReq.PCCode.Trim().Substring(0, 2));
                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                            cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                            cmd.Parameters.AddWithValue("@TransactionNo", strReqCodeCPYAssly);
                            cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcCNCReq.PCCode.Trim().Substring(0, 2));
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }

                        //Auto Canopy Assembly Req
                        #endregion
                        //Auto Canopy Assembly Req


                    }

                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", CpyPrcCNCReq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "CNC Process");
                    cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcCNCReq.PCCode.Substring(0, 2).Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    tran.Commit();
                    //tran.Rollback();
                    PrcNo = "ProcessCode=" + PrcNo + " For CNC  and Req No " + strReqCode + " For Fabrication Saved SuccessFully ";
                    return PrcNo;
                }
                else if (CpyPrcCNCReq.TkitId.Substring(0, 3) == "PSH")
                {
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedBack set EDt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where PFBCode='" + CpyPrcCNCReq.TkitId.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,StageName)");
                    sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + CpyPrcCNCReq.SheetPartcode.Trim() + "',");
                    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + CpyPrcCNCReq.ShWtperBatch + "','01.076',0,'0')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    int recCount = ComCon.CountChars(CpyPrcCNCReq.PrcDts, ",");
                    string[] strPrcDts = Regex.Split(CpyPrcCNCReq.PrcDts, ",");
                    int SrNo = 0;

                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {
                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");
                        #region
                        //if (Dts[0].ToString().Trim() == "0000030103000001000")
                        //{
                        //    sb.Remove(0, sb.Length);
                        //    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                        //    sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                        //    sb.Append("'" + CpyPrcCNCReq.TkitId.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcCNCReq.PCCode.Trim() + "',0)");
                        //    cmd = new SqlCommand(sb.ToString(), con);
                        //    cmd.Transaction = tran;
                        //    cmd.ExecuteNonQuery();
                        //    cmd.Dispose();
                        //}
                        //else
                        //{
                        //    sb.Remove(0, sb.Length);
                        //    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                        //    sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                        //    sb.Append("'" + CpyPrcCNCReq.TkitId.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','01.002',1)");
                        //    cmd = new SqlCommand(sb.ToString(), con);
                        //    cmd.Transaction = tran;
                        //    cmd.ExecuteNonQuery();
                        //    cmd.Dispose();
                        //}
                    }

                    //string cntSheet = "0";
                    //cntSheet = ComCon.getTranName("select isnull(Count(PrcStatus),0) as PrcStatus from TurretKitForPrc where PrcStatus='P'  and CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CanopyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' ", "TurretKitForPrc", "PrcStatus", con, tran);
                    //if (cntSheet == "0")
                    //{
                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("Update CanopyPlanDtsSub set CPTQty='" + CpyPrcCNCReq.BatchQty + "' ,CPTStatus='D'  where CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' ");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();

                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                    //    sb.Append(" values('" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "','01.002',");
                    //    sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "',0)");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();

                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                    //    sb.Append(" values('" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "','01.002',");
                    //    sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "',0)");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();

                    //}
                    #endregion
                    tran.Commit();
                    //tran.Rollback();
                    PrcNo = "ProcessCode=" + CpyPrcCNCReq.TkitId.Trim() + " For CNC  End SuccessFully ";
                    return PrcNo;
                }
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return "StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString();
            }
            finally
            {
                con.Close();
            }
            return PrcNo;
        }
        */

        //OLd CNC
        protected void GetReqDetailsSub(string ReqCode, string strKitCode, int strPCwise, double KitQty)
        {
            dsReqDtsSub = ComCon.procTranDS("exec InternalReqLogisticsdetails '" + strKitCode + "'," + strPCwise + "", "tbl_RaiseReqDtsSub", con, tran);
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
        public string SubmitPartCut(CpyPrcPartcutRequest CpyPrcPartCut)
        {
            //int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;

            string PrcNo = "";
            string strTKitCode = "";
            string ShRate = "0";

            try
            {
                if (CpyPrcPartCut.TkitId.Substring(0, 3) == "TK/")
                {
                    string[] strMachineNo = Regex.Split(CpyPrcPartCut.MachineCodeSrNo, "-->");
                    ChkforStartCPY = getChkforStartCpy(CpyPrcPartCut.PCCode.Trim(), CpyPrcPartCut.PlanCode, CpyPrcPartCut.ProductCode, CpyPrcPartCut.CatID);
                    ChkforStart = getChkforStart(CpyPrcPartCut.PCCode.Trim(), CpyPrcPartCut.PlanCode, CpyPrcPartCut.ProductCode, strMachineNo[1].ToString(), CpyPrcPartCut.CatID);
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcPartCut.PCCode.Trim().Substring(0, 2), con, tran);
                    strTKitCode = ComCon.getTranName("select TurretKitPartcode+'-->'+convert(nvarchar(10),TLength)+'-->'+convert(nvarchar(10),TWidth)+'-->'+convert(nvarchar(10),TThickness) as TurretKitPartcode From TurretKitForPrc where TKitId='" + CpyPrcPartCut.TkitId + "'", "tblTKitCode", "TurretKitPartcode", con, tran);
                    string[] strTKitCodeDts = Regex.Split(strTKitCode, "-->");
                    if (double.Parse(strTKitCodeDts[3].Trim()) <= 1.5)
                    {
                        ShRate = ComCon.getTranName("select convert(nvarchar(10),PRate) as PRate From Process where PCode='01.032' ", "tblProcess", "PRate", con, tran);
                    }
                    else if (double.Parse(strTKitCodeDts[3].Trim()) > 1.5)
                    {
                        ShRate = ComCon.getTranName("select convert(nvarchar(10),PRate) as PRate From Process where PCode='01.003' ", "tblProcess", "PRate", con, tran);
                    }
                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                    sb.Append("PartCode,VersionCode,ProcessQty,PKitQty,PLength,PWidth,PThickness, WtPerUt,CompanyCode,PFBRate,PPWCode,Remark)");
                    sb.Append(" values('" + PrcNo.Trim() + "','" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "', ");
                    if (ChkforStart == true)
                    {
                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                    }
                    else if (ChkforStart == false)
                    {
                        sb.Append("'" + GetPrevPrcTime(CpyPrcPartCut.PCCode, CpyPrcPartCut.PlanCode, CpyPrcPartCut.ProductCode, strMachineNo[1].ToString(), con, tran) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                    }
                    sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcPartCut.PCCode.Trim() + "','" + CpyPrcPartCut.ProductCode.Trim() + "','" + CpyPrcPartCut.PlanCode.Trim() + "',");
                    sb.Append("'" + strTKitCodeDts[0].Trim() + "','" + CpyPrcPartCut.SheetPartcode + "','" + CpyPrcPartCut.SerialNo + "','" + CpyPrcPartCut.BatchQty + "','" + CpyPrcPartCut.ShQtyPerset + "','" + strTKitCodeDts[1].Trim() + "', ");
                    sb.Append("'" + strTKitCodeDts[2].Trim() + "', '" + strTKitCodeDts[3].Trim() + "', '" + CpyPrcPartCut.ShWtperUts + "','01', ");
                    sb.Append("'" + ShRate.Trim() + "','" + CpyPrcPartCut.EmpCode + "','Nil')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();


                    //Action Taken File Attachment
                    #region
                    if (!string.IsNullOrEmpty(CpyPrcPartCut.AttachFileDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(CpyPrcPartCut.AttachFileDts, "@#@");
                        SrNoA = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNoA += 1;
                            DtsA = Regex.Split(StrSub.ToString().Trim(), "-->");
                            string FileName = PrcNo.ToString().Trim().Substring(4, 5).Trim() + PrcNo.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNoA) + Path.GetExtension(DtsA[1].ToString().Trim());
                            String StrMpath = ComCon.getMainFilePath("PrcPartCut") + "/" + FileName.ToString().Trim();
                            string StrTpath = "C:/TempERPFile/TempPrcPartCut/" + CpyPrcPartCut.EmpCode.Trim() + "/" + DtsA[1].ToString().Trim();

                            string StrTempPath = "C:/TempERPFile/TempPrcPartCut/" + CpyPrcPartCut.EmpCode.Trim();
                            if (Directory.Exists(StrTempPath))
                            {
                                Directory.GetAccessControl(StrTpath);
                                File.Copy(StrTpath, StrMpath);
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessFeedbackFiles");
                            sb.Append("(GroupPFBCode,SrNo,FileName)");
                            sb.Append(" VALUES('" + PrcNo.Trim() + "' ,'" + SrNoA + "','" + FileName.ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }
                    #endregion
                    //Action Taken File Attachment

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,issueDate,issueQty,ToProfitCenterCode,StockType,StageName)");
                    sb.Append(" values('" + CpyPrcPartCut.PCCode.Trim() + "','" + CpyPrcPartCut.SheetPartcode.Trim() + "',");
                    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + CpyPrcPartCut.ShWtperBatch + "','" + CpyPrcPartCut.PCCode.Trim() + "',0,'0')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //if (ChkforStartCPY == true)
                    //{
                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                    //    sb.Append(" values('" + CpyPrcPartCut.ProductCode.ToString().Trim() + "','" + CpyPrcPartCut.PCCode.Trim() + "','" + CpyPrcPartCut.PCCode.Trim() + "',");
                    //    sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcPartCut.BatchQty.ToString().Trim() + "',0)");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();
                    //}

                    sb.Remove(0, sb.Length);
                    sb.Append("Update TurretKitForPrc set PartCutStatus='D' where TKitId='" + CpyPrcPartCut.TkitId + "'  and CPCode='" + CpyPrcPartCut.PlanCode + "' and CanopyPartcode='" + CpyPrcPartCut.ProductCode.ToString().Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    int recCount = ComCon.CountChars(CpyPrcPartCut.PrcDts, ",");
                    string[] strPrcDts = Regex.Split(CpyPrcPartCut.PrcDts, ",");
                    int SrNo = 0;

                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {

                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                        sb.Append("PFBRate,PLength,PWidth,PThickness,PLossWt,PHeight,PLength1,PLength2,");
                        sb.Append("PWidth1,PWidth2,PLossSqft,PCatagoryCode)");
                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + Dts[0].Trim() + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[1].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[2].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[3].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[4].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[5].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[6].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[7].Trim()) + "',");
                        sb.Append("'0','0','0','0','0','0','" + Dts[8].Trim() + "')");
                        //Partcode,KitQty,TotQty,PfbRate,Plen,Pwidth,PThk,PLossWt,WtPeruts,SqftPerUts,catCode
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        if (ChkforStart == false)
                        {
                            if (Dts[0].ToString().Trim() == "0000030103000001000")
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                                sb.Append(" values('" + CpyPrcPartCut.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                                sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcPartCut.PCCode.Trim() + "',0)");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                            else
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                                sb.Append(" values('" + CpyPrcPartCut.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                                sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','01.002',1)");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }
                    }

                    string cntSheet = "0";
                    cntSheet = ComCon.getTranName("select isnull(Count(PartCutStatus),0) as PrcStatus from TurretKitForPrc where PartCutStatus='P'  and CPCode='" + CpyPrcPartCut.PlanCode.Trim() + "' and CanopyPartcode='" + CpyPrcPartCut.ProductCode.Trim() + "' ", "TurretKitForPrc", "PrcStatus", con, tran);
                    if (cntSheet == "0")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDtsSub set CPPartCutQty='" + CpyPrcPartCut.BatchQty + "' ,CPPartCutStatus='D'  where CPCode='" + CpyPrcPartCut.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcPartCut.ProductCode.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                        sb.Append(" values('" + CpyPrcPartCut.ProductCode.ToString().Trim() + "','" + CpyPrcPartCut.PCCode.Trim() + "','01.076',");
                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcPartCut.BatchQty.ToString().Trim() + "',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                        sb.Append(" values('" + CpyPrcPartCut.ProductCode.ToString().Trim() + "','" + CpyPrcPartCut.PCCode.Trim() + "','01.002',");
                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcPartCut.BatchQty.ToString().Trim() + "',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }

                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", CpyPrcPartCut.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "PartCut Process");
                    cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcPartCut.PCCode.Substring(0, 2).Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    tran.Commit();
                    //tran.Rollback();
                    PrcNo = "ProcessCode=" + PrcNo + " For PartCutting  Saved SuccessFully ";
                    return PrcNo;
                }
                else if (CpyPrcPartCut.TkitId.Substring(0, 3) == "PSH")
                {
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedBack set EDt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where PFBCode='" + CpyPrcPartCut.TkitId.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();


                    int recCount = ComCon.CountChars(CpyPrcPartCut.PrcDts, ",");
                    string[] strPrcDts = Regex.Split(CpyPrcPartCut.PrcDts, ",");
                    int SrNo = 0;



                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {

                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                        if (Dts[0].ToString().Trim() == "0000030103000001000")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                            sb.Append(" values('" + CpyPrcPartCut.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                            sb.Append("'" + CpyPrcPartCut.TkitId.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcPartCut.PCCode.Trim() + "',0)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                        else
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                            sb.Append(" values('" + CpyPrcPartCut.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                            sb.Append("'" + CpyPrcPartCut.TkitId.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','01.002',1)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                    }

                    //string cntSheet = "0";
                    //cntSheet = ComCon.getTranName("select isnull(Count(PrcStatus),0) as PrcStatus from TurretKitForPrc where PrcStatus='P'  and CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CanopyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' ", "TurretKitForPrc", "PrcStatus", con, tran);
                    //if (cntSheet == "0")
                    //{
                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("Update CanopyPlanDtsSub set CPTQty='" + CpyPrcCNCReq.BatchQty + "' ,CPTStatus='D'  where CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' ");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();



                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                    //    sb.Append(" values('" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "','01.002',");
                    //    sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "',0)");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();

                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                    //    sb.Append(" values('" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "','01.002',");
                    //    sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "',0)");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();


                    //}

                    tran.Commit();
                    //tran.Rollback();
                    PrcNo = "ProcessCode=" + CpyPrcPartCut.TkitId.Trim() + " For PartCutting  End SuccessFully ";
                    return PrcNo;
                }
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
            return PrcNo;
        }
        public string SubmitBending(CpyPrcBendRequest CpyPrcBendReq)
        {

            //int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;

            string PrcNo = "";
            strReqCodeCPYAssly = "";
            try
            {
                String StrFabrication_PCCode = "0";
                if (CpyPrcBendReq.PCCode.Trim() == "01.002")
                {
                    StrFabrication_PCCode = "01.008"; //Fab
                }
                else if (CpyPrcBendReq.PCCode.Trim() == "03.004")
                {
                    StrFabrication_PCCode = "03.002"; ////Fab
                }
                else if (CpyPrcBendReq.PCCode.Trim() == "28.014")
                {
                    StrFabrication_PCCode = "28.015"; ////Fab
                }

                if (CpyPrcBendReq.PFBCode.Substring(0, 3) == "NEW")
                {
                    string[] strMachineNo = Regex.Split(CpyPrcBendReq.MachineCodeSrNo, "-->");
                    ChkforStartCPY = getChkforStartCpy(CpyPrcBendReq.PCCode.Trim(), CpyPrcBendReq.PlanCode, CpyPrcBendReq.ProductCode, CpyPrcBendReq.CatID);
                    ChkforStart = getChkforStart(CpyPrcBendReq.PCCode.Trim(), CpyPrcBendReq.PlanCode, CpyPrcBendReq.ProductCode, strMachineNo[1].ToString(), CpyPrcBendReq.CatID);

                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();
                    // Mst Entry
                    #region
                    PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcBendReq.PCCode.Trim().Substring(0, 2), con, tran);


                    string NstPart = "0";
                    string NstWtsqft = "0";
                    if (CpyPrcBendReq.CpyKitcode.Trim().Substring(11, 1) == "1" || CpyPrcBendReq.CpyKitcode.Trim().Substring(11, 1) == "0")
                    {

                        NstPart = ComCon.getTranName("select KitCode from Bomdetails where BOMCode='" + CpyPrcBendReq.BOMcode + "' and Kitcode Like '004%' and  substring(Kitcode,11,1) in ('4') and Partcode='" + CpyPrcBendReq.CpyKitcode.Trim() + "'", "TblNstPartCode", "KitCode", con, tran);

                    }
                    else if (CpyPrcBendReq.CpyKitcode.Trim().Substring(11, 1) == "2" || CpyPrcBendReq.CpyKitcode.Trim().Substring(11, 1) == "3")
                    {
                        NstPart = CpyPrcBendReq.CpyKitcode.Trim();
                    }

                    NstWtsqft = ComCon.getTranName("Select convert(varchar(10),Pwt)+'-->'+convert(varchar(10),PSqft ) as PwtSqft from ProfitcenterPlDetails where ProfitcenterCode='01.007' and Partcode='" + NstPart + "'", "TblPwtSqft", "PwtSqft", con, tran);
                    string[] strNstWtsqft = Regex.Split(NstWtsqft.Trim(), "-->");




                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                    sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,SqftperUt,");
                    sb.Append("PartCode,ProcessQty,CompanyCode,PFBRate,PPWCode,Remark,SilCladdingRate,CatID)");
                    sb.Append(" values('" + PrcNo.Trim() + "','" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "', ");
                    if (ChkforStart == true)
                    {
                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                    }
                    else if (ChkforStart == false)
                    {
                       // sb.Append("'" + ComCon.dateinyyyymmdd(GetPrevPrcTime(CpyPrcBendReq.PCCode, CpyPrcBendReq.PlanCode, CpyPrcBendReq.ProductCode, strMachineNo[1].ToString(), con, tran)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                       sb.Append("'" + GetPrevPrcTime(CpyPrcBendReq.PCCode, CpyPrcBendReq.PlanCode, CpyPrcBendReq.ProductCode, strMachineNo[1].ToString(), con, tran) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                    }
                    sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcBendReq.PCCode.Trim() + "','" + CpyPrcBendReq.ProductCode.Trim() + "',");
                    sb.Append("'" + CpyPrcBendReq.PlanCode.Trim() + "','" + CpyPrcBendReq.BOMcode.Trim() + "',");
                    sb.Append("'" + NstPart + "','" + CpyPrcBendReq.BatchQty + "','" + double.Parse(strNstWtsqft[0].Trim()) + "','" + double.Parse(strNstWtsqft[1].Trim()) + "','" + CpyPrcBendReq.PWt + "','" + CpyPrcBendReq.PSqft + "',");
                    sb.Append("'" + CpyPrcBendReq.CpyKitcode.Trim() + "','" + CpyPrcBendReq.PrcQty + "','" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + "', ");
                    sb.Append("'" + CpyPrcBendReq.PFBRate + "','" + CpyPrcBendReq.EmpCode + "','Nil','" + CpyPrcBendReq.Strokes + "', '" + CpyPrcBendReq.CatID + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion
                    // Mst Entry

                    //Action Taken File Attachment
                    #region
                    if (!string.IsNullOrEmpty(CpyPrcBendReq.AttachFileDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(CpyPrcBendReq.AttachFileDts, "@#@");
                        SrNoA = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNoA += 1;
                            DtsA = Regex.Split(StrSub.ToString().Trim(), "-->");

                            string FileName = PrcNo.ToString().Trim().Substring(4, 5).Trim() + PrcNo.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNoA) + Path.GetExtension(DtsA[1].ToString().Trim());
                            String StrMpath = ComCon.getMainFilePath("PrcBend") + "/" + FileName.ToString().Trim();
                            string StrTpath = "C:/TempERPFile/TempPrcBend/" + CpyPrcBendReq.EmpCode.Trim() + "/" + DtsA[1].ToString().Trim();

                            string StrTempPath = "C:/TempERPFile/TempPrcBend/" + CpyPrcBendReq.EmpCode.Trim();
                            if (Directory.Exists(StrTempPath))
                            {
                                Directory.GetAccessControl(StrTpath);
                                File.Copy(StrTpath, StrMpath);
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessFeedbackFiles");
                            sb.Append("(GroupPFBCode,SrNo,FileName)");
                            sb.Append(" VALUES('" + PrcNo.Trim() + "' ,'" + SrNoA + "','" + FileName.ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }
                    #endregion
                    //Action Taken File Attachment


                    // Dts Entry
                    #region
                    int recCount = ComCon.CountChars(CpyPrcBendReq.PrcDts, ",");
                    string[] strPrcDts = Regex.Split(CpyPrcBendReq.PrcDts, ",");
                    int SrNo = 0;



                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {

                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                        sb.Append("PFBRate,SaleRate,PLength,PWidth,PThickness,PLossWt,PCatagoryCode,WtPerUt,SqftPerUt)");
                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + Dts[0].Trim() + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[1].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[2].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[3].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[4].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[5].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[6].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[7].Trim()) + "',");
                        sb.Append("'" + Dts[8].Trim() + "','" + Dts[9].Trim() + "','" + Math.Round(double.Parse(Dts[10].Trim()), 2) + "','" + Math.Round(double.Parse(Dts[11].Trim()), 2) + "')");
                        //Partcode,KitQty,TotQty,PfbRate,Plen,Pwidth,PThk,PLossWt,WtPeruts,SqftPerUts,catCode
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();


                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                        sb.Append(" values('" + CpyPrcBendReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcBendReq.PCCode.Trim() + "',1)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        if (ChkforStart == false)
                        {

                            // Commented by KB on 20/12/2023 as Quality System is Started
                            //sb.Remove(0, sb.Length);
                            //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                            //sb.Append(" values('" + CpyPrcBendReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                            //sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + StrFabrication_PCCode.Trim() + "',0)");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();
                        }
                    }

                    #endregion
                    // Dts Entry
                    //Status Update
                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("Update CanopyPlanDtsSub set CPBQty=CPBQty + '" + CpyPrcBendReq.PrcQty + "' where CPCode='" + CpyPrcBendReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcBendReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcBendReq.CpyKitcode + "' and CatId='" + CpyPrcBendReq.CatID.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    string cntPrcQty = "0";
                    cntPrcQty = ComCon.getTranName("select CPQty-CPBQty as BalQty from CanopyPlanDtsSub where CPCode='" + CpyPrcBendReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcBendReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcBendReq.CpyKitcode + "' and CatId='" + CpyPrcBendReq.CatID.Trim() + "'  ", "BendingPrc", "BalQty", con, tran);
                    if (cntPrcQty == "0")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDtsSub set CPBStatus='D' where CPCode='" + CpyPrcBendReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcBendReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcBendReq.CpyKitcode + "' and CatId='" + CpyPrcBendReq.CatID.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }

                    string cntBndStatus = "0";
                    cntBndStatus = ComCon.getTranName("select Count(CPBStatus) as CPBStatus from CanopyPlanDtsSub where CPCode='" + CpyPrcBendReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcBendReq.ProductCode.Trim() + "' and CatId='" + CpyPrcBendReq.CatID.Trim() + "' and  CPBStatus='P'  ", "BendingPrc", "CPBSTatus", con, tran);
                    if (cntBndStatus == "0")
                    {
                        if (CpyPrcBendReq.CatID.ToString() == "029")
                        {



                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                            sb.Append(" values('" + CpyPrcBendReq.ProductCode.ToString().Trim() + "','" + CpyPrcBendReq.PCCode.Trim() + "','" + StrFabrication_PCCode.Trim() + "',");
                            sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcBendReq.BatchQty.ToString().Trim() + "',0)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                            sb.Append(" values('" + CpyPrcBendReq.ProductCode.ToString().Trim() + "','" + CpyPrcBendReq.PCCode.Trim() + "','" + StrFabrication_PCCode.Trim() + "',");
                            sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcBendReq.BatchQty.ToString().Trim() + "',0)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                           // Rohan Added

                            sb.Remove(0, sb.Length);
                            sb.Append("Update CanopyPlanSerialNo set CPBSerialStatus='D' where CPCode='" + CpyPrcBendReq.PlanCode.ToString().Trim() + "' and Partcode='" + CpyPrcBendReq.ProductCode.ToString().Trim() + "'  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();


                        }

                        #region

                        ////Auto PC Req
                        //#region

                        //dsReqMst = ComCon.procTranDS("exec CPYToLogPrcReq '" + CpyPrcBendReq.ProductCode.ToString().Trim() + "','01.007','01.007,01.005' ", "tbl_RaiseReqMst", con, tran);
                        //if (dsReqMst != null && dsReqMst.Tables["tbl_RaiseReqMst"].Rows.Count > 0)
                        //{

                        //    //Master Req
                        //    #region
                        //    GetMaxValue = "";
                        //    GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcBendReq.PCCode.Trim().Substring(0, 2), con, tran);
                        //    //strReqCode = Convert.ToString("REQ/" + ComCon.yearEnd(con, tran) + "/" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + GetMaxValue);
                        //    strReqCode = GetMaxValue;
                        //    //sb.Remove(0, sb.Length);
                        //    //sb.Append("UPDATE GetMaxCode ");
                        //    //sb.Append("SET MaxValue='" + GetMaxValue + "' ");
                        //    //sb.Append("WHERE Prefix='REQ' and TblName='MaterialRequisitionWithOutPlan' and ");
                        //    //sb.Append("CompCode='" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + "' AND Yr='" + ComCon.yearEnd(con, tran) + "'");
                        //    //cmd = new SqlCommand(sb.ToString(), con);
                        //    //cmd.Transaction = tran;
                        //    //cmd.ExecuteNonQuery();
                        //    //cmd.Dispose();

                        //    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanProcess", con);
                        //    cmd.CommandType = CommandType.StoredProcedure;
                        //    cmd.Parameters.AddWithValue("@REQCode", strReqCode);
                        //    cmd.Parameters.AddWithValue("@MaxSrNo", GetMaxValue.Substring(10, 8).ToString());
                        //    cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd"));
                        //    cmd.Parameters.AddWithValue("@Yr", ComCon.yearEnd(con, tran));
                        //    cmd.Parameters.AddWithValue("@ProfitCenterCode", "01.007");
                        //    cmd.Parameters.AddWithValue("@ToProfitCenterCode", "03.059");
                        //    cmd.Parameters.AddWithValue("@ClassCode", CpyPrcBendReq.ProductCode);
                        //    cmd.Parameters.AddWithValue("@ActNo", dsReqMst.Tables["tbl_RaiseReqMst"].Rows[0]["RaiseReqQty"].ToString().Trim());
                        //    cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcBendReq.PCCode.Trim().Substring(0, 2));
                        //    cmd.Parameters.AddWithValue("@REQStatus", "P");
                        //    cmd.Parameters.AddWithValue("@REQType", "WIP");
                        //    cmd.Parameters.AddWithValue("@Remark", "Auto Req For Plan No: " + CpyPrcBendReq.ProductCode + " and Prc No: " + PrcNo);
                        //    cmd.Parameters.AddWithValue("@Discard", 1);
                        //    cmd.Parameters.AddWithValue("@Active", 1);
                        //    cmd.Parameters.AddWithValue("@Auth", 1);
                        //    cmd.Transaction = tran;
                        //    cmd.ExecuteNonQuery();
                        //    cmd.Dispose();

                        //    #endregion
                        //    //Master Req

                        //    //Details Req
                        //    #region


                        //    dsReqDts = ComCon.procTranDS("exec InternalReqLogisticsKit '" + CpyPrcBendReq.ProductCode.ToString().Trim() + "',2 ", "tbl_RaiseReqDts", con, tran);
                        //    if (dsReqDts != null && dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count > 0)
                        //    {
                        //        int SrNoReq = 0;

                        //        for (int cntd = 0; cntd < dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count; cntd++)
                        //        {
                        //            SrNoReq = SrNoReq + 1;
                        //            cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                        //            cmd.CommandType = CommandType.StoredProcedure;
                        //            cmd.Parameters.AddWithValue("@REQCode", strReqCode);
                        //            cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                        //            cmd.Parameters.AddWithValue("@PartCode", dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim());
                        //            cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(dsReqMst.Tables["tbl_RaiseReqMst"].Rows[0]["RaiseReqQty"].ToString().Trim()));
                        //            cmd.Parameters.AddWithValue("@REQStatus", "P");
                        //            cmd.Transaction = tran;
                        //            cmd.ExecuteNonQuery();
                        //            cmd.Dispose();

                        //            GetReqDetailsSub(strReqCode.Trim(), dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim(), 2, double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));


                        //        }
                        //    }
                        //    #endregion
                        //    //Details Req
                        //    //DateTime.Now.ToString("yyyy-MM-dd")
                        //    //*********************User Acivity ***************************
                        //    cmd = new SqlCommand("insertLoginTransactionDetails", con);
                        //    cmd.CommandType = CommandType.StoredProcedure;
                        //    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                        //    cmd.Parameters.AddWithValue("@EmpID", CpyPrcBendReq.EmpCode);
                        //    cmd.Parameters.AddWithValue("@TransactionType", "S");
                        //    cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                        //    cmd.Parameters.AddWithValue("@TransactionNo", strReqCode);
                        //    cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcBendReq.PCCode.Trim().Substring(0, 2));
                        //    cmd.Transaction = tran;
                        //    cmd.ExecuteNonQuery();
                        //    cmd.Dispose();

                        //}

                        //#endregion
                        ////Auto PC Req

                        ////Auto Canopy Assembly Req
                        //#region
                        //dsReqMstCPYAssly = ComCon.procTranDS("exec CPYToLogPrcReq '" + CpyPrcBendReq.ProductCode.ToString().Trim() + "','01.005','01.005' ", "tbl_RaiseReqMstCPYAssly", con, tran);
                        //if (dsReqMstCPYAssly != null && dsReqMstCPYAssly.Tables["tbl_RaiseReqMstCPYAssly"].Rows.Count > 0)
                        //{

                        //    // Master Req
                        //    #region
                        //    GetMaxValue = "";

                        //    GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcBendReq.PCCode.Trim().Substring(0, 2), con, tran);
                        //    //strReqCodeCPYAssly = Convert.ToString("REQ/" + ComCon.yearEnd(con, tran) + "/" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + GetMaxValue);
                        //    //PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcBendReq.PCCode.Trim().Substring(0, 2), con, tran);
                        //    strReqCodeCPYAssly = GetMaxValue;

                        //    //sb.Remove(0, sb.Length);
                        //    //sb.Append("UPDATE GetMaxCode ");
                        //    //sb.Append("SET MaxValue='" + GetMaxValue + "' ");
                        //    //sb.Append("WHERE Prefix='REQ' and TblName='MaterialRequisitionWithOutPlan' and ");
                        //    //sb.Append("CompCode='" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + "' AND Yr='" + ComCon.yearEnd(con, tran) + "'");
                        //    //cmd = new SqlCommand(sb.ToString(), con);
                        //    //cmd.Transaction = tran;
                        //    //cmd.ExecuteNonQuery();
                        //    //cmd.Dispose();

                        //    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanProcess", con);
                        //    cmd.CommandType = CommandType.StoredProcedure;
                        //    cmd.Parameters.AddWithValue("@REQCode", strReqCodeCPYAssly);
                        //    cmd.Parameters.AddWithValue("@MaxSrNo", GetMaxValue.Substring(10, 8).ToString());
                        //    cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd"));
                        //    cmd.Parameters.AddWithValue("@Yr", ComCon.yearEnd(con, tran));
                        //    cmd.Parameters.AddWithValue("@ProfitCenterCode", "01.005");
                        //    cmd.Parameters.AddWithValue("@ToProfitCenterCode", "03.059");


                        //    cmd.Parameters.AddWithValue("@ClassCode", CpyPrcBendReq.ProductCode);
                        //    cmd.Parameters.AddWithValue("@ActNo", dsReqMstCPYAssly.Tables["tbl_RaiseReqMstCPYAssly"].Rows[0]["RaiseReqQty"].ToString().Trim());
                        //    cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcBendReq.PCCode.Trim().Substring(0, 2));
                        //    cmd.Parameters.AddWithValue("@REQStatus", "P");
                        //    cmd.Parameters.AddWithValue("@REQType", "WIP");
                        //    cmd.Parameters.AddWithValue("@Remark", "Auto Req For Plan No: " + CpyPrcBendReq.ProductCode + " and Prc No: " + PrcNo);
                        //    cmd.Parameters.AddWithValue("@Discard", 1);
                        //    cmd.Parameters.AddWithValue("@Active", 1);
                        //    cmd.Parameters.AddWithValue("@Auth", 1);
                        //    cmd.Transaction = tran;
                        //    cmd.ExecuteNonQuery();
                        //    cmd.Dispose();

                        //    #endregion
                        //    //Master Req

                        //    //Details Req
                        //    #region



                        //    dsReqDtsCPYAssly = ComCon.procTranDS("exec InternalReqLogisticsKit '" + CpyPrcBendReq.ProductCode + "',3 ", "tbl_RaiseReqDtsCPYAssly", con, tran);
                        //    if (dsReqDtsCPYAssly != null && dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows.Count > 0)
                        //    {
                        //        int SrNoReq = 0;

                        //        for (int cntd = 0; cntd < dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows.Count; cntd++)
                        //        {
                        //            SrNoReq = SrNoReq + 1;
                        //            cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                        //            cmd.CommandType = CommandType.StoredProcedure;
                        //            cmd.Parameters.AddWithValue("@REQCode", strReqCodeCPYAssly);
                        //            cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                        //            cmd.Parameters.AddWithValue("@PartCode", dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["Partcode"].ToString().Trim());
                        //            cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(dsReqMstCPYAssly.Tables["tbl_RaiseReqMstCPYAssly"].Rows[0]["RaiseReqQty"].ToString().Trim()));
                        //            cmd.Parameters.AddWithValue("@REQStatus", "P");
                        //            cmd.Transaction = tran;
                        //            cmd.ExecuteNonQuery();
                        //            cmd.Dispose();

                        //            GetReqDetailsSub(strReqCodeCPYAssly.Trim(), dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["Partcode"].ToString().Trim(), 3, double.Parse(dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));
                        //        }
                        //    }
                        //    #endregion
                        //    //Details Req
                        //    //DateTime.Now.ToString("yyyy-MM-dd")
                        //    //*********************User Acivity ***************************
                        //    cmd = new SqlCommand("insertLoginTransactionDetails", con);
                        //    cmd.CommandType = CommandType.StoredProcedure;
                        //    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                        //    cmd.Parameters.AddWithValue("@EmpID", CpyPrcBendReq.PCCode.Trim().Substring(0, 2));
                        //    cmd.Parameters.AddWithValue("@TransactionType", "S");
                        //    cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                        //    cmd.Parameters.AddWithValue("@TransactionNo", strReqCodeCPYAssly);
                        //    cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcBendReq.PCCode.Trim().Substring(0, 2));
                        //    cmd.Transaction = tran;
                        //    cmd.ExecuteNonQuery();
                        //    cmd.Dispose();

                        //}

                        ////Auto Canopy Assembly Req
                        //#endregion
                        ////Auto Canopy Assembly Req
                        #endregion

                        //Kanban Done By fs 08/08/2024
                        #region
                        strKanBan = "";
                        string GetMaxValue = "";
                        // Master Req
                        #region
                        GetMaxValue = "";

                        GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcBendReq.PCCode.Substring(0, 2), con, tran);
                        strKanBan = GetMaxValue;


                        sb.Remove(0, sb.Length);
                        sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                            " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                        sb.Append("values('" + strKanBan.Trim() + "','" + GetMaxValue.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                        sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + CpyPrcBendReq.PCCode.Trim() + "','23.001','" + CpyPrcBendReq.ProductCode + "','" + CpyPrcBendReq.PCCode.Substring(0, 2) + "','" + CpyPrcBendReq.BatchQty.ToString().Trim() + "','P','WIP',");
                        sb.Append("'Auto Req For Plan No: " + CpyPrcBendReq.ProductCode + " and Prc No: " + PrcNo + "','1','1','1','KanBan','0')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();


                        #endregion

                        //Master Req
                        //Details Req
                        #region

                        dsKanBan = ComCon.procTranDS("exec InternalTOCReq '" + CpyPrcBendReq.PCCode.Trim() + "' ", "tbl_RaiseReqDtsKanBan", con, tran);
                        if (dsKanBan != null && dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows.Count > 0)
                        {
                            int SrNoReq = 0;

                            for (int cntd = 0; cntd < dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows.Count; cntd++)
                            {
                                SrNoReq = SrNoReq + 1;
                                cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@REQCode", strKanBan);
                                cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                cmd.Parameters.AddWithValue("@PartCode", dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows[cntd]["Partcode"].ToString().Trim());
                                cmd.Parameters.AddWithValue("@Qty", double.Parse(dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));
                                cmd.Parameters.AddWithValue("@REQStatus", "P");
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                            }
                        }


                        #endregion
                        //Details Req
                        //DateTime.Now.ToString("yyyy-MM-dd")
                        //*********************User Acivity ***************************
                        cmd = new SqlCommand("insertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EmpID", CpyPrcBendReq.EmpCode);
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                        cmd.Parameters.AddWithValue("@TransactionNo", strKanBan);
                        cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcBendReq.PCCode.Substring(0, 2).Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        #endregion
                        //Kanban Done By fs 08/08/2024
                    }

                    #endregion
                    //Status Update With Req

                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", CpyPrcBendReq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "Bending Process");
                    cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcBendReq.PCCode.Substring(0, 2).Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Prc Below 1000    
                    #region
                    //string GetMaxRate = "0";
                    string GetMaxRatePartCode = "0";

                    string PrcBelowRate = "";
                    //GetMaxRate = ComCon.getTranName("Select Isnull(Max(Rate),0) as MRate From CanopyplandtsSub where CPCode='" + CpyPrcBendReq.PlanCode + "' and CpyPartcode='" + CpyPrcBendReq.ProductCode + "' and CatID='" + CpyPrcBendReq.CatID + "' ", "tbl_ChkForMaxRate", "MRate", con, tran);
                    //if (double.Parse(GetMaxRate.Trim()) == CpyPrcBendReq.Rate)

                    GetMaxRatePartCode = ComCon.getTranName("Select top 1 PartCode From CanopyplandtsSub where CPCode='" + CpyPrcBendReq.PlanCode + "' and CpyPartcode='" + CpyPrcBendReq.ProductCode + "' and CatID='" + CpyPrcBendReq.CatID + "' order by rate desc  ", "tbl_ChkForMaxRate", "PartCode", con, tran);


                   // GetMaxRatePartCode = ComCon.getTranName("Select top 1 PartCode From CanopyplandtsSub where CPCode='" + CpyPrcFabReq.PlanCode + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode + "' and CatID='" + CpyPrcFabReq.CatID + "' order by rate desc ", "tbl_ChkForMaxRate", "PartCode", con, tran);
                    if (GetMaxRatePartCode.Trim() == CpyPrcBendReq.CpyKitcode.Trim())

                    {
                        //dsKitbelowRate = ComCon.procTranDS("select Partcode From CanopyPlanDtsSubBelowStdRate where CpyPartcode='" + CpyPrcBendReq.ProductCode.ToString().Trim() + "' and CPCode='" + CpyPrcBendReq.PlanCode + "' ", "tbl_KitbelowRate", con, tran);
                        dsKitbelowRate = ComCon.procTranDS("select Pf.Partcode,Pl.rate,Pl.PurRate,Pwt,Psqft From CanopyPlanDtsSubBelowStdRate Pf Inner Join ProfitcenterPldetails Pl on  Pf.Partcode=Pl.partcode where CpyPartcode='" + CpyPrcBendReq.ProductCode.ToString().Trim() + "' and CPCode='" + CpyPrcBendReq.PlanCode + "' and CatID='" + CpyPrcBendReq.CatID + "' and ProfitcenterCode='01.002' ", "tbl_KitbelowRate", con, tran);
                        if (dsKitbelowRate != null && dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count > 0)
                        {
                            for (int br = 0; br < dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count; br++)
                            {

                                // Mst Entry NestingForQty,
                                #region
                                PrcBelowRate = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcBendReq.PCCode.Trim().Substring(0, 2), con, tran);
                                //dsDetails.Tables["tbl_RaiseReqDts"].Rows[i]["Partcode"].ToString().Trim()
                                sb.Remove(0, sb.Length);
                                sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                                sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,SqftperUt,");
                                sb.Append("PartCode,ProcessQty,CompanyCode,PFBRate,PPWCode,Remark,CatID,silCladdingRate)");
                                sb.Append(" values('" + PrcNo.Trim() + "','" + PrcBelowRate.Trim() + "','" + (PrcBelowRate.Substring(10, 8)) + "', ");
                                //sb.Append("'" + ComCon.dateinyyyymmdd(GetPrevPrcTime(CpyPrcBendReq.PCCode, CpyPrcBendReq.PlanCode, CpyPrcBendReq.ProductCode, strMachineNo[1].ToString(), con, tran)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                //sb.Append("'" + GetPrevPrcTime(CpyPrcBendReq.PCCode, CpyPrcBendReq.PlanCode, CpyPrcBendReq.ProductCode, strMachineNo[1].ToString(), con, tran) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcBendReq.PCCode.Trim() + "','" + CpyPrcBendReq.ProductCode.Trim() + "',");
                                sb.Append("'" + CpyPrcBendReq.PlanCode.Trim() + "','" + CpyPrcBendReq.BOMcode.Trim() + "',");
                                sb.Append("'" + NstPart + "','" + CpyPrcBendReq.BatchQty + "','" + NstWtsqft[0].ToString().Trim() + "','" + NstWtsqft[1].ToString().Trim() + "','" + double.Parse(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Pwt"].ToString().Trim()) + "','" + double.Parse(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["PSqft"].ToString().Trim()) + "',");
                                sb.Append("'" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "','" + CpyPrcBendReq.PrcQty + "','" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + "', ");
                                sb.Append("'" + ComCon.getTranName("Select Isnull(Max(Rate),0) as Rate From ProfitcenterPLDetails where ProfitcenterCode='" + CpyPrcBendReq.PCCode + "' and Partcode='" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "' ", "tbl_PLRate", "Rate", con, tran) + "'");
                                sb.Append(", '" + CpyPrcBendReq.EmpCode + "','Nil','" + CpyPrcBendReq.CatID + "' , '" + ComCon.getTranName("Select Isnull(Max(Strokes),0) as Strokes From CanopyPlanDtsSubBelowStdRate where CPCode='" + CpyPrcBendReq.PlanCode + "' and CPYPartcode='" + CpyPrcBendReq.ProductCode + "' and Partcode='" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "' ", "tbl_Strokes", "Strokes", con, tran) + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();




                                #endregion
                                // Mst Entry

                                //Dts Entry
                                #region

                                int ChkStk = 0;
                                dsKitbelowRatedts = ComCon.procTranDS("select Bd.Partcode,P.Partdesc,p.AliseName,Qty,Purrate,rate,Pwt,Psqft,Bd.Length,bd.Width,bd.Thickness,bd.LossWgt,Bd.categoryID, " +
                                   " (select Round(Isnull(Sum(Recqty) - sum(IssueQty), 0), 00) as Stk From ( select Sum(ReceivedQty) as Recqty, " +
                                  " 0.00 as IssueQty from stockwip where ToProfitcenterCode = '" + CpyPrcBendReq.PCCode + "' and StockType = '1' " +
                                  " and Partcode = Bd.Partcode and  ReceivedQty > 0  Union all " +
                                  " select 0.00 as Recqty, sum(IssueQty) as IssueQty from stockwip where FromProfitcenterCode = '" + CpyPrcBendReq.PCCode + "' and StockType = '1' " +
                                  " and Partcode = Bd.Partcode and  IssueQty > 0) as stk) as Stock " +
                                  "From BOMDetails Bd Inner Join ProfitcenterPLdetails Pl On   Bd.KitCode=Pl.Partcode  Inner Join part P On Bd.Partcode=P.partcode " +
                                  " where Bd.BOMCode='" + CpyPrcBendReq.BOMcode + "' and Bd.KitCode='" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "' " +
                                  " and Pl.ProfitcenterCode='01.002' and Bd.MOB='M' and P.Kit='0' ", "tbl_KitbelowRatedts", con, tran);
                                if (dsKitbelowRatedts != null && dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows.Count > 0)
                                {
                                    SrNo = 0;

                                    for (int brd = 0; brd < dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows.Count; brd++)

                                    {
                                        if ((Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) * CpyPrcBendReq.PrcQty) >
                                        Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Stock"].ToString().Trim()))
                                        {
                                            if (ChkStk == 0)
                                            {
                                                PrcNo = dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partdesc"].ToString().Trim();
                                                ChkStk = 1;

                                            }
                                            else
                                            {
                                                PrcNo = PrcNo + "," + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partdesc"].ToString().Trim();
                                            }
                                        }

                                        else if (ChkStk == 0)
                                        {
                                            SrNo += 1;
                                            sb.Remove(0, sb.Length);
                                            sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                                            sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt,PLength,PWidth,PThickness,PLossWt,PCatagoryCode)");
                                            sb.Append("values('" + PrcBelowRate.Trim() + "','" + SrNo + "',");
                                            sb.Append("'" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partcode"].ToString().Trim().Trim() + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) * CpyPrcBendReq.PrcQty + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Purrate"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["rate"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Pwt"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Psqft"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Length"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Width"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Thickness"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["LossWgt"].ToString().Trim()) + "',");
                                            sb.Append("'" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["categoryID"].ToString().Trim() + "')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();


                                            sb.Remove(0, sb.Length);
                                            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                            sb.Append(" values('" + CpyPrcBendReq.PCCode.Trim() + "','" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partcode"].ToString().Trim() + "',");
                                            sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString()) * CpyPrcBendReq.PrcQty + "','" + CpyPrcBendReq.PCCode.Trim() + "',1)");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();

                                            //Commented by KB on 20/12/2023
                                            //sb.Remove(0, sb.Length);
                                            //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                                            //sb.Append(" values('" + CpyPrcBendReq.PCCode.Trim() + "','" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partcode"].ToString().Trim() + "',");
                                            //sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim().Trim()) * CpyPrcBendReq.PrcQty + "','" + StrFabrication_PCCode.Trim() + "',0)");
                                            //cmd = new SqlCommand(sb.ToString(), con);
                                            //cmd.Transaction = tran;
                                            //cmd.ExecuteNonQuery();
                                            //cmd.Dispose();
                                        }
                                    }
                                    if (ChkStk > 0)
                                    {
                                        PrcNo = "Insufficient Stock For Part(BR): " + PrcNo;
                                        return PrcNo;
                                    }
                                }
                                #endregion
                                //Dts Entry
                            }


                        }
                    }
                    #endregion

                    //Prc Below 1000    

                    tran.Commit();
                     //tran.Rollback();

                    PrcNo = "ProcessCode:" + PrcNo + " and ReqCode: " + strReqCode + "," + strReqCodeCPYAssly + "  For Bending  Saved SuccessFully ";
                }
                else if (CpyPrcBendReq.PFBCode.Substring(0, 3) == "PSH")
                {
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedBack set EDt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where PFBCode='" + CpyPrcBendReq.PFBCode.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();


                    int recCount = ComCon.CountChars(CpyPrcBendReq.PrcDts, ",");
                    string[] strPrcDts = Regex.Split(CpyPrcBendReq.PrcDts, ",");
                    int SrNo = 0;


                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {

                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");


                        //Commented By KB on 21/12/2023
                        //sb.Remove(0, sb.Length);
                        //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                        //sb.Append(" values('" + CpyPrcBendReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                        //sb.Append("'" + CpyPrcBendReq.PFBCode.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + StrFabrication_PCCode.Trim() + "',0)");
                        //cmd = new SqlCommand(sb.ToString(), con);
                        //cmd.Transaction = tran;
                        //cmd.ExecuteNonQuery();
                        //cmd.Dispose();


                    }

                   tran.Commit();
                    //tran.Rollback();
                    PrcNo = "ProcessCode=" + CpyPrcBendReq.PFBCode.Trim() + " For Bending  End SuccessFully ";
                }
                return PrcNo;
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


        public string SubmitCNC(CpyPrcCNCRequest CpyPrcCNCReq)
        {
            string PrcNo = "";
            string strTKitCode = "";
            string ShRate = "0";
            string strBOMCode = "0";
            strReqCode = "";


            try
            {
                if (CpyPrcCNCReq.TkitId.Substring(0, 3) == "TK/")
                {
                    string NstWtsqft = "";
                    string[] strMachineNo = Regex.Split(CpyPrcCNCReq.MachineCodeSrNo, "-->");
                    // ChkforStartCPY = getChkforStartCpy(CpyPrcCNCReq.PCCode.Trim(), CpyPrcCNCReq.PlanCode, CpyPrcCNCReq.ProductCode);
                    ChkforStartCPY = getChkforStartCpy(CpyPrcCNCReq.PCCode.Trim(), CpyPrcCNCReq.PlanCode, CpyPrcCNCReq.ProductCode, CpyPrcCNCReq.CatID);
                    ChkforStart = getChkforStart(CpyPrcCNCReq.PCCode.Trim(), CpyPrcCNCReq.PlanCode, CpyPrcCNCReq.ProductCode, strMachineNo[1].ToString(), CpyPrcCNCReq.CatID);
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    NstWtsqft = ComCon.getTranName("Select convert(varchar(10),Pwt)+'-->'+convert(varchar(10),PSqft ) as PwtSqft from ProfitcenterPlDetails where ProfitcenterCode='01.005' and Partcode='" + CpyPrcCNCReq.ProductCode + "'", "TblPwtSqft", "PwtSqft", con, tran);
                    string[] strNstWtsqft = Regex.Split(NstWtsqft.Trim(), "-->");

                    PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcCNCReq.PCCode.Trim().Substring(0, 2), con, tran);

                    strTKitCode = ComCon.getTranName("select TurretKitPartcode+'-->'+convert(nvarchar(10),TLength)+'-->'+convert(nvarchar(10),TWidth)+'-->'+convert(nvarchar(10),TThickness) as TurretKitPartcode From TurretKitForPrc where TKitId='" + CpyPrcCNCReq.TkitId + "'", "tblTKitCode", "TurretKitPartcode", con, tran);

                    string[] strTKitCodeDts = Regex.Split(strTKitCode, "-->");
                    if (double.Parse(strTKitCodeDts[3].Trim()) <= 1.5)
                    {
                        ShRate = ComCon.getTranName("select convert(nvarchar(10),PRate) as PRate From Process where PCode='01.032' ", "tblProcess", "PRate", con, tran);
                    }
                    else if (double.Parse(strTKitCodeDts[3].Trim()) > 1.5)
                    {
                        ShRate = ComCon.getTranName("select convert(nvarchar(10),PRate) as PRate From Process where PCode='01.003' ", "tblProcess", "PRate", con, tran);
                    }

                    strBOMCode = ComCon.getTranName("select Max(Bd.BOMCode) as BOMCode From BOM B Inner Join BOMDetails BD on B.BomCode=Bd.BomCode  where B.Active='1' and Bd.KitCode='" + strTKitCodeDts[0].Trim() + "'", "tblBOM", "BOMCode", con, tran);

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,SupplierCode,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                    sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,");
                    sb.Append("PartCode,VersionCode,ProcessQty,PKitQty,PLength,PWidth,PThickness, CompanyCode,PFBRate,PPWCode,Remark,CatID)");
                    sb.Append(" values('" + PrcNo.Trim() + "','" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "', ");
                    if (ChkforStart == true)
                    {
                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                    }
                    else if (ChkforStart == false)
                    {
                        //ComCon.dateinyyyymmdd
                        //sb.Append("'" + ComCon.dateinyyyymmdd(GetPrevPrcTime(CpyPrcCNCReq.PCCode, CpyPrcCNCReq.PlanCode, CpyPrcCNCReq.ProductCode, strMachineNo[1].ToString(), con, tran)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                         sb.Append("'" + GetPrevPrcTime(CpyPrcCNCReq.PCCode, CpyPrcCNCReq.PlanCode, CpyPrcCNCReq.ProductCode, strMachineNo[1].ToString(), con, tran) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                    }
                    sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcCNCReq.OSSupplierCode.Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "',");
                    sb.Append("'" + CpyPrcCNCReq.ProductCode.Trim() + "','" + CpyPrcCNCReq.PlanCode.Trim() + "','" + strTKitCodeDts[0].Trim() + "',");
                    sb.Append("'" + CpyPrcCNCReq.ProductCode + "','" + CpyPrcCNCReq.BatchQty + "','" + double.Parse(strNstWtsqft[0].Trim()) + "','" + double.Parse(strNstWtsqft[1].Trim()) + "','" + CpyPrcCNCReq.ShWtperUts + "',");
                    sb.Append("'" + CpyPrcCNCReq.SheetPartcode + "','" + CpyPrcCNCReq.SerialNo + "','" + CpyPrcCNCReq.BatchQty + "','" + CpyPrcCNCReq.ShQtyPerset + "','" + strTKitCodeDts[1].Trim() + "', ");
                    sb.Append("'" + strTKitCodeDts[2].Trim() + "', '" + strTKitCodeDts[3].Trim() + "', '" + CpyPrcCNCReq.PCCode.Substring(0, 2).Trim() + "', ");
                    sb.Append("'" + ShRate.Trim() + "','" + CpyPrcCNCReq.EmpCode + "','Nil','" + CpyPrcCNCReq.CatID + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Action Taken File Attachment
                    #region
                    if (!string.IsNullOrEmpty(CpyPrcCNCReq.AttachFileDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(CpyPrcCNCReq.AttachFileDts, "@#@");
                        SrNoA = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNoA += 1;
                            DtsA = Regex.Split(StrSub.ToString().Trim(), "-->");

                            string FileName = PrcNo.ToString().Trim().Substring(4, 5).Trim() + PrcNo.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNoA) + Path.GetExtension(DtsA[1].ToString().Trim());
                            String StrMpath = ComCon.getMainFilePath("PrcCNC") + "/" + FileName.ToString().Trim();
                            string StrTpath = "C:/TempERPFile/TempPrcCNC/" + CpyPrcCNCReq.EmpCode.Trim() + "/" + DtsA[1].ToString().Trim();

                            string StrTempPath = "C:/TempERPFile/TempPrcCNC/" + CpyPrcCNCReq.EmpCode.Trim();
                            if (Directory.Exists(StrTempPath))
                            {
                                Directory.GetAccessControl(StrTpath);
                                File.Copy(StrTpath, StrMpath);
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessFeedbackFiles");
                            sb.Append("(GroupPFBCode,SrNo,FileName)");
                            sb.Append(" VALUES('" + PrcNo.Trim() + "' ,'" + SrNoA + "','" + FileName.ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }
                    #endregion
                    //Action Taken File Attachment


                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,issueDate,issueQty,ToProfitCenterCode,StockType,StageName)");
                    sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + CpyPrcCNCReq.SheetPartcode.Trim() + "',");
                    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + CpyPrcCNCReq.ShWtperBatch + "','" + CpyPrcCNCReq.PCCode.Trim() + "',0,'0')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    if (ChkforStart == false)
                    {

                        ////Not Required as PartCutting Prc to skip
                        //sb.Remove(0, sb.Length);
                        //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,StageName)");
                        //sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + CpyPrcCNCReq.SheetPartcode.Trim() + "',");
                        //sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + CpyPrcCNCReq.ShWtperBatch + "','01.076',0,'0')");
                        //cmd = new SqlCommand(sb.ToString(), con);
                        //cmd.Transaction = tran;
                        //cmd.ExecuteNonQuery();
                        //cmd.Dispose();


                    }
                    if (ChkforStartCPY == true  )

                    {
                        if (CpyPrcCNCReq.CatID.ToString() == "029")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                            sb.Append(" values('" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "',");
                            sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "',0)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }

                    sb.Remove(0, sb.Length);
                    sb.Append("Update TurretKitForPrc set PrcStatus='D' where TKitId='" + CpyPrcCNCReq.TkitId + "'  and CPCode='" + CpyPrcCNCReq.PlanCode + "' and CanopyPartcode='" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "' and CatID='" + CpyPrcCNCReq.CatID.ToString().Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    int recCount = ComCon.CountChars(CpyPrcCNCReq.PrcDts, ",");
                    string[] strPrcDts = Regex.Split(CpyPrcCNCReq.PrcDts, ",");
                    int SrNo = 0;

                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {

                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                        sb.Append("PFBRate,PLength,PWidth,PThickness,PLossWt,PHeight,PLength1,PLength2,");
                        sb.Append("PWidth1,PWidth2,PLossSqft,PCatagoryCode)");
                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + Dts[0].Trim() + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[1].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[2].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[3].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[4].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[5].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[6].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[7].Trim()) + "',");
                        sb.Append("'0','0','0','0','0','0','" + Dts[8].Trim() + "')");
                        //Partcode,KitQty,TotQty,PfbRate,Plen,Pwidth,PThk,PLossWt,WtPeruts,SqftPerUts,catCode
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        if (ChkforStart == false)
                        {
                            String StrBending_Skip_PCCode = "0";
                            if (CpyPrcCNCReq.PCCode.Trim() == "01.009")
                            {
                                StrBending_Skip_PCCode = "01.002"; //Bending
                            }
                            else if (CpyPrcCNCReq.PCCode.Trim() == "03.061")
                            {
                                StrBending_Skip_PCCode = "03.004"; //Bending
                            }
                            else if (CpyPrcCNCReq.PCCode.Trim() == "28.013")
                            {
                                StrBending_Skip_PCCode = "28.014"; //Bending
                            }

                            //Direct Transfer To bending PartCutting Skip (Stockwip)

                            #region
                            if (Dts[0].ToString().Trim() == "0000030103000001000")
                            {
                                // Commented by KB on 20/12/2023 as Quality System is Started
                                //sb.Remove(0, sb.Length);
                                //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                                //sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                                //sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcCNCReq.PCCode.Trim() + "',0)");
                                //cmd = new SqlCommand(sb.ToString(), con);
                                //cmd.Transaction = tran;
                                //cmd.ExecuteNonQuery();
                                //cmd.Dispose();
                            }
                            else
                            {
                                // Commented by KB on 20/12/2023 as Quality System is Started
                                //sb.Remove(0, sb.Length);
                                //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                                //sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                                //sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + StrBending_Skip_PCCode.Trim() + "',1)");
                                //cmd = new SqlCommand(sb.ToString(), con);
                                //cmd.Transaction = tran;
                                //cmd.ExecuteNonQuery();
                                //cmd.Dispose();
                            }
                            #endregion
                            //Direct Transfer To bending PartCutting Skip  (Stockwip)
                        }

                    }

                    string cntSheet = "0";
                    cntSheet = ComCon.getTranName("select isnull(Count(PrcStatus),0) as PrcStatus from TurretKitForPrc where PrcStatus='P'  and CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CanopyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' and CatId='" + CpyPrcCNCReq.CatID.Trim() + "' ", "TurretKitForPrc", "PrcStatus", con, tran);
                    if (cntSheet == "0")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDtsSub set CPTQty='" + CpyPrcCNCReq.BatchQty + "' ,CPTStatus='D'  where CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' and CatId='" + CpyPrcCNCReq.CatID.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();



                        cntSheet = ComCon.getTranName("select isnull(Count(PrcStatus),0) as PrcStatus from TurretKitForPrc where PrcStatus='P'  and CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CanopyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' and  CatId='" + CpyPrcCNCReq.CatID.Trim() + "' ", "TurretKitForPrc", "PrcStatus", con, tran);
                        if (cntSheet == "0")

                            sb.Remove(0, sb.Length);
                        sb.Append("Update TurretKitForPrc set PartCutStatus='D'  where CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CanopyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' and CatId='" + CpyPrcCNCReq.CatID.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDtsSub set CPPartCutQty='" + CpyPrcCNCReq.BatchQty + "' ,CPPartCutStatus='D'  where CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' and CatId='" + CpyPrcCNCReq.CatID.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        String StrBendingPCCode = "0";
                        if (CpyPrcCNCReq.PCCode.Trim() == "01.009")
                        {
                            StrBendingPCCode = "01.002"; //Bending
                        }
                        else if (CpyPrcCNCReq.PCCode.Trim() == "03.061")
                        {
                            StrBendingPCCode = "03.004"; //Bending
                        }
                        else if (CpyPrcCNCReq.PCCode.Trim() == "28.013")
                        {
                            StrBendingPCCode = "28.014"; //Bending
                        }

                        if (CpyPrcCNCReq.CatID.ToString() == "029")
                        {

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                            sb.Append(" values('" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "','" + StrBendingPCCode.Trim() + "',");
                            sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "',0)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                            sb.Append(" values('" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "','" + StrBendingPCCode.Trim() + "',");
                            sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "',0)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //Rohan Addded

                            sb.Remove(0, sb.Length);
                            sb.Append("Update CanopyPlanSerialNo set CPTSerialStatus='D' where CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and Partcode='" + CpyPrcCNCReq.ProductCode.Trim() + "'  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }


                        //Consumable Prc
                        #region
                        //string GetMaxRate = "0";
                        string PrcConsumable = "";
                        string ConsumableRate = "";

                        ConsumableRate = ComCon.getTranName("Select Isnull(Rate,0) as Rate  from ProfitcenterPlDetails where ProfitcenterCode='03.059' and Partcode='" + strTKitCodeDts[0].Trim() + "'", "TblPCPL", "Rate", con, tran);

                        //ConsumableRate = ComCon.getTranName("Select Isnull(Rate,0) as Rate  from ProfitcenterPlDetails where ProfitcenterCode='23.001' and Partcode='" + strTKitCodeDts[0].Trim() + "'", "TblPCPL", "Rate", con, tran);

                        PrcConsumable = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcCNCReq.PCCode.Trim().Substring(0, 2), con, tran);
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,SupplierCode,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                        sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,");
                        sb.Append("PartCode,VersionCode,ProcessQty,PKitQty,PLength,PWidth,PThickness, CompanyCode,PFBRate,PPWCode,Remark,CatID)");
                        sb.Append(" values('" + PrcNo.Trim() + "','" + PrcConsumable.Trim() + "','" + (PrcConsumable.Substring(10, 8)) + "', ");
                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                        sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcCNCReq.OSSupplierCode.Trim() + "','" + CpyPrcCNCReq.PCCode.Trim() + "',");
                        sb.Append("'" + CpyPrcCNCReq.ProductCode.Trim() + "','" + CpyPrcCNCReq.PlanCode.Trim() + "','" + strTKitCodeDts[0].Trim() + "',");
                        //NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt
                        sb.Append("'" + CpyPrcCNCReq.ProductCode + "','" + CpyPrcCNCReq.BatchQty + "','" + double.Parse(strNstWtsqft[0].Trim()) + "','" + double.Parse(strNstWtsqft[1].Trim()) + "','0',");
                        //PartCode,VersionCode,ProcessQty,PKitQty,
                        sb.Append("'" + strTKitCodeDts[0].Trim() + "','0','" + CpyPrcCNCReq.BatchQty + "','0', ");
                        //PLength,PWidth,PThickness, CompanyCode
                        sb.Append("'0','0', '0','" + CpyPrcCNCReq.PCCode.Substring(0, 2).Trim() + "', ");
                        //PFBRate,PPWCode,Remark
                        sb.Append("'" + ConsumableRate.Trim() + "','" + CpyPrcCNCReq.EmpCode + "','Nil','" + CpyPrcCNCReq.CatID + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //QUR Change By FS

                        int ChkStk = 0;

                        dsCNCCons = ComCon.procTranDS("select Partdesc,Bd.Partcode,Qty as KitQty," + CpyPrcCNCReq.BatchQty + " * Qty as TotQty , (select Round(Isnull(Sum(Recqty) - sum(IssueQty), 0), 00) as Stk From (select Sum(ReceivedQty) as Recqty," +
                      "0.00 as IssueQty from stockwip where ToProfitcenterCode = '" + CpyPrcCNCReq.PCCode.Trim() + "' and StockType = '0' " +
                      " and Partcode = Bd.Partcode and  ReceivedQty > 0   Union all " +
                      " select 0.00 as Recqty, sum(IssueQty) as IssueQty from stockwip where FromProfitcenterCode = '" + CpyPrcCNCReq.PCCode.Trim() + "' and StockType = '0' " +
                      " and Partcode = Bd.Partcode and  IssueQty > 0) as stk) as StockQty,Bd.SuppRate,Bd.categoryID  " +
                       " from BOMDetails Bd Inner Join Part P On Bd.PartCode = P.Partcode " +
                      " where BOMCode ='" + strBOMCode + "'  and  kITCode = '" + strTKitCodeDts[0].Trim() + "' " +
                       " and Bd.Partcode Not like '006%' ", "tbl_dsCNCCons", con, tran);



                        //dsCNCCons = ComCon.procTranDS("select Partdesc,Bd.Partcode,Qty as KitQty," + CpyPrcCNCReq.BatchQty + " * Qty as TotQty , 100 as StockQty," +
                        //" Bd.SuppRate,Bd.categoryID  " +
                        // " from BOMDetails Bd Inner Join Part P On Bd.PartCode = P.Partcode " +
                        //" where BOMCode ='" + strBOMCode + "'  and  kITCode = '" + strTKitCodeDts[0].Trim() + "' " +
                        // " and Bd.Partcode Not like '006%' ", "tbl_dsCNCCons", con, tran);




                        if (dsCNCCons != null && dsCNCCons.Tables["tbl_dsCNCCons"].Rows.Count > 0)
                        {
                            SrNo = 0;

                            for (int brd = 0; brd < dsCNCCons.Tables["tbl_dsCNCCons"].Rows.Count; brd++)

                            {
                                if (Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["TotQty"].ToString().Trim()) >
                                Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["StockQty"].ToString().Trim()))
                                {
                                    if (ChkStk == 0)
                                    {
                                        PrcNo = dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["Partdesc"].ToString().Trim();
                                        ChkStk = 1;
                                    }
                                    else
                                    {
                                        PrcNo = PrcNo + "," + dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["Partdesc"].ToString().Trim();
                                    }
                                }

                                else if (ChkStk == 0)
                                {
                                    #region
                                    SrNo += 1;
                                    sb.Remove(0, sb.Length);
                                    sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                                    sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt,PLength,PWidth,PThickness,PLossWt,PCatagoryCode)");
                                    sb.Append("values('" + PrcConsumable.Trim() + "','" + SrNo + "',");
                                    sb.Append("'" + dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["Partcode"].ToString().Trim().Trim() + "',");
                                    sb.Append("'" + Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["KitQty"].ToString().Trim()) + "',");
                                    sb.Append("'" + Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["TotQty"].ToString().Trim()) + "',");
                                    sb.Append("'" + Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["SuppRate"].ToString().Trim()) + "',");
                                    sb.Append("'" + Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["SuppRate"].ToString().Trim()) + "',");
                                    sb.Append("'0','0','0','0','0','0',");
                                    sb.Append("'" + dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["categoryID"].ToString().Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();


                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                    sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["Partcode"].ToString().Trim() + "',");
                                    sb.Append("'" + PrcConsumable.Trim() + "',GetDate(),'" + Convert.ToDouble(dsCNCCons.Tables["tbl_dsCNCCons"].Rows[brd]["TotQty"].ToString()) + "','" + CpyPrcCNCReq.PCCode.Trim() + "',0)");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    #endregion

                                }
                            }
                            if (ChkStk > 0)
                            {
                                PrcNo = "Insufficient Stock For Consumable: " + PrcNo;
                                return PrcNo;
                            }
                        }
                        #endregion
                        //Consumable Prc

                        string GetMaxValue = "";

                        GetMaxValue = "";
                        string RequisitionForPartCode = "";
                        #region //Auto Req
                        if (CpyPrcCNCReq.PCCode.Trim() == "28.013")
                        {
                            // Auto PC Req
                            #region
                            GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", "28", con, tran);
                            strReqCode = GetMaxValue;
                            #region
                            // Powder Coating to Logistics
                            sb.Remove(0, sb.Length);
                            sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode) ");
                            sb.Append("values('" + strReqCode.Trim() + "','" + GetMaxValue.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                            sb.Append("'" + ComCon.yearEnd(con, tran) + "','28.016','28.006','" + CpyPrcCNCReq.ProductCode + "','28','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "','P','WIP',");
                            sb.Append("'Auto Req For Plan No: " + CpyPrcCNCReq.ProductCode + " and Prc No: " + PrcNo + "','1','1','1','" + CpyPrcCNCReq.PlanCode.Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();


                            #endregion
                            //Master Req

                            //Details Req
                            #region


                            dsReqDts = ComCon.procTranDS("exec InternalReqLogisticsKit '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "',2 ", "tbl_RaiseReqDts", con, tran);
                            if (dsReqDts != null && dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count > 0)
                            {
                                int SrNoReq = 0;

                                for (int cntd = 0; cntd < dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count; cntd++)
                                {
                                    SrNoReq = SrNoReq + 1;
                                    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@REQCode", strReqCode);
                                    cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                    cmd.Parameters.AddWithValue("@PartCode", dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(CpyPrcCNCReq.BatchQty.ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@REQStatus", "P");
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    GetReqDetailsSub(strReqCode.Trim(), dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim(), 2, double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));


                                }
                            }
                            #endregion
                            //Details Req
                            //DateTime.Now.ToString("yyyy-MM-dd")
                            //*********************User Acivity ***************************
                            cmd = new SqlCommand("insertLoginTransactionDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@EmpID", CpyPrcCNCReq.EmpCode);
                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                            cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                            cmd.Parameters.AddWithValue("@TransactionNo", strReqCode);
                            cmd.Parameters.AddWithValue("@CompanyCode", "28");
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //}

                            #endregion
                            //Auto PC Req


                            //***********************************************

                            //Auto Canopy Assembly Req
                            #region
                            string GetKVA = ComCon.getTranName("Select KVA From Part where Partcode='" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "'", "tblPart", "KVA", con, tran);

                            String Str_ProfitCenterCode_CPY = "0";
                            String Str_Company_CPY = "0";

                            //if (double.Parse(GetKVA) <= 160)
                            //{
                            //    dsReqMstCPYAssly = ComCon.procTranDS("exec CPYToLogPrcReq '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','01.005','01.005' ", "tbl_RaiseReqMstCPYAssly", con, tran);
                            //}
                            //else
                            //{
                            //    dsReqMstCPYAssly = ComCon.procTranDS("exec CPYToLogPrcReq '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "','03.038','03.038' ", "tbl_RaiseReqMstCPYAssly", con, tran);
                            //}

                            // if (dsReqMstCPYAssly != null && dsReqMstCPYAssly.Tables["tbl_RaiseReqMstCPYAssly"].Rows.Count > 0)
                            // {

                            // Master Req
                            // #region
                            GetMaxValue = "";
                            //if (double.Parse(GetKVA) <= 160)
                            //{
                            //GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", "01", con, tran);

                            Str_ProfitCenterCode_CPY = "28.017";// Canopy Assembly
                            Str_Company_CPY = "28";
                            //}
                            //else
                            //{

                            //    Str_ProfitCenterCode_CPY = "03.038";
                            //    Str_Company_CPY = "03";
                            //}

                            GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", Str_Company_CPY.Trim(), con, tran);

                            //strReqCodeCPYAssly = Convert.ToString("REQ/" + ComCon.yearEnd(con, tran) + "/" + CpyPrcBendReq.PCCode.Trim().Substring(0, 2) + GetMaxValue);
                            //PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcBendReq.PCCode.Trim().Substring(0, 2), con, tran);
                            strReqCodeCPYAssly = GetMaxValue;


                            sb.Remove(0, sb.Length);
                            sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode) ");
                            sb.Append("values('" + strReqCodeCPYAssly.Trim() + "','" + GetMaxValue.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                            sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + Str_ProfitCenterCode_CPY.Trim() + "','28.006','" + CpyPrcCNCReq.ProductCode + "','" + Str_Company_CPY.Trim() + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "','P','WIP',");
                            sb.Append("'Auto Req For Plan No: " + CpyPrcCNCReq.ProductCode + " and Prc No: " + PrcNo + "','1','1','1','" + CpyPrcCNCReq.PlanCode.Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();


                            #endregion
                            //Master Req

                            //Details Req
                            #region


                            //if (double.Parse(GetKVA) <= 160)
                            //{
                            dsReqDtsCPYAssly = ComCon.procTranDS("exec InternalReqLogisticsKit '" + CpyPrcCNCReq.ProductCode + "',3 ", "tbl_RaiseReqDtsCPYAssly", con, tran);
                            if (dsReqDtsCPYAssly != null && dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows.Count > 0)
                            {
                                int SrNoReq = 0;

                                for (int cntd = 0; cntd < dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows.Count; cntd++)
                                {
                                    SrNoReq = SrNoReq + 1;
                                    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@REQCode", strReqCodeCPYAssly);
                                    cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                    cmd.Parameters.AddWithValue("@PartCode", dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["Partcode"].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(CpyPrcCNCReq.BatchQty.ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@REQStatus", "P");
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    GetReqDetailsSub(strReqCodeCPYAssly.Trim(), dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["Partcode"].ToString().Trim(), 3, double.Parse(dsReqDtsCPYAssly.Tables["tbl_RaiseReqDtsCPYAssly"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));
                                }
                            }


                            #endregion
                            //Details Req
                            //DateTime.Now.ToString("yyyy-MM-dd")
                            //*********************User Acivity ***************************
                            cmd = new SqlCommand("insertLoginTransactionDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@EmpID", CpyPrcCNCReq.EmpCode);
                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                            cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                            cmd.Parameters.AddWithValue("@TransactionNo", strReqCodeCPYAssly);
                            cmd.Parameters.AddWithValue("@CompanyCode", Str_Company_CPY.Trim());
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        else // Material Req For Unit I and Unit IV
                        {
                            //Auto PC Req
                            #region
                            if (CpyPrcCNCReq.PCCode.Substring(0, 2).ToString() == "01" &&
                                   CpyPrcCNCReq.CatID == "029")

                            {
                                RequisitionForPartCode = ComCon.getTranName(
                                                                            "SELECT Partcode FROM BOMdetails " +
                                                                            "WHERE BOMCode='" + strBOMCode + "' " +
                                                                              //"AND KitCode='" + Dts[2].ToString().Trim() + "' " +
                                                                              "AND SUBSTRING(Partcode, 11, 1) IN ('4') " +
                                                                              "AND Partcode LIKE '004%'",
                                                                              "tblBP", "Partcode", con, tran);



                                #region
                                GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcCNCReq.PCCode.Substring(0, 2).ToString(), con, tran);
                                strReqCode = GetMaxValue;

                                #region
                                sb.Remove(0, sb.Length);
                                sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                    " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                                sb.Append("values('" + strReqCode.Trim() + "','" + GetMaxValue.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                                sb.Append("'" + ComCon.yearEnd(con, tran) + "','01.007','23.001','" + CpyPrcCNCReq.ProductCode + "','01','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "','P','WIP',");
                                sb.Append("'Auto Req For Plan No: " + CpyPrcCNCReq.ProductCode + " and Prc No: " + PrcNo + "','1','1','1','" + CpyPrcCNCReq.PlanCode.Trim() + "','" + RequisitionForPartCode.Trim() + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                                #endregion
                                //Master Req

                                //Details Req
                                #region


                                dsReqDts = ComCon.procTranDS("exec InternalReqLogisticsKit '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "',2,'" + CpyPrcCNCReq.CatID + "'", "tbl_RaiseReqDts", con, tran);
                                if (dsReqDts != null && dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count > 0)
                                {
                                    int SrNoReq = 0;

                                    for (int cntd = 0; cntd < dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count; cntd++)
                                    {
                                        SrNoReq = SrNoReq + 1;
                                        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@REQCode", strReqCode);
                                        cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                        cmd.Parameters.AddWithValue("@PartCode", dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim());
                                        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(CpyPrcCNCReq.BatchQty.ToString().Trim()));
                                        cmd.Parameters.AddWithValue("@REQStatus", "P");
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        GetReqDetailsSub(strReqCode.Trim(), dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim(), 2, double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));


                                    }
                                }
                                #endregion
                                //Details Req
                                //DateTime.Now.ToString("yyyy-MM-dd")
                                //*********************User Acivity ***************************
                                cmd = new SqlCommand("insertLoginTransactionDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@EmpID", CpyPrcCNCReq.EmpCode);
                                cmd.Parameters.AddWithValue("@TransactionType", "S");
                                cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                                cmd.Parameters.AddWithValue("@TransactionNo", strReqCode);
                                cmd.Parameters.AddWithValue("@CompanyCode", "01");
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                //}

                                #endregion
                            }
                            else if (CpyPrcCNCReq.PCCode.Substring(0, 2).ToString() == "03" &&
                                   CpyPrcCNCReq.CatID == "038")

                            {
                                RequisitionForPartCode = ComCon.getTranName(
                                                                               "SELECT Partcode FROM BOMdetails " +
                                                                               "WHERE BOMCode='" + strBOMCode + "' " +
                                                                               "AND SUBSTRING(Partcode, 12, 1) IN ('3') " +
                                                                               "AND SUBSTRING(kitcode, 11, 1) IN ('5') " +
                                                                               "AND KITCode LIKE '004%'",
                                                                               "tblBP", "Partcode", con, tran);


                                #region
                                // Unit 1 PC only 
                               // GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcCNCReq.PCCode.Substring(0, 2).ToString(), con, tran);
                                GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", "01", con, tran);
                                strReqCode = GetMaxValue;

                                #region
                                sb.Remove(0, sb.Length);
                                sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                    " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                                sb.Append("values('" + strReqCode.Trim() + "','" + GetMaxValue.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                                sb.Append("'" + ComCon.yearEnd(con, tran) + "','01.007','23.001','" + CpyPrcCNCReq.ProductCode + "','01','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "','P','WIP',");
                                sb.Append("'Auto Req For Plan No: " + CpyPrcCNCReq.ProductCode + " and Prc No: " + PrcNo + "','1','1','1','" + CpyPrcCNCReq.PlanCode.Trim() + "' ,'" + RequisitionForPartCode.Trim() + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                                #endregion
                                //Master Req

                                //Details Req
                                #region


                                dsReqDts = ComCon.procTranDS("exec InternalReqLogisticsKit '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "',2,'" + CpyPrcCNCReq.CatID + "' ", "tbl_RaiseReqDts", con, tran);
                                if (dsReqDts != null && dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count > 0)
                                {
                                    int SrNoReq = 0;

                                    for (int cntd = 0; cntd < dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count; cntd++)
                                    {
                                        SrNoReq = SrNoReq + 1;
                                        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@REQCode", strReqCode);
                                        cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                        cmd.Parameters.AddWithValue("@PartCode", dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim());
                                        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(CpyPrcCNCReq.BatchQty.ToString().Trim()));
                                        cmd.Parameters.AddWithValue("@REQStatus", "P");
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        GetReqDetailsSub(strReqCode.Trim(), dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim(), 2, double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));


                                    }
                                }
                                #endregion
                                //Details Req
                                //DateTime.Now.ToString("yyyy-MM-dd")
                                //*********************User Acivity ***************************
                                cmd = new SqlCommand("insertLoginTransactionDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@EmpID", CpyPrcCNCReq.EmpCode);
                                cmd.Parameters.AddWithValue("@TransactionType", "S");
                                cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                                cmd.Parameters.AddWithValue("@TransactionNo", strReqCode);
                                cmd.Parameters.AddWithValue("@CompanyCode", "01");
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                //}

                                #endregion
                            }
                            else if (CpyPrcCNCReq.PCCode.Substring(0, 2).ToString() == "03" &&
                                 CpyPrcCNCReq.CatID == "084")

                            {
                                RequisitionForPartCode = ComCon.getTranName(
                                                                               "SELECT Partcode FROM BOMdetails " +
                                                                               "WHERE BOMCode='" + strBOMCode + "' " +
                                                                               "AND SUBSTRING(Partcode, 12, 1) IN ('3') " +
                                                                               "AND SUBSTRING(kitcode, 11, 1) IN ('5') " +
                                                                               "AND KITCode LIKE '004%'",
                                                                               "tblBP", "Partcode", con, tran);


                                #region
                               // GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcCNCReq.PCCode.Substring(0, 2).ToString(), con, tran);
                                GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", "01", con, tran);
                                strReqCode = GetMaxValue;

                                #region
                                sb.Remove(0, sb.Length);
                                sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                    " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                                sb.Append("values('" + strReqCode.Trim() + "','" + GetMaxValue.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                                sb.Append("'" + ComCon.yearEnd(con, tran) + "','01.007','23.001','" + CpyPrcCNCReq.ProductCode + "','01','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "','P','WIP',");
                                sb.Append("'Auto Req For Plan No: " + CpyPrcCNCReq.ProductCode + " and Prc No: " + PrcNo + "','1','1','1','" + CpyPrcCNCReq.PlanCode.Trim() + "','" + RequisitionForPartCode.Trim() + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                                #endregion
                                //Master Req

                                //Details Req
                                #region


                                dsReqDts = ComCon.procTranDS("exec InternalReqLogisticsKit '" + CpyPrcCNCReq.ProductCode.ToString().Trim() + "',2,'" + CpyPrcCNCReq.CatID + "' ", "tbl_RaiseReqDts", con, tran);
                                if (dsReqDts != null && dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count > 0)
                                {
                                    int SrNoReq = 0;

                                    for (int cntd = 0; cntd < dsReqDts.Tables["tbl_RaiseReqDts"].Rows.Count; cntd++)
                                    {
                                        SrNoReq = SrNoReq + 1;
                                        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@REQCode", strReqCode);
                                        cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                        cmd.Parameters.AddWithValue("@PartCode", dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim());
                                        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(CpyPrcCNCReq.BatchQty.ToString().Trim()));
                                        cmd.Parameters.AddWithValue("@REQStatus", "P");
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        GetReqDetailsSub(strReqCode.Trim(), dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["Partcode"].ToString().Trim(), 2, double.Parse(dsReqDts.Tables["tbl_RaiseReqDts"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));


                                    }
                                }
                                #endregion
                                //Details Req
                                //DateTime.Now.ToString("yyyy-MM-dd")
                                //*********************User Acivity ***************************
                                cmd = new SqlCommand("insertLoginTransactionDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@EmpID", CpyPrcCNCReq.EmpCode);
                                cmd.Parameters.AddWithValue("@TransactionType", "S");
                                cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                                cmd.Parameters.AddWithValue("@TransactionNo", strReqCode);
                                cmd.Parameters.AddWithValue("@CompanyCode", "01");
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                //}

                                #endregion
                            }


                            #endregion

                            //Auto PC Req
                            //Kanban
                            #region
                            strKanBan = "";
                            dsKanBan = ComCon.procTranDS("exec InternalTOCReq '" + CpyPrcCNCReq.PCCode.Trim() + "' ", "tbl_RaiseReqDtsKanBan", con, tran);
                            if (dsKanBan != null && dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows.Count > 0)
                            {
                                // Master Req
                                #region
                                GetMaxValue = "";

                                GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcCNCReq.PCCode.Substring(0, 2), con, tran);
                                strKanBan = GetMaxValue;


                                sb.Remove(0, sb.Length);
                                sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                    " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode) ");
                                sb.Append("values('" + strKanBan.Trim() + "','" + GetMaxValue.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                                sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + CpyPrcCNCReq.PCCode.Trim() + "','23.001','" + CpyPrcCNCReq.ProductCode + "','" + CpyPrcCNCReq.PCCode.Substring(0, 2) + "','" + CpyPrcCNCReq.BatchQty.ToString().Trim() + "','P','WIP',");
                                sb.Append("'Auto Req For Plan No: " + CpyPrcCNCReq.ProductCode + " and Prc No: " + PrcNo + "','1','1','1','KanBan')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                                #endregion
                                //Master Req

                                //Details Req
                                #region


                                int SrNoReq = 0;

                                for (int cntd = 0; cntd < dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows.Count; cntd++)
                                {
                                    SrNoReq = SrNoReq + 1;
                                    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@REQCode", strKanBan);
                                    cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                    cmd.Parameters.AddWithValue("@PartCode", dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows[cntd]["Partcode"].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Qty", double.Parse(dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@REQStatus", "P");
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();


                                }
                                #endregion
                                //Details Req
                            }



                            //DateTime.Now.ToString("yyyy-MM-dd")
                            //*********************User Acivity ***************************
                            cmd = new SqlCommand("insertLoginTransactionDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@EmpID", CpyPrcCNCReq.EmpCode);
                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                            cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                            cmd.Parameters.AddWithValue("@TransactionNo", strKanBan);
                            cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcCNCReq.PCCode.Substring(0, 2).Trim());
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            #endregion
                            //KanBan


                        }
                    }
                    #endregion
                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", CpyPrcCNCReq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "CNC Process");
                    cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcCNCReq.PCCode.Substring(0, 2).Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                   tran.Commit();
                    //tran.Rollback();
                    PrcNo = "ProcessCode=" + PrcNo + " For CNC  and Req No " + strReqCode + "," + strReqCodeCPYAssly + " For Fabrication To Canopy Assly Saved SuccessFully ";
                    return PrcNo;


                }
                else if (CpyPrcCNCReq.TkitId.Substring(0, 3) == "PSH")
                {
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedBack set EDt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where PFBCode='" + CpyPrcCNCReq.TkitId.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();



                    // //Not Required Part Cutting Skip
                    //sb.Remove(0, sb.Length);
                    //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,StageName)");
                    //sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + CpyPrcCNCReq.SheetPartcode.Trim() + "',");
                    //sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + CpyPrcCNCReq.ShWtperBatch + "','01.076',0,'0')");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();
                    // //Not Required Part Cutting Skip

                    int recCount = ComCon.CountChars(CpyPrcCNCReq.PrcDts, ",");
                    string[] strPrcDts = Regex.Split(CpyPrcCNCReq.PrcDts, ",");
                    int SrNo = 0;



                    String StrBendingPCCode_END = "0";
                    if (CpyPrcCNCReq.PCCode.Trim() == "01.009")
                    {
                        StrBendingPCCode_END = "01.002"; //Bending
                    }
                    else if (CpyPrcCNCReq.PCCode.Trim() == "03.061")
                    {
                        StrBendingPCCode_END = "03.004"; //Bending
                    }
                    else if (CpyPrcCNCReq.PCCode.Trim() == "28.013")
                    {
                        StrBendingPCCode_END = "28.014"; //Bending
                    }


                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {

                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");
                        #region
                        if (Dts[0].ToString().Trim() == "0000030103000001000")
                        {
                            //Commented by KB on 21/12/2023
                            //sb.Remove(0, sb.Length);
                            //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                            //sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                            //sb.Append("'" + CpyPrcCNCReq.TkitId.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcCNCReq.PCCode.Trim() + "',0)");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();
                        }
                        else
                        {
                            //Commented by KB on 21/12/2023
                            //sb.Remove(0, sb.Length);
                            //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                            //sb.Append(" values('" + CpyPrcCNCReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                            //sb.Append("'" + CpyPrcCNCReq.TkitId.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + StrBendingPCCode_END.Trim() + "',1)");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();
                        }
                        #endregion
                    }
                    //skip
                    #region

                    string cntSheet = "0";
                    cntSheet = ComCon.getTranName("select isnull(Count(PrcStatus),0) as PrcStatus from TurretKitForPrc where PrcStatus='P'  and CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CanopyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "'  ", "TurretKitForPrc", "PrcStatus", con, tran);
                    if (cntSheet == "0")
                    {


                        sb.Remove(0, sb.Length);
                        sb.Append("Update TurretKitForPrc set PartCutStatus='D'  where CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CanopyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDtsSub set CPPartCutQty='" + CpyPrcCNCReq.BatchQty + "' ,CPPartCutStatus='D'  where CPCode='" + CpyPrcCNCReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcCNCReq.ProductCode.Trim() + "'   ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }
                    #endregion
                    //Auto Part Cutting
                   tran.Commit();
                   // tran.Rollback();

                    PrcNo = "ProcessCode=" + CpyPrcCNCReq.TkitId.Trim() + " For CNC  End SuccessFully ";
                    return PrcNo;
                }


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

            return PrcNo;
        }

        //Old
        /*
        public string SubmitFabrication(CpyPrcFabRequest CpyPrcFabReq)
        {

            //int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;

            string PrcNo = "";
            try
            {
                if (CpyPrcFabReq.PFBCode.Substring(0, 3) == "NEW")
                {
                    string[] strMachineNo = Regex.Split(CpyPrcFabReq.MachineCodeSrNo, "-->");
                    ChkforStartCPY = getChkforStartCpy(CpyPrcFabReq.PCCode.Trim(), CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode);
                    ChkforStart = getChkforStart(CpyPrcFabReq.PCCode.Trim(), CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString());
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcFabReq.PCCode.Trim().Substring(0, 2), con, tran);

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,SupplierCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                    sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,SqftperUt,");
                    sb.Append("PartCode,ProcessQty,CompanyCode,PFBRate,PPWCode,Remark)");
                    sb.Append(" values('" + PrcNo.Trim() + "','" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "', ");
                    if (ChkforStart == true)
                    {
                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                    }
                    else if (ChkforStart == false)
                    {
                        //sb.Append("'" + ComCon.dateinyyyymmdd(GetPrevPrcTime(CpyPrcFabReq.PCCode, CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), con, tran)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                        sb.Append("'" + GetPrevPrcTime(CpyPrcFabReq.PCCode, CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), con, tran) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                    }
                    sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.OSSupplierCode.Trim() + "','" + CpyPrcFabReq.ProductCode.Trim() + "',");
                    sb.Append("'" + CpyPrcFabReq.PlanCode.Trim() + "','" + CpyPrcFabReq.BOMcode.Trim() + "',");
                    //
                    if (CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "1" || CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "0")
                    {
                        sb.Append("'" + ComCon.getTranName("select KitCode from Bomdetails where BOMCode='" + CpyPrcFabReq.BOMcode + "' and Kitcode Like '004%' and  substring(Kitcode,11,1) in ('4') and Partcode='" + CpyPrcFabReq.CpyKitcode.Trim() + "'", "TblNstPartCode", "KitCode", con, tran) + "',");
                    }
                    else if (CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "2" || CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "3")
                    {
                        sb.Append("'" + CpyPrcFabReq.CpyKitcode.Trim() + "',");
                    }
                    sb.Append("'" + CpyPrcFabReq.BatchQty + "','" + Math.Round(double.Parse(CpyPrcFabReq.BatchQty.ToString()) * double.Parse(CpyPrcFabReq.PWt.ToString()), 2) + "',");
                    sb.Append("'" + Math.Round(double.Parse(CpyPrcFabReq.BatchQty.ToString()) * double.Parse(CpyPrcFabReq.PSqft.ToString()), 2) + "',");
                    sb.Append("'" + CpyPrcFabReq.PWt + "','" + CpyPrcFabReq.PSqft + "',");
                    sb.Append("'" + CpyPrcFabReq.CpyKitcode.Trim() + "','" + CpyPrcFabReq.PrcQty + "','01',");
                    sb.Append("'" + CpyPrcFabReq.PFBRate + "','" + CpyPrcFabReq.EmpCode.Trim() + "','Nil')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();


                    //Action Taken File Attachment
                    #region
                    if (!string.IsNullOrEmpty(CpyPrcFabReq.AttachFileDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(CpyPrcFabReq.AttachFileDts, "@#@");
                        SrNoA = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNoA += 1;
                            DtsA = Regex.Split(StrSub.ToString().Trim(), "-->");

                            string FileName = PrcNo.ToString().Trim().Substring(4, 5).Trim() + PrcNo.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNoA) + Path.GetExtension(DtsA[1].ToString().Trim());
                            String StrMpath = ComCon.getMainFilePath("TempPrcFab") + "/" + FileName.ToString().Trim();
                            string StrTpath = "C:/TempERPFile/TempPrcFab/" + CpyPrcFabReq.EmpCode.Trim() + "/" + DtsA[1].ToString().Trim();

                            string StrTempPath = "C:/TempERPFile/TempPrcFab/" + CpyPrcFabReq.EmpCode.Trim();
                            if (Directory.Exists(StrTempPath))
                            {
                                Directory.GetAccessControl(StrTpath);
                                File.Copy(StrTpath, StrMpath);
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessFeedbackFiles");
                            sb.Append("(GroupPFBCode,SrNo,FileName)");
                            sb.Append(" VALUES('" + PrcNo.Trim() + "' ,'" + SrNoA + "','" + FileName.ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }
                    #endregion
                    //Action Taken File Attachment

                    //if (ChkforStart == false)
                    //{
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                    sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.CpyKitcode.Trim() + "',");
                    sb.Append("'" + PrcNo.Trim() + "', GetDate(),'" + CpyPrcFabReq.PrcQty.ToString().Trim() + "','01.007','1')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    // }

                    int recCount = ComCon.CountChars(CpyPrcFabReq.PrcDts, ",");
                    string[] strPrcDts = Regex.Split(CpyPrcFabReq.PrcDts, ",");
                    int SrNo = 0;

                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {

                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                        sb.Append("PFBRate,SaleRate,PLength,PWidth,PThickness,PLossWt,PCatagoryCode,WtPerUt,SqftPerUt)");
                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + Dts[0].Trim() + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[1].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[2].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[3].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[4].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[5].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[6].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[7].Trim()) + "',");
                        sb.Append("'" + Dts[8].Trim() + "','" + Dts[9].Trim() + "','" + Math.Round(double.Parse(Dts[10].Trim()), 2) + "','" + Math.Round(double.Parse(Dts[11].Trim()), 2) + "')");
                        //Partcode,KitQty,TotQty,PfbRate,Plen,Pwidth,PThk,PLossWt,WtPeruts,SqftPerUts,catCode
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();


                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                        sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcFabReq.PCCode.Trim() + "',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }

                    sb.Remove(0, sb.Length);
                    sb.Append("Update CanopyPlanDtsSub set CPFQty=CPFQty + '" + CpyPrcFabReq.PrcQty + "' where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    string cntPrcQty = "0";
                    cntPrcQty = ComCon.getTranName("select CPQty-CPFQty as BalQty from CanopyPlanDtsSub where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "'   ", "FabPrc", "BalQty", con, tran);
                    if (cntPrcQty == "0")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDtsSub set CPFStatus='D' where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    string cntBndStatus = "0";
                    cntBndStatus = ComCon.getTranName("select Count(CPFStatus) as CPFStatus from CanopyPlanDtsSub where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "'  and  CPFStatus='P'  ", "FabPrc", "CPFStatus", con, tran);
                    if (cntBndStatus == "0")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                        sb.Append(" values('" + CpyPrcFabReq.ProductCode.ToString().Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','01.007',");
                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcFabReq.BatchQty.ToString().Trim() + "',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                        sb.Append(" values('" + CpyPrcFabReq.ProductCode.ToString().Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','01.007',");
                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcFabReq.BatchQty.ToString().Trim() + "',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }

                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", CpyPrcFabReq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "Fabrication Process");
                    cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcFabReq.PCCode.Substring(0, 2).Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Prc Below 1000    
                    #region
                    string GetMaxRate = "0";
                    string PrcBelowRate = "";
                    GetMaxRate = ComCon.getTranName("Select Isnull(Max(Rate),0) as MRate From CanopyplandtsSub where CPCode='" + CpyPrcFabReq.PlanCode + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode + "' ", "tbl_ChkForMaxRate", "MRate", con, tran);
                    if (double.Parse(GetMaxRate.Trim()) == CpyPrcFabReq.Rate)
                    {
                        dsKitbelowRate = ComCon.procTranDS("select Partcode From CanopyPlanDtsSubBelowStdRate where CpyPartcode='" + CpyPrcFabReq.ProductCode.ToString().Trim() + "' and CPCode='" + CpyPrcFabReq.PlanCode + "' ", "tbl_KitbelowRate", con, tran);
                        if (dsKitbelowRate != null && dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count > 0)
                        {

                            for (int br = 0; br < dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count; br++)
                            {

                                // Mst Entry
                                #region
                                PrcBelowRate = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcFabReq.PCCode.Trim().Substring(0, 2), con, tran);
                                //dsDetails.Tables["tbl_RaiseReqDts"].Rows[i]["Partcode"].ToString().Trim()
                                sb.Remove(0, sb.Length);
                                sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                                sb.Append("PartCode,ProcessQty,CompanyCode,PFBRate,PPWCode,Remark)");
                                sb.Append(" values('" + PrcNo.Trim() + "','" + PrcBelowRate.Trim() + "','" + (PrcBelowRate.Substring(10, 8)) + "', ");
                                //sb.Append("'" + ComCon.dateinyyyymmdd(GetPrevPrcTime(CpyPrcFabReq.PCCode, CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), con, tran)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                 sb.Append("'" + GetPrevPrcTime(CpyPrcFabReq.PCCode, CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), con, tran) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.ProductCode.Trim() + "',");
                                sb.Append("'" + CpyPrcFabReq.PlanCode.Trim() + "','" + CpyPrcFabReq.BOMcode.Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "','" + CpyPrcFabReq.PrcQty + "','01', ");
                                sb.Append("'" + ComCon.getTranName("Select Isnull(Max(Rate),0) as Rate From ProfitcenterPLDetails where ProfitcenterCode='" + CpyPrcFabReq.PCCode + "' and Partcode='" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "' ", "tbl_PLRate", "Rate", con, tran) + "'");
                                sb.Append(", '" + CpyPrcFabReq.EmpCode + "','Nil')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                                sb.Remove(0, sb.Length);
                                sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                                sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "',");
                                sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + CpyPrcFabReq.PrcQty + "','01.007',1)");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                #endregion
                                // Mst Entry

                                //Dts Entry
                                #region

                                int ChkStk = 0;
                                dsKitbelowRatedts = ComCon.procTranDS("select Bd.Partcode,P.AliseName,Qty,Purrate,rate,Pwt,Psqft,Bd.Length,bd.Width,bd.Thickness,bd.LossWgt,Bd.categoryID, " +
                                   " (select Round(Isnull(Sum(Recqty) - sum(IssueQty), 0), 00) as Stk From ( select Sum(ReceivedQty) as Recqty, " +
                                  " 0.00 as IssueQty from stockwip where ToProfitcenterCode = '01.008' and StockType = '0' " +
                                 " and Partcode = Bd.Partcode and  ReceivedQty > 0   Union all " +
                                  " select 0.00 as Recqty, sum(IssueQty) as IssueQty from stockwip where FromProfitcenterCode = '01.008' and StockType = '0' " +
                                  "  and Partcode = Bd.Partcode and  IssueQty > 0) as stk) as Stock " +
                                  "From BOMDetails Bd Inner Join ProfitcenterPLdetails Pl On   Bd.KitCode=Pl.Partcode  Inner Join part P On Bd.Partcode=P.partcode " +
                                    " where Bd.BOMCode='" + CpyPrcFabReq.BOMcode + "' and Bd.KitCode='" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "' " +
                                    " and Pl.ProfitcenterCode='" + CpyPrcFabReq.PCCode + "'  ", "tbl_KitbelowRatedts", con, tran);
                                if (dsKitbelowRatedts != null && dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows.Count > 0)
                                {
                                    SrNo = 0;

                                    for (int brd = 0; brd < dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows.Count; brd++)

                                    {
                                        if ((Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) * CpyPrcFabReq.PrcQty) >
                                        Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Stock"].ToString().Trim()))
                                        {
                                            if (ChkStk == 0)
                                            {
                                                PrcNo = dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["AliseName"].ToString().Trim();
                                                ChkStk = 1;

                                            }
                                            else
                                            {
                                                PrcNo = PrcNo + "," + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["AliseName"].ToString().Trim();
                                            }
                                        }

                                        else if (ChkStk == 0)
                                        {
                                            SrNo += 1;
                                            sb.Remove(0, sb.Length);
                                            sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                                            sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt,PLength,PWidth,PThickness,PLossWt,PCatagoryCode)");
                                            sb.Append("values('" + PrcBelowRate.Trim() + "','" + SrNo + "',");
                                            sb.Append("'" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partcode"].ToString().Trim().Trim() + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) * CpyPrcFabReq.PrcQty + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Purrate"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["rate"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Pwt"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Psqft"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Length"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Width"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Thickness"].ToString().Trim()) + "',");
                                            sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["LossWgt"].ToString().Trim()) + "',");
                                            sb.Append("'" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["categoryID"].ToString().Trim() + "')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();


                                            sb.Remove(0, sb.Length);
                                            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                            sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partcode"].ToString().Trim() + "',");
                                            sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString()) * CpyPrcFabReq.PrcQty + "','" + CpyPrcFabReq.PCCode.Trim() + "',1)");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();



                                        }
                                    }
                                    if (ChkStk > 0)
                                    {
                                        PrcNo = "Insufficient Stock For Part(BR): " + PrcNo;
                                        return PrcNo;
                                    }
                                }
                                #endregion
                                //Dts Entry
                            }


                        }
                    }
                    #endregion
                    //Prc Below 1000    

                    tran.Commit();
                    // tran.Rollback();
                    PrcNo = "ProcessCode=" + PrcNo + " For Fabrication  Saved SuccessFully ";
                }
                else if (CpyPrcFabReq.PFBCode.Substring(0, 3) == "PSH")
                {
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedBack set EDt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where PFBCode='" + CpyPrcFabReq.PFBCode.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //sb.Remove(0, sb.Length);
                    //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                    //sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.CpyKitcode.Trim() + "',");
                    //sb.Append("'" + CpyPrcFabReq.PFBCode.Trim() + "',GetDate(),'" + CpyPrcFabReq.PrcQty.ToString().Trim() + "','01.007',1)");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();


                    tran.Commit();
                    //tran.Rollback();
                    PrcNo = "ProcessCode=" + CpyPrcFabReq.PFBCode.Trim() + " For Fabrication  End SuccessFully ";
                }
                return PrcNo;
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
        //Old
        //public string SubmitFabrication(CpyPrcFabRequest CpyPrcFabReq)
        //{
        //    //int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;
        //    string PrcNo = "";
        //    try
        //    {
        //        if (CpyPrcFabReq.PFBCode.Substring(0, 3) == "NEW")
        //        {
        //            if (CpyPrcFabReq.OSSupplierCode == "0")
        //            {
        //                PrcNo = "Pl Select Supplier !!! ";
        //                return PrcNo;
        //            }

        //            string[] strMachineNo = Regex.Split(CpyPrcFabReq.MachineCodeSrNo, "-->");
        //            ChkforStartCPY = getChkforStartCpy(CpyPrcFabReq.PCCode.Trim(), CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode);
        //            ChkforStart = getChkforStart(CpyPrcFabReq.PCCode.Trim(), CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString());
        //            if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
        //            tran = con.BeginTransaction();

        //            PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcFabReq.PCCode.Trim().Substring(0, 2), con, tran);
        //            string NstPart = "0";
        //            string NstWt = "0";
        //            string CpyStageType = "Line1";
        //            if (CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "1" || CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "0")
        //            {
        //                NstPart = ComCon.getTranName("select KitCode from Bomdetails where BOMCode='" + CpyPrcFabReq.BOMcode + "' and Kitcode Like '004%' and  substring(Kitcode,11,1) in ('4') and Partcode='" + CpyPrcFabReq.CpyKitcode.Trim() + "'", "TblNstPartCode", "KitCode", con, tran);
        //            }
        //            else if (CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "2" || CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "3")
        //            {
        //                NstPart = CpyPrcFabReq.CpyKitcode.Trim();
        //                CpyStageType = "Line2";
        //            }
        //            NstWt = ComCon.getTranName("select Pwt from ProfitcenterPlDetails where ProfitcenterCode='01.008' and Partcode='" + NstPart + "'", "TblPartWt", "Pwt", con, tran);
        //            SqlCommand cmd = new SqlCommand();
        //            sb.Remove(0, sb.Length);
        //            sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,SupplierCode,ProductCode,CanopyPlanCode,TurretKitCode,");
        //            sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,SqftperUt,CpyStageType,");
        //            sb.Append("PartCode,ProcessQty,CompanyCode,PFBRate,PPWCode,Remark)");
        //            //If (ChkforGrpCode==true)
        //            sb.Append(" values('" + PrcNo.Trim() + "', ");
        //            sb.Append("'" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "',");
        //            if (ChkforStart == true)
        //            {
        //                sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
        //            }
        //            else if (ChkforStart == false)
        //            {
        //                // sb.Append("'" + ComCon.dateinyyyymmdd(GetPrevPrcTime(CpyPrcFabReq.PCCode, CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), con, tran)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //                sb.Append("'" + GetPrevPrcTime(CpyPrcFabReq.PCCode, CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), con, tran) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //            }
        //            sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.OSSupplierCode.Trim() + "','" + CpyPrcFabReq.ProductCode.Trim() + "',");
        //            sb.Append("'" + CpyPrcFabReq.PlanCode.Trim() + "','" + CpyPrcFabReq.BOMcode.Trim() + "',");
        //            sb.Append("'" + NstPart + "','" + CpyPrcFabReq.BatchQty + "',");
        //            sb.Append("'" + NstWt.Trim() + "','0',");
        //            sb.Append("'" + CpyPrcFabReq.PWt + "','" + CpyPrcFabReq.PSqft + "','" + CpyStageType.Trim() + "',");
        //            sb.Append("'" + CpyPrcFabReq.CpyKitcode.Trim() + "','" + CpyPrcFabReq.PrcQty + "','01',");
        //            sb.Append("'" + CpyPrcFabReq.PFBRate + "','" + CpyPrcFabReq.EmpCode.Trim() + "','Nil')");
        //            cmd = new SqlCommand(sb.ToString(), con);
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();


        //            //Action Taken File Attachment
        //            #region
        //            if (!string.IsNullOrEmpty(CpyPrcFabReq.AttachFileDts.ToString().Trim()))
        //            {
        //                strPlanDts = null;
        //                strPlanDts = Regex.Split(CpyPrcFabReq.AttachFileDts, "@#@");
        //                SrNoA = 0;
        //                foreach (String StrSub in strPlanDts)
        //                {
        //                    SrNoA += 1;
        //                    DtsA = Regex.Split(StrSub.ToString().Trim(), "-->");

        //                    string FileName = PrcNo.ToString().Trim().Substring(4, 5).Trim() + PrcNo.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNoA) + Path.GetExtension(DtsA[1].ToString().Trim());
        //                    String StrMpath = ComCon.getMainFilePath("TempPrcFab") + "/" + FileName.ToString().Trim();
        //                    string StrTpath = "C:/TempERPFile/TempPrcFab/" + CpyPrcFabReq.EmpCode.Trim() + "/" + DtsA[1].ToString().Trim();

        //                    string StrTempPath = "C:/TempERPFile/TempPrcFab/" + CpyPrcFabReq.EmpCode.Trim();
        //                    if (Directory.Exists(StrTempPath))
        //                    {
        //                        Directory.GetAccessControl(StrTpath);
        //                        File.Copy(StrTpath, StrMpath);
        //                    }

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO ProcessFeedbackFiles");
        //                    sb.Append("(GroupPFBCode,SrNo,FileName)");
        //                    sb.Append(" VALUES('" + PrcNo.Trim() + "' ,'" + SrNoA + "','" + FileName.ToString().Trim() + "')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();
        //                }
        //            }
        //            #endregion
        //            //Action Taken File Attachment

        //            //if (ChkforStart == false)
        //            //{
        //            sb.Remove(0, sb.Length);
        //            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
        //            sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.CpyKitcode.Trim() + "',");
        //            sb.Append("'" + PrcNo.Trim() + "', GetDate(),'" + CpyPrcFabReq.PrcQty.ToString().Trim() + "','01.007','1')");
        //            cmd = new SqlCommand(sb.ToString(), con);
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();
        //            // }

        //            int recCount = ComCon.CountChars(CpyPrcFabReq.PrcDts, ",");
        //            string[] strPrcDts = Regex.Split(CpyPrcFabReq.PrcDts, ",");
        //            int SrNo = 0;

        //            for (int cSub = 0; cSub <= recCount; cSub++)
        //            {
        //                SrNo += 1;
        //                string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

        //                sb.Remove(0, sb.Length);
        //                sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
        //                sb.Append("PFBRate,SaleRate,PLength,PWidth,PThickness,PLossWt,PCatagoryCode,WtPerUt,SqftPerUt)");
        //                sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
        //                sb.Append("'" + Dts[0].Trim() + "',");
        //                sb.Append("'" + Convert.ToDouble(Dts[1].Trim()) + "',");
        //                sb.Append("'" + Convert.ToDouble(Dts[2].Trim()) + "',");
        //                sb.Append("'" + Convert.ToDouble(Dts[3].Trim()) + "',");
        //                sb.Append("'" + Convert.ToDouble(Dts[4].Trim()) + "',");
        //                sb.Append("'" + Convert.ToDouble(Dts[5].Trim()) + "',");
        //                sb.Append("'" + Convert.ToDouble(Dts[6].Trim()) + "',");
        //                sb.Append("'" + Convert.ToDouble(Dts[7].Trim()) + "',");
        //                sb.Append("'" + Dts[8].Trim() + "','" + Dts[9].Trim() + "','" + Math.Round(double.Parse(Dts[10].Trim()), 2) + "','" + Math.Round(double.Parse(Dts[11].Trim()), 2) + "')");
        //                //Partcode,KitQty,TotQty,PfbRate,Plen,Pwidth,PThk,PLossWt,WtPeruts,SqftPerUts,catCode
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.Transaction = tran;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();

        //                sb.Remove(0, sb.Length);
        //                sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
        //                sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
        //                sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcFabReq.PCCode.Trim() + "',0)");
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.Transaction = tran;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();
        //            }

        //            sb.Remove(0, sb.Length);
        //            sb.Append("Update CanopyPlanDtsSub set CPFQty=CPFQty + '" + CpyPrcFabReq.PrcQty + "' where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "' ");
        //            cmd = new SqlCommand(sb.ToString(), con);
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();

        //            string cntPrcQty = "0";
        //            cntPrcQty = ComCon.getTranName("select CPQty-CPFQty as BalQty from CanopyPlanDtsSub where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "'   ", "FabPrc", "BalQty", con, tran);
        //            if (cntPrcQty == "0")
        //            {
        //                sb.Remove(0, sb.Length);
        //                sb.Append("Update CanopyPlanDtsSub set CPFStatus='D' where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "' ");
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.Transaction = tran;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();
        //            }

        //            string cntBndStatus = "0";
        //            cntBndStatus = ComCon.getTranName("select Count(CPFStatus) as CPFStatus from CanopyPlanDtsSub where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "'  and  CPFStatus='P'  ", "FabPrc", "CPFStatus", con, tran);
        //            if (cntBndStatus == "0")
        //            {
        //                sb.Remove(0, sb.Length);
        //                sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
        //                sb.Append(" values('" + CpyPrcFabReq.ProductCode.ToString().Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','01.007',");
        //                sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcFabReq.BatchQty.ToString().Trim() + "',0)");
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.Transaction = tran;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();

        //                sb.Remove(0, sb.Length);
        //                sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
        //                sb.Append(" values('" + CpyPrcFabReq.ProductCode.ToString().Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','01.007',");
        //                sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + CpyPrcFabReq.BatchQty.ToString().Trim() + "',0)");
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.Transaction = tran;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();
        //            }

        //            //****************User Acivity****************
        //            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
        //            cmd.Parameters.AddWithValue("@EmpID", CpyPrcFabReq.EmpCode.Trim());
        //            cmd.Parameters.AddWithValue("@TransactionType", "S");
        //            cmd.Parameters.AddWithValue("@TransactionFrom", "Fabrication Process");
        //            cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
        //            cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcFabReq.PCCode.Substring(0, 2).Trim());
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();

        //            //Prc Below 1000    
        //            #region
        //            string GetMaxRate = "0";
        //            string PrcBelowRate = "";
        //            GetMaxRate = ComCon.getTranName("Select Isnull(Max(Rate),0) as MRate From CanopyplandtsSub where CPCode='" + CpyPrcFabReq.PlanCode + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode + "' ", "tbl_ChkForMaxRate", "MRate", con, tran);
        //            if (double.Parse(GetMaxRate.Trim()) == CpyPrcFabReq.Rate)
        //            {
        //                dsKitbelowRate = ComCon.procTranDS("select Pf.Partcode,Pl.rate,Pl.PurRate,Pwt,Psqft From CanopyPlanDtsSubBelowStdRate Pf Inner Join ProfitcenterPldetails Pl on  Pf.Partcode = Pl.partcode where CpyPartcode = '" + CpyPrcFabReq.ProductCode.ToString().Trim() + "' and CPCode = '" + CpyPrcFabReq.PlanCode + "' and ProfitcenterCode = '01.008' ", "tbl_KitbelowRate", con, tran);
        //                if (dsKitbelowRate != null && dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count > 0)
        //                {
        //                    for (int br = 0; br < dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count; br++)
        //                    {
        //                        // Mst Entry
        //                        #region
        //                        PrcBelowRate = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcFabReq.PCCode.Trim().Substring(0, 2), con, tran);
        //                        //dsDetails.Tables["tbl_RaiseReqDts"].Rows[i]["Partcode"].ToString().Trim()
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,CpyStageType,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,supplierCode,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
        //                        sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,SqftperUt,");
        //                        sb.Append("PartCode,ProcessQty,CompanyCode,PFBRate,PPWCode,Remark)");
        //                        sb.Append(" values('" + PrcNo.Trim() + "','" + PrcBelowRate.Trim() + "','" + CpyStageType + "','" + (PrcBelowRate.Substring(10, 8)) + "', ");
        //                        // sb.Append("'" + ComCon.dateinyyyymmdd(GetPrevPrcTime(CpyPrcFabReq.PCCode, CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), con, tran)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //                        sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.ProductCode.Trim() + "',");
        //                        sb.Append("'" + CpyPrcFabReq.PlanCode.Trim() + "','" + CpyPrcFabReq.BOMcode.Trim() + "',");
        //                        sb.Append("'" + NstPart + "','" + CpyPrcFabReq.BatchQty + "','" + NstWt.Trim() + "','0',  '" + double.Parse(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Pwt"].ToString().Trim()) + "',  '" + double.Parse(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["PSqft"].ToString().Trim()) + "',");
        //                        sb.Append("'" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "','" + CpyPrcFabReq.PrcQty + "','01', ");
        //                        sb.Append("'" + ComCon.getTranName("Select Isnull(Max(Rate),0) as Rate From ProfitcenterPLDetails where ProfitcenterCode='" + CpyPrcFabReq.PCCode + "' and Partcode='" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "' ", "tbl_PLRate", "Rate", con, tran) + "'");
        //                        sb.Append(", '" + CpyPrcFabReq.EmpCode + "','Nil')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();


        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
        //                        sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "',");
        //                        sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + CpyPrcFabReq.PrcQty + "','01.007',1)");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        #endregion
        //                        // Mst Entry

        //                        //Dts Entry
        //                        #region
        //                        int ChkStk = 0;
        //                        dsKitbelowRatedts = ComCon.procTranDS("select Bd.Partcode,P.PartDesc as AliseName,Qty,Purrate,rate,Pwt,Psqft,Bd.Length,bd.Width,bd.Thickness,bd.LossWgt,Bd.categoryID, " +
        //                           " (select Round(Isnull(Sum(Recqty) - sum(IssueQty), 0), 00) as Stk From ( select Sum(ReceivedQty) as Recqty, " +
        //                          " 0.00 as IssueQty from stockwip where ToProfitcenterCode = '01.008' and StockType = '0' " +
        //                         " and Partcode = Bd.Partcode and  ReceivedQty > 0   Union all " +
        //                          " select 0.00 as Recqty, sum(IssueQty) as IssueQty from stockwip where FromProfitcenterCode = '01.008' and StockType = '0' " +
        //                          "  and Partcode = Bd.Partcode and  IssueQty > 0) as stk) as Stock " +
        //                          "From BOMDetails Bd Inner Join ProfitcenterPLdetails Pl On   Bd.KitCode=Pl.Partcode  Inner Join part P On Bd.Partcode=P.partcode " +
        //                            " where Bd.BOMCode='" + CpyPrcFabReq.BOMcode + "' and Bd.KitCode='" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "' " +
        //                            " and Pl.ProfitcenterCode='" + CpyPrcFabReq.PCCode + "'  ", "tbl_KitbelowRatedts", con, tran);
        //                        if (dsKitbelowRatedts != null && dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows.Count > 0)
        //                        {
        //                            SrNo = 0;

        //                            for (int brd = 0; brd < dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows.Count; brd++)
        //                            {
        //                                if ((Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) * CpyPrcFabReq.PrcQty) >
        //                                Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Stock"].ToString().Trim()))
        //                                {
        //                                    if (ChkStk == 0)
        //                                    {
        //                                        PrcNo = dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["AliseName"].ToString().Trim();
        //                                        ChkStk = 1;

        //                                    }
        //                                    else
        //                                    {
        //                                        PrcNo = PrcNo + "," + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["AliseName"].ToString().Trim();
        //                                    }
        //                                }

        //                                if (ChkStk == 0)
        //                                {
        //                                    SrNo += 1;
        //                                    sb.Remove(0, sb.Length);
        //                                    sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
        //                                    sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt,PLength,PWidth,PThickness,PLossWt,PCatagoryCode)");
        //                                    sb.Append("values('" + PrcBelowRate.Trim() + "','" + SrNo + "',");
        //                                    sb.Append("'" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partcode"].ToString().Trim().Trim() + "',");
        //                                    sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) + "',");
        //                                    sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) * CpyPrcFabReq.PrcQty + "',");
        //                                    sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Purrate"].ToString().Trim()) + "',");
        //                                    sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["rate"].ToString().Trim()) + "',");
        //                                    sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Pwt"].ToString().Trim()) + "',");
        //                                    sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Psqft"].ToString().Trim()) + "',");
        //                                    sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Length"].ToString().Trim()) + "',");
        //                                    sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Width"].ToString().Trim()) + "',");
        //                                    sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Thickness"].ToString().Trim()) + "',");
        //                                    sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["LossWgt"].ToString().Trim()) + "',");
        //                                    sb.Append("'" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["categoryID"].ToString().Trim() + "')");
        //                                    cmd = new SqlCommand(sb.ToString(), con);
        //                                    cmd.Transaction = tran;
        //                                    cmd.ExecuteNonQuery();
        //                                    cmd.Dispose();

        //                                    sb.Remove(0, sb.Length);
        //                                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
        //                                    sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partcode"].ToString().Trim() + "',");
        //                                    sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString()) * CpyPrcFabReq.PrcQty + "','" + CpyPrcFabReq.PCCode.Trim() + "',0)");
        //                                    cmd = new SqlCommand(sb.ToString(), con);
        //                                    cmd.Transaction = tran;
        //                                    cmd.ExecuteNonQuery();
        //                                    cmd.Dispose();

        //                                }
        //                            }
        //                            if (ChkStk > 0)
        //                            {
        //                                PrcNo = "Insufficient Stock For Part(BR): " + PrcNo;
        //                                return PrcNo;
        //                            }
        //                        }
        //                        #endregion
        //                        //Dts Entry
        //                    }
        //                }
        //            }
        //            #endregion
        //            //Prc Below 1000    

        //            tran.Commit();
        //            //tran.Rollback();
        //            PrcNo = "ProcessCode=" + PrcNo + " For Fabrication  Saved SuccessFully ";
        //        }
        //        else if (CpyPrcFabReq.PFBCode.Substring(0, 3) == "PSH")
        //        {
        //            if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
        //            tran = con.BeginTransaction();

        //            SqlCommand cmd = new SqlCommand();
        //            sb.Remove(0, sb.Length);
        //            sb.Append("Update ProcessFeedBack set EDt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where PFBCode='" + CpyPrcFabReq.PFBCode.Trim() + "' ");
        //            cmd = new SqlCommand(sb.ToString(), con);
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();

        //            //sb.Remove(0, sb.Length);
        //            //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
        //            //sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.CpyKitcode.Trim() + "',");
        //            //sb.Append("'" + CpyPrcFabReq.PFBCode.Trim() + "',GetDate(),'" + CpyPrcFabReq.PrcQty.ToString().Trim() + "','01.007',1)");
        //            //cmd = new SqlCommand(sb.ToString(), con);
        //            //cmd.Transaction = tran;
        //            //cmd.ExecuteNonQuery();
        //            //cmd.Dispose();

        //            tran.Commit();
        //            //tran.Rollback();
        //            PrcNo = "ProcessCode=" + CpyPrcFabReq.PFBCode.Trim() + " For Fabrication  End SuccessFully ";
        //        }
        //        return PrcNo;
        //    }
        //    catch (Exception ex)
        //    {
        //        tran.Rollback();
        //        return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //}

        // Old
        /*
        public string SubmitPowderCoating(CpyPrcPCRequest CpyPrcPCReq)
        {

            //int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;

            string PrcNo = "";
            string Trans = "";
            try
            {
                int recCount = ComCon.CountChars(CpyPrcPCReq.PrcDts, ",");
                string[] strPrcDts = Regex.Split(CpyPrcPCReq.PrcDts, ",");
                int SrNo = 0;
                string GrpPfbCode = "";
                string PCkitFlag = "No";
                string AllPrcCode = "";
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                string[] strMachineNo = Regex.Split(CpyPrcPCReq.MachineCodeSrNo, "-->");
                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo = 0;
                    string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                    if (Dts[10].Substring(0, 3) == "NEW")
                    {
                        Trans = "Start";
                        //Master Entry
                        #region
                        PrcNo = "";

                        PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcPCReq.PCCode.Trim().Substring(0, 2), con, tran);
                        if (cSub == 0)
                        {
                            GrpPfbCode = PrcNo;
                            AllPrcCode = PrcNo;
                        }
                        else
                        {
                            AllPrcCode = AllPrcCode + "," + PrcNo;
                        }
                        //SqlCommand cmd = new SqlCommand();
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,SupplierCode,CanopyPlanCode,ProductCode,TurretKitCode,");
                        sb.Append("PartCode,NestingForCode,NestingForQty,SqftPerUt,WtPerUt,PFBRate,ProcessQty,NstWtPerUt,NstSqftPerUt,PPWCode,CompanyCode,Remark)");
                        sb.Append(" values('" + GrpPfbCode.Trim() + "','" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "', ");
                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                        sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcPCReq.PCCode.Trim() + "','" + CpyPrcPCReq.SupplierCode.Trim() + "',");
                        sb.Append("'" + Dts[0].Trim() + "','" + Dts[1].Trim() + "','" + Dts[2].Trim() + "','" + Dts[3].Trim() + "','" + Dts[3].Trim() + "','" + Dts[4].Trim() + "', ");
                        sb.Append("'" + Dts[5].Trim() + "','" + Dts[6].Trim() + "','" + Dts[7].Trim() + "','" + Dts[9].Trim() + "',");
                        sb.Append("'" + double.Parse(Dts[6].Trim()) * double.Parse(Dts[9].Trim()) + "','" + double.Parse(Dts[5].Trim()) * double.Parse(Dts[9].Trim()) + "',");
                        sb.Append("'" + CpyPrcPCReq.EmpCode.Trim() + "','01','Nil')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                        sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + Dts[3].ToString().Trim() + "',");
                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[9].ToString().Trim()) + "','" + CpyPrcPCReq.PCCode.Trim() + "',1)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode, ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                        sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + Dts[3].ToString().Trim() + "',");
                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[9].ToString().Trim()) + "','01.005',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        #endregion
                        //Master Entry

                        //Details Entry
                        #region
                        SrNo = +1; ;
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                        sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt)");
                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + Dts[3].Trim() + "',");
                        sb.Append("'1','" + Convert.ToDouble(Dts[9].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[8].Trim()) + "',"); //PurRate
                        sb.Append("'" + Convert.ToDouble(Dts[7].Trim()) + "',"); //SaleRate
                        sb.Append("'" + Convert.ToDouble(Dts[6].Trim()) + "',");//WtPerUt
                        sb.Append("'" + Convert.ToDouble(Dts[5].Trim()) + "')");//Sqft

                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //PC Asskit and KitBelow 1000
                        #region
                        string GetMaxRate = "0";
                        string PrcBelowRate = "";
                        PCkitFlag = "No";
                        GetMaxRate = ComCon.getTranName("Select Isnull(Max(Rate),0) as MRate From CanopyplandtsSub where CPCode='" + Dts[0].Trim() + "' and CpyPartcode='" + Dts[1].Trim() + "' ", "tbl_ChkForMaxRate", "MRate", con, tran);
                        if (Math.Round(double.Parse(GetMaxRate.Trim()), 0) == Math.Round(double.Parse(Dts[7].Trim()), 0))
                        // if (Dts[3].Trim().Substring(0, 3) == "004" && Dts[3].Trim().Substring(11, 3) == "120")
                        {
                            //PC Asskit
                            #region
                            PCkitFlag = "Yes";
                            dsDetailsSub = ComCon.procTranDS("Exec GetPCKit '" + Dts[2].Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "' ", "tbl_PCKit", con, tran);

                            if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_PCKit"].Rows.Count > 0)
                            {

                                for (int k = 0; k < dsDetailsSub.Tables["tbl_PCKit"].Rows.Count; k++)
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,SaleRate)");
                                    sb.Append("values('" + PrcNo.Trim() + "','" + (k + 2) + "',");
                                    sb.Append("'" + dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["PartCode"].ToString().Trim() + "',");
                                    sb.Append("'" + dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["Qty"].ToString().Trim() + "',");
                                    sb.Append("'" + double.Parse(dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["Qty"].ToString().Trim()) * double.Parse(Dts[9].Trim()) + "',");
                                    sb.Append("'" + dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["SuppRate"].ToString().Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();


                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                    sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["PartCode"].ToString().Trim() + "',");
                                    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["Qty"].ToString().Trim()) * double.Parse(Dts[9].Trim()) + "','" + CpyPrcPCReq.PCCode.Trim() + "',1)");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();




                                }
                            }


                            #endregion
                            //PC Asskit

                            //KitBelow 1000
                            #region
                            dsKitbelowRate = ComCon.procTranDS("select Partcode,(select Convert(nvarchar(10),PurRate)+'-->'+Convert(nvarchar(10),Rate)+'-->'+Convert(nvarchar(10),PWt)+'-->'+Convert(nvarchar(10),PSqft) " +
                            " from ProfitcenterPLDetails where  ProfitcenterCode = '01.007' and Partcode=cd.partcode) as PartDts ," +
                            " (select Round(Isnull(Sum(Recqty) - sum(IssueQty), 0), 00) as Stk From ( select Sum(ReceivedQty) as Recqty, " +
                            " 0.00 as IssueQty from stockwip where ToProfitcenterCode = '01.007' and StockType = '1' " +
                            " and Partcode = cd.Partcode and  ReceivedQty > 0   Union all " +
                            " select 0.00 as Recqty, sum(IssueQty) as IssueQty from stockwip where FromProfitcenterCode = '01.007' and StockType = '1' " +
                            "  and Partcode = cd.Partcode and  IssueQty > 0) as stk) as Stock " +
                            "From CanopyPlanDtsSubBelowStdRate cd where CpyPartcode='" + Dts[1].Trim() + "' and CPCode='" + Dts[0].Trim() + "' ", "tbl_KitbelowRate", con, tran);
                            if (dsKitbelowRate != null && dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count > 0)
                            {
                                int ChkStk = 0;
                                for (int br = 0; br < dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count; br++)
                                {
                                    string[] DtsBR = Regex.Split(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["PartDts"].ToString().Trim(), "-->");
                                    // Mst Entry
                                    #region
                                    if (Convert.ToDouble(Dts[9].Trim()) >
                                    Convert.ToDouble(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Stock"].ToString().Trim()))
                                    {
                                        if (ChkStk == 0)
                                        {
                                            PrcNo = dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["AliseName"].ToString().Trim();
                                            ChkStk = 1;

                                        }
                                        else
                                        {
                                            PrcNo = PrcNo + "," + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["AliseName"].ToString().Trim();
                                        }
                                    }

                                    else if (ChkStk == 0)
                                    {


                                        PrcBelowRate = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcPCReq.PCCode.Trim().Substring(0, 2), con, tran);
                                        //dsDetails.Tables["tbl_RaiseReqDts"].Rows[i]["Partcode"].ToString().Trim()
                                        sb.Remove(0, sb.Length);
                                        sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,CpyStageType,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,SupplierCode,CanopyPlanCode,ProductCode,TurretKitCode,");
                                        sb.Append("PartCode,NestingForCode,NestingForQty,SqftPerUt,WtPerUt,PFBRate,ProcessQty,NstWtPerUt,NstSqftPerUt,PPWCode,CompanyCode,Remark) ");
                                        sb.Append(" values('" + GrpPfbCode.Trim() + "','" + PrcBelowRate.Trim() + "','" + PrcNo.Trim() + "','" + (PrcBelowRate.Substring(10, 8)) + "', ");
                                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                                        sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcPCReq.PCCode.Trim() + "','" + CpyPrcPCReq.SupplierCode.Trim() + "',");
                                        sb.Append("'" + Dts[0].Trim() + "','" + Dts[1].Trim() + "','" + Dts[2].Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "','" + Dts[4].Trim() + "', ");
                                        sb.Append("'" + DtsBR[3].Trim() + "','" + DtsBR[2].Trim() + "','" + DtsBR[1].Trim() + "','" + Dts[9].Trim() + "',");
                                        sb.Append("'" + double.Parse(DtsBR[2].Trim()) * double.Parse(Dts[9].Trim()) + "','" + double.Parse(DtsBR[3].Trim()) * double.Parse(Dts[9].Trim()) + "',");
                                        sb.Append("'" + CpyPrcPCReq.EmpCode.Trim() + "','01','Nil')");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        sb.Remove(0, sb.Length);
                                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                        sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "',");
                                        sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + Dts[9].Trim() + "','01.007',1)");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        sb.Remove(0, sb.Length);
                                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                                        sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "',");
                                        sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + Dts[9].Trim() + "','01.005',0)");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        #endregion
                                        // Mst Entry

                                        // Dts Entry
                                        #region

                                        sb.Remove(0, sb.Length);
                                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                                        sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt)");
                                        sb.Append("values('" + PrcBelowRate.Trim() + "','" + SrNo + "',");
                                        sb.Append("'" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "',");
                                        sb.Append("'1',");
                                        sb.Append("'" + Convert.ToDouble(Dts[9].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[0].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[1].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[2].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[3].Trim()) + "')");

                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                        #endregion
                                        // Dts Entry

                                    }
                                }

                                if (ChkStk > 0)
                                {
                                    PrcNo = "Insufficient Stock For Part(BR): " + PrcNo;
                                    return PrcNo;
                                }

                            }
                            #endregion
                            //KitBelow 1000
                        }
                        #endregion
                        //PC Asskit and KitBelow 1000

                        #endregion
                        //Details Entry

                        if (PCkitFlag == "Yes")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update ProcessFeedBack set CanopyCode='PCKit' where PFBCode='" + PrcNo.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDtsSub set CPPCQty=CPPCQty + '" + double.Parse(Dts[9].ToString().Trim()) + "' where CPCode='" + Dts[0].ToString().Trim() + "' and CpyPartcode='" + Dts[1].ToString().Trim() + "' and Partcode='" + Dts[3].ToString().Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();


                        string cntPrcQty = "0";
                        cntPrcQty = ComCon.getTranName("select CPQty-CPPCQty as BalQty from CanopyPlanDtsSub where  CPCode='" + Dts[0].ToString().Trim() + "' and CpyPartcode='" + Dts[1].ToString().Trim() + "' and Partcode='" + Dts[3].ToString().Trim() + "'  ", "PCPrc", "BalQty", con, tran);
                        if (cntPrcQty == "0")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update CanopyPlanDtsSub set CPPCStatus='D' where CPCode='" + Dts[0].ToString().Trim() + "' and CpyPartcode='" + Dts[1].ToString().Trim() + "' and Partcode='" + Dts[3].ToString().Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }


                        string cntPCStatus = "0";
                        cntPCStatus = ComCon.getTranName("select Count(CPPCStatus) as CPPCStatus from CanopyPlanDtsSub where CPCode='" + Dts[0].ToString().Trim() + "' and CpyPartcode='" + Dts[1].ToString().Trim() + "' and  CPPCStatus='P'  ", "BendingPrc", "CPPCStatus", con, tran);
                        if (cntPCStatus == "0")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                            sb.Append(" values('" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "','01.007',");
                            sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Dts[9].ToString().Trim() + "',0)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                            sb.Append(" values('" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "','01.005',");
                            sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Dts[9].ToString().Trim() + "',0)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }


                        //****************User Acivity****************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", CpyPrcPCReq.EmpCode.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "PowderCoating Process");
                        cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcPCReq.PCCode.Substring(0, 2).Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }
                    else if (Dts[10].Substring(0, 3) == "PSH")
                    {
                        Trans = "End";

                        if (cSub == 0)
                        {

                            AllPrcCode = Dts[10].Trim();
                        }
                        else
                        {
                            AllPrcCode = AllPrcCode + "," + Dts[10].Trim();
                        }


                        //SqlCommand cmd = new SqlCommand();
                        sb.Remove(0, sb.Length);
                        sb.Append("Update ProcessFeedBack set EDt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where PFBCode='" + Dts[10].Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        GrpPfbCode = "";
                        GrpPfbCode = ComCon.getTranName("select GroupPfbCode   from ProcessFeedBack where PFBCode='" + Dts[10].Trim() + "' group By GroupPfbCode ", "TblPCPrc", "GroupPfbCode", con, tran);

                    }

                }
                dsDetailsSub.Clear();
                //For stk Matching
                #region
                if (Trans == "Start")
                {
                    dsDetailsSub = ComCon.procTranDS("Exec GetPrcDtsForStk '" + GrpPfbCode + "'", "tbl_PCKitForStk", con, tran);

                    if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_PCKitForStk"].Rows.Count > 0)
                    {
                        int stk = 0;
                        for (int s = 0; s < dsDetailsSub.Tables["tbl_PCKitForStk"].Rows.Count; s++)
                        {

                            if (double.Parse(dsDetailsSub.Tables["tbl_PCKitForStk"].Rows[s]["PrcQty"].ToString().Trim()) >
                                (double.Parse(dsDetailsSub.Tables["tbl_PCKitForStk"].Rows[s]["Stock"].ToString().Trim()) + double.Parse(dsDetailsSub.Tables["tbl_PCKitForStk"].Rows[s]["PrcQty"].ToString().Trim())))
                            {

                                if (stk == 0)
                                {
                                    AllPrcCode = dsDetailsSub.Tables["tbl_PCKitForStk"].Rows[s]["Partdesc"].ToString().Trim();
                                    stk = 1;
                                }
                                else if (stk > 0)
                                {
                                    AllPrcCode = AllPrcCode + "," + dsDetailsSub.Tables["tbl_PCKitForStk"].Rows[s]["Partdesc"].ToString().Trim();
                                }
                            }

                        }
                        if (stk > 0)
                        {
                            AllPrcCode = "In sufficient Stock For Part: " + AllPrcCode;
                            return AllPrcCode;

                        }
                    }
                }
                #endregion
                //For stk Matching

                //Action Taken File Attachment
                #region
                if (Trans == "End")
                {
                    if (!string.IsNullOrEmpty(CpyPrcPCReq.AttachFileDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(CpyPrcPCReq.AttachFileDts, "@#@");
                        SrNoA = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNoA += 1;
                            DtsA = Regex.Split(StrSub.ToString().Trim(), "-->");

                            string FileName = GrpPfbCode.ToString().Trim().Substring(4, 5).Trim() + GrpPfbCode.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNoA) + Path.GetExtension(DtsA[1].ToString().Trim());
                            String StrMpath = ComCon.getMainFilePath("TempPrcPC") + "/" + FileName.ToString().Trim();
                            string StrTpath = "C:/TempERPFile/TempPrcPC/" + CpyPrcPCReq.EmpCode.Trim() + "/" + DtsA[1].ToString().Trim();

                            string StrTempPath = "C:/TempERPFile/TempPrcPC/" + CpyPrcPCReq.EmpCode.Trim();
                            if (Directory.Exists(StrTempPath))
                            {
                                Directory.GetAccessControl(StrTpath);
                                File.Copy(StrTpath, StrMpath);
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessFeedbackFiles");
                            sb.Append("(GroupPFBCode,SrNo,FileName)");
                            sb.Append(" VALUES('" + PrcNo.Trim() + "' ,'" + SrNoA + "','" + FileName.ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }
                }
                #endregion
                //Action Taken File Attachment



                tran.Commit();
                //tran.Rollback();
                if (Trans == "Start")
                {
                    AllPrcCode = "ProcessCode =" + AllPrcCode + " For PowderCoating  Started SuccessFully ";
                }
                else if (Trans == "End")
                {
                    AllPrcCode = "ProcessCode =" + AllPrcCode + " For PowderCoating  Ended SuccessFully ";
                }

                return AllPrcCode;
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
        //Old

        public Boolean ckhDoubleEntry(string PCCode, string PlanCode, string productcode, string SuppCode, string Partcode)
        {
            Boolean ckhDoubleEntry = false;
            CommonCon ComCon = new CommonCon();
            dsckhDoubleEntry = ComCon.procDS("Select  isNull(Count(PfbCode),0)  as CntDobleEntry From processfeedback where ProfitcenterCode='" + PCCode + "' and  CanopyPlanCode='" + PlanCode + "' and Productcode='" + productcode + "' and Partcode='" + Partcode + "'  and SupplierCode='" + SuppCode.Trim() + "' and Active='1'", "tbl_TurretKitForPrc");
            if (dsckhDoubleEntry != null && dsckhDoubleEntry.Tables["tbl_TurretKitForPrc"].Rows.Count == 1)
            {
                if (int.Parse(dsckhDoubleEntry.Tables["tbl_TurretKitForPrc"].Rows[0]["CntDobleEntry"].ToString().Trim()) == 0)

                {
                    ckhDoubleEntry = false;
                }
                else
                {
                    ckhDoubleEntry = true;
                }
            }
            else
            {
                ckhDoubleEntry = true;

            }
            return ckhDoubleEntry;

        }
        public string SubmitFabrication(CpyPrcFabRequest CpyPrcFabReq)
        {

            //int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;

            string PrcNo = "";
            string FabkitFlag = "No";
            try
            {

                String StrPowderCoating_PCCode = "0";

                if (CpyPrcFabReq.PCCode.Trim() == "01.008")
                {
                    StrPowderCoating_PCCode = "01.007"; //PowderCoating
                }
                else if (CpyPrcFabReq.PCCode.Trim() == "03.002")
                {
                    StrPowderCoating_PCCode = "03.002"; ////Fab
                }
                else if (CpyPrcFabReq.PCCode.Trim() == "28.015")
                {
                    StrPowderCoating_PCCode = "28.016"; ////Fab
                }


                string CurrentMnth = ComCon.getName("select Mon=case MONTH(GETDATE()) " +
                                                     "when 1 then '01' when 2 then '02' when 3 then '03' " +
                                                     "when 4 then '04' when 5 then '05' when 6 then '06' " +
                                                     "when 7 then '07' when 8 then '08' when 9 then '09' " +
                                                     "when 10 then '10' when 11 then '11' when  12 then '12' end", "tblM", "Mon");
                if (CpyPrcFabReq.PFBCode.Substring(0, 3) == "NEW")
                {
                    string[] strMachineNo = Regex.Split(CpyPrcFabReq.MachineCodeSrNo, "-->");

                    if (CpyPrcFabReq.OSSupplierCode == "0")
                    {
                        PrcNo = "Pl Select Supplier !!! ";
                        return PrcNo;
                    }

                    if (ckhDoubleEntry(CpyPrcFabReq.PCCode.Trim().ToString(), CpyPrcFabReq.PlanCode.Trim().ToString(), CpyPrcFabReq.ProductCode.Trim().ToString(), CpyPrcFabReq.OSSupplierCode.Trim().ToString(), CpyPrcFabReq.CpyKitcode.Trim().ToString()) == false)
                    {
                        #region

                        ChkforStartCPY = getChkforStartCpy(CpyPrcFabReq.PCCode.Trim(), CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, CpyPrcFabReq.CatID);
                        ChkforStart = getChkforStart(CpyPrcFabReq.PCCode.Trim(), CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), CpyPrcFabReq.CatID);
                        if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                        tran = con.BeginTransaction();

                        PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcFabReq.PCCode.Trim().Substring(0, 2), con, tran);
                        string NstPart = "0";
                        string CpyStageType = "Line1";
                        string NstWt = "0";
                        if (CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "1" || CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "0")
                        {

                            NstPart = ComCon.getTranName("select KitCode from Bomdetails where BOMCode='" + CpyPrcFabReq.BOMcode + "' and Kitcode Like '004%' and  substring(Kitcode,11,1) in ('4') and Partcode='" + CpyPrcFabReq.CpyKitcode.Trim() + "'", "TblNstPartCode", "KitCode", con, tran);

                        }
                        else if (CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "2" || CpyPrcFabReq.CpyKitcode.Trim().Substring(11, 1) == "3")
                        {
                            NstPart = CpyPrcFabReq.CpyKitcode.Trim();
                            CpyStageType = "Line2";
                        }

                        NstWt = ComCon.getTranName("select Pwt from ProfitcenterPlDetails where ProfitcenterCode='01.008' and Partcode='" + NstPart + "'", "TblPartWt", "Pwt", con, tran);

                        SqlCommand cmd = new SqlCommand();
                        sb.Remove(0, sb.Length);

                        sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,SupplierCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                        sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,SqftperUt,CpyStageType,");
                        sb.Append("PartCode,ProcessQty,CompanyCode,PFBRate,PPWCode,Remark,CatID)");
                        //If (ChkforGrpCode==true)
                        sb.Append(" values('" + PrcNo.Trim() + "', ");
                        sb.Append("'" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "',");
                        if (ChkforStart == true)
                        {
                            sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                        }
                        else if (ChkforStart == false)
                        {
                            //sb.Append("'" + ComCon.dateinyyyymmdd(GetPrevPrcTime(CpyPrcFabReq.PCCode, CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), con, tran)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                            sb.Append("'" + GetPrevPrcTime(CpyPrcFabReq.PCCode, CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), con, tran) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                        }
                        sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.OSSupplierCode.Trim() + "','" + CpyPrcFabReq.ProductCode.Trim() + "',");
                        sb.Append("'" + CpyPrcFabReq.PlanCode.Trim() + "','" + CpyPrcFabReq.BOMcode.Trim() + "',");
                        sb.Append("'" + NstPart + "','" + CpyPrcFabReq.BatchQty + "',");
                        sb.Append("'" + NstWt.Trim() + "','0',");

                        sb.Append("'" + CpyPrcFabReq.PWt + "','" + CpyPrcFabReq.PSqft + "','" + CpyStageType.Trim() + "',");
                        sb.Append("'" + CpyPrcFabReq.CpyKitcode.Trim() + "','" + CpyPrcFabReq.PrcQty + "','" + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + "',");
                        sb.Append("'" + CpyPrcFabReq.PFBRate + "','" + CpyPrcFabReq.EmpCode.Trim() + "','Nil','" + CpyPrcFabReq.CatID + "' )");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();


                        //Action Taken File Attachment
                        #region
                        if (!string.IsNullOrEmpty(CpyPrcFabReq.AttachFileDts.ToString().Trim()))
                        {
                            strPlanDts = null;
                            strPlanDts = Regex.Split(CpyPrcFabReq.AttachFileDts, "@#@");
                            SrNoA = 0;
                            foreach (String StrSub in strPlanDts)
                            {
                                SrNoA += 1;
                                DtsA = Regex.Split(StrSub.ToString().Trim(), "-->");

                                string FileName = PrcNo.ToString().Trim().Substring(4, 5).Trim() + PrcNo.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNoA) + Path.GetExtension(DtsA[1].ToString().Trim());
                                String StrMpath = ComCon.getMainFilePath("TempPrcFab") + "/" + FileName.ToString().Trim();
                                string StrTpath = "C:/TempERPFile/TempPrcFab/" + CpyPrcFabReq.EmpCode.Trim() + "/" + DtsA[1].ToString().Trim();

                                string StrTempPath = "C:/TempERPFile/TempPrcFab/" + CpyPrcFabReq.EmpCode.Trim();
                                if (Directory.Exists(StrTempPath))
                                {
                                    Directory.GetAccessControl(StrTpath);
                                    File.Copy(StrTpath, StrMpath);
                                }

                                sb.Remove(0, sb.Length);
                                sb.Append("INSERT INTO ProcessFeedbackFiles");
                                sb.Append("(GroupPFBCode,SrNo,FileName)");
                                sb.Append(" VALUES('" + PrcNo.Trim() + "' ,'" + SrNoA + "','" + FileName.ToString().Trim() + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                            }
                        }
                        #endregion
                        //Action Taken File Attachment

                        //if (ChkforStart == false)
                        //{
                        // Commented by KB on 23/12/2023
                        //sb.Remove(0, sb.Length);
                        //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                        //sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.CpyKitcode.Trim() + "',");
                        //sb.Append("'" + PrcNo.Trim() + "', GetDate(),'" + CpyPrcFabReq.PrcQty.ToString().Trim() + "','" + StrPowderCoating_PCCode.Trim() + "','1')");
                        //cmd = new SqlCommand(sb.ToString(), con);
                        //cmd.Transaction = tran;
                        //cmd.ExecuteNonQuery();
                        //cmd.Dispose();
                        // }

                        int recCount = ComCon.CountChars(CpyPrcFabReq.PrcDts, ",");
                        string[] strPrcDts = Regex.Split(CpyPrcFabReq.PrcDts, ",");
                        int SrNo = 0;

                        for (int cSub = 0; cSub <= recCount; cSub++)
                        {

                            SrNo += 1;
                            string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                            sb.Remove(0, sb.Length);
                            sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                            sb.Append("PFBRate,SaleRate,PLength,PWidth,PThickness,PLossWt,PCatagoryCode,WtPerUt,SqftPerUt)");
                            sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                            sb.Append("'" + Dts[0].Trim() + "',");
                            sb.Append("'" + Convert.ToDouble(Dts[1].Trim()) + "',");
                            sb.Append("'" + Convert.ToDouble(Dts[2].Trim()) + "',");
                            sb.Append("'" + Convert.ToDouble(Dts[3].Trim()) + "',");
                            sb.Append("'" + Convert.ToDouble(Dts[4].Trim()) + "',");
                            sb.Append("'" + Convert.ToDouble(Dts[5].Trim()) + "',");
                            sb.Append("'" + Convert.ToDouble(Dts[6].Trim()) + "',");
                            sb.Append("'" + Convert.ToDouble(Dts[7].Trim()) + "',");
                            sb.Append("'" + Dts[8].Trim() + "','" + Dts[9].Trim() + "','" + Math.Round(double.Parse(Dts[10].Trim()), 2) + "','" + Math.Round(double.Parse(Dts[11].Trim()), 2) + "')");
                            //Partcode,KitQty,TotQty,PfbRate,Plen,Pwidth,PThk,PLossWt,WtPeruts,SqftPerUts,catCode
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();


                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                            sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                            sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcFabReq.PCCode.Trim() + "',0)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }


                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanOSDetails set OSFQty=OSFQty + '" + CpyPrcFabReq.PrcQty + "' where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "' and SCode='" + CpyPrcFabReq.OSSupplierCode + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        string cntPrcQtyOS = "0";
                        cntPrcQtyOS = ComCon.getTranName("select Qty-OSFQty as BalQty from CanopyPlanOSDetails where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "'  and SCode='" + CpyPrcFabReq.OSSupplierCode + "' ", "FabPrc", "BalQty", con, tran);
                        if (cntPrcQtyOS == "0")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update CanopyPlanOSDetails set OSFStatus='D' where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "' and SCode='" + CpyPrcFabReq.OSSupplierCode + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }

                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDtsSub set CPFQty=CPFQty + '" + CpyPrcFabReq.PrcQty + "' where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "' and CatID='" + CpyPrcFabReq.CatID + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        string cntPrcQty = "0";
                        cntPrcQty = ComCon.getTranName("select CPQty-CPFQty as BalQty from CanopyPlanDtsSub where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "' and CatID='" + CpyPrcFabReq.CatID + "' ", "FabPrc", "BalQty", con, tran);
                        if (cntPrcQty == "0")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update CanopyPlanDtsSub set CPFStatus='D' where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and Partcode='" + CpyPrcFabReq.CpyKitcode + "'and CatID='" + CpyPrcFabReq.CatID + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        string cntBndStatus = "0";
                        cntBndStatus = ComCon.getTranName("select Count(CPFStatus) as CPFStatus from CanopyPlanDtsSub where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode.Trim() + "' and CatID='" + CpyPrcFabReq.CatID + "'  and  CPFStatus='P'  ", "FabPrc", "CPFStatus", con, tran);
                        if (cntBndStatus == "0")
                        {
                            string PlanQty = "0";
                            PlanQty = ComCon.getTranName("select Qty  from CanopyPlanDetails where CPCode='" + CpyPrcFabReq.PlanCode.Trim() + "' and Partcode='" + CpyPrcFabReq.ProductCode.Trim() + "' ", "TblCPQty", "Qty", con, tran);


                            //CHECK Later ASK PK Sir    

                            // Commented by KB on 
                            //sb.Remove(0, sb.Length);
                            //sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                            //sb.Append(" values('" + CpyPrcFabReq.ProductCode.ToString().Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','01.007',");
                            //sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + double.Parse(PlanQty.Trim()) + "',0)");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();

                            //sb.Remove(0, sb.Length);
                            //sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                            //sb.Append(" values('" + CpyPrcFabReq.ProductCode.ToString().Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','01.007',");
                            //sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + double.Parse(PlanQty.Trim()) + "',0)");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();
                            if (CpyPrcFabReq.CatID.ToString() == "029")
                            {

                                sb.Remove(0, sb.Length);
                                sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                                sb.Append(" values('" + CpyPrcFabReq.ProductCode.ToString().Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + StrPowderCoating_PCCode.Trim() + "',");
                                sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + double.Parse(PlanQty.Trim()) + "',0)");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                sb.Remove(0, sb.Length);
                                sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                                sb.Append(" values('" + CpyPrcFabReq.ProductCode.ToString().Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + StrPowderCoating_PCCode.Trim() + "',");
                                sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + double.Parse(PlanQty.Trim()) + "',0)");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                                // Rohan Added

                                sb.Remove(0, sb.Length);
                                sb.Append("Update CanopyPlanSerialNo set CPFSerialStatus='D' where CPCode='" + CpyPrcFabReq.PlanCode.ToString().Trim() + "' and Partcode='" + CpyPrcFabReq.ProductCode.ToString().Trim() + "'  ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                            }


                            //SrNo
                            #region  SrNo To be gentreated in canopy Assly Plan
                            //for (int m = 0; m < Convert.ToDouble(PlanQty.Trim()); m++)
                            //{
                            //    int GetCpyMax = Convert.ToInt32(ComCon.getTranName("SELECT ISNULL(MaxValue,0) as MaxValue FROM getMaxSerialNo WHERE CompCode='" + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + "' AND Prefix='CPY' AND Yr='" + ComCon.yearEnd(con, tran) + "' ", "tblMx", "MaxValue", con, tran));
                            //    int GetBfmMax = Convert.ToInt32(ComCon.getTranName("SELECT ISNULL(MaxValue,0) as MaxValue FROM getMaxSerialNo WHERE CompCode='" + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + "' AND Prefix='BFM' AND Yr='" + ComCon.yearEnd(con, tran) + "' ", "tblMx", "MaxValue", con, tran));
                            //    int GetFltMax = Convert.ToInt32(ComCon.getTranName("SELECT ISNULL(MaxValue,0) as MaxValue FROM getMaxSerialNo WHERE CompCode='" + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + "' AND Prefix='FTK' AND Yr='" + ComCon.yearEnd(con, tran) + "' ", "tblMx", "MaxValue", con, tran));
                            //    string strCpymax = "0";
                            //    string strBfmmax = "0";
                            //    string strFtkmax = "0";
                            //    string CpySerialNo = "", BfmSerialNo = "", FtkSerialNo = "";

                            //    //Cpy
                            //    #region
                            //    if (GetCpyMax == 0)
                            //        strCpymax = "0001";
                            //    else if (GetCpyMax <= 9)
                            //        strCpymax = "000" + (GetCpyMax + 1);
                            //    else if (GetCpyMax <= 99)
                            //        strCpymax = "00" + (GetCpyMax + 1);
                            //    else if (GetCpyMax <= 999)
                            //        strCpymax = "0" + (GetCpyMax + 1);
                            //    else
                            //    {
                            //        strCpymax = Convert.ToString(GetCpyMax + 1);
                            //    }
                            //    //CpySerialNo = "CPY" + DateTime.Now.Year.ToString().Substring(2, 2) + CurrentMnth.Trim() + "01" + strCpymax;

                            //    CpySerialNo = "CPY" + DateTime.Now.Year.ToString().Substring(2, 2) + CurrentMnth.Trim() + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + strCpymax;

                            //    if (double.Parse(strCpymax.Trim()) > 0)
                            //    {
                            //        sb.Remove(0, sb.Length);
                            //        sb.Append("UPDATE getMaxSerialNo SET MaxValue='" + (GetCpyMax + 1) + "' WHERE ");
                            //        sb.Append("CompCode='" + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + "' AND Yr='" + ComCon.yearEnd(con, tran) + "' AND Prefix='CPY' ");
                            //        cmd = new SqlCommand(sb.ToString(), con);
                            //        cmd.Transaction = tran;
                            //        cmd.ExecuteNonQuery();
                            //        cmd.Dispose();
                            //    }
                            //    #endregion
                            //    //Cpy

                            //    //Base
                            //    #region
                            //    if (GetBfmMax == 0)
                            //        strBfmmax = "0001";
                            //    else if (GetBfmMax <= 9)
                            //        strBfmmax = "000" + (GetBfmMax + 1);
                            //    else if (GetBfmMax <= 99)
                            //        strBfmmax = "00" + (GetBfmMax + 1);
                            //    else if (GetBfmMax <= 999)
                            //        strBfmmax = "0" + (GetBfmMax + 1);
                            //    else
                            //    {
                            //        strBfmmax = Convert.ToString(GetBfmMax + 1);
                            //    }
                            //    BfmSerialNo = "BFM" + DateTime.Now.Year.ToString().Substring(2, 2) + CurrentMnth.Trim() + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + strBfmmax;
                            //    if (double.Parse(strBfmmax.Trim()) > 0)
                            //    {
                            //        sb.Remove(0, sb.Length);
                            //        sb.Append("UPDATE getMaxSerialNo SET MaxValue='" + (GetBfmMax + 1) + "' WHERE ");
                            //        sb.Append("CompCode='" + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + "' AND Yr='" + ComCon.yearEnd(con, tran) + "' AND Prefix='BFM' ");
                            //        cmd = new SqlCommand(sb.ToString(), con);
                            //        cmd.Transaction = tran;
                            //        cmd.ExecuteNonQuery();
                            //        cmd.Dispose();
                            //    }
                            //    #endregion
                            //    //Base

                            //    //Fuel Tank
                            //    #region
                            //    if (GetFltMax == 0)
                            //        strFtkmax = "0001";
                            //    else if (GetFltMax <= 9)
                            //        strFtkmax = "000" + (GetFltMax + 1);
                            //    else if (GetFltMax <= 99)
                            //        strFtkmax = "00" + (GetFltMax + 1);
                            //    else if (GetFltMax <= 999)
                            //        strFtkmax = "0" + (GetFltMax + 1);
                            //    else
                            //    {
                            //        strFtkmax = Convert.ToString(GetFltMax + 1);
                            //    }
                            //    FtkSerialNo = "FTK" + DateTime.Now.Year.ToString().Substring(2, 2) + CurrentMnth.Trim() + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + strFtkmax;
                            //    if (double.Parse(strFtkmax.Trim()) > 0)
                            //    {
                            //        sb.Remove(0, sb.Length);
                            //        sb.Append("UPDATE getMaxSerialNo SET MaxValue='" + (GetFltMax + 1) + "' WHERE ");
                            //        sb.Append("CompCode='" + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + "' AND Yr='" + ComCon.yearEnd(con, tran) + "' AND Prefix='FTK' ");
                            //        cmd = new SqlCommand(sb.ToString(), con);
                            //        cmd.Transaction = tran;
                            //        cmd.ExecuteNonQuery();
                            //        cmd.Dispose();
                            //    }
                            //    //Fuel Tank
                            //    #endregion
                            //    sb.Remove(0, sb.Length);
                            //    sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo,PFBBOTSerialNo,BFMSrNo,FLKSrNo,Status,QPCStatus, RWStatus) ");
                            //    sb.Append("Values('" + PrcNo.Trim() + "','" + (m + 1) + "','" + CpyPrcFabReq.ProductCode + "',");
                            //    sb.Append("'" + CpySerialNo.Trim() + "','" + CpyPrcFabReq.PlanCode.Trim() + "','" + BfmSerialNo.Trim() + "','" + FtkSerialNo.Trim() + "','P','OK','OK')");
                            //    cmd = new SqlCommand(sb.ToString(), con);
                            //    cmd.Transaction = tran;
                            //    cmd.ExecuteNonQuery();
                            //    cmd.Dispose();


                            //}
                            #endregion
                            //SrNo


                            //Kanban Done By fs 08/08/2024
                            #region
                            strKanBan = "";
                            string GetMaxValue = "";
                            dsKanBan = ComCon.procTranDS("exec InternalTOCReq '" + CpyPrcFabReq.PCCode.Trim() + "' ", "tbl_RaiseReqDtsKanBan", con, tran);
                            if (dsKanBan != null && dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows.Count > 0)
                            {
                                // Master Req
                                #region
                                GetMaxValue = "";

                                GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcFabReq.PCCode.Substring(0, 2), con, tran);
                                strKanBan = GetMaxValue;


                                sb.Remove(0, sb.Length);
                                sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                    " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                                sb.Append("values('" + strKanBan.Trim() + "','" + GetMaxValue.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                                sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + CpyPrcFabReq.PCCode.Trim() + "','23.001','" + CpyPrcFabReq.ProductCode + "','" + CpyPrcFabReq.PCCode.Substring(0, 2) + "','" + CpyPrcFabReq.BatchQty.ToString().Trim() + "','P','WIP',");
                                sb.Append("'Auto Req For Plan No: " + CpyPrcFabReq.ProductCode + " and Prc No: " + PrcNo + "','1','1','1','KanBan','0')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                                #endregion
                                //Master Req
                                //Details Req
                                #region


                                int SrNoReq = 0;

                                for (int cntd = 0; cntd < dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows.Count; cntd++)
                                {
                                    SrNoReq = SrNoReq + 1;
                                    cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@REQCode", strKanBan);
                                    cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                    cmd.Parameters.AddWithValue("@PartCode", dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows[cntd]["Partcode"].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Qty", double.Parse(dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@REQStatus", "P");
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                }
                                #endregion
                                //Details Req`
                            }

                            //DateTime.Now.ToString("yyyy-MM-dd")
                            //*********************User Acivity ***************************
                            cmd = new SqlCommand("insertLoginTransactionDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@EmpID", CpyPrcFabReq.EmpCode);
                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                            cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                            cmd.Parameters.AddWithValue("@TransactionNo", strKanBan);
                            cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcFabReq.PCCode.Substring(0, 2).Trim());
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            #endregion
                            //Kanban Done By fs 08/08/2024
                        }

                        //****************User Acivity****************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", CpyPrcFabReq.EmpCode.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "Fabrication Process");
                        cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcFabReq.PCCode.Substring(0, 2).Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //Prc Below 1000    
                        #region
                       // string GetMaxRate = "0";
                        string GetMaxRatePartCode = "0";
                        string PrcBelowRate = "";
                        //GetMaxRate = ComCon.getTranName("Select Isnull(Max(Rate),0) as MRate From CanopyplandtsSub where CPCode='" + CpyPrcFabReq.PlanCode + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode + "' and CatID='" + CpyPrcFabReq.CatID + "' ", "tbl_ChkForMaxRate", "MRate", con, tran);
                        //if (double.Parse(GetMaxRate.Trim()) == CpyPrcFabReq.Rate)
                        GetMaxRatePartCode = ComCon.getTranName("Select top 1 PartCode From CanopyplandtsSub where CPCode='" + CpyPrcFabReq.PlanCode + "' and CpyPartcode='" + CpyPrcFabReq.ProductCode + "' and CatID='" + CpyPrcFabReq.CatID + "' order by rate desc ", "tbl_ChkForMaxRate", "PartCode", con, tran);
                         if (GetMaxRatePartCode.Trim() == CpyPrcFabReq.CpyKitcode.Trim())
                        {
                            //dsKitbelowRate = ComCon.procTranDS("select Partcode From CanopyPlanDtsSubBelowStdRate where CpyPartcode='" + CpyPrcFabReq.ProductCode.ToString().Trim() + "' and CPCode='" + CpyPrcFabReq.PlanCode + "' ", "tbl_KitbelowRate", con, tran);

                            dsKitbelowRate = ComCon.procTranDS("select Pf.Partcode,Pl.rate,Pl.PurRate,Pwt,Psqft From CanopyPlanDtsSubBelowStdRate Pf Inner Join ProfitcenterPldetails Pl on  Pf.Partcode = Pl.partcode where CpyPartcode = '" + CpyPrcFabReq.ProductCode.ToString().Trim() + "' and CPCode = '" + CpyPrcFabReq.PlanCode + "' and CatID='" + CpyPrcFabReq.CatID + "' and ProfitcenterCode = '01.008' ", "tbl_KitbelowRate", con, tran);
                            if (dsKitbelowRate != null && dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count > 0)
                            {

                                for (int br = 0; br < dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count; br++)
                                {

                                    // Mst Entry
                                    #region
                                    PrcBelowRate = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcFabReq.PCCode.Trim().Substring(0, 2), con, tran);
                                    //dsDetails.Tables["tbl_RaiseReqDts"].Rows[i]["Partcode"].ToString().Trim()
                                    sb.Remove(0, sb.Length);
                                    sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,CpyStageType,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,supplierCode,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                                    sb.Append("NestingforCode,NestingforQty,nstWtPerUt,nstSqftPerUt,WtperUt,SqftperUt,");
                                    sb.Append("PartCode,ProcessQty,CompanyCode,PFBRate,PPWCode,Remark,CatID)");
                                    sb.Append(" values('" + PrcNo.Trim() + "','" + PrcBelowRate.Trim() + "','" + CpyStageType + "','" + (PrcBelowRate.Substring(10, 8)) + "', ");
                                    // sb.Append("'" + ComCon.dateinyyyymmdd(GetPrevPrcTime(CpyPrcFabReq.PCCode, CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString(), con, tran)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                    sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                    sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.ProductCode.Trim() + "',");
                                    sb.Append("'" + CpyPrcFabReq.PlanCode.Trim() + "','" + CpyPrcFabReq.BOMcode.Trim() + "',");
                                    sb.Append("'" + NstPart + "','" + CpyPrcFabReq.BatchQty + "','" + NstWt.Trim() + "','0',  '" + double.Parse(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Pwt"].ToString().Trim()) + "',  '" + double.Parse(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["PSqft"].ToString().Trim()) + "',");
                                    sb.Append("'" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "','" + CpyPrcFabReq.PrcQty + "','" + CpyPrcFabReq.PCCode.Trim().Substring(0, 2) + "', ");
                                    sb.Append("'" + ComCon.getTranName("Select Isnull(Max(Rate),0) as Rate From ProfitcenterPLDetails where ProfitcenterCode='" + CpyPrcFabReq.PCCode + "' and Partcode='" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "' ", "tbl_PLRate", "Rate", con, tran) + "'");
                                    sb.Append(", '" + CpyPrcFabReq.EmpCode + "','Nil','" + CpyPrcFabReq.CatID.Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    // Commented by KB on 20/12/2023
                                    //sb.Remove(0, sb.Length);
                                    //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                                    //sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "',");
                                    //sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + CpyPrcFabReq.PrcQty + "','" + StrPowderCoating_PCCode.Trim() + "',1)");
                                    //cmd = new SqlCommand(sb.ToString(), con);
                                    //cmd.Transaction = tran;
                                    //cmd.ExecuteNonQuery();
                                    //cmd.Dispose();

                                    #endregion
                                    // Mst Entry

                                    //Dts Entry
                                    #region

                                    int ChkStk = 0;
                                    dsKitbelowRatedts = ComCon.procTranDS("select Bd.Partcode,P.PartDesc,Qty,Purrate,rate,Pwt,Psqft,Bd.Length,bd.Width,bd.Thickness,bd.LossWgt,Bd.categoryID, " +
                                       " (select Round(Isnull(Sum(Recqty) - sum(IssueQty), 0), 00) as Stk From ( select Sum(ReceivedQty) as Recqty, " +
                                      " 0.00 as IssueQty from stockwip where ToProfitcenterCode = '" + CpyPrcFabReq.PCCode.Trim() + "' and StockType = '0' " +
                                     " and Partcode = Bd.Partcode and  ReceivedQty > 0   Union all " +
                                      " select 0.00 as Recqty, sum(IssueQty) as IssueQty from stockwip where FromProfitcenterCode = '" + CpyPrcFabReq.PCCode.Trim() + "' and StockType = '0' " +
                                      "  and Partcode = Bd.Partcode and  IssueQty > 0) as stk) as Stock " +
                                      "From BOMDetails Bd Inner Join ProfitcenterPLdetails Pl On   Bd.KitCode=Pl.Partcode  Inner Join part P On Bd.Partcode=P.partcode " +
                                        " where Bd.BOMCode='" + CpyPrcFabReq.BOMcode + "' and Bd.KitCode='" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "' " +
                                        " and Pl.ProfitcenterCode='" + CpyPrcFabReq.PCCode + "'  ", "tbl_KitbelowRatedts", con, tran);

                                    if (dsKitbelowRatedts != null && dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows.Count > 0)
                                    {
                                        SrNo = 0;

                                        for (int brd = 0; brd < dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows.Count; brd++)

                                        {
                                            if ((Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) * CpyPrcFabReq.PrcQty) >
                                            Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Stock"].ToString().Trim()))
                                            {
                                                if (ChkStk == 0)
                                                {
                                                    PrcNo = dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["PartDesc"].ToString().Trim();
                                                    ChkStk = 1;

                                                }
                                                else
                                                {
                                                    PrcNo = PrcNo + "," + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["PartDesc"].ToString().Trim();
                                                }
                                            }
                                            //else
                                            if (ChkStk == 0)
                                            {
                                                SrNo += 1;
                                                sb.Remove(0, sb.Length);
                                                sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                                                sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt,PLength,PWidth,PThickness,PLossWt,PCatagoryCode)");
                                                sb.Append("values('" + PrcBelowRate.Trim() + "','" + SrNo + "',");
                                                sb.Append("'" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partcode"].ToString().Trim().Trim() + "',");
                                                sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) + "',");
                                                sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString().Trim()) * CpyPrcFabReq.PrcQty + "',");
                                                sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Purrate"].ToString().Trim()) + "',");
                                                sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["rate"].ToString().Trim()) + "',");
                                                sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Pwt"].ToString().Trim()) + "',");
                                                sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Psqft"].ToString().Trim()) + "',");
                                                sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Length"].ToString().Trim()) + "',");
                                                sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Width"].ToString().Trim()) + "',");
                                                sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Thickness"].ToString().Trim()) + "',");
                                                sb.Append("'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["LossWgt"].ToString().Trim()) + "',");
                                                sb.Append("'" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["categoryID"].ToString().Trim() + "')");
                                                cmd = new SqlCommand(sb.ToString(), con);
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();


                                                sb.Remove(0, sb.Length);
                                                sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                                sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Partcode"].ToString().Trim() + "',");
                                                sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + Convert.ToDouble(dsKitbelowRatedts.Tables["tbl_KitbelowRatedts"].Rows[brd]["Qty"].ToString()) * CpyPrcFabReq.PrcQty + "','" + CpyPrcFabReq.PCCode.Trim() + "',0)");
                                                cmd = new SqlCommand(sb.ToString(), con);
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();

                                            }
                                        }
                                        if (ChkStk > 0)
                                        {
                                            PrcNo = "Insufficient Stock For Part(BR): " + PrcNo;
                                            return PrcNo;
                                        }
                                    }
                                    #endregion
                                    //Dts Entry
                                }


                            }

                            #region

                            //// Fab kit
                            #region
                            FabkitFlag = "Yes";
                            dsDetailsSub = ComCon.procTranDS("Exec Get_FabKit '" + CpyPrcFabReq.BOMcode.Trim() + "','" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.CatID.Trim() + "' ", "tbl_FabKit", con, tran);
                            if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_FabKit"].Rows.Count > 0)
                            {
                                for (int k = 0; k < dsDetailsSub.Tables["tbl_FabKit"].Rows.Count; k++)
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,SaleRate)");
                                    sb.Append("values('" + PrcNo.Trim() + "','" + (k + 2) + "',");
                                    sb.Append("'" + dsDetailsSub.Tables["tbl_FabKit"].Rows[k]["PartCode"].ToString().Trim() + "',");
                                    sb.Append("'" + dsDetailsSub.Tables["tbl_FabKit"].Rows[k]["Qty"].ToString().Trim() + "',");
                                    sb.Append("'" + double.Parse(dsDetailsSub.Tables["tbl_FabKit"].Rows[k]["Qty"].ToString().Trim()) * double.Parse(CpyPrcFabReq.PrcQty.ToString()) + "',");

                                    sb.Append("'" + dsDetailsSub.Tables["tbl_FabKit"].Rows[k]["SuppRate"].ToString().Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                    sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + dsDetailsSub.Tables["tbl_FabKit"].Rows[k]["PartCode"].ToString().Trim() + "',");
                                    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(dsDetailsSub.Tables["tbl_FabKit"].Rows[k]["Qty"].ToString().Trim()) * double.Parse(CpyPrcFabReq.PrcQty.ToString()) + "','" + CpyPrcFabReq.PCCode.Trim() + "',0)");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }
                            #endregion
                            // Fab kit

                            #endregion
                        }
                        #endregion
                        //Prc Below 1000    

                        tran.Commit();
                        // tran.Rollback();
                        PrcNo = "ProcessCode=" + PrcNo + " For Fabrication  Saved SuccessFully ";
                        #endregion
                    }
                    else
                    {
                        PrcNo = "Fabrication Process For Part Already Saved ";
                        return PrcNo;
                    }

                }
                else if (CpyPrcFabReq.PFBCode.Substring(0, 3) == "PSH")
                {
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedBack set EDt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where PFBCode='" + CpyPrcFabReq.PFBCode.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //sb.Remove(0, sb.Length);
                    //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                    //sb.Append(" values('" + CpyPrcFabReq.PCCode.Trim() + "','" + CpyPrcFabReq.CpyKitcode.Trim() + "',");
                    //sb.Append("'" + CpyPrcFabReq.PFBCode.Trim() + "',GetDate(),'" + CpyPrcFabReq.PrcQty.ToString().Trim() + "','01.007',1)");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();


                    tran.Commit();
                    //tran.Rollback();
                    PrcNo = "ProcessCode=" + CpyPrcFabReq.PFBCode.Trim() + " For Fabrication  End SuccessFully ";
                }
                return PrcNo;
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
        public string SubmitPowderCoating(CpyPrcPCRequest CpyPrcPCReq)
        {
            //int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;

            string PrcNo = "";
            string Trans = "";
            try
            {
                int recCount = ComCon.CountChars(CpyPrcPCReq.PrcDts, ",");
                string[] strPrcDts = Regex.Split(CpyPrcPCReq.PrcDts, ",");
                int SrNo = 0;
                string GrpPfbCode = "";
                string PCkitFlag = "No";
                string AllPrcCode = "";
                string NstPart = "0";
                string NstWtsqft = "0";
                string CpyStageType = "Line1";
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                string[] strMachineNo = Regex.Split(CpyPrcPCReq.MachineCodeSrNo, "-->");
                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo = 0;
                    string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                    if (Dts[10].Substring(0, 3) == "NEW")
                    {
                        Trans = "Start";
                        //Master Entry
                        #region
                        PrcNo = "";

                        PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcPCReq.PCCode.Trim().Substring(0, 2), con, tran);
                        if (cSub == 0)
                        {
                            GrpPfbCode = PrcNo;
                            AllPrcCode = PrcNo;
                        }
                        else
                        {
                            AllPrcCode = AllPrcCode + "," + PrcNo;
                        }

                        NstPart = "0";
                        NstWtsqft = "0";
                        if (Dts[3].Trim().Substring(11, 1) == "1" || Dts[3].Trim().Substring(11, 1) == "0")
                        {
                            NstPart = ComCon.getTranName("select KitCode from Bomdetails where BOMCode='" + Dts[2].Trim() + "' and Kitcode Like '004%' and  substring(Kitcode,11,1) in ('4') and Partcode='" + Dts[3].Trim() + "'", "TblNstPartCode", "KitCode", con, tran);
                        }
                        else if (Dts[3].Trim().Substring(11, 1) == "2" || Dts[3].Trim().Substring(11, 1) == "3")
                        {
                            NstPart = Dts[3].Trim();
                            CpyStageType = "Line2";
                        }

                        //NstWtsqft = ComCon.getTranName("select convert(Pwt,nvarchar(10))+'-->'+convert(PSqft,nvarchar(10)) as PwtSqft from ProfitcenterPlDetails where ProfitcenterCode='01.008' and Partcode='" + NstPart + "'", "TblPwtSqft", "PwtSqft", con, tran);
                        NstWtsqft = ComCon.getTranName("Select convert(varchar(10),Pwt)+'-->'+convert(varchar(10),PSqft ) as PwtSqft from ProfitcenterPlDetails where ProfitcenterCode='01.007' and Partcode='" + NstPart + "'", "TblPwtSqft", "PwtSqft", con, tran);
                        string[] strNstWtsqft = Regex.Split(NstWtsqft.Trim(), "-->");

                        //SqlCommand cmd = new SqlCommand();
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,SupplierCode,CanopyPlanCode,ProductCode,TurretKitCode,");
                        sb.Append("PartCode,NestingForCode,NestingForQty,SqftPerUt,WtPerUt,PFBRate,ProcessQty,NstWtPerUt,NstSqftPerUt,CpyStageType,PPWCode,CompanyCode,Remark,CatID)");
                        sb.Append(" values('" + GrpPfbCode.Trim() + "','" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "', ");
                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                        sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcPCReq.PCCode.Trim() + "','" + CpyPrcPCReq.SupplierCode.Trim() + "',");
                        sb.Append("'" + Dts[0].Trim() + "','" + Dts[1].Trim() + "','" + Dts[2].Trim() + "','" + Dts[3].Trim() + "','" + NstPart.Trim() + "','" + Dts[4].Trim() + "', ");
                        sb.Append("'" + Dts[5].Trim() + "','" + Dts[6].Trim() + "','" + Dts[7].Trim() + "','" + Dts[9].Trim() + "',");
                        sb.Append("'" + double.Parse(strNstWtsqft[0].Trim()) + "','" + double.Parse(strNstWtsqft[1]) + "','" + CpyStageType + "',");
                        sb.Append("'" + CpyPrcPCReq.EmpCode.Trim() + "','" + CpyPrcPCReq.PCCode.Trim().Substring(0, 2) + "','Nil','" + Dts[13].Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                        sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + Dts[3].ToString().Trim() + "',");
                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[9].ToString().Trim()) + "','" + CpyPrcPCReq.PCCode.Trim() + "',1)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        String StrKVA = ComCon.getTranName("Select Kva from Part where Partcode='" + Dts[1].ToString().Trim() + "' and active='1'", "PartKVA", "Kva", con, tran).ToString().Trim();

                        if (double.Parse(StrKVA.ToString().Trim()) < 200)
                        {
                            //Commented by KB on 20/12/2023
                            //sb.Remove(0, sb.Length);
                            //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode, ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                            //sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + Dts[3].ToString().Trim() + "',");
                            //sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[9].ToString().Trim()) + "','01.005',0)");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();
                        }
                        //else
                        //{
                        //    sb.Remove(0, sb.Length);
                        //    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode, ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                        //    sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + Dts[3].ToString().Trim() + "',");
                        //    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[9].ToString().Trim()) + "','01.007',2)");
                        //    cmd = new SqlCommand(sb.ToString(), con);
                        //    cmd.Transaction = tran;
                        //    cmd.ExecuteNonQuery();
                        //    cmd.Dispose();
                        //}

                        #endregion
                        //Master Entry

                        //Details Entry
                        #region
                        SrNo = +1; ;
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                        sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt)");
                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + Dts[3].Trim() + "',");
                        sb.Append("'1','" + Convert.ToDouble(Dts[9].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[8].Trim()) + "',"); //PurRate
                        sb.Append("'" + Convert.ToDouble(Dts[7].Trim()) + "',"); //SaleRate
                        sb.Append("'" + Convert.ToDouble(Dts[6].Trim()) + "',");//WtPerUt
                        sb.Append("'" + Convert.ToDouble(Dts[5].Trim()) + "')");//Sqft

                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //PC Asskit and KitBelow 1000
                        #region
                        //string GetMaxRate = "0";
                        string GetMaxRatePartCode = "0";

                        string PrcBelowRate = "";
                        PCkitFlag = "No";
                        // START Commented on 18/03/2025 by KB to add Condition of Partcode instead of MaxRate as Below 1000 Partcode not Saved in WIP.
                        // GetMaxRate = ComCon.getTranName("Select Isnull(Max(Rate),0) as MRate From CanopyplandtsSub where CPCode='" + Dts[0].Trim() + "' and CpyPartcode='" + Dts[1].Trim() + "' and CatID='" + Dts[13].Trim() + "' ", "tbl_ChkForMaxRate", "MRate", con, tran);
                        // if (Math.Round(double.Parse(GetMaxRate.Trim()), 0) == Math.Round(double.Parse(Dts[7].Trim()), 0))
                        // END Commented on 18/03/2025 by KB to add Condition of Partcode instead of MaxRate as Below 1000 Partcode not Saved in WIP.
                        GetMaxRatePartCode = ComCon.getTranName("Select top 1 PartCode From CanopyplandtsSub where CPCode='" + Dts[0].Trim() + "' and CpyPartcode='" + Dts[1].Trim() + "' and CatID='" + Dts[13].Trim() + "' order by rate desc ", "tbl_ChkForMaxRate", "PartCode", con, tran);

                        if (GetMaxRatePartCode.Trim() == Dts[3].Trim())
                        // if (Dts[3].Trim().Substring(0, 3) == "004" && Dts[3].Trim().Substring(11, 3) == "120")
                        {
                            //PC Asskit
                            #region
                            PCkitFlag = "Yes";
                            dsDetailsSub = ComCon.procTranDS("Exec GetPCKit '" + Dts[2].Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "' , '" + Dts[13].Trim() + "' ", "tbl_PCKit", con, tran);
                            if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_PCKit"].Rows.Count > 0)
                            {
                                for (int k = 0; k < dsDetailsSub.Tables["tbl_PCKit"].Rows.Count; k++)
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,SaleRate)");
                                    sb.Append("values('" + PrcNo.Trim() + "','" + (k + 2) + "',");
                                    sb.Append("'" + dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["PartCode"].ToString().Trim() + "',");
                                    sb.Append("'" + dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["Qty"].ToString().Trim() + "',");
                                    sb.Append("'" + double.Parse(dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["Qty"].ToString().Trim()) * double.Parse(Dts[9].Trim()) + "',");
                                    sb.Append("'" + dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["SuppRate"].ToString().Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                    sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["PartCode"].ToString().Trim() + "',");
                                    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(dsDetailsSub.Tables["tbl_PCKit"].Rows[k]["Qty"].ToString().Trim()) * double.Parse(Dts[9].Trim()) + "','" + CpyPrcPCReq.PCCode.Trim() + "',1)");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }
                            #endregion
                            //PC Asskit

                            //KitBelow 1000
                            #region
                            dsKitbelowRate = ComCon.procTranDS("select P.AliseName, CD.Partcode,(select Convert(nvarchar(10),PurRate)+'-->'+Convert(nvarchar(10),Rate)+'-->'+Convert(nvarchar(10),PWt)+'-->'+Convert(nvarchar(10),PSqft) " +
                            " from ProfitcenterPLDetails where  ProfitcenterCode = '01.007' and Partcode=cd.partcode) as PartDts ," +
                            " (select Round(Isnull(Sum(Recqty) - sum(IssueQty), 0), 00) as Stk From ( select Sum(ReceivedQty) as Recqty, " +
                            " 0.00 as IssueQty from stockwip where ToProfitcenterCode = '01.007' and StockType = '1' " +
                            " and Partcode = cd.Partcode and  ReceivedQty > 0   Union all " +
                            " select 0.00 as Recqty, sum(IssueQty) as IssueQty from stockwip where FromProfitcenterCode = '01.007' and StockType = '1' " +
                            "  and Partcode = cd.Partcode and  IssueQty > 0) as stk) as Stock " +
                            "From CanopyPlanDtsSubBelowStdRate cd  INNER JOIN Part P ON CD.PartCode = P.PartCode where CpyPartcode='" + Dts[1].Trim() + "' and CPCode='" + Dts[0].Trim() + "' and CatID='" + Dts[13].Trim() + "'  ", "tbl_KitbelowRate", con, tran);
                            if (dsKitbelowRate != null && dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count > 0)
                            {
                                int ChkStk = 0;
                                for (int br = 0; br < dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count; br++)
                                {
                                    string[] DtsBR = Regex.Split(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["PartDts"].ToString().Trim(), "-->");
                                    // Mst Entry
                                    #region
                                    if (Convert.ToDouble(Dts[9].Trim()) >
                                    Convert.ToDouble(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Stock"].ToString().Trim()))
                                    {
                                        if (ChkStk == 0)
                                        {
                                            PrcNo = dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["AliseName"].ToString().Trim();
                                            ChkStk = 1;
                                        }
                                        else
                                        {
                                            PrcNo = PrcNo + "," + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["AliseName"].ToString().Trim();
                                        }
                                    }
                                    else if (ChkStk == 0)
                                    {
                                        PrcBelowRate = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcPCReq.PCCode.Trim().Substring(0, 2), con, tran);
                                        //dsDetails.Tables["tbl_RaiseReqDts"].Rows[i]["Partcode"].ToString().Trim()
                                        sb.Remove(0, sb.Length);
                                        sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,CpyStageType,MOFCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,SupplierCode,CanopyPlanCode,ProductCode,TurretKitCode,");
                                        sb.Append("PartCode,NestingForCode,NestingForQty,SqftPerUt,WtPerUt,PFBRate,ProcessQty,NstWtPerUt,NstSqftPerUt,PPWCode,CompanyCode,Remark,CatID) ");
                                        sb.Append(" values('" + GrpPfbCode.Trim() + "','" + PrcBelowRate.Trim() + "','" + CpyStageType + "','" + PrcNo.Trim() + "','" + (PrcBelowRate.Substring(10, 8)) + "', ");
                                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                                        sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcPCReq.PCCode.Trim() + "','" + CpyPrcPCReq.SupplierCode.Trim() + "',");
                                        sb.Append("'" + Dts[0].Trim() + "','" + Dts[1].Trim() + "','" + Dts[2].Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "','" + NstPart.Trim() + "','" + Dts[4].Trim() + "', ");
                                        sb.Append("'" + DtsBR[3].Trim() + "','" + DtsBR[2].Trim() + "','" + DtsBR[1].Trim() + "','" + Dts[9].Trim() + "',");
                                        sb.Append("'" + double.Parse(strNstWtsqft[0].Trim()) + "','" + double.Parse(strNstWtsqft[1].Trim()) + "',");
                                        sb.Append("'" + CpyPrcPCReq.EmpCode.Trim() + "','" + CpyPrcPCReq.PCCode.Trim().Substring(0, 2) + "','Nil','" + Dts[13].Trim() + "')");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        sb.Remove(0, sb.Length);
                                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                        sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "',");
                                        sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + Dts[9].Trim() + "','01.007',1)");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        // Commented By KB on 20/12/2023
                                        //sb.Remove(0, sb.Length);
                                        //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                                        //sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "',");
                                        //sb.Append("'" + PrcBelowRate.Trim() + "',GetDate(),'" + Dts[9].Trim() + "','01.005',0)");
                                        //cmd = new SqlCommand(sb.ToString(), con);
                                        //cmd.Transaction = tran;
                                        //cmd.ExecuteNonQuery();
                                        //cmd.Dispose();

                                        #endregion
                                        // Mst Entry

                                        // Dts Entry
                                        #region

                                        sb.Remove(0, sb.Length);
                                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                                        sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt)");
                                        sb.Append("values('" + PrcBelowRate.Trim() + "','" + SrNo + "',");
                                        sb.Append("'" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "',");
                                        sb.Append("'1',");
                                        sb.Append("'" + Convert.ToDouble(Dts[9].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[0].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[1].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[2].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[3].Trim()) + "')");

                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                        #endregion
                                        // Dts Entry

                                    }
                                }

                                if (ChkStk > 0)
                                {
                                    PrcNo = "Insufficient Stock For Part(BR): " + PrcNo;
                                    return PrcNo;
                                }

                            }
                            #endregion
                            //KitBelow 1000
                        }
                        #endregion
                        //PC Asskit and KitBelow 1000

                        #endregion
                        //Details Entry

                        if (PCkitFlag == "Yes")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update ProcessFeedBack set CanopyCode='PCKit' where PFBCode='" + PrcNo.Trim() + "'and CatID='" + Dts[13].Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDtsSub set CPPCQty=CPPCQty + '" + double.Parse(Dts[9].ToString().Trim()) + "' where CPCode='" + Dts[0].ToString().Trim() + "' and CpyPartcode='" + Dts[1].ToString().Trim() + "' and Partcode='" + Dts[3].ToString().Trim() +  "' and CatID='" + Dts[13].ToString().Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        string cntPrcQty = "0";
                        cntPrcQty = ComCon.getTranName("select CPQty-CPPCQty as BalQty from CanopyPlanDtsSub where  CPCode='" + Dts[0].ToString().Trim() + "' and CpyPartcode='" + Dts[1].ToString().Trim() + "' and Partcode='" + Dts[3].ToString().Trim() + "' and CatID='" + Dts[13].ToString().Trim() + "' ", "PCPrc", "BalQty", con, tran);
                        if (cntPrcQty == "0")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update CanopyPlanDtsSub set CPPCStatus='D' where CPCode='" + Dts[0].ToString().Trim() + "' and CpyPartcode='" + Dts[1].ToString().Trim() + "' and Partcode='" + Dts[3].ToString().Trim() + "' and CatID='" + Dts[13].ToString().Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();



                        }

                        string cntPCStatus = "0";
                        cntPCStatus = ComCon.getTranName("select Count(CPPCStatus) as CPPCStatus from CanopyPlanDtsSub where CPCode='" + Dts[0].ToString().Trim() + "' and CpyPartcode='" + Dts[1].ToString().Trim() + "'  and CatID='" + Dts[13].ToString().Trim() + "' and  CPPCStatus='P'  ", "BendingPrc", "CPPCStatus", con, tran);

                        if (CpyPrcPCReq.PCCode.Trim() == "28.016")
                        {
                            if (cntPCStatus == "0")
                            {
                                if (CpyPrcPCReq.CatID.ToString() == "029")
                                {


                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                                    sb.Append(" values('" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "','28.016',");
                                    sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Dts[9].ToString().Trim() + "',0)");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    if (double.Parse(StrKVA.ToString().Trim()) <= 58.5)
                                    {

                                        sb.Remove(0, sb.Length);
                                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                                        sb.Append(" values('" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "','28.017',");
                                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Dts[9].ToString().Trim() + "',0)");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                    }
                                    else if (double.Parse(StrKVA.ToString().Trim()) > 58.5 )
                                    {

                                        sb.Remove(0, sb.Length);
                                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                                        sb.Append(" values('" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "','28.017',");
                                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Dts[9].ToString().Trim() + "',0)");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                    }


                                    //Rohan Addded

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update CanopyPlanSerialNo set CPPCSerialStatus='D' where CPCode='" + Dts[0].ToString().Trim() + "' and Partcode='" + Dts[1].ToString().Trim() + "'  ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();




                                }

                                sb.Remove(0, sb.Length);
                                sb.Append("Update ProcessFeedbackDetailsSub set PCStatus='D' where PFBBOTSerialNo='" + Dts[0].Trim().Trim() + "' and Partcode='" + Dts[1].ToString().Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                            }
                        }
                        else
                        {
                            if (cntPCStatus == "0")
                            {
                                if (Dts[13].ToString() == "029")
                                {

                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode,IssueCode,IssueDate, IssueQty, StockType)");
                                    sb.Append(" values('" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "','01.007',");
                                    sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Dts[9].ToString().Trim() + "',0)");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }

                                //Kanban Done By fs 08/08/2024
                                #region
                                strKanBan = "";
                                string GetMaxValue = "";
                                dsKanBan = ComCon.procTranDS("exec InternalTOCReq '" + CpyPrcPCReq.PCCode.Trim() + "' ", "tbl_RaiseReqDtsKanBan", con, tran);
                                if (dsKanBan != null && dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows.Count > 0)
                                {
                                    // Master Req
                                    #region
                                    GetMaxValue = "";

                                    GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcPCReq.PCCode.Substring(0, 2), con, tran);
                                    strKanBan = GetMaxValue;


                                    sb.Remove(0, sb.Length);
                                    sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                        " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                                    sb.Append("values('" + strKanBan.Trim() + "','" + GetMaxValue.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                                    sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + CpyPrcPCReq.PCCode.Trim() + "','23.001','" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Substring(0, 2) + "','" + double.Parse(Dts[9].ToString().Trim()) + "','P','WIP',");
                                    sb.Append("'Auto Req For Plan No: " + Dts[1].ToString().Trim() + " and Prc No: " + PrcNo + "','1','1','1','KanBan','0')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();


                                    #endregion
                                    //Master Req
                                    //Details Req
                                    #region


                                    int SrNoReq = 0;

                                    for (int cntd = 0; cntd < dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows.Count; cntd++)
                                    {
                                        SrNoReq = SrNoReq + 1;
                                        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@REQCode", strKanBan);
                                        cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                        cmd.Parameters.AddWithValue("@PartCode", dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows[cntd]["Partcode"].ToString().Trim());
                                        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));
                                        cmd.Parameters.AddWithValue("@REQStatus", "P");
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();


                                    }
                                    #endregion
                                    //Details Req
                                }



                                //DateTime.Now.ToString("yyyy-MM-dd")
                                //*********************User Acivity ***************************
                                cmd = new SqlCommand("insertLoginTransactionDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@EmpID", CpyPrcPCReq.EmpCode);
                                cmd.Parameters.AddWithValue("@TransactionType", "S");
                                cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                                cmd.Parameters.AddWithValue("@TransactionNo", strKanBan);
                                cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcPCReq.PCCode.Substring(0, 2).Trim());
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                #endregion
                                //Kanban Done By fs 08/08/2024
                                if (Dts[13].ToString() == "029")
                                {
                                    if (double.Parse(StrKVA.ToString().Trim()) <= 58.5)
                                    {
                                        // Commented by KB on 06/03/2025 as Stock will be Transaferred to Flat Pack Profitcenter instead of Canopy Assembly
                                        //sb.Remove(0, sb.Length);
                                        //sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                                        //sb.Append(" values('" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "','01.005',");
                                        //sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Dts[9].ToString().Trim() + "',0)");
                                        //cmd = new SqlCommand(sb.ToString(), con);
                                        //cmd.Transaction = tran;
                                        //cmd.ExecuteNonQuery();
                                        //cmd.Dispose();

                                        // Code added by KB on 06/03/2025 as Stock will be Transaferred to Flat Pack Profitcenter instead of Canopy Assembly
                                        sb.Remove(0, sb.Length);
                                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                                        sb.Append(" values('" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "','01.093',");
                                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Dts[9].ToString().Trim() + "',0)");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                    }
                                    else if (double.Parse(StrKVA.ToString().Trim()) > 58.5)
                                    {
                                        // Commented by KB on 06/03/2025 as Stock will be Transaferred to Flat Pack Profitcenter  instead of Canopy Assembly
                                        //sb.Remove(0, sb.Length);
                                        //sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                                        //sb.Append(" values('" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "','03.038',");
                                        //sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Dts[9].ToString().Trim() + "',0)");
                                        //cmd = new SqlCommand(sb.ToString(), con);
                                        //cmd.Transaction = tran;
                                        //cmd.ExecuteNonQuery();
                                        //cmd.Dispose();

                                        // Code added by KB on 06/03/2025 as Stock will be Transaferred to Flat Pack Profitcenter instead of Canopy Assembly
                                        sb.Remove(0, sb.Length);
                                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, ReceivedCode, ReceivedDate, ReceivedQty, StockType)");
                                        sb.Append(" values('" + Dts[1].ToString().Trim() + "','" + CpyPrcPCReq.PCCode.Trim() + "','01.093',");
                                        sb.Append("'" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Dts[9].ToString().Trim() + "',0)");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                    }
                                    //Rohan bhosale
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update CanopyPlanSerialNo set CPPCSerialStatus='D' where CPCode='" + Dts[0].ToString().Trim() + "' and Partcode='" + Dts[1].ToString().Trim() + "'  ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }


                                sb.Remove(0, sb.Length);
                                sb.Append("Update ProcessFeedbackDetailsSub set PCStatus='D' where PFBBOTSerialNo='" + Dts[0].Trim().Trim() + "' and Partcode='" + Dts[1].ToString().Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                            }


                        }

                        //****************User Acivity****************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", CpyPrcPCReq.EmpCode.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "PowderCoating Process");
                        cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcPCReq.PCCode.Substring(0, 2).Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    else if (Dts[10].Substring(0, 3) == "PSH")
                    {
                        Trans = "End";
                        if (cSub == 0)
                        {
                            AllPrcCode = Dts[10].Trim();
                        }
                        else
                        {
                            AllPrcCode = AllPrcCode + "," + Dts[10].Trim();
                        }

                        String StrKVA = ComCon.getTranName("Select Kva from Part where Partcode='" + Dts[1].ToString().Trim() + "' and active='1'", "PartKVA", "Kva", con, tran).ToString().Trim();

                        if (double.Parse(StrKVA.ToString().Trim()) >= 200)
                        {
                            // Commented by KB on 21/12/2023
                            //sb.Remove(0, sb.Length);
                            //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode, ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                            //sb.Append(" values('" + CpyPrcPCReq.PCCode.Trim() + "','" + Dts[3].ToString().Trim() + "',");
                            //sb.Append("'" + Dts[10].ToString().Trim() + "',GetDate(),'" + double.Parse(Dts[9].ToString().Trim()) + "','01.007',2)");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();
                        }

                        //SqlCommand cmd = new SqlCommand();

                        GrpPfbCode = "";
                        GrpPfbCode = ComCon.getTranName("select GroupPfbCode   from ProcessFeedBack where PFBCode='" + Dts[10].Trim() + "' group By GroupPfbCode ", "TblPCPrc", "GroupPfbCode", con, tran);

                        sb.Remove(0, sb.Length);
                        sb.Append("Update ProcessFeedBack set EDt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where PFBCode='" + Dts[10].Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                      
                    }
                }
                dsDetailsSub.Clear();
                //For stk Matching
                #region
                if (Trans == "Start")
                {
                    dsDetailsSub = ComCon.procTranDS("Exec GetPrcDtsForStk '" + GrpPfbCode + "'", "tbl_PCKitForStk", con, tran);

                    if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_PCKitForStk"].Rows.Count > 0)
                    {
                        int stk = 0;
                        for (int s = 0; s < dsDetailsSub.Tables["tbl_PCKitForStk"].Rows.Count; s++)
                        {

                            if (double.Parse(dsDetailsSub.Tables["tbl_PCKitForStk"].Rows[s]["PrcQty"].ToString().Trim()) >
                                (double.Parse(dsDetailsSub.Tables["tbl_PCKitForStk"].Rows[s]["Stock"].ToString().Trim()) + double.Parse(dsDetailsSub.Tables["tbl_PCKitForStk"].Rows[s]["PrcQty"].ToString().Trim())))
                            {

                                if (stk == 0)
                                {
                                    AllPrcCode = dsDetailsSub.Tables["tbl_PCKitForStk"].Rows[s]["Partdesc"].ToString().Trim();
                                    stk = 1;
                                }
                                else if (stk > 0)
                                {
                                    AllPrcCode = AllPrcCode + "," + dsDetailsSub.Tables["tbl_PCKitForStk"].Rows[s]["Partdesc"].ToString().Trim();
                                }
                            }

                        }
                        if (stk > 0)
                        {
                            AllPrcCode = "In sufficient Stock For Part: " + AllPrcCode;
                            return AllPrcCode;

                        }
                    }
                }


                #endregion
                //For stk Matching

                //Action Taken File Attachment
                #region
                if (Trans == "End")
                {
                    if (!string.IsNullOrEmpty(CpyPrcPCReq.AttachFileDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(CpyPrcPCReq.AttachFileDts, "@#@");
                        SrNoA = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNoA += 1;
                            DtsA = Regex.Split(StrSub.ToString().Trim(), "-->");

                            string FileName = GrpPfbCode.ToString().Trim().Substring(4, 5).Trim() + GrpPfbCode.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNoA) + Path.GetExtension(DtsA[1].ToString().Trim());
                            String StrMpath = ComCon.getMainFilePath("TempPrcPC") + "/" + FileName.ToString().Trim();
                            string StrTpath = "C:/TempERPFile/TempPrcPC/" + CpyPrcPCReq.EmpCode.Trim() + "/" + DtsA[1].ToString().Trim();

                            string StrTempPath = "C:/TempERPFile/TempPrcPC/" + CpyPrcPCReq.EmpCode.Trim();
                            if (Directory.Exists(StrTempPath))
                            {
                                Directory.GetAccessControl(StrTpath);
                                File.Copy(StrTpath, StrMpath);
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessFeedbackFiles");
                            sb.Append("(GroupPFBCode,SrNo,FileName)");
                            sb.Append(" VALUES('" + GrpPfbCode.ToString().Trim() + "' ,'" + SrNoA + "','" + FileName.ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }
                }
                #endregion
                //Action Taken File Attachment

                 tran.Commit();
             
                //tran.Rollback();
                if (Trans == "Start")
                {
                    AllPrcCode = "ProcessCode =" + AllPrcCode + " For PowderCoating  Started SuccessFully ";
                }
                else if (Trans == "End")
                {
                    AllPrcCode = "ProcessCode =" + AllPrcCode + " For PowderCoating  Ended SuccessFully ";
                }

                return AllPrcCode;
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
        public string SubmitCanopyAssly(CpyPrcRequest CpyPrcReq)
        {

            //int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;

            string PrcNo = "";
            try
            {
                if (CpyPrcReq.PFBCode.Substring(0, 3) == "NEW")
                {
                    string CurrentMnth = ComCon.getName("select Mon=case MONTH(GETDATE()) " +
                                                        "when 1 then '01' when 2 then '02' when 3 then '03' " +
                                                        "when 4 then '04' when 5 then '05' when 6 then '06' " +
                                                        "when 7 then '07' when 8 then '08' when 9 then '09' " +
                                                        "when 10 then '10' when 11 then '11' when  12 then '12' end", "tblM", "Mon");
                    string[] strMachineNo = Regex.Split(CpyPrcReq.MachineCodeSrNo, "-->");
                    //ChkforStartCPY = getChkforStartCpy(CpyPrcReq.PCCode.Trim(), CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode);
                    //ChkforStart = getChkforStart(CpyPrcFabReq.PCCode.Trim(), CpyPrcFabReq.PlanCode, CpyPrcFabReq.ProductCode, strMachineNo[1].ToString());
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    string Getrate = "0";
                    Getrate = ComCon.getTranName("select Convert(nvarchar(10),Rate)+'-->'+Convert(nvarchar(10),PSqFt)+'-->'+Convert(nvarchar(10),PWt) as CPYDts From ProfitcenterPlDetails where ProfitcenterCode='01.005' and Partcode='" + CpyPrcReq.ProductCode.Trim() + "' ", "CpyRate", "CPYDts", con, tran);

                    string[] strCPYWtDts = Regex.Split(Getrate, "-->");
                    // Mst Entry
                    #region
                    PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcReq.PCCode.Trim().Substring(0, 2), con, tran);

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,ProductCode,CanopyPlanCode,TurretKitCode,");
                    sb.Append("PartCode,ProcessQty,CompanyCode,PFBRate,PPWCode,Remark,WtPerUt,SqftPerUt,NstWtPerUt,NstSqftPerUt)");
                    sb.Append(" values('" + PrcNo.Trim() + "','" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "', ");
                    sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                    sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcReq.PCCode.Trim() + "','" + CpyPrcReq.ProductCode.Trim() + "',");
                    sb.Append("'" + CpyPrcReq.PlanCode.Trim() + "','" + CpyPrcReq.BOMcode.Trim() + "','" + CpyPrcReq.ProductCode.Trim() + "','" + CpyPrcReq.PrcQty + "','" + CpyPrcReq.PCCode.Trim().Substring(0, 2) + "', ");
                    sb.Append("'" + double.Parse(strCPYWtDts[0].Trim()) + "','" + CpyPrcReq.EmpCode.Trim() + "','Nil','" + double.Parse(strCPYWtDts[2].Trim()) + "','" + double.Parse(strCPYWtDts[1].Trim()) + "',");
                    sb.Append("'" + Math.Round(CpyPrcReq.PrcQty * double.Parse(strCPYWtDts[2].Trim()), 2) + "','" + Math.Round(CpyPrcReq.PrcQty * double.Parse(strCPYWtDts[1].Trim()), 2) + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //sb.Remove(0, sb.Length);
                    //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType)");
                    //sb.Append(" values('" + CpyPrcReq.PCCode.Trim() + "','" + CpyPrcReq.ProductCode.Trim() + "',");
                    //sb.Append("'" + PrcNo.Trim() + "', GetDate(),'" + CpyPrcReq.PrcQty.ToString().Trim() + "','01.005','0')");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    // Mst Entry
                    #endregion
                    // Mst Entry

                    // Dts Entry
                    #region
                    int recCount = ComCon.CountChars(CpyPrcReq.PrcDts, ",");
                    string[] strPrcDts = Regex.Split(CpyPrcReq.PrcDts, ",");
                    int SrNo = 0;

                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {
                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");
                        //Dts
                        #region
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                        sb.Append("PFBRate,WtPerUt,SqftPerUt)");
                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + Dts[0].Trim() + "',");//Partcode
                        sb.Append("'" + Convert.ToDouble(Dts[1].Trim()) + "',");//KitQty
                        sb.Append("'" + Convert.ToDouble(Dts[2].Trim()) + "',");//PrcQty
                        sb.Append("'" + Convert.ToDouble(Dts[3].Trim()) + "',");//RateQty
                        sb.Append("'" + Convert.ToDouble(Dts[4].Trim()) + "',");//Wt
                        sb.Append("'" + Convert.ToDouble(Dts[5].Trim()) + "')");//Sqft
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                        sb.Append(" values('" + CpyPrcReq.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + CpyPrcReq.PCCode.Trim() + "',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        #endregion
                        //Dts

                        //KitBelow 1000
                        #region
                        //string GetMaxRate = "0";
                        string GetMaxRatePartCode = "0";
                        string PrcBelowRate = "";
                        //GetMaxRate = ComCon.getTranName("Select Isnull(Max(Rate),0) as MRate From CanopyplandtsSub where CPCode='" + CpyPrcReq.PlanCode + "' and CpyPartcode='" + CpyPrcReq.ProductCode + "' ", "tbl_ChkForMaxRate", "MRate", con, tran);
                        //if (double.Parse(GetMaxRate.Trim()) == double.Parse(Dts[3].Trim()))


                        GetMaxRatePartCode = ComCon.getTranName("Select top 1 PartCode From CanopyplandtsSub where CPCode='" + CpyPrcReq.PlanCode + "' and CpyPartcode='" + CpyPrcReq.ProductCode + "'  order by rate desc  ", "tbl_ChkForMaxRate", "PartCode", con, tran);

                        //GetMaxRatePartCode = ComCon.getTranName("Select top 1 PartCode From CanopyplandtsSub where CPCode='" + Dts[0].Trim() + "' and CpyPartcode='" + Dts[1].Trim() + "' and CatID='" + Dts[13].Trim() + "' order by rate desc ", "tbl_ChkForMaxRate", "PartCode", con, tran);

                        if (GetMaxRatePartCode.Trim() == Dts[0].Trim())

                        {
                            dsKitbelowRate = ComCon.procTranDS("select Cd.Partcode,AliseName,(select Convert(nvarchar(10),PurRate)+'-->'+Convert(nvarchar(10),Rate)+'-->'+Convert(nvarchar(10),PWt)+'-->'+Convert(nvarchar(10),PSqft) " +
                            " from ProfitcenterPLDetails where  ProfitcenterCode = '01.007' and Partcode=cd.partcode) as PartDts, " +
                            " (select Round(Isnull(Sum(Recqty) - sum(IssueQty), 0), 00) as Stk From ( select Sum(ReceivedQty) as Recqty, " +
                            " 0.00 as IssueQty from stockwip where ToProfitcenterCode = '" + CpyPrcReq.PCCode.Trim() + "' and StockType = '0' " +
                            " and Partcode = cd.Partcode and  ReceivedQty > 0   Union all " +
                            " select 0.00 as Recqty, sum(IssueQty) as IssueQty from stockwip where FromProfitcenterCode ='" + CpyPrcReq.PCCode.Trim() + "' and StockType = '0' " +
                            "  and Partcode = cd.Partcode and  IssueQty > 0) as stk) as Stock " +
                            " From CanopyPlanDtsSubBelowStdRate cd Inner Join Part P On Cd.Partcode=P.partcode where CpyPartcode='" + CpyPrcReq.ProductCode + "' and CPCode='" + CpyPrcReq.PlanCode + "' ", "tbl_KitbelowRate", con, tran);
                            if (dsKitbelowRate != null && dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count > 0)
                            {
                                int ChkStk = 0;
                                for (int br = 0; br < dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows.Count; br++)
                                {
                                    string[] DtsBR = Regex.Split(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["PartDts"].ToString().Trim(), "-->");

                                    if (Convert.ToDouble(CpyPrcReq.PrcQty) >
                                    Convert.ToDouble(dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Stock"].ToString().Trim()))
                                    {
                                        if (ChkStk == 0)
                                        {
                                            PrcNo = dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["AliseName"].ToString().Trim();
                                            ChkStk = 1;
                                        }
                                        else
                                        {
                                            PrcNo = PrcNo + "," + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["AliseName"].ToString().Trim();
                                        }
                                    }
                                    if (ChkStk == 0)
                                    {
                                        // Mst Entry
                                        #region
                                        //PrcBelowRate = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), CpyPrcReq.PCCode.Trim().Substring(0, 2), con, tran);
                                        //sb.Remove(0, sb.Length);
                                        //sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,CpyStageType,MaxSrNo,Dt,EDt,Yr,MachineCode,SerialNo,ProfitCenterCode,CanopyPlanCode,ProductCode,TurretKitCode,");
                                        //sb.Append("PartCode,NestingForCode,NestingForQty,SqftPerUt,WtPerUt,PFBRate,ProcessQty,NstWtPerUt,NstSqftPerUt,PPWCode,CompanyCode,Remark)");
                                        //sb.Append(" values('" + PrcNo.Trim() + "','" + PrcBelowRate.Trim() + "','" + PrcNo.Trim() + "','" + (PrcBelowRate.Substring(10, 8)) + "', ");
                                        //sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Null,");
                                        //sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + strMachineNo[0].ToString() + "','" + strMachineNo[1].ToString() + "','" + CpyPrcReq.PCCode.Trim() + "',");
                                        //sb.Append("'" + CpyPrcReq.PlanCode + "','" + CpyPrcReq.ProductCode + "','" + CpyPrcReq.BOMcode + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "','" + CpyPrcReq.PrcQty + "', ");
                                        //sb.Append("'" + DtsBR[3].Trim() + "','" + DtsBR[2].Trim() + "','" + DtsBR[1].Trim() + "','" + CpyPrcReq.PrcQty + "',");
                                        //sb.Append("'" + double.Parse(DtsBR[2].Trim()) * double.Parse(CpyPrcReq.PrcQty.ToString()) + "','" + double.Parse(DtsBR[3].Trim()) * double.Parse(CpyPrcReq.PrcQty.ToString()) + "',");
                                        //sb.Append("'" + CpyPrcReq.EmpCode.Trim() + "','01','Nil')");
                                        //cmd = new SqlCommand(sb.ToString(), con);
                                        //cmd.Transaction = tran;
                                        //cmd.ExecuteNonQuery();
                                        //cmd.Dispose();

                                        sb.Remove(0, sb.Length);
                                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                        sb.Append(" values('" + CpyPrcReq.PCCode.Trim() + "','" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim() + "',");
                                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + CpyPrcReq.PrcQty + "','" + CpyPrcReq.PCCode.Trim() + "',0)");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        #endregion
                                        // Mst Entry

                                        // Dts Entry
                                        #region
                                        sb.Remove(0, sb.Length);
                                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,");
                                        sb.Append("PFBRate,SaleRate,WtPerUt,SqftPerUt)");
                                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                                        sb.Append("'" + dsKitbelowRate.Tables["tbl_KitbelowRate"].Rows[br]["Partcode"].ToString().Trim().Trim() + "',");
                                        sb.Append("'1',");
                                        sb.Append("'" + Convert.ToDouble(CpyPrcReq.PrcQty) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[0].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[1].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[2].Trim()) + "',");
                                        sb.Append("'" + Convert.ToDouble(DtsBR[3].Trim()) + "')");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                        #endregion
                                        // Dts Entry
                                    }
                                }

                                if (ChkStk == 1)
                                {
                                    PrcNo = "In sufficient Stock(BR) For Part: " + PrcNo;
                                    return PrcNo;
                                }
                            }
                        }
                        #endregion
                        //KitBelow 1000
                    }

                    #endregion
                    // Dts Entry

                    //SrNo  RB cmd 28/03/2025
                    #region
                    //for (int m = 0; m < Convert.ToDouble(CpyPrcReq.PrcQty); m++)
                    //{
                    //    int GetCpyMax = Convert.ToInt32(ComCon.getTranName("SELECT ISNULL(MaxValue,0) as MaxValue FROM getMaxSerialNo WHERE CompCode='01' AND Prefix='CPY' AND Yr='" + ComCon.yearEnd(con, tran) + "' ", "tblMx", "MaxValue", con, tran));
                    //    int GetBfmMax = Convert.ToInt32(ComCon.getTranName("SELECT ISNULL(MaxValue,0) as MaxValue FROM getMaxSerialNo WHERE CompCode='01' AND Prefix='BFM' AND Yr='" + ComCon.yearEnd(con, tran) + "' ", "tblMx", "MaxValue", con, tran));
                    //    int GetFltMax = Convert.ToInt32(ComCon.getTranName("SELECT ISNULL(MaxValue,0) as MaxValue FROM getMaxSerialNo WHERE CompCode='01' AND Prefix='FTK' AND Yr='" + ComCon.yearEnd(con, tran) + "' ", "tblMx", "MaxValue", con, tran));
                    //    string strCpymax = "0";
                    //    string strBfmmax = "0";
                    //    string strFtkmax = "0";
                    //    string CpySerialNo = "", BfmSerialNo = "", FtkSerialNo = "";

                    //    //Cpy
                    //    #region
                    //    if (GetCpyMax == 0)
                    //        strCpymax = "0001";
                    //    else if (GetCpyMax <= 9)
                    //        strCpymax = "000" + (GetCpyMax + 1);
                    //    else if (GetCpyMax <= 99)
                    //        strCpymax = "00" + (GetCpyMax + 1);
                    //    else if (GetCpyMax <= 999)
                    //        strCpymax = "0" + (GetCpyMax + 1);
                    //    else
                    //    {
                    //        strCpymax = Convert.ToString(GetCpyMax + 1);
                    //    }
                    //    CpySerialNo = "CPY" + DateTime.Now.Year.ToString().Substring(2, 2) + CurrentMnth.Trim() + "01" + strCpymax;

                    //    if (double.Parse(strCpymax.Trim()) > 0)
                    //    {
                    //        sb.Remove(0, sb.Length);
                    //        sb.Append("UPDATE getMaxSerialNo SET MaxValue='" + (GetCpyMax + 1) + "' WHERE ");
                    //        sb.Append("CompCode='01' AND Yr='" + ComCon.yearEnd(con, tran) + "' AND Prefix='CPY' ");
                    //        cmd = new SqlCommand(sb.ToString(), con);
                    //        cmd.Transaction = tran;
                    //        cmd.ExecuteNonQuery();
                    //        cmd.Dispose();
                    //    }
                    //    #endregion
                    //    //Cpy

                    //    //Base
                    //    #region
                    //    if (GetBfmMax == 0)
                    //        strBfmmax = "0001";
                    //    else if (GetBfmMax <= 9)
                    //        strBfmmax = "000" + (GetBfmMax + 1);
                    //    else if (GetBfmMax <= 99)
                    //        strBfmmax = "00" + (GetBfmMax + 1);
                    //    else if (GetBfmMax <= 999)
                    //        strBfmmax = "0" + (GetBfmMax + 1);
                    //    else
                    //    {
                    //        strBfmmax = Convert.ToString(GetBfmMax + 1);
                    //    }
                    //    BfmSerialNo = "BFM" + DateTime.Now.Year.ToString().Substring(2, 2) + CurrentMnth.Trim() + "01" + strBfmmax;
                    //    if (double.Parse(strBfmmax.Trim()) > 0)
                    //    {
                    //        sb.Remove(0, sb.Length);
                    //        sb.Append("UPDATE getMaxSerialNo SET MaxValue='" + (GetBfmMax + 1) + "' WHERE ");
                    //        sb.Append("CompCode='01' AND Yr='" + ComCon.yearEnd(con, tran) + "' AND Prefix='BFM' ");
                    //        cmd = new SqlCommand(sb.ToString(), con);
                    //        cmd.Transaction = tran;
                    //        cmd.ExecuteNonQuery();
                    //        cmd.Dispose();
                    //    }
                    //    #endregion
                    //    //Base

                    //    //Fuel Tank
                    //    #region
                    //    if (GetFltMax == 0)
                    //        strFtkmax = "0001";
                    //    else if (GetFltMax <= 9)
                    //        strFtkmax = "000" + (GetFltMax + 1);
                    //    else if (GetFltMax <= 99)
                    //        strFtkmax = "00" + (GetFltMax + 1);
                    //    else if (GetFltMax <= 999)
                    //        strFtkmax = "0" + (GetFltMax + 1);
                    //    else
                    //    {
                    //        strFtkmax = Convert.ToString(GetFltMax + 1);
                    //    }
                    //    FtkSerialNo = "FTK" + DateTime.Now.Year.ToString().Substring(2, 2) + CurrentMnth.Trim() + "01" + strFtkmax;
                    //    if (double.Parse(strFtkmax.Trim()) > 0)
                    //    {
                    //        sb.Remove(0, sb.Length);
                    //        sb.Append("UPDATE getMaxSerialNo SET MaxValue='" + (GetFltMax + 1) + "' WHERE ");
                    //        sb.Append("CompCode='01' AND Yr='" + ComCon.yearEnd(con, tran) + "' AND Prefix='FTK' ");
                    //        cmd = new SqlCommand(sb.ToString(), con);
                    //        cmd.Transaction = tran;
                    //        cmd.ExecuteNonQuery();
                    //        cmd.Dispose();
                    //    }
                    //    //Fuel Tank
                    //    #endregion
                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo,PFBBOTSerialNo,BFMSrNo,FLKSrNo,Status,QPCStatus, RWStatus) ");
                    //    sb.Append("Values('" + PrcNo.Trim() + "','" + (m + 1) + "','" + CpyPrcReq.ProductCode + "',");
                    //    sb.Append("'" + CpySerialNo.Trim() + "','" + CpySerialNo.Trim() + "','" + BfmSerialNo.Trim() + "','" + FtkSerialNo.Trim() + "','P','OK','OK')");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();


                    //}
                    #endregion
                    //SrNo

                    //  RB Added CanopyPlanSerialNo  CPYSerialStatus  

                    //sb.Remove(0, sb.Length);
                    //sb.Append("Update CanopyPlanSerialNo set CPYSerialStatus='D' where CPCode='" + CpyPrcReq.PCCode.Trim() + "' and Partcode='" + CpyPrcReq.ProductCode.Trim() + "'  ");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();





                    #region Insert Serial No from Canopy Plan. RB 08/04/2025

                    dsCPYSerialNo = ComCon.procTranDS("Exec GetCPYSerialNo '" + CpyPrcReq.ProductCode.Trim() + "', '" + CpyPrcReq.PrcQty + "' ", "tbl_CPYSerialNo", con, tran);
                    for (int m = 0; m < Convert.ToDouble(CpyPrcReq.PrcQty); m++)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo,PFBBOTSerialNo,BFMSrNo,FLKSrNo,Status,QPCStatus, RWStatus) ");
                        sb.Append("Values('" + PrcNo.Trim() + "','" + (m + 1) + "','" + CpyPrcReq.ProductCode + "',");
                        sb.Append("'" + dsCPYSerialNo.Tables["tbl_CPYSerialNo"].Rows[m]["SerialNo"].ToString().Trim() + "',");
                        sb.Append("'" + dsCPYSerialNo.Tables["tbl_CPYSerialNo"].Rows[m]["SerialNo"].ToString().Trim() + "',");
                        sb.Append("'" + dsCPYSerialNo.Tables["tbl_CPYSerialNo"].Rows[m]["BFMSrNo"].ToString().Trim() + "',");
                        sb.Append("'" + dsCPYSerialNo.Tables["tbl_CPYSerialNo"].Rows[m]["FLKSrNo"].ToString().Trim() + "',");
                        sb.Append("'P','OK','OK')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //  RB 08/04/2025
                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanSerialNo set CPYSerialStatus='D' where SerialNo='" + dsCPYSerialNo.Tables["tbl_CPYSerialNo"].Rows[m]["SerialNo"].ToString().Trim() + "' and Partcode='" + CpyPrcReq.ProductCode.Trim() + "'  ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    #endregion 





                    //Chk Assly Stk
                    #region
                    //  dsDetailsSub = ComCon.procTranDS("Exec GetPCKit '" + CpyPrcReq.BOMcode.Trim() + "', '" + CpyPrcReq.PCCode.Trim() + "'", "tbl_AsslyKit", con, tran);
                    dsDetailsSub = ComCon.procTranDS("Exec GetPCKit '" + CpyPrcReq.BOMcode.Trim() + "', '" + CpyPrcReq.PCCode.Trim() + "', '029'", "tbl_AsslyKit", con, tran);


                    if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_AsslyKit"].Rows.Count > 0)
                    {
                        string ChkStk = "";
                        for (int k = 0; k < dsDetailsSub.Tables["tbl_AsslyKit"].Rows.Count; k++)
                        {

                            if (Math.Round(double.Parse(dsDetailsSub.Tables["tbl_AsslyKit"].Rows[k]["Qty"].ToString().Trim()) * double.Parse(CpyPrcReq.PrcQty.ToString()), 2) >
                                    Math.Round(double.Parse(dsDetailsSub.Tables["tbl_AsslyKit"].Rows[k]["Stock"].ToString().Trim()), 2))
                            {
                                if (ChkStk == "")
                                {
                                    ChkStk = dsDetailsSub.Tables["tbl_AsslyKit"].Rows[k]["Partdesc"].ToString().Trim();
                                }
                                else
                                {
                                    ChkStk = ChkStk + "," + dsDetailsSub.Tables["tbl_AsslyKit"].Rows[k]["Partdesc"].ToString().Trim();
                                }
                            }
                            if (ChkStk == "")
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,SaleRate)");
                                sb.Append("values('" + PrcNo.Trim() + "','" + (SrNo + 1) + "',");
                                sb.Append("'" + dsDetailsSub.Tables["tbl_AsslyKit"].Rows[k]["PartCode"].ToString().Trim() + "',");
                                sb.Append("'" + dsDetailsSub.Tables["tbl_AsslyKit"].Rows[k]["Qty"].ToString().Trim() + "',");
                                sb.Append("'" + double.Parse(dsDetailsSub.Tables["tbl_AsslyKit"].Rows[k]["Qty"].ToString().Trim()) * double.Parse(CpyPrcReq.PrcQty.ToString()) + "',");
                                sb.Append("'" + dsDetailsSub.Tables["tbl_AsslyKit"].Rows[k]["SuppRate"].ToString().Trim() + "')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                sb.Remove(0, sb.Length);
                                sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                                sb.Append(" values('" + CpyPrcReq.PCCode.Trim() + "','" + dsDetailsSub.Tables["tbl_AsslyKit"].Rows[k]["PartCode"].ToString().Trim() + "',");
                                sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(dsDetailsSub.Tables["tbl_AsslyKit"].Rows[k]["Qty"].ToString().Trim()) * double.Parse(CpyPrcReq.PrcQty.ToString()) + "','" + CpyPrcReq.PCCode.Trim() + "',0)");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }

                        if (ChkStk != "")
                        {
                            PrcNo = "In sufficient Stock(Assly Kit) For Part: " + ChkStk;
                            return PrcNo;
                        }
                    }
                    #endregion
                    //Chk Assly Stk

                    //Update Plan
                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("Update CanopyPlanDetails set CPYWIPQty=CPYWIPQty + '" + CpyPrcReq.PrcQty + "', CPYWOPQty=CPYWOPQty + '" + CpyPrcReq.PrcQty + "'  where CPCode='" + CpyPrcReq.PlanCode.Trim() + "' and Partcode='" + CpyPrcReq.ProductCode.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    string cntPrcQty = "0";
                    cntPrcQty = ComCon.getTranName("select Qty-CPYWIPQty as BalQty from CanopyPlanDetails where CPCode='" + CpyPrcReq.PlanCode.Trim() + "' and Partcode='" + CpyPrcReq.ProductCode.Trim() + "'", "CPYPrc", "BalQty", con, tran);
                    if (cntPrcQty == "0")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update CanopyPlanDetails set CPYWIPStatus='D',CPYWOPStatus='D' where CPCode='" + CpyPrcReq.PlanCode.Trim() + "' and Partcode='" + CpyPrcReq.ProductCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //Kanban Done By fs 08/08/2024
                        #region
                        strKanBan = "";
                        string GetMaxValue = "";
                        dsKanBan = ComCon.procTranDS("exec InternalTOCReq '" + CpyPrcReq.PCCode.Trim() + "' ", "tbl_RaiseReqDtsKanBan", con, tran);
                        if (dsKanBan != null && dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows.Count > 0)
                        {
                            // Master Req
                            #region
                            GetMaxValue = "";

                            GetMaxValue = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", CpyPrcReq.PCCode.Substring(0, 2), con, tran);
                            strKanBan = GetMaxValue;


                            sb.Remove(0, sb.Length);
                            sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                                " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                            sb.Append("values('" + strKanBan.Trim() + "','" + GetMaxValue.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                            sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + CpyPrcReq.PCCode.Trim() + "','23.001','" + CpyPrcReq.ProductCode + "','" + CpyPrcReq.PCCode.Substring(0, 2) + "','" + CpyPrcReq.BatchQty.ToString().Trim() + "','P','WIP',");
                            sb.Append("'Auto Req For Plan No: " + CpyPrcReq.ProductCode + " and Prc No: " + PrcNo + "','1','1','1','KanBan','0')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();


                            #endregion
                            //Master Req
                            //Details Req
                            #region


                            int SrNoReq = 0;

                            for (int cntd = 0; cntd < dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows.Count; cntd++)
                            {
                                SrNoReq = SrNoReq + 1;
                                cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@REQCode", strKanBan);
                                cmd.Parameters.AddWithValue("@SrNo", SrNoReq);
                                cmd.Parameters.AddWithValue("@PartCode", dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows[cntd]["Partcode"].ToString().Trim());
                                cmd.Parameters.AddWithValue("@Qty", double.Parse(dsKanBan.Tables["tbl_RaiseReqDtsKanBan"].Rows[cntd]["RaiseReqQty"].ToString().Trim()));
                                cmd.Parameters.AddWithValue("@REQStatus", "P");
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();


                            }
                            #endregion
                            //Details Req
                        }



                        //DateTime.Now.ToString("yyyy-MM-dd")
                        //*********************User Acivity ***************************
                        cmd = new SqlCommand("insertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@EmpID", CpyPrcReq.EmpCode);
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "MaterialRequisitionWithoutPlan");
                        cmd.Parameters.AddWithValue("@TransactionNo", strKanBan);
                        cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcReq.PCCode.Substring(0, 2).Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        #endregion
                        //Kanban Done By fs 08/08/2024

                    }

                    #endregion
                    //Update Plan

                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", CpyPrcReq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "Canopy Assembly Process");
                    cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", CpyPrcReq.PCCode.Substring(0, 2).Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    tran.Commit();

                     //tran.Rollback();
                    PrcNo = "ProcessCode=" + PrcNo + " For Canopy Assembly  Started SuccessFully ";
                }
                else if (CpyPrcReq.PFBCode.Substring(0, 3) == "PSH")
                {
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    tran = con.BeginTransaction();

                    string SerialNo = "";
                    dsDetailsSub = ComCon.procTranDS("select Top ( " + CpyPrcReq.PrcQty + " )  SerialNo From ProcessFeedbackDetailsSub where PFbCode = '" + CpyPrcReq.PFBCode.Trim() + "' and EdtD is Null Order By SerialNo", "tbl_SerialNo", con, tran);

                    if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_SerialNo"].Rows.Count > 0)
                    {
                        SerialNo = "";
                        for (int Sr = 0; Sr < dsDetailsSub.Tables["tbl_SerialNo"].Rows.Count; Sr++)
                        {
                            if (Sr == 0)
                            {
                                SerialNo = dsDetailsSub.Tables["tbl_SerialNo"].Rows[Sr]["SerialNo"].ToString().Trim();
                            }
                            else
                            {
                                SerialNo = SerialNo + "-->" + dsDetailsSub.Tables["tbl_SerialNo"].Rows[Sr]["SerialNo"].ToString().Trim();
                            }
                        }
                    }
                    // string[] strSerialNo = Regex.Split(SerialNo.Trim(), "-->");

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedbackDetailsSub set EdtD='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where PFbCode = '" + CpyPrcReq.PFBCode.Trim() + "' and EdtD is Null and SerialNo in ( ");
                    sb.Append("select Top ( " + CpyPrcReq.PrcQty + " ) SerialNo From ProcessFeedbackDetailsSub where PFbCode = '" + CpyPrcReq.PFBCode.Trim() + "' and EdtD is Null Order By SerialNo) ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //SqlCommand cmd = new SqlCommand();
                    //sb.Remove(0, sb.Length);
                    //sb.Append("Update ProcessFeedbackDetailsSub set EdtD='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',CAStatus='D' where  PFBBOTSerialNo = '" + CpyPrcReq.PlanCode.Trim() + "' and EdtD is Null and Partcode='" + CpyPrcReq.ProductCode + "'");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    string cntSPrcQty = "0";
                    cntSPrcQty = ComCon.getTranName("select ProcessQty as SPrcQty from ProcessFeedback where PFBCode='" + CpyPrcReq.PFBCode.Trim() + "'", "TblSPrcQty", "SPrcQty", con, tran);

                    string cntEPrcQty = "0";
                   cntEPrcQty = ComCon.getTranName("select Count(*) as EPrcQty from ProcessFeedbackDetailsSub where PFBCode='" + CpyPrcReq.PFBCode.Trim() + "'  and EdtD is Not Null ", "TblEPrcQty", "EPrcQty", con, tran);
                   // cntEPrcQty = ComCon.getTranName("select Count(*) as EPrcQty from ProcessFeedbackDetailsSub where PFBBOTSerialNo = '" + CpyPrcReq.PlanCode.Trim() + "'  and EdtD is Not Null and Partcode='" + CpyPrcReq.ProductCode + "' ", "TblEPrcQty", "EPrcQty", con, tran);

                    if (int.Parse(cntSPrcQty) == int.Parse(cntEPrcQty))
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update ProcessFeedBack set EDt='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where PFBCode='" + CpyPrcReq.PFBCode.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    // Commented by KB on 21/12/2023
                    //sb.Remove(0, sb.Length);
                    //sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,LotNo)");
                    //sb.Append(" values('" + CpyPrcReq.PCCode.Trim() + "','" + CpyPrcReq.ProductCode.Trim() + "',");
                    //sb.Append("'" + CpyPrcReq.PFBCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + CpyPrcReq.PrcQty.ToString().Trim() + "','" + CpyPrcReq.PCCode.Trim() + "','0','" + SerialNo + "')");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                   

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO ProductWip(ProductCode, FromPCCode, ToPCCode, IssueCode, IssueDate, IssueQty, StockType)");
                        sb.Append(" values('" + CpyPrcReq.ProductCode.Trim() + "','" + CpyPrcReq.PCCode.Trim() + "','" + CpyPrcReq.PCCode.Trim() + "',");
                        sb.Append("'" + CpyPrcReq.PFBCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + CpyPrcReq.PrcQty + "',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                   
                    //Action Taken File Attachment
                    #region
                    if (!string.IsNullOrEmpty(CpyPrcReq.AttachFileDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(CpyPrcReq.AttachFileDts, "@#@");
                        SrNoA = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNoA += 1;
                            DtsA = Regex.Split(StrSub.ToString().Trim(), "-->");

                            string FileName = CpyPrcReq.PFBCode.ToString().Trim().Substring(4, 5).Trim() + CpyPrcReq.PFBCode.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNoA) + Path.GetExtension(DtsA[1].ToString().Trim());
                            String StrMpath = ComCon.getMainFilePath("TempPrcCpy") + "/" + FileName.ToString().Trim();
                            string StrTpath = "C:/TempERPFile/TempPrcCpy/" + CpyPrcReq.EmpCode.Trim() + "/" + DtsA[1].ToString().Trim();

                            string StrTempPath = "C:/TempERPFile/TempPrcCpy/" + CpyPrcReq.EmpCode.Trim();
                            if (Directory.Exists(StrTempPath))
                            {
                                Directory.GetAccessControl(StrTpath);
                                File.Copy(StrTpath, StrMpath);
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessFeedbackFiles");
                            sb.Append("(GroupPFBCode,SrNo,FileName)");
                            sb.Append(" VALUES('" + CpyPrcReq.PFBCode.ToString().Trim() + "' ,'" + SrNoA + "','" + FileName.ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }

                    #endregion
                    //Action Taken File Attachment


                    tran.Commit();
                   //tran.Rollback();
                    PrcNo = "ProcessCode=" + CpyPrcReq.PFBCode.Trim() + " For Canopy Assembly  End SuccessFully ";
                }
                return PrcNo;
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

        public DataTable getRevPCCode(string StrTransType)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("RevPCCode", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@ddlType", SqlDbType.Char).Value = StrTransType;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dsDetailsSub = new DataSet();
            dAd.Fill(dsDetailsSub);
            return dsDetailsSub.Tables[0];
        }
        public DataTable LoadRevPrcDts(string StrPCCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("RevTransCPY", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = StrPCCode;
            if (StrPCCode == "01.009")
            {
                dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "01.002";
            }
            if (StrPCCode == "03.061") // U 4 CNC 
            {
                dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "03.004";
            }
            //else if (StrPCCode == "01.076")
            //{
            //    dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "01.002";
            //}
            else if (StrPCCode == "01.002")
            {
                dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "01.008";
            }
            else if (StrPCCode == "03.004") // U4 Bending
            {
                dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "03.002";
            }
            else if (StrPCCode == "01.008")
            {
                dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "01.007";
            }
            else if (StrPCCode == "03.002") // U4 Fabrication
            {
                dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "03.001";
            }
            else if (StrPCCode == "03.001") // U4 Powder Coating
            {
                dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "03.038";
            }
            else if (StrPCCode == "01.007")
            {
                dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "01.005";
            }
            else if (StrPCCode == "01.005")
            {
                dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "01.005";
            }
            else if (StrPCCode == "03.038") // U4 Canopy Assembly
            {
                dAd.SelectCommand.Parameters.Add("@PCCodeNext", SqlDbType.Char).Value = "03.038";
            }
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dsDetailsSub = new DataSet();
            dAd.Fill(dsDetailsSub);
            return dsDetailsSub.Tables[0];
        }
        public string SubmitRevCpyTrans(CpyRevRequest CpyRevReq)
        {

            string PrcNo = "";
            try
            {
                string strNextPCCode = "", strPlanStatus = ""; ;

                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                if (CpyRevReq.PCCode.Trim() == "01.009")
                {
                    strNextPCCode = "01.076','01.002','01.008','01.007','01.005";
                    strPlanStatus = "CPTQty=0,CPTStatus='P',CPPartCutQty=0,CPPartCutStatus='P',CPBQty=0,CPBStatus='P',CPFQty=0,CPFStatus='P',CPPCQty=0,CPPCStatus='P' ";
                }
                if (CpyRevReq.PCCode.Trim() == "03.061")
                {
                    strNextPCCode = "01.076','03.004','03.002','03.001','03.038";
                    strPlanStatus = "CPTQty=0,CPTStatus='P',CPPartCutQty=0,CPPartCutStatus='P',CPBQty=0,CPBStatus='P',CPFQty=0,CPFStatus='P',CPPCQty=0,CPPCStatus='P' ";
                }
                //else if (CpyRevReq.PCCode.Trim() == "01.076")
                //    {
                //        strNextPCCode = "01.002','01.008','01.007','01.005";
                //        strPlanStatus = " CPPartCutQty=0,CPPartCutStatus='P',CPBQty=0,CPBStatus='P',CPFQty=0,CPFStatus='P',CPPCQty=0,CPPCStatus='P' ";
                //    }
                else if (CpyRevReq.PCCode.Trim() == "01.002")
                {
                    strNextPCCode = " 01.008','01.007','01.005";
                    strPlanStatus = " CPBQty=0,CPBStatus='P',CPFQty=0,CPFStatus='P',CPPCQty=0,CPPCStatus='P' ";
                }
                else if (CpyRevReq.PCCode.Trim() == "03.004")
                {
                    strNextPCCode = " 03.002','03.001','03.038";
                    strPlanStatus = " CPBQty=0,CPBStatus='P',CPFQty=0,CPFStatus='P',CPPCQty=0,CPPCStatus='P' ";
                }
                else if (CpyRevReq.PCCode.Trim() == "01.008")
                {
                    strNextPCCode = " 01.007','01.005";
                    strPlanStatus = " CPFQty=0,CPFStatus='P',CPPCQty=0,CPPCStatus='P' ";
                }
                else if (CpyRevReq.PCCode.Trim() == "03.002")
                {
                    strNextPCCode = " 03.001','03.038";
                    strPlanStatus = " CPFQty=0,CPFStatus='P',CPPCQty=0,CPPCStatus='P' ";
                }
                else if (CpyRevReq.PCCode.Trim() == "01.007")
                {
                    strNextPCCode = " 01.005";
                    strPlanStatus = " CPPCQty=0,CPPCStatus='P' ";
                }
                else if (CpyRevReq.PCCode.Trim() == "03.001")
                {
                    strNextPCCode = " 03.038";
                    strPlanStatus = " CPPCQty=0,CPPCStatus='P' ";
                }
                else if (CpyRevReq.PCCode.Trim() == "01.005")
                {
                    strNextPCCode = " 01.005";
                    strPlanStatus = " CpyWopQty='0',CpyWopStatus='P',CpyWipQty='0',CpyWipStatus='P' ";
                }
                else if (CpyRevReq.PCCode.Trim() == "03.038")
                {
                    strNextPCCode = " 03.038";
                    strPlanStatus = " CpyWopQty='0',CpyWopStatus='P',CpyWipQty='0',CpyWipStatus='P' ";
                }
                int recCount = ComCon.CountChars(CpyRevReq.RevPrcDts, ",");
                string[] strPrcDts = Regex.Split(CpyRevReq.RevPrcDts, ",");
                int SrNo = 0;

                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    // Mst Entry

                    #region
                    PrcNo = "";
                    SrNo += 1;
                    string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");
                    PrcNo = GetmaxPrc("CpyrevTrans", "REVCode", ComCon.yearEnd(con, tran), CpyRevReq.PCCode.Trim().Substring(0, 2), con, tran);
                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into CpyrevTrans(REVCode,MaxSrNo,Dt,Yr,PCCode,TransType,CPCode,ProductCode,CompanyCode ) ");
                    sb.Append(" values('" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ComCon.yearEnd(con, tran) + "', ");
                    sb.Append("'" + CpyRevReq.PCCode.Trim() + "','" + CpyRevReq.TransType.Trim() + "','" + Dts[0].Trim() + "', ");
                    sb.Append("'" + Dts[1].Trim() + "','" + CpyRevReq.PCCode.Trim().Substring(0, 2) + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion
                    // Mst Entry

                    //Update Prc Dts
                    #region
                    if (CpyRevReq.TransType.Trim() == "IndividualCode")
                    {

                        //StockWip
                        #region
                        // For StkWip issue Individual
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete From Stockwip where Issuecode in ( select PFBCode from ProcessFeedback where ProfitcenterCode = '" + CpyRevReq.PCCode + "' ");
                        sb.Append(" and CanopyPlanCode='" + Dts[0].Trim() + "' and ProductCode='" + Dts[1].Trim() + "') ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        // For StkWip issue Individual

                        // For StkWip issue Next
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete From Stockwip where Issuecode in ( select PFBCode from ProcessFeedback where ProfitcenterCode in ('" + strNextPCCode.Trim() + "') ");
                        sb.Append(" and CanopyPlanCode='" + Dts[0].Trim() + "' and ProductCode='" + Dts[1].Trim() + "') ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        // For StkWip issue Next

                        // For StkWip Received Individual
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete From Stockwip where ReceivedCode in ( select PFBCode from ProcessFeedback where ProfitcenterCode = '" + CpyRevReq.PCCode + "' ");
                        sb.Append(" and CanopyPlanCode='" + Dts[0].Trim() + "' and ProductCode='" + Dts[1].Trim() + "' ) ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        // For StkWip Individual

                        // For StkWip Received Next
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete From Stockwip where ReceivedCode in ( select PFBCode from ProcessFeedback where ProfitcenterCode in ('" + strNextPCCode.Trim() + "') ");
                        sb.Append(" and CanopyPlanCode='" + Dts[0].Trim() + "' and ProductCode='" + Dts[1].Trim() + "') ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        // For StkWip Next
                        #endregion
                        //StockWip

                        //PrdWip
                        #region

                        // For PrdWip issue Individual
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete From Productwip where issueCode in ( select PFBCode from ProcessFeedback where ProfitcenterCode = '" + CpyRevReq.PCCode + "' ");
                        sb.Append(" and CanopyPlanCode='" + Dts[0].Trim() + "' and ProductCode='" + Dts[1].Trim() + "' )");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        // For PrdWip Received Individual

                        // For PrdWip issue Next
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete From ProductWip where issueCode in ( select PFBCode from ProcessFeedback where ProfitcenterCode in ('" + strNextPCCode.Trim() + "') ");
                        sb.Append(" and CanopyPlanCode='" + Dts[0].Trim() + "' and ProductCode='" + Dts[1].Trim() + "' )");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        // For PrdWip Received Next

                        // For PrdWip Received Individual
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete From ProductWip where ReceivedCode in ( select PFBCode from ProcessFeedback where ProfitcenterCode = '" + CpyRevReq.PCCode + "' ");
                        sb.Append(" and CanopyPlanCode='" + Dts[0].Trim() + "' and ProductCode='" + Dts[1].Trim() + "') ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        // For PrdWip Received Individual

                        // For PrdWip Received Next
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete From ProductWip where ReceivedCode in ( select PFBCode from ProcessFeedback where ProfitcenterCode in ('" + strNextPCCode.Trim() + "') ");
                        sb.Append(" and CanopyPlanCode='" + Dts[0].Trim() + "' and ProductCode='" + Dts[1].Trim() + "' )");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        // For PrdWip Received Next
                        #endregion
                        //PrdWip
                        //Inactive Process
                        #region
                        // Partial Prc For Individual Prc
                        sb.Remove(0, sb.Length);
                        sb.Append("Update ProcessFeedback set Active='0'  where CanopyPlanCode='" + Dts[0].Trim() + "' and ProductCode='" + Dts[1].Trim() + "' and ProfitcenterCode='" + CpyRevReq.PCCode.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        // Partial Prc For Individual Prc

                        // Partial Prc For Nxt Prc
                        sb.Remove(0, sb.Length);
                        sb.Append("Update ProcessFeedback set Active='0'  where CanopyPlanCode='" + Dts[0].Trim() + "' and ProductCode='" + Dts[1].Trim() + "' and ProfitcenterCode in ('" + strNextPCCode.Trim() + "') ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        // Partial Prc For Nxt Prc
                        #endregion
                        //Inactive Process



                        //Update Plan
                        #region
                        if (CpyRevReq.PCCode == "01.005" || CpyRevReq.PCCode == "03.038")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update canopyPlanDetails set " + strPlanStatus + "  where ");
                            sb.Append("CPCode='" + Dts[0].Trim() + "' and PartCode='" + Dts[1].Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                        else
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update canopyPlanDtsSub set " + strPlanStatus + "  where ");
                            sb.Append("CPCode='" + Dts[0].Trim() + "' and CpyPartCode='" + Dts[1].Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                        if (CpyRevReq.PCCode == "01.009" || CpyRevReq.PCCode == "03.061")
                        {

                            sb.Remove(0, sb.Length);
                            sb.Append("Update TurretKitForPrc set PrcStatus='P',PartcutStatus='P' where ");
                            sb.Append("CPCode='" + Dts[0].Trim() + "' and CanopyPartCode='" + Dts[1].Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }



                        //else if (CpyRevReq.PCCode == "01.076")
                        //{
                        //    sb.Remove(0, sb.Length);
                        //    sb.Append("Update TurretKitForPrc set PartcutStatus='P' where ");
                        //    sb.Append("CPCode='" + Dts[0].Trim() + "' and CanopyPartCode='" + Dts[1].Trim() + "'");
                        //    cmd = new SqlCommand(sb.ToString(), con);
                        //    cmd.Transaction = tran;
                        //    cmd.ExecuteNonQuery();
                        //    cmd.Dispose();
                        //}

                        if (CpyRevReq.PCCode == "01.008" || CpyRevReq.PCCode == "03.002")
                        {

                            sb.Remove(0, sb.Length);
                            sb.Append("Update CanopyPlanOSDetails set OSFQty='0',OSFStatus='P' where ");
                            sb.Append("CPCode='" + Dts[0].Trim() + "' and CpyPartCode='" + Dts[1].Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }


                        #endregion
                        //Update Plan
                    }
                    #endregion
                    //Update Prc Dts

                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", CpyRevReq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "Canopy Assembly Process");
                    cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", CpyRevReq.PCCode.Substring(0, 2).Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                tran.Commit();

                // tran.Rollback();
                PrcNo = "ReverseCode=" + PrcNo + " For Reverse Saved SuccessFully ";


                return PrcNo;
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