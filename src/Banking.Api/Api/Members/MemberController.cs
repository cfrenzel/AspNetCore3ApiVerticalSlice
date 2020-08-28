using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using AutoMapper;
using MediatR;

using Banking.Api.Members;

namespace Banking.Api.Members
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MemberController : ControllerBase
    {
        private readonly ILogger _logger = null;
        private readonly IMediator _mediator;

        public MemberController(IMediator mediator,ILogger<MemberController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<List.MemberModel>>> List([FromQuery]List.Query query) => await _mediator.Send(query);


        [HttpGet("{id}")]
        public async Task<ActionResult<Find.MemberModel>> Find(int id)
        {
            var member = await _mediator.Send(new Find.Query(id));

            if (member == null)
                return NotFound();
            return member;
        }

        [HttpPost]
        public async Task<ActionResult<Create.MemberModel>> Create([FromBody] Create.Command command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newMember = await _mediator.Send(command);
            return CreatedAtAction(nameof(Find), new { id = newMember.Id }, newMember);
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Members.Update.Command command)
        {

            if (command.Id != id)
                ModelState.AddModelError("id", "id in uri must match message body");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var res = await _mediator.Send(command);
            if (res == null)
                return NotFound();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var res = await _mediator.Send(new Delete.Command(id));
            if (!res.HasValue)
                return NotFound();

            return NoContent();
        }



    }
}
