namespace Jurassic.Numerics
{
    using System;
    using Jurassic;
    using Jurassic.Library;
    using MathNet.Numerics.Statistics;

    [Serializable]
    public class StatisticsObject : ObjectInstance
    {
        public StatisticsObject(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        #region User-Visible Functions

        // Documented
        [JSFunction(Name = "max", Flags = JSFunctionFlags.HasEngineParameter)]
        public static double Max(ScriptEngine engine, object A)
        {
            Utilities.Guard(engine, A, new Type[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(MatrixInstance) });

            return Statistics.Maximum(A.ToDoubleArray());
        }

        // Documented
        [JSFunction(Name = "mean", Flags = JSFunctionFlags.HasEngineParameter)]
        public static double Mean(ScriptEngine engine, object A)
        {
            Utilities.Guard(engine, A, new Type[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(MatrixInstance) });

            return Statistics.Mean(A.ToDoubleArray());
        }

        // Documented
        [JSFunction(Name = "median", Flags = JSFunctionFlags.HasEngineParameter)]
        public static double Median(ScriptEngine engine, object A)
        {
            Utilities.Guard(engine, A, new Type[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(MatrixInstance) });

            return Statistics.Median(A.ToDoubleArray());
        }

        // Documented
        [JSFunction(Name = "min", Flags = JSFunctionFlags.HasEngineParameter)]
        public static double Minimum(ScriptEngine engine, object A)
        {
            Utilities.Guard(engine, A, new Type[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(MatrixInstance) });

            return Statistics.Minimum(A.ToDoubleArray());
        }

        // Documented
        [JSFunction(Name = "order", Flags = JSFunctionFlags.HasEngineParameter)]
        public static double Order(ScriptEngine engine, object A, int n)
        {
            Utilities.Guard(engine, A, new Type[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(MatrixInstance) });

            return Statistics.OrderStatistic(A.ToDoubleArray(), n);
        }

        // Documented
        [JSFunction(Name = "variance", Flags = JSFunctionFlags.HasEngineParameter)]
        public static double Variance(ScriptEngine engine, object A, [DefaultParameterValue(false)] bool f = true)
        {
            Utilities.Guard(engine, A, new Type[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(MatrixInstance) });

            if (f) return Statistics.Variance(A.ToDoubleArray());
            else   return Statistics.PopulationVariance(A.ToDoubleArray());
        }

        // Documented
        [JSFunction(Name = "stddev", Flags = JSFunctionFlags.HasEngineParameter)]
        public static double StandardDeviation(ScriptEngine engine, object A, [DefaultParameterValue(true)] bool f = true)
        {
            Utilities.Guard(engine, A, new Type[2] { typeof(ArrayInstance), typeof(VectorInstance) });

            if (f) return Statistics.StandardDeviation(A.ToDoubleArray());
            else   return Statistics.PopulationStandardDeviation(A.ToDoubleArray());
        }

        // Documented
        [JSFunction(Name = "hist", Flags = JSFunctionFlags.HasEngineParameter)]
        public static VectorInstance Histogram(ScriptEngine engine, object A,
                                              [DefaultParameterValue(10)] int nbuckets = 10,
                                              [DefaultParameterValue(double.MinValue)] double lowerBound = double.MinValue,
                                              [DefaultParameterValue(double.MaxValue)] double upperBound = double.MaxValue)
        {
            Utilities.Guard(engine, A, new Type[] { typeof(ArrayInstance), typeof(VectorInstance), typeof(MatrixInstance) });

            try
            {
                Histogram h;
                if (lowerBound == double.MinValue && upperBound == double.MaxValue)
                {
                    h = new Histogram(A.ToDoubleArray(), nbuckets);
                }
                else
                {
                    h = new Histogram(A.ToDoubleArray(), nbuckets, lowerBound, upperBound);
                }

                double[] d = new double[h.BucketCount];
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = h[i].Count;
                }

                return new VectorInstance(engine.Object.InstancePrototype, d.ToVector());
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        // Documented
        [JSFunction(Name = "all", Flags = JSFunctionFlags.HasEngineParameter)]
        public static ObjectInstance All(ScriptEngine engine, object A)
        {
            Utilities.Guard(engine, A, new Type[3] { typeof(ArrayInstance), typeof(VectorInstance), typeof(MatrixInstance) });
            var stats = new DescriptiveStatistics(A.ToDoubleArray());
            var statsObj = engine.Object.Construct();

            statsObj.SetPropertyValue("kurtosis", stats.Kurtosis, false);
            statsObj.SetPropertyValue("count", stats.Count, false);
            statsObj.SetPropertyValue("maximum", stats.Maximum, false);
            statsObj.SetPropertyValue("minimum", stats.Minimum, false);
            statsObj.SetPropertyValue("mean", stats.Mean, false);
            statsObj.SetPropertyValue("median", stats.Median, false);
            statsObj.SetPropertyValue("skewness", stats.Skewness, false);
            statsObj.SetPropertyValue("stddev", stats.StandardDeviation, false);
            statsObj.SetPropertyValue("variance", stats.Variance, false);

            return statsObj;
        }

        #endregion
    }
}