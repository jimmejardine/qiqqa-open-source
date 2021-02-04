<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    <h1>Qiqqa Internals :: Configuration Overrides for Developers and Testers</h1>
<h2>Overriding the Qiqqa library <em>Base Path</em></h2>
<p>The regular ‘base path’, i.e. the base directory where all Qiqqa libraries are stored locally, is stored in the Windows Registry.</p>
<p>You can override this ‘base path’ by specifying another base path on the commandline.</p>
<blockquote>
<h3>Extra since v83</h3>
<p>Since v83 you can also click the ‘Change this path’ button in the startup dialog and point Qiqqa at another base directory,
which is then persisted by Qiqqa, i.e. Qiqqa will keep using the new directory in subsequent runs – until you change it again.</p>
</blockquote>
<h3>Why would you want to override this registry setting?</h3>
<p>For example:</p>
<ul>
<li>
<p>when you are testing Qiqqa and want to use a different (set of) Qiqqa Libraries for that. Overriding the ‘base path’ ensures your valuable Qiqqa libraries for regular use cannot be touched by the Qiqqa run-time under test.</p>
<blockquote>
<p>Assuming, of course, that the regular base path directory tree and the one you specified via the commandline do not overlap.</p>
</blockquote>
</li>
<li>
<p>when you wish to work on one or more Qiqqa Libraries which should not be integrated into your regular set of libraries, e.g. when you wish to help someone else by having a look into their library/libraries you got copied locally.</p>
</li>
</ul>
<h3>Commandline format</h3>
<pre class="language-sh"><code class="language-sh">qiqqa.exe &lt;basepath&gt;
</code></pre>
<p>e.g.</p>
<pre class="language-sh"><code class="language-sh">qiqqa.exe D:\Qiqqa.Test.Libs\base\
</code></pre>
<h2>Overriding Qiqqa behaviour</h2>
<p>You can override several Qiqqa behaviours by adding a <a href="https://json5.org/">JSON5</a> configuration file in the Qiqqa ‘base path’, i.e. the base directory where all Qiqqa libraries are stored locally, named <code>Qiqqa.Developer.Settings.json5</code>. Qiqqa will load this file at application startup.</p>
<h3>Configuring <code>Qiqqa.Developer.Settings.json5</code></h3>
<p>Here’s an example which lists all supported settings:</p>
<pre class="language-json5"><code class="language-json5"><span class="token comment">// This file may contain comments.</span>
<span class="token comment">//</span>
<span class="token comment">// Lines can be commented out at will.</span>
<span class="token punctuation">{</span>
	<span class="token property unquoted">LoadKnownWebLibraries</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>
	<span class="token property unquoted">AddLegacyWebLibrariesThatCanBeFoundOnDisk</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>
	<span class="token property unquoted">SaveKnownWebLibraries</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>
	<span class="token property unquoted">DoInterestingAnalysis_GoogleScholar</span><span class="token operator">:</span> <span class="token boolean">false</span><span class="token punctuation">,</span>

	<span class="token property unquoted">FolderWatcher</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>
	<span class="token property unquoted">TextExtraction</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>           <span class="token comment">// when </span><span class="token property unquoted">false</span><span class="token operator">:</span> this kills the mupdf based text extraction and OCR tasks
	<span class="token property unquoted">SuggestingMetadata</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>       <span class="token comment">// when </span><span class="token property unquoted">false</span><span class="token operator">:</span> this kills the metadata (Title<span class="token punctuation">,</span> Author<span class="token punctuation">,</span> etc.) suggesting from extracted text
	<span class="token property unquoted">BuildSearchIndex</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>         <span class="token comment">// when </span><span class="token property unquoted">false</span><span class="token operator">:</span> this kills the Lucene-based search index build/update process
	<span class="token property unquoted">RenderPDFPagesForSidePanels</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>         <span class="token comment">// this kills the rendering of PDF pages to thumbnails in preview sidepanels</span>
	<span class="token property unquoted">RenderPDFPagesForReading</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>            <span class="token comment">// this kills the rendering of PDF pages to main panel PDF view/read/edit tabs (NOT thumbnails!)</span>
	<span class="token property unquoted">RenderPDFPagesForOCR</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>                <span class="token comment">// this kills the rendering of PDF pages for OCR-ing a document's pages via QiqqaOCR background application</span>
<span class="token punctuation">}</span>
</code></pre>
<h4>Defaults</h4>
<p>Note that all settings in this file are assumed to be <code>true</code> by default, i.e. anything you don’t mentionexplicitly in there is assumed to be <code>true</code>.</p>
<h4><code>LoadKnownWebLibraries</code></h4>
<p>Set to <code>false</code> to <strong>disable</strong> Qiqqa’s default behaviour where it scans the ‘base path’ to <em>discover</em> all available Qiqqa Libraries.</p>
<p>All libraries which have not been included in your <strong>load list</strong> (as saved by Qiqqa in the previous run in the file <code>Guest/Qiqqa.known_web_libraries</code>) will be ignored.</p>
<p>Set to <code>true</code> to <strong>enable</strong> Qiqqa’s default behaviour.</p>
<h4><code>AddLegacyWebLibrariesThatCanBeFoundOnDisk</code></h4>
<p>Normally, Qiqqa will scan the base directory for any subdirectories (one level deep only!) containing a Qiqqa library, i.e. a <code>Qiqqa.library</code> database – and hopefully more stuff, like <em>documents</em>.</p>
<p>When this option is set to <code>false</code>, the scanning behaviour is <strong>skipped</strong>, thus producing a very <em>bare</em> library list in your Qiqqa Home page: you may expect only the <code>Guest</code> library to show up, or, when <code>LoadKnownWebLibraries</code> is <code>true</code>, the list of libraries remembered in that internal configuration file, i.e. the library list as persisted by the previous Qiqqa run (which had <code>SaveKnownWebLibraries</code> set to <code>true</code>).</p>
<p>Handy when your libraries are giving you headaches and you want to run Qiqqa on a minimal/reduced set.</p>
<p>Auto-discovery is back as soon as you set this flag to <code>true</code> again and restart Qiqqa – after all, auto-discovery of libraries only happens at the <em>start</em>.</p>
<h4><code>SaveKnownWebLibraries</code></h4>
<p>Set to <code>false</code> to <strong>disable</strong> Qiqqa’s default behaviour where it will save your <strong>load list</strong> (the list of Qiqqa Libraries currently discovered and loaded into Qiqqa) to disk in the file <code>Guest/Qiqqa.known_web_libraries</code>.</p>
<h4><code>DoInterestingAnalysis_GoogleScholar</code></h4>
<p>Set to <code>false</code> to <strong>disable</strong> Qiqqa’s default behaviour where it will perform a background <em>scrape</em> in Google Scholar for every PDF document you open / have opened in Qiqqa.</p>
<p>::: tip</p>
<p>Since Google is pretty picky and pedantic about “proper use of Scholar” (from their perspective), hitting that search website more often than strictly necessary should be regarded with some concern: with those background scrapes (which are used to fill the “Scholar” left side panel with some suggestions while the PDF document is open in the Qiqqa Viewer) you MAY expect Google to throw a tantrum and restrict your Scholar access using convoluted Captchas and other means when you really <strong>want</strong> to use Google Scholar in the Qiqqa Sniffer or elsewhere in the application.</p>
<p>Hence the smart move here is to kill those background scrapes as they don’t add a lot of value (unless you really like those left side panel Scholar suggestions, of course!)</p>
<p>:::</p>
<h4><code>FolderWatcher</code></h4>
<p>Set to <code>false</code> to <strong>disable</strong> Qiqqa’s default behaviour where it will perform a background <em>scrape</em> in Google Scholar for every PDF document you open / have opened in Qiqqa.</p>
<h4><code>TextExtraction</code></h4>
<p>when false: this kills the mupdf based text extraction and OCR tasks</p>
<h4><code>SuggestingMetadata</code></h4>
<p>when false: this kills the metadata (Title, Author, etc.) suggesting from extracted text</p>
<h4><code>BuildSearchIndex</code></h4>
<p>when false: this kills the Lucene-based search index build/update process</p>

  </head>
  <body>

    <h1>Qiqqa Internals :: Configuration Overrides for Developers and Testers</h1>
<h2>Overriding the Qiqqa library <em>Base Path</em></h2>
<p>The regular ‘base path’, i.e. the base directory where all Qiqqa libraries are stored locally, is stored in the Windows Registry.</p>
<p>You can override this ‘base path’ by specifying another base path on the commandline.</p>
<blockquote>
<h3>Extra since v83</h3>
<p>Since v83 you can also click the ‘Change this path’ button in the startup dialog and point Qiqqa at another base directory,
which is then persisted by Qiqqa, i.e. Qiqqa will keep using the new directory in subsequent runs – until you change it again.</p>
</blockquote>
<h3>Why would you want to override this registry setting?</h3>
<p>For example:</p>
<ul>
<li>
<p>when you are testing Qiqqa and want to use a different (set of) Qiqqa Libraries for that. Overriding the ‘base path’ ensures your valuable Qiqqa libraries for regular use cannot be touched by the Qiqqa run-time under test.</p>
<blockquote>
<p>Assuming, of course, that the regular base path directory tree and the one you specified via the commandline do not overlap.</p>
</blockquote>
</li>
<li>
<p>when you wish to work on one or more Qiqqa Libraries which should not be integrated into your regular set of libraries, e.g. when you wish to help someone else by having a look into their library/libraries you got copied locally.</p>
</li>
</ul>
<h3>Commandline format</h3>
<pre class="language-sh"><code class="language-sh">qiqqa.exe &lt;basepath&gt;
</code></pre>
<p>e.g.</p>
<pre class="language-sh"><code class="language-sh">qiqqa.exe D:\Qiqqa.Test.Libs\base\
</code></pre>
<h2>Overriding Qiqqa behaviour</h2>
<p>You can override several Qiqqa behaviours by adding a <a href="https://json5.org/">JSON5</a> configuration file in the Qiqqa ‘base path’, i.e. the base directory where all Qiqqa libraries are stored locally, named <code>Qiqqa.Developer.Settings.json5</code>. Qiqqa will load this file at application startup.</p>
<h3>Configuring <code>Qiqqa.Developer.Settings.json5</code></h3>
<p>Here’s an example which lists all supported settings:</p>
<pre class="language-json5"><code class="language-json5"><span class="token comment">// This file may contain comments.</span>
<span class="token comment">//</span>
<span class="token comment">// Lines can be commented out at will.</span>
<span class="token punctuation">{</span>
	<span class="token property unquoted">LoadKnownWebLibraries</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>
	<span class="token property unquoted">AddLegacyWebLibrariesThatCanBeFoundOnDisk</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>
	<span class="token property unquoted">SaveKnownWebLibraries</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>
	<span class="token property unquoted">DoInterestingAnalysis_GoogleScholar</span><span class="token operator">:</span> <span class="token boolean">false</span><span class="token punctuation">,</span>

	<span class="token property unquoted">FolderWatcher</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>
	<span class="token property unquoted">TextExtraction</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>           <span class="token comment">// when </span><span class="token property unquoted">false</span><span class="token operator">:</span> this kills the mupdf based text extraction and OCR tasks
	<span class="token property unquoted">SuggestingMetadata</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>       <span class="token comment">// when </span><span class="token property unquoted">false</span><span class="token operator">:</span> this kills the metadata (Title<span class="token punctuation">,</span> Author<span class="token punctuation">,</span> etc.) suggesting from extracted text
	<span class="token property unquoted">BuildSearchIndex</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>         <span class="token comment">// when </span><span class="token property unquoted">false</span><span class="token operator">:</span> this kills the Lucene-based search index build/update process
	<span class="token property unquoted">RenderPDFPagesForSidePanels</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>         <span class="token comment">// this kills the rendering of PDF pages to thumbnails in preview sidepanels</span>
	<span class="token property unquoted">RenderPDFPagesForReading</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>            <span class="token comment">// this kills the rendering of PDF pages to main panel PDF view/read/edit tabs (NOT thumbnails!)</span>
	<span class="token property unquoted">RenderPDFPagesForOCR</span><span class="token operator">:</span> <span class="token boolean">true</span><span class="token punctuation">,</span>                <span class="token comment">// this kills the rendering of PDF pages for OCR-ing a document's pages via QiqqaOCR background application</span>
<span class="token punctuation">}</span>
</code></pre>
<h4>Defaults</h4>
<p>Note that all settings in this file are assumed to be <code>true</code> by default, i.e. anything you don’t mentionexplicitly in there is assumed to be <code>true</code>.</p>
<h4><code>LoadKnownWebLibraries</code></h4>
<p>Set to <code>false</code> to <strong>disable</strong> Qiqqa’s default behaviour where it scans the ‘base path’ to <em>discover</em> all available Qiqqa Libraries.</p>
<p>All libraries which have not been included in your <strong>load list</strong> (as saved by Qiqqa in the previous run in the file <code>Guest/Qiqqa.known_web_libraries</code>) will be ignored.</p>
<p>Set to <code>true</code> to <strong>enable</strong> Qiqqa’s default behaviour.</p>
<h4><code>AddLegacyWebLibrariesThatCanBeFoundOnDisk</code></h4>
<p>Normally, Qiqqa will scan the base directory for any subdirectories (one level deep only!) containing a Qiqqa library, i.e. a <code>Qiqqa.library</code> database – and hopefully more stuff, like <em>documents</em>.</p>
<p>When this option is set to <code>false</code>, the scanning behaviour is <strong>skipped</strong>, thus producing a very <em>bare</em> library list in your Qiqqa Home page: you may expect only the <code>Guest</code> library to show up, or, when <code>LoadKnownWebLibraries</code> is <code>true</code>, the list of libraries remembered in that internal configuration file, i.e. the library list as persisted by the previous Qiqqa run (which had <code>SaveKnownWebLibraries</code> set to <code>true</code>).</p>
<p>Handy when your libraries are giving you headaches and you want to run Qiqqa on a minimal/reduced set.</p>
<p>Auto-discovery is back as soon as you set this flag to <code>true</code> again and restart Qiqqa – after all, auto-discovery of libraries only happens at the <em>start</em>.</p>
<h4><code>SaveKnownWebLibraries</code></h4>
<p>Set to <code>false</code> to <strong>disable</strong> Qiqqa’s default behaviour where it will save your <strong>load list</strong> (the list of Qiqqa Libraries currently discovered and loaded into Qiqqa) to disk in the file <code>Guest/Qiqqa.known_web_libraries</code>.</p>
<h4><code>DoInterestingAnalysis_GoogleScholar</code></h4>
<p>Set to <code>false</code> to <strong>disable</strong> Qiqqa’s default behaviour where it will perform a background <em>scrape</em> in Google Scholar for every PDF document you open / have opened in Qiqqa.</p>
<p>::: tip</p>
<p>Since Google is pretty picky and pedantic about “proper use of Scholar” (from their perspective), hitting that search website more often than strictly necessary should be regarded with some concern: with those background scrapes (which are used to fill the “Scholar” left side panel with some suggestions while the PDF document is open in the Qiqqa Viewer) you MAY expect Google to throw a tantrum and restrict your Scholar access using convoluted Captchas and other means when you really <strong>want</strong> to use Google Scholar in the Qiqqa Sniffer or elsewhere in the application.</p>
<p>Hence the smart move here is to kill those background scrapes as they don’t add a lot of value (unless you really like those left side panel Scholar suggestions, of course!)</p>
<p>:::</p>
<h4><code>FolderWatcher</code></h4>
<p>Set to <code>false</code> to <strong>disable</strong> Qiqqa’s default behaviour where it will perform a background <em>scrape</em> in Google Scholar for every PDF document you open / have opened in Qiqqa.</p>
<h4><code>TextExtraction</code></h4>
<p>when false: this kills the mupdf based text extraction and OCR tasks</p>
<h4><code>SuggestingMetadata</code></h4>
<p>when false: this kills the metadata (Title, Author, etc.) suggesting from extracted text</p>
<h4><code>BuildSearchIndex</code></h4>
<p>when false: this kills the Lucene-based search index build/update process</p>


    <footer>
      © 2020 Qiqqa Contributors ::
      <a href="https://github.com/GerHobbelt/qiqqa-open-source/blob/docs-src/Qiqqa Internals/Configuration Overrides for Developers and Testers.md">Edit this page on GitHub</a>
    </footer>
  </body>
</html>
