
namespace KalaERPApi.Models.Request.UserActivity
{
    public class DeviceLocation
    {
        public string log_dt { get; set; }
        public string imei_no { get; set; }
        public string user_id { get; set; }
        public string latitude { get; set; }       
        public string longitude { get; set; }
        public string address { get; set; }       
    }

    public class UserMaster
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string mobile_no { get; set; }
        public string email_id { get; set; }
    }

}