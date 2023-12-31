# Multiprocessing and IPC (Inter-Process Communications)

Qiqqa is a document research tool which combines several processes to deliver a toolbox which allows you to collect, read, search, cite/reference, evaluate, categorize, index, annotate, rate, share and perform any kind of meta-research on your documents, gathered in your libraries.

> Aside: “tasks” vs. “processes”: while these words focus on different aspects of the same *work* aiming for a specific goal, I may come across as a tad sloppy with these terms, using them apparently interchangeably, while the point of view shifts and rotates through the text as we consider the processes required to accomplish our goals. I reserve my severest rigor for systems (risk) analyses and software development, while I don't mind taking a few *liberties* when writing about systems.
> If you like a different approach or maybe wish the text would read a little differently, you can simply fork the repo, edit the text(s) and file a pull request to have your version replace the current one. Feedback is appreciated.

As described elsewhere, Qiqqa includes at least these major *user* processes:

1. document gathering / collecting
2. document reading & annotating
3. content and metadata searching (including “meta-research”)
4. document *indexing*: categorizing, vetting and correcting/augmenting any and all metadata
5. sharing & merging your libraries with others (you accessing the data on multiple sites or in collaborative efforts)

These are all *user* processes (or “actions”), which are enabled and empowered by machine/software processes included in the Qiqqa tool. Here we list these processes as one set for each user process:

1. document gathering:
    - Qiqqa Sniffer, where you can scour the Internet in search of both new documents and help fill in the blanks in your metadata through downloading/importing bibliography records in BibTeX / XML / etc. formats.
    - Watch Folder, where Qiqqa monitors a given set of directories, looking for any new documents you may drop in there.
    - Import Document
2. document reading & annotating:
   - rendering (PDF) documents on screen for reading (“viewing”)
   - providing a UI for the user to annotate / mark up / edit the documents while these are read (“annotation”).
   - providing a UI for quoting / copying from the document at hand (“copying”)
      
      > While one can mark/select text extracted from a document and copy it anywhere using the Windows clipboard with current Qiqqa software releases, the ability to mark and copy both text and *images* in the document text and transport these to other applications is still *future music*.
      >
      > See [[../Document OCR & Text Extraction/Unexpected Hurdles Producing Decent Text And Images From PDF Documents]] when you are interested in the nitty gritty surrounding this particular subject.
      
- content and metadata searching (including “meta-research”)

   For *searching* to produce any results, we need to provide a “search index” (which is a fully automated process, quite different and only sideways related to the *user task* of "document indexing”: the latter represents the job of a *librarian* rather, while the former is all about facilitating the single task of *searching* the document library, google-like, via a on-site, local, *search engine*.
   For that to work, we need to ensure each document can be represented as (searchable) *text*, hence this set of machine processes is required:
   
   - document content text and metadata extraction, delivering the document and its metadata in a usable text format, feeding it to the *search engine*.
       - this includes an optional (*enforceable*) OCR task when the document does not contain usable text yet as it *may* be an *images*/*scans* based document.
   - updating / maintaining the *search index* in the local *search engine*.
   
      > *Local* as this functionality does not want to depend on Google, Bing or other on-line-only search services: Qiqqa is engineered to be a *stand-alone*, *employ anywhere, anytime* application, which doesn't require an Internet connection for it to work – except for some very specific Internet-facing activities such as Qiqqa Sniffer, of course.

   - perusing the local *search engine* to find matching documents and rank those by estimated *relevance*, akin to google / bing / duckduckgo / etc.

- document *indexing*: categorizing, vetting and correcting/augmenting any and all metadata
- sharing & merging your libraries with others (you accessing the data on multiple sites or in collaborative efforts)






# References

- [SO :: C# read binary data from socket](https://stackoverflow.com/questions/3701637/c-sharp-read-binary-data-from-socket)
- [ZeroMQ](https://zeromq.org/) and other network messaging low-level protocol stuff
    - [ClrZMQ4: .NET wrapper for `libzmq`](https://github.com/zeromq/clrzmq4)
    - [NetMQ4: ZeroMQ done in .NET](https://github.com/zeromq/netmq)
        - didn’t run reliably for me, while `clrzmq4` delivered immediately. `netmq4` MAY be the advised way to go by the folks over at ZeroMQ, but I don’t find the current state of affairs encouraging, having attempted builds and running their test set across multiple commits, including a tagged release. It may be me, but I have have the `clrmzq4` option ready to go...
    - [Sasha’s Blog :: ZeroMQ #6 : Divide And Conquer](https://sachabarbs.wordpress.com/2014/09/01/zeromq-6-divide-and-conquer/)
    - [Sasha’s Blog :: ZeroMQ #2 : The Socket Types](https://sachabarbs.wordpress.com/2014/08/21/zeromq-2-the-socket-types-2/)
    - [SO :: NetMQ vs clrzmq](https://stackoverflow.com/questions/38682886/netmq-vs-clrzmq)
    - [clrzmq4 :: Problem with polling #73](https://github.com/zeromq/clrzmq4/issues/73)
    - [RFC :: ZeroMQ Message Transport Protocol](https://rfc.zeromq.org/spec/23/)
    - [SO :: Fastest way to serialize and deserialize .NET objects](https://stackoverflow.com/questions/4143421/fastest-way-to-serialize-and-deserialize-net-objects)
    - [Serialization Performance comparison (C#/.NET) – Formats & Frameworks (XML–DataContractSerializer & XmlSerializer, BinaryFormatter, JSON– Newtonsoft & ServiceStack.Text, Protobuf, MsgPack)](https://maxondev.com/serialization-performance-comparison-c-net-formats-frameworks-xmldatacontractserializer-xmlserializer-binaryformatter-json-newtonsoft-servicestack-text/)
    - [binaryformatter vs binaron serializer by Zach Saw](https://dotnetfiddle.net/gOqQ7p)
    - [FlatBuffers :: Writing a Schema](https://google.github.io/flatbuffers/flatbuffers_guide_writing_schema.html)
    - [FlatBuffers :: C++ Benchmarks](https://google.github.io/flatbuffers/flatbuffers_benchmarks.html)
    - [Lessons Learned Coding Flatbuffers in JavaScript](http://blog.misterblue.com/programming/notes/Javascript-and-Flatbuffers.html)
 - [Determining Whether a Directory Is a Mounted Folder](https://docs.microsoft.com/en-us/windows/win32/fileio/determining-whether-a-directory-is-a-volume-mount-point?redirectedfrom=MSDN)
 - [MvsSln :: parsing and processing MSVC solution files](https://github.com/3F/MvsSln)
 - [SQLite :: Modern C++](https://github.com/SqliteModernCpp/sqlite_modern_cpp)
 - [This is how you do CMake](https://pabloariasal.github.io/2018/02/19/its-time-to-do-cmake-right/)
 - [.NET preprocessor directives and defines](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives)
- [Obtaining Directory Change Notifications](https://docs.microsoft.com/en-us/windows/win32/fileio/obtaining-directory-change-notifications?redirectedfrom=MSDN)

