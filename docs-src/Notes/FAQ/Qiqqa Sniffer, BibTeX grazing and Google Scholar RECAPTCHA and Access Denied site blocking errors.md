
# FAQ : Qiqqa Sniffer, BibTeX grazing, Google Scholar RECAPTCHA and Access Denied site blocking errors

## Is Qiqqa Sniffer broken since Qiqqa went Open Source or when?

The correct answer here is that **all versions of Qiqqa suffer the same issue** as this is not a Qiqqa issue but something **caused by Google and Google Scholar in particular: Google does not want any *robots* (computer applications *with* or without human supervision) to connect to their Scholar data**. 

Read on if you want to know more.


## Why do I get a RECAPTCHA and/or Access Denied blocking response from Google Scholar?

The true reasons probably are manyfold and complex but some may be gleaned at [[Links to Stuff To Look At#Google Scholar]] 
which are my quick notes while looking into what everybody else has been doing and how far they've got in *solving* this issue.

**TL;DR / Bottom Line**: Google does not want you to access Google Scholar:

- via anything but a regular, well-known, browser such as Chrome, Safari, FireFox or Microsoft Edge. 

  Anything that ever so slightly *smells like it **might** not be a regular mainstream browser* for *direct use* by a *human being* is automatically penalized by the Google Scholar site. You'll get RECAPTCHA's *sooner* and *more often* and when you happen to search a bit too long or fast to the discerning tastes of the Google management, you'll be **blocked** for quite a while -- the timeout limit is currently unknown to me and certainly subject to change as that's one of the undocumented knobs Google can twiddle if it gets upset about your behaviour.
  
  Qiqqa has an **embedded browser**, which is, given some effort -- and Google expends of *lot of effort* on this -- differentiable from a mainline browser like Chrome. 
  
  > For example, Google can employ [fingerprinting technology](https://pixelprivacy.com/resources/browser-fingerprinting/) like
  > [the panopticlick tool](https://panopticlick.eff.org/) demonstrated at [the EFF site](https://www.eff.org/deeplinks/2017/11/panopticlick-30) 
  > in such a way as to recognize 'non-standard browsers'. I don't **know** if they do it this way, but given my observations of Google Scholar behaviour over the years, 
  > its change in behaviour over time and the question "how would *you* do it, if you were *them*?" I'm betting on them having *all* non-invasive fingerprinting tech 
  > included in their code: they don't need consent for that and it's what a browser transmits anyway -- unless it is filtered or altered 
  > and those two modes happen to be of particular interest to a business like Google which has *ads* as a major revenue stream: 
  > if you altered or filtered your browser fingerprint, you're **very probably** also filtering ads. 1+1=no-Scholar-for-you. 
  >
  > Since this whole endeavour is bathed in *probability*, Google cannot be absolutely 100% sure about their conclusions about you being an alleged robot/automaton/undesirable-user and 
  > the easiest answer to the conundrum what to do about *that* uncertainty is to severely limit the number of Scholar requests for any such entity. If it's a 'live one' (clicking human with a 'good' browser) then too bad, but they get a sip and that's good enough. If they want more, they'll quickly find it works out for them to move to Chrome. Yay! Meanwhile we (Google) make sure the bots don't get our stuff in a 1000 years. Yay! × 2! 
  > 
  > Google can simply log your IP address and count requests,
  > hitting you with a RECAPTCHA after, say, 10 searches (*10* being one of the unknown/undocumented parameters that can be easily adjusted by Google
  > and *seem* to have been adjusted over the last couple of years). Google can also hit you with a site error -- anything in the 400 or 500 range will do 
  > and is darn cheap to execute for Google as it doesn't cost any extra resources. Just fail and we're done. Then Google allows your IP to "cool down" (timeout period)
  > during which any Scholar request will immediately error out *again*. (I expect them to register these attempts of yours and reset the timeout on every occurrence.)
  > That's what *I* would do anyway: *frequent hitter is adamant after we decide he's had enough, then we keep blocking the bugger until he really quits*.



## Is this a Qiqqa problem or a Google Scholar issue or maybe *both*?

Do note that **this is not a Qiqqa-specific issue**, though Qiqqa happens to be currently worse at it than a few others, who are up-to-date in their war-of-tugs with Scholar's anti-automaton/anti-embedding detection and blockage logic. 

Qiqqa is hit badly by this because Qiqqa currently uses an old Mozilla browser internally (XULrunner), which' technology is about on par with FireFox version 33.

When you search the Internet, you'll find lots of others, who don't use Qiqqa, having the very same problem. Qiqqa **has it worse** thanks to the old Mozilla browser it is using internally, but even upgrading to a bleeding edge Chrome/Chromium-based embedded browser won't solve the problem entirely as it's fundamentally an arms' race between Scholar users automating/simplifying their Scholar use and Google, who offers the service but does not want, and has strong incentives to fight, this kind of automation / assistance as it's directly cutting into Google and others' (e.g. Elsevier) revenue streams.


## Many places around the Internet advise to use a (paid) VPN as a work-around. Would this work and why?

**TL;DR: umm, don't think so, but you might get lucky. For a while. Google is actively fighting this and gets smart quickly. *Bloody engineers.*** 😣😈 

The VPN should be an *anonymizing VPN*, i.e. a VPN which picks up your network access and outputs it from one of many nodes spread throughout the world. NordVPN is a good example of such a VPN but there are many VPN service providers like that.

The trick here is that the IP-based request counting (also often described as *IP tracking*, though that would be a slightly different tech with almost the same effect) by Google Scholar is now spread over N '*exit nodes*' of your VPN service provider as your subsequent Scholar requests seem to originate from another random node of theirs. 

### So that would be a good solution, eh?

Well... There's a few things that make it a temporarily *lucky fix* at best, in my opinion and experience. Consider not just yourself but *many* folks wanting to access Google Scholar hassle-free and *many* of them taking the VPN route: now *their* Scholar queries are mixed in with *yours* and all those requests now happen to pop out at the VPN exit nodes, which, you guessed it, now start to look like very active, *probably abusive* Google Scholar accessing machines and it then takes the proverbial microsecond for Google to decide to *block* the buggers or at the very least spit out RECAPTCHAS. Given that *the same exit node* from the VPN serves many users like you, chances are that Google has to transmit multiple RECAPTCHAS to that node before the first RECAPTCHA is answered by a user: 
it would take just a little piece of server-side code and a minimal amount of persistence (of which Google has plenty as there's plenty disks around the place :wink: ) and I (being Google) can easily observe a single machine **apparently** serving multiple people as only the *last* RECAPTCHA should be answered if it was a single user and a single browser with a single tab open, thus *not* abusing my Scholar service. Hence I can quickly decide to be done with it utterly and, correct RECAPTCHA answer or not, block the crafty buggers.
*I* would probably also make the little extra effort to register said IP as a 'VPN exit node' for at least a *week* before expiring that bit of data and blocking *all* Scholar access from that node for the duration, forget about the other checks that we do.

Again, I am *second guessing* what Google does in their very **closed source** of the Scholar site, but observations to date have at least strongly suggested (temporary) IP blacklisting to me, probably done the way I describe above. You don't need rocket scientists to implement that stuff anyway.

So... that would make a VPN a risky proposition as you exchange getting blocked/RECAPTCHAd for your own activities with getting blocked/RECAPTCHAd for *other folks*' Scholar activities. Plus the blacklisting is easy to, ah, *escalate*: take a blocked offender IP off the list, but *keep it around in a 'previous offences' list*: when an IP, at any time, is *going to be* blocked, we (Google) can quickly check the list of 'previous offenders' and when the IP happens to be one, hit it with a doubled time penalty, for example. If the node happens to be the kind of incurable 'turn style criminal', that doubling quickly adds up to a *year* of blockage and the problem will go away by itself.

### [Tor](https://www.torproject.org/) to the rescue?

**Tip**: my research dug up [`scholarly`](https://pypi.org/project/scholarly/) as being a very smart little piece of software, which can use the [Tor network](https://www.torproject.org/) instead of regular VPN services. Now don't get excited, as the description for VPNs above goes for [Tor](https://www.torproject.org/) and any other distributed anonymizing network alike: your traffic is anonymized by outputting it at a random *exit node*. The number of *exit nodes* is however limited and is quite a lower number than the total number of Internet-accessing humans, so their traffic gets mixed in with yours, indicating the same problem for Tor exit nodes as for VPN exit nodes: I (being Google) can more easily detect the nodes as serving multiple people at once, and randomly!, thus having a quicker path towards blocking (cheap) vs. RECAPTCHA slow-down (more expensive for me).

The [Tor](https://www.torproject.org/) way might have slightly *more* luck as Google is also savvy to some social issues and it is politically opportune to keep the tor gates open for designated oppressed people. As Tor is meant to give these folks some potentially safe access to the Net at large, Google **may** be less inclined to block Tor exit nodes, compared to other VPN exit nodes, but again, I'm second-guessing a large corporate entity here, so YMMV.

### Nice, but you've forgotten about corporate proxies serving many users at work: they are not blocked!

Of course, things are not so clear-cut as described here as there's also corporate routers/proxies to consider, but those are recognizable as well if we include mandatory **cookies** in our Scholar traffic (and Google does, gosh, what a surprise!): one user session will always exit from the same IP node for a corporate firewall/proxy, but MAY jump around to different IP numbers when it's a anonymizing VPN's exit node collective we're dealing with: this will happen when we close the connection after a request+response and it will be quite expensive for the VPN service provider to track and link later user requests to Scholar *in order to connect them to one exit node consistently*, while we are tracking that Scholar 'session' already because *we* (Google) intend it to last not too long or be (ab)used too frequently: you may sip from our teat but *feeding* off it is objectionable (by us = Google) and hence denied.

So, yes, all the wild stuff on the Net makes the logic on the Google Scholar server-side quite a bit more finicky, but the basics remains intact: Google can easily install a few knobs to tweak the parameters for RECAPTCHA and Access Denied decisions. As Google has the data and the ability to restrict access unless you conform to their input demands, you only get *more* Scholar access if you're human (in the perception of Google!) with very high probability.

That is why you get less hassle from Google when you log into your google account and access Scholar while being logged in.

## That's all dandy, but I cannot log into my Google account inside the Qiqqa Sniffer!

Yep, that's the 'old embedded browser' kicking in: Google doesn't allow logins in 'outdated' / 'outmoded' or otherwise *unsupported* browsers, and XULrunner happens to be one of those, alas. Hence we have [issue #2](https://github.com/jimmejardine/qiqqa-open-source/issues/2) which is, regrettably, a lot of work to accomplish, so it will take a while before that one is done.

**Note**:  it looks like the User-Agent tweak in Qiqqa v82 allows you to log into your Google account inside the Qiqqa Sniffer, but that only succeeds anyway when the planets are aligned or what-not. It worked kinda okay back in Q4 2019, but, like with all other things Scholar Access, its success rate seems to decline, so YMMV. Now, at the start of Q3 2021, this trick is dead in the water, regrettably.


## But you can tell Google that you are a modern-day Chrome, right? There's the User-Agent!

Yup, we do that already in [Qiqqa v82](https://github.com/GerHobbelt/qiqqa-open-source/releases). It *seemed* to work for a bit last year, but then Google got smart and my access statistics started to go bad again, so it *may help a bit* but Google knows better than to merely check the User-Agent. You can include JavaScript in your webpage to detect the browser in other ways (Language and feature support checks, f.e.) and then you can decide what to do after all.

## Is there no end to this RECAPTCHA problem?

Frankly, *no*. 

Like I wrote above, it's a *race of arms* as you and Google do have quite different goals with Scholar: you want to use it as a free metadata provider to complete your own library, while Google offers it as an appetizer: we have the best metadata, come into our berth! But we need to be paid after all! And we don't want anyone siphoning it off our servers and take off with the loot. And while you are only a petty small-time criminal from that perspective, having your own library instead of depending entirely on *us* (Google), you still are more or less undesirable by us (Google), for Scholar provides you with ways to 'manage your library' over at our (Google's) servers, if only you'ld get an account, so why don't you?

"Well, I use Qiqqa!" 😊

## Jumping Jehoshapath! TL;DR! Give me a list of things to do!

I can give you a list of things to *try*. As you had a TL;DR! moment, the key take-away of all the above should be: it can change any time, any day, depending on the mood Google corporate is in. They decide how tight they turn the screws on your ways into Scholar.

Given that caveat, here's a list of things to try:

- try to log into your google account using the browser in the Sniffer. See if it succeeds and is persistent, i.e. if Google still calls you by your name while you query Scholar from inside that same Sniffer.

  Expected Result: you might get lucky and be able to do more searches before you're hit with RECAPTCHA or Access Denied.
  
- try to 'Add to Library' inside Google Scholar (assuming you could log in) and then extract the BibTeX from there. Convoluted way to access BibTeX, I'll grant you, but sometimes that works when regular BibTeX grabbing off Scholar fails.

- [try the other suggestions listed in the comments for issue #113](https://github.com/jimmejardine/qiqqa-open-source/issues/113)

- try an anonymizing VPN or SOCKS proxy.

Always: YMMV.
