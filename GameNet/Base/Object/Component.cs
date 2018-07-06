namespace GN
{
    public abstract class Component : Object
    {
        public Component()
            :base()
        {
            AddEventSystem();
        }
        public Component(long id)
            :base(id)
        {
            AddEventSystem();
        }

        public void AddEventSystem()
        {
            Game.EventSystem.Add(this);            
        }

        public override void Dispose()
        {
            if(IsDisposed)
            {
                return;
            }

            base.Dispose();

            Game.EventSystem.Destroy(this);
        }
    }
}
