using Newtonsoft.Json;

namespace API_Categories.Model
{
    public class Category
    {
        [JsonProperty(PropertyName = "id")]
        public string? CategoryID { get; set; }
        [JsonProperty(PropertyName = "CategoryName")]
        public string? CategoryName { get; set; }
        [JsonProperty(PropertyName = "Description")]
        public string? Description { get; set; }
        [JsonProperty(PropertyName = "Picture")]
        public string? Picture { get; set; }
    }
}
