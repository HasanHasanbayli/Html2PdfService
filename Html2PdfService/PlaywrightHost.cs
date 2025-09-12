using Microsoft.Playwright;

namespace WebAPI;

public static class PlaywrightHost
{
    // One-time, thread-safe lazy bootstrap (no DI)
    private static readonly Lazy<Task<IBrowser>> BrowserLazy = new(CreateBrowserAsync, isThreadSafe: true);

    public static Task<IBrowser> GetBrowserAsync() => BrowserLazy.Value;

    private static async Task<IBrowser> CreateBrowserAsync()
    {
        IPlaywright pw = await Playwright.CreateAsync();
        return await pw.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args =
            [
                "--no-sandbox", // optional (useful in containers)
                "--disable-dev-shm-usage" // optional (useful in containers)
            ]
        });
    }
}