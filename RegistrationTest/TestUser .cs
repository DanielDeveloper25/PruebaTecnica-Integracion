using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PruebaTecnicaAPI.Models;
using PruebaTecnicaAPI.Models.DTOs;
using PruebaTecnicaAPI.Services;

namespace RegistrationTest
{
    [TestClass]
    public class TestUser
    {
        [TestMethod]
        public async Task TestUpdateUserAsync()
        {
            var options = new DbContextOptionsBuilder<DbpruebaTecnicaContext>()
                .UseInMemoryDatabase(databaseName: "UpdateUserTestDB")
                .Options;

            var context = new DbpruebaTecnicaContext(options);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Nombre original",
                Email = "update@test.com",
                Password = "x", 
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Token = "",
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context);

            var dto = new UpdateUserDto
            {
                Name = "Nuevo Nombre",
                Phones = new List<PhoneDto>
        {
            new PhoneDto { Number = "7654321", CityCode = "2", CountryCode = "58" }
        }
            };

            var result = await service.UpdateUserAsync(user.Id, dto);

            var updatedUser = await context.Users.Include(u => u.Phones).FirstOrDefaultAsync(u => u.Id == user.Id);

            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("Nuevo Nombre", updatedUser.Name);
            Assert.AreEqual(1, updatedUser.Phones.Count);
            Assert.AreEqual("7654321", updatedUser.Phones.First().Number);

            var messageProp = result.GetType().GetProperty("mensaje");
            var messageValue = messageProp?.GetValue(result)?.ToString();

            Assert.AreEqual("Usuario actualizado correctamente", messageValue);
        }

        [TestMethod]
        public async Task TestDeleteUserAsync()
        {
            var options = new DbContextOptionsBuilder<DbpruebaTecnicaContext>()
                .UseInMemoryDatabase(databaseName: "DeleteUserTestDB")
                .Options;

            var context = new DbpruebaTecnicaContext(options);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Usuario a eliminar",
                Email = "delete@test.com",
                Password = "x",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Token = "",
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context);

            var result = await service.DeleteUserAsync(user.Id);
            var deletedUser = await context.Users.FindAsync(user.Id);

            Assert.IsNotNull(deletedUser);
            Assert.IsFalse(deletedUser.IsActive);

            var messageProp = result.GetType().GetProperty("mensaje");
            var messageValue = messageProp?.GetValue(result)?.ToString();

            Assert.AreEqual("Usuario eliminado correctamente", messageValue);
        }

        [TestMethod]
        public async Task TestGetUsersAsync()
        {
            var options = new DbContextOptionsBuilder<DbpruebaTecnicaContext>()
                .UseInMemoryDatabase(databaseName: "GetUsersTestDB")
                .Options;

            var context = new DbpruebaTecnicaContext(options);

            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Name = "Usuario 1",
                Email = "user1@test.com",
                Password = "x",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Token = "",
                IsActive = true,
                Phones = new List<Phone>
        {
            new Phone { Number = "111", CityCode = "1", CountryCode = "57" }
        }
            };

            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Name = "Usuario 2",
                Email = "user2@test.com",
                Password = "x",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Token = "",
                IsActive = false 
            };

            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var service = new UserService(context);

            var result = await service.GetUsersAsync();

            Assert.IsNotNull(result);
            var usersList = result.ToList();
            Assert.AreEqual(1, usersList.Count); 
            Assert.AreEqual("Usuario 1", usersList[0].GetType().GetProperty("name")?.GetValue(usersList[0])?.ToString());
        }

        [TestMethod]
        public async Task TestGetCurrentUserAsync()
        {
            var options = new DbContextOptionsBuilder<DbpruebaTecnicaContext>()
                .UseInMemoryDatabase(databaseName: "GetCurrentUserTestDB")
                .Options;

            var context = new DbpruebaTecnicaContext(options);

            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Name = "Usuario Actual",
                Email = "me@test.com",
                Password = "x",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Token = "",
                IsActive = true,
                Phones = new List<Phone>
                {
                new Phone { Number = "999", CityCode = "9", CountryCode = "57" }
                }
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context);
            var result = await service.GetCurrentUserAsync(userId);

            Assert.IsNotNull(result);
            var nameProp = result.GetType().GetProperty("name");
            Assert.IsNotNull(nameProp);
            Assert.AreEqual("Usuario Actual", nameProp.GetValue(result)?.ToString());
        }

    }
}