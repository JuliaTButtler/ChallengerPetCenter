using ChallengerPetCenter.Common;
using ChallengerPetCenter.Controllers;
using ChallengerPetCenter.DTOs.Users;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChallengerPetCenter.UnitTests.Controllers
{
    /// <summary>
    /// Testes unitários da camada de API com Moq da dependência IUserService.
    /// </summary>
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetUserById_QuandoUsuarioExiste_RetornaOk()
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

            var controller = new UsersController(mockService.Object);

            // Act
            var actionResult = await controller.GetUserById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var payload = Assert.IsType<UserResponseDto>(okResult.Value);
            Assert.Equal(1, payload.Id);
            mockService.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetUserById_QuandoUsuarioNaoExiste_RetornaNotFound()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService
                .Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync(ServiceResult<UserResponseDto>.AsNotFound("Usuário não encontrado."));

            var controller = new UsersController(mockService.Object);

            // Act
            var actionResult = await controller.GetUserById(99);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("Usuário não encontrado.", notFound.Value);
        }

        [Fact]
        public async Task CreateUser_QuandoEmailDuplicado_RetornaBadRequest()
        {
            // Arrange
            var mockService = new Mock<IUserService>();
            mockService
                .Setup(s => s.CreateAsync(It.IsAny<CreateUserDto>()))
                .ReturnsAsync(ServiceResult<UserResponseDto>.Failure("Email já cadastrado."));

            var controller = new UsersController(mockService.Object);
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
            var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Email já cadastrado.", badRequest.Value);
        }

        [Fact]
        public async Task CreateUser_QuandoDadosValidos_RetornaCreatedAtAction()
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

            var controller = new UsersController(mockService.Object);
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
            var payload = Assert.IsType<UserResponseDto>(createdResult.Value);
            Assert.Equal(10, payload.Id);
        }
    }
}
