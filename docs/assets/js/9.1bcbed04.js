(window.webpackJsonp = window.webpackJsonp || []).push([
  [9],
  {
    174: function (e, t, a) {
      "use strict";
      a.r(t);
      var o = a(0),
        s = Object(o.a)(
          {},
          function () {
            var e = this,
              t = e.$createElement,
              a = e._self._c || t;
            return a(
              "ContentSlotsDistributor",
              { attrs: { "slot-key": e.$parent.slotKey } },
              [
                a(
                  "h1",
                  {
                    attrs: {
                      id:
                        "qiqqa-internals-processing-pdf-documents-text-and-the-impact-on-ui-ux",
                    },
                  },
                  [
                    a(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: {
                          href:
                            "#qiqqa-internals-processing-pdf-documents-text-and-the-impact-on-ui-ux",
                        },
                      },
                      [e._v("#")]
                    ),
                    e._v(
                      " Qiqqa Internals :: Processing PDF documents’ text and the impact on UI+UX"
                    ),
                  ]
                ),
                e._v(" "),
                a("blockquote", [
                  a("p", [
                    e._v("Rippeed off from my reply to "),
                    a(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/issues/165",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        e._v(
                          "https://github.com/jimmejardine/qiqqa-open-source/issues/165"
                        ),
                        a("OutboundLink"),
                      ],
                      1
                    ),
                  ]),
                ]),
                e._v(" "),
                a("p", [
                  e._v(
                    "Had a look at what happened exactly. It has been enlightening as I discovered I was working with a couple of internal assumptions that are clearly based on developer rather than user experience influencing my user experience."
                  ),
                ]),
                e._v(" "),
                a("h1", { attrs: { id: "what-is-going-on" } }, [
                  a(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#what-is-going-on" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" What is going on?"),
                ]),
                e._v(" "),
                a("p", [
                  e._v(
                    "When Qiqqa imports the PDF into the library, a few things happen under the hood:"
                  ),
                ]),
                e._v(" "),
                a("ul", [
                  a("li", [
                    e._v("a queued background task will attempt to "),
                    a("em", [e._v("infer")]),
                    e._v(" metadata, such as author, from the PDF metadata."),
                  ]),
                  e._v(" "),
                  a("li", [
                    e._v("other "),
                    a("em", [e._v("inferred")]),
                    e._v(" metadata, e.g. "),
                    a("em", [e._v("title")]),
                    e._v(" and "),
                    a("em", [e._v("abstract")]),
                    e._v(", is obtained by going through the "),
                    a("em", [e._v("OCR text")]),
                    e._v(" of the PDF."),
                  ]),
                ]),
                e._v(" "),
                a("p", [
                  e._v(
                    "Both of these ‘trigger’ a request to fetch the document text, i.e. the "
                  ),
                  a("em", [e._v("OCR text")]),
                  e._v("."),
                ]),
                e._v(" "),
                a("h1", { attrs: { id: "what-is-ocr-text-in-this-context" } }, [
                  a(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#what-is-ocr-text-in-this-context" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" What "),
                  a("em", [e._v("is")]),
                  e._v(" “OCR text” (in this context)?"),
                ]),
                e._v(" "),
                a("p", [
                  e._v("Qiqqa “OCR text” is the "),
                  a("em", [e._v("word text")]),
                  e._v(
                    " plus location rectangle coordinates collection extracted from the PDF by the "
                  ),
                  a("em", [e._v("OCR background process")]),
                  e._v(". Think of it as each word "),
                  a("strong", [e._v("plus")]),
                  e._v(
                    " its precise position on the page, stored in a Qiqqa proprietary ocr file format."
                  ),
                ]),
                e._v(" "),
                a(
                  "h2",
                  { attrs: { id: "how-does-qiqqa-obtain-this-ocr-text" } },
                  [
                    a(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: { href: "#how-does-qiqqa-obtain-this-ocr-text" },
                      },
                      [e._v("#")]
                    ),
                    e._v(" How does Qiqqa obtain this "),
                    a("em", [e._v("OCR text")]),
                    e._v("?"),
                  ]
                ),
                e._v(" "),
                a("p", [
                  e._v(
                    "That’s where some confusion can occur: Qiqqa has two methods to extract text from a PDF. "
                  ),
                  a("strong", [
                    e._v(
                      "It does not matter which of these methods has produced that text content"
                    ),
                  ]),
                  e._v(
                    ": either way it’s stored in the “Qiqqa OCR text cache”."
                  ),
                ]),
                e._v(" "),
                a("h3", { attrs: { id: "text-extraction" } }, [
                  a(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#text-extraction" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" Text "),
                  a("em", [e._v("Extraction")]),
                ]),
                e._v(" "),
                a("p", [
                  e._v("The "),
                  a("strong", [e._v("primary")]),
                  e._v(" method is "),
                  a("strong", [e._v("direct text extraction")]),
                  e._v(":  using the "),
                  a("code", [e._v("mupdf")]),
                  e._v(
                    " tool, Qiqqa can get the text (plus coordinates) for any PDF which has a text layer embedded."
                  ),
                ]),
                e._v(" "),
                a("p", [
                  a("strong", [
                    e._v(
                      "Your sample PDF is entirely processed by this first method, all 69 pages of it."
                    ),
                  ]),
                ]),
                e._v(" "),
                a("h3", { attrs: { id: "text-recognition" } }, [
                  a(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#text-recognition" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" Text "),
                  a("em", [e._v("Recognition")]),
                ]),
                e._v(" "),
                a("p", [
                  e._v(
                    "When the primary method fails to deliver a text for a given page, that page is then "
                  ),
                  a("em", [e._v("re-queued")]),
                  e._v(
                    " to have it OCR-ed using a Tesseract-based subprocess. This is the "
                  ),
                  a("strong", [e._v("secondary")]),
                  e._v(" method for obtaining the text of a document (page)."),
                ]),
                e._v(" "),
                a("h1", { attrs: { id: "how-does-this-impact-ux" } }, [
                  a(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#how-does-this-impact-ux" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" How does this impact UX?"),
                ]),
                e._v(" "),
                a("p", [
                  e._v("As long as Qiqqa does "),
                  a("em", [e._v("not")]),
                  e._v(" have the PDF text available in its cache, it will "),
                  a("em", [e._v("disable")]),
                  e._v(" any user activity that needs this data:"),
                ]),
                e._v(" "),
                a("ul", [
                  a("li", [
                    a("strong", [e._v("text selection")]),
                    e._v(
                      " for copy & paste, export-to-Word and similar purposes"
                    ),
                  ]),
                  e._v(" "),
                  a("li", [
                    a("strong", [e._v("highlight text")]),
                    e._v(
                      " annotating text by marking it using a selection process."
                    ),
                  ]),
                  e._v(" "),
                  a("li", [e._v("…")]),
                ]),
                e._v(" "),
                a("p", [
                  e._v(
                    "The background tasks mentioned before (inferring metadata) are "
                  ),
                  a("em", [e._v("postponed")]),
                  e._v(" until the "),
                  a("em", [e._v("OCR text")]),
                  e._v(" is available."),
                ]),
                e._v(" "),
                a("p", [
                  e._v(
                    "There a few more background tasks which have not been mentioned yet, including the one "
                  ),
                  a("em", [e._v("updating the text search index")]),
                  e._v(": that task of course requires the "),
                  a("em", [e._v("OCR text")]),
                  e._v(" as well."),
                ]),
                e._v(" "),
                a("p", [
                  e._v(
                    "From a user perspective, one can say that text searching in Qiqqa will only pick up on the new documents after "
                  ),
                  a("em", [e._v("both")]),
                  e._v(
                    " the  OCR process (methods 1 or 2, whatever it took to get some text out of those new PDFs) "
                  ),
                  a("em", [e._v("and")]),
                  e._v(
                    " the background Lucene text search indexing process have processed the new PDF documents."
                  ),
                ]),
                e._v(" "),
                a("h2", { attrs: { id: "performance" } }, [
                  a(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#performance" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" Performance"),
                ]),
                e._v(" "),
                a("p", [
                  e._v("Qiqqa "),
                  a("em", [e._v("may")]),
                  e._v(
                    " seem to be ‘slow’ in picking up new imported PDFs as the above processes all happen in the background and are currently set up to load the CPU only "
                  ),
                  a("em", [e._v("moderately")]),
                  e._v(
                    ": this was specifically done to make Qiqqa cope much better with large & "
                  ),
                  a("em", [e._v("huge")]),
                  e._v(
                    " libraries filled with technical datasheets and other PDF documents, which caused all sorts of trouble, including UI lockups and application crashes. (In commercial Qiqqa this included fatal crashes, where the application was unwilling to start up again and/or fatal loss of the text search index.)"
                  ),
                ]),
                e._v(" "),
                a("p", [
                  e._v(
                    "Yes, we still have a way to go before Qiqqa will be fast and responsive as the current drive was first to make Qiqqa stable in such a ‘large library’ environment. To make Qiqqa behave well and "
                  ),
                  a("em", [e._v("responsive")]),
                  e._v(
                    " in various environments, it will take quite some more effort."
                  ),
                ]),
                e._v(" "),
                a("h1", { attrs: { id: "now-back-on-topic" } }, [
                  a(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#now-back-on-topic" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" Now back on topic"),
                ]),
                e._v(" "),
                a("p", [
                  e._v(
                    "Now we have a description of what goes on and an observed run, I can address the issue at hand:"
                  ),
                ]),
                e._v(" "),
                a("ul", [
                  a("li", [
                    a("p", [
                      e._v(
                        "as described above, Qiqqa will take some time before it runs and completes the new document(s) text extraction and then allow text marking and selecting  actions. Up till that moment those user activities are disallowed."
                      ),
                    ]),
                    e._v(" "),
                    a("p", [
                      e._v(
                        "Hence these activities should be possible after some patience has been exercised. (Unless the PDF is one of the crappy sort, causing the “OCR” methods trouble, which is yet another chapter. 😉 )"
                      ),
                    ]),
                  ]),
                ]),
                e._v(" "),
                a("p", [
                  e._v("My initial confusion was due to me thinking in Qiqqa "),
                  a("em", [e._v("coding")]),
                  e._v(" terms: both text "),
                  a("em", [e._v("extraction")]),
                  e._v(" and "),
                  a("em", [e._v("recognition")]),
                  e._v(
                    " are filed under the single title of “OCR-ing the text”, because that’s how Qiqqa approaches this under the hood."
                  ),
                ]),
                e._v(" "),
                a("hr"),
                e._v(" "),
                a("blockquote", [
                  a("p", [
                    e._v(
                      "To complicate matters further, there’s also a couple of options to "
                    ),
                    a("em", [e._v("freeze")]),
                    e._v(" the OCR/text extraction and/or "),
                    a("em", [e._v("all background processes")]),
                    e._v(
                      ". Suffice to say those options (in the Qiqqa Tools menu and Qiqqa Configuration window) are "
                    ),
                    a("strong", [e._v("not active")]),
                    e._v(
                      " unless the user has activated them (e.g. a developer or power user testing Qiqqa or importing a large set of documents). The use of these options is out of scope."
                    ),
                  ]),
                ]),
              ]
            );
          },
          [],
          !1,
          null,
          null,
          null
        );
      t.default = s.exports;
    },
  },
]);