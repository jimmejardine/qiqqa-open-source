# WPF-based CEFSharp embedded browser Usability & Viability Tester / Demo

## Purpose

To discover what needs to be done to make CEFSharp our new all-purpose embedded browser control.

Key features which must be supported:

- hook/observe any content that's coming in from a website. This includes:
  + clicked PDFs, whether they are going to be viewed in-browser or downloaded
  + content which is of any MIME type: plain text may be valid BibTeX or other metadata formats which we might want to grab & import.
  + any page HTML for we must be able to 'read the page' and find any links in there, possibly even a bit of content scraping as well.
  + when part of the page is cached, e.g. when we re-visit a page, we SHOULD be able to hook and grab the cached content as well as there *might* be a retry attempt from the user to grab a PDF or metadata file (BibTeX as plaintext, etc.) -- I've done this type of action myself in the past quite a few times as there were intermittant network errors or whatever kind of issue that seemed to crap up the PDF grab on the first round, hence you go 'Back' and *retry*. The old XULrunner sometimes would not cooperate due to caching and thus no actual HTTP request to the server we could grab on the re-run. :-(
  + must support use of multiple views, using a single instance, or at least in a setting where you "log in" in a site in one view, and the other views get the login update via website cookies, just like would happen when you had multiple tabs open in a browser and hit *F5-refresh* on the other tabs after you've logged in in one tab only: all the tabs should 'see' this.
  + allow keyboard shortcuts, such as F5-refresh, to arrive at the embedded browser.
  + MAY have the ability to 'inject' (user/Qiqqa-defined) JavaScript in a page // or we might want to support Chrome extensions such as WitchCraft or User JavaScript And CSS.
  + have support for browser-displayed page interrogation (e.g. getting a dump of the displayed DOM)
  + or even better: have full Chrome Debugger support available and usable.



---

## Motto

This here is part of the technical storyboarding side of a UI & UX overhaul of Qiqqa.

Before we put it to Qiqqa, it will be tested here.
