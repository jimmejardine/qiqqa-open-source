# Database Normalization vs storing document trees and graphs

Quick jot-down note:

check out the new `WITH RECURSIVE` SQL query syntax - it's available with all modern major database engines, *including SQLite* and serves to retrieve a (partial) tree or graph chain using a single SQL query. 🎉yay!🎉

Asked around in my own neighbourhood: no experience with this yet, so no performance knowledge/experience to lean there. Internet is also pretty quiet when I look. 
Of course, fundamentally, it's a series of `JOIN`s, so *guestimated* performance should follow that trail.

Anyway: we can safely consider storing graphs and trees and expect outside dev/users to be able to query* that info straight from the DB.
Saves writing dedicated APIs...


