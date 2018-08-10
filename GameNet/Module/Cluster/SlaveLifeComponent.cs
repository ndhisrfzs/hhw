#if Server
namespace GN
{
    public class SlaveLifeComponent : Component
    {
        private int AppId;
        public void Awake(int appId)
        {
            this.AppId = appId;
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            var master = Game.Scene.GetComponent<MasterComponent>();
            master.Remove(this.AppId);

            base.Dispose();
        }
    }
}
#endif