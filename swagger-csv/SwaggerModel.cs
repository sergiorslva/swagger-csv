using Newtonsoft.Json;
using System.Collections.Generic;

namespace swagger_csv
{
    public class SwaggerModel
    {
        [JsonProperty("swagger")]
        public string swagger { get; set; }

        [JsonProperty("paths")]
        public Dictionary<string, Dictionary<string, SwaggerTagModel>> Paths { get; set; }
    }

    public class SwaggerTagModel
    {
        [JsonProperty("operationId")]
        public string OperationId { get; set; }

        [JsonProperty("parameters")]
        public List<SwaggerParameterModel> Parameters { get; set; }

    }

    public class SwaggerParameterModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("in")]
        public string In { get; set; }

        [JsonProperty("required")]
        public string Required { get; set; }
    }
}
