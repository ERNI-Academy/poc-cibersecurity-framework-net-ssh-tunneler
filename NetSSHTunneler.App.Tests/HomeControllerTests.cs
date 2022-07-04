using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NetSSHTunneler.Controllers;
using NetSSHTunneler.Domain.DTOs;
using NetSSHTunneler.Domain.Responses;
using NetSSHTunneler.Services.Interfaces;
using Xunit;

namespace NetSSHTunneler.App.Tests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> loggerMock;
        private readonly Mock<ISshConnector> sshConnectorMock;
        private readonly Mock<IFileOperations> fileOperationsMock;
        public HomeControllerTests()
        {
            fileOperationsMock = new Mock<IFileOperations>();
            sshConnectorMock = new Mock<ISshConnector>();
            loggerMock = new Mock<ILogger<HomeController>>();
        }

        [Fact]
        public void CheckConnection_ShouldBeSuccesfully()
        {
            // Arrange
            var sshConnectionDto = new SshConnectionDto
            {
                TargetIp = "127.0.0.1"
            };

            var expected = new ConnectionStatusResponse
            {
                Status = true
            };

            this.sshConnectorMock.Setup(x => x.CheckConnection(sshConnectionDto)).Returns(expected);
            var controller = this.GetHomeControllerSut();
            
            // Act
            var result = controller.CheckConnection(sshConnectionDto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result.Result).Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void CheckConnection_NullSshConnectionDto_412Status()
        {
            // Arrange
            var controller = this.GetHomeControllerSut();

            // Act
            var result = controller.CheckConnection(null);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result.Result).Value.Should().BeOfType<ProblemDetails>();
            ((ProblemDetails)((ObjectResult)result.Result).Value).Status.Should().Be(412);
        }

        [Fact]
        public void CheckConnection_NoTargetIP_412Status()
        {
            // Arrange
            var sshConnectionDto = new SshConnectionDto
            {
                TargetIp = string.Empty
            };

            var controller = this.GetHomeControllerSut();

            // Act
            var result = controller.CheckConnection(sshConnectionDto);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result.Result).Value.Should().BeOfType<ProblemDetails>();
            ((ProblemDetails)((ObjectResult)result.Result).Value).Status.Should().Be(412);
        }

        [Fact]
        public void DeleteDashboardConfig_ShouldBeSuccesfully()
        {
            // Arrange
            this.fileOperationsMock.Setup(x => x.DeleteFile(It.IsAny<string>()));

            var controller = this.GetHomeControllerSut();

            // Act
            var result = controller.DeleteDashboardConfig();

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void DeleteDashboardConfig_NoFile_ExceptionThrown()
        {
            // Arrange
            this.fileOperationsMock.Setup(x => x.DeleteFile(It.IsAny<string>())).Throws(new System.Exception());

            var controller = this.GetHomeControllerSut();

            // Act
            var result = controller.DeleteDashboardConfig();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).Value.Should().BeOfType<ProblemDetails>();
            ((ProblemDetails)((ObjectResult)result).Value).Status.Should().Be(500);
        }

        [Fact]
        public void CheckDashboard_ShouldBeSuccesfully()
        {
            // Arrange
            var json = "{}";
            DashboardStatusResponse expected = new(json);
            this.fileOperationsMock.Setup(x => x.ReadFile(It.IsAny<string>(), It.IsAny<string>())).Returns(json);
            var controller = this.GetHomeControllerSut();

            // Act
            var result = controller.CheckDashboard();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result.Result).Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void CheckDashboard_NoFile_ExceptionThrown()
        {
            // Arrange
            this.fileOperationsMock.Setup(x => x.ReadFile(It.IsAny<string>(), It.IsAny<string>())).Throws(new System.Exception());
            var controller = this.GetHomeControllerSut();

            // Act
            var result = controller.CheckDashboard();

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result.Result).Value.Should().BeOfType<ProblemDetails>();
            ((ProblemDetails)((ObjectResult)result.Result).Value).Status.Should().Be(500);
        }

        private HomeController GetHomeControllerSut()
        {
            return new HomeController(
                this.loggerMock.Object,
                this.sshConnectorMock.Object,
                this.fileOperationsMock.Object
                );
        }
    }
}
