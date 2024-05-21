using Microsoft.Azure.Cosmos;

namespace CosmosDBEmployeeManagementAPI.Model
{
    public static class CosmosHelper
    {
        // Cosmos DB details, In real use cases, these details should be configured in secure configuraion file.
        public static readonly string CosmosDBAccountUri = "https://localhost:8081/";
        public static readonly string CosmosDBAccountPrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public static readonly string CosmosDbName = "EmployeeManagementDB";
        public static readonly string CosmosDbContainerName = "Employees";
        public static readonly string conString = @"AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public async static Task<Container> CreateDBAndContainer(string dbName, string containerName, string partitionKey)
        {
            ContainerProperties properties = new ContainerProperties()
            {
                Id = containerName,
                PartitionKeyPath = "/" + partitionKey,
                DefaultTimeToLive = -1,
                //if DefaultTimeToLive = NULL - Items will never expire. Individual item values ignored.
                //if DefaultTimeToLive = -1 - By default Items will never expire. Individual item values gets applied.
                //if DefaultTimeToLive = 10 - all items gets expired after 10 sec.

            };
            CosmosClient client = new CosmosClient(conString);
            Database db = await client.CreateDatabaseIfNotExistsAsync(dbName);
            Container container = await db.CreateContainerIfNotExistsAsync(properties);

            return container;
        }
    }
}
