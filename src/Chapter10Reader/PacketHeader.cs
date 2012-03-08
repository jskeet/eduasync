using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chapter10Reader
{
    public sealed class PacketHeader
    {
        private readonly long filePosition;
        private readonly ushort channelId;
        private readonly uint packetLength;
        private readonly uint dataLength;
        private readonly byte dataTypeVersion;
        private readonly byte sequenceNumber;
        private readonly byte packetFlags;
        private readonly byte dataType;
        private readonly ulong relativeTimeCounter;

        public long FilePosition { get { return filePosition; } }
        public ushort ChannelId { get { return channelId; } }
        public uint PacketLength { get { return packetLength; } }
        public uint DataLength { get { return dataLength; } }
        public byte DataTypeVersion { get { return dataTypeVersion; } }
        public byte SequenceNumber { get { return sequenceNumber; } }
        public byte PacketFlags { get { return packetFlags; } }
        public byte DataType { get { return dataType; } }
        public ulong RelativeTimeCounter { get { return relativeTimeCounter; } }

        internal PacketHeader(int position)
        {
            this.position = position;
        }

        public PacketHeader(ushort channelId, uint packetLength, uint dataLength, byte dataTypeVersion,
            byte sequenceNumber, byte packetFlags, byte dataType, ulong relativeTimeCounter, long filePosition)
        {
            // TODO: Complete member initialization
            this.channelId = channelId;
            this.packetLength = packetLength;
            this.dataLength = dataLength;
            this.dataTypeVersion = dataTypeVersion;
            this.sequenceNumber = sequenceNumber;
            this.packetFlags = packetFlags;
            this.dataType = dataType;
            this.relativeTimeCounter = relativeTimeCounter;
            this.filePosition = filePosition;
        }

        internal static PacketHeader Parse(byte[] data, int index, long filePosition)
        {
            if (index + 24 < data.Length)
            {
                throw new ArgumentException("Packet header must is 24 bytes long; not enough data");
            }
            ushort syncPattern = BitConverter.ToUInt16(data, index + 0);
            if (syncPattern != 0xeb25)
            {
                throw new ArgumentException("Packet header must start with the sync pattern");
            }
            ushort channelId = BitConverter.ToUInt16(data, index + 2);
            uint packetLength = BitConverter.ToUInt32(data, index + 4);
            uint dataLength = BitConverter.ToUInt32(data, index + 8);
            byte dataTypeVersion = data[index + 12];
            byte sequenceNumber = data[index + 13];
            byte packetFlags = data[index + 14];
            byte dataType = data[index + 15];
            // TODO: Validate this...
            ulong relativeTimeCounter = BitConverter.ToUInt32(data, index + 16) + (BitConverter.ToUInt15(data, index + 20) << 32);            
            // Assume we've already validated the checksum...
            return new PacketHeader(channelId, packetLength, dataLength, dataTypeVersion, sequenceNumber,
                packetFlags, dataType, relativeTimeCounter, filePosition);
        }

        /// <summary>
        /// Checks a packet header's checksum to see whether this *looks* like a packet header.
        /// </summary>
        internal static bool CheckPacketHeaderChecksum(byte[] data, int index)
        {
            if (index + 24 < data.Length)
            {
                throw new ArgumentException("Packet header must is 24 bytes long; not enough data");
            }
            ushort computed = 0;
            for (int i = 0; i < 11; i++)
            {
                computed += BitConverter.ToUInt16(data, index + i * 2);
            }
            return computed == BitConverter.ToUInt16(data, index + 22);
        }
    }
}
