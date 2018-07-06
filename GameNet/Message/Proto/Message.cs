namespace GN
{
    [Message(Opcode.RegisterAppRequest)]
    public class AppInfo : MessageRequest
    {
        public int AppId { get; set; }
        public AppType AppType { get; set; }
        public Address InnerAddress { get; set; }
        public Address OuterAdderss { get; set; }
    }
    [Message(Opcode.RegisterAppResponse)]
    public class RegisterAppResponse : MessageResponse
    {
        public bool IsSuccess { get; set; }
    }

    [Message(Opcode.GetAppById)]
    public class GetAppById : MessageRequest
    {
        public int appId { get; set; }
    }
    [Message(Opcode.GetAppByType)]
    public class GetAppByType : MessageRequest
    {
        public AppType appType { get; set; }
    }
    [Message(Opcode.ResponseAppInfo)]
    public class ResponseAppInfo : MessageResponse
    {
        public AppInfo appInfo { get; set; }
    }
    public class DB
    {
        [Message(Opcode.DBQueryFirst)]
        public class QueryFirst : MessageRequest
        {
            public string CollectionName { get; set; }
            public string Filter { get; set; }
        }
        
        [Message(Opcode.DBQuery)]
        public class Query : MessageRequest
        {
            public string CollectionName { get; set; }
            public string Filter { get; set; }
        }

        [Message(Opcode.DBQueryResponse)]
        public class QueryResponse : MessageResponse
        {

        }
    }
}

