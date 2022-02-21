using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulProcessControl.Models;

namespace ProcessControlTests.ModelTests;

[TestClass]
public class UserModelTests
{
	[TestMethod]
	public void Serialization()
	{
		UserModel um1 = new()
		{
			Username = "08793=)/§§$",
			Password = "(/&$)!=$oiaehfoa128476",
			Role = "soieljhfois"
		};
		var s = um1.Serialize();
		var um2 = UserModel.Deserialize(s);
		Assert.IsNotNull(um2);
		Assert.AreEqual(um1.Username, um2.Username);
		Assert.AreEqual(um1.Password, um2.Password);
		Assert.AreEqual(um1.Role, um2.Role);
	}
}