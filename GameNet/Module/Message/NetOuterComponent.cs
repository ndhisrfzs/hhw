namespace GN
{
    public class NetOuterComponent : NetworkComponent
    {
        public NetOuterComponent()
        {
            MessagePacker = new MessagePakcer();
            MessageDispatcher = new OuterMessageDispatcher();
        }
    }
}
