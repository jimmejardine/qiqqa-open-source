(window.webpackJsonp = window.webpackJsonp || []).push([
  [9],
  {
    176: function (e, a, t) {
      "use strict";
      t.r(a);
      var s = t(0),
        r = Object(s.a)(
          {},
          function () {
            var e = this,
              a = e.$createElement,
              t = e._self._c || a;
            return t(
              "ContentSlotsDistributor",
              { attrs: { "slot-key": e.$parent.slotKey } },
              [
                t(
                  "h1",
                  {
                    attrs: {
                      id:
                        "qiqqa-internals-configuration-overrides-for-developers-and-testers",
                    },
                  },
                  [
                    t(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: {
                          href:
                            "#qiqqa-internals-configuration-overrides-for-developers-and-testers",
                        },
                      },
                      [e._v("#")]
                    ),
                    e._v(
                      " Qiqqa Internals :: Configuration Overrides for Developers and Testers"
                    ),
                  ]
                ),
                e._v(" "),
                t(
                  "h2",
                  { attrs: { id: "overriding-the-qiqqa-library-base-path" } },
                  [
                    t(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: {
                          href: "#overriding-the-qiqqa-library-base-path",
                        },
                      },
                      [e._v("#")]
                    ),
                    e._v(" Overriding the Qiqqa library "),
                    t("em", [e._v("Base Path")]),
                  ]
                ),
                e._v(" "),
                t("p", [
                  e._v(
                    "The regular ‘base path’, i.e. the base directory where all Qiqqa libraries are stored locally, is stored in the Windows Registry."
                  ),
                ]),
                e._v(" "),
                t("p", [
                  e._v(
                    "You can override this ‘base path’ by specifying another base path on the commandline."
                  ),
                ]),
                e._v(" "),
                t(
                  "h3",
                  {
                    attrs: {
                      id:
                        "why-would-you-want-to-override-this-registry-setting",
                    },
                  },
                  [
                    t(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: {
                          href:
                            "#why-would-you-want-to-override-this-registry-setting",
                        },
                      },
                      [e._v("#")]
                    ),
                    e._v(
                      " Why would you want to override this registry setting?"
                    ),
                  ]
                ),
                e._v(" "),
                t("p", [e._v("For example:")]),
                e._v(" "),
                t("ul", [
                  t("li", [
                    t("p", [
                      e._v(
                        "when you are testing Qiqqa and want to use a different (set of) Qiqqa Libraries for that. Overriding the ‘base path’ ensures your valuable Qiqqa libraries for regular use cannot be touched by the Qiqqa run-time under test."
                      ),
                    ]),
                    e._v(" "),
                    t("blockquote", [
                      t("p", [
                        e._v(
                          "Assuming, of course, that the regular base path directory tree and the one you specified via the commandline do not overlap."
                        ),
                      ]),
                    ]),
                  ]),
                  e._v(" "),
                  t("li", [
                    t("p", [
                      e._v(
                        "when you wish to work on one or more Qiqqa Libraries which should not be integrated into your regular set of libraries, e.g. when you wish to help someone else by having a look into their library/libraries you got copied locally."
                      ),
                    ]),
                  ]),
                ]),
                e._v(" "),
                t("h3", { attrs: { id: "commandline-format" } }, [
                  t(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#commandline-format" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" Commandline format"),
                ]),
                e._v(" "),
                t("div", { staticClass: "language-sh line-numbers-mode" }, [
                  t("pre", { pre: !0, attrs: { class: "language-sh" } }, [
                    t("code", [
                      e._v("qiqqa.exe "),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token operator" } },
                        [e._v("<")]
                      ),
                      e._v("basepath"),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token operator" } },
                        [e._v(">")]
                      ),
                      e._v("\n"),
                    ]),
                  ]),
                  e._v(" "),
                  t("div", { staticClass: "line-numbers-wrapper" }, [
                    t("span", { staticClass: "line-number" }, [e._v("1")]),
                    t("br"),
                  ]),
                ]),
                t("p", [e._v("e.g.")]),
                e._v(" "),
                t("div", { staticClass: "language-sh line-numbers-mode" }, [
                  t("pre", { pre: !0, attrs: { class: "language-sh" } }, [
                    t("code", [
                      e._v("qiqqa.exe D:"),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token punctuation" } },
                        [e._v("\\")]
                      ),
                      e._v("Qiqqa.Test.Libs"),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token punctuation" } },
                        [e._v("\\")]
                      ),
                      e._v("base"),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token punctuation" } },
                        [e._v("\\")]
                      ),
                      e._v("\n"),
                    ]),
                  ]),
                  e._v(" "),
                  t("div", { staticClass: "line-numbers-wrapper" }, [
                    t("span", { staticClass: "line-number" }, [e._v("1")]),
                    t("br"),
                  ]),
                ]),
                t("h2", { attrs: { id: "overriding-qiqqa-behaviour" } }, [
                  t(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#overriding-qiqqa-behaviour" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" Overriding Qiqqa behaviour"),
                ]),
                e._v(" "),
                t("p", [
                  e._v(
                    "You can override several Qiqqa behaviours by adding a "
                  ),
                  t(
                    "a",
                    {
                      attrs: {
                        href: "https://json5.org/",
                        target: "_blank",
                        rel: "noopener noreferrer",
                      },
                    },
                    [e._v("JSON5"), t("OutboundLink")],
                    1
                  ),
                  e._v(
                    " configuration file in the Qiqqa ‘base path’, i.e. the base directory where all Qiqqa libraries are stored locally, named "
                  ),
                  t("code", [e._v("Qiqqa.Developer.Settings.json5")]),
                  e._v(". Qiqqa will load this file at application startup."),
                ]),
                e._v(" "),
                t(
                  "h3",
                  {
                    attrs: { id: "configuring-qiqqa-developer-settings-json5" },
                  },
                  [
                    t(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: {
                          href: "#configuring-qiqqa-developer-settings-json5",
                        },
                      },
                      [e._v("#")]
                    ),
                    e._v(" Configuring "),
                    t("code", [e._v("Qiqqa.Developer.Settings.json5")]),
                  ]
                ),
                e._v(" "),
                t("p", [
                  e._v("Here’s an example which lists all supported settings:"),
                ]),
                e._v(" "),
                t("div", { staticClass: "language-json5 line-numbers-mode" }, [
                  t("pre", { pre: !0, attrs: { class: "language-json5" } }, [
                    t("code", [
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token comment" } },
                        [e._v("// This file may contain comments.")]
                      ),
                      e._v("\n"),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token comment" } },
                        [e._v("//")]
                      ),
                      e._v("\n"),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token comment" } },
                        [e._v("// Lines can be commented out at will.")]
                      ),
                      e._v("\n"),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token punctuation" } },
                        [e._v("{")]
                      ),
                      e._v("\n    "),
                      t(
                        "span",
                        {
                          pre: !0,
                          attrs: { class: "token property unquoted" },
                        },
                        [e._v("LoadKnownWebLibraries")]
                      ),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token operator" } },
                        [e._v(":")]
                      ),
                      e._v(" "),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token boolean" } },
                        [e._v("false")]
                      ),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token punctuation" } },
                        [e._v(",")]
                      ),
                      e._v("\n    "),
                      t(
                        "span",
                        {
                          pre: !0,
                          attrs: { class: "token property unquoted" },
                        },
                        [e._v("AddLegacyWebLibrariesThatCanBeFoundOnDisk")]
                      ),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token operator" } },
                        [e._v(":")]
                      ),
                      e._v(" "),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token boolean" } },
                        [e._v("false")]
                      ),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token punctuation" } },
                        [e._v(",")]
                      ),
                      e._v("\n    "),
                      t(
                        "span",
                        {
                          pre: !0,
                          attrs: { class: "token property unquoted" },
                        },
                        [e._v("SaveKnownWebLibraries")]
                      ),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token operator" } },
                        [e._v(":")]
                      ),
                      e._v(" "),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token boolean" } },
                        [e._v("false")]
                      ),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token punctuation" } },
                        [e._v(",")]
                      ),
                      e._v("\n    "),
                      t(
                        "span",
                        {
                          pre: !0,
                          attrs: { class: "token property unquoted" },
                        },
                        [e._v("DoInterestingAnalysis_GoogleScholar")]
                      ),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token operator" } },
                        [e._v(":")]
                      ),
                      e._v(" "),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token boolean" } },
                        [e._v("false")]
                      ),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token punctuation" } },
                        [e._v(",")]
                      ),
                      e._v("\n"),
                      t(
                        "span",
                        { pre: !0, attrs: { class: "token punctuation" } },
                        [e._v("}")]
                      ),
                      e._v("\n"),
                    ]),
                  ]),
                  e._v(" "),
                  t("div", { staticClass: "line-numbers-wrapper" }, [
                    t("span", { staticClass: "line-number" }, [e._v("1")]),
                    t("br"),
                    t("span", { staticClass: "line-number" }, [e._v("2")]),
                    t("br"),
                    t("span", { staticClass: "line-number" }, [e._v("3")]),
                    t("br"),
                    t("span", { staticClass: "line-number" }, [e._v("4")]),
                    t("br"),
                    t("span", { staticClass: "line-number" }, [e._v("5")]),
                    t("br"),
                    t("span", { staticClass: "line-number" }, [e._v("6")]),
                    t("br"),
                    t("span", { staticClass: "line-number" }, [e._v("7")]),
                    t("br"),
                    t("span", { staticClass: "line-number" }, [e._v("8")]),
                    t("br"),
                    t("span", { staticClass: "line-number" }, [e._v("9")]),
                    t("br"),
                  ]),
                ]),
                t("h4", { attrs: { id: "loadknownweblibraries" } }, [
                  t(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#loadknownweblibraries" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" "),
                  t("code", [e._v("LoadKnownWebLibraries")]),
                ]),
                e._v(" "),
                t("p", [
                  e._v("Set to "),
                  t("code", [e._v("false")]),
                  e._v(" to "),
                  t("strong", [e._v("disable")]),
                  e._v(
                    " Qiqqa’s default behaviour where it scans the ‘base path’ to "
                  ),
                  t("em", [e._v("discover")]),
                  e._v(" all available Qiqqa Libraries."),
                ]),
                e._v(" "),
                t("p", [
                  e._v("All libraries which have not been included in your "),
                  t("strong", [e._v("load list")]),
                  e._v(" (as saved by Qiqqa in the previous run in the file "),
                  t("code", [e._v("Guest/Qiqqa.known_web_libraries")]),
                  e._v(") will be ignored."),
                ]),
                e._v(" "),
                t("p", [
                  e._v("Set to "),
                  t("code", [e._v("true")]),
                  e._v(" to "),
                  t("strong", [e._v("enable")]),
                  e._v(" Qiqqa’s default behaviour."),
                ]),
                e._v(" "),
                t(
                  "h4",
                  {
                    attrs: { id: "addlegacyweblibrariesthatcanbefoundondisk" },
                  },
                  [
                    t(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: {
                          href: "#addlegacyweblibrariesthatcanbefoundondisk",
                        },
                      },
                      [e._v("#")]
                    ),
                    e._v(" "),
                    t("code", [
                      e._v("AddLegacyWebLibrariesThatCanBeFoundOnDisk"),
                    ]),
                  ]
                ),
                e._v(" "),
                t("p", [e._v("TBD")]),
                e._v(" "),
                t("h4", { attrs: { id: "saveknownweblibraries" } }, [
                  t(
                    "a",
                    {
                      staticClass: "header-anchor",
                      attrs: { href: "#saveknownweblibraries" },
                    },
                    [e._v("#")]
                  ),
                  e._v(" "),
                  t("code", [e._v("SaveKnownWebLibraries")]),
                ]),
                e._v(" "),
                t("p", [
                  e._v("Set to "),
                  t("code", [e._v("false")]),
                  e._v(" to "),
                  t("strong", [e._v("disable")]),
                  e._v(" Qiqqa’s default behaviour where it will save your "),
                  t("strong", [e._v("load list")]),
                  e._v(
                    " (the list of Qiqqa Libraries currently discovered and loaded into Qiqqa) to disk in the file "
                  ),
                  t("code", [e._v("Guest/Qiqqa.known_web_libraries")]),
                  e._v("."),
                ]),
                e._v(" "),
                t(
                  "h4",
                  { attrs: { id: "dointerestinganalysis-googlescholar" } },
                  [
                    t(
                      "a",
                      {
                        staticClass: "header-anchor",
                        attrs: { href: "#dointerestinganalysis-googlescholar" },
                      },
                      [e._v("#")]
                    ),
                    e._v(" "),
                    t("code", [e._v("DoInterestingAnalysis_GoogleScholar")]),
                  ]
                ),
                e._v(" "),
                t("p", [
                  e._v("Set to "),
                  t("code", [e._v("false")]),
                  e._v(" to "),
                  t("strong", [e._v("disable")]),
                  e._v(
                    " Qiqqa’s default behaviour where it will perform a background "
                  ),
                  t("em", [e._v("scrape")]),
                  e._v(
                    " in Google Scholar for every PDF document you open / have opened in Qiqqa."
                  ),
                ]),
                e._v(" "),
                t("div", { staticClass: "custom-block tip" }, [
                  t("p", { staticClass: "custom-block-title" }, [e._v("TIP")]),
                  e._v(" "),
                  t("p", [
                    e._v(
                      "Since Google is pretty picky and pedantic about “proper use of Scholar” (from their perspective), hitting that search website more often than strictly necessary should be regarded with some concern: with those background scrapes (which are used to fill the “Scholar” left side panel with some suggestions while the PDF document is open in the Qiqqa Viewer) you MAY expect Google to throw a tantrum and restrict your Scholar access using convoluted Captchas and other means when you really "
                    ),
                    t("strong", [e._v("want")]),
                    e._v(
                      " to use Google Scholar in the Qiqqa Sniffer or elsewhere in the application."
                    ),
                  ]),
                  e._v(" "),
                  t("p", [
                    e._v(
                      "Hence the smart move here is to kill those background scrapes as they don’t add a lot of value (unless you really like those left side panel Scholar suggestions, of course!)"
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
      a.default = r.exports;
    },
  },
]);