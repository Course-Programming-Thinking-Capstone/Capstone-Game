using System.Diagnostics;

public static class Logger
{
	[Conditional("ENABLE_LOGS")]
	public static void Debug(object message) => UnityEngine.Debug.Log(message);

	[Conditional("ENABLE_LOGS")]
	public static void Warning(object message) => UnityEngine.Debug.LogWarning(message);

	[Conditional("ENABLE_LOGS")]
	public static void Error(object message) => UnityEngine.Debug.LogError(message);

	[Conditional("ENABLE_LOGS")]
	public static void Exception(System.Exception exception) => UnityEngine.Debug.LogException(exception);
}
