namespace TestBackendTask.App.Tests.Base;

[CollectionDefinition(CollectionName)]
public class TestsCollection : ICollectionFixture<BaseTestsFixture>
{
    public const string CollectionName = "Process tests";

    // Используется для объединения тестовых кейсов в коллекцию
    // ради использования одного процесса с сервером
}