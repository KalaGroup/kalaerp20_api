using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using KalaERPApi.Models.Response;
using System.Data.Common;

namespace KalaERPApi.Service
{
    public class DBConnection
    {        
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        public DataTable PurchaseRequsitionPCName(string compCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("PurchaseRequision_PCName", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@CompCode", SqlDbType.Char).Value = compCode;            
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0]; 
        }

        public DataTable GetPartDescReqByClassCode(string ClassCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("PurchaseRequision_Part", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@ClassCode", SqlDbType.Char).Value = ClassCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataSet PurchaseRequsitionSuppName(string partcode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("PurchaseRequision_SuppName", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@PartCode", SqlDbType.VarChar,20).Value = partcode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet; 
        }
        public DataTable BOMKitType()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("BOM_KitType", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            //dAd.SelectCommand.Parameters.Add("@CompCode", SqlDbType.Char).Value = compCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable BOMProduct(string KitType,string ProdType)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("BOMProductDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@KitType", SqlDbType.VarChar, 20).Value = KitType;
            dAd.SelectCommand.Parameters.Add("@ProdType", SqlDbType.Char, 6).Value = ProdType;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
    }
}