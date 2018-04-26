using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HHW.Service
{
    public class SessionAwakeSystem : AwakeSystem<Session, NetworkComponent, AClient>
    {
        public override void Awake(Session self, NetworkComponent a, AClient b)
        {
            self.Awake(a, b);
        }
    }
    public class SessionStartSystem : StartSystem<Session>
    {
        public override void Start(Session self)
        {
            self.Start();
        }
    }
    public sealed class Session : Entity
    {
        private static int RpcId { get; set; }
        private AClient client;

        private readonly Dictionary<int, Action<IResponse>> requestCallback = new Dictionary<int, Action<IResponse>>();
        private readonly List<byte[]> byteses = new List<byte[]> { new byte[1], new byte[0], new byte[0] };

        public NetworkComponent Network { get; set; }

        public void Awake(NetworkComponent net, AClient client)
        {
            this.client = client;
            this.requestCallback.Clear();
        }

        public void Start()
        {
            this.StartRecv();
        }

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            long id = this.id;

            base.Dispose();

            foreach (Action<IResponse> action in this.requestCallback.Values.ToArray())
            {
                action.Invoke(new ResponseMessage { Error = ErrorCode.ERR_SocketDisconnected });
            }

            this.client.Dispose();
            this.Network.Remove(id);
            this.requestCallback.Clear();
        }

        public IPEndPoint RemoteAddress
        {
            get
            {
                return this.client.RemoteAddress;
            }
        }

        public ClientType ClientType
        {
            get
            {
                return this.client.ClientType;
            }
        }

        private async void StartRecv()
        {
            while(true)
            {
                if(this.IsDisposed)
                {
                    return;
                }

                Packet packet;
                try
                {
                    packet = await this.client.Recv();
                    if(this.IsDisposed)
                    {
                        return;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    continue;
                }

                try
                {
                    this.Execute(packet);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private void Execute(Packet packet)
        {
            if(packet.Length < Packet.MinSize)
            {
                this.Network.Remove(this.id);
                return;
            }

            byte flag = packet.Flag;
            ushort opcode = packet.Opcode;

            //flag为0表示rpc请求,1表示rpc返回消息
            if((flag & 0x01) == 0)
            {
                this.Network.MessageDispatcher.Dispatch(this, packet);
                return;
            }

            OpcodeTypeComponent opcodeTypeComponent = this.Network.Parent.GetComponent<OpcodeTypeComponent>();
            Type responseType = opcodeTypeComponent.GetType(opcode);
            object message = this.Network.MessagePacker.DeserializeFrom(responseType, packet.Bytes, Packet.Index, packet.Length - Packet.Index);

            IResponse response = message as IResponse;
            if(response == null)
            {
                throw new Exception($"flag is response, but message is not! {opcode}");
            }

            Action<IResponse> action;
            if(!this.requestCallback.TryGetValue(response.RpcId, out action))
            {
                return;
            }

            this.requestCallback.Remove(response.RpcId);

            action(response);
        }

        public Task<IResponse> Call(IRequest request)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                try
                {
                    if (response.Error > ErrorCode.ERR_Exception)
                    {
                        throw new Exception(response.Message);
                    }

                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            };

            request.RpcId = rpcId;
            this.Send(0x00, request);
            return tcs.Task;
        }

        public Task<IResponse> Call(IResponse request, CancellationToken cancellationToken)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                try
                {
                    if (response.Error > ErrorCode.ERR_Exception)
                    {
                        throw new Exception(response.Message);
                    }

                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            };

            cancellationToken.Register(() => this.requestCallback.Remove(rpcId));

            request.RpcId = rpcId;
            this.Send(0x00, request);
            return tcs.Task;
        }

        public void Reply(IResponse message)
        {
            if(this.IsDisposed)
            {
                throw new Exception("session已经被Dispose了");
            }

            this.Send(0x01, message);
        }

        public void Send(IMessage message)
        {
            this.Send(0x00, message);
        }

        public void Send(byte flag, IMessage message)
        {
            OpcodeTypeComponent opcodeTypeComponent = this.Network.Parent.GetComponent<OpcodeTypeComponent>();
            ushort opcode = opcodeTypeComponent.GetOpcode(message.GetType());
            byte[] bytes = this.Network.MessagePacker.Serialize(message);
            Send(flag, opcode, bytes);
        }

        public void Send(byte flag, ushort opcode, byte[] bytes)
        {
            if(this.IsDisposed)
            {
                throw new Exception("session已经被Dispose了");
            }

            this.byteses[0][0] = flag;
            this.byteses[1] = BitConverter.GetBytes(opcode);
            this.byteses[2] = bytes;

            client.Send(byteses);
        }
    }
}
