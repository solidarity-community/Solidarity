namespace Solidarity.Installers;

public class PaymentMethodsInstaller : IInstaller
{
	public void Install(IServiceCollection services)
		=> services.AddTransient<IPaymentMethodProvider, PaymentMethodProvider>();
}