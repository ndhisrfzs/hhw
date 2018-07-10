using GN;
using System;

namespace Logic
{
    [DBModel]
    public class User
    {
        public long uid { get; set; }
        public string name { get; set; }
        public short sex { get; set; }
        public int user_level { get; set; }
        public int exp { get; set; }
        public int gold { get; set; }
        public int pvp_score { get; set; }
        public DateTime create_time { get; set; }
        public DateTime last_login_time { get; set; }
    }
}
