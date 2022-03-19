namespace RestfulProcessControl.Util;

public static class UnixTime
{
	/// <summary>
	/// Unix Epoch
	/// </summary>
	public static int UnixEpoch => 0;

	/// <summary>
	/// The amount of seconds since Unix Epoch (Recalculated on call)
	/// </summary>
	public static int Now => (int)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
}