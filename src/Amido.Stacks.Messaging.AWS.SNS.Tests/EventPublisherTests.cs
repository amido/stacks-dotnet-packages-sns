using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amido.Stacks.Application.CQRS.ApplicationEvents;
using Amido.Stacks.Configuration;
using Amido.Stacks.Messaging.AWS.SNS.Publisher;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Amido.Stacks.Messaging.AWS.SNS.Tests;

public class EventPublisherTests
{
	[Fact]
	public void Should_BeDerivedFromIApplicationEventPublisher()
	{
		// arrange
		// act
		// assert
		typeof(EventPublisher)
			.Should()
			.Implement<IApplicationEventPublisher>();
	}

	[Fact]
	public void Given_IOptionsIsNull_Should_ThrowArgumentNullException()
	{
		// arrange
		// act
		Action constructor = () =>
			new EventPublisher(
				null!,
				A.Fake<ISecretResolver<string>>(),
				A.Fake<IAmazonSimpleNotificationService>(),
				A.Fake<ILogger<EventPublisher>>());

		// assert
		constructor
			.Should()
			.Throw<ArgumentNullException>()
			.WithMessage("Value cannot be null. (Parameter 'configuration')"); ;
	}

	[Fact]
	public void Given_ISecretResolverIsNull_Should_ThrowArgumentNullException()
	{
		// arrange
		// act
		Action constructor = () =>
			new EventPublisher(
				A.Fake<IOptions<AwsSnsConfiguration>>(),
				null!,
				A.Fake<IAmazonSimpleNotificationService>(),
				A.Fake<ILogger<EventPublisher>>());

		// assert
		constructor
			.Should()
			.Throw<ArgumentNullException>()
			.WithMessage("Value cannot be null. (Parameter 'secretResolver')"); ;
	}

	[Fact]
	public void Given_IAmazonSimpleNotificationServiceIsNull_Should_ThrowArgumentNullException()
	{
		// arrange
		// act
		Action constructor = () =>
			new EventPublisher(
				A.Fake<IOptions<AwsSnsConfiguration>>(),
				A.Fake<ISecretResolver<string>>(),
				null!,
				A.Fake<ILogger<EventPublisher>>());

		// assert
		constructor
			.Should()
			.Throw<ArgumentNullException>()
			.WithMessage("Value cannot be null. (Parameter 'snsClient')"); ;
	}

	[Fact]
	public void Given_ILoggerIsNull_Should_ThrowArgumentNullException()
	{
		// arrange
		// act
		Action constructor = () =>
			new EventPublisher(
				A.Fake<IOptions<AwsSnsConfiguration>>(),
				A.Fake<ISecretResolver<string>>(),
				A.Fake<IAmazonSimpleNotificationService>(),
				null!);

		// assert
		constructor
			.Should()
			.Throw<ArgumentNullException>()
			.WithMessage("Value cannot be null. (Parameter 'logger')"); ;
	}

	[Fact]
	public void Given_AllRequiredParameters_Should_NotThrow()
	{
		// arrange
		// act
		Action constructor = () =>
			new EventPublisher(
				A.Fake<IOptions<AwsSnsConfiguration>>(),
				A.Fake<ISecretResolver<string>>(),
				A.Fake<IAmazonSimpleNotificationService>(),
				A.Fake<ILogger<EventPublisher>>());

		// assert
		constructor
			.Should()
			.NotThrow();
	}

	[Fact]
	public async Task PublishAsync_Should_MakeCallToPublishAsync()
	{
		// arrange
		var awsSnsConfiguration = new AwsSnsConfiguration
		{
			TopicArn = new Secret()
		};
		var fakeApplicationEvent = A.Fake<IApplicationEvent>();
		var fakeSecretResolver = A.Fake<ISecretResolver<string>>();
		A.CallTo(() => fakeSecretResolver.ResolveSecretAsync(A<Secret>._)).Returns("TopicArn");

		var fakeAmazonSns = A.Fake<IAmazonSimpleNotificationService>();
		var fakeLogger = A.Fake<ILogger<EventPublisher>>();
		var fakeOptions = A.Fake<IOptions<AwsSnsConfiguration>>();
		A.CallTo(() => fakeOptions.Value).Returns(awsSnsConfiguration);

		var sut = new EventPublisher(fakeOptions, fakeSecretResolver, fakeAmazonSns, fakeLogger);

		// act
		await sut.PublishAsync(fakeApplicationEvent);

		// assert
		A.CallTo(() => fakeAmazonSns.PublishAsync(A<PublishRequest>._, CancellationToken.None))
			.MustHaveHappenedOnceExactly();
	}
}