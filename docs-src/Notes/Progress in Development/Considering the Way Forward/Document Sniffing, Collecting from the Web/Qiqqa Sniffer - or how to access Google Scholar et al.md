
There's two ways forward as I can see, which are the most viable:

- embedded CEFSharp or CEFGlue in the WPF application.

  Problem with that one: non-portable as WPF is not Cross-Platform, so we'll have to redo the effort entirely for UNIX boxes.

  Besides: I'ld rather not do a UI upgrade / refit in WPF as I don't want to dive deep into that technology to make it perform well (lists, rendering, etc.), but fundamentally this is an emotional thing ("developer happiness") rather than a technology boundary.

- embed a Google Scholar capable browser in an Electron / Chromely / CEFGlue / CEFSharp style redo of Qiqqa

  That's a bigger project initially as we'll have to redo the entire UI in HTML5/CSS/JS, but it would be portable, at least in *theory*.

  Trouble there is that `<iframe>` is **out** to serve as a Google Scholar embed function, since Scholar and friends have similar checks and restrictions built in server-side to what's described here (emphasis mine): [MDN: From object to iframe — other embedding technologies: Security Concerns](https://developer.mozilla.org/en-US/docs/Learn/HTML/Multimedia_and_embedding/Other_embedding_technologies#Security_concerns)

    > A quick example first though — try loading the previous example we showed above into your browser — you can find it live on Github (see the source code too.) You won't actually see anything displayed on the page, and if you look at the Console in the browser developer tools, you'll see a message telling you why. In Firefox, you'll get told 
    >
    > > **Load denied by X-Frame-Options: https://developer.mozilla.org/en-US/docs/Glossary does not permit framing.**
    >
    > **This is because the developers that built MDN have included a setting on the server that serves the website pages to disallow them from being embedded inside `<iframe>`s** (see [Configure CSP directives](https://developer.mozilla.org/en-US/docs/Learn/HTML/Multimedia_and_embedding/Other_embedding_technologies#Configure_CSP_directives), below. [+](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options) [+](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/frame-ancestors) ) This makes sense — an entire MDN page doesn't really make sense to be embedded in other pages unless you want to do something like embed them on your site and claim them as your own — or attempt to steal data via clickjacking, which are both really bad things to do. Plus if everybody started to do this, all the additional bandwidth would start to cost Mozilla a lot of money.
    > 
    
## So it's CEF's `<webview>` or bust.

Or is it?

Suppose we want *most* of the Qiqqa application portable and the `<webview>` tag happens to not deliver?
    
Then we *might* consider using a *absolutely minimal native UI* base application, which splits the UI in two: 
- one part for our own stuff, however rendered (using a CEF view if we want to do it in HTML/CSS rather than WPF)
- a *second* part / half where the *independent* embedded browser resides, which mimics Chrome sufficiently for Scholar not to barf a hairball.

**Or** there's the 'browser extension' idea a la Zotero et al, which popped up yesterday while I was working on [[Links to Stuff To Look At]] and the Google Scholar access frolics that abound on the 'Net: better to *steal* a nice set of rims than flintstone your own set of tires *sans road suspension*, right? ;-)

Such a browser extension would be suboptimal in another way: you wouldn't be able to look at the PDF / bibTeX part of your Sniffer UI unless we'd rebuild that in HTML/CSS *inside* the extension, so talking about having your work cut for you... Sounds like the HTML/CSS UI way isn't so dumb / over the top as I sometimes think it is when the hours are dark. :-/

### `<webview>` --> Electron *et al*?

I've looked at Electron and various similar solutions which might be more closely related to C#/.NET -- which is what we're currently using as a programming language in Qiqqa.
Chromely might be a viable candidate as it's not using WinForms or WPF and is advertised as Cross Platform (Win, Mac, Linux). Electron.NET is what I looked at before that, but it's a bit too much ASP.NET oriented to my tastes and I don't wish to exchange WPF for ASP.NET: I believe we can do a fine UI without any need for ASP.NET.

Meanwhile -- and **this is the important bit to consider across the board, irrespective of which UI/CEF solution we pick** -- there's this page: https://www.electronjs.org/docs/api/webview-tag#warning. Quoting (emphasis mine):

> ## Warning
>
> Electron's `webview` tag is based on [Chromium's `webview`](https://developer.chrome.com/apps/tags/webview), which is undergoing dramatic architectural changes. This impacts the stability of `webviews`, including rendering, navigation, and event routing. 
>
> **We currently recommend to NOT USE the `webview` tag and to consider alternatives, like `iframe`, Electron's `BrowserView`, or an architecture that avoids embedded content altogether.**

Given that recommendation (and some noise I saw elsewhere about removing `<webview>` from CEF altogether), (re)building the Qiqqa UI in pure HTML (via Electron *et al*) is NOT DOABLE. Quoting the next bit from that same page:

> ### Enabling
>
> By default the `webview` tag is disabled in Electron >= 5. You need to enable the tag by setting the `webviewTag` webPreferences option when constructing your `BrowserWindow`.
> For more information [see the `BrowserWindow` constructor docs](https://www.electronjs.org/docs/api/browser-window).

And from the Chrome `<webview>` documentation page:

> ### Usage
>
> Use the `webview` tag to embed 'guest' content (such as web pages) in your Chrome App. The guest content is contained within the `webview` container; an embedder page within your Chrome App controls how the guest content is laid out and rendered.
>
> Different from the `iframe`, the `webview` runs in a separate process than your app; it doesn't have the same permissions as your app and all interactions between your app and embedded content will be asynchronous. This keeps your app safe from the embedded content.

Also read [the Electron Security Considerations page](https://www.electronjs.org/docs/tutorial/security#isolation-for-untrusted-content) as that one applies to *anyone* with an embedded (CEF) browser. **That includes us!** Quoting from that page:

> ### Isolation For Untrusted Content
>
> A security issue exists whenever you receive code from an untrusted source (e.g. a remote server) and execute it locally. As an example, consider a remote website being displayed inside a default [`BrowserWindow`](https://www.electronjs.org/docs/api/browser-window). If an attacker somehow manages to change said content (either by attacking the source directly, or by sitting between your app and the actual destination), they will be able to execute native code on the user's machine.
>
> **⚠️ Under no circumstances should you load and execute remote code with `Node.js` integration enabled. Instead, use only local files (packaged together with your application) to execute `Node.js` code. To display remote content, use [the `<webview>` tag](https://www.electronjs.org/docs/api/webview-tag) or [`BrowserView`](https://www.electronjs.org/docs/api/browser-view), make sure to disable the `nodeIntegration` and enable `contextIsolation`.

... and check [their Security Recommendations Checklist!](https://www.electronjs.org/docs/tutorial/security#checklist-security-recommendations) when we finally do ours.
  
**Q:** Should we maybe use an Electron (or Chromely) derivative which has a very basic native UI, which embeds multiple [`BrowserViews`](https://www.electronjs.org/docs/api/browser-view)? Or can Electron / Chromely do that already? 

**A:** Chromely does not seem to have this, out of the box, while a further check uncovered [this PR-16148 for Electron](https://github.com/electron/electron/pull/16148) which has been merged into their codebase since December 2018. Related Electron issues also referenced in that PR: 
- [16181 - Allow more than one BrowserView per BrowserWindow](https://github.com/electron/electron/issues/16181)
- [10323 - Trying to tile 2 BrowserViews into one BrowserWindow, 1st not rendering](https://github.com/electron/electron/issues/10323)
plus some `webview` woes that may hit us too:
- [14905 - webview no longer emits keyboard events ](https://github.com/electron/electron/issues/14905)
- [14258 - Webview: traps keyboard events once focused (comment on why this won't be fixed)](https://github.com/electron/electron/issues/14258#issuecomment-416794070)
- [16064 - Allow more than one BrowserView per BrowserWindow](https://github.com/electron/electron/pull/16064)
- [BeakerBrowser: 1297 - Inconsistent Keyboard Shortcuts](https://github.com/beakerbrowser/beaker/issues/1297#issuecomment-459932323)
- [Ferdi i.e. Open Source Franz: 305 - Considering BrowserViews](https://github.com/getferdi/ferdi/issues/305)
- [Reddit: Google bans Falkon and Konqueror browsers! Probably other niche browsers too.](https://www.reddit.com/r/kde/comments/e7136e/google_bans_falkon_and_konqueror_browsers/)
- [Awesome Electron - Useful resources for creating apps with Electron](https://github.com/sindresorhus/awesome-electron)
- [Package apps with native module dependencies on platforms other than the host platform (cross-compilation) -> advice is to use multi-platform CI build processes to produce electron installers](https://github.com/electron/electron-packager/issues/215)
- [📡 100 tiny steps to build cross-platform desktop application using Electron/Node.js/C++](https://github.com/maciejczyzewski/airtrash)
- [Electron: Using Native Node Modules](https://www.electronjs.org/docs/tutorial/using-native-node-modules)
- [Can you catch a native exception in C# code?](https://stackoverflow.com/questions/150544/can-you-catch-a-native-exception-in-c-sharp-code)
- [Electron: Multi Platform Build](https://www.electron.build/multi-platform-build)
- [Electron with SQLite](http://blog.arrayofbytes.co.uk/?p=379)
- [Open Distro for Elasticsearch](https://opendistro.github.io/for-elasticsearch/)
- [Electron Documentation: Class: BrowserView](https://www.electronjs.org/docs/api/browser-view)
- [Electron Documentation: BrowserWindow](https://www.electronjs.org/docs/api/browser-window)
- [Electron Documentation: Web embeds in Electron](https://www.electronjs.org/docs/tutorial/web-embeds)
- [How to implement browser like tabs in Electron Framework](https://ourcodeworld.com/articles/read/925/how-to-implement-browser-like-tabs-in-electron-framework)
  Note that this article uses `<webview>` which is advised against by Electron. Check the Min and Wex web browsers listed in the 'Awesome Electron' list to see if they use some other means, e.g. BrowserView. [Ferdi](https://github.com/getferdi/ferdi) and [RamBox](https://github.com/ramboxapp/community-edition) are also candidates for inspection as these apps link to a plethora of external services. **Do check out these issues of theirs**:
  + [Unable to login to google account with 2-step verification](https://github.com/ramboxapp/community-edition/issues/2521)
  + [Google says: "You are trying to sign in from a browser or app that doesn't allow us to keep your account secure."](https://github.com/ramboxapp/community-edition/issues/2495)
  + [I have problems when i try to login in Microsoft teams and Microsoft outlook since around 1 week](https://github.com/ramboxapp/community-edition/issues/2375)
  + [Please fix your security posture.](https://github.com/ramboxapp/community-edition/issues/1765)
  + [Memory comsumption and process opened](https://github.com/meetfranz/franz/issues/15)
- [Chromium: Out-of-Process iframes (OOPIFs)](https://www.chromium.org/developers/design-documents/oop-iframes)
- [Chromium: Site Isolation Design Document](https://www.chromium.org/developers/design-documents/site-isolation)
- [Chromium: Rendering and compositing out of process iframes](https://www.chromium.org/developers/design-documents/oop-iframes/oop-iframes-rendering)
- [CEF: `<webview>` support](https://bitbucket.org/chromiumembedded/cef/issues/1748/support)




### Backing up to the main problem: how to get a Qiqqa Sniffer done in a cross-platform UI (not WPF)

The Qiqqa Sniffer is a UI part which is useful in a large part by having both a 'Google'-like public internet facing search engine *and* a PDF + metadata viewer visible and accessible for select/copy/paste and various editing activities: this requires at least *some way of communication* between the PDF viewer, metadata editor, additional controls and the multi-tabbed generic search engine, while security concerns & [Google Scholar quirks](https://github.com/jimmejardine/qiqqa-open-source/blob/master/docs-src/FAQ/Qiqqa%20Sniffer%2C%20BibTeX%20grazing%20and%20Google%20Scholar%20RECAPTCHA%20and%20Access%20Denied%20site%20blocking%20errors.md) will make this *probably hard* when we do it in plain Electron or Chromely.

How about having a very basic *native* UI (which must then be ported to the various platforms 😰) which embeds multiple BrowserViews? (Rough thought right now going like this: take Chromely, augment to have native UI for window with 2 or 3 panels, each carrying its own independent BrowserWindow, sandboxed if need be. That would make the dialogs/windows *containers* _native_, while all content and controls nitty-gritty would then be done in a CEF control, i.e. HTML/CSS/JS)



## Analysis Notes

So, after all this looking around and researching I did, there's at least one solid conclusion:

`<iframe>`s are *out*.

Now we either stick with WPF for the Sniffer -- which would mean at least one important part of our functionality depends on WPF and is thus non-portable, where the question then becomes: how much stuff do we park / keep in WPF and how much stuff do we dump elsewhere, as in: using HTML/CSS/JS technology for the UI bits -- *or* we ditch WPF and then either make `<webview>` work for us *or* use a few OS-native panels in which we run two CEF instances to accomplish what we need *or* we go a UI/UX where the sniffer moves into a native browser via a *browser extension/plugin* which then communicates with the Qiqqa app via websocket or some such, while the native browser does the work we need to provide Sniffer functionality: BibTeX/metadata extraction, PDF download redirection/fetching and (**do not forget this way of user interaction in the current Sniffer!**) copy/paste from browser pane into BibTeX editor plus copy/paste from OCR-ed PDF view into browser search/url box -- which constitutes both *manual* metadata editing and metadata *validation*, the latter being done by *eyeballing the browser page while eyeing the BibTeX panel at the same time*. (Having this done as a Chrome extension might be technically easier but would require moving/resizing different application screens or Alt-TABbing like mad: there's good reasons to have the panes in the Qiqqa Sniffer together as they are!)

CEF webview:

- https://bitbucket.org/chromiumembedded/cef/issues/1748/support
     





