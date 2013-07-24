namespace Jurassic.Numerics
{
    using System;
    using Jurassic.Library;
    using MathNet.Numerics.LinearAlgebra.Double;

    [Serializable]
    public class MatrixInstance : ObjectInstance
    {
        #region Members

        public Matrix M = null;

        #endregion

        #region Constructors & Initialization

        public MatrixInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.M = null;
        }

        public MatrixInstance(ObjectInstance prototype, params object[] args)
            : base(prototype)
        {
            this.PopulateFunctions();
            this.Init(args);
        }

        /// <summary>
        /// Initializes the instance using the parameters provided
        /// </summary>
        /// <param name="args">A variable-length list of arguments used to configure the instance</param>
        protected void Init(params object[] args)
        {
            switch (args.Length)
            {
                #region 0 Args
                case 0:
                    this.M = new DenseMatrix(3);
                    return;
                #endregion

                #region 1 Args: int or 2D array
                case 1:
                    if (args[0] is int)
                    {
                        this.M = new DenseMatrix((int)args[0]);
                        return;
                    }
                    else if (args[0] is ArrayInstance)
                    {
                        this.M = new DenseMatrix(Utilities.ToDoubleArray2D((ArrayInstance)args[0]));
                        return;
                    }
                    else if (args[0] is Matrix)
                    {
                        this.M = (Matrix)args[0];
                        return;

                    }
                    throw new ArgumentException();
                #endregion

                #region 2 Args: (int,int) , (int,string)
                case 2:
                    if (args[0] is int && args[1] is int)
                    {
                        this.M = new DenseMatrix((int)args[1], (int)args[0]);
                        return;
                    }
                    else if (args[0] is int && args[1] is System.String)
                    {
                        switch ((string)args[1])
                        {
                            case "sparse": this.M = new SparseMatrix((int)args[0]); break;
                            case "dense": this.M = new DenseMatrix((int)args[0]); break;
                            case "identity": this.M = new DiagonalMatrix((int)args[0], (int)args[0], 1); break;
                            default: throw new ArgumentOutOfRangeException("Error: Matrix type must be 'sparse', 'dense' or 'identity'");
                        }
                        return;
                    }
                    throw new ArgumentException();
                #endregion

                #region 3 Args: (int,int,string)
                case 3:
                    if (args[0] is int && args[1] is int && args[2] is string)
                    {
                        switch ((string)args[2])
                        {
                            case "sparse": this.M = new SparseMatrix((int)args[0], (int)args[1]); break;
                            case "dense": this.M = new DenseMatrix((int)args[0], (int)args[1]); break;
                            case "identity": this.M = new DiagonalMatrix((int)args[0], (int)args[1], 1); break;
                            default: throw new ArgumentOutOfRangeException("type", "Matrix type must be 'sparse', 'dense' or 'identity'");
                        }
                        return;
                    }
                    throw new ArgumentException();
                #endregion

                default:
                    throw new ArgumentException();
            }
        }

        #endregion

        #region Properties

        // Documented
        [JSProperty]
        public int rows
        {
            get
            {
                return this.M.RowCount;
            }
        }

        // Documented
        [JSProperty]
        public int cols
        {
            get
            {
                return this.M.ColumnCount;
            }
        }

        // 
        [JSProperty]
        public bool symmetric
        {
            get
            {
                return this.M.IsSymmetric;
            }
        }

        #endregion

        #region Instance Methods

        // Documented
        [JSFunction(Name = "add")]
        public void Addition(object A)
        {
            Utilities.Guard(this.Engine, A, new[] { typeof(MatrixInstance), typeof(double), typeof(int) });
            this.M = MatrixInstance.Add(this, A).M;
        }

        // 
        [JSFunction]
        public MatrixInstance concat(MatrixInstance A)
        {
            return new MatrixInstance(this.Prototype, this.M.Append(A.M));
        }

        // Documented
        [JSFunction]
        public double cond()
        {
            return this.M.ConditionNumber();
        }

        // Documented
        [JSFunction(Name = "ctranspose")]
        public MatrixInstance ConjugateTranspose()
        {
            return new MatrixInstance(this.Prototype, this.M.ConjugateTranspose());
        }

        // Documented
        [JSFunction(Name = "det")]
        public double Determinant()
        {
            return this.M.Determinant();
        }

        // Documented
        [JSFunction(Name = "div")]
        public void Divide(object A)
        {
            Utilities.Guard(this.Engine, A, new[] { typeof(MatrixInstance), typeof(double), typeof(int) });
            this.M = MatrixInstance.Divide(this, A).M;
        }

        // Documented
        [JSFunction(Name = "inv")]
        public MatrixInstance Inverse()
        {
            return new MatrixInstance(this.Prototype, this.M.Inverse());
        }

        //
        [JSFunction(Name = "kron")]
        public MatrixInstance KroneckerProduct(MatrixInstance A)
        {
            return new MatrixInstance(this.Prototype, this.M.KroneckerProduct(A.M));
        }

        //
        [JSFunction]
        public MatrixInstance mod(double k)
        {
            return new MatrixInstance(this.Prototype, this.M.Modulus(k));
        }
        
        // Documented
        [JSFunction(Name = "mul")]
        public void Multiply(object A, [DefaultParameterValue(false)] bool pointwise = false)
        {
            Utilities.Guard(this.Engine, A, new[] { typeof(MatrixInstance), typeof(double), typeof(int) });
            this.M = MatrixInstance.Multiply(this, A, pointwise).M;
        }

        // Documented
        [JSFunction(Name = "neg")]
        public MatrixInstance Negative()
        {
            return new MatrixInstance(this.Prototype, this.M.Negate());
        }

        //
        [JSFunction]
        public double norm(string f)
        {
            switch (f)
            {
                case "fro": return this.M.FrobeniusNorm();
                case "inf": return this.M.InfinityNorm();
                case "1": return this.M.L1Norm();
                case "2": return this.M.L2Norm();
                default: return double.NaN;
            }
        }

        // Documented
        [JSFunction(Name="rank")]
        public int Rank()
        {
            return this.M.Rank();
        }

        //
        [JSFunction]
        public MatrixInstance slice(int x, int xOffset, int y, int yOffset)
        {
            return new MatrixInstance(this.Prototype, this.M.SubMatrix(x, xOffset, y, yOffset));
        }

        // 
        [JSFunction(Name = "sub")]
        public void Subtract(object A)
        {
            Utilities.Guard(this.Engine, A, new[] { typeof(MatrixInstance), typeof(double), typeof(int) });
            this.M = MatrixInstance.Sub(this, A).M;
        }

        // Documented
        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return string.Format("[Matrix {0},{1}]", this.M.RowCount, this.M.ColumnCount);
        }

        // Documented
        [JSFunction(Name = "trace")]
        public double Trace()
        {
            return this.M.Trace();
        }

        // 
        [JSFunction]
        public ArrayInstance toArray([DefaultParameterValue(null)] string wise = null)
        {
            if (wise == null)
            {
                return Utilities.ToArrayInstance(this.Engine, this.M);
            }
            else if (wise.ToLowerInvariant() == "row")
            {
                return Utilities.ToArrayInstance(this.Engine, this.M.ToRowWiseArray());
            }
            else if (wise.ToLowerInvariant() == "column")
            {
                return Utilities.ToArrayInstance(this.Engine, this.M.ToColumnWiseArray());
            }
            throw new ArgumentException();
        }

        // Documented
        [JSFunction(Name = "v")]
        public double GetterSetter(int x, int y, double v)
        {
            return (this.M[x, y] = v);
        }

        #endregion

        #region Utility Methods

        public static MatrixInstance Add(MatrixInstance A, object B) 
        {
            return new MatrixInstance(A.Prototype, A.M.Add(B.ToMatrix()));
        }

        public static MatrixInstance Sub(MatrixInstance A, object B)
        {
            return new MatrixInstance(A.Prototype, A.M.Subtract(B.ToMatrix()));
        }

        public static MatrixInstance Multiply(MatrixInstance A, object B, bool pointwise = false)
        {
            if (B is double) return new MatrixInstance(A.Prototype, A.M.Multiply((double)B));
            if (pointwise)   return new MatrixInstance(A.Prototype, A.M.PointwiseMultiply(B.ToMatrix()));
            else             return new MatrixInstance(A.Prototype, A.M.Multiply(B.ToMatrix()));
        }

        public static MatrixInstance Divide(MatrixInstance A, object B)
        {
            if (B is double) return new MatrixInstance(A.Prototype, A.M.Divide((double)B));
            else             return new MatrixInstance(A.Prototype, A.M.PointwiseDivide(B.ToMatrix()));
        }

        #endregion
    }
}