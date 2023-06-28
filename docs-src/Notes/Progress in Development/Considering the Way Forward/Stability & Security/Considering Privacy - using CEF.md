# Considering Privacy: using CEF

CEF is basically Chromium with extra work done.

Hence, without having looked into this deeply myself yet, the same security/privacy issues will apply to Qiqqa Browser and Qiqqa Sniffer (and thus Qiqqa as a tool), as apply to Chromium.

It **may** be an option to investigate further moving *into* [Chromium County](https://github.com/chromium/chromium) by foregoing or patching [CEF](https://github.com/chromiumembedded/cef) to use a more privacy/security-hardened browser core, that's itself derived off Chromium: I got this idea while looking for was to make Chromium more privacy-minded (and less Google-data-collecting) when I stumbled across the Android-specific fork [Bromite](https://www.bromite.org/) ([git](https://github.com/bromite/bromite)) and [its SDK](https://www.bromite.org/system_web_view), leading to these forks listed below, which offer desktop Windows/Linux/OSX flavours:

- patches for Chromium: https://gitlab.com/CalyxOS/chromium-patches
- https://github.com/f32by/dumb-browser
- https://github.com/chromiumembedded/cef
- https://github.com/iridium-browser/iridium-browser + https://iridiumbrowser.de/
- https://github.com/Eloston/ungoogled-chromium
- https://github.com/gcarq/inox-patchset
- https://github.com/Eloston/ungoogled-chromium/blob/master/docs/design.md#source-file-processors
- https://github.com/ungoogled-software/ungoogled-chromium-windows
- https://github.com/Eloston/ungoogled-chromium
- 



## References

- https://chromium.googlesource.com/chromium/src/+/refs/heads/main/docs/windows_build_instructions.md
- https://github.com/chromium/chromium
- https://github.com/Eloston/ungoogled-chromium/blob/master/docs/design.md#source-file-processors
- Brave Browser dev: 
	- https://github.com/brave/brave-browser/wiki/Windows-Development-Environment 
	- https://github.com/brave/brave-browser
	- https://github.com/brave/brave-core
	- https://github.com/brave/brave-ui (see the UI components demoed at https://brave.github.io/brave-ui/?path=/story/components-layout--tabs)
- CEF integrations with other packages:
	- https://github.com/obsproject/obs-browser
	- https://github.com/ttalvitie/browservice
	- https://github.com/abhijitnandy2011/JuceCEF
	- https://github.com/Zabrimus/vdr-osr-browser
- Chrome hacking:
	- https://www.mdsec.co.uk/2021/01/breaking-the-browser-a-tale-of-ipc-credentials-and-backdoors/ + the relevant tool code: https://github.com/bats3c/ChromeTools
	- https://github.com/simonberson/ChromeURLSniffer
- Other browser engines:
	- https://github.com/weolar/miniblink49 + https://github.com/weolar/mb-demo
	- https://github.com/litehtml/litehtml + https://github.com/litehtml/litebrowser + https://github.com/litehtml/litebrowser-linux
	- https://github.com/KDE/konqueror
	- https://github.com/ReneNyffenegger/cpp-MSHTML
- Web UI with websockets: https://github.com/alifcommunity/webui

