# Database Normalization and flattening into arbitrary column counts for document metadata retrieval

Jot-down note:

`COALESCE` and a few other bits and pieces (which I need to dig up again, *sigh*) can serve here, so to answer my own doubt/question from a long time:

NO NEED to store a de-normalized attribute table for the metadata: title, author, and all the others can be plonked into a neatly *normalized* table with (very) minor performance worries: the SQL queries to reconstruct the metadata as a *flattened single row per document* exist (of course!) and also seem to perform reasonably well these days (SQLite, others...). 

Has to do with using `WITH` query optimizations in most generic database engines and it *sounds* like SQLite is *on par* there, at least for our purposes: 100K+ document estimate, 10-100 metadata attributes per document, some of these "*non-unique*" (e.g. *author*: document can have multiple authors and you can *fold* those buggers using `COALESCE` plus `WITH` or similar (classical) subquery-alike approaches; from what I gather thus far, `WITH` can be optimized through auto-temporary-table-construction, where classically one had to create stored procedures and do the temp table stuff scratch-padding by oneself -- hello Oracle! -- but Oracle 11g2 or what-is-it has this stuff, PostgreSQL has it, and everyone that's anyone else has too, or so it seems. 😘)

TL;DR or myself: stop worrying about the normalization-or-pre-flatten-anyway insistent mind-eating worry! It's gonna be fine!

--------------


## We're actually talking about "*the [EAV](https://en.wikipedia.org/wiki/Entity%E2%80%93attribute%E2%80%93value_model) problem*" here. 

Quoting https://modern-sql.com/use-case/pivot:
 
The greatest challenge with the pivot problem is to recognize it when you run into it. This is particularity true when dealing with the so-called [entity-attribute-value (EAV) model](https://en.wikipedia.org/wiki/Entity%E2%80%93attribute%E2%80%93value_model): it does not look like a pivot problem, but it can nevertheless be solved in the very same way.
 
The EAV model takes normalization to the extreme and no longer uses columns in the traditional way. Instead, every single value is stored in its own row. Besides the value, the row also has a column to specify which attribute the value represents and a third column to identify what entity the values belongs to. Ultimately, a three column table can hold any data without ever needing to change the table definition. The EAV model is thus often used to store dynamic attributes.
 
The EAV model does not come without drawbacks: It is almost impossible to use constraints for data validation, for example. However, the most puzzling issue with the EAV model is that the transformation into a one-column-per-attribute notation is almost always done using joins—quite often one outer join per attribute. This is not only cumbersome, it also results in very poor performance—a true anti-pattern.
 
However, turning rows into columns is the pivot problem in its purest form. Therefore, these steps should be followed again: (1) use `group by` to reduce the rows to one row per entity, (2) use `[filter](https://modern-sql.com/feature/filter)` or `[case](https://modern-sql.com/feature/filter#conforming-alternatives)` to pick the right attribute for each column.

```
SELECT submission_id
     , MAX(CASE WHEN attribute='name'    THEN value END) name
     , MAX(CASE WHEN attribute='email'   THEN value END) email
     , MAX(CASE WHEN attribute='website' THEN value END) website
  FROM form_submissions
 GROUP BY submission_id
```

Note the use of the `max` function: it is required to reduce the rows of the group (all attributes) into a single value. This is a purely syntactic requirement that is applicable regardless of the actual number of rows that are grouped.
 
To obtain the original value for each attribute—even though we have to use an aggregate function—the respective filter logic (`case` or `filter`) must not return more than one not-`null` value. In the example above, it is crucial that each of the named attributes (`name`, `email`, `website`) exists only once per `submission_id`. If duplicates exist, the query returns only one of them.
 
The prerequisite that each attribute must not appear more than once is best enforced by a unique constraint.[0](https://modern-sql.com/use-case/pivot#footnote-0 "In this case, the constraint is on (SUBSIDIARY_ID, ATTRIBUTE).") Alternatively, the query can count the aggregated rows using `count(*)` and the [respective `case` expressions](https://modern-sql.com/feature/filter#emulate-count) (or `filter` clauses). The results can be validated in the application—if selected as additional columns—or in the `having` clause: `having count(*) filter (...) <= 1`.

If the prerequisite is satisfied and the aggregation is always done on a single not-`null` value, every aggregate function just returns the input value. However, `min` and `max` have the advantage that they also work for character strings (`char`, `varchar`, etc.).

---------




