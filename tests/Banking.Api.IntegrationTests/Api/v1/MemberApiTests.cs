using System;
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
    public class MemberApiTests : IClassFixture<MemberApiTests.Fixture>
    {
        private readonly MemberApiTests.Fixture _fixture;
        private readonly ITestOutputHelper _output;
        public MemberApiTests(MemberApiTests.Fixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        public class Fixture : ApiIntegrationTestFixture
        {
            public Fixture() : base("integrationTestData2.json"){}
        }

        [Fact]
        public async Task Should_List_With_No_Params()
        {
            var response = await _fixture.Client.GetAsync("/api/v1/member");

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            response.EnsureSuccessStatusCode();

            /********** Json result **********/
            dynamic membList = JArray.Parse(contentString);// JsonConvert.DeserializeObject(contentString);
            Assert.Equal(4, membList.Count); //should have 2 items

            foreach(dynamic item in membList) //each inst should have a non-empty id/name
            {
                Assert.Equal(JTokenType.Integer, item.id.Type);
                Assert.NotEqual(default(Int32), item.id.Value);
                Assert.Equal(JTokenType.String, item.givenName.Type);
                Assert.True(!String.IsNullOrWhiteSpace(item.givenName.Value));
                Assert.Equal(JTokenType.String, item.surName.Type);
                Assert.True(!String.IsNullOrWhiteSpace(item.surName.Value));
                Assert.Equal(JTokenType.Integer, item.institutionId.Type);
                Assert.NotEqual(default(Int32), item.institutionId.Value);
                Assert.True(item.createdAtUtc.Value != null && item.createdAtUtc.Value != default(DateTime));//ISO-8601
                var accounts = item.accounts;
                foreach (dynamic acc in accounts) 
                {
                    Assert.Equal(JTokenType.Integer, acc.id.Type);
                    Assert.NotEqual(default(Int32), acc.id.Value);
                    Assert.Equal(JTokenType.Float, acc.balance.Type);
                    Assert.NotEqual(default(float), acc.balance.Value);
                    Assert.Equal(JTokenType.String, acc.currencyCode.Type);
                    Assert.Equal("USD", acc.currencyCode.Value);
                }
            }
        }

        [Fact]
        public async Task Should_List_With_Paging()
        {
            var response = await _fixture.Client.GetAsync("/api/v1/member?pageNumber=1&pageSize=2");

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            response.EnsureSuccessStatusCode();

            /********** Json result **********/
            dynamic membList = JArray.Parse(contentString);
            Assert.Equal(2,membList.Count); //should have 2 items on page 1

            response = await _fixture.Client.GetAsync("/api/v1/member?pageNumber=2&pageSize=2");

            contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            response.EnsureSuccessStatusCode();

            /********** Json result **********/
            membList = JArray.Parse(contentString);
            Assert.Equal(2, membList.Count); //should have 2 item on page 2
        }

        [Fact]
        public async Task Should_Return_BadRequest_When_List_With_Page_Less_Than_1()
        {
            var response = await _fixture.Client.GetAsync("/api/v1/member?pageNumber=0&pageSize=2");

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
           
         }

        [Fact]
        public async Task Should_Find_By_Id()
        {
            var response = await _fixture.Client.GetAsync("/api/v1/member/234790");

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            response.EnsureSuccessStatusCode();

            /********** Json result **********/
            dynamic member = JsonConvert.DeserializeObject(contentString);

            Assert.Equal(JTokenType.Integer, member.id.Type);
            Assert.Equal(234790, member.id.Value);

            Assert.Equal(JTokenType.String, member.givenName.Type);
            Assert.Equal("Jane", member.givenName.Value);

            Assert.Equal(JTokenType.String, member.surName.Type);
            Assert.Equal("Doe", member.surName.Value);

            Assert.Equal(JTokenType.Integer, member.institutionId.Type);
            Assert.Equal(78923, member.institutionId.Value);

            Assert.Equal(JTokenType.Date, member.createdAtUtc.Type);
            Assert.True(member.createdAtUtc.Value != null && member.createdAtUtc.Value != default(DateTime));//ISO-8601

            var accounts = member.accounts;
       
            dynamic account = accounts[0];
            Assert.Equal(JTokenType.Integer, account.id.Type);
            Assert.Equal(23457, account.id.Value);
            Assert.Equal(JTokenType.Float, account.balance.Type);
            Assert.Equal(20.50, account.balance.Value);
            // Assert.Equal("USD", item.currencyCode.Value);
            // Assert.Equal(JTokenType.String, item.currencyCode.Type);
        }
        
        
        [Fact]
        public async Task Should_Return_NotFound_When_Find_By_Id_Not_Exists()
        {
            var response = await _fixture.Client.GetAsync("/api/v1/member/11111");

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }
    }
}
