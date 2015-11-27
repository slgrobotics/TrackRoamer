/*
* Copyright (c) 2011..., Sergei Grichine
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of Sergei Grichine nor the
*       names of contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY Sergei Grichine ''AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL Sergei Grichine BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*
* this is a X11 (BSD Revised) license - you do not have to publish your changes,
* although doing so, donating and contributing is always appreciated
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.ChrInterface
{
    // See UM6 datasheet for more info. Source code in C++ for working with UM6 is at http://sourceforge.net/projects/chrinterface/  (get SVN tarball there)
    // C:\Projects\Robotics\src\CHR Interface\CHR Interface\SerialPacket.h
    // C:\Projects\Robotics\src\CHR Interface\CHR Interface\SerialPacket.cpp

    public class SerialPacket
    {
        public const int MAX_DATA_LENGTH = (16 * 4);

        public byte Address;
        public byte PacketDescriptor;
        public UInt16 Checksum;

        // commands - see UM2B.XML and Datasheet p.56 "11.3. UM6 Command Registers":
        public const byte UM6_ZERO_GYROS = 0xAC;    // 172
        public const byte UM6_SET_ACCEL_REF = 0xAF; // 175
        public const byte UM6_SET_MAG_REF = 0xB0;   // 176

        /// <summary>
        /// Packet has data and is a batch. This means it contains ‘batch_length' registers, each
        /// of which has a length of 4 bytes:  data_length = 4*BatchLength
        /// </summary>
        public bool IsBatch
        {
            get
            {
                if ((PacketDescriptor & (byte)0x40) != (byte)0)
                    return true;
                else
                    return false;
            }

            set
            {
                if (value)
                {
                    PacketDescriptor |= 0x40;
                }
                else
                {
                    PacketDescriptor |= 0x40;
                    PacketDescriptor ^= 0x40;
                }
            }
        }

        /// <summary>
        /// May be a Batch or not.
        /// if Packet has data but is not a batch - it means it contains one register (4 bytes)
        /// if it is a batch, it contains ‘batch_length' registers, each of which has a length of 4 bytes
        /// </summary>
        public bool HasData
        {
            get
            {
                if ((PacketDescriptor & (byte)0x80) != (byte)0)
                    return true;
                else
                    return false;
            }

            set
            {
                if (value)
                {
                    PacketDescriptor |= 0x80;
                }
                else
                {
                    PacketDescriptor |= 0x80;
                    PacketDescriptor ^= 0x80;
                }
            }
        }

        public byte BatchLength
        {
            get
            {
                return (byte)((PacketDescriptor >> 2) & 0x0F);
            }
            set
            {
                value &= 0x0F;
                // Clear batch length bits
                PacketDescriptor |= (0x0F << 2);
                PacketDescriptor ^= (0x0F << 2);
                // Set batch length bits
                PacketDescriptor |= (byte)(value << 2);
            }
        }

        public byte CommandFailed
        {
            get
            {
                return (byte)(PacketDescriptor & 0x01);
            }
            set
            {
                value &= 0x01;
                PacketDescriptor |= 0x01;
                PacketDescriptor ^= 0x01;
                PacketDescriptor |= value;
            }
        }

        public int DataLength
        {
            get
            {
                if (HasData && IsBatch)
                {
                    return 4 * BatchLength;
                }
                if (HasData && !IsBatch)
                {
                    return 4;
                }

                return 0;
            }
        }

        public int PacketLength
        {
            get
            {
                return DataLength + 7;
            }
        }

        public void SetDataByte(int index, byte value)
        {
            Data[index] = value;
        }

        public byte GetDataByte(int index) { return Data[index]; }

        public void ComputeChecksum()
        {
            UInt16 checksum;

            checksum = 0;

            checksum += (byte)'s';
            checksum += (byte)'n';
            checksum += (byte)'p';
            checksum += PacketDescriptor;
            checksum += Address;

            for (int i = 0; i < DataLength; i++)
            {
                checksum += (ushort)(((int)Data[i]) & 0x00FF);
            }

            Checksum = checksum;
        }

        private byte[] Data;

        public SerialPacket(byte address = 0)
        {
            HasData = false;
            IsBatch = false;
            Address = address;
            BatchLength = 0;
            Checksum = 0;
            CommandFailed = 0;

            Data = new byte[MAX_DATA_LENGTH];
        }

        /// <summary>
        /// prepare packet to be transmitted to the UM6 device
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            ComputeChecksum();

            byte[] ret = new byte[BatchLength + 7];

            int i = 0;
            ret[i++] = (byte)'s';
            ret[i++] = (byte)'n';
            ret[i++] = (byte)'p';
            ret[i++] = PacketDescriptor;
            ret[i++] = Address;

            for (int j = 0; j < DataLength; j++)
            {
                ret[i++] = Data[j];
            }

            // last two bytes ([5] and [6] when no data) are for checksum's high and low bytes:
            ret[i++] = (byte)(Checksum >> 8);
            ret[i++] = (byte)(Checksum & 0xFF);

            return ret;
        }
    }
}

