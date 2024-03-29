﻿namespace Solidarity.Application.Authentication;

public abstract class AuthenticationMethod : Model
{
	public int AccountId { get; set; }

	public Account? Account { get; set; }

	public virtual AuthenticationMethodType Type { get; }

	public virtual bool SupportsMultiple => false;

	public int Salt { get; private set; }

	public string Data { get; set; } = null!;

	public bool Authenticate(string data) => Data == GetEncrypted(data);

	public void Encrypt()
	{
		Data = GetEncrypted(Data);
		Salt = SupportsMultiple ? new Random().Next(1, 99999) : 0;
	}

	protected abstract string GetEncrypted(string data);
}

public static class AuthenticationExtensions
{
	public static T WithoutData<T>(this T auth) where T : AuthenticationMethod
	{
		auth.Data = null!;
		return auth;
	}
}