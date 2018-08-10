namespace GN
{
    public class NetClientComponent : NetworkComponent
    {
        public NetClientComponent()
        {
            MessagePacker = new MessagePackcer();
            MessageDispatcher = new ClientMessageDispatcher();
        }
    }
}
