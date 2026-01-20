using System.Diagnostics.CodeAnalysis;

namespace Monaco.Template.Backend.IntegrationTests;

/// <summary>
/// Defines the "IntegrationTests" collection so that all test classes
/// sharing this collection use a single <see cref="AppFixture"/> instance.
/// This ensures containers start once and migrations run once per test run.
/// </summary>
[ExcludeFromCodeCoverage]
[CollectionDefinition("IntegrationTests")]
public class IntegrationTestsCollection : ICollectionFixture<AppFixture>
{
	// No code required; this class ties AppFixture to the "IntegrationTests" collection.
}