
namespace Jurassic.Cloudlab
{
    using System;
    using Jurassic.Library;
    using Jurassic.Numerics;
    using System.Dynamic;

    [Serializable]
    public class PlotInstance : ObjectInstance
    {
        #region Construction

        public PlotInstance(ObjectInstance prototype, Action<string,string> postMessage, string username, params object[] settings) 
            : base(prototype)
        {
            this.PopulateFunctions();
            this.PopulateProperties();
            this.PostMessage = postMessage;
            this.Username = username;
        }

        private void PopulateProperties()
        {
            this.DefineProperty("xLabel",  new PropertyDescriptor(string.Empty, PropertyAttributes.Writable), false);
            this.DefineProperty("yLabel",  new PropertyDescriptor(string.Empty, PropertyAttributes.Writable), false);
            this.DefineProperty("x2Label", new PropertyDescriptor(string.Empty, PropertyAttributes.Writable), false);
            this.DefineProperty("y2Label", new PropertyDescriptor(string.Empty, PropertyAttributes.Writable), false);
            this.DefineProperty("title",   new PropertyDescriptor(string.Empty, PropertyAttributes.Writable), false);
            this.DefineProperty("markers", new PropertyDescriptor(false,        PropertyAttributes.Writable), false);
            this.DefineProperty("type",    new PropertyDescriptor(string.Empty, PropertyAttributes.Writable), false);
        }

        #endregion

        #region Members

        public Action<string, string> PostMessage;
        public string Username;

        #endregion

        #region Properties

        #endregion

        #region User-Visible Methods

        [JSFunction(Name="plot")]
        public void Plot(object data)
        {
            ConstructJQPlotCall(data);
        }

        #endregion

        #region Background Methods

        private void ConstructJQPlotCall(object data)
        {
            this.PostMessage(this.Username, "Plotting.Plot(" + this.DataToJSON(data) + "," + this.ToJSON() + ");");
        }

        private string DataToJSON(object data)
        {
            if (data is ArrayInstance)  return JSONObject.Stringify(this.Engine, (ArrayInstance)data);
            if (data is VectorInstance) return JSONObject.Stringify(this.Engine, this.Engine.Array.Construct(((VectorInstance)data).ToArrayInstance(this.Engine)));
            
            throw new JavaScriptException(this.Engine, "Error", "input parameter must be of type Array or Vector");
        }

        private string ToJSON()
        {
            string _r = "{";

            _r = this.AddTitle(_r);
            _r = this.AddAxes(_r);
            _r = this.AddSeries(_r);
            _r = this.AddSeriesDefaults(_r);

            if (_r.EndsWith(",")) { _r = _r.TrimEnd(','); }
            return _r + "}";
        }

        private string AddSeries(string src)
        {
            if ((bool)this.GetPropertyValue("markers"))
                src += "series:[{showMarker:true}],";
            else
                src += "series:[{showMarker:false}],";
            return src;
        }

        private string AddSeriesDefaults(string src)
        {
            var prop = (string)this.GetPropertyValue("type");
            switch (prop)
	        {
                case "bar": src += "seriesDefaults: { renderer: jQuery.jqplot.BarRenderer },"; break;
                case "line":
		        default:
                    break;
	        }
            return src;
        }

        private string AddTitle(string src)
        {
            var prop = (string)this.GetPropertyValue("title");
            if (!string.IsNullOrWhiteSpace(prop))
            {
                src += "title:'" + prop + "',";
            }
            return src;
        }

        
        private string AddAxes(string src)
        {
            var _s = "axes:{";

            var xLabel = (string)this.GetPropertyValue("xLabel");
            if (!string.IsNullOrWhiteSpace(xLabel))
            {
                _s += "xaxis:{ label:'" + xLabel + "'},";
            }

            var yLabel = (string)this.GetPropertyValue("yLabel");
            if (!string.IsNullOrWhiteSpace(yLabel))
            {
                _s += "yaxis:{ label:'" + yLabel + "'},";
            }

            var x2Label = (string)this.GetPropertyValue("x2Label");
            if (!string.IsNullOrWhiteSpace(x2Label))
            {
                _s += "x2axis:{ label:'" + x2Label + "'},";
            }

            var y2Label = (string)this.GetPropertyValue("y2Label");
            if (!string.IsNullOrWhiteSpace(y2Label))
            {
                _s += "y2axis:{ label:'" + y2Label + "'},";
            }

            if (_s.EndsWith(",")) { _s = _s.TrimEnd(','); }
            _s += "},";

            if (_s == "axes:{},") return src;
            return src + _s;
        }

        #endregion

    }
}