using System;

namespace GN
{
    internal enum ParserState
    {
        PacketSize,
        PacketBody
    }
    public struct Packet
    {
        public const int FlagIndex = 0;
        public const int OpcodeIndex = 1;
        public const int RpcIdIndex = 3;
        public const int MinSize = 2;
        public const int Index = 7;

        public byte[] Bytes { get; }

        public ushort Length { get; set; }
        
        public Packet(int length)
        {
            this.Length = 0;
            this.Bytes = new byte[length];
        }

        public Packet(byte[] bytes)
        {
            this.Bytes = bytes;
            this.Length = (ushort)bytes.Length;
        }

        public byte Flag
        {
            get
            {
                return this.Bytes[FlagIndex];
            }
        }

        public ushort Opcode
        {
            get
            {
                return BitConverter.ToUInt16(this.Bytes, OpcodeIndex);
            }
        }
        public uint RpcId
        {
            get
            {
                return BitConverter.ToUInt32(this.Bytes, RpcIdIndex);
            }
        }
    }

    public class PacketParser
    {
        private readonly CircularBuffer buffer;

        private ushort packetSize;
        private ParserState state;
        private Packet packet = new Packet(ushort.MaxValue);
        private bool isOK;

        public PacketParser(CircularBuffer buffer)
        {
            this.buffer = buffer;
        }

        public bool Parse()
        {
            if(this.isOK)
            {
                return true;
            }

            bool finish = false;
            while(!finish)
            {
                switch(this.state)
                {
                    case ParserState.PacketSize:
                        {
                            if(this.buffer.Length < 2)
                            {
                                finish = true;
                            }
                            else
                            {
                                this.buffer.Read(this.packet.Bytes, 0, 2);
                                this.packetSize = BitConverter.ToUInt16(this.packet.Bytes, 0);
                                if(packetSize > 60000)
                                {
                                    throw new Exception($"packet too large, size: {this.packetSize}");
                                }
                                this.state = ParserState.PacketBody;
                            }
                        }
                        break;
                    case ParserState.PacketBody:
                        {
                            if(this.buffer.Length < this.packetSize)
                            {
                                finish = true;
                            }
                            else
                            {
                                this.buffer.Read(this.packet.Bytes, 0, this.packetSize);
                                this.packet.Length = this.packetSize;
                                this.isOK = true;
                                this.state = ParserState.PacketSize;
                                finish = true;
                            }
                        }
                        break;
                }
            }

            return this.isOK;
        }

        public Packet GetPacket()
        {
            this.isOK = false;
            return this.packet;
        }
    }
}
