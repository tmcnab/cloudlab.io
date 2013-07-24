namespace Jurassic.Numerics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Numerics;
    using Jurassic;
    using Jurassic.Library;
    using MathNet.Numerics.LinearAlgebra.Double;
    using System.Text;

    public static class Utilities
    {
        #region Guards

        public static bool AreSameLength(ArrayInstance a, ArrayInstance b)
        {
            return a.Length == b.Length;
        }

        public static void GuardSameLength(ArrayInstance a, ArrayInstance b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException("arrays are not same length");
            }
        }

        public static void GuardSameLength(object a, object b)
        {
            if (((ArrayInstance)a).Length != ((ArrayInstance)b).Length)
            {
                throw new ArgumentException("arrays are not same length");
            }
        }

        public static void Guard(ScriptEngine engine, object param, params Type[] types)
        {
            // Iterate and return if the type was found
            var paramType = param.GetType();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == paramType) return;
            }

            // If we got here, the param type was not in the provided list
            throw new JavaScriptException(engine, "TypeError", "incorrect type supplied to function");
        }

        #endregion

        #region Other Extensions

        public static object[] ToArray(ArrayInstance array)
        {
            return array.ElementValues.ToArray();
        }

        public static double[] ToDoubleArray(this object A)
        {
            if (A is ArrayInstance) return ((ArrayInstance)A).ToDoubleArray();
            if (A is VectorInstance) return ((VectorInstance)A).ToDoubleArray();
            if (A is MatrixInstance) return ((MatrixInstance)A).ToDoubleArray();

            throw new ArgumentException();
        }

        public static double[] ToDoubleArray(this ArrayInstance A)
        {
            var _array = A.ElementValues.ToArray();
            var _dArray = new double[_array.Length];
            for (int i = 0; i < _array.Length; i++)
            {
                _dArray[i] = Convert.ToDouble(_array[i]);
            }

            return _dArray;
        }

        public static double[] ToDoubleArray(this VectorInstance A)
        {
            return A.V.ToArray();
        }

        public static double[] ToDoubleArray(this MatrixInstance A)
        {
            return A.M.ToRowWiseArray();
        }

        public static double[,] ToDoubleArray2D(ArrayInstance array)
        {
            var nRows = array.ElementValues.Count();
            var nCols = ((ArrayInstance)(array.ElementValues.First())).Length;
            var _array = new double[nRows, nCols];

            for (int i = 0; i < nRows; i++)
            {
                var _rowData = Utilities.ToDoubleArray((ArrayInstance)array[i]);
                for (int j = 0; j < nCols; j++)
                {
                    _array[i, j] = _rowData[j];
                }
            }

            return _array;
        }

        public static int[] ToIntArray(ArrayInstance array)
        {
            var _array = array.ElementValues.ToArray();
            var _iArray = new int[_array.Length];

            for (int i = 0; i < _array.Length; i++)
            {
                _iArray[i] = Convert.ToInt32(_array[i]);
            }

            return _iArray;
        }

        public static Complex[] ToComplex(ArrayInstance x)
        {
            var _x = x.ElementValues.ToArray();


            if (x.Length == 2 && _x[0] is ArrayInstance && _x[1] is ArrayInstance)
            {
                // real and complex arrrays
            }
            else
            {

            }
            var length = x.Length;


            var complexArray = new Complex[length];
            for (int i = 0; i < length; i++)
            {
                complexArray[i] = (Complex)(Convert.ToDouble(_x[i]));
            }
            return complexArray;
        }

        public static VectorInstance ToVectorInstance(ScriptEngine engine, object A)
        {
            if (A is Vector) return Utilities.ToVectorInstance(engine, (Vector)A);

            throw new ArgumentException();
        }

        private static VectorInstance ToVectorInstance(ScriptEngine engine, Vector A)
        {
            return new VectorInstance(engine.Object.Prototype, A);
        }

        public static object ToInstance(ScriptEngine engine, Type type, object A)
        {
            if (type == typeof(ArrayInstance)) return ToArrayInstance(engine, (ArrayInstance)A);

            throw new ArgumentException();
        }

        #endregion

        #region ArrayInstance Extensions

        public static ArrayInstance ToArrayInstance(this double[] A, ScriptEngine engine)
        {
            var _array = new object[A.Length];
            Array.Copy(A, _array, A.Length);
            return engine.Array.Construct(_array);
        }

        public static ArrayInstance ToArrayInstance(this VectorInstance A, ScriptEngine engine)
        {
            return A.V.ToArray().ToArrayInstance(engine);
        }

        public static ArrayInstance ToArrayInstance(ScriptEngine engine, object A)
        {
            if (A is Complex[]) return Utilities.ToArrayInstance(engine, (Complex[])A);
            if (A is double[]) return Utilities.ToArrayInstance(engine, (double[])A);
            if (A is Vector) return Utilities.ToArrayInstance(engine, (Vector)A);
            if (A is MatrixInstance) return Utilities.ToArrayInstance(engine, (MatrixInstance)A);
            if (A is VectorInstance) return Utilities.ToArrayInstance(engine, (VectorInstance)A);

            throw new ArgumentException();
        }

        private static ArrayInstance ToArrayInstance(ScriptEngine engine, Complex[] A)
        {
            var _realValues = new object[A.Length];
            var _imagValues = new object[A.Length];
            for (int i = 0; i < A.Length; i++)
            {
                _realValues[i] = A[i].Real;
                _imagValues[i] = A[i].Imaginary;
            }
            var _realInstance = engine.Array.Construct(_realValues);
            var _imagInstance = engine.Array.Construct(_imagValues);

            return engine.Array.Construct(_realInstance, _imagInstance);
        }

        private static ArrayInstance ToArrayInstance(ScriptEngine engine, MatrixInstance A)
        {
            var rows = new List<object>();
            for (int i = 0; i < A.M.RowCount; i++)
            {
                var rowvalues = A.M.Row(i).ToArray();
                var _array = new object[rowvalues.Length];
                Array.Copy(rowvalues, _array, rowvalues.Length);
                rows.Add(engine.Array.Construct(_array));
            }

            return engine.Array.Construct(rows.ToArray());
        }

        private static ArrayInstance ToArrayInstance(ScriptEngine engine, VectorInstance A)
        {
            return engine.Array.Construct(A.V.ToArray());
        }

        public static ArrayInstance ToArrayInstance(this byte[] A, ScriptEngine engine)
        {
            var _array = new object[A.Length];
            for (int i = 0; i < A.Length; i++)
            {
                _array[i] = (int)A[i];
            }
            return engine.Array.Construct(_array);
        }

        #endregion

        #region Vector Extensions

        public static Vector ToVector(this object A)
        {
            if (A is ArrayInstance)  return  ((ArrayInstance)A).ToVector();
            if (A is VectorInstance) return ((VectorInstance)A).ToVector();
            throw new ArgumentException();
        }

        public static Vector ToVector(this ArrayInstance A)
        {
            return new DenseVector(A.ToDoubleArray());
        }

        public static Vector ToVector(this VectorInstance A)
        {
            return (Vector)A.V.Clone();
        }

        #endregion

        #region Matrix Extensions

        public static Matrix ToMatrix(this object A)
        {
            if (A is MatrixInstance) return ((MatrixInstance)A).ToMatrix();
            throw new ArgumentException();
        }

        public static Matrix ToMatrix(this MatrixInstance A)
        {
            return (Matrix)A.M.Clone();
        }

        public static Matrix ToMatrix(this double d, int rows, int cols)
        {
            return new DenseMatrix(rows, cols, d);
        }

        #endregion

        public static byte[] ToByteArray(this ArrayInstance A)
        {
            var _array = A.ElementValues.ToArray();
            var _bArray = new byte[_array.Length];
            for (int i = 0; i < _array.Length; i++)
            {
                _bArray[i] = Convert.ToByte(_array[i]);
            }

            return _bArray;
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string AsString(this byte[] bytes)
        {
            StringBuilder builder = new StringBuilder(bytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.AppendFormat("{0},", (int)bytes[i]);
            }
            return builder.ToString().TrimEnd(',');
        }
    }
}