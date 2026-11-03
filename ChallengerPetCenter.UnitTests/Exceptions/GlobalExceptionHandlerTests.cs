using ChallengerPetCenter.API.Exceptions;
using ChallengerPetCenter.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text.Json;

namespace ChallengerPetCenter.UnitTests.Exceptions
{
    public class GlobalExceptionHandlerTests
    {
        [Fact]
        public async Task TryHandleAsync_QuandoNotFoundException_Retorna404ProblemDetails()
        {
            // Arrange
            var env = new Mock<IHostEnvironment>();
            env.SetupGet(e => e.EnvironmentName).Returns(Environments.Development);

            var handler = new GlobalExceptionHandler(
                NullLogger<GlobalExceptionHandler>.Instance,
                env.Object);

            var context = new DefaultHttpContext();
            context.Request.Path = "/api/users/1";
            context.Response.Body = new MemoryStream();

            // Act
            var handled = await handler.TryHandleAsync(
                context,
                new NotFoundException("Usuário não encontrado."),
                CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);

            context.Response.Body.Position = 0;
            var json = await new StreamReader(context.Response.Body).ReadToEndAsync();
            using var doc = JsonDocument.Parse(json);
            Assert.Equal(404, doc.RootElement.GetProperty("status").GetInt32());
            Assert.Equal("Usuário não encontrado.", doc.RootElement.GetProperty("detail").GetString());
            Assert.Equal("Recurso não encontrado", doc.RootElement.GetProperty("title").GetString());
        }

        [Fact]
        public async Task TryHandleAsync_QuandoBusinessRuleException_Retorna400ProblemDetails()
        {
            // Arrange
            var env = new Mock<IHostEnvironment>();
            env.SetupGet(e => e.EnvironmentName).Returns(Environments.Production);

            var handler = new GlobalExceptionHandler(
                NullLogger<GlobalExceptionHandler>.Instance,
                env.Object);

            var context = new DefaultHttpContext();
            context.Request.Path = "/api/pets";
            context.Response.Body = new MemoryStream();

            // Act
            var handled = await handler.TryHandleAsync(
                context,
                new BusinessRuleException("Tutor não encontrado."),
                CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

            context.Response.Body.Position = 0;
            var json = await new StreamReader(context.Response.Body).ReadToEndAsync();
            using var doc = JsonDocument.Parse(json);
            Assert.Equal(400, doc.RootElement.GetProperty("status").GetInt32());
            Assert.Equal("Tutor não encontrado.", doc.RootElement.GetProperty("detail").GetString());
        }

        [Fact]
        public async Task TryHandleAsync_QuandoExceptionGenerica_Retorna500SemDetalheInterno()
        {
            // Arrange
            var env = new Mock<IHostEnvironment>();
            env.SetupGet(e => e.EnvironmentName).Returns(Environments.Production);

            var handler = new GlobalExceptionHandler(
                NullLogger<GlobalExceptionHandler>.Instance,
                env.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            var handled = await handler.TryHandleAsync(
                context,
                new InvalidOperationException("segredo interno"),
                CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

            context.Response.Body.Position = 0;
            var json = await new StreamReader(context.Response.Body).ReadToEndAsync();
            using var doc = JsonDocument.Parse(json);
            Assert.Equal(500, doc.RootElement.GetProperty("status").GetInt32());
            Assert.DoesNotContain("segredo interno", json);
        }
    }
}
