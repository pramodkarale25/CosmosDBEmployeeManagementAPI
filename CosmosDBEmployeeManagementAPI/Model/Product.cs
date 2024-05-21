using Newtonsoft.Json;

namespace CosmosDBEmployeeManagementAPI.Model
{
    public class Product
    {
        public string id { get; set; }

        public string name { get; set; }

        public string categoryId { get; set; }

        public double price { get; set; }

        public string[] tags { get; set; }

        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? ttl { get; set; }
    }
}
