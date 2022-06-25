namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class PaymentMethodController : ControllerBase
{
	private readonly PaymentMethodService _paymentMethodService;

	public PaymentMethodController(PaymentMethodService mediaService) => _paymentMethodService = mediaService;

	[HttpGet]
	public ActionResult<IEnumerable<string>> GetAll() => Ok(_paymentMethodService.GetAllIdentifiers());
}