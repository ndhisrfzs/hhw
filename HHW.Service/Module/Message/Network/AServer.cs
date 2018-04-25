using System.Net;
using System.Threading.Tasks;

namespace HHW.Service
{
    public abstract class AServer : Component
    {
        public abstract AClient GetClient(long id);

        public abstract Task<AClient> AcceptClient();

        public abstract AClient ConnectClient(IPEndPoint ipEndPoint);

        public abstract void Remove(long id);

        public abstract void Update();
    }
}
