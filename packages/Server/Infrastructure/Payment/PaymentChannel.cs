namespace Solidarity.Infrastructure.Payment;

public abstract class PaymentChannel
{
	public abstract Task<decimal> GetBalance();
	public abstract Task Transfer(string destination, decimal amount);
}