
There's two ways forward as I can see, which are the most viable:

- embedded CEFSharp or CEFGlue in the WPF application.

  Problem with that one: non-portable as WPF is not Cross-Platform, so we'll have to redo the effort entirely for UNIX boxes.

  Besides: I'ld rather not do a UI upgrade / refit in WPF as I don't want to dive deep into that technology to make it perform well (lists, rendering, etc.), but fundamentally this is an emotional thing ("developer happiness") rather than a technology boundary.

- embed a Google Scholar capable browser in an Electron / Chromely / CEFGlue / CEFSharp style redo of Qiqqa

  That's a bigger project initially as we'll have to redo the entire UI in HTML5/CSS/JS, but it would be portable, at least in *theory*.

  Trouble there is that `<iframe>` is **out** to serve as a Google Scholar embed function, since Scholar and friends have similar checks and restrictions built in server-side to what's described here (emphasis mine): https://developer.mozilla.org/en-US/docs/Learn/HTML/Multimedia_and_embedding/Other_embedding_technologies#Security_concerns

    > A quick example first though — try loading the previous example we showed above into your browser — you can find it live on Github (see the source code too.) You won't actually see anything displayed on the page, and if you look at the Console in the browser developer tools, you'll see a message telling you why. In Firefox, you'll get told **Load denied by X-Frame-Options: https://developer.mozilla.org/en-US/docs/Glossary does not permit framing. This is because the developers that built MDN have included a setting on the server that serves the website pages to disallow them from being embedded inside `<iframe>`s** (see [Configure CSP directives](https://developer.mozilla.org/en-US/docs/Learn/HTML/Multimedia_and_embedding/Other_embedding_technologies#Configure_CSP_directives), below. [+](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options) [+](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/frame-ancestors) ) This makes sense — an entire MDN page doesn't really make sense to be embedded in other pages unless you want to do something like embed them on your site and claim them as your own — or attempt to steal data via clickjacking, which are both really bad things to do. Plus if everybody started to do this, all the additional bandwidth would start to cost Mozilla a lot of money.
    > 
    
## So it's CEF's `<webview>` or bust.

Or is it?

Suppose we want *most* of the Qiqqa application portable and the `<webview>` tag happens to not deliver?
    
Then we *might* consider using a *absolutely minimal native UI* base application, which splits the UI in two: 
- one part for our own stuff, however rendered (using a CEF view if we want to do it in HTML/CSS rather than WPF)
- a *second* part / half where the *independent* embedded browser resides, which mimics Chrome sufficiently for Scholar not to barf a hairball.

**Or** there's the 'browser extension' idea a la Zotero et al, which popped up yesterday while I was working on [[Links to Stuff To Look At]] and the Google Scholar access frolics that abound on the 'Net: better to *steal* a nice set of rims than flintstone your own set of tires *sans road suspension*, right? ;-)

Such a browser extension would be suboptimal in another way: you wouldn't be able to look at the PDF / bibTeX part of your Sniffer UI unless we'd rebuild that in HTML/CSS *inside* the extension, so talking about having your work cut for you... Sounds like the HTML/CSS UI way isn't so dumb / over the top as I sometimes think it is when the hours are dark. :-/


## Analysis Notes

So, after all this looking around and researching I did, there's at least one solid conclusion:

`<iframe>`s are *out*.

Now we either stick with WPF for the Sniffer -- which would mean at least one important part of our functionality depends on WPF and is thus non-portable, where the question then becomes: how much stuff do we park / keep in WPF and how much stuff do we dump elsewhere, as in: using HTML/CSS/JS technology for the UI bits -- *or* we ditch WPF and then either make `<webview>` work for us *or* use a few OS-native panels in which we run two CEF instances to accomplish what we need *or* we go a UI/UX where the sniffer moves into a native browser via a *browser extension/plugin* which then communicates with the Qiqqa app via websocket or some such, while the native browser does the work we need to provide Sniffer functionality: BibTeX/metadata extraction, PDF download redirection/fetching and (**do not forget this way of user interaction in the current Sniffer!**) copy/paste from browser pane into BibTeX editor plus copy/paste from OCR-ed PDF view into browser search/url box -- which constitutes both *manual* metadata editing and metadata *validation*, the latter being done by *eyeballing the browser page while eyeing the BibTeX panel at the same time*. (Having this done as a Chrome extension might be tecnically easier but would require moving/resizing different application screens or Alt-TABbing like mad: there's good reasons to have the panes in the Qiqqa Sniffer together as they are!)

CEF webview:

- https://bitbucket.org/chromiumembedded/cef/issues/1748/support
-     





