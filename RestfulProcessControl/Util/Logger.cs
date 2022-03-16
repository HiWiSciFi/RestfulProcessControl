namespace RestfulProcessControl.Util;

public static class Logger
{
	public static void Log(LogLevel level, string? message) =>
		Log<object?, object?, object?, object?, object?>(level, message, null, null, null, null, null);
	public static void Log<T0>(LogLevel level, string? message, T0? a0) =>
		Log<T0?, object?, object?, object?, object?>(level, message, a0, null, null, null, null);
	public static void Log<T0, T1>(LogLevel level, string? message, T0? a0, T1? a1) =>
		Log<T0?, T1?, object?, object?, object?>(level, message, a0, a1, null, null, null);
	public static void Log<T0, T1, T2>(LogLevel level, string? message, T0? a0, T1? a1, T2? a2) =>
		Log<T0?, T1?, T2?, object?, object?>(level, message, a0, a1, a2, null, null);
	public static void Log<T0, T1, T2, T3>(LogLevel level, string? message, T0? a0, T1? a1, T2? a2, T3? a3) =>
		Log<T0?, T1?, T2?, T3?, object?>(level, message, a0, a1, a2, a3, null);
	public static void Log<T0, T1, T2, T3, T4>(LogLevel level, string? message, T0? a0, T1? a1, T2? a2, T3? a3, T4? a4)
	{
		message ??= string.Empty;
		switch (level)
		{
			case LogLevel.Information:
				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.Write("info");
				Console.ResetColor();
				Console.Write(": ");
				Console.WriteLine(message, a0, a1, a2, a3, a4);
				break;
			case LogLevel.Debug:
				Console.Write("dbug: ");
				Console.WriteLine(message, a0, a1, a2, a3, a4);
				break;
			case LogLevel.Trace:
				Console.Write("trce: ");
				Console.WriteLine(message, a0, a1, a2, a3, a4);
				break;
			case LogLevel.Warning:
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.Write("warn");
				Console.ResetColor();
				Console.Write(": ");
				Console.WriteLine(message, a0, a1, a2, a3, a4);
				break;
			case LogLevel.Error:
				Console.ForegroundColor = ConsoleColor.Black;
				Console.BackgroundColor = ConsoleColor.DarkRed;
				Console.Write("fail");
				Console.ResetColor();
				Console.Write(": ");
				Console.WriteLine(message, a0, a1, a2, a3, a4);
				break;
			case LogLevel.Critical:
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.BackgroundColor = ConsoleColor.DarkRed;
				Console.Write("crit");
				Console.ResetColor();
				Console.Write(": ");
				Console.WriteLine(message, a0, a1, a2, a3, a4);
				break;
			case LogLevel.None:
				Console.WriteLine(message, a0, a1, a2, a3, a4);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(level), level, null);
		}
	}

	public static void LogInformation(string? message) =>
		Log<object?, object?, object?, object?, object?>(LogLevel.Information, message, null, null, null, null, null);
	public static void LogInformation<T0>(string? message, T0? a0) =>
		Log<T0?, object?, object?, object?, object?>(LogLevel.Information, message, a0, null, null, null, null);
	public static void LogInformation<T0, T1>(string? message, T0? a0, T1? a1) =>
		Log<T0?, T1?, object?, object?, object?>(LogLevel.Information, message, a0, a1, null, null, null);
	public static void LogInformation<T0, T1, T2>(string? message, T0? a0, T1? a1, T2? a2) =>
		Log<T0?, T1?, T2?, object?, object?>(LogLevel.Information, message, a0, a1, a2, null, null);
	public static void LogInformation<T0, T1, T2, T3>(string? message, T0? a0, T1? a1, T2? a2, T3? a3) =>
		Log<T0?, T1?, T2?, T3?, object?>(LogLevel.Information, message, a0, a1, a2, a3, null);
	public static void LogInformation<T0, T1, T2, T3, T4>(string? message, T0? a0, T1? a1, T2? a2, T3? a3, T4? a4) =>
		Log<T0?, T1?, T2?, T3?, T4?>(LogLevel.Information, message, a0, a1, a2, a3, a4);

	public static void LogDebug(string? message) =>
		Log<object?, object?, object?, object?, object?>(LogLevel.Debug, message, null, null, null, null, null);
	public static void LogDebug<T0>(string? message, T0? a0) =>
		Log<T0?, object?, object?, object?, object?>(LogLevel.Debug, message, a0, null, null, null, null);
	public static void LogDebug<T0, T1>(string? message, T0? a0, T1? a1) =>
		Log<T0?, T1?, object?, object?, object?>(LogLevel.Debug, message, a0, a1, null, null, null);
	public static void LogDebug<T0, T1, T2>(string? message, T0? a0, T1? a1, T2? a2) =>
		Log<T0?, T1?, T2?, object?, object?>(LogLevel.Debug, message, a0, a1, a2, null, null);
	public static void LogDebug<T0, T1, T2, T3>(string? message, T0? a0, T1? a1, T2? a2, T3? a3) =>
		Log<T0?, T1?, T2?, T3?, object?>(LogLevel.Debug, message, a0, a1, a2, a3, null);
	public static void LogDebug<T0, T1, T2, T3, T4>(string? message, T0? a0, T1? a1, T2? a2, T3? a3, T4? a4) =>
		Log<T0?, T1?, T2?, T3?, T4?>(LogLevel.Debug, message, a0, a1, a2, a3, a4);

	public static void LogTrace(string? message) =>
		Log<object?, object?, object?, object?, object?>(LogLevel.Trace, message, null, null, null, null, null);
	public static void LogTrace<T0>(string? message, T0? a0) =>
		Log<T0?, object?, object?, object?, object?>(LogLevel.Trace, message, a0, null, null, null, null);
	public static void LogTrace<T0, T1>(string? message, T0? a0, T1? a1) =>
		Log<T0?, T1?, object?, object?, object?>(LogLevel.Trace, message, a0, a1, null, null, null);
	public static void LogTrace<T0, T1, T2>(string? message, T0? a0, T1? a1, T2? a2) =>
		Log<T0?, T1?, T2?, object?, object?>(LogLevel.Trace, message, a0, a1, a2, null, null);
	public static void LogTrace<T0, T1, T2, T3>(string? message, T0? a0, T1? a1, T2? a2, T3? a3) =>
		Log<T0?, T1?, T2?, T3?, object?>(LogLevel.Trace, message, a0, a1, a2, a3, null);
	public static void LogTrace<T0, T1, T2, T3, T4>(string? message, T0? a0, T1? a1, T2? a2, T3? a3, T4? a4) =>
		Log<T0?, T1?, T2?, T3?, T4?>(LogLevel.Trace, message, a0, a1, a2, a3, a4);

	public static void LogWarning(string? message) =>
		Log<object?, object?, object?, object?, object?>(LogLevel.Warning, message, null, null, null, null, null);
	public static void LogWarning<T0>(string? message, T0? a0) =>
		Log<T0?, object?, object?, object?, object?>(LogLevel.Warning, message, a0, null, null, null, null);
	public static void LogWarning<T0, T1>(string? message, T0? a0, T1? a1) =>
		Log<T0?, T1?, object?, object?, object?>(LogLevel.Warning, message, a0, a1, null, null, null);
	public static void LogWarning<T0, T1, T2>(string? message, T0? a0, T1? a1, T2? a2) =>
		Log<T0?, T1?, T2?, object?, object?>(LogLevel.Warning, message, a0, a1, a2, null, null);
	public static void LogWarning<T0, T1, T2, T3>(string? message, T0? a0, T1? a1, T2? a2, T3? a3) =>
		Log<T0?, T1?, T2?, T3?, object?>(LogLevel.Warning, message, a0, a1, a2, a3, null);
	public static void LogWarning<T0, T1, T2, T3, T4>(string? message, T0? a0, T1? a1, T2? a2, T3? a3, T4? a4) =>
		Log<T0?, T1?, T2?, T3?, T4?>(LogLevel.Warning, message, a0, a1, a2, a3, a4);

	public static void LogError(string? message) =>
		Log<object?, object?, object?, object?, object?>(LogLevel.Error, message, null, null, null, null, null);
	public static void LogError<T0>(string? message, T0? a0) =>
		Log<T0?, object?, object?, object?, object?>(LogLevel.Error, message, a0, null, null, null, null);
	public static void LogError<T0, T1>(string? message, T0? a0, T1? a1) =>
		Log<T0?, T1?, object?, object?, object?>(LogLevel.Error, message, a0, a1, null, null, null);
	public static void LogError<T0, T1, T2>(string? message, T0? a0, T1? a1, T2? a2) =>
		Log<T0?, T1?, T2?, object?, object?>(LogLevel.Error, message, a0, a1, a2, null, null);
	public static void LogError<T0, T1, T2, T3>(string? message, T0? a0, T1? a1, T2? a2, T3? a3) =>
		Log<T0?, T1?, T2?, T3?, object?>(LogLevel.Error, message, a0, a1, a2, a3, null);
	public static void LogError<T0, T1, T2, T3, T4>(string? message, T0? a0, T1? a1, T2? a2, T3? a3, T4? a4) =>
		Log<T0?, T1?, T2?, T3?, T4?>(LogLevel.Error, message, a0, a1, a2, a3, a4);

	public static void LogCritical(string? message) =>
		Log<object?, object?, object?, object?, object?>(LogLevel.Critical, message, null, null, null, null, null);
	public static void LogCritical<T0>(string? message, T0? a0) =>
		Log<T0?, object?, object?, object?, object?>(LogLevel.Critical, message, a0, null, null, null, null);
	public static void LogCritical<T0, T1>(string? message, T0? a0, T1? a1) =>
		Log<T0?, T1?, object?, object?, object?>(LogLevel.Critical, message, a0, a1, null, null, null);
	public static void LogCritical<T0, T1, T2>(string? message, T0? a0, T1? a1, T2? a2) =>
		Log<T0?, T1?, T2?, object?, object?>(LogLevel.Critical, message, a0, a1, a2, null, null);
	public static void LogCritical<T0, T1, T2, T3>(string? message, T0? a0, T1? a1, T2? a2, T3? a3) =>
		Log<T0?, T1?, T2?, T3?, object?>(LogLevel.Critical, message, a0, a1, a2, a3, null);
	public static void LogCritical<T0, T1, T2, T3, T4>(string? message, T0? a0, T1? a1, T2? a2, T3? a3, T4? a4) =>
		Log<T0?, T1?, T2?, T3?, T4?>(LogLevel.Critical, message, a0, a1, a2, a3, a4);
}