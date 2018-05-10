namespace HHW.Service
{
    [Message(MessageOpcode.C2S_Login)]
    public class C2S_Login : RequestMessage
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }

    [Message(MessageOpcode.S2C_Login)]
    public class S2C_Login : ResponseMessage
    {
        public bool IsLogin { get; set; }
    }
}
