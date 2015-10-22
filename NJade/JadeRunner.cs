namespace NJade
{
    using System;
    using System.IO;
    using Microsoft.ClearScript.V8;
    using NJade.Properties;

    class JadeRunner
    {
        public string Render(string path)
        {
            using (var engine = new V8ScriptEngine())
            {
                Environment.CurrentDirectory = Path.GetDirectoryName(path);

                engine.AddHostType("externalFS", typeof(FsModule));

                engine.Execute("jade.js", Resources.jade);

                var result = (string)engine.Script.jade.renderFile(path); 

                return result;
            }
        }

    }
}
