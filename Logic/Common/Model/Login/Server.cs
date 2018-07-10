using GN;

namespace Logic
{
    [DBModel]
    public class Server
    {
        public int id { get; set; }
        public short game_id { get; set; }
        public short status { get; set; }
        public short order_by { get; set; }
        public string name { get; set; }
        public string ip { get; set; }
        public short port { get; set; }
        public string prompt { get; set; }
    }
}
