using System;
using System.Collections.Generic;
using System.IO;

namespace GN
{
    public class ConfigComponent : Component
    {
        public int AppId;
        public AppType AppType;
        public Address MasterAddress;
        public Address InnerAddress;
        public Address OuterAddress;
        public Address DbAddress;
        public string DbConnection;
        public void Awake(string path)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] ss = File.ReadAllText(path).Split('\n');
            foreach (var s in ss)
            {
                string s1 = s.Replace(" ", "");
                if(string.IsNullOrEmpty(s1))
                {
                    continue;
                }

                string[] ss1 = s1.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (ss1.Length == 2)
                {
                    dict.Add(ss1[0], ss1[1]);
                }
            }

            this.AppId = int.Parse(dict["AppId"]);
            this.AppType = (AppType)Enum.Parse(typeof(AppType), dict["AppType"]);

            string v;
            if(dict.TryGetValue("MasterAddress", out v))
            {
                this.MasterAddress = new Address(v);
            }
            if (dict.TryGetValue("InnerAddress", out v))
            {
                this.InnerAddress = new Address(v);
            }
            if (dict.TryGetValue("OuterAddress", out v))
            {
                this.OuterAddress = new Address(v);
            }
            if(dict.TryGetValue("DbAddress", out v))
            {
                this.DbAddress = new Address(v);
            }
            if(dict.TryGetValue("DbConnection", out v))
            {
                this.DbConnection = v;
            }
        }
    }
}
