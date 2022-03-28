# Why it might be smart to treat 64-bit *document id*  `INTEGER` as a 63-bit one instead

Check out [Migrating to Manticore 3: document ids – Manticore Search](https://manticoresearch.com/2019/07/04/migrating-to-manticore-3-document-ids/).

Now, I hear you say -- and you are *so utterly correct* -- "*but that's so damn easy, no bloody $2^{64} - \text{value}$ subtraction needed! It's just a `(int64_t)id` typecast away and that's **zero cost**! What's the bloody fuss? And why do you come up with* 63-bit, *anyway?!"

Yes, as long as we all remain in the nice computer programming languages (C/C++ preferentially, [for there's *no admittance* ones like, well, *JavaScript* and *TypeScript*]([Number.MAX_SAFE_INTEGER - JavaScript | MDN (mozilla.org)](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/MAX_SAFE_INTEGER))!)

However, there are many circumstances where we will have some form of *formatted-as-text* ids traveling the communications' paths, both between Qiqqa subsystems (FTS <-> SQLite metadata database core engine, f.e.) and external access APIs (localhost web queries where external user-written scripts interface with our subsystems directly using cURL/REST like interfaces and/or larger JSON/XML data containers): in these circumstances it is highly desirable to keep the risk of confusion, including confusion and subtle bugs about whether something should be encoded and then (re)parsed as either *signed* or *unsigned* 64bit integer to a bare minimum.

Better yet: we SHOULD make sure these types of confusion and error cannot ever happen! That's one source for bugs & errors less to worry about!

The solution there is to specify that all *document ids* used where "64 bit integer type" is to be expected (note my intentional ambiguity about that "integer type" being signed or unsigned: that's your first line of errors: inconclusive specifications anywhere!) to enforce the rule / specification that those numbers MUST always be non-negative, whether they are ever interpreted as *signed* or *unsigned*.

*That* means we will only accept **63-bit(!) document ids**.

This has consequences elsewhere in our codebases, of course, because folding a 256bit BLAKE3 identifier onto a potential 63-bit *document id* throws up a few obvious questions:

- how would we like to fold then? With 64-bit it might be rather trivial, given the origin being a cryptographically strong 256bit hash number, to *fold* that number using a basic 64-bit XOR operation on all the 64bit QuadWords in that hash value: that would be 3 XOR operations and *presto*!

  Now that we require 6**3**bits, do we fold bit 63 after we have first folded to 64bit using the cheapest approach possible (XOR)? And how? XOR onto bit 0? Or do we choose to do something a little more sophisticated or even less CPU intensive, iff that were possible? So! Many! Choices!
- Or do we just discard those bits (№'s 63, 127, 195 and 255) from the original 256bit hash? Where's that paper I read that clipping/discarding bits from a (cryptographic) hash does make a rotten quality *folded hash*?



## References

- [Number.MAX_SAFE_INTEGER - JavaScript | MDN (mozilla.org)](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/MAX_SAFE_INTEGER) 
- [max integer value in JavaScript - Stack Overflow](https://stackoverflow.com/questions/21350175/max-integer-value-in-javascript)
- [Fixed width integer types (since C++11) - cppreference.com](https://en.cppreference.com/w/cpp/types/integer)
- [Migrating to Manticore 3: document ids – Manticore Search](https://manticoresearch.com/2019/07/04/migrating-to-manticore-3-document-ids/)
- 


