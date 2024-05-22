using CosmosDBEmployeeManagementAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Container = Microsoft.Azure.Cosmos.Container;

namespace CosmosDBEmployeeManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransactionBatchOperationController : ControllerBase
    {
        //Batch operations only works with same partition key
        private record Product(string id, string name, string categoryId);
        Product saddle = new Product("0120", "Worn Saddle", "accessories-used");
        Product handlebar = new Product("012A", "Rusty Handlebar", "accessories-used");
        PartitionKey partitionKey = new PartitionKey("accessories-used");

        private Container GetContainer()
        {
            return CosmosHelper.CreateDBAndContainer("ProductDB", "Product", "categoryId").Result;
        }

        private List<Product> GetItemCollection(TransactionalBatchResponse batchRes)
        {
            List<Product> lstProduct = new List<Product>();

            if (batchRes.IsSuccessStatusCode)
            {
                TransactionalBatchOperationResult<Product> result;

                for (int i = 0; i < batchRes.Count; i++)
                {
                    result = batchRes.GetOperationResultAtIndex<Product>(i);
                    lstProduct.Add(result.Resource);
                }
            }

            return lstProduct;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMultipleItem()
        {
            TransactionalBatch batch = GetContainer().CreateTransactionalBatch(partitionKey)
                .CreateItem<Product>(saddle)
                .CreateItem<Product>(handlebar);
            using TransactionalBatchResponse batchRes = await batch.ExecuteAsync();
            return Ok(GetItemCollection(batchRes));
        }

        [HttpGet]
        public async Task<IActionResult> ReadMultipleItem()
        {
            TransactionalBatch batch = GetContainer().CreateTransactionalBatch(partitionKey)
                .ReadItem(saddle.id)
                .ReadItem(handlebar.id);
            using TransactionalBatchResponse batchRes = await batch.ExecuteAsync();
            return Ok(GetItemCollection(batchRes));
        }

        [HttpPatch]
        public async Task<IActionResult> ReplaceMultipleItem()
        {
            saddle = new Product("0120", "Worn Saddle 1", "accessories-used");
            handlebar = new Product("012A", "Rusty Handlebar 1", "accessories-used");

            TransactionalBatch batch = GetContainer().CreateTransactionalBatch(partitionKey)
                .ReplaceItem<Product>(saddle.id, saddle)
                .ReplaceItem<Product>(handlebar.id, handlebar);
            using TransactionalBatchResponse batchRes = await batch.ExecuteAsync();
            return Ok(GetItemCollection(batchRes));
        }

        [HttpPut]
        public async Task<IActionResult> UpsertMultipleItem()
        {
            saddle = new Product("0120", "Worn Saddle 2", "accessories-used");
            handlebar = new Product("012A", "Rusty Handlebar 2", "accessories-used");

            TransactionalBatch batch = GetContainer().CreateTransactionalBatch(partitionKey)
                .UpsertItem<Product>(saddle)
                .UpsertItem<Product>(handlebar);
            using TransactionalBatchResponse batchRes = await batch.ExecuteAsync();
            return Ok(GetItemCollection(batchRes));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMultipleItem()
        {
            TransactionalBatch batch = GetContainer().CreateTransactionalBatch(partitionKey)
                .DeleteItem(saddle.id)
                .DeleteItem(handlebar.id);
            using TransactionalBatchResponse batchRes = await batch.ExecuteAsync();
            return Ok(GetItemCollection(batchRes));
        }
    }
}
