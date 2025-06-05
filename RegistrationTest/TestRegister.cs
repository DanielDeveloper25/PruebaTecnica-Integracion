using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PruebaTecnicaAPI.Models;
using PruebaTecnicaAPI.Models.DTOs;
using PruebaTecnicaAPI.Services;

namespace RegistrationTest
{
    [TestClass]
    public class TestRegister
    {
        [TestMethod]
        public void TestEncriptSHA256()
        {
            var config = new ConfigurationBuilder().Build();
            var service = new RegistrationService(config, null!);

            string password = "abc123";
            string hash1 = service.EncriptSHA256(password);
            string hash2 = service.EncriptSHA256(password);

            Assert.IsNotNull(hash1);
            Assert.AreEqual(hash1, hash2); 
            Assert.AreEqual(64, hash1.Length);
        }

        [TestMethod]
        public void TestgenerateJWT()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {{ "Jwt:key", "test_key_123456789012345678901234567890" }}!).Build();

            var service = new RegistrationService(config, null!);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com"
            };

            string token = service.generateJWT(user);

            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
            Assert.IsTrue(token.Split('.').Length == 3); 
        }

        [TestMethod]
        public async Task TestRegistrarUsuarioAsync()
        {
            var options = new DbContextOptionsBuilder<DbpruebaTecnicaContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            var context = new DbpruebaTecnicaContext(options);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
            { "Jwt:key", "test_key_123456789012345678901234567890" },
            { "PasswordRegex", "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{6,}$" }
                }!).Build();

            var service = new RegistrationService(config, context);

            var dto = new RegisterUserDto
            {
                Name = "John Doe",
                Email = "john@test.com",
                Password = "abc123",
                Phones = new List<PhoneDto>
        {
            new PhoneDto { Number = "1234567", CityCode = "1", CountryCode = "57" }
        }
            };

            var result = await service.RegistrarUsuarioAsync(dto);

            Assert.IsInstanceOfType(result, typeof(object));
            var response = result.GetType().GetProperty("token") != null;
            Assert.IsTrue(response);
        }

    }
}