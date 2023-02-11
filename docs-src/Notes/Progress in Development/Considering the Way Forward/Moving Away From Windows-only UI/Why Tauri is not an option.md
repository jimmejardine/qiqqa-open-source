# Why [Tauri](https://github.com/tauri-apps/tauri) is not an option

[Tauri](https://github.com/tauri-apps/tauri)  showed up on our radar while we were investigating `electron`-style options for our GUI approach and it ticked all the boxes *initially*:

- cross-platform for desktop applications: Windows, Linux and Mac
- leaner: `electron` without the load of CEF+NodeJS, just for the front-end app
- *dev diverse*: though only a minor priority, I think it would be good if we have the entire Qiqqa architecture done using a couple of programming languages, instead of just one (previously: C#/.NET) as this would allow other developers to step up, chime in and contribute to clearly separable parts of the Qiqqa system. Tauri is done in Go, which is pretty mainstream AFAIAC.
- no XAML: though Tauri appears to have its own tag-based layout system 🤔

## Not an option after all

What is *mandatory* for any new GUI dev system is the capability to host/serve *multiple [WebViews]([Webview Versions | Tauri Apps](https://tauri.app/v1/references/webview-versions)) with different security settings*: the Qiqqa Sniffer *works* by being a UI which serves both local content (active PDF: pages + metadata) **and** a (*possibly unsafe*) web search browser view (for Google Scholar, et al). The latter needs to be a full-fledged browser view, **not an iframe**, to ensure cookie management and browsing safety concerns can be dealt with properly by having that panel exist in its own compartment/context.

Tauri doesn't offer this capability (of two WebView panes in the same window) and does not intend to offer that capability either: [Multiple webviews in one window · Issue #2975 · tauri-apps/tauri (github.com)](https://github.com/tauri-apps/tauri/issues/2975)

