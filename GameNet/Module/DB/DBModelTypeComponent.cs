#if Server
using System;
using System.Collections.Generic;

namespace GN
{
    public class DBModelTypeComponent : Component
    {
        private readonly Dictionary<string, Type> DBModelTypes = new Dictionary<string, Type>();

        void Awake()
        {
            var types = Game.EventSystem.GetTypes();
            foreach (var type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(DBModelAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                this.DBModelTypes.Add(type.Name, type);
            }
        }

        public Type GetType(string name)
        {
            Type type;
            this.DBModelTypes.TryGetValue(name, out type);
            return type;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}
#endif