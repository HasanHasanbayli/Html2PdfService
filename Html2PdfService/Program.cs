using WebAPI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// TODO: Ensure Playwright is installed before running this service.
//       You can do this once at application startup by calling:
//           Microsoft.Playwright.Program.Main(new[] { "install" });
//       This will download the required browser binaries (Chromium, Firefox, WebKit).
//       Without it, Playwright will throw errors when trying to launch a browser.
Microsoft.Playwright.Program.Main(["install"]);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PdfRenderService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/print", async (PdfRenderService pdfRender) =>
{
    string fileRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "doc.html");
    string htmlData = File.ReadAllText(fileRoot);
    
    byte[] pdfBytes = await pdfRender.RenderPdfAsync(htmlData);

    return Results.File(pdfBytes, "application/pdf", "document.pdf");
});

app.Run();