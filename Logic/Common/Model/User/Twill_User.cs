using GN;

namespace Logic
{
    [DBModel]
    public class Twill_User
    {
        public long uid { get; set; }
        public string name { get; set; }
        public short sex { get; set; }
        public int room_card { get; set; }
        public int gold { get; set; }
        public int score { get; set; }
        public string head_url { get; set; }
    }
}
