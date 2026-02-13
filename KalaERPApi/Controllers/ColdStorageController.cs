using KalaERPApi.Models.Request;
using KalaERPApi.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KalaERPApi.Controllers
{
    public class ColdStorageController : ApiController
    {
        
        [HttpGet]
        [Route("ColdStorage/Healthy")]
        public DataTable GetHealthy(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId,"Healthy");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/PLAD")]
        public DataTable GetPLAD(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "PLAD");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/HVAC")]
        public DataTable GetHVAC(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "HVAC");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/Temperature")]
        public DataTable GetTemperature(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "Temperature");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/Humidity")]
        public DataTable GetHumidity(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "Humidity");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/VR")]
        public DataTable VR(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "VR");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/VY")]
        public DataTable VY(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "VY");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/VB")]
        public DataTable VB(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "VB");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/AR")]
        public DataTable AR(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "AR");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/AY")]
        public DataTable AY(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "AY");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/AB")]
        public DataTable AB(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "AB");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/Compressor")]
        public DataTable Compressor(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "Compressor");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/LP")]
        public DataTable LP(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "LP");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/HP")]
        public DataTable HP(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "HP");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/TempSensor")]
        public DataTable TempSensor(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "TempSensor");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/DefrostSensor")]
        public DataTable DefrostSensor(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "DefrostSensor");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/EB")]
        public DataTable EB(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "EB");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/UnderVoltage")]
        public DataTable UnderVoltage(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "UnderVoltage");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/OverVoltage")]
        public DataTable OverVoltage(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "OverVoltage");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/PHseq")]
        public DataTable PHseq(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "PHseq");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/OverCurrent")]
        public DataTable OverCurrent(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "OverCurrent");
            return ds;
        }

        [HttpGet]
        [Route("ColdStorage/UnderCurrent")]
        public DataTable UnderCurrent(string SiteId)
        {
            DataTable ds = new DataTable();
            ColdStorageCon dc = new ColdStorageCon();
            ds = dc.CSParameter(SiteId, "UnderCurrent");
            return ds;
        }
    }
}
