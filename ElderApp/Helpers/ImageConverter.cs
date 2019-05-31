using System;
using System.IO;

namespace ElderApp.Helpers
{
    public class ImageConverter
    {
        public static byte[] StreamToByteArray(Stream input)
        {
            using(MemoryStream ms = new MemoryStream())
            {

                input.CopyTo(ms);

                return ms.ToArray();

            }
        }


        public static Stream ByteArrayToStream(byte[] input)
        {
            using (MemoryStream ms = new MemoryStream(input))
            {
                return ms;
            }
        }


    }
}
