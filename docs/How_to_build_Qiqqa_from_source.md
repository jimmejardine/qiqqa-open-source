<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    <h1>How to build Qiqqa from source</h1>
<toc>
<h2>Prerequisites :: setting up your development environment</h2>
<p>Qiqqa is largely written in C#. Qiqqa is built using a Microsoft Developer Studio ‘solution file’ with multiple ‘project files’, one for each executable or DLL that’s part of the Qiqqa project.</p>
<p>Qiqqa <strong>software releases</strong> are created using the Inno Setup System, which packs the relevant compiled binaries and other files into a single <code>setup.exe</code>-style installer. The <code>setup.exe</code> is produced using shell scripts, as are a few other bits &amp; pieces in the qiqqa software.</p>
<p>If you want to build qiqqa from scratch yourself and/or wish to participate in its Open Source development process, which is centered around the Qiqqa GitHub repository and GitHub website, then you first need to install these tools to make sure you don’t run into nasty surprises where the compile/build/packaging processes expect certain tools to be present:</p>
<ul>
<li>
<p><a href="https://visualstudio.microsoft.com/">Microsoft Developer Studio 2019 <img src="assets/visualstudio-help-me-choose.png" alt=""></a></p>
</li>
<li>
<p><a href="https://dotnet.microsoft.com/download/visual-studio-sdks">The .NET Framework SDK for Visual Studio</a>, version 4.8 or later. (Migration to .NET Core is considered, but not decided on yet.)</p>
</li>
<li>
<p><code>git</code> : <a href="https://git-scm.com/downloads">https://git-scm.com/downloads</a></p>
</li>
<li>
<p><code>bash</code> UNIX shell - this one is included in the git-for-windows install linked above: <a href="https://git-scm.com/downloads">https://git-scm.com/downloads</a></p>
</li>
<li>
<p><code>node</code> + <code>npm</code> : NodeJS v12 or later - since we have some JavaScript in Qiqqa and build the documentation site and other bits &amp; pieces using Node.</p>
<p>While there is a pure NodeJS installer, we advise you to install <code>nvm</code> so you can switch Node versions on your development machine. This comes in handy when you do other projects with Node / JavaScript too:</p>
<ul>
<li>
<p><a href="https://docs.microsoft.com/en-us/windows/nodejs/setup-on-windows">https://docs.microsoft.com/en-us/windows/nodejs/setup-on-windows</a> : a nice write-up by the folks at Microsoft how to setup <code>nvm</code> and the rest.</p>
</li>
<li>
<p><a href="https://nodejs.org/en/download/">https://nodejs.org/en/download/</a> : the pure NodeJS installer.</p>
<p><strong>DOES NOT include <code>nvm</code> and DOES NOT work well with <code>nvm</code> when you install that one later on!</strong></p>
</li>
</ul>
<p>You are strongly advised to <a href="https://docs.microsoft.com/en-us/windows/nodejs/setup-on-windows">use the Windows-nvm approach described above</a>.</p>
<p>Make sure to install and activate the latest node v12 release via <code>nvm</code>, e.g.</p>
<pre class="language-bash"><code class="language-bash">nvm <span class="token function">install</span> <span class="token number">12.18</span>.3
nvm use <span class="token number">12.18</span>.3
</code></pre>
<p>Do <em>not</em> forget the <code>nvm use &lt;version&gt;</code> command there as that one <em>activates</em> the installed node version!</p>
</li>
</ul>
<h3>Extra bits you might need later</h3>
<ul>
<li>MSYS2 - or another way to get at tools like <code>wget</code> et al on Windows. Not mandatory and certainly not advised to install on a first try / first meet date with Qiqqa sources.
<ul>
<li><a href="https://www.msys2.org/#installation">https://www.msys2.org/#installation</a> - MSYS2 installer</li>
<li><a href="https://docs.microsoft.com/en-us/windows/wsl/install-win10">https://docs.microsoft.com/en-us/windows/wsl/install-win10</a> - Microsoft’s new approach: Windows Subsystem for Linux a.k.a. WSL or WSL2. Use this if you want/need something stronger than MSYS2.</li>
</ul>
</li>
</ul>

  </head>
  <body>

    <h1>How to build Qiqqa from source</h1>
<toc>
<h2>Prerequisites :: setting up your development environment</h2>
<p>Qiqqa is largely written in C#. Qiqqa is built using a Microsoft Developer Studio ‘solution file’ with multiple ‘project files’, one for each executable or DLL that’s part of the Qiqqa project.</p>
<p>Qiqqa <strong>software releases</strong> are created using the Inno Setup System, which packs the relevant compiled binaries and other files into a single <code>setup.exe</code>-style installer. The <code>setup.exe</code> is produced using shell scripts, as are a few other bits &amp; pieces in the qiqqa software.</p>
<p>If you want to build qiqqa from scratch yourself and/or wish to participate in its Open Source development process, which is centered around the Qiqqa GitHub repository and GitHub website, then you first need to install these tools to make sure you don’t run into nasty surprises where the compile/build/packaging processes expect certain tools to be present:</p>
<ul>
<li>
<p><a href="https://visualstudio.microsoft.com/">Microsoft Developer Studio 2019 <img src="assets/visualstudio-help-me-choose.png" alt=""></a></p>
</li>
<li>
<p><a href="https://dotnet.microsoft.com/download/visual-studio-sdks">The .NET Framework SDK for Visual Studio</a>, version 4.8 or later. (Migration to .NET Core is considered, but not decided on yet.)</p>
</li>
<li>
<p><code>git</code> : <a href="https://git-scm.com/downloads">https://git-scm.com/downloads</a></p>
</li>
<li>
<p><code>bash</code> UNIX shell - this one is included in the git-for-windows install linked above: <a href="https://git-scm.com/downloads">https://git-scm.com/downloads</a></p>
</li>
<li>
<p><code>node</code> + <code>npm</code> : NodeJS v12 or later - since we have some JavaScript in Qiqqa and build the documentation site and other bits &amp; pieces using Node.</p>
<p>While there is a pure NodeJS installer, we advise you to install <code>nvm</code> so you can switch Node versions on your development machine. This comes in handy when you do other projects with Node / JavaScript too:</p>
<ul>
<li>
<p><a href="https://docs.microsoft.com/en-us/windows/nodejs/setup-on-windows">https://docs.microsoft.com/en-us/windows/nodejs/setup-on-windows</a> : a nice write-up by the folks at Microsoft how to setup <code>nvm</code> and the rest.</p>
</li>
<li>
<p><a href="https://nodejs.org/en/download/">https://nodejs.org/en/download/</a> : the pure NodeJS installer.</p>
<p><strong>DOES NOT include <code>nvm</code> and DOES NOT work well with <code>nvm</code> when you install that one later on!</strong></p>
</li>
</ul>
<p>You are strongly advised to <a href="https://docs.microsoft.com/en-us/windows/nodejs/setup-on-windows">use the Windows-nvm approach described above</a>.</p>
<p>Make sure to install and activate the latest node v12 release via <code>nvm</code>, e.g.</p>
<pre class="language-bash"><code class="language-bash">nvm <span class="token function">install</span> <span class="token number">12.18</span>.3
nvm use <span class="token number">12.18</span>.3
</code></pre>
<p>Do <em>not</em> forget the <code>nvm use &lt;version&gt;</code> command there as that one <em>activates</em> the installed node version!</p>
</li>
</ul>
<h3>Extra bits you might need later</h3>
<ul>
<li>MSYS2 - or another way to get at tools like <code>wget</code> et al on Windows. Not mandatory and certainly not advised to install on a first try / first meet date with Qiqqa sources.
<ul>
<li><a href="https://www.msys2.org/#installation">https://www.msys2.org/#installation</a> - MSYS2 installer</li>
<li><a href="https://docs.microsoft.com/en-us/windows/wsl/install-win10">https://docs.microsoft.com/en-us/windows/wsl/install-win10</a> - Microsoft’s new approach: Windows Subsystem for Linux a.k.a. WSL or WSL2. Use this if you want/need something stronger than MSYS2.</li>
</ul>
</li>
</ul>


    <footer>
      © 2020 Qiqqa Contributors ::
      <a href="https://github.com/GerHobbelt/qiqqa-open-source/blob/docs-src/How to build Qiqqa from source.md">Edit this page on GitHub</a>
    </footer>
  </body>
</html>
