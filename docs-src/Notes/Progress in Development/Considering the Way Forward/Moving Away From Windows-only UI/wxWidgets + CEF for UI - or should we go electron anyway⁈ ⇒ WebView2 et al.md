# wxWidgets + CEF for UI - or should we go `electron` anyway? ⇒ WebView2 et al

wxWidgets also supports OS-native WebView panes: [migrate to WebView2 on Windows](https://forums.wxwidgets.org/viewtopic.php?t=47638) --> [wxWebView Class Reference](https://docs.wxwidgets.org/trunk/classwx_web_view.html) + `wxWEBVIEW_BACKEND_EDGE` (f.e. [wxPython Phoenix WebView documentation](https://docs.wxpython.org/wx.html2.WebView.html))

This means we can have WebView2 goodness in our GUI i.e. web-based UI development + a WebView2 instance serving as *embedded browser* for the Qiqqa Sniffer.

Should we do the 'local' part of the UI (i.e. anything that doesn't *require* a (embedded) web browser) natively in wxWidgets or only use wxWidgets as a carrier for the web-based UI via full-size webview panes? 🤔

Initially I was inclined to go the *native UI* way (2021) but the counter-argument remains strong for doing the entire UI in web-based tech (webview):
- easier to render advanced graphs (brainstorm, expedition) by using widely employed OSS charting frameworks, e.g. D3. With which we already have experience, to boot. Native solutions, such as wxCharts, are available and pretty nice, but a bit lacking in the *force graph* and other complex graph layouts arena.
- Qiqqa Sniffer would require *two*(2) compartmentalized webviews: one for the UI + local data (top half) + one for the Scholar Search/web browser (bottom half; will visit unsafe web sites with that one, so it needs to be sandboxed!). Anything else in Qiqqa UI can do with a single webview (all local data), except the Qiqqa Browse view, which is like the Sniffer: an embedded web browser that is intended to visit external websites and thus needs to be sandboxed from the rest of the application.
- easier to style and (re)design the GUI in web tech: as we will already carry the cost of a full-fledged webview anyway (Sniffer + Browse panes), we could use it to style the rest of the UI as well on all platforms. While this won't look 'native', we can produce easy-to-use & neat designs.
- using web tech (HTML/CSS/JS) for the GUI enables a much larger pool of developers to contribute to Qiqqa then when we would do it in wxWidgets natively: the latter requires wxWidgets + C++ experience and, while relatively easy, is not a huge developer pool. Using web tech and an 'electron-like dev experience' on the other hand has much more affinity with a very large developer pool. What we need to do then is to get the base pane system set up in native wxWidgets and offer that as a development platform for anyone contributing to the web tech based Qiqqa GUI. (All the "real work" is to be done in the *local* server executables anyway, separating back-end tech (database, etc.) from GUI plus UI business logic)



