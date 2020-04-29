using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;
using Tenatus.API.Data;
using Tenatus.API.Extensions;
using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Controllers
{
    public class StrategyController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public StrategyController(UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext, IMapper mapper)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetStrategies()
        {
            try
            {
                var user = await _userManager.GetApplicationUserAsync(User);
                var strategies = await _dbContext.Strategies.Where(x => x.UserId == user.Id).ToListAsync();
                var response = _mapper.Map<IEnumerable<Strategy>, IEnumerable<StrategyModel>>(strategies);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddStrategy([FromBody] StrategyModel request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var service = new StrategyService(_dbContext, _mapper);
                var user = await _userManager.GetApplicationUserAsync(User);
                service.AddStrategy(user, request);
                await _dbContext.SaveChangesAsync();
                
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditStrategy([FromBody] StrategyModel request)
        {
            try
            {
                var service = new StrategyService(_dbContext, _mapper);
                service.EditStrategy(request);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStrategy(int id)
        {
            try
            {
                var strategy = _dbContext.Strategies.Single(x => x.Id == id);
                _dbContext.Strategies.Remove(strategy);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}