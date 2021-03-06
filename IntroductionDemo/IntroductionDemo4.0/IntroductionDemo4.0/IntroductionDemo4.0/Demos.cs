using PloobsEngine.Engine;
using PloobsEngine.SceneControl;
using PloobsEngine.Engine.Logger;
using System;

namespace IntroductionDemo4._0
{    
    /// <summary>
    /// Introduction Demos entry point
    /// </summary>
    public class Demos
    {
        public Demos()
        {
            InitialEngineDescription desc = InitialEngineDescription.Default();
            ///We are using the simplest parameters to work in all computers
            ///Check the Advanced Demos to know how to change those
            ///optional parameters, the default is good for most situations
            //desc.UseVerticalSyncronization = true;
            //desc.isFixedGameTime = true;
            //desc.isMultiSampling = true; ///Only works on forward rendering
            //desc.useMipMapWhenPossible = true;
            desc.Logger = new SimpleLogger();
            desc.UnhandledException_Handler = UnhandledException;

            ///start the engine
            using (EngineStuff engine = new EngineStuff(ref desc, LoadScreen))
            {
                engine.Run();
            }
        }        
        

        static void LoadScreen(ScreenManager manager)
        {            
            ///add the title screen
            manager.AddScreen(new TitleScreen());                                    
        }

        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ///handle unhandled excetption here (log, send to a server ....)
            Console.WriteLine("Exception: " + e.ToString());
        }
    }

    /// <summary>
    /// Custom log class
    /// </summary>
    class SimpleLogger : ILogger
    {
        #region ILogger Members

        public override void Log(string Message, LogLevel logLevel)
        {
            ///handle messages logs
            Console.WriteLine(Message + "  -  "  + logLevel.ToString());
        }

        #endregion
    }
}




