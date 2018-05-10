namespace HHW.Service
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

        public override void Dispose()
        {
            if(IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}
