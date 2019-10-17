//     cd TestData/data/fixtures/bibtex
//     ( for f in *.bib **/*.bib **/**/*.bib **/**/*.biblatex ; do echo "[DataRow(\"$f\")]" ; done ) | sort >> ../../../TestBibTeX.cs

var fs = require('fs');
var path = require('path');
var globby = require('globby');

//console.log("bump dir:", __dirname);


async function scandir() {
    const paths = await globby(['**/*.bib', '**/*.biblatex', '**/*.bibtex'], {
        cwd: __dirname + '/../TestData/data/fixtures/bibtex',
        absolute: false,
        onlyFiles: true,
        caseSensitiveMatch: false,
        ignore: []
    });
 
 	paths.sort();

    if (opts.debug) {
    	console.log("Paths to inspect:", paths);
    }

	return paths;    
}

function updateBibTeXTestFile(paths) {
	var csfile = __dirname + '/../QiqqaUnitTester/TestBibTex.cs';
	var content = fs.readFileSync(csfile, 'utf8');
	var original_content = content;

	var lines = paths.map(path => {
		return `[DataRow("${path}")]`;
	});
	var handled = lines.map(line => {
		if (!content.includes(line)) {
			return 0;
		}

		var cnt = 1;
		var pos = content.indexOf(line);

		for (;;) {
			pos += line.length;

			pos = content.indexOf(line, pos);
			if (pos > 0) {
				cnt++;
			} else {
				break;
			}
		}

		return cnt;
	});


	content = content.replace(/\r\n/g, '\n');

	// find location of Do_TestFiles_Exist
	// this test is assumed to be the first in the file!
	var item_pos = content.indexOf('[DataRow(');
	var inject_pos = content.indexOf('[DataTestMethod]');

	var pre_content = content.substring(0, Math.min(inject_pos, item_pos));
	content = content.substring(inject_pos);

	// find location of Pending_TestFiles
	// this test is assumed to be the second in the file!
	var comment_pos = content.indexOf('// TestData items:');
	item_pos = content.indexOf('[DataRow(');
	inject_pos = content.indexOf('[DataTestMethod]', 10);

	var mid1_content = content.substring(0, Math.min(inject_pos, (item_pos > 0 ? item_pos : inject_pos), (comment_pos > 0 ? comment_pos : inject_pos)));
	var post_content = content.substring(inject_pos);



	var full_set = [];
	var partly_done = [];
	var missing_entirely = [];

	for (var i = 0; i < lines.length; i++) {
		var cnt = handled[i];
		var line = lines[i];

		full_set.push(line);

		if (cnt >= 2) continue;

		if (cnt == 1) {
			partly_done.push(line);
		} else {
			missing_entirely.push(line);
		}
	}

	content = pre_content;
	if (full_set.length) {
		content += '' + full_set.join('\n        ') + '\n        ';
	}
	content += mid1_content;
	if (partly_done.length) {
		content += '// TestData items: Partly processed:\n        ' + partly_done.join('\n        ') + '\n        ';
	}
	if (missing_entirely.length) {
		content += '// TestData items: Must be processed yet:\n        ' + missing_entirely.join('\n        ') + '\n        ';
	}
	if (partly_done.length == 0 && missing_entirely.length == 0) {
		content += '// TestData items: All data files are employed in at least one BibTeX test! Hence this list is empty!\n        ';
	}
	content += post_content;

	content.replace(/\n/g, '\r\n');

	if (original_content != content) {
		console.log("Patched:", csfile);
		
		fs.writeFileSync(csfile, content, 'utf8');
	}
}





var opts = require("@gerhobbelt/nomnom")
   .option('debug', {
      abbr: 'd',
      flag: true,
      help: 'Print debugging info'
   })
   .option('refresh', {
      abbr: 'r',
   	  flag: true,
      help: 'refresh list of BibTeX files in test C# code, based on TestData data/fixtures/ tree'
   })
   .parse();

if (opts._.length > 0) {
	console.error('Unsupported arguments to bump tool: ', opts._);
	process.exit(42);
}

//console.log(opts);
//process.exit(0);





if (opts.refresh) {
	scandir()
	.then(updateBibTeXTestFile)
	.catch(console.error);
}
