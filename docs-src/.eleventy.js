const path = require('path');

module.exports = function(eleventyConfig) {
  
  console.log("*** Eleventy Config", eleventyConfig, __dirname);

  /* Pass through - stop eleventy touching */
  //eleventyConfig.addPassthroughCopy('src/images')

  //eleventyConfig.permalink = '{{ title }}{{ fileSlug }}';

  let rv = {
    dir: { 
    	input: 'docs-src', 
    	output: 'docs', 

    	// !important!: the next few entries are relative to `dir.input`!
    	// https://www.11ty.dev/docs/config/#directory-for-global-data-files
    	data: '_meta/data',
    	includes: '_meta/includes',
        layouts: '_meta/layouts',
    },
    passthroughFileCopy: true,
    templateFormats: ['liquid', 'md', 'css', 'html', 'yml'],
    htmlTemplateEngine: 'liquid',
    markdownTemplateEngine: 'liquid',
	//permalink: '{{ title }}{{ fileSlug }}-2',
  };
  console.log("RV = ", rv);
  return rv;
}
