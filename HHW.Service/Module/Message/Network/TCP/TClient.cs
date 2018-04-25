using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HHW.Service
{
    public class TClient : AClient
    {
        private readonly TcpClient tcpClient;

        private readonly CircularBuffer recvBuffer = new CircularBuffer();
        private readonly CircularBuffer sendBuffer = new CircularBuffer();

        private bool isSending;
        private readonly PacketParser parser;
        private bool isConnected;
        private TaskCompletionSource<Packet> recvTcs;

        public TClient(TcpClient tcpClient, TServer server) 
            : base(server, ClientType.Accept)
        {
            this.tcpClient = tcpClient;
            this.parser = new PacketParser(this.recvBuffer);

            IPEndPoint iPEndPoint = (IPEndPoint)this.tcpClient.Client.RemoteEndPoint;
            this.RemoteAddress = iPEndPoint;
            this.OnAccepted();
        }
        public TClient(TcpClient tcpClient, IPEndPoint iPEndPoint, TServer server)
            : base(server, ClientType.Connect)
        {
            this.tcpClient = tcpClient;
            this.parser = new PacketParser(this.recvBuffer);
            this.RemoteAddress = iPEndPoint;

            this.ConnectAsync(iPEndPoint);
        }

        private async void ConnectAsync(IPEndPoint ipEndPoint)
        {
            try
            {
                await this.tcpClient.ConnectAsync(ipEndPoint.Address, ipEndPoint.Port);

                this.isConnected = true;
                this.StartSend();
                this.StartRecv();
            }
            catch(SocketException e)
            {
                this.OnError(e.SocketErrorCode);
            }
            catch(Exception)
            {
                this.OnError(SocketError.SocketError);
            }
        }

        public override void Dispose()
        {
            if(this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.tcpClient.Close();
        }

        public void OnAccepted()
        {
            this.isConnected = true;
            this.StartSend();
            this.StartRecv();
        }
        public override Task<Packet> Recv()
        {
            if(this.IsDisposed)
            {
                throw new Exception("TClient已经被Dispose, 不能接收消息");
            }

            bool isOK = this.parser.Parse();
            if(isOK)
            {
                Packet packet = this.parser.GetPacket();
                return Task.FromResult(packet);
            }

            recvTcs = new TaskCompletionSource<Packet>();
            return recvTcs.Task;
        }

        public override void Send(byte[] buffer, int index, int length)
        {
            if(this.IsDisposed)
            {
                throw new Exception("TClient已经被Dispose, 不能发送消息");
            }

            byte[] size = BitConverter.GetBytes((ushort)buffer.Length);
            this.sendBuffer.Write(size, 0, size.Length);
            this.sendBuffer.Write(buffer, index, length);
            if(this.isConnected)
            {
                this.StartSend();
            }
        }

        public override void Send(List<byte[]> buffers)
        {
            if (this.IsDisposed)
            {
                throw new Exception("TClient已经被Dispose, 不能发送消息");
            }

            ushort size = (ushort)buffers.Select(c => c.Length).Sum();
            byte[] sizeBuffer = BitConverter.GetBytes(size);
            this.sendBuffer.Write(sizeBuffer, 0, sizeBuffer.Length);
            foreach (byte[] buffer in buffers)
            {
                this.sendBuffer.Write(buffer, 0, buffer.Length);
            }
            if(this.isConnected)
            {
                this.StartSend();
            }
        }

        private async void StartSend()
        {
            try
            {
                if(this.IsDisposed)
                {
                    return;
                }

                if(this.isSending)
                {
                    return;
                }

                while(true)
                {
                    if(this.IsDisposed)
                    {
                        return;
                    }

                    long bufferLength = this.sendBuffer.Length;
                    if(bufferLength == 0)
                    {
                        this.isSending = false;
                        return;
                    }

                    this.isSending = true;

                    NetworkStream stream = this.tcpClient.GetStream();
                    if(!stream.CanWrite)
                    {
                        return;
                    }

                    await this.sendBuffer.ReadAsync(stream);
                }
            }
            catch(IOException)
            {
                this.OnError(SocketError.SocketError);
            }
            catch (ObjectDisposedException)
            {
                this.OnError(SocketError.SocketError);
            }
            catch (Exception)
            {
                this.OnError(SocketError.SocketError);
            }
        }

        private async void StartRecv()
        {
            try
            {
                while(true)
                {
                    if(this.IsDisposed)
                    {
                        return;
                    }

                    NetworkStream stream = this.tcpClient.GetStream();
                    if(!stream.CanRead)
                    {
                        return;
                    }

                    int n = await this.recvBuffer.WriteAsync(stream);
                    if(n == 0)
                    {
                        this.OnError(SocketError.NetworkReset);
                        return;
                    }

                    if(this.recvTcs != null)
                    {
                        bool isOK = this.parser.Parse();
                        if(isOK)
                        {
                            Packet packet = this.parser.GetPacket();

                            var tcs = this.recvTcs;
                            this.recvTcs = null;
                            tcs.SetResult(packet);
                        }
                    }
                }
            }
            catch (IOException)
            {
                this.OnError(SocketError.SocketError);
            }
            catch (ObjectDisposedException)
            {
                this.OnError(SocketError.SocketError);
            }
            catch (Exception)
            {
                this.OnError(SocketError.SocketError);
            }
        }
    }
}
