using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Services.API.Hotel.Core;

namespace Hotel
{
    public class ErrorHandlerMiddlewareTests
    {
        [Fact]
        public async Task Invoke_Returns_404_NotFound_For_Invalid_URL()
        {
            var middleware = new ErrorHandlerMiddleware((innerHttpContext) => Task.CompletedTask);
            var context = new DefaultHttpContext();
            context.Request.Path = "/invalid/path";
            context.Response.Body = new MemoryStream();

            await middleware.Invoke(context);

            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
            Assert.Equal("La URL solicitada no es válida.", await GetResponseBody(context.Response));
        }

        [Fact]
        public async Task Invoke_Returns_405_MethodNotAllowed_For_Invalid_Method()
        {
            // Arrange
            var middleware = new ErrorHandlerMiddleware((innerHttpContext) => Task.CompletedTask);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal("Método no permitido para esta URL.", await GetResponseBody(context.Response));
        }

        [Theory]
        [InlineData(typeof(FormatException), StatusCodes.Status400BadRequest, "Formato inválido para el Id, debe ser una cadena hexadecimal de 24 dígitos válida.")]
        [InlineData(typeof(MongoWriteException), StatusCodes.Status400BadRequest, "Error al hacer la inserción datos, clave duplicada.")]
        [InlineData(typeof(MongoCommandException), StatusCodes.Status400BadRequest, "Error al actualizar los datos, clave duplicada.")]
        [InlineData(typeof(Exception), StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado en el servidor.")]
        public async Task Invoke_Handles_Exceptions_Correctly(Type exceptionType, int expectedStatusCode, string expectedMessage)
        {
            // Arrange
            var middleware = new ErrorHandlerMiddleware((innerHttpContext) => Task.CompletedTask);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await middleware.Invoke(context);
            context.Features.Set<IExceptionHandlerFeature>(new ExceptionHandlerFeature { Error = (Exception)Activator.CreateInstance(exceptionType) });
            await middleware.Invoke(context);

            // Assert
            Assert.Equal(expectedStatusCode, context.Response.StatusCode);
            Assert.Equal(expectedMessage, await GetResponseBody(context.Response));
        }

        private async Task<string> GetResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(response.Body))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
