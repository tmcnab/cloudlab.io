
namespace Jurassic.Numerics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using Jurassic;
    using Jurassic.Library;
    using MathNet.Numerics.LinearAlgebra.Double;

    /// <summary>
    /// Utility functions used in Jurassic execution
    /// </summary>
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

        #endregion

        #region Converters

        public static object[] ToArray(ArrayInstance array)
        {
            return array.ElementValues.ToArray();
        }

        public static double[] ToDoubleArray(ArrayInstance array)
        {
            var _array = array.ElementValues.ToArray();
            var _dArray = new double[_array.Length];
            for (int i = 0; i < _array.Length; i++) {
                _dArray[i] = Convert.ToDouble(_array[i]);
            }

            return _dArray;
        }

        public static double[,] ToDoubleArray2D(ArrayInstance array)
        {
            var nRows = array.ElementValues.Count();
            var nCols = ((ArrayInstance)(array.ElementValues.First())).Length;
            var _array = new double[nRows, nCols];

            for (int i = 0; i < nRows; i++) {
                var _rowData = Utilities.ToDoubleArray((ArrayInstance)array[i]);
                for (int j = 0; j < nCols; j++) {
                    _array[i, j] = _rowData[j];
                }
            }

            return _array;
        }

        /// <summary>
        /// Attempts to convert a JS array into an array of integers
        /// </summary>
        /// <param name="array">The array to convert</param>
        /// <returns>An array of integers</returns>
        public static int[] ToIntArray(ArrayInstance array)
        {
            var _array = array.ElementValues.ToArray();
            var _iArray = new int[_array.Length];

            for (int i = 0; i < _array.Length; i++) {
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

        /// <summary>
        /// Converts a Complex .NET array into a Jurassic 2xN double array
        /// </summary>
        /// <param name="engine">The VM's engine to create the array in</param>
        /// <param name="values">The values to convert into JS arrays</param>
        /// <returns>A 2D JS array representing the complex values</returns>
        public static ArrayInstance ToArrayInstance(ScriptEngine engine, Complex[] values)
        {
            var _realValues = new object[values.Length];
            var _imagValues = new object[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                _realValues[i] = values[i].Real;
                _imagValues[i] = values[i].Imaginary;
            }
            var _realInstance = engine.Array.Construct(_realValues);
            var _imagInstance = engine.Array.Construct(_imagValues);
            
            //return _realInstance;
            
            return engine.Array.Construct(_realInstance, _imagInstance);
        }

        public static ArrayInstance ToArrayInstance(ScriptEngine engine, double[] values)
        {
            var _array = new object[values.Length];
            Array.Copy(values, _array, values.Length);
            return engine.Array.Construct(_array);
        }

        public static ArrayInstance ToArrayInstance(ScriptEngine engine, Matrix m)
        {
            var rows = new List<object>();
            for (int i = 0; i < m.RowCount; i++)
            {
                var rowvalues = m.Row(i).ToArray();
                var _array = new object[rowvalues.Length];
                Array.Copy(rowvalues, _array, rowvalues.Length);
                rows.Add(engine.Array.Construct(_array));
            }

            return engine.Array.Construct(rows.ToArray());
        }

        public static Vector ToVector(ArrayInstance array)
        {
            return new DenseVector(Utilities.ToDoubleArray(array));
        }

        #endregion
    }
}