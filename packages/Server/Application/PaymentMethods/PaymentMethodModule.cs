namespace Solidarity.Application.PaymentMethods;

public sealed class PaymentMethodModule : Module
{
	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/paymentMethod", (IPaymentMethodProvider paymentMethodProvider) => paymentMethodProvider.GetAll().Select(pm => pm.Identifier));
		endpoints.MapGet("/api/paymentMethod/{identifier}/is-allocation-destination-valid/{allocationDestination}",
			(IPaymentMethodProvider paymentMethodProvider, string identifier, string allocationDestination)
				=> paymentMethodProvider.Get(identifier).IsAllocationDestinationValid(allocationDestination));
	}
}