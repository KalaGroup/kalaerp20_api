using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using KalaERPApi.Models.Request.UserActivity;

namespace KalaERPApi.Service.UserActivity
{
    public class LoginAppCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        public SqlTransaction tran = null;
        CommonCon cls = new CommonCon();

        public DataTable CheckLogin(string UserName, string Psw)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("CheckLogin", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@UserName", SqlDbType.Char).Value = UserName;
            dAd.SelectCommand.Parameters.Add("@Psw", SqlDbType.Char).Value = Psw;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetCompanyName()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetLoginCompany", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            //dAd.SelectCommand.Parameters.Add("@UserName", SqlDbType.Char).Value = UserName;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetPageRights(string userid, int pagetype, string comp_id, string pccode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("PageRights", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@userid", SqlDbType.Char).Value = userid;
            dAd.SelectCommand.Parameters.Add("@pagetype", SqlDbType.Char).Value = pagetype;
            dAd.SelectCommand.Parameters.Add("@CompCode", SqlDbType.Char).Value = comp_id;
            dAd.SelectCommand.Parameters.Add("@pccode", SqlDbType.Char).Value = pccode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable MainMenuApp(string userid, string comp_id)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("MainMenuApp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@userid", SqlDbType.Char).Value = userid;
            dAd.SelectCommand.Parameters.Add("@CompCode", SqlDbType.Char).Value = comp_id;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable UserDetails(string IMEINo)
        {
            string strSql = "select e.DeptName,l.LoginType,l.Name as UserID,Fname+' '+LName as UserName,e.CompMailID as EmailID,m.CompanyCode as CID,CAliseName as CName from " +
                " MenuRights m inner join MenuRightsDetailsSub mrds on m.mrid = mrds.mrid inner join loginmst l on m.empcode = l.name inner join employee e on e.ecode = l.name " +
                " inner join company c on c.cid = m.CompanyCode where m.active = '1' and l.active = '1' and mrds.AllowMApp in('1', '2') and l.IMEINo = @IMEINo " +
                " group by e.DeptName,l.LoginType,l.Name,e.Fname,e.LName,m.CompanyCode,CAliseName,e.CompMailID order by m.CompanyCode";
            SqlDataAdapter dAd = new SqlDataAdapter(strSql, con);
            dAd.SelectCommand.CommandType = CommandType.Text;
            dAd.SelectCommand.Parameters.Add("@IMEINo", SqlDbType.Char).Value = IMEINo;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable KVendorUserDetails(string IMEINo)
        {
            string strSql = "select Active=case l.active when '1' then 'Yes' else 'No' end,l.Password,l.LoginType,l.Name as UserID,s.ContactPerson as UserName,s.EMail as EmailID,s.Mobile,s.SCode as CID,SName as CName " +
                " ,l.Password,l.active from loginmst l inner join Supplier s on s.scode = l.suppliercode where s.active='1' and s.discard='1' and l.active = '1' " +
                " and l.IMEINo=@IMEINo group by l.LoginType,l.Name,s.ContactPerson,s.EMail,s.SCode,SName,s.Mobile,l.active,l.Password";
            SqlDataAdapter dAd = new SqlDataAdapter(strSql, con);
            dAd.SelectCommand.CommandType = CommandType.Text;
            dAd.SelectCommand.Parameters.Add("@IMEINo", SqlDbType.Char).Value = IMEINo;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string UserActivation(string UserId, string Password, string mPin, string IMEINo)
        {
            string status = "Failed";
            try
            {
                string chkDeActive = cls.getName("select ID From Loginmst where Name='" + UserId.Trim() + "' and PassWord='" + Password.Trim() + "' and Active='0'", "tbl_Loginmst_DeActive", "ID");
                if (chkDeActive.Trim() != "0")
                {
                    status = "DeActive";
                    return status;
                }
                string chkStatus = cls.getName("select ID From Loginmst where Name='" + UserId.Trim() + "' and PassWord='" + Password.Trim() + "' and Active='1'", "tbl_Loginmst_Invalid", "ID");
                if (chkStatus.Trim() == "0")
                {
                    status = "Invalid";
                    return status;
                }
                else
                {
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };

                    sb.Remove(0, sb.Length);
                    sb.Append("Update LoginMst set IMEINo=@IMEINo,mPIN=@mPIN where Name=@Name and PassWord=@PassWord and Active='1'");
                    SqlCommand cmd = new SqlCommand(sb.ToString(), con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@IMEINo", SqlDbType.Char).Value = IMEINo.Trim();
                    cmd.Parameters.Add("@mPIN", SqlDbType.Char).Value = mPin.Trim();
                    cmd.Parameters.Add("@Name", SqlDbType.Char).Value = UserId.Trim();
                    cmd.Parameters.Add("@PassWord", SqlDbType.Char).Value = Password.Trim();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    status = "Success";
                }
            }
            catch (Exception ex)
            {
                status = "StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString();
            }

            finally
            {
                con.Close();
            }
            return status;
        }

        public string LoginCredintial(string IMEINo, string mPin)
        {
            string status = "Failed";
            try
            {
                string chkStatus = cls.getName("select Name From Loginmst where Active='1' and mPIN='" + mPin.Trim() + "' and IMEINo='" + IMEINo.Trim() + "'", "Loginmst", "Name");
                if (chkStatus.Trim() != "0")
                {
                    status = "Success";
                    return status;
                }
            }
            catch (Exception ex)
            {
                status = "StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return status;
        }

        public string InsertDeviceLocation(DeviceLocation deviceLocation)
        {
            string status = "Failed";
            try
            {
                string chkAlreadySaved = cls.getName("select ID from GPSTracker where log_dt between '" + deviceLocation.log_dt.Trim() + "' and '" + deviceLocation.log_dt.Trim() + "' and IMEINo='" + deviceLocation.imei_no.Trim() + "' and active='1'", "tbl_GPSTracker", "ID");
                if (chkAlreadySaved.Trim() == "0")
                {
                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into GPSTracker(Log_Dt,IMEINo,UserID,Latitude,Longitude,Location) values(@Log_Dt,@IMEINo,@UserID,@Latitude,@Longitude,@Location)");
                    SqlCommand cmd = new SqlCommand(sb.ToString(), con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@Log_Dt", SqlDbType.Char).Value = deviceLocation.log_dt.Trim();
                    cmd.Parameters.Add("@IMEINo", SqlDbType.Char).Value = deviceLocation.imei_no.Trim();
                    cmd.Parameters.Add("@UserID", SqlDbType.Char).Value = deviceLocation.user_id.Trim();
                    cmd.Parameters.Add("@Latitude", SqlDbType.Char).Value = deviceLocation.latitude.Trim();
                    cmd.Parameters.Add("@Longitude", SqlDbType.Char).Value = deviceLocation.longitude.Trim();
                    cmd.Parameters.Add("@Location", SqlDbType.Char).Value = deviceLocation.address.Trim();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    con.Close();
                    status = "Success";
                }
                else
                {
                    status = "Success";
                }
            }
            catch (Exception ex)
            {
                status = "StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString();
            }

            finally
            {
                con.Close();
            }
            return status;
        }

    }
}