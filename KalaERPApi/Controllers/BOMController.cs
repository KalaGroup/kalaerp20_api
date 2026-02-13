using System.Web.Http;
using System.Data;
using KalaERPApi.Service;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System;

namespace KalaERPApi.Controllers
{
    public class BOMController : ApiController
    {
        CommonCon cf = new CommonCon();

        [HttpGet]
        [Route("BOM/GetBOMKitType")]
        public DataTable GetBOMKitType()
        {
            DataTable ds = new DataTable();
            DBConnection dc = new DBConnection();
            ds = dc.BOMKitType();
            return ds;
       
        }
        [HttpGet]
        [Route("BOM/GetBOMProduct")]
        public DataTable GetBOMProduct(string KitType,string ProdType)
        {
            DataTable ds = new DataTable();
            DBConnection dc = new DBConnection();
            ds = dc.BOMProduct(KitType,ProdType);
            return ds;
        }
               
    }
}
