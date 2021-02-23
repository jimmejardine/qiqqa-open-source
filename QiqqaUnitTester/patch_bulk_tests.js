
const fs = require('fs');
const path = require('path');

//console.log('ARGV:', process.argv);


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

if (opts._.length < 2) {
	console.error('MUST provide at least a pdf_list file and one (or more) .cs files to patch: ', opts._);
	process.exit(42);
}

//console.log(opts);

let pdf_list = fs.readFileSync(opts._[0], "utf8").split('\n').map(f => f.trim().replace(/^"\.\//, '').replace(/"$/, '')).filter(f => f.length > 2);
// also set up a hash table as quick lookup for our later processing of the CS file(s)
const pdf_files_hash = {};
const pdf_files_hash_passworded = {};
pdf_list.forEach(f => pdf_files_hash[f] = false);
pdf_list.forEach(f => pdf_files_hash_passworded[f] = false);

let pdf_password_protected_list = pdf_list.filter(f => /password(?:s?)=/.test(f));

//console.log("pdf list:", pdf_files_hash);

if (opts.go) {
	console.log(`
Processing CS files...
---------------------------------------------
		`);

   for (let i = 1; i < opts._.length; i++) {
      let cs_file = opts._[i];

      console.log("Processing", cs_file);

      let cs_content = fs.readFileSync(cs_file, 'utf8');

      // detect and extract the source code for the first test chunk: that code chunk serves as the template for all the others!
      let chunk_re = /(\[DataTestMethod\])([\s\S]+?)(\[Data)/g;
      let m = chunk_re.exec(cs_content);
      let snippet = m[2].trim().replace(/\r?\n/g, '\n');
      snippet = snippet.replace(/(public +void +)([\w\d_]+)(\([\s\S]+)$/, (m, p1, p2, p3) => {
         return p1 + 'XXXXXXXXX' + p3;
      });

      console.log(`Extracted first test's source code from the ${cs_file} source file:\n\n        ${snippet}\n\n`);



      console.log("Checking if all PDF files are included in a test set...");

      // Now seek out every DataRow and match it against our list of PDF files:
      let datarow_re = /\[DataRow\("([^"]+)"\)\]/g;

      for (m = datarow_re.exec(cs_content); m; m = datarow_re.exec(cs_content)) {
         let f = m[1].replace(/^\.\//, '');

         if (pdf_files_hash[f] != null) {
            pdf_files_hash[f] = true;
         } else {
            console.log("Unlisted file mentioned in the tests:", f);
         }
      }


      datarow_re = /\[DataRow\("([^"]+)"(?:,[^\])]+)\)\]/g;

      for (m = datarow_re.exec(cs_content); m; m = datarow_re.exec(cs_content)) {
         let f = m[1].replace(/^\.\//, '');

         if (pdf_files_hash_passworded[f] != null) {
            pdf_files_hash_passworded[f] = true;
         } else {
            console.log("Unlisted file mentioned in the password-protected tests:", f);
         }
      }





      console.log("Adding any not-yet-used PDF files to the test set...");

      // Add the yet-not-included test files to the rig:
      let appended_lst = [];
      let counter = 0;
      let total = 0;
      for (let idx = 0; idx < pdf_list.length; idx++) {
         let f = pdf_list[idx];
         if (pdf_files_hash[f] === false) {
            appended_lst.push(`[DataRow("${f}")]`);
            counter++;
            total++;

            // Add them in series of 20 each so each UnitTest remains reasonably sized & timed:
            if (counter >= 20) {
               appended_lst.push(`[DataTestMethod]
         public void BogusXXXXXXXXXXX(string filepath)
         {
            Bogus!

            To Be Filled With The Snippet We Already Got!
         }

               `);
               counter = 0;
            }
         }
      }

      // And make sure the last couple of items get their own test chunk too!
      if (counter > 0) {
         appended_lst.push(`[DataTestMethod]
         public void BogusXXXXXXXXXXX(string filepath)
         {
            Bogus!

            To Be Filled With The Snippet We Already Got!
         }

         `);
         counter = 0;
      }      




      // Add the yet-not-included password-protected test files to the rig:
      counter = 0;
      for (let idx = 0; idx < pdf_password_protected_list.length; idx++) {
         let f = pdf_password_protected_list[idx];
         if (pdf_files_hash_passworded[f] === false) {
            let m = /password(?:s?)=(\w+)(?:,\w+)?/.exec(f);

            appended_lst.push(`[DataRow("${f}", "${m[1]}")]`);
            counter++;
            total++;

            // Add them in series of 20 each so each UnitTest remains reasonably sized & timed:
            if (counter >= 20) {
               appended_lst.push(`[DataTestMethod]
         public void BogusXXXXXXXXXXX(string filepath, string password)
         {
            Bogus!

            To Be Filled With The Snippet We Already Got!
         }

               `);
               counter = 0;
            }
         }
      }

      // And make sure the last couple of items get their own test chunk too!
      if (counter > 0) {
         appended_lst.push(`[DataTestMethod]
         public void BogusXXXXXXXXXXX(string filepath, string password)
         {
            Bogus!

            To Be Filled With The Snippet We Already Got!
         }

         `);
         counter = 0;
      }      


      console.log(`Added ${total} PDF files.`);



      console.log("Updating the test code snippet for each test...");

      // Now append this stuff to the source code!
      let apcs = appended_lst.join('\n        ');

      cs_content = cs_content.trimEnd().replace(/\n\s*\}\s*\n\}$/, apcs + '\n    }\n}\n\n');

      // Now re-use the chunk regex to find and replace every *subsequent* test code chunk in there!
      chunk_re.lastIndex = 0;

      // nasty hack to make sure we also get the LAST one of them!
      cs_content += '\n\n\n\n[Data\n';

      counter = 1;
      cs_content = cs_content.replace(chunk_re, (m, p1, p2, p3) => {
         let has_password = p2.includes(', string password)');
         let cc = snippet.replace("XXXXXXXXX", `Test_PDF_bulk_chunk${ counter.toString(10).padStart(4, '0') }${ has_password ? '_password_protected' : '' }`);
         if (has_password) {
            cc = cc
            .replace('(string filepath)', '(string filepath, string password)')
            .replace('pdf_filename, null, ProcessPriorityClass', 'pdf_filename, password, ProcessPriorityClass');
         }

         counter++;

         return p1 + '\n        ' + cc + '\n\n\n\n        ' + p3;
      });

      // now, when all is done, patch the hack-sentinel back to useful code:
      cs_content = cs_content.trimEnd().replace(/\s*\[Data$/, '\n    }\n}\n\n');


      console.log(`Rewriting test source code file ${cs_file}`);
      cs_content = cs_content.replace(/\r?\n/g, '\n');

      fs.writeFileSync(cs_file, cs_content, 'utf8');
   }
}

