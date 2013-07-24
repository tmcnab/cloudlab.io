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

        // Documented
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
        [JSFunction]
        public MatrixInstance add(MatrixInstance A)
        {
            return new MatrixInstance(this.Prototype, A.M + this.M);
        }

        // Documented
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
        [JSFunction]
        public MatrixInstance ctranspose()
        {
            return new MatrixInstance(this.Prototype, this.M.ConjugateTranspose());
        }

        // Documented
        [JSFunction]
        public double det()
        {
            return this.M.Determinant();
        }

        [JSFunction]
        public MatrixInstance div(double k)
        {
            return new MatrixInstance(this.Prototype, this.M.Divide(k));
        }

        // Documented
        [JSFunction]
        public MatrixInstance inv()
        {
            return new MatrixInstance(this.Prototype, this.M.Inverse());
        }

        [JSFunction]
        public MatrixInstance kron(MatrixInstance A)
        {
            return new MatrixInstance(this.Prototype, this.M.KroneckerProduct(A.M));
        }

        [JSFunction]
        public MatrixInstance mod(double k)
        {
            return new MatrixInstance(this.Prototype, this.M.Modulus(k));
        }

        [JSFunction]
        public object mul(object A)
        {
            if (A is MatrixInstance)
                return new MatrixInstance(this.Prototype, this.M * ((MatrixInstance)A).M);
            else if (A is VectorInstance)
                return new MatrixInstance(this.Prototype, this.M * ((VectorInstance)A).V);
            else if (A is ArrayInstance)
                return new MatrixInstance(this.Prototype, this.M * Utilities.ToVector((ArrayInstance)A));
            else if (A is double || A is int)
                return new MatrixInstance(this.Prototype, this.M * (double)A);

            return Undefined.Value;
        }

        // Documented
        [JSFunction]
        public MatrixInstance neg()
        {
            return new MatrixInstance(this.Prototype, this.M.Negate());
        }

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
        [JSFunction]
        public int rank()
        {
            return this.M.Rank();
        }

        [JSFunction]
        public MatrixInstance slice(int x, int xOffset, int y, int yOffset)
        {
            return new MatrixInstance(this.Prototype, this.M.SubMatrix(x, xOffset, y, yOffset));
        }

        // Documented
        [JSFunction]
        public MatrixInstance sub(MatrixInstance A)
        {
            return new MatrixInstance(this.Prototype, this.M - ((MatrixInstance)A).M);
        }

        [JSFunction]
        public string toString()
        {
            return this.M.ToString();
        }

        // Documented
        [JSFunction]
        public double trace()
        {
            return this.M.Trace();
        }

        // Documented
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

        [JSFunction]
        public double v(int x, int y, object v)
        {
            if (v is double || v is int)
            {
                return this.M[x, y] = Convert.ToDouble(v);
            }
            else
            {
                return this.M[x, y];
            }
        }

        #endregion
    }
}