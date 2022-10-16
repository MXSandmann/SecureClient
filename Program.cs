using Microsoft.Identity.Client;
using SecureClient;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;


Console.WriteLine("Making a request...");
RunAsync().GetAwaiter().GetResult();

static async Task RunAsync()
{
    //AuthConfig config = AuthConfig.ReadJsonFromFile("appsettings.json");
    AuthConfig config = AuthConfig.ReadFromSecrets("appsettings.json");

    IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
        .WithClientSecret(config.ClientSecret)
        .WithAuthority(new Uri(config.Authority))
        .Build();

    string[] resourceIds = new string[] { config.ResourceId };

    AuthenticationResult result = null!;

    try
    {
        result = await app.AcquireTokenForClient(resourceIds).ExecuteAsync();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Token acquired: \n" + result.AccessToken);
    }
    catch (MsalClientException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
    }
    finally
    {
        Console.ResetColor();
    }

    if (string.IsNullOrEmpty(result.AccessToken))
    {
        Console.WriteLine("No token recieved");
        return;
    }

    await Request(config, result);
}

static async Task Request(AuthConfig config, AuthenticationResult result)
{
    var httpClient = new HttpClient();
    var defaultRequestHeaders = httpClient.DefaultRequestHeaders;

    if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType!.Equals("application/json")))
    {
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);

    var response = await httpClient.GetAsync(config.BaseAddress);

    if (response.IsSuccessStatusCode)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine(json);
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Failed to call API: {response.StatusCode}");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Content: {content}");

    }
    Console.ResetColor();
}