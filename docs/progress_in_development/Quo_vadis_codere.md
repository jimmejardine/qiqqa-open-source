<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    <h1>Quo vadis, codere?</h1>
<p>Is WPF the future?</p>
<p>What are the alternatives?</p>
<p>Chromely? <a href="http://Electron.NET">Electron.NET</a>? Electron? Will we code <em>anything</em> in .NET after all?</p>
<p>I’ve looked at several technological achievements available out there, including <a href="http://Electron.NET">Electron.NET</a> and Chromely as possible futures when I <em>really</em> want to get rid of WPF (which <a href="https://weblog.west-wind.com/posts/2019/Feb/14/WPF-Hanging-in-Infinite-Rendering-Loop#cmt_1155300">is a bear</a> and is non-portable to boot).</p>
<p>Most folks look at these from the perspective of being a .NET developer <em>first</em> and any other type of developer <em>second</em>.</p>
<p>I’m <strong>not that guy</strong>.</p>
<p>I’ve done my share of C#/.NET coding, and while the language and environment are nice (as long as you don’t mention <a href="http://ASP.NET">ASP.NET</a> or WPF around me 😉 ) there’s my preferences to consider (as I’m the main dev <em>anyway</em>):</p>
<ul>
<li>
<p>C/C++ is my old love. As long as you don’t go Boost-nutso, I’m happy there.</p>
</li>
<li>
<p>JavaScript is my second base. It lacked a type system, so, compared to C/C++/C#, it has invited in old <em>bug group</em> that I previously thought addressed adequately in the '70s already, but with the prominence of TypeScript I now have compile-time type checking back again, where and when I want it, plus a GC (Garbage Collector) that’s seen a devastating amount of energy poured into it to optimize the tek, so where does that leave C#, for <em>me</em>?</p>
<p>C# is still beautiful to me. If I get to choose, I’d prefer it above C++ or Java for the ‘business logic’ layer: C# is C++ without the <em>boost templates accolites</em>. Plus GC. Yay!</p>
</li>
<li>
<p>I’m of the sort that has regretted that MFC (Microsoft Foundation Classes) wasn’t cross-platform. Not because MFC is great (<em>it isn’t</em>), but because it’s close enough to the <em>metal</em> of driving a OS-level UI core that I liked it. Because MFC was <em>hackable</em> and, yes, it came with full source code – thanks to a steady stream of MSDN CDs.</p>
<p>C# brought us WinForms, which is, to me, MFC in disguise. Non-portable again. <em>Sans source code</em>. Sigh.</p>
<p>Then there’s all the “<em>alternatives</em>” everyone has time and again brought to my attention:</p>
</li>
</ul>
<ul>
<li>
<p>Qt (in one word: <em>yuck</em> – mostly because what I’ve seen done on Windows multiple times where the interfaces are often this side of crappy looking, while the good-looking ones invariably turned out to be highly complex endeavours when you lift the hood and look at the code <em>for real</em>. Sounds so much like trying to be <em>better</em> than MFC / WinForms but still carries over enough <em>smell</em> from it’s X-Windows days that it gives me goosebumps. Yes, coding and engineering is <em>emotion</em> too, not just <em>ratio</em>. You love it, I’ll go sit somewhere else if it bothers you.</p>
</li>
<li>
<p>Mono (<em>come again</em>? That’s <em>not</em> a GUI system, just .NET <em>stdlib</em> done for UNIX. All of you who ever mentioned Mono… made me understand women so much better: <em>men don’t listen</em>. Indeed.)</p>
</li>
<li>
<p>plenty “wrappers” which map to “native UI systems” on multiple platforms. The fact I forget their names as soon as I leave their web pages, manuals or header files behind may tell you something. Used several, nothing but trouble.</p>
<p>I’m a curious individual and don’t <em>need a reason or a purpose</em> to look at stuff and spend time with it, but the conclusion after a bunch of decades working with “cross-platform GUI systems” that attempt to “encapsulate” (lemme call that a “wrapper”, 'kay?) native UI systems on multiple platforms… well, it’s too much hassle for the learning curve <em>every time</em>.</p>
<p>For basic applications, form filling and that sort of corporate/accounting stuff, it doesn’t matter as it’s all fine, but as soon as you’re looking at <em>design</em> and <em>graphics</em>, or <em>smoothness</em> re interaction, anything fancy really, then you end up with a cactus up your spinxter. Or two. And since I’m not into BDSM, guess how that makes me feel. <strong>Next!</strong></p>
</li>
</ul>
<p>Notice how this rant hasn’t mentioned XAML yet? Or, eh… <em>Java</em>?!</p>
<p>Okay, let’s deal with Java <em>swiftly</em>. Except some <em>notable</em> exceptions (ANTLR! <em>Beautiful!</em> But no GUI, only CLI), my close encounters with Java invariably landed me in a land where a particular type of corporate IT onany sprang into my face like the Aliens jumping from their eggs (you know the movie; if you don’t, <em>get a life</em>. Then watch the sequels. Those crawlers. That’s Java. Not so much the language, but the scaffolding. Application configuration? Oh, Swift is <em>great</em>. OMG. log4net is <em>nice</em>, but give me UNIX <code>syslog</code> any day. Regrettably, Windows facilitates this kind of beancounter’s splurging on outer planets IT “engineering” by having the EventLog and, Gods <em>forbid</em>, anything that’s <code>grep</code>pable without spending additional $$$. So people feel the way is blowing and then come up with similarly convoluted “fine grained access control” systems like, well, Swift, log4net, etc.  All of which make Java programming <em>no fun at all</em>. (Hm, maybe <em>that</em> was the purpose. Silly me. Hadn’t thought about tit that way before. It’s been said to my face: “we don’t pay y’all to have <em>fun</em>!” What was that about males and <em>not listening</em> again? Checks Berkeley Breathed pinup.</p>

  </head>
  <body>

    <h1>Quo vadis, codere?</h1>
<p>Is WPF the future?</p>
<p>What are the alternatives?</p>
<p>Chromely? <a href="http://Electron.NET">Electron.NET</a>? Electron? Will we code <em>anything</em> in .NET after all?</p>
<p>I’ve looked at several technological achievements available out there, including <a href="http://Electron.NET">Electron.NET</a> and Chromely as possible futures when I <em>really</em> want to get rid of WPF (which <a href="https://weblog.west-wind.com/posts/2019/Feb/14/WPF-Hanging-in-Infinite-Rendering-Loop#cmt_1155300">is a bear</a> and is non-portable to boot).</p>
<p>Most folks look at these from the perspective of being a .NET developer <em>first</em> and any other type of developer <em>second</em>.</p>
<p>I’m <strong>not that guy</strong>.</p>
<p>I’ve done my share of C#/.NET coding, and while the language and environment are nice (as long as you don’t mention <a href="http://ASP.NET">ASP.NET</a> or WPF around me 😉 ) there’s my preferences to consider (as I’m the main dev <em>anyway</em>):</p>
<ul>
<li>
<p>C/C++ is my old love. As long as you don’t go Boost-nutso, I’m happy there.</p>
</li>
<li>
<p>JavaScript is my second base. It lacked a type system, so, compared to C/C++/C#, it has invited in old <em>bug group</em> that I previously thought addressed adequately in the '70s already, but with the prominence of TypeScript I now have compile-time type checking back again, where and when I want it, plus a GC (Garbage Collector) that’s seen a devastating amount of energy poured into it to optimize the tek, so where does that leave C#, for <em>me</em>?</p>
<p>C# is still beautiful to me. If I get to choose, I’d prefer it above C++ or Java for the ‘business logic’ layer: C# is C++ without the <em>boost templates accolites</em>. Plus GC. Yay!</p>
</li>
<li>
<p>I’m of the sort that has regretted that MFC (Microsoft Foundation Classes) wasn’t cross-platform. Not because MFC is great (<em>it isn’t</em>), but because it’s close enough to the <em>metal</em> of driving a OS-level UI core that I liked it. Because MFC was <em>hackable</em> and, yes, it came with full source code – thanks to a steady stream of MSDN CDs.</p>
<p>C# brought us WinForms, which is, to me, MFC in disguise. Non-portable again. <em>Sans source code</em>. Sigh.</p>
<p>Then there’s all the “<em>alternatives</em>” everyone has time and again brought to my attention:</p>
</li>
</ul>
<ul>
<li>
<p>Qt (in one word: <em>yuck</em> – mostly because what I’ve seen done on Windows multiple times where the interfaces are often this side of crappy looking, while the good-looking ones invariably turned out to be highly complex endeavours when you lift the hood and look at the code <em>for real</em>. Sounds so much like trying to be <em>better</em> than MFC / WinForms but still carries over enough <em>smell</em> from it’s X-Windows days that it gives me goosebumps. Yes, coding and engineering is <em>emotion</em> too, not just <em>ratio</em>. You love it, I’ll go sit somewhere else if it bothers you.</p>
</li>
<li>
<p>Mono (<em>come again</em>? That’s <em>not</em> a GUI system, just .NET <em>stdlib</em> done for UNIX. All of you who ever mentioned Mono… made me understand women so much better: <em>men don’t listen</em>. Indeed.)</p>
</li>
<li>
<p>plenty “wrappers” which map to “native UI systems” on multiple platforms. The fact I forget their names as soon as I leave their web pages, manuals or header files behind may tell you something. Used several, nothing but trouble.</p>
<p>I’m a curious individual and don’t <em>need a reason or a purpose</em> to look at stuff and spend time with it, but the conclusion after a bunch of decades working with “cross-platform GUI systems” that attempt to “encapsulate” (lemme call that a “wrapper”, 'kay?) native UI systems on multiple platforms… well, it’s too much hassle for the learning curve <em>every time</em>.</p>
<p>For basic applications, form filling and that sort of corporate/accounting stuff, it doesn’t matter as it’s all fine, but as soon as you’re looking at <em>design</em> and <em>graphics</em>, or <em>smoothness</em> re interaction, anything fancy really, then you end up with a cactus up your spinxter. Or two. And since I’m not into BDSM, guess how that makes me feel. <strong>Next!</strong></p>
</li>
</ul>
<p>Notice how this rant hasn’t mentioned XAML yet? Or, eh… <em>Java</em>?!</p>
<p>Okay, let’s deal with Java <em>swiftly</em>. Except some <em>notable</em> exceptions (ANTLR! <em>Beautiful!</em> But no GUI, only CLI), my close encounters with Java invariably landed me in a land where a particular type of corporate IT onany sprang into my face like the Aliens jumping from their eggs (you know the movie; if you don’t, <em>get a life</em>. Then watch the sequels. Those crawlers. That’s Java. Not so much the language, but the scaffolding. Application configuration? Oh, Swift is <em>great</em>. OMG. log4net is <em>nice</em>, but give me UNIX <code>syslog</code> any day. Regrettably, Windows facilitates this kind of beancounter’s splurging on outer planets IT “engineering” by having the EventLog and, Gods <em>forbid</em>, anything that’s <code>grep</code>pable without spending additional $$$. So people feel the way is blowing and then come up with similarly convoluted “fine grained access control” systems like, well, Swift, log4net, etc.  All of which make Java programming <em>no fun at all</em>. (Hm, maybe <em>that</em> was the purpose. Silly me. Hadn’t thought about tit that way before. It’s been said to my face: “we don’t pay y’all to have <em>fun</em>!” What was that about males and <em>not listening</em> again? Checks Berkeley Breathed pinup.</p>


    <footer>
      © 2020 Qiqqa Contributors ::
      <a href="https://github.com/GerHobbelt/qiqqa-open-source/blob/docs-src/Progress in Development/Quo vadis, codere.md">Edit this page on GitHub</a>
    </footer>
  </body>
</html>
