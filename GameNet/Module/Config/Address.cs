using System;
using System.Net;

namespace GN
{
    public class Address
    {
        public string IP { get; set; }
        public int Port { get; set; }

        public Address() { }

        public Address(string address)
        {
            try
            {
                var ss = address.Split(':');
                this.IP = ss[0];
                this.Port = int.Parse(ss[1]);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public IPEndPoint IpEndPoint()
        {
            return NetworkHelper.ToIPEndPoint(this.IP, this.Port);
        }
    }
}
