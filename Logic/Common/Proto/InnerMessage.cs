using GN;

namespace Logic
{
    [Message(InnerOpcode.GetLoginKey)]
    public class GetLoginKey
    {
        public class Request : MessageRequest
        {
            public string account { get; set; }
        }

        public class Response : MessageResponse
        {
            public long key { get; set; }
        }
    }
}
