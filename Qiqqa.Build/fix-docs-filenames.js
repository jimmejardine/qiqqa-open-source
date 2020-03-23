var fs = require('fs');
var path = require('path');
var globby = require('globby');


async function scandir() {
    const paths = await globby(['**/*.md'], {
        cwd: __dirname + '/../docs',
        absolute: true,
        onlyFiles: true,
        caseSensitiveMatch: false,
        ignore: ['packages/', 'research/']
    });
 
 	paths.sort();

    if (opts.debug) {
    	console.log("Paths to inspect:", paths);
    }

	return paths;    
}

function updateFiles(paths) {
	let count = 0;

	paths.forEach(path => {
		//console.log(path);

		let newPath = path.replace(/\s/g,  '.');
		if (newPath !== path)
		{
			console.log(`Rename: ${path}
  --> ${newPath}`);
			fs.renameSync(path, newPath);
			count++;
		}
	});

	console.log(count > 0 ? `${count} files renamed.` : "Nothing to do...");
}





var opts = require("@gerhobbelt/nomnom")
   .option('debug', {
      abbr: 'd',
      flag: true,
      help: 'Print debugging info'
   })
   .option('go', {
      abbr: 'g',
   	  flag: true,
      help: 'and... action!'
   })
   .parse();

if (opts._.length > 0) {
	console.error('Unsupported arguments to bump tool: ', opts._);
	process.exit(42);
}

//console.log(opts);
//process.exit(0);

if (opts.go) {
	console.log(`
Processing docs/ directory...
---------------------------------------------
		`);

	scandir()
	.then(updateFiles)
	.catch(console.error);
}
