# git :: recovering from b0rked repos and systems

Had a fatal systems failure while running MuPDF (Qiqqa-related) bulk tests overnight.

End result: cold hard boot was required (which gave a dreaded win10 BSOD ("Page Fault") during intial bootup, so that was a sure sign things had gone the way of the Dodo...)

End result: Qiqqa + MuPDF git repos are b0rked; bad ref reports, tortoisegit crashes on startup, etc.

My own (older) tooling using `gpp -c` ([in-house developed scripts](https://github.com/GerHobbelt/developer-utility-commands)) didn't recover.

## Pages that helped resolve:

- https://github.com/desktop/desktop/issues/5438 ("error: cannot lock ref 'refs/remotes/origin/") --> that one mentions deleting refs/remotes/XYZ/ entirely, like many others. After several runs of `git fetch --all` and AGAIN deleting refs/remotes/XYZ for all XYZ that reported `bad sha` or `bad ref` *gradually* improved matters.
- https://stackoverflow.com/questions/11796580/git-pull-error-error-remote-ref-is-at-but-expected#
- https://stackoverflow.com/questions/18563246/git-gc-error-failed-to-run-repack-message
- https://stackoverflow.com/questions/59347462/how-to-resolve-git-reporting-invalid-sha1-pointer-000000000000000000000000000000
- https://stackoverflow.com/questions/9955713/git-dangling-blobs --> make that a `git gc --prune=now --aggressive`, by the way.
- **do NOT STOP until `git fsck --full --strict` is completely satisfied and doesn't yak about anything any more!**
- the relevant part of the `gpp -c` script:

  ```sh
  c )
  echo "--- clean up the git submodules remote references etc. ---"
  for (( i=OPTIND; i > 1; i-- )) do
    shift
  done
  #echo args: $@
  for f in $( git submodule foreach --recursive --quiet pwd ) ; do
    pushd .                                                               2> /dev/null  > /dev/null
    echo processing PATH/SUBMODULE: $f
    cd $f
    #echo $@
    $@
    # http://kparal.wordpress.com/2011/04/15/git-tip-of-the-day-pruning-stale-remote-tracking-branches/
    # http://stackoverflow.com/questions/13881609/git-refs-remotes-origin-master-does-not-point-to-a-valid-object
    git gc
    git fsck --full --unreachable --strict
    git reflog expire --expire=0 --all
    git reflog expire --expire-unreachable=now --all
    git repack -d
    git repack -A
    #git update-ref
    git gc --aggressive --prune=all
    git remote update --prune
    git remote prune origin
    popd                                                                  2> /dev/null  > /dev/null
  done
  echo processing MAIN REPO: $wd
  $@
  git gc
  git fsck --full --unreachable --strict
  git reflog expire --expire=0 --all
  git reflog expire --expire-unreachable=now --all
  git repack -d
  git repack -A
  #git update-ref
  git gc --aggressive --prune=all
  git remote update --prune
  git remote prune origin
  ;;
  ```
- it helps *oodles* if you previously ran the `collect_git_remote_add_recusively.sh` ([in-house dev](https://github.com/GerHobbelt/developer-utility-commands)) script to generate a `util/...` shell script to *re*-register git remotes as I had to nuke the important ones locally in refs/remotes/ before I could anywhere near a fixable solution. Otherwise it's back to `git remote add ...` after the deleting when `git fetch` goes t\*ts up and you need to kill more to appease `git fsck` + `git gc`.
- https://stackoverflow.com/questions/3765234/listing-and-deleting-git-commits-that-are-under-no-branch-dangling --> to help delete those "unreachable" blobs and commits too.
- https://stackoverflow.com/questions/10570319/cannot-pull-git-cannot-resolve-reference-orig-head
