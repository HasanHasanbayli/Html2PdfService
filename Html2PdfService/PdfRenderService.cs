using Microsoft.Playwright;

namespace WebAPI;

public class PdfRenderService
{
    public async Task<byte[]> RenderPdfAsync(
        string htmlData,
        PagePdfOptions? pagePdfOptions = null,
        BrowserNewContextOptions? browserNewContextOptions = null,
        PageEmulateMediaOptions? pageEmulateMediaOptions = null,
        PageSetContentOptions? pageSetContentOptions = null)
    {
        if (string.IsNullOrWhiteSpace(htmlData))
            throw new ArgumentException("HTML content must not be null or empty.", nameof(htmlData));

        // Ensure we have an options object (Playwright handles null too, but being explicit is clearer)
        browserNewContextOptions ??= new BrowserNewContextOptions();
        pageEmulateMediaOptions ??= new PageEmulateMediaOptions { Media = Media.Print };
        pageSetContentOptions ??= new PageSetContentOptions
        {
            // NetworkIdle is often more reliable for PDF rendering if there are external assets
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = 20_000
        };
        pagePdfOptions ??= new PagePdfOptions
        {
            Format = "A4",
            PreferCSSPageSize = true,
            PrintBackground = true,
            Margin = new Margin
            {
                Top = "40px", Bottom = "40px",
                Left = "10px", Right = "10px"
            },
            DisplayHeaderFooter = false
        };

        IBrowser browser = await PlaywrightHost.GetBrowserAsync();

        await using IBrowserContext context = await browser.NewContextAsync(browserNewContextOptions);
        IPage page = await context.NewPageAsync();

        try
        {
            // Apply print media so @media print CSS is respected
            await page.EmulateMediaAsync(pageEmulateMediaOptions);

            // Load the provided HTML, wait for network to be idle for more stable output
            await page.SetContentAsync(htmlData, pageSetContentOptions);

            // Produce the PDF
            return await page.PdfAsync(pagePdfOptions);
        }
        finally
        {
            // Ensure tab is closed even if something fails
            await page.CloseAsync();
        }
    }
}