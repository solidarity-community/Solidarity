namespace Solidarity.Application.PaymentMethods;

public class PaymentMethodModule : Module
{
	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/paymentMethod", (PaymentMethodService paymentMethodService) => paymentMethodService.GetAllIdentifiers());

		endpoints.MapGet("/paymentMethod/{identifier}/is-allocation-destination-valid/{allocationDestination}",
			(PaymentMethodService paymentMethodService, string identifier, string allocationDestination)
				=> paymentMethodService.IsAllocationDestinationValid(identifier, allocationDestination));
	}
}