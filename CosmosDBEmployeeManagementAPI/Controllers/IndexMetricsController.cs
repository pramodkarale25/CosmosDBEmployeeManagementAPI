using CosmosDBEmployeeManagementAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosDBEmployeeManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IndexMetricsController : ControllerBase
    {
        static string id = "027D0B9A-F9D9-4C96-8213-C8546C4AAE71";
        static string partitionKey = "26C74104-40BC-4541-8EF5-9892F7F03D72";

        private Container GetContainer()
        {
            return CosmosHelper.CreateDBAndContainer("ProductDB", "Product", "categoryId").Result;
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem()
        {
            Product product = new Product()
            {
                id = id,
                categoryId = partitionKey,
                name = "LL Road Seat/Saddle",
                price = 27.12d,
                tags = new string[] { "brown", "weathered" }
            };
            ItemResponse<Product> pr;

            try
            {
                pr = await GetContainer().CreateItemAsync<Product>(product, new PartitionKey(product.categoryId));
                //pr.RequestCharge; Request charge for point operation.
                return Ok(pr);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                //Add logic to handle conflicting ids
                //400 Bad Request           - Something was wrong with the item in the body of the request
                //403 Forbidden             - Container was likely full
                //409 Conflict              - Item in container likely already had a matching id
                //413 RequestEntityTooLarge - Item exceeds max entity size
                //429 TooManyRequests       - Current request exceeds the maximum RU / s provisioned for the container
            }
            catch (CosmosException ex)
            {
                // Add general exception handling logic
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> ReadItems()
        {
            try
            {
                QueryRequestOptions options = new QueryRequestOptions()
                {
                    PopulateIndexMetrics = true,
                    PartitionKey = new PartitionKey(partitionKey)
                };

                string query = "select * from c where c.price>=32.4";
                QueryDefinition def = new QueryDefinition(query);

                FeedIterator<Product> iterator = GetContainer().GetItemQueryIterator<Product>(def, requestOptions: options);

                while(iterator.HasMoreResults)
                {
                    FeedResponse<Product> res = await iterator.ReadNextAsync();

                    var x= res.IndexMetrics.ToString(); // This will give the index recommendation if any.

                    foreach (var item in res)
                    {

                    }

                    //res.RequestCharge; Provides RU"s consumed to fetch this particular page.
                    //totalRUs += res.RequestCharge;  Total RU's for all pages.
                }

                return Ok();
            }
            catch (CosmosException ex)
            {
                return BadRequest();
            }
        }
    }
}
