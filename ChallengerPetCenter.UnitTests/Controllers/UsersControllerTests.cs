using ChallengerPetCenter.Common;
using ChallengerPetCenter.Controllers;
using ChallengerPetCenter.DTOs.Common;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChallengerPetCenter.UnitTests.Controllers
{
    /// <summary>
    /// Testes unitários da camada de API com Moq da dependência IUserService.
    /// </summary>
    public class UsersControllerTests
    {
        private static UsersController CreateController(IUserService service)
        {
            var controller = new UsersController(service);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            return controller;
        }

        [Fact]
        public async Task GetUserById_QuandoUsuarioExiste_RetornaOkComLinksHateoas()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var dto = new UserResponseDto
            {
                Id = 1,
                Nome = "Ana",
                Email = "ana@email.com",
                TipoUsuario = "TUTOR",
                Ativo = 1
            };

            mockService
                .Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(ServiceResult<UserResponseDto>.Success(dto));

            var controller = CreateController(mockService.Object);

            // Act
            var actionResult = await controller.GetUserById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var payload = Assert.IsType<ResourceResponseDto<UserResponseDto>>(okResult.Value);
            Assert.Equal(1, payload.Data.Id);
            Assert.Contains(payload.Links, l => l.Rel == "self");
            Assert.Contains(payload.Links, l => l.Rel == "update");
            Assert.Contains(payload.Links, l => l.Rel == "delete");
            mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetUserById_QuandoUsuarioNaoExiste_RetornaProblemDetailsNotFound()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService
                .Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync(ServiceResult<UserResponseDto>.AsNotFound("Usuário não encontrado."));

            var controller = CreateController(mockService.Object);

            // Act
            var actionResult = await controller.GetUserById(99);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            var problem = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Usuário não encontrado.", problem.Detail);
            Assert.Equal("Recurso não encontrado", problem.Title);
        }

        [Fact]
        public async Task CreateUser_QuandoEmailDuplicado_RetornaProblemDetailsBadRequest()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService
                .Setup(s => s.CreateAsync(It.IsAny<CreateUserDto>()))
                .ReturnsAsync(ServiceResult<UserResponseDto>.Failure("Email já cadastrado."));

            var controller = CreateController(mockService.Object);
            var dto = new CreateUserDto
            {
                Nome = "Ana",
                Email = "ana@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };

            // Act
            var actionResult = await controller.CreateUser(dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            var problem = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Email já cadastrado.", problem.Detail);
        }

        [Fact]
        public async Task CreateUser_QuandoDadosValidos_RetornaCreatedAtActionComHateoas()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            var created = new UserResponseDto
            {
                Id = 10,
                Nome = "Ana",
                Email = "ana@email.com",
                TipoUsuario = "TUTOR",
                Ativo = 1
            };

            mockService
                .Setup(s => s.CreateAsync(It.IsAny<CreateUserDto>()))
                .ReturnsAsync(ServiceResult<UserResponseDto>.Success(created));

            var controller = CreateController(mockService.Object);
            var dto = new CreateUserDto
            {
                Nome = "Ana",
                Email = "ana@email.com",
                Senha = "123",
                TipoUsuario = "TUTOR"
            };

            // Act
            var actionResult = await controller.CreateUser(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal(nameof(UsersController.GetUserById), createdResult.ActionName);
            var payload = Assert.IsType<ResourceResponseDto<UserResponseDto>>(createdResult.Value);
            Assert.Equal(10, payload.Data.Id);
            Assert.Contains(payload.Links, l => l.Rel == "self");
        }

        [Fact]
        public async Task GetUsers_QuandoPaginado_RetornaLinksSelfNext()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(Enumerable.Range(1, 15).Select(i => new UserResponseDto
                {
                    Id = i,
                    Nome = $"User{i}",
                    Email = $"u{i}@email.com",
                    TipoUsuario = "TUTOR",
                    Ativo = 1
                }));

            var controller = CreateController(mockService.Object);

            // Act
            var actionResult = await controller.GetUsers(page: 1, pageSize: 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var payload = Assert.IsType<PagedResponseDto<UserResponseDto>>(okResult.Value);
            Assert.Equal(15, payload.Total);
            Assert.Equal(10, payload.Items.Count);
            Assert.Contains(payload.Links, l => l.Rel == "self");
            Assert.Contains(payload.Links, l => l.Rel == "next");
            Assert.DoesNotContain(payload.Links, l => l.Rel == "prev");
        }
    }
}
