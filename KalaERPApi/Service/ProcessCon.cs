using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace KalaERPApi.Service
{
    public class ProcessCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());

        public object SqlDbTypeChar { get; private set; }

        public DataTable DGPrcStockMOFWithPart(string compCode,string PCName)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getProductProcessMOF_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@CompanyCode", SqlDbType.Char, 5).Value = compCode;
            dAd.SelectCommand.Parameters.Add("@PCName", SqlDbType.Char, 20).Value = PCName;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
      
    }
}