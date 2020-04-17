
const JSON5 = require('@gerhobbelt/JSON5');
const path = require('path');

console.trace(`@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ DEFAULT.JS TEMPLATE @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@`);

// https://stackoverflow.com/questions/7616461/generate-a-hash-from-string-in-javascript
function hashFnv32a(str) {
  /*jshint bitwise:false */
  let i, l,
    hval = 0x811c9dc5;

  for (i = 0, l = str.length; i < l; i++) {
    hval ^= str.charCodeAt(i);
    hval += (hval << 1) + (hval << 4) + (hval << 7) + (hval << 8) + (hval << 24);
  }
  return hval >>> 0;
}


class MyDefaultOuterWrapper {
  data(inputPath, engine, template) {
  	//console.trace(`MyDefaultOuterWrapper.data(${JSON5.stringify({inputPath, template: template || "**NULL**", engine: !!engine}, null, 2, true)})`);
    return {
      title: function (data, config, tmpl) {
        //if (data && data.page && data.page.fileslug) {
          let title = data.page.filePathStem; // = data.page.fileSlug;
          let h = hashFnv32a(title);
          let hs4 = h.toString(16).substr(0, 4).toUpperCase();
          let fn = path.basename(title);
          let p = path.dirname(title);
          if (fn.length > 32) {
            title = `${ p }/${ fn.substring(0, 32) }~${ hs4 }`;
          }
          console.trace(`MyDefaultOuterWrapper.data RETURNS ${JSON5.stringify({inputPath, title, h, fn, p}, null, 2, true)})`);
          return title.replace(/[\\\/]+/g, ' :: ');
        //}
        //return "This is my blog post title";
      },

      // Writes to "/this-is-my-blog-post-title/index.html"
      //
      //permalink: function({ title }) {
      //  return `/my-permalink/${this.slug(title)}/`;
      //}
      permalink: function (data, config, tmpl) {
        function myGetFilter(name) {
          return (
            config.javascriptFunctions[name] ||
            config.nunjucksFilters[name] ||
            config.liquidFilters[name] ||
            config.handlebarsHelpers[name]
          );
        }

        let slugfn = myGetFilter("slug");
        let fnstr = slugfn ? slugfn.toString() : "**null**";

        let title = data.title;
        if (typeof title === 'function') {
          title = title(data, config, tmpl);
        }
        let tp = title.replace(/ :: /g, '\/').replace(/\s+/g, '.').replace(/:/g, '~').replace(/^~+/, '').replace(/~+$/, '');
        tp = tp.split(/[\\\/]+/g).map((p) => p.trim()).filter((p) => p.length > 0).join('/');

	  	  console.trace(`MyDefaultOuterWrapper.data.permalink(${JSON5.stringify({
          data, 
          tp,
          title,
          sluggedTitle: slugfn(tp),
          getFilter: !!config.getFilter, 
          slugList: config.javascriptFunctions, 
          //slug: fnstr, 
          //"this": this || '**NULL**', 
          //config: config || '**NULL**', 
          //tmpl: tmpl || '**NULL**'
        }, null, 2, true)})`);
        
        // WARNING: this.slug() is advertised in the 11ty documentation, but it doesn't fly as this doesn't have a slug member!
        let rv = `/${ title }-${ /* this.slug(data.title) */ slugfn( data.page.inputPath ) }.html`;  // <-- only for testing; messing around!
        rv = `/${ tp }.html`;
        console.log(`PERMALINK custom function --> ${rv}\n  via input args: (${JSON5.stringify({
          title: data.title,
          date: data.date,
          page: data.page,
          categories: data.categories,
          tags: data.tags,
          //data,
        }, null, 2)})`);
        return rv;
      }
    };
  }

  render(data) {
	  //console.trace(`MyDefaultOuterWrapper.render(${JSON5.stringify(data, null, 2, true)})`);
  	return `
<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>${ data.title }</title>
  </head>
  <body>
    
    ${ data.content }

    <footer>
      © 2020 Qiqqa Contributors ::
      <a href="https://github.com/GerHobbelt/qiqqa-open-source/blob/documentation/{{ page.inputPath }}">Edit this page on GitHub</a>
    </footer>
  </body>
</html>
`.trimLeft();
  }
}

module.exports = MyDefaultOuterWrapper;
