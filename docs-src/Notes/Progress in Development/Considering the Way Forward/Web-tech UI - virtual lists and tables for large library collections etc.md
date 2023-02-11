# Web-tech UI: virtual lists and tables for large library collections, etc.

We will have large and *huge* libraries, for which we'll need to display lists (tables?) of documents. Given that these libraries easily can grow into the 10K+ size realm, and the per-document row display is composed of many components (title, author, year, page count, publisher, rating and read status markers/icons, graphic bar representing keyword space, etc.), it is *very probably* a good idea to use *virtual lists* and *virtual tables* for this to keep the UI *fast* and responsive.

For (virtual) tables, there's, of course, our old friend [SlickGrid](https://slickgrid.net/).

For (virtual) lists, we might consider using the same, or some others. Check out the links below for virtual lists, tables and related issues in web tech:

- https://github.com/sergi/virtual-list#virtual-dom-list / https://sergimansilla.com/blog/virtual-scrolling/
	- https://bugzilla.mozilla.org/show_bug.cgi?id=373875
	- https://bugzilla.mozilla.org/show_bug.cgi?id=1518433
- https://slickgrid.net/
	- https://github.com/6pac/SlickGrid (OG: https://github.com/mleibman/SlickGrid)
- https://github.com/jpmorganchase/regular-table
- https://github.com/SheetJS/sheetjs
- https://github.com/wenzhixin/bootstrap-table
- https://github.com/javve/list.js
- https://github.com/jspreadsheet/ce
- https://github.com/agershun/alasql
- https://github.com/filamentgroup/tablesaw -- don't like this one; the 'swipe' UI looks nice but its execution is unclear to me in actual practice (UX); 'toggle checkboxes' should *toggle* them, not flip all to the same state. Would rather code this in SlickGrid, if it doesn't have those features already.
- https://github.com/olifolkerd/tabulator
- https://github.com/Mottie/tablesorter (or maybe https://github.com/christianbach/tablesorter?)
- https://github.com/mkoryak/floatThead
- https://github.com/future-architect/cheetah-grid
- https://github.com/ag-grid/ag-grid


- Extras:
	- https://github.com/denysdovhan/wtfjs  :-)
	- https://github.com/adamschwartz/chrome-tabs
	- https://github.com/NigelOToole/direction-reveal :-) 
	- https://github.com/codrops/FullWidthTabs
	- https://github.com/CodyHouse/responsive-tabbed-navigation
