using log4net.Config;
using log4net.Core;
using System;
using System.Runtime.CompilerServices;

namespace ToolsLibrary
{
    public static class Logger
    {

        private static readonly ILogger defaultLogger;

        static Logger()
        {
            var conf = XmlConfigurator.Configure();

            //Here is the once-per-class call to initialize the log object
            defaultLogger = LoggerManager.GetLogger(System.Reflection.Assembly.GetCallingAssembly(), "StaticLogger");
        }

        private static void Log(Level level, string message, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Log(level, message, null, callingMethod, callingFilePath, callingLineNumber);
        }

        private static void Log(Level level, string message, Exception exception, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {

#if !DEBUG
            try
            {
#endif
            if (defaultLogger.IsEnabledFor(level))
            {
                defaultLogger.Log(typeof(Logger), level, message, exception);
            }
#if !DEBUG
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
#endif
        }

        public static void Debug(string message, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Debug(message, null, callingMethod, callingFilePath, callingLineNumber);
        }

        public static void Debug(string message, Exception exception, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Log(Level.Debug, message, exception);
        }

        public static void Info(string message, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Info(message, null, callingMethod, callingFilePath, callingLineNumber);
        }

        public static void Info(string message, Exception exception, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Log(Level.Info, message, exception);
        }

        public static void Warn(string message, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Warn(message, null, callingMethod, callingFilePath, callingLineNumber);
        }

        public static void Warn(string message, Exception exception, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Log(Level.Warn, message, exception);
        }

        public static void Error(string message, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Error(message, null, callingMethod, callingFilePath, callingLineNumber);
        }

        public static void Error(string message, Exception exception, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Log(Level.Error, message, exception);
        }

        public static void Fatal(string message, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Fatal(message, null, callingMethod, callingFilePath, callingLineNumber);
        }

        public static void Fatal(string message, Exception exception, [CallerMemberName] string callingMethod = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int callingLineNumber = 0)
        {
            Log(Level.Fatal, message, exception);
        }


    }
}
