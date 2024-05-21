# Installing R + TeX Live + bookdown for producing documentation

From: https://handsondataviz.org/ ; adapted to my situation / scenario where I DO NOT use `tinytex` but a full install of [TeX Live](https://www.tug.org/texlive/windows.html) instead, as I already had that previously installed and wished to use that one for all TeX/LaTeX work done.


------ (\[edited\] quote) -------


## [Install and Set Up Bookdown](https://handsondataviz.org/install.html#install)

Below are steps we followed to set up the Bookdown publishing platform and related tools for this book, using our Macintosh OS 10.14 computers. The same general principles also should apply to Windows computers. No special knowledge is required, but these tools may not be ideal for novice computer users. Installation steps—and inevitable problems that pop up—will be easier if you are comfortable with exploring your computer, or already have some familiarity with text editors, GitHub, or R Studio.

1. Install R Project statistical programming language [https://www.r-project.org](https://www.r-project.org/), which is required by Bookdown. [See screenshot](https://handsondataviz.org/images/20-bookdown/r-download.png)

   > On Windows, this means running the downloaded R 'base' download, e.g. `R-4.4.0-win.exe` (May 2024). You may optionally install the R Tools, but those are a different animal and are not needed here.
    
2. Install the free version of [RStudio Desktop](https://posit.co/download/rstudio-desktop/) to make R easier to use with a visual editor. [See screenshot](https://handsondataviz.org/images/20-bookdown/rstudio-download.png). Some authors compose their books in RStudio, but you may use any text editor. Our personal preference is the [Atom editor](https://atom.io/) from GitHub.
    
   > On Windows, this means running the installer after download, e.g. `RStudio-2024.04.1-748.exe` (May 2024).
   
3. Inside RStudio, select the Packages tab, and select Install. [See screenshot](https://handsondataviz.org/images/20-bookdown/packages-install.png)
    
   > I want to use the existing TeX Live full install, which was previously downloaded from the [TeX Live](https://www.tug.org/texlive/windows.html) site and is named `install-tl-windows.exe` (May 2024). 
   > 
   > To ensure this TeX instance can be reached from RStudio, you need to start RStudio and enter the following commands in the "console" tab:
   > 
   > ```
   > print(Sys.which('pdflatex'))
   > system2('pdflatex', '--version')
   > ```
   > 
   > and you should see a path and a chunk of information being dumped by both commands. If not, then your TeX install cannot be reached and you may need to restart your computer and/or check your PATH environment settings.
   > 
	
4. Inside RStudio, install the “`bookdown`” package to build your book, and select Install Dependencies. [See screenshot](https://handsondataviz.org/images/20-bookdown/bookdown-install.png)
    
5. Bookdown now should be successfully installed in RStudio. [See screenshot](https://handsondataviz.org/images/20-bookdown/bookdown-installed.png)
    
6. For Bookdown to create a PDF edition of your book, you need to install a [LaTeX](https://en.wikipedia.org/wiki/LaTeX) engine to prepare your Markdown plain text, citations, and images into stylized pages. Since the full-sized [LaTeX project](https://www.latex-project.org/get/) is very large, Bookdown recommends the smaller TinyTeX package. Inside RStudio, select the Packages tab, select Install, and enter “tinytex” to find and upload the package. [See screenshot](https://handsondataviz.org/images/20-bookdown/tinytex-install.png)
    
   > I wanted to use the existing TeX Live full install instead, so there was no need for me to install `tinytex`.
   > To ensure this TeX instance can be reached from RStudio, you need to start RStudio and enter the following commands in the "console" tab:
   > 
   > ```
   > print(Sys.which('pdflatex'))
   > system2('pdflatex', '--version')
   > ```
   > 
   > and you should see a path and a chunk of information being dumped by both commands. If not, then your TeX install cannot be reached and you may need to restart your computer and/or check your PATH environment settings.
   > 
    
7. ~~To finish installing tinytex, in the RStudio console, type `tinytex::install_tinytex()` and press return. [See screenshot](https://handsondataviz.org/images/20-bookdown/tinytex-finish.png)~~
    
8. When you installed RStudio, it also should have installed its own version of [Pandoc](https://pandoc.org/installing.html), the package that converts files from Markdown format to HMTL and other formats. To confirm the Pandoc installation and version number, in the RStudio console, **type `rmarkdown::pandoc_version()` and press return**. The resulting version number should be `2.3.1` or higher. To install a newer version of Pandoc, which is highly recommended, go to [https://pandoc.org](https://pandoc.org/).
    

### [Download, Build, and Host a Sample Bookdown Book](https://handsondataviz.org/install.html#download-build-and-host-a-sample-bookdown-book)

While Bookdown does not require you to use GitHub, these steps show how to integrate these tools to make your own copy of a sample Bookdown book.

1. Create a free [GitHub](https://github.com/) account to simplify steps for the next two sections. While Bookdown does not require you to use GitHub, the workflow described below features GitHub to copy a sample Bookdown template and to host your own Bookdown editions online. To learn more about the basics of this tool, see [Chapter 10: Edit and Host Code with GitHub](https://handsondataviz.org/github.html).
    
2. In your web browser, log into your GitHub account, go to the Bookdown developer’s `bookdown-minimal` repo [https://github.com/yihui/bookdown-minimal](https://github.com/yihui/bookdown-minimal), and fork a copy to your GitHub account.
    
3. Install GitHub Desktop [https://desktop.github.com](https://desktop.github.com/) to transfer files between your online GitHub repo and local computer. While software developers may prefer to access GitHub by typing commands in their terminal (`git clone ...`), GitHub Desktop provides easier point-and-click access for most users.
    
4. In your web browser, go to your forked copy of `bookdown-minimal`, click the green `Code` button, and select `Open in Desktop`. This should automatically open the GitHub Desktop application, and you can navigate where you wish to store a copy of your code repo on a folder in your local computer.
    
5. In RStudio in the upper-right corner, select *Project > Open Project* to open the `bookdown-minimal` folder on your local computer. [See screenshot](https://handsondataviz.org/images/20-bookdown/project-open.png)
    
6. In RStudio, open the `index.Rmd` file and make some simple edits to the text of this minimal book. For example, remove the hashtag `#` comment symbol in line 8 to “uncomment” and activate the PDF book option. Save your edits. [See screenshot](https://handsondataviz.org/images/20-bookdown/edit-book.png)
    
7. Optional: If you wish, you can modify your `bookdown-minimal` files outside of RStudio, by using your preferred text editor, such as Atom editor [https://atom.io](https://atom.io/).
    
8. In RStudio, upper-right corner, select the Build tab, select Build Book, and choose All Formats to build both the gitbook-style static web edition and PDF edition.
    
9. If RStudio successfully builds both editions of your minimal book, the output will be saved into your `bookdown-minimal` folder, in a subfolder named `_book`, because that’s how this sample is configured. The RStudio internal browser should automatically open your web edition (but it’s not a very good browser, so we typically close it and manually open the `index.html` file with our regular browser.)
    
10. Also, open the subfolder and inspect the PDF edition of your book. If any errors were generated in the process, error messages will appear in red type in the RStudio Build viewer, which may require you to debug errors and delete temporary files as instructed. [See screenshot](https://handsondataviz.org/images/20-bookdown/build-successful.png).
    

Tip: In future sessions with RStudio, you should select the Packages tab and click *Update* to keep Bookdown and other software packages up to date. [See screenshot](https://handsondataviz.org/images/20-bookdown/update-packages.png)

11. Close your project, and quit RStudio. The next set of steps will focus on pushing your edited book to your GitHub repository using the GitHub Desktop tool.
    
12. Open GitHub Desktop and navigate to the `bookdown-minimal` folder on your local computer. Write a quick summary to commit (or save) the changes you made above to your master branch, and push this version to your online GitHub repo.
    
13. In your web browser, go to your online GitHub repo, with a web address similar to `https://github.com/USERNAME/bookdown-minimal`.
    
14. In your GitHub repo, select *Settings*, and scroll down to the *GitHub Pages* section, which is a free web hosting service to publish your code and book editions on the public web. Change the *Source* from *None* to *Main*, keep the default `/root` option in the middle, and press *Save*.
    
15. Scroll down to the *GitHub Pages* section again, and the web address of your published site should appear similar to `https://USERNAME.github.io/bookdown-minimal`.
    
16. Copy your published web address from above, paste into a new browser tab, and at the end add `_book/index.html`. The reason is because your sample book is configured by default to store all web and PDF editions in your `_book` subfolder, with `index.html` serving as the home page. Therefore, the full web address in your new browser tab should be similar to: `https://USERNAME.github.io/bookdown-minimal/_book/index.html`
    

**Tip**: You may need to wait up to one minute for edits to your GitHub online repo to appear live at your GitHub Pages web address. Also, after waiting for GitHub Pages to make changes, be sure to “force reload” or “hard refresh” your web browser to update directly from the GitHub Pages server, not the browser’s internal cache.


------ (end of \[edited\] quote) -------


## Using Obsidian with those `.Rmd` R MarkDown files

Also note https://forum.obsidian.md/t/rmarkdown-compatibility-let-obsidian-handle-rmd-files-just-as-md-files/27092 as I generally use Obsidian to edit and manage my MarkDown notes: given the response there (2021 A.D.), ~~I may need to provide a push/transform script to copy those MD files to `.Rmd` extension/format before RStudio + bookdown will be able to pick those up and process them.
Plus a reverse transform script/tool in case those Rmd files happen to be edited...~~ 🤔

Turns out that the magic anno 2024 goes like this:

- install the "Custom File Extensions" community plugin and edit its *Options* by adding the `Rmd` extension in the quoted set of supported extensions listed there.
- create a simply test markdown file in Obsidian. Write a bit of test test (*lorem ipsum* perhaps).
- Go to this file as shown in the Obsidian Navigation Browser (left panel, via right-click on title tab of the test file and selecting "*Reveal file in navigation*" menu item).
- Go to the file in the *navigation panel* and right-click, then select the "*Edit Extension*" menu item.
- Change the extension from `md` to `Rmd` and choose to add this extension to the registry when requested. 

🥳 Now you can use `Rmd` note files just like as if they are basic `md` MarkDown files in Obsidian. 🥳



