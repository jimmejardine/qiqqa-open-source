# Can and should we use our BLAKE3+BASE58X based unique identifier as unique document id?

Any full-text search subsystem (Lucene, SOLR, manticore) needs to identify the documents found (and previously indexed). This is their *document id*, which, obviously, MUST be unique.

## Q 

A few questions need to be asked and evaluated:

- **re-use on other nodes**: since search index building is such a costly task, do we wish to (ever) provide the search index database as part of any synchronization mechanism across machines?

  > This would be useful when you have multiple workstations and you have done bulk document import work on a powerful one and wish to transport/copy those ready-for-use results to your other workstations, including some light laptops, etc.: not having to redo the heavy indexing+OCR+whatnot work on those less powerful machines is not only good for your carbon footprint but also MAY provide you with ready-to-use search databases that much quicker! (A Lucene database init would than be a mere sync/download away; it would still take *time* and *bandwidth*, but save you a heck of a lot of CPU time.)

- **ease of use of Qiqqa subsystems from a programmable origin / transportability of raw search results**: should we make sure our *document ids* are globally unique? Or can, or should, we use document ids which are only *single system unique*?

   > I want to be able to access the Qiqqa subsystems from a *scriptable environment* for custom "Big Data"-like analysis and meta-research. 
   > 
   > For that, obviously, you only need to have document ids which are unique to thee subsystems which your scripts are addressing -- the FTS engine, for example. When there's an easy mapping from those *document ids* to globally unique document hashes (BLAKE3+BASE58X) then you can link results produced by the various Qiqqa subsystems and even share those with others: those BLAKE3 hashes will then identify the documents in your own scripted output. 
   > 
   > However, it might be beneficial to have all subsystems produce that BLAKE3 hash as *the* document id already: one less mapping table to access for every query run. Plus much less confusion about which id is what (type confusion) when you are just starting out in this area of computing (novice-safe subsystem output).

- **performance penalties**: is there any performance penalty for using these 44-character wide BLAKE3+BASE58X globally unique document ids?

  > SQLite benefits from having 64-bit integer primary keys for its tables. *That* then is a *locally unique document id* as it will benefit the performance of that particular subsystem. 
  > 
  > If we want to (be able to) present globally unique identifiers to the outside world, a 64-bit `rowid` to BLAKE3 document hash mapping table will have to be provided, making all queries more complex as that's an added `JOIN tables` for every query which has to adhere to these "*globally unique document ids only*" demands.
  > 
  > > The BASE58X part is only important when you wish to reduce the published ASCII string hash id in a short form: BASE58X delivers the BLAKE3 hash in 44 characters while a simple `hex(hash)` function will produce a 64 characters wide string for the same.
  > 
  > Another important subsystem (Lucene/SOLR/manticore) usually produces its own unique document ids in UUID v4 form (SOLR). Ours are shorter strings, but we do not know whether this will be acceptable to these subsystems and whether performance is impacted negatively when we feed it custom document ids.
  > 
  > > \[Edit:] looks like there's no impact... but what about their storage cost? That remains unmentioned so we'll have to find out ourselves.

- **acceptable to all subsystems**: is the BLAKE3 256-bit hash acceptable as identifying *unique document id* for all our subsystems (much of which we won't have written ourselves) or do we need to jump through a couple of hoops to get them accepted?

  > My *initial guess* is that everybody will be able to cope, more or less easily, but actual practice can open a few cans of worms, if you are onto Murphy's Law.
  > 
  > In point of fact, the FTS subsystem will not be able to accept plain binary BLAKE3 hashes anyway (iff we stick to our path towards SOLR) as this subsystem will be expecting *text data formats* only, including any enforced *document ids* we may be feeding it. This then is an area of use for our BASE58X encoding design as a size-optimal competitor for ubiquitous `hex()`.
  > 
  > External use through scripting (as mentioned further above) is another such source/destination where binary data is not welcome in raw form, at least not for basic fields such as `document_id` as we intend to provide this possibility through offering up our subsystems as small(-ish) web services which any script language of the user's choice and preference can easily query: those "web queries" will be a lot easier for everyone to code and process when I can give them some JSON or XML data to munch on instead of *raw binary*: another spot where BASE58X may shine.


## A
### re-use on other nodes

While one can argue that any user scripts will run locally and that we do not expect any *remote access* to our subsystems, there's still the data produced by those *locally run user scripts*: by applying our BLAKE3(+BASE58X) globally unique document ids rigorously throughout all our subsystems, we make life easier for any scripting users: no confusion whether the various local ids match up and the reports produced by these scripts can be simply and safely transported to other places anywhere in the world: the BLAKE3 content hash will ensure the global uniqueness of each document. 

> **Warning**: there's also those pesky "Qiqqa References", which are document records which already contain metadata but no document yet. Those come with an identifying document id / "content hash" *anyway*, but there's another article in it for those alone about handling these upon "sync": do we make them so they never (or *very rarely*) collide? In Qiqqa, you can *bind* those buggers to newly imported documents: that means the Reference Id is now deleted? Or what? And how do other nodes cope on sync?

#### Advice: use these BLAKE3-based *globally unique ids* /everywhere/.


### ease of use of Qiqqa subsystems from a programmable origin / transportability of raw search results

That one has already been addressed above (and in the notes in the questions) as it is argued that this is very closely related to "re-use on other nodes": it's not the scripts *peer se*, but their output that benefits from unambiguous transportability everywhere.


## performance penalties

Ditto: see the notes at the question.  SQLite benefits from using a 64bit integer number internally, so that one SHOULD use the mapping table for internal id -> BLAKE3-based globally unique id. We'll just have to put up with the consequences.

After all, it is near identical to having a basic database table where the primary kay is *not* an `INTEGER PRIMARY KEEY`: under the hood, SQLite will do an additional lookup, i.e. use a mapping table, *anyhow*. Only *this time* around it'll be called *an index on the primary key*. Which is not `rowid` then.

Thus I expect minimal impact on that front (storage in database), while the programming costs for queries will increase as we now have to address that *explicit* mapping table (which is merely there to host that same lookup index).

This would, therefor, be a little hairy, compared to direct blatant use of the BLAKE3+BASE58X hash as each table's *document id* reference/key.


### acceptable to all subsystems

Nope. Not in raw binary form. Hence BLAKE3+BASE58X for anyone who needs it as a string. The FTS subsystem is one customer for those. Future user-defined scripts (for the *scriptable access to all subsystems*) are another.

Also note that iff we were to go with `manticore` instead of `SOLR`, then we'd be facing *document id* issues as manticore assumes you're feeding it 64bit integers or auto-generating them *for you* AFAICT. (See also: [Migrating to Manticore 3: document ids – Manticore Search](https://manticoresearch.com/2019/07/04/migrating-to-manticore-3-document-ids/))
That would mean there's yet another 64bit integer to BLAKE3+BASE58X string-based globally unique id mapping table parked in your subsystems, but then the question becomes: is *this* FTS database transportable across nodes, now that we know it's restricted to using 64bit unique id integers?

Yes, it is... *probably*. **With caveats, however!**
Here the reasoning goes like this:

Suppose you can make manticore use the SQLite `INTEGER PRIMARY KEY` 64bit unique ids, then that would be a handy 1:1 mapping without much fuss for anyone to peruse.
When you need the BLAKE3 hash, you can do a quick SQLite table lookup query to add those and you'd be set.

So far, so good.

How about Qiqqa Sync? Uh-oh.

Two base scenarios: 

1. you work alone and **never, ever, evar, EVAH** make the mistake of forgetting to run a *successful* sync before you run to your other node and sync that one to track your work and thus "drag it along with you as you move around". Then one can argue that all SQLite 64bit-to-BLAKE3 mapping tables will have the exact same content since there's never any 3-way merge activity happening. Golden.

   Sweet dreams, because this scenario is so full of vipers and rattle snakes, I don't even sweat no more. Way past that phase, we are. Because you only need *one mistake, once* and it's *poof*!
   
2. there's multiple Qiqqa nodes and whether you're a bit foggy and absent-minded in the brain sometimes -- like me -- or you're working with a team of any size doesn't matter: 3-way merges will be your joy and livelihood, if you care about syncing all those nodes (and their libraries) in a nice, safe, data-preserving way.

   Suffice to say that those SQLite 64->BLAKE mapping tables will start to deviate as soon as you hit the first 3-way merge. **That is the moment the precondition breaks for being able to simply copy your FTS/manticore database around at sync** -- not because we're so hung up on that SQLite mapping table, but because that one is the simplest *equivalent problem* as we're facing with manticore, whether we have it use its own 64bit integers or whatnot: it's not the SQLite mapping table *per se* that's so important here, it's that it follows the exact same patterns and ideas and faces the exact same dilemmas and challenges as we would with manticore's 64bit integer document ids.

   > *Pardon me*, that would be **signed 64 bit**. For more, see [[Why it might be smart to treat 64-bit document id integer as a 63-bit one instead|why it might be smart to treat 64-bit *document id* `INTEGER` as a *63-bit* one instead]].

   **The caveats**: given the logic above, we can conclude that *any time* your Qiqqa sync has to perform a 3-way sync, your manticore FTS database becomes *disconnected as it were, i.e. you'll have to do the work of updating it from this point onwards on your own* -- thus you then lose the nice feature of being able to copy it around just like that.

---
   
*\<german>Aber o-ho! Moment mal!\</german>* Wouldn't it be possible to produce ourselves a 64bit integer *worldwide unique key* after all? Say, by *folding* the BLAKE3 hash into 64bits and call that one our *unique identifier*? That's still $1 : 2^{\frac {64} {2}} = 1 : 2^{32} \approx 1 : 4 \text{ billion}$ for the birthday paradox, eh? *Plenty* for *my* library, you say!

Sure. But regrettably, chance doesn't work that way. You don't always get treated like the Average Joe. You will be lucky, probably often, but the birthday paradox doesn't keep you away from having 4 people with the same birthday in a company of, say, 20. 
   
Why's BLAKE3 good enough then? 

Because we're betting on the math behind the chances: BLAKE3 promises us a $1 : 2^{\frac {256} {2}} = 1 : 2^{128}$ chance of getting ourselves bricked like a castrated camel and *we decided we feel that chance is small enough that we're willing to risk it*. So we go play with the Sultan's daughter anyway. 

Of course, some folks will bristle at this exposition of statistical chance and what it really means to be a *cryptographic strong hash*, but that's the bottom line right there. To quote the famous Mr. Eastwood: "*Do you feel* lucky, *punk? Well? Do you?!*"

And with only $1 : 2^{32}$ working for *me*, I just don't feel lucky enough to go and live that dangerously. Call me a wuss or a sissy, I don't care. I'll wait and I'll see you burn. Maybe not you, but surely someone else will go down in flames with that one. I brought the marshmallows. And a little stick to put them on. Anybody got a guitar with them, perchance? Super!

What to do then, hm?


   


   
   


---

## Ergo?

Use BLAKE3+BASE58X where possible, so it is consistently appearing at the interfaces of all subsystems.

The SQLite database may be optimized by using a 64bit integer primary key internally, but then the onus is on the scripting user to `JOIN` any query result set with the BLAKE3+BASE58X mapping/lookup table to ensure she and others perusing her data can be assured that the reported document ids are globally unique.



## References

- [UniqueKey - Solr - Apache Software Foundation](https://cwiki.apache.org/confluence/display/solr/UniqueKey#UniqueKey-Usecaseswhichrequireauniquekeygeneratedfromdatainthedocument)
- [Updating Parts of Documents | Apache Solr Reference Guide 8.8.2](https://solr.apache.org/guide/8_8/updating-parts-of-documents.html)

  > Incidentally, *Child Documents*, it seems, are not the way forward towards registering and tracking our *document duplicates and near duplicates* which are mentioned elsewhere.
  > 
  > I wonder if I misunderstand what I've read and seen about them, or if I indeed should stick with the *mind view* of a flat table like storage organization in Lucene/SOLR and thus have near duplicates and such simply marked by an extra attribute which assigns them to a *group of similar documents*.
  > Given that we identify multiple causes for duplication and near duplication, the attribute-based categorization seems to be the more natural choice there anyway.
  > 
  > > - [DocValues | Apache Solr Reference Guide 8.11](https://solr.apache.org/guide/8_11/docvalues.html)
  > > - [Indexing Nested Child Documents | Apache Solr Reference Guide 8.0](https://solr.apache.org/guide/8_0/indexing-nested-documents.html)

- [Automatically Generate Document Identifiers – Solr 4.x – Solr.pl](https://solr.pl/en/2013/07/08/automatically-generate-document-identifiers-solr-4-x/)
- 