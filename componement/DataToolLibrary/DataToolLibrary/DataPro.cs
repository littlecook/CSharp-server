using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dzw
{
    namespace DataLevel
    {
        //底层数据数据序列化操作
        public class DataPro
        {
            public static void addData(byte[] dest, ref int index, int data)
            {
                dest[index++] = (Byte)(data >> 24);

                dest[index++] = (Byte)(data >> 16);
                dest[index++] = (Byte)(data >> 8);
                dest[index++] = (Byte)data;
            }

            public static void addData(byte[] dest, ref int index, byte data)
            {
                dest[index++] = data;
            }

            public static void addData(byte[] dest, ref int index, ushort data)
            {
                dest[index++] = (Byte)(data >> 8);
                dest[index++] = (Byte)(data);
            }
            public static void addData(byte[] dest, ref int index, char data)
            {
                addData(dest, ref index, (ushort)data);
            }
            public static void addData(byte[] dest, ref int index, byte[] source)
            {
                for (int i = 0; i < source.Length; i ++ )
                {
                    dest[index++] = source[i];
                }
            }
            public static void addData(byte[] dest, ref int index, String data)
            {
                byte[] stringBuffer = System.Text.Encoding.UTF8.GetBytes(data);
                int bufferLength = stringBuffer.Length;
                for (int i = 0; i < bufferLength; i++)
                {
                    addData(dest, ref index, stringBuffer[i]);
                }
            }

            public static byte getByte(byte[] source, ref int index)
            {
                return source[index++];
            }

            public static int getInt(byte[] source, ref int index)
            {
                int tempInt = 0;
                tempInt += (getByte(source, ref index) << 24);
                tempInt += (getByte(source, ref index) << 16);
                tempInt += (getByte(source, ref index) << 8);
                tempInt += (getByte(source, ref index));
                return tempInt;
            }

            public static void getByteArray(byte[] source, ref int index, byte[] dest)
            {
                for (int i = 0; i < dest.Length; i ++ )
                {
                    dest[i] = source[index++];
                }
            }
            public static String getString(byte[] source, ref int index)
            {
                byte stringLength = getByte(source, ref index);
                String tempString = System.Text.Encoding.UTF8.GetString(source, index, stringLength);
                index += stringLength;
                return tempString;
            }
        }
    }
}
