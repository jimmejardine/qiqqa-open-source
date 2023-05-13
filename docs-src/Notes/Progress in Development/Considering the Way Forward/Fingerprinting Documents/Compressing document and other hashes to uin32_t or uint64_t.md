# Can't we compress our document and task hashes to a regular integer number so it's swifter and taking up less memory?

Yes, we can. IFF...

...we reckon with the number of documents (or pages / tasks / what-have-you) we expect to *uniquely identify* using the BLAKE3 hash system.

First, let's identify the several sources we would like to index/identify that way:

- **documents** = **document hashes**. Preferably not per-library but *across-all-my-libraries-please*. Which would make the `uint64_t` *shorthand index* a very personal, nay, *machine*-specific shorthand, as we won't be able to know about the documents we haven't seen yet -- while we may be synchronizing and working in a multi-node or multi-user environment. See also [[Multi-user, Multi-node, Sync-Across and Remote-Backup Considerations]].
  That means we might have a different shorthand index value for document XYZ at machine A than at machine B. Definitely not something you would want to pollute your SQLite database with, for it would otherwise complicate Sync-Across across activity *quite a bit* as the thus-shorthand-linked data *would require transposing to the target machine*. **Ugh! *Hörk*!**
- **document+page**. Text extracts, etc. are kept per document, per page.
 
  Must say I don't have a particularly strong feeling towards needing a *shorthand index* for this one, though. Given [[BLAKE3+BASE58X - Qiqqa Fingerprint 2.0]], the raw, unadulterated cost[^1] would run me at: 
  
[^1]: see [[Fingerprinting - moving forward and away from b0rked SHA1|here for the factors]] used in the tables below
  
  For documents:
  
  | encoding     | calculus                   | # output chars        |
  |--------------|----------------------------|-----------------------|
  | binary:         | $32$              | 32 chars              |
  | Base64:      | $32 \times \frac {log(256)} {log(64)} = 42.7$     | 43 chars              |
  | Base58:      | $32 \times \frac {log(256)} {log(58)} = 43.7$     | 44 chars              |
  | **Base58X**: | $32 \times 7 \times \frac {8} {41} = 43.7$   | 44 chars too!    |

  For documents+pages, where we assume a 'safe upper limit' in the page count of `MAX(uin16_t)` i.e. 65535, which fits in 2 bytes:
  
  | encoding     | calculus                   | # output chars        |
  |--------------|----------------------------|-----------------------|
  | binary:         | $32 + 2$              | 34 chars              |
  | Base64:      | $(32 + 2) \times \frac {4} {3} = 45.3$     | 46 chars       |
  | **Base58X**: | $(32 + 2) \times 7 \times \frac {8} {41} = 46.4$   | 47 chars    |

  For *tasks*, which are generally page oriented (e.g. OCR document page) documents+page+taskCategoryID, where we assume a 'safe upper limit' in the page count of `MAX(uin16_t)` i.e. 65535, which fits in 2 bytes, plus the taskCategoryID assumed to always fit in a `uint8_t`, i.e. a single byte:
  
  | encoding     | calculus                   | # output chars        |
  |--------------|----------------------------|-----------------------|
  | binary:         | $32 + 2 + 1$              | 35 chars              |
  | Base64:      | $(32 + 2 + 1) \times \frac {4} {3} = 46.7$     | 47 chars       |
  | **Base58X**: | $(32 + 2 + 1) \times 7 \times \frac {8} {41} = 47.8$   | 48 chars    |

  i.e. storing every task performance record in an SQLite database would incur us the added key cost of  48 bytes (*plus SQLite-internal overhead*).
  
  
  
## Is hash compression useful at all?
  
Maybe.
  
Hashes for documents, tasks, etc. take, as we saw above, between 44 and 48 bytes each -- *and a string compare for equality checks*.
  
When we map these to internal, *system-native* `uint64_t` numbers, that would cost 8 bytes per index number and a very fast integer equality test.
  
*Alas*, we should wonder whether this is an undesirable  *micro-optimization* now that we've much bigger fish to fry still.
  Given the amount of extra work and confusion I can see already, I'd say: *nice thought, but not making it past the mark*. *Rejected.*
  
> After all, *huge* Qiqqa libraries would be between 10K .. 100K documents, where each document would, perhaps, average at less than 100 pages, thus resulting in about 100K document hashes and 10M (100K * 100) *task hashes*, which would clock in at 480M space (sans terminating NUL bytes, etc.) if we'd kept all those hashes around forever, which is kind of ridiculous.
> 
> Hence it's *probably* far smarter to assign fast `uint32_t` indexes for hashes while the application is running and for use in the application run-time and *no persistence ever*. And that's assuming you won't be running your Qiqqa server components for several days on end... *Nah. **Cute but no cigar**!*
  
  
  
## TL;DR
  
*Don't.* Too much fuss for very little gain. Does not mix well with
[[Multi-user, Multi-node, Sync-Across and Remote-Backup Considerations|Sync-Across]] either. 
  
