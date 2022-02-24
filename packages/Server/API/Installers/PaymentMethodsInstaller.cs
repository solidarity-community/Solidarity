namespace Solidarity.Installers;

public class PaymentMethodsInstaller : IInstaller
{
	public void Install(IServiceCollection services)
	{
		services.AddScoped<IPaymentMethodProvider, PaymentMethodProvider>();
	}
}