using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Production.CP.Plan
{
    public class ControlPanel_Request
    {

        public string Code { get; set; }
        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string CompCode { get; set; }
        public string JobCard_CpyDts { get; set; }

        public string Remark { get; set; }
        // New Monthly Plan Properties
        public string PlanCode { get; set; }
        public DateTime? PlanDate { get; set; }
        public int DayPlanQty { get; set; }

        // Optional: Additional properties that might be useful
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? DayNumber { get; set; }
        public string DayName { get; set; }
        public string TodayFlag { get; set; }
        public int PenPQty { get; set; }
    }
}
