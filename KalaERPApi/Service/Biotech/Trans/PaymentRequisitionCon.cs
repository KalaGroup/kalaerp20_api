using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace KalaERPApi.Service.Biotech.Trans
{
    public class PaymentRequisitionCon
    {
        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder();
        DataSet dsChkStage = new DataSet();
        CommonCon ComCon = new CommonCon();
        string strSql = "";
        public SqlTransaction tran = null;
        SqlCommand cmd = new SqlCommand();
        #endregion

        public string Submit(string supplier_name, string supplier_code, string supp_address, string supplier_city,
            string contact_person, string contact_number, string bill_no, string bill_date, string bill_amt,
            string trans_type, string trans_amt, string remark, string beneficiary_name, string bank_name,
            string bank_address, string bank_account_no, string ifsc_no, string user_id, string pc_code)
        {
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); }
                con.Open();

                tran = con.BeginTransaction();

                if (supplier_code == "" || supplier_code == "0")
                {
                    supplier_code = ComCon.getName("select SCode from Supplier where SName='" + supplier_name.Trim() + "' and active='1' and Wadd like '%" + supp_address.Trim() + "%'", "tbl_Supplier", "SCode");
                }
                if (supplier_code == "" || supplier_code == "0")
                {
                    string onaccount_code = ComCon.getName("select SACCId from OnAccountofSup where Name='" + supplier_name.Trim() + "' and active='1'", "tbl_OnAccountofSup", "SACCId");
                    if (onaccount_code == "" || onaccount_code == "0")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Insert Into OnAccountofSup(Dt,Name,Remark)");
                        sb.Append(" VALUES ('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "','" + supplier_name.Trim() + "','Auto From System'); SELECT @@IDENTITY;");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        int ID = Convert.ToInt32(cmd.ExecuteScalar());
                        cmd.Dispose();
                        onaccount_code = ID.ToString();

                        //*********************User Acivity ***************************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                        cmd.Parameters.AddWithValue("@EmpID", user_id);
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "OnAccountofSup");
                        cmd.Parameters.AddWithValue("@TransactionNo", ID);
                        cmd.Parameters.AddWithValue("@CompanyCode", pc_code.Substring(0, 2).Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    string city = ComCon.getName("select s.SCode+'-->'+c.CCode+'-->'+cast(c.StateID as varchar) as ID from city c inner join state s on s.SID = c.StateID where c.CID= '" + supplier_city.Trim() + "'", "tbl_state", "ID");
                    string[] city_items = Regex.Split(city, "-->");
                    supplier_code = "02.02.01." + city_items[0].Trim() + "." + city_items[1].Trim() + ".";
                    supplier_code = ComCon.getCCodeMax("Select max(SCode) from Supplier where SCode like '" + supplier_code.Trim() + "%'", supplier_code.Trim());
                    if (supplier_code == "" || supplier_code == "0")
                    {
                        tran.Rollback();
                        con.Close();
                        return "Something Went Wrong";
                    }
                    else
                    {
                        #region
                        cmd = new SqlCommand("InsertUpdateSupplier", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@SCode", supplier_code);
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                        cmd.Parameters.AddWithValue("@SName", supplier_name.Trim());
                        cmd.Parameters.AddWithValue("@SAccId", onaccount_code.Trim());
                        cmd.Parameters.AddWithValue("@WAdd", supp_address.Trim());
                        cmd.Parameters.AddWithValue("@EMail", "");
                        cmd.Parameters.AddWithValue("@URL", "");
                        cmd.Parameters.AddWithValue("@PH1", "");
                        cmd.Parameters.AddWithValue("@PH2", "");
                        cmd.Parameters.AddWithValue("@Mobile", contact_number.Trim());
                        cmd.Parameters.AddWithValue("@Fax", "");
                        cmd.Parameters.AddWithValue("@ContactPerson", contact_person.Trim());
                        cmd.Parameters.AddWithValue("@CPPhNo", contact_number.Trim());
                        cmd.Parameters.AddWithValue("@VATTinNo", "");
                        cmd.Parameters.AddWithValue("@CSTTinNo", "");
                        cmd.Parameters.AddWithValue("@TANNo", "");
                        cmd.Parameters.AddWithValue("@PANNo", "");
                        cmd.Parameters.AddWithValue("@ECCNo", "");
                        cmd.Parameters.AddWithValue("@Range", "");
                        cmd.Parameters.AddWithValue("@GSTTinNo", "");
                        cmd.Parameters.AddWithValue("@Division", "");
                        cmd.Parameters.AddWithValue("@CommRate", "");
                        cmd.Parameters.AddWithValue("@CountryID", "01");
                        cmd.Parameters.AddWithValue("@StateID", city_items[2].Trim());
                        cmd.Parameters.AddWithValue("@CityID", supplier_city.Trim());
                        cmd.Parameters.AddWithValue("@OrgnTypeID", "02");
                        cmd.Parameters.AddWithValue("@PaySchedule", "01");
                        cmd.Parameters.AddWithValue("@LedgerID", "02");
                        cmd.Parameters.AddWithValue("@Remark", "Auto From Kala Fresh Mobile App");
                        cmd.Parameters.AddWithValue("@STNo", "");
                        cmd.Parameters.AddWithValue("@SSIRegNo", "");
                        cmd.Parameters.AddWithValue("@WEF", "");
                        cmd.Parameters.AddWithValue("@AuthRemark", "Auto From ERP");
                        cmd.Parameters.AddWithValue("@Active", 1);
                        cmd.Parameters.AddWithValue("@Auth", 1);
                        cmd.Parameters.AddWithValue("@Status", 0);
                        cmd.Parameters.AddWithValue("@WsStatusKalaToPms", 0);
                        cmd.Parameters.AddWithValue("@KalaToBio", 0);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("update Supplier set Auth1='1',Auth1Remark='Auto From ERP' where SCode='" + supplier_code + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //*********************User Acivity ***************************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                        cmd.Parameters.AddWithValue("@EmpID", user_id);
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "Supplier");
                        cmd.Parameters.AddWithValue("@TransactionNo", supplier_code);
                        cmd.Parameters.AddWithValue("@CompanyCode", pc_code.Substring(0, 2).Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion
                    }
                }

                #region     
                string txtDispCode = ComCon.GetMaxNo("ExpenceRequisitionWithPlan", "ERW", pc_code.Substring(0, 2).Trim(), con, tran);
                cmd = new SqlCommand("InsertExpenseRequisition", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@REQCode", txtDispCode.Trim());
                cmd.Parameters.AddWithValue("@MaxSrNo", txtDispCode.Substring(10, 8).ToString().Trim());
                cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                cmd.Parameters.AddWithValue("@Yr", txtDispCode.Substring(4, 5).ToString().Trim());
                cmd.Parameters.AddWithValue("@PCCode", pc_code.Trim());
                cmd.Parameters.AddWithValue("@ExpType", "S");
                cmd.Parameters.AddWithValue("@SupEmpCode", supplier_code.Trim());
                cmd.Parameters.AddWithValue("@EmpSupCode", "0");
                cmd.Parameters.AddWithValue("@BalAmount", 0);
                cmd.Parameters.AddWithValue("@Advance", "1");
                cmd.Parameters.AddWithValue("@PathA", trans_type.Trim());
                cmd.Parameters.AddWithValue("@Remark", remark.Trim());
                cmd.Parameters.AddWithValue("@AuthRemark", "NIL");
                cmd.Parameters.AddWithValue("@CompanyCode", pc_code.Substring(0, 2).Trim());
                cmd.Parameters.AddWithValue("@SRVNo", "0");
                cmd.Parameters.AddWithValue("@ACTNo", "0");
                cmd.Parameters.AddWithValue("@EngTransAmt", 0);
                cmd.Parameters.AddWithValue("@Auth", "0");
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                sb.Remove(0, sb.Length);
                sb.Append("update ExpenceRequisitionWithPlan set AuthHOD='1',AuthHODRemark='Auto From App',");
                sb.Append("BeneficiaryName='"+ beneficiary_name.Trim() + "',BankName='"+ bank_name.Trim() + "',");
                sb.Append("BankAaddress='" + bank_address.Trim() + "',BankAccountNo='"+ bank_account_no.Trim() + "',"); 
                sb.Append("IFSCNo='" + ifsc_no.Trim() + "' where REQCode='" + txtDispCode.Trim() + "'");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();    
                #endregion

                #region 
                cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@REQCode", txtDispCode.Trim());
                cmd.Parameters.AddWithValue("@SrNo", "1");
                cmd.Parameters.AddWithValue("@EXPCode", "0113");
                cmd.Parameters.AddWithValue("@Qty", "1");
                cmd.Parameters.AddWithValue("@Rate", bill_amt);
                cmd.Parameters.AddWithValue("@Amount", bill_amt);
                cmd.Parameters.AddWithValue("@ExpReqVATPer", "0");
                cmd.Parameters.AddWithValue("@ExpReqCSTPer", "0");
                cmd.Parameters.AddWithValue("@ExpReqServieTPer", "0");
                cmd.Parameters.AddWithValue("@ExpReqSBCessPer", "0");
                cmd.Parameters.AddWithValue("@ExpReqTDSPer", "0");
                cmd.Parameters.AddWithValue("@ExpBillNo", bill_no.Trim());
                if (string.IsNullOrEmpty(bill_date.Trim()))
                {
                    cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ExpBillDt", bill_date.Trim());
                }
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                if (trans_type.Trim() == "Exclude")
                {
                    cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@REQCode", txtDispCode.Trim());
                    cmd.Parameters.AddWithValue("@SrNo", "2");
                    cmd.Parameters.AddWithValue("@EXPCode", "0024");
                    cmd.Parameters.AddWithValue("@Qty", "1");
                    cmd.Parameters.AddWithValue("@Rate", trans_amt);
                    cmd.Parameters.AddWithValue("@Amount", trans_amt);
                    cmd.Parameters.AddWithValue("@ExpReqVATPer", "0");
                    cmd.Parameters.AddWithValue("@ExpReqCSTPer", "0");
                    cmd.Parameters.AddWithValue("@ExpReqServieTPer", "0");
                    cmd.Parameters.AddWithValue("@ExpReqSBCessPer", "0");
                    cmd.Parameters.AddWithValue("@ExpReqTDSPer", "0");
                    cmd.Parameters.AddWithValue("@ExpBillNo", bill_no.Trim());
                    if (string.IsNullOrEmpty(bill_date.Trim()))
                    {
                        cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ExpBillDt", bill_date.Trim());
                    }
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                #endregion

                #region 
                //****************User Acivity****************
                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@EmpID", user_id.Trim());
                cmd.Parameters.AddWithValue("@TransactionType", "S");
                cmd.Parameters.AddWithValue("@TransactionFrom", "ExpenseRequisition");
                cmd.Parameters.AddWithValue("@TransactionNo", txtDispCode.Trim());
                cmd.Parameters.AddWithValue("@CompanyCode", pc_code.Substring(0, 2).Trim());
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                #endregion

                tran.Commit();
                return txtDispCode.Trim();
            }
            catch (Exception ex)
            {
                #region 
                FileStream fs = new FileStream("C:/Error/ApiError.txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter m_streamWriter = new StreamWriter(fs);
                m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                m_streamWriter.WriteLine();
                m_streamWriter.WriteLine("***************** " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + "*****************");
                m_streamWriter.WriteLine("StackTrace " + ex.StackTrace.ToString());
                m_streamWriter.WriteLine("Message " + ex.Message.ToString());
                m_streamWriter.Flush();
                m_streamWriter.Close();
                #endregion
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