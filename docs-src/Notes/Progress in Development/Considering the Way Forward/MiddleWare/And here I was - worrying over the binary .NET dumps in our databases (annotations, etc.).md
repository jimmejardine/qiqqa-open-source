# And here I was ... worrying over the binary .NET dump records in our databases (annotations, etc.)

As I've found so often before, the way forward, *towards* an *answer* , is first *discovering what question to ask*.

One of the parts that irked me and made me feel like I could not safely *transition* the Qiqqa database to any other system/platform, were the binary blobs stored in these metadata databases: the `BinaryEncoder`-ed[^enc] custom annotation records, etc.: somehow I got glued to the notion that I had to do some future fancy footwork with C#/.NET code if I ever wanted to remain backwards compatible with the Qiqqa database.

Today is the day that notion can be put out to pasture: what I needed to know is all already out there! Yes, when I don't do this using .NET, I will have some coding (and testing) work cut out for me, but the binary format used by Qiqqa is *documented*.

What it required was a sabbatical of sorts and then a slower evening where I just wondered: isn't there some C++ code to deserialize the usual BinaryFormatter C#/.NET output? Because when I look at the hexdumps again, it still looks like utter crap to me.

(*Answer to self:* that's because you're looking at a generic RPC/deserialization protocol, son. Nothing wrong at the low level design, but the layers of serialization control crap and IoC fanboys having their way all on top of it, somehow seep through into the very bytes for you, *so it seems*, eh?)

Here ya go, Sunny Jim! No C/C++, but protocol documentation (and the hint somebody might have done something with it already [in *Python*](https://github.com/gurnec/Undo_FFG/blob/master/nrbf.py))



- Here's where a slightly dazed google walk led me: https://reverseengineering.stackexchange.com/questions/21107/decode-c-binary-serialization-data
- **kaboom! 🎉**
- https://docs.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-security-guide a.k.a. "Deserialization risks in use of `BinaryFormatter` and related types"
- https://github.com/pwntester/ysoserial.net (heh!)
- [# [MS-NRBF]: .NET Remoting: Binary Format Data Structure](https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-nrbf/75b9fe09-be15-475f-85b8-ae7b7558cfe5?redirectedfrom=MSDN) -- *paydirt!* This page carries all the format documentation as PDFs.
- https://stackoverflow.com/questions/3052202/how-to-analyse-contents-of-binary-serialization-stream/30176566#30176566


The weird bit that I've looked for this type of material last year at least (2021) and nothing useful like that came up out of google. 😖


[^enc]: yeah, I know `BinaryEncoder` isn't the correct name for it, but I can't be buggered right now to look up the exact name of that C# class 'n' all.
  (Edit:) it's called `BinaryFormatter`. *Anyhoo.* [*Footnotes!*](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax#footnotes)
    
