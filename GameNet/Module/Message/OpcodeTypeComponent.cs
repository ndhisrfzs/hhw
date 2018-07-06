using System;
using System.Reflection;

namespace GN
{
    public class OpcodeTypeComponent : Component
    {
        private readonly DoubleMap<ushort, Type> opcodeTypes = new DoubleMap<ushort, Type>();
        void Awake()
        {
            var types = Game.EventSystem.GetTypes();// DllHelper.GetMonoTypes(typeof(Game).Assembly, DllHelper.GetLogicAssembly());
            foreach (var type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(MessageAttribute), false);
                if(attrs.Length == 0)
                {
                    continue;
                }

                MessageAttribute messageAttribute = attrs[0] as MessageAttribute;
                if(messageAttribute == null)
                {
                    continue;
                }

                this.opcodeTypes.Add(messageAttribute.Opcode, type);
            }
        }
        public ushort GetOpcode(Type type)
        {
            return this.opcodeTypes.GetKeyByValue(type);
        }

        public Type GetType(ushort opcode)
        {
            return this.opcodeTypes.GetValueByKey(opcode);
        }

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}
