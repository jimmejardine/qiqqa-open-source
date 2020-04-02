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

  evergreen: true,

  head: [
    ['link', { rel: 'icon', href: '/favicon.ico' }],
    //['meta', { name: 'viewport', content: 'width=device-width,initial-scale=1,user-scalable=no' }]
  ],

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
  	'@vuepress/back-to-top',
  	'vuepress-plugin-global-toc',
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
