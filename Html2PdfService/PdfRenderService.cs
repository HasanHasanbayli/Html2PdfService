using Microsoft.Playwright;

namespace WebAPI;

public class PdfRenderService
{
    private T Configure<T>(T options, Action<T>? configure)
    {
        configure?.Invoke(options);

        return options;
    }

    public async Task<byte[]> RenderPdfAsync(
        string html,
        Action<PagePdfOptions>? configurePdf = null,
        Action<BrowserNewContextOptions>? configureContext = null,
        Action<PageEmulateMediaOptions>? configureMedia = null,
        Action<PageSetContentOptions>? configureContent = null)
    {
        if (string.IsNullOrWhiteSpace(html))
            throw new ArgumentException("HTML content must not be null or empty.", nameof(html));

        PagePdfOptions pdfOptions = Configure(new PagePdfOptions
        {
            Format = "A4",
            PreferCSSPageSize = true,
            PrintBackground = true,
            Margin = new Margin { Top = "40px", Bottom = "40px", Left = "10px", Right = "10px" },
            DisplayHeaderFooter = false
        }, configurePdf);

        BrowserNewContextOptions contextOptions = Configure(new BrowserNewContextOptions(), configureContext);

        PageEmulateMediaOptions mediaOptions = Configure(new PageEmulateMediaOptions
        {
            Media = Media.Print
        }, configureMedia);

        PageSetContentOptions contentOptions = Configure(new PageSetContentOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = 20_000
        }, configureContent);

        IBrowser browser = await PlaywrightHost.GetBrowserAsync();

        await using IBrowserContext context = await browser.NewContextAsync(contextOptions);
        IPage page = await context.NewPageAsync();

        try
        {
            await page.EmulateMediaAsync(mediaOptions);
            await page.SetContentAsync(html, contentOptions);
            return await page.PdfAsync(pdfOptions);
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}