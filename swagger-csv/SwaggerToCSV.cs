using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
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

        public async Task OnExecute(CommandLineApplication app)
        {
            try
            {
                if (IsParametersValid(app))
                {
                    Console.WriteLine("Initializing process...");
                    await GenerateCSVFile();
                    Console.WriteLine("Process finished successfully");
                }
            }
            catch (DirectoryNotFoundException)
            {
                this.console.WriteLine("Path Not Found");
            }
            catch (HttpRequestException ex)
            {
                this.console.WriteLine(ex.Message);
            }
            catch (JsonReaderException ex)
            {
                this.console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                this.console.WriteLine(ex.Message);
            }
        }

        private async Task GenerateCSVFile()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Method;API;Operation ID;Parameters;Summary");

            using (HttpClient httpClient = new HttpClient())
            {
                var jsonResponse = await httpClient.GetStringAsync(this.URL);

                SwaggerModel swaggerModel = JsonConvert.DeserializeObject<SwaggerModel>(jsonResponse);

                Console.WriteLine($"{swaggerModel.Paths.Count} paths found\n");

                foreach (var item in swaggerModel.Paths)
                {
                    string url = item.Key;
                    foreach (var key in item.Value.Keys)
                    {
                        string method = key.ToUpper();
                        string operationId = item.Value[key].OperationId;
                        string parameteres = JsonConvert.SerializeObject(item.Value[key].Parameters);
                        string summary = item.Value[key].Summary;

                        stringBuilder.AppendLine($"{method};{url};{operationId};{parameteres};{summary}");
                        Console.WriteLine($"{method} - {url}");
                    }
                }
            };
            string fileName = Path.GetFileName(this.Output);
            string extension = Path.GetExtension(this.Output);
            const string csvExtension = ".csv";

            if (string.IsNullOrEmpty(fileName))
            {
                this.Output += $"swagger-csv-{ DateTime.Now:yyyyMMdd-HHmmss}.csv";
            }
            else if (string.IsNullOrEmpty(extension) || extension.Equals(csvExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Output += ".csv";
            }

            using (var sw = new StreamWriter(this.Output, false, Encoding.UTF8))
            {
                sw.Write(stringBuilder.ToString());
                sw.Close();
            }

            Console.WriteLine($"\nThe file {this.Output} has been created.\n");
        }

        private bool IsParametersValid(CommandLineApplication app)
        {
            if (app.Options.All(o => o.Values.Count == 0))
            {
                app.ShowHelp();
                return false;
            }

            if (string.IsNullOrEmpty(this.URL))
            {
                app.ShowHint();
                this.console.WriteLine("URL must be supllied");
                return false;
            }

            return true;
        }
    }
}
