namespace Solidarity.Domain.Models;

public class CryptoMnemonic : Model
{
	public CryptoMnemonic(string mnemonic)
	{
		Mnemonic = mnemonic;
	}

	public string Mnemonic { get; set; }
}