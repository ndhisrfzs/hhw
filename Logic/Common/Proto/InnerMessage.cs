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

    [Message(InnerOpcode.GetGamePlayerId)]
    public class GetGamePlayerId
    {
        public class Request : MessageRequest
        {

        }

        public class Response : MessageResponse
        {
            public long id { get; set; }
        }
    }
}
