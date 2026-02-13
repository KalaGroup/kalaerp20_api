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


namespace KalaERPApi.Service
{
    public class ColdStorageCon
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["CScon"].ToString());
        StringBuilder sb = new StringBuilder();
        public SqlTransaction tran = null;
        DataSet dsDetailsSub = new DataSet();


        public object SqlDbTypeChar { get; private set; }

        public DataTable CSParameter(string SiteId,string Parameter)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("[GetCSParameter]", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@SiteId", SqlDbType.Char).Value = SiteId;
            dAd.SelectCommand.Parameters.Add("@Parameter", SqlDbType.Char).Value = Parameter;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
    }
}