
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

        [JSProperty(IsEnumerable=true)]
        new public double this[int index]
        {
            get { return this.V[index]; }
            set { this.V[index] = value; }
        }

        #endregion

        #region Instance Methods

        // Documented
        [JSFunction(Name="add")]
        public void Add(object A)
        {
            Utilities.Guard(this.Engine, A, new[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(double), typeof(int) });
            this.V = VectorInstance.Add(this, A).V;
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
        public void Divide(object A)
        {
            Utilities.Guard(this.Engine, A, new[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(double), typeof(int) });
            this.V = VectorInstance.Divide(this, A).V;
        }

        // Documented
        [JSFunction(Name = "dot")]
        public double DotProduct(object A)
        {
            Utilities.Guard(this.Engine, A, new Type[] { typeof(ArrayInstance), typeof(VectorInstance) });
            
            return VectorInstance.DotProduct(this.V, A.ToVector());
        }

        // Documented
        [JSFunction(Name = "mul")]
        public void Multiply(object A)
        {
            Utilities.Guard(this.Engine, A, new[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(double), typeof(int) });
            this.V = VectorInstance.Multiply(this, A).V;
        }

        // Documented
        [JSFunction(Name = "neg")]
        public void Negate()
        {
            this.V = (Vector)this.V.Negate();
        }
 
        // Documented
        [JSFunction(Name = "sub")]
        public void Subtract(object A)
        {
            Utilities.Guard(this.Engine, A, new[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(double), typeof(int) });
            this.V = VectorInstance.Subtract(this, A).V;
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
            return this.ToArrayInstance(this.Engine);
        }

        // Documented
        [JSFunction(Name="toString")]
        public override string ToString()
        {
            return string.Format("[Vector {0}]", this.V.Count);
        }

        // Documented
        [JSFunction(Name = "v")]
        public double GetterSetter(int x, object v)
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

        #region Backend Abstraction

        public static double DotProduct(object A, object B)
        {
            return A.ToVector().DotProduct(B.ToVector());
        }

        public static VectorInstance Multiply(VectorInstance A, object B)
        {
            if (B is double || B is int)
            {
                return new VectorInstance(A.Prototype, A.V.Multiply(Convert.ToDouble(B)));
            }
            else
            {
                return new VectorInstance(A.Prototype, A.V.PointwiseMultiply(B.ToVector()));
            }
        }

        public static VectorInstance Add(VectorInstance A, object B)
        {
            if (B is double || B is int)
            {
                return new VectorInstance(A.Prototype, A.V.Add(Convert.ToDouble(B)));
            }
            else
            {
                return new VectorInstance(A.Prototype, A.V.Add(B.ToVector()));
            }
        }

        public static VectorInstance Subtract(VectorInstance A, object B)
        {
            if (B is double || B is int)
            {
                return new VectorInstance(A.Prototype, A.V.Subtract(Convert.ToDouble(B)));
            }
            else
            {
                return new VectorInstance(A.Prototype, A.V.Subtract(B.ToVector()));
            }
        }

        public static VectorInstance Divide(VectorInstance A, object B)
        {
            if (B is double || B is int)
            {
                return new VectorInstance(A.Prototype, A.V.Divide(Convert.ToDouble(B)));
            }
            else
            {
                return new VectorInstance(A.Prototype, A.V.PointwiseDivide(B.ToVector()));
            }
        }

        #endregion
    }
}