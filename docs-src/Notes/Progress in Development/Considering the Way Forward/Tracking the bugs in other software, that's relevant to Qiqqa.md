# Tracking the bugs in other software, that are relevant to Qiqqa

## Bugs we filed & other bugs *directly relevant to Qiqqa*

- https://github.com/sumatrapdfreader/sumatrapdf/issues/2206

   This means using SumatraPDF right now for annotation editing *may nuke your PDF* --> this is **another important reason to store every PDF *revision* in our database** (instead of the original idea to extract the annotations and rewrite them upon request) -- we must reckon with other software also containing bugs and *wrecking* your library entry somehow (or at least *revision X* of it). Hence we **must** always keep the original PDF intact and **immutable**, whatever we decide to do to it, internally or externally!
- x
- x



## Bugs *potentially relevant to Qiqqa*

Some bugs and issues out there don't touch the code we're currently working on **yet**. 

**But** these bugs are serious issues for us to monitor as we consider our options and choices in moving Qiqqa forward. For example, as of this writing, we are still very much on the fence re using the system-native browser (Edge on Windows, Firefox, usually, on Linuxes, Safari/Webkit on Mac) via [webview](https://github.com/WebView/webview) et al, or making sure our web-based UI parts render and behave predictably and consistently on all platforms by using embedded CEF -- both approaches being complicated by the fact that we need both a *severely protected* web context and a fairly permissive one in the same application to allow us rebuilding Qiqqa Sniffer as the alternative there would be having only the *protected, world-wide web facing* browser context and doing thee rest of the UI in native controls, thus necessitating a rebuilt of the PDF viewer, etc.

- https://github.com/obsproject/obs-browser/issues/219 - CEF and GTK getting in an event loop fight. Seems fixed with latest CEF, though.
 
- [ZeroMQ](https://zeromq.org/) networking (which we've picked for our internal backbone connectivity):
	- ZeroMQ over [websocket](https://datatracker.ietf.org/doc/html/rfc6455), so our browser/WebView-app(s) can communicate with the other Qiqqa components: [ZeroMQ::ZWS 2.0](https://rfc.zeromq.org/spec/45/)
	- https://github.com/zeromq/czmq/issues/2158 -- websocket support in [CZMQ](https://github.com/zeromq/czmq)
	- https://github.com/zeromq/libzmq/issues/3581 -- websocket support in [`libzmq`](https://github.com/zeromq/libzmq) itself :yay:
	- https://github.com/zeromq/zeromq.js/issues/264 -- how to use ZeroMQ over websocket in the browser/webview
	- https://github.com/zeromq/jszmq -- alternative library claiming [ZWS 2.0](https://rfc.zeromq.org/spec/45/) protocol support, i.e. ZeroMQ over websocket. This one's specifically for web browsers/webview, while [`zeromq.js`](https://github.com/zeromq/zeromq.js) is specifically aiming at NodeJS, which is a much lower priority for us :: we'll worry about NodeJS being able to connect only when we're considering end-user scripts interacting with Qiqqa, as we won't be using NodeJS ourselves (at least no internal use of it is planned ATM). 
	 
       **Important Note**: `zeromq.js` requires adding a native code library to NodeJS (i.e. compilation of C code via node-gyp) which, in our experience, isn't always working out on all (development) machines. It is therefore **strongly advised** to go with the ZWS/`jszmq` approach instead as that would *not* depend on these native build add-ons, but use plain old standard websockets instead, allowing for any conformant JS engine to talk to us.
	- in the issues above, there's mention of a 'proper close' of the websocket, i.e. https://datatracker.ietf.org/doc/html/rfc6455#section-7 :hm: I thought that would be covered by ZWS, but after reading that one, this is indeed an open question: there's no mention what the websocket-server-side or ~-client-side should do when its use as a ZeroMQ transport is completed, i.e. all ZeroMQ connections have been closed.
	- [`zwssock`](https://github.com/zeromq/zwssock) is *DIW* (Dead In the Water): no development in the last 4 years. So `jszmq` is the prime candidate, in combination with `libzmq`/CZMS/`cppzmq` and friends on the C/C++ side. This one, however, mentioned *message data compression* at the websocket level, which might be interesting...

      ZWSSock implements [ZWS (ZeroMQ WebSocket)](http://rfc.zeromq.org/spec:39) for use in ZeroMQ applications. Additionally it supports [Compression Extensions for WebSocket](https://tools.ietf.org/html/draft-ietf-hybi-permessage-compression-28) for per message deflate.

