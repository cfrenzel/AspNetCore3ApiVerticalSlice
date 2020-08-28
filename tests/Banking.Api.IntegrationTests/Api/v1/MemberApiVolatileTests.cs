using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using Banking.Api.Institutions;

namespace Bank.Api.IntegrationTests
{
    public class MemberApiVolatileTests : IClassFixture<MemberApiTests.Fixture>
    {
        private readonly MemberApiTests.Fixture _fixture;
        private readonly ITestOutputHelper _output;
        public MemberApiVolatileTests(MemberApiTests.Fixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        public class Fixture : ApiIntegrationTestFixture
        {
            public Fixture() : base("integrationTestData2.json"){}
        }


        [Fact]
        public async Task Should_Return_Created_On_Member_Create()
        {
            var memerData = new
            {
                id = 3335,
                givenName = "Joan",
                surName = "Smith",
                institutionId = 78923,
            };

            // Act
            var response = await _fixture.Client.PostAsync("/api/v1/member",
                new StringContent(JsonConvert.SerializeObject(memerData), Encoding.UTF8, "application/json")
            );

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
           
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.True(response.Headers.Contains("Location"));
            Assert.Contains("api/v1/member/3335", response.Headers.Location.AbsoluteUri.ToLower());
        }
    }
}
