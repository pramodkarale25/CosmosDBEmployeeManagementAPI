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
    }
}
