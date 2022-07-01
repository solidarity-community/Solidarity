namespace Solidarity.Infrastructure.Payment;

public class PaymentModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
		=> services.AddHealthChecks().AddCheck<PaymentMethodProvider>("Payment Method Provider");
}