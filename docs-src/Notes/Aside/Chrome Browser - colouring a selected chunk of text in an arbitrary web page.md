# \[useful feature:] Chrome Browser :: colouring a selected chunk of text in an arbitrary web page

*(Handy when you want to have searched phrases in a larger text highlighted.)*

I noticed this behaviour for a while in Chrome when I followed links from explanatory paragraphs in Google Search: it's a specially crafted `#` hash part of the URL, e.g.

`https://en.wikipedia.org/wiki/Attention_(machine_learning)#:~:text=which%20are%20computed`

Note the `#:~:text=` in that URL: that suffices to have Chrome show the web page with any matched text highlighted in purple for your convenience, e.g. try & click this link: https://en.wikipedia.org/wiki/Attention_(machine_learning)#:~:text=which%20are%20computed

Once I had realized it's a *browser*-specific thing, here's more info on this browser feature:

- https://support.google.com/chrome/answer/10256233?hl=en&co=GENIE.Platform%3DDesktop
- https://stackoverflow.com/questions/62161819/what-exactly-is-the-text-location-hash-in-an-url
- https://wicg.github.io/ScrollToTextFragment/ + https://chromestatus.com/feature/4733392803332096
- **[CanIUse](https://caniuse.com/) feature check**: https://caniuse.com/url-scroll-to-text-fragment --> everybody's got it, in 2024AD, which is a good thing for us, as I can use this in Qiqqa Ⅱ 🥳



------

Quoting https://stackoverflow.com/posts/62162093/timeline:

## Scroll To Text Fragment

This is a feature called **[Scroll To Text Fragment](https://chromestatus.com/feature/4733392803332096)**. It is [enabled by default since Chrome 80](https://www.chromestatus.com/features/4733392803332096), but apparently not yet implemented in other browsers.

There are quite nice examples in the ["W3C Community Group Draft Report"](https://wicg.github.io/ScrollToTextFragment/). More good examples can be found on [Wikipedia](https://en.wikipedia.org/wiki/Fragment_identifier#Examples).

### Highlighting the first appearance of a certain text

Just append `#:~:text=<text>` to the URL. The text search is not case-sensitive.

**Example:** [https://example.com#:~:text=domain](https://example.com/#:%7E:text=domain) [![The word "domain" is highlighted on example.com](https://i.sstatic.net/mHPz1.png)](https://i.sstatic.net/mHPz1.png)

### Highlighting a whole section of text

You can use `#:~:text=<first word>,<last word>` to highlight a whole section of text.

**Example:** [https://stackoverflow.com/questions/62161819/what-exactly-is-the-text-location-hash-in-an-url/62162093#:~:text=Apparently,Wikipedia](https://stackoverflow.com/questions/62161819/what-exactly-is-the-text-location-hash-in-an-url/62162093#:%7E:text=Apparently,Wikipedia) [![part of this very answer is highlighted](https://i.sstatic.net/fIqVh.jpg)](https://i.sstatic.net/fIqVh.jpg)

### More advanced techniques

- Prefixing and suffixing like the [example suggested in the repository for the suggestion](https://github.com/WICG/ScrollToTextFragment/#identifying-a-text-snippet) [https://en.wikipedia.org/wiki/Cat#:~:text=Claws-,Like%20almost,the%20Felidae%2C,-cats](https://en.wikipedia.org/wiki/Cat#:%7E:text=Claws-,Like%20almost,the%20Felidae%2C,-cats) texts as proposed don't seem to work for me (yet? I use Chrome 83).
- You can [style the look of the highlighted text](https://github.com/WICG/ScrollToTextFragment/#target) with the CSS `:target` and you can [opt your website out](https://github.com/WICG/ScrollToTextFragment/#opting-out) so this feature does not work with it anymore.




--------------

Allegedly already available since Chrome 80/83, which is quite a while back. Ah well, browser feature trend watching isn't my forte.  😅



