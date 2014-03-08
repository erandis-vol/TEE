/*
 * Copyright 2014 eien no itari
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at:
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;

namespace TEE
{
    public class GBAFile
    {
        private string path; // file path
        private FileStream fs; // stream for the file
        private bool open;

        #region Construction

        public GBAFile(string file)
        {
            path = file;

            open = false;
            Open();
        }

        ~GBAFile()
        {
            if (open) Close(); // Takes care of disposal.
        }

        /// <summary>
        /// Opens the file for IO. Can be called again if Close() was used.
        /// </summary>
        public void Open()
        {
            if (open) Close(); // Closess fs if already being used.

            fs = File.Open(path, FileMode.Open,
                FileAccess.ReadWrite, FileShare.ReadWrite);
            open = true;
        }

        /// <summary>
        /// Closes the file.
        /// </summary>
        public void Close()
        {
            open = false;
            fs.Close();
            fs.Dispose();
        }

        #endregion

        public void Seek(uint offset)
        {
            fs.Seek(offset, SeekOrigin.Begin);
        }

        public void Skip(uint offset)
        {
            fs.Seek(offset, SeekOrigin.Current);
        }

        #region Read

        public byte ReadByte()
        {
            return (byte)fs.ReadByte();
        }

        public ushort ReadWord()
        {
            byte[] b = new byte[2];
            fs.Read(b, 0, 2);
            return BitConverter.ToUInt16(b, 0);
        }

        public uint ReadDWord()
        {
            byte[] b = new byte[4];
            fs.Read(b, 0, 4);
            return BitConverter.ToUInt32(b, 0);
        }

        public ulong ReadQWord()
        {
            byte[] b = new byte[8];
            fs.Read(b, 0, 8);
            return BitConverter.ToUInt64(b, 0);
        }

        public byte[] ReadBytes(int count)
        {
            byte[] b = new byte[count];
            for (int i = 0; i < count; i++)
                b[i] = ReadByte();
            return b;
        }

        public string ReadString(int length)
        {
            string result = ""; byte[] b = ReadBytes(length);
            for (int i = 0; i < length; i++)
            {
                if (b[i] != 0) result += (char)b[i];
                else break;
            }
            return result;
        }

        public byte[] ReadLz77Bytes(out uint DataLength)
        {
            uint StartOffset = Position;
            byte[] data = ReadBytes(4);

            uint Offset = Position;

            if (data[0] == 0x10)
            {
                DataLength = BitConverter.ToUInt32(new Byte[] { data[1], data[2], data[3], 0x0 }, 0);
                data = new Byte[DataLength];

                Offset += 4;

                string watch = "";
                int i = 0;
                byte pos = 8;

                while (i < DataLength)
                {
                    Seek(Offset);
                    if (pos != 8)
                    {
                        if (watch[pos] == "0"[0])
                        {
                            data[i] = ReadByte();
                        }
                        else
                        {
                            byte[] r = ReadBytes(2);
                            int length = r[0] >> 4;
                            int start = ((r[0] - ((r[0] >> 4) << 4)) << 8) + r[1];
                            AmmendArray(ref data, ref i, i - start - 1, length + 3);
                            Offset++;
                        }
                        Offset++;
                        i++;
                        pos++;

                    }
                    else
                    {
                        watch = Convert.ToString(ReadByte(), 2);
                        while (watch.Length != 8)
                        {
                            watch = "0" + watch;
                        }
                        Offset++;
                        pos = 0;
                    }
                }
                DataLength = Offset - StartOffset;

                return data;
            }
            else
            {
                throw new Exception("This data is not Lz77 compressed!");
            }
        }

        private void AmmendArray(ref byte[] Bytes, ref int Index, int Start, int Length)
        {
            int a = 0; // Act
            int r = 0; // Rel

            byte Backup = 0;

            if (Index > 0)
            {
                Backup = Bytes[Index - 1];
            }

            while (a != Length)
            {
                if (Index + r >= 0 && Start + r >= 0 && Index + a < Bytes.Length)
                {
                    if (Start + r >= Index)
                    {
                        r = 0;
                        Bytes[Index + a] = Bytes[Start + r];
                    }
                    else
                    {
                        Bytes[Index + a] = Bytes[Start + r];
                        Backup = Bytes[Index + r];
                    }
                }
                a++;
                r++;
            }

            Index += Length - 1;
        }

        // Pointers

        public uint ReadPointer()
        {
            return (uint)(ReadDWord() - 0x8000000);
        }

        #region Peek

        public byte PeekByte()
        {
            uint pos = Position;
            byte b = ReadByte();
            Position = pos;
            return b;
        }

        public ushort PeekWord()
        {
            uint pos = Position;
            ushort b = ReadWord();
            Position = pos;
            return b;
        }

        public uint PeekDWord()
        {
            uint pos = Position;
            uint b = ReadDWord();
            Position = pos;
            return b;
        }

        public ulong PeekQWord()
        {
            uint pos = Position;
            ulong b = ReadQWord();
            Position = pos;
            return b;
        }

        #endregion

        #endregion

        #region Write

        public void WriteByte(byte b)
        {
            fs.WriteByte(b);
        }

        public void WriteWord(ushort u)
        {
            byte[] b = BitConverter.GetBytes(u);
            fs.Write(b, 0, 2);
        }

        public void WriteDWord(uint u)
        {
            byte[] b = BitConverter.GetBytes(u);
            fs.Write(b, 0, 4);
        }

        public void WriteBytes(byte[] b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                WriteByte(b[i]);
            }
        }

        public void WritePointer(uint offset)
        {
            uint actual = (uint)(0x8000000 + offset);
            WriteDWord(actual);
        }

        #endregion

        #region Find

        public int FindFreeSpace(int length, byte fsByte = 0xFF)
        {
            byte[] search = new byte[length];
            for (int i = 0; i < length; i++) search[i] = fsByte;
            
            int offset = FindBytes(search, 0);
            return offset;
        }

        public int FindBytes(byte[] search, uint start = 0)
        {
            uint iPos = Position;
            Seek(0);
            byte[] buffer = ReadBytes(Length);
            Position = iPos;

            // safe-ify
            int me = (int)(start - (start % 4));

            int offset = -1;
            bool found = false;
            while (!(me == buffer.Length - search.Length | offset != -1 | me == buffer.Length))
            {
                if (buffer[me] == search[0] & buffer[me + 1] == search[1])
                {
                    found = true;
                    int pos = 0;
                    while (!(pos == search.Length || found == false))
                    {
                        if (buffer[me + pos] != search[pos])
                        {
                            found = false;
                        }
                        pos += 1;
                    }

                    if (found)
                    {
                        offset = me;
                    }
                    else
                    {
                        offset = -1;
                    }
                }
                me += 4;
            }

            return offset;
        }

        #endregion

        public string Path
        {
            get { return path; }
        }

        public uint Position
        {
            get { return (uint)fs.Position; }
            set { fs.Position = value; }
        }

        public int Length
        {
            get { return (int)fs.Length; }
        }
    }
}
