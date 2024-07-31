using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SwiftApi;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

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
            // Arrange
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
                { new StringContent(fileContent), "file", "test.txt" }
            };

            // Act
            var response = await _client.PostAsync("/Swift/upload", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("File processed successfully.");
        }

        [Fact]
        public async Task Get_Messages_Should_Return_Stored_Messages()
        {
            // Arrange
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
                { new StringContent(fileContent), "file", "test.txt" }
            };
            await _client.PostAsync("/Swift/upload", content);

            // Act
            var response = await _client.GetAsync("/Swift/messages");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("OTDEL BANKOVI GARANTSII");
            responseString.Should().Contain("POTVARJDENIE NA AVTENTICHNOST NA PRIDRUJITELNO PISMO KAM ISKANE ZA PLASHTANE PO BANKOVA GARANCIA");
            responseString.Should().Contain("UVAJAEMI KOLEGI, UVEDOMJAVAME VI, CHE IZPRASHTAME ISKANE ZA PLASHTANE NA STOYNOST BGN 3.100,00, PREDSTAVENO OT NASHIA KLIENT. S NASTOYASHTOTO POTVARZDAVAME AVTENTICHNOSTTA NA PODPISITE VARHU PISMOTO NI, I CHE TEZI LICA SA UPALNOMOSHTENI DA PODPISVAT TAKAV DOKUMENT OT IMETO NA BANKATA AD. POZDRAVI, TARGOVSKO FINANSIRANE");
        }
    }
}
