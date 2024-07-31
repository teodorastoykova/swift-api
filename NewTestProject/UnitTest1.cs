using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SwiftApiTests
{
    public class SwiftControllerTests : IClassFixture<WebApplicationFactory<SwiftApi.Program>>
    {
        private readonly HttpClient _client;

        public SwiftControllerTests(WebApplicationFactory<SwiftApi.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Upload_File_Should_Return_Success()
        {
            var fileContent = @"
                {1:F01PRCBBGSFAXXX1111111111}{2:O7991111111111ABGRSWACAXXX11111111111111111111N}{4:
                :20:67-C111111-KNTRL 
                :21:30-111-1111111
                :79:NA VNIMANIETO NA: OTDEL BANKOVI GARANTSII
                .
                OTNOSNO: POTVARJDENIE NA AVTENTICHNOST NA
                PRIDRUJITELNO PISMO KAM ISKANE ZA
                PLASHTANE PO BANKOVA GARANCIA
                .
                UVAJAEMI KOLEGI,
                .
                UVEDOMJAVAME VI, CHE IZPRASHTAME ISKANE ZA 
                PLASHTANE NA STOYNOST BGN 3.100,00, PREDSTAVENO 
                OT NASHIA KLIENT.
                .
                S NASTOYASHTOTO POTVARZDAVAME AVTENTICHNOSTTA NA 
                PODPISITE VARHU PISMOTO NI, I CHE TEZI LICA SA 
                UPALNOMOSHTENI DA PODPISVAT TAKAV DOKUMENT OT 
                IMETO NA BANKATA AD.
                .
                POZDRAVI,
                TARGOVSKO FINANSIRANE
                -}{5:{MAC:00000000}{CHK:111111111111}}";

            var content = new MultipartFormDataContent
            {
                { new StringContent(fileContent, Encoding.UTF8, "text/plain"), "file", "test.txt" }
            };

            var response = await _client.PostAsync("/Swift/upload", content);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("File processed successfully.");
        }

        [Fact]
        public async Task UploadFile_NoFile_ShouldReturnBadRequest()
        {
            var content = new MultipartFormDataContent();

            var response = await _client.PostAsync("/Swift/upload", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UploadFile_EmptyFile_ShouldReturnBadRequest()
        {
            var content = new MultipartFormDataContent();
            var emptyFileContent = new ByteArrayContent(new byte[0]);
            emptyFileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            content.Add(emptyFileContent, "file", "emptyfile.txt");

            var response = await _client.PostAsync("/Swift/upload", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UploadFile_InvalidFormat_ShouldReturnInternalServerError()
        {
            var content = new MultipartFormDataContent();
            var invalidFileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("This is not a valid format"));
            invalidFileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            content.Add(invalidFileContent, "file", "invalidfile.txt");

            var response = await _client.PostAsync("/Swift/upload", content);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("Internal server error");
        }
    }
}
