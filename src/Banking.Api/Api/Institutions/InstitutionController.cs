using System.Collections.Generic;
using System.Security.Principal;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MediatR;
using AutoMapper;

using Banking.Core.Entities;
using Banking.Api.Institutions;

namespace Banking.Api.Controllers
{


    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class InstitutionController : ControllerBase
    {
        private readonly ILogger _logger = null;
        private readonly IMediator _mediator;

        public InstitutionController(IMediator mediator, ILogger<InstitutionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<List.InstitutionModel>>> List([FromQuery]List.Query query) => await _mediator.Send(query);


        [HttpGet("{id}")]
        public async Task<ActionResult<Find.InstitutionModel>> Find(int id)
        {
            var inst = await _mediator.Send(new Find.Query(id));

            if(inst == null)
                return NotFound();
            return inst;
        }


        [HttpPost]
        public async Task<ActionResult<Create.InstitutionModel>> Create(ApiVersion version, [FromBody] Create.Command command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newInstitution = await _mediator.Send(command);
            return CreatedAtAction(nameof(Find), new { id = newInstitution.Id, version = $"{version}" }, newInstitution );
        }


    }
}
