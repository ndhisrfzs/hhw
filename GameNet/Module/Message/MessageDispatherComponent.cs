using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GN
{
    public class MessageDispatherComponent : Component
    {
        public AppType AppType;
        private readonly Dictionary<ushort, IMHandler> handlers = new Dictionary<ushort, IMHandler>();

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

                MessageHandlerAttribute messageHandlerAttribute = attrs[0] as MessageHandlerAttribute;
                if (!messageHandlerAttribute.Type.Is(this.AppType))
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
                
                this.RegisterHandler(opcode, iMHandler);
            }
        }

        private void RegisterHandler(ushort opcode, IMHandler handler)
        {
            if(this.handlers.ContainsKey(opcode))
            {
                Log.Error("RegisterHandler Error repeat handler opcode:" + opcode);
                return;
            }

            this.handlers.Add(opcode, handler);
        }

        public bool IsLocalHandler(ushort opcode)
        {
            return handlers.ContainsKey(opcode);
        }

        public async Task Handle(Session session, MessageInfo messageInfo)
        {
            IMHandler ev;
            if (!this.handlers.TryGetValue(messageInfo.Opcode, out ev))
            {
                return;
            }

            try
            {
                await ev.Handle(session, messageInfo.entity, messageInfo.RpcId, messageInfo.Message);
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
