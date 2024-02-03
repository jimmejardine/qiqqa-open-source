# GUI using `Evidence` to write analysis reports with live charts and data

To help write a report/analysis/overview-paper? with direct access to the Qiqqa metadata database and producing inline charts based on database queries written as part of said writeup, i.e. integrating Evidence into our GUI.

Another interesting project was/is [Excalichart](https://github.com/excalichart/excalichart) but current judgement is that that one is too focused on charts-only, while Evidence feels a bit more like a [Jupyter Notebook](https://jupyter.org/) (without the drawbacks I see in that one). Incidentally, others in that direction are [Cantor for KDE](https://cantor.kde.org/)  and , from one of my favorite software architects/developers: [ObservableHQ](https://observablehq.com/) -- regrettably the latter is SaaS and my policy with Qiqqa is to *not use SaaS at all*: you don't really own your own data and software that way.
Anyway, at the moment `Evidence` looks like a reasonable way forward for the Qiqqa UI.


Here's the original info on Evidence:

- https://github.com/evidence-dev/evidence
- https://evidence.dev/ :: Build polished data products with SQL.


## Evidence

Evidence is a lightweight framework for building data apps. It's open source and free to get started.

Evidence is Business Intelligence (BI) as Code: Generate reports using SQL and markdown.

### How It Works

Evidence is an open-source, code-based alternative to drag-and-drop business intelligence tools.

[![how-it-works](https://github.com/evidence-dev/evidence/raw/main/sites/docs/static/img/how-it-works.png)](https://github.com/evidence-dev/evidence/blob/main/sites/docs/static/img/how-it-works.png)

Evidence generates a website from markdown files:

- **SQL statements** inside markdown files run queries against your data sources
- **Charts and components** are rendered using these query results
- **Templated pages** generate many pages from a single markdown template
- **Loops** and **If / Else** statements allow control of what is displayed to users

