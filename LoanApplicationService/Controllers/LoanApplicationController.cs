﻿using LoanApplicationService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LoanApplicationService.Controllers
{
    [ApiController]
    [Route("loanApplicationService")]
    public class LoanApplicationController : ControllerBase
    {
        private readonly LoanApplicationDbContext _loanApplicationDbContext;

        public LoanApplicationController(LoanApplicationDbContext loanApplicationDbContext)
        {
            _loanApplicationDbContext = loanApplicationDbContext;
        }

        [HttpGet]
        [Route("getLoanApplications")]
        public async Task<ActionResult<IEnumerable<LoanApplication>>> GetLoanApplications() { 
            return await _loanApplicationDbContext.LoanApplications.ToListAsync();
        }

        [HttpPut]
        [Route("updateLoanApplications")]
        public async Task<ActionResult<LoanApplication>> UpdateLoanApplication(LoanApplication application)
        {
            _loanApplicationDbContext.LoanApplications.Update(application);
            await _loanApplicationDbContext.SaveChangesAsync();

            var loanApplication = JsonSerializer.Serialize(new LoanApplication
            {
                Id = application.Id,
                Name = application.Name,
                LoanLimit = application.LoanLimit,
                Purpose = application.Purpose,
            });
            return CreatedAtAction("GetLoanApplications", new { application.Id }, application);
        }

        [HttpPost]
        [Route("submitLoanApplication")]
        public async Task<ActionResult<LoanApplication>> SubmitLoanApplication(LoanApplication application)
        {
            _loanApplicationDbContext.LoanApplications.Add(application);
            await _loanApplicationDbContext.SaveChangesAsync();

            var loanApplication = JsonSerializer.Serialize(new LoanApplication
            {
                Id = application.Id,
                Name = application.Name,
                LoanLimit = application.LoanLimit,
                Purpose = application.Purpose,
            });

            return CreatedAtAction("GetLoanApplications", new {application.Id}, application);
        }
    }
}
