namespace HHW.Service
{
    [Message(1)]
    public class C2S_Login : IRequest
    {
        public int RpcId { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
    }

    [Message(2)]
    public class S2C_Login : IResponse
    {
        public int RpcId { get; set; }
        public int Error { get; set; }
        public string Message { get; set; } 
        public bool IsLogin { get; set; }
    }
}
