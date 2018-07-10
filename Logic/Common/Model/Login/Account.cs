using GN;
using System;

namespace Logic
{
    [DBModel]
    public class Account
    {
        public long loginid { get; set; }
        public string account { get; set; }
        public string password { get; set; }
        public short type { get; set; }
        public DateTime create_time { get; set; }
        public DateTime last_login_time { get; set; }
        public string last_login_ip { get; set; }
        public short status { get; set; }
    }
}
