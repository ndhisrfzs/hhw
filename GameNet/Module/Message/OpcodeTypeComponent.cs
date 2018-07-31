using System;
using System.Reflection;

namespace GN
{
    public class OpcodeTypeComponent : Component
    {
        private readonly DoubleMap<ushort, Type> RequestTypes = new DoubleMap<ushort, Type>();
        private readonly DoubleMap<ushort, Type> ResponseTypes = new DoubleMap<ushort, Type>();
        void Awake()
        {
            var types = Game.EventSystem.GetTypes();
            foreach (var type in types)
            {
                if (!type.IsInterface && !type.IsAbstract && typeof(IMessage).IsAssignableFrom(type))
                {
                    object[] attrs = type.GetCustomAttributes(typeof(MessageAttribute), false);
                    if (attrs.Length == 0)
                    {
                        if (type.IsNested && type.DeclaringType != null)
                        {
                            attrs = type.DeclaringType.GetCustomAttributes(typeof(MessageAttribute), false);
                            if (attrs.Length == 0)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    MessageAttribute messageAttribute = attrs[0] as MessageAttribute;
                    if (messageAttribute == null)
                    {
                        continue;
                    }

                    if (typeof(IRequest).IsAssignableFrom(type))
                    {
                        this.RequestTypes.Add(messageAttribute.Opcode, type);
                    }
                    else if(typeof(IResponse).IsAssignableFrom(type))
                    {
                        this.ResponseTypes.Add(messageAttribute.Opcode, type);
                    }
                }
            }
        }
        public ushort GetOpcode(Type type)
        {
            ushort key = 0;
            key = this.RequestTypes.GetKeyByValue(type);
            if (key == default(ushort))
            {
                key = this.ResponseTypes.GetKeyByValue(type);
            }
            return key;
        }

        public Type GetRequestType(ushort opcode)
        {
            return this.RequestTypes.GetValueByKey(opcode);
        }

        public Type GetResponseType(ushort opcode)
        {
            return this.ResponseTypes.GetValueByKey(opcode);
        }

        public bool HasResponse(ushort opcode)
        {
            return this.ResponseTypes.ContainsKey(opcode);
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
