namespace Solidarity.Application.Authentication;

public static class RSAExtensions
{
	public static readonly Encoding encoding = Encoding.ASCII;
	public static readonly RSAEncryptionPadding encryptionPadding = RSAEncryptionPadding.OaepSHA512;
	public static readonly RSASignaturePadding signaturePadding = RSASignaturePadding.Pkcs1;
	public static readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
	public static bool VerifyKeys(string privateKey, string publicKey)
	{
		try
		{
			var rsa = RSA.Create();
			rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

			return Convert.ToBase64String(rsa.ExportRSAPublicKey()) == publicKey;
		}
		catch
		{
			return false;
		}
	}

	public static string ExportRSAPublicKeyString(this RSA rsa)
	{
		return Convert.ToBase64String(rsa.ExportRSAPublicKey());
	}

	public static string ExportRSAPrivateKeyString(this RSA rsa)
	{
		return Convert.ToBase64String(rsa.ExportRSAPrivateKey());
	}

	public static string EncryptToString(this RSA rsa, string data)
	{
		return Convert.ToBase64String(rsa.Encrypt(encoding.GetBytes(data), encryptionPadding));
	}

	public static string SignDataToString(this RSA rsa, string data)
	{
		return Convert.ToBase64String(rsa.SignData(encoding.GetBytes(data), hashAlgorithm, signaturePadding));
	}

	public static string DecryptToString(this RSA rsa, string encryptedData)
	{
		return encoding.GetString(rsa.Decrypt(Convert.FromBase64String(encryptedData), encryptionPadding));
	}
}