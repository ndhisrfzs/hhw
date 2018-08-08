namespace GN
{
    public abstract class Component : Object
    {
        public Component()
            :base()
        {
        }
        public Component(long id)
            :base(id)
        {
        }

        public override void AddManager()
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

            Game.EventSystem.Remove(this.id);
        }
    }
}
