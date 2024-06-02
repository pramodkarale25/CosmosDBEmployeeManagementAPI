using CosmosDBEmployeeManagementAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using static Microsoft.Azure.Cosmos.Container;

namespace CosmosDBEmployeeManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChangeFeedProcessorController : ControllerBase
    {
        ChangesHandler<Product> changeHandlerDelegate = async 
            (
                IReadOnlyCollection<Product> changes,
                CancellationToken cancellationToken
            )
            => 
        {
            foreach (Product product in changes)
            {
                await Console.Out.WriteLineAsync($"Detected Operation:\t[{product.id}]\t{product.name}");
                // Do something with each change
            }
        };

        private Container GetSourceContainer()
        {
            return CosmosHelper.CreateDBAndContainer("cosmicworks", "products", "categoryId").Result;
        }

        private Container GetLeaseContainer()
        {
            return CosmosHelper.CreateDBAndContainer("cosmicworks", "productslease", "categoryId").Result;
        }

        private async void ImplementChangeFeed()
        {
            //Need to check how we can implement this.
            Container sourceContainer = GetSourceContainer();
            Container leaseContainer = GetLeaseContainer();

            ChangeFeedProcessorBuilder processorBuilder = sourceContainer.GetChangeFeedProcessorBuilder<Product>
                (
                    processorName: "productItemProcessor",
                    onChangesDelegate: changeHandlerDelegate
                );

            ChangeFeedProcessor processor = processorBuilder
                .WithInstanceName("desktopApplication")
                .WithLeaseContainer(leaseContainer)
                .WithPollInterval(TimeSpan.FromSeconds(1))
                .WithMaxItems(5)
                .WithStartTime(DateTime.Now)
                .Build();

            processor.StartAsync();
            // Wait while processor handles items
            //Thread.Sleep(1000);
            //await processor.StopAsync();
        }
    }
}
