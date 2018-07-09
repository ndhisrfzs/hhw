using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GN
{
    public sealed class Session : Entity
    {
        private static uint RpcId { get; set; }
        private AClient client;
        public SocketError error;

        private readonly Dictionary<uint, Action<IResponse>> requestCallback = new Dictionary<uint, Action<IResponse>>();
        private readonly List<byte[]> byteses = new List<byte[]> { new byte[1], new byte[0], new byte[0], new byte[0] };

        public NetworkComponent Network
        {
            get
            {
                return (NetworkComponent)this.Parent;
            }
        }

        public void Awake(AClient client)
        {
            this.client = client;
            this.requestCallback.Clear();
            this.client.ErrorCallback += (c, e) =>
            {
                this.error = e;
                this.Network.Remove(id);
            };
            this.client.ReadCallback += this.OnRead;
            this.client.Start();
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
                action.Invoke(new MessageResponse { Error = ErrorCode.ERR_SocketDisconnected });
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

        public void OnRead(Packet packet)
        {
            try
            {
                this.Execute(packet);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
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
            uint rpcId = packet.RpcId;

            if((flag & 0x01) == 0)
            {
                //flag为0表示rpc请求
                this.Network.MessageDispatcher.Dispatch(this, packet);
                return;
            }

            //flag为1表示rpc返回消息
            OpcodeTypeComponent opcodeTypeComponent = (this.Network.Parent as Entity).GetComponent<OpcodeTypeComponent>();
            Type responseType = opcodeTypeComponent.GetResponseType(opcode);
            object message = this.Network.MessagePacker.DeserializeFrom(responseType, packet.Bytes, Packet.Index, packet.Length);

            IResponse response = message as IResponse;
            if(response == null)
            {
                throw new Exception($"flag is response, but message is not! {opcode}");
            }

            Action<IResponse> action;
            if(!this.requestCallback.TryGetValue(rpcId, out action))
            {
                return;
            }

            this.requestCallback.Remove(rpcId);

            action(response);
        }

        public Task<IResponse> Call(IRequest request)
        {
            uint rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                try
                {
                    if (response.Error > ErrorCode.ERR_Exception)
                    {
                        throw new RpcException(response.Error, response.Message);
                    }

                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(new Exception($"RPC Error:{request.GetType().FullName}", e));
                }
            };

            this.Send(0x00, rpcId, request);
            return tcs.Task;
        }

        public Task<IResponse> Call(IResponse request, CancellationToken cancellationToken)
        {
            uint rpcId = ++RpcId;
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

            this.Send(0x00, rpcId, request);
            return tcs.Task;
        }

        public void Reply(uint rpcId, IResponse message)
        {
            if(this.IsDisposed)
            {
                throw new Exception("session已经被Dispose了");
            }

            this.Send(0x01, rpcId, message);
        }

        public void Send(IMessage message)
        {
            this.Send(0x00, 0, message);
        }

        private void Send(byte flag, uint rpcId, IMessage message)
        {
            OpcodeTypeComponent opcodeTypeComponent = (this.Network.Parent as Entity).GetComponent<OpcodeTypeComponent>();
            ushort opcode = opcodeTypeComponent.GetOpcode(message.GetType());
            byte[] bytes = this.Network.MessagePacker.Serialize(message);
            Send(flag, opcode, rpcId, bytes);
        }

        private void Send(byte flag, ushort opcode, uint rpcId, byte[] bytes)
        {
            if(this.IsDisposed)
            {
                throw new Exception("session已经被Dispose了");
            }

            this.byteses[0][0] = flag;
            this.byteses[1] = BitConverter.GetBytes(opcode);
            this.byteses[2] = BitConverter.GetBytes(rpcId);
            this.byteses[3] = bytes;

            client.Send(byteses);
        }
    }
}
