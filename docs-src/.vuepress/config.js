const fs = require('fs');
const path = require('path');

function absSrcPath(rel) {
	let p = path.join(__dirname, '..', rel);
	return path.resolve(p);
}

const destinationPath = absSrcPath('../docs/');

function absDstPath(rel) {
	let p = path.join(destinationPath, rel);
	return path.resolve(p);
}

// https://vuepress.vuejs.org/guide/basic-config.html#config-file
// https://vuepress.vuejs.org/config/#basic-config

const cfg = {
  title: 'Qiqqa / VuePress',
  description: 'Documentation in its infancy...',

  base: '/09fe62659c00446c93b70259e45165fc0e210a53067563d837b5d5b639ac8062/magick-BS/', // '/qiqqa/',
  port: 8043,			// port # on the dev server
  dest: destinationPath,

  patterns: ['**/*.md', '**/*.vue'],

  evergreen: true,

  head: [
    ['link', { rel: 'icon', href: '/favicon.ico' }],
    //['meta', { name: 'viewport', content: 'width=device-width,initial-scale=1,user-scalable=no' }]
  ],

  // https://vuepress.vuejs.org/theme/default-theme-config.html#navbar
  themeConfig: {
      logo: "/assets/img/image001.d0f5.png", // Dang! doesn't transform URLs: 'images/image001.png',

      // search functionality doesn't work ATM:
      // https://v1.vuepress.vuejs.org/theme/default-theme-config.html#search-box
      search: false,

      nav: [
          { text: 'Home', link: '/' },
          { text: 'Guide', link: '/The.Qiqqa.Manual.html' },
          { text: 'External', link: 'https://google.com/' },
          {
              text: 'Languages',
              ariaLabel: 'Language Menu',
              items: [
                  { text: 'Chinese', link: '/language/chinese/' },
                  { text: 'Dutch', link: '/language/dutch/' },
                  { text: 'English', link: '/' },
              ]
          },
      ],

      sidebar: "auto",
      //[
      //    '/',
      //    '/page-a',
      //    ['/page-b', 'Explicit link text']
      //],

      sidebarDepth: 2,

      displayAllHeaders: true, // Default: false

      lastUpdated: 'Last Updated', // string | boolean

      // default value is true. Set it to false to hide next page links on all pages
      nextLinks: true,
      // default value is true. Set it to false to hide prev page links on all pages
      prevLinks: true,

      // Assumes GitHub. Can also be a full GitLab url.
      repo: 'jimmejardine/qiqqa-open-source',
      // Customising the header label
      // Defaults to "GitHub"/"GitLab"/"Bitbucket" depending on `themeConfig.repo`
      repoLabel: 'Contribute!',

      // Optional options for generating "Edit this page" link

      // if your docs are in a different repo from your main project:
      docsRepo: 'jimmejardine/qiqqa-open-source',
      // if your docs are not at the root of the repo:
      docsDir: 'docs',
      // if your docs are in a specific branch (defaults to 'master'):
      docsBranch: 'master',
      // defaults to false, set to true to enable
      editLinks: true,
      // custom text for edit link. Defaults to "Edit this page"
      editLinkText: 'Help us improve this page!',

  },

  markdown: {
    lineNumbers: true,

  	toc: {
  	  includeLevel: [1, 2, 3, 4]
  	},

  	extractHeaders: [ 'h1', 'h2', 'h3', 'h4' ],

  	extendMarkdown: md => {
      md.set({
	    html:         true,        // Enable HTML tags in source
	    //breaks:       false,        // Convert '\n' in paragraphs into <br>
	    linkify:      true,        // autoconvert URL-like texts to links

	    // highSecurity:
	    // - false:           lower protection against XSS/Unicode-Homologue/etc. attacks via the input MarkDown.
	    //                    This setting assumes you own or at least trust the Markdown
	    //                    being fed to MarkDonw-It. The result is a nicer render.
	    // - true (default):  maximum protection against XSS/Unicode-Homologue/etc. attacks via the input MarkDown.
	    //                    This is the default setting and assumes you have no control or absolute trust in the Markdown
	    //                    being fed to MarkDonw-It. Use this setting when using markdown-it as part of a forum or other
	    //                    website where more-or-less arbitrary users can enter and feed any MarkDown to markdown-it.
	    //
	    // See https://en.wikipedia.org/wiki/Internationalized_domain_name for details on homograph attacks, for example.
	    highSecurity: false,

	    // Enable some language-neutral replacements + quotes beautification
	    typographer:  true,

	    // Double + single quotes replacement pairs, when typographer enabled,
	    // and smartquotes on. Could be either a String or an Array.
	    //
	    // For example, you can use '«»„“' for Russian, '„“‚‘' for German,
	    // and ['«\xA0', '\xA0»', '‹\xA0', '\xA0›'] for French (including nbsp).
	    quotes: '\u201c\u201d\u2018\u2019', /* “”‘’ */
	  });

      md.use(require('markdown-it-sub'));
      md.use(require('markdown-it-sup'));
      md.use(require('markdown-it-footnote'));
      md.use(require('markdown-it-deflist'));
      md.use(require('markdown-it-ins'));
      md.use(require('markdown-it-mark'));
      md.use(require('markdown-it-include'), {
        root: absSrcPath('.'),
      });
      //md.use(require('markdown-it-github-toc').default);
      //md.use(require('markdown-it-toc-and-anchor').default);
      //md.use(require('markdown-it-anchor'));
    },
  },

  // https://github.com/vuejs/vuepress/issues/700
  // https://vuepress.vuejs.org/config/#configurewebpack

  configureWebpack: (config, isServer) => {
  	// ...
  },

  chainWebpack: (config, isServer) => {
    config.module
      .rule('pdfs')
      .test(/\.pdf$/)
      .use('file-loader')
        .loader('file-loader')
      .options({
        name: `[path][name].[hash:4].[ext]`
      });

console.error("images rule setting hit");
  config.module
    .rule('images')
      .test(/\.(png|jpe?g|gif)(\?.*)?$/)
      .use('url-loader')
        .loader('url-loader')
        .options({
          limit: 200,   // ~ 200 bytes max size of image file for embedding (data:base64)
          name: `assets/img/[name].[hash:4].[ext]`
        });

  config.module
    .rule('svg')
      .test(/\.(svg)(\?.*)?$/)
      .use('file-loader')
        .loader('file-loader')
        .options({
          name: `assets/img/[name].[hash:4].[ext]`
        });

    config.module.rule('vue')
      .uses.store
      .get('vue-loader').store
      .get('options').transformAssetUrls = {
        audio: 'src',
        video: ['src', 'poster'],
        source: 'src',
        img: ['src', 'img-src'],
        image: ['xlink:href', 'href'],
        a: 'href'
      };
  },

  plugins: [
    ['@vuepress/nprogress'],
  	['@vuepress/back-to-top'],
    ['@vuepress/search', {
      searchMaxSuggestions: 10
    }],
    ['@vuepress/last-updated'],
    ['@vuepress/active-header-links'],
  	['vuepress-plugin-global-toc'],
    ['vuepress-plugin-table-of-contents'],

    // you can use this plugin multiple times
    [
      'vuepress-plugin-container',
      {
        type: 'right',
        defaultTitle: '',
      },
    ],
    [
      'vuepress-plugin-container',
      {
        type: 'theorem',
        before: info => `<div class="theorem"><p class="title">${info}</p>`,
        after: '</div>',
      },
    ],

    // this is how VuePress Default Theme use this plugin
    [
      'vuepress-plugin-container',
      {
        type: 'tip',
        defaultTitle: {
          '/': 'TIP',
          '/zh/': '提示',
        },
      },
    ],
    [
      'vuepress-plugin-container',
      {
        type: 'warning',
        //defaultTitle: 'WARNING',   <== default value
      },
    ],
  ],

  // https://vuepress.vuejs.org/plugin/life-cycle.html#updated
  updated() {
    // ...
  },

  // https://vuepress.vuejs.org/plugin/life-cycle.html#generated
  async generated (pagePaths) {
    // cp docs-src/.nojekyll docs/ && cp docs-src/CNAME docs/
    console.error("async generated HIT");

	fs.writeFileSync(absDstPath('CNAME'), 'qiqqa.org\n', 'utf8');

	fs.writeFileSync(absDstPath('.nojekyll'), '');
  },
};

module.exports = cfg;
