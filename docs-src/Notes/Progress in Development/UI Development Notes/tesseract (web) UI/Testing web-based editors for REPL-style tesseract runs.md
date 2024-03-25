# RRR

As we intend to add image preprocessing to tesseract, plus engine configuration editing, etc. it seems nice to offer a REPL-style interface using your favorite browser in place of the old ScrollView app.

I don't intend it to be a replacement of ScrollView but rather enable us to run images through tesseract and see what gives at the various stages.

Given that the image preprocess will be script-based (as everybody will have different image processing needs, for sure) it is paramount to have a REPL editor environment where you can look at the various image stages produced by tesseract as it goes through your test image and your processing script.

First tests indicate that we either should set up something akin to a Jupyter environment (which is quite hairy and complex from where I stand, right now) or roll out a simplified approximation.
There's a few web-based "code editors" out there, including the classic `codemirror`, but a few trials suggest I might be best served with picking the Microsoft/VSCode editor: monaco.

Here's an online trial run with getting image content into the viewable editor (click to view on-line):

https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#XQAAAALrBgAAAAAAAABBqQkHQ5NjdMjwa-jY7SIQ9S7DNlzs5W-mwj0fe1ZCDRFc9ws9XQE0SJE1jc2VKxhaLFIw9vEWSxW3yscxAK2OZbDTi8k_w_aMN4yWxfoFuZVkAYEruvRlZBzjt1ZjD75dyouwvyu_60wrAvetttlxHdM__VHZ_81xkoUCAe0wSOGlEX61kLrYGXZEPQzNoW9a7kcTePcr2vsjmj6kY8dE-gWwHyO8c7gRe8OkCz59FvGwD2ze7oW76Pk21kTvH42CyZMvfngrWbYzsYk4ydALhmZXM4j91YPNXmcPTiLu5Om7qs9fEVtEZF7yaloUZHPuuMrQgrYg90AaVkl9H6O1PTlKkFS2pGVItiZqbZpSW1uDBEeQHxcJs9P0enbHzwjIc05tglWLOs2EhaEdEoph88oi0FwQFN9AR3b1wFLeVbCAqU7DgVcpMiLqYo5_ffxAnjW_NuQ96_CuCb1TIg3YJ0NpPdOPpLgEk1cjuUB-s6O57FnC_0wBf7abwZSYoQ_Bq3dXLHzEuZU8lCGKnEsRYQT_4S2LZ0wmYn0ZCYpaoAxBL-7TNrV4IaZu3A0OZkah0wMtPbkrXZFBHp4KgpEXMWo5IteDY3TwGiXTs_0_h8ygrKdPmhh_WjJlwc5tObjUBNJpXJmTTW00ErU60b0p_lgNxjGRCb7rzrpEbLBNZaHoOoPMJiznZC0ORkIKikCHpCMNrR-QmXGTGypSY6he0mfYI1disf5nQA8k1whUijPzbniaRdYWOQ6KTU-6XwsQZuoFUzofmprzajQIKbvjHKt7J4pPmCt1SqIh-HegNdKhznpoxlnm-3dzu5P81uRVLAEQ64TMHRUmkPtJsLlUlUc2iIed48NrsUEumf9D1X-R5PRUI3Nul5QPfML5M2KwwDz5KgYWzEZVS-jPKlmMZMFkMbQXBnnb0-0tTxLJZIqa1opL37Z2iQjTg5UsutDTd22X0lVDmxsQnLDb2hMtx0ju2JydJT_b0N6V3v5fOfw7Ip13L7scCKwDqWNfcQYrlxozymfWQ4CoKHCr0L8t0KW6dYRCh_DW_ZIEk7zMzfsvTEDNA1T1IziCLAqCQEIqE5eZSGM88rfK7GxvGAmJpzhywqAKo5wLLzByeKdKRIfECiPptBelVZdO4__7_AII

Code:

```
monaco.languages.register({ id: "mySpecialLanguage" });

monaco.languages.registerHoverProvider("mySpecialLanguage", {
	provideHover: function (model, position) {
		return xhr("./playground.html").then(function (res) {
			return {
				range: new monaco.Range(
					1,
					1,
					model.getLineCount(),
					model.getLineMaxColumn(model.getLineCount())
				),
				contents: [
					{ value: "**IMAGE**" },
					{
						value:
							//"```html\n" +
							//res.responseText /* .substring(0, 200) */ +
							//"\n```" +
							`
							
XXXX

---

![image](https://plus.unsplash.com/premium_photo-1680740103993-21639956f3f0?w=700&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c2FtcGxlfGVufDB8fDB8fHww)

YYYY

---

ZZZZ


![image](https://plus.unsplash.com/premium_photo-1680740103993-21639956f3f0?w=700&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c2FtcGxlfGVufDB8fDB8fHww)

AAAAAA

---


							`,
							isTrusted: true 
					},
				],
			};
		});
	},
});

monaco.editor.create(document.getElementById("container"), {
	value: "\n\nHover over this text",
	language: "mySpecialLanguage",
});

function xhr(url) {
	var req = null;
	return new Promise(function (c, e) {
		req = new XMLHttpRequest();
		req.onreadystatechange = function () {
			if (req._canceled) {
				return;
			}

			if (req.readyState === 4) {
				if (
					(req.status >= 200 && req.status < 300) ||
					req.status === 1223
				) {
					c(req);
				} else {
					e(req);
				}
				req.onreadystatechange = function () {};
			}
		};

		req.open("GET", url, true);
		req.responseType = "";

		req.send(null);
	}).catch(function () {
		req._canceled = true;
		req.abort();
	});
}
```


What happens here is the output image is made part of a Markdown chunk, which becomes visible when hovered over. That's good enough for me, today. Not ideal, but see the notes below for the other (FAILED) approaches we tried.

Notes regarding the above:

- only basic Markdown language is accepted. I haven't been able to inject literal HTML, so I suppose there's some hacking required if we require that.
- To restrict/shape the size of the image, we cannot use Markdown extensions (as seen here: https://github.com/markedjs/marked/issues/339:
```
![img](http://foo.com/img.png "foo" =100x300)  
```
   as that doesn't work in 2024 A.D. with the monaco editor, alas. I suppose this requires changing style sheets to tweak the rendered output to the max-width/max-height we would possibly want. 
   
- The HoverContent can be positioned within the editor at line/column, so it can track the script/content being edited: in my example blurb I imagined tesseract spitting back a "corrected/augmented" script for this, where a comment line or some such has the image URL to display, plus web client (i.e. browser) side JS code picking this up this server response and re-rendering the HoverContent.

So far: HoverContent.


## The other attempt(s)

What I tried initially is INLINE the image as-is: no go. I cannot find any monaco API that suggest I can custom-render, say, some specific language keywords in such a way that they take up more vertical space than a basic edit line.


Next, I ran into the "peekable content" in the form of the editor facility to show "server-side compiler messages: errors, warnings, info lines": **setModelMarkers**. This sounded great, for ideally it would allow me to inline-show the images as server-responses, which is pretty close to the mark anyhow (no pun intended ;-) ), however, there's a couple of observations that make this a probably tougher path to walk:

- currently, no custom HTML is accepted in the Marker content either.
- nor does that facility any Markdown like the HoverContent interface does. Everything is treated as PLAIN TEXT. Pity.
- If we're going to hack the "peekable" that sits below this feature, we'll have to be very careful as inspection shows the error/warning/info chunk is shown inline (yay!) using CSS `position: absolute` (not so yay, any more!) **plus** a judicious CSS adjustment for the next text/script line: each of those buggers come with their own CSS `top: 123px`-like line, where it seems the editor code has calculated these pixel offsets per line. That means *someone in there* is measuring that "peekable" interjection and adjusting `<div>`-based text/script lines' vertical positions via monaco JS code.
- Oh, almost forgot to mention: there's no facility in the API to *immediately twirl the info-line open* so all such messages initially show up as colored waves under the script line they're attached to; only after hover+click do they show up as peekable stuff in sight.


Here's our on-line discovery test run for that:

https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#XQAAAAJrCQAAAAAAAABBqQkHQ5NjdMjwa-jY7SIQ9S7DNlzs5W-mwj0fe1ZCDRFc9ws9XQE0SJE1jc2VKxhaLFIw9vEWSxW3yscxGkzsR5v06HEtAeRia3pQhA_FGNfd48ki5iBSj69vVqCq-ZyKaia3bbl-tazfoxn7Gs_XoE_AdAk5gKpR8gLgaaNASA_LDiMABc4SJE3339888DZ8OQUrh-cJ8abYrl5f6RaEXAUQ5c9xbbXhgyGfniZlG_HS_WCaXTu_3spYEMVyF_-ZlA1SNvoiu7oFGqgoo3nzlplJgNnE7ZdMb3dr8V0IZ1_uwGXwAf7Rdvz4zy5Y3Kr4H-P-zqTAntbOffCHRB9fFH7PPDiswLT7pC3fL1FK9M4R2jvi93s0fEs6Xsi6gLq02wre_cwNF_TLl6kM2teWLdKLiFM-wwD0P9r4lSfX76AzDwJTNnYDOl5kN8X_eaydErpH8MCQYYyo43p5CKhjBDhUOPp_S9pVBAVk15dFKjNlgkEEC540perotxBNh4msoBpL7_NcY8LC8ISkfU2TOdN5BT9dhevUZ2Ink0wN61CnMrplY-DFLfUKY7E0gDFNjIV7Fd3dD5S63zMqtGLyMvfwgIygb4o1RmgEh_KeiNWVjInpygyzeKCyvHXelTsehsV79RZBBVQmyVSFfrtIHCokJbsHzjG1n6ugPdAJYbb7OO0jU5kBOEuD8QXqdXi14D2jEjXN6IeLz5zgCSPY6OrVWY147eFtbh_UYZSyuXSfNjNYddzbB-X_Tbh2uKNYjvPJeMdj2e7qlEVHEnuf0JSbeX-TCMVi4w_zWc6MyPy9fb1qOVIzmfNBquoQD7o2aJhKK1kVvJb0tjx7ixDD2ieOTHObiPM2hzBnfUsIAk4tcrdW7S0hhP00IPctm0K1x6KQOKws903ZNYV8SoOAgcFfS-H5idTfiHAXGq34ggelwK5J4J4MSfnYvfZIX8my9Hhpi7ef3LK8HXgqO-Osq_heCfdnuojZrREjtVNYoK-Tfbc6xfBRK4KnoE92ED-C7DUYzudG0e5nRvzruVB_lxJmdApdwZ-tqqi12Q4Z6wvGXUNra1CznQa_W4LJvM8xTGJfCKl4Tsy_Hf3vXmFPStc7jq6R5ygusB3VwAFClLTcuOm5G8kHWV-SZ_P_DRPr4OMS3XNC7nbF4a69endPmvmU7LmwERKasmBuXUjjhFnJv13QGDgAVgMeOFJdfoUl-88SVKnfRKmaUzNk-5z7xx336QjdPrwOM38Faib4LFjnMIBg8aycLKibsTMpybs_snA8wMUlTL2E0xfRwSuTnmKd_wDi1GxgT0DYQgU6Ifegb27caPyDGzguRYkZ9m8Ywtjjs4h-NDmiLlAR5lCPkIXRJXnkqwopqkpambfmqKrLzp5rJGMFVfk98QbrJ4pl63pwilodUi4Fzgq3rxCI-oz_ej3AAA

Code:

```

var jsCode = [
	'"use strict";',

	'function Person(age) {',
	'	if (age) {',
	'		this.age = age;',
	'	}',
	'}',
	'// xyz',
	'// comment IMG: https://images.unsplash.com/photo-1579353977828-2a4eab540b9a?w=700&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8c2FtcGxlfGVufDB8fDB8fHww',
    '// abc',
	'Person.prototype.getAge = function () {',
	'	return this.age;',
	'};'
].join('\n');

// Hover on each property to see its docs!
var editor = monaco.editor.create(document.getElementById("container"), {
	value: jsCode,
	language: "javascript",
	automaticLayout: true,
	glyphMargin: true,
	contextmenu: false
});


// Add a content widget (scrolls inline with text)
var contentWidget = {
	domNode: null,
	getId: function() {
		return 'my.content.widget';
	},
	getDomNode: function() {
		if (!this.domNode) {
			this.domNode = document.createElement('div');
			this.domNode.style = "border: 2px solid red; ";
			this.domNode.innerHTML = '<img width="100px" height="100px" src="https://plus.unsplash.com/premium_photo-1680740103993-21639956f3f0?w=700&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c2FtcGxlfGVufDB8fDB8fHww" />';
		}
		return this.domNode;
	},
	getPosition: function() {
		return {
			position: {
				lineNumber: 7,
				column: 28
			},
			preference: [monaco.editor.ContentWidgetPositionPreference.ABOVE]
		};
	}
};
editor.addContentWidget(contentWidget);

monaco.editor.setModelMarkers(editor.getModel(), 'test', [{
    startLineNumber: 8,
    startColumn: 1,
    endLineNumber: 2,
    endColumn: 1000,
    message: `a message <img width="100px" height="100px" src="https://images.unsplash.com/photo-1579353977828-2a4eab540b9a?w=700&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8c2FtcGxlfGVufDB8fDB8fHww" />
	

---

![image](https://plus.unsplash.com/premium_photo-1680740103993-21639956f3f0?w=700&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c2FtcGxlfGVufDB8fDB8fHww)

YYYY
	`,
    severity: monaco.MarkerSeverity.Info
}])


var output = document.getElementById('output');
function showEvent(str) {
	while(output.childNodes.length > 6) {
		output.removeChild(output.firstChild.nextSibling.nextSibling);
	}
	output.appendChild(document.createTextNode(str));
	output.appendChild(document.createElement('br'));
}
```

Hold on! Before you start to get enthousiastic about the image that's *actually* showing up in there *immediately*: that's something else, discussed next, and comes with it's own... issues.



### monaco: contentWidgets instead?

See the last test mentioned above: there we also test monaco's **contentWidget** feature: it works. We can show an image anywhere we wish. It scrolls along with our script.

BUT... these images don't take up space in the script editor space but are shown as a bit of  OVERLAY instead. 
Naturally we could inject a bunch of empty lines to accommodate, but I that's crap and I haven't found how to make these contentWidgets sit nicely **between lines**, which is how I'ld like them to behave instead.

Of course, realizing there's those "peekables" in there, the thought is stuck in my mind that it WHOULD be possible to plug these buggers in between text lines a la **modelMarkers**!




### attempts no worky...

**createDecorationsCollection**: nah. Would've been cool, but those are limited/clipped to the bit of pixel space before and inside the lines anyway.

https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#example-interacting-with-the-editor-line-and-inline-decorations



Do note that the modelMarkers approach sounded viable at first, it turns out that you always only will see ONE of those: the others are hidden when you hover/click over the next one: see https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#example-extending-language-services-model-markers-example for this behaviour!

So you can bet / expect that positioning stuff we mentioned as a risk above only works for this "active" spot, unless someone at Microsoft has been anticipating my needs... Chances of that are relatively low.




### hoverZones DO support HTML however!

I mentioned above hoverZones; I was of the impression, thanks to SO et al, that that one liked only rather limited marked.js-processed MarkDown. Turns out I can feed it direct `<img src=...>` HTML anyway: `supportHtml: true` is the magic ingredient!

https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#XQAAAAIlBwAAAAAAAABBqQkHQ5NjdMjwa-jY7SIQ9S7DNlzs5W-mwj0fe1ZCDRFc9ws9XQE0SJE1jc2VKxhaLFIw9vEWSxW3yscxAh7jwnmJTiNdlbZ8DX-17E4KVEqPLe3831nfbDw-lE-iUSXBko7Q2YS-nwSVoWAXhhe5D5YXt58KaJh_D-K4mj8n6Sv4HQQr4ucr81HzbhscTcQYosd87FKxP54kheTCEuswzltK9cVJ5KmcX54_j-Y6KbE_9rR7MIa3ca_dUKhci7xLHqBcZemiQbOA-cH8xaXzVxLs4NJgTnVjKaaB-3j7UebZJGflEl2AR2qc4wrBIec3yLck4gnc_KGAvKZ6WN2aLRcUwkjlyHt94U9zsbZKdmfGQP9-p6kk5UoW_vz_TbtZp1zc06eUnYn7VoshJLHsXHBQUqSlZXu7kbRPzVw6ymhWDacITBYMYO1ymU2KJf4YGloFRTdGI_S5SQmE4gPjQ09wd-AZN1FA-QPaDJJucJQVH3hpKesIbKNGN7drYCRLonWNh8cM75VQSFcMdmkBW80YjJhVzU1ooAdm0V43h0YLTNnGdc-RSaltlaXMIZBZW12Q0cg6VPr7xWhm2BA69bAiHBKmdiMWeBSFcuiNCGsMHEHFPQOe_kLhbbYZ6sTJLEHfGgLmCWE4nFGFZBRUOM_UcQR5O8FqGpKwzqEMf-9UUqbqGu5YUIjTpQ8LOQIYdmOElZja3hQA6prfxJLGGWVKgli-roT9aly9vthRJjENnDW7QA8nN6eTQRa5Koyufq0nGSfRuh0aBUOvcT-qD268G1Mth6i9-0DsOWBQUbx5Tpj76dw_0D40H0bpcIpFzM4J4ckQRe15Hut7_IO6LRZFIlHk27OqBUhH09fgIH67VL7qPuVI28H2z_DQr1_eyYoR4J32lUQ0cniofMeatFPjw2aSf6VgzuO4ZZsdCRQ3XI4d88VDjqKEhPdqdwsQPDcJ4EDkg1nu_aB48x2GiLp7djzDLsltP-POr0JHNdPS1XMWIQz4ysAHNgZL1GGFViXnFM4hBn3Z0rNvG88gDyExOajcaeAnRYRw98QObL2BKOnCwcwx38209Sz3gdieP5bSxZBGqKVJEZ9sRh_A3TWO_3O1fieyJ9l9ECpu-3CmkLFVZJGlaxxnIl4zZYKeToNDbu9C7WYTWPPVXi8XoPyVse4JS9MCcAdMZRN3Jm9LYsn_uD4Pdw

```
monaco.languages.register({ id: "mySpecialLanguage" });

monaco.languages.registerHoverProvider("mySpecialLanguage", {
	provideHover: function (model, position) {
		return xhr("./playground.html").then(function (res) {
			return {
				range: new monaco.Range(
					1,
					1,
					model.getLineCount(),
					model.getLineMaxColumn(model.getLineCount())
				),
				contents: [
					{ value: "**IMAGE**" },
					{
						value:
							//"```html\n" +
							//res.responseText /* .substring(0, 200) */ +
							//"\n```" +
							`
							
XXXX

---

![image](https://plus.unsplash.com/premium_photo-1680740103993-21639956f3f0?w=700&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c2FtcGxlfGVufDB8fDB8fHww)

YYYY

---

ZZZZ


<img width="200px" height="200px" src="https://plus.unsplash.com/premium_photo-1680740103993-21639956f3f0?w=700&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c2FtcGxlfGVufDB8fDB8fHww" />

AAAAAA

---


							`,
							isTrusted: true,
							supportHtml: true
					},
				],
			};
		});
	},
});

monaco.editor.create(document.getElementById("container"), {
	value: "\n\nHover over this text",
	language: "mySpecialLanguage",
});

function xhr(url) {
	var req = null;
	return new Promise(function (c, e) {
		req = new XMLHttpRequest();
		req.onreadystatechange = function () {
			if (req._canceled) {
				return;
			}

			if (req.readyState === 4) {
				if (
					(req.status >= 200 && req.status < 300) ||
					req.status === 1223
				) {
					c(req);
				} else {
					e(req);
				}
				req.onreadystatechange = function () {};
			}
		};

		req.open("GET", url, true);
		req.responseType = "";

		req.send(null);
	}).catch(function () {
		req._canceled = true;
		req.abort();
	});
}
```




## Hola! viewZones: now *this* works! (kinda, sorta...)

https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#XQAAAALlAwAAAAAAAABBqQkHQ5NjdMjwa-jY7SIQ9S7DNlzs5W-mwj0fe1ZCDRFc9ws9XQE0SJE1jc2VKxhaLFIw9vEWSxW3yscw48-JnW5xVloiUNinCaTC6cNl7839oJjJvv-L941-J-TGThArxS5kVJY--N_Qv7TzfMGizKkI-NM9ALRiboGXq3poBlxrMgq5k9xPALbVPSAApGIziEO8GJcTMPvepJSOAlFXvwWE6NQdBI_knWq3A0SKVYExRSgjM72V0N-Vnlux2gwOMRjNsiVXxKZdQaWADbgDnc2BArjjXLiFh3mUCihMhN0SGFMvsZeO2CTnS8nFb1CC2pfBAXSEPDq6D9q0TqzpVakV7lQFakem8I89FhzNiEqrNKMyvswkt8BX8cie-tI-UDGmZ-BTRvKM5cLPY4ougRcakTmbrnd6L67fU36JLNCiXN4kAKJ9JopLWWRq4yG70jhEJSDKK3bcMc2UyjbggwuF_HtvIgmYeTg_Ug3XY81SzJPjmjPDgOZVd1oSKrDxcx2VyVAABH4byo_c_vqJSu3PxR3g-uEcNS6w20xFz8jrZuNa2flhbRYJuQry9vhSJbJvpq88kAVzKSDuAF-EjXWyc3eujyC9uAc3lgEewBe9DHBT5HOpJxi2lGirAevnF1wtV4NuyVPlhvdpt7nLWaXGhKFIHjuMuW53hKmXrkKBkounWnoBagdiifs6Mwuljo6SP4O74-nQ3ec01OHM2eh3wAnfHbg7VFJzx0yaEmxwCppbsghT0ElRnghX74MzRcrBYIE_d0s8ru0i7HlWCr5L_GGstBc5EYfNlvo2sZGXZcX6dcaMbTHZ5PpxIR3VtF1-hV6qi358VArYr-7E9ww4Qo4_9i0UVwBxNyfwmWatTu5OCDg1TI7_ic76AA

```
let ed = monaco.editor.create(document.getElementById("container"), {
	value: "1. Insert more lines here by pressing enter:\n\nZone should always be below this line.\n\n\n2. Delete most of this line to trigger ViewZones._recomputeWhitespaceProps()",
    wordWrap: "wordWrapColumn",
    wordWrapColumn: 40
});

ed.changeViewZones((vzChanger)=>{
    let domNode = document.createElement("div");
    //domNode.style.background = "#ff0000";
	domNode.style = "border: 2px solid red; background-color: rgb(210,210,210); ";
	domNode.innerHTML = `
<div  style="overflow: auto; border: 2px green solid; width: 200px; height: 100px; " >
	<img  src="https://plus.unsplash.com/premium_photo-1680740103993-21639956f3f0?w=700&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c2FtcGxlfGVufDB8fDB8fHww" />
</div>
	`;

    vzChanger.addZone({
        afterLineNumber:3,
        heightInLines: 6,
        domNode
    });
});
```

The trick here is using **viewZones**: that allows us to reserve space (so many lines of text as we think we need; 6 in the example above), it scrolls along, it is skipped by the cursor and everything looks fine.

EXCEPT... the image sample above is rather large and we forced a few scrollbars around in its outer div; nothing fancy. *However*, it turns out monaco doesn't allow us to fiddle with those scrollbars. Maybe some CSS magick I missed someplace else, bu this implies we can get away with this option, render our images "inline" as envisioned, *but we will not be able to scroll large images*, so we'ld better other ways to accomplish that if we want that.


**At least this viewZones thing looks like a viable option at least, the Markers were a nice initial idea but NOT what you'ld like (only one visible ever, and then only when you hover/click on it); the hoverContent is a nice concept too, but having to hover means we'll be only viewing a single image ever, just as with the contentMarkers approach, while *we*'ld like all rendered (sub)images inlined in the rewritten script all at once, just by running (scrolling) through such a script using the cursor keys: that would show what's going on pretty quickly and THAT is what we wish to see. Hence viewZones (one per inline server-sourced tesseract internal image) is the way to go, AFAICT.****
**




Okay, we tested a bit further with these viewZones not being clickable and everything...
It was weird as the cursor didn't change, so certainly there was some CSS preventing a bunch of stuff.
DOM inspection has the viewZone rendered in a parent DIV that's the *second* sibling *below* the transparent editor line DIV collection parent DIV, which explains why mouse events aren't really making it into our viewZone.

Anyway, bunch of rather low-success-rate CSS tweak attempts further and we're out of ideas for now: if we want to deal with anything clickable in there, we'll have to feed it through the `editor.onMouseEvent()` anyhow, is the preliminary conclusion of this event messing with monaco.

Here's the on-line test sample:

https://microsoft.github.io/monaco-editor/playground.html?source=v0.48.0-dev-20240325#XQAAAALaCQAAAAAAAABBqQkHQ5NjdMjwa-jY7SIQ9S7DNlzs5W-mwj0fe1ZCDRFc9ws9XQE0SJE1jc2VKxhaLFIw9vEWSxW3yscxHOtbQbPIG3HE_osxA-CISItMZibo7WDh_LP6GCA7ZaS7SJTYEvkI2vUYg-ZzCgBZDXJqOfAiKB_OAbjbanIc7qAV7junHA-en7kjkNOgiEXV8AeHD1PO_d2gJRJwlM3cUK1o1qQzK2X8JkrjptE9d4TDD9PqsuYGbin3Q_ucFmWS1F6LN_bKb2Gycg4xX6xKhgrkabuTQYaWxGUJRgLyWJIvj5Jp4540ET81YYHZhJE8IVAXrN-_BQMXJ5w6nbAFR4D2QoojSb7TPBJuHoAaW7htPLreeQibIdJQYB8f2SRBWfzhxjcPpzSN9ypUNvfi6I78ZSrHb7IDm1ixk1zkSpNMH2-PkaZPpibCCgcI3cArtsnwe1VVvMrxtHH8az_Lvr8yDzsbqh12C0es_vWjhV2eQ2aFdkRQwbJFRteE_SIuRSeLaywh-g9srqxZ-V44B2ZTBRH81Brr0OEYH_C2ydE2OVcZ-9M_wQ8QgP-gHV8ZfjxP4Q1gpjfxbiKoqsQVn_RaL0Yhgr7UdpF-76VcCtyI0SYeFZT0z6uoqToxlACouTpRsWZaW5DWiWOxi_Sd3xcHjpPUEVFBEwdb8SlRnQDJemJdM-NAgKidhnlbL_DY-Dlstrm3a98fq-fWly_0zbiq77R_a1Pb99ZS-ghb3DtW7uQtpZB9pctkfBgn-d_dDzrcBu75u6eTUqgMta71-C-NWssIoBLWF3eAz8684Gc_hPo9eyEkjzFrtM-NQSDBt6vJbVjzFur2brzh_-zt4KfoEGOo-afwsfbN76TYwYDEA7dam6ed9GWOOrpHuewp6sDdO0q4pWhnBKdodPIxSOUFOjURsVxyeE3RDbV5x2uiQm41q46wvbumPPm8znHsYC81Lq6B0GaA3OF_kQvcYzMqL2N2isvbJMMWWuaPNKsltX9OEdEyhxYfv-4A3Q_ceuYDs1aYzfkTaZ8GTi3jlLRsbrYATc0cB8eFY2zggna4VZ5nxSAgAyfVJJL6dHE2__5j_mEto6Mi3nvLzzCtlDdIytEDlYj9Flp4IM1BUOXM27-22lhSyBOI7cI2VSjAbnrgw4ZYalN5UqjomHEzP0OpFBDWOPmkFghFmjNrQfY4EpEaKsnc1kgK7cwHXd4yFJYcxcLVHSznkN311kbQSbBhZa6itvrlr-oWCht6D6AqZUl3q44iu7o5zVyBF-Kuid9Errz44oz2TBJIEMTp8-rXe_uYLJCaAsSHmh3omiRDLAJMM9HoNXt6GsDKzj1eJ5bhU7aupG-BreBPaKnE4UUMMhUsPD-duNZyxJv79rqYMFOQPBpHZ0FTFBMl4KUJowf0eqe1yBXxbl88j32-vUS7YNWID0PPlWXKR-BrD5VxVtuBbbP1n3GhGP3xlJL5Ta7rNq_Q14y-Duw6BMbP-r4gtUC9mp4bbLuaAVWMb59QTqG0ITIZOq1rkHHp0MjuMwFPicHJc1dan_kcDOQtjVpSznuT30tVhZMeyYOEGEdy7xcs6UkqjnvE9BtMR1JydKHokeDTrL8y5ZQKpUAwNgwq_H2slETMYeJUBq-f8W4pQOSIjasBwuj76349F0ekKGCsmk20M9RZN0S_k87iDUj9UmSO3xtTIhEyJeClfvY7hxXZKeEkCC1uBRlUgijyeigJEH4ffslNRdE2eQuEBpUjbU1MAhyJFEiZd2Cpuv_QFfUR2GWrmRN_qpnRXT_HsQ1ob-k5F2s83SKLok3P7UkljGiB6__rnvAk

```
let ed = monaco.editor.create(document.getElementById("container"), {
	value: "1. Insert more lines here by pressing enter:\n\nZone should always be below this line.\n\n\n2. Delete most of this line to trigger ViewZones._recomputeWhitespaceProps()",
    wordWrap: "wordWrapColumn",
    wordWrapColumn: 40
});

ed.changeViewZones((vzChanger)=>{
    let domNode = document.createElement("div");
    //domNode.style.background = "#ff0000";
	domNode.style = `border: 2px solid red; background-color: rgb(210,210,210); 
		z-index: 2; 
		pointer-events: initial !important;  cursor: pointer  !important;  text-decoration: none;   
		-webkit-user-select: initial;  user-select: initial !important; 
	`;
	domNode.innerHTML = `
<div  style="overflow: auto; border: 2px green solid; width: 500px; height: 250px; 
pointer-events: initial !important;   cursor: pointer  !important;  text-decoration: none;   
-webkit-user-select: initial;  user-select: initial !important; " >
    <p><label name"urg">Try clicking on this input file! (Tough luck without our focus code...) </label> <input id="urg" name="urg"></input>
	
	<!-- https://stackoverflow.com/questions/17711146/how-to-open-link-in-a-new-tab-in-html -->
	<p>Click link? Yeah, but still no worky!!1! 
	   <a  target="_blank" rel="noopener noreferrer" href="https://www.microsoft.com/">Microsoft</a>

	<p><img  src="https://plus.unsplash.com/premium_photo-1680740103993-21639956f3f0?w=700&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8c2FtcGxlfGVufDB8fDB8fHww" />
</div>
	`;

    vzChanger.addZone({
        afterLineNumber:3,
        //heightInLines: 6,
		heightInPx: 300,
		suppressMouseDown: false,
        domNode
    });
});


// got this hinted from looking at
// https://github.com/microsoft/monaco-editor/issues/3527
ed.onMouseDown(event => {
    if (event.target.type === monaco.editor.MouseTargetType.CONTENT_VIEW_ZONE) {
		if (false) {
	        ed.setSelection(new monaco.Selection(1,1,1,10));
			alert("bingo");
	        //ed.focus();    <-- doing this would require the fix of #3527, I'll bet!
		}
		if (true) {
			// focus on the input field inside our custom viewZone...
			let el = document.getElementById("urg");
			el.focus();
			if (event.target.element) {
				event.target.element.focus();
			}
		}
		//event.stopPropagation();   <-- no noticeable effect
		return false;
    }
});

if (false) {
  ed.setSelection(new monaco.Selection(6,1,6,21));
  ed.focus();
}
```

The `if (false) ...` chunks in there is just me messing about and trying a few things without throwing away older attempts in the same code. Yeah, it's a friggin' mess, I concur.

Anyway, the z-index on the outer viewZone DIV "unlocked" the cursor CSS statements, etc. but we're still stuck with the A-HREF now being clickable but doing absolutely *nothing*; all this stuff lands in that ed.onMouseEvent handler and that's what's killing this bunch, or at least the outer monaco event handler is the culprit AFAICT, without digging into the editor code.

BTW: the scrollbars around the image (in the inner DIV) do work now when you click and drag on them with the mouse, but scroll-wheel still isn't coming through -- surely something more that's impacted by the monaco-internal mouse/wheel handler; I couldn't find a fix for this quickly enough, so the preliminary conclusion there is unchanged from a while back: show the images as-is, at full size if possible and DO NOT expect them to be clickable, scalable, or anything else that would require easy user access to the image without the page -- unless we RTFC/hack the monaco editor code to change this and right now I'm not up for that...


For now, the results are good enough that this editor is indeed usable for our (tesseract-centered) needs.








## ... and what if we go with JupyterLab / Jupyter anyway?



And when we go the JupyterLab anyway: see https://jupyterlab.readthedocs.io/en/stable/extension/extension_dev.html#prebuilt-dev-workflow for some basic info; I imagine we would then have to do most of this stuff as "pre-built extension(s)" as source-level extensions are not advised in multi-user (administrated) environments, while pre-built extensions can be installed per-user (and thus per-project) there.





## References

The stuff we've been looking at thus far:


microsoft/monaco-editor: A browser based code editor
https://github.com/microsoft/monaco-editor?tab=readme-ov-file

Monaco Editor
https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#XQAAAAJrCQAAAAAAAABBqQkHQ5NjdMjwa-jY7SIQ9S7DNlzs5W-mwj0fe1ZCDRFc9ws9XQE0SJE1jc2VKxhaLFIw9vEWSxW3yscxGkzsR5v06HEtAeRia3pQhA_FGNfd48ki5iBSj69vVqCq-ZyKaia3bbl-tazfoxn7Gs_XoE_AdAk5gKpR8gLgaaNASA_LDiMABc4SJE3339888DZ8OQUrh-cJ8abYrl5f6RaEXAUQ5c9xbbXhgyGfniZlG_HS_WCaXTu_3spYEMVyF_-ZlA1SNvoiu7oFGqgoo3nzlplJgNnE7ZdMb3dr8V0IZ1_uwGXwAf7Rdvz4zy5Y3Kr4H-P-zqTAntbOffCHRB9fFH7PPDiswLT7pC3fL1FK9M4R2jvi93s0fEs6Xsi6gLq02wre_cwNF_TLl6kM2teWLdKLiFM-wwD0P9r4lSfX76AzDwJTNnYDOl5kN8X_eaydErpH8MCQYYyo43p5CKhjBDhUOPp_S9pVBAVk15dFKjNlgkEEC540perotxBNh4msoBpL7_NcY8LC8ISkfU2TOdN5BT9dhevUZ2Ink0wN61CnMrplY-DFLfUKY7E0gDFNjIV7Fd3dD5S63zMqtGLyMvfwgIygb4o1RmgEh_KeiNWVjInpygyzeKCyvHXelTsehsV79RZBBVQmyVSFfrtIHCokJbsHzjG1n6ugPdAJYbb7OO0jU5kBOEuD8QXqdXi14D2jEjXN6IeLz5zgCSPY6OrVWY147eFtbh_UYZSyuXSfNjNYddzbB-X_Tbh2uKNYjvPJeMdj2e7qlEVHEnuf0JSbeX-TCMVi4w_zWc6MyPy9fb1qOVIzmfNBquoQD7o2aJhKK1kVvJb0tjx7ixDD2ieOTHObiPM2hzBnfUsIAk4tcrdW7S0hhP00IPctm0K1x6KQOKws903ZNYV8SoOAgcFfS-H5idTfiHAXGq34ggelwK5J4J4MSfnYvfZIX8my9Hhpi7ef3LK8HXgqO-Osq_heCfdnuojZrREjtVNYoK-Tfbc6xfBRK4KnoE92ED-C7DUYzudG0e5nRvzruVB_lxJmdApdwZ-tqqi12Q4Z6wvGXUNra1CznQa_W4LJvM8xTGJfCKl4Tsy_Hf3vXmFPStc7jq6R5ygusB3VwAFClLTcuOm5G8kHWV-SZ_P_DRPr4OMS3XNC7nbF4a69endPmvmU7LmwERKasmBuXUjjhFnJv13QGDgAVgMeOFJdfoUl-88SVKnfRKmaUzNk-5z7xx336QjdPrwOM38Faib4LFjnMIBg8aycLKibsTMpybs_snA8wMUlTL2E0xfRwSuTnmKd_wDi1GxgT0DYQgU6Ifegb27caPyDGzguRYkZ9m8Ywtjjs4h-NDmiLlAR5lCPkIXRJXnkqwopqkpambfmqKrLzp5rJGMFVfk98QbrJ4pl63pwilodUi4Fzgq3rxCI-oz_ej3AAA

Monaco Editor
https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#XQAAAAIlBwAAAAAAAABBqQkHQ5NjdMjwa-jY7SIQ9S7DNlzs5W-mwj0fe1ZCDRFc9ws9XQE0SJE1jc2VKxhaLFIw9vEWSxW3yscxAh7jwnmJTiNdlbZ8DX-17E4KVEqPLe3831nfbDw-lE-iUSXBko7Q2YS-nwSVoWAXhhe5D5YXt58KaJh_D-K4mj8n6Sv4HQQr4ucr81HzbhscTcQYosd87FKxP54kheTCEuswzltK9cVJ5KmcX54_j-Y6KbE_9rR7MIa3ca_dUKhci7xLHqBcZemiQbOA-cH8xaXzVxLs4NJgTnVjKaaB-3j7UebZJGflEl2AR2qc4wrBIec3yLck4gnc_KGAvKZ6WN2aLRcUwkjlyHt94U9zsbZKdmfGQP9-p6kk5UoW_vz_TbtZp1zc06eUnYn7VoshJLHsXHBQUqSlZXu7kbRPzVw6ymhWDacITBYMYO1ymU2KJf4YGloFRTdGI_S5SQmE4gPjQ09wd-AZN1FA-QPaDJJucJQVH3hpKesIbKNGN7drYCRLonWNh8cM75VQSFcMdmkBW80YjJhVzU1ooAdm0V43h0YLTNnGdc-RSaltlaXMIZBZW12Q0cg6VPr7xWhm2BA69bAiHBKmdiMWeBSFcuiNCGsMHEHFPQOe_kLhbbYZ6sTJLEHfGgLmCWE4nFGFZBRUOM_UcQR5O8FqGpKwzqEMf-9UUqbqGu5YUIjTpQ8LOQIYdmOElZja3hQA6prfxJLGGWVKgli-roT9aly9vthRJjENnDW7QA8nN6eTQRa5Koyufq0nGSfRuh0aBUOvcT-qD268G1Mth6i9-0DsOWBQUbx5Tpj76dw_0D40H0bpcIpFzM4J4ckQRe15Hut7_IO6LRZFIlHk27OqBUhH09fgIH67VL7qPuVI28H2z_DQr1_eyYoR4J32lUQ0cniofMeatFPjw2aSf6VgzuO4ZZsdCRQ3XI4d88VDjqKEhPdqdwsQPDcJ4EDkg1nu_aB48x2GiLp7djzDLsltP-POr0JHNdPS1XMWIQz4ysAHNgZL1GGFViXnFM4hBn3Z0rNvG88gDyExOajcaeAnRYRw98QObL2BKOnCwcwx38209Sz3gdieP5bSxZBGqKVJEZ9sRh_A3TWO_3O1fieyJ9l9ECpu-3CmkLFVZJGlaxxnIl4zZYKeToNDbu9C7WYTWPPVXi8XoPyVse4JS9MCcAdMZRN3Jm9LYsn_uD4Pdw

javascript - Adding inline widget in monaco editor - Stack Overflow
https://stackoverflow.com/questions/68872342/adding-inline-widget-in-monaco-editor

javascript - How to insert snippet in Monaco Editor? - Stack Overflow
https://stackoverflow.com/questions/48212023/how-to-insert-snippet-in-monaco-editor?rq=4

monaco editor suggestion popup text is not visible - Stack Overflow
https://stackoverflow.com/questions/48592605/monaco-editor-suggestion-popup-text-is-not-visible?rq=4

javascript - How do I insert text into a Monaco Editor? - Stack Overflow
https://stackoverflow.com/questions/41642649/how-do-i-insert-text-into-a-monaco-editor?rq=3

Unable to get the text inside the monaco editor using protractor - Stack Overflow
https://stackoverflow.com/questions/41040985/unable-to-get-the-text-inside-the-monaco-editor-using-protractor?rq=3

javascript - Protractor - How to store the value of browser.executeScript in variable? - Stack Overflow
https://stackoverflow.com/questions/38100810/protractor-how-to-store-the-value-of-browser-executescript-in-variable

javascript - Microsoft Monaco Editor in browser get value of line - Stack Overflow
https://stackoverflow.com/questions/38674092/microsoft-monaco-editor-in-browser-get-value-of-line?rq=3

Monaco editor hoverMessage not showing - Stack Overflow
https://stackoverflow.com/questions/69616905/monaco-editor-hovermessage-not-showing?rq=3

Monaco Editor
https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#example-interacting-with-the-editor-line-and-inline-decorations

How to allow completion suggestions to appear while inside a snippet in Monaco Editor? - Stack Overflow
https://stackoverflow.com/questions/62325624/how-to-allow-completion-suggestions-to-appear-while-inside-a-snippet-in-monaco-e?rq=3

javascript - Monaco editor - integration with JSHint - Stack Overflow
https://stackoverflow.com/questions/51537491/monaco-editor-integration-with-jshint?rq=4

Develop Extensions — JupyterLab 4.1.5 documentation
https://jupyterlab.readthedocs.io/en/stable/extension/extension_dev.html#prebuilt-dev-workflow

javascript - Monaco editor change custom types behaviour - Stack Overflow
https://stackoverflow.com/questions/52597869/monaco-editor-change-custom-types-behaviour?rq=4

javascript - How do you set monaco-editor from readonly to writeable? - Stack Overflow
https://stackoverflow.com/questions/45129541/how-do-you-set-monaco-editor-from-readonly-to-writeable?rq=3

reactjs - How to add a custom UI/intellisense inside Monaco editor? - Stack Overflow
https://stackoverflow.com/questions/69772578/how-to-add-a-custom-ui-intellisense-inside-monaco-editor?rq=4

Need Monaco Editor For Custom Expression - Stack Overflow
https://stackoverflow.com/questions/67826759/need-monaco-editor-for-custom-expression?rq=3

Monaco Editor
https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#example-extending-language-services-custom-languages

Monaco Editor
https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#XQAAAAJvDAAAAAAAAABBqQkHQ5NjdZ76Sv_fvV7aSEHtH3wvJgbIGfhhbNxsZorBCr68QX8cj7riltqfDh-psYMjmtWwnRYsGiPX26BJejXF_mIjvZQ2fKjeLNnP8nt-t45EiLCJZMSAfWWvU5C8yZgTjkPOUTss_WG0kKvQqCw5KJnRLUYXn9yCfEpJ3m5ddv-XaS-6zrhZnA-EHwIJIxDuVyyDj57-NnkqeLsASf6qIqb6mNyY7hIofMR1u2X7APhqahfV_GzF8vjP4pG8Y226yTE1LRUlMFswqi7-xup99sVNch_LyePaf9lVpyJuh5ICEA6yC8vhIxNPYFX-FPaY2IdMXLMb_qU-FnhuFNqd0G_YO7YjfwsctIwsQlJTCmuXD1ZAfNYaZ29T7gfMl4-UHJ6SI1SNB9Wbd4b-BnmbtEsbw4B7oS56yqO2GGZbV7RmHY6ZLxJK_B1oKraLG7XVaOQpVC-3CdTuhe_CNLFt6FbkfZSLph3bLlP11d4s9ChLruaIWP7LVz2zqryUQNCM_DXnWbE1bsF4yXiRRK6mWF31tzo2Vp1L62DlIMbVQ06zUVCHiO4XKmUf6vSJLrt8j0MPuZHgPKX7Ocybqq_mmr-0diyRMNUvCnI4T37940sYqGmW_nz3KLJ29RMqbQTkO0kaORnKK1RAdH2rH7tqikLM0N1uzHTAyXraypuUGAP1PHYB1tpDH6FpzYhymaOU5QEu3nk3mV2k_DCKkwtfRCj8sRmPzr1eMJKQUtHvC4iP8V9Q8mhbtPm_oRWIt5gP-TPE_xbeFpaU5IkMsJRqBEtqJFl3rs7F3JdmgduJxCUPcSJ_BogMCVN6dg-uQfq9ofT9Cpo5ns340RmHB7Lkj0reTknH61tUrjlINyEO6xxOQYaG8lYtO7fmizW2a64sPfjCGNNZ8J4pFzB9OnWnrmd4NayJHMhZqTNMdmmh9hsffLbg9h3aWBw2e4pxOr_oQTdVGmBbbX2QJh_bBi7iYvwAcXXq_1dCvPVeHI1NeLqTGRjTTW7AftqsvYahAX0MDG15CsMk4NITF6CVg7tjuwGl_zvbC1XtkKBLGzMU-E3muogaP03Wv7t79nPhJI1krKMFXkEXtI54r60txFbXUmp67FhtKNST__okoRaDscB3MhnVBtzRdBJGBxH3On_4ZMLeBPYX6MxrNZLL0czb7zK98ExwqUWNWRRTu53Bh0PvhN7a_NYlVPSLLlQ2v09XdgB354_TYSpxPilgip6YSYMkBV7gohC203Rb2Qjwt-9hBz09gjlLJ0Til7JmZAU9--qYYbwdHwjoO79Jn_LQOvwnclGuge9C8-b_r_Ilb-7FeN8IN_8eThY_igWfZRXC0y6novIwIEym5VQjiTERp6JbQlcMTTgAXgYBoHJlY2Ix9eqYa2p953kiLwNiNwBf1EVGBjaY8uHeRRQaCTKqOprVzX8xqN9XAtfSMk88YkYyY1aXDiRLffowlVCpIDPkEEV--jSskO-pZedoAZ9Kl6eeYDDYui8r1Qshq7ToghBuMkt__bt_zA

Monaco Editor
https://microsoft.github.io/monaco-editor/playground.html?source=v0.47.0#XQAAAALlAwAAAAAAAABBqQkHQ5NjdMjwa-jY7SIQ9S7DNlzs5W-mwj0fe1ZCDRFc9ws9XQE0SJE1jc2VKxhaLFIw9vEWSxW3yscw48-JnW5xVloiUNinCaTC6cNl7839oJjJvv-L941-J-TGThArxS5kVJY--N_Qv7TzfMGizKkI-NM9ALRiboGXq3poBlxrMgq5k9xPALbVPSAApGIziEO8GJcTMPvepJSOAlFXvwWE6NQdBI_knWq3A0SKVYExRSgjM72V0N-Vnlux2gwOMRjNsiVXxKZdQaWADbgDnc2BArjjXLiFh3mUCihMhN0SGFMvsZeO2CTnS8nFb1CC2pfBAXSEPDq6D9q0TqzpVakV7lQFakem8I89FhzNiEqrNKMyvswkt8BX8cie-tI-UDGmZ-BTRvKM5cLPY4ougRcakTmbrnd6L67fU36JLNCiXN4kAKJ9JopLWWRq4yG70jhEJSDKK3bcMc2UyjbggwuF_HtvIgmYeTg_Ug3XY81SzJPjmjPDgOZVd1oSKrDxcx2VyVAABH4byo_c_vqJSu3PxR3g-uEcNS6w20xFz8jrZuNa2flhbRYJuQry9vhSJbJvpq88kAVzKSDuAF-EjXWyc3eujyC9uAc3lgEewBe9DHBT5HOpJxi2lGirAevnF1wtV4NuyVPlhvdpt7nLWaXGhKFIHjuMuW53hKmXrkKBkounWnoBagdiifs6Mwuljo6SP4O74-nQ3ec01OHM2eh3wAnfHbg7VFJzx0yaEmxwCppbsghT0ElRnghX74MzRcrBYIE_d0s8ru0i7HlWCr5L_GGstBc5EYfNlvo2sZGXZcX6dcaMbTHZ5PpxIR3VtF1-hV6qi358VArYr-7E9ww4Qo4_9i0UVwBxNyfwmWatTu5OCDg1TI7_ic76AA

reactjs - Monaco editor intellisense font sizing wrong making it unusable - Stack Overflow
https://stackoverflow.com/questions/69270815/monaco-editor-intellisense-font-sizing-wrong-making-it-unusable?rq=3

css - How can I make the monaco editor size to its parent div? - Stack Overflow
https://stackoverflow.com/questions/69280251/how-can-i-make-the-monaco-editor-size-to-its-parent-div?rq=3

javascript - How can I set the tab width in a monaco editor instance? - Stack Overflow
https://stackoverflow.com/questions/41107540/how-can-i-set-the-tab-width-in-a-monaco-editor-instance?rq=3

css - Let Monaco Editor fill the rest of the page (cross-browser) - Stack Overflow
https://stackoverflow.com/questions/45625660/let-monaco-editor-fill-the-rest-of-the-page-cross-browser?rq=3

How do I auto-resize an image to fit a "div" container? | Sentry
https://sentry.io/answers/how-do-i-auto-resize-an-image-to-fit-a-div-container/

monaco editor dynamic set viewzone's height - Stack Overflow
https://stackoverflow.com/questions/66795742/monaco-editor-dynamic-set-viewzones-height

View Zones move to wrong position · Issue #1858 · microsoft/monaco-editor
https://github.com/microsoft/monaco-editor/issues/1858

ICodeEditor | Monaco Editor API
https://microsoft.github.io/monaco-editor/typedoc/interfaces/editor.ICodeEditor.html

Hidden areas and view zones mess the line numbers · Issue #1307 · microsoft/monaco-editor
https://github.com/Microsoft/monaco-editor/issues/1307

Support image resizing · Issue #339 · markedjs/marked
https://github.com/markedjs/marked/issues/339

How to override render method of `<img>` · Issue #774 · markedjs/marked
https://github.com/markedjs/marked/issues/774

500+ Sample Images | Download Free Pictures & Stock Photos On Unsplash
https://unsplash.com/s/photos/sample

codemirror/dev: Development repository for the CodeMirror editor project
https://github.com/codemirror/dev/

glorious-codes/glorious-demo: The easiest way to demonstrate your code in action.
https://github.com/glorious-codes/glorious-demo

glorious-codes/glorious-demo: The easiest way to demonstrate your code in action.
https://github.com/glorious-codes/glorious-demo#installation

judge0/ide: ✨ Simple, free and open-source online code editor.
https://github.com/judge0/ide

kazzkiq/CodeFlask: A micro code-editor for awesome web pages.
https://github.com/kazzkiq/CodeFlask

kazzkiq/CodeFlask: A micro code-editor for awesome web pages.
https://github.com/kazzkiq/CodeFlask

wanglin2/code-run: 一个代码在线编辑预览工具，类似codepen、jsbin、jsfiddle等。
https://github.com/wanglin2/code-run

ternjs/tern: A JavaScript code analyzer for deep, cross-editor language support
https://github.com/ternjs/tern

purocean/yn: A highly extensible Markdown editor. Version control, AI Copilot, mind map, documents encryption, code snippet running, integrated terminal, chart embedding, HTML applets, Reveal.js, plug-in, and macro replacement.
https://github.com/purocean/yn

nteract/hydrogen: :atom: Run code interactively, inspect data, and plot. All the power of Jupyter kernels, inside your favorite text editor.
https://github.com/nteract/hydrogen

premieroctet/openchakra: ⚡️ Full-featured visual editor and code generator for React using Chakra UI
https://github.com/premieroctet/openchakra

antonmedv/codejar: An embeddable code editor for the browser 🍯
https://github.com/antonmedv/codejar

microsoft/TouchDevelop: TouchDevelop is a touch-friendly app creation environment for iPad, iPhone, Android, Windows, Mac, Linux. Our mobile-friendly editor makes coding fun, even on your phone or tablet!
https://github.com/microsoft/TouchDevelop

Graviton-Code-Editor/Graviton-App: 🚀 A modern-looking Code Editor
https://github.com/Graviton-Code-Editor/Graviton-App

live-codes/livecodes: Code Playground That Just Works!
https://github.com/live-codes/livecodes

Bridgewater/scala-notebook: Interactive Scala REPL in a browser
https://github.com/Bridgewater/scala-notebook

JonyEpsilon/gorilla-repl: A rich REPL for Clojure in the notebook style.
https://github.com/JonyEpsilon/gorilla-repl

