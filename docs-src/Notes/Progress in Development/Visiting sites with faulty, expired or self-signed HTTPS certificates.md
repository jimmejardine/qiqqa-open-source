# Visiting sites with faulty, expired or self-signed HTTPS/SSL certificates

For example: the doi2bib.org search site.


## For WebView2

- https://stackoverflow.com/questions/62512147/bypass-invalid-ssl-certificate-for-kestrel-server-displayed-in-webview2

   > The WebView2 doesn't currently directly expose that feature. If you like, you can open an issue in [WebView2 Feedback](https://github.com/MicrosoftEdge/WebViewFeedback/issues) and we can make a feature request.
   > 
   > As a workaround you might try using the [CoreWebView2.CallDevToolsProtocolMethodAsync method](https://docs.microsoft.com/en-us/microsoft-edge/webview2/reference/dotnet/0-9-538/microsoft-web-webview2-core-corewebview2#calldevtoolsprotocolmethodasync) to invoke the [Security.setIgnoreCertificateErrors DevTools Protocol method](https://chromedevtools.github.io/devtools-protocol/tot/Security/#method-setIgnoreCertificateErrors). However, I haven't tried setIgnoreCertificateErrors out, and its also marked experimental so not positive it will work in the manner you'd like.
   > 
   > >   I can confirm that this works (WebView2 1.0.818.41) and this line of code `var result = await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Security.setIgnoreCertificateErrors", "{\"ignore\": true}");` result is an empty json object and I placed this code inside the `webView.CoreWebView2InitializationCompleted` event handler. 
   > >   
   > >   – [Jürgen Steinblock](https://stackoverflow.com/users/98491/j%c3%bcrgen-steinblock "27,861 reputation"), [Jun 28 at](https://stackoverflow.com/questions/62512147/bypass-invalid-ssl-certificate-for-kestrel-server-displayed-in-webview2#comment120474870_62519111)
   > 
   > ---
   > 
   > Follow-up/related: https://github.com/MicrosoftEdge/WebView2Feedback/issues/3

- https://docs.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2weberrorstatus?view=webview2-dotnet-1.0.992.28
- https://docs.microsoft.com/en-us/microsoft-edge/webview2/release-notes
- 

## For CEF3

Not much to find. However, there's info for CEFSharp, which is a wrapper for CEF3:

- https://stackoverflow.com/questions/35555754/how-to-bypass-ssl-error-cefsharp-winforms

   > Option 1 (Preferred):
   > 
   > Implement [IRequestHandler.OnCertificateError](http://cefsharp.github.io/api/89.0.x/html/M_CefSharp_IRequestHandler_OnCertificateError.htm) - this method will be called for every invalid certificate. If you only wish to override a few methods of `IRequestHandler` then you can inherit from [RequestHandler](http://cefsharp.github.io/api/89.0.x/html/T_CefSharp_Handler_RequestHandler.htm) and `override` the methods you are interested in specifically, in this case `OnCertificateError`

- https://chromedevtools.github.io/devtools-protocol/tot/Security/#method-setIgnoreCertificateErrors
- 

