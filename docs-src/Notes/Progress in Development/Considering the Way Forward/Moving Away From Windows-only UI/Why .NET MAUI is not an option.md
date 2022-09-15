# Why .NET MAUI is *not an option*

This pops up now and again. Cross platform. .NET. Shouldn't be a *big move*, hm?! 
<sup>(Rather than going and using something *totally else*, e.g. wxWindows, <i>pardon</i>, wxWidgets.)</sup>

Well, the definitive answer -- which I'll need to re-read again somewhere in future I'm sure -- to the question "Why not just go with MAUI?" is...

Not because it's XAML based 🧐 but because of [**this**](https://docs.microsoft.com/en-us/dotnet/maui/supported-platforms). 
It's 2022 AD by now and we still are at that point where MAUI is exhorted as cross-platform-*baby*-*yeah!* by [the evangelists on the .net](https://devblogs.microsoft.com/dotnet/introducing-dotnet-maui-one-codebase-many-platforms/), but the pure unadulterated reality is this, and I *quote*:

---

# Supported platforms for .NET MAUI apps

(08/22/2022, 2 minutes to read)

.NET Multi-platform App UI (.NET MAUI) apps can be written for the following platforms:

-   Android 5.0 (API 21) or higher.
-   iOS 10 or higher.
-   macOS 10.15 or higher, using Mac Catalyst.
-   Windows 11 and Windows 10 version 1809 or higher, using [Windows UI Library (WinUI) 3](https://docs.microsoft.com/en-us/windows/apps/winui/winui3/).

.NET MAUI Blazor apps have the following additional platform requirements:

-   Android 7.0 (API 24) or higher is required
-   iOS 14 or higher is required.
-   macOS 11 or higher, using Mac Catalyst.

.NET MAUI Blazor apps also require an updated platform specific WebView control. For more information, see [Blazor supported platforms](https://docs.microsoft.com/en-us/aspnet/core/blazor/supported-platforms).

.NET MAUI apps for Android, iOS, and Windows can be built in Visual Studio. However, a networked Mac is required for iOS development.

## Additional platform support

.NET MAUI also includes Tizen support, which is provided by Samsung.

----

Okay. Now answer me this: **do you see any "Linux" mention in there, or do I, after all, need new medication and an eye replacement, or two?**

And by-the-by, if you don't trust the info from the horse's mouth (Microsoft), there's [this](https://github.com/Elanis/web-to-desktop-framework-comparison), for example. Where the OS support row named "Linux" is all-green across the field when spotted on me mo'bee, except you do a bit of swipe to see the MAUI column show and observe it say... "*Soon*". Rrrrright. And I've got a bridge to sell ya in case you're interested in a great horizontal investment opportunity.

Yeah. It's the **UI support** I'm after, not some funky server/core routines, that everyone and their pet gerbil have in spades already, .NET or not. 
Oh! Oh! Oh! But! [*Blazor!*](https://docs.microsoft.com/en-us/dotnet/maui/user-interface/controls/blazorwebview) Dude! ... 
🤧I'm not looking for/at doing web *apps* of any kind 🤢; besides, which bit in the quote above did you *miss*? Blazor is even less keenly pouring for the opposition. What I want is an embeddedable full-fledged *web browser* 🌍 so I can easily view (🤗 and possibly grab stuff off 😍) *others'* web pages *of any kind*. Edge/Chrome/FF style, 🤠 *nøwhummæssayin*? 
For *that* to get anywhere *near* your hamper when it's Santa time, you'll need some jazz like [webview](https://github.com/WebView/webview) and a kindred spirit with way too much time to spare to get it into your UI framework of choice. Or **something** along those lines. 😶 For it has to dazzle Google Scholar *et al*. 🙏 At least for *one second* or so. 🙏

