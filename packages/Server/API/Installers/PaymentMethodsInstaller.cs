namespace Solidarity.Installers;

public class PaymentMethodsInstaller : IInstaller
{
	public void Install(IServiceCollection services)
	{
		services.AddTransient<IPaymentMethodProvider, PaymentMethodProvider>();
		services.AddHealthChecks().AddCheck<PaymentMethodProvider>("Payment Method Provider");
	}
}