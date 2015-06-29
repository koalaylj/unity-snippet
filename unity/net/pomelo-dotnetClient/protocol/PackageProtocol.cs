#define ADLER

using System;
using System.Text;
using UnityEngine;


namespace Pomelo.DotNetClient
{
    public class PackageProtocol
    {
        public const int HEADER_LENGTH = 4;

#if ADLER
        public const int ADLER_LENGTH = 4;
#endif

        public static byte[] encode(PackageType type, byte[] body)
        {
            int length = HEADER_LENGTH;

#if ADLER

            if (body == null || body.Length == 0)
            {
            }
            else
            {
                length += (body.Length + ADLER_LENGTH);
            }


#else
            length += body.Length;
#endif

            byte[] buf = new byte[length];

            int index = 0;
            int bodyLen = 0;

            if (body != null)
            {
                bodyLen = body.Length;
            }

            buf[index++] = Convert.ToByte(type);
            buf[index++] = Convert.ToByte(bodyLen >> 16 & 0xFF);
            buf[index++] = Convert.ToByte(bodyLen >> 8 & 0xFF);
            buf[index++] = Convert.ToByte(bodyLen & 0xFF);

#if ADLER
            if (body == null || body.Length == 0)
            {
                //buf[index++] = 0;
                //buf[index++] = 0;
                //buf[index++] = 0;
                //buf[index++] = 0;
            }
            else
            {
                int aInt = adler32(body);

                buf[index++] = Convert.ToByte(aInt & 0xFF);
                buf[index++] = Convert.ToByte(aInt >> 8 & 0xFF);
                buf[index++] = Convert.ToByte(aInt >> 16 & 0xFF);
                buf[index++] = Convert.ToByte(aInt >> 24 & 0xFF);
            }
#endif
            while (index < length)
            {
#if ADLER
                int len = index - HEADER_LENGTH;

                if (body == null || body.Length == 0)
                {
                }
                else
                {
                    len -= ADLER_LENGTH;
                }
#else
                int len = index - HEADER_LENGTH;
#endif
                buf[index] = body[len];
                index++;
            }

            //string str = "--->";
            //foreach (byte code in buf)
            //{
            //    str += Convert.ToString(code, 16) + " ";
            //}
            //Debug.Log("type:" + type.ToString() + "," + buf.Length + "," + bodyLen + ",buf:" + str);

            //if (body != null)
            //{
            //    if (ii == 0)
            //    {
            //        buf = new byte[] { 0x04 };
            //    }
            //    else if (ii == 1)
            //    {
            //        buf = new byte[] { 0x00, 0x00, 0x23 };
            //    }
            //    else if (ii == 2)
            //    {
            //        buf = new byte[] { 0xcf, 0x0c, 0x06, 0xcf };
            //    }
            //    else if (ii == 3)
            //    {
            //        buf = new byte[] { 0x00, 0x01, 0x1e, 0x67, 0x61, 0x74, 0x65, 0x2e, 0x67, 0x61, 0x74, 0x65, 0x48, 0x61, 0x6e, 0x64, 0x6c, 0x65, 0x72, 0x2e, 0x72, 0x65, 0x71, 0x53, 0x65, 0x72, 0x76, 0x65, 0x72, 0x4c, 0x69, 0x73, 0x74, 0x7b, 0x7d };
            //    }
            //    else
            //    {
            //        return null;
            //    }

            //    ii++;

            //    Log.Info("send:" + buf.Length);
            //}

            return buf;
        }

        static int ii = 0;

        public static int adler32(byte[] data)
        {
            int a = 1, b = 0;
            int MOD_ADLER = 65521;
            for (int index = 0; index < data.Length; ++index)
            {
                a = (a + data[index]) % MOD_ADLER;
                b = (b + a) % MOD_ADLER;
            }
            return (b << 16) | a;
        }

        public static Package decode(byte[] buf)
        {
            PackageType type = (PackageType)buf[0];

            byte[] body = new byte[buf.Length - HEADER_LENGTH];

            for (int i = 0; i < body.Length; i++)
            {
                body[i] = buf[i + HEADER_LENGTH];
            }

            return new Package(type, body);
        }
    }
}