namespace HHW.Service
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
            EventSystem.Add(this);            
        }

        public override void Dispose()
        {
            if(IsDisposed)
            {
                return;
            }

            base.Dispose();

            EventSystem.Destroy(this);
        }
    }
}
