﻿using GN;

namespace Logic
{
    [Message(OuterOpcode.Ping)]
    public class Ping : MessageRequest
    {

    }

    [Message(OuterOpcode.Login)]
    public class Login
    {
        public class Request : MessageRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }
        public class Response : MessageResponse
        {
            public bool IsLogin { get; set; }
            public long Key { get; set; }
            public string GateIP { get; set; }
            public int GatePort { get; set; }
        }
    }

    [Message(OuterOpcode.GateLogin)]
    public class GateLogin
    {
        public class Request : MessageRequest
        {
            public string account { get; set; }
            public long key { get; set; }
        }

        public class Response : MessageResponse
        {
            public Twill_User twill_user { get; set; }
            public PlayerState state { get; set; }
            public RoomInfo room_info { get; set; }
        }
    }
}
