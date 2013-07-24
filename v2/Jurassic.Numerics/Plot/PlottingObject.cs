
namespace Jurassic.Numerics
{
    using System;
    using Jurassic;
    using Jurassic.Library;

    [Serializable]
    public class PlottingObject : ObjectInstance
    {
        public PlottingObject(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
            this.DefineProperties();
        }

        protected void DefineProperties()
        {
            // Textual Attributes
            this.DefineProperty("title", new PropertyDescriptor(null, PropertyAttributes.Writable), false);
            this.DefineProperty("xlabel", new PropertyDescriptor(null, PropertyAttributes.Writable), false);
            this.DefineProperty("ylabel", new PropertyDescriptor(null, PropertyAttributes.Writable), false);
            this.DefineProperty("x2label", new PropertyDescriptor(null, PropertyAttributes.Writable), false);
            this.DefineProperty("y2label", new PropertyDescriptor(null, PropertyAttributes.Writable), false);

            // Options
            this.DefineProperty("legend", new PropertyDescriptor(false, PropertyAttributes.Writable), false);
            this.DefineProperty("stacked", new PropertyDescriptor(false, PropertyAttributes.Writable), false);
            this.DefineProperty("highlighter", new PropertyDescriptor(false, PropertyAttributes.Writable), false);
            this.DefineProperty("zoom", new PropertyDescriptor(true, PropertyAttributes.Writable), false);
        }

        #region Members

        private const string Preamble = "$('a[data-target=\"#graphing-tab\"]').click();$('#graphing-content').html('');";

        public enum Renderer
        {
            Bar,
            Line,
            Pie
        }

        private static string RendererToString(Renderer renderer)
        {
            switch (renderer)
            {
                case Renderer.Bar:  return "Scripts/jqplot/jqplot.barRenderer.min.js";
                case Renderer.Pie: return "Scripts/jqplot/jqplot.pieRenderer.min.js";
                default:            return string.Empty;
            }
        }

        private static string RequireScript (Renderer renderer, string innerScript)
        {
            return "$.getScript('" + RendererToString(renderer) + "',function(){" + innerScript + "});";
        }

        private static string GenerateJS(Renderer renderer, ArrayInstance dataSeries)
        {
            var opts = new PlottingOptions(renderer, dataSeries.Engine);
            string _js = Preamble + "IDE.plot = $.jqplot('graphing-content'," + JSONObject.Stringify(dataSeries.Engine, dataSeries);
            var _opts = opts.ToJSON();

            if (_opts != "{}")
            {
                _js += "," + _opts;
            }
            _js += ");";
            
            
            if (renderer == Renderer.Line)
            {
                return string.Format("###{0}###", _js);
            }
            return string.Format("###{0}###", RequireScript(renderer, _js));
        }

        #endregion

        #region Public JS Object Functions

        [JSFunction]
        public static string pie(ArrayInstance dataSeries)
        {
            return GenerateJS(Renderer.Pie, dataSeries);
        }

        [JSFunction]
        public static string line(ArrayInstance dataSeries)
        {
            return GenerateJS(Renderer.Line, dataSeries);
        }

        [JSFunction]
        public static string bar(ArrayInstance dataSeries)
        {
            return GenerateJS(Renderer.Bar, dataSeries);
        }

        #endregion

    }
}