using Newtonsoft.Json;
namespace ClientProject.Request
{
    public class ProductRequest
    {
        [JsonProperty("ProductId")]
        public int ProductId { get; set; }
        [JsonProperty("ProductName")]
        public string ProductName { get; set; } = null!;
        [JsonProperty("SupplierId")]
        public int? SupplierId { get; set; }
        [JsonProperty("CategoryId")]
        public int? CategoryId { get; set; }
        [JsonProperty("QuantityPerUnit")]
        public string? QuantityPerUnit { get; set; }
        [JsonProperty("UnitPrice")]
        public decimal? UnitPrice { get; set; }
        [JsonProperty("UnitsInStock")]
        public short? UnitsInStock { get; set; }
        [JsonProperty("UnitsOnOrder")]
        public short? UnitsOnOrder { get; set; }
        [JsonProperty("ReorderLevel")]
        public short? ReorderLevel { get; set; }
        [JsonProperty("Discontinued")]
        public bool Discontinued { get; set; }
    }
}
