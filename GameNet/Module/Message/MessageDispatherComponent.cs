using System;
using System.Collections.Generic;

namespace GN
{
    public class MessageDispatherComponent : Component
    {
        public AppType AppType;
        private readonly Dictionary<ushort, IMHandler> handlers = new Dictionary<ushort, IMHandler>();
        private readonly Dictionary<ushort, AppType> appOpcodes = new Dictionary<ushort, AppType>();

        public void Awake(AppType appType)
        {
            this.AppType = appType;
            this.handlers.Clear();

            var types = Game.EventSystem.GetTypes();// DllHelper.GetMonoTypes(DllHelper.GetLogicAssembly());
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(MessageHandlerAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                IMHandler iMHandler = Activator.CreateInstance(type) as IMHandler;
                if (iMHandler == null)
                {
                    continue;
                }

                Type messageType = iMHandler.GetMessageType();
                ushort opcode = (this.Parent as Entity).GetComponent<OpcodeTypeComponent>().GetOpcode(messageType);
                if(opcode == 0)
                {
                    continue;
                }

                MessageHandlerAttribute messageHandlerAttribute = attrs[0] as MessageHandlerAttribute;
                this.AddOpcodeApp(opcode, messageHandlerAttribute.Type);
                if (!messageHandlerAttribute.Type.Is(this.AppType))
                {
                    continue;
                }
                this.RegisterHandler(opcode, iMHandler);
            }
        }

        private void RegisterHandler(ushort opcode, IMHandler handler)
        {
            if(!this.handlers.TryAdd(opcode, handler))
            {
                Log.Error("RegisterHandler Error repeat handler opcode:" + opcode);
            }
        }

        private void AddOpcodeApp(ushort opcode, AppType type)
        {
            if(!this.appOpcodes.TryAdd(opcode, type))
            {
                Log.Error("AddOpcodeApp Error repeat opcode:" + opcode);
            }
        }

        public AppType GetOpcodeApp(ushort opcode)
        {
            AppType type;
            this.appOpcodes.TryGetValue(opcode, out type);
            return type;
        }

        public void Handle(Session session, MessageInfo messageInfo)
        {
            IMHandler ev;
            if (!this.handlers.TryGetValue(messageInfo.Opcode, out ev))
            {
                return;
            }

            try
            {
                ev.Handle(session, messageInfo.RpcId, messageInfo.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
