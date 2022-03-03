namespace RestfulProcessControl;

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
}