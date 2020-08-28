using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using Banking.Api.Institutions;

namespace Bank.Api.IntegrationTests
{
    public class AccountApiTests : IClassFixture<AccountApiTests.Fixture>
    {
        private readonly AccountApiTests.Fixture _fixture;
        private readonly ITestOutputHelper _output;
        public AccountApiTests(AccountApiTests.Fixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        public class Fixture : ApiIntegrationTestFixture
        {
            public Fixture() : base("integrationTestData1.json"){}
        }

       
        [Fact]
        public async Task Should_Transfer_When_Sufficient_Balance()
        {
            var transferData = new
            {
                sourceAccountId = 23456,
                beneficiaryAccountId = 23457,
                transferAmount = 0.33M
            };

            // Act
            var response = await _fixture.Client.PostAsync("/api/v1/account/23456/transfer",
                new StringContent(JsonConvert.SerializeObject(transferData), Encoding.UTF8, "application/json")
            );

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            response.EnsureSuccessStatusCode();

            /********** Json result **********/
            dynamic result = JsonConvert.DeserializeObject(contentString);

            //"sourceAccountId":23456,"sourceAccountOriginalBalance":12.5,"sourceAccountBalance":12.17,"beneficiaryAccountId":23457,"transferAmount":0.33,"transferedAtUtc":"2020-08-27T23:04:00.3252414Z"}
            Assert.Equal(JTokenType.Integer, result.sourceAccountId.Type);
            Assert.Equal(23456, result.sourceAccountId.Value);

            Assert.Equal(JTokenType.Integer, result.beneficiaryAccountId.Type);
            Assert.Equal(23457, result.beneficiaryAccountId.Value);

            Assert.Equal(JTokenType.Float, result.sourceAccountOriginalBalance.Type);
            Assert.Equal(12.50, result.sourceAccountOriginalBalance.Value);

            Assert.Equal(JTokenType.Float, result.sourceAccountBalance.Type);
            Assert.Equal(12.17, result.sourceAccountBalance.Value);

            Assert.Equal(JTokenType.Float, result.transferAmount.Type);
            Assert.Equal(.33, result.transferAmount.Value);

            Assert.Equal(JTokenType.Date, result.transferedAtUtc.Type);
            Assert.True(result.transferedAtUtc.Value < DateTime.UtcNow && result.transferedAtUtc.Value != default(DateTime));//ISO-8601
        }


      
        [Fact]
        public async Task Should_Return_BadRequest_With_Transfer_On_Amount_le_Zero()
        {
            var transferData = new
            {
                sourceAccountId = 23456,
                beneficiaryAccountId = 11111,
                transferAmount = 0M//invalid
            };

            // Act
            var response = await _fixture.Client.PostAsync("/api/v1/account/23456/transfer",
                new StringContent(JsonConvert.SerializeObject(transferData), Encoding.UTF8, "application/json")
            );

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
 
        }


        [Fact]
        public async Task Should_Return_BadRequest_With_Transfer_On_Amount_More_Than_Two_Decimals()
        {

            var transferData = new
            {
                sourceAccountId = 23456,
                beneficiaryAccountId = 23457,
                transferAmount = 20.333M//invalid
            };

            var response = await _fixture.Client.PostAsync("/api/v1/account/23456/transfer",
                new StringContent(JsonConvert.SerializeObject(transferData), Encoding.UTF8, "application/json")
            );

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        
        [Fact]
        public async Task Should_Return_BadRequest_With_Transfer_On_Insufficient_Balance()
        {
            var transferData = new
            {
                sourceAccountId = 23456,
                beneficiaryAccountId = 23457,
                transferAmount = 100.01M
            };

            // Act
            var response = await _fixture.Client.PostAsync("/api/v1/account/23456/transfer",
                new StringContent(JsonConvert.SerializeObject(transferData), Encoding.UTF8, "application/json")
            );

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }


        [Fact]
        public async Task Should_Return_NotFound_With_Transfer_On_Invald_Source_Account()
        {
            var transferData = new
            {
                sourceAccountId = 11111,
                beneficiaryAccountId = 23457,
                transferAmount = 0.33M
            };

            // Act
            var response = await _fixture.Client.PostAsync("/api/v1/account/11111/transfer",
                new StringContent(JsonConvert.SerializeObject(transferData), Encoding.UTF8, "application/json")
            );

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }

        [Fact]
        public async Task Should_Return_BadRequest_With_Transfer_On_Invald_Beneficiary_Account()
        {
            var transferData = new
            {
                sourceAccountId = 23456,
                beneficiaryAccountId = 11111,
                transferAmount = 0.33M
            };

            // Act
            var response = await _fixture.Client.PostAsync("/api/v1/account/23456/transfer",
                new StringContent(JsonConvert.SerializeObject(transferData), Encoding.UTF8, "application/json")
            );

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }

    }
}
