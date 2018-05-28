namespace HHW.Service
{
    [Message(MessageOpcode.C2S_Login)]
    public class C2S_Login : Request
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }

    [Message(MessageOpcode.S2C_Login)]
    public class S2C_Login : Response
    {
        public bool IsLogin { get; set; }
    }
}
