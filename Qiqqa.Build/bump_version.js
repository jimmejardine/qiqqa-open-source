var fs = require('fs');
var path = require('path');
var moment = require('moment');
var semver = require('semver');
var globby = require('globby');

var package_json = require(__dirname + '/../package.json');

console.log("bump dir:", __dirname, package_json.version);


// See: https://jonthysell.com/2017/01/10/automatically-generating-version-numbers-in-visual-studio/
// 
// Quoting:
// 
// The first option is given to us right in the default AssemblyInfo.cs, letting us know that we can 
// simply set our version to Major.Minor.* and let Visual Studio auto-generate the Build and Release 
// numbers.
// 
// Visual Studio then sets the Build based on the number of days that have passed since 
// January 1st, 2000, and sets the Release to the number of two-second intervals that have elapsed 
// since midnight. Sounds good so far, and of course you can always switch back to a manual version 
// at any time. But there are a couple of caveats:
// 
// - This only works with AssemblyVersion, not AssemblyFileVersion. 
//   To make this work for both, you have to specify just the AssemblyVersion (comment out or delete 
//   the AssemblyFileVersion line) and then Visual Studio is smart enough to use the same value for 
//   AssemblyFileVersion.
// - Both numbers are generated at the exact time the particular project was built.
//   So if you have multiple projects in your solution and any one takes more than two seconds to 
//   build, you will end up with different versions for different assemblies.
//   
function calcMicrosoftStyleBuildAndReleaseVersionNumbers() {
	var now = moment();
	var epoch = new Date(2000, 1, 1);
	var b = now.diff(epoch, 'days');
	var r = Math.round(now.diff(now.clone().startOf('day'), 'seconds') / 2);

	//console.log({now, epoch, b, r});

	return {
		build: b,
		release: r
	}
}

var version_info = calcMicrosoftStyleBuildAndReleaseVersionNumbers();
version_info.major = semver.major(package_json.version);
version_info.minor = semver.minor(package_json.version);
version_info.patch = semver.patch(package_json.version);
version_info.toString = function () {
	return '' + this.major + '.' + this.minor + '.' + this.build + '.' + this.release;
}

console.log("Current version:", version_info.toString());


async function scandir() {
    const paths = await globby(['**/AssemblyInfo.cs', '**/ClientVersion.xml', '**/*.csproj'], {
        cwd: __dirname + '/..',
        absolute: true,
        onlyFiles: true,
        caseSensitiveMatch: false,
        ignore: ['Qiqqa.Build/Packages', 'packages/', 'research/']
    });
 
 	paths.sort();

    //console.log(paths);

    //<ApplicationVersion>82.0.0.0</ApplicationVersion>
    //[assembly: AssemblyVersion("82.0.*")]
    //[assembly: AssemblyFileVersion("82.0")]
    //[assembly: AssemblyCompany("Quantisle")]
    //[assembly: AssemblyCopyright("Copyright © Quantisle 2010-2019.  All rights reserved.")]
    //<LatestVersion>82</LatestVersion>

	return paths;    
}

function updateFilesWithVersion(paths) {
	paths.forEach(path => {
		var content = fs.readFileSync(path, 'utf8');
		var original_content = content;

		// patch CSPROJ files:
		if (path.includes('csproj')) {
			let re = /\<ApplicationVersion\>.*?\<\/ApplicationVersion\>/g;  
			content = content.replace(re, `<ApplicationVersion>${version_info.toString()}</ApplicationVersion>`);
		}

		// patch ClientVersion files:
		if (path.includes('ClientVersion')) {
			let re = /\<LatestVersion\>.*?\<\/LatestVersion\>/g;  
			content = content.replace(re, `<LatestVersion>${version_info.toString()}</LatestVersion>`);
		}

		// patch AssemblyInfo files:
		if (path.includes('AssemblyInfo')) {
			let re = /assembly: AssemblyVersion\(".*?"\)/g;
			content = content.replace(re, `assembly: AssemblyVersion("${version_info.toString()}")`);

			re = /assembly: AssemblyFileVersion\(".*?"\)/g;
			content = content.replace(re, `assembly: AssemblyFileVersion("${version_info.toString()}")`);

			re = /assembly: AssemblyCompany\(".*?"\)/g;
			content = content.replace(re, `assembly: AssemblyCompany("Quantisle")`);

			re = /assembly: AssemblyCopyright(".*?")/g;
			content = content.replace(re, `assembly: AssemblyCopyright("Copyright © Quantisle 2010-2019.  All rights reserved.")`);
		}

		if (original_content != content)
		{
			console.log("Patched:", path);

			// fs.writeFileSync(path, content, 'utf8');
		}
	});
}


scandir()
.then(updateFilesWithVersion)
.catch(console.error);

