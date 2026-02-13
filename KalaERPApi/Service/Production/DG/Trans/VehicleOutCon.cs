
using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Http;
using VOutRequest = KalaERPApi.Models.Transport.Trans.VehicleOutRequest;

namespace KalaERPApi.Service.Transport.Trans
{
    public class VehicleOutCon
    {
        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb3 = new StringBuilder();
        StringBuilder sb4 = new StringBuilder();
        SqlCommand cmdSave = null;
        double CInvUnloadAmt = 0;
        double CInvTransAmt = 0;
        double CInvTransUnloadAmt = 0;
        string MaxCI = "0";
        string strYearEnd = "";
        public SqlTransaction tran = null;
        CommonCon ComCon = new CommonCon();
        #endregion

        public object SqlDbTypeChar { get; private set; }

        public DataTable GetDGScanDts(string strComp, string strInvNo, string strDGNo)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("VehileLoading_SerialNo_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@CompCode", SqlDbType.Char).Value = strComp;
            dAd.SelectCommand.Parameters.Add("@InvNo", SqlDbType.Char).Value = strInvNo;
            dAd.SelectCommand.Parameters.Add("@SerialNo", SqlDbType.Char).Value = strDGNo;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetVehicleNo(string CompCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleOutNo", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@CompID", SqlDbType.Char).Value = CompCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetNextDestination(string CompCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleNextDestination", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@CompID", SqlDbType.Char).Value = CompCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetVehicleTransporterNameComp()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleTransportNameComp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetVehicleTransporterName()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleTransportNameCompApi", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetVehicleDtls(string VNo)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleOutDetails", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@VNo", SqlDbType.Char).Value = VNo;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string getMaxNoCIV(string tablename, string fieldname, string Yr, string CompID, SqlConnection con, SqlTransaction tran)
        {
            string strmax = "0";
            int intmax = 0;
            SqlCommand cmd = new SqlCommand("select isnull(SUBSTRING(max(" + fieldname.Trim() + "),3,6),0) as MX from " + tablename.Trim() + " where yr='" + Yr.Trim() + "' and CompanyCode='" + CompID.Trim() + "' and CCode like 'CIV%'", con);

            cmd.Transaction = tran;
            cmd.CommandTimeout = 0;
            intmax = Convert.ToInt32(cmd.ExecuteScalar());
            if (intmax == 0)
                strmax = "000001";
            else if (intmax < 9)
                strmax = "00000" + (intmax + 1);
            else if (intmax < 99)
                strmax = "0000" + (intmax + 1);
            else if (intmax < 999)
                strmax = "000" + (intmax + 1);
            else if (intmax < 9999)
                strmax = "00" + (intmax + 1);
            else if (intmax < 99999)
                strmax = "0" + (intmax + 1);
            else
                strmax = Convert.ToString(intmax + 1);
            cmd.Dispose();

            return strmax;
        }

        public string Submit([FromBody] VOutRequest VoutReq)
        {
            string PrcNo = "", strVType = "", VhNo = "", StrExpNo = "";
            string strVehicleNo = "", strLRNo = "", TempExpAmt = "", TempAmtToPay = "";
            string SupOutStatus = "P";
            string CINStatus = "P", EmptyStatus = "0";
            double TravelKm = 0;
            double Tansport4Exp = 0.0;
            byte[] imageBytes = null;
            MemoryStream ms = null;

            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                string file_name = null;
                int recCount = ComCon.CountChars(VoutReq.VoutDts, ",");
                string[] strVoutDts = Regex.Split(VoutReq.VoutDts, ",");

                strVType = VoutReq.VType.Trim();
                int SrNo = 0;
                strYearEnd = ComCon.yearEnd(con, tran);
                if (strVType == "R")
                {
                    string CurrentMnth = ComCon.getName("select Mon=case MONTH(GETDATE()) " +
                     "when 1 then '01' when 2 then '02' when 3 then '03' " +
                     "when 4 then '04' when 5 then '05' when 6 then '06' " +
                     "when 7 then '07' when 8 then '08' when 9 then '09' " +
                     "when 10 then '10' when 11 then '11' when  12 then '12' end", "tblM", "Mon");

                    strLRNo = DateTime.Now.Year.ToString().Substring(2, 2) + CurrentMnth.Trim() + DateTime.Now.ToString("dd") + DateTime.Now.Hour + DateTime.Now.Minute;
                }
                else if (strVType == "NR")
                {
                    strLRNo = VoutReq.LRNo.ToString().Trim();
                }


                if (VoutReq.TType.ToString().Trim() == "E")
                {
                    if (VoutReq.NextDestination.ToString().Trim() == "")
                    {
                        if (string.IsNullOrEmpty(VoutReq.FinalDestination.ToString().Trim()))
                        {
                            if (strVType == "R")
                            {
                                SupOutStatus = "S";
                                CINStatus = "C";
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(VoutReq.LRNo.ToString().Trim()))
                {
                    int CntDuplicate = int.Parse(ComCon.getName("SELECT COUNT(VoutNo) AS CntLR FROM VehicleOut WHERE Active='1' AND LRNo='" + VoutReq.LRNo.ToString().Trim() + "' AND Transporter='" + VoutReq.TransporterName.ToString().Trim() + "'", "tblDup", "CntLR"));

                    if (CntDuplicate > 0)
                    {
                        strVehicleNo = "LRNo Already Exists for Transporter : " + VoutReq.TransporterName.ToString().Trim() + " !";
                        return strVehicleNo;
                    }
                }

                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    string[] Dts = Regex.Split(strVoutDts[cSub].ToString().Trim(), "-->");
                    if (VoutReq.ExpAmt.Trim() != "")
                    {
                        TempExpAmt += double.Parse(VoutReq.ExpAmt.Trim());
                    }
                    if (Dts[7].Trim() != "")
                    // AmtToPay
                    {
                        TempAmtToPay += double.Parse(Dts[7].Trim());
                    }
                    if (TempExpAmt != TempAmtToPay)
                    {
                        // VhNo = "Divided amount should not match";
                        //return VhNo;
                    }
                    TravelKm = Convert.ToDouble(VoutReq.VoutReading.Trim()) - Convert.ToDouble(VoutReq.VPrevReading.Trim());
                }

                strVehicleNo = ComCon.GetMaxNo("VehicleOut", "VON", VoutReq.CompID.Trim(), con, tran);
                StrExpNo = ComCon.GetMaxNo("ExpenceRequisitionWithPlan", "ERW", VoutReq.CompID.Trim(), con, tran);

                cmd = new SqlCommand("InsertVehicleOut", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VOutNo", strVehicleNo.Trim());
                cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@Yr", strYearEnd.Trim());
                cmd.Parameters.AddWithValue("@MaxSrNo", (strVehicleNo.Substring(10, 8)));
                cmd.Parameters.AddWithValue("@PCName", VoutReq.PCCode.Trim());
                if (VoutReq.TType.ToString().Trim() == "E")
                {
                    cmd.Parameters.AddWithValue("@NextDestination", VoutReq.FinalDestination);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@NextDestination", VoutReq.NextDestination);
                }
                cmd.Parameters.AddWithValue("@FinalDestination", VoutReq.FinalDestination);
                cmd.Parameters.AddWithValue("@VehicleNo", VoutReq.VehicleNo);
                cmd.Parameters.AddWithValue("@Transporter", VoutReq.TransporterName);
                cmd.Parameters.AddWithValue("@DriverName", VoutReq.DriverName);
                cmd.Parameters.AddWithValue("@DriverMobileNo", VoutReq.DriverMobileNo);
                cmd.Parameters.AddWithValue("@PrvReading", Convert.ToDouble(VoutReq.VPrevReading));
                cmd.Parameters.AddWithValue("@OutReading", Convert.ToDouble(VoutReq.VoutReading));
                cmd.Parameters.AddWithValue("@TransportCharge", 0);
                cmd.Parameters.AddWithValue("@LoadUnLoadCharge", 0);
                cmd.Parameters.AddWithValue("@Diesel", Convert.ToDouble(VoutReq.Diesel));
                cmd.Parameters.AddWithValue("@LRNo", strLRNo.Trim());
                if (VoutReq.TType.ToString().Trim() == "E")
                {
                    cmd.Parameters.AddWithValue("@LRDt", VoutReq.LRDate);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@LRDt", DateTime.Now.ToString("yyyy-MM-dd"));
                }
                cmd.Parameters.AddWithValue("@Remark", VoutReq.Remark);
                cmd.Parameters.AddWithValue("@AuthRemark", "OK");
                cmd.Parameters.AddWithValue("@SecurityRemark", "NIL");
                cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
                cmd.Parameters.AddWithValue("@TType", VoutReq.TType.Trim());
                cmd.Parameters.AddWithValue("@GateOutStatus", "P");
                cmd.Parameters.AddWithValue("@SupOutStatus", SupOutStatus.Trim());
                cmd.Parameters.AddWithValue("@EmptyStatus", EmptyStatus.Trim());
                cmd.Parameters.AddWithValue("@CINStatus", CINStatus.Trim());
                cmd.Parameters.AddWithValue("@BreakDownStatus", 0);
                cmd.Parameters.AddWithValue("@VType", strVType.Trim());
                cmd.Parameters.AddWithValue("@Active", 1);
                cmd.Parameters.AddWithValue("@Auth", 1);
                cmd.Parameters.AddWithValue("@Status", 0);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmd = new SqlCommand("InsertVehicleTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VehicleNo", VoutReq.VehicleNo);
                cmd.Parameters.AddWithValue("@Reading", Convert.ToDouble(VoutReq.VoutReading));
                cmd.Parameters.AddWithValue("@OutTransactionNo", strVehicleNo.Trim());
                cmd.Parameters.AddWithValue("@OutDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@TransportCharge", 0);
                cmd.Parameters.AddWithValue("@Diesel", Convert.ToDouble(VoutReq.Diesel));
                cmd.Parameters.AddWithValue("@InTransactionNo", 0);
                cmd.Parameters.AddWithValue("@InDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@TravelKM", TravelKm);
                cmd.Parameters.AddWithValue("@BreakDownStatus", 0);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                DataSet dsDiesel = ComCon.procDS("select ID,VehicleNo,convert(varchar(10),Dt,103)+' '+right(convert(varchar,Dt,100),8) as sysDt " +
                " ,convert(varchar(10),DieselFillDt,103)+' '+right(convert(varchar,DieselFillDt,100),8) as FillDt " +
                " ,DieselQty,DieselAmt,DieselQty,Remark from DieselUpdation where VehicleNo='" + VoutReq.VehicleNo.Trim() + "' " +
                 " and InOutUsedStatus='P' order by ID", "DieselUpdation");
                if (dsDiesel.Tables["DieselUpdation"].Rows.Count > 0)
                {
                    for (int i = 0; i < dsDiesel.Tables["DieselUpdation"].Rows.Count; i++)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Insert into DieselUpdationDetails(SrNo,DieselUpdationID,VInOrVOutNo)");
                        sb.Append(" VALUES('" + (i + 1) + "','" + dsDiesel.Tables["DieselUpdation"].Rows[i]["ID"].ToString().Trim() + "',");
                        sb.Append("'" + strVehicleNo.Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE DieselUpdation SET InOutUsedStatus='C' WHERE VehicleNo='" + VoutReq.VehicleNo.Trim() + "' ");
                        sb.Append("AND Active='1' AND InOutUsedStatus='P' and ID='" + dsDiesel.Tables["DieselUpdation"].Rows[i]["ID"].ToString().Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                dsDiesel.Dispose();
                dsDiesel.Clear();

                int cntSrNo = 0;
                string strAmt = "0";
                Tansport4Exp = 0;

                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo += 1;
                    string[] Dts = Regex.Split(strVoutDts[cSub].ToString().Trim(), "-->");
                    if (Dts[0].Trim() == "")
                    {
                        strAmt = "0";
                    }
                    else
                    {
                        strAmt = Dts[6].Trim();
                    }

                    string strExpectedDOD = "";
                    if (!string.IsNullOrEmpty(Dts[10].Trim()))
                    {
                        strExpectedDOD = Dts[10].Trim().Substring(0, 10);
                    }
                    else
                    {
                        strExpectedDOD = Convert.ToString(DBNull.Value);
                    }

                    Tansport4Exp = Tansport4Exp + Convert.ToDouble(Dts[9].Trim());
                    cntSrNo = cntSrNo + 1;
                    cmd = new SqlCommand("InsertVehicleOutDetailsApi", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VOutNo", strVehicleNo.Trim());
                    cmd.Parameters.AddWithValue("@SrNo", cntSrNo);
                    cmd.Parameters.AddWithValue("@InvoiceID", Dts[0].Trim());
                    cmd.Parameters.AddWithValue("@INVCode", Dts[3].Trim());
                    cmd.Parameters.AddWithValue("@ExpectedDOD", strExpectedDOD.Trim());
                    cmd.Parameters.AddWithValue("@INVDt", Dts[1].Trim().Substring(0, 10));
                    cmd.Parameters.AddWithValue("@PartDescT", "0");
                    cmd.Parameters.AddWithValue("@PartCodeT", "0");
                    cmd.Parameters.AddWithValue("@QtyT", 0);
                    cmd.Parameters.AddWithValue("@Amt", Convert.ToDouble(strAmt.Trim()));
                    cmd.Parameters.AddWithValue("@SanctionAmtT", 0);
                    cmd.Parameters.AddWithValue("@TentativeKm", Dts[5].Trim());
                    cmd.Parameters.AddWithValue("@InvoiceType", Dts[4].Trim());
                    cmd.Parameters.AddWithValue("@MeMOFCode", Dts[2].Trim());
                    cmd.Parameters.AddWithValue("@Status", 0);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE InvoiceSales SET VIOStatus='O' WHERE INVID='" + Dts[0].Trim() + "' ");
                    sb.Append("AND VIOStatus='I' AND Active='1' AND Auth='1' AND CompanyCode='" + VoutReq.CompID.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE Challan45 SET VIOStatus='O' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE ChallanOutReturnable SET VIOStatus='O' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE InvoiceExport SET VIOStatus='O' WHERE INVID='" + Dts[0].Trim() + "' ");
                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE InvoiceCommercial SET VIOStatus='O' WHERE CCode='" + Dts[0].Trim() + "' ");
                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE InvoiceDealer SET VIOStatus='O' WHERE INVID='" + Dts[0].Trim() + "' ");
                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE InvoiceDealer SET VIOStatus='O' WHERE INVID='" + Dts[0].Trim() + "' ");
                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE deliverychallan SET VehIOStatus='O' WHERE DCCode='" + Dts[0].Trim() + "' ");
                    sb.Append(" and Active='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    if (VoutReq.CompID.Trim() == "01")
                    {
                        if (Dts[0].Trim().Substring(10, 2) == "14" || Dts[0].Trim().Substring(10, 2) == "10" || Dts[0].Trim().Substring(10, 2) == "13")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("UPDATE InvoiceSales SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
                            sb.Append(" AND VIOStatus='O' AND Active='1' AND Auth='1' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("UPDATE Challan45 SET VIOStatus='OO' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
                            sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("UPDATE ChallanOutReturnable SET VIOStatus='OO' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
                            sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("UPDATE InvoiceExport SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
                            sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("UPDATE InvoiceCommercial SET VIOStatus='OO' WHERE CCode='" + Dts[0].Trim() + "' ");
                            sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("UPDATE InvoiceDealer SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
                            sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                    else if (VoutReq.CompID.Trim() != "01" || VoutReq.CompID.Trim() != "02" || VoutReq.CompID.Trim() != "03")
                    {
                        if (VoutReq.TType.Trim() == "E")
                        {
                            if (VoutReq.NextDestination.Trim() == "")
                            {
                                if (Dts[0].Trim().Substring(10, 2) == "14" || Dts[0].Trim().Substring(10, 2) == "10" || Dts[0].Trim().Substring(10, 2) == "13")
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("UPDATE InvoiceSales SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
                                    sb.Append(" AND VIOStatus='O' AND Active='1' AND Auth='1' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("UPDATE Challan45 SET VIOStatus='OO' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
                                    sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("UPDATE ChallanOutReturnable SET VIOStatus='OO' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
                                    sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("UPDATE InvoiceExport SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
                                    sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("UPDATE InvoiceCommercial SET VIOStatus='OO' WHERE CCode='" + Dts[0].Trim() + "' ");
                                    sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("UPDATE InvoiceDealer SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
                                    sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }
                        }
                    }
                }

                // AND IOStatus='I'
                sb.Remove(0, sb.Length);
                sb.Append("UPDATE Vehicle SET IOStatus='O',IOCompany='" + VoutReq.CompID.Trim() + "'");
                sb.Append(" WHERE VNo='" + strVehicleNo.Trim() + "' AND Active='1' AND BreakDownStatus='0'");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                if (!string.IsNullOrEmpty(VoutReq.image_string))
                {
                    string[] image_string_items = Regex.Split(VoutReq.image_string, ",");
                    if (image_string_items.Length > 0)
                    {
                        System.Drawing.Image image = null;
                        int srno = 0;
                        for (int j = 0; j < image_string_items.Length; j++)
                        {
                            imageBytes = Convert.FromBase64String(image_string_items[j].Trim());
                            if (imageBytes.Length > 0)
                            {
                                ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                                ms.Write(imageBytes, 0, imageBytes.Length);
                                image = System.Drawing.Image.FromStream(ms, true);

                                file_name = strVehicleNo.ToString().Trim().Substring(4, 5).Trim() + strVehicleNo.ToString().Trim().Substring(10, 8).Trim() + "-" + (j + 1) + ".jpg";
                                var filePath = ComCon.getMainFilePath("VehicleOut") + "\\" + file_name;
                                if (!File.Exists(filePath))
                                {
                                    image.Save(filePath);

                                    srno += 1;
                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO VehicleOutFileDetails");
                                    sb.Append("(VOutNo, SrNo, FileAttachment, Active, Status)");
                                    sb.Append(" VALUES('" + strVehicleNo.Trim() + "' ,'" + srno + "','" + file_name.Trim() + "','1','0')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }
                        }
                    }
                }

                if (VoutReq.TType.Trim() == "E")
                {
                    if (VoutReq.NextDestination.Trim() == "")
                    {
                        if (!string.IsNullOrEmpty(VoutReq.NextDestination.Trim()))
                        {
                            if (strVType == "R")
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("UPDATE VehicleOut SET CINStatus='C' WHERE VehicleNo='" + strVehicleNo.Trim() + "' ");
                                sb.Append(" and Active='1' and CINStatus='P' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }
                    }
                }

                /* ExpenseRequisition */
                #region
                double Expencetotal = 0.0;

                if (Convert.ToDouble(VoutReq.ExpAmt.Trim()) > 0)
                {
                    Expencetotal = Convert.ToDouble(VoutReq.ExpAmt.Trim());
                    //Tansport4Exp = Tansport4Exp + Convert.ToDouble(dataItem["Transport"].ToString().Trim());
                    SrNo += 1;

                    cmd = new SqlCommand("InsertExpenseRequisition", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
                    cmd.Parameters.AddWithValue("@MaxSrNo", (StrExpNo.Substring(10, 8)));
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@Yr", strYearEnd.Trim());
                    cmd.Parameters.AddWithValue("@PCCode", VoutReq.PCCode.Trim());
                    if (VoutReq.TransporterName.Trim() == "0249")
                    {
                        cmd.Parameters.AddWithValue("@ExpType", "E");
                    }
                    else if (VoutReq.TransporterName.Trim() == "130079")
                    {
                        cmd.Parameters.AddWithValue("@ExpType", "E");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ExpType", "S");
                    }
                    cmd.Parameters.AddWithValue("@SupEmpCode", VoutReq.TransporterName.Trim());
                    cmd.Parameters.AddWithValue("@EmpSupCode", "0");
                    cmd.Parameters.AddWithValue("@BalAmount", 0);
                    cmd.Parameters.AddWithValue("@Advance", "0");
                    cmd.Parameters.AddWithValue("@PathA", "NA");
                    cmd.Parameters.AddWithValue("@Remark", "");
                    cmd.Parameters.AddWithValue("@CompanyCode", "03");
                    cmd.Parameters.AddWithValue("@SRVNo", 0);
                    cmd.Parameters.AddWithValue("@ACTNo", strVehicleNo.Trim());
                    cmd.Parameters.AddWithValue("@EngTransAmt", 0);
                    if ((Expencetotal <= Tansport4Exp - (Tansport4Exp * 15 / 100)) && (Tansport4Exp > 0))
                    {
                        cmd.Parameters.AddWithValue("@Auth", 1);
                        cmd.Parameters.AddWithValue("@AuthRemark", "Auto Authorize by ERP");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Auth", 0);
                        cmd.Parameters.AddWithValue("@AuthRemark", "NIL");
                    }
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //****************User Acivity****************
                    //cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
                    //cmd.Parameters.AddWithValue("@EmpID", ((string)Session["UserID"]));
                    //cmd.Parameters.AddWithValue("@TransactionType", "S");
                    //cmd.Parameters.AddWithValue("@TransactionFrom", "ExpenseRequisitionAuto");
                    //cmd.Parameters.AddWithValue("@TransactionNo", ExpReqCode.Trim());
                    //cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim() );
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    if ((Expencetotal <= Tansport4Exp - (Tansport4Exp * 15 / 100)) && (Tansport4Exp > 0))
                    {
                        if (VoutReq.TransporterName.Trim() == "0249" || VoutReq.TransporterName.Trim() == "130079")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("insert into EmpFundTransaction(EmpCode,DocType,TransCode,IssueCode,IssueDt,IssAmount,ReceivedCode,ReceivedDt,RecAmount) ");
                            sb.Append("Values('" + VoutReq.TransporterName.Trim() + "','ERW','" + StrExpNo.Trim() + "','0',NULL,'0.00',");
                            sb.Append("'" + StrExpNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Expencetotal + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO AuthorizationDetails");
                        sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                        sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,'0101','ExpenseRequisition Authorize','" + StrExpNo.Trim() + "','03')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }


                    if (Convert.ToDouble(VoutReq.ExpAmtLoadUnload.Trim()) > 0)
                    {

                        cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", "1");
                        // string[] EXPCode = Regex.Split(dataItem["EXPName"].ToString().Trim(), "-->");
                        cmd.Parameters.AddWithValue("@EXPCode", "0133");
                        cmd.Parameters.AddWithValue("@Qty", "1");
                        cmd.Parameters.AddWithValue("@Rate", Convert.ToDouble(VoutReq.ExpAmtLoadUnload.Trim()));
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(VoutReq.ExpAmtLoadUnload.Trim()));
                        cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
                        cmd.Parameters.AddWithValue("@ExpBillNo", 0);
                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        //}
                        //}
                    }

                    if (Convert.ToDouble(VoutReq.ExpAmtToll.Trim()) > 0)
                    {

                        cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", "1");
                        // string[] EXPCode = Regex.Split(dataItem["EXPName"].ToString().Trim(), "-->");
                        cmd.Parameters.AddWithValue("@EXPCode", "0130");
                        cmd.Parameters.AddWithValue("@Qty", "1");
                        cmd.Parameters.AddWithValue("@Rate", Convert.ToDouble(VoutReq.ExpAmtToll.Trim()));
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(VoutReq.ExpAmtToll.Trim()));
                        cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
                        cmd.Parameters.AddWithValue("@ExpBillNo", 0);
                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        //}
                        //}
                    }

                    if (Convert.ToDouble(VoutReq.ExpAmtStaffwel.Trim()) > 0)
                    {

                        cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", "1");
                        // string[] EXPCode = Regex.Split(dataItem["EXPName"].ToString().Trim(), "-->");
                        cmd.Parameters.AddWithValue("@EXPCode", "0132");
                        cmd.Parameters.AddWithValue("@Qty", "1");
                        cmd.Parameters.AddWithValue("@Rate", Convert.ToDouble(VoutReq.ExpAmtStaffwel.Trim()));
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(VoutReq.ExpAmtStaffwel.Trim()));
                        cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
                        cmd.Parameters.AddWithValue("@ExpBillNo", 0);
                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        //}
                        //}
                    }

                    if (Convert.ToDouble(VoutReq.ExpAmtVehExp.Trim()) > 0)
                    {

                        cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", "1");
                        // string[] EXPCode = Regex.Split(dataItem["EXPName"].ToString().Trim(), "-->");
                        cmd.Parameters.AddWithValue("@EXPCode", "0131");
                        cmd.Parameters.AddWithValue("@Qty", "1");
                        cmd.Parameters.AddWithValue("@Rate", Convert.ToDouble(VoutReq.ExpAmtVehExp.Trim()));
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(VoutReq.ExpAmtVehExp.Trim()));
                        cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
                        cmd.Parameters.AddWithValue("@ExpBillNo", 0);
                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        //}
                        //}
                    }

                    if (Convert.ToDouble(VoutReq.ExpAmtCarriageOut.Trim()) > 0)
                    {

                        cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", "1");
                        // string[] EXPCode = Regex.Split(dataItem["EXPName"].ToString().Trim(), "-->");
                        cmd.Parameters.AddWithValue("@EXPCode", "0028");
                        cmd.Parameters.AddWithValue("@Qty", "1");
                        cmd.Parameters.AddWithValue("@Rate", Convert.ToDouble(VoutReq.ExpAmtCarriageOut.Trim()));
                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(VoutReq.ExpAmtCarriageOut.Trim()));
                        cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
                        cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
                        cmd.Parameters.AddWithValue("@ExpBillNo", 0);
                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        //}
                        //}
                    }

                    //        }
                }

                #endregion
                string StrComInv = string.Empty;
                string strComInvCode = string.Empty;
                string strComInvCodeInsp = string.Empty;
                string StrTempStr1 = string.Empty;
                string StrTempStr3 = string.Empty;

                /*Start Generate CommercialInvoice for DGInspection */
                #region

                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo += 1;
                    string[] Dts = Regex.Split(strVoutDts[cSub].ToString().Trim(), "-->");
                    string DispatchFromCode = ComCon.getTranName("SELECT DispatchFromCode FROM MOF WHERE MOFCode='" + Dts[2].Trim().Trim() + "'", "tblMof1", "DispatchFromCode", con, tran);
                    string Saletransit = ComCon.getTranName("SELECT Saletransit FROM MOF WHERE MOFCode='" + Dts[2].Trim().Trim() + "'", "tblMof1", "Saletransit", con, tran);
                    CInvTransAmt = 0;
                    CInvUnloadAmt = 0;

                    if (Saletransit.Trim() == "False")
                    {
                        if (VoutReq.CompID.Trim() == DispatchFromCode.Trim() || VoutReq.CompID.Trim() == "18")
                        {
                            string Category = ComCon.getTranName("SELECT P.CategoryID FROM Part P INNER JOIN MOF M ON P.PartCode=M.PartCode WHERE M.MOFCode='" + Dts[2].Trim() + "' ", "tblC1", "CategoryID", con, tran);

                            string TransportBy = ComCon.getTranName("SELECT TransportBy FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' AND Active='1' ", "tblt1", "TransportBy", con, tran);
                            string UnloadingBy = ComCon.getTranName("SELECT UnloadingBy FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' AND Active='1' ", "tblt2", "UnloadingBy", con, tran);

                            string DICode = ComCon.getTranName("select DINo from DispatchInstruction where MOFCode='" + Dts[2].Trim() + "'", "tbDI", "DINo", con, tran);

                            string MOFPCCode = ComCon.getTranName("SELECT BranchCode FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblI1", "BranchCode", con, tran);
                            string IndentorCode = ComCon.getTranName("SELECT IndentorCode FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblI1", "IndentorCode", con, tran);
                            string OnAccountOf = ComCon.getTranName("SELECT OnAccountOf FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblOA1", "OnAccountOf", con, tran);
                            string CustomerCode = ComCon.getTranName("SELECT ConsigneeCode as CustomerCode FROM DispatchInstruction WHERE DINo='" + DICode.Trim() + "'", "tblCust1", "CustomerCode", con, tran);

                            if (VoutReq.VType.Trim() == "R")
                            {
                                if (VoutReq.CompID.Trim() == "18")
                                {
                                    if (TransportBy.Trim() == "E")
                                    {
                                        CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport,0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM1", "Transport", con, tran));
                                    }
                                    if (UnloadingBy.Trim() == "E")
                                    {
                                        CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading,0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Unloading", con, tran));
                                    }
                                }
                                else
                                {
                                    if (TransportBy.Trim() == "E")
                                    {
                                        CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport-((Transport)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM1", "Transport", con, tran));
                                    }
                                    if (UnloadingBy.Trim() == "E")
                                    {
                                        CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading-((Unloading)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Unloading", con, tran));
                                    }
                                }

                                /* if DispatchFrom Dealer-->02/10/13 Company and (TransportBy='I' & UnloadingBy='I' in MOF for only Register Vehicle) */
                                if (DispatchFromCode.Trim() == "02" || DispatchFromCode.Trim() == "10" || DispatchFromCode.Trim() == "13")
                                {
                                    if (VoutReq.CompID.Trim() == "18")
                                    {
                                        if (TransportBy.Trim() == "I")
                                        {
                                            CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport,0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM3", "Transport", con, tran));
                                        }
                                        if (UnloadingBy.Trim() == "I")
                                        {
                                            CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading,0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM4", "Unloading", con, tran));
                                        }
                                    }
                                    else
                                    {
                                        if (TransportBy.Trim() == "I")
                                        {
                                            CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport-((Transport)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM3", "Transport", con, tran));
                                        }
                                        if (UnloadingBy.Trim() == "I")
                                        {
                                            CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading-((Unloading)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM4", "Unloading", con, tran));
                                        }
                                    }
                                }

                                if (TransportBy.Trim() == "C")
                                {
                                    if (Dts[7].Trim() != "")
                                    {
                                        if (Dts[6].Trim() != "0")
                                        {
                                            //CInvTransAmt = Convert.ToDouble(dataItem["BillAmt"].ToString().Trim());
                                            //Calculate Transport Basic
                                            if (VoutReq.CompID.Trim() == "18")
                                            {
                                                CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round('" + Convert.ToDouble(Dts[7].Trim()) + "'-(('" + Convert.ToDouble(Dts[7].Trim()) + "')/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Transport", con, tran));
                                            }
                                            else
                                            {
                                                CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round('" + Convert.ToDouble(Dts[7].Trim()) + "'-(('" + Convert.ToDouble(Dts[7].Trim()) + "')/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Transport", con, tran));
                                            }
                                        }
                                    }
                                }
                            }
                            else if (VoutReq.VType.Trim() == "NR")
                            {
                                if (TransportBy.Trim() == "E")
                                {
                                    CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round('" + Convert.ToDouble(Dts[7].Trim()) + "'-(('" + Convert.ToDouble(Dts[7].Trim()) + "')/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Transport", con, tran));
                                    if (VoutReq.CompID.Trim() == "18")
                                    {
                                        CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport,0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM1", "Transport", con, tran));
                                    }
                                    else
                                    {
                                        CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport-((Transport)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM1", "Transport", con, tran));
                                    }
                                }
                                if (UnloadingBy.Trim() == "E")
                                {
                                    if (VoutReq.CompID.Trim() == "18")
                                    {
                                        CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading,0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Unloading", con, tran));
                                    }
                                    else
                                    {
                                        CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading-((Unloading)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Unloading", con, tran));
                                    }
                                }
                            }

                            StrComInv = "";
                            if (Category.Trim() == "047" || Category.Trim() == "003" || Category.Trim() == "029")/* Only For DG/Canopy/ControlPanel */
                            {
                                if (VoutReq.VType.Trim() == "R")
                                {
                                    if ((CInvTransAmt + CInvUnloadAmt) > 0)
                                    {
                                        if (CInvTransAmt > 0 && TransportBy.Trim() == "C") // For TransportBy Customer 
                                        {
                                            double CGSTPer = 0;
                                            double SGSTper = 0;
                                            double IGSTper = 0;
                                            double TaxCost = 0;
                                            CInvTransUnloadAmt = 0;
                                            DataSet ds;

                                            string strProc = "";
                                            if (VoutReq.CompID.Trim() == "18")
                                            {
                                                if (TransportBy.Trim() == "C")
                                                {
                                                    strProc = "set nocount on; ";
                                                    strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,'" + CInvTransAmt + "' as Transport,0 as Unloading,'" + CInvTransAmt + "' as TUCost, " +
                                                    " 0  as TaxationAmount " +
                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                                }
                                                else
                                                {
                                                    strProc = "set nocount on; ";
                                                    strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                    " 0 as TaxationAmount " +
                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                                }
                                            }
                                            else
                                            {
                                                if (TransportBy.Trim() == "C")
                                                {
                                                    strProc = "set nocount on; ";
                                                    strProc += "select Mofcode,CGST,SGST,IGST,'" + CInvTransAmt + "' as Transport,0 as Unloading,'" + CInvTransAmt + "' as TUCost, " +
                                                    "(('" + CInvTransAmt + "')*CGSt)/100 + (('" + CInvTransAmt + "')*SGSt)/100  + (('" + CInvTransAmt + "')*IGSt)/100  as TaxationAmount " +
                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                                }
                                                else
                                                {
                                                    strProc = "set nocount on; ";
                                                    strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                    "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                                }
                                            }

                                            ds = ComCon.procDS(strProc, "TUDetails");
                                            if (ds.Tables["TUDetails"].Rows.Count > 0)
                                            {
                                                CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
                                                SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
                                                IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
                                                TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
                                                CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());
                                            }
                                            ds.Dispose();
                                            ds.Clear();

                                            string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");

                                            //OLD
                                            // MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
                                            // StrComInv = "";
                                            //StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

                                            //Changes on DT 04-04-2021
                                            StrComInv = "";
                                            StrComInv = ComCon.GetMaxNo("InvoiceSales", "INV", VoutReq.CompID.Trim(), con, tran);


                                            StrTempStr1 = "";
                                            StrTempStr3 = "";
                                            StrTempStr1 = Dts[8].Trim();// For Partdesc
                                            StrTempStr3 = "CIVNo:" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

                                            //sb.Remove(0, sb.Length);
                                            //sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
                                            //sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
                                            //sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
                                            //sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
                                            //cmd = new SqlCommand(sb.ToString(), con);
                                            //cmd.Transaction = tran;
                                            //cmd.ExecuteNonQuery();
                                            //cmd.Dispose();

                                            //Changes on DT 04-04-2021
                                            sb.Remove(0, sb.Length);
                                            sb.Append("INSERT INTO InvoiceSales ");
                                            sb.Append("(InvId, MaxSrNo, Dt, Yr, ");
                                            sb.Append("InvCode,MECode, SOFCode, DICode, CustomerCode, IndentorCode,Qty, ");
                                            sb.Append("PONo,PODate, ChapterNo, TMCode, ActOff,TransportName,TMobileNo,VehicleNo,NatureOfRemoval, ");
                                            sb.Append("IssueDate, IssueTime, RemovalDate, RemovalTime,  ");
                                            sb.Append(" CSTPer, Frieght,Other, PrintCount,CompanyCode, TCSPer, ExciseDutyInWords, CESSInwords, HEDCESSInwords, ");
                                            sb.Append("AmountInWords, Remark, INVType, BasicInWords, OnAcParty, DescriptionManual, ");
                                            sb.Append("NetWeight, GrossWeight, PortOfDischarge, DeliveryTerms, TransportRoute,PortOfLoading, VesselFlightNo, TallyNarration, StockTransferStatus, ");
                                            sb.Append("CGSTPer,SGSTPer,IGSTPer,CGSTInWords,SGSTInWords,IGSTInWords, MatAmt,LabAmt, InvDesc,InvUOM)");
                                            sb.Append(" VALUES ('" + StrComInv.Trim() + "','" + (StrComInv.Substring(10, 8)) + "','" + DateTime.Now + "','" + strYearEnd.Trim() + "' ,");
                                            sb.Append("'" + Convert.ToInt32((StrComInv.Substring(12, 6)).ToString().Trim()) + "', '" + Dts[0].Trim() + "','0','0','" + CustomerCode.Trim() + "', '" + IndentorCode.Trim() + "', '1', ");
                                            sb.Append(" '', '" + DBNull.Value + "', '0','01','-','" + VoutReq.TransporterName.Trim() + "','NA','" + VoutReq.VehicleNo.Trim() + "','-', ");
                                            sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                            sb.Append(" '0', '0','0','0','" + VoutReq.CompID.Trim() + "' ,'0','','','',");
                                            sb.Append(" '" + CInvAmtInWord + "','Auto Invoice For Transport','CM','Zero Only','NIL','NIL',");
                                            sb.Append("'0','0','NIL','NIL','NIL','NIL','NIL','" + StrTempStr3.ToString().Trim() + "','0',");
                                            sb.Append(" '" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','Zero Only','Zero Only','Zero Only','0','0','NA','NA')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();



                                            //sb1.Remove(0, sb1.Length);
                                            //sb1.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
                                            //sb1.Append(" VALUES('" + StrComInv.Trim() + "','1','T','" + CInvTransAmt + "','996519')");
                                            //cmd = new SqlCommand(sb1.ToString(), con);
                                            //cmd.Transaction = tran;
                                            //cmd.ExecuteNonQuery();
                                            //cmd.Dispose();

                                            //Changes on DT 04-04-2021
                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into InvoicesalesDetails ");
                                            sb.Append("(InvID,SrNo,PartCode,DPUOM,Qty,Rate,DCType,DCGSTPer,DSGSTPer,DIGSTPer,DHSNCode)");
                                            sb.Append(" VALUES ('" + StrComInv.Trim() + "','1','0','Nos','1','" + CInvTransAmt + "','T','0','0','0','996519')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();


                                            //****************User Acivity****************
                                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
                                            cmd.Parameters.AddWithValue("@EmpID", (""));
                                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                                            cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
                                            cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
                                            cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                        }
                                        else if (CInvTransAmt > 0 && TransportBy.Trim() == "E")  // For TransportBy Exluded
                                        {
                                            double CGSTPer = 0;
                                            double SGSTper = 0;
                                            double IGSTper = 0;
                                            double TaxCost = 0;
                                            CInvTransUnloadAmt = 0;
                                            DataSet ds;

                                            string strProc = "";
                                            if (VoutReq.CompID.Trim() == "18")
                                            {
                                                strProc = "set nocount on; ";
                                                strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                " 0  as TaxationAmount " +
                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                            }
                                            else
                                            {
                                                strProc = "set nocount on; ";
                                                strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                            }
                                            ds = ComCon.procDS(strProc, "TUDetails");

                                            if (ds.Tables["TUDetails"].Rows.Count > 0)
                                            {
                                                CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
                                                SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
                                                IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
                                                TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
                                                CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

                                            }
                                            ds.Dispose();
                                            ds.Clear();

                                            string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");

                                            //OLD
                                            //MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
                                            //StrComInv = "";
                                            //StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

                                            //Changes on DT 04-04-2021
                                            StrComInv = "";
                                            StrComInv = ComCon.GetMaxNo("InvoiceSales", "INV", VoutReq.CompID.Trim(), con, tran);

                                            StrTempStr1 = "";
                                            StrTempStr3 = "";
                                            StrTempStr1 = Dts[8].Trim();//partdesc
                                            StrTempStr3 = "CIVNo:" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;


                                            //sb.Remove(0, sb.Length);
                                            //sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
                                            //sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                            //sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
                                            //sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
                                            //cmd = new SqlCommand(sb.ToString(), con);
                                            //cmd.Transaction = tran;
                                            //cmd.ExecuteNonQuery();
                                            //cmd.Dispose();

                                            //Changes on DT 04-04-2021
                                            sb.Remove(0, sb.Length);
                                            sb.Append("INSERT INTO InvoiceSales ");
                                            sb.Append("(InvId, MaxSrNo, Dt, Yr, ");
                                            sb.Append("InvCode,MECode, SOFCode, DICode, CustomerCode, IndentorCode,Qty, ");
                                            sb.Append("PONo,PODate, ChapterNo, TMCode, ActOff,TransportName,TMobileNo,VehicleNo,NatureOfRemoval, ");
                                            sb.Append("IssueDate, IssueTime, RemovalDate, RemovalTime,  ");
                                            sb.Append(" CSTPer, Frieght,Other, PrintCount,CompanyCode, TCSPer, ExciseDutyInWords, CESSInwords, HEDCESSInwords, ");
                                            sb.Append("AmountInWords, Remark, INVType, BasicInWords, OnAcParty, DescriptionManual, ");
                                            sb.Append("NetWeight, GrossWeight, PortOfDischarge, DeliveryTerms, TransportRoute,PortOfLoading, VesselFlightNo, TallyNarration, StockTransferStatus, ");
                                            sb.Append("CGSTPer,SGSTPer,IGSTPer,CGSTInWords,SGSTInWords,IGSTInWords, MatAmt,LabAmt, InvDesc,InvUOM)");
                                            sb.Append(" VALUES ('" + StrComInv.Trim() + "','" + (StrComInv.Substring(10, 8)) + "','" + DateTime.Now + "','" + strYearEnd.Trim() + "' ,");
                                            sb.Append("'" + Convert.ToInt32((StrComInv.Substring(12, 6)).ToString().Trim()) + "', '" + Dts[0].Trim() + "','0','0','" + CustomerCode.Trim() + "', '" + IndentorCode.Trim() + "', '1', ");
                                            sb.Append(" '', '" + DBNull.Value + "', '0','01','-','" + VoutReq.TransporterName.Trim() + "','NA','" + VoutReq.VehicleNo.Trim() + "','-', ");
                                            sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                            sb.Append(" '0', '0','0','0','" + VoutReq.CompID.Trim() + "' ,'0','','','',");
                                            sb.Append(" '" + CInvAmtInWord + "','Auto Invoice For Transport','CM','Zero Only','NIL','NIL',");
                                            sb.Append("'0','0','NIL','NIL','NIL','NIL','NIL','" + StrTempStr3.ToString().Trim() + "','0',");
                                            sb.Append(" '" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','Zero Only','Zero Only','Zero Only','0','0','NA','NA')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();

                                            //sb1.Remove(0, sb1.Length);
                                            //sb1.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
                                            //sb1.Append(" VALUES('" + StrComInv.Trim() + "','1','T','" + CInvTransAmt + "','996519')");
                                            //cmd = new SqlCommand(sb1.ToString(), con);
                                            //cmd.Transaction = tran;
                                            //cmd.ExecuteNonQuery();
                                            //cmd.Dispose();

                                            //Changes on DT 04-04-2021
                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into InvoicesalesDetails ");
                                            sb.Append("(InvID,SrNo,PartCode,DPUOM,Qty,Rate,DCType,DCGSTPer,DSGSTPer,DIGSTPer,DHSNCode)");
                                            sb.Append(" VALUES ('" + StrComInv.Trim() + "','1','0','Nos','1','" + CInvTransAmt + "','T','0','0','0','996519')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();

                                            //****************User Acivity****************
                                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                            cmd.Parameters.AddWithValue("@EmpID", (""));
                                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                                            cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
                                            cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
                                            cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                        }
                                        else if (CInvTransAmt > 0 && TransportBy.Trim() == "I")  // For TransportBy Included
                                        {

                                            double CGSTPer = 0;
                                            double SGSTper = 0;
                                            double IGSTper = 0;
                                            double TaxCost = 0;
                                            CInvTransUnloadAmt = 0;
                                            DataSet ds;
                                            string strProc = "";

                                            if (VoutReq.CompID.Trim() == "18")
                                            {
                                                strProc = "set nocount on; ";
                                                strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                " 0  as TaxationAmount " +
                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                            }
                                            else
                                            {
                                                strProc = "set nocount on; ";
                                                strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                            }
                                            ds = ComCon.procDS(strProc, "TUDetails");

                                            if (ds.Tables["TUDetails"].Rows.Count > 0)
                                            {
                                                CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
                                                SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
                                                IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
                                                TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
                                                CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

                                            }
                                            ds.Dispose();
                                            ds.Clear();

                                            string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");

                                            //OLD
                                            //MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
                                            //StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

                                            //Changes on DT 04-04-2021
                                            StrComInv = ComCon.GetMaxNo("InvoiceSales", "INV", VoutReq.CompID.Trim(), con, tran);


                                            StrTempStr1 = "";
                                            StrTempStr3 = "";
                                            StrTempStr1 = Dts[8].Trim();
                                            StrTempStr3 = "CIVNo:" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

                                            //sb.Remove(0, sb.Length);
                                            //sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
                                            //sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
                                            //sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
                                            //sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
                                            //cmd = new SqlCommand(sb.ToString(), con);
                                            //cmd.Transaction = tran;
                                            //cmd.ExecuteNonQuery();
                                            //cmd.Dispose();

                                            //Changes on DT 04-04-2021
                                            sb.Remove(0, sb.Length);
                                            sb.Append("INSERT INTO InvoiceSales ");
                                            sb.Append("(InvId, MaxSrNo, Dt, Yr, ");
                                            sb.Append("InvCode,MECode, SOFCode, DICode, CustomerCode, IndentorCode,Qty, ");
                                            sb.Append("PONo,PODate, ChapterNo, TMCode, ActOff,TransportName,TMobileNo,VehicleNo,NatureOfRemoval, ");
                                            sb.Append("IssueDate, IssueTime, RemovalDate, RemovalTime,  ");
                                            sb.Append(" CSTPer, Frieght,Other, PrintCount,CompanyCode, TCSPer, ExciseDutyInWords, CESSInwords, HEDCESSInwords, ");
                                            sb.Append("AmountInWords, Remark, INVType, BasicInWords, OnAcParty, DescriptionManual, ");
                                            sb.Append("NetWeight, GrossWeight, PortOfDischarge, DeliveryTerms, TransportRoute,PortOfLoading, VesselFlightNo, TallyNarration, StockTransferStatus, ");
                                            sb.Append("CGSTPer,SGSTPer,IGSTPer,CGSTInWords,SGSTInWords,IGSTInWords, MatAmt,LabAmt, InvDesc,InvUOM)");
                                            sb.Append(" VALUES ('" + StrComInv.Trim() + "','" + (StrComInv.Substring(10, 8)) + "','" + DateTime.Now + "','" + strYearEnd.Trim() + "' ,");
                                            sb.Append("'" + Convert.ToInt32((StrComInv.Substring(12, 6)).ToString().Trim()) + "', '" + Dts[0].Trim() + "','0','0','" + CustomerCode.Trim() + "', '" + IndentorCode.Trim() + "', '1', ");
                                            sb.Append(" '', '" + DBNull.Value + "', '0','01','-','" + VoutReq.TransporterName.Trim() + "','NA','" + VoutReq.VehicleNo.Trim() + "','-', ");
                                            sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                            sb.Append(" '0', '0','0','0','" + VoutReq.CompID.Trim() + "' ,'0','','','',");
                                            sb.Append(" '" + CInvAmtInWord + "','Auto Invoice For Transport','CM','Zero Only','NIL','NIL',");
                                            sb.Append("'0','0','NIL','NIL','NIL','NIL','NIL','" + StrTempStr3.ToString().Trim() + "','0',");
                                            sb.Append(" '" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','Zero Only','Zero Only','Zero Only','0','0','NA','NA')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();

                                            //sb1.Remove(0, sb1.Length);
                                            //sb1.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
                                            //sb1.Append(" VALUES('" + StrComInv.Trim() + "','1','T','" + CInvTransAmt + "','996519')");
                                            //cmd = new SqlCommand(sb1.ToString(), con);
                                            //cmd.Transaction = tran;
                                            //cmd.ExecuteNonQuery();
                                            //cmd.Dispose();

                                            //Changes on DT 04-04-2021
                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into InvoicesalesDetails ");
                                            sb.Append("(InvID,SrNo,PartCode,DPUOM,Qty,Rate,DCType,DCGSTPer,DSGSTPer,DIGSTPer,DHSNCode)");
                                            sb.Append(" VALUES ('" + StrComInv.Trim() + "','1','0','Nos','1','" + CInvTransAmt + "','T','0','0','0','996519')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();


                                            //****************User Acivity****************
                                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
                                            cmd.Parameters.AddWithValue("@EmpID", (""));
                                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                                            cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
                                            cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
                                            cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                        }
                                        if (CInvUnloadAmt > 0) // MOF Unloding Amount
                                        {
                                            // int CntInvoice = Convert.ToInt32(ComCon.getTranName("SELECT COUNT(CCode)AS CntInvoice FROM InvoiceCommercial WHERE MemoCode='" + Dts[0].Trim() + "' AND Active='1'", "tblC1", "CntInvoice", con, tran));

                                            //Changes on DT 04-04-2021
                                            int CntInvoice = Convert.ToInt32(ComCon.getTranName("SELECT COUNT(INVID) AS CntInvoice FROM InvoiceSales WHERE MECode='" + Dts[0].Trim() + "' AND Active='1'", "tblC1", "CntInvoice", con, tran));

                                            if (CntInvoice == 0)
                                            {
                                                double CGSTPer = 0;
                                                double SGSTper = 0;
                                                double IGSTper = 0;
                                                double TaxCost = 0;
                                                CInvTransUnloadAmt = 0;
                                                DataSet ds;

                                                string strProc = "";
                                                if (VoutReq.CompID.Trim() == "18")
                                                {
                                                    strProc = "set nocount on; ";
                                                    strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                    "0  as TaxationAmount " +
                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                                }
                                                else
                                                {
                                                    strProc = "set nocount on; ";
                                                    strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                    "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                                }

                                                ds = ComCon.procDS(strProc, "TUDetails");

                                                if (ds.Tables["TUDetails"].Rows.Count > 0)
                                                {
                                                    CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
                                                    SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
                                                    IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
                                                    TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
                                                    CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

                                                }
                                                ds.Dispose();
                                                ds.Clear();

                                                string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");

                                                //OLD
                                                //MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
                                                //StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

                                                //Changes on DT 04-04-2021
                                                StrComInv = ComCon.GetMaxNo("InvoiceSales", "INV", VoutReq.CompID.Trim(), con, tran);


                                                StrTempStr1 = "";
                                                StrTempStr3 = "";
                                                StrTempStr1 = Dts[8].Trim();
                                                StrTempStr3 = "CIVNo:" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

                                                //sb.Remove(0, sb.Length);
                                                //sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
                                                //sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
                                                //sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
                                                //sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
                                                //cmd = new SqlCommand(sb.ToString(), con);
                                                //cmd.Transaction = tran;
                                                //cmd.ExecuteNonQuery();
                                                //cmd.Dispose();

                                                //Changes on DT 04-04-2021
                                                sb.Remove(0, sb.Length);
                                                sb.Append("INSERT INTO InvoiceSales ");
                                                sb.Append("(InvId, MaxSrNo, Dt, Yr, ");
                                                sb.Append("InvCode,MECode, SOFCode, DICode, CustomerCode, IndentorCode,Qty, ");
                                                sb.Append("PONo,PODate, ChapterNo, TMCode, ActOff,TransportName,TMobileNo,VehicleNo,NatureOfRemoval, ");
                                                sb.Append("IssueDate, IssueTime, RemovalDate, RemovalTime,  ");
                                                sb.Append(" CSTPer, Frieght,Other, PrintCount,CompanyCode, TCSPer, ExciseDutyInWords, CESSInwords, HEDCESSInwords, ");
                                                sb.Append("AmountInWords, Remark, INVType, BasicInWords, OnAcParty, DescriptionManual, ");
                                                sb.Append("NetWeight, GrossWeight, PortOfDischarge, DeliveryTerms, TransportRoute,PortOfLoading, VesselFlightNo, TallyNarration, StockTransferStatus, ");
                                                sb.Append("CGSTPer,SGSTPer,IGSTPer,CGSTInWords,SGSTInWords,IGSTInWords, MatAmt,LabAmt, InvDesc,InvUOM)");
                                                sb.Append(" VALUES ('" + StrComInv.Trim() + "','" + (StrComInv.Substring(10, 8)) + "','" + DateTime.Now + "','" + strYearEnd.Trim() + "' ,");
                                                sb.Append("'" + Convert.ToInt32((StrComInv.Substring(12, 6)).ToString().Trim()) + "', '" + Dts[0].Trim() + "','0','0','" + CustomerCode.Trim() + "', '" + IndentorCode.Trim() + "', '1', ");
                                                sb.Append(" '', '" + DBNull.Value + "', '0','01','-','" + VoutReq.TransporterName.Trim() + "','NA','" + VoutReq.VehicleNo.Trim() + "','-', ");
                                                sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                                sb.Append(" '0', '0','0','0','" + VoutReq.CompID.Trim() + "' ,'0','','','',");
                                                sb.Append(" '" + CInvAmtInWord + "','Auto Invoice For Transport','CM','Zero Only','NIL','NIL',");
                                                sb.Append("'0','0','NIL','NIL','NIL','NIL','NIL','" + StrTempStr3.ToString().Trim() + "','0',");
                                                sb.Append(" '" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','Zero Only','Zero Only','Zero Only','0','0','NA','NA')");
                                                cmd = new SqlCommand(sb.ToString(), con);
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();



                                                //****************User Acivity****************
                                                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
                                                cmd.Parameters.AddWithValue("@EmpID", (""));
                                                cmd.Parameters.AddWithValue("@TransactionType", "S");
                                                cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
                                                cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
                                                cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();
                                            }

                                            //sb2.Remove(0, sb2.Length);
                                            //sb2.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
                                            //sb2.Append(" VALUES('" + StrComInv.Trim() + "','2','U','" + CInvUnloadAmt + "','999799')");
                                            //cmdSave = new SqlCommand(sb2.ToString(), con);
                                            //cmdSave.Transaction = tran;
                                            //cmdSave.ExecuteNonQuery();
                                            //cmdSave.Dispose();

                                            //Changes on DT 04-04-2021
                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into InvoicesalesDetails (InvID,SrNo,PartCode,DPUOM,Qty,Rate,DCType,DCGSTPer,DSGSTPer,DIGSTPer,DHSNCode)");
                                            sb.Append(" VALUES ('" + StrComInv.Trim() + "','2','0','Nos','1','" + CInvUnloadAmt + "','U','0','0','0','999799')");
                                            cmdSave = new SqlCommand(sb.ToString(), con);
                                            cmdSave.Transaction = tran;
                                            cmdSave.ExecuteNonQuery();
                                            cmdSave.Dispose();

                                        }
                                    }
                                }
                                else if (VoutReq.VType.Trim() == "NR")
                                {
                                    if ((CInvTransAmt + CInvUnloadAmt) > 0)
                                    {
                                        if (CInvTransAmt > 0)
                                        {

                                            double CGSTPer = 0;
                                            double SGSTper = 0;
                                            double IGSTper = 0;
                                            double TaxCost = 0;
                                            CInvTransUnloadAmt = 0;
                                            DataSet ds;

                                            string strProc = "";
                                            if (VoutReq.CompID.Trim() == "18")
                                            {
                                                strProc = "set nocount on; ";
                                                strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                "0  as TaxationAmount " +
                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                            }
                                            else
                                            {
                                                strProc = "set nocount on; ";
                                                strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                            }
                                            ds = ComCon.procDS(strProc, "TUDetails");
                                            if (ds.Tables["TUDetails"].Rows.Count > 0)
                                            {
                                                CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
                                                SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
                                                IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
                                                TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
                                                CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

                                            }
                                            ds.Dispose();
                                            ds.Clear();

                                            string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");

                                            //MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
                                            //StrComInv = string.Empty;
                                            //StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

                                            //Changes on DT 04-04-2021
                                            StrComInv = string.Empty;
                                            StrComInv = ComCon.GetMaxNo("InvoiceSales", "INV", VoutReq.CompID.Trim(), con, tran);

                                            StrTempStr1 = "";
                                            StrTempStr3 = "";
                                            StrTempStr1 = Dts[8].Trim();
                                            StrTempStr3 = "CIVNo :" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

                                            //sb.Remove(0, sb.Length);
                                            //sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
                                            //sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
                                            //sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
                                            //sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
                                            //cmd = new SqlCommand(sb.ToString(), con);
                                            //cmd.Transaction = tran;
                                            //cmd.ExecuteNonQuery();
                                            //cmd.Dispose();


                                            //Changes on DT 04-04-2021
                                            sb.Remove(0, sb.Length);
                                            sb.Append("INSERT INTO InvoiceSales ");
                                            sb.Append("(InvId, MaxSrNo, Dt, Yr, ");
                                            sb.Append("InvCode,MECode, SOFCode, DICode, CustomerCode, IndentorCode,Qty, ");
                                            sb.Append("PONo,PODate, ChapterNo, TMCode, ActOff,TransportName,TMobileNo,VehicleNo,NatureOfRemoval, ");
                                            sb.Append("IssueDate, IssueTime, RemovalDate, RemovalTime,  ");
                                            sb.Append(" CSTPer, Frieght,Other, PrintCount,CompanyCode, TCSPer, ExciseDutyInWords, CESSInwords, HEDCESSInwords, ");
                                            sb.Append("AmountInWords, Remark, INVType, BasicInWords, OnAcParty, DescriptionManual, ");
                                            sb.Append("NetWeight, GrossWeight, PortOfDischarge, DeliveryTerms, TransportRoute,PortOfLoading, VesselFlightNo, TallyNarration, StockTransferStatus, ");
                                            sb.Append("CGSTPer,SGSTPer,IGSTPer,CGSTInWords,SGSTInWords,IGSTInWords, MatAmt,LabAmt, InvDesc,InvUOM)");
                                            sb.Append(" VALUES ('" + StrComInv.Trim() + "','" + (StrComInv.Substring(10, 8)) + "','" + DateTime.Now + "','" + strYearEnd.Trim() + "' ,");
                                            sb.Append("'" + Convert.ToInt32((StrComInv.Substring(12, 6)).ToString().Trim()) + "', '" + Dts[0].Trim() + "','0','0','" + CustomerCode.Trim() + "', '" + IndentorCode.Trim() + "', '1', ");
                                            sb.Append(" '', '" + DBNull.Value + "', '0','01','-','" + VoutReq.TransporterName.Trim() + "','NA','" + VoutReq.VehicleNo.Trim() + "','-', ");
                                            sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                            sb.Append(" '0', '0','0','0','" + VoutReq.CompID.Trim() + "' ,'0','','','',");
                                            sb.Append(" '" + CInvAmtInWord + "','Auto Invoice For Transport','CM','Zero Only','NIL','NIL',");
                                            sb.Append("'0','0','NIL','NIL','NIL','NIL','NIL','" + StrTempStr3.ToString().Trim() + "','0',");
                                            sb.Append(" '" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','Zero Only','Zero Only','Zero Only','0','0','NA','NA')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();


                                            //sb3.Remove(0, sb3.Length);
                                            //sb3.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
                                            //sb3.Append(" VALUES('" + StrComInv.Trim() + "','1','T','" + CInvTransAmt + "','996519')");
                                            //cmd = new SqlCommand(sb3.ToString(), con);
                                            //cmd.Transaction = tran;
                                            //cmd.ExecuteNonQuery();
                                            //cmd.Dispose();

                                            //Changes on DT 04-04-2021
                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into InvoicesalesDetails (InvID,SrNo,PartCode,DPUOM,Qty,Rate,DCType,DCGSTPer,DSGSTPer,DIGSTPer,DHSNCode)");
                                            sb.Append(" VALUES ('" + StrComInv.Trim() + "','1','0','Nos','1','" + CInvTransAmt + "','T','0','0','0','996519')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();


                                            //****************User Acivity****************
                                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
                                            cmd.Parameters.AddWithValue("@EmpID", (""));
                                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                                            cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
                                            cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
                                            cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                        }
                                        if (CInvUnloadAmt > 0)
                                        {
                                            //old
                                            //int CntInvoice = Convert.ToInt32(ComCon.getTranName("SELECT COUNT(CCode)AS CntInvoice FROM InvoiceCommercial WHERE MemoCode='" + Dts[0].Trim() + "' AND Active='1'", "tblC1", "CntInvoice", con, tran));

                                            //Changes on DT 04-04-2021
                                            int CntInvoice = Convert.ToInt32(ComCon.getTranName("SELECT COUNT(INVID) AS CntInvoice FROM InvoiceSales  WHERE MECode='" + Dts[0].Trim() + "' AND Active='1'", "tblC1", "CntInvoice", con, tran));


                                            if (CntInvoice == 0)
                                            {
                                                double CGSTPer = 0;
                                                double SGSTper = 0;
                                                double IGSTper = 0;
                                                double TaxCost = 0;
                                                CInvTransUnloadAmt = 0;
                                                DataSet ds;

                                                string strProc = "";
                                                if (VoutReq.CompID.Trim() == "18")
                                                {
                                                    strProc = "set nocount on; ";
                                                    strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                    "0  as TaxationAmount " +
                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                                }
                                                else
                                                {
                                                    strProc = "set nocount on; ";
                                                    strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                                    "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                                }
                                                ds = ComCon.procDS(strProc, "TUDetails");
                                                if (ds.Tables["TUDetails"].Rows.Count > 0)
                                                {
                                                    CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
                                                    SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
                                                    IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
                                                    TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
                                                    CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

                                                }
                                                ds.Dispose();
                                                ds.Clear();

                                                string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");

                                                //MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
                                                // StrComInv = string.Empty;
                                                // StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

                                                //Changes on DT 04-04-2021
                                                StrComInv = string.Empty;
                                                StrComInv = ComCon.GetMaxNo("InvoiceSales", "INV", VoutReq.CompID.Trim(), con, tran);

                                                StrTempStr1 = "";
                                                StrTempStr3 = "";
                                                StrTempStr1 = Dts[8].Trim();
                                                StrTempStr3 = "CIVNo :" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

                                                //sb.Remove(0, sb.Length);
                                                //sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
                                                //sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
                                                //sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
                                                //sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
                                                //cmd = new SqlCommand(sb.ToString(), con);
                                                //cmd.Transaction = tran;
                                                //cmd.ExecuteNonQuery();
                                                //cmd.Dispose();

                                                //Changes on DT 04-04-2021
                                                sb.Remove(0, sb.Length);
                                                sb.Append("INSERT INTO InvoiceSales ");
                                                sb.Append("(InvId, MaxSrNo, Dt, Yr, ");
                                                sb.Append("InvCode,MECode, SOFCode, DICode, CustomerCode, IndentorCode,Qty, ");
                                                sb.Append("PONo,PODate, ChapterNo, TMCode, ActOff,TransportName,TMobileNo,VehicleNo,NatureOfRemoval, ");
                                                sb.Append("IssueDate, IssueTime, RemovalDate, RemovalTime,  ");
                                                sb.Append(" CSTPer, Frieght,Other, PrintCount,CompanyCode, TCSPer, ExciseDutyInWords, CESSInwords, HEDCESSInwords, ");
                                                sb.Append("AmountInWords, Remark, INVType, BasicInWords, OnAcParty, DescriptionManual, ");
                                                sb.Append("NetWeight, GrossWeight, PortOfDischarge, DeliveryTerms, TransportRoute,PortOfLoading, VesselFlightNo, TallyNarration, StockTransferStatus, ");
                                                sb.Append("CGSTPer,SGSTPer,IGSTPer,CGSTInWords,SGSTInWords,IGSTInWords, MatAmt,LabAmt, InvDesc,InvUOM)");
                                                sb.Append(" VALUES ('" + StrComInv.Trim() + "','" + (StrComInv.Substring(10, 8)) + "','" + DateTime.Now + "','" + strYearEnd.Trim() + "' ,");
                                                sb.Append("'" + Convert.ToInt32((StrComInv.Substring(12, 6)).ToString().Trim()) + "', '" + Dts[0].Trim() + "','0','0','" + CustomerCode.Trim() + "', '" + IndentorCode.Trim() + "', '1', ");
                                                sb.Append(" '', '" + DBNull.Value + "', '0','01','-','" + VoutReq.TransporterName.Trim() + "','NA','" + VoutReq.VehicleNo.Trim() + "','-', ");
                                                sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                                sb.Append(" '0', '0','0','0','" + VoutReq.CompID.Trim() + "' ,'0','','','',");
                                                sb.Append(" '" + CInvAmtInWord + "','Auto Invoice For Transport','CM','Zero Only','NIL','NIL',");
                                                sb.Append("'0','0','NIL','NIL','NIL','NIL','NIL','" + StrTempStr3.ToString().Trim() + "','0',");
                                                sb.Append(" '" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','Zero Only','Zero Only','Zero Only','0','0','NA','NA')");
                                                cmd = new SqlCommand(sb.ToString(), con);
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();


                                                //****************User Acivity****************
                                                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
                                                cmd.Parameters.AddWithValue("@EmpID", (""));
                                                cmd.Parameters.AddWithValue("@TransactionType", "S");
                                                cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
                                                cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
                                                cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();
                                            }

                                            //sb4.Remove(0, sb4.Length);
                                            //sb4.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
                                            //sb4.Append(" VALUES('" + StrComInv.Trim() + "','2','U','" + CInvUnloadAmt + "','999799')");
                                            //cmdSave = new SqlCommand(sb4.ToString(), con);
                                            //cmdSave.Transaction = tran;
                                            //cmdSave.ExecuteNonQuery();
                                            //cmdSave.Dispose();

                                            //Changes on DT 04-04-2021
                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into InvoicesalesDetails (InvID,SrNo,PartCode,DPUOM,Qty,Rate,DCType,DCGSTPer,DSGSTPer,DIGSTPer,DHSNCode)");
                                            sb.Append(" VALUES ('" + StrComInv.Trim() + "','2','0','Nos','1','" + CInvUnloadAmt + "','U','0','0','0','999799')");
                                            cmdSave = new SqlCommand(sb.ToString(), con);
                                            cmdSave.Transaction = tran;
                                            cmdSave.ExecuteNonQuery();
                                            cmdSave.Dispose();

                                        }
                                    }
                                }

                                if ((CInvTransAmt + CInvUnloadAmt) > 0)
                                {
                                    // Issue Against Bill  
                                    double TaxCost = 0;
                                    CInvTransUnloadAmt = 0;
                                    DataSet ds;

                                    string strProc = "";
                                    if (VoutReq.CompID.Trim() == "18")
                                    {
                                        if (TransportBy.Trim() == "C")
                                        {
                                            strProc = "set nocount on; ";
                                            strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,'" + CInvTransAmt + "' as Transport,0 as Unloading,'" + CInvTransAmt + "' as TUCost, " +
                                            "0  as TaxationAmount " +
                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                        }
                                        else
                                        {
                                            strProc = "set nocount on; ";
                                            strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                            "0  as TaxationAmount " +
                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                        }
                                    }
                                    else
                                    {
                                        if (TransportBy.Trim() == "C")
                                        {
                                            strProc = "set nocount on; ";
                                            strProc += "select Mofcode,CGST,SGST,IGST,'" + CInvTransAmt + "' as Transport,0 as Unloading,'" + CInvTransAmt + "' as TUCost, " +
                                            "(('" + CInvTransAmt + "')*CGSt)/100 + (('" + CInvTransAmt + "')*SGSt)/100  + (('" + CInvTransAmt + "')*IGSt)/100  as TaxationAmount " +
                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                        }
                                        else
                                        {
                                            strProc = "set nocount on; ";
                                            strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
                                            "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                        }
                                    }
                                    ds = ComCon.procDS(strProc, "TUDetails");
                                    if (ds.Tables["TUDetails"].Rows.Count > 0)
                                    {
                                        TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
                                        CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

                                    }
                                    ds.Dispose();
                                    ds.Clear();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO ReceiptNoteTransaction");
                                    sb.Append("(ONAccountOF,TrnsIndCode,TrnsCustCode,TransactionCode,TrnsPCCode,RecDINo,IssueCode,IssueDt,IssueAmount,Type,DueDt,Status,TransCompCode)");
                                    sb.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "', ");
                                    sb.Append(" '" + CustomerCode.Trim() + "','" + StrComInv.Trim() + "',");
                                    sb.Append(" '" + MOFPCCode.Trim() + "','" + DICode.Trim() + "','" + Dts[2].Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");
                                    sb.Append(" '" + Math.Round((CInvTransUnloadAmt)) + "','IV','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");
                                    sb.Append(" '0','" + VoutReq.CompID.Trim() + "' )");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    // Received Against DI (if select any cheque)
                                    ds = ComCon.procTranDS("select ReceiptNo,sum(IssAmt) as IssAmt,sum(RecAmt) as RecAmt,(sum(IssAmt)-sum(RecAmt)) as BalAmt " +
                                           " from (select IssueCode as ReceiptNo,sum(IssueAmount) as IssAmt,0.00 as RecAmt from receiptnotetransaction " +
                                           " where recdino='" + DICode.Trim() + "' and IssueCode like 'RCN%' group by IssueCode " +
                                           " union all " +
                                           " select ReceivedCode as ReceiptNo,0.00 as IssAmt,sum(ReceivedAmount) as RecAmt from receiptnotetransaction " +
                                           " where recdino='" + DICode.Trim() + "' and ReceivedCode like 'RCN%' group by ReceivedCode )as T " +
                                           " group by ReceiptNo having (sum(IssAmt)-sum(RecAmt))>0", "ReceivedData", con, tran);
                                    if (ds != null && ds.Tables["ReceivedData"].Rows.Count > 0)
                                    {
                                        double PayTotalAmt = Math.Round((CInvTransAmt + CInvUnloadAmt + TaxCost));
                                        for (int j = 0; j < ds.Tables["ReceivedData"].Rows.Count; j++)
                                        {
                                            if (PayTotalAmt == 0)
                                            {
                                                break;
                                            }
                                            double BalAdjAmt = Convert.ToDouble(ComCon.getTranName("select (sum(IssAmt)-sum(RecAmt)) as BalAmt from " +
                                                    " (select IssueCode as ReceiptNo,sum(IssueAmount) as IssAmt,0.00 as RecAmt from receiptnotetransaction " +
                                                    " where recdino='" + DICode.Trim() + "' and IssueCode like 'RCN%' " +
                                                    " group by IssueCode union all select ReceivedCode as ReceiptNo,0.00 as IssAmt,sum(ReceivedAmount) as RecAmt from receiptnotetransaction " +
                                                    " where recdino='" + DICode.Trim() + "' and ReceivedCode like 'RCN%' " +
                                                    " group by ReceivedCode )as T where ReceiptNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "' group by ReceiptNo " +
                                                    " having (sum(IssAmt)-sum(RecAmt))>0", "DIReceiptTransaction", "BalAmt", con, tran));
                                            if (PayTotalAmt >= BalAdjAmt)
                                            {
                                                //Adj DI Adv Against Inv (Receive)
                                                sb2.Remove(0, sb2.Length);
                                                sb2.Append("INSERT INTO ReceiptNoteTransaction");
                                                sb2.Append("(ONAccountOF,TrnsIndCode,TrnsCustCode, TransactionCode,TrnsPCCode,RecDINo,ReceivedCode,");
                                                sb2.Append("ReceivedDt,ReceivedAmount,Type,DueDt,Status,TransCompCode)");
                                                sb2.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "','" + CustomerCode.Trim() + "',");
                                                sb2.Append("'" + StrComInv.Trim() + "','" + MOFPCCode.Trim() + "','" + DICode.Trim() + "',");
                                                sb2.Append("'" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + BalAdjAmt + "',");
                                                sb2.Append("'" + ComCon.getTranName("select PayType from dispatchinstructiondetailssub where AdjRecNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "'", "dispatchinstructiondetailssub", "PayType", con, tran).Trim() + "',");
                                                sb2.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','0','" + Convert.ToString(VoutReq.CompID.Trim()) + "')");
                                                cmd = new SqlCommand(sb2.ToString(), con);
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();

                                                PayTotalAmt = PayTotalAmt - BalAdjAmt;
                                            }
                                            else if (PayTotalAmt < BalAdjAmt)
                                            {
                                                //Adj DI Adv Against Inv (Receive)
                                                sb2.Remove(0, sb2.Length);
                                                sb2.Append("INSERT INTO ReceiptNoteTransaction");
                                                sb2.Append("(ONAccountOF,TrnsIndCode,TrnsCustCode, TransactionCode,TrnsPCCode,RecDINo,ReceivedCode,");
                                                sb2.Append("ReceivedDt,ReceivedAmount,Type,DueDt,Status,TransCompCode)");
                                                sb2.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "','" + CustomerCode.Trim() + "',");
                                                sb2.Append("'" + StrComInv.Trim() + "','" + MOFPCCode.Trim() + "','" + DICode.Trim() + "',");
                                                sb2.Append("'" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + PayTotalAmt + "',");
                                                sb2.Append("'" + ComCon.getTranName("select PayType from dispatchinstructiondetailssub where AdjRecNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "'", "dispatchinstructiondetailssub", "PayType", con, tran).Trim() + "',");
                                                sb2.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','0','" + Convert.ToString(VoutReq.CompID.Trim()) + "')");
                                                cmd = new SqlCommand(sb2.ToString(), con);
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();

                                                PayTotalAmt = 0;
                                            }
                                        }
                                    }
                                    ds.Dispose();
                                    ds.Clear();
                                }

                                if (strComInvCode.Trim() == "")
                                {
                                    if (StrComInv.Trim() != "")
                                    {
                                        strComInvCode = " AND Transport Commercial : " + StrComInv.Trim();
                                    }
                                }
                                else
                                {
                                    if (StrComInv.Trim() != "")
                                    {
                                        strComInvCode += ", " + StrComInv.Trim();
                                    }
                                }
                            }
                        }
                    }

                    if (DispatchFromCode.Trim() != "10" && DispatchFromCode.Trim() != "13")
                    {
                        if (VoutReq.CompID.Trim() == DispatchFromCode.Trim())
                        {
                            string Category = ComCon.getTranName("SELECT P.CategoryID FROM Part P INNER JOIN MOF M ON P.PartCode=M.PartCode WHERE M.MOFCode='" + Dts[2].Trim().Trim() + "' ", "tblC1", "CategoryID", con, tran);

                            string DICode = ComCon.getTranName("SELECT DINo from DispatchInstruction WHERE MOFCode='" + Dts[2].Trim() + "'", "tbDI", "DINo", con, tran);
                            string MOFPCCode = ComCon.getTranName("SELECT BranchCode FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblI1", "BranchCode", con, tran);
                            string IndentorCode = ComCon.getTranName("SELECT IndentorCode FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblI1", "IndentorCode", con, tran);
                            string OnAccountOf = ComCon.getTranName("SELECT OnAccountOf FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblOA1", "OnAccountOf", con, tran);
                            string CustomerCode = ComCon.getTranName("SELECT CustomerCode FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblCust1", "CustomerCode", con, tran);

                            DataSet ds = new DataSet();
                            double CInvDGInspAmt = 0;
                            double CInvDGSrvTaxAmt = 0;
                            double CInvDGInspTotalAmt = 0;
                            string DGInspection = "";
                            string CInvDGInspAmtInWord = "";
                            DGInspection = (ComCon.getName("SELECT DGinspectionBy FROM MOF WHERE MOFCode= '" + Dts[2].Trim() + "'", "MOFDG", "DGinspectionBy"));

                            //DG Inspection Amt
                            if (DGInspection.ToString().Trim() == "Y")
                            {
                                CInvDGInspAmt += Convert.ToDouble((ComCon.getName("SELECT  (DGinspection-((DGinspection)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST))) as DGinspection FROM MOF WHERE MOFCode= '" + Dts[2].Trim().Trim() + "' ", "MOFDG", "DGinspection")));

                                if (Category.Trim() == "047" || Category.Trim() == "003" || Category.Trim() == "029")/* Only For DG/Canopy/ControlPanel */
                                {
                                    if ((CInvDGInspAmt) > 0)
                                    {
                                        double CGSTPer = 0;
                                        double SGSTper = 0;
                                        double IGSTper = 0;
                                        double TaxCost = 0;
                                        DataSet ds1;

                                        string strProc = "";
                                        if (VoutReq.CompID.Trim() == "18")
                                        {
                                            strProc = "set nocount on; ";
                                            strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,DGinspection as TUCost, " +
                                            "0  as TaxationAmount " +
                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                        }
                                        else
                                        {
                                            strProc = "set nocount on; ";
                                            strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,DGinspection as TUCost, " +
                                            "((DGinspection)*CGSt)/100 + ((DGinspection)*SGSt)/100  + ((DGinspection)*IGSt)/100  as TaxationAmount " +
                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
                                        }
                                        ds1 = ComCon.procDS(strProc, "TUDetails");
                                        if (ds1.Tables["TUDetails"].Rows.Count > 0)
                                        {
                                            CGSTPer = Convert.ToDouble(ds1.Tables["TUDetails"].Rows[0]["CGST"].ToString());
                                            SGSTper = Convert.ToDouble(ds1.Tables["TUDetails"].Rows[0]["SGST"].ToString());
                                            IGSTper = Convert.ToDouble(ds1.Tables["TUDetails"].Rows[0]["IGST"].ToString());
                                            TaxCost = Convert.ToDouble(ds1.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
                                        }
                                        ds1.Dispose();
                                        ds1.Clear();
                                        CInvDGInspTotalAmt = CInvDGInspAmt + TaxCost;
                                        CInvDGInspAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round(CInvDGInspTotalAmt)) + " Only");

                                        //MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
                                        //StrComInv = "";
                                        //StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

                                        //Changes on DT 04-04-2021
                                        StrComInv = "";
                                        StrComInv = ComCon.GetMaxNo("InvoiceSales", "INV", VoutReq.CompID.Trim(), con, tran);


                                        StrTempStr1 = "";
                                        StrTempStr3 = "";
                                        StrTempStr1 = Dts[3].Trim();
                                        StrTempStr3 = "CIVNo:" + StrComInv.Trim() + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

                                        //sb.Remove(0, sb.Length);
                                        //sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
                                        //sb.Append(" VALUES('" + StrComInv.Trim().Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
                                        //sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
                                        //sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvDGInspAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
                                        //cmd = new SqlCommand(sb.ToString(), con);
                                        //cmd.Transaction = tran;
                                        //cmd.ExecuteNonQuery();
                                        //cmd.Dispose();

                                        //Changes on DT 04-04-2021
                                        sb.Remove(0, sb.Length);
                                        sb.Append("INSERT INTO InvoiceSales ");
                                        sb.Append("(InvId, MaxSrNo, Dt, Yr, ");
                                        sb.Append("InvCode,MECode, SOFCode, DICode, CustomerCode, IndentorCode,Qty, ");
                                        sb.Append("PONo,PODate, ChapterNo, TMCode, ActOff,TransportName,TMobileNo,VehicleNo,NatureOfRemoval, ");
                                        sb.Append("IssueDate, IssueTime, RemovalDate, RemovalTime,  ");
                                        sb.Append(" CSTPer, Frieght,Other, PrintCount,CompanyCode, TCSPer, ExciseDutyInWords, CESSInwords, HEDCESSInwords, ");
                                        sb.Append("AmountInWords, Remark, INVType, BasicInWords, OnAcParty, DescriptionManual, ");
                                        sb.Append("NetWeight, GrossWeight, PortOfDischarge, DeliveryTerms, TransportRoute,PortOfLoading, VesselFlightNo, TallyNarration, StockTransferStatus, ");
                                        sb.Append("CGSTPer,SGSTPer,IGSTPer,CGSTInWords,SGSTInWords,IGSTInWords, MatAmt,LabAmt, InvDesc,InvUOM)");
                                        sb.Append(" VALUES ('" + StrComInv.Trim() + "','" + (StrComInv.Substring(10, 8)) + "','" + DateTime.Now + "','" + strYearEnd.Trim() + "' ,");
                                        sb.Append("'" + Convert.ToInt32((StrComInv.Substring(12, 6)).ToString().Trim()) + "', '" + Dts[0].Trim() + "','0','0','" + CustomerCode.Trim() + "', '" + IndentorCode.Trim() + "', '1', ");
                                        sb.Append(" '', '" + DBNull.Value + "', '0','01','-','" + VoutReq.TransporterName.Trim() + "','NA','" + VoutReq.VehicleNo.Trim() + "','-', ");
                                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                                        sb.Append(" '0', '0','0','0','" + VoutReq.CompID.Trim() + "' ,'0','','','',");
                                        sb.Append(" '" + CInvDGInspAmtInWord + "','Auto Invoice For Transport','CM','Zero Only','NIL','NIL',");
                                        sb.Append("'0','0','NIL','NIL','NIL','NIL','NIL','" + StrTempStr3.ToString().Trim() + "','0',");
                                        sb.Append(" '" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','Zero Only','Zero Only','Zero Only','0','0','NA','NA')");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        //int srno = 0;
                                        //srno += 1;
                                        //sb.Remove(0, sb.Length);
                                        //sb.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount)");
                                        //sb.Append(" VALUES('" + StrComInv.Trim().Trim() + "','" + srno + "','D','" + CInvDGInspAmt + "')");
                                        //cmd = new SqlCommand(sb.ToString(), con);
                                        //cmd.Transaction = tran;
                                        //cmd.ExecuteNonQuery();
                                        //cmd.Dispose();

                                        //Changes on DT 04-04-2021
                                        int srno = 0;
                                        srno += 1;
                                        sb.Remove(0, sb.Length);
                                        sb.Append("Insert into InvoicesalesDetails (InvID,SrNo,PartCode,DPUOM,Qty,Rate,DCType,DCGSTPer,DSGSTPer,DIGSTPer,DHSNCode)");
                                        sb.Append(" VALUES ('" + StrComInv.Trim() + "','" + srno + "','0','Nos','1','" + CInvDGInspAmt + "','D','0','0','0','')");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        sb.Remove(0, sb.Length);
                                        sb.Append("INSERT INTO TaxTransactionDetails(PartyCode,ReceivedCode,ReceivedDt,TaxPer,ReceivedAmount,TaxType,CompCode,IOType)");
                                        sb.Append(" VALUES('" + IndentorCode.Trim() + "','" + StrComInv.Trim().Trim() + "',");
                                        sb.Append("'" + DateTime.Now + "',");
                                        sb.Append("'14','" + Convert.ToDouble(CInvDGSrvTaxAmt) + "',");
                                        sb.Append("'06','" + VoutReq.CompID.Trim() + "','O')");
                                        cmd = new SqlCommand(sb.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        //****************Start ReceiptNoteTransaction****************
                                        //Issue Bill
                                        sb2.Remove(0, sb2.Length);
                                        sb2.Append("INSERT INTO ReceiptNoteTransaction");
                                        sb2.Append("(ONAccountOF, TrnsIndCode, TrnsCustCode, TransactionCode, TrnsPCCode, IssueCode, IssueDt, IssueAmount, Type, DueDt, Status ,TransCompCode)");
                                        sb2.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "','" + CustomerCode.Trim() + "', ");
                                        sb2.Append("'" + StrComInv.Trim().Trim() + "','" + MOFPCCode.Trim() + "','" + Dts[2].Trim() + "',");
                                        sb2.Append("'" + DateTime.Now + "','" + Math.Round(CInvDGInspTotalAmt) + "', ");
                                        sb2.Append("'IV','" + DateTime.Now + "','0','" + VoutReq.CompID.Trim() + "')");
                                        cmd = new SqlCommand(sb2.ToString(), con);
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                        cmd.Dispose();

                                        // Received Against DI (if select any cheque)
                                        ds = ComCon.procTranDS("select ReceiptNo,sum(IssAmt) as IssAmt,sum(RecAmt) as RecAmt,(sum(IssAmt)-sum(RecAmt)) as BalAmt " +
                                                " from (select IssueCode as ReceiptNo,sum(IssueAmount) as IssAmt,0.00 as RecAmt from receiptnotetransaction " +
                                                " where recdino='" + DICode.Trim() + "' and IssueCode like 'RCN%' group by IssueCode " +
                                                " union all " +
                                                " select ReceivedCode as ReceiptNo,0.00 as IssAmt,sum(ReceivedAmount) as RecAmt from receiptnotetransaction " +
                                                " where recdino='" + DICode.Trim() + "' and ReceivedCode like 'RCN%' group by ReceivedCode )as T " +
                                                " group by ReceiptNo having (sum(IssAmt)-sum(RecAmt))>0", "ReceivedData", con, tran);
                                        if (ds != null && ds.Tables["ReceivedData"].Rows.Count > 0)
                                        {
                                            double PayTotalAmt = Convert.ToDouble(CInvDGInspTotalAmt);
                                            for (int j = 0; j < ds.Tables["ReceivedData"].Rows.Count; j++)
                                            {
                                                if (PayTotalAmt == 0)
                                                {
                                                    break;
                                                }
                                                double BalAdjAmt = Convert.ToDouble(ComCon.getTranName("select (sum(IssAmt)-sum(RecAmt)) as BalAmt from " +
                                                        " (select IssueCode as ReceiptNo,sum(IssueAmount) as IssAmt,0.00 as RecAmt from receiptnotetransaction " +
                                                        " where recdino='" + DICode.Trim() + "' and IssueCode like 'RCN%' " +
                                                        " group by IssueCode union all select ReceivedCode as ReceiptNo,0.00 as IssAmt,sum(ReceivedAmount) as RecAmt from receiptnotetransaction " +
                                                        " where recdino='" + DICode.Trim() + "' and ReceivedCode like 'RCN%' " +
                                                        " group by ReceivedCode )as T where ReceiptNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "' group by ReceiptNo " +
                                                        " having (sum(IssAmt)-sum(RecAmt))>0", "DIReceiptTransaction", "BalAmt", con, tran));
                                                if (PayTotalAmt >= BalAdjAmt)
                                                {
                                                    //Adj DI Adv Against Inv (Receive)
                                                    sb2.Remove(0, sb2.Length);
                                                    sb2.Append("INSERT INTO ReceiptNoteTransaction");
                                                    sb2.Append("(ONAccountOF, TrnsIndCode, TrnsCustCode, TransactionCode, TrnsPCCode, RecDINo, ReceivedCode,");
                                                    sb2.Append("ReceivedDt, ReceivedAmount, Type, DueDt, Status, TransCompCode)");
                                                    sb2.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "','" + CustomerCode.Trim() + "',");
                                                    sb2.Append("'" + StrComInv.Trim().Trim() + "','" + MOFPCCode.Trim() + "','" + DICode.Trim() + "',");
                                                    sb2.Append("'" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "','" + DateTime.Now + "','" + BalAdjAmt + "',");
                                                    sb2.Append("'" + ComCon.getTranName("select PayType from dispatchinstructiondetailssub where AdjRecNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "'", "dispatchinstructiondetailssub", "PayType", con, tran).Trim() + "',");
                                                    sb2.Append("'" + DateTime.Now + "','0','" + VoutReq.CompID.Trim() + "')");
                                                    cmd = new SqlCommand(sb2.ToString(), con);
                                                    cmd.Transaction = tran;
                                                    cmd.ExecuteNonQuery();
                                                    cmd.Dispose();

                                                    PayTotalAmt = PayTotalAmt - BalAdjAmt;
                                                }
                                                else if (PayTotalAmt < BalAdjAmt)
                                                {
                                                    //Adj DI Adv Against Inv (Receive)
                                                    sb2.Remove(0, sb2.Length);
                                                    sb2.Append("INSERT INTO ReceiptNoteTransaction");
                                                    sb2.Append("(ONAccountOF, TrnsIndCode, TrnsCustCode, TransactionCode, TrnsPCCode, RecDINo, ReceivedCode,");
                                                    sb2.Append("ReceivedDt, ReceivedAmount, Type, DueDt, Status, TransCompCode)");
                                                    sb2.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "','" + CustomerCode.Trim() + "',");
                                                    sb2.Append("'" + StrComInv.Trim().Trim() + "','" + MOFPCCode.Trim() + "','" + DICode.Trim() + "',");
                                                    sb2.Append("'" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "','" + DateTime.Now + "','" + PayTotalAmt + "',");
                                                    sb2.Append("'" + ComCon.getTranName("select PayType from dispatchinstructiondetailssub where AdjRecNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "'", "dispatchinstructiondetailssub", "PayType", con, tran).Trim() + "',");
                                                    sb2.Append("'" + DateTime.Now + "','0','" + VoutReq.CompID.Trim() + "')");
                                                    cmd = new SqlCommand(sb2.ToString(), con);
                                                    cmd.Transaction = tran;
                                                    cmd.ExecuteNonQuery();
                                                    cmd.Dispose();

                                                    PayTotalAmt = 0;
                                                }
                                            }
                                        }
                                        ds.Dispose();
                                        ds.Clear();
                                        //****************End**************** 
                                    }

                                    if (strComInvCodeInsp.Trim() == "")
                                    {
                                        if (StrComInv.Trim().Trim() != "")
                                        {
                                            strComInvCodeInsp = " AND Inspection Commercial : " + StrComInv.Trim().Trim();
                                        }
                                    }
                                    else
                                    {
                                        if (StrComInv.Trim().Trim() != "")
                                        {
                                            strComInvCodeInsp += ", " + StrComInv.Trim().Trim();
                                        }
                                    }
                                }
                            }
                        }
                    }


                    #endregion
                    /*End Generate CommercialInvoice */
                }
                tran.Commit();
                return strVehicleNo.Trim();
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



//using System;
//using System.Data.SqlClient;
//using System.Configuration;
//using System.Data;
//using System.Text;
//using System.IO;
//using System.Text.RegularExpressions;
//using System.Web.Http;
//using VOutRequest = KalaERPApi.Models.Transport.Trans.VehicleOutRequest;

//namespace KalaERPApi.Service.Transport.Trans
//{
//    public class VehicleOutCon
//    {
//        #region
//        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
//        StringBuilder sb = new StringBuilder();
//        StringBuilder sb2 = new StringBuilder();
//        StringBuilder sb1 = new StringBuilder();
//        StringBuilder sb3 = new StringBuilder();
//        StringBuilder sb4 = new StringBuilder();
//        SqlCommand cmdSave = null;      
//        double CInvUnloadAmt = 0;
//        double CInvTransAmt = 0;
//        double CInvTransUnloadAmt = 0;
//        string MaxCI = "0";
//        string strYearEnd = "";      
//        public SqlTransaction tran = null;
//        CommonCon ComCon = new CommonCon();
//        #endregion

//        public object SqlDbTypeChar { get; private set; }

//        public DataTable GetDGScanDts(string strComp, string strInvNo, string strDGNo)
//        {
//            SqlDataAdapter dAd = new SqlDataAdapter("VehileLoading_SerialNo_sp", con);
//            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
//            dAd.SelectCommand.Parameters.Add("@CompCode", SqlDbType.Char).Value = strComp;
//            dAd.SelectCommand.Parameters.Add("@InvNo", SqlDbType.Char).Value = strInvNo;
//            dAd.SelectCommand.Parameters.Add("@SerialNo", SqlDbType.Char).Value = strDGNo;

//            dAd.SelectCommand.CommandTimeout = 0;
//            DataSet dSet = new DataSet();
//            dAd.Fill(dSet);
//            return dSet.Tables[0];
//        }

//        public DataTable GetVehicleNo(string CompCode)
//        {
//            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleOutNo", con);
//            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
//            dAd.SelectCommand.Parameters.Add("@CompID", SqlDbType.Char).Value = CompCode;
//            dAd.SelectCommand.CommandTimeout = 0;
//            DataSet dSet = new DataSet();
//            dAd.Fill(dSet);
//            return dSet.Tables[0];
//        }

//        public DataTable GetNextDestination(string CompCode)
//        {
//            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleNextDestination", con);
//            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
//            dAd.SelectCommand.Parameters.Add("@CompID", SqlDbType.Char).Value = CompCode;
//            dAd.SelectCommand.CommandTimeout = 0;
//            DataSet dSet = new DataSet();
//            dAd.Fill(dSet);
//            return dSet.Tables[0];
//        }

//        public DataTable GetVehicleTransporterNameComp()
//        {
//            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleTransportNameComp", con);
//            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
//            dAd.SelectCommand.CommandTimeout = 0;
//            DataSet dSet = new DataSet();
//            dAd.Fill(dSet);
//            return dSet.Tables[0];
//        }

//        public DataTable GetVehicleTransporterName()
//        {
//            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleTransportNameCompApi", con);
//            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
//            dAd.SelectCommand.CommandTimeout = 0;
//            DataSet dSet = new DataSet();
//            dAd.Fill(dSet);
//            return dSet.Tables[0];
//        }

//        public DataTable GetVehicleDtls(string VNo)
//        {
//            SqlDataAdapter dAd = new SqlDataAdapter("getVehicleOutDetails", con);
//            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
//            dAd.SelectCommand.Parameters.Add("@VNo", SqlDbType.Char).Value = VNo;
//            dAd.SelectCommand.CommandTimeout = 0;
//            DataSet dSet = new DataSet();
//            dAd.Fill(dSet);
//            return dSet.Tables[0];
//        }

//        public string getMaxNoCIV(string tablename, string fieldname, string Yr, string CompID, SqlConnection con, SqlTransaction tran)
//        {
//            string strmax = "0";
//            int intmax = 0;
//            SqlCommand cmd = new SqlCommand("select isnull(SUBSTRING(max(" + fieldname.Trim() + "),3,6),0) as MX from " + tablename.Trim() + " where yr='" + Yr.Trim() + "' and CompanyCode='" + CompID.Trim() + "' and CCode like 'CIV%'", con);

//            cmd.Transaction = tran;
//            cmd.CommandTimeout = 0;
//            intmax = Convert.ToInt32(cmd.ExecuteScalar());
//            if (intmax == 0)
//                strmax = "000001";
//            else if (intmax < 9)
//                strmax = "00000" + (intmax + 1);
//            else if (intmax < 99)
//                strmax = "0000" + (intmax + 1);
//            else if (intmax < 999)
//                strmax = "000" + (intmax + 1);
//            else if (intmax < 9999)
//                strmax = "00" + (intmax + 1);
//            else if (intmax < 99999)
//                strmax = "0" + (intmax + 1);
//            else
//                strmax = Convert.ToString(intmax + 1);
//            cmd.Dispose();

//            return strmax;
//        }

//        public string Submit([FromBody] VOutRequest VoutReq)
//        {
//            string PrcNo = "", strVType = "", VhNo = "", StrExpNo = "";
//            string strVehicleNo = "", strLRNo = "", TempExpAmt = "", TempAmtToPay = "";
//            string SupOutStatus = "P";
//            string CINStatus = "P", EmptyStatus = "0";
//            double TravelKm = 0;
//            double Tansport4Exp = 0.0;
//            byte[] imageBytes = null;
//            MemoryStream ms = null;

//            try
//            {
//                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
//                tran = con.BeginTransaction();
//                SqlCommand cmd = new SqlCommand();
//                string file_name = null;
//                int recCount = ComCon.CountChars(VoutReq.VoutDts, ",");
//                string[] strVoutDts = Regex.Split(VoutReq.VoutDts, ",");

//                strVType = VoutReq.VType.Trim();
//                int SrNo = 0;
//                strYearEnd = ComCon.yearEnd(con, tran);
//                if (strVType == "R")
//                {
//                    string CurrentMnth = ComCon.getName("select Mon=case MONTH(GETDATE()) " +
//                     "when 1 then '01' when 2 then '02' when 3 then '03' " +
//                     "when 4 then '04' when 5 then '05' when 6 then '06' " +
//                     "when 7 then '07' when 8 then '08' when 9 then '09' " +
//                     "when 10 then '10' when 11 then '11' when  12 then '12' end", "tblM", "Mon");

//                    strLRNo = DateTime.Now.Year.ToString().Substring(2, 2) + CurrentMnth.Trim() + DateTime.Now.ToString("dd") + DateTime.Now.Hour + DateTime.Now.Minute;
//                }
//                else if (strVType == "NR")
//                {
//                    strLRNo = VoutReq.LRNo.ToString().Trim();

//                }

//                if (VoutReq.TType.ToString().Trim() == "E")
//                {
//                    if (VoutReq.NextDestination.ToString().Trim() == "")
//                    {
//                        if (string.IsNullOrEmpty(VoutReq.FinalDestination.ToString().Trim()))
//                        {
//                            if (strVType == "R")
//                            {
//                                SupOutStatus = "S";
//                                CINStatus = "C";
//                            }
//                        }
//                    }
//                }

//                if (!string.IsNullOrEmpty(VoutReq.LRNo.ToString().Trim()))
//                {
//                    int CntDuplicate = int.Parse(ComCon.getName("SELECT COUNT(VoutNo) AS CntLR FROM VehicleOut WHERE Active='1' AND LRNo='" + VoutReq.LRNo.ToString().Trim() + "' AND Transporter='" + VoutReq.TransporterName.ToString().Trim() + "'", "tblDup", "CntLR"));

//                    if (CntDuplicate > 0)
//                    {
//                        strVehicleNo = "LRNo Already Exists for Transporter : " + VoutReq.TransporterName.ToString().Trim() + " !";
//                        return strVehicleNo;
//                    }
//                }

//                for (int cSub = 0; cSub <= recCount; cSub++)
//                {
//                    string[] Dts = Regex.Split(strVoutDts[cSub].ToString().Trim(), "-->");
//                    if (VoutReq.ExpAmt.Trim() != "")
//                    {
//                        TempExpAmt += double.Parse(VoutReq.ExpAmt.Trim());
//                    }
//                    if (Dts[7].Trim() != "")
//                    // AmtToPay
//                    {
//                        TempAmtToPay += double.Parse(Dts[7].Trim());
//                    }
//                    if (TempExpAmt != TempAmtToPay)
//                    {
//                        // VhNo = "Divided amount should not match";
//                        //return VhNo;
//                    }
//                    TravelKm = Convert.ToDouble(VoutReq.VoutReading.Trim()) - Convert.ToDouble(VoutReq.VPrevReading.Trim());
//                }

//                strVehicleNo = ComCon.GetMaxNo("VehicleOut", "VON", VoutReq.CompID.Trim(), con, tran);
//                StrExpNo = ComCon.GetMaxNo("ExpenceRequisitionWithPlan", "ERW", VoutReq.CompID.Trim(), con, tran);

//                cmd = new SqlCommand("InsertVehicleOut", con);
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.Parameters.AddWithValue("@VOutNo", strVehicleNo.Trim());
//                cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
//                cmd.Parameters.AddWithValue("@Yr", strYearEnd.Trim());
//                cmd.Parameters.AddWithValue("@MaxSrNo", (strVehicleNo.Substring(10, 8)));              
//                cmd.Parameters.AddWithValue("@PCName", VoutReq.PCCode.Trim());
//                if (VoutReq.TType.ToString().Trim() == "E")
//                {
//                    cmd.Parameters.AddWithValue("@NextDestination", VoutReq.FinalDestination);
//                }
//                else
//                {
//                    cmd.Parameters.AddWithValue("@NextDestination", VoutReq.NextDestination);
//                }
//                cmd.Parameters.AddWithValue("@FinalDestination", VoutReq.FinalDestination);
//                cmd.Parameters.AddWithValue("@VehicleNo", VoutReq.VehicleNo);
//                cmd.Parameters.AddWithValue("@Transporter", VoutReq.TransporterName);
//                cmd.Parameters.AddWithValue("@DriverName", VoutReq.DriverName);
//                cmd.Parameters.AddWithValue("@DriverMobileNo", VoutReq.DriverMobileNo);
//                cmd.Parameters.AddWithValue("@PrvReading", Convert.ToDouble(VoutReq.VPrevReading));
//                cmd.Parameters.AddWithValue("@OutReading", Convert.ToDouble(VoutReq.VoutReading));
//                cmd.Parameters.AddWithValue("@TransportCharge", 0);
//                cmd.Parameters.AddWithValue("@LoadUnLoadCharge", 0);
//                cmd.Parameters.AddWithValue("@Diesel", Convert.ToDouble(VoutReq.Diesel));
//                cmd.Parameters.AddWithValue("@LRNo", strLRNo.Trim());
//                if (VoutReq.TType.ToString().Trim() == "E")
//                {
//                    cmd.Parameters.AddWithValue("@LRDt", VoutReq.LRDate);
//                }
//                else
//                {
//                    cmd.Parameters.AddWithValue("@LRDt", DateTime.Now.ToString("yyyy-MM-dd"));
//                }
//                cmd.Parameters.AddWithValue("@Remark", VoutReq.Remark);
//                cmd.Parameters.AddWithValue("@AuthRemark", "OK");
//                cmd.Parameters.AddWithValue("@SecurityRemark", "NIL");
//                cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
//                cmd.Parameters.AddWithValue("@TType", VoutReq.TType.Trim());
//                cmd.Parameters.AddWithValue("@GateOutStatus", "P");
//                cmd.Parameters.AddWithValue("@SupOutStatus", SupOutStatus.Trim());
//                cmd.Parameters.AddWithValue("@EmptyStatus", EmptyStatus.Trim());
//                cmd.Parameters.AddWithValue("@CINStatus", CINStatus.Trim());
//                cmd.Parameters.AddWithValue("@BreakDownStatus", 0);
//                cmd.Parameters.AddWithValue("@VType", strVType.Trim());
//                cmd.Parameters.AddWithValue("@Active", 1);
//                cmd.Parameters.AddWithValue("@Auth", 1);
//                cmd.Parameters.AddWithValue("@Status", 0);
//                cmd.Transaction = tran;
//                cmd.ExecuteNonQuery();
//                cmd.Dispose();

//                cmd = new SqlCommand("InsertVehicleTransaction", con);
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.Parameters.AddWithValue("@VehicleNo", VoutReq.VehicleNo);
//                cmd.Parameters.AddWithValue("@Reading", Convert.ToDouble(VoutReq.VoutReading));
//                cmd.Parameters.AddWithValue("@OutTransactionNo", strVehicleNo.Trim());
//                cmd.Parameters.AddWithValue("@OutDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
//                cmd.Parameters.AddWithValue("@TransportCharge", 0);
//                cmd.Parameters.AddWithValue("@Diesel", Convert.ToDouble(VoutReq.Diesel));
//                cmd.Parameters.AddWithValue("@InTransactionNo", 0);
//                cmd.Parameters.AddWithValue("@InDate", DBNull.Value);
//                cmd.Parameters.AddWithValue("@TravelKM", TravelKm);
//                cmd.Parameters.AddWithValue("@BreakDownStatus", 0);
//                cmd.Transaction = tran;
//                cmd.ExecuteNonQuery();
//                cmd.Dispose();

//                DataSet dsDiesel = ComCon.procDS("select ID,VehicleNo,convert(varchar(10),Dt,103)+' '+right(convert(varchar,Dt,100),8) as sysDt " +
//                " ,convert(varchar(10),DieselFillDt,103)+' '+right(convert(varchar,DieselFillDt,100),8) as FillDt " +
//                " ,DieselQty,DieselAmt,DieselQty,Remark from DieselUpdation where VehicleNo='" + VoutReq.VehicleNo.Trim() + "' " +
//                 " and InOutUsedStatus='P' order by ID", "DieselUpdation");
//                if (dsDiesel.Tables["DieselUpdation"].Rows.Count > 0)
//                {
//                    for (int i = 0; i < dsDiesel.Tables["DieselUpdation"].Rows.Count; i++)
//                    {
//                        sb.Remove(0, sb.Length);
//                        sb.Append("Insert into DieselUpdationDetails(SrNo,DieselUpdationID,VInOrVOutNo)");
//                        sb.Append(" VALUES('" + (i + 1) + "','" + dsDiesel.Tables["DieselUpdation"].Rows[i]["ID"].ToString().Trim() + "',");
//                        sb.Append("'" + strVehicleNo.Trim() + "')");
//                        cmd = new SqlCommand(sb.ToString(), con);
//                        cmd.Transaction = tran;
//                        cmd.ExecuteNonQuery();
//                        cmd.Dispose();

//                        sb.Remove(0, sb.Length);
//                        sb.Append("UPDATE DieselUpdation SET InOutUsedStatus='C' WHERE VehicleNo='" + VoutReq.VehicleNo.Trim() + "' ");
//                        sb.Append("AND Active='1' AND InOutUsedStatus='P' and ID='" + dsDiesel.Tables["DieselUpdation"].Rows[i]["ID"].ToString().Trim() + "'");
//                        cmd = new SqlCommand(sb.ToString(), con);
//                        cmd.Transaction = tran;
//                        cmd.ExecuteNonQuery();
//                        cmd.Dispose();
//                    }
//                }
//                dsDiesel.Dispose();
//                dsDiesel.Clear();

//                int cntSrNo = 0;
//                string strAmt = "0";
//                Tansport4Exp = 0;

//                for (int cSub = 0; cSub <= recCount; cSub++)
//                {
//                    SrNo += 1;
//                    string[] Dts = Regex.Split(strVoutDts[cSub].ToString().Trim(), "-->");                                       
//                    if (Dts[0].Trim() == "")
//                    {
//                        strAmt = "0";
//                    }
//                    else
//                    {
//                        strAmt = Dts[6].Trim();
//                    }

//                    string strExpectedDOD = "";
//                    if (!string.IsNullOrEmpty(Dts[10].Trim()))
//                    {                       
//                        strExpectedDOD = Dts[10].Trim().Substring(0, 10);
//                    }
//                    else
//                    {
//                        strExpectedDOD = Convert.ToString(DBNull.Value);
//                    }

//                    Tansport4Exp = Tansport4Exp + Convert.ToDouble(Dts[9].Trim());
//                    cntSrNo = cntSrNo + 1;
//                    cmd = new SqlCommand("InsertVehicleOutDetailsApi", con);
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@VOutNo", strVehicleNo.Trim());
//                    cmd.Parameters.AddWithValue("@SrNo", cntSrNo);
//                    cmd.Parameters.AddWithValue("@InvoiceID", Dts[0].Trim());
//                    cmd.Parameters.AddWithValue("@INVCode", Dts[3].Trim());                  
//                    cmd.Parameters.AddWithValue("@ExpectedDOD", strExpectedDOD.Trim());
//                    cmd.Parameters.AddWithValue("@INVDt", Dts[1].Trim().Substring(0, 10));
//                    cmd.Parameters.AddWithValue("@PartDescT", "0");
//                    cmd.Parameters.AddWithValue("@PartCodeT", "0");
//                    cmd.Parameters.AddWithValue("@QtyT", 0);
//                    cmd.Parameters.AddWithValue("@Amt", Convert.ToDouble(strAmt.Trim()));
//                    cmd.Parameters.AddWithValue("@SanctionAmtT", 0);
//                    cmd.Parameters.AddWithValue("@TentativeKm", Dts[5].Trim());                 
//                    cmd.Parameters.AddWithValue("@InvoiceType", Dts[4].Trim());
//                    cmd.Parameters.AddWithValue("@MeMOFCode", Dts[2].Trim());
//                    cmd.Parameters.AddWithValue("@Status", 0);
//                    cmd.Transaction = tran;
//                    cmd.ExecuteNonQuery();
//                    cmd.Dispose();

//                    sb.Remove(0, sb.Length);
//                    sb.Append("UPDATE InvoiceSales SET VIOStatus='O' WHERE INVID='" + Dts[0].Trim() + "' ");
//                    sb.Append("AND VIOStatus='I' AND Active='1' AND Auth='1' AND CompanyCode='" + VoutReq.CompID.Trim() + "'");
//                    cmd = new SqlCommand(sb.ToString(), con);
//                    cmd.Transaction = tran;
//                    cmd.ExecuteNonQuery();
//                    cmd.Dispose();

//                    sb.Remove(0, sb.Length);
//                    sb.Append("UPDATE Challan45 SET VIOStatus='O' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
//                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
//                    cmd = new SqlCommand(sb.ToString(), con);
//                    cmd.Transaction = tran;
//                    cmd.ExecuteNonQuery();
//                    cmd.Dispose();

//                    sb.Remove(0, sb.Length);
//                    sb.Append("UPDATE ChallanOutReturnable SET VIOStatus='O' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
//                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
//                    cmd = new SqlCommand(sb.ToString(), con);
//                    cmd.Transaction = tran;
//                    cmd.ExecuteNonQuery();
//                    cmd.Dispose();

//                    sb.Remove(0, sb.Length);
//                    sb.Append("UPDATE InvoiceExport SET VIOStatus='O' WHERE INVID='" + Dts[0].Trim() + "' ");
//                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
//                    cmd = new SqlCommand(sb.ToString(), con);
//                    cmd.Transaction = tran;
//                    cmd.ExecuteNonQuery();
//                    cmd.Dispose();

//                    sb.Remove(0, sb.Length);
//                    sb.Append("UPDATE InvoiceCommercial SET VIOStatus='O' WHERE CCode='" + Dts[0].Trim() + "' ");
//                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
//                    cmd = new SqlCommand(sb.ToString(), con);
//                    cmd.Transaction = tran;
//                    cmd.ExecuteNonQuery();
//                    cmd.Dispose();

//                    sb.Remove(0, sb.Length);
//                    sb.Append("UPDATE InvoiceDealer SET VIOStatus='O' WHERE INVID='" + Dts[0].Trim() + "' ");
//                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
//                    cmd = new SqlCommand(sb.ToString(), con);
//                    cmd.Transaction = tran;
//                    cmd.ExecuteNonQuery();
//                    cmd.Dispose();

//                    sb.Remove(0, sb.Length);
//                    sb.Append("UPDATE InvoiceDealer SET VIOStatus='O' WHERE INVID='" + Dts[0].Trim() + "' ");
//                    sb.Append(" and VIOStatus='I' and Active='1' and Auth='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
//                    cmd = new SqlCommand(sb.ToString(), con);
//                    cmd.Transaction = tran;
//                    cmd.ExecuteNonQuery();
//                    cmd.Dispose();

//                    sb.Remove(0, sb.Length);
//                    sb.Append("UPDATE deliverychallan SET VehIOStatus='O' WHERE DCCode='" + Dts[0].Trim() + "' ");
//                    sb.Append(" and Active='1' and CompanyCode='" + VoutReq.CompID.Trim() + "'");
//                    cmd = new SqlCommand(sb.ToString(), con);
//                    cmd.Transaction = tran;
//                    cmd.ExecuteNonQuery();
//                    cmd.Dispose();

//                    if (VoutReq.CompID.Trim() == "01")
//                    {
//                        if (Dts[0].Trim().Substring(10, 2) == "14" || Dts[0].Trim().Substring(10, 2) == "10" || Dts[0].Trim().Substring(10, 2) == "13")
//                        {
//                            sb.Remove(0, sb.Length);
//                            sb.Append("UPDATE InvoiceSales SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
//                            sb.Append(" AND VIOStatus='O' AND Active='1' AND Auth='1' ");
//                            cmd = new SqlCommand(sb.ToString(), con);
//                            cmd.Transaction = tran;
//                            cmd.ExecuteNonQuery();
//                            cmd.Dispose();

//                            sb.Remove(0, sb.Length);
//                            sb.Append("UPDATE Challan45 SET VIOStatus='OO' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
//                            sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
//                            cmd = new SqlCommand(sb.ToString(), con);
//                            cmd.Transaction = tran;
//                            cmd.ExecuteNonQuery();
//                            cmd.Dispose();

//                            sb.Remove(0, sb.Length);
//                            sb.Append("UPDATE ChallanOutReturnable SET VIOStatus='OO' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
//                            sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
//                            cmd = new SqlCommand(sb.ToString(), con);
//                            cmd.Transaction = tran;
//                            cmd.ExecuteNonQuery();
//                            cmd.Dispose();

//                            sb.Remove(0, sb.Length);
//                            sb.Append("UPDATE InvoiceExport SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
//                            sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
//                            cmd = new SqlCommand(sb.ToString(), con);
//                            cmd.Transaction = tran;
//                            cmd.ExecuteNonQuery();
//                            cmd.Dispose();

//                            sb.Remove(0, sb.Length);
//                            sb.Append("UPDATE InvoiceCommercial SET VIOStatus='OO' WHERE CCode='" + Dts[0].Trim() + "' ");
//                            sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
//                            cmd = new SqlCommand(sb.ToString(), con);
//                            cmd.Transaction = tran;
//                            cmd.ExecuteNonQuery();
//                            cmd.Dispose();

//                            sb.Remove(0, sb.Length);
//                            sb.Append("UPDATE InvoiceDealer SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
//                            sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
//                            cmd = new SqlCommand(sb.ToString(), con);
//                            cmd.Transaction = tran;
//                            cmd.ExecuteNonQuery();
//                            cmd.Dispose();
//                        }
//                    }
//                    else if (VoutReq.CompID.Trim() != "01" || VoutReq.CompID.Trim() != "02" || VoutReq.CompID.Trim() != "03")
//                    {
//                        if (VoutReq.TType.Trim() == "E")
//                        {
//                            if (VoutReq.NextDestination.Trim() == "")
//                            {
//                                if (Dts[0].Trim().Substring(10, 2) == "14" || Dts[0].Trim().Substring(10, 2) == "10" || Dts[0].Trim().Substring(10, 2) == "13")
//                                {
//                                    sb.Remove(0, sb.Length);
//                                    sb.Append("UPDATE InvoiceSales SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
//                                    sb.Append(" AND VIOStatus='O' AND Active='1' AND Auth='1' ");
//                                    cmd = new SqlCommand(sb.ToString(), con);
//                                    cmd.Transaction = tran;
//                                    cmd.ExecuteNonQuery();
//                                    cmd.Dispose();

//                                    sb.Remove(0, sb.Length);
//                                    sb.Append("UPDATE Challan45 SET VIOStatus='OO' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
//                                    sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
//                                    cmd = new SqlCommand(sb.ToString(), con);
//                                    cmd.Transaction = tran;
//                                    cmd.ExecuteNonQuery();
//                                    cmd.Dispose();

//                                    sb.Remove(0, sb.Length);
//                                    sb.Append("UPDATE ChallanOutReturnable SET VIOStatus='OO' WHERE ChallanCode='" + Dts[0].Trim() + "' ");
//                                    sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
//                                    cmd = new SqlCommand(sb.ToString(), con);
//                                    cmd.Transaction = tran;
//                                    cmd.ExecuteNonQuery();
//                                    cmd.Dispose();

//                                    sb.Remove(0, sb.Length);
//                                    sb.Append("UPDATE InvoiceExport SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
//                                    sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
//                                    cmd = new SqlCommand(sb.ToString(), con);
//                                    cmd.Transaction = tran;
//                                    cmd.ExecuteNonQuery();
//                                    cmd.Dispose();

//                                    sb.Remove(0, sb.Length);
//                                    sb.Append("UPDATE InvoiceCommercial SET VIOStatus='OO' WHERE CCode='" + Dts[0].Trim() + "' ");
//                                    sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
//                                    cmd = new SqlCommand(sb.ToString(), con);
//                                    cmd.Transaction = tran;
//                                    cmd.ExecuteNonQuery();
//                                    cmd.Dispose();

//                                    sb.Remove(0, sb.Length);
//                                    sb.Append("UPDATE InvoiceDealer SET VIOStatus='OO' WHERE INVID='" + Dts[0].Trim() + "' ");
//                                    sb.Append(" and VIOStatus='O' and Active='1' and Auth='1' ");
//                                    cmd = new SqlCommand(sb.ToString(), con);
//                                    cmd.Transaction = tran;
//                                    cmd.ExecuteNonQuery();
//                                    cmd.Dispose();
//                                }
//                            }
//                        }
//                    }
//                }

//                // AND IOStatus='I'
//                sb.Remove(0, sb.Length);
//                sb.Append("UPDATE Vehicle SET IOStatus='O',IOCompany='" + VoutReq.CompID.Trim() + "'");
//                sb.Append(" WHERE VNo='" + strVehicleNo.Trim() + "' AND Active='1' AND BreakDownStatus='0'");
//                cmd = new SqlCommand(sb.ToString(), con);
//                cmd.Transaction = tran;
//                cmd.ExecuteNonQuery();
//                cmd.Dispose();

//                if (!string.IsNullOrEmpty(VoutReq.image_string))
//                {
//                    string[] image_string_items = Regex.Split(VoutReq.image_string, ",");
//                    if (image_string_items.Length > 0)
//                    {
//                        System.Drawing.Image image = null;
//                        int srno = 0;
//                        for (int j = 0; j < image_string_items.Length; j++)
//                        {
//                            imageBytes = Convert.FromBase64String(image_string_items[j].Trim());
//                            if (imageBytes.Length > 0)
//                            {
//                                ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
//                                ms.Write(imageBytes, 0, imageBytes.Length);
//                                image = System.Drawing.Image.FromStream(ms, true);

//                                file_name = strVehicleNo.ToString().Trim().Substring(4, 5).Trim() + strVehicleNo.ToString().Trim().Substring(10, 8).Trim() + "-" + (j + 1) + ".jpg";
//                                var filePath = ComCon.getMainFilePath("VehicleOut") + "\\" + file_name;
//                                if (!File.Exists(filePath))
//                                {
//                                    image.Save(filePath);

//                                    srno += 1;
//                                    sb.Remove(0, sb.Length);
//                                    sb.Append("INSERT INTO VehicleOutFileDetails");
//                                    sb.Append("(VOutNo, SrNo, FileAttachment, Active, Status)");
//                                    sb.Append(" VALUES('" + strVehicleNo.Trim() + "' ,'" + srno + "','" + file_name.Trim() + "','1','0')");
//                                    cmd = new SqlCommand(sb.ToString(), con);
//                                    cmd.Transaction = tran;
//                                    cmd.ExecuteNonQuery();
//                                    cmd.Dispose();
//                                }
//                            }
//                        }
//                    }
//                }

//                if (VoutReq.TType.Trim() == "E")
//                {
//                    if (VoutReq.NextDestination.Trim() == "")
//                    {
//                        if (!string.IsNullOrEmpty(VoutReq.NextDestination.Trim()))
//                        {
//                            if (strVType == "R")
//                            {
//                                sb.Remove(0, sb.Length);
//                                sb.Append("UPDATE VehicleOut SET CINStatus='C' WHERE VehicleNo='" + strVehicleNo.Trim() + "' ");
//                                sb.Append(" and Active='1' and CINStatus='P' ");
//                                cmd = new SqlCommand(sb.ToString(), con);
//                                cmd.Transaction = tran;
//                                cmd.ExecuteNonQuery();
//                                cmd.Dispose();
//                            }
//                        }
//                    }
//                }

//                /* ExpenseRequisition */
//                #region
//                double Expencetotal = 0.0;

//                if (Convert.ToDouble(VoutReq.ExpAmt.Trim()) > 0)
//                {
//                    Expencetotal = Convert.ToDouble(VoutReq.ExpAmt.Trim());
//                    //Tansport4Exp = Tansport4Exp + Convert.ToDouble(dataItem["Transport"].ToString().Trim());
//                    SrNo += 1;

//                    cmd = new SqlCommand("InsertExpenseRequisition", con);
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
//                    cmd.Parameters.AddWithValue("@MaxSrNo", (StrExpNo.Substring(10, 8)));
//                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
//                    cmd.Parameters.AddWithValue("@Yr", strYearEnd.Trim());
//                    cmd.Parameters.AddWithValue("@PCCode", VoutReq.PCCode.Trim());
//                    if (VoutReq.TransporterName.Trim() == "0249")
//                    {
//                        cmd.Parameters.AddWithValue("@ExpType", "E");
//                    }
//                    else if (VoutReq.TransporterName.Trim() == "130079")
//                    {
//                        cmd.Parameters.AddWithValue("@ExpType", "E");
//                    }
//                    else
//                    {
//                        cmd.Parameters.AddWithValue("@ExpType", "S");
//                    }
//                    cmd.Parameters.AddWithValue("@SupEmpCode", VoutReq.TransporterName.Trim());
//                    cmd.Parameters.AddWithValue("@EmpSupCode", "0");
//                    cmd.Parameters.AddWithValue("@BalAmount", 0);
//                    cmd.Parameters.AddWithValue("@Advance", "0");
//                    cmd.Parameters.AddWithValue("@PathA", "NA");
//                    cmd.Parameters.AddWithValue("@Remark", "");
//                    cmd.Parameters.AddWithValue("@CompanyCode", "03");
//                    cmd.Parameters.AddWithValue("@SRVNo", 0);
//                    cmd.Parameters.AddWithValue("@ACTNo", strVehicleNo.Trim());
//                    cmd.Parameters.AddWithValue("@EngTransAmt", 0);
//                    if ((Expencetotal <= Tansport4Exp - (Tansport4Exp * 15 / 100)) && (Tansport4Exp > 0))
//                    {
//                        cmd.Parameters.AddWithValue("@Auth", 1);
//                        cmd.Parameters.AddWithValue("@AuthRemark", "Auto Authorize by ERP");
//                    }
//                    else
//                    {
//                        cmd.Parameters.AddWithValue("@Auth", 0);
//                        cmd.Parameters.AddWithValue("@AuthRemark", "NIL");
//                    }
//                    cmd.Transaction = tran;
//                    cmd.ExecuteNonQuery();
//                    cmd.Dispose();

//                    //****************User Acivity****************
//                    //cmd = new SqlCommand("InsertLoginTransactionDetails", con);
//                    //cmd.CommandType = CommandType.StoredProcedure;
//                    //cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
//                    //cmd.Parameters.AddWithValue("@EmpID", ((string)Session["UserID"]));
//                    //cmd.Parameters.AddWithValue("@TransactionType", "S");
//                    //cmd.Parameters.AddWithValue("@TransactionFrom", "ExpenseRequisitionAuto");
//                    //cmd.Parameters.AddWithValue("@TransactionNo", ExpReqCode.Trim());
//                    //cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim() );
//                    //cmd.Transaction = tran;
//                    //cmd.ExecuteNonQuery();
//                    //cmd.Dispose();

//                    if ((Expencetotal <= Tansport4Exp - (Tansport4Exp * 15 / 100)) && (Tansport4Exp > 0))
//                    {
//                        if (VoutReq.TransporterName.Trim() == "0249" || VoutReq.TransporterName.Trim() == "130079")
//                        {
//                            sb.Remove(0, sb.Length);
//                            sb.Append("insert into EmpFundTransaction(EmpCode,DocType,TransCode,IssueCode,IssueDt,IssAmount,ReceivedCode,ReceivedDt,RecAmount) ");
//                            sb.Append("Values('" + VoutReq.TransporterName.Trim() + "','ERW','" + StrExpNo.Trim() + "','0',NULL,'0.00',");
//                            sb.Append("'" + StrExpNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Expencetotal + "')");
//                            cmd = new SqlCommand(sb.ToString(), con);
//                            cmd.Transaction = tran;
//                            cmd.ExecuteNonQuery();
//                            cmd.Dispose();
//                        }
//                        sb.Remove(0, sb.Length);
//                        sb.Append("INSERT INTO AuthorizationDetails");
//                        sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
//                        sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,'0101','ExpenseRequisition Authorize','" + StrExpNo.Trim() + "','03')");
//                        cmd = new SqlCommand(sb.ToString(), con);
//                        cmd.Transaction = tran;
//                        cmd.ExecuteNonQuery();
//                        cmd.Dispose();
//                    }


//                    if (Convert.ToDouble(VoutReq.ExpAmtLoadUnload.Trim()) > 0)
//                    {

//                        cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
//                        cmd.Parameters.AddWithValue("@SrNo", "1");
//                        // string[] EXPCode = Regex.Split(dataItem["EXPName"].ToString().Trim(), "-->");
//                        cmd.Parameters.AddWithValue("@EXPCode", "0133");
//                        cmd.Parameters.AddWithValue("@Qty", "1");
//                        cmd.Parameters.AddWithValue("@Rate", Convert.ToDouble(VoutReq.ExpAmtLoadUnload.Trim()));
//                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(VoutReq.ExpAmtLoadUnload.Trim()));
//                        cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpBillNo", 0);
//                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
//                        cmd.Transaction = tran;
//                        cmd.ExecuteNonQuery();
//                        cmd.Dispose();
//                        //}
//                        //}
//                    }

//                    if (Convert.ToDouble(VoutReq.ExpAmtToll.Trim()) > 0)
//                    {

//                        cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
//                        cmd.Parameters.AddWithValue("@SrNo", "1");
//                        // string[] EXPCode = Regex.Split(dataItem["EXPName"].ToString().Trim(), "-->");
//                        cmd.Parameters.AddWithValue("@EXPCode", "0130");
//                        cmd.Parameters.AddWithValue("@Qty", "1");
//                        cmd.Parameters.AddWithValue("@Rate", Convert.ToDouble(VoutReq.ExpAmtToll.Trim()));
//                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(VoutReq.ExpAmtToll.Trim()));
//                        cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpBillNo", 0);
//                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
//                        cmd.Transaction = tran;
//                        cmd.ExecuteNonQuery();
//                        cmd.Dispose();
//                        //}
//                        //}
//                    }

//                    if (Convert.ToDouble(VoutReq.ExpAmtStaffwel.Trim()) > 0)
//                    {

//                        cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
//                        cmd.Parameters.AddWithValue("@SrNo", "1");
//                        // string[] EXPCode = Regex.Split(dataItem["EXPName"].ToString().Trim(), "-->");
//                        cmd.Parameters.AddWithValue("@EXPCode", "0132");
//                        cmd.Parameters.AddWithValue("@Qty", "1");
//                        cmd.Parameters.AddWithValue("@Rate", Convert.ToDouble(VoutReq.ExpAmtStaffwel.Trim()));
//                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(VoutReq.ExpAmtStaffwel.Trim()));
//                        cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpBillNo", 0);
//                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
//                        cmd.Transaction = tran;
//                        cmd.ExecuteNonQuery();
//                        cmd.Dispose();
//                        //}
//                        //}
//                    }

//                    if (Convert.ToDouble(VoutReq.ExpAmtVehExp.Trim()) > 0)
//                    {

//                        cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
//                        cmd.Parameters.AddWithValue("@SrNo", "1");
//                        // string[] EXPCode = Regex.Split(dataItem["EXPName"].ToString().Trim(), "-->");
//                        cmd.Parameters.AddWithValue("@EXPCode", "0131");
//                        cmd.Parameters.AddWithValue("@Qty", "1");
//                        cmd.Parameters.AddWithValue("@Rate", Convert.ToDouble(VoutReq.ExpAmtVehExp.Trim()));
//                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(VoutReq.ExpAmtVehExp.Trim()));
//                        cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpBillNo", 0);
//                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
//                        cmd.Transaction = tran;
//                        cmd.ExecuteNonQuery();
//                        cmd.Dispose();
//                        //}
//                        //}
//                    }

//                    if (Convert.ToDouble(VoutReq.ExpAmtCarriageOut.Trim()) > 0)
//                    {

//                        cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.AddWithValue("@REQCode", StrExpNo.Trim());
//                        cmd.Parameters.AddWithValue("@SrNo", "1");
//                        // string[] EXPCode = Regex.Split(dataItem["EXPName"].ToString().Trim(), "-->");
//                        cmd.Parameters.AddWithValue("@EXPCode", "0028");
//                        cmd.Parameters.AddWithValue("@Qty", "1");
//                        cmd.Parameters.AddWithValue("@Rate", Convert.ToDouble(VoutReq.ExpAmtCarriageOut.Trim()));
//                        cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(VoutReq.ExpAmtCarriageOut.Trim()));
//                        cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
//                        cmd.Parameters.AddWithValue("@ExpBillNo", 0);
//                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
//                        cmd.Transaction = tran;
//                        cmd.ExecuteNonQuery();
//                        cmd.Dispose();
//                        //}
//                        //}
//                    }

//                    //        }
//                }

//                #endregion
//                string StrComInv = string.Empty;
//                string strComInvCode = string.Empty;
//                string strComInvCodeInsp = string.Empty;
//                string StrTempStr1 = string.Empty;
//                string StrTempStr3 = string.Empty;

//                /*Start Generate CommercialInvoice for DGInspection */
//                #region

//                for (int cSub = 0; cSub <= recCount; cSub++)
//                {                   
//                    SrNo += 1;
//                    string[] Dts = Regex.Split(strVoutDts[cSub].ToString().Trim(), "-->");
//                    string DispatchFromCode = ComCon.getTranName("SELECT DispatchFromCode FROM MOF WHERE MOFCode='" + Dts[2].Trim().Trim() + "'", "tblMof1", "DispatchFromCode", con, tran);
//                    string Saletransit = ComCon.getTranName("SELECT Saletransit FROM MOF WHERE MOFCode='" + Dts[2].Trim().Trim() + "'", "tblMof1", "Saletransit", con, tran);
//                    CInvTransAmt = 0;
//                    CInvUnloadAmt = 0;

//                    if (Saletransit.Trim() == "False")
//                    {
//                        if (VoutReq.CompID.Trim() == DispatchFromCode.Trim() || VoutReq.CompID.Trim() == "18")
//                        {
//                            string Category = ComCon.getTranName("SELECT P.CategoryID FROM Part P INNER JOIN MOF M ON P.PartCode=M.PartCode WHERE M.MOFCode='" + Dts[2].Trim() + "' ", "tblC1", "CategoryID", con, tran);

//                            string TransportBy = ComCon.getTranName("SELECT TransportBy FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' AND Active='1' ", "tblt1", "TransportBy", con, tran);
//                            string UnloadingBy = ComCon.getTranName("SELECT UnloadingBy FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' AND Active='1' ", "tblt2", "UnloadingBy", con, tran);

//                            string DICode = ComCon.getTranName("select DINo from DispatchInstruction where MOFCode='" + Dts[2].Trim() + "'", "tbDI", "DINo", con, tran);

//                            string MOFPCCode = ComCon.getTranName("SELECT BranchCode FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblI1", "BranchCode", con, tran);
//                            string IndentorCode = ComCon.getTranName("SELECT IndentorCode FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblI1", "IndentorCode", con, tran);
//                            string OnAccountOf = ComCon.getTranName("SELECT OnAccountOf FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblOA1", "OnAccountOf", con, tran);
//                            string CustomerCode = ComCon.getTranName("SELECT ConsigneeCode as CustomerCode FROM DispatchInstruction WHERE DINo='" + DICode.Trim() + "'", "tblCust1", "CustomerCode", con, tran);

//                            if (VoutReq.VType.Trim() == "R")
//                            {
//                                if (VoutReq.CompID.Trim() == "18")
//                                {
//                                    if (TransportBy.Trim() == "E")
//                                    {
//                                        CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport,0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM1", "Transport", con, tran));
//                                    }
//                                    if (UnloadingBy.Trim() == "E")
//                                    {
//                                        CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading,0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Unloading", con, tran));
//                                    }
//                                }
//                                else
//                                {
//                                    if (TransportBy.Trim() == "E")
//                                    {
//                                        CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport-((Transport)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM1", "Transport", con, tran));
//                                    }
//                                    if (UnloadingBy.Trim() == "E")
//                                    {
//                                        CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading-((Unloading)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Unloading", con, tran));
//                                    }
//                                }

//                                /* if DispatchFrom Dealer-->02/10/13 Company and (TransportBy='I' & UnloadingBy='I' in MOF for only Register Vehicle) */
//                                if (DispatchFromCode.Trim() == "02" || DispatchFromCode.Trim() == "10" || DispatchFromCode.Trim() == "13")
//                                {
//                                    if (VoutReq.CompID.Trim() == "18")
//                                    {
//                                        if (TransportBy.Trim() == "I")
//                                        {
//                                            CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport,0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM3", "Transport", con, tran));
//                                        }
//                                        if (UnloadingBy.Trim() == "I")
//                                        {
//                                            CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading,0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM4", "Unloading", con, tran));
//                                        }
//                                    }
//                                    else
//                                    {
//                                        if (TransportBy.Trim() == "I")
//                                        {
//                                            CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport-((Transport)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM3", "Transport", con, tran));
//                                        }
//                                        if (UnloadingBy.Trim() == "I")
//                                        {
//                                            CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading-((Unloading)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM4", "Unloading", con, tran));
//                                        }
//                                    }
//                                }

//                                if (TransportBy.Trim() == "C")
//                                {
//                                    if (Dts[7].Trim() != "")
//                                    {
//                                        if (Dts[6].Trim() != "0")
//                                        {
//                                            //CInvTransAmt = Convert.ToDouble(dataItem["BillAmt"].ToString().Trim());
//                                            //Calculate Transport Basic
//                                            if (VoutReq.CompID.Trim() == "18")
//                                            {
//                                                CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round('" + Convert.ToDouble(Dts[7].Trim()) + "'-(('" + Convert.ToDouble(Dts[7].Trim()) + "')/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Transport", con, tran));
//                                            }
//                                            else
//                                            {
//                                                CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round('" + Convert.ToDouble(Dts[7].Trim()) + "'-(('" + Convert.ToDouble(Dts[7].Trim()) + "')/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Transport", con, tran));
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                            else if (VoutReq.VType.Trim() == "NR")
//                            {
//                                if (TransportBy.Trim() == "E")
//                                {
//                                    CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round('" + Convert.ToDouble(Dts[7].Trim()) + "'-(('" + Convert.ToDouble(Dts[7].Trim()) + "')/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Transport", con, tran));
//                                    if (VoutReq.CompID.Trim() == "18")
//                                    {
//                                        CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport,0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM1", "Transport", con, tran));
//                                    }
//                                    else
//                                    {
//                                        CInvTransAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Transport-((Transport)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Transport FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM1", "Transport", con, tran));
//                                    }
//                                }
//                                if (UnloadingBy.Trim() == "E")
//                                {
//                                    if (VoutReq.CompID.Trim() == "18")
//                                    {
//                                        CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading,0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Unloading", con, tran));
//                                    }
//                                    else
//                                    {
//                                        CInvUnloadAmt = Convert.ToDouble(ComCon.getTranName("SELECT round(Unloading-((Unloading)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST)),0) as Unloading FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "' ", "tblM2", "Unloading", con, tran));
//                                    }
//                                }
//                            }

//                            StrComInv = "";
//                            if (Category.Trim() == "047" || Category.Trim() == "003" || Category.Trim() == "029")/* Only For DG/Canopy/ControlPanel */
//                            {
//                                if (VoutReq.VType.Trim() == "R")
//                                {
//                                    if ((CInvTransAmt + CInvUnloadAmt) > 0)
//                                    {
//                                        if (CInvTransAmt > 0 && TransportBy.Trim() == "C") // For TransportBy Customer 
//                                        {
//                                            double CGSTPer = 0;
//                                            double SGSTper = 0;
//                                            double IGSTper = 0;
//                                            double TaxCost = 0;
//                                            CInvTransUnloadAmt = 0;
//                                            DataSet ds;

//                                            string strProc = "";
//                                            if (VoutReq.CompID.Trim() == "18")
//                                            {
//                                                if (TransportBy.Trim() == "C")
//                                                {
//                                                    strProc = "set nocount on; ";
//                                                    strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,'" + CInvTransAmt + "' as Transport,0 as Unloading,'" + CInvTransAmt + "' as TUCost, " +
//                                                    " 0  as TaxationAmount " +
//                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                                }
//                                                else
//                                                {
//                                                    strProc = "set nocount on; ";
//                                                    strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                    " 0 as TaxationAmount " +
//                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                                }
//                                            }
//                                            else
//                                            {
//                                                if (TransportBy.Trim() == "C")
//                                                {
//                                                    strProc = "set nocount on; ";
//                                                    strProc += "select Mofcode,CGST,SGST,IGST,'" + CInvTransAmt + "' as Transport,0 as Unloading,'" + CInvTransAmt + "' as TUCost, " +
//                                                    "(('" + CInvTransAmt + "')*CGSt)/100 + (('" + CInvTransAmt + "')*SGSt)/100  + (('" + CInvTransAmt + "')*IGSt)/100  as TaxationAmount " +
//                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                                }
//                                                else
//                                                {
//                                                    strProc = "set nocount on; ";
//                                                    strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                    "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
//                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                                }
//                                            }

//                                            ds = ComCon.procDS(strProc, "TUDetails");
//                                            if (ds.Tables["TUDetails"].Rows.Count > 0)
//                                            {
//                                                CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
//                                                SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
//                                                IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
//                                                TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
//                                                CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());
//                                            }
//                                            ds.Dispose();
//                                            ds.Clear();

//                                            string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");

//                                            MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
//                                            StrComInv = "";
//                                            StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

//                                            StrTempStr1 = "";
//                                            StrTempStr3 = "";
//                                            StrTempStr1 = Dts[8].Trim();// For Partdesc
//                                            StrTempStr3 = "CIVNo:" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

//                                            sb.Remove(0, sb.Length);
//                                            sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
//                                            sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
//                                            sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
//                                            sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
//                                            cmd = new SqlCommand(sb.ToString(), con);
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();

//                                            sb1.Remove(0, sb1.Length);
//                                            sb1.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
//                                            sb1.Append(" VALUES('" + StrComInv.Trim() + "','1','T','" + CInvTransAmt + "','996519')");
//                                            cmd = new SqlCommand(sb1.ToString(), con);
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();

//                                            //****************User Acivity****************
//                                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
//                                            cmd.CommandType = CommandType.StoredProcedure;
//                                            cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
//                                            cmd.Parameters.AddWithValue("@EmpID", (""));
//                                            cmd.Parameters.AddWithValue("@TransactionType", "S");
//                                            cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
//                                            cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
//                                            cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();
//                                        }
//                                        else if (CInvTransAmt > 0 && TransportBy.Trim() == "E")  // For TransportBy Exluded
//                                        {
//                                            double CGSTPer = 0;
//                                            double SGSTper = 0;
//                                            double IGSTper = 0;
//                                            double TaxCost = 0;
//                                            CInvTransUnloadAmt = 0;
//                                            DataSet ds;

//                                            string strProc = "";
//                                            if (VoutReq.CompID.Trim() == "18")
//                                            {
//                                                strProc = "set nocount on; ";
//                                                strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                " 0  as TaxationAmount " +
//                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                            }
//                                            else
//                                            {
//                                                strProc = "set nocount on; ";
//                                                strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
//                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                            }
//                                            ds = ComCon.procDS(strProc, "TUDetails");

//                                            if (ds.Tables["TUDetails"].Rows.Count > 0)
//                                            {
//                                                CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
//                                                SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
//                                                IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
//                                                TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
//                                                CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

//                                            }
//                                            ds.Dispose();
//                                            ds.Clear();

//                                            string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");

//                                            MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
//                                            StrComInv = "";
//                                            StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

//                                            StrTempStr1 = "";
//                                            StrTempStr3 = "";
//                                            StrTempStr1 = Dts[8].Trim();//partdesc
//                                            StrTempStr3 = "CIVNo:" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;


//                                            sb.Remove(0, sb.Length);
//                                            sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
//                                            sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
//                                            sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
//                                            sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
//                                            cmd = new SqlCommand(sb.ToString(), con);
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();

//                                            sb1.Remove(0, sb1.Length);
//                                            sb1.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
//                                            sb1.Append(" VALUES('" + StrComInv.Trim() + "','1','T','" + CInvTransAmt + "','996519')");
//                                            cmd = new SqlCommand(sb1.ToString(), con);
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();

//                                            //****************User Acivity****************
//                                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
//                                            cmd.CommandType = CommandType.StoredProcedure;
//                                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
//                                            cmd.Parameters.AddWithValue("@EmpID", (""));
//                                            cmd.Parameters.AddWithValue("@TransactionType", "S");
//                                            cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
//                                            cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
//                                            cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();
//                                        }
//                                        else if (CInvTransAmt > 0 && TransportBy.Trim() == "I")  // For TransportBy Included
//                                        {

//                                            double CGSTPer = 0;
//                                            double SGSTper = 0;
//                                            double IGSTper = 0;
//                                            double TaxCost = 0;
//                                            CInvTransUnloadAmt = 0;
//                                            DataSet ds;
//                                            string strProc = "";

//                                            if (VoutReq.CompID.Trim() == "18")
//                                            {
//                                                strProc = "set nocount on; ";
//                                                strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                " 0  as TaxationAmount " +
//                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                            }
//                                            else
//                                            {
//                                                strProc = "set nocount on; ";
//                                                strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
//                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                            }
//                                            ds = ComCon.procDS(strProc, "TUDetails");

//                                            if (ds.Tables["TUDetails"].Rows.Count > 0)
//                                            {
//                                                CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
//                                                SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
//                                                IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
//                                                TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
//                                                CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

//                                            }
//                                            ds.Dispose();
//                                            ds.Clear();

//                                            string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");
//                                            MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
//                                            StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

//                                            StrTempStr1 = "";
//                                            StrTempStr3 = "";
//                                            StrTempStr1 = Dts[8].Trim();
//                                            StrTempStr3 = "CIVNo:" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

//                                            sb.Remove(0, sb.Length);
//                                            sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
//                                            sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
//                                            sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
//                                            sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
//                                            cmd = new SqlCommand(sb.ToString(), con);
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();

//                                            sb1.Remove(0, sb1.Length);
//                                            sb1.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
//                                            sb1.Append(" VALUES('" + StrComInv.Trim() + "','1','T','" + CInvTransAmt + "','996519')");
//                                            cmd = new SqlCommand(sb1.ToString(), con);
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();

//                                            //****************User Acivity****************
//                                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
//                                            cmd.CommandType = CommandType.StoredProcedure;
//                                            cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
//                                            cmd.Parameters.AddWithValue("@EmpID", (""));
//                                            cmd.Parameters.AddWithValue("@TransactionType", "S");
//                                            cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
//                                            cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
//                                            cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();
//                                        }
//                                        if (CInvUnloadAmt > 0) // MOF Unloding Amount
//                                        {
//                                            int CntInvoice = Convert.ToInt32(ComCon.getTranName("SELECT COUNT(CCode)AS CntInvoice FROM InvoiceCommercial WHERE MemoCode='" + Dts[0].Trim() + "' AND Active='1'", "tblC1", "CntInvoice", con, tran));
//                                            if (CntInvoice == 0)
//                                            {
//                                                double CGSTPer = 0;
//                                                double SGSTper = 0;
//                                                double IGSTper = 0;
//                                                double TaxCost = 0;
//                                                CInvTransUnloadAmt = 0;
//                                                DataSet ds;

//                                                string strProc = "";
//                                                if (VoutReq.CompID.Trim() == "18")
//                                                {
//                                                    strProc = "set nocount on; ";
//                                                    strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                    "0  as TaxationAmount " +
//                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                                }
//                                                else
//                                                {
//                                                    strProc = "set nocount on; ";
//                                                    strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                    "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
//                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                                }

//                                                ds = ComCon.procDS(strProc, "TUDetails");

//                                                if (ds.Tables["TUDetails"].Rows.Count > 0)
//                                                {
//                                                    CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
//                                                    SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
//                                                    IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
//                                                    TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
//                                                    CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

//                                                }
//                                                ds.Dispose();
//                                                ds.Clear();

//                                                string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");
//                                                MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
//                                                StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

//                                                StrTempStr1 = "";
//                                                StrTempStr3 = "";
//                                                StrTempStr1 = Dts[8].Trim();
//                                                StrTempStr3 = "CIVNo:" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

//                                                sb.Remove(0, sb.Length);
//                                                sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
//                                                sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
//                                                sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
//                                                sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");

//                                                cmd = new SqlCommand(sb.ToString(), con);
//                                                cmd.Transaction = tran;
//                                                cmd.ExecuteNonQuery();
//                                                cmd.Dispose();

//                                                //****************User Acivity****************
//                                                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
//                                                cmd.CommandType = CommandType.StoredProcedure;
//                                                cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
//                                                cmd.Parameters.AddWithValue("@EmpID", (""));
//                                                cmd.Parameters.AddWithValue("@TransactionType", "S");
//                                                cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
//                                                cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
//                                                cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
//                                                cmd.Transaction = tran;
//                                                cmd.ExecuteNonQuery();
//                                                cmd.Dispose();
//                                            }

//                                            sb2.Remove(0, sb2.Length);
//                                            sb2.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
//                                            sb2.Append(" VALUES('" + StrComInv.Trim() + "','2','U','" + CInvUnloadAmt + "','999799')");
//                                            cmdSave = new SqlCommand(sb2.ToString(), con);
//                                            cmdSave.Transaction = tran;
//                                            cmdSave.ExecuteNonQuery();
//                                            cmdSave.Dispose();
//                                        }
//                                    }
//                                }
//                                else if (VoutReq.VType.Trim() == "NR")
//                                {
//                                    if ((CInvTransAmt + CInvUnloadAmt) > 0)
//                                    {
//                                        if (CInvTransAmt > 0)
//                                        {

//                                            double CGSTPer = 0;
//                                            double SGSTper = 0;
//                                            double IGSTper = 0;
//                                            double TaxCost = 0;
//                                            CInvTransUnloadAmt = 0;
//                                            DataSet ds;

//                                            string strProc = "";
//                                            if (VoutReq.CompID.Trim() == "18")
//                                            {
//                                                strProc = "set nocount on; ";
//                                                strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                "0  as TaxationAmount " +
//                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                            }
//                                            else
//                                            {
//                                                strProc = "set nocount on; ";
//                                                strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
//                                                "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                            }
//                                            ds = ComCon.procDS(strProc, "TUDetails");
//                                            if (ds.Tables["TUDetails"].Rows.Count > 0)
//                                            {
//                                                CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
//                                                SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
//                                                IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
//                                                TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
//                                                CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

//                                            }
//                                            ds.Dispose();
//                                            ds.Clear();

//                                            string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");
//                                            MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
//                                            StrComInv = string.Empty;
//                                            StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

//                                            StrTempStr1 = "";
//                                            StrTempStr3 = "";
//                                            StrTempStr1 = Dts[8].Trim();
//                                            StrTempStr3 = "CIVNo :" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

//                                            sb.Remove(0, sb.Length);
//                                            sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
//                                            sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
//                                            sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
//                                            sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
//                                            cmd = new SqlCommand(sb.ToString(), con);
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();

//                                            sb3.Remove(0, sb3.Length);
//                                            sb3.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
//                                            sb3.Append(" VALUES('" + StrComInv.Trim() + "','1','T','" + CInvTransAmt + "','996519')");
//                                            cmd = new SqlCommand(sb3.ToString(), con);
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();

//                                            //****************User Acivity****************
//                                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
//                                            cmd.CommandType = CommandType.StoredProcedure;
//                                            cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
//                                            cmd.Parameters.AddWithValue("@EmpID", (""));
//                                            cmd.Parameters.AddWithValue("@TransactionType", "S");
//                                            cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
//                                            cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
//                                            cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
//                                            cmd.Transaction = tran;
//                                            cmd.ExecuteNonQuery();
//                                            cmd.Dispose();
//                                        }
//                                        if (CInvUnloadAmt > 0)
//                                        {
//                                            int CntInvoice = Convert.ToInt32(ComCon.getTranName("SELECT COUNT(CCode)AS CntInvoice FROM InvoiceCommercial WHERE MemoCode='" + Dts[0].Trim() + "' AND Active='1'", "tblC1", "CntInvoice", con, tran));
//                                            if (CntInvoice == 0)
//                                            {
//                                                double CGSTPer = 0;
//                                                double SGSTper = 0;
//                                                double IGSTper = 0;
//                                                double TaxCost = 0;
//                                                CInvTransUnloadAmt = 0;
//                                                DataSet ds;

//                                                string strProc = "";
//                                                if (VoutReq.CompID.Trim() == "18")
//                                                {
//                                                    strProc = "set nocount on; ";
//                                                    strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                    "0  as TaxationAmount " +
//                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                                }
//                                                else
//                                                {
//                                                    strProc = "set nocount on; ";
//                                                    strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                                    "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
//                                                    "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                                }
//                                                ds = ComCon.procDS(strProc, "TUDetails");
//                                                if (ds.Tables["TUDetails"].Rows.Count > 0)
//                                                {
//                                                    CGSTPer = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["CGST"].ToString());
//                                                    SGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["SGST"].ToString());
//                                                    IGSTper = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["IGST"].ToString());
//                                                    TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
//                                                    CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

//                                                }
//                                                ds.Dispose();
//                                                ds.Clear();

//                                                string CInvAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round((CInvTransUnloadAmt))) + " Only");
//                                                MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
//                                                StrComInv = string.Empty;
//                                                StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

//                                                StrTempStr1 = "";
//                                                StrTempStr3 = "";
//                                                StrTempStr1 = Dts[8].Trim();
//                                                StrTempStr3 = "CIVNo :" + StrComInv + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

//                                                sb.Remove(0, sb.Length);
//                                                sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
//                                                sb.Append(" VALUES('" + StrComInv.Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
//                                                sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
//                                                sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
//                                                cmd = new SqlCommand(sb.ToString(), con);
//                                                cmd.Transaction = tran;
//                                                cmd.ExecuteNonQuery();
//                                                cmd.Dispose();

//                                                //****************User Acivity****************
//                                                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
//                                                cmd.CommandType = CommandType.StoredProcedure;
//                                                cmd.Parameters.AddWithValue("@TransactionDtTime", System.DateTime.Now);
//                                                cmd.Parameters.AddWithValue("@EmpID", (""));
//                                                cmd.Parameters.AddWithValue("@TransactionType", "S");
//                                                cmd.Parameters.AddWithValue("@TransactionFrom", "CommercialInvoiceAuto");
//                                                cmd.Parameters.AddWithValue("@TransactionNo", StrComInv.Trim());
//                                                cmd.Parameters.AddWithValue("@CompanyCode", VoutReq.CompID.Trim());
//                                                cmd.Transaction = tran;
//                                                cmd.ExecuteNonQuery();
//                                                cmd.Dispose();
//                                            }

//                                            sb4.Remove(0, sb4.Length);
//                                            sb4.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount,HSNCode)");
//                                            sb4.Append(" VALUES('" + StrComInv.Trim() + "','2','U','" + CInvUnloadAmt + "','999799')");
//                                            cmdSave = new SqlCommand(sb4.ToString(), con);
//                                            cmdSave.Transaction = tran;
//                                            cmdSave.ExecuteNonQuery();
//                                            cmdSave.Dispose();
//                                        }
//                                    }
//                                }

//                                if ((CInvTransAmt + CInvUnloadAmt) > 0)
//                                {
//                                    // Issue Against Bill  
//                                    double TaxCost = 0;
//                                    CInvTransUnloadAmt = 0;
//                                    DataSet ds;

//                                    string strProc = "";
//                                    if (VoutReq.CompID.Trim() == "18")
//                                    {
//                                        if (TransportBy.Trim() == "C")
//                                        {
//                                            strProc = "set nocount on; ";
//                                            strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,'" + CInvTransAmt + "' as Transport,0 as Unloading,'" + CInvTransAmt + "' as TUCost, " +
//                                            "0  as TaxationAmount " +
//                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                        }
//                                        else
//                                        {
//                                            strProc = "set nocount on; ";
//                                            strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                            "0  as TaxationAmount " +
//                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                        }
//                                    }
//                                    else
//                                    {
//                                        if (TransportBy.Trim() == "C")
//                                        {
//                                            strProc = "set nocount on; ";
//                                            strProc += "select Mofcode,CGST,SGST,IGST,'" + CInvTransAmt + "' as Transport,0 as Unloading,'" + CInvTransAmt + "' as TUCost, " +
//                                            "(('" + CInvTransAmt + "')*CGSt)/100 + (('" + CInvTransAmt + "')*SGSt)/100  + (('" + CInvTransAmt + "')*IGSt)/100  as TaxationAmount " +
//                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                        }
//                                        else
//                                        {
//                                            strProc = "set nocount on; ";
//                                            strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,Transport+Unloading as TUCost, " +
//                                            "((Transport+Unloading)*CGSt)/100 + ((Transport+Unloading)*SGSt)/100  + ((Transport+Unloading)*IGSt)/100  as TaxationAmount " +
//                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                        }
//                                    }
//                                    ds = ComCon.procDS(strProc, "TUDetails");
//                                    if (ds.Tables["TUDetails"].Rows.Count > 0)
//                                    {
//                                        TaxCost = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
//                                        CInvTransUnloadAmt = Convert.ToDouble(ds.Tables["TUDetails"].Rows[0]["TUCost"].ToString());

//                                    }
//                                    ds.Dispose();
//                                    ds.Clear();

//                                    sb.Remove(0, sb.Length);
//                                    sb.Append("INSERT INTO ReceiptNoteTransaction");
//                                    sb.Append("(ONAccountOF,TrnsIndCode,TrnsCustCode,TransactionCode,TrnsPCCode,RecDINo,IssueCode,IssueDt,IssueAmount,Type,DueDt,Status,TransCompCode)");
//                                    sb.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "', ");
//                                    sb.Append(" '" + CustomerCode.Trim() + "','" + StrComInv.Trim() + "',");
//                                    sb.Append(" '" + MOFPCCode.Trim() + "','" + DICode.Trim() + "','" + Dts[2].Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");
//                                    sb.Append(" '" + Math.Round((CInvTransUnloadAmt)) + "','IV','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");
//                                    sb.Append(" '0','" + VoutReq.CompID.Trim() + "' )");
//                                    cmd = new SqlCommand(sb.ToString(), con);
//                                    cmd.Transaction = tran;
//                                    cmd.ExecuteNonQuery();
//                                    cmd.Dispose();

//                                    // Received Against DI (if select any cheque)
//                                    ds = ComCon.procTranDS("select ReceiptNo,sum(IssAmt) as IssAmt,sum(RecAmt) as RecAmt,(sum(IssAmt)-sum(RecAmt)) as BalAmt " +
//                                           " from (select IssueCode as ReceiptNo,sum(IssueAmount) as IssAmt,0.00 as RecAmt from receiptnotetransaction " +
//                                           " where recdino='" + DICode.Trim() + "' and IssueCode like 'RCN%' group by IssueCode " +
//                                           " union all " +
//                                           " select ReceivedCode as ReceiptNo,0.00 as IssAmt,sum(ReceivedAmount) as RecAmt from receiptnotetransaction " +
//                                           " where recdino='" + DICode.Trim() + "' and ReceivedCode like 'RCN%' group by ReceivedCode )as T " +
//                                           " group by ReceiptNo having (sum(IssAmt)-sum(RecAmt))>0", "ReceivedData", con, tran);
//                                    if (ds != null && ds.Tables["ReceivedData"].Rows.Count > 0)
//                                    {
//                                        double PayTotalAmt = Math.Round((CInvTransAmt + CInvUnloadAmt + TaxCost));
//                                        for (int j = 0; j < ds.Tables["ReceivedData"].Rows.Count; j++)
//                                        {
//                                            if (PayTotalAmt == 0)
//                                            {
//                                                break;
//                                            }
//                                            double BalAdjAmt = Convert.ToDouble(ComCon.getTranName("select (sum(IssAmt)-sum(RecAmt)) as BalAmt from " +
//                                                    " (select IssueCode as ReceiptNo,sum(IssueAmount) as IssAmt,0.00 as RecAmt from receiptnotetransaction " +
//                                                    " where recdino='" + DICode.Trim() + "' and IssueCode like 'RCN%' " +
//                                                    " group by IssueCode union all select ReceivedCode as ReceiptNo,0.00 as IssAmt,sum(ReceivedAmount) as RecAmt from receiptnotetransaction " +
//                                                    " where recdino='" + DICode.Trim() + "' and ReceivedCode like 'RCN%' " +
//                                                    " group by ReceivedCode )as T where ReceiptNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "' group by ReceiptNo " +
//                                                    " having (sum(IssAmt)-sum(RecAmt))>0", "DIReceiptTransaction", "BalAmt", con, tran));
//                                            if (PayTotalAmt >= BalAdjAmt)
//                                            {
//                                                //Adj DI Adv Against Inv (Receive)
//                                                sb2.Remove(0, sb2.Length);
//                                                sb2.Append("INSERT INTO ReceiptNoteTransaction");
//                                                sb2.Append("(ONAccountOF,TrnsIndCode,TrnsCustCode, TransactionCode,TrnsPCCode,RecDINo,ReceivedCode,");
//                                                sb2.Append("ReceivedDt,ReceivedAmount,Type,DueDt,Status,TransCompCode)");
//                                                sb2.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "','" + CustomerCode.Trim() + "',");
//                                                sb2.Append("'" + StrComInv.Trim() + "','" + MOFPCCode.Trim() + "','" + DICode.Trim() + "',");
//                                                sb2.Append("'" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + BalAdjAmt + "',");
//                                                sb2.Append("'" + ComCon.getTranName("select PayType from dispatchinstructiondetailssub where AdjRecNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "'", "dispatchinstructiondetailssub", "PayType", con, tran).Trim() + "',");
//                                                sb2.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','0','" + Convert.ToString(VoutReq.CompID.Trim()) + "')");
//                                                cmd = new SqlCommand(sb2.ToString(), con);
//                                                cmd.Transaction = tran;
//                                                cmd.ExecuteNonQuery();
//                                                cmd.Dispose();

//                                                PayTotalAmt = PayTotalAmt - BalAdjAmt;
//                                            }
//                                            else if (PayTotalAmt < BalAdjAmt)
//                                            {
//                                                //Adj DI Adv Against Inv (Receive)
//                                                sb2.Remove(0, sb2.Length);
//                                                sb2.Append("INSERT INTO ReceiptNoteTransaction");
//                                                sb2.Append("(ONAccountOF,TrnsIndCode,TrnsCustCode, TransactionCode,TrnsPCCode,RecDINo,ReceivedCode,");
//                                                sb2.Append("ReceivedDt,ReceivedAmount,Type,DueDt,Status,TransCompCode)");
//                                                sb2.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "','" + CustomerCode.Trim() + "',");
//                                                sb2.Append("'" + StrComInv.Trim() + "','" + MOFPCCode.Trim() + "','" + DICode.Trim() + "',");
//                                                sb2.Append("'" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + PayTotalAmt + "',");
//                                                sb2.Append("'" + ComCon.getTranName("select PayType from dispatchinstructiondetailssub where AdjRecNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "'", "dispatchinstructiondetailssub", "PayType", con, tran).Trim() + "',");
//                                                sb2.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','0','" + Convert.ToString(VoutReq.CompID.Trim()) + "')");
//                                                cmd = new SqlCommand(sb2.ToString(), con);
//                                                cmd.Transaction = tran;
//                                                cmd.ExecuteNonQuery();
//                                                cmd.Dispose();

//                                                PayTotalAmt = 0;
//                                            }
//                                        }
//                                    }
//                                    ds.Dispose();
//                                    ds.Clear();
//                                }

//                                if (strComInvCode.Trim() == "")
//                                {
//                                    if (StrComInv.Trim() != "")
//                                    {
//                                        strComInvCode = " AND Transport Commercial : " + StrComInv.Trim();
//                                    }
//                                }
//                                else
//                                {
//                                    if (StrComInv.Trim() != "")
//                                    {
//                                        strComInvCode += ", " + StrComInv.Trim();
//                                    }
//                                }
//                            }
//                        }
//                    }

//                    if (DispatchFromCode.Trim() != "10" && DispatchFromCode.Trim() != "13")
//                    {
//                        if (VoutReq.CompID.Trim() == DispatchFromCode.Trim())
//                        {
//                            string Category = ComCon.getTranName("SELECT P.CategoryID FROM Part P INNER JOIN MOF M ON P.PartCode=M.PartCode WHERE M.MOFCode='" + Dts[2].Trim().Trim() + "' ", "tblC1", "CategoryID", con, tran);

//                            string DICode = ComCon.getTranName("SELECT DINo from DispatchInstruction WHERE MOFCode='" + Dts[2].Trim() + "'", "tbDI", "DINo", con, tran);
//                            string MOFPCCode = ComCon.getTranName("SELECT BranchCode FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblI1", "BranchCode", con, tran);
//                            string IndentorCode = ComCon.getTranName("SELECT IndentorCode FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblI1", "IndentorCode", con, tran);
//                            string OnAccountOf = ComCon.getTranName("SELECT OnAccountOf FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblOA1", "OnAccountOf", con, tran);
//                            string CustomerCode = ComCon.getTranName("SELECT CustomerCode FROM MOF WHERE MOFCode='" + Dts[2].Trim() + "'", "tblCust1", "CustomerCode", con, tran);

//                            DataSet ds = new DataSet();
//                            double CInvDGInspAmt = 0;
//                            double CInvDGSrvTaxAmt = 0;
//                            double CInvDGInspTotalAmt = 0;
//                            string DGInspection = "";
//                            string CInvDGInspAmtInWord = "";
//                            DGInspection = (ComCon.getName("SELECT DGinspectionBy FROM MOF WHERE MOFCode= '" + Dts[2].Trim() + "'", "MOFDG", "DGinspectionBy"));

//                            //DG Inspection Amt
//                            if (DGInspection.ToString().Trim() == "Y")
//                            {
//                                CInvDGInspAmt += Convert.ToDouble((ComCon.getName("SELECT  (DGinspection-((DGinspection)/((100)+(CGST+SGST+IGST))*(CGST+SGST+IGST))) as DGinspection FROM MOF WHERE MOFCode= '" + Dts[2].Trim().Trim() + "' ", "MOFDG", "DGinspection")));

//                                if (Category.Trim() == "047" || Category.Trim() == "003" || Category.Trim() == "029")/* Only For DG/Canopy/ControlPanel */
//                                {
//                                    if ((CInvDGInspAmt) > 0)
//                                    {

//                                        double CGSTPer = 0;
//                                        double SGSTper = 0;
//                                        double IGSTper = 0;
//                                        double TaxCost = 0;
//                                        DataSet ds1;

//                                        string strProc = "";
//                                        if (VoutReq.CompID.Trim() == "18")
//                                        {
//                                            strProc = "set nocount on; ";
//                                            strProc += "select Mofcode,0 as CGST,0 as SGST,0 as IGST,Transport,Unloading,DGinspection as TUCost, " +
//                                            "0  as TaxationAmount " +
//                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                        }
//                                        else
//                                        {
//                                            strProc = "set nocount on; ";
//                                            strProc += "select Mofcode,CGST,SGST,IGST,Transport,Unloading,DGinspection as TUCost, " +
//                                            "((DGinspection)*CGSt)/100 + ((DGinspection)*SGSt)/100  + ((DGinspection)*IGSt)/100  as TaxationAmount " +
//                                            "from MOF where mofcode='" + Dts[2].Trim() + "'";
//                                        }
//                                        ds1 = ComCon.procDS(strProc, "TUDetails");
//                                        if (ds1.Tables["TUDetails"].Rows.Count > 0)
//                                        {
//                                            CGSTPer = Convert.ToDouble(ds1.Tables["TUDetails"].Rows[0]["CGST"].ToString());
//                                            SGSTper = Convert.ToDouble(ds1.Tables["TUDetails"].Rows[0]["SGST"].ToString());
//                                            IGSTper = Convert.ToDouble(ds1.Tables["TUDetails"].Rows[0]["IGST"].ToString());
//                                            TaxCost = Convert.ToDouble(ds1.Tables["TUDetails"].Rows[0]["TaxationAmount"].ToString());
//                                        }
//                                        ds1.Dispose();
//                                        ds1.Clear();
//                                        CInvDGInspTotalAmt = CInvDGInspAmt + TaxCost;
//                                        CInvDGInspAmtInWord = Convert.ToString(ComCon.NumberToText((int)Math.Round(CInvDGInspTotalAmt)) + " Only");

//                                        MaxCI = getMaxNoCIV("InvoiceCommercial", "MaxSrNo", strYearEnd.Trim(), VoutReq.CompID.Trim(), con, tran);
//                                        StrComInv = "";
//                                        StrComInv = Convert.ToString("CIV/" + strYearEnd.Trim() + "/" + VoutReq.CompID.Trim() + MaxCI.Trim());

//                                        StrTempStr1 = "";
//                                        StrTempStr3 = "";
//                                        StrTempStr1 = Dts[3].Trim();
//                                        StrTempStr3 = "CIVNo:" + StrComInv.Trim() + " MOFNo :" + Dts[2].Trim() + " InvNo :" + Dts[3].Trim() + " Dt:" + Dts[1].Trim().Substring(0, 10) + " " + StrTempStr1;

//                                        sb.Remove(0, sb.Length);
//                                        sb.Append("INSERT INTO InvoiceCommercial(CCode,MaxSrNo,Dt,Yr,InvCode,MemoCode,IndentorCode,VehicleNo,Transporter,VAT,SrvTax,CGSTPer,SGSTPer,IGSTPer,InvoiceTotal,Remark,TallyNarration,CompanyCode)");
//                                        sb.Append(" VALUES('" + StrComInv.Trim().Trim() + "','" + VoutReq.CompID.Trim() + MaxCI.Trim() + "','" + DateTime.Now + "',");
//                                        sb.Append("'" + strYearEnd.Trim() + "','" + Convert.ToInt32(MaxCI.ToString().Trim()) + "','" + Dts[0].Trim() + "',");
//                                        sb.Append("'" + IndentorCode.Trim() + "','" + VoutReq.VehicleNo.Trim() + "','" + VoutReq.TransporterName.Trim() + "','0','0','" + CGSTPer + "','" + SGSTper + "','" + IGSTper + "','" + CInvDGInspAmtInWord + "','Auto Invoice For Transport','" + StrTempStr3.ToString().Trim() + "','" + VoutReq.CompID.Trim() + "')");
//                                        cmd = new SqlCommand(sb.ToString(), con);
//                                        cmd.Transaction = tran;
//                                        cmd.ExecuteNonQuery();
//                                        cmd.Dispose();

//                                        int srno = 0;
//                                        srno += 1;
//                                        sb.Remove(0, sb.Length);
//                                        sb.Append("INSERT INTO InvoiceCommercialDetails(CCode,SrNo,CType,Amount)");
//                                        sb.Append(" VALUES('" + StrComInv.Trim().Trim() + "','" + srno + "','D','" + CInvDGInspAmt + "')");
//                                        cmd = new SqlCommand(sb.ToString(), con);
//                                        cmd.Transaction = tran;
//                                        cmd.ExecuteNonQuery();
//                                        cmd.Dispose();

//                                        sb.Remove(0, sb.Length);
//                                        sb.Append("INSERT INTO TaxTransactionDetails(PartyCode,ReceivedCode,ReceivedDt,TaxPer,ReceivedAmount,TaxType,CompCode,IOType)");
//                                        sb.Append(" VALUES('" + IndentorCode.Trim() + "','" + StrComInv.Trim().Trim() + "',");
//                                        sb.Append("'" + DateTime.Now + "',");
//                                        sb.Append("'14','" + Convert.ToDouble(CInvDGSrvTaxAmt) + "',");
//                                        sb.Append("'06','" + VoutReq.CompID.Trim() + "','O')");
//                                        cmd = new SqlCommand(sb.ToString(), con);
//                                        cmd.Transaction = tran;
//                                        cmd.ExecuteNonQuery();
//                                        cmd.Dispose();

//                                        //****************Start ReceiptNoteTransaction****************
//                                        //Issue Bill
//                                        sb2.Remove(0, sb2.Length);
//                                        sb2.Append("INSERT INTO ReceiptNoteTransaction");
//                                        sb2.Append("(ONAccountOF, TrnsIndCode, TrnsCustCode, TransactionCode, TrnsPCCode, IssueCode, IssueDt, IssueAmount, Type, DueDt, Status ,TransCompCode)");
//                                        sb2.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "','" + CustomerCode.Trim() + "', ");
//                                        sb2.Append("'" + StrComInv.Trim().Trim() + "','" + MOFPCCode.Trim() + "','" + Dts[2].Trim() + "',");
//                                        sb2.Append("'" + DateTime.Now + "','" + Math.Round(CInvDGInspTotalAmt) + "', ");
//                                        sb2.Append("'IV','" + DateTime.Now + "','0','" + VoutReq.CompID.Trim() + "')");
//                                        cmd = new SqlCommand(sb2.ToString(), con);
//                                        cmd.Transaction = tran;
//                                        cmd.ExecuteNonQuery();
//                                        cmd.Dispose();

//                                        // Received Against DI (if select any cheque)
//                                        ds = ComCon.procTranDS("select ReceiptNo,sum(IssAmt) as IssAmt,sum(RecAmt) as RecAmt,(sum(IssAmt)-sum(RecAmt)) as BalAmt " +
//                                                " from (select IssueCode as ReceiptNo,sum(IssueAmount) as IssAmt,0.00 as RecAmt from receiptnotetransaction " +
//                                                " where recdino='" + DICode.Trim() + "' and IssueCode like 'RCN%' group by IssueCode " +
//                                                " union all " +
//                                                " select ReceivedCode as ReceiptNo,0.00 as IssAmt,sum(ReceivedAmount) as RecAmt from receiptnotetransaction " +
//                                                " where recdino='" + DICode.Trim() + "' and ReceivedCode like 'RCN%' group by ReceivedCode )as T " +
//                                                " group by ReceiptNo having (sum(IssAmt)-sum(RecAmt))>0", "ReceivedData", con, tran);
//                                        if (ds != null && ds.Tables["ReceivedData"].Rows.Count > 0)
//                                        {
//                                            double PayTotalAmt = Convert.ToDouble(CInvDGInspTotalAmt);
//                                            for (int j = 0; j < ds.Tables["ReceivedData"].Rows.Count; j++)
//                                            {
//                                                if (PayTotalAmt == 0)
//                                                {
//                                                    break;
//                                                }
//                                                double BalAdjAmt = Convert.ToDouble(ComCon.getTranName("select (sum(IssAmt)-sum(RecAmt)) as BalAmt from " +
//                                                        " (select IssueCode as ReceiptNo,sum(IssueAmount) as IssAmt,0.00 as RecAmt from receiptnotetransaction " +
//                                                        " where recdino='" + DICode.Trim() + "' and IssueCode like 'RCN%' " +
//                                                        " group by IssueCode union all select ReceivedCode as ReceiptNo,0.00 as IssAmt,sum(ReceivedAmount) as RecAmt from receiptnotetransaction " +
//                                                        " where recdino='" + DICode.Trim() + "' and ReceivedCode like 'RCN%' " +
//                                                        " group by ReceivedCode )as T where ReceiptNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "' group by ReceiptNo " +
//                                                        " having (sum(IssAmt)-sum(RecAmt))>0", "DIReceiptTransaction", "BalAmt", con, tran));
//                                                if (PayTotalAmt >= BalAdjAmt)
//                                                {
//                                                    //Adj DI Adv Against Inv (Receive)
//                                                    sb2.Remove(0, sb2.Length);
//                                                    sb2.Append("INSERT INTO ReceiptNoteTransaction");
//                                                    sb2.Append("(ONAccountOF, TrnsIndCode, TrnsCustCode, TransactionCode, TrnsPCCode, RecDINo, ReceivedCode,");
//                                                    sb2.Append("ReceivedDt, ReceivedAmount, Type, DueDt, Status, TransCompCode)");
//                                                    sb2.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "','" + CustomerCode.Trim() + "',");
//                                                    sb2.Append("'" + StrComInv.Trim().Trim() + "','" + MOFPCCode.Trim() + "','" + DICode.Trim() + "',");
//                                                    sb2.Append("'" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "','" + DateTime.Now + "','" + BalAdjAmt + "',");
//                                                    sb2.Append("'" + ComCon.getTranName("select PayType from dispatchinstructiondetailssub where AdjRecNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "'", "dispatchinstructiondetailssub", "PayType", con, tran).Trim() + "',");
//                                                    sb2.Append("'" + DateTime.Now + "','0','" + VoutReq.CompID.Trim() + "')");
//                                                    cmd = new SqlCommand(sb2.ToString(), con);
//                                                    cmd.Transaction = tran;
//                                                    cmd.ExecuteNonQuery();
//                                                    cmd.Dispose();

//                                                    PayTotalAmt = PayTotalAmt - BalAdjAmt;
//                                                }
//                                                else if (PayTotalAmt < BalAdjAmt)
//                                                {
//                                                    //Adj DI Adv Against Inv (Receive)
//                                                    sb2.Remove(0, sb2.Length);
//                                                    sb2.Append("INSERT INTO ReceiptNoteTransaction");
//                                                    sb2.Append("(ONAccountOF, TrnsIndCode, TrnsCustCode, TransactionCode, TrnsPCCode, RecDINo, ReceivedCode,");
//                                                    sb2.Append("ReceivedDt, ReceivedAmount, Type, DueDt, Status, TransCompCode)");
//                                                    sb2.Append(" VALUES('" + OnAccountOf.Trim() + "','" + IndentorCode.Trim() + "','" + CustomerCode.Trim() + "',");
//                                                    sb2.Append("'" + StrComInv.Trim().Trim() + "','" + MOFPCCode.Trim() + "','" + DICode.Trim() + "',");
//                                                    sb2.Append("'" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "','" + DateTime.Now + "','" + PayTotalAmt + "',");
//                                                    sb2.Append("'" + ComCon.getTranName("select PayType from dispatchinstructiondetailssub where AdjRecNo='" + ds.Tables["ReceivedData"].Rows[j]["ReceiptNo"].ToString().Trim() + "'", "dispatchinstructiondetailssub", "PayType", con, tran).Trim() + "',");
//                                                    sb2.Append("'" + DateTime.Now + "','0','" + VoutReq.CompID.Trim() + "')");
//                                                    cmd = new SqlCommand(sb2.ToString(), con);
//                                                    cmd.Transaction = tran;
//                                                    cmd.ExecuteNonQuery();
//                                                    cmd.Dispose();

//                                                    PayTotalAmt = 0;
//                                                }
//                                            }
//                                        }
//                                        ds.Dispose();
//                                        ds.Clear();
//                                        //****************End**************** 
//                                    }

//                                    if (strComInvCodeInsp.Trim() == "")
//                                    {
//                                        if (StrComInv.Trim().Trim() != "")
//                                        {
//                                            strComInvCodeInsp = " AND Inspection Commercial : " + StrComInv.Trim().Trim();
//                                        }
//                                    }
//                                    else
//                                    {
//                                        if (StrComInv.Trim().Trim() != "")
//                                        {
//                                            strComInvCodeInsp += ", " + StrComInv.Trim().Trim();
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }


//                    #endregion
//                /*End Generate CommercialInvoice */
//                }
//                tran.Commit();                                
//                return strVehicleNo.Trim();
//            }
//            catch (Exception ex)
//            {
//                tran.Rollback();
//                return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());
//            }
//            finally
//            {
//                con.Close();
//            }
//        }      

//    }
//}