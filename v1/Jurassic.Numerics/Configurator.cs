using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jurassic.Numerics
{
    public static class Configurator
    {
        public static ScriptEngine New()
        {
            var engine = new ScriptEngine();

            engine.SetGlobalValue("Analysis", new AnalysisObject(engine));
            engine.SetGlobalValue("Matrix", new MatrixConstructor(engine));
            engine.SetGlobalValue("Plot", new PlottingObject(engine));
            engine.SetGlobalValue("Special", new SpecialObject(engine));
            engine.SetGlobalValue("Stats", new StatisticsObject(engine));
            engine.SetGlobalValue("Trig", new TrigObject(engine));
            engine.SetGlobalValue("Vector", new VectorConstructor(engine));

            return engine;
        }

        public static ScriptEngine Demo()
        {
            var engine = new ScriptEngine();

            engine.SetGlobalValue("Analysis", new AnalysisObject(engine));
            engine.SetGlobalValue("Matrix", new MatrixConstructor(engine));
            engine.SetGlobalValue("Special", new SpecialObject(engine));
            engine.SetGlobalValue("Stats", new StatisticsObject(engine));
            engine.SetGlobalValue("Trig", new TrigObject(engine));
            engine.SetGlobalValue("Vector", new VectorConstructor(engine));

            return engine;
        }
    }
}
