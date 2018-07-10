namespace GN
{
    public class NetOuterComponent : NetworkComponent
    {
        public NetOuterComponent()
        {
            MessagePacker = new MessagePackcer();
            MessageDispatcher = new OuterMessageDispatcher();
        }
    }
}
