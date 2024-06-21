# x

See also the Sancho Panza repo and the `preamble.tex` therein.

Here's an extract from that one, as I took notes and jotted them down there first:

---


You must use XELATEX for the font stuff to work.

Also notes for Windows:

Install the Iosevka fonts in the `C:/texlive/2024/texmf-dist/fonts/truetype/public/` fonts directory where the others already are.) 
It can be obtained at https://github.com/be5invis/Iosevka -> Releases.

Run `fc-cache -f --verbose` to refresh the XeTeX / fontconfig cache (as seen at https://github.com/NixOS/nixpkgs/issues/24485, but this works for a TeX install on MSWindows too, fortunately)

**WARNING**:  execute the above command in an ADMINISTRATOR command shell or you will be treated to all sorts of `fontconfig` crashes and "*Fontconfig error: No writable cache directories*"!

Check the Iosevka font is reachable by inspecting the output of this command: `fc-check Iosevka`, which must say something like this:

```
Iosevka-Regular.ttc: "Iosevka" "Regular"
```

NOTE that before I got the fontcache to update properly, that same `fc-check Iosevka` command produced this nonsense:
```
CascadiaCode-Regular.otf: "Cascadia Code" "Regular"
```
so be careful to READ what is output as the errors (like this one) are very much non-obvious.

Also note that dropping the fonts in the Windows Fonts directory didn't seem to produce the desired result (but then I still was struggling with fc-cache so that may be the important factor),
also note that editing the `Path` option in the `\setmonofont` statement below didn't deliver either as it all seems to always harken back to the (headache) fontconfig configuration of your local TeX rig.



Incidentally, when you're fiddling with this preamble stuff, it's much faster to just tweak the generated `.tex` file until you're satisfied.

The command to produce a PDF file straight off that one is:

      xelatex -no-shell-escape probriskreward-bookdown.tex


```
% Iosevka-Regular.ttc, Iosevka-*.ttc
\setmonofont{Iosevka}[
  % Color = {FF1934} ,
  % Path = {./PkgTTC-Iosevka/} ,
  % Extension = .ttc ,
  % UprightFont = *-Light ,
  % BoldFont = *-Regular ,
  % FontFace = {k}{n}{*-Heavy} ,
  Scale = MatchLowercase ,
  % Scale = 0.85 ,
  % Ligatures = TeX ,
  % Contextuals = {Alternate} ,
]
% \setmonofont[ItalicFont={Consolas Italic},BoldFont={Consolas Bold},BoldItalicFont={Consolas Bold Italic}]{Consolas}
```

...

