(window.webpackJsonp = window.webpackJsonp || []).push([
  [18],
  {
    168: function (e, n, t) {
      "use strict";
      t.r(n);
      const l = {
        name: "TableOfContents",
        props: {
          includeLevel: { type: Array, required: !1, default: () => [2, 3] },
        },
        computed: {
          headers() {
            if (this.$page && this.$page.headers) {
              const e = this.includeLevel[0],
                n = this.includeLevel[1],
                t = (l, s = e) => {
                  const i = [];
                  for (let e = 0; e !== l.length; ) {
                    const r = l.slice(e + 1).findIndex((e) => e.level === s),
                      u = -1 === r ? l.length : r + e + 1,
                      c = l[e];
                    if (c.level >= s && c.level <= n) {
                      const r = l.slice(e + 1, u),
                        d = s < n && r.length > 0 ? t(r, s + 1) : null;
                      i.push(
                        Object.assign(Object.assign({}, c), { children: d })
                      );
                    }
                    e = u;
                  }
                  return i;
                };
              return t(this.$page.headers);
            }
            return null;
          },
        },
        render(e) {
          if (!this.headers) return null;
          const n = (t) =>
            e(
              "ul",
              t.map((t) =>
                e("li", [
                  e("RouterLink", { props: { to: `#${t.slug}` } }, t.title),
                  t.children ? n(t.children) : null,
                ])
              )
            );
          return e("div", [n(this.headers)]);
        },
      };
      n.default = l;
    },
  },
]);