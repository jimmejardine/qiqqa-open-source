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

  > SQLite benefits from having 64bit integer primary keys for its tables. *That* then is a *locally unique document id* as it will benefit the performance of that particular subsystem. 
  > 
  > If we want to (be able to) present globally unique identifiers to the outside world, a 64bit `rowid` to BLAKE3 document hash mapping table will have to be provided, making all queries more complex as that's an added `JOIN tables` for every query which has to adhere to these "*globally unique document ids only*" demands.
  > 
  > > The BASE58X part is only important when you wish to reduce the published ASCII string hash id in a short form: BASE58X delivers the BLAKE3 hash in 44 characters while a simple `hex(hash)` function will produce a 64 characters wide string for the same.
  > 
  > Another important subsystem (Lucene/SOLR/manticore) usually produces its own unique document ids in UUID v4 form (SOLR). Ours are shorter strings, but we do not know whether this will be acceptable to these subsystems and whether performance is impacted negatively when we feed it custom document ids.
  > x
  > > \[Edit:] looks like there's no impact... but what about their storage cost? That remains unmentioned so we'll have to find out ourselves.

- **acceptable to all subsystems**: is the BLAKE3 256bit hash acceptable as identifying *unique document id* for all our subsystems (much of which we won't have written ourselves) or do we need to jump through a couple of hoops to get them accepted?

  > My *initial guess* is that everybody will be able to cope, more or less easily, but actual practice can open a few cans of worms, if you are onto Murphy's Law.
  > x
  > In point of fact, the FTS subsystem will not be able to accept plain binary BLAKE3 hashes anyway (iff we stick to our path towards SOLR) as this subsystem will be expecting *text data formats* only, including any enforced *document ids* we may be feeding it. This then is an area of use for our BASE58X encoding design as a size-optimal competitor for ubiquitous `hex()`.
  > x
  > External use through scripting (as mentioned further above) is another such source/destination where binary data is not welcome in raw form, at least not for basic fields such as `document_id` as we intend to provide this possibility through offering up our subsystems as small(-ish) web services which any script language of the user's choice and preference can easily query: those "web queries" will be a lot easier for everyone to code and process when I can give them some JSON or XML data to munch on instead of *raw binary*: another spot where BASE58X may shine.


## A
### re-use on other nodes

While one can argue that any user scripts will run locally and that we do not expect any *remote access* to our subsystems, there's still the data produced by those *locally run user scripts*: by applying our BLAKE3(+BASE58X) globally unique document ids rigorously throughout all our subsystems, we make life easier for any scripting users: no confusion whether the various local ids match up and the reports produced by these scripts can be simply and safely transported to other places anywhere in the world: the BLAKE3 content hash will ensure the global uniqueness of each document. 

> **Warning**: there's also those pesky "Qiqqa References", which are document records which already contain metadata but no document yet. Those come with an identifying document id / "content hash" *anyway*, but there's another article in it for those alone about handling these upon "sync": do we make them so they never (or *very rarely*) collide? In Qiqqa, you can *bind* those buggers to newly imported documents: that means the Reference Id is now deleted? Or what? And how do other nodes cope on sync?

#### Advice: use these BLAKE3-based *globally unique ids* /everywhere/.


### ease of use of Qiqqa subsystems from a programmable origin / transportability of raw search results

That one has already been addressed above (and in the notes in the questions) as it is argued that this is very closeely related to "re-use on other nodes": it's not the scripts *peer se*, but their output that benefits from unambiguous transportability everywhere.


## performance penalties

Ditto: see the notes at the question.  SQLite benefits from using a 64bit integer number internally, so that one SHOULD use the mapping table for internal id -> BLAKE3-based globally unique id. We'll just have to put up with the consequences.

After all, it is near identical to having a basic database table where the primary kay is *not* an `INTEGER PRIMARY KEEY`: under the hood, SQLite will do an additional lookup, i.e.e use a mapping table, *anyhow*. Only *this time* around it'll be called *an index on the primary key*. Which is not `rowid` then.

Thus I expect minimal impact on that front (storage in database), while the programming costs for queries will increase as we now have to address that *explicit* mapping table (which is merely there to host that same lookup index).

This would, therefor, be a little hairy, compared to direct blatant use of the BLAKE3+BASE58X hash as each table's *document id* reference/key.


### acceptable to all subsystems

Nope. Not in raw binary form. Hence BLAKE3+BASE58X for anyone who needs it as a string. The FTS subsystem is one customer for those. Future user-defined scripts (for the *scriptable access to all subsystems*) are another.


---

## Ergo?

Use BLAKE3+BASE58X where possible, so it is consistently appearing at the interfaces of all subsystems.

The SQLite database may be optimized by using a 64bit integer primary key internally, but then the onus is on the scripting user to `JOIN` any query result set with the BLAKE3+BASE58X mapping/lookup table to ensure she and others perusing her data can be assured that the reported document ids are globally unique.
