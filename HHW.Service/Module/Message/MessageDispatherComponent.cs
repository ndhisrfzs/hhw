using System;
using System.Collections.Generic;

namespace HHW.Service
{
    public class MessageDispatherComponent : Component
    {
        private readonly Dictionary<ushort, List<IMHandler>> handlers = new Dictionary<ushort, List<IMHandler>>();

        public void Awake()
        {
            this.handlers.Clear();

            Type[] types = DllHelper.GetMonoTypes(DllHelper.GetLogicAssembly());
            foreach (Type type in types)
            {
                if (type.IsInterface || type.IsAbstract || !typeof(IMHandler).IsAssignableFrom(type))
                {
                    continue;
                }

                IMHandler iMHandler = Activator.CreateInstance(type) as IMHandler;
                if (iMHandler == null)
                {
                    continue;
                }

                Type messageType = iMHandler.GetMessageType();
                ushort opcode = this.Parent.GetComponent<OpcodeTypeComponent>().GetOpcode(messageType);
                if(opcode == 0)
                {
                    continue;
                }
                this.RegisterHandler(opcode, iMHandler);
            }
        }

        public void RegisterHandler(ushort opcode, IMHandler handler)
        {
            if(!this.handlers.ContainsKey(opcode))
            {
                this.handlers.Add(opcode, new List<IMHandler>());
            }
            this.handlers[opcode].Add(handler);
        }

        public void Handle(Session session, MessageInfo messageInfo)
        {
            List<IMHandler> actions;
            if(!this.handlers.TryGetValue(messageInfo.Opcode, out actions))
            {
                return;
            }

            foreach (IMHandler ev in actions)
            {
                try
                {
                    ev.Handle(session, messageInfo.Message);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
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
