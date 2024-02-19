# Database Normalization vs storing document trees and graphs

Quick jot-down note:

check out the new `WITH RECURSIVE` SQL query syntax - it's available with all modern major database engines, *including SQLite* and serves to retrieve a (partial) tree or graph chain using a single SQL query. 🎉yay!🎉

Asked around in my own neighbourhood: no experience with this yet, so no performance knowledge/experience to learn from there. Internet is also pretty quiet when I look. 
Of course, fundamentally, it's a series of `JOIN`s, so *guestimated* performance should follow that trail.

Anyway: we can safely consider storing graphs and trees and expect outside dev/users to be able to query* that info straight from the DB.
Saves writing dedicated APIs...



## References
- https://codedamn.com/news/sql/recursive-queries-in-sql
- https://builtin.com/data-science/recursive-sql
- https://learnsql.com/blog/recursive-cte-sql-server/
- [Creating arbitrary-depth recursive queries in SQLITE (works for any SQL compliant system) using CTEs (common table expressions)](https://gist.github.com/jbrown123/b65004fd4e8327748b650c77383bf553)
- [I found this really simple recursive CTE useful for ensuring I understood how to write recursive CTEs.](https://til.simonwillison.net/sqlite/simple-recursive-cte)
- 
- 

## Commit comment per 2024/02:

... working on the new database layout.

While this is now more flexible and "normalized" qua table layout, I'm still worried about performance once we load this animal past 100K+ metadata records, which is easily possible once we start processing the collected document corpuses once again.

There's also to consider the option where we PRE-load a *very* large metadata database from elsewhere so as to have most metadata on-board while we load and relate documents to those records, which didn't come with documents (PDF, ...) when we imported the metadata database. Such a database can easily reach 100M+ records if we grab and import certain sources: that's where a normalized design with several N:M relations (table joins all) will screech down to a halt. However, such imported animals would not come with full content and files attached, so maybe it's a good choice to have a raw FLAT import table alongside for all such metadata that hasn't been linked to a document yet?

🤔


