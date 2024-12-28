namespace Solidarity.Infrastructure.Payment;

public sealed class PaymentModule : Module
{
	public override void ConfigureServices(IServiceCollection services)
	{
		services.AddHealthChecks().AddCheck<PaymentMethodProvider>("Payment Method Provider");
		services.AddTransient<IPaymentMethodProvider, PaymentMethodProvider>();
	}
}