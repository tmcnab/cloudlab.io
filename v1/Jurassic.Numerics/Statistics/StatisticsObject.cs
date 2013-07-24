
namespace Jurassic.Numerics
{
    using System;
    using Jurassic;
    using Jurassic.Library;
    using MathNet.Numerics.Statistics;

    /// <summary>
    /// Implements the static functions dedicated to statistical calculations.
    /// </summary>
    [Serializable]
    public class StatisticsObject : ObjectInstance
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="engine"></param>
        public StatisticsObject(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        /*
         This function determines the maximum value in a vector.

### Usage
    b = Stats.max(A)

| Variable | Required | Type | Description |
| - | - | - | - |
| **A** | Yes | `Array`<sup>&alpha;</sup>, `Vector` | The array to determine the maximum of |
| **b** | &mdash; | `Numeric` | The maximum value in **A**
<sup>&alpha;</sup> All values must be of type `Numeric` or an exception will be thrown

### Example
Consider the following commands entered into the [interactive terminal](/IDE):

    >> var data = [ 1, 3.4, 8.7, 5.2, 0.3, -4.6, 2.2 ];
    >> Stats.max(data)
    8.7
         */

        #region Basic Stats

        // Documented
        [JSFunction]
        public static object max(object x)
        {
            if (x is ArrayInstance)
                return Statistics.Maximum(Utilities.ToDoubleArray((ArrayInstance)x));
            else if (x is VectorInstance)
                return Statistics.Maximum(((VectorInstance)x).V.ToArray());
            else
                return Jurassic.Undefined.Value;
        }

        // Documented
        [JSFunction]
        public static object mean(object x)
        {
            if (x is ArrayInstance)
                return Statistics.Mean(Utilities.ToDoubleArray((ArrayInstance)x));
            else if (x is VectorInstance)
                return Statistics.Mean(((VectorInstance)x).V.ToArray());
            else
                return Jurassic.Undefined.Value;
        }

        // Documented
        [JSFunction]
        public static object median(object x)
        {
            if (x is ArrayInstance)
                return Statistics.Median(Utilities.ToDoubleArray((ArrayInstance)x));
            else if (x is VectorInstance)
                return Statistics.Median(((VectorInstance)x).V.ToArray());
            else
                return Jurassic.Undefined.Value;
        }

        // Documented
        [JSFunction]
        public static object min(object x)
        {
            if (x is ArrayInstance)
                return Statistics.Minimum(Utilities.ToDoubleArray((ArrayInstance)x));
            else if (x is VectorInstance)
                return Statistics.Minimum(((VectorInstance)x).V.ToArray());
            else
                return Jurassic.Undefined.Value;
        }

        // Documented
        [JSFunction]
        public static object order(object x, int n)
        {
            if (x is ArrayInstance)
                return Statistics.OrderStatistic(Utilities.ToDoubleArray((ArrayInstance)x), n);
            else if (x is VectorInstance)
                return Statistics.OrderStatistic(((VectorInstance)x).V.ToArray(), n);
            else
                return Jurassic.Undefined.Value;
        }

        // Documented
        [JSFunction]
        public static object variance(object x, bool f = true)
        {
            if (f) {
                if (x is ArrayInstance)
                    return Statistics.Variance(Utilities.ToDoubleArray((ArrayInstance)x));
                else if (x is VectorInstance)
                    return Statistics.Variance(((VectorInstance)x).V.ToArray());
                else
                    return Jurassic.Undefined.Value;
            }
            else {
                if (x is ArrayInstance)
                    return Statistics.PopulationVariance(Utilities.ToDoubleArray((ArrayInstance)x));
                else if (x is VectorInstance)
                    return Statistics.PopulationVariance(((VectorInstance)x).V.ToArray());
                else
                    return Jurassic.Undefined.Value;
            }
        }

        // Documented
        [JSFunction]
        public static object stddev(object x, bool f = true)
        {
            if (f) {
                if (x is ArrayInstance)
                    return Statistics.StandardDeviation(Utilities.ToDoubleArray((ArrayInstance)x));
                else if (x is VectorInstance)
                    return Statistics.StandardDeviation(((VectorInstance)x).V.ToArray());
                else
                    return Jurassic.Undefined.Value;
            }
            else {
                if (x is ArrayInstance)
                    return Statistics.PopulationStandardDeviation(Utilities.ToDoubleArray((ArrayInstance)x));
                else if (x is VectorInstance)
                    return Statistics.PopulationStandardDeviation(((VectorInstance)x).V.ToArray());
                else
                    return Jurassic.Undefined.Value;
            }
        }

        #endregion

        #region Advanced Stats

        // Documented
        [JSFunction]
        public static ArrayInstance hist(ArrayInstance data, int nbuckets = 10,
                                         double lowerBound = double.MinValue,
                                         double upperBound = double.MaxValue)
        {
            try
            {
                Histogram h;
                if (lowerBound == double.MinValue || upperBound == double.MaxValue)
                {
                    h = new Histogram(Utilities.ToDoubleArray(data), nbuckets);
                }
                else
                {
                    h = new Histogram(Utilities.ToDoubleArray(data), nbuckets, lowerBound, upperBound);
                }

                double[] d = new double[h.BucketCount];
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = h[i].Count;
                }
                return Utilities.ToArrayInstance(data.Engine, d);
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        #endregion
    }
}