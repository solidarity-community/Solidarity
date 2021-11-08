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
		private readonly CampaignService campaignService;

		public CampaignController(CampaignService campainService) => campaignService = campainService;

		[HttpGet, AllowAnonymous]
		public ActionResult<IEnumerable<Campaign>> GetAll() => Ok(campaignService.GetAll());

		[HttpGet("{id}"), AllowAnonymous]
		public ActionResult<string> Get([FromRoute] int id) => Ok(campaignService.Get(id));

		[HttpGet("{id}/balance"), AllowAnonymous]
		public ActionResult<decimal> GetBalance([FromRoute] int id) => Ok(campaignService.GetBalance(id));

		[HttpPost]
		public ActionResult<Campaign> Create([FromBody, Bind(nameof(Campaign.CreatorId), nameof(Campaign.Description), nameof(Campaign.Title))] Campaign campaign)
			=> Ok(campaignService.Create(campaign));

		[HttpPut("{id}")]
		public ActionResult<Campaign> Update([FromBody, Bind(nameof(Campaign.Id), nameof(Campaign.Description), nameof(Campaign.Title))] Campaign campaign)
			=> Ok(campaignService.Update(campaign));
	}
}