# HtmlToPdf Service (using Microsoft Playwright)

This project provides a simple **HTML to PDF rendering service** built on top of [Microsoft.Playwright](https://playwright.dev/dotnet/).  
It allows you to pass in an HTML string and receive a fully rendered PDF as a byte array, with options to control PDF format, margins, context options, and more.

---

## ✨ Features
- Render raw HTML into PDF files
- Customize output via `PagePdfOptions` (page size, margins, background, headers/footers, etc.)
- Support for custom `BrowserNewContextOptions` (BaseURL, viewport, locale, etc.)
- Automatic print media emulation (`@media print` CSS support)
- Async API (`Task<byte[]>`)
- Safe resource cleanup (browser context and page disposal)

---

## 🚀 Getting Started

### 1. Install Dependencies
Make sure Playwright and its browser binaries are installed:

```bash
dotnet add package Microsoft.Playwright
dotnet build
playwright install
