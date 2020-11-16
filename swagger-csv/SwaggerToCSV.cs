using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace swagger_csv
{
    [Command(Name = "swagger-csv",
             Description = "Swagger to CSV",
             OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [HelpOption("-?")]
    public class SwaggerToCSV : ConfigurationOptions
    {
        private readonly IConsole console;

        public SwaggerToCSV(IConsole console)
        {
            this.console = console;
        }

        public async Task<int> OnExecute(CommandLineApplication app)
        {
            if (app.Options.All(o => o.Values.Count == 0))
            {
                app.ShowHelp();
                return -1;
            }

            if (string.IsNullOrEmpty(this.URL))
            {
                app.ShowHint();
                this.console.WriteLine("URL must be supllied");
                return -2;
            }

            HttpClient httpClient = new HttpClient();
            var jsonResponse = await httpClient.GetStringAsync(this.URL);

            SwaggerJSON swaggerJSON = JsonConvert.DeserializeObject<SwaggerJSON>(jsonResponse);
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Verbo;API");

            foreach (KeyValuePair<string, Dictionary<string, object>> api in swaggerJSON.Paths)
            {
                foreach (KeyValuePair<string, object> verbo in api.Value)
                {
                    stringBuilder.AppendLine($"{verbo.Key};{api.Key}");
                }
            }

            using (var sw = new StreamWriter(this.Output))
            {
                sw.Write(stringBuilder.ToString());
                sw.Close();
            }

            return 0;
        }
    }

    public class SwaggerJSON
    {
        public Dictionary<string, Dictionary<string, object>> Paths { get; set; }
    }
}
