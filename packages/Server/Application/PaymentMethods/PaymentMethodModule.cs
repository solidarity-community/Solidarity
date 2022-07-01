namespace Solidarity.Application.PaymentMethods;

public class PaymentMethodModule : Module
{
	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/paymentMethod", (PaymentMethodService paymentMethodService) => paymentMethodService.GetAllIdentifiers());
	}
}