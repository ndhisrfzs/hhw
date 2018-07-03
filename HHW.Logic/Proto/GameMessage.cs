using HHW.Service;

namespace HHW.Service
{
    public class Login
    {
        [Message(MessageOpcode.C2S_Login)]
        public class Request : MessageRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }
        [Message(MessageOpcode.S2C_Login)]
        public class Response : MessageResponse
        {
            public bool IsLogin { get; set; }
        }
    }

    [Message(MessageOpcode.Ping)]
    public class Ping: MessageRequest
    {

    }
}
