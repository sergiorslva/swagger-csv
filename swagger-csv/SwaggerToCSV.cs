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
                    await GenerateCSVFile();
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
            stringBuilder.AppendLine("Method;API;Operation ID;Parameters");

            using (HttpClient httpClient = new HttpClient())
            {
                var jsonResponse = await httpClient.GetStringAsync(this.URL);

                SwaggerModel swaggerModel = JsonConvert.DeserializeObject<SwaggerModel>(jsonResponse);

                foreach (var item in swaggerModel.Paths)
                {
                    string url = item.Key;
                    foreach (var key in item.Value.Keys)
                    {
                        string method = key.ToUpper();
                        string operationId = item.Value[key].OperationId;
                        string parameteres = JsonConvert.SerializeObject(item.Value[key].Parameters);

                        stringBuilder.AppendLine($"{method};{url};{operationId};{parameteres}");
                    }
                }
            };

            string extension = Path.GetExtension(this.Output);

            if(string.IsNullOrEmpty(extension) || extension.ToLower() != ".csv")
            {
                this.Output += ".csv";
            }

            using (var sw = new StreamWriter(this.Output))
            {
                sw.Write(stringBuilder.ToString());
                sw.Close();
            }
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
