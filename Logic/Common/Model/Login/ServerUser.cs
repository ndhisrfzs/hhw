using GN;
using System;

namespace Logic
{
    [DBModel]
    public class ServerUser 
    {
        public long uid { get; set; }
        public long loginid { get; set; }
        public int server { get; set; }
        public DateTime last_login_time { get; set; }
    }
}
