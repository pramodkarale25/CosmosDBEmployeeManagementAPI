using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using System.Collections.ObjectModel;

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
            RequestOptions dbRequestOptions = new RequestOptions();
            dbRequestOptions.PriorityLevel = PriorityLevel.High;
            dbRequestOptions.IfNoneMatchEtag = "";//mostly we use this in update operation.
            dbRequestOptions.IfMatchEtag = "";//mostly we use this in update operation.
            //dbRequestOptions.AddRequestHeaders;
            dbRequestOptions.CosmosThresholdOptions = new CosmosThresholdOptions() { };
            
            ContainerProperties properties = new ContainerProperties()
            {
                Id = containerName,
                PartitionKeyPath = "/" + partitionKey,
                DefaultTimeToLive = -1,
                //if DefaultTimeToLive = NULL - Items will never expire. Individual item values ignored.
                //if DefaultTimeToLive = -1 - By default Items will never expire. Individual item values gets applied.
                //if DefaultTimeToLive = 10 - all items gets expired after 10 sec.
                IndexingPolicy = GetIndexingPolicy(),
            };

            CosmosClient client = new CosmosClient(conString);
            Database db = await client.CreateDatabaseIfNotExistsAsync(id: dbName, throughput: 500, requestOptions:dbRequestOptions);
            Container container = await db.CreateContainerIfNotExistsAsync(properties);

            return container;
        }

        public static IndexingPolicy GetIndexingPolicy()
        {
            IndexingPolicy defaultPolicy = new IndexingPolicy()
            {
                //IndexingMode = IndexingMode.Consistent,//Default - indexing is updated synchronously with create/update/delete operation.
                //IndexingMode = IndexingMode.Lazy,//indexing is updated asynchronously with create/update/delete operation.
                IndexingMode = IndexingMode.None,//No index is provided.
                //Configures whether automatically indexes items as they are written
                Automatic = true,//default
                //Set of paths to include in the index
                //IncludedPaths =  deafualt All - "*"
                //Set of paths to exclude from the index
                //ExcludedPaths - Default - _etag property path
            };

            /*
                The ? operator indicates that a path terminates with a string or number (scalar) value
                The [] operator indicates that this path includes an array and avoids having to specify an array index value
                The * operator is a wildcard and matches any element beyond the current path

                Path expression	    Description
                /*	                All properties
                /name/?	            The scalar value of the name property
                /category/*	        All properties under the category property
                /metadata/sku/?	    The scalar value of the metadata.sku property
                /tags/[]/name/?	    Within the tags array, the scalar values of all possible name properties
             */

            IndexingPolicy policy = new IndexingPolicy()
            {
                IndexingMode = IndexingMode.Consistent,
                Automatic = true
            };

            policy.IncludedPaths.Add(new IncludedPath() { Path = "/name/?" });
            policy.IncludedPaths.Add(new IncludedPath() { Path = "/categoryName/?" });
            policy.ExcludedPaths.Add(new ExcludedPath() { Path = "/*" });

            //Optional Composite Index
            //SELECT* FROM products p WHERE p.name = "Road Saddle" AND p.price > 50
            //SELECT * FROM products p ORDER BY p.price ASC, p.name ASC
            Collection<CompositePath> compositeIndexList = new Collection<CompositePath>
            {
                new CompositePath() { Path = "/name", Order = CompositePathSortOrder.Ascending },
                new CompositePath() { Path = "/price", Order = CompositePathSortOrder.Descending }
            };

            policy.CompositeIndexes.Add(compositeIndexList);

            return policy;
        }
    }
}
