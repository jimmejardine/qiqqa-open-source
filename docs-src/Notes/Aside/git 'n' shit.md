or rather: *github* 'n' shit.

## WARNING: github LFS 

Yet another reason to stay away from git LFS: github's payment model & collateral.

This will happen to you too: 

```
batch response: This repository is over its data quota. 
Account responsible for LFS bandwidth should purchase more data packs 
to restore access.
```

Yeah, you need to pay until Kingdom Come for *anyone* to be able to reach your stuff.
This is observed at [the virtualanup/nepalingram repo](https://github.com/virtualanup/nepalingram) but the more important key take-away here is: GitHub LFS support is behind a paywall, *always* and *forever*. So stay the heck away from it!

I know this is suboptimal as you'll have to hard limit your files to a reasonable size (a little above 15 MByte), but I consider that minor effort to prevent future unrecoverable mishaps.
This approach also means you can more easily transfer/mirror your git repositories at other sites if/when the individuals at Github decide to cross over to The Dark Side: no LFS means you only need basic git repo support and there's plenty of that around.

### Update

And another one popped up on my radar here while I was writing that bit above:

```
Smudge error: Error downloading corpus-ver-3.0/iSAI-NLP2020-
paper-experiment/[...]/sample_data/lstmcrf.dset 
(6f05ccb540ecc66968d41e24bec772c17eee6b0ff00016e1d8a9115b5a0145ff)
: batch response: This repository is over its data quota. Account 
responsible for LFS bandwidth should purchase more data packs 
to restore access.
[...]
Errors logged to 'G:\prj-chk\myPOS___ye-kyaw-thu\.git\lfs\logs\20250330T154000.7074008.log'.
Use `git lfs logs last` to view the log.
error: external filter 'git-lfs filter-process' failed
fatal: corpus-ver-3.0/iSAI-NLP2020-paper-experiment/[...]/sample_
data/lstmcrf.dset: smudge filter lfs failed
```
(repo: https://github.com/ye-kyaw-thu/myPOS)

... what have I just told you?!




