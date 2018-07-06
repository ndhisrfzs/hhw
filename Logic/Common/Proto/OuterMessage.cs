using GN;

namespace Logic
{
    public class Login
    {
        [Message(OuterOpcode.C2S_Login)]
        public class Request : MessageRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }
        [Message(OuterOpcode.S2C_Login)]
        public class Response : MessageResponse
        {
            public bool IsLogin { get; set; }
            public long Key { get; set; }
            public string GateIP { get; set; }
            public int GatePort { get; set; }
        }
    }

    [Message(OuterOpcode.Ping)]
    public class Ping: MessageRequest
    {

    }

    public class GateLogin
    {
        [Message(OuterOpcode.C2G_Login)]
        public class Request : MessageRequest
        {
            public string account { get; set; }
            public long key { get; set; }
        }

        [Message(OuterOpcode.G2C_Login)]
        public class Response : MessageResponse
        {
            public Twill_User twill_user { get; set; }
            public PlayerState state { get; set; }
            public RoomInfo room_info { get; set; }
        }
    }
}
