using System;

namespace AsDebuggerExtension
{
    public class AsMemoryBlock
    {
        #region Private Fields
        private byte[] bytes_;
        private uint count_;
        #endregion Private Fields

        #region Public Methods
        public AsMemoryBlock(byte[] bytes, uint count)
        {
            bytes_ = bytes;
            count_ = count;
        }

        public int[] ConvertToIntArray()
        {
            int[] result = new int[bytes_.Length / 4];
            for (int i = 0; i < bytes_.Length; i += 4)
            {
                result[i / 4] = BitConverter.ToInt32(bytes_, i);
            }
            return result;
        }

        public int ConvertToInt()
        {
            return BitConverter.ToInt32(bytes_, 0);
        }

        public string ConvertToAsciiString()
        {
            return System.Text.Encoding.ASCII.GetString(bytes_);
        }

        public string ConvertToUnicodeString()
        {
            return System.Text.Encoding.Unicode.GetString(bytes_);
        }

        public string ConvertToNullTerminatedString()
        {
            int nullIndex = Array.FindIndex(bytes_, x => x == '\0');
            if (nullIndex != -1)
            {
                return System.Text.Encoding.ASCII.GetString(bytes_, 0, nullIndex);
            }
            else
            {
                return System.Text.Encoding.ASCII.GetString(bytes_);
            }
        }
        #endregion Public Methods
    }
}
