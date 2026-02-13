using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.IO;
using System.Web.Http;
using System.Text.RegularExpressions;
using VehicleOutCustFbkRequest = KalaERPApi.Models.Transport.Trans.VehicleOutCustFbkRequest;

namespace KalaERPApi.Service.Transport.Trans
{
    public class VehicleOutCustFbkCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder();              
        public SqlTransaction tran = null;
        public object SqlDbTypeChar { get; private set; }

        public DataTable GetInvScanDts(string strComp, string strInvNo)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("VehileOutCustFbk_InvDetails_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@CompCode", SqlDbType.Char).Value = strComp;
            dAd.SelectCommand.Parameters.Add("@InvNo", SqlDbType.Char).Value = strInvNo;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit([FromBody] VehicleOutCustFbkRequest VoutCustFbkreq)
        {
            CommonCon ComCon = new CommonCon();
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                SqlCommand cmd = new SqlCommand();               
                string strInvNo = VoutCustFbkreq.InvDts;
                string file_name = null, stringdate = "";
                string image_string_items = VoutCustFbkreq.CustSign;
                System.Drawing.Image image = null;
                byte[] imageBytes = null;
                MemoryStream ms = null;
                int recCount = ComCon.CountChars(VoutCustFbkreq.InvDts, ",");
                string[] strInvDts = Regex.Split(VoutCustFbkreq.InvDts, ",");

                stringdate = DateTime.Now.ToString("MM-dd-yyyy_HHmmss").Trim();              
                file_name = stringdate.ToString().Trim() + ".jpg";
                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    if (strInvDts[cSub].ToString().Trim().Substring(10, 2) == "01" || strInvDts[cSub].ToString().Trim().Substring(10, 2) == "03" || strInvDts[cSub].ToString().Trim().Substring(10, 2) == "08")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update InvoiceSales set CustAckStatus='D',CustAckDt=GetDate(),CustAckName='" + VoutCustFbkreq.CustName.Trim() + "',CustAckSign='" + file_name.Trim() + "',CustLatitude='" + Convert.ToDouble(VoutCustFbkreq.Latitude.Trim()) + "',CustLongitude='" + Convert.ToDouble(VoutCustFbkreq.Longitude.Trim()) + "' where  Invid='" + strInvDts[cSub].ToString().Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    else
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update InvoiceDealer set CustAckStatus='D',CustAckDt=GetDate(),CustAckName='" + VoutCustFbkreq.CustName.Trim() + "',CustAckSign='" + file_name.Trim() + "',CustLatitude='" + Convert.ToDouble(VoutCustFbkreq.Latitude.Trim()) + "',CustLongitude='" + Convert.ToDouble(VoutCustFbkreq.Longitude.Trim()) + "' where  Invid='" + strInvDts[cSub].ToString().Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }

                if (image_string_items.Length > 0)
                {                    
                    imageBytes = Convert.FromBase64String(image_string_items.Trim());
                    if (imageBytes.Length > 0)
                    {
                        ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                        ms.Write(imageBytes, 0, imageBytes.Length);
                        image = System.Drawing.Image.FromStream(ms, true);
                        var filePath = ComCon.getMainFilePath("InvoiceAck") + "\\" + file_name;
                        if (!File.Exists(filePath))
                        {
                            image.Save(filePath);                           
                        }
                    }

                }

                tran.Commit();             
                return strInvNo;
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

        private void writetodisk(string fullpath, byte[] byteImage)
        {
            StreamReader sr = new StreamReader(new MemoryStream(byteImage));
            FileInfo file = new FileInfo(fullpath);

            try
            {                
                if (byteImage != null)
                {
                    if (!File.Exists(fullpath))
                    {
                        System.Drawing.Image img = System.Drawing.Bitmap.FromStream(sr.BaseStream);
                        img.Save(fullpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        img.Dispose();
                    }
                    sr.Dispose();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {                
                sr.Dispose();
                sr.Close();
            }
        }
    }
}