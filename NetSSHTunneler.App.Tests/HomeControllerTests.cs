using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetSSHTunneler.Controllers;
using NetSSHTunneler.Domain.DTOs;
using NetSSHTunneler.Domain.Responses;
using NetSSHTunneler.Services.Interfaces;
using NetSSHTunneler.Utils.Tests.Helper;
using System.Text.Json;
using Xunit;

namespace NetSSHTunneler.App.Tests
{
    public class HomeControllerTests
    {
        public HomeControllerTests() { }

        [Theory, AutoMoqData]
        public void CheckConnection_ShouldBeSuccesfully(
            SshConnectionDto sshConnectionDto,
            HomeController sut)
        {
            // Act
            var result = sut.CheckConnection(sshConnectionDto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            ((ConnectionStatusResponse)((OkObjectResult)result.Result).Value).Status.Should().BeTrue();
        }

        [Theory, AutoMoqData]
        public void CheckConnection_NullSshConnectionDto_412Status(
            HomeController sut)
        {
            // Act
            var result = sut.CheckConnection(null);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result.Result).Value.Should().BeOfType<ProblemDetails>();
            ((ProblemDetails)((ObjectResult)result.Result).Value).Status.Should().Be(412);
        }

        [Theory, AutoMoqData]
        public void CheckConnection_NoTargetIP_412Status(
            [NoAutoProperties] SshConnectionDto sshConnectionDto,
            HomeController sut)
        {
            // Act
            var result = sut.CheckConnection(sshConnectionDto);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result.Result).Value.Should().BeOfType<ProblemDetails>();
            ((ProblemDetails)((ObjectResult)result.Result).Value).Status.Should().Be(412);
        }

        [Theory, AutoMoqData]
        public void DeleteDashboardConfig_ShouldBeSuccesfully(
            HomeController sut)
        {
            // Act
            var result = sut.DeleteDashboardConfig();

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Theory, AutoMoqData]
        public void DeleteDashboardConfig_NoFile_ExceptionThrown(
            [Frozen] Mock<IFileOperations> fileOperationsMock,
            HomeController sut)
        {
            // Arrange
            fileOperationsMock.Setup(x => x.DeleteFile(It.IsAny<string>())).Throws(new System.Exception());

            // Act
            var result = sut.DeleteDashboardConfig();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).Value.Should().BeOfType<ProblemDetails>();
            ((ProblemDetails)((ObjectResult)result).Value).Status.Should().Be(500);
        }

        [Theory, AutoMoqData]
        public void CheckDashboard_ShouldBeSuccesfully(
            DashboardStatusResponse content,
            [Frozen] Mock<IFileOperations> fileOperationsMock,
            HomeController sut)
        {
            // Arrange
            var message = "Dashboard configured!";
            fileOperationsMock.Setup(x => x.ReadFile(It.IsAny<string>(), It.IsAny<string>())).Returns(JsonSerializer.Serialize(content));

            // Act
            var result = sut.CheckDashboard();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            ((DashboardStatusResponse)((OkObjectResult)result.Result).Value).Message.Should().BeEquivalentTo(message);
            ((DashboardStatusResponse)((OkObjectResult)result.Result).Value).Configured.Should().BeTrue();
        }

        [Theory, AutoMoqData]
        public void CheckDashboard_NoFile_ExceptionThrown(
            [Frozen] Mock<IFileOperations> fileOperationsMock,
            HomeController sut)
        {
            // Arrange
            fileOperationsMock.Setup(x => x.ReadFile(It.IsAny<string>(), It.IsAny<string>())).Throws(new System.Exception());

            // Act
            var result = sut.CheckDashboard();

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result.Result).Value.Should().BeOfType<ProblemDetails>();
            ((ProblemDetails)((ObjectResult)result.Result).Value).Status.Should().Be(500);
        }
    }
}