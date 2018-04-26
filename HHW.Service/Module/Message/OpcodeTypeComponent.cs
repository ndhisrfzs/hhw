using System;

namespace HHW.Service
{
    public class OpcodeTypeComponentSystem : AwakeSystem<OpcodeTypeComponent>
    {
        public override void Awake(OpcodeTypeComponent self)
        {
            self.Awake();
        }
    }
    public class OpcodeTypeComponent : Component
    {
        public void Awake()
        {

        }

        public ushort GetOpcode(Type type)
        {
            return 0;
        }

        public Type GetType(ushort opcode)
        {
            return null;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
