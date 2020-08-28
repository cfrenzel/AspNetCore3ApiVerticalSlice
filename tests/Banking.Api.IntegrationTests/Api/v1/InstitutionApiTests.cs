using Banking.Api.Institutions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Bank.Api.IntegrationTests
{
    public class InstitutionApiTests : IClassFixture<InstitutionApiTests.Fixture>
    {
        private readonly InstitutionApiTests.Fixture _fixture;
        private readonly ITestOutputHelper _output;
        public InstitutionApiTests(InstitutionApiTests.Fixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        public class Fixture : ApiIntegrationTestFixture
        {
            public Fixture() : base("integrationTestData1.json"){}
        }

        [Fact]
        public async Task Should_List_With_No_Params()
        {
            var response = await _fixture.Client.GetAsync("/api/v1/institution");

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            response.EnsureSuccessStatusCode();

            /********** Json result **********/
            dynamic instList = JArray.Parse(contentString);
            Assert.Equal(3, instList.Count); //should have 2 items

            foreach(dynamic item in instList) //each inst should have a non-empty id/name
            {
                Assert.Equal(JTokenType.Integer, item.id.Type);
                Assert.NotEqual(default(Int32), item.id.Value);
                Assert.Equal(JTokenType.String, item.name.Type);
                Assert.True(!String.IsNullOrWhiteSpace(item.name.Value));
            }
        }

        [Fact]
        public async Task Should_List_With_Paging()
        {
            var response = await _fixture.Client.GetAsync("/api/v1/institution?pageNumber=1&pageSize=2");

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            response.EnsureSuccessStatusCode();

            /********** Json result **********/
            dynamic instList = JArray.Parse(contentString);
            Assert.Equal(2, instList.Count); //should have 2 items on page 1

            response = await _fixture.Client.GetAsync("/api/v1/institution?pageNumber=2&pageSize=2");

            contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            response.EnsureSuccessStatusCode();

            /********** Json result **********/
            instList = JArray.Parse(contentString);
            Assert.Equal(1, instList.Count); //should have 1 item on page 2
        }

        [Fact]
        public async Task Should_Return_BadRequest_When_List_With_Page_Less_Than_1()
        {
            var response = await _fixture.Client.GetAsync("/api/v1/institution?pageNumber=0&pageSize=2");

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
           
         }

        [Fact]
        public async Task Should_Find_By_Id()
        {
            var response = await _fixture.Client.GetAsync("/api/v1/institution/78924");

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            response.EnsureSuccessStatusCode();

            /********** Json result **********/
            dynamic inst = JsonConvert.DeserializeObject(contentString);

            Assert.Equal(JTokenType.Integer, inst.id.Type);
            Assert.Equal(78924, inst.id.Value);
            Assert.Equal(JTokenType.String, inst.name.Type);
            Assert.Equal("Test Credit Union 2", inst.name.Value);
            Assert.Equal(JTokenType.Date, inst.createdAtUtc.Type);
            Assert.Equal(JsonConvert.DeserializeObject("\"2020-08-26T19:22:35.6094803Z\""), inst.createdAtUtc.Value);//ISO-8601
        }

        [Fact]
        public async Task Should_Return_NotFound_When_Find_By_Id_Not_Exists()
        {
            var response = await _fixture.Client.GetAsync("/api/v1/institution/666");

            var contentString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentString);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        }
    }
}
