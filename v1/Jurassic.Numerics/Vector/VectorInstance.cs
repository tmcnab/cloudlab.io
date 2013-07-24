
namespace Jurassic.Numerics
{
    using System;
    using Jurassic.Library;
    using MathNet.Numerics.LinearAlgebra.Double;

    [Serializable]
    public class VectorInstance : ObjectInstance
    {
        public Vector V { get; protected set; }

        #region Constructors

        public VectorInstance(ObjectInstance prototype, params object[] args)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.Init(args);
        }

        protected void Init(params object[] args)
        {
            switch (args.Length)
            {
                #region 1 Args: (int),(array),(Vector)
                case 1:
                    if (args[0] is int) {
                        this.V = new DenseVector((int)args[0]);
                        return;
                    }
                    else if (args[0] is ArrayInstance) {
                        this.V = new DenseVector(Utilities.ToDoubleArray((ArrayInstance)args[0]));
                        return;
                    }
                    else if (args[0] is Vector) {
                        this.V = (Vector)args[0];
                        return;
                    }
                    throw new ArgumentException();
                #endregion

                #region 2 Args: (int,string)
                case 2:
                    if (args[0] is int && args[1] is System.String)
                    {
                        switch ((string)args[1])
                        {
                            case "sparse": this.V = new SparseVector((int)args[0]); break;
                            case "dense": this.V = new DenseVector((int)args[0]); break;
                            default: throw new ArgumentOutOfRangeException("Error: Vector type must be 'sparse' or 'dense'");
                        }
                        return;
                    }
                    throw new ArgumentException();
                #endregion

                default:
                    this.V = null;
                    return;
            }
        }

        #endregion

        #region Properties
        
        // Documented
        [JSProperty]
        public int length {
            get {
                return this.V.Count;
            }
        }

        #endregion

        #region Instance Methods
        
        // Documented
        [JSFunction(Name="add")]
        public VectorInstance Add(object A)
        {
            if (A is VectorInstance)
                return new VectorInstance(this.Prototype, this.V.Add(((VectorInstance)A).V));
            else if (A is ArrayInstance)
                return new VectorInstance(this.Prototype, this.V.Add(Utilities.ToVector((ArrayInstance)A)));
            else if (A is double || A is int)
                return new VectorInstance(this.Prototype, this.V.Add(Convert.ToDouble(A)));
            else
                throw new JavaScriptException(this.Engine, "TypeError", "Input must be Numeric, Array or Vector", null);
        }

        // Documented
        [JSFunction(Name="clear")]
        public void Clear()
        {
            this.V.Clear();
        }

        // Documented
        [JSFunction(Name="clone")]
        public VectorInstance Clone()
        {
            return new VectorInstance(this.Prototype, this.V);
        }

        // Documented
        [JSFunction(Name = "div")]
        public VectorInstance Divide(object A)
        {
            if (A is VectorInstance)
                return new VectorInstance(this.Prototype, this.V.PointwiseDivide(((VectorInstance)A).V));
            else if (A is ArrayInstance)
                return new VectorInstance(this.Prototype, this.V.PointwiseDivide(Utilities.ToVector((ArrayInstance)A)));
            else if (A is double || A is int)
                return new VectorInstance(this.Prototype, this.V.Divide(Convert.ToDouble(A)));
            else
                throw new JavaScriptException(this.Engine, "TypeError", "Input must be Numeric, Array or Vector", null);
        }

        // Documented
        [JSFunction(Name = "dot")]
        public double DotProduct(object A)
        {
            if(A is VectorInstance)
                return this.V.DotProduct(((VectorInstance)A).V);
            else if (A is ArrayInstance)
                return this.V.DotProduct(Utilities.ToVector((ArrayInstance)A));
            else
                throw new JavaScriptException(this.Engine, "TypeError", "Input must be Array or Vector", null);
        }

        // Documented
        [JSFunction(Name = "mul")]
        public VectorInstance Multiply(object A)
        {
            if (A is VectorInstance)
                return new VectorInstance(this.Prototype, this.V.PointwiseMultiply(((VectorInstance)A).V));
            else if (A is ArrayInstance)
                return new VectorInstance(this.Prototype, this.V.PointwiseMultiply(Utilities.ToVector((ArrayInstance)A)));
            else if (A is double || A is int)
                return new VectorInstance(this.Prototype, this.V.Multiply(Convert.ToDouble(A)));
            else
                throw new JavaScriptException(this.Engine, "TypeError", "Input must be Numeric, Array or Vector", null);
        }

        // Documented
        [JSFunction(Name = "neg")]
        public VectorInstance Negate()
        {
            return new VectorInstance(this.Prototype, this.V.Negate());
        }

        // Documented
        [JSFunction(Name = "sub")]
        public VectorInstance Subtract(object A)
        {
            if (A is VectorInstance)
                return new VectorInstance(this.Prototype, this.V.Subtract(((VectorInstance)A).V));
            else if (A is ArrayInstance)
                return new VectorInstance(this.Prototype, this.V.Subtract(Utilities.ToVector((ArrayInstance)A)));
            else if (A is double || A is int)
                return new VectorInstance(this.Prototype, this.V.Subtract(Convert.ToDouble(A)));
            else
                throw new JavaScriptException(this.Engine, "TypeError", "Input must be Numeric, Array or Vector", null);
        }

        // Documented
        [JSFunction(Name="sum")]
        public double Sum()
        {
            return this.V.Sum();
        }

        // Documented
        [JSFunction(Name="toArray")]
        public ArrayInstance ToArray()
        {
            return Utilities.ToArrayInstance(this.Engine, this.V.ToArray());
        }

        // Documented
        [JSFunction(Name="toString")]
        public override string ToString()
        {
            return this.V.ToString();
        }

        // Documented
        [JSFunction]
        public double v(int x, object v)
        {
            if (v is double || v is int)
            {
                return this.V[x] = Convert.ToDouble(v);
            }
            else
            {
                return this.V[x];
            }
        }

        #endregion
    }
}