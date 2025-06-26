
# `codestripper`






Example process:

- keep only comments and function/class prototypes, i.e. only lines which start at left edge. 
- keep left-edge C multi-line comments. 

- 'trim' sequences of empty lines.

- Ditch code scopes: `{ ... }`
- Discard preprocessor statements that are not `#define`.
- Discard `extern "C" {` and anonymous `namespace` statements.

```
cat "$2" | sed -E -e 's/^ [*]/.*/' -e 's/^[[:space:]{}].*$//' -e 's/^#[^d].*$//' -e 's/^extern "C" [{]\s*$//' -e 's/^namespace$//' | uniq
```

