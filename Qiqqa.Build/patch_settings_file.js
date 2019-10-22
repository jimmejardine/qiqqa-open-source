
var fs = require('fs');
var path = require('path');
var nomnom = require('@gerhobbelt/nomnom');


//console.error("argv = ", process.argv);
//process.exit(30);


var opts = nomnom
   .option('debug', {
      abbr: 'd',
      flag: true,
      help: 'Print debugging info'
   })
   .option('TargetDir', {
      help: 'Visual Studio macro value'
   })
   .option('SolutionDir', {
      help: 'Visual Studio macro value'
   })
   .option('ProjectDir', {
      help: 'Visual Studio macro value'
   })
   .option('Build', {
      help: 'Visual Studio macro value'
   })
   .parse();

if (opts._.length != 1) {
	console.error('Destination base directory parameter expected: ', opts._);
	process.exit(42);
}

//console.log("opts = ", opts);
//console.log("args = ", opts._);
//console.log("JSON:", JSON.stringify(opts, null, 2));



// construct the path to the Misc/Constants.cs file: we're going to patch these values into that master file:
var settings_path = path.join(opts._[0], "Misc/Constants.cs");
if (fs.existsSync(settings_path))
{
	var cs_str = fs.readFileSync(settings_path, 'utf8');
	var original_cs_str = cs_str;

	var names = [ "TargetDir", "ProjectDir", "SolutionDir", "Build" ];
	for (var key in names) {
		var name = names[key];

		if (!opts[name]) {
			console.error(`key ${name} not found in provided commandline parameters?!`);
		}

		// public const string QiqqaDevProjectDir = "VANILLA_REFERENCE";
		var re = new RegExp(`public\\s+const\\s+string\\s+QiqqaDev${name}\\s*=\\s*"[^"]*";`, 'g');
		if (opts.debug) console.log("NAME regexp + match = ", re, re.exec(cs_str));
		
		cs_str = cs_str.replace(re, function (m) {
			if (opts.debug) console.log("processing NAME match = ", name, m);

			var dst = opts[name];
			dst = dst.replace(/[\\/]/g, '/');

			return `public const string QiqqaDev${name} = "${dst}";`;
		});
	}

	if (opts.debug) console.log("Resulting C#:", cs_str);

	if (original_cs_str != cs_str) {
		console.log("Rewriting the constants file: ", settings_path);

		fs.writeFileSync(settings_path, cs_str, 'utf8');
	} else {
		console.log("Nothing to rewrite in the constants file: ", settings_path);
	}
}
else
{
	throw new Error("file does not exist: " + settings_path);
}


process.exit(0);

