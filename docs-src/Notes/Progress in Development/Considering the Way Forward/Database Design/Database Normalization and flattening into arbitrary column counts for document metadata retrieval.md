# Database Normalization and flattening into arbitrary column counts for document metadata retrieval

Jot-down note:

`COALESCE` and a few other bits and pieces (which I need to dig up again, *sigh*) can serve here, so to answer my own doubt/question from a long time:

NO NEED to store a de-normalized attribute table for the metadata: title, author, and all the others can be plonked into a neatly *normalized* table with (very) minor performance worries: the SQL queries to reconstruct the metadata as a *flattened single row per document* exist (of course!) and also seem to perform reasonably well these days (SQLite, others...). 

Has to do with using `WITH` query optimizations in most generic database engines and it *sounds* like SQLite is *on par* there, at least for our purposes: 100K+ document estimate, 10-100 metadata attributes per document, some of these "*non-unique*" (e.g. *author*: document can have multiple authors and you can *fold* those buggers using `COALESCE` plus `WITH` or similar (classical) subquery-alike approaches; from what I gather thus far, `WITH` can be optimized through auto-temporary-table-construction, where classically one had to create stored procedures and do the temp table stuff scratch-padding by oneself -- hello Oracle! -- but Oracle 11g2 or what-is-it has this stuff, PostgreSQL has it, and everyone that's anyone else has too, or so it seems. 😘)

TL;DR or myself: stop worrying about the normalization-or-pre-flatten-anyway insistent mind-eating worry! It's gonna be fine!

