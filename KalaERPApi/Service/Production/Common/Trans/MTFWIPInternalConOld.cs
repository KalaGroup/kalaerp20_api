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
using System.Web.Http;
using KalaERPApi.Controllers.Common.Trans;
using MTFWIPInternalRequest = KalaERPApi.Models.Request.MTFWIPInternalRequest;

namespace KalaERPApi.Service.Production.Common.Trans
{
    public class MTFWIPInternalCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        public SqlTransaction tran = null;
        DataSet dsDetailsSub = new DataSet();
        DataSet dsDetailsSub_Memo = new DataSet();
        DataSet dsDetailsSubV = new DataSet();
        DataSet dsDetailsSubV_Other = new DataSet();
        string strProc = "";
        string strSql = "";

        public object SqlDbTypeChar { get; private set; }

        public DataTable GetReqCodeAll(string FPCCode, string TPCCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetMTFWIPInternalReqCode", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@FPCCode", SqlDbType.Char).Value = FPCCode;
            dAd.SelectCommand.Parameters.Add("@TPCCode", SqlDbType.Char).Value = TPCCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable GetReqProdDetails(string ReqCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetReqProdDetails", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@ReqMTFCode", SqlDbType.Char).Value = ReqCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetReqDetails(string PCCode, string strBomCode, string StrReqCode, double StrReqQty, double StrMTFQty)
        {


            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();

            // string[] PCCode = Regex.Split(PCCode.Trim(), "-->");
            SqlCommand cmd = new SqlCommand();
            //#temp
            #region
            strProc = "";
            strProc =
            "CREATE TABLE #TempBOMDetails(Partcode nvarchar(50),Qty Float,SuppRate Float,KitCode nvarchar(50),length Float,Width Float,Thickness Float,lossWgt Float,MOB char(5)) " +
            "INSERT INTO #TempBOMDetails(PartCode,Qty,SuppRate,KitCode,length,Width,Thickness,lossWgt,MOB ) " +
            "SELECT Partcode,Qty,SuppRate,KitCode,length,Width,Thickness,lossWgt,MOB FROM BOMdetails WHERE BOMCode='" + strBomCode.Trim() + "' and MOB='B' ";
            cmd = new SqlCommand(strProc, con);
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            strSql = "";
            strSql = "SELECT * FROM #TempBOMDetails ";
            DataSet dsBOMDetails = new DataSet();
            SqlDataAdapter da1 = new SqlDataAdapter(strSql, con);
            da1.Fill(dsBOMDetails, "TempBOMDetails");

            strProc = "";
            strProc =
            "CREATE TABLE #stockSumStock(FromProfitCenterCode nvarchar(50),PartCode nvarchar(50)," +
            "IssueQty float,ToProfitCenterCode nvarchar(50),ReceivedQty float,stockType Int) " +
            "INSERT INTO #stockSumStock(FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType) " +
            "SELECT FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType " +
            "FROM stockwip WHERE fromprofitcentercode IN ('" + PCCode.Trim() + "') AND IssueQty>'0' AND " +
            "len(ToProfitCenterCode)='6' AND len(FromProfitCenterCode)='6' " +
            "INSERT INTO #stockSumStock(FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType) " +
            "SELECT FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType " +
            "FROM stockwip WHERE toprofitcentercode IN ('" + PCCode.Trim() + "') AND ReceivedQty>'0' AND " +
            "len(ToProfitCenterCode)='6' AND len(FromProfitCenterCode)='6' ";
            cmd = new SqlCommand(strProc, con);
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            strSql = "";
            strSql = "SELECT * FROM #stockSumStock ";
            DataSet dsStock = new DataSet();
            SqlDataAdapter da4 = new SqlDataAdapter(strSql, con);
            da4.Fill(dsStock, "stockSumStock");
            #endregion
            //#temp
            strProc = "";
            strProc = "select Partdesc+'-->'+R.Partcode as PartDesc,R.Partcode as PartCode,UName,Uid,0.000 as KitQty,Round(Qty,3) as ReqQty, " +
                                 " 0.000 as Pqty,0.00 as Stk,0.000 as MTFQty,0.000 as QtyAfterMTF,0.00 as Rate," +
                                 " 0.00 as Amt,0.000 as SheetQty,P.ConvUOMCode,P.Length,P.Width,P.Thickness " +
                                 " From MaterialRequisitionWithoutPlandetails R Inner Join Part P on R.Partcode=P.partcode " +
                                 " Inner join UOM U On P.UomCode=U.Uid " +
                                 "where ReqCode='" + StrReqCode.Trim() + "' and ReqStatus='P' ";

            cmd = new SqlCommand(strProc, con);
            DataSet dsFillData = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(dsFillData, "TempProcess");
            cmd.Dispose();
            if (dsFillData.Tables["TempProcess"].Rows.Count > 0)
            {

                for (int i = 0; i < dsFillData.Tables["TempProcess"].Rows.Count; i++)
                {
                    dsFillData.Tables["TempProcess"].Rows[i]["KitQty"] = Math.Round(double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["ReqQty"].ToString().Trim()) /
                                                                                                                                double.Parse(StrReqQty.ToString().Trim()), 3);

                    dsFillData.Tables["TempProcess"].Rows[i]["PQty"] = Math.Round(double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["KitQty"].ToString().Trim()) * double.Parse(StrMTFQty.ToString().Trim()), 3);
                    dsFillData.Tables["TempProcess"].Rows[i]["Stk"] = Math.Round(getWipStock(dsFillData.Tables["TempProcess"].Rows[i]["Partcode"].ToString().Trim(), 0), 2);

                    dsFillData.Tables["TempProcess"].Rows[i]["MtfQty"] = Math.Round(double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["PQty"].ToString().Trim()), 2);

                    dsFillData.Tables["TempProcess"].Rows[i]["QtyAfterMTF"] = Math.Round(double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["Stk"].ToString().Trim()) -
                                                                                                                                        double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["MtfQty"].ToString().Trim()), 3);

                    dsFillData.Tables["TempProcess"].Rows[i]["Rate"] = Math.Round(getPtRate(dsFillData.Tables["TempProcess"].Rows[i]["Partcode"].ToString().Trim(),
                                                                                                                                    dsFillData.Tables["TempProcess"].Rows[i]["ConvUOMCode"].ToString().Trim(),
                                                                                                                                    dsFillData.Tables["TempProcess"].Rows[i]["Uid"].ToString().Trim()), 2);

                    dsFillData.Tables["TempProcess"].Rows[i]["Amt"] = Math.Round(double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["Rate"].ToString().Trim()) *
                                                                                                                        double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["MtfQty"].ToString().Trim()), 2);

                    if (dsFillData.Tables["TempProcess"].Rows[i]["Partcode"].ToString().Trim().Substring(0, 5) == "00624" ||
                            dsFillData.Tables["TempProcess"].Rows[i]["Partcode"].ToString().Trim().Substring(0, 5) == "00625")
                    {

                        dsFillData.Tables["TempProcess"].Rows[i]["SheetQty"] = Math.Round((double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["MtfQty"].ToString().Trim())) /
                                                                Math.Round((double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["Length"].ToString().Trim()) * double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["Width"].ToString().Trim()) *
                                                                        double.Parse(dsFillData.Tables["TempProcess"].Rows[i]["Thickness"].ToString().Trim()) * 7.85 / 1000000), 3), 3);


                    }
                }
                //gvwPartDtls.DataSource = dsFillData.Tables["TempProcess"];
                //gvwPartDtls.DataBind();
            }

            ///***************** Drop all Temp tables *****************/

            sb.Remove(0, sb.Length);
            sb.Append("Drop table #TempBOMDetails;");
            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            sb.Remove(0, sb.Length);
            sb.Append("Drop table #stockSumStock;");
            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();
            cmd.Dispose();


            //Sorting the Table
            DataView dv = dsFillData.Tables["TempProcess"].DefaultView;
            dv.Sort = "Rate desc";
            DataTable sortedtable = dv.ToTable();

            // return dsFillData.Tables["TempProcess"];

            return sortedtable;
        }
        protected double getPtRate(string PartCode, string ConversionValue, string UOMCode)
        {
            double tempRate = 0;
            DataSet dsPtRate = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand();

            strProc = "";
            strProc = "SELECT TOP 1 SuppRate AS Rate FROM #TempBOMdetails WHERE PartCode='" + PartCode.Trim() + "'";
            cmd = new SqlCommand(strProc, con);
            adapter.SelectCommand = cmd;
            adapter.Fill(dsPtRate, "TempPricelist");
            cmd.Dispose();



            if (dsPtRate.Tables["TempPricelist"].Rows.Count > 0)
            {
                tempRate = double.Parse(dsPtRate.Tables["TempPricelist"].Rows[0]["Rate"].ToString().Trim());
            }
            else
            {
                strProc = "";
                strProc = "SELECT TOP 1 Rate,CostingCode FROM Supplierpricelistchanged " +
                                 "WHERE PartCode='" + PartCode.Trim() + "' AND ChangeDateTime=(SELECT max(ChangeDateTime) " +
                                 "FROM supplierpricelistchanged SPC " +
                                 "INNER JOIN PurchaseCosting PC ON SPC.CostingCode=PC.PCCode " +
                                 "WHERE SPC.PartCode='" + PartCode.Trim() + "' " +
                                 "AND (Rateselected='M' OR Rateselected='T') AND PC.Active='1' AND PC.Discard='1')";
                cmd = new SqlCommand(strProc, con);
                DataSet dsUom = new DataSet();
                adapter.SelectCommand = cmd;
                adapter.Fill(dsUom, "uom");
                cmd.Dispose();

                if (dsUom.Tables["uom"].Rows.Count > 0)
                {
                    if (UOMCode == getCostingUOM(dsUom.Tables["uom"].Rows[0]["CostingCode"].ToString().Trim()))
                    {
                        tempRate = double.Parse(dsUom.Tables["uom"].Rows[0]["Rate"].ToString().Trim());
                    }
                    else
                    {
                        tempRate = double.Parse(dsUom.Tables["uom"].Rows[0]["Rate"].ToString().Trim()) / double.Parse(ConversionValue);
                    }
                }

            }

            return tempRate;
        }
        protected string getCostingUOM(string CostingCode)
        {
            CommonCon ComCon = new CommonCon();
            string UOMCode = "";
            strProc = "";
            strProc = "SELECT UOMCode FROM PurchaseCosting WHERE PCCode='" + CostingCode.Trim() + "' AND Active='1'";
            UOMCode = ComCon.getName(strProc, "tblCost1", "UOMCode");
            if (UOMCode != "" || UOMCode != "0")
            {
                return UOMCode;
            }
            else
            {
                return "0";
            }
        }


        protected string getConsigneeCode(string ToPCCode)
        {
            string GetCustomerCode = "";

            if (ToPCCode.ToString().Trim().Substring(0, 2).Trim() == "01")
            {
                GetCustomerCode = "03.01.01.01.23.0001";
            }
            else if (ToPCCode.ToString().Trim().Substring(0, 2).Trim() == "02")
            {
                GetCustomerCode = "03.01.01.01.01.0001";
            }
            else if (ToPCCode.ToString().Trim().Substring(0, 2).Trim() == "03")
            {
                GetCustomerCode = "03.01.01.01.01.0002";
            }
            else if (ToPCCode.ToString().Trim().Substring(0, 2).Trim() == "04")
            {
                GetCustomerCode = "03.01.01.02.03.0001";
            }
            else if (ToPCCode.ToString().Trim().Substring(0, 2).Trim() == "05")
            {
                GetCustomerCode = "03.01.01.03.02.0001";
            }
            else if (ToPCCode.ToString().Trim().Substring(0, 2).Trim() == "10")
            {
                GetCustomerCode = "03.01.01.02.08.0009";
            }
            else if (ToPCCode.ToString().Trim().Substring(0, 2).Trim() == "14")
            {
                GetCustomerCode = "03.01.01.37.01.0119";
            }
            else
            {
                GetCustomerCode = "0";
            }

            return GetCustomerCode;
        }
        protected double getWipStock(string PartCode, int StockType)
        {
            //string[] PCCode = Regex.Split(txtProfitCenter.Text.Trim(), "-->");
            DataSet RecQty = new DataSet();
            DataSet IssueQty = new DataSet();
            double tempRecQty = 0;
            double tempIssueQty = 0;
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            strProc = "";
            strProc = "SELECT Sum(ReceivedQty) AS RecQty FROM #stockSumStock WHERE  PartCode='" + PartCode.Trim() + "' AND StockType='" + StockType + "' ";
            cmd = new SqlCommand(strProc, con);
            adapter.SelectCommand = cmd;
            adapter.Fill(RecQty, "stockSumStock");
            cmd.Dispose();

            strProc = "";
            strProc = "SELECT Sum(IssueQty) AS IssueQty FROM #stockSumStock WHERE PartCode='" + PartCode.Trim() + "' AND StockType='" + StockType + "' ";
            cmd = new SqlCommand(strProc, con);
            adapter.SelectCommand = cmd;
            adapter.Fill(IssueQty, "stockSumStock");
            cmd.Dispose();

            if (RecQty.Tables["stockSumStock"].Rows[0]["RecQty"].ToString().Trim() == "")
            {
                tempRecQty = 0;
            }
            else
            {
                tempRecQty = double.Parse(RecQty.Tables["stockSumStock"].Rows[0]["RecQty"].ToString().Trim());
            }

            if (IssueQty.Tables["stockSumStock"].Rows[0]["IssueQty"].ToString().Trim() == "")
            {
                tempIssueQty = 0;
            }
            else
            {
                tempIssueQty = double.Parse(IssueQty.Tables["stockSumStock"].Rows[0]["IssueQty"].ToString().Trim());
            }

            return tempRecQty - tempIssueQty;
        }

        public DataTable GetSerialNoDateWise(string FromDt, string ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("SerialNoDateWise", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@FromDate", SqlDbType.Char).Value = FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDate", SqlDbType.Char).Value = ToDt;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet, "tbl_GetSrNoDtsChart");
            return dSet.Tables[0];
        }

        public string Submit([FromBody] MTFWIPInternalRequest MTFWIPIntreq)
        {
            string MTFCode = "";
            CommonCon ComCon = new CommonCon();

            if ((MTFWIPIntreq.ReqBalQty) < (MTFWIPIntreq.MTFQty))
            {
                MTFCode = "MTF Qty Greater Then MTF Balance Qty!";
                return MTFCode;
            }

            int strEngQty = 0, strAltQty = 0, strBatQty = 0;
            int recCountV = ComCon.CountChars(MTFWIPIntreq.MTFDetails, ",");
            string[] strMTFDtsV = Regex.Split(MTFWIPIntreq.MTFDetails, ",");
            // //Validation
            #region

            for (int cSubV = 0; cSubV <= recCountV; cSubV++)
            {
                string[] DtsV = Regex.Split(strMTFDtsV[cSubV].ToString().Trim(), "-->");
                if (double.Parse(DtsV[3].ToString().Trim()) - double.Parse(DtsV[1].ToString().Trim()) < 0)
                {
                    MTFCode = "Insufficient Stock for Part : " + ComCon.getName("select PartDesc+'-->'+PartCode as Partdesc From part where Partcode='" + (DtsV[0].ToString().Trim()) + "' and Active='1'", "tblpart", "Partdesc") + "!";
                    return MTFCode;
                }
                if (DtsV[0].Trim().Substring(0, 3) == "001" || DtsV[0].Trim().Substring(0, 3) == "002" || DtsV[0].Trim().Substring(0, 3) == "010")
                {
                    dsDetailsSubV = ComCon.procTranDS("exec MTFSRNo '" + DtsV[0] + "'," + DtsV[1].Trim() + "", "tbl_MTFSrNoV", con, tran);
                    if (dsDetailsSubV != null && dsDetailsSubV.Tables["tbl_MTFSrNoV"].Rows.Count > 0)
                    {
                        strEngQty = 0; strAltQty = 0; strBatQty = 0;
                        for (int v = 0; v < dsDetailsSubV.Tables["tbl_MTFSrNoV"].Rows.Count; v++)
                        {
                            if (dsDetailsSubV.Tables["tbl_MTFSrNoV"].Rows[v]["PartCode"].ToString().Trim().Substring(0, 3) == "001")
                            {
                                strEngQty = strEngQty + 1;
                            }
                            else if (dsDetailsSubV.Tables["tbl_MTFSrNoV"].Rows[v]["PartCode"].ToString().Trim().Substring(0, 3) == "002")
                            {
                                strAltQty = strAltQty + 1;
                            }
                            else if (dsDetailsSubV.Tables["tbl_MTFSrNoV"].Rows[v]["PartCode"].ToString().Trim().Substring(0, 3) == "010")
                            {
                                strBatQty = strBatQty + 1;
                            }
                        }

                        if (Double.Parse(DtsV[1].Trim()) > strEngQty && DtsV[0].ToString().Substring(0, 3) == "001")
                        {
                            MTFCode = "Engine SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[0].Trim() + "'", "TblPart", "Partdesc");
                            return MTFCode;
                        }
                        else if (Double.Parse(DtsV[1].Trim()) > strAltQty && DtsV[0].ToString().Substring(0, 3) == "002")
                        {
                            MTFCode = "Alternator SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[0].Trim() + "'", "TblPart", "Partdesc");
                            return MTFCode;
                        }
                        else if (Double.Parse(DtsV[1].Trim()) > strBatQty && DtsV[0].ToString().Substring(0, 3) == "010")
                        {
                            MTFCode = "Battery SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[0].Trim() + "'", "TblPart", "Partdesc");
                            return MTFCode;
                        }
                    }
                }
                else
                {

                    //For U3 to Other Example U3 to U1
                    if (MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim() == "03"
                    &&
                    (MTFWIPIntreq.FromPCCode.Trim().ToString().Trim().Substring(0, 2).Trim() != MTFWIPIntreq.ToPCCode.Trim().ToString().Trim().Substring(0, 2).Trim())
                    )
                    {


                        //Other Than Product "001" / "002" / "010"
                        dsDetailsSubV_Other = ComCon.procTranDS("exec MTFSRNo_GIRQty 'Summary', '" + DtsV[0] + "'", "tbl_GIIRSrNoV", con, tran);
                        if (dsDetailsSubV_Other != null && dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows.Count > 0)
                        {
                            for (int v = 0; v < dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows.Count; v++)
                            {
                                if (Double.Parse(DtsV[1].Trim()) > Double.Parse(dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["BalQty"].ToString().Trim()))
                                {
                                    Double DiffQty = (Double.Parse(DtsV[1].Trim()) - Double.Parse(dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["BalQty"].ToString().Trim()));
                                    MTFCode = DiffQty.ToString().Trim() + " - Giir Qty not aviable For Product " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[0].Trim() + "'", "TblPart", "Partdesc");
                                    return MTFCode;
                                }
                            }
                        }
                        else
                        {
                            MTFCode = DtsV[0].ToString().Trim() + " - Giir Qty not aviable For Product " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[0].Trim() + "'", "TblPart", "Partdesc");
                            return MTFCode;
                        }
                        dsDetailsSubV_Other.Dispose();
                    }

                }
            }

            #endregion
            //Validation

            try
            {
                // String Eng_Alt_BalSrNo = "";

                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //MTF
                #region
                MTFCode = ComCon.GetMaxNo("MTF", "MTF", MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2), con, tran);
                //string Max =MTFCode.Substring(10,7).ToString();
                SqlCommand cmd = new SqlCommand();

                cmd = new SqlCommand("InsertPlanMTF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MTFCode", MTFCode.Trim());
                cmd.Parameters.AddWithValue("@MaxSrNo", (MTFCode.Substring(10, 8)));
                cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@Yr", ComCon.yearEnd(con, tran));
                cmd.Parameters.AddWithValue("@FromProfitCenterCode", MTFWIPIntreq.FromPCCode.Trim());
                cmd.Parameters.AddWithValue("@ToProfitCenterCode", MTFWIPIntreq.ToPCCode.Trim());
                cmd.Parameters.AddWithValue("@ForProfitCenterCode", 0);
                cmd.Parameters.AddWithValue("@RequisitionCode", MTFWIPIntreq.ReqCode.Trim());
                cmd.Parameters.AddWithValue("@CpyPartCode", MTFWIPIntreq.ProdPartCode.Trim());
                cmd.Parameters.AddWithValue("@PlanPartCode", "0");
                cmd.Parameters.AddWithValue("@SCode", "0");
                cmd.Parameters.AddWithValue("@PlanIssueQty", MTFWIPIntreq.MTFQty);
                cmd.Parameters.AddWithValue("@ReceivedDt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


                //For U3 to Other Example U3 to U1
                if (MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim() == "03"
                &&
                (MTFWIPIntreq.FromPCCode.Trim().ToString().Trim().Substring(0, 2).Trim() != MTFWIPIntreq.ToPCCode.Trim().ToString().Trim().Substring(0, 2).Trim())
                )
                {
                    cmd.Parameters.AddWithValue("@MTFStatus", "P");
                }
                else
                //For Internal
                {
                    cmd.Parameters.AddWithValue("@MTFStatus", "D");
                }

                cmd.Parameters.AddWithValue("@CompanyCode", MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2));
                cmd.Parameters.AddWithValue("@Remark", MTFWIPIntreq.Remark.Trim());
                cmd.Parameters.AddWithValue("@WtPerUt", 0);
                cmd.Parameters.AddWithValue("@SqftPerUt", 0);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                int recCount = ComCon.CountChars(MTFWIPIntreq.MTFDetails, ",");
                string[] strMTFDts = Regex.Split(MTFWIPIntreq.MTFDetails, ",");

                int SrNo = 0;
                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo += 1;
                    string[] Dts = Regex.Split(strMTFDts[cSub].ToString().Trim(), "-->");

                    cmd = new SqlCommand("InsertPlanMTFDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MTFCode", MTFCode.Trim());
                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                    cmd.Parameters.AddWithValue("@PartCode", Dts[0].ToString().Trim());
                    cmd.Parameters.AddWithValue("@PendingQty", 0);
                    cmd.Parameters.AddWithValue("@IssueQty", double.Parse(Dts[1].ToString().Trim()));
                    // cmd.Parameters.AddWithValue("@Remark", "WIP Internal");

                    if (MTFWIPIntreq.ToPCCode.Trim().ToString().Trim().Substring(0, 2).Trim() == "01")
                    {
                        cmd.Parameters.AddWithValue("@Remark", "WIP Internal Unit-1");
                    }
                    else if (MTFWIPIntreq.ToPCCode.Trim().ToString().Trim().Substring(0, 2).Trim() == "03")
                    {
                        cmd.Parameters.AddWithValue("@Remark", "WIP Internal Unit-4");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Remark", "WIP Internal");
                    }

                    cmd.Parameters.AddWithValue("@Rate", double.Parse(Dts[2].ToString().Trim()));
                    cmd.Parameters.AddWithValue("@MemoStatus", "D");
                    cmd.Parameters.AddWithValue("@WtPerUt", 0);
                    cmd.Parameters.AddWithValue("@SqftPerUt", 0);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Issue
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                    sb.Append(" values('" + MTFWIPIntreq.FromPCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
                    sb.Append("'" + MTFCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + double.Parse(Dts[1].ToString().Trim()) + "','" + MTFWIPIntreq.ToPCCode.Trim() + "',0)");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Eng/Alt/bat --Start
                    if (Dts[0].Trim().Substring(0, 3) == "001" || Dts[0].Trim().Substring(0, 3) == "002" || Dts[0].Trim().Substring(0, 3) == "010")
                    {
                        dsDetailsSub = ComCon.procTranDS("exec MTFSRNo '" + Dts[0] + "'," + Dts[1].Trim() + "", "tbl_MTFSrNo", con, tran);
                        if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_MTFSrNo"].Rows.Count > 0)
                        {
                            int SrNok = 0;
                            for (int k = 0; k < dsDetailsSub.Tables["tbl_MTFSrNo"].Rows.Count; k++)
                            {
                                SrNok += 1;
                                sb.Remove(0, sb.Length);
                                sb.Append("insert into MTFDetailssub(MTFCode,SrNo,PartCode,SerialNo,JobcardStatus) ");
                                sb.Append("values('" + MTFCode.Trim() + "','" + SrNok + "','" + Dts[0].Trim() + "',");
                                sb.Append("'" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["SerialNo"].ToString().Trim() + "',");

                                if (dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["JobcardStatus"].ToString().Trim() == "J")
                                {
                                    sb.Append("'J')");
                                }
                                else
                                {
                                    sb.Append("'P')");
                                }
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                sb.Remove(0, sb.Length);
                                sb.Append("Update GIIRDetailsSub set TRFStatus='D',TRFCode='" + MTFCode.Trim() + "'  where  ");
                                sb.Append("SerialNo='" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["SerialNo"].ToString().Trim() + "' and GIIrCode='" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["GCode"].ToString().Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                            }
                        }


                        //For Internal Only  Example - U4 to U4 From and to Profit center Same
                        if (MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim() == "03"
                       &&
                       (MTFWIPIntreq.FromPCCode.Trim().ToString().Trim().Substring(0, 2).Trim() == MTFWIPIntreq.ToPCCode.Trim().ToString().Trim().Substring(0, 2).Trim())
                       )
                        {
                            if (Dts[0].Trim().Substring(0, 3) == "001" || Dts[0].Trim().Substring(0, 3) == "002")
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,StageName)");
                                sb.Append(" values('" + MTFWIPIntreq.FromPCCode.Trim() + "','" + Dts[0].ToString().Trim().Trim() + "' ,");
                                sb.Append("'" + MTFCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + double.Parse(Dts[1].ToString().Trim()) + "','" + MTFWIPIntreq.ToPCCode.Trim() + "','0','StageI')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                            else if (Dts[0].Trim().Substring(0, 3) == "010")
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,StageName)");
                                sb.Append(" values('" + MTFWIPIntreq.FromPCCode.Trim() + "','" + Dts[0].ToString().Trim().Trim() + "' ,");
                                sb.Append("'" + MTFCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + double.Parse(Dts[1].ToString().Trim()) + "','" + MTFWIPIntreq.ToPCCode.Trim() + "','0','StageIII')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }
                    }
                    else
                    {
                        //For Internal Only  Example - U4 to U4 From and to Profit center Same
                        if (MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim() == "03"
                       &&
                       (MTFWIPIntreq.FromPCCode.Trim().ToString().Trim().Substring(0, 2).Trim() == MTFWIPIntreq.ToPCCode.Trim().ToString().Trim().Substring(0, 2).Trim())
                       )
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,StageName)");
                            sb.Append(" values('" + MTFWIPIntreq.FromPCCode.Trim() + "','" + Dts[0].ToString().Trim().Trim() + "' ,");
                            sb.Append("'" + MTFCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + double.Parse(Dts[1].ToString().Trim()) + "','" + MTFWIPIntreq.ToPCCode.Trim() + "','0','0')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }

                    //Eng/Alt/bat --END
                }


                if ((MTFWIPIntreq.ReqBalQty) == (MTFWIPIntreq.MTFQty))
                {
                    sb.Remove(0, sb.Length);
                    sb.Append("Update MaterialRequisitionWithoutPlan set ReqStatus='D' where  ReqCode='" + MTFWIPIntreq.ReqCode.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@EmpID", MTFWIPIntreq.UserID.ToString().Trim());  //ASK FS
                cmd.Parameters.AddWithValue("@TransactionType", "S");
                cmd.Parameters.AddWithValue("@TransactionFrom", "MTF");
                cmd.Parameters.AddWithValue("@TransactionNo", MTFCode.Trim().Trim());
                cmd.Parameters.AddWithValue("@CompanyCode", MTFWIPIntreq.CompID.Trim());
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();


                #endregion
                // END MTF

                //For MEmo 

                String strEngGIIR_Memo = "";
                String strAltGIIR_Memo = "";
                String strBatGIIR_Memo = "";

                int recCountV_M = ComCon.CountChars(MTFWIPIntreq.MTFDetails, ",");
                string[] strMTFDtsV_M = Regex.Split(MTFWIPIntreq.MTFDetails, ",");

                #region

                for (int cSubV_M = 0; cSubV_M <= recCountV_M; cSubV_M++)
                {
                    string[] DtsV_M = Regex.Split(strMTFDtsV_M[cSubV_M].ToString().Trim(), "-->");
                    if (DtsV_M[0].Trim().Substring(0, 3).Trim() == "001" || DtsV_M[0].Trim().Substring(0, 3).Trim() == "002" || DtsV_M[0].Trim().Substring(0, 3).Trim() == "010")
                    {
                        dsDetailsSubV = ComCon.procTranDS("exec MTFSRNoGIIRNo_Qty '" + DtsV_M[0] + "'," + DtsV_M[1].Trim() + "", "tbl_MTFSrNoV_M", con, tran);
                        if (dsDetailsSubV != null && dsDetailsSubV.Tables["tbl_MTFSrNoV_M"].Rows.Count > 0)
                        {
                            for (int v = 0; v < dsDetailsSubV.Tables["tbl_MTFSrNoV_M"].Rows.Count; v++)
                            {
                                if (DtsV_M[0].Trim().Substring(0, 3).Trim() == "001")
                                {
                                    if (strEngGIIR_Memo.Trim() == "")
                                    {
                                        strEngGIIR_Memo = dsDetailsSubV.Tables["tbl_MTFSrNoV_M"].Rows[v]["GIIRNO"].ToString().Trim();
                                    }
                                    else
                                    {
                                        strEngGIIR_Memo = "," + dsDetailsSubV.Tables["tbl_MTFSrNoV_M"].Rows[v]["GIIRNO"].ToString().Trim();
                                    }
                                }

                                if (DtsV_M[0].Trim().Substring(0, 3).Trim() == "002")
                                {
                                    if (strAltGIIR_Memo.Trim() == "")
                                    {
                                        strAltGIIR_Memo = dsDetailsSubV.Tables["tbl_MTFSrNoV_M"].Rows[v]["GIIRNO"].ToString().Trim();
                                    }
                                    else
                                    {
                                        strAltGIIR_Memo = "," + dsDetailsSubV.Tables["tbl_MTFSrNoV_M"].Rows[v]["GIIRNO"].ToString().Trim();
                                    }
                                }

                                if (DtsV_M[0].Trim().Substring(0, 3).Trim() == "010")
                                {
                                    if (strBatGIIR_Memo.Trim() == "")
                                    {
                                        strBatGIIR_Memo = dsDetailsSubV.Tables["tbl_MTFSrNoV_M"].Rows[v]["GIIRNO"].ToString().Trim();
                                    }
                                    else
                                    {
                                        strBatGIIR_Memo = "," + dsDetailsSubV.Tables["tbl_MTFSrNoV_M"].Rows[v]["GIIRNO"].ToString().Trim();
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion


                //*******************************************************
                //Excise Memo Save Code Start U4 DG to U1 DG
                #region
                if (MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim() == "03"
                   &&
                   (MTFWIPIntreq.FromPCCode.Trim().ToString().Trim().Substring(0, 2).Trim() != MTFWIPIntreq.ToPCCode.Trim().ToString().Trim().Substring(0, 2).Trim())
                   )
                {
                    //MAster Table Code
                 
                    int SrNo_Memo = 0;
                    String MemoCode = "";

                    MemoCode = ComCon.GetMaxNo("MemoExciseMfg", "MOE", MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2), con, tran);

                    cmd = new SqlCommand("InsertMemoExciseMfg_DG", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MECode", MemoCode.Trim());
                    cmd.Parameters.AddWithValue("@MaxSrNo", (MemoCode.Substring(10, 8)).Trim());
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@Yr", ComCon.yearEnd(con, tran));
                    cmd.Parameters.AddWithValue("@FromProfitCenter", MTFWIPIntreq.FromPCCode.Trim());
                    cmd.Parameters.AddWithValue("@ToProfitCenter", MTFWIPIntreq.ToPCCode.Trim());
                    cmd.Parameters.AddWithValue("@TallyHeadCode", "2");//Ask FS
                    cmd.Parameters.AddWithValue("@ConsigneeCode", getConsigneeCode(MTFWIPIntreq.ToPCCode.Trim()).Trim());
                    cmd.Parameters.AddWithValue("@TMcode", "01");
                    cmd.Parameters.AddWithValue("@PONo", "0");
                    cmd.Parameters.AddWithValue("@PODate", "");
                    cmd.Parameters.AddWithValue("@CompanyCode", MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim());
                    cmd.Parameters.AddWithValue("@StockType", "0");
                    cmd.Parameters.AddWithValue("@WtPerUt", 0);
                    cmd.Parameters.AddWithValue("@SqftPerUt", 0);
                    cmd.Parameters.AddWithValue("@Auth", 0);
                    cmd.Parameters.AddWithValue("@MMTFCode", MTFCode.Trim());
                    cmd.Parameters.AddWithValue("@MTFScanStatus", "P");
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                  

                    //****************User Acivity****************                
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@EmpID", MTFWIPIntreq.UserID.ToString().Trim());  //ASK FS
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "MemoExciseMfg");
                    cmd.Parameters.AddWithValue("@TransactionNo", MemoCode.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();


                    //Details Table memo Save   Code 

                    int recCount_Memo = ComCon.CountChars(MTFWIPIntreq.MTFDetails, ",");
                    string[] strMTFDts_Memo = Regex.Split(MTFWIPIntreq.MTFDetails, ",");

                    for (int cSub_Memo = 0; cSub_Memo <= recCount_Memo; cSub_Memo++)
                    {

                        string[] Dts_Memo = Regex.Split(strMTFDts_Memo[cSub_Memo].ToString().Trim(), "-->");
                        String StrGIIRCodeAndQty = "";

                        if (Dts_Memo[1].ToString().Trim() != "" && double.Parse(Dts_Memo[1].ToString().Trim()) > 0)//IssueQty
                        {
                            string StrMOB = ComCon.getTranName("SELECT MOB FROM Part where partcode= '" + Dts_Memo[0].ToString().Trim() + "'", "tblG1", "MOB", con, tran);

                            //ENG/Alt/Bat
                            if (Dts_Memo[0].Trim().Substring(0, 3).Trim() == "001" || Dts_Memo[0].Trim().Substring(0, 3).Trim() == "002" || Dts_Memo[0].Trim().Substring(0, 3).Trim() == "010")
                            {
                                SrNo_Memo = SrNo_Memo + 1;
                                cmd = new SqlCommand("InsertMemoExciseMfgDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@MECode", MemoCode.Trim());
                                cmd.Parameters.AddWithValue("@SrNo", SrNo_Memo);

                                if (Dts_Memo[0].Trim().Substring(0, 3).Trim() == "001")
                                {
                                    cmd.Parameters.AddWithValue("@GIIRCode", strEngGIIR_Memo);
                                }

                                if (Dts_Memo[0].Trim().Substring(0, 3).Trim() == "002")
                                {
                                    cmd.Parameters.AddWithValue("@GIIRCode", strAltGIIR_Memo);
                                }

                                if (Dts_Memo[0].Trim().Substring(0, 3).Trim() == "010")
                                {
                                    cmd.Parameters.AddWithValue("@GIIRCode", strBatGIIR_Memo);
                                }

                            }

                            else if ((Dts_Memo[0].Trim().Substring(0, 3).Trim() != "001" || Dts_Memo[0].Trim().Substring(0, 3).Trim() != "002"
                                     || Dts_Memo[0].Trim().Substring(0, 3).Trim() != "010") && (StrMOB.ToString().Trim() == "B"))
                            {
                                double IssueQty = Convert.ToDouble(Dts_Memo[1].Trim());
                                double GIIRIssue = 0;

                                //Other Than Product "001" / "002" / "010"
                                dsDetailsSubV_Other = ComCon.procTranDS("exec MTFSRNo_GIRQty 'Details', '" + Dts_Memo[0].Trim() + "'", "tbl_GIIRSrNoV", con, tran);
                                if (dsDetailsSubV_Other != null && dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows.Count > 0)
                                {
                                    StrGIIRCodeAndQty = "";
                                    for (int v = 0; v < dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows.Count; v++)
                                    {
                                        if (IssueQty > 0)
                                        {
                                            if (Convert.ToDouble(dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["BalQty"].ToString().Trim()) >= IssueQty) // GIIR Bal Qty Greater than Issue Qty 
                                            {
                                                GIIRIssue = IssueQty;
                                                IssueQty = 0;
                                            }
                                            else // GIIR Bal Qty Less than Issue Qty 
                                            {
                                                // 1700-1600 =100
                                                IssueQty = (IssueQty - Convert.ToDouble(dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["BalQty"].ToString().Trim()));
                                                GIIRIssue = Convert.ToDouble(dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["BalQty"].ToString().Trim());
                                            }

                                            StrGIIRCodeAndQty += "#" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "@" + GIIRIssue.ToString().Trim();

                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into memoExciseMfgGIIR (MECODE,GiirCode,IssueQty) values ('" + MemoCode.Trim() + "' ,'" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "'");
                                            sb.Append(",'" + GIIRIssue + "')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();

                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert Into " + ComCon.getStockTbl(MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim()) + "(PartCode,ReceivedCode,IssueCode,IssueDate,IssueQty)");
                                            sb.Append((" VALUES ( '" + Dts_Memo[0].Trim() + "','" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "','" + MemoCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + GIIRIssue + "') "));
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();

                                            double GIRBalQty = 0;
                                            GIRBalQty = double.Parse(ComCon.getTranName("SELECT ISNULL((Sum(ReceivedQty)- Sum(IssueQty)),0) AS Bal FROM " + ComCon.getStockTbl(MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim()) + " WHERE ReceivedCode='" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "' AND Partcode='" + Dts_Memo[0].Trim() + "'", "STOCK10", "Bal", con, tran));
                                            if (GIRBalQty == 0)
                                            {
                                                sb.Remove(0, sb.Length);
                                                sb.Append("UPDATE GIIRDetails SET GIIRStatus='D' WHERE GIIRCode='" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "' ");
                                                sb.Append(" AND PartCode='" + Dts_Memo[0].Trim() + "'");
                                                cmd = new SqlCommand(sb.ToString(), con);
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();
                                            }

                                            int CntGIIR = 0;
                                            CntGIIR = int.Parse(ComCon.getTranName("Select count(GIIRStatus) as CNT From GIIRDetails Where GIIRStatus<>'D' and GIIRCode='" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "'", "GIIRD1", "CNT", con, tran));
                                            if (CntGIIR == 0)
                                            {
                                                sb.Remove(0, sb.Length);
                                                sb.Append("UPDATE GIIR SET GIIRStatus='D' WHERE GIIRCode='" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "'");
                                                cmd = new SqlCommand(sb.ToString(), con);
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();
                                            }

                                        }
                                    }
                                    //dsDetailsSubV_Other.Dispose();
                                    //dsDetailsSubV_Other.Clear();
                                }


                                SrNo_Memo = SrNo_Memo + 1;
                                cmd = new SqlCommand("InsertMemoExciseMfgDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@MECode", MemoCode.Trim());
                                cmd.Parameters.AddWithValue("@SrNo", SrNo_Memo);
                                cmd.Parameters.AddWithValue("@GIIRCode", StrGIIRCodeAndQty.ToString().Trim());
                            }
                            else
                            {
                                SrNo_Memo = SrNo_Memo + 1;
                                cmd = new SqlCommand("InsertMemoExciseMfgDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@MECode", MemoCode.Trim());
                                cmd.Parameters.AddWithValue("@SrNo", SrNo_Memo);
                                cmd.Parameters.AddWithValue("@GIIRCode", 0);
                            }

                            cmd.Parameters.AddWithValue("@PartCode", Dts_Memo[0].Trim());
                            cmd.Parameters.AddWithValue("@StockQty", Dts_Memo[3].ToString().Trim());//STKQty
                            cmd.Parameters.AddWithValue("@Qty", double.Parse(Dts_Memo[3].ToString().Trim()) - double.Parse(Dts_Memo[1].ToString().Trim())); //STKQty - MtfQty
                            cmd.Parameters.AddWithValue("@IssueQty", double.Parse(Dts_Memo[1].ToString().Trim())); // MtfQty
                            cmd.Parameters.AddWithValue("@InvUOM", 0);
                            cmd.Parameters.AddWithValue("@GIIRBalQty", 0);
                            cmd.Parameters.AddWithValue("@Rate", double.Parse(Dts_Memo[2].ToString().Trim())); //Rate
                            cmd.Parameters.AddWithValue("@SaleRate", double.Parse(Dts_Memo[2].ToString().Trim()));// SaleRate
                            cmd.Parameters.AddWithValue("@ManulDescription", 0);
                            cmd.Parameters.AddWithValue("@PerUnitWt", 0);
                            cmd.Parameters.AddWithValue("@PerUnitSqft", 0);
                            cmd.Parameters.AddWithValue("@MCRWt", 0);
                            cmd.Parameters.AddWithValue("@MHRWt", 0);
                            cmd.Parameters.AddWithValue("@MCRAmt", 0);
                            cmd.Parameters.AddWithValue("@MHRAmt", 0);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            cmd = new SqlCommand("InsertMemoExciseMfgDetailsSub", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@MECode", MemoCode.Trim());
                            cmd.Parameters.AddWithValue("@SrNo", SrNo_Memo);
                            cmd.Parameters.AddWithValue("@PartCode", Dts_Memo[0].Trim());// PartCode
                            cmd.Parameters.AddWithValue("@MTFCode", MTFCode.Trim()); //MTFCode
                            cmd.Parameters.AddWithValue("@IssueQty", Dts_Memo[1].ToString().Trim()); //MtfQty
                            cmd.Parameters.AddWithValue("@BalQty", double.Parse(Dts_Memo[3].ToString().Trim()) - double.Parse(Dts_Memo[1].ToString().Trim())); //STKQty - MtfQty
                            cmd.Parameters.AddWithValue("@MTFIssueQty", Dts_Memo[1].ToString().Trim()); //MtfQty
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();


                        }

                    }
                }

                #endregion

                //BAckUp Dated 04_05_2023
                #region



                //if (MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim() == "03"
                //    &&
                //    (MTFWIPIntreq.FromPCCode.Trim().ToString().Trim().Substring(0, 2).Trim() != MTFWIPIntreq.ToPCCode.Trim().ToString().Trim().Substring(0, 2).Trim())
                //    )
                //{
                //    int k = 0;
                //    int m = 0;
                //    int j = 0;
                //    int i = 0;
                //    int SrNo_Memo = 0;
                //    String MemoCode = "";
                //    String TotalMemoCode = "";

                //    int recCount_Memo = ComCon.CountChars(MTFWIPIntreq.MTFDetails, ",");
                //    string[] strMTFDts_Memo = Regex.Split(MTFWIPIntreq.MTFDetails, ",");

                //    for (int cSub_Memo = 0; cSub_Memo <= recCount_Memo; cSub_Memo++)
                //    {
                //        // SrNo_Memo += 1;
                //        string[] Dts_Memo = Regex.Split(strMTFDts_Memo[cSub_Memo].ToString().Trim(), "-->");
                //        //String strSerialNo_Memo = "";
                //        String StrGIIRCodeAndQty = "";

                //        if (Dts_Memo[1].ToString().Trim() != "" && double.Parse(Dts_Memo[1].ToString().Trim()) > 0)//IssueQty
                //        {
                //            string StrMOB = ComCon.getTranName("SELECT MOB FROM Part where partcode= '" + Dts_Memo[0].ToString().Trim() + "'", "tblG1", "MOB", con, tran);

                //            if (k == 0 || k == m)
                //            {
                //                MemoCode = ComCon.GetMaxNo("MemoExciseMfg", "MOE", MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2), con, tran);

                //                if (k == 0)
                //                {
                //                    TotalMemoCode = MemoCode;
                //                }
                //                else
                //                {
                //                    TotalMemoCode += "," + MemoCode;
                //                }
                //                //Mst

                //                cmd = new SqlCommand("InsertMemoExciseMfg_DG", con);
                //                cmd.CommandType = CommandType.StoredProcedure;
                //                cmd.Parameters.AddWithValue("@MECode", MemoCode.Trim());
                //                cmd.Parameters.AddWithValue("@MaxSrNo", (MemoCode.Substring(10, 8)).Trim());
                //                cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //                cmd.Parameters.AddWithValue("@Yr", ComCon.yearEnd(con, tran));
                //                cmd.Parameters.AddWithValue("@FromProfitCenter", MTFWIPIntreq.FromPCCode.Trim());
                //                cmd.Parameters.AddWithValue("@ToProfitCenter", MTFWIPIntreq.ToPCCode.Trim());
                //                cmd.Parameters.AddWithValue("@TallyHeadCode", "2");//Ask FS
                //                cmd.Parameters.AddWithValue("@ConsigneeCode", getConsigneeCode(MTFWIPIntreq.ToPCCode.Trim()).Trim());
                //                cmd.Parameters.AddWithValue("@TMcode", "01");
                //                cmd.Parameters.AddWithValue("@PONo", "0");
                //                cmd.Parameters.AddWithValue("@PODate", "");
                //                cmd.Parameters.AddWithValue("@CompanyCode", MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim());
                //                cmd.Parameters.AddWithValue("@StockType", "0");
                //                cmd.Parameters.AddWithValue("@WtPerUt", 0);
                //                cmd.Parameters.AddWithValue("@SqftPerUt", 0);
                //                cmd.Parameters.AddWithValue("@Auth", 0);
                //                cmd.Parameters.AddWithValue("@MMTFCode", MTFCode.Trim());
                //                cmd.Parameters.AddWithValue("@MTFScanStatus", "P");
                //                cmd.Transaction = tran;
                //                cmd.ExecuteNonQuery();
                //                cmd.Dispose();

                //                m = k + 4;

                //                //****************User Acivity****************                
                //                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                //                cmd.CommandType = CommandType.StoredProcedure;
                //                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //                cmd.Parameters.AddWithValue("@EmpID", MTFWIPIntreq.UserID.ToString().Trim());  //ASK FS
                //                cmd.Parameters.AddWithValue("@TransactionType", "S");
                //                cmd.Parameters.AddWithValue("@TransactionFrom", "MemoExciseMfg");
                //                cmd.Parameters.AddWithValue("@TransactionNo", MemoCode.Trim());
                //                cmd.Parameters.AddWithValue("@CompanyCode", MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim());
                //                cmd.Transaction = tran;
                //                cmd.ExecuteNonQuery();
                //                cmd.Dispose();

                //                SrNo_Memo = 0;
                //            }

                //            //ENG/Alt/Bat
                //            if (Dts_Memo[0].Trim().Substring(0, 3).Trim() == "001" || Dts_Memo[0].Trim().Substring(0, 3).Trim() == "002" || Dts_Memo[0].Trim().Substring(0, 3).Trim() == "010")
                //            {
                //                SrNo_Memo = SrNo_Memo + 1;
                //                cmd = new SqlCommand("InsertMemoExciseMfgDetails", con);
                //                cmd.CommandType = CommandType.StoredProcedure;
                //                cmd.Parameters.AddWithValue("@MECode", MemoCode.Trim());
                //                cmd.Parameters.AddWithValue("@SrNo", SrNo_Memo);

                //                if (Dts_Memo[0].Trim().Substring(0, 3).Trim() == "001")
                //                {
                //                    cmd.Parameters.AddWithValue("@GIIRCode", strEngGIIR_Memo);
                //                }

                //                if (Dts_Memo[0].Trim().Substring(0, 3).Trim() == "002")
                //                {
                //                    cmd.Parameters.AddWithValue("@GIIRCode", strAltGIIR_Memo);
                //                }

                //                if (Dts_Memo[0].Trim().Substring(0, 3).Trim() == "010")
                //                {
                //                    cmd.Parameters.AddWithValue("@GIIRCode", strBatGIIR_Memo);
                //                }

                //            }

                //            else if ((Dts_Memo[0].Trim().Substring(0, 3).Trim() != "001" || Dts_Memo[0].Trim().Substring(0, 3).Trim() != "002"
                //                     || Dts_Memo[0].Trim().Substring(0, 3).Trim() != "010") && (StrMOB.ToString().Trim() == "B"))
                //            {
                //                double IssueQty = Convert.ToDouble(Dts_Memo[1].Trim());
                //                double GIIRIssue = 0;

                //                //Other Than Product "001" / "002" / "010"
                //                dsDetailsSubV_Other = ComCon.procTranDS("exec MTFSRNo_GIRQty 'Details', '" + Dts_Memo[0].Trim() + "'", "tbl_GIIRSrNoV", con, tran);
                //                if (dsDetailsSubV_Other != null && dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows.Count > 0)
                //                {
                //                    StrGIIRCodeAndQty = "";
                //                    for (int v = 0; v < dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows.Count; v++)
                //                    {
                //                        if (IssueQty > 0)
                //                        {
                //                            if (Convert.ToDouble(dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["BalQty"].ToString().Trim()) >= IssueQty) // GIIR Bal Qty Greater than Issue Qty 
                //                            {
                //                                GIIRIssue = IssueQty;
                //                                IssueQty = 0;
                //                            }
                //                            else // GIIR Bal Qty Less than Issue Qty 
                //                            {
                //                                // 1700-1600 =100
                //                                IssueQty = (IssueQty - Convert.ToDouble(dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["BalQty"].ToString().Trim()));
                //                                GIIRIssue = Convert.ToDouble(dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["BalQty"].ToString().Trim());
                //                            }

                //                            StrGIIRCodeAndQty += "#" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "@" + GIIRIssue.ToString().Trim();

                //                            sb.Remove(0, sb.Length);
                //                            sb.Append("Insert into memoExciseMfgGIIR (MECODE,GiirCode,IssueQty) values ('" + MemoCode.Trim() + "' ,'" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "'");
                //                            sb.Append(",'" + GIIRIssue + "')");
                //                            cmd = new SqlCommand(sb.ToString(), con);
                //                            cmd.Transaction = tran;
                //                            cmd.ExecuteNonQuery();
                //                            cmd.Dispose();

                //                            sb.Remove(0, sb.Length);
                //                            sb.Append("Insert Into " + ComCon.getStockTbl(MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim()) + "(PartCode,ReceivedCode,IssueCode,IssueDate,IssueQty)");
                //                            sb.Append((" VALUES ( '" + Dts_Memo[0].Trim() + "','" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "','" + MemoCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + GIIRIssue + "') "));
                //                            cmd = new SqlCommand(sb.ToString(), con);
                //                            cmd.Transaction = tran;
                //                            cmd.ExecuteNonQuery();
                //                            cmd.Dispose();

                //                            double GIRBalQty = 0;
                //                            GIRBalQty = double.Parse(ComCon.getTranName("SELECT ISNULL((Sum(ReceivedQty)- Sum(IssueQty)),0) AS Bal FROM " + ComCon.getStockTbl(MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2).Trim()) + " WHERE ReceivedCode='" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "' AND Partcode='" + Dts_Memo[0].Trim() + "'", "STOCK10", "Bal", con, tran));
                //                            if (GIRBalQty == 0)
                //                            {
                //                                sb.Remove(0, sb.Length);
                //                                sb.Append("UPDATE GIIRDetails SET GIIRStatus='D' WHERE GIIRCode='" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "' ");
                //                                sb.Append(" AND PartCode='" + Dts_Memo[0].Trim() + "'");
                //                                cmd = new SqlCommand(sb.ToString(), con);
                //                                cmd.Transaction = tran;
                //                                cmd.ExecuteNonQuery();
                //                                cmd.Dispose();
                //                            }

                //                            int CntGIIR = 0;
                //                            CntGIIR = int.Parse(ComCon.getTranName("Select count(GIIRStatus) as CNT From GIIRDetails Where GIIRStatus<>'D' and GIIRCode='" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "'", "GIIRD1", "CNT", con, tran));
                //                            if (CntGIIR == 0)
                //                            {
                //                                sb.Remove(0, sb.Length);
                //                                sb.Append("UPDATE GIIR SET GIIRStatus='D' WHERE GIIRCode='" + dsDetailsSubV_Other.Tables["tbl_GIIRSrNoV"].Rows[v]["ReceivedCode"].ToString().Trim() + "'");
                //                                cmd = new SqlCommand(sb.ToString(), con);
                //                                cmd.Transaction = tran;
                //                                cmd.ExecuteNonQuery();
                //                                cmd.Dispose();
                //                            }

                //                        }
                //                    }
                //                    //dsDetailsSubV_Other.Dispose();
                //                    //dsDetailsSubV_Other.Clear();
                //                }


                //                SrNo_Memo = SrNo_Memo + 1;
                //                cmd = new SqlCommand("InsertMemoExciseMfgDetails", con);
                //                cmd.CommandType = CommandType.StoredProcedure;
                //                cmd.Parameters.AddWithValue("@MECode", MemoCode.Trim());
                //                cmd.Parameters.AddWithValue("@SrNo", SrNo_Memo);
                //                cmd.Parameters.AddWithValue("@GIIRCode", StrGIIRCodeAndQty.ToString().Trim());
                //            }
                //            else
                //            {
                //                SrNo_Memo = SrNo_Memo + 1;
                //                cmd = new SqlCommand("InsertMemoExciseMfgDetails", con);
                //                cmd.CommandType = CommandType.StoredProcedure;
                //                cmd.Parameters.AddWithValue("@MECode", MemoCode.Trim());
                //                cmd.Parameters.AddWithValue("@SrNo", SrNo_Memo);
                //                cmd.Parameters.AddWithValue("@GIIRCode", 0);
                //            }

                //            cmd.Parameters.AddWithValue("@PartCode", Dts_Memo[0].Trim());
                //            cmd.Parameters.AddWithValue("@StockQty", Dts_Memo[3].ToString().Trim());//STKQty
                //            cmd.Parameters.AddWithValue("@Qty", double.Parse(Dts_Memo[3].ToString().Trim()) - double.Parse(Dts_Memo[1].ToString().Trim())); //STKQty - MtfQty
                //            cmd.Parameters.AddWithValue("@IssueQty", double.Parse(Dts_Memo[1].ToString().Trim())); // MtfQty
                //            cmd.Parameters.AddWithValue("@InvUOM", 0);
                //            cmd.Parameters.AddWithValue("@GIIRBalQty", 0);
                //            cmd.Parameters.AddWithValue("@Rate", double.Parse(Dts_Memo[2].ToString().Trim())); //Rate
                //            cmd.Parameters.AddWithValue("@SaleRate", double.Parse(Dts_Memo[2].ToString().Trim()));// SaleRate
                //            cmd.Parameters.AddWithValue("@ManulDescription", 0);
                //            cmd.Parameters.AddWithValue("@PerUnitWt", 0);
                //            cmd.Parameters.AddWithValue("@PerUnitSqft", 0);
                //            cmd.Parameters.AddWithValue("@MCRWt", 0);
                //            cmd.Parameters.AddWithValue("@MHRWt", 0);
                //            cmd.Parameters.AddWithValue("@MCRAmt", 0);
                //            cmd.Parameters.AddWithValue("@MHRAmt", 0);
                //            cmd.Transaction = tran;
                //            cmd.ExecuteNonQuery();
                //            cmd.Dispose();

                //            cmd = new SqlCommand("InsertMemoExciseMfgDetailsSub", con);
                //            cmd.CommandType = CommandType.StoredProcedure;
                //            cmd.Parameters.AddWithValue("@MECode", MemoCode.Trim());
                //            cmd.Parameters.AddWithValue("@SrNo", SrNo_Memo);
                //            cmd.Parameters.AddWithValue("@PartCode", Dts_Memo[0].Trim());// PartCode
                //            cmd.Parameters.AddWithValue("@MTFCode", MTFCode.Trim()); //MTFCode
                //            cmd.Parameters.AddWithValue("@IssueQty", Dts_Memo[1].ToString().Trim()); //MtfQty
                //            cmd.Parameters.AddWithValue("@BalQty", double.Parse(Dts_Memo[3].ToString().Trim()) - double.Parse(Dts_Memo[1].ToString().Trim())); //STKQty - MtfQty
                //            cmd.Parameters.AddWithValue("@MTFIssueQty", Dts_Memo[1].ToString().Trim()); //MtfQty
                //            cmd.Transaction = tran;
                //            cmd.ExecuteNonQuery();
                //            cmd.Dispose();

                //            j = j + 1;
                //            k = k + 1;
                //        }

                //    }
                //}
                #endregion
                //*******************************************************
                //Excise Memo Save Code END

                tran.Commit();

                return MTFCode;
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

        //public string Submit([FromBody] MTFWIPInternalRequest MTFWIPIntreq)
        //{

        //    string MTFCode = "";
        //    CommonCon ComCon = new CommonCon();


        //    if ((MTFWIPIntreq.ReqBalQty) < (MTFWIPIntreq.MTFQty))
        //    {
        //        MTFCode = "MTF Qty Greater Then MTF Balance Qty!";
        //        return MTFCode;
        //    }

        //    int recCountV = ComCon.CountChars(MTFWIPIntreq.MTFDetails, ",");
        //    string[] strMTFDtsV = Regex.Split(MTFWIPIntreq.MTFDetails, ",");

        //    for (int cSubV = 0; cSubV <= recCountV; cSubV++)            
        //    {

        //        string[] DtsV = Regex.Split(strMTFDtsV[cSubV].ToString().Trim(), "-->");
        //        if (double.Parse(DtsV[1].ToString().Trim()) < 0)
        //        {
        //            MTFCode = "Insufficient Stock for Part :" + ComCon.GetPartDesc(DtsV[0].ToString().Trim()) + "!";
        //            return MTFCode;
        //        }

        //    }

        //        try
        //    {
        //        if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
        //        tran = con.BeginTransaction();

        //        MTFCode = ComCon.GetMaxNo("MTF", "MTF", MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2), con, tran);
        //        //string Max =MTFCode.Substring(10,7).ToString();
        //        SqlCommand cmd = new SqlCommand();

        //        cmd = new SqlCommand("InsertPlanMTF", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@MTFCode", MTFCode.Trim());
        //        cmd.Parameters.AddWithValue("@MaxSrNo", (MTFCode.Substring(10, 8)));
        //        cmd.Parameters.AddWithValue("@Dt", System.DateTime.Now);
        //        cmd.Parameters.AddWithValue("@Yr", ComCon.yearEnd(con, tran));
        //        cmd.Parameters.AddWithValue("@FromProfitCenterCode", MTFWIPIntreq.FromPCCode.Trim());
        //        cmd.Parameters.AddWithValue("@ToProfitCenterCode", MTFWIPIntreq.ToPCCode.Trim());
        //        cmd.Parameters.AddWithValue("@ForProfitCenterCode", 0);
        //        cmd.Parameters.AddWithValue("@RequisitionCode", MTFWIPIntreq.ReqCode.Trim());
        //        cmd.Parameters.AddWithValue("@CpyPartCode", MTFWIPIntreq.ProdPartCode.Trim());
        //        cmd.Parameters.AddWithValue("@PlanPartCode", "0");
        //        cmd.Parameters.AddWithValue("@SCode", "0");
        //        cmd.Parameters.AddWithValue("@PlanIssueQty", MTFWIPIntreq.MTFQty);
        //        cmd.Parameters.AddWithValue("@ReceivedDt", DateTime.Now);
        //        cmd.Parameters.AddWithValue("@MTFStatus", "D");
        //        cmd.Parameters.AddWithValue("@CompanyCode", MTFWIPIntreq.FromPCCode.Trim().Substring(0, 2));
        //        cmd.Parameters.AddWithValue("@Remark", MTFWIPIntreq.Remark.Trim());
        //        cmd.Parameters.AddWithValue("@WtPerUt", 0);
        //        cmd.Parameters.AddWithValue("@SqftPerUt", 0);
        //        cmd.Transaction = tran;
        //        cmd.ExecuteNonQuery();
        //        cmd.Dispose();

        //       int recCount = ComCon.CountChars(MTFWIPIntreq.MTFDetails, ",");
        //        string[] strMTFDts = Regex.Split(MTFWIPIntreq.MTFDetails, ",");

        //         int SrNo = 0;
        //        for (int cSub = 0; cSub <= recCount; cSub++)                
        //        {
        //            SrNo += 1;
        //            string[] Dts = Regex.Split(strMTFDts[cSub].ToString().Trim(), "-->");

        //            cmd = new SqlCommand("InsertPlanMTFDetails", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@MTFCode", MTFCode.Trim());
        //            cmd.Parameters.AddWithValue("@SrNo", SrNo);
        //            cmd.Parameters.AddWithValue("@PartCode", Dts[0].ToString().Trim());
        //            cmd.Parameters.AddWithValue("@PendingQty", 0);
        //            cmd.Parameters.AddWithValue("@IssueQty", double.Parse(Dts[1].ToString().Trim()));
        //            cmd.Parameters.AddWithValue("@Remark", "WIP Internal");
        //            cmd.Parameters.AddWithValue("@Rate", double.Parse(Dts[2].ToString().Trim()));
        //            cmd.Parameters.AddWithValue("@MemoStatus", "D");
        //            cmd.Parameters.AddWithValue("@WtPerUt", 0);
        //            cmd.Parameters.AddWithValue("@SqftPerUt", 0);
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();

        //            sb.Remove(0, sb.Length);
        //            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
        //            sb.Append(" values('" + MTFWIPIntreq.FromPCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
        //            sb.Append("'" + MTFCode.Trim() + "','" + System.DateTime.Now + "','" + double.Parse(Dts[1].ToString().Trim()) + "','" + MTFWIPIntreq.ToPCCode.Trim() + "',0)");
        //            cmd = new SqlCommand(sb.ToString(), con);
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();

        //            if (Dts[0].Trim().Substring(0, 3) == "001" || Dts[0].Trim().Substring(0, 3) == "002" || Dts[0].Trim().Substring(0, 3) == "010")
        //            {
        //                dsDetailsSub = ComCon.procTranDS("exec MTFSRNo '" + Dts[0] + "'," + Dts[1].Trim() + "", "tbl_MTFSrNo", con, tran);
        //                if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_MTFSrNo"].Rows.Count > 0)
        //                {

        //                    int SrNok = 0;
        //                    for (int k = 0; k < dsDetailsSub.Tables["tbl_MTFSrNo"].Rows.Count; k++)
        //                    {
        //                        SrNok += 1;
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("insert into MTFDetailssub(MTFCode,SrNo,PartCode,SerialNo,JobcardStatus) ");
        //                        sb.Append("values('" + MTFCode.Trim() + "','" + SrNok + "','" + Dts[0].Trim() + "',");
        //                        sb.Append("'" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["SerialNo"].ToString().Trim() + "',");
        //                        if (dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["JobcardStatus"].ToString().Trim() == "J")
        //                        {
        //                            sb.Append("'J')");
        //                        }
        //                        else
        //                        {
        //                            sb.Append("'P')");
        //                        }
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update GIIRDetailsSub set TRFStatus='D',TRFCode='" + MTFCode.Trim() + "'  where  ");
        //                        sb.Append("SerialNo='" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["SerialNo"].ToString().Trim() + "' and GIIrCode='" + dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["GCode"].ToString().Trim() + "' ");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        if (dsDetailsSub.Tables["tbl_MTFSrNo"].Rows[k]["JobcardStatus"].ToString().Trim()=="J")
        //                        { 

        //                        }

        //                    }
        //                }
        //            }


        //        }
        //        if ((MTFWIPIntreq.ReqBalQty) == (MTFWIPIntreq.MTFQty))
        //        {
        //            sb.Remove(0, sb.Length);
        //            sb.Append("Update MaterialRequisitionWithoutPlan set ReqStatus='D' where  ReqCode='" + MTFWIPIntreq.ReqCode.Trim() + "' ");
        //            cmd = new SqlCommand(sb.ToString(), con);
        //            cmd.Transaction = tran;
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();
        //        }

        //        tran.Commit();
        //        return MTFCode;
        //    }
        //    catch (Exception ex)
        //    { 
        //        tran.Rollback();
        //        return ("StackTrace"+  ex.StackTrace.ToString() + "\n"+ "Message" + ex.Message.ToString()) ;
        //    }

        //    finally
        //    {
        //        con.Close();
        //    }
        //}
    }


}
