using GN;

namespace Logic
{
    public class GetLoginKey
    {
        [Message(InnerOpcode.L2G_GetLoginKey)]
        public class Request : MessageRequest
        {
            public string account { get; set; }
        }

        [Message(InnerOpcode.G2L_GetLoginKey)]
        public class Response : MessageResponse
        {
            public long key { get; set; }
        }
    }
}
