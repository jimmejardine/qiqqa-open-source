# Sharing and publishing Qiqqa libraries in encrypted form

## Why & when you want to do this

- Your library contains *sensitive data* or *derivatives thereof* -- which often are *insecure* from a *privacy* standpoint, private and/or corporate.[^privacy] 
- Your library contains publications which you need to *protect* from public/unauthorized access, e.g. *early or PENDING research that has not been published yet*.
- Your library contains (or *possibly contains*[^holdit]) data / information that's GDPR/AVG[^euro] sensitive or risk subject to similar.
- Your library contains publications which are on a need-to-know or other access-restriction list. Company side letters come to mind here, f.e., next to the obvious stuff (military). Your flavour of the Enron emails: ditto. 
  Anything that's on a *reader access list* like that SHOULD NOT be trusted onto cloud storage willy-nilly.
- You're involved in patentable research. It might behoof you to tread carefully, Sunny Jim.
- Anything else you're fretting over...
- ...
- You want/need to *obfuscate* your library from public robotic analysis, e.g. spam and marketing efforts (e.g. Google reading your gmail to feed you targeted merchandise, that sort of thing) -- this is a *much lower threat level* than the previous items: you don't mind *humans* having access but you *do* mind machines barging through your library or their own (yet ultimate human-driven 😉) purposes. 







[^euro]: GDPR: Euro-centric writer here.😉 Whiteout and replace by your relevant organization when you live or work elsewhere.

<!-- https://pandoc.org/MANUAL.html#footnotes : *long footnotes* -> *wide sidenotes* akin to Gwern -->

[^holdit]: "*possibly contains*": meaning: when you're in doubt err on the side of safety, until a proper vetting process has cleared any confusion and doubt all around.

  In an ideal world, all data wants be free, but *human nature* a.k.a. "*the human condition*" may very well demand another approach in actual practice.
  
  An example: a while ago I spent a year helping to create a therapy book (translation and redesign), which, for both health factor and original content licensing reasons, we wanted to protect from "leaking" into the *pirating scene*. 
  Given the level of clout we imagined we had (*little*, realistically speaking), the technical and legal *savvy* in the team, *the lay of the land* in national academia and a few other minor socio-political factors, the conclusion was to restrict the book to *paper-only publication*: as this concerns a therapy client's guide + workbook, the really useful form is *paper* anyway[^genaccess], but we explicitly decided on a policy of *no eBook; no electronic copies EVER, not even for promotional purposes*: this raises the level of effort for plagiarism and pirating to a higher bar and thus will thwart quite a lot of the low-hanging-fruit pirates out there; xerox-ing, while possibly common, still clearly identifies the copies. Of course, this is *not* a complete solution, but I mention this example so you get a glimpse of what may be desirable and is not obvious in this modern age where everyone and their granny has (therapy) books on their iPad, ready at hand. Sometimes context makes a non-obvious approach the more desirable and least costly one. 
  
  And THAT is what I'm on about in this chapter, after all!

[^genaccess]: while we observe a world where, at least in the Netherlands, national government and elsewhere is moving towards a "*computer-savvy or you're toast*" assumption of *compos mentis*, we find that this is increasingly *excluding/alienating* not just the elderly but also several other large groups of humans, who simply did not receive the abilities and schooling to operate as a full-fledged adult in this *digital world*. Also wo unto ye, when you are or have become *disabled*, for being able to hold a pen and exhibit the unique human trait of *writing* does not suffice any more! Computer *admin* skills are a basic necessity for self-sufficiency.
 While you *may* be a TikTok veteran, I dare you to be a veteran in filling out tax forms, insurance forms or file a claim with the police because someone (again!) stole your bike. The station is closed as the website is open. 
 I'm still waiting for the research publication that investigated and reports on the human costs of such actions. Yes, I've seen the *online* inquiries; talk about research *skew* 🤦-- regrettably, here's where my copious cynicism still shows *lacking* to match reality.
 
 And not letting down the American (USA) perspective either: I hear there's a serious stress level increase when y'all are facing the yearly filing of your tax forms to the IRS, due to tax regulations and forms' *complexity*.  Now, as a Dutch national and part-time chauvinist, I *thought* we (the Dutch) *excelled at least there* but from what I hear and read it sounds like, on this subject at least, America has indeed been Made Great Again, Washingtonian donkeys and elephants united under several presidencies in their efforts re *les peuples*.
 ❓Now was that *fake news* or *propaganda* that's skewing and obscuring my plebeian view of IRS reality, or does this indeed fit *youse* experience? 🙏Let me know as I'm open to critique and education here, for I am, after all, a *scholar*!

[^privacy] : An example: 
  when I can gather *anonymized* movement data, such as mobile phone location data (which is always available as telcos must track which transmission tower(s) you're accessing with your mobile for the mobile phone technology to work *at all*), I can *infer* which phone(s) belong to you & your family by simply gathering the statistics about which phones are at or near your home address most often. A bit of facebook and maybe some other public sources narrow this down from zone/street-level down to the individual pretty quickly as each phone will travel different routes at different times of day: your work and other habits will quickly show through and all I need is a few (one or more) known habits, such as your place of work and possibly your type of work, e.g. office workers generally adhere to 9-to-5 while production folks work in 3 or similar *shift* schedules, while almost never starting at 9AM but rather earlier; logistics people and construction site workers usually start very early (7AM) and end early in the day. Hand me that data, which is easy to come by (I don't need *precise info*; having such *precision* *helps* but is *not mandatory to succeed*) and then we can *pin* that *anonymized* phone on *you* after a while; once *a single habit*  is clear (another one is going for groceries every day or every weekend and at the same supermarkets), the rest of your travels and behaviour will pop up as you carry your *tracker device* (your phone) with you. Easy-peasy. 
  All it takes is a lot of data (🎉 yay computers! 😘) and some patience on my side. This type of round-about approach is known as *traffic analysis*, which is, from my point of view[^1], a kind of *side channel attack*, even when that particular jargon is more often applied to technological cryptography attacks, e.g. *timing attacks*: it's the same basic idea: if you can't get at **what** you want to know, then you can at least try to get at the **where**, **(with) whom** and **when** and *infer* from there. Yes, I won't know what you've said exactly, but subject matter is *inferable* as I analyze not just your communications graph but also for the ones you've *probably communicated with*, not just checking the *where* (and with *whom*) but very importantly also the *when*, particularly the *execution order*, i.e. which actions *followed* after each *event* (i.e. probably meeting moment). 
  All this is quite doable nowadays, not just at state and large marketeer company levels. In this game, your friends' and relatives' habits define not just them, but they also define *you*, after a while.

  The same goes for many other *anonymized data*: many published attacks and analyses by data researchers show that *upholding privacy* is a very difficult task indeed. Thus it seems *prudent* to *block access to potentially relevant data* until a conscientious and *thorough* analysis by several parties (spanning abilities and viewpoints) has vetted the information as *sufficiently safe*: (anonymized) medical records, human traffic patterns, e.g. travel data, etc.

  For *some* considerations, see the **How to proceed** section.


[^1]: I consider *traffic analysis* a part of the *side channel*: you're analyzing the *side channel* which is the number, size, timing and duration of the messages (data/files/sound) that form the actual communication. I o realize most folks view these as different *categories* altogether; as far as I'm concerned we're arguing semantics by now... woodshedding which obscures the important point: I can *infer* your private communications even when I don't have actual *direct* access to them. Thus it follows that basic *encryption* will not suffice in most cases: you will have to consider your [[MOO]]/[SOP](https://en.wikipedia.org/wiki/Standard_operating_procedure) as well. For *some* considerations, see the **How to proceed** section.





## How to proceed

### The definition of "Under Your Control"

First off, let's revisit that all-important definition, as it's *fundamental* to anything *security*:

A workplace and **equipment** is **Under Your Control** when you have full control over where, when and how *anyone* can access your workplace and equipment.

Of course, the military get pretty paranoid about this as *security* is their purpose in life and, yes, *everyone is out to get you*. (Not because you're important, but you are *in the way*, most of the time. That's what a lot of folks who think they've nothing to hide or loose tend to ignore[^forget].)

From that perspective, your own machines are *barely* under your control: did you vet all the software, and the compilers and linkers used, that's installed on your machine? No, you didn't. Chances are you left that to the guys 'n' gals at Microsoft, or *maybe* Mr. Torvalds & cie. Not a clean start, but let's proceed.
Your equipment is one, your workplace is another: is that one *fully under your control*. Only *potentially* when you're the boss or have your own private room, with a known monitoring system and you able to verify nobody will be able to film or hear you, nor look over your shoulder in a friendly way.
That's why some of us prefer key dongles. Yes, we do trust you to be able to type a long, *secure*, password. We also trust others to be *fluent* readers of typists' hand movements, 's all. And blind folks can *infer* a lot from *rhythm* alone; visuals is great but not a requirement to snooping your shit. Writing your PIN on the back of your bank card is only the beginning, hon'.)

You see now, that in the harsh light of *security* we are already bargaining with a pretty bad hand from the start? Let's make a sweeping (false) statement that most of us believe:

You are In Control of your personal machines and work environment. (Oh, so you can guarantee that any entry and access by other individuals becomes known *instantly* to you? No worries, I'll let it rest now...)

Now let's assume that breaking & entering at your workplace / where your machines are kept, is pretty hard to do. Once data travels *off* your machine, over the (local) network, risks gather and once you zip through the gates of your Internet firewall you are *guaranteed* promoted to *chow mein* among the feral, hungry hordes. So let's talk some basic precautions; **always consider** the security implications and purpose of each item I mention: usefulness, applicability and execution differs per use case / scenario.

- Encrypt anything that goes out. Period.
- *How* to encrypt? Classically this is done using a *secret key* which you schlepp around in a *secure fashion*. That idea never worked for anyone long-term, so they came up with *public key cryptography*: See a need, fill a need! And here the need is private *secret* keys should NEVER travel. 
  - Hence every secure location has it's own private key. 
  - Encrypted data gets *exchanged* and SHOULD be *authenticated*, i.e. checked to make sure it originated from the sender we *expect*. Anything else is noise and an *intrusion attempt*.
  - ...
- Don't just encrypt your content! Encrypt your filenames, directory structure, everything *meta* about your data! Ergo: ZIP (*bundle*) that stuff and encrypt the *bundle*. 
  - Top this off: ZIP has one zero-info-content filename for all: use random numbers for the naming scheme.
    Dates inform. so do otherwise-helpful *descriptive names*: remember, this stuff is traveling *abroad* and out there, *informative* is **empathetically not what you want**. So random noise for the name it is. It's already bad enough the *timestamp* of the resulting file gives observers a *timeline* of your activities. *crap!*
* And when you encrypt individual files (or bundles), the filesize is another information point. Which is yet one more reason to *bundle* everything into a ZIP. **And then chop that bugger into pieces, say 10MByte apiece** -- that's for email targets and such-like. Works a treat on public commercial-grade cloud storage too as the files won't be horribly large (great! no rejects at gmail et al!) and will usually be *few*, unless you're shipping GBytes worth' of data.
    * It might serve you to *compress* the data to reduce public filesize. However, when you *update*/*upgrade* your library, the *difference* between releases is telling by itself, which is why security folks love *padding with random data* to a segment boundary. While this seems counterproductive when you first spent so much effort *compressing* your actual data, using a large(ish) segment size, say, the same 10MByte we mention earlier, reduces the delta-inferable info *over time* to practically *nil*. Sure, smaller segment sizes can be considered, regular crypto pads by one or more *code blocks*, usually 256 bytes or there-abouts, but I don't see any harm in keeping with modern persistent storage hardware (HDDs, SSDs, etc. and pick, say, a 4KBye *segment size*. That's practically *nothing* in today's world and you've got yourself a beautiful padding scheme basic there.)
      Of course, keeping padding segment size equal to your ZIP-splitting segment size of 10MB means nobody can *obviously* find the last segment in the published set, assuming you don't leave other (meta)data informing nasty observers which part goes where. When done *randomly*, all segments are equal and only the recipient, successful at decrypting each, will find the info about which part goes where in the ZIP/bundle-*depack order*. Just a thought. 😜
* Give each (raw, unencrypted) segment a *header* which can be quickly decrypted, separate from the data chunk itself: that comes in handy at the recipient when you publish highly randomized stuff and we want to be *quick* about it, finding out which chunks are for real and which are *adversarial*. No need to decrypt *everything* when there's a way to decrypt just a bit to see if we *want* that bit. 
  Comes with a fat *proviso*: many a scheme where small messages were encrypted, or where a *series of messages* was encrypted using the same key usually *broke* under cryptanalysis after a while. (In this scenario our quick header is message 1, the actual segment data content is larger message nr. 2!) 
  Hence you're strongly advised to *independently* encrypt those *header* chunks affixed (prefixed or postfixed doesn't matter) to the actual segment content.
* Have you considered *stenography*? Some email services *demand* you hand them *parsable* data to munch. Gmail, or example, only accepts a *few* file formats as attachments. ZIP is among them, but I noticed elsewhere that ZIP files are *inspected* and the restricted laid upon you also apply to the *ZIP content* then.
   How about wrapping the content in a nice juicy lossless-compression PNG header: we're not about dumping our content with a generic PNG prefix, *naye*, here we consider taking the *encrypted segment* data, treat those as if it were a large image bitmap (which is very much legal; heck, it might even make for a nice random-noise NFT image 🤡) and then we take that *bitmap* and pull it through the PNG compressor to ensure we'll be producing a very much legal, yet probably random-noise looking, lossless PNG image file: accepted across the pond and in the whole wide internet world! 
   We're NOT on about encoding messages in porn messages posted on public fora to facilitate secret communication with any mujahedeen brethren who fancy gawking at *the female artform* before they go and blow themselves (and possibly some *infidels*) to Kingdom Come. No, that's another level of stenography we don't need; all we want to thwart ore the marketeers and their robotic content inspectors, so we *steno* (hide) in an image what would otherwise be complained about as "cannot open your upload; upload is denied"; so we must ensure we'll come across as pure, virginal PNG *image material* to *those*.
* Okay, so you got the lesson about you being two different *entities* when you access the data in two places (home & work), because those have different *control structures* and thus should be treated as *independent*, hence the need to reduce *key leakage risk* by *not sharing anything you don't have to*: one key for each workplace. Sites, companies, *your home*, your *dorm*, your partner's place -- checked the divorce and relationship lifetime statistics somewhere this century, eh? 'Nuf said. Sorry to rain on your parade but partner's home turf is *independent location* until you have built a level of trust that's beyond many of us -- enough divorcees around to make it a non-negligible data point in our security analysis re *zones of control*. We're not romantic, *I know*. 💔)
* Given the above, now consider *teams*. Expect some of you to *leave before the end*. Can we deny people who leave access? 
  At least we CAN deny them access to any later posted goodies. (Once a-now-potentially-adversary has had access, you're basically *turfed*; that's why counter-espionage is such an important effort and why military security types beat the drum of *prevention* until your hemorrhaging from every orifice.)
  Can we also come up with a scheme where older data gets inaccessible once someone leaves? Short answer: no. Longer answer: the way to approach this is to deny team members *control* from the very beginning. I've seen it done where everyone had to log into a remote development system where all (patentable) code was kept, compiled, built and tested: *nothing* ever landed on the developer consoles *ever*. Plus they were not allowed to work at home, so *indirect* methods (remember: *side channel*!), like filming the console screen while logged in and then later feeding that into a big machine that can do OCR on the filmed scrolling content as the henchman/developer scrolled through the so-hotly-desired sourcecode. This was not 10 years ago, all modern hardware and everything, no "*old stories from geriatric curmudgeons*" either, so it's *doable* and *it is done*, when someone feels the need. (Of course, a side-effect of this highly secure approach was a much bemoaned *throughput* of developers at that company: for some unfathomable reason most developers did not last long and the *attrition rate* was around 20% per year. (*knowledge drain*, anyone?) Something about a *trust issue* that nobody had the *cohones* to put to paper and confront the management and investors about their socio-political psychological *faux pas*; the only funny thing was the investors "*did not understand because they are paid well-above market rates*". Gosh. 🤔 Darn! 😱 Ghee wiz. I don't understand it either![^helping] Yeah. 🤡)


Yes, setting up security is tough, but mixing it with actual *humanity* is a great social & ethical challenge! And it's all about trust and trust issues. Some of which will be hurt and you'll have to anticipate for that eventuality as well. When you doubt this, study history. Every major military endeavour has been marred by spies, traitors and other *security mishaps* (whistleblowers, ...), best efforts of the original party not-withstanding. While they are both good and bad influences and effects -- depending on your vantage point, world view and a case-by-case re-evaluation -- the key take-away is this:

Ultimately, planning for *failure* and how to cope with such is a paramount *last act* of the security / risk analyst professional, before the job can be called *done*: no matter what you decide and how you implement it, expect *failure of your design* and at the very least sit down with some friendly folks to discuss how such eventualities should be handled. 
Think **when**, NOT **if**, for the latter is the prelude to a pipedream, where you OD at the end! And every serious case of OD is fatal. 100%.
In other words: in security the LD<sub>50</sub> of "*if*" is 1: you're a certifiably lucky bastard if the shit didn't hit the fan during your lifetime! The key phrases you'll need to concern yourselves with instead are the Double-You ones: *when*. who. *how*. where. *which*. what. 




  



[^helping]: Give the man a hand a *generous helping* of that superb "*I have nothing to hide*" medicine! Remember I mentioned that one earlier. Well, the trouble with *security* is that it's... *people*. And when you don't generate sufficient *buy-in* (e.g. chauvinistic sentiment in a country's army), don't expect your mercs to stay around when you clearly communicate to them you trust them as far as you can throw 'em. And that's what such a secured "remote shell work" setup is telling everyone with wo *functioning* brain cells: the buggers *do not trust me*. Which is a kind of invitation, all by its lonesome, but in a perhaps... ? *sado-masochistic* way. 
  As I am not of the dominatrix-seeking persuasion, our *baltz* was swift: *don't look no further*, because all you'll see is my tail lights shifting into the deep infrared and I don't think youse snowflakes you are eagerly anticipating that. Dream the dream, I'm gone!
  
[^forget]: I wonder... I initially wrote "tend to forget" because that's the idiom around here, but then I'd have to assume everyone except my own group must be horribly *stupid*. Statistics and logic dictate that is a ludicrous assumption, hence "tend to forget" is a grave mistake, possibly inviting some grievous bodily harm if I don't run fast and far enough, I imagine. So... that *idiom* must be faulty and should've read "tend to ignore" or something of that ilk. *Discard*? *Reject*? "willy-nilly", "*hauteur*" (**ō-tœr′**) and "megalomania" are fighting for first place as *prima* mental association while I pondered this. *Neglect*? 🤔 Näh, too much Dunning-Kruger flavour, that one. But! Is it stupidity, then, after all? Then! Do I really know what *stupid* means?! (Can't speak nor hear, if I recall. But I digress while I doubt my mental verbal state and ability to judge my fellow upright ape.) 
  At the start of it all, I became convinced quite early in life that the droll statement "*I have nothing to hide*" (with or without the "*or loose*" clause) is up there with the best and brightest when you wish to claim *primality* on Planet Inane Stupidity. Heck, I don't know a faster way to showcase the applicable use of those two words together: *inane* and *stupid* -- as in its colloquial use for *dumb beyond belief*. You, my friend, are *the pinnacle* indeed.
  Well, let's take it from there and pick our least connotationally loaded version and apply that: "tend to... ignore." *Sigh*. When, later in life, I met people who were active in the military environment and received some education re *the security mentality and processes*, those "paranoid" ethics nicely slotted into my pre-existing notions, only showing me I had been partly *dead wrong* and fundamentally *had not been paranoid enough yet*.






## The technology behind it

- Uses `gpg` (GnuPG) to encrypt and decrypt your library data.
- By using `gpg` we can set up sync systems which can be made publicly available, i.e. placed **in encrypted form** on public-access or low security cloud storage
* see SO: `gpg` can "address" multiple recipients at once by encoding the random secret encryption key multiple times in the single message: each encoding is encrypted with one recipient's public key, so's every *legal recipient* will be able to grab the actual decrypt key by decrypting their particular little leader chunk.
  * https://security.stackexchange.com/questions/117600/how-to-decrypt-a-file-encrypted-under-multiple-public-keys-gpg?rq=1
  * https://security.stackexchange.com/questions/161319/how-to-encrypt-data-asymmetrically-to-be-decrypt-my-multiple-users?rq=1
  * https://security.stackexchange.com/questions/251762/why-am-i-able-to-decrypt-the-data-i-send-using-someone-elses-public-key?rq=1
  * https://stackoverflow.com/questions/3491481/encrypting-large-files-using-a-public-key + https://en.wikipedia.org/wiki/Watermarking_attack
  * ...
* ...

