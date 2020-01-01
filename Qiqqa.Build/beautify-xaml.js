
var beautify = require('@gerhobbelt/xml-beautifier');
var fs = require('fs');
var path = require('path');


var xaml_list = [].concat(process.argv).slice(2);

for(const source_file of xaml_list) {
	console.log(`processing file ${source_file}:`);
	var src = fs.readFileSync(source_file, 'utf8');
	var dst = beautify(src);
	fs.writeFileSync(source_file, dst, 'utf8');
}
