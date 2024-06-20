using CosmosDBEmployeeManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;

namespace CosmosDBEmployeeManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SP_UDF_Triggers_Controller : ControllerBase
    {
        private record Product(string id, string name, string categoryId);
        Product saddle = new Product("0120", "Worn Saddle", "accessories-used");
        PartitionKey partitionKey = new PartitionKey("accessories-used");
        private Container GetContainer()
        {
            return CosmosHelper.CreateDBAndContainer("ProductDB", "Product", "categoryId").Result;
        }

        [HttpPost]
        public async Task CreateStoredProc()
        {
            string storedProc = @"
                                    function createProduct(item)
                                    {
                                        var context = getContext();
                                        var container = context.getCollection(); 
                                        var accepted = container.createDocument(
                                            container.getSelfLink(),
                                            item,
                                            (error, newItem) => {
                                                if (error) throw error;
                                                context.getResponse().setBody(newItem)
                                            }
                                        );
                                        if (!accepted) return;
                                    }
                                ";

            StoredProcedureProperties storedProcedureProperties = new StoredProcedureProperties()
            {
                Id = "CreateProductSP",
                Body = storedProc
            };

            StoredProcedureResponse storedProcedureResponse = await GetContainer().Scripts.CreateStoredProcedureAsync(storedProcedureProperties);
        }

        [HttpPost]
        public async Task CreateUDF()
        {
            string UDF = @"
                            function getTaxedPrice(price)
                            {
                                return (price * 1.5);
                            }
                        ";

            UserDefinedFunctionProperties userDefinedFunctionProperties = new UserDefinedFunctionProperties()
            {
                Id = "getTaxedPriceUDF",
                Body = UDF
            };

            UserDefinedFunctionResponse UdfResponse = await GetContainer().Scripts.CreateUserDefinedFunctionAsync(userDefinedFunctionProperties);
        }

        [HttpPost]
        public async Task CreatePreTrigger()
        {
            string UDF = @"
                            function AddPrice()
                            {
                               var context = getContext();
                               var request = context.getRequest();
                               var item = request.getBody();
                                
                               if (!('price' in item))
                                    item['price'] = 100;

                                request.setBody(item);
                            }
                        ";

            UserDefinedFunctionProperties userDefinedFunctionProperties = new UserDefinedFunctionProperties()
            {
                Id = "AddPricePreTriggerUDF",
                Body = UDF
            };

            UserDefinedFunctionResponse UdfResponse = await GetContainer().Scripts.CreateUserDefinedFunctionAsync(userDefinedFunctionProperties);
        }

        [HttpPost]
        public async Task CreatePostTrigger()
        {
            string UDF = @"
                            function CreateNewProduct()
                            {
                               var context = getContext();
                               var container = context.getCollection();
                               var request = context.getResponse();
                               var item = request.getBody();
                                
                               var newProduct = 
                                            {
                                                id : '1254',
                                                name: 'new item',
                                                categoryId : item.categoryId
                                            }

                                   var accepted = container.createDocument(
                                    container.getSelfLink(),
                                    newProduct,
                                    (error, newItem) => {
                                        if (error) throw error;
                                    }
                                );
                                if (!accepted) return;
                            }
                        ";

            UserDefinedFunctionProperties userDefinedFunctionProperties = new UserDefinedFunctionProperties()
            {
                Id = "CreateNewProductPostTriggerUDF",
                Body = UDF
            };

            UserDefinedFunctionResponse UdfResponse = await GetContainer().Scripts.CreateUserDefinedFunctionAsync(userDefinedFunctionProperties);
        }
    }
}
