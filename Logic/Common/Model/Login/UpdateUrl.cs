using GN;

namespace Logic
{
    [DBModel]
    public class UpdateUrl
    {
        public short game_id { get; set; }
        public short os { get; set; }
        public string url { get; set; }
    }
}
