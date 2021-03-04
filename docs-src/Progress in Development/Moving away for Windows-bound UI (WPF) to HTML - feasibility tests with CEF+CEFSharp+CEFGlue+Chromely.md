# Moving away for Windows-bound UI (WPF) to HTML :: feasibility tests with CEF / CEFSharp / CEFGlue / Chromely

As stated elsewhere in these documents, several major components must be migrated to allow Qiqqa to become cross-platform portable software (or at least approach that goal, so that the maintenance effort is manageable for such a multi-platform app).

One of the troublesome areas is the UI framework used: WPF. Microsoft's WPF is not available outside Windows (though a derivative/approximation of it allegedly is: Avalonia).

As noted elsewhere, various tech solutions have been considered and the one with the best prospects of longevity is migrating to HTML/CSS, i.e. ultimately discarding the WPF code entirely.

While this surely will smell like "Second System Syndrome" to some (it does to *me*), here are the considerations:

- do I want to ditch the C# code completely? No, not necessarily. Quite a bit of it can and will be done elsewhere, but I like C# at least for the "business layer", i.e. the glue logic. 
- Heavy processing should be done in C/C++ (and that stuff is moving into the mupdf-derivative/clone).
- most of the UI X-plat stuff out there is either fresh, new, young and not surviving for long. Or rather: when you pick one, how do you know you're betting on the horse that will still be running great in 5-10 years time? Microsoft is cycling through ideas almost as quickly as Google (labs), when it comes to UI frameworks. Besides, most of the new stuff is way too focused on "mobile", i.e. sample POS style applications, not the more complex desktop applications.
- then there's the *Fun Factor*: what do I like? (As right now I'm still the main dev for all this, so personal motivation is an issue. When others join in, these preferences must be mixed with theirs.
  - UWP? not for complex desktop applications. 
  - WPF? doesn't make me happy *at all*. Complex and not future proof. No fun.
  - WinForms? another Windows-only one. Does the job, but *fun*? Don't know, haven't done complex UI stuff in WinForms yet. Do I want to learn that? No, better concentrate on what I think is ubiquitous: HTML/CSS/JS.
  - ASP.NET to drive a HTML/CSS UI then? (Electron.NET) not my cup of tea.
  - Xamarin / Blazor / ...: mobile-focused: not geared towards desktop-level complexity and the longevity concern: they come and they go.
  - [MAUI](https://devblogs.microsoft.com/dotnet/introducing-net-multi-platform-app-ui/)? WPF regurgitated. Might have the longevity, finally, but no fun.
  - electron: not a UI per se, but a target when doing X-plat work. Drawback for Qiqqa: comes with NodeJS backend, which we do not need unless I ditch the C# codebase utterly.
  - electron.NET: that's electron + ASP.NET: no fun as I would be moving from WPF to ASP.NET. Motivation.
  - Chromely: viable as it's CEF plus C# backend. We'll have a look at this one (see below)

## Having a look at Chromely :: feasibility for Qiqqa (and a few words about electron et al)

As mentioned elsewhere, I still don't see Chromely as having enough "installed base" / "momentum" to jump the longevity barrier, but it's close.

So the question then becomes: when the shit hits the fan, how well will I be able to work with it and *debug* issues in that core building block?

This is \[one of the reasons] why I always want source code level access to my libraries.

### Chromely and the others: electron, NW.js, ...

While I was considering Chromely, I also looked at electron et al. Only later did I reconsider NW.js as possibly more viable, when I found out that electron basically is a webbrowser with a webserver (nodejs) jammed together in a single package, when I started looking at the answers they provide for this question:

**How doees the "backend layer" communicate with the "frontend layer" (i.e. CEF i.e. Chromium browser core)?**

- electron does this via PostMessage and a few other means, all of which are socket-based IPC on the local machine. Nice, but not the fastest possible means. As some describe it: electron is nodeJS kicking up a browser and talking to it (as a webserver).

  I don't need or want to that using JavaScript (TypeScript) for the backend layer i.e. "business logic": I'm fine with using C# for that and if I don't want to do a 100% rewrite of Qiqqa at once, then that extra NodeJS layer will only be sitting there, hogging space and time between my own C# "server code" and the frontend I'm trying to reach: CEF/Chromium.
  
- NW.js popped up as a possibly better candidate then as it's the other way around: Chromium (frontend) calling a backend JavaScript layer, which is more tighly integrated: not a "web server" per se.

  However, it shares the same "total Qiqqa rewrite" concern with electron. (Of course, moving to anything but WPF and given the quite interwoven/entangled state of the current code, one might wonder how close to a "total rewrite" *any sort of UI migration* would be. The frank answer? Pretty close.
  
- Chromely was, after initial rejection due to "lack of momentum", reconsidered as electron turned out to be too "hoggy" to my taste and I'd rather keep the C# business layer (or whatever will be left of it when the dust has settled).

  Chromely is electron, but now with Chromium as frontend and a .NET layer as backend. The communications, again, are mostly socket-based IPC again, so no difference there in terms of potential performance.
  
  So far, it's the best candidate for migrating the Qiqqa codebase as I can do the UI in HTML/CSS and keep the business logic in C# as much as I want.
  
### Development / Diagnostics Prerequisites

I want to be sure I can easily debug both the frontend and backend layers, **plus** the IPC (Inter Process Communications) between those. When the shit hits the fan, I don't want to be restricted by `printf()` style debugging, if that's possible.

Also I **do not** want to be restricted to **releases only** as that nukes my ability to move forward on glitches, as it makes me wholly dependent on the "Vendor" once again: no new release means no update, no matter the state and availability of the source code.

### The feasibility tests: outcome

So I took Chromely for a spin and wanted it to build from source in one go, as much as possible.

That ook about 2 days (1.5 to be precise) until I can to the conclusion that **somewhere** in there is something nasty that I don't see and is making this process **extremely brittle**. (This is **non-conventional** use: I do not use the packages, but the source code straight away, bundling it in ways so that I can debug into and through all the layers. The only compromise I was willing to accept was having CEF itself in binary DLL form only. The rest must be buildable and debuggable from source as one compound build.

Suffice to say that didn't fly. I got *close*, but there was always *something* breaking the result when I started fiddling with it, which makes this stuff pretty brittle from my perspective. (And **I know** I'll have to fiddle with CEF quite a bit for **one of the most important Qiqqa components to be ported to this is the Sniffer**, which requires pretty intimate access to the DOM, caching and network traffic boundaries of CEF/Chromium. (The old xulrunner didn't allow access to its cache, which caused certain *unfixable* bugs/issues when reloading pages to retry downloads over loweer quality connections. Seen that, struggled with it a lot, not wishing to revisit and not be able to do something about it then.

## Considering slightly less desirable alternatives?

Given this result, the question had to be adjusted a little so we might consider X-plat alternatives, which would have some, but few, lower severity, drawbacks.

How about using CEFSharp and CEFGlue straight away, out of the box? After all, there was a time when I considered riding on top of C/C++ CEF raw and having a basic C#-based webserver as a separate process anyway!

CEFSharp worked out of the box (while very nice and *cute* for development, the auto-downloading and installing of the proper CEF DLLs, etc. is not all that great from my point of view in production: what if users install Qiqqa whilee off-net? Everything should be in the installer, so you have a single download and then you're *done* until you *want* to go onto the network (Sniffer activity, f.e.)

CEFSharp is also Windows-only. The consideration here was: "what if I go and use that one, port the majority of UI into HTML/CSS, and then, when others want to, they can do an easier port from CEFSharp to CEFGlue or Chromely (if it's viable by then) to get Qiqqa on Linux or Mac."

CEFGlue was, in my memory/recall, quite inactive and down on its luck. That was 2018/2019. Turns out CEFGlue has picked up again and is tracking latest CEF/Chromium, just like CEFSharp.

The benefit of using CEFGlue is two-fold:

- CEFGluee is also available on Linux (and from what I see in the commit tree/forks, work on Mac is done there too, in combination with Chromely)
- CEFGlue *promises* as tighter and more complete integration with CEF, meaning I may expect better luck re those additional interface planes I need for Qiqqa sniffer: DOM access, HTML page I/O including cache access and page-driven network traffic.

CEFGlue is available for Windows. The only thing that bothers me is Gitlab, which has a horrible interface for when you want to look into work done by others besides mainstream (which is something I often too: having a peek at others' work to see if there's anything useful in there, so I can go a invent other wheels instead of *those*.
Not a big problem, just a nuisance, that one. Anyway, there are forks on github, which I can hook up to and then `git remote` and some judicious scripting can do the rest.

### CEFSharp : feasilibity tests part 1

CEFSharp builds out of the box, quite nicely.

Also hooking it up to the CEF library **source part of the distro** (lidcef_dll_wrapper source code) was relatively easy, though note:

- download CEF tar
- unpack
- `cmake .` to create the MSVC projects
- build those to test the cefdemo to make sure the CEEF distro itself is kosher
- add the libcef_dll_Wrapper vcxproj to the CEFSharp solution and ditch the CEF.SDK package dependency
- **important tweak**: make sure the libcef_dll_wrapper "runtime lib" config bit is set to use "Multithreaded DLL" instead of the default "Multithreaded Static"!
  - That's something we might need to tweak in the CMakefiles, perhaps, when this gets serious. 
- add libcef_dll_wrapper as a dependency to the two C++/CLI.NET projects in CEFSharp and rebuild.
  - don't forget to point the include directories to the unpacked CEF distro, but that's pretty trivial, both in the errors you geet when you don't and how to fix it.

As such, CEFSharp (and the demo applications included) pass the initial feasibility test: I can debug through the stack until I hit libcef itself. That's have to do. (Building libcef itself from source is more work, so I'll do that only when I feel t he need for it. Currently CEF has shown to be stable and robust enough for multiple releases, so *knowing that I can* is currently enough.

### CEFGlue : feasibility tests part 1

Ditto as for CEFSharp, but CEFGlue lacks that auto-download feature, so you have to make sure the CEF DLLs etc. are reachable from the start.

While this may sound a little obnoxious to do, when you do tests like I do, this actually is *easier* as I had less trouble with CEFGlue giving me unclear behaviour during the initial builds than CEFSharp. CEFSharp "recovered" itself from my mistakes, without mentioning it, by downloading the CEF tarball time and again, giving me a less "what does this do" predictable behaviour overall.

In short: CEFGlue is a little more trouble right at the start, but when you are using it and getting somewhere, it is far more predictable thanks to the **absense** of that CEF-download feature!

Okay, what do you have to do?

- **important**: **DO NOT** use the `Debug` versions of the CEF dlls. Don't know why exactly, but all (Chromely, CEFSharp and CEFGlue) crashed silently somewhere down the line when I was feeding them the Debug DLLs, even while the projects themselves were compiled in Debug mode. Surely something must be amiss with the P/Invoke or whatnot interface layer there? Yes, CEFSharp does C++/CLI instead of P/Invoke, but that only means I probably have some setting wrong, same as for the P/Invokes...
- Grab the x64 Release DLL files and the rest of that CEF directory and copy it into your bin target dir.
- Ditto for the .pak and .dat files from the Resources directory: just add those to the bin target dir.

Now you have libcef.dll and all it needs in the bin directory of your build and you can run and debug everything. Again, of course, until you hit the CEF boundary itself, but that's fine for now.

The fact that CEFGlue offers *more* and is available cross-platform moves me away from CEFSharp and makes me consider using CEFGlue as a poor-mans Chromely, the latter still being the more interesting one, but that brittleness I experienced while digging through and messing around has lowered my confidence level about whether I'll be able to dig through, find and then *fix* issues found in testing and *production*.

My current way of thinking about the overarching challenge is this: get CEFGlue going with some rudimentary C# Qiqqa business logic; do the absolute minimum of "native Windows" UI work to ensure we have a safe and workable Qiqqa Sniffer (which would require at leeast *two* WebView instances: that's why I was looking so much at electron at the start, as there you have the documented means to create multiple browser instances in a single *user view*. Alas, electron is too much overhead elsewhere, so I'll have to find other ways to get a safe browser pane in there.

When I get such an app going, it's still Windows-only, but only very slightly so (the minimal native outer carrier for the 1+2=3 browser instances, which can together produce the entire Qiqqa UI).
Once that's getting somewhere, someone else (or me) can pick this up and replacate only the outer container for the next platform, resulting in a (hopefully) minimal platform-specific code chunk to maintain per platform.

If I would use CEFSharp, such cross-platform porting would include porting to CEFGlue for Linux + Mac, adding to the porting cost and thus maintenance cost for the platform-specific codebase (now larger), which is why I prefer to go with CEFGlue for windows as well: than it's purely the basic containers which need to be platform-specific UI code.



## Conclusion

I hope Chromely will behave better when I take some time off later and retry it my way, but that's for in 6 months or so. Let's monitor its progress meanwhile.

CEFSharp is nice, but Windows only. 

CEFGlue works on both Windows and Linux (and Mac?). This makes the *cross-platform* cost low & minimal as only the outer container UI (the bit of app code which makes sure the CEF views are displayed and receive user input (mouse, keyboard, touch) would need to be rewritten for each platform.

All UI migration options are considered very costly as it's a migration from WPF to another system (and the devil is always in the \[UI] details), however, anything that's purely HTML/CSS (plus JS support) based is deemed far more *fun* and therefor *motivating* than moving to other tek such as ASP.NET, Blazor, MAUI/Xamarin, what-not.



## References

- https://stackoverflow.com/questions/12224798/any-reason-to-prefer-cefsharp-over-cefglue-or-vice-versa
- https://devblogs.microsoft.com/dotnet/introducing-net-multi-platform-app-ui/
- https://trends.google.com/trends/explore?q=nwjs,Electron%20js,Chromely,nw.js,MAUI%20.NET
- https://trends.google.com/trends/explore?q=CEFSharp,Electron%20js,Chromely,CEFGlue,MAUI%20.NET : CEFGlue has far fewer 'hits' than CEFSharp. How are they doing relatively?
  - https://trends.google.com/trends/explore?q=CEF%20Chromium,Chromely,CEFGlue,MAUI%20.NET -> same ballpark. Of course you can edit any of those search terms a little and get completely different results :-)
- https://stackoverflow.com/questions/23509356/node-webkit-vs-electron#:~:text=In%20NW.,js%20file%20in%20the%20package.&text=While%20in%20Electron%2C%20the%20entry,in%20it%20with%20corresponding%20API.
- http://my2iu.blogspot.com/2017/06/nwjs-vs-electron.html
- https://github.com/nwjs/nw.js

 
