using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using AutoMapper;
using MediatR;

using Banking.Api.Accounts;
using Microsoft.AspNetCore.Http;

namespace Banking.Api.Controllers
{


    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger _logger = null;
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator, ILogger<AccountController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpPost("{id}/transfer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Transfer.Response>> Transfer(Int32 id, [FromBody] Transfer.Command command)
        {
            if (command.SourceAccountId != id)
                ModelState.AddModelError("id", "id in uri must match message body");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _mediator.Send(command);
        }

   


    }
}
