using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;

namespace swagger_csv
{    
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication<SwaggerToCSV>();
            app.Conventions.UseDefaultConventions();            

            try
            {
                app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                Console.WriteLine($"<r>{ex.Message}</r>");
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Console.CursorVisible = true;
                Console.Clear();
                Console.WriteLine($"<r>An unexpected error ocurred: <w>{ex.Message}</w></r>");
            }             
        }        
    }    
}
