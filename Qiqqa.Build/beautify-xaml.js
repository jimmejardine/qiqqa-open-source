
var beautify = require('xml-beautifier');
var fs = require('fs');
var path = require('path');

const xml = beautify('<div><span>foo</span></div>');
console.log(xml); // => will output correctly indented elements

var xaml_list = [].concat(process.argv).slice(2);
console.log(xaml_list);

foreach(var source_file in xaml_list) {
	console.log(`processing file ${source_file}:");
	var src = fs.readFileSync(source_file, 'utf8');
	var dst = beautify(src);
	fs.writeFileSync(source_file, dst, 'utf8');
}
