using Microsoft.AspNetCore.Mvc;

namespace RestfulProcessControl;

public class Logger : ControllerBase
{
	private static ILogger<Logger> _logger;

	public Logger(ILogger<Logger> logger) => _logger = logger;

	public static void Log<T0>(LogLevel level, string? message, T0? a0)
	{
		if (message is null) return;
		switch (level)
		{
			case LogLevel.Information:
				_logger.LogInformation(message, a0);
				break;
			case LogLevel.Debug:
				_logger.LogDebug(message, a0);
				break;
			case LogLevel.Trace:
				_logger.LogTrace(message, a0);
				break;
			case LogLevel.Warning:
				_logger.LogWarning(message, a0);
				break;
			case LogLevel.Error:
				_logger.LogError(message, a0);
				break;
			case LogLevel.Critical:
				_logger.LogCritical(message, a0);
				break;
			case LogLevel.None:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(level), level, null);
		}
	}

	public static void Log<T0, T1>(LogLevel level, string? message, T0? a0, T1? a1)
	{
		if (message is null) return;
		switch (level)
		{
			case LogLevel.Information:
				_logger.LogInformation(message, a0, a1);
				break;
			case LogLevel.Debug:
				_logger.LogDebug(message, a0, a1);
				break;
			case LogLevel.Trace:
				_logger.LogTrace(message, a0, a1);
				break;
			case LogLevel.Warning:
				_logger.LogWarning(message, a0, a1);
				break;
			case LogLevel.Error:
				_logger.LogError(message, a0, a1);
				break;
			case LogLevel.Critical:
				_logger.LogCritical(message, a0, a1);
				break;
			case LogLevel.None:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(level), level, null);
		}
	}
}