(window.webpackJsonp = window.webpackJsonp || []).push([
  [7],
  {
    165: function (e, t, a) {
      e.exports = a.p + "assets/img/divider-end.87ee.svg";
    },
    182: function (e, t, a) {
      "use strict";
      a.r(t);
      var r = a(0),
        n = Object(r.a)(
          {},
          function () {
            var e = this,
              t = e.$createElement,
              r = e._self._c || t;
            return r(
              "ContentSlotsDistributor",
              { attrs: { "slot-key": e.$parent.slotKey } },
              [
                r(
                  "h1",
                  {
                    attrs: {
                      id:
                        "qiqqa-internals-extracting-the-text-from-pdf-documents",
                    },
                  },
                  [
                    r(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: {
                          href:
                            "#qiqqa-internals-extracting-the-text-from-pdf-documents",
                        },
                      },
                      [e._v("#")]
                    ),
                    e._v(
                      " Qiqqa Internals :: Extracting the text from PDF documents"
                    ),
                  ]
                ),
                e._v(" "),
                r("h1", { attrs: { id: "the-qiqqa-ocr-background-process" } }, [
                  r(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#the-qiqqa-ocr-background-process" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" The Qiqqa OCR "),
                  r("em", [e._v("background")]),
                  e._v(" process"),
                ]),
                e._v(" "),
                r("title-runner", [e._v("(as per 2020-03-22)")]),
                e._v(" "),
                r("p", [
                  e._v(
                    "Before we dive in, there’s one important question to ask:"
                  ),
                ]),
                e._v(" "),
                r(
                  "h2",
                  {
                    attrs: { id: "given-a-pdf-what-does-qiqqa-store-on-disk" },
                  },
                  [
                    r(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: {
                          href: "#given-a-pdf-what-does-qiqqa-store-on-disk",
                        },
                      },
                      [e._v("#")]
                    ),
                    e._v(" Given a PDF, "),
                    r("em", [e._v("what")]),
                    e._v(" does Qiqqa store on disk?"),
                  ]
                ),
                e._v(" "),
                r("h3", { attrs: { id: "tl-dr" } }, [
                  r(
                    "a",
                    { staticClass: "header-anchor", attrs: { href: "#tl-dr" } },
                    [e._v("#")]
                  ),
                  e._v(" TL;DR"),
                ]),
                e._v(" "),
                r("ol", [
                  r("li", [
                    e._v("the original PDF, "),
                    r("em", [e._v("renamed")]),
                    e._v(" using the "),
                    r("em", [e._v("content hash")]),
                    e._v("."),
                  ]),
                  e._v(" "),
                  r("li", [
                    e._v(
                      "the extracted text (as a series of words plus box coordinates in a propietary text format)"
                    ),
                  ]),
                ]),
                e._v(" "),
                r(
                  "h3",
                  { attrs: { id: "☞-the-long-answer-to-this-question-🙉🎉" } },
                  [
                    r(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: {
                          href: "#☞-the-long-answer-to-this-question-🙉🎉",
                        },
                      },
                      [e._v("#")]
                    ),
                    e._v(" ☞ The long answer to this question 🙉🎉"),
                  ]
                ),
                e._v(" "),
                r("details", [
                  r("summary", [r("b", [e._v("(Click here to unfold)")])]),
                  e._v(" "),
                  r("br"),
                  e._v(" "),
                  r("blockquote", [
                    r(
                      "h4",
                      {
                        attrs: {
                          id: "does-it-matter-where-the-pdf-is-coming-from",
                        },
                      },
                      [
                        r(
                          "a",
                          {
                            staticClass: "header-anchor",
                            attrs: {
                              href:
                                "#does-it-matter-where-the-pdf-is-coming-from",
                            },
                          },
                          [e._v("#")]
                        ),
                        e._v(" Does it matter where the PDF is coming from?"),
                      ]
                    ),
                    e._v(" "),
                    r("p", [
                      e._v("It does not matter "),
                      r("em", [e._v("how")]),
                      e._v(
                        " Qiqqa obtained the incoming PDF document, be it by “watch folder” directory scanning, website sniffer download, drag&drop or other means to import: all incoming PDFs are processed the same way."
                      ),
                    ]),
                    e._v(" "),
                    r("p", [
                      e._v("Some "),
                      r("strong", [e._v("metadata")]),
                      e._v(
                        " bits may be different: a source URL may be saved on Sniffer download or alike, but that’s about it."
                      ),
                    ]),
                  ]),
                  e._v(" "),
                  r("ul", [
                    r("li", [
                      r("p", [
                        e._v("The incoming "),
                        r("strong", [e._v("original PDF")]),
                        e._v(" is copied to the Qiqqa Library "),
                        r("strong", [e._v("document store")]),
                        e._v(", which is located in the "),
                        r("code", [e._v("<LibraryID>/documents/")]),
                        e._v(" directory tree."),
                      ]),
                      e._v(" "),
                      r("p", [
                        e._v("The PDF "),
                        r("strong", [e._v("content")]),
                        e._v(" is hashed (using a "),
                        r(
                          "a",
                          {
                            attrs: {
                              href:
                                "https://github.com/jimmejardine/qiqqa-open-source/blob/0b015c923e965ba61e3f6b51218ca509fcd6cabb/Utilities/Files/StreamFingerprint.cs#L14",
                              target: "_blank",
                              rel: "noopener noreferrer",
                            },
                          },
                          [e._v("SHA1 derivative"), r("OutboundLink")],
                          1
                        ),
                        e._v(
                          ") to produce a unique identifier for this particular PDF "
                        ),
                        r("strong", [e._v("content")]),
                        e._v(
                          ". That hash is used throughout Qiqqa for indexing "
                        ),
                        r("em", [e._v("and")]),
                        e._v(" is to "),
                        r("em", [e._v("name")]),
                        e._v(
                          " the cached version of the incoming PDF, using a simple yet effective distribution scheme to help NTFS/filesystem performance for large libraries: the first character of the hash is also used as a "
                        ),
                        r("em", [e._v("subdirectory")]),
                        e._v(" name."),
                      ]),
                      e._v(" "),
                      r("p", [
                        e._v("Example path for a PDF file stored in the "),
                        r("code", [e._v("Guest")]),
                        e._v(" Qiqqa Library:"),
                      ]),
                      e._v(" "),
                      r("div", { staticClass: "language- line-numbers-mode" }, [
                        r(
                          "pre",
                          { pre: !0, attrs: { class: "language-text" } },
                          [
                            r("code", [
                              e._v(
                                "  base/Guest/documents/D/DA7B8FDA82E6D7465ADC7590EEC0C914E955C5B8.pdf\n"
                              ),
                            ]),
                          ]
                        ),
                        e._v(" "),
                        r("div", { staticClass: "line-numbers-wrapper" }, [
                          r("span", { staticClass: "line-number" }, [
                            e._v("1"),
                          ]),
                          r("br"),
                        ]),
                      ]),
                    ]),
                    e._v(" "),
                    r("li", [
                      r("p", [
                        e._v("The "),
                        r("strong", [e._v("extracted text")]),
                        e._v(" is saved in a Qiqqa-global store at "),
                        r("code", [e._v("base/ocr/")]),
                        e._v(
                          " using a similar filesystem performance scheme as for the PDF  file itself."
                        ),
                      ]),
                      e._v(" "),
                      r("p", [
                        e._v(
                          "Example paths for the OCR output cached for the same PDF file as shown above:"
                        ),
                      ]),
                      e._v(" "),
                      r("div", { staticClass: "language- line-numbers-mode" }, [
                        r(
                          "pre",
                          { pre: !0, attrs: { class: "language-text" } },
                          [
                            r("code", [
                              e._v(
                                "  base/ocr/DA/DA7B8FDA82E6D7465ADC7590EEC0C914E955C5B8.pagecount.0.txt\n  base/ocr/DA/DA7B8FDA82E6D7465ADC7590EEC0C914E955C5B8.text.4.txt\n  base/ocr/DA/DA7B8FDA82E6D7465ADC7590EEC0C914E955C5B8.textgroup.001_to_020.txt\n  base/ocr/DA/DA7B8FDA82E6D7465ADC7590EEC0C914E955C5B8.textgroup.021_to_040.txt\n"
                              ),
                            ]),
                          ]
                        ),
                        e._v(" "),
                        r("div", { staticClass: "line-numbers-wrapper" }, [
                          r("span", { staticClass: "line-number" }, [
                            e._v("1"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("2"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("3"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("4"),
                          ]),
                          r("br"),
                        ]),
                      ]),
                      r("blockquote", [
                        r("p", [
                          e._v(
                            "Note that in this example, we apparently had a PDF which had its page 4 OCRed using "
                          ),
                          r("code", [e._v("tesseract")]),
                          e._v(" (a.k.a. the "),
                          r("strong", [e._v("SINGLE")]),
                          e._v(
                            " process), while the other 20+ pages got extracted using "
                          ),
                          r("code", [e._v("mupdf")]),
                          e._v(" (a.k.a. the "),
                          r("strong", [e._v("GROUP")]),
                          e._v(
                            " process): apparently the given PDF was a text-based PDF which "
                          ),
                          r("em", [e._v("possibly")]),
                          e._v(
                            " an empty page or a full-page graphic without embedded text on page 4."
                          ),
                        ]),
                        e._v(" "),
                        r("p", [
                          e._v(
                            "See the process description below for more info."
                          ),
                        ]),
                      ]),
                      e._v(" "),
                      r("p", [
                        e._v("The "),
                        r("strong", [e._v("TEXT DATA")]),
                        e._v(
                          " stored in these ‘ocr’ files uses a custom text format, where each word is listed on a separate line and accompanied by a set of coordinates describing the rectangle of its location within the page."
                        ),
                      ]),
                      e._v(" "),
                      r("p", [e._v("Example OCR text file snippet:")]),
                      e._v(" "),
                      r("div", { staticClass: "language- line-numbers-mode" }, [
                        r(
                          "pre",
                          { pre: !0, attrs: { class: "language-text" } },
                          [
                            r("code", [
                              e._v(
                                "  # Generated by: QiqqaOCR.\n  # Version: 3\n  # List source: PDFText\n  # System culture: en-US\n  @PAGE: 1\n\n  0.62114,0.04798,0.11382,0.01641:USOO695.2431B1\n\n  0.12683,0.08586,0.02602,0.02904:(12)\n\n  0.15935,0.08586,0.08455,0.02904:United\n\n  0.25366,0.08586,0.07480,0.02904:States\n\n  0.33984,0.08586,0.07967,0.02904:Patent\n\n  0.52683,0.08586,0.02602,0.02904:(10)\n\n  0.55935,0.08586,0.05528,0.02904:Patent\n\n  0.62114,0.08586,0.03415,0.02904:No.:\n\n  0.69593,0.08586,0.03089,0.02904:US\n\n  0.73333,0.08586,0.09106,0.02904:6,952,431\n\n  0.83252,0.08586,0.02602,0.02904:B1\n\n  0.15772,0.10732,0.04553,0.02399:Dally\n\n  0.20813,0.10732,0.01626,0.02399:et\n\n  0.22927,0.10732,0.02276,0.02399:al.\n\n  0.52683,0.10732,0.02602,0.02399:(45)\n\n  0.55935,0.10732,0.03902,0.02399:Date\n\n  0.60325,0.10732,0.02114,0.02399:of\n\n  0.62764,0.10732,0.05854,0.02399:Patent:\n\n  0.75772,0.10732,0.03740,0.02399:Oct.\n\n  0.79837,0.10732,0.01626,0.02399:4,\n\n  0.81789,0.10732,0.03902,0.02399:2005\n\n  0.12683,0.14899,0.02602,0.01641:(54)\n\n  0.16585,0.14899,0.05528,0.01641:CLOCK\n\n  0.22439,0.14899,0.10569,0.01641:MULTIPLYING\n\n  0.33333,0.14899,0.11707,0.01641:DELAY-LOCKED\n\n  0.53821,0.14899,0.05366,0.01641:6,037,812\n\n  0.59675,0.14899,0.01138,0.01641:A\n\n  0.63577,0.14899,0.03740,0.01641:3/2000\n\n  0.68293,0.14899,0.03902,0.01641:Gaudet\n\n  0.72683,0.14899,0.08455,0.01641:.......................\n\n  0.81463,0.14899,0.04390,0.01641:327/116\n\n  0.16748,0.16035,0.04228,0.01641:LOOP\n\n  0.21301,0.16035,0.03089,0.01641:FOR\n\n  0.24878,0.16035,0.03902,0.01641:DATA\n\n  0.29106,0.16035,0.14146,0.01641:COMMUNICATIONS\n\n  0.53821,0.16035,0.05366,0.01641:6,043,717\n\n  0.59675,0.16035,0.01138,0.01641:A\n\n  0.63577,0.16035,0.03740,0.01641:3/2000\n\n  0.68293,0.16035,0.02764,0.01641:Kurd\n\n  0.71545,0.16035,0.09919,0.01641:...........................\n\n  0.82114,0.16035,0.01463,0.01641:33\n"
                              ),
                            ]),
                          ]
                        ),
                        e._v(" "),
                        r("div", { staticClass: "line-numbers-wrapper" }, [
                          r("span", { staticClass: "line-number" }, [
                            e._v("1"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("2"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("3"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("4"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("5"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("6"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("7"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("8"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("9"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("10"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("11"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("12"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("13"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("14"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("15"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("16"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("17"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("18"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("19"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("20"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("21"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("22"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("23"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("24"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("25"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("26"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("27"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("28"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("29"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("30"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("31"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("32"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("33"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("34"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("35"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("36"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("37"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("38"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("39"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("40"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("41"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("42"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("43"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("44"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("45"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("46"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("47"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("48"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("49"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("50"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("51"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("52"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("53"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("54"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("55"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("56"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("57"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("58"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("59"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("60"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("61"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("62"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("63"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("64"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("65"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("66"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("67"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("68"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("69"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("70"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("71"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("72"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("73"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("74"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("75"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("76"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("77"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("78"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("79"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("80"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("81"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("82"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("83"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("84"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("85"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("86"),
                          ]),
                          r("br"),
                          r("span", { staticClass: "line-number" }, [
                            e._v("87"),
                          ]),
                          r("br"),
                        ]),
                      ]),
                      r("p", [
                        e._v(
                          "As you can already see, a ‘word’ here is not always in accordance of the human purview of the meaning of ‘word’, e.g. the ‘word’ "
                        ),
                        r("code", [e._v("...........................")]),
                        e._v(" at the end of the snippet there."),
                      ]),
                      e._v(" "),
                      r("p", [
                        e._v("Qiqqa "),
                        r(
                          "a",
                          {
                            attrs: {
                              href:
                                "https://github.com/jimmejardine/qiqqa-open-source/blob/1ef3403788d2b2d5efcc08dc244a60d1694f5453/Qiqqa/DocumentLibrary/DocumentLibraryIndex/LibraryIndex.cs#L629-L638",
                              target: "_blank",
                              rel: "noopener noreferrer",
                            },
                          },
                          [
                            e._v("applies a few filters to this data"),
                            r("OutboundLink"),
                          ],
                          1
                        ),
                        e._v(" before it is injected into the "),
                        r("code", [e._v("Lucene")]),
                        e._v(" search index database."),
                      ]),
                    ]),
                  ]),
                ]),
                e._v(" "),
                r("h2", { attrs: { id: "the-qiqqa-ocr-internal-workflow" } }, [
                  r(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#the-qiqqa-ocr-internal-workflow" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" The Qiqqa OCR internal workflow"),
                ]),
                e._v(" "),
                r("h3", { attrs: { id: "tl-dr-2" } }, [
                  r(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#tl-dr-2" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" TL;DR"),
                ]),
                e._v(" "),
                r("ol", [
                  r("li", [
                    e._v("background process Stage 1: "),
                    r("code", [e._v("mupdf")]),
                    e._v(" — extract text from PDF.\n"),
                    r("br"),
                    e._v("\nGo to next step when you fail."),
                  ]),
                  e._v(" "),
                  r("li", [
                    e._v("background process Stage 2: "),
                    r("code", [e._v("tesseract")]),
                    e._v("/OCR — extract text from PDF "),
                    r("em", [e._v("page images")]),
                    e._v(".\n"),
                    r("br"),
                    e._v("\nGo to next step when you fail."),
                  ]),
                  e._v(" "),
                  r("li", [
                    e._v("v80 and before: give it the run-around. For ever.\n"),
                    r("br"),
                    e._v("\nv82+: Fake it and "),
                    r("em", [e._v("shut up")]),
                    e._v(" until we "),
                    r("em", [e._v("improve")]),
                    e._v("."),
                  ]),
                ]),
                e._v(" "),
                r("p", [
                  e._v("Other Qiqqa (background) processes "),
                  r("em", [e._v("will")]),
                  e._v(
                    " impact OCR activity: the Lucene text search index and metadata inference systems "
                  ),
                  r("em", [e._v("want")]),
                  e._v(" OCR data and don’t stop until they "),
                  r("em", [e._v("do")]),
                  e._v("."),
                ]),
                e._v(" "),
                r("h3", { attrs: { id: "tl-dr-☞-🙥-the-whole-story-🙧-🙉🎉" } }, [
                  r(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#tl-dr-☞-🙥-the-whole-story-🙧-🙉🎉" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" "),
                  r("s", [e._v("TL;DR")]),
                  e._v("            ☞ 🙥—— The whole story ——🙧 🙉🎉"),
                ]),
                e._v(" "),
                r("details", [
                  r("summary", [r("b", [e._v("(Click here to unfold)")])]),
                  e._v(" "),
                  r("br"),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "Once the background task gets around to it, the PDF is OCRed if this has not happened yet.\nThis is generally detected by checking whether the expected OCR data for page 1 is available."
                    ),
                  ]),
                  e._v(" "),
                  r("blockquote", [
                    r("p", [
                      e._v("The correct(er) answer here is: "),
                      r("em", [e._v("it depends")]),
                      e._v(
                        ": several conditions exist (e.g. when the document is viewed by the user in a Qiqqa panel) when "
                      ),
                      r("em", [e._v("all pages")]),
                      e._v(
                        " of the document are requested and any of them missing will (re)trigger the OCR process."
                      ),
                    ]),
                    e._v(" "),
                    r("p", [
                      e._v("See all the invocations of "),
                      r(
                        "a",
                        {
                          attrs: {
                            href:
                              "https://github.com/jimmejardine/qiqqa-open-source/blob/1ef3403788d2b2d5efcc08dc244a60d1694f5453/Qiqqa/Documents/PDF/PDFRendering/PDFRenderer.cs#L98",
                            target: "_blank",
                            rel: "noopener noreferrer",
                          },
                        },
                        [
                          e._v("the "),
                          r("code", [e._v("GetOCRText()")]),
                          e._v(" method"),
                          r("OutboundLink"),
                        ],
                        1
                      ),
                      e._v(" in the Qiqqa source code."),
                    ]),
                  ]),
                  e._v(" "),
                  r(
                    "h3",
                    {
                      attrs: {
                        id:
                          "qiqqa-ocr-stage-1-the-extract-attempt-the-group-call",
                      },
                    },
                    [
                      r(
                        "a",
                        {
                          staticClass: "header-anchor",
                          attrs: {
                            href:
                              "#qiqqa-ocr-stage-1-the-extract-attempt-the-group-call",
                          },
                        },
                        [e._v("#")]
                      ),
                      e._v(" Qiqqa OCR Stage 1: The Extract Attempt (= "),
                      r(
                        "a",
                        {
                          attrs: {
                            href:
                              "https://github.com/jimmejardine/qiqqa-open-source/blob/a50888e836224e1d293457c8cd9a59cfef403bf7/Qiqqa/Documents/PDF/PDFRendering/PDFTextExtractor.cs#L652",
                            target: "_blank",
                            rel: "noopener noreferrer",
                          },
                        },
                        [
                          e._v("the "),
                          r("code", [e._v('"GROUP"')]),
                          e._v(" call"),
                          r("OutboundLink"),
                        ],
                        1
                      ),
                      e._v(")"),
                    ]
                  ),
                  e._v(" "),
                  r("p", [
                    e._v("First, Qiqqa attempts to "),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/1ef3403788d2b2d5efcc08dc244a60d1694f5453/QiqqaOCR/TextExtractEngine.cs#L178",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        e._v(
                          "extract text from the PDF without OCR-ing it, using the "
                        ),
                        r("code", [e._v("mupdf")]),
                        e._v(" tool"),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                    e._v(
                      ": this should deliver for all PDFs which are not ‘page image based’."
                    ),
                  ]),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "The text data collected this way is stored in proprietary format text files, up to  20 pages per file, in the "
                    ),
                    r("code", [e._v("ocr")]),
                    e._v(" global directory tree."),
                  ]),
                  e._v(" "),
                  r("p", [e._v("Example paths:")]),
                  e._v(" "),
                  r("div", { staticClass: "language- line-numbers-mode" }, [
                    r("pre", { pre: !0, attrs: { class: "language-text" } }, [
                      r("code", [
                        e._v(
                          "  base/ocr/DA/DA7B8FDA82E6D7465ADC7590EEC0C914E955C5B8.textgroup.001_to_020.txt\n  base/ocr/DA/DA7B8FDA82E6D7465ADC7590EEC0C914E955C5B8.textgroup.021_to_040.txt\n"
                        ),
                      ]),
                    ]),
                    e._v(" "),
                    r("div", { staticClass: "line-numbers-wrapper" }, [
                      r("span", { staticClass: "line-number" }, [e._v("1")]),
                      r("br"),
                      r("span", { staticClass: "line-number" }, [e._v("2")]),
                      r("br"),
                    ]),
                  ]),
                  r("p", [
                    e._v(
                      "However, when this fails to produce any text, Qiqqa "
                    ),
                    r("em", [e._v("will")]),
                    e._v(
                      " trigger a Stage 2 OCR action for each of those pages of the PDF which do not produce any text this way."
                    ),
                  ]),
                  e._v(" "),
                  r("blockquote", [
                    r("p", [
                      e._v(
                        "In actual practice, this means many text-based PDFs will have an OCR job running for them anyway when there’s an empty page, or one with only some graphics, or a title page which did not deliver any text by way of "
                      ),
                      r("code", [e._v("mupdf")]),
                      e._v("."),
                    ]),
                  ]),
                  e._v(" "),
                  r(
                    "h3",
                    {
                      attrs: {
                        id: "qiqqa-ocr-stage-2-the-ocr-attempt-the-single-call",
                      },
                    },
                    [
                      r(
                        "a",
                        {
                          staticClass: "header-anchor",
                          attrs: {
                            href:
                              "#qiqqa-ocr-stage-2-the-ocr-attempt-the-single-call",
                          },
                        },
                        [e._v("#")]
                      ),
                      e._v(" Qiqqa OCR Stage 2: The OCR Attempt (= "),
                      r(
                        "a",
                        {
                          attrs: {
                            href:
                              "https://github.com/jimmejardine/qiqqa-open-source/blob/a50888e836224e1d293457c8cd9a59cfef403bf7/Qiqqa/Documents/PDF/PDFRendering/PDFTextExtractor.cs#L711",
                            target: "_blank",
                            rel: "noopener noreferrer",
                          },
                        },
                        [
                          e._v("the "),
                          r("code", [e._v('"SINGLE"')]),
                          e._v(" call"),
                          r("OutboundLink"),
                        ],
                        1
                      ),
                      e._v(")"),
                    ]
                  ),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "This background job is executed for every single page in the PDF which  did not deliver any text in the Stage 1 process above."
                    ),
                  ]),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "By now, Qiqqa assumes the PDF is image based and requires a true OCR process to obtain the text from the PDF page."
                    ),
                  ]),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "Currently it uses the Sorax PDF library to render the PDF page"
                    ),
                    r("b", { attrs: { id: "Stage2OCR1" } }, [
                      r("a", { attrs: { href: "#SoraxWoes" } }, [
                        r("sup", [e._v("†")]),
                      ]),
                    ]),
                    e._v(", which is then "),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/1ef3403788d2b2d5efcc08dc244a60d1694f5453/QiqqaOCR/OCREngine.cs#L230",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        e._v("fed into Tesseract v3 for OCR-ing"),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                    e._v(". Region detection is performed by Qiqqa "),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/1ef3403788d2b2d5efcc08dc244a60d1694f5453/QiqqaOCR/OCREngine.cs#L251",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [e._v("proprietary logic"), r("OutboundLink")],
                      1
                    ),
                    e._v(" and passed into Tesseract."),
                    r("a", { attrs: { href: "#TesseractWoes" } }, [
                      r("sup", { attrs: { id: "user-content-stage2ocr2" } }, [
                        e._v("‡"),
                      ]),
                    ]),
                  ]),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "Again, the expected OCR output is a set of ‘words’ and box coordinates pointing at the position of these OCR-ed words in the page. This information is stored on a per-page basis in that same  proprietary Qiqqa text format."
                    ),
                  ]),
                  e._v(" "),
                  r("p", [e._v("Example path:")]),
                  e._v(" "),
                  r("div", { staticClass: "language- line-numbers-mode" }, [
                    r("pre", { pre: !0, attrs: { class: "language-text" } }, [
                      r("code", [
                        e._v(
                          "  base/ocr/DA/DA7B8FDA82E6D7465ADC7590EEC0C914E955C5B8.text.4.txt\n"
                        ),
                      ]),
                    ]),
                    e._v(" "),
                    r("div", { staticClass: "line-numbers-wrapper" }, [
                      r("span", { staticClass: "line-number" }, [e._v("1")]),
                      r("br"),
                    ]),
                  ]),
                  r(
                    "h3",
                    {
                      attrs: {
                        id:
                          "what-happens-when-stage-2-and-stage-1-has-failed…-🥶-😱",
                      },
                    },
                    [
                      r(
                        "a",
                        {
                          staticClass: "header-anchor",
                          attrs: {
                            href:
                              "#what-happens-when-stage-2-and-stage-1-has-failed…-🥶-😱",
                          },
                        },
                        [e._v("#")]
                      ),
                      e._v(
                        " What happens when Stage 2 (and Stage 1) has failed…? 🥶 😱"
                      ),
                    ]
                  ),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "Qiqqa v80 (and commercial Qiqqa v79 at least) will then go and re-queue the same OCR job(s) after a while since no OCR text cache files could be produced (the page(s) did not produce a single word after all and the Qiqqa text OCR files are not supposed to be "
                    ),
                    r("em", [e._v("empty")]),
                    e._v("!"),
                  ]),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "The result here is that Qiqqa will continuously re-attempt the same (failing) OCR activity for these troublesome pages in the background, loading the machine indefinitely. 🥶 😱"
                    ),
                  ]),
                  e._v(" "),
                  r(
                    "h4",
                    {
                      attrs: {
                        id:
                          "v82-experimental-releases-stage-3-faking-it-the-single-fake-call",
                      },
                    },
                    [
                      r(
                        "a",
                        {
                          staticClass: "header-anchor",
                          attrs: {
                            href:
                              "#v82-experimental-releases-stage-3-faking-it-the-single-fake-call",
                          },
                        },
                        [e._v("#")]
                      ),
                      e._v(" v82 "),
                      r("em", [e._v("experimental")]),
                      e._v(" releases: Stage 3: Faking It (= "),
                      r(
                        "a",
                        {
                          attrs: {
                            href:
                              "https://github.com/GerHobbelt/qiqqa-open-source/blob/bc80c1c07b0beda99e99021029c875bde36e2bd1/Qiqqa/Documents/PDF/PDFRendering/PDFTextExtractor.cs#L793",
                            target: "_blank",
                            rel: "noopener noreferrer",
                          },
                        },
                        [
                          e._v("the "),
                          r("code", [e._v('"SINGLE-FAKE"')]),
                          e._v(" call"),
                          r("OutboundLink"),
                        ],
                        1
                      ),
                      e._v(")"),
                    ]
                  ),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "Qiqqa v82 (and later, I expect 😉) has added a Stage 3: when Stage 1 and Stage 2 have failed to deliver any words for the given page, then we are sure that either the PDF page has no text or at the very least Qiqqa is currently incapable of retrieving any text on that page. To prevent Qiqqa from running heavy CPU loading OCR tasks indefinitely (= until you quit the application), we “fake it” by storing a specific “magic sequence” in the Stage 2 OCR text cache file. 🤷"
                    ),
                  ]),
                  e._v(" "),
                  r("blockquote", [
                    r("p", [
                      e._v(
                        "Future versions of Qiqqa SHOULD have improved OCR capabilities and will find and detect these “faked pages” and erase them before re-doing the OCR process then. But tthat is, at this very moment (2020-03-22 AD) still future music: "
                      ),
                      r(
                        "a",
                        {
                          attrs: {
                            href:
                              "https://github.com/jimmejardine/qiqqa-open-source/issues/160",
                            target: "_blank",
                            rel: "noopener noreferrer",
                          },
                        },
                        [e._v("#160"), r("OutboundLink")],
                        1
                      ),
                    ]),
                  ]),
                  e._v(" "),
                  r(
                    "h2",
                    {
                      attrs: {
                        id:
                          "other-qiqqa-background-processes-which-use-and-influence-the-ocr-process-behaviour",
                      },
                    },
                    [
                      r(
                        "a",
                        {
                          staticClass: "header-anchor",
                          attrs: {
                            href:
                              "#other-qiqqa-background-processes-which-use-and-influence-the-ocr-process-behaviour",
                          },
                        },
                        [e._v("#")]
                      ),
                      e._v(
                        " Other Qiqqa background processes which use and influence the OCR process’ behaviour"
                      ),
                    ]
                  ),
                  e._v(" "),
                  r(
                    "h3",
                    {
                      attrs: {
                        id: "the-lucene-text-searchindex-update-process",
                      },
                    },
                    [
                      r(
                        "a",
                        {
                          staticClass: "header-anchor",
                          attrs: {
                            href: "#the-lucene-text-searchindex-update-process",
                          },
                        },
                        [e._v("#")]
                      ),
                      e._v(" The Lucene Text SearchIndex Update Process"),
                    ]
                  ),
                  e._v(" "),
                  r("p", [
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/0b015c923e965ba61e3f6b51218ca509fcd6cabb/Qiqqa/Common/BackgroundWorkerDaemonStuff/BackgroundWorkerDaemon.cs#L231",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        e._v("Another Qiqqa background process"),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                    e._v(
                      " updates the Qiqqa text search index, which is powered by LuceneNET."
                    ),
                  ]),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "This process walks through your Qiqqa Library/Libraries and checks whether the OCR process for each PDF document has completed."
                    ),
                  ]),
                  e._v(" "),
                  r("blockquote", [
                    r("p", [
                      e._v(
                        "Incidentally, this background-running check will (re)trigger the OCR process if the answer to that question is not a resounding "
                      ),
                      r("em", [e._v("yes")]),
                      e._v("!"),
                    ]),
                  ]),
                  e._v(" "),
                  r("p", [
                    e._v(
                      "When the OCR text data is new, the data is collected and "
                    ),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/1ef3403788d2b2d5efcc08dc244a60d1694f5453/Qiqqa/DocumentLibrary/DocumentLibraryIndex/LibraryIndex.cs#L646",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        e._v("fed into the Lucene search index database"),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                    e._v(". See the "),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/a50888e836224e1d293457c8cd9a59cfef403bf7/Utilities/Language/TextIndexing/LuceneIndex.cs#L180",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        r("code", [e._v("AddDocumentPage()")]),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                    e._v(" and "),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/1ef3403788d2b2d5efcc08dc244a60d1694f5453/Qiqqa/DocumentLibrary/DocumentLibraryIndex/LibraryIndex.cs#L466",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        r("code", [e._v("IncrementalBuildNextDocuments()")]),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                    e._v(
                      " methods’ code for more. Also check out the use of the "
                    ),
                    r("code", [
                      e._v("PDFDocumentInLibrary.pages_already_indexed"),
                    ]),
                    e._v(" and "),
                    r("code", [e._v("PDFDocumentInLibrary.finished_indexing")]),
                    e._v(
                      " attribute members; any retry attempts are relaxed via the "
                    ),
                    r("code", [e._v("PDFDocumentInLibrary.last_indexed")]),
                    e._v(" attribute member: "),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/1ef3403788d2b2d5efcc08dc244a60d1694f5453/Qiqqa/DocumentLibrary/DocumentLibraryIndex/PDFDocumentInLibrary.cs#L13",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [e._v("(def)"), r("OutboundLink")],
                      1
                    ),
                    e._v(" & "),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/1ef3403788d2b2d5efcc08dc244a60d1694f5453/Qiqqa/DocumentLibrary/DocumentLibraryIndex/LibraryIndex.cs#L466",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [e._v("(use)"), r("OutboundLink")],
                      1
                    ),
                    e._v("."),
                  ]),
                  e._v(" "),
                  r(
                    "h3",
                    {
                      attrs: {
                        id: "ooh-almost-forgot-the-metadata-inference-process",
                      },
                    },
                    [
                      r(
                        "a",
                        {
                          staticClass: "header-anchor",
                          attrs: {
                            href:
                              "#ooh-almost-forgot-the-metadata-inference-process",
                          },
                        },
                        [e._v("#")]
                      ),
                      e._v(" Ooh! "),
                      r("em", [e._v("Almost forgot!")]),
                      e._v(" The metadata inference process!"),
                    ]
                  ),
                  e._v(" "),
                  r("p", [
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/0b015c923e965ba61e3f6b51218ca509fcd6cabb/Qiqqa/DocumentLibrary/MetadataExtractionDaemonStuff/MetadataExtractionDaemon.cs",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [e._v("Yet another background task"), r("OutboundLink")],
                      1
                    ),
                    e._v(
                      " goes through your libraries’ documents and attempts to infer a "
                    ),
                    r("em", [e._v("title")]),
                    e._v(", "),
                    r("em", [e._v("author")]),
                    e._v(", "),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/0b015c923e965ba61e3f6b51218ca509fcd6cabb/Qiqqa/Documents/PDF/PDFControls/Page/Tools/PDFAbstractExtraction.cs#L11",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [r("em", [e._v("abstract")]), r("OutboundLink")],
                      1
                    ),
                    e._v(" and other "),
                    r("em", [e._v("metadata")]),
                    e._v(
                      " from the OCR-ed text data for the given PDF. This MAY also (re)trigger the OCR process when the text data has not been produced before. (By now you’ll surely understand why the v82 “Stage 3” = “SINGLE-FAKE” hack was invented…)"
                    ),
                  ]),
                  e._v(" "),
                  r("p", [
                    e._v("This "),
                    r("em", [e._v("inferred")]),
                    e._v(
                      " metadata is shown and used by Qiqqa when there is no BibTeX metadata provided by the user (via Qiqqa Sniffer or manually entry):  the BibTeX metadata is deemed "
                    ),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/1ef3403788d2b2d5efcc08dc244a60d1694f5453/Qiqqa/Documents/PDF/PDFDocument.cs#L604",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        r("em", [e._v("superior")]),
                        e._v(" and "),
                        r("em", [e._v("overriding")]),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                    e._v(
                      ". This metadata is also added to the Lucene search index to help users dig up articles by [parts of the] title, author, etc. (Most of the relevant source code can be spotted in the "
                    ),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/0b015c923e965ba61e3f6b51218ca509fcd6cabb/Qiqqa/Documents/PDF/MetadataSuggestions/PDFMetadataInferenceFromPDFMetadata.cs",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        r("code", [
                          e._v("PDFMetadataInferenceFromPDFMetadata"),
                        ]),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                    e._v(" and "),
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/blob/0b015c923e965ba61e3f6b51218ca509fcd6cabb/Qiqqa/Documents/PDF/MetadataSuggestions/PDFMetadataInferenceFromOCR.cs",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        r("code", [e._v("PDFMetadataInferenceFromOCR")]),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                    e._v(" classes.)"),
                  ]),
                ]),
                e._v(" "),
                r("p", [r("br"), r("br")]),
                r(
                  "p",
                  {
                    staticStyle: { "margin-top": "50px" },
                    attrs: { align: "center" },
                  },
                  [r("img", { attrs: { src: a(165), width: "200" } })]
                ),
                e._v(" "),
                r("br"),
                r("br"),
                r("br"),
                r("p"),
                e._v(" "),
                r("p", [
                  r("b", { attrs: { id: "SoraxWoes" } }, [e._v("†")]),
                  e._v(
                    ": The Sorax library doesn’t support some ‘protected’ PDFs and renders those pages as white-on-white, resulting in a completely blank view inside Qiqqa. See also these woes viewing PDFs in Qiqqa:"
                  ),
                ]),
                e._v(" "),
                r("ul", [
                  r("li", [
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://getsatisfaction.com/qiqqa/topics/pdfs_stop_displaying_blank_pages#reply_17983571",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        e._v(
                          "https://getsatisfaction.com/qiqqa/topics/pdfs_stop_displaying_blank_pages#reply_17983571"
                        ),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                  ]),
                  e._v(" "),
                  r("li", [
                    r(
                      "a",
                      {
                        attrs: {
                          href:
                            "https://github.com/jimmejardine/qiqqa-open-source/issues/136",
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                      },
                      [
                        e._v(
                          "https://github.com/jimmejardine/qiqqa-open-source/issues/136"
                        ),
                        r("OutboundLink"),
                      ],
                      1
                    ),
                  ]),
                ]),
                e._v(" "),
                r("blockquote", [
                  r("p", [
                    e._v(
                      "At the time of this writing, I know/strongly suspect almost all these white-pages-rendered-only problems are due to bugs in the  Sorax lib as  I have many PDFs in my collection suffering from this. 🤬"
                    ),
                  ]),
                ]),
                e._v(" "),
                r("p", [
                  r("a", { attrs: { href: "#user-content-stage2ocr1" } }, [
                    e._v("⤣"),
                  ]),
                ]),
                e._v(" "),
                r("p", [
                  r("b", { attrs: { id: "TesseractWoes" } }, [e._v("‡")]),
                  e._v(": Your family name doesn’t have to be "),
                  r(
                    "a",
                    {
                      attrs: {
                        href:
                          "https://en.wikipedia.org/wiki/Statler_and_Waldorf",
                        target: "_blank",
                        rel: "noopener noreferrer",
                      },
                    },
                    [e._v("Statler or Waldorf"), r("OutboundLink")],
                    1
                  ),
                  e._v(
                    " to have plenty to complain about that region detection logic too: "
                  ),
                  r(
                    "a",
                    {
                      attrs: {
                        href:
                          "https://github.com/jimmejardine/qiqqa-open-source/issues/135",
                        target: "_blank",
                        rel: "noopener noreferrer",
                      },
                    },
                    [e._v("#135"), r("OutboundLink")],
                    1
                  ),
                  e._v(
                    ". And then there’s the old Tesseract which needs some assist as well: "
                  ),
                  r(
                    "a",
                    {
                      attrs: {
                        href:
                          "https://github.com/jimmejardine/qiqqa-open-source/issues/160",
                        target: "_blank",
                        rel: "noopener noreferrer",
                      },
                    },
                    [e._v("#160"), r("OutboundLink")],
                    1
                  ),
                  e._v(" and "),
                  r(
                    "a",
                    {
                      attrs: {
                        href:
                          "https://github.com/jimmejardine/qiqqa-open-source/issues/135#issuecomment-569827317",
                        target: "_blank",
                        rel: "noopener noreferrer",
                      },
                    },
                    [
                      e._v("one other bit mentioned in #135"),
                      r("OutboundLink"),
                    ],
                    1
                  ),
                  e._v("."),
                ]),
                e._v(" "),
                r("p", [
                  e._v(
                    "However, it’s not all that bleak when your research does not include diving into old/historic documents and/or PDFs published by companies: many modern scientific papers are published in a PDF format which can be grokked by "
                  ),
                  r("code", [e._v("mupdf")]),
                  e._v(
                    " just fine — though here I have found that quite a few PDFs which "
                  ),
                  r("em", [e._v("appear")]),
                  e._v(
                    " to have been produced by some unidentified TeX variants "
                  ),
                  r("em", [e._v("do")]),
                  e._v(" cause trouble in Stage 1 ("),
                  r("code", [e._v('"GROUP"')]),
                  e._v(") and produce some crap of their own: "),
                  r(
                    "a",
                    {
                      attrs: {
                        href:
                          "https://github.com/jimmejardine/qiqqa-open-source/issues/86",
                        target: "_blank",
                        rel: "noopener noreferrer",
                      },
                    },
                    [e._v("#86"), r("OutboundLink")],
                    1
                  ),
                ]),
                e._v(" "),
                r("p", [
                  r("a", { attrs: { href: "#user-content-stage2ocr2" } }, [
                    e._v("⤣"),
                  ]),
                ]),
              ],
              1
            );
          },
          [],
          !1,
          null,
          null,
          null
        );
      t.default = n.exports;
    },
  },
]);