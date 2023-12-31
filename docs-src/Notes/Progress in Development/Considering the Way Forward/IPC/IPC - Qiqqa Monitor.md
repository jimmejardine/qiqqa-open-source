# IPC :: Qiqqa Monitor

The QiqqaMonitor wants a steady stream of performance data from the various Qiqqa components.

The Monitor is a *client*, where the various Qiqqa (background) processes are *servers*.

## Pull vs. Pull-based monitor data

While we *could* choose to use a *server push* mechanism, we opt, instead, to use a **data pull** approach where the Monitor determines the pace of the incoming data by explicitly sending requests for the various performance data.

Initially, I had some qualms about this, as this implies we would add *requesting overhead* that way, but a **pull**-based approach is more flexible:

- *server push* would either need a good timer server-side and a fast pace to ensure the client receives ample updates under all circumstances. Of course, this can be made *smarter*, but that would add development cost. (See also the next item: multiple clients)
- *multiple clients* for the performance data is a consideration: why would we *not* open up this part of the Qiqqa data, when we intend to open up everything else to user-scripted and other kinds of direct external access to our components? **pull**-based performance data feeds would then automatically pace to each client's 'clock' without the need to complicate the server-side code-base.
- *pace* must be *configured* for *server push* systems if you don't like the current data stream, while we don't have to do *anything* server-side when we go for **pull**-based data feeds: if the client desires a faster (or slower) pace, it can simply and *immediately* attain this by sending more (or fewer) *data requests*.

## Which data to track server-side

Next to this, there are a few more bits to keep in mind when we code this baby up:

- we don't want (potentially large) distortions in our performance data under load.

Previous experience with Qiqqa (v80 series and older) has shown us that the machine can be swiftly consumed and (over)loaded with PDF processing requests when importing, reloading, recovering or otherwise bulk-processing large libraries. We may want to know the size of our task queues, but *performance* is not equal to the delta of the *fill size* of said queues: when the machine is choking on the load the number of entries processed per second may be low or high, but we wouldn't know as many requests (PDF text extracts) trigger PDF OCR tasks thus filling up the task queue fast while work is being done.
  
Hence we would want to see both the **number of tasks completed** and the **number of tasks pending**.
  
As we could derive the other numbers we would need for a performance chart from these two, these should suffice:
- actual number of queued work items
- running total of work items completed
- extra:
 	- running total of work items *skipped*
 	- running total of work items *rescheduled*
  	
	These latter two are only relevant when we want to observe the effects of (temporarily) disabling certain background processes as we did in the old Qiqqa application.
	

Derived data values:

- Total Work Pending =~ Backpressure In The System = `QSize`
- Total Work Completed = `NCompleted`
- Total Work Requested = `QSize + NCompleted`
- Work Speed = `NCompleted[now] - NCompleted[previous] / TimeInterval`
- Work Request Speed =~ Queue Growth Speed = `TotalWorkRequested[now] - TotalWorkRequested[previous] / TimeInterval`

and if we track the base data (number of queued items + running total of completed items) per task type/category/priority, then we can derive those data values for each priority category and thus show some useful diagnostics to the user when the Qiqqa backend is hard at work.


### Extra data: what are we currently working on?

Qiqqa v80 series has a rather chaotic UI statusbar based report going to show what's being done.

While this can be replicated by asking the background processes what task they're each working on *right now*, perhaps we could provide a smarter and potentially more useful UX by also tracking the amount of time spent per work category and thus be able to report the (average/minimum/peak) run-time cost per work category. 


#### ... and veering off at a tangent...

Some times we even dream of being able to drill down to the *document* and possibly *page* level for this data, but that would mean we'ld be storing all that work item monitor info in a persistent database. 
   
Which makes one wonder: use SQLite for this (with its query construction and parsing overhead), or go the NoSQL LMDB route? 
- https://www.pdq.com/blog/improving-bulk-insert-speed-in-sqlite-a-comparison-of-transactions/
- what about our document hashes? A *task hash* would be a document hash, task type code *plus* page number (up to at least 2500 as that's about the largest page count in a book I've ever seen). [[../Fingerprinting Documents/Compressing document and other hashes to uin32_t or uint64_t|Can't we compress our document and task hashes to a regular integer number so it's swifter and taking up less memory?]]