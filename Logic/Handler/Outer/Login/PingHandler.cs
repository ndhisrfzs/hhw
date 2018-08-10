using GN;
using System.Threading.Tasks;

namespace Logic
{
    [MessageHandler(AppType.Login)]
    public class PingHandler : AMHandler<Session, Ping>
    {
        static int i = 0;
        protected override Task Run(Session session, Ping message)
        {
            Log.Info("ping ping" + ++i);
            return Task.CompletedTask;
        }
    }
}
