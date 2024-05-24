using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace CosmosDBEmployeeManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulkOperationController : ControllerBase
    {
        private record Product(string id, string name, string categoryId, string partitionKeyValue);

        [HttpPost]
        public async Task<IActionResult> InsertBulk()
        {
            CosmosClientOptions options = new()
            {
                AllowBulkExecution = true
            };

            CosmosClient client = new CosmosClient("", options);
            Microsoft.Azure.Cosmos.Container container = client.GetContainer("", "");

            List<Product> productsToInsert = new List<Product>();// = GetOurProductsFromSomeWhere();

            List<Task> concurrentTasks = new List<Task>();

            foreach (Product product in productsToInsert)
            {
                concurrentTasks.Add(
                    container.CreateItemAsync<Product>(
                        product,
                        new PartitionKey(product.partitionKeyValue))
                );
            }

            Task.WhenAll(concurrentTasks).ConfigureAwait(true);

            return BadRequest();
        }
    }
}
