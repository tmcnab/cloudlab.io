
namespace Jurassic.Cloudlab
{
    using System;
    using Jurassic.Library;
    using System.Text;
    
    [Serializable]
    public class ByteStringInstance : ObjectInstance
    {
        #region Constructors

        public ByteStringInstance(ObjectInstance prototype, params object[] args) : base(prototype)
        {
            this.PopulateFunctions();
            switch (args.Length)
            {
                case 0: Initializer();
                    break;

                case 1:
                    if (args[0] is ByteStringInstance) { Initializer(args[0] as ByteStringInstance); break; }
                    if (args[0] is ByteArrayInstance)  { Initializer(args[0] as ByteArrayInstance);  break; }
                    if (args[0] is ArrayInstance)      { Initializer(args[0] as ArrayInstance);      break; }
                    if (args[0] is string)             { Initializer(args[0] as string);             break; }
                    if (args[0] is byte[])             { Initializer(args[0] as byte[]);             break; }
                    throw new JavaScriptException(this.Engine, "TypeError", "unsupported type in constructor");

                default:
                    if (args[0] is string && args[1] is string)
                    {
                        Initializer(args[0] as string, args[1] as string);
                        break;
                    }
                    else
                    {
                        throw new JavaScriptException(this.Engine, "TypeError", "unsupported type in constructor");
                    }
            }
        }

        private void Initializer()
        {
            this.Value = new byte[0];
        }

        private void Initializer(byte[] array)
        {
            this.Value = array;
        }

        private void Initializer(ByteStringInstance instance)
        {
            this.Value = new byte[instance.Value.Length];
            Array.Copy(instance.Value, this.Value, instance.Value.Length);
        }

        private void Initializer(ByteArrayInstance instance)
        {
            this.Value = new byte[instance.Value.Length];
            Array.Copy(instance.Value, this.Value, instance.Value.Length);
        }

        private void Initializer(ArrayInstance instance)
        {
            this.Value = new byte[instance.Length];
            var i = 0;
            foreach (var element in instance.ElementValues)
            {
                try   { this.Value[i] = (byte)element; }
                catch { throw new JavaScriptException(this.Engine, "RangeError", "input array elements must be bounded between 0 and 255"); }
            }
        }

        private void Initializer(string content, [DefaultParameterValue("utf-8")] string charset = "utf-8")
        {
            switch (charset.ToLowerInvariant())
            {
                case "utf-8":
                    this.Value = UTF8Encoding.UTF8.GetBytes(content);
                    break;

                case "us-ascii":
                    this.Value = ASCIIEncoding.ASCII.GetBytes(content);
                    break;

                default:
                    throw new JavaScriptException(this.Engine, "Error", "supplied charset is not supported");
            }
        }

        #endregion

        #region Members

        public byte[] Value;

        #endregion

        #region Properties

        [JSProperty(Name="length",IsEnumerable=false)]
        public int Length {
            get {
                return this.Value.Length;
            }
        }

        #endregion

        #region User-Visible Methods

        [JSFunction(Name = "toSource")]
        public string ToSource([DefaultParameterValue("")] string format = "")
        {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(format)) {
                foreach (var element in this.Value) {
                    builder.Append(element.ToString() + ',');
                }
            }
            else if (format == "hexadecimal") {
                foreach (var element in this.Value) {
                    builder.Append(element.ToString("X") + ',');
                }
            }
            else {
                throw new JavaScriptException(this.Engine, "Error", "unsupported format '" + format + "'");
            }

            return string.Format("ByteString([{0}])", builder.ToString().TrimEnd(','));
        }

        [JSFunction(Name="toString")]
        public override string ToString()
        {
            return string.Format("[ByteString {0}]", this.Value.Length);
        }

        #endregion

        #region Helpers


        #endregion
    }
}