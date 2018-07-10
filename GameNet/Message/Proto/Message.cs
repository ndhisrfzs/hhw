using System.Collections.Generic;

namespace GN
{
    public class AppInfo
    {
        public int appId { get; set; }
        public AppType appType { get; set; }
        public Address innerAddress { get; set; }
        public Address outerAdderss { get; set; }
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
            public bool isSuccess { get; set; }
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
            public string collectionName { get; set; }
            public string sql { get; set; }
            public object parms { get; set; }
        }

        public class Response : MessageResponse
        {
            public object data { get; set; }
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
            public List<object> data { get; set; }
        }
    }
}

