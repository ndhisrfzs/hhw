using System;

#if Server
namespace GN
{
    public static class Log
    {
        private static readonly ILog globalLog = new NLogAdapter();

        public static void Trace(string message)
        {
            globalLog.Trace(message);
        }
        public static void Warning(string message)
        {
            globalLog.Warning(message);
        }
        public static void Info(string message)
        {
            globalLog.Info(message);
        }
        public static void Debug(string message)
        {
            globalLog.Debug(message);
        }
        public static void Error(Exception e)
        {
            globalLog.Error(e.ToString());
        }
        public static void Error(string message)
        {
            globalLog.Error(message);
        }
        public static void Fatal(Exception e)
        {
            globalLog.Error(e.ToString());
        }
        public static void Fatal(string message)
        {
            globalLog.Error(message);
        }
    }
}
#else
namespace GN
{
    public static class Log
    {
        public static void Trace(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public static void Warning(string msg)
        {
            UnityEngine.Debug.LogWarning(msg);
        }

        public static void Info(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public static void Error(Exception e)
        {
            UnityEngine.Debug.LogError(e.ToString());
        }

        public static void Error(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }

        public static void Debug(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }
    }
}
#endif