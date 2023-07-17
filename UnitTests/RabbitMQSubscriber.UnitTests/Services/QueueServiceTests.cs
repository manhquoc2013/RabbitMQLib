using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQSubscriber.Common.Options;

namespace RabbitMQSubscriber.UnitTests.Services
{
    public class QueueServiceTests
    {
        #region Initialize
        private readonly Mock<ILoggerFactory> _queueServiceMock = new();
        private Mock<IConnection> _connectionMock = new();
        private Mock<IConnectionFactory> _connectionFactoryMock = new();

        private RabbitMQCoreOptions options = new RabbitMQCoreOptions();
        #endregion

        #region Connect method
        [Fact]
        public void GivenRequestIsValid_WhenConnectIsCalled_ThenConnectSuccess()
        {
            // Arrange
            _connectionFactoryMock.Setup(s => s.CreateConnection()).Returns(_connectionMock.Object);

            // Act

            // Assert
        }
        #endregion
    }
}
