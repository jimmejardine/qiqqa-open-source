(window.webpackJsonp = window.webpackJsonp || []).push([
  [4],
  {
    100: function (t, e, n) {
      "use strict";
      var i = n(32);
      n.n(i).a;
    },
    101: function (t, e, n) {
      "use strict";
      var i = n(33);
      n.n(i).a;
    },
    12: function (t, e, n) {
      "use strict";
      n.d(e, "d", function () {
        return i;
      }),
        n.d(e, "a", function () {
          return s;
        }),
        n.d(e, "i", function () {
          return r;
        }),
        n.d(e, "f", function () {
          return l;
        }),
        n.d(e, "g", function () {
          return c;
        }),
        n.d(e, "h", function () {
          return u;
        }),
        n.d(e, "b", function () {
          return h;
        }),
        n.d(e, "e", function () {
          return p;
        }),
        n.d(e, "k", function () {
          return d;
        }),
        n.d(e, "l", function () {
          return f;
        }),
        n.d(e, "c", function () {
          return m;
        }),
        n.d(e, "j", function () {
          return g;
        });
      const i = /#.*$/,
        a = /\.(md|html)$/,
        s = /\/$/,
        r = /^[a-z]+:/i;
      function o(t) {
        return decodeURI(t).replace(i, "").replace(a, "");
      }
      function l(t) {
        return r.test(t);
      }
      function c(t) {
        return /^mailto:/.test(t);
      }
      function u(t) {
        return /^tel:/.test(t);
      }
      function h(t) {
        if (l(t)) return t;
        const e = t.match(i),
          n = e ? e[0] : "",
          a = o(t);
        return s.test(a) ? t : a + ".html" + n;
      }
      function p(t, e) {
        const n = decodeURIComponent(t.hash),
          a = (function (t) {
            const e = t.match(i);
            if (e) return e[0];
          })(e);
        return (!a || n === a) && o(t.path) === o(e);
      }
      function d(t, e, n) {
        if (l(e)) return { type: "external", path: e };
        n &&
          (e = (function (t, e, n) {
            const i = t.charAt(0);
            if ("/" === i) return t;
            if ("?" === i || "#" === i) return e + t;
            const a = e.split("/");
            (n && a[a.length - 1]) || a.pop();
            const s = t.replace(/^\//, "").split("/");
            for (let t = 0; t < s.length; t++) {
              const e = s[t];
              ".." === e ? a.pop() : "." !== e && a.push(e);
            }
            "" !== a[0] && a.unshift("");
            return a.join("/");
          })(e, n));
        const i = o(e);
        for (let e = 0; e < t.length; e++)
          if (o(t[e].regularPath) === i)
            return Object.assign({}, t[e], {
              type: "page",
              path: h(t[e].path),
            });
        return (
          console.error(
            `[vuepress] No matching page found for sidebar item "${e}"`
          ),
          {}
        );
      }
      function f(t, e, n, i) {
        const { pages: a, themeConfig: s } = n,
          r = (i && s.locales && s.locales[i]) || s;
        if ("auto" === (t.frontmatter.sidebar || r.sidebar || s.sidebar))
          return (function (t) {
            const e = m(t.headers || []);
            return [
              {
                type: "group",
                collapsable: !1,
                title: t.title,
                path: null,
                children: e.map((e) => ({
                  type: "auto",
                  title: e.title,
                  basePath: t.path,
                  path: t.path + "#" + e.slug,
                  children: e.children || [],
                })),
              },
            ];
          })(t);
        const o = r.sidebar || s.sidebar;
        if (o) {
          const { base: t, config: n } = (function (t, e) {
            if (Array.isArray(e)) return { base: "/", config: e };
            for (const i in e)
              if (
                0 ===
                ((n = t), /(\.html|\/)$/.test(n) ? n : n + "/").indexOf(
                  encodeURI(i)
                )
              )
                return { base: i, config: e[i] };
            var n;
            return {};
          })(e, o);
          return n
            ? n.map((e) =>
                (function t(e, n, i, a = 1) {
                  if ("string" == typeof e) return d(n, e, i);
                  if (Array.isArray(e))
                    return Object.assign(d(n, e[0], i), { title: e[1] });
                  {
                    const s = e.children || [];
                    return 0 === s.length && e.path
                      ? Object.assign(d(n, e.path, i), { title: e.title })
                      : {
                          type: "group",
                          path: e.path,
                          title: e.title,
                          sidebarDepth: e.sidebarDepth,
                          children: s.map((e) => t(e, n, i, a + 1)),
                          collapsable: !1 !== e.collapsable,
                        };
                  }
                })(e, a, t)
              )
            : [];
        }
        return [];
      }
      function m(t) {
        let e;
        return (
          (t = t.map((t) => Object.assign({}, t))).forEach((t) => {
            2 === t.level
              ? (e = t)
              : e && (e.children || (e.children = [])).push(t);
          }),
          t.filter((t) => 2 === t.level)
        );
      }
      function g(t) {
        return Object.assign(t, {
          type: t.items && t.items.length ? "links" : "link",
        });
      }
    },
    168: function (t, e, n) {
      "use strict";
      n.r(e);
      var i = n(12),
        a = {
          name: "NavLink",
          props: { item: { required: !0 } },
          computed: {
            link() {
              return Object(i.b)(this.item.link);
            },
            exact() {
              return this.$site.locales
                ? Object.keys(this.$site.locales).some((t) => t === this.link)
                : "/" === this.link;
            },
            isNonHttpURI() {
              return Object(i.g)(this.link) || Object(i.h)(this.link);
            },
            isBlankTarget() {
              return "_blank" === this.target;
            },
            isInternal() {
              return !Object(i.f)(this.link) && !this.isBlankTarget;
            },
            target() {
              return this.isNonHttpURI
                ? null
                : this.item.target
                ? this.item.target
                : Object(i.f)(this.link)
                ? "_blank"
                : "";
            },
            rel() {
              return this.isNonHttpURI
                ? null
                : this.item.rel
                ? this.item.rel
                : this.isBlankTarget
                ? "noopener noreferrer"
                : "";
            },
          },
          methods: {
            focusoutAction() {
              this.$emit("focusout");
            },
          },
        },
        s = n(0),
        r = Object(s.a)(
          a,
          function () {
            var t = this,
              e = t.$createElement,
              n = t._self._c || e;
            return t.isInternal
              ? n(
                  "RouterLink",
                  {
                    staticClass: "nav-link",
                    attrs: { to: t.link, exact: t.exact },
                    nativeOn: {
                      focusout: function (e) {
                        return t.focusoutAction(e);
                      },
                    },
                  },
                  [t._v("\n  " + t._s(t.item.text) + "\n")]
                )
              : n(
                  "a",
                  {
                    staticClass: "nav-link external",
                    attrs: { href: t.link, target: t.target, rel: t.rel },
                    on: { focusout: t.focusoutAction },
                  },
                  [
                    t._v("\n  " + t._s(t.item.text) + "\n  "),
                    t.isBlankTarget ? n("OutboundLink") : t._e(),
                  ],
                  1
                );
          },
          [],
          !1,
          null,
          null,
          null
        ).exports,
        o = {
          name: "Home",
          components: { NavLink: r },
          computed: {
            data() {
              return this.$page.frontmatter;
            },
            actionLink() {
              return { link: this.data.actionLink, text: this.data.actionText };
            },
          },
        },
        l =
          (n(46),
          Object(s.a)(
            o,
            function () {
              var t = this,
                e = t.$createElement,
                n = t._self._c || e;
              return n(
                "main",
                {
                  staticClass: "home",
                  attrs: { "aria-labelledby": "main-title" },
                },
                [
                  n("header", { staticClass: "hero" }, [
                    t.data.heroImage
                      ? n("img", {
                          attrs: {
                            src: t.$withBase(t.data.heroImage),
                            alt: t.data.heroAlt || "hero",
                          },
                        })
                      : t._e(),
                    t._v(" "),
                    null !== t.data.heroText
                      ? n("h1", { attrs: { id: "main-title" } }, [
                          t._v(
                            "\n      " +
                              t._s(t.data.heroText || t.$title || "Hello") +
                              "\n    "
                          ),
                        ])
                      : t._e(),
                    t._v(" "),
                    null !== t.data.tagline
                      ? n("p", { staticClass: "description" }, [
                          t._v(
                            "\n      " +
                              t._s(
                                t.data.tagline ||
                                  t.$description ||
                                  "Welcome to your VuePress site"
                              ) +
                              "\n    "
                          ),
                        ])
                      : t._e(),
                    t._v(" "),
                    t.data.actionText && t.data.actionLink
                      ? n(
                          "p",
                          { staticClass: "action" },
                          [
                            n("NavLink", {
                              staticClass: "action-button",
                              attrs: { item: t.actionLink },
                            }),
                          ],
                          1
                        )
                      : t._e(),
                  ]),
                  t._v(" "),
                  t.data.features && t.data.features.length
                    ? n(
                        "div",
                        { staticClass: "features" },
                        t._l(t.data.features, function (e, i) {
                          return n("div", { key: i, staticClass: "feature" }, [
                            n("h2", [t._v(t._s(e.title))]),
                            t._v(" "),
                            n("p", [t._v(t._s(e.details))]),
                          ]);
                        }),
                        0
                      )
                    : t._e(),
                  t._v(" "),
                  n("Content", { staticClass: "theme-default-content custom" }),
                  t._v(" "),
                  t.data.footer
                    ? n("div", { staticClass: "footer" }, [
                        t._v("\n    " + t._s(t.data.footer) + "\n  "),
                      ])
                    : t._e(),
                ],
                1
              );
            },
            [],
            !1,
            null,
            null,
            null
          ).exports),
        c = n(167),
        u =
          (n(89),
          Object(s.a)(
            {},
            function () {
              var t = this,
                e = t.$createElement,
                n = t._self._c || e;
              return n(
                "div",
                {
                  staticClass: "sidebar-button",
                  on: {
                    click: function (e) {
                      return t.$emit("toggle-sidebar");
                    },
                  },
                },
                [
                  n(
                    "svg",
                    {
                      staticClass: "icon",
                      attrs: {
                        xmlns: "http://www.w3.org/2000/svg",
                        "aria-hidden": "true",
                        role: "img",
                        viewBox: "0 0 448 512",
                      },
                    },
                    [
                      n("path", {
                        attrs: {
                          fill: "currentColor",
                          d:
                            "M436 124H12c-6.627 0-12-5.373-12-12V80c0-6.627 5.373-12 12-12h424c6.627 0 12 5.373 12 12v32c0 6.627-5.373 12-12 12zm0 160H12c-6.627 0-12-5.373-12-12v-32c0-6.627 5.373-12 12-12h424c6.627 0 12 5.373 12 12v32c0 6.627-5.373 12-12 12zm0 160H12c-6.627 0-12-5.373-12-12v-32c0-6.627 5.373-12 12-12h424c6.627 0 12 5.373 12 12v32c0 6.627-5.373 12-12 12z",
                        },
                      }),
                    ]
                  ),
                ]
              );
            },
            [],
            !1,
            null,
            null,
            null
          ).exports),
        h = n(45),
        p = n(91),
        d = n.n(p),
        f = {
          name: "DropdownLink",
          components: { NavLink: r, DropdownTransition: h.a },
          props: { item: { required: !0 } },
          data: () => ({ open: !1 }),
          computed: {
            dropdownAriaLabel() {
              return this.item.ariaLabel || this.item.text;
            },
          },
          watch: {
            $route() {
              this.open = !1;
            },
          },
          methods: {
            setOpen(t) {
              this.open = t;
            },
            isLastItemOfArray: (t, e) => d()(e) === t,
          },
        },
        m =
          (n(92),
          {
            name: "NavLinks",
            components: {
              NavLink: r,
              DropdownLink: Object(s.a)(
                f,
                function () {
                  var t = this,
                    e = t.$createElement,
                    n = t._self._c || e;
                  return n(
                    "div",
                    {
                      staticClass: "dropdown-wrapper",
                      class: { open: t.open },
                    },
                    [
                      n(
                        "button",
                        {
                          staticClass: "dropdown-title",
                          attrs: {
                            type: "button",
                            "aria-label": t.dropdownAriaLabel,
                          },
                          on: {
                            click: function (e) {
                              return t.setOpen(!t.open);
                            },
                          },
                        },
                        [
                          n("span", { staticClass: "title" }, [
                            t._v(t._s(t.item.text)),
                          ]),
                          t._v(" "),
                          n("span", {
                            staticClass: "arrow",
                            class: t.open ? "down" : "right",
                          }),
                        ]
                      ),
                      t._v(" "),
                      n("DropdownTransition", [
                        n(
                          "ul",
                          {
                            directives: [
                              {
                                name: "show",
                                rawName: "v-show",
                                value: t.open,
                                expression: "open",
                              },
                            ],
                            staticClass: "nav-dropdown",
                          },
                          t._l(t.item.items, function (e, i) {
                            return n(
                              "li",
                              {
                                key: e.link || i,
                                staticClass: "dropdown-item",
                              },
                              [
                                "links" === e.type
                                  ? n("h4", [
                                      t._v(
                                        "\n          " +
                                          t._s(e.text) +
                                          "\n        "
                                      ),
                                    ])
                                  : t._e(),
                                t._v(" "),
                                "links" === e.type
                                  ? n(
                                      "ul",
                                      {
                                        staticClass: "dropdown-subitem-wrapper",
                                      },
                                      t._l(e.items, function (i) {
                                        return n(
                                          "li",
                                          {
                                            key: i.link,
                                            staticClass: "dropdown-subitem",
                                          },
                                          [
                                            n("NavLink", {
                                              attrs: { item: i },
                                              on: {
                                                focusout: function (n) {
                                                  t.isLastItemOfArray(
                                                    i,
                                                    e.items
                                                  ) &&
                                                    t.isLastItemOfArray(
                                                      e,
                                                      t.item.items
                                                    ) &&
                                                    t.setOpen(!1);
                                                },
                                              },
                                            }),
                                          ],
                                          1
                                        );
                                      }),
                                      0
                                    )
                                  : n("NavLink", {
                                      attrs: { item: e },
                                      on: {
                                        focusout: function (n) {
                                          t.isLastItemOfArray(
                                            e,
                                            t.item.items
                                          ) && t.setOpen(!1);
                                        },
                                      },
                                    }),
                              ],
                              1
                            );
                          }),
                          0
                        ),
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
              ).exports,
            },
            computed: {
              userNav() {
                return (
                  this.$themeLocaleConfig.nav ||
                  this.$site.themeConfig.nav ||
                  []
                );
              },
              nav() {
                const { locales: t } = this.$site;
                if (t && Object.keys(t).length > 1) {
                  const e = this.$page.path,
                    n = this.$router.options.routes,
                    i = this.$site.themeConfig.locales || {},
                    a = {
                      text: this.$themeLocaleConfig.selectText || "Languages",
                      ariaLabel:
                        this.$themeLocaleConfig.ariaLabel || "Select language",
                      items: Object.keys(t).map((a) => {
                        const s = t[a],
                          r = (i[a] && i[a].label) || s.lang;
                        let o;
                        return (
                          s.lang === this.$lang
                            ? (o = e)
                            : ((o = e.replace(this.$localeConfig.path, a)),
                              n.some((t) => t.path === o) || (o = a)),
                          { text: r, link: o }
                        );
                      }),
                    };
                  return [...this.userNav, a];
                }
                return this.userNav;
              },
              userLinks() {
                return (this.nav || []).map((t) =>
                  Object.assign(Object(i.j)(t), {
                    items: (t.items || []).map(i.j),
                  })
                );
              },
              repoLink() {
                const { repo: t } = this.$site.themeConfig;
                return t
                  ? /^https?:/.test(t)
                    ? t
                    : `https://github.com/${t}`
                  : null;
              },
              repoLabel() {
                if (!this.repoLink) return;
                if (this.$site.themeConfig.repoLabel)
                  return this.$site.themeConfig.repoLabel;
                const t = this.repoLink.match(/^https?:\/\/[^/]+/)[0],
                  e = ["GitHub", "GitLab", "Bitbucket"];
                for (let n = 0; n < e.length; n++) {
                  const i = e[n];
                  if (new RegExp(i, "i").test(t)) return i;
                }
                return "Source";
              },
            },
          }),
        g =
          (n(93),
          Object(s.a)(
            m,
            function () {
              var t = this,
                e = t.$createElement,
                n = t._self._c || e;
              return t.userLinks.length || t.repoLink
                ? n(
                    "nav",
                    { staticClass: "nav-links" },
                    [
                      t._l(t.userLinks, function (t) {
                        return n(
                          "div",
                          { key: t.link, staticClass: "nav-item" },
                          [
                            "links" === t.type
                              ? n("DropdownLink", { attrs: { item: t } })
                              : n("NavLink", { attrs: { item: t } }),
                          ],
                          1
                        );
                      }),
                      t._v(" "),
                      t.repoLink
                        ? n(
                            "a",
                            {
                              staticClass: "repo-link",
                              attrs: {
                                href: t.repoLink,
                                target: "_blank",
                                rel: "noopener noreferrer",
                              },
                            },
                            [
                              t._v("\n    " + t._s(t.repoLabel) + "\n    "),
                              n("OutboundLink"),
                            ],
                            1
                          )
                        : t._e(),
                    ],
                    2
                  )
                : t._e();
            },
            [],
            !1,
            null,
            null,
            null
          ).exports);
      function b(t, e) {
        return t.ownerDocument.defaultView.getComputedStyle(t, null)[e];
      }
      var v = {
          name: "Navbar",
          components: {
            SidebarButton: u,
            NavLinks: g,
            SearchBox: c.a,
            AlgoliaSearchBox: {},
          },
          data: () => ({ linksWrapMaxWidth: null }),
          computed: {
            algolia() {
              return (
                this.$themeLocaleConfig.algolia ||
                this.$site.themeConfig.algolia ||
                {}
              );
            },
            isAlgoliaSearch() {
              return (
                this.algolia && this.algolia.apiKey && this.algolia.indexName
              );
            },
          },
          mounted() {
            const t =
                parseInt(b(this.$el, "paddingLeft")) +
                parseInt(b(this.$el, "paddingRight")),
              e = () => {
                document.documentElement.clientWidth < 719
                  ? (this.linksWrapMaxWidth = null)
                  : (this.linksWrapMaxWidth =
                      this.$el.offsetWidth -
                      t -
                      ((this.$refs.siteName &&
                        this.$refs.siteName.offsetWidth) ||
                        0));
              };
            e(), window.addEventListener("resize", e, !1);
          },
        },
        _ =
          (n(94),
          Object(s.a)(
            v,
            function () {
              var t = this,
                e = t.$createElement,
                n = t._self._c || e;
              return n(
                "header",
                { staticClass: "navbar" },
                [
                  n("SidebarButton", {
                    on: {
                      "toggle-sidebar": function (e) {
                        return t.$emit("toggle-sidebar");
                      },
                    },
                  }),
                  t._v(" "),
                  n(
                    "RouterLink",
                    { staticClass: "home-link", attrs: { to: t.$localePath } },
                    [
                      t.$site.themeConfig.logo
                        ? n("img", {
                            staticClass: "logo",
                            attrs: {
                              src: t.$withBase(t.$site.themeConfig.logo),
                              alt: t.$siteTitle,
                            },
                          })
                        : t._e(),
                      t._v(" "),
                      t.$siteTitle
                        ? n(
                            "span",
                            {
                              ref: "siteName",
                              staticClass: "site-name",
                              class: { "can-hide": t.$site.themeConfig.logo },
                            },
                            [t._v(t._s(t.$siteTitle))]
                          )
                        : t._e(),
                    ]
                  ),
                  t._v(" "),
                  n(
                    "div",
                    {
                      staticClass: "links",
                      style: t.linksWrapMaxWidth
                        ? { "max-width": t.linksWrapMaxWidth + "px" }
                        : {},
                    },
                    [
                      t.isAlgoliaSearch
                        ? n("AlgoliaSearchBox", {
                            attrs: { options: t.algolia },
                          })
                        : !1 !== t.$site.themeConfig.search &&
                          !1 !== t.$page.frontmatter.search
                        ? n("SearchBox")
                        : t._e(),
                      t._v(" "),
                      n("NavLinks", { staticClass: "can-hide" }),
                    ],
                    1
                  ),
                ],
                1
              );
            },
            [],
            !1,
            null,
            null,
            null
          ).exports),
        k = n(38),
        $ = n.n(k),
        C = {
          name: "PageEdit",
          computed: {
            lastUpdated() {
              return this.$page.lastUpdated;
            },
            lastUpdatedText() {
              return "string" == typeof this.$themeLocaleConfig.lastUpdated
                ? this.$themeLocaleConfig.lastUpdated
                : "string" == typeof this.$site.themeConfig.lastUpdated
                ? this.$site.themeConfig.lastUpdated
                : "Last Updated";
            },
            editLink() {
              const t = $()(this.$page.frontmatter.editLink)
                  ? this.$site.themeConfig.editLinks
                  : this.$page.frontmatter.editLink,
                {
                  repo: e,
                  docsDir: n = "",
                  docsBranch: i = "master",
                  docsRepo: a = e,
                } = this.$site.themeConfig;
              return t && a && this.$page.relativePath
                ? this.createEditLink(e, a, n, i, this.$page.relativePath)
                : null;
            },
            editLinkText() {
              return (
                this.$themeLocaleConfig.editLinkText ||
                this.$site.themeConfig.editLinkText ||
                "Edit this page"
              );
            },
          },
          methods: {
            createEditLink(t, e, n, a, s) {
              if (/bitbucket.org/.test(t)) {
                return (
                  (i.i.test(e) ? e : t).replace(i.a, "") +
                  "/src" +
                  `/${a}/` +
                  (n ? n.replace(i.a, "") + "/" : "") +
                  s +
                  `?mode=edit&spa=0&at=${a}&fileviewer=file-view-default`
                );
              }
              return (
                (i.i.test(e) ? e : `https://github.com/${e}`).replace(i.a, "") +
                "/edit" +
                `/${a}/` +
                (n ? n.replace(i.a, "") + "/" : "") +
                s
              );
            },
          },
        },
        L =
          (n(95),
          Object(s.a)(
            C,
            function () {
              var t = this,
                e = t.$createElement,
                n = t._self._c || e;
              return n("footer", { staticClass: "page-edit" }, [
                t.editLink
                  ? n(
                      "div",
                      { staticClass: "edit-link" },
                      [
                        n(
                          "a",
                          {
                            attrs: {
                              href: t.editLink,
                              target: "_blank",
                              rel: "noopener noreferrer",
                            },
                          },
                          [t._v(t._s(t.editLinkText))]
                        ),
                        t._v(" "),
                        n("OutboundLink"),
                      ],
                      1
                    )
                  : t._e(),
                t._v(" "),
                t.lastUpdated
                  ? n("div", { staticClass: "last-updated" }, [
                      n("span", { staticClass: "prefix" }, [
                        t._v(t._s(t.lastUpdatedText) + ":"),
                      ]),
                      t._v(" "),
                      n("span", { staticClass: "time" }, [
                        t._v(t._s(t.lastUpdated)),
                      ]),
                    ])
                  : t._e(),
              ]);
            },
            [],
            !1,
            null,
            null,
            null
          ).exports),
        x = n(96),
        y = n.n(x),
        O = {
          name: "PageNav",
          props: ["sidebarItems"],
          computed: {
            prev() {
              return S(w.PREV, this);
            },
            next() {
              return S(w.NEXT, this);
            },
          },
        };
      const w = {
        NEXT: {
          resolveLink: function (t, e) {
            return j(t, e, 1);
          },
          getThemeLinkConfig: ({ nextLinks: t }) => t,
          getPageLinkConfig: ({ frontmatter: t }) => t.next,
        },
        PREV: {
          resolveLink: function (t, e) {
            return j(t, e, -1);
          },
          getThemeLinkConfig: ({ prevLinks: t }) => t,
          getPageLinkConfig: ({ frontmatter: t }) => t.prev,
        },
      };
      function S(
        t,
        { $themeConfig: e, $page: n, $route: a, $site: s, sidebarItems: r }
      ) {
        const {
            resolveLink: o,
            getThemeLinkConfig: l,
            getPageLinkConfig: c,
          } = t,
          u = l(e),
          h = c(n),
          p = $()(h) ? u : h;
        return !1 === p
          ? void 0
          : y()(p)
          ? Object(i.k)(s.pages, p, a.path)
          : o(n, r);
      }
      function j(t, e, n) {
        const i = [];
        !(function t(e, n) {
          for (let i = 0, a = e.length; i < a; i++)
            "group" === e[i].type ? t(e[i].children || [], n) : n.push(e[i]);
        })(e, i);
        for (let e = 0; e < i.length; e++) {
          const a = i[e];
          if ("page" === a.type && a.path === decodeURIComponent(t.path))
            return i[e + n];
        }
      }
      var N = O,
        T =
          (n(97),
          {
            components: {
              PageEdit: L,
              PageNav: Object(s.a)(
                N,
                function () {
                  var t = this,
                    e = t.$createElement,
                    n = t._self._c || e;
                  return t.prev || t.next
                    ? n("div", { staticClass: "page-nav" }, [
                        n("p", { staticClass: "inner" }, [
                          t.prev
                            ? n(
                                "span",
                                { staticClass: "prev" },
                                [
                                  t._v("\n      ←\n      "),
                                  "external" === t.prev.type
                                    ? n(
                                        "a",
                                        {
                                          staticClass: "prev",
                                          attrs: {
                                            href: t.prev.path,
                                            target: "_blank",
                                            rel: "noopener noreferrer",
                                          },
                                        },
                                        [
                                          t._v(
                                            "\n        " +
                                              t._s(
                                                t.prev.title || t.prev.path
                                              ) +
                                              "\n\n        "
                                          ),
                                          n("OutboundLink"),
                                        ],
                                        1
                                      )
                                    : n(
                                        "RouterLink",
                                        {
                                          staticClass: "prev",
                                          attrs: { to: t.prev.path },
                                        },
                                        [
                                          t._v(
                                            "\n        " +
                                              t._s(
                                                t.prev.title || t.prev.path
                                              ) +
                                              "\n      "
                                          ),
                                        ]
                                      ),
                                ],
                                1
                              )
                            : t._e(),
                          t._v(" "),
                          t.next
                            ? n(
                                "span",
                                { staticClass: "next" },
                                [
                                  "external" === t.next.type
                                    ? n(
                                        "a",
                                        {
                                          attrs: {
                                            href: t.next.path,
                                            target: "_blank",
                                            rel: "noopener noreferrer",
                                          },
                                        },
                                        [
                                          t._v(
                                            "\n        " +
                                              t._s(
                                                t.next.title || t.next.path
                                              ) +
                                              "\n\n        "
                                          ),
                                          n("OutboundLink"),
                                        ],
                                        1
                                      )
                                    : n(
                                        "RouterLink",
                                        { attrs: { to: t.next.path } },
                                        [
                                          t._v(
                                            "\n        " +
                                              t._s(
                                                t.next.title || t.next.path
                                              ) +
                                              "\n      "
                                          ),
                                        ]
                                      ),
                                  t._v("\n      →\n    "),
                                ],
                                1
                              )
                            : t._e(),
                        ]),
                      ])
                    : t._e();
                },
                [],
                !1,
                null,
                null,
                null
              ).exports,
            },
            props: ["sidebarItems"],
          }),
        I =
          (n(98),
          Object(s.a)(
            T,
            function () {
              var t = this,
                e = t.$createElement,
                n = t._self._c || e;
              return n(
                "main",
                { staticClass: "page" },
                [
                  t._t("top"),
                  t._v(" "),
                  n("Content", { staticClass: "theme-default-content" }),
                  t._v(" "),
                  n("PageEdit"),
                  t._v(" "),
                  n(
                    "PageNav",
                    t._b({}, "PageNav", { sidebarItems: t.sidebarItems }, !1)
                  ),
                  t._v(" "),
                  t._t("bottom"),
                ],
                2
              );
            },
            [],
            !1,
            null,
            null,
            null
          ).exports),
        E = {
          name: "Sidebar",
          components: { SidebarLinks: n(44).default, NavLinks: g },
          props: ["items"],
        },
        A =
          (n(101),
          {
            name: "Layout",
            components: {
              Home: l,
              Page: I,
              Sidebar: Object(s.a)(
                E,
                function () {
                  var t = this.$createElement,
                    e = this._self._c || t;
                  return e(
                    "aside",
                    { staticClass: "sidebar" },
                    [
                      e("NavLinks"),
                      this._v(" "),
                      this._t("top"),
                      this._v(" "),
                      e("SidebarLinks", {
                        attrs: { depth: 0, items: this.items },
                      }),
                      this._v(" "),
                      this._t("bottom"),
                    ],
                    2
                  );
                },
                [],
                !1,
                null,
                null,
                null
              ).exports,
              Navbar: _,
            },
            data: () => ({ isSidebarOpen: !1 }),
            computed: {
              shouldShowNavbar() {
                const { themeConfig: t } = this.$site,
                  { frontmatter: e } = this.$page;
                return (
                  !1 !== e.navbar &&
                  !1 !== t.navbar &&
                  (this.$title ||
                    t.logo ||
                    t.repo ||
                    t.nav ||
                    this.$themeLocaleConfig.nav)
                );
              },
              shouldShowSidebar() {
                const { frontmatter: t } = this.$page;
                return !t.home && !1 !== t.sidebar && this.sidebarItems.length;
              },
              sidebarItems() {
                return Object(i.l)(
                  this.$page,
                  this.$page.regularPath,
                  this.$site,
                  this.$localePath
                );
              },
              pageClasses() {
                const t = this.$page.frontmatter.pageClass;
                return [
                  {
                    "no-navbar": !this.shouldShowNavbar,
                    "sidebar-open": this.isSidebarOpen,
                    "no-sidebar": !this.shouldShowSidebar,
                  },
                  t,
                ];
              },
            },
            mounted() {
              this.$router.afterEach(() => {
                this.isSidebarOpen = !1;
              });
            },
            methods: {
              toggleSidebar(t) {
                (this.isSidebarOpen =
                  "boolean" == typeof t ? t : !this.isSidebarOpen),
                  this.$emit("toggle-sidebar", this.isSidebarOpen);
              },
              onTouchStart(t) {
                this.touchStart = {
                  x: t.changedTouches[0].clientX,
                  y: t.changedTouches[0].clientY,
                };
              },
              onTouchEnd(t) {
                const e = t.changedTouches[0].clientX - this.touchStart.x,
                  n = t.changedTouches[0].clientY - this.touchStart.y;
                Math.abs(e) > Math.abs(n) &&
                  Math.abs(e) > 40 &&
                  (e > 0 && this.touchStart.x <= 80
                    ? this.toggleSidebar(!0)
                    : this.toggleSidebar(!1));
              },
            },
          }),
        P = Object(s.a)(
          A,
          function () {
            var t = this,
              e = t.$createElement,
              n = t._self._c || e;
            return n(
              "div",
              {
                staticClass: "theme-container",
                class: t.pageClasses,
                on: { touchstart: t.onTouchStart, touchend: t.onTouchEnd },
              },
              [
                t.shouldShowNavbar
                  ? n("Navbar", { on: { "toggle-sidebar": t.toggleSidebar } })
                  : t._e(),
                t._v(" "),
                n("div", {
                  staticClass: "sidebar-mask",
                  on: {
                    click: function (e) {
                      return t.toggleSidebar(!1);
                    },
                  },
                }),
                t._v(" "),
                n("Sidebar", {
                  attrs: { items: t.sidebarItems },
                  on: { "toggle-sidebar": t.toggleSidebar },
                  scopedSlots: t._u(
                    [
                      {
                        key: "top",
                        fn: function () {
                          return [t._t("sidebar-top")];
                        },
                        proxy: !0,
                      },
                      {
                        key: "bottom",
                        fn: function () {
                          return [t._t("sidebar-bottom")];
                        },
                        proxy: !0,
                      },
                    ],
                    null,
                    !0
                  ),
                }),
                t._v(" "),
                t.$page.frontmatter.home
                  ? n("Home")
                  : n("Page", {
                      attrs: { "sidebar-items": t.sidebarItems },
                      scopedSlots: t._u(
                        [
                          {
                            key: "top",
                            fn: function () {
                              return [t._t("page-top")];
                            },
                            proxy: !0,
                          },
                          {
                            key: "bottom",
                            fn: function () {
                              return [t._t("page-bottom")];
                            },
                            proxy: !0,
                          },
                        ],
                        null,
                        !0
                      ),
                    }),
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
      e.default = P.exports;
    },
    17: function (t, e, n) {},
    23: function (t, e, n) {},
    24: function (t, e, n) {},
    25: function (t, e, n) {},
    26: function (t, e, n) {},
    27: function (t, e, n) {},
    28: function (t, e, n) {},
    29: function (t, e, n) {},
    30: function (t, e, n) {},
    31: function (t, e, n) {},
    32: function (t, e, n) {},
    33: function (t, e, n) {},
    44: function (t, e, n) {
      "use strict";
      n.r(e);
      var i = n(12),
        a = {
          name: "SidebarGroup",
          components: { DropdownTransition: n(45).a },
          props: ["item", "open", "collapsable", "depth"],
          beforeCreate() {
            this.$options.components.SidebarLinks = n(44).default;
          },
          methods: { isActive: i.e },
        },
        s = (n(99), n(0)),
        r = Object(s.a)(
          a,
          function () {
            var t = this,
              e = t.$createElement,
              n = t._self._c || e;
            return n(
              "section",
              {
                staticClass: "sidebar-group",
                class: [
                  { collapsable: t.collapsable, "is-sub-group": 0 !== t.depth },
                  "depth-" + t.depth,
                ],
              },
              [
                t.item.path
                  ? n(
                      "RouterLink",
                      {
                        staticClass: "sidebar-heading clickable",
                        class: {
                          open: t.open,
                          active: t.isActive(t.$route, t.item.path),
                        },
                        attrs: { to: t.item.path },
                        nativeOn: {
                          click: function (e) {
                            return t.$emit("toggle");
                          },
                        },
                      },
                      [
                        n("span", [t._v(t._s(t.item.title))]),
                        t._v(" "),
                        t.collapsable
                          ? n("span", {
                              staticClass: "arrow",
                              class: t.open ? "down" : "right",
                            })
                          : t._e(),
                      ]
                    )
                  : n(
                      "p",
                      {
                        staticClass: "sidebar-heading",
                        class: { open: t.open },
                        on: {
                          click: function (e) {
                            return t.$emit("toggle");
                          },
                        },
                      },
                      [
                        n("span", [t._v(t._s(t.item.title))]),
                        t._v(" "),
                        t.collapsable
                          ? n("span", {
                              staticClass: "arrow",
                              class: t.open ? "down" : "right",
                            })
                          : t._e(),
                      ]
                    ),
                t._v(" "),
                n(
                  "DropdownTransition",
                  [
                    t.open || !t.collapsable
                      ? n("SidebarLinks", {
                          staticClass: "sidebar-group-items",
                          attrs: {
                            items: t.item.children,
                            "sidebar-depth": t.item.sidebarDepth,
                            depth: t.depth + 1,
                          },
                        })
                      : t._e(),
                  ],
                  1
                ),
              ],
              1
            );
          },
          [],
          !1,
          null,
          null,
          null
        ).exports;
      function o(t, e, n, i, a) {
        const s = {
          props: { to: e, activeClass: "", exactActiveClass: "" },
          class: { active: i, "sidebar-link": !0 },
        };
        return (
          a > 2 && (s.style = { "padding-left": a + "rem" }),
          t("RouterLink", s, n)
        );
      }
      function l(t, e, n, a, s, r = 1) {
        return !e || r > s
          ? null
          : t(
              "ul",
              { class: "sidebar-sub-headers" },
              e.map((e) => {
                const c = Object(i.e)(a, n + "#" + e.slug);
                return t("li", { class: "sidebar-sub-header" }, [
                  o(t, n + "#" + e.slug, e.title, c, e.level - 1),
                  l(t, e.children, n, a, s, r + 1),
                ]);
              })
            );
      }
      var c = {
        functional: !0,
        props: ["item", "sidebarDepth"],
        render(
          t,
          {
            parent: {
              $page: e,
              $site: n,
              $route: a,
              $themeConfig: s,
              $themeLocaleConfig: r,
            },
            props: { item: c, sidebarDepth: u },
          }
        ) {
          const h = Object(i.e)(a, c.path),
            p =
              "auto" === c.type
                ? h ||
                  c.children.some((t) =>
                    Object(i.e)(a, c.basePath + "#" + t.slug)
                  )
                : h,
            d =
              "external" === c.type
                ? (function (t, e, n) {
                    return t(
                      "a",
                      {
                        attrs: {
                          href: e,
                          target: "_blank",
                          rel: "noopener noreferrer",
                        },
                        class: { "sidebar-link": !0 },
                      },
                      [n, t("OutboundLink")]
                    );
                  })(t, c.path, c.title || c.path)
                : o(t, c.path, c.title || c.path, p),
            f = [
              e.frontmatter.sidebarDepth,
              u,
              r.sidebarDepth,
              s.sidebarDepth,
              1,
            ].find((t) => void 0 !== t),
            m = r.displayAllHeaders || s.displayAllHeaders;
          if ("auto" === c.type) return [d, l(t, c.children, c.basePath, a, f)];
          if ((p || m) && c.headers && !i.d.test(c.path)) {
            return [d, l(t, Object(i.c)(c.headers), c.path, a, f)];
          }
          return d;
        },
      };
      n(100);
      function u(t, e) {
        return (
          "group" === e.type &&
          e.children.some((e) =>
            "group" === e.type
              ? u(t, e)
              : "page" === e.type && Object(i.e)(t, e.path)
          )
        );
      }
      var h = {
          name: "SidebarLinks",
          components: {
            SidebarGroup: r,
            SidebarLink: Object(s.a)(c, void 0, void 0, !1, null, null, null)
              .exports,
          },
          props: ["items", "depth", "sidebarDepth"],
          data: () => ({ openGroupIndex: 0 }),
          watch: {
            $route() {
              this.refreshIndex();
            },
          },
          created() {
            this.refreshIndex();
          },
          methods: {
            refreshIndex() {
              const t = (function (t, e) {
                for (let n = 0; n < e.length; n++) {
                  const i = e[n];
                  if (u(t, i)) return n;
                }
                return -1;
              })(this.$route, this.items);
              t > -1 && (this.openGroupIndex = t);
            },
            toggleGroup(t) {
              this.openGroupIndex = t === this.openGroupIndex ? -1 : t;
            },
            isActive(t) {
              return Object(i.e)(this.$route, t.regularPath);
            },
          },
        },
        p = Object(s.a)(
          h,
          function () {
            var t = this,
              e = t.$createElement,
              n = t._self._c || e;
            return t.items.length
              ? n(
                  "ul",
                  { staticClass: "sidebar-links" },
                  t._l(t.items, function (e, i) {
                    return n(
                      "li",
                      { key: i },
                      [
                        "group" === e.type
                          ? n("SidebarGroup", {
                              attrs: {
                                item: e,
                                open: i === t.openGroupIndex,
                                collapsable: e.collapsable || e.collapsible,
                                depth: t.depth,
                              },
                              on: {
                                toggle: function (e) {
                                  return t.toggleGroup(i);
                                },
                              },
                            })
                          : n("SidebarLink", {
                              attrs: {
                                "sidebar-depth": t.sidebarDepth,
                                item: e,
                              },
                            }),
                      ],
                      1
                    );
                  }),
                  0
                )
              : t._e();
          },
          [],
          !1,
          null,
          null,
          null
        );
      e.default = p.exports;
    },
    45: function (t, e, n) {
      "use strict";
      var i = {
          name: "DropdownTransition",
          methods: {
            setHeight(t) {
              t.style.height = t.scrollHeight + "px";
            },
            unsetHeight(t) {
              t.style.height = "";
            },
          },
        },
        a = (n(90), n(0)),
        s = Object(a.a)(
          i,
          function () {
            var t = this.$createElement;
            return (this._self._c || t)(
              "transition",
              {
                attrs: { name: "dropdown" },
                on: {
                  enter: this.setHeight,
                  "after-enter": this.unsetHeight,
                  "before-leave": this.setHeight,
                },
              },
              [this._t("default")],
              2
            );
          },
          [],
          !1,
          null,
          null,
          null
        );
      e.a = s.exports;
    },
    46: function (t, e, n) {
      "use strict";
      var i = n(17);
      n.n(i).a;
    },
    89: function (t, e, n) {
      "use strict";
      var i = n(23);
      n.n(i).a;
    },
    90: function (t, e, n) {
      "use strict";
      var i = n(24);
      n.n(i).a;
    },
    92: function (t, e, n) {
      "use strict";
      var i = n(25);
      n.n(i).a;
    },
    93: function (t, e, n) {
      "use strict";
      var i = n(26);
      n.n(i).a;
    },
    94: function (t, e, n) {
      "use strict";
      var i = n(27);
      n.n(i).a;
    },
    95: function (t, e, n) {
      "use strict";
      var i = n(28);
      n.n(i).a;
    },
    97: function (t, e, n) {
      "use strict";
      var i = n(29);
      n.n(i).a;
    },
    98: function (t, e, n) {
      "use strict";
      var i = n(30);
      n.n(i).a;
    },
    99: function (t, e, n) {
      "use strict";
      var i = n(31);
      n.n(i).a;
    },
  },
]);
