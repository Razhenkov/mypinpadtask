using MyPinPad.Core.Exceptions;
using MyPinPad.Core.Extensions;

namespace MyPinPad.Core.EMV
{
    public class EMVParser: IEMVParser
    {
        public Dictionary<string, byte[]> Parse(string hex)
        {
            try
            {
                var result = new Dictionary<string, byte[]>();

                byte[] data = hex.HexToBytes();
                int index = 0;

                while (index < data.Length)
                {
                    // --- Parse Tag ---
                    var tagBytes = new List<byte> { data[index++] };

                    if ((tagBytes[0] & 0x1F) == 0x1F)
                    {
                        // Read until MSB == 0
                        while (index < data.Length)
                        {
                            byte next = data[index++];
                            tagBytes.Add(next);
                            if ((next & 0x80) == 0)
                                break;
                        }
                    }

                    string tagHex = BitConverter.ToString(tagBytes.ToArray()).Replace("-", "");

                    // --- Parse Length ---
                    if (index >= data.Length)
                        throw new InvalidOperationException($"Unexpected end of data after tag {tagHex}.");

                    byte lengthByte = data[index++];
                    int valueLength;

                    if ((lengthByte & 0x80) == 0)
                    {
                        valueLength = lengthByte;
                    }
                    else
                    {
                        int numBytes = lengthByte & 0x7F;
                        if (index + numBytes > data.Length)
                            throw new InvalidOperationException($"Invalid length field in tag {tagHex}.");

                        valueLength = 0;
                        for (int i = 0; i < numBytes; i++)
                            valueLength = (valueLength << 8) | data[index++];
                    }

                    if (index + valueLength > data.Length)
                        throw new InvalidOperationException($"Tag {tagHex} length {valueLength} exceeds data length.");

                    byte[] value = new byte[valueLength];
                    Buffer.BlockCopy(data, index, value, 0, valueLength);
                    index += valueLength;

                    result[tagHex] = value;
                }

                return result;
            }
            catch (Exception e)
            {
                throw new EMVParserException(hex, e);
            }
        }
    }
}
