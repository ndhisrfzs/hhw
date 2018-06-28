using HHW.Service;

namespace HHW.Logic
{
    [MessageHandler(AppType.Login)]
    public class PingHandler : AMHandler<Ping>
    {
        static int i = 0;
        protected override void Run(Session session, Ping message)
        {
            Log.Info("ping ping" + ++i);
        }
    }
}
