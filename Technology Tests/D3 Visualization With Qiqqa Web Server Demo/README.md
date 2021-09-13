# WPF D3 Visualization and Qiqqa Web Server Demo

## Purpose

To test how best to approach the idea of moving Brainstorm, Expedition, etc. *out* of the qiqqa core C# application and into the JavaScript realm: this makes these visual analysis tools much more end-user tweakable and customizable. If I can get the qiqqa data (document metadata) served on a localhost web server running inside Qiqqa and maybe some NodeJS or at least browser-based JS scripting to construct the **interactive views**, then I believe we have something really valuable. C# is nice, but it requires a serious developer rig while anyone can already do some serious harm in JavaScript with a basic 'programmers' text editor' at their disposal (e.g. Sublime, NotepadPlusPlus, etc.) 

Note also the recent private email where researchers were looking at older D3 force graph work of mine to use in COVID literature research, where I felt the better path forward would be using other graphical methods to analyze literature data. Qiqqa Brainstorm is nice, but at the complexity/density of libraries like mine the default force graph based network view is merely good for some initial "wow" responses, but not really useful for actual visual relationship analysis in large(ish) networks.

This tester/demo should include these features:

- run a local webserver. 
  + MAY be able to bind to 2 listening ports: that way you can have a local net server service multiple local nodes in a corporate setting: just run your own WiFi off network interface #2, bind the webserver to it and all the mobiles and iPad in the room can access your Qiqqa metadata via the generated website.
- see if it suffices to do minimal work server-side, all in C#, so that we do NOT also have to run NodeJS: the brunt of the visualization work can then be done client-side in the browser.
  + where the 'browser' may be the embedded CEFSharp-based browser View.
- provide **interactivity** in the website,  hence we must come up with a way to facilitate user-activity in the (now) web based Brainstorm and Expedition pages. And in any other pages we happen to serve through that embedded web server of ours.
- the webserver SHOULD detect and loudly warn when used in a potentially Internet-facing scenario
  + when not serving localhost 127.0.0.1, show a biohazard screen at application start to make the user very aware of the security risks when hackers gain entry.
  + maybe mark the 'webserver active' state by also coloring the application differently so users will know when the server is 'up'.
- MAY provide button or UI element to click to open an external Web Browser, e.g. Chrome, fireFox, Microsoft Edge?
- use D3 and whatever you need to create the visualizations of the library metadata. Allow the web pages to be edited/customized by the end-user, e.g. by making them available in in `/User/Data/Local/XYZ/` rather than `/Program files/XYZ/`, so that end-users can edit them and even provide their own, if they like. 

  What can be done in JavaScript and HTML should NOT be done in C#.



---

## Motto

This here is part of the technical storyboarding side of a UI & UX overhaul of Qiqqa.

Before we put it to Qiqqa, it will be tested here.
