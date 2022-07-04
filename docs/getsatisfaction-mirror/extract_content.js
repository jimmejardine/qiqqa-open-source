//
// Remove these DOM chunks at least:
// 
// <div id="topic_reply_box">...</div> - this is the getsatisfaction reply editor
// 
// Sidebar elements:
// 
// <a href="https://getsatisfaction.com/qiqqa/topics/new" class="button translate" id="create_new_topic_button">Create a new topic</a>
// 
// <div class="sidebar pod claimed" id="employees">
// 
// <div id="products">
// 
// <div class="sidebar pod" id="satisfactometer">
// 
// 
// Content top:
// 
// <div class="div hi_balloon">
// 
// multiple occurrences of: <a href="..." class="button reply_link translate">Reply</a>
// 
// This chunk of DIVs: <div class="footer"><div class="arrow"></div></div>
// 
// <div id="toolbar">
// 
// <div id="inline_login" style="display:none"></div>
// 
// <div id="overlay_screen" style="display:none"></div>
// 
// <div class="disclaimer">
// 
// 
// 
// In text:
// 
// <span class="follow_link">
// 
// <span class="summary_right_border_point"></span>
// 
// and immediately following that one the chunk wrapped in: <span class="wrapper">
// 
// <div class="action_bar">
// 
// 
// Remove when there's no important content in these:
// 
// <li class="edit_comment_row">
// 
// <li class="add_comment_row">
// 
// <div class="tools translate">
// 

const jsdom = require("jsdom");
const { JSDOM } = jsdom;
var glob = require("@gerhobbelt/glob")
const fs = require('fs')
const path = require('path')
const crypto = require('crypto');
const { PassThrough, Writable, finished } = require('stream');
const { StringDecoder } = require('string_decoder');

class HashWritable extends Writable {
  constructor() {
    super();
    this._decoder = new StringDecoder('hex');
    this.data = '';
  }
  _write(chunk, encoding, callback) {
    if (encoding === 'buffer') {
      chunk = this._decoder.write(chunk);
    }
    this.data += chunk;
  	//console.error('writable:write()', { chunk, encoding, data: this.data })
    callback();
  }
  _final(callback) {
    this.data += this._decoder.end();
  	//console.error('writable:final()', this.data);
    callback();
  }
}



const options = {
	nocase: true,
	mark: true,
	debug: false, // function (msg, obj) { console.error('glob: ', msg, obj); },
	cwd: __dirname,
};


let hash2FileTable = new Map();
let file2HashTable = new Map();

async function processOneAssetFile(f) {
	console.log("Processing file: ", f);

	// thanks to options.mark, there's a trailing / for every directory entry in the set:
	if (/\/$/.test(f)) {
		//console.log('skipping directory entry:', f);
		return;
	}

	const hash = crypto.createHash('sha256');

	//const pass = new PassThrough();
	const writable = new HashWritable();
	const input = fs.createReadStream(f);
	let rp = new Promise((resolve, reject) => {

		finished(input, (err) => {
		  if (err) {
		    console.error('Data Stream failed.', {err, f});
		    reject(new Error(`Data Stream failed: ${f} ==> ${err}`));
		  } else {
		    //console.log('Data Stream is done reading.');
		  }
		});

		finished(writable, (err) => {
		  if (err) {
		    console.error('Hash Calc Stream failed.', {err, f});
		    reject(new Error(`Hash Calc Stream failed: ${f} ==> ${err}`));
		  } else {
		    //console.log('Hash Calc Stream is done writing.');

			const h = writable.data;
			//console.log('hash:', {f, h});

			hash2FileTable.set(h, f);
			file2HashTable.set(f, h);

			resolve();
		  }
		});

		input.pipe(hash).pipe(writable);
		//writable.end();  <-- that doesn't work! Use the finished() API instead!
	});
	return rp;
}

async function processAllAssetFiles(files) {
	for (let f of files) {
		//console.log("Going to process file: ", f);
		await processOneAssetFile(f);
	}
	return;
}


function domRemoveNodeById(doc, id) {
	let n = doc.getElementById(id);
	if (n) {
		n.remove();
	}
}

function domRemoveNodesByClass(doc, cl) {
	let n = doc.getElementsByClassName(cl);
	if (n && n.length > 0) {
		// warning: when you .remove() straight from the returned list, the list will be modified internally and 'skip' some nodes.
		// 
		// Hence we collect the items in an array and delete the nodes from there.
		let dels = [];
		for (let el of n) {
			dels.push(el);
		}
		for (let el of dels) {
			el.remove();
		}
		n = doc.getElementsByClassName(cl);
	}
}

function domRemoveNodesBySelector(doc, sel) {
	let n = doc.querySelectorAll(sel);
	if (n) {
		// warning: when you .remove() straight from the returned list, the list will be modified internally and 'skip' some nodes.
		// 
		// Hence we collect the items in an array and delete the nodes from there.
		let dels = [];
		for (let el of n) {
			dels.push(el);
		}
		for (let el of dels) {
			el.remove();
		}
	}
}

function updateHREFattribute(attr) {
	let a = attr;

	if (/^\.\//.test(a)) {
		a = a.substr(2);
		if (file2HashTable.has(a)) {
			let h = file2HashTable.get(a);
			let t = hash2FileTable.get(h);
			let t_orig = t;
			//console.log('node attr HASH:', {a, h, t});

			// now move t (target) into the ____extracted/assets/ directory if it hasn't already:
			if (!/^assets\//.test(t)) {
				t = t.replace(/\.js\.download$/, '.js');
				t = t.replace(/\.jpeg/g, '.jpg');
				t = t.replace(/\.jpg[^.]+$/, '');  // b0rked jpg file extension? --> let the code below auto-discover the file format then
				if (path.extname(t) === '') {
					// is this a PNG or something else? sometimes these buggers were saved without an extension!
					let checkContent = fs.readFileSync(a, 'hex');
					if (/^89504e470d0a/.test(checkContent)) {
						t = t + '.png';
					} 
					else if (/^ffd8ffe0....4a46494600/.test(checkContent)) {
						// https://en.wikipedia.org/wiki/JPEG_File_Interchange_Format#File_format_structure
						t = t + '.jpg';
					}
					else {
						console.error('unknown file type for file:', a, '\nhex dump:', checkContent.substr(0, 512));
					}
				}
				t = path.basename(t);
				let mustCopy = true;
				if (fs.existsSync(path.join('____extracted/assets', t))) {
					// whoops! asset target filename already exists!
					// 
					// However, don't bother with near-identical-but-ever-so-slightly-different like.html and tweet_button.html files:
					if (t === 'like.html' || t === 'tweet_button.html') {
						mustCopy = false;
					} else {
						// generate a fresh filename until we hit an empty slot:
						let ext = path.extname(t);
						t = path.basename(t, ext);
						for (let cnt = 2;; cnt++) {
							let t2 = `${t}_${cnt}${ext}`;
							if (!fs.existsSync(path.join('____extracted/assets', t2))) {
								t = t2;
								break;
							}
						}
						console.log('whoops! asset target filename already exists!', a, '==>', path.join('____extracted/assets', t));  
					}
				}
				t = 'assets/' + t;
				// now copy asset to destination:
				if (mustCopy) {
					fs.copyFileSync(t_orig, '____extracted/' + t);
				}
				// and update the hash table:
				hash2FileTable.set(h, t);
			}
			// update attribute value:
			a = './' + t;
			//el.setAttribute('href', a);
		}
	}
	return a;
}

function isUselessHREF(href) {
	if (!href || href === '') return false;

	if (href.includes('https://bam.nr-data.net/')) return true;
	if (href.includes('https://www.facebook.com/')) return true;
	if (href.includes('https://syndication.twitter.com/')) return true;
	if (href.includes('https://js-agent.newrelic.com/')) return true;

	if (href.includes('nr-1167.min.js')) return true;
	if (href.includes('nr-1169.min.js')) return true;
	if (href.includes('en.js')) return true;      // getsatisfaction UI translations - useless now
	if (href.includes('api.js')) return true;
	if (href.includes('recaptcha__en.js')) return true;

	return false;
}

function cleanHtmlFile(f, dom) {
	let d = dom.window.document;

	//
	// Remove these DOM chunks at least:
	// 
	// <div id="topic_reply_box">...</div> - this is the getsatisfaction reply editor
	domRemoveNodeById(d, "topic_reply_box");
	// 
	// Sidebar elements:
	// 
	// <a href="https://getsatisfaction.com/qiqqa/topics/new" class="button translate" id="create_new_topic_button">Create a new topic</a>
if (10) {
	domRemoveNodeById(d, "create_new_topic_button");
	// 
	// <div class="sidebar pod claimed" id="employees">
	domRemoveNodeById(d, "employees");
	// 
	// <div id="products">
	domRemoveNodeById(d, "products");
	// 
	// <div class="sidebar pod" id="satisfactometer">
	domRemoveNodeById(d, "satisfactometer");
}
	// 
	// 
if (10) {
	// Content top:
	// 
	// <div class="div hi_balloon">
	domRemoveNodesByClass(d, "hi_balloon");
	// 
	// multiple occurrences of: <a href="..." class="button reply_link translate">Reply</a>
	domRemoveNodesBySelector(d, "a.button.reply_link");
	// 
	// This chunk of DIVs: <div class="footer"><div class="arrow"></div></div>
	domRemoveNodesBySelector(d, "div.arrow");
	// 
	// <div id="toolbar">
	domRemoveNodeById(d, "toolbar");
	// 
	// <div id="inline_login" style="display:none"></div>
	domRemoveNodeById(d, "inline_login");
	// 
	// <div id="overlay_screen" style="display:none"></div>
	domRemoveNodeById(d, "overlay_screen");
	// 
	// <div class="disclaimer">
	domRemoveNodesByClass(d, "disclaimer");
	//
	// <form action="https://getsatisfaction.com/qiqqa/searches" class="sb_pod clearfix" id="company_topic_search" method="get">
	domRemoveNodeById(d, "company_topic_search");

	domRemoveNodesByClass(d, "add_link_label");
	domRemoveNodesByClass(d, "add_a_tag");

	domRemoveNodeById(d, "share");

	domRemoveNodeById(d, "footer");

	// 
	// 
	// 
	// In text:
	// 
	// <span class="follow_link">
	domRemoveNodesByClass(d, "follow_link");
	// 
	// <span class="summary_right_border_point"></span>
	domRemoveNodesByClass(d, "summary_right_border_point");
	// 
	// and immediately following that one the chunk wrapped in: <span class="wrapper">
	// 
	// <div class="action_bar">
	domRemoveNodesByClass(d, "action_bar");
	// 
	// 
	// Remove when there's no important content in these:
	// 
	// <li class="edit_comment_row">
	domRemoveNodesByClass(d, "edit_comment_row");
	// 
	// <li class="add_comment_row">
	domRemoveNodesByClass(d, "add_comment_row");
	// 
	// <div class="tools translate">
	domRemoveNodesBySelector(d, "div.tools");
	//
	// message content is duplicated in this invisible textarea + form:
	domRemoveNodesByClass(d, "edit_reply_row");
}

	//
	// Rewrite all asset refereces to assets/* and copy the assets there if they don't exist yet:
	// 
	let hrefs = d.querySelectorAll('[href]');
	for (let el of hrefs) {
		let a = el.getAttribute('href');

		if (isUselessHREF(a)) {
			el.remove();
			continue;
		}

		a = updateHREFattribute(a);
		// update attribute value:
		el.setAttribute('href', a);
		//console.log('node href attr:', a);
	} 
	let imgsrcs = d.querySelectorAll('[src]');
	for (let el of imgsrcs) {
		let a = el.getAttribute('src');

		if (isUselessHREF(a)) {
			el.remove();
			continue;
		}

		a = updateHREFattribute(a);
		// update attribute value:
		el.setAttribute('src', a);
		//console.log('node src attr:', a);
	} 
	
	let scripts = d.querySelectorAll('script');
	for (let el of scripts) {
		let txt = el.text;
		if (txt && txt.includes('window.NREUM||(NREUM={})')) {
			el.remove();
		}
	}

	return d;
}

async function processOneHtmlFile(f) {
	console.log("Processing file: ", f);
	return new Promise((resolve, reject) => {
		fs.readFile(f, 'utf8', (err, content) => {
			if (err) {
				reject(err);
			} else {
				const dom = new JSDOM(content);
				//console.log(dom.window.document.querySelector("p").textContent);

				cleanHtmlFile(f, dom);

				let modified_content = dom.serialize();

				// remaining rough hacks to the content:
				// 
				// 														
				// move the BING header below the meta + CSS + JS code of the original page, if possible:
				modified_content = modified_content.replace(/(<!-- Banner:Start -->[\s\S]+<div class="cacheContent">)([\s\S]+)(page.company = "qiqqa")([^<]+)(<\/script>)/, (m, p1, p2, p3, p4, p5) => {
					return p2 + p3 + p4 + p5 + '\n\n\n' + p1;
				});

				fs.writeFile(path.join('____extracted', f), modified_content, 'utf8', (err, d) => {
					if (err) {
						reject(err);
					} else {
						resolve();
					}
				});
			}
		});
	});
}

async function processAllHtmlFiles(files) {
	console.log("Processing files: ", files, { h1: hash2FileTable.size, h2: file2HashTable.size });

	if (!fs.existsSync('____extracted')) {
		fs.mkdirSync('____extracted');
	}
	if (!fs.existsSync('____extracted/assets')) {
		fs.mkdirSync('____extracted/assets');
	}

	for (let f of files) {
		await processOneHtmlFile(f);
	}
}

async function globAllHtmlFiles() {
	return new Promise((resolve, reject) => {
		console.log("Scanning directory tree for HTML files...");
		glob("*.html", options, function (er, files) {
			if (er) {
				reject(er);
			}

			processAllHtmlFiles(files)
			.then(() => {
				resolve();
			})
			.catch((err) => {
				reject(err);
			})
		});
	});
}

console.log("Scanning directory tree for assets...");
glob("**/*", options, function (er, files) {
	if (er) {
		throw er;
	}
	console.log("Processing files: ", files);

	processAllAssetFiles(files).then(() => {
		console.log('hash table:', file2HashTable.entries());

		// save the (updated) hash cache to disk for future use in a rerun:
		// 
		// fs.writeFileSync('extract_content_cache.json', JSON.stringify({
		// 	hash2FileTable,
		// 	file2HashTable,
		// }, null, 2), 'utf8');

		globAllHtmlFiles().then(() => {
			console.log('done!');
		})
		.catch((err) => {
			process.exitCode = 1;
			consol.error('EX:', err);
		});
	});
})

// https://stackoverflow.com/questions/5266152/how-to-exit-in-node-js/37592669#37592669
if (0) {
	process.exitCode = 1;
}
return;











