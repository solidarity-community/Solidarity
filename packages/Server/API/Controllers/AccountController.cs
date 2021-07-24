using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solidarity.Application.Services;
using Solidarity.Domain.Models;

namespace Solidarity.Controllers
{
	[ApiController, Route("[controller]"), Authorize]
	public class AccountController : ControllerBase
	{
		private AccountService AccountService { get; }

		public AccountController(AccountService accountService) => AccountService = accountService;

		[HttpGet]
		public ActionResult<Account> Get() => Ok(AccountService.GetWithoutAuthentication(null));

		[HttpPost, AllowAnonymous]
		public ActionResult<string> Create([FromBody, Bind(nameof(Account.Username), nameof(Account.Identity.FirstName), nameof(Account.Identity.LastName), nameof(Account.Identity.BirthDate))] Account account)
			=> Ok(AccountService.Create(account));

		[HttpPut]
		public ActionResult<Account> Update([FromBody, Bind(nameof(Account.Id), nameof(Account.Username), nameof(Account.Identity.FirstName), nameof(Account.Identity.LastName), nameof(Account.Identity.BirthDate))] Account account)
			=> Ok(AccountService.Update(account));

		[HttpGet("reset"), AllowAnonymous]
		public ActionResult<string> Reset([FromRoute] int id) => Ok(AccountService.Reset(id));

		[HttpGet("recover"), AllowAnonymous]
		public ActionResult<string> Recover([FromQuery] string phrase) => Ok(AccountService.Recover(phrase));
	}
}