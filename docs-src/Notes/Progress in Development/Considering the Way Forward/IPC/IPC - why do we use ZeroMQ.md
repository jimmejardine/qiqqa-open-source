# IPC :: why do we use ZeroMQ?

Qiqqa is composed of several software chunks.
While several of them use very generic (web) interfaces, including JSON message data exchange (e.g. QiqqaSearch which employs Apache SOLR, using REST/JSON style communications) there are plenty spots where we control both sides of the communications channel and would benefit of some *added speed and leanliness*.

The QiqqaLauncher also serves as *monitor*, watching the health and activity of the many Qiqqa background processes, e.g. PDF text extraction, OCR, etc.

To separate the work technology from the UI/UX and open up the Qiqqa processes to other (automated and scripted) user processes, our background processes communicate with the UI components using *sockets*. 

This makes the background processes much easier to port across platforms besides, while we can focus on the UI parts separately and possibly using entirely different technological solutions (programming languages, frameworks, etc.) 
As such, Qiqqa can be seen as a collection of (web) servers and client(s) running on a single machine.

The trivial choice there would be to use regular web communications methods everywhere then, but this will result in a significant amount of useless overhead, slowing your machine and the Qiqqa application as a whole down.

While we do offer a REST/JSON interface for the user-scriptable parts of our internal communications, we will be using ZeroMQ ourselves where-ever possible, using efficient coded *binary messages*: ZeroMQ enables us to employ important communication patterns cross-platform at (almost) zero cost, so that bit wouldn't be bothered with you running on Windows, Linux or other capable hardware.

ZeroMQ gives us *fast I/O* for our message exchanges, thus helping us ensure the amount of overhead remains very low while you work and the Qiqqa processes chug along, doing your bidding ASAP.



## Any examples of Qiqqa's use of ZeroMQ being useful?

- QiqqaLauncher also serves as process and task monitor, providing a visual cue to the user about the amount of work scheduled and progress made. This is done using numbers, bars and time-charts similar to Windows' TaskManager, so the user can diagnose the state of affairs.

  The PDF processing tasks (and other bits) must provide thee monitor with progress data. Sending this in JSON form, using regular web requests, would add a repeated cost of (unnecessary) JSON encoding+decoding, *plus* generating and parsing the usual HTTP Request and Response Headers that come with each web request being served: that's *a lot* of data serialization and text parsing activity for sending a bunch of numbers across the process boundaries, just to have them displayed in the Launcher's Monitor panel.
  
  Enter ZeroMQ and binary ('*native*') encoding of those numbers, using full-binary *asynchronous* messaging and you gain these:
  - **Fully Asynchronous vs. Sequential/Blocking** :: web requests (and their responses) are inherently *sequential* and thus *blocking* subsequent requests from the same node until the current request has been served.
  
    ZeroMQ gives us *fully asynchronous* interfaces, where requests cannot block subsequent requests from the same node as responses are received as soon as they become available. Of course this means we'll need to handle our request-response time series quite differently from the trivial web approach, but we get *responsiveness* and *speed* in return.
	
   > While technologies such as ServerPush and such do exist, they still require the use of HTTP (nay! HTTPS!) connections, thus costing a pretty penny in superfluous Request/Response Header overhead!
   > x
   > There's also HTTP/2, which, at face value, offers the same *asynchronous messaging* benefits, but that one is only supported by browsers and (web)servers when you use *HTTPS* which may be *very sensible* when using this technology across a (by definition) *insecure* network, but the `S` in `HTTPS` is *a tremendous and useless added cost* when used for *localhost* inter-process communications like we have between the various Qiqqa components. Thus, HTTP/2 (and HTTP/3) are *even worse* than regular IPC as we would then have added a huge encryption/decryption cost across the board!

- **(Near-)Zero Message Preamble vs. Request/Response Headers** :: web requests generally use the HTTP protocol. This comes with mandatory and optional Request and Response Headers, which can run up a pretty large number of *text characters* per message -- a non-issue for a (large-ish) HTML page, but an order-of-magnitude cost for our *small* monitor data messages and requests, both in generating, parsing and transmitting.

  ZeroMQ does away with all this HTTP overhead, thus providing a very efficient communications base for our internal messaging. It also enables us to easily use *fully binary messages*, thus removing the need for another layer of translation, making for a potentially very efficient messaging framework.
  
  > Of course, there's *websockets*, which could help to accomplish a same/similar message I/O efficiency, but ZeroMQ has been specifically engineered to transmit *messages* across TCP, which is, itself, by design, a *streaming protocol*. *websockets* do not include such a guarantee, which can/will result in arbitrary message delays due to IPC layer behaviour mismatch (see TCP Nagle Option and related 'telnet-like behaviour' information for some generic background info).
  > x
  > As such, *websockets* have been considered and looked like the easier choice when we were considering using `electron` for any future UI work, but ZeroMQ made the final grade as it's a better match to our actual needs and we won't be using a *pure* electron/webbrowser-based UI system after all: it's (run-time) cost was deemed too steep, in particular for *intentionally lean* application components, such as the Qiqqa Launcher/Monitor.
  
- **Full Binary vs. Text-Based Message Protocol** :: HTTP web requests are fundamentally text-based (mandatory CRLF line endings, no length prefix for the Request and Response Header block), while ZeroMQ is all about *full binary* messaging: each message is preceded by its total length and little else, enabling some very fast message preparation, transmission and reception, allowing us to opt for '*native messages*' where applicable, e.g. when the data is short-lived and primarily aimed at internal use, e.g. activity & performance data gathered and sent to the Qiqqa Monitor.

  See also [[IPC - Qiqqa Monitor]] for more about the message format and data included.
  
  


 
 - web requests 
   

