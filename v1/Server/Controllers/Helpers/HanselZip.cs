using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Web;
using Server.Models;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server.Controllers.Helpers
{
    public static class Zip
    {
        public static readonly ZipSessionInternal Session = new ZipSessionInternal();

        public class ZipSessionInternal
        {
            public DataProcessor this[string index]
            {
                get
                {
                    return GZipHelpers.DeCompress(HttpContext.Current.Session["zip" + index] as byte[]);
                }
                set
                {
                    HttpContext.Current.Session["zip" + index] = GZipHelpers.Compress(value);
                }
            }
        }

        public static class GZipHelpers
        {
            public static DataProcessor DeCompress(byte[] unsquishMe)
            {
                using (MemoryStream mem = new MemoryStream(unsquishMe))
                {
                    using (GZipStream gz = new GZipStream(mem, CompressionMode.Decompress))
                    {
                        var sr = new StreamReader(gz);
                        BinaryFormatter formatter = new BinaryFormatter();
                        return (DataProcessor)formatter.Deserialize(gz);
                        //return sr.ReadToEnd();
                    }
                }
            }

            public static byte[] Compress(DataProcessor squishMe)
            {
                byte[] compressedBuffer = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
                    {
                        (new BinaryFormatter()).Serialize(zip, squishMe);
                    }
                    compressedBuffer = stream.ToArray();
                    //Trace.WriteLine("GZipHelper: Size Out:" + compressedBuffer.Length);
                }
                return compressedBuffer;
            }
        }
    }
}