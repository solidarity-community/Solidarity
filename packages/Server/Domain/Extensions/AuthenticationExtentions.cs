namespace Solidarity.Domain.Extensions;

public static class AuthenticationExtentions
{
	public static T WithoutData<T>(this T auth) where T : AuthenticationMethod
	{
		auth.Data = null!;
		return auth;
	}

	public static IEnumerable<T> WithoutData<T>(this IEnumerable<T> authenticationMethods) where T : AuthenticationMethod
	{
		foreach (var auth in authenticationMethods)
		{
			auth.WithoutData();
		}

		return authenticationMethods;
	}
}