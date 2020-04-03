// https://vuepress.vuejs.org/api/node.html#usage

//const { createApp, dev, build, eject } = require('vuepress');
const cfg = require('./.vuepress/config.js');
const glob = require('glob');

//console.error(cfg);

const path = require('path');
const fs = require('fs');


let rootdir = path.resolve(path.join(__dirname, '..'));
let docsDir = path.join(rootdir, 'docs');

let magickRoot = cfg.base.replace(/\/$/, '');

// options is optional
glob('**/*.{html,svg,js,css}', {
    cwd: docsDir,
    dot: true,
    debug: false,
    nocase: false,
}, (err, files) => {
    // `files` is an array of filenames.
    // If the `nonull` option is set, and nothing
    // was found, then files is ["**/*.js"]
    // `err` is an error object or null.

    //console.error({ err, files });
    if (err) {
        console.error("glob failure: ", err);
        process.exit(5);
    }
        for (let fi in files) {
            let f = path.join(docsDir, files[fi]);
            //console.log(f, cfg.base);

            let myOwnDir = path.dirname(files[fi]).replace(/\\/g, '/');
            let myOwnDirDepth = myOwnDir.split('/').length;
            let myOwnDirReplacement = [...new Array(0).keys()].map(() => "..").join('/');
            let parentCnt = 0;
            //console.log({ myOwnDir, myOwnDirDepth, myOwnDirReplacement });

            let srcDirs = [myOwnDir];
            let replacementDirs = ['./'];

            while (myOwnDir !== '.') {
                myOwnDir = path.dirname(myOwnDir);
                parentCnt++;
                myOwnDirReplacement = [...new Array(parentCnt).keys()].map(() => "..").join('/') + '/';

                srcDirs.push(myOwnDir);
                replacementDirs.push(myOwnDirReplacement);
            }
            srcDirs = srcDirs.map((v) => '/' + (v !== '.' ? v : ''));
            //replacementDirs = replacementDirs.map((v) => '/' + (v !== '.' ? v : ''));

            //console.log({ srcDirs, replacementDirs });

            let src = fs.readFileSync(f, 'utf8');
            let repl = src;
            for (let pi in srcDirs) {
                let srcPath = srcDirs[pi];
                let dstPath = replacementDirs[pi];

                let re = new RegExp((magickRoot + srcPath).replace(/\//g, '\\/').replace(/\./g, '\\.'), 'g');

                //console.log({ pi, srcPath, dstPath, re });

                repl = repl.replace(re, dstPath);
            }

            repl = fixAdditionalBitsInPage(f, repl);

            if (repl !== src) {
                console.log("Rewriting ", f);
                fs.writeFileSync(f, repl, 'utf8');
            }
        }
});




function fixAdditionalBitsInPage(filepath, src) {
    // kill all JavaScript in the generated pages:
    if (1) {
        let re = /\<script[^>]*?src *= *"[^>]*?\.js"[^]*?\<\/script *\>/g;

        src = src.replace(re, '<!-- JS -->');

        re = /\<link[^>]*?href *= *"[^>]*?\.js"[^>]*?\/\>/g;

        src = src.replace(re, '<!-- JS -->');
    }

    // remove useless comments:
    let re = /\<!--\s*(JS)?\s*--\>/g;

    src = src.replace(re, '');

    // trim useless whitespace & empty lines:
    let lines = src.split('\n');
    lines = lines.map((l) => l.trimEnd()).filter((l) => l.length > 0);

    src = lines.join('\n');

    return src;
}


