# Unicode ~~replacement~~ ~~alternative~~  *homoglyphs* for \[...\] square brackets in MarkDown

Because I'm kinda lazy and slightly irritated at having to ugly-`\\`-backslash-escape the square brackets when I want them to appear as-is.

First, the general I-am-looking-for-??? Unicode glyph/codepoint site that works well (best) for me is: https://symbl.cc/

But finding homoglyphs when you need/want them is still a bit of a hassle as the added/follow-up problem is: does the codepoint I selected as a homoglyph-du-jour actually *exist* in my display/screen font? Sometimes it doesn't, so the process becomes iterative. Alas.

Some homoglyph lists:

- http://xahlee.info/comp/unicode_look_alike_math_symbols.html
- https://gist.github.com/StevenACoffman/a5f6f682d94e38ed804182dc2693ed4b
- https://github.com/codebox/homoglyph
- https://github.com/life4/homoglyphs
- and the inverse: getting back to the base/ASCII form: https://github.com/nodeca/unhomoglyph, which happens to reference the very useful **official set of Unicode Confusables**: [Recommended confusable mapping for IDN](http://www.unicode.org/Public/security/latest/confusables.txt)

Anyway, let's see what we got for the `[` bracket glyph....

- `[`: "\[...]" 
- `⟦`: "⟦..." -- https://symbl.cc/en/27E6/ "Mathematical Left White Square Bracket"
- (note the ugly extra left-side whitespace occupied by the codepoint) `〚`: "〚....]" -- https://symbl.cc/en/301A/ "Left White Square Bracket"
- (uglier due to minimal underline-like cruft...) `⦋`: "⦋...]" -- https://symbl.cc/en/298B/ "Left Square Bracket with Underbar"
- - `⦍`: "⦍...]" -- https://symbl.cc/en/298D/ "Left Square Bracket with Tick In Top Corner"
- - `⦏`: "⦏...]" -- https://symbl.cc/en/298F/ "Left Square Bracket with Tick In Bottom Corner"

and the other one of the 'matched set':

- `]`
- `⟧`: https://symbl.cc/en/27E7/ "Mathematical Right White Square Bracket"

(... TODO ....)
