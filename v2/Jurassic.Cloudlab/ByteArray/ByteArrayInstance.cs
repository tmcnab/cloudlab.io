
namespace Jurassic.Cloudlab
{
    using System;
    using Jurassic.Library;
    using Jurassic.Numerics;
    
    [Serializable]
    public class ByteArrayInstance : ObjectInstance
    {
        #region Constructors

        public ByteArrayInstance(ObjectInstance prototype, params object[] args) : base(prototype)
        {
            this.PopulateFunctions();
            this.Initialize(args);
        }

        private void Initialize(params object[] args)
        {
            if (args.Length == 0)
            {
                // ByteArray() - New, empty ByteArray.
                this.Value = new byte[0];
                return;
            }
            else if (args.Length == 1)
            {
                if (args[0] is int)
                {
                    // ByteArray(length) - New ByteArray filled with length zero bytes.
                    this.Value = new byte[(int)args[0]];
                    return;
                }
                else if (args[0] is ByteArrayInstance)
                {
                    // ByteArray(byteArray) - Copy byteArray.
                    this.Value = (byte[])((ByteArrayInstance)args[0]).Value.Clone();
                    return;
                }
                else if (args[0] is ByteStringInstance)
                {
                    // ByteArray(byteString) - Copy contents of byteString.
                    this.Value = (byte[])((ByteStringInstance)args[0]).Value.Clone();
                    return;
                }
                else if (args[0] is ArrayInstance)
                {
                    // ByteArray(arrayOfBytes) - Use numbers in arrayOfBytes as contents.
                    try { this.Value = ((ArrayInstance)args[0]).ToByteArray(); return; }
                    catch { throw new JavaScriptException(this.Engine, "RangeError", "array values must be in the range 0...255"); }
                }

                throw new ArgumentException();
            }
            else if (args.Length == 2)
            {
                if (args[0] is string && args[1] is string)
                {
                    // ByteArray(string, charset) - Create a ByteArray from a Javascript string, the result being encoded with charset.
                    throw new NotImplementedException();
                }
            }

            throw new ArgumentException();
        }

        #endregion

        #region Members

        public byte[] Value;

        #endregion

        #region Properties

        [JSProperty(Name="length",IsEnumerable=false)]
        public int Length {
            get {
                return this.Value == null ? 0 : this.Value.Length;
            }
            set
            {
                var newValue = new byte[value];
                Array.Copy(this.Value, newValue, value < this.Value.Length ? value : this.Value.Length);
                this.Value = newValue;
            }
        }

        #endregion

        #region User-Visible Methods

        // Documented
        [JSFunction(Name = "codeAt")]
        public object CodeAt(int index)
        {
            if (index < 0 || index >= this.Value.Length) return Undefined.Value;
            else return (int)this.Value[index];
        }

        // Documented
        [JSFunction(Name = "pop")]
        public object Pop()
        {
            if (this.Value.Length == 0)
            {
                return Undefined.Value;
            }
            else
            {
                var rVal = this.Value[this.Value.Length - 1];
                var newValue = new byte[this.Value.Length - 1];
                Array.Copy(this.Value, newValue, newValue.Length);
                this.Value = newValue;
                return rVal;
            }
        }

        // Documented
        [JSFunction(Name="toArray")]
        public ArrayInstance ToArrayInstance()
        {
            return this.Value.ToArrayInstance(this.Engine);
        }

        // Documented
        [JSFunction(Name = "toSource")]
        public string ToSource()
        {
            return string.Format("ByteArray([{0}])", this.Value.AsString());
        }

        // Documented
        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return string.Format("[ByteArray {0}]", this.Value.Length);
        }

        #endregion

        #region [] Getter/Setter

        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index)
        {
            if (index < 0 || index >= this.Value.Length)
            {
                return new PropertyDescriptor(Undefined.Value, PropertyAttributes.Sealed);
            }
            else
            {
                return new PropertyDescriptor((int)this.Value[index], PropertyAttributes.Sealed);
            }
        }

        public override void SetPropertyValue(uint index, object value, bool throwOnError)
        {
            if (value is int)
            {
                this.Value[index] = (byte)((int)value > byte.MaxValue ? byte.MaxValue : (int)value);
            }
            else
            {
                throw new JavaScriptException(this.Engine, "TypeError", "type must be integer Numeric between 0 and 255");
            }
        }


        #endregion

        #region Utilities


        #endregion
    }
}