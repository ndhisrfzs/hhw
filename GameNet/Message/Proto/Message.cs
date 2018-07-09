using System.Collections.Generic;

namespace GN
{
    public class AppInfo
    {
        public int AppId { get; set; }
        public AppType AppType { get; set; }
        public Address InnerAddress { get; set; }
        public Address OuterAdderss { get; set; }
    }

    [Message(Opcode.RegisterApp)]
    public class RegisterApp
    {
        public class Request : MessageRequest
        {
            public AppInfo appInfo { get; set; }
        }
        public class Response : MessageResponse
        {
            public bool IsSuccess { get; set; }
        }
    }
    
    [Message(Opcode.GetAppById)]
    public class GetAppById
    {
        public class Request : MessageRequest
        {
            public int appId { get; set; }
        }
        public class Response : MessageResponse
        {
            public AppInfo appInfo { get; set; }
        }
        
    }

    [Message(Opcode.GetAppByType)]
    public class GetAppByType
    {
        public class Request : MessageRequest
        {
            public AppType appType { get; set; }
        }
        public class Response : MessageResponse
        {
            public AppInfo appInfo { get; set; }
        }
    }

    [Message(Opcode.DBQueryFirst)]
    public class DBQueryFirst
    {
        public class Request : MessageRequest
        {
            public string CollectionName { get; set; }
            public string Filter { get; set; }
        }

        public class Response : MessageResponse
        {
            public object obj { get; set; }
        }
    }

    [Message(Opcode.DBQuery)]
    public class DBQuery
    {
        public class Request : MessageRequest
        {
            public string CollectionName { get; set; }
            public string Filter { get; set; }
        }
        public class Response : MessageResponse
        {
            public List<object> objs { get; set; }
        }
    }
}

