namespace Solidarity.Infrastructure.Payment.Bitcoin;

public record struct BitcoinAllocation(BitcoinAddress bitcoinAddress, Money amount)
{
	public static implicit operator (BitcoinAddress bitcoinAddress, Money amount)(BitcoinAllocation value)
	{
		return (value.bitcoinAddress, value.amount);
	}

	public static implicit operator BitcoinAllocation((BitcoinAddress bitcoinAddress, Money amount) value)
	{
		return new BitcoinAllocation(value.bitcoinAddress, value.amount);
	}
}