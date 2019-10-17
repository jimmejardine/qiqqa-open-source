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

	var inject_pos = content.indexOf('[DataTestMethod]');

	var pre_content = content.substring(0, pos);
	var post_content = content.substring(pos);

	var partly_done = [];
	var missing_entirely = [];

	for (var i = 0; i < lines.length; i++) {
		var cnt = handled[i];
		var line = lines[i];

		if (cnt >= 2) continue;

		if (cnt == 1) {
			partly_done.push(line);
		} else {
			missing_entirely.push(line);
		}
	}

	content = pre_content;
	if (partly_done.length) {
		content += '// Partly processed:\n        ' + partly_done.join('\n        ') + '\n        ';
	}
	if (missing_entirely.length) {
		content += '// Must be processed yet:\n        ' + missing_entirely.join('\n        ') + '\n        ';
	}
	content += post_content;

	fs.writeFileSync(csfile, content, 'utf8');
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
