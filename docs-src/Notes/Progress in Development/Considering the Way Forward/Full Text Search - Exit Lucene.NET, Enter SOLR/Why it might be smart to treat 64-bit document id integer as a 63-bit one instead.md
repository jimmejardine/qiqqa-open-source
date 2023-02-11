# Why it might be smart to treat 64-bit *document id*  `INTEGER` as a 63-bit one instead

Check out [Migrating to Manticore 3: document ids – Manticore Search](https://manticoresearch.com/2019/07/04/migrating-to-manticore-3-document-ids/).

Now, I hear you say -- and you are *so utterly correct* -- "*but that's so damn easy, no bloody $2^{64} - \text{value}$ subtraction needed! It's just a `(int64_t)id` typecast away and that's **zero cost**! What's the bloody fuss? And why do you come up with* 63-bit, *anyway?!"

Yes, as long as we all remain in the nice computer programming languages (C/C++ preferentially, [for there's *no admittance* for ones like, well, *JavaScript* and *TypeScript*](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/MAX_SAFE_INTEGER)!)

However, there are many circumstances where we will have some form of *formatted-as-text* ids traveling the communications' paths, both between Qiqqa subsystems (FTS <-> SQLite metadata database core engine, f.e.) and external access APIs (localhost web queries where external user-written scripts interface with our subsystems directly using cURL/REST like interfaces and/or larger JSON/XML data containers): in these circumstances it is highly desirable to keep the risk of confusion, including confusion and subtle bugs about whether something should be encoded and then (re)parsed as either *signed* or *unsigned* 64bit integer to a bare minimum.

Better yet: we SHOULD make sure these types of confusion and error cannot ever happen! That's one source for bugs & errors less to worry about!

The solution there is to specify that all *document ids* used where "64 bit integer type" is to be expected (note my intentional ambiguity about that "integer type" being signed or unsigned: that's your first line of errors: inconclusive specifications anywhere!) to enforce the rule / specification that those numbers MUST always be non-negative, whether they are ever interpreted as *signed* or *unsigned*.

*That* means we will only accept **63-bit(!) document ids**.

This has consequences elsewhere in our codebases, of course, because folding a 256bit BLAKE3 identifier onto a potential 63-bit *document id* throws up a few obvious questions:

- how would we like to fold then? With 64-bit it might be rather trivial, given the origin being a cryptographically strong 256bit hash number, to *fold* that number using a basic 64-bit XOR operation on all the 64bit QuadWords in that hash value: that would be 3 XOR operations and *presto*!

  Now that we require 6**3**bits, do we fold bit 63 after we have first folded to 64bit using the cheapest approach possible (XOR)? And how? XOR onto bit 0? Or do we choose to do something a little more sophisticated or even less CPU intensive, iff that were possible? So! Many! Choices!
- Or do we just discard those bits (№'s 63, 127, 195 and 255) from the original 256bit hash? Where's that paper I read that clipping/discarding bits from a (cryptographic) hash does make a rotten quality *folded hash*?

----

**Update**: at least *older* Sphinx/Manticore Search documentation mentions *document ids* cannot be zero. This would mean a minimum legal range would then be $1 .. 2^{63}$ and our folding / id producing algorithm should provide for those -- a simple `if h = 0 then h = +x` won't fly. So I'm thinking along these lines:

- fold onto 64bit space quickly using a blunt 64-bit XOR.
- shift the bits we'ld loose that way so they form a relatively small integer. (Given the "id cannot be zero" condition, I'm counting bit0 among them.) For example:
     $$\begin{aligned}
v &= 1 + bit_0 \mathbin{◮} 1 + bit_{63} \mathbin{⧩} (63 - 2) + bit_{64+63} \mathbin{⧩} (64 + 63 - 3)      \\
&  \qquad  + \:  bit_{2 \cdot 64+63} \mathbin{⧩} (2 \cdot 64 + 63 - 4) + bit_{3 \cdot 64+63} \mathbin{⧩} (3 \cdot 64 + 63 - 5)  \\

      &= 1 + bit_0 \mathbin{◮} 1 + bit_{63} \mathbin{⧩} 61 + bit_{127} \mathbin{⧩} 124 + bit_{191} \mathbin{⧩} 187 + bit_{255} \mathbin{⧩} 250
      
      \end{aligned}$$
  where $\mathbin{◮}$ and $\mathbin{⧩}$ are the logical shift *left* and *right* operators, respectively. Then mix this value $v$ into the *masked* 64bit folded hash (the mask throwing away both $bit_{0}$ and $bit_{63}$). The idea being that the $\mathbin{+} 1$ ensures our end value will not be zero(0), but XOR-ing that one in just like that would loose us $bit_0$ so we take that one, alongside all the bits at $bit_{63}$-equivalent positions in the quadwords of the BLAKE3 hash that we would loose by restricting the result to a *positive 64bit integer range* and give them a new place in a new value $v$ (at bit positions 1,2,3,4 and 5) and then mix that value into the folded value.

  If we were to worry about those bits skewing those bit positions in the final result, we can always go and copy/distribute them across the entire 62bit area ($bit_1 .. bit_{62}$) that we get to work with here in the end.
  
Of course, we shouldn't put too much effort into this *folding* as we're not aiming for a *perfect hash* -- because we know we can't -- and any collision discovered at library sync/merge time will force us to adjust the 63-bit *document id* (with only 62 bits of spread as we now set $bit_0$ to 1 to ensure the *document id* is never zero). Maybe borrow some ideas from linear and quadratic hashing there, if we hit a collision? I.e. calculate a new *document id* "nearby", check if that one is still available and when it's not (another collision), loop (while incrementing our test step). That's a $2^{62}$ range of *document ids* to test, so we're practically guaranteed a free slot here.

> **Note**: note that the "may not be zero" requirement has taken another bit from our range power: while the number will be value in the integer value range $\lbrace 1 .. (2^{63} - 1) \rbrace$ thanks to two's complement 64 bit use, we are left with only $2^{(64-1-1)} = 2^{62}$ values we can land on as every *even number* has now become *off limits* thanks to our quick move to set $bit_0$ to 1.
> 
> The other option we have been pondering was to take the 256-bit hash value and **add 1 arrhythmically** and then fold *that one*, only this would require the use of BigInt calculus and would be rather more costly to do at run-time.
> 
> Then there's the next idea: do this *plus one(1)* work on the (already $bit_{63}$-masked) quadwords of the original hash, just before we fold them. While that would work (since drop those $bit_{63}$ bits before we do this) we will end up with a folded value that spans *beyond* the `MAXINT` ($2^{63} - 1$) value, which means another correction is mandatory before we would be done. And that correction would land us in the same (or very similar) situation as we have now, unless we are willing to do this using *arithmetic modulo*. As division is still costly on modern hardware, I prefer the faster bit operations, expecting a negligible difference in quality of output of this "shortened hash"-style *document id*...
> 
> **Third alternative** then would deliver a 62-biit range like we have, but we would have the (dubious) benefit of having both *even* and *odd* document ids: mask off the top 2 bits of every quadword, mix those into a new value $v_2$, resulting in an 8-bit value and mix that one into the folded 62-bit number we now get using XOR around. Now correct for the non-zero requirement by simply adding 1 and you're done.
> 
> > Hm, *maybe* I like that one better and go with that, but I know it's all bike-shedding from here...
> > 

---

**Update**: the **third alternative** mentioned above has been implemented as the `qiqqa_documentid62_main` example tool program. 

That one was ultimately chosen as it's the cleanest solution out of all the variants above, including the initial design described in detail above. Besides *cleanest* it also is the more consistent one: in this one we merit each dropped bit in folded value its own spot in the extra mix-in value, while the original design took the **folded** $bit_0$ and assigned it a single bit spot vs. assigning each of the masked top bits ($bit_{63}$) its own spot in $v$: that's using two different approaches without cause, where a single approach suffices and *probably* would do *better*, bit-distribution wise.



## References

- [Number.MAX_SAFE_INTEGER - JavaScript | MDN (mozilla.org)](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/MAX_SAFE_INTEGER) 
- [max integer value in JavaScript - Stack Overflow](https://stackoverflow.com/questions/21350175/max-integer-value-in-javascript)
- [Fixed width integer types (since C++11) - cppreference.com](https://en.cppreference.com/w/cpp/types/integer)
- [Migrating to Manticore 3: document ids – Manticore Search](https://manticoresearch.com/2019/07/04/migrating-to-manticore-3-document-ids/)
- 


