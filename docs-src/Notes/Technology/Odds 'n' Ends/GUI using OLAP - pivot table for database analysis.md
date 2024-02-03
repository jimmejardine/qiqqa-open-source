# GUI using OLAP - `tad` pivot table for database analysis

For (pivot table) table-based analysis & filtering of our PDF/document library metadata.

`tad` reportedly uses SlickGrid (which we're very familiar with) as the major UI component; `tad` is done in TypeScript + ReactUI; the way I read it it's using a WASM=browser-side instantiation of DuckDB, though I doubt if that is ultimately necessary as we're running a local server and I know SlickGrid always came with a DataView table/view cache component which should be able to serve the pivot table logic. Unless I am mistaken, of course.

To be researched further at a later date.

Here's the info and partial README on `tad`:

- https://github.com/antonycourtney/tad
## `tad`

[Tad](https://www.tadviewer.com/) is an application for viewing and analyzing tabular data sets.

The Tad desktop application enables you to quickly view and explore tabular data in several of the most popular tabular data file formats: CSV, Parquet, and SQLite and DuckDb database files. Internally, the application is powered by an in-memory instance of [DuckDb](https://duckdb.org/), a fast, embeddable database engine optimized for analytic queries.

The core of Tad is a React UI component that implements a hierarchical pivot table that allows you to specify a combination of pivot, filter, aggregate, sort, column selection, column ordering and basic column formatting operations. Tad delegates to a SQL database for storage and analytics, and generates SQL queries to perform all analytic operations specified in the UI.

Tad can be launched from the command line like this:

```
$ tad MetObjects.csv
```

This will open a window with a scrollable view of the full contents of the CSV file:

[![Tad screenshot](https://github.com/antonycourtney/tad/raw/master/doc/screenshots/tad-metobjects-unpivoted.png "Unpivoted view of CSV file")](https://github.com/antonycourtney/tad/blob/master/doc/screenshots/tad-metobjects-unpivoted.png)

Tad uses [SlickGrid](http://slickgrid.net/) for rendering the data grid. This allows Tad to support efficient linear scrolling of the entire file, even for very large (millions of rows) data sets.

A few additional mouse clicks on the above view yields this view, pivoted by a few columns (`Department`, `Classification`, `Period` and `Culture`), sorted by the `Object Start Date` column, and with columns re-ordered:

[![tad screenshot](https://github.com/antonycourtney/tad/raw/master/doc/screenshots/tad-metobjects-pivoted.png "Met Museum Objects with Pivots")](https://github.com/antonycourtney/tad/blob/master/doc/screenshots/tad-metobjects-pivoted.png)

### Installing Tad

The easiest way to install the Tad desktop app is to use a pre-packaged binary release. See [The Tad Landing Page](http://tadviewer.com/#news) for information on the latest release and download links, or go straight to the [releases](https://github.com/antonycourtney/tad/blob/master/releases) page.

### History and What's Here

Tad was initially released in 2017 as a standalone desktop application for viewing and exploring CSV files.

The core of Tad is a React UI component that implements a hierarchical pivot table that allows you to specify a combination of pivot, filter, aggregate, sort, column selection, column ordering and basic column formatting operations. Tad delegates to a SQL database for storage and analytics, and generates SQL queries to perform all analytic operations specified in the UI.

This repository is a modular refactor of the original Tad source code, with several key improvements on the original code base:

- The repository is organized as a modular [Lerna](https://lerna.js.org/) based monorepo.
- The code has been ported to TypeScript and the UI code has been updated to React Hooks.
- There is support for communicating with multiple database back ends for reltab (Tad's SQL generation and query evaluation layer), in addition to the original sqlite. Current backends (in varying degrees of completeness) include DuckDb, Snowflake, Google BigQuery, and AWS Athena (Presto)
- There is a minimal proof-of-concept web-based front-end to demonstrate how Tad can be deployed on the web.
- The core Tad pivot table component now builds in its own module independent of any front end. This should allow embedding the Tad pivot table in other applications or contexts.




