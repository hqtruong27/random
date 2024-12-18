//using Microsoft.Playwright;

//namespace Shared.Extensions;

//public static class PlaywrightExtensions
//{
//    public static async Task<IBrowser> LaunchChromiumAsync(this IPlaywright playwright)
//    {
//        playwright = await Playwright.CreateAsync();
//        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
//        {
//            Headless = true
//        });

//        return browser;
//    }

//    public static Task CloseAsync(this IBrowser browser)
//    {
//        return browser.CloseAsync();
//    }
//}
