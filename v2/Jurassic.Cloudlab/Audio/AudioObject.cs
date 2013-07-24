namespace Jurassic.Cloudlab
{
    using System;
    using Jurassic;
    using Jurassic.Library;

    [Serializable]
    public class AudioObject : ObjectInstance
    {
        public AudioObject(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        #region Members

        const string Preamble = "$('a.[data-target=\"#T6\"]').click();";

        #endregion

        private static string Generate(string source)
        {
            var str = "Audio.Source('data:audio/wav;base64," + source + "');";
            return string.Format("###{0}###", Preamble + str + "Audio.Play();");
        }

        #region JSVM Methods

        [JSFunction(Name="play")]
        public static string Play(string source)
        {
            return Generate(source);
        }

        #endregion
    }
}
