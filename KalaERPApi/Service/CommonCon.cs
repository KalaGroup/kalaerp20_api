using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.IO;

namespace KalaERPApi.Service
{
    public class CommonCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        public string GetMaxNo(string TableName, string Prefix, string CompCode, SqlConnection con, SqlTransaction tran)
        {
            string strmax = "";
            string NewTransCode = "";
            int intmax = 0;

            try
            {
                //GetMaxCode
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(MaxValue,0) as MXNO FROM GetMaxCode WHERE TblName='" + TableName + "'  and CompCode='" + CompCode + "' AND Prefix='" + Prefix + "' and Yr='" + yearEnd(con, tran) + "'", con);
                cmd.CommandTimeout = 0;
                cmd.Transaction = tran;
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

                NewTransCode = Prefix + "/" + yearEnd(con, tran) + "/" + CompCode + strmax;

                //Update MaxCode
                sb.Remove(0, sb.Length);
                sb.Append("UPDATE GetMaxCode SET MaxValue='" + strmax + "' WHERE Prefix='" + Prefix + "' and TblName='" + TableName + "' ");
                sb.Append("and CompCode='" + CompCode + "' AND Yr='" + yearEnd(con, tran) + "'");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                return NewTransCode;
            }
            catch
            {
                return null;
            }
        }
        public DataSet procTranDS(string strproc, string tblName, SqlConnection con, SqlTransaction tran)
        {
            SqlDataAdapter dAd = new SqlDataAdapter(strproc, con);
            dAd.SelectCommand.CommandType = CommandType.Text;
            dAd.SelectCommand.Transaction = tran;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            try
            {
                dAd.Fill(dSet, tblName);
                return dSet;
            }
            catch
            {
                throw;
            }
            finally
            {
                dSet.Dispose();
                dAd.Dispose();
            }
        }
        public DataTable GetEmpPCINFO(string Type, string Code)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetEmpPCINFO_SP", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Code;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable GetUserLoginInfo(string Id, string Password)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetLoginInfo_SP", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Id", SqlDbType.Char).Value = Id;
            dAd.SelectCommand.Parameters.Add("@Password", SqlDbType.Char).Value = Password;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable GetLoginCompInfo(string Type, string Code)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetLoginCompInfo_SP", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Code;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();

            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataSet procDS(string strproc, string tblName)
        {
            SqlDataAdapter dAd = new SqlDataAdapter(strproc, con);
            dAd.SelectCommand.CommandType = CommandType.Text;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            try
            {
                dAd.Fill(dSet, tblName);
                return dSet;
            }
            catch
            {
                throw;
            }
            finally
            {
                dSet.Dispose();
                dAd.Dispose();
            }
        }
        public DataTable procDT(string strproc, string tblName)
        {
            SqlDataAdapter dAd = new SqlDataAdapter(strproc, con);
            dAd.SelectCommand.CommandType = CommandType.Text;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            try
            {
                dAd.Fill(dSet, tblName);
                return dSet.Tables[tblName];
            }
            catch
            {
                throw;
            }
            finally
            {
                dSet.Dispose();
                dAd.Dispose();
            }
        }
        public DataTable GetPCodeAll(string PCCode, string ReqType)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetPCCodeALL", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
            dAd.SelectCommand.Parameters.Add("@ReqType", SqlDbType.Char).Value = ReqType;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable GetPartDesc(string PartCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetPartDesc", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@PartCode", SqlDbType.Char).Value = PartCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public string yearEnd(SqlConnection con, SqlTransaction tran)
        {
            DataSet ds = procTranDS("select substring(convert(Varchar(10),startdate,103),9,2)+'-'+ substring(convert(Varchar(10),enddate,103),9,2) as yr from yearend", "yearend", con, tran);
            return ds.Tables["yearend"].Rows[0]["yr"].ToString();
        }
        public string SysStartDate(SqlConnection con, SqlTransaction tran)
        {
            DataSet ds = procTranDS("select convert(varchar(10),StartDate,103) as SDate ,Convert(varchar(10),EndDate,103) as EDate from  YearEnd", "SysStart", con, tran);
            return ds.Tables["SysStart"].Rows[0]["SDate"].ToString();
        }
        public string SysEndDate(SqlConnection con, SqlTransaction tran)
        {
            DataSet ds = procTranDS("select convert(varchar(10),StartDate,103) as SDate ,Convert(varchar(10),EndDate,103) as EDate from  YearEnd", "SysEnd", con, tran);
            return ds.Tables["SysEnd"].Rows[0]["EDate"].ToString();
        }
        public int CountChars(string sText, string str)
        {
            char[] TextArray;
            int lCount = 0;
            TextArray = sText.ToCharArray();
            for (int i = 0; (i < TextArray.Length); i++)
            {
                if ((TextArray[i] == Convert.ToChar(str)))
                {
                    lCount = lCount + 1;
                }
            }
            return lCount;
        }
        public string getName(string strqry, string tblName, string fieldName)
        {
            SqlDataAdapter dAd = new SqlDataAdapter(strqry, con);
            dAd.SelectCommand.CommandType = CommandType.Text;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            try
            {
                dAd.Fill(dSet, tblName);
                if (dSet.Tables[tblName].Rows.Count > 0)
                {
                    if (dSet.Tables[tblName].Rows[0][fieldName].ToString() == "")
                    {
                        return "0";
                    }
                    else
                    {
                        return dSet.Tables[tblName].Rows[0][fieldName].ToString();
                    }
                }
                else
                {
                    return "0";
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                dSet.Dispose();
                dAd.Dispose();
            }
        }
        public string dateinyyyymmdd(string dt)
        {
            if (dt == "")

                return "1900-01-01";
            else
                return dt.Substring(6, 4) + "-" + dt.Substring(3, 2) + "-" + dt.Substring(0, 2);
        }
        public string getTranName(string strqry, string tblName, string fieldName, SqlConnection con, SqlTransaction tran)
        {
            SqlDataAdapter dAd = new SqlDataAdapter(strqry, con);
            dAd.SelectCommand.CommandType = CommandType.Text;
            dAd.SelectCommand.Transaction = tran;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            try
            {
                dAd.Fill(dSet, tblName);
                if (dSet.Tables[tblName].Rows.Count > 0)
                {
                    if (dSet.Tables[tblName].Rows[0][fieldName].ToString() == "")
                    {
                        return "0";
                    }
                    else
                    {
                        return dSet.Tables[tblName].Rows[0][fieldName].ToString();
                    }
                }
                else
                {
                    return "0";
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                dSet.Dispose();
                dAd.Dispose();
            }
        }
        public string getMainFilePath(string MFileFolder)
        {
            string yr = getName("SELECT Year(GETDATE())AS Yr", "tblYr", "Yr");
            string Mainfolder = "F:\\ERP" + "\\" + yr.Trim();
            if (!Directory.Exists(Mainfolder))
            {
                Directory.CreateDirectory(Mainfolder);
            }
            string Mnth = getName("SELECT CASE WHEN month(GETDATE())<10 THEN '0'+ cast(month(GETDATE())AS nvarchar(10))+' '+DATENAME(MONTH, GETDATE()) " +
                        "WHEN month(GETDATE())>=10 THEN cast(month(GETDATE())AS nvarchar(10))+' '+DATENAME(MONTH, GETDATE()) " +
                        "END AS Mnth", "tblCmnth", "Mnth");
            Mainfolder = "F:\\ERP" + "\\" + yr.Trim() + "\\" + Mnth.Trim();
            if (!Directory.Exists(Mainfolder))
            {
                Directory.CreateDirectory(Mainfolder);
            }
            Mainfolder = "F:\\ERP" + "\\" + yr.Trim() + "\\" + Mnth.Trim() + "\\" + MFileFolder.Trim();
            if (!Directory.Exists(Mainfolder))
            {
                Directory.CreateDirectory(Mainfolder);
            }
            return Mainfolder;
        }

        public DataTable GetPrcStatus()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetTRPrcStatus", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetPrcChkDts(string strStageNo, string PrcStatusName)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetTRPrcChkDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@strStageNo", SqlDbType.Char).Value = strStageNo;
            dAd.SelectCommand.Parameters.Add("@PrcStatusName", SqlDbType.Char).Value = PrcStatusName;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string NumberToText(int number)
        {
            if (number == 0) return "Zero";
            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Fourty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };
            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        public string getCCodeMax(string strSqlQuery, string StrCode)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(strSqlQuery, con);
                cmd.CommandTimeout = 0;
                con.Open();
                if ((cmd.ExecuteScalar() == DBNull.Value))
                {
                    StrCode = (StrCode + "0001");
                }
                else
                {
                    StrCode = Convert.ToString(cmd.ExecuteScalar());
                    int codeCnt = Convert.ToInt32(StrCode.Substring(15, 4));
                    codeCnt = (codeCnt + 1);
                    if ((codeCnt.ToString().Length == 4))
                    {
                        StrCode = (StrCode.Substring(0, 15) + codeCnt);
                    }
                    else if ((codeCnt.ToString().Length == 3))
                    {
                        StrCode = (StrCode.Substring(0, 15) + ("0" + codeCnt));
                    }
                    else if ((codeCnt.ToString().Length == 2))
                    {
                        StrCode = (StrCode.Substring(0, 15) + ("00" + codeCnt));
                    }
                    else if ((codeCnt.ToString().Length == 1))
                    {
                        StrCode = (StrCode.Substring(0, 15) + ("000" + codeCnt));
                    }
                }
                return StrCode;
            }
            catch 
            {
                return "0";
            }
            finally {
                con.Close();
            }
        }

        public DataTable Get6M()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("Get6M", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }


        public  string getStockTbl(string CompID)
        {
            string strmax = "0";
            if (CompID == "01")
                strmax = "Stock01";
            else if (CompID == "02")
                strmax = "Stock04";
            else if (CompID == "03")
                strmax = "Stock03";
            else if (CompID == "04")
                strmax = "Stock02";
            else if (CompID == "05")
                strmax = "Stock05";
            else if (CompID == "07")
                strmax = "Stock07";
            else if (CompID == "08")
                strmax = "Stock08";
            else if (CompID == "09")
                strmax = "Stock09";
            else if (CompID == "10")
                strmax = "Stock10";
            else if (CompID == "13")
                strmax = "Stock13";
            else if (CompID == "14")
                strmax = "Stock14";
            else if (CompID == "15")
                strmax = "Stock15";
            else if (CompID == "16")
                strmax = "Stock16";
            else if (CompID == "17")
                strmax = "Stock17";
            else if (CompID == "18")
                strmax = "Stock18";
            else if (CompID == "19")
                strmax = "Stock19";
            else if (CompID == "20")
                strmax = "Stock20";
            else if (CompID == "21")
                strmax = "Stock21";
            else if (CompID == "22")
                strmax = "Stock22";
            else if (CompID == "23")
                strmax = "Stock23";
            return strmax;
        }
    }
}
