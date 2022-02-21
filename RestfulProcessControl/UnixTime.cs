namespace RestfulProcessControl;

public static class UnixTime
{
	/// <summary>
	/// The amount of seconds since Unix Epoch (Recalculated on call)
	/// </summary>
	public static int Now => (int)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
}