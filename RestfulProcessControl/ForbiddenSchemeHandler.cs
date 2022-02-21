using Microsoft.AspNetCore.Authentication;

namespace RestfulProcessControl;

public class ForbiddenSchemeHandler : IAuthenticationHandler
{
	private HttpContext? _context;

	public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
	{
		_context = context;
		return Task.CompletedTask;
	}

	public Task<AuthenticateResult> AuthenticateAsync() => Task.FromResult(AuthenticateResult.NoResult());

	public Task ChallengeAsync(AuthenticationProperties? properties)
	{
		return Task.FromResult(AuthenticateResult.NoResult());
	}

	public Task ForbidAsync(AuthenticationProperties? properties)
	{
		properties = properties ?? new AuthenticationProperties();
		if (_context != null) _context.Response.StatusCode = 403;
		return Task.CompletedTask;
	}
}