using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
#if USE_DEBUG
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    public static void Log(object message, Object context)
    {
        Debug.Log(message, context);
    }

    public static void LogFormat(string message, params object[] args)
    {
        Debug.LogFormat(message, args);
    }

    public static void LogFormat(Object context, string message, params object[] args)
    {
        Debug.LogFormat(context, message, args);
    }

    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }

    public static void LogWarning(object message, Object context)
    {
        Debug.LogWarning(message, context);
    }

    public static void LogWarningFormat(string message, params object[] args)
    {
        Debug.LogWarningFormat(message, args);
    }

    public static void LogWarningFormat(Object context, string message, params object[] args)
    {
        Debug.LogWarningFormat(context, message, args);
    }

    public static void LogError(object message)
    {
        Debug.LogError(message);
    }

    public static void LogError(object message, Object context)
    {
        Debug.LogError(message, context);
    }

    public static void LogErrorFormat(string message, params object[] args)
    {
        Debug.LogErrorFormat(message, args);
    }

    public static void LogErrorFormat(Object context, string message, params object[] args)
    {
        Debug.LogErrorFormat(context, message, args);
    }

    public static void LogException(System.Exception exception)
    {
        Debug.LogException(exception);
    }

    public static void LogException(System.Exception exception, Object context)
    {
        Debug.LogException(exception, context);
    }
    
    public static void Assert(bool condition)
    {
        Debug.Assert(condition);
    }

    public static void Assert(bool condition, Object context)
    {
        Debug.Assert(condition, context);
    }

    public static void AssertFormat(bool condition, string message, params object[] args)
    {
        Debug.AssertFormat(condition, message, args);
    }

    public static void AssertFormat(bool condition, Object context, string message, params object[] args)
    {
        Debug.AssertFormat(condition, context, message, args);
    }
#else
    public static void Log(object message) { }
    public static void Log(object message, Object context) { }
    public static void LogFormat(string message, params object[] args) { }
    public static void LogFormat(Object context, string message, params object[] args) { }
    public static void LogWarning(object message) { }
    public static void LogWarning(object message, Object context) { }
    public static void LogWarningFormat(string message, params object[] args) { }
    public static void LogWarningFormat(Object context, string message, params object[] args) { }
    public static void LogError(object message) { }
    public static void LogError(object message, Object context) { }
    public static void LogErrorFormat(string message, params object[] args) { }
    public static void LogErrorFormat(Object context, string message, params object[] args) { }
    public static void LogException(System.Exception exception) { }
    public static void LogException(System.Exception exception, Object context) { }
    public static void Assert(bool condition) { }
    public static void Assert(bool condition, Object context) { }
    public static void AssertFormat(bool condition, string message, params object[] args) { }
    public static void AssertFormat(bool condition, Object context, string message, params object[] args) { }
#endif
}
