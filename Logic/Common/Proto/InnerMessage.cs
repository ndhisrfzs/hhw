using GN;
using System.Collections.Generic;

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
            public long sessionId { get; set; }
        }

        public class Response : MessageResponse
        {
            public long actorId { get; set; }
        }
    }

    [Message(InnerOpcode.G2M_MatchPlayer)]
    public class G2M_MatchPlayer
    {
        public class Request : MessageRequest
        {
            public long uid { get; set; }
            public string name { get; set; }
            public short sex { get; set; }
            public string head_url { get; set; }
            public int score { get; set; }
            public Games game { get; set; }
            public GameType game_type { get; set; }
            public byte model_type { get; set; }

        }

        public class Response : MessageResponse
        {
            public bool isSuccess { get; set; }
        }
    }


    [Message(InnerOpcode.M2G_InitRoom)]
    public class M2G_InitRoom
    {
        public class Request : MessageRequest
        {
            public List<IMatcher> players { get; set; }
        }

        public class Response : MessageResponse
        {
            public long actorId { get; set; }
        }
    }

    [Message(InnerOpcode.M2G_MatchSuccess)]
    public class M2G_MatchSuccess
    {
        public class Request : MessageRequest
        {
            public long uid { get; set; }
            public long roomActorId { get; set; }
        }

        public class Response : MessageResponse
        {

        }
    }
}
