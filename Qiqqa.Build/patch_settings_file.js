
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



// construct the path to the settings.settings file as managed by Visual Studio: we're going to patch these values into that master file:
var settings_path = path.join(opts._[0], "Properties/Settings.settings");
if (fs.existsSync(settings_path))
{
	var xml_str = fs.readFileSync(settings_path, 'utf8');
	var original_xml_str = xml_str;

	var names = [ "TargetDir", "ProjectDir", "SolutionDir", "Build" ];
	for (var key in names) {
		var name = names[key];

		if (!opts[name]) {
			console.error(`key ${name} not found in provided commandline parameters?!`);
		}

		//     <Setting Name="TargetDir" Type="System.String" Scope="Application">
		//      <Value Profile="(Default)">$(TargetDir)</Value>
		//    </Setting>
		var re = new RegExp(`Name="${name}"[^]+?</Setting>`, 'g');
		if (opts.debug) console.log("NAME regexp + match = ", re, re.exec(xml_str));
		
		xml_str = xml_str.replace(re, function (m) {
			var re2 = new RegExp('<Value Profile="([^"]*)">([^]*?)</Value>|<Value Profile="([^"]*)" />', 'g');

			if (opts.debug) console.log("processing NAME match = ", name, m, re2, re2.exec(m));

			m = m.replace(re2, function (m2, p1, p2) {
				if (opts.debug) console.log("processing NAME SUBmatch = ", name, m2, p1, p2, re2);

				var dst = opts[name];
				dst = dst.replace(/[\\/]/g, '/');

				return `<Value Profile="${p1}">${dst}</Value>`;
			});

			return m;
		});
	}

	if (opts.debug) console.log("Resulting XML:", xml_str);

	if (original_xml_str != xml_str) {
		console.log("Rewriting the settings file: ", settings_path);

		fs.writeFileSync(settings_path, xml_str, 'utf8');
	} else {
		console.log("Nothing to rewrite in the settings file: ", settings_path);
	}
}
else
{
	throw new Error("file does not exist: " + settings_path);
}



// construct the path to the app.config file as managed by Visual Studio: we're going to patch these values into that master file:
var settings_path = path.join(opts._[0], "app.config");
if (fs.existsSync(settings_path))
{
	var xml_str = fs.readFileSync(settings_path, 'utf8');
	var original_xml_str = xml_str;

	var names = [ "TargetDir", "ProjectDir", "SolutionDir", "Build" ];
	for (var key in names) {
		var name = names[key];

		if (!opts[name]) {
			console.error(`key ${name} not found in provided commandline parameters?!`);
		}

        //    <setting name="TargetDir" serializeAs="String">
        //        <value>$(TargetDir)</value>
        //    </setting>
		var re = new RegExp(`<setting name="${name}"[^]+?</setting>`, 'g');
		if (opts.debug) console.log("NAME regexp + match = ", re, re.exec(xml_str));
		
		xml_str = xml_str.replace(re, function (m) {
			var re2 = new RegExp('<value>([^]*?)</value>|<value />', 'g');

			if (opts.debug) console.log("processing NAME match = ", name, m, re2, re2.exec(m));
			m = m.replace(re2, function (m2, p1) {
				if (opts.debug) console.log("processing NAME SUBmatch = ", name, m2, p1, re2);

				var dst = opts[name];
				dst = dst.replace(/[\\/]/g, '/');

				return `<value>${dst}</value>`;
			});

			return m;
		});
	}

	if (opts.debug) console.log("Resulting XML:", xml_str);

	if (original_xml_str != xml_str) {
		console.log("Rewriting the settings file: ", settings_path);

		fs.writeFileSync(settings_path, xml_str, 'utf8');
	} else {
		console.log("Nothing to rewrite in the settings file: ", settings_path);
	}
}
else
{
	throw new Error("file does not exist: " + settings_path);
}



process.exit(0);

