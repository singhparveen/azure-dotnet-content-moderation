using Microsoft.Azure.CognitiveServices.ContentModerator;
using Newtonsoft.Json;
using System.Text;

/// <summary>
/// Represents the entry point of the program.
/// </summary>
class Program
{   
    // Add your Azure Content Moderator subscription key to your environment variables.
    private static readonly string SubscriptionKey = "xxxxx";

    // Add your Azure Content Moderator endpoint to your environment variables.
    private static readonly string Endpoint = "https://xxxxx.cognitiveservices.azure.com/";

    // The function authenticates the Content Moderator client.
    public static ContentModeratorClient Authenticate(string key, string endpoint)
    {
        ContentModeratorClient client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(key));
        client.Endpoint = endpoint;

        return client;
    }
    
    // The function does the text moderation.
    public static void ModerateText(ContentModeratorClient client, string inputFile, string outputFile)
    {
        Console.WriteLine("--------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("TEXT MODERATION");
        Console.WriteLine();
        // Load the input text.
        string text = File.ReadAllText(inputFile);

        // Remove carriage returns
        text = text.Replace(Environment.NewLine, " ");
        // Convert string to a byte[], then into a stream (for parameter in ScreenText()).
        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        MemoryStream stream = new MemoryStream(textBytes);

        Console.WriteLine("Screening {0}...", inputFile);
        // Format text

        // Save the moderation results to a file.
        using (StreamWriter outputWriter = new StreamWriter(outputFile, false))
        {
            using (client)
            {
                // Screen the input text: check for profanity, classify the text into three categories,
                // do autocorrect text, and check for personally identifying information (PII)
                outputWriter.WriteLine("Autocorrect typos, check for matching terms, PII, and classify.");

                // Moderate the text
                var screenResult = client.TextModeration.ScreenText("text/plain", stream, "eng", true, true, null, true);
                outputWriter.WriteLine(JsonConvert.SerializeObject(screenResult, Formatting.Indented));
            }

            outputWriter.Flush();
            outputWriter.Close();
        }

        Console.WriteLine("Results written to {0}", outputFile);
        Console.WriteLine();
    }

    // TEXT MODERATION:  Name of the file that contains text
    private static readonly string TextFile = "TextFile.txt";

    // The name of the file to contain the output from the evaluation.
    private static string TextOutputFile = "TextModerationOutput.txt";

    // The main method for the program.
    public static void Main(string[] args)
    {
        // Initialize the client
        ContentModeratorClient clientText = Authenticate(SubscriptionKey, Endpoint);

        // Moderate text from text in a file
        ModerateText(clientText, TextFile, TextOutputFile);
    }
}