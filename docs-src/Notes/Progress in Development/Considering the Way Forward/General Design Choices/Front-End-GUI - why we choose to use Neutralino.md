# Front-End/GUI : why we choose to use Neutralino

From the [`gluon` github](https://github.com/gluon-framework/gluon) README:


**Gluon is a new framework for creating desktop apps from websites**, using **system installed browsers** *(not webviews)* and NodeJS, differing a lot from other existing active projects - opening up innovation and allowing some major advantages. Instead of other similar frameworks bundling a browser like Chromium or using webviews (like Edge Webview2 on Windows), **Gluon just uses system installed browsers** like Chrome, Edge, Firefox, etc. Gluon supports Chromium ***and Firefox*** based browsers as the frontend, while Gluon's backend uses NodeJS to be versatile and easy to develop (also allowing easy learning from other popular frameworks like Electron by using the same-ish stack).

## Features

\[...\]
- **Cross-platform** - Gluon works on Windows, Linux, and macOS (WIP)
\[...\]

## Comparisons

### Internals

| Part | Gluon | Electron | Tauri | Neutralinojs |
| ---- | ----- | -------- | ------------ | ----- |
| Frontend | System installed Chromium *or Firefox* | Self-contained Chromium | System installed webview | System installed webview |
| Backend | System installed *or bundled* Node.JS | Self-contained Node.JS | Native (Rust) | Native (Any) |
| IPC | Window object | Preload | Window object | Window object |
| Status | Early in development | Production ready | Usable | Usable |
| Ecosystem | Integrated | Distributed | Integrated | Integrated |


### Benchmark / Stats

Basic (plain HTML) Hello World demo, measured on up to date Windows 10, on my machine (your experience will probably differ). Used latest stable versions of all frameworks as of 9th Dec 2022. (You shouldn't actually use random stats in benchmarks to compare frameworks, this is more so you know what Gluon is like compared to other similar projects.)

| Stat | Gluon | Electron | Tauri | Neutralinojs |
| ---- | ----- | -------- | ------------ | ----- |
| Build Size | <1MB[^system][^gluon][^1] | ~220MB | ~1.8MB[^system] | ~2.6MB[^system] |
| Memory Usage | ~80MB[^gluon] | ~100MB | ~90MB | ~90MB |
| Backend[^2] Memory Usage | ~13MB[^gluon] (Node) | ~22MB (Node) | ~3MB (Native) | ~3MB (Native) |
| Build Time | ~0.7s[^3] | ~20s[^4] | ~120s[^5] | ~2s[^3][^6] |

*Extra info: All HTML/CSS/JS is unminified (including Gluon). Built in release configuration. All binaries were left as compiled with common size optimizations enabled for that language, no stripping/packing done.*

[^system]: Does not include system installed components.
[^gluon]: Using Chrome as system browser. Early/WIP data, may change in future.

[^1]: *How is Gluon so small?* Since NodeJS is expected as a system installed component, it is "just" bundled and minified Node code.
[^2]: Backend like non-Web (not Chromium/WebView2/etc).
[^3]: Includes Node.JS spinup time.
[^4]: Built for win32 zip (not Squirrel) as a fairer comparison.
[^5]: Cold build (includes deps compiling) in release mode.
[^6]: Using `neu build -r`.

--------------

**And that's exactly why we choose to use Neutralino: "Backend: Native (Any)" plus the same main advantages as `tauri` or `gluon` (or a whole slew of others, e.g. Chromely, DeskCap, etc., all of whom target the market of Electron-without-the-extra-load).

The only alternative we still ponder instead is using the system-installed bowser directly, as we will need to use that one anyway for Qiqqa Sniffer style PDF grabbing off websites as many employ advanced fingerprinting technology to detect non-standard-and-fully-up-to-date system browser applications anno 2024, which turns using a webview-based application of any kind like Electron/Neutralino not that much of an advantage already, except for more available screen real estate and possibly a cleaner UI/UX on desktops and mobiles when we're not actively hunting PDFs on the internet?

I don't know... we need to scan/monitor the download directory anyway for the user can download/drop a new PDF at *any time* and dumping PDFs via drag&drop into the browser/application window has to be supported as well, which would mean a local system 'upload' transfer that's rather quick to run: data is copied a few more times on the way into the database / storage directory tree, but that's overhead we can easily handle on modern-ish hardware. Writing the UI code once for web viewing/use helps to move toward 'remote access', i.e. using Qiqqa as a web server alike backend where folks can access their Qiqqa database from different places as long as they have internet connectivity... (which adds another challenge: securing the data access, but anyhoo... it's either straight to browser or PWA/NeutralinoJS for our future.)



