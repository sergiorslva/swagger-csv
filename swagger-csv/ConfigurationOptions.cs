using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;

namespace swagger_csv
{
    public class ConfigurationOptions
    {        
        [Option(CommandOptionType.SingleValue, Template = "-u|--url",
                        Description = "Required. Swagger URL to serialize.",
                        ShowInHelpText = true, ValueName = "URL")]
        public string URL { get; set; }

        [Option(CommandOptionType.SingleValue, Template = "-f|--file",
                    Description = "Optional. File name to generate",
                    ShowInHelpText = true, ValueName = "Output")]
        public string Output { get; set; } = $"swagger-csv-{DateTime.Now.ToString("hhmmss")}.csv";
    }
}
