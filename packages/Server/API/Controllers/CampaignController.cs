using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solidarity.Application.Services;
using Solidarity.Domain.Models;
using System.Collections.Generic;

namespace Solidarity.Controllers
{
	[ApiController, Route("[controller]"), Authorize]
	public class CampaignController : ControllerBase
	{
		private CampaignService CampaignService { get; }

		public CampaignController(CampaignService campainService) => CampaignService = campainService;

		[HttpGet, AllowAnonymous]
		public ActionResult<IEnumerable<Campaign>> GetAll() => Ok(CampaignService.GetAll());

		[HttpGet("{id}"), AllowAnonymous]
		public ActionResult<string> Get([FromRoute] int id) => Ok(CampaignService.Get(id));

		[HttpGet("{id}/balance"), AllowAnonymous]
		public ActionResult<decimal> GetBalance([FromRoute] int id) => Ok(CampaignService.GetBalance(id));

		[HttpPost]
		public ActionResult<Campaign> Create([FromBody, Bind(nameof(Campaign.CreatorId), nameof(Campaign.Description), nameof(Campaign.Title))] Campaign campaign)
			=> Ok(CampaignService.Create(campaign));

		[HttpPut("{id}")]
		public ActionResult<Campaign> Update([FromRoute] int id, [FromBody, Bind(nameof(Campaign.Id), nameof(Campaign.Description), nameof(Campaign.Title))] Campaign campaign)
			=> Ok(CampaignService.Update(id, campaign));
	}
}