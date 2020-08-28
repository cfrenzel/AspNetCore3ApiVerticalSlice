using Xunit;

using Banking.Api.Controllers;
using MediatR;
using FakeItEasy;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Banking.Api.Institutions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Banking.Api.UnitTests
{
    /// <summary>
    /// With vertical slice (or pretty much any "thin" controller), testing controller actions is trivial.
    /// The controller serves the purpose to route, bind models, validate, and serialize responses
    /// Since controller unit tests don't test any of those things;
    /// our tests aren't going to give us much confidence. 
    /// If we end up with a "thick" controller that has logic in it, then unit testing may serve us well
    /// I'll provide one example below to show how the test would
    /// simply confirm that we're sending the message.  After this we'll move
    /// on to some Integration tests that will give us higher confidence in the api
    /// </summary>
    public class InstitutionControllerTests
    {
        InstitutionController _controller;
        IMediator _mediator;
        ILogger<InstitutionController> _logger;
        
        public InstitutionControllerTests()
        {
            _logger = new NullLogger<InstitutionController>();
            _mediator = A.Fake<IMediator>();
            _controller = new InstitutionController(_mediator, _logger );
        }

        /// <summary>
        /// The test becomes meaningless whether you fake the result from the mediator
        /// or the services injected into the handler.  In ths case particularly because most of the 
        /// logic is in converting the input into an appropriate query.  A better test would be an end to
        /// end test on the api to test the full pipeline and a unit/integration test on the handler
        /// </summary>
        [Fact]
        public async Task Should_Return_InstitutionModels_On_List()
        {

            A.CallTo(() => _mediator.Send(A<List.Query>.Ignored, A<CancellationToken>.Ignored))
                .Returns(
                    Task<List<List.InstitutionModel>>.FromResult(new List<List.InstitutionModel>()
                        {
                        new List.InstitutionModel(){Id = 12345, Name = "Fake Institution" }
                    })
                 );
            // Act
            var res = await _controller.List(new List.Query()
            {
                 PageNumber = 1,
                 PageSize = 20
            });
            // Assert: this is meaningless
            Assert.IsType<List<List.InstitutionModel>>(res.Value);
        }
    }
}
