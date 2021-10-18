using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solidarity.Application.Services;
using Solidarity.Domain.Models;

namespace Solidarity.Controllers
{
	[ApiController, Route("[controller]"), Authorize]
	public class AccountController : ControllerBase
	{
		private readonly AccountService accountService;

		public AccountController(AccountService accountService) => this.accountService = accountService;

		[HttpGet]
		public ActionResult<Account> Get() => Ok(accountService.GetWithoutAuthentication(null));

		[HttpPost, AllowAnonymous]
		public ActionResult<string> Create([FromBody, Bind(nameof(Account.Username), nameof(Account.Identity.FirstName), nameof(Account.Identity.LastName), nameof(Account.Identity.BirthDate))] Account account)
			=> Ok(accountService.Create(account));

		[HttpPut]
		public ActionResult<Account> Update([FromBody, Bind(nameof(Account.Id), nameof(Account.Username), nameof(Account.Identity.FirstName), nameof(Account.Identity.LastName), nameof(Account.Identity.BirthDate))] Account account)
			=> Ok(accountService.Update(account));

		[HttpGet("{id}/reset"), AllowAnonymous]
		public ActionResult<string> Reset([FromRoute] int id) => Ok(accountService.Reset(id));

		[HttpGet("recover"), AllowAnonymous]
		public ActionResult<string> Recover([FromQuery] string phrase) => Ok(accountService.Recover(phrase));
	}
}