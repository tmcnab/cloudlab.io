namespace Jurassic.Cloudlab
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Jurassic;
    using DropNet;
    using MongoDB.Driver;
    using System.Configuration;

    public static class Configurator
    {
        // Configures a new IDE Engine
        public static ScriptEngine Configure(ScriptEngine engine,
                                             string username,
                                             DropNetClient storage, 
                                             Action<string,string> messageBox, 
                                             string mdbConnectionString)
        {
            
            engine.SetGlobalValue("HttpRequest", new HttpRequestConstructor(engine));
            engine.SetGlobalValue("ByteString",  new ByteStringConstructor(engine));
            engine.SetGlobalValue("ByteArray",   new ByteArrayConstructor(engine));
            engine.SetGlobalValue("Plot",        new PlotConstructor(engine, messageBox, username));
            engine.SetGlobalValue("console",     new ConsoleObject(engine, messageBox, username));
            try
            {
                engine.SetGlobalValue("Storage", new StorageObject(engine, MongoDatabase.Create(mdbConnectionString), username));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            if (storage != null) {
                engine.SetGlobalValue("File",    new FileSystemModule(engine, storage));
            }

            return engine;
        }

        // Configures a new API Engine
        public static ScriptEngine Configure(ScriptEngine engine,
                                             string username,
                                             DropNetClient storage,
                                             string mdbConnectionString)
        {
            engine.SetGlobalValue("HttpRequest", new HttpRequestConstructor(engine));
            engine.SetGlobalValue("ByteString",  new ByteStringConstructor(engine));
            engine.SetGlobalValue("ByteArray",   new ByteArrayConstructor(engine));
            engine.SetGlobalValue("Storage",     new StorageObject(engine, MongoDatabase.Create(mdbConnectionString), username));
            engine.SetGlobalValue("APIResponse", "");
            if (storage != null)
            {
                engine.SetGlobalValue("File",    new FileSystemModule(engine, storage));
            }

            return engine;
        }
    }
}
