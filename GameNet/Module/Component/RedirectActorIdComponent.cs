namespace GN
{
    public class RedirectActorIdComponent : Component
    {
        public long ActorId;
        public void Awake(long redirectActorId)
        {
            this.ActorId = redirectActorId;
        }
    }
}
