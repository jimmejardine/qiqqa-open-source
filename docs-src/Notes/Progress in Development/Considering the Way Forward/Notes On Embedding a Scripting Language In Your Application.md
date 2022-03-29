
# Notes On Embedding a Scripting Language In Your Application

Others have gone there before... this is what they say:

- [Embedding Lua vs Python](https://eev.ee/blog/2016/04/30/embedding-lua-vs-python/)
- [Lua: states, threads, libraries and memory layout](http://www.thijsschreijer.nl/blog/?p=693)

Those stood out at least as apparently having scratched deeper than the mere surface. Which is important in these matters. [^There was *something* about the Devil, ceiling fans, a certain amount of feces and other details we peeps don't wanna hear about. La.La.la.la!]

Python: while for data processing it would be my first choice as that's what every kid and his uncle is supposed to poke into these days when they *go big data*, it already looked like a serious bit of hassle to accomplish a full embedding python engine[^which includes the ability to call C/C++ code from your embedded script, which should then, in turn, be able to call python code via callback or similar means, hence nested invocations and thus stack unwinding over both script and native layers when an exception happens in the innermost software circuitry.]

Lua seems more lenient on that matter, coding-hours-wise.[^By Jove, let's pray I didn't overlook something major there!]

## Hints to help do this

Lua:

- [Embedding a scripting language inside your C/C++ code](http://web.archive.org/web/20201027092132/https://debian-administration.org/article/264/Embedding_a_scripting_language_inside_your_C/C_code)  -- *viva el Wayback Machine!* 
- [Awesome Lua](https://github.com/LewisJEllis/awesome-lua)



## Notes

First preference was python due to perceived market share in the expected application areas. Embedding the python interpreter is described in the official docs as treating it like writing your own APIs as if those were native plugins to Python, and then focus separately on the embedding of the engine itself. Sounds good, but there seem to be a few issues with stack unwinding when an exception is thrown inside a python coded callback, when the unwinding will travel "through" the native code section(s).

First glance at the code samples doesn't make this look like an easy task if you want to produce production-quality integration.

See also https://eev.ee/blog/2016/04/30/embedding-lua-vs-python/ for related noises.

So we looked for other options: Lua and JavaScript[^not V8 mind you because that one is at about the same level of difficulty (estimated work hours) as embedding CPython properly]; here we're looking at the `mujs` JavaScript engine provided by Artifex and a few other options, which are mostly targeting embedding devices, etc.: we're not interested in maximum performance for the scripts here but ease of integration rather: they're meant to glue together native components which do the grunt work of the process that you're setting up, so this is supposed to use only a few CPU cycles from the grand total of your to-be-scripted processes.

One drawback of `mujs` is its JS language support: ES5 only. `ducktape` is another well supported candidate but suffers the same issue, while `jerryscript`, which I hadn't heard about before, does look like a *modern* JS engine, language wise: [this is what @kangax has to say about that!](https://github.com/LewisJEllis/awesome-lua), so my vote goes to `jerryscript` this time. Sorry folks!

Yes, I'm quite fond of JavaScript, so I looked a little harder there. ;-)

Which leads us to Lua, which is engineered for tweaking configurations and such, so potentially ideal for component-gluing basic scripts. My programmer brain has a hard time accepting 1-based arrays though: that Pascal heritage is, what, 30 years ago for me now? 

**Ah! But who's the *target audience* here?** I.e. who is supposed to use that script language? Not me, but *others*. People with different interests and generally no software *engineering* experience under their belt. So we did a sanity check and browsed a bit to see what people are complaining about. There's always plenty of that, like [this one](https://www.reddit.com/r/javascript/comments/ft7zn/why_i_switched_from_lua_to_javascript/)^[oh, no need for a rocket scientist degree to observe my *bias* here ;-) ] but the general noise level, at least to my eyes, is that of "generally happy with it, few quirks, nothing *nasty*"^[While there's always good old Mr. Crockford to hit on the net if you want a leg up on what's really *ugly* in JavaScript, plus there's a *slew* of folks who really don't *dig* prototypical inheritance -- which I find rather more beautiful than class-based OOP, which is also great but pretty much ruined by peeps going nuts on class hierarchies and patterns for the purpose of patterns.</rant> Sorry, been around corporate contractors too long, I guess. I think one of the folks over at [PVS](https://pvs-studio.com/en/pvs-studio/) hit the nail on the head when they subtly mentioned in an aside that their software analyzers are bought, used and lauded by many companies, but they had looked at their sales data and the total number of IT mercs, pardon, contractors in their customer base was an absolute **zero**, which had given the man pause and some unfortunate yet irrefutable, logical, conclusions. Guess whom I've been around most of my life. ;-) ]

Conclusion: let's do Lua, shall we? Looks like the easiest path for an MVP at least, if I want user scripting in there. And when folks start to use it more and want *more*, we can always move in and offer an embedded CPython implementation alongside: they don't bite AFAICT and the multithreading challenges (if you want multithreading there, that is) are about the same.

For the *users of that language*, here's a few bits to keep in mind as maintenance/support is probably gonna end up in my lap anyway?

- https://www.reddit.com/r/javascript/comments/ft7zn/why_i_switched_from_lua_to_javascript/  -- read the comments. Es5 and ES6 is a different world. And Lua 5.3 + 5.4 support *integers*, which JavaScript really does *not*.^[Ok, insert hand wavy non-verbal thing and yes-no-ho-humming here, but keep in mind all things *numeric* in JS are floating point numbers. Cool and all, but I'm from C and I *like* my integers! The *one* thing I don't like about JS. Yeah, this is just me being a bit *miffed* I've got only 53-bit integers, not 64bit unless I go BigNum crazy! So sad. I'll take my Teddy Roosevelt now and go sulk and suck my thumb in a quiet corner.]
- http://notebook.kulchenko.com/programming/lua-good-different-bad-and-ugly-parts (While he's paraphrasing the Crockford line^[That one's as *murdered* by now, 2021 AD, by copycatting as is the oldest and most famous meme in IT county: "GOTO considered harmful"^[The original paper with that title was from Edsger Dijkstra and keeping in mind *<span language="nl">dat 'ie een Fries was die door omstandigheden in Amsterdam was beland, ja, dan kijk je er niet raar van op dat je op je ouwe dag wat veneinig uit de hoek kan komen. Friezen schijnen namelijk net als Duitsers toch over enige humor te beschikken (het onderzoek loopt nog; we houden hoop!) maar dat kon je in z'n epistels, ehhh, niet echt goed terugvinden, za'k maar zeggen. Dijkstra is verder dik okee; de man heeft ons semaforen gegeven en nog wat andere machtige derrie waar de meeste machientjes van nu toch wel verduld veel gebruik van maken onder de motorkap! Dus petje af, en ja ik weet, 't is **meneer Dijkstra** voor mij, maar die "GOTO considered harmful", die hoort bij jou, dat geeft je charme^[èn een beetje èxtra respect: je bent Fries en je zegt wat je d'r van vindt, zonder dure woorden - meneer Dijkstra praat geen poep, dat kan ik waarderen!] terwijl de piepjes die niet verder komen dan die titel plagiëren, nog *decennia* later, ach, wat zal ik d'r van zeggen... niet iedereen heeft genoeg talent om een Bob Ross over te schilderen, laat staan een Matisse of een Mondriaan, al zijn de tubetjes genummerd.</span>* Even if you did that for SEO rank-boosting click bait to help move your career forward.].], now for Lua, there's plenty good info there.


