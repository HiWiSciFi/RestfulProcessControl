using System.Security.Cryptography;
using System.Text;

namespace RestfulProcessControl;

public static class Authenticator
{
	private static IEnumerable<string> JWTs;
	private static Dictionary<string, string> users;

	static Authenticator() {
		JWTs = new List<string>();
		users = new Dictionary<string, string>();
	}

	public static string? Authenticate(string username, string password) {
		if (!users.ContainsKey(username)) return null;
		string hashedPW = GetStringHash(password);
		if (users[username] == hashedPW) return CreateJWT();
		return null;
	}

	public static bool IsTokenValid(string token) {
		return JWTs.Contains(token);
	}

	private static string GetStringHash(string input) {
		using HashAlgorithm algorithm = SHA256.Create();
		byte[] buffer = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
		return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
	}

	private static string CreateJWT() {
		string token = string.Empty;
		JWTs.Append(token);
		return token;
	}
}