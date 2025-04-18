﻿using LoanApplicationService.Models;
using LoanApplicationService.RabbitMq;
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
        private readonly IRabbitMqUtil _rabbitMqUtil;

        public LoanApplicationController(LoanApplicationDbContext loanApplicationDbContext, IRabbitMqUtil rabbitMqUtil)
        {
            _loanApplicationDbContext = loanApplicationDbContext;
            _rabbitMqUtil = rabbitMqUtil;
        }

        [HttpGet]
        [Route("getLoanApplications")]
        public async Task<ActionResult<IEnumerable<LoanApplication>>> GetLoanApplications() { 
            return await _loanApplicationDbContext.LoanApplications.ToListAsync();
        }

        [HttpPut]
        [Route("updateLoanApplicationApproval")]
        public async Task<ActionResult<LoanApplication>> UpdateLoanApplicationApproval(Guid applicationId, bool approvalStatus)
        {
            var application = await _loanApplicationDbContext.LoanApplications
                .FirstOrDefaultAsync(l => l.Id == applicationId);
            application.Approved = approvalStatus;
            _loanApplicationDbContext.LoanApplications.Update(application);
            await _loanApplicationDbContext.SaveChangesAsync();

            var loanApplication = JsonSerializer.Serialize(new LoanApplication
            {
                Id = application.Id,
                Name = application.Name,
                LoanLimit = application.LoanLimit,
                Purpose = application.Purpose,
                CustomerId = application.CustomerId,
                Approved = application.Approved,
                Cancelled = application.Cancelled
            });
            await _rabbitMqUtil.PublishMessageQueue("loanEvaluation.loanApplication", loanApplication);
            return CreatedAtAction("GetLoanApplications", new { application.Id }, application);
        }

        [HttpPost]
        [Route("submitLoanApplication")]
        public async Task<ActionResult<LoanApplication>> SubmitLoanApplication(string name, int limit, string purpose, Guid customerId)
        {
            var application = new LoanApplication
            {
                Id = Guid.NewGuid(),
                Name = name,
                LoanLimit = limit,
                Purpose = purpose,
                CustomerId = customerId,
                Approved = false,
                Cancelled = false
            };
            _loanApplicationDbContext.LoanApplications.Add(application);
            await _loanApplicationDbContext.SaveChangesAsync();

            var loanApplication = JsonSerializer.Serialize(new LoanApplication
            {
                Id = application.Id,
                Name = application.Name,
                LoanLimit = application.LoanLimit,
                Purpose = application.Purpose,
                CustomerId = application.CustomerId,
                Approved = application.Approved,
                Cancelled = application.Cancelled
            });

            await _rabbitMqUtil.PublishMessageQueue("loanEvaluation.loanApplication", loanApplication);

            return CreatedAtAction("GetLoanApplications", new {application.Id}, application);
        }
    }
}
