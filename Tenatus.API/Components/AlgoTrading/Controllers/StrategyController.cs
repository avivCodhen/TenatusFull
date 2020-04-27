using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tenatus.API.Components.AlgoTrading.Models;
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
                
                var user = await _userManager.GetApplicationUserAsync(User);
                Strategy strategy;
                switch (request.Type)
                {
                    case AppConstants.StrategyTypePercent:
                        strategy = _mapper.Map<StrategyModel, PercentStrategy>(request);
                        break;
                    case AppConstants.StrategyTypeRange:
                        strategy = _mapper.Map<StrategyModel, RangeStrategy>(request);
                        break;
                    default:
                        throw new Exception($"Unknown type: {request.Type}");
                }

                strategy.UserId = user.Id;
                _dbContext.Strategies.Add(strategy);

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
                var user = await _userManager.GetApplicationUserAsync(User);

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
                var user = await _userManager.GetApplicationUserAsync(User);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}