using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Marketing.Auth
{
    public class NFAEntryAuth
    {
        public string NFAID { get; set; }
        public string UserID { get; set; }
        public string AuthRemark { get; set; }
    }
}