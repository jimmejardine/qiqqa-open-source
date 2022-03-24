# Cross-Platform UI: wxWidgets or CEF Views?

*New Qiqqa* must come with a cross-platform UI, which supports Windows, Linux and Mac.

We have been looking at using wxWidgets + WebView (wxWidgets+CEF is unsupported and hairy).

Not much trouble, except we'ld be using different browser engines for our UI in different platforms, potentially causing issues with our web-technologies based UI.

The alternative, that's been in the picture since we started looking at CEF, is CEF Views, plus a bare metal minimal wrapper for each platform.

*That* means we'ld have a (slightly) different codebase for each platform, which is where wxWindows pops up as a we-covered-that-hassle -- while it comes itself with the afore-mentioned issue of using different engines on different platforms to render the UI HTML.

The [Cross Platform GUI Trainwreck article](https://blog.johnnovak.net/2016/05/29/cross-platform-gui-toolkit-trainwreck-2016-edition/) puts it succinctly: whatever you choose, you're SOL (and out of karma).

While I might still dither a bit between bare-metal minimal native + CEF vs. wxWindows+WebView for my UI and Qiqqa Sniffer/Browser needs, I've come to the conclusion that the rest is cute in all their own ways, but not fit for me and my purposes & preferences.

While [Trainwreck](https://blog.johnnovak.net/2016/05/29/cross-platform-gui-toolkit-trainwreck-2016-edition/) dates back to 2016, here's [another daring soul from 2019 AD](https://www.hohlerde.org/rauch/en/blog/2019-12-08-GUI-Frameworks/) taking a stab at it. He goes for Qt in the end, with some grumbling. I myself am not fond of Qt at all, so I'll stick with wxWindows then? However, UI flickering is mentioned and [a link to the wxWiki provided](https://wiki.wxwidgets.org/Flicker-Free_Drawing) where this is discussed more in-depth.
From the noises there I gather *wx* doesn't suffer from that issue anymore, or very little, in 2021AD... something to (re-)check myself soon!



### So whatchammagonnado?

*\<sarcasm\>* Revert to one of my oldest friends of yonder days: [TVision?](https://github.com/magiblot/tvision) **Now *that* is what I'ld call *cool* and *totally RETRO*. Utterly!**  :+1: *\</sarcasm\>*

> But should we redo Qiqqa UI in that one? *Come on*...






## References

- [CEF: Expose the Views/Aura framework as an alternative API for client applications](https://bitbucket.org/chromiumembedded/cef/issues/1749/)
- [How to combine another framework's event loop to wxWidgets's event loop](https://forums.wxwidgets.org/viewtopic.php?f=1&t=48665&sid=1040cbb50f88fbf23d6603185a3ea234)
- [Multiple major keyboard focus issues on Linux (branches 2883, 2840, 2785)](https://bitbucket.org/chromiumembedded/cef/issues/2026/multiple-major-keyboard-focus-issues-on)
- https://github.com/chromiumembedded/cef
- https://bitbucket.org/chromiumembedded/cef/issues/1258/linux-use-aura-instead-of-gtk
- https://softwarerecs.stackexchange.com/questions/52332/skinnable-multiplatform-free-gui-library-for-c
- https://blog.johnnovak.net/2016/05/29/cross-platform-gui-toolkit-trainwreck-2016-edition/
- https://www.cockos.com/wdl/ & https://github.com/justinfrankel/WDL
- https://docs.microsoft.com/en-us/dotnet/maui/what-is-maui : MAUI is **not for Linux**. *Plus*: it's XAML.🤮 
- https://github.com/Immediate-Mode-UI/Nuklear : like IMGUI. Not For Qiqqa, which is in need of a full web browser anyway.
- https://www.hohlerde.org/rauch/en/blog/2019-12-08-GUI-Frameworks/ 
- https://www.libhunt.com/compare-cef-vs-wxWidgets?ref=compare
- https://groups.google.com/g/wx-dev/c/JInhqwyRB8U / https://trac.wxwidgets.org/ticket/19209 - Retina/HighDPI woes with wx: resolved.
- https://github.com/wxWidgets/Phoenix/issues/288#issuecomment-294896145 / https://groups.google.com/g/cefpython/c/DheRybqvcI0 - OSX wx details that might become relevant once we add OSX support ourselves, despite *not* using wxPython.
- https://news.ycombinator.com/item?id=9793920
- https://tech-in-japan.github.io/articles/475092/index.html -- another fellow traveller. MHTML woes, etc.
- http://trac.wxwidgets.org/ticket/19041
- https://news.ycombinator.com/item?id=24117758
- [Flutter](https://flutter.dev/) - *sigh*: Win32 application development seems to have a lower priority for them? [caveats about Win platform on the site.](https://flutter.dev/desktop) :-S
- https://news.ycombinator.com/item?id=26194990 - includes comments that ring true with me: using CEF for all platforms would mean I don't have to bother with the various browser quirks on there. wxWebView doesn't provide that.
- https://federicoterzi.com/blog/why-electron-is-a-necessary-evil/
- https://github.com/Elanis/web-to-desktop-framework-comparison
- 