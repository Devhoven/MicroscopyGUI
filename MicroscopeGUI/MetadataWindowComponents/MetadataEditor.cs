using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace MicroscopeGUI
{
    static class MetadataEditor
    {
        // Created through the help of https://dev.exiv2.org/projects/exiv2/wiki/The_Metadata_in_PNG_files
        public static unsafe void AddiTXt(FileStream Stream, string Key, string Text)
        {
            // If the stream was used before, it won't start at pos 0 and thus runs danger that the IDAT-chunk won't be found
            Stream.Position = 0;

            // Saving the old lenght, since a new one is going to be set and we still need it
            long OldLength = Stream.Length;

            // The second part of the png chunk, the "Chunk Type"
            // iTXt in this case, since we want to encode text
            byte[] ChunkType = Encoding.ASCII.GetBytes("iTXt");

            // Retreiving the byte representation of the key and the text, which are given
            byte[] KeyData = Encoding.UTF8.GetBytes(Key);
            byte[] TextData = Encoding.UTF8.GetBytes(Text);
            // Creating a new array, which combines the key and text to a new byte array and adds padding for the flags etc.
            byte[] Data = new byte[TextData.Length + KeyData.Length + 5];

            // The first few bytes of the Data-array are the bytes of the Key
            Array.Copy(KeyData, Data, KeyData.Length);
            // After that, 5 bytes are skipped for the flags etc. and then the text is added
            Array.Copy(TextData, 0, Data, KeyData.Length + 5, TextData.Length);

            // Retreiving the 4 byte representation of the length of the data
            byte[] Bytes = BitConverter.GetBytes(Data.Length);

            // Getting the position of the first IDAT chunk, so we can put the text before that
            int IDATPos = SearchForBytePattern(Stream, Encoding.ASCII.GetBytes("IDAT"));

            // Saving the end chunk, since they have to be moved to the end again 
            // + 4, because of the length part of the IDAT chunk
            byte[] Ending = new byte[OldLength - IDATPos + 4];
            Stream.Position = IDATPos - 4;
            Stream.Read(Ending, 0, Ending.Length);

            // Extending the array about the length of the chunk (12 bytes for the other 3 parts)
            Stream.SetLength(OldLength + Data.Length + 12);

            // Going to the start of the new chunk
            Stream.Position = IDATPos - 4;

            // Writing the length of the data part into the chunk
            // Reversed, since the most significant bit has to be on the left side
            for (int i = Bytes.Length - 1; i >= 0; i--)
                Stream.WriteByte(Bytes[i]);

            // CodeBlock is a combination of the Data and CodeBlock arrays
            // This is done, since the last part of the chunk is a CRC32-Hash of these two and we need the full array for that
            byte[] CodeBlock = new byte[Data.Length + ChunkType.Length];
            Array.Copy(ChunkType, CodeBlock, ChunkType.Length);
            Array.Copy(Data, 0, CodeBlock, ChunkType.Length, Data.Length);

            // Writing the two parts in the file
            Stream.Write(CodeBlock);

            // Retreiving the pointer to the array
            fixed (byte* CodeBlockPointer = CodeBlock)
            {
                // Retreiving the 4 byte representation of the CRC32 Hash of the CodeBlock array
                byte[] CRCHashByte = BitConverter.GetBytes(CRC32.crc32(0, CodeBlockPointer, CodeBlock.Length));

                // Writing it to the stream
                // Reversed for the same reason
                for (int i = CRCHashByte.Length - 1; i >= 0; i--)
                    Stream.WriteByte(CRCHashByte[i]);
            }

            // Writing the ending to the end of the stream again
            Stream.Write(Ending);
        }

        // Simple seraching algorithm for a byte pattern
        static int SearchForBytePattern(Stream Stream, byte[] Pattern)
        {
            long StreamEnd = Stream.Length - Pattern.Length + 1;

            for (int i = 0; i < StreamEnd; i++)
            {
                if (Stream.ReadByte() != Pattern[0]) 
                    continue;

                for (int j = 1; j < Pattern.Length; j++)
                {
                    if (Stream.ReadByte() != Pattern[j])
                    {
                        Stream.Position = i + 1;
                        break;
                    }
                    if (j == 1) 
                        return i;
                }
            }

            return -1;
        }

        // Filters all of the iTXt key - value pairs out of a stream, with the help of the BitmapMetadata-class
        public static Dictionary<string, string> GetValuePairs(FileStream Stream)
        {
            Dictionary<string, string> Values = new Dictionary<string, string>();

            BitmapSource img = BitmapFrame.Create(Stream);
            BitmapMetadata md = (BitmapMetadata)img.Metadata;

            object CurrentKey;
            object CurrentValue;
            int Index = 0;

            // If there are multiple iTXt chunks, they are numbered and can be filtered with and '[N]' in front of the iTXt
            // I am using this to go through all of the iTXt chunks 
            // If there are no more chunks, the GetQuery function is going to return null, which is the point I'm going to stop the search
            while (true)
            {
                CurrentKey = md.GetQuery("/[" + Index + "]iTXt/Keyword");
                if (CurrentKey is null)
                    break;

                CurrentValue = md.GetQuery("/[" + Index + "]iTXt/TextEntry");

                if (!Values.ContainsKey((string)CurrentKey))
                    Values.Add((string)CurrentKey, (string)CurrentValue);

                Index++;
            } 

            return Values;
        }

        // Stolen from https://gist.github.com/martin31821/6a4736521043233bf7cdc05aa785149d
        private static class CRC32
        {
            static readonly uint[] crc32_tab = {
            0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419, 0x706af48f,
            0xe963a535, 0x9e6495a3, 0x0edb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988,
            0x09b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91, 0x1db71064, 0x6ab020f2,
            0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
            0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9,
            0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172,
            0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b, 0x35b5a8fa, 0x42b2986c,
            0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
            0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423,
            0xcfba9599, 0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,
            0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 0x76dc4190, 0x01db7106,
            0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f, 0x9fbfe4a5, 0xe8b8d433,
            0x7807c9a2, 0x0f00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x086d3d2d,
            0x91646c97, 0xe6635c01, 0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e,
            0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950,
            0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
            0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7,
            0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0,
            0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9, 0x5005713c, 0x270241aa,
            0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
            0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81,
            0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6, 0x03b6e20c, 0x74b1d29a,
            0xead54739, 0x9dd277af, 0x04db2615, 0x73dc1683, 0xe3630b12, 0x94643b84,
            0x0d6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1,
            0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb,
            0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc,
            0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5, 0xd6d6a3e8, 0xa1d1937e,
            0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
            0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55,
            0x316e8eef, 0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
            0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe, 0xb2bd0b28,
            0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
            0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x026d930a, 0x9c0906a9, 0xeb0e363f,
            0x72076785, 0x05005713, 0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38,
            0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242,
            0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
            0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69,
            0x616bffd3, 0x166ccf45, 0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2,
            0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc,
            0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
            0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693,
            0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94,
            0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d
        };

            /// <summary>
            /// Calculate the crc32 checksum of the given memory block.
            /// </summary>
            /// <param name="crc">The start value for the crc</param>
            /// <param name="buf">Pointer to the memory block</param>
            /// <param name="size">Number of bytes</param>
            public static unsafe uint crc32(uint crc, byte* buf, int size)
            {
                crc = crc ^ ~0U; //0xFFFFFFFF
                while (size-- > 0)
                    crc = crc32_tab[(crc ^ *buf++) & 0xFF] ^ (crc >> 8);

                return crc ^ ~0U; //0xFFFFFFFF
            }
        }
    }
}
