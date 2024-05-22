using CosmosDBEmployeeManagementAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Reflection.Metadata;

namespace CosmosDBEmployeeManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConcurrencyController : ControllerBase
    {
        private record Product(string id, string name, string categoryId);
        Product saddle = new Product("0120", "Worn Saddle", "accessories-used");
        PartitionKey partitionKey = new PartitionKey("accessories-used");

        private Container GetContainer()
        {
            return CosmosHelper.CreateDBAndContainer("ProductDB", "Product", "categoryId").Result;
        }

        [HttpPut]
        public async Task<IActionResult> UpsertMultipleItem()
        {
            ItemResponse<Product> itemResponse = await GetContainer().ReadItemAsync<Product>(saddle.id, partitionKey);
            saddle = new Product("0120", "Worn Saddle 55", "accessories-used");

            ItemRequestOptions options = new ItemRequestOptions() { IfMatchEtag = itemResponse.ETag };
            Product res = await GetContainer().UpsertItemAsync<Product>(saddle, partitionKey, options);
            return Ok(res);
        }
    }
}
