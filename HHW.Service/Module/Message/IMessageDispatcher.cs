namespace HHW.Service
{
    public interface IMessageDispatcher
    {
        void Dispatch(Session session, Packet packet);
    }
}
