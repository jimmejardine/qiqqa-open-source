# Why most electron alternatives are not an option

As addressed in, for example, [[Why Tauri is not an option]], most `electron` alternatives don't list the ability to show/render dual independently sandboxed webviews in a single window -- and I'm **not** interested in any **iframe** there, for I need a properly sandboxed webview that presents itself to obnoxious web sites (such as Google Scholar!) as a full browser view: many of those search sites we need / wish to use, have particular and *strong* defenses against use inside an *iframe* of any kind, so we must be able to cope with that adversarial scenario in our GUI core framework.

## Investigated contenders:

- [Electron](https://www.electronjs.org/) : too bulky, looks like it's on its way out re dev trends besides, *plus* no happy noises about sandboxed dual webview panes either. 😟
- [Tauri](https://tauri.app/) : [[Why Tauri is not an option]]
- [Neutralino](https://neutralino.js.org/) : [[Why most electron alternatives are not an option]]
- [Chromely](http://chromely.org/) :: [What does Chromely offer extra over MS's WebView2? · Issue #369](https://github.com/chromelyapps/Chromely/issues/369)
  + [EdgeSharp](https://github.com/webview2/EdgeSharp): [Linux and Mac 🚫 · Issue #2](https://github.com/webview2/EdgeSharp/issues/2)
- [webview](https://github.com/webview/webview) :: is still considered a *potentially viable* option, but we expect wxWidgets to be easier to deliver a good-looking, well-behaved, UI cross-platform.
- [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui?view=net-maui-7.0) : [Supported platforms for .NET MAUI apps: 🚫Linux🚫](https://learn.microsoft.com/en-us/dotnet/maui/supported-platforms?view=net-maui-7.0) (plus they can't leave that XAML *sh✪te* alone apparently: XML DSLs vs. HTML5 🤦) 
  See also [[Why .NET MAUI is not an option]].
- ... (I forgot, but there's a couple more) ...

