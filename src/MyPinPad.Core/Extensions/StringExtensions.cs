using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyPinPad.Core.Extensions
{
    public static class StringExtensions
    {
        public static byte[] HexToBytes(this string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string has odd length");
            
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

            return bytes;
        }
    }
}
