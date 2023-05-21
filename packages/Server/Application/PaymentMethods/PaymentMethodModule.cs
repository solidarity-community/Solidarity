namespace Solidarity.Application.PaymentMethods;

public class PaymentMethodModule : Module
{
	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/paymentMethod", (IPaymentMethodProvider paymentMethodProvider) => paymentMethodProvider.GetAll().Select(pm => pm.Identifier));

		endpoints.MapGet("/paymentMethod/{identifier}/is-allocation-destination-valid/{allocationDestination}",
			(IPaymentMethodProvider paymentMethodProvider, string identifier, string allocationDestination)
				=> paymentMethodProvider.Get(identifier).IsAllocationDestinationValid(allocationDestination));
	}
}