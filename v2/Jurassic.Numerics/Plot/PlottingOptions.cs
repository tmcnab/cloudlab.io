
namespace Jurassic.Numerics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Jurassic;
    using Jurassic.Library;

    public class PlottingOptions
    {
        public string title { get; set; }
        public string xLabel { get; set; }
        public string yLabel { get; set; }
        public string x2Label { get; set; }
        public string y2Label { get; set; }
        public bool legend { get; set; }
        public bool stacked { get; set; }
        public bool highlighter { get; set; }
        public bool zoom { get; set; }

        public string Renderer { get; set; }

        public PlottingOptions(PlottingObject.Renderer renderer, ObjectInstance obj)
        {
            this.stacked = false;

            switch (renderer)
            {
                case PlottingObject.Renderer.Bar:   this.Renderer = "jQuery.jqplot.BarRenderer";    break;
                case PlottingObject.Renderer.Pie:   this.Renderer = "jQuery.jqplot.PieRenderer";    break;
            }
            this.Parse(obj);
        }

        public PlottingOptions(PlottingObject.Renderer renderer, ScriptEngine engine)
        {
            this.stacked = false;

            switch (renderer)
            {
                case PlottingObject.Renderer.Bar: this.Renderer = "jQuery.jqplot.BarRenderer"; break;
                case PlottingObject.Renderer.Pie: this.Renderer = "jQuery.jqplot.PieRenderer"; break;
            }
            this.Parse(engine.GetGlobalValue<ObjectInstance>("Plot"));
        }

        protected void Parse(ObjectInstance obj)
        {
            var properties = obj.Properties.ToDictionary(p => p.Name, p => p.Value);
            if (properties.ContainsKey("title") && properties["title"] is System.String) {
                this.title = (string)properties["title"];
            }

            if (properties.ContainsKey("xlabel") && properties["xlabel"] is System.String) {
                this.xLabel = (string)properties["xlabel"];
            }
            if (properties.ContainsKey("ylabel") && properties["ylabel"] is System.String) {
                this.yLabel = (string)properties["ylabel"];
            }
            if (properties.ContainsKey("x2label") && properties["x2label"] is System.String) {
                this.x2Label = (string)properties["x2label"];
            }
            if (properties.ContainsKey("y2label") && properties["y2label"] is System.String) {
                this.y2Label = (string)properties["y2label"];
            }

            if (properties.ContainsKey("stacked") && properties["stacked"] is System.Boolean) {
                this.stacked = (bool)properties["stacked"];
            }

            if (properties.ContainsKey("legend") && properties["legend"] is System.Boolean) {
                this.legend = (bool)properties["legend"];
            }

            if (properties.ContainsKey("highlighter") && properties["highlighter"] is System.Boolean)
            {
                this.highlighter = (bool)properties["highlighter"];
            }

            if (properties.ContainsKey("zoom") && properties["zoom"] is System.Boolean)
            {
                this.zoom = (bool)properties["zoom"];
            }

        }

        #region JSON Generation Methods

        protected string AddTitle(string src)
        {
            if (!string.IsNullOrWhiteSpace(this.title)) {
                src += "title:'" + this.title + "',";
            }
            return src;
        }

        protected string AddSeriesDefaults(string src)
        {
            if (!string.IsNullOrWhiteSpace(this.Renderer)) {
                src += "seriesDefaults: {" + "renderer:" + this.Renderer + "},";
            }
            return src;
        }

        protected string _Axes(string src)
        {
            var _s = "axes:{";

            if (!string.IsNullOrWhiteSpace(this.xLabel)) {
                _s += "xaxis:{ label:'" + this.xLabel + "'";
                /*if(this.Renderer == "jQuery.jqplot.BarRenderer") {
                    _s += ",renderer:$.jqplot.CategoryAxisRenderer";
                }*/
                _s += "},";
            }

            if (!string.IsNullOrWhiteSpace(this.yLabel)) {
                _s += "yaxis:{ label:'" + this.yLabel + "'},";
            }

            if (!string.IsNullOrWhiteSpace(this.x2Label)) {
                _s += "x2axis:{ label:'" + this.x2Label + "'},";
            }

            if (!string.IsNullOrWhiteSpace(this.y2Label)) {
                _s += "y2axis:{ label:'" + this.y2Label + "'},";
            }

            /*axes: {
                xaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer,
                    ticks: ticks
                }
            }*/

            if (_s.EndsWith(",")) { _s = _s.TrimEnd(','); }
            _s += "},";

            if (_s == "axes:{},") {
                return src;
            }
            else {
                return src + _s;
            }
        }

        protected string _Stacked(string src)
        {
            if(this.stacked) {
                src += "stackSeries:true,";
            }
            return src;
        }

        protected string _Legend(string src)
        {
            if (this.legend) {
                src += "legend:{show:true,location:'e',placement:'outside'},";
            }
            return src;
        }

        protected string _Highlighter(string src)
        {
            if (this.highlighter)
            {
                src += "highlighter:{show:true},";
            }
            return src;
        }

        protected string _Zoom(string src)
        {
            if (this.zoom)
            {
                src += "cursor:{zoom:true,show:true},";
            }
            return src;
        }

        #endregion

        public string ToJSON()
        {
            string _r = "{";

            _r = this._Stacked(_r);
            _r = this.AddTitle(_r);
            _r = this.AddSeriesDefaults(_r);
            _r = this._Axes(_r);
            _r = this._Legend(_r);
            _r = this._Highlighter(_r);
            _r = this._Zoom(_r);

            if (_r.EndsWith(",")) { _r = _r.TrimEnd(','); }
            return _r + "}";
        }
    }
}