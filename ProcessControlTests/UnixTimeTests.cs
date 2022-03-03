using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulProcessControl.Models;

namespace ProcessControlTests;

[TestClass]
public class UnixTimeTests
{
	[TestMethod]
	public void NowTest()
	{
		var first = UnixTime.Now;
		Thread.Sleep(1000);
		var second = UnixTime.Now;
		Assert.AreEqual(first+1, second, 0.1);
	}
}