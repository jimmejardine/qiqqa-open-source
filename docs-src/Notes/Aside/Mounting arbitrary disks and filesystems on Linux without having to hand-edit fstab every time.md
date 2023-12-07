# The problem

As this question (and a few others on SO sites) hit very close to the mark of my particular problem criteria: 

1. **mount arbitrary format disks/partitions (ntfs, btrfs, ext4, fat32, ...), without modifying/editing `fstab` in any way** and no need for the user to know exactly what it should be.
1. **does not use any GUI** (the PyDSM answer above calls it "apps"); commandline only, nothing else suffices! 
   + bonus points: can be executed via a regular `ssh` session: commandline FTW!
1. **do not clobber existing mount points**, please
1. **fully automated, no questions asked, no manual prep (`mkdir` et al) _at all_!**
1. **mounts all into `/media/...` tree**; just like your GUI tools do, e.g. Nemo
    * would be *great* if this mimics those click-and-go GUI tools to a tee.

and the working answer is not provided here, but only the start of it ("using `blkid`"), here's what I came up with:

# Solution

## A *brutal* commandline to automatically mount all yet-unmounted drives

```
blkid | grep /dev/sd | sed -e 's/:.*$//' | xargs -n 1  udisksctl  mount --no-user-interaction -b
```

## FAT WARNING

**This ain't for the squeamish. Read the sections below for info on what happens here and why I did it this way; adapt to suit your own needs.**

Execute this commandline as user `root` to get the full effect. (At least on my Linux Mint, I had to `sudo bash` 😉︎ after `ssh`-ing into the box from a second node on the local air-gapped network.)


# Analysis

## What happens and why is that bit in there?

### `blkid`

This is the one that starts it all. Others mention this tool to see the UUID; others for the partition label; I'm not interested in those unless they are useful to pump into an automounter. That took some doing, as I had to trawl through several dozen pages on and off SO before I ran into https://askubuntu.com/questions/365052/how-to-mount-drive-in-media-username-like-nautilus-does-using-udisks, which was enough of a hint, plus unix manpages after that.

To give you some idea of what `blkid` output looks like, here's the raw output from a off-net rig that's used to mount disks for recovery operations. Caveat: *security* is **not** an item here; I want/need access to *everything*: *maximum access*. If you need a more restricted approach, you've got some more research work to do...

```
root@ger-GA-A75M-UD2H:/home/ger# blkid
/dev/sdf1: LABEL="USB8" BLOCK_SIZE="512" UUID="2AC8D135C8D1004D" TYPE="ntfs" PARTUUID="6540671e-01"
/dev/sdd2: LABEL="16TB2303A" BLOCK_SIZE="512" UUID="34FC2444FC24032A" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="ba9d412f-6b87-4ca6-bd31-2b697b141049"
/dev/sdb1: LABEL="16TB-btrfs02" UUID="70f2d37d-2534-4dca-ac06-ce31a3f9c56b" UUID_SUB="8bc3e3f3-cdc9-4b79-8506-a763fed2c8f0" BLOCK_SIZE="4096" TYPE="btrfs" PARTLABEL="16TB-btrfs2" PARTUUID="71c59478-0527-42e0-ab31-df549cf3cf8f"
/dev/sdc2: LABEL="4TB201701" BLOCK_SIZE="512" UUID="863EC5FD3EC5E671" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="69b817eb-0614-47de-9266-6c1d3de5a6fa"
/dev/sda2: UUID="BFCB-C886" BLOCK_SIZE="512" TYPE="vfat" PARTLABEL="EFI System Partition" PARTUUID="ad819fa6-308b-4302-b476-205f58ab23aa"
/dev/sda3: UUID="cefb8307-2722-4b89-b66f-692251cb9ce7" BLOCK_SIZE="4096" TYPE="ext4" PARTUUID="afc3f6dc-afc7-42d2-8eb2-21603d6b4b37"
/dev/sdi2: LABEL="3TB002" BLOCK_SIZE="512" UUID="2ED8898CD889534F" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="29fafbfb-94a7-4c3d-baae-abdc46c663d4"
/dev/sdg1: LABEL="16TB-linux001" UUID="171163e9-539a-4369-b273-e186f5509b2d" UUID_SUB="a0b8c894-3b6d-49c5-a378-4fd25b963e0c" BLOCK_SIZE="4096" TYPE="btrfs" PARTUUID="fc303633-6b39-db46-9355-0f1be2f3ff81"
/dev/sdh2: LABEL="4TB0A1" BLOCK_SIZE="512" UUID="F07ABFBA7ABF7C42" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="aff0d461-a492-4ce9-8203-395e9be9009c"
/dev/sdd1: PARTLABEL="Microsoft reserved partition" PARTUUID="d80e5508-4829-43ca-a67d-88119a509973"
/dev/sdi1: PARTLABEL="Microsoft reserved partition" PARTUUID="ad9896cc-50f9-4183-bda8-3d8e2348f511"
/dev/sdc1: PARTLABEL="Microsoft reserved partition" PARTUUID="e6b4be16-b5f7-4fc5-a995-f9e81a6914f7"
/dev/sda1: PARTUUID="c65d2d74-e6c9-416f-b6a5-cc1bd4783646"
/dev/sdh1: PARTLABEL="Microsoft reserved partition" PARTUUID="f86513b7-0fa5-4b2e-9a78-c835752247ac"
```

This lists a mish-mash of connected hardware: an internal SSD drive for the OS (boot drive), a scratch disk and the rest of 'em are USB-connected drive bays with a few harddisks dropped in. **Those** are the ones I want auto-mounted onto `/media/...` directories.


### `grep /dev/sd`

That's me attempting to "err on the side of safety" as this commandline will end up in a simple, fast script, and I don't want any nasty surprises when I happen to have jacked in other non-disk-y hardware several months later, so I filter on `sd*` devices: that should impf me against most nasties, I expect. 

Broad strokes, here, not going for perfect 10.

This does not modify the above list.



### `sed -e 's/:.*$//'`

As I wrote this **after** having fiddled with the `udisksctl` (elsewhere referred to as `udiskctl` which doesn't exist on Linux Mint Victoria (v22)) I now know what I need from every `blkid` line: the raw device path only. This drops everything beyond that:

```
root@ger-GA-A75M-UD2H:/home/ger# blkid | grep /dev/sd | sed -e 's/:.*$//'
/dev/sdf1
/dev/sdd2
/dev/sdb1
/dev/sdc2
/dev/sda2
/dev/sda3
/dev/sdi2
/dev/sdg1
/dev/sdh2
/dev/sdd1
/dev/sdi1
/dev/sdc1
/dev/sda1
/dev/sdh1
```


### `xargs -n 1`

That's the magic sauce to feed every device path listed above into its own instance of the next tool, which will do the heavy lifting for us: `udisksctl`.



### `udisksctl  mount --no-user-interaction -b /dev/your_device_to_be_mounted`

That's the key element required to make the automatic automount a reality: https://askubuntu.com/questions/365052/how-to-mount-drive-in-media-username-like-nautilus-does-using-udisks got me started, but:

- `--no-user-interaction` is a **necessity** for otherwise your ssh (console) session will lock up, at least on Linux Mint, when you happen to try this as a non-root user (duh!): the `udisksctl` wants to pop up a login dialog then and with an ssh console session, that ain't gonna fly, so everything just *stops* 😭︎ and you'll have to get in with a second `ssh` session and `killall udisksctl` to make it give back control. 

- no `/by_label/` or `/by_uuid/` for me. Read on for the resulting `/media/...` mount points and you'll see why that was good enough for me. Basically, it produces `/by_label` automounts this way...





### `blkid | grep /dev/sd | sed -e 's/:.*$//' | xargs -n 1  udisksctl  mount --no-user-interaction -b`

thus produces this set of automounted mount points:

```
root@ger-GA-A75M-UD2H:/home/ger# mount | grep /media
/dev/sdf1 on /media/root/USB8 type fuseblk (rw,nosuid,nodev,relatime,user_id=0,group_id=0,default_permissions,allow_other,blksize=4096,uhelper=udisks2)
/dev/sdd2 on /media/root/16TB2303A type fuseblk (rw,nosuid,nodev,relatime,user_id=0,group_id=0,default_permissions,allow_other,blksize=4096,uhelper=udisks2)
/dev/sdb1 on /media/root/16TB-btrfs02 type btrfs (rw,nosuid,nodev,relatime,space_cache=v2,subvolid=5,subvol=/,uhelper=udisks2)
/dev/sdc2 on /media/root/4TB201701 type fuseblk (rw,nosuid,nodev,relatime,user_id=0,group_id=0,default_permissions,allow_other,blksize=4096,uhelper=udisks2)
/dev/sdi2 on /media/root/3TB002 type fuseblk (rw,nosuid,nodev,relatime,user_id=0,group_id=0,default_permissions,allow_other,blksize=4096,uhelper=udisks2)
/dev/sdg1 on /media/root/16TB-linux001 type btrfs (rw,nosuid,nodev,relatime,space_cache=v2,subvolid=5,subvol=/,uhelper=udisks2)
/dev/sdh2 on /media/root/4TB0A1 type fuseblk (rw,nosuid,nodev,relatime,user_id=0,group_id=0,default_permissions,allow_other,blksize=4096,uhelper=udisks2)
```

I'm running this as user `root` so the mounts (Linux Mint Victoria) end up under `/media/root/` which is fine with me.

If you need them to land at another place, I see more research effort in your 
near future. 😉︎

*Was it this easy? Is that all?* Nope. For a good scare, here's the actual run output of that brutal commandline:

```
root@ger-GA-A75M-UD2H:/home/ger# blkid | grep /dev/sd | sed -e 's/:.*$//' | xargs -n 1 echo  udisksctl  mount --no-user-interaction -b
udisksctl mount --no-user-interaction -b /dev/sdf1
udisksctl mount --no-user-interaction -b /dev/sdd2
udisksctl mount --no-user-interaction -b /dev/sdb1
udisksctl mount --no-user-interaction -b /dev/sdc2
udisksctl mount --no-user-interaction -b /dev/sda2
udisksctl mount --no-user-interaction -b /dev/sda3
udisksctl mount --no-user-interaction -b /dev/sdi2
udisksctl mount --no-user-interaction -b /dev/sdg1
udisksctl mount --no-user-interaction -b /dev/sdh2
udisksctl mount --no-user-interaction -b /dev/sdd1
udisksctl mount --no-user-interaction -b /dev/sdi1
udisksctl mount --no-user-interaction -b /dev/sdc1
udisksctl mount --no-user-interaction -b /dev/sda1
udisksctl mount --no-user-interaction -b /dev/sdh1
root@ger-GA-A75M-UD2H:/home/ger# blkid | grep /dev/sd | sed -e 's/:.*$//' | xargs -n 1  udisksctl  mount --no-user-interaction -b
Error mounting /dev/sdf1: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdf1 is already mounted at `/media/root/USB8'.

Mounted /dev/sdd2 at /media/root/16TB2303A
Mounted /dev/sdb1 at /media/root/16TB-btrfs02
Mounted /dev/sdc2 at /media/root/4TB201701
Error mounting /dev/sda2: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sda2 is already mounted at `/boot/efi'.

Error mounting /dev/sda3: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sda3 is already mounted at `/'.

Mounted /dev/sdi2 at /media/root/3TB002
Mounted /dev/sdg1 at /media/root/16TB-linux001
Mounted /dev/sdh2 at /media/root/4TB0A1
Object /org/freedesktop/UDisks2/block_devices/sdd1 is not a mountable filesystem.
Object /org/freedesktop/UDisks2/block_devices/sdi1 is not a mountable filesystem.
Object /org/freedesktop/UDisks2/block_devices/sdc1 is not a mountable filesystem.
Object /org/freedesktop/UDisks2/block_devices/sda1 is not a mountable filesystem.
Object /org/freedesktop/UDisks2/block_devices/sdh1 is not a mountable filesystem.
root@ger-GA-A75M-UD2H:/home/ger# mount
sysfs on /sys type sysfs (rw,nosuid,nodev,noexec,relatime)
proc on /proc type proc (rw,nosuid,nodev,noexec,relatime)
udev on /dev type devtmpfs (rw,nosuid,relatime,size=7857784k,nr_inodes=1964446,mode=755,inode64)
devpts on /dev/pts type devpts (rw,nosuid,noexec,relatime,gid=5,mode=620,ptmxmode=000)
tmpfs on /run type tmpfs (rw,nosuid,nodev,noexec,relatime,size=1584920k,mode=755,inode64)
/dev/sda3 on / type ext4 (rw,relatime,errors=remount-ro)
securityfs on /sys/kernel/security type securityfs (rw,nosuid,nodev,noexec,relatime)
tmpfs on /dev/shm type tmpfs (rw,nosuid,nodev,inode64)
tmpfs on /run/lock type tmpfs (rw,nosuid,nodev,noexec,relatime,size=5120k,inode64)
cgroup2 on /sys/fs/cgroup type cgroup2 (rw,nosuid,nodev,noexec,relatime,nsdelegate,memory_recursiveprot)
pstore on /sys/fs/pstore type pstore (rw,nosuid,nodev,noexec,relatime)
efivarfs on /sys/firmware/efi/efivars type efivarfs (rw,nosuid,nodev,noexec,relatime)
bpf on /sys/fs/bpf type bpf (rw,nosuid,nodev,noexec,relatime,mode=700)
systemd-1 on /proc/sys/fs/binfmt_misc type autofs (rw,relatime,fd=29,pgrp=1,timeout=0,minproto=5,maxproto=5,direct,pipe_ino=22270)
hugetlbfs on /dev/hugepages type hugetlbfs (rw,relatime,pagesize=2M)
mqueue on /dev/mqueue type mqueue (rw,nosuid,nodev,noexec,relatime)
debugfs on /sys/kernel/debug type debugfs (rw,nosuid,nodev,noexec,relatime)
tracefs on /sys/kernel/tracing type tracefs (rw,nosuid,nodev,noexec,relatime)
fusectl on /sys/fs/fuse/connections type fusectl (rw,nosuid,nodev,noexec,relatime)
configfs on /sys/kernel/config type configfs (rw,nosuid,nodev,noexec,relatime)
none on /run/credentials/systemd-sysusers.service type ramfs (ro,nosuid,nodev,noexec,relatime,mode=700)
nfsd on /proc/fs/nfsd type nfsd (rw,relatime)
tmpfs on /run/qemu type tmpfs (rw,nosuid,nodev,relatime,mode=755,inode64)
/dev/sda2 on /boot/efi type vfat (rw,relatime,fmask=0077,dmask=0077,codepage=437,iocharset=iso8859-1,shortname=mixed,errors=remount-ro)
binfmt_misc on /proc/sys/fs/binfmt_misc type binfmt_misc (rw,nosuid,nodev,noexec,relatime)
sunrpc on /run/rpc_pipefs type rpc_pipefs (rw,relatime)
tmpfs on /run/user/1000 type tmpfs (rw,nosuid,nodev,relatime,size=1584916k,nr_inodes=396229,mode=700,uid=1000,gid=1000,inode64)
gvfsd-fuse on /run/user/1000/gvfs type fuse.gvfsd-fuse (rw,nosuid,nodev,relatime,user_id=1000,group_id=1000)
portal on /run/user/1000/doc type fuse.portal (rw,nosuid,nodev,relatime,user_id=1000,group_id=1000)
gvfsd-fuse on /root/.cache/gvfs type fuse.gvfsd-fuse (rw,nosuid,nodev,relatime,user_id=0,group_id=0)
/dev/sdf1 on /media/root/USB8 type fuseblk (rw,nosuid,nodev,relatime,user_id=0,group_id=0,default_permissions,allow_other,blksize=4096,uhelper=udisks2)
/dev/sdd2 on /media/root/16TB2303A type fuseblk (rw,nosuid,nodev,relatime,user_id=0,group_id=0,default_permissions,allow_other,blksize=4096,uhelper=udisks2)
/dev/sdb1 on /media/root/16TB-btrfs02 type btrfs (rw,nosuid,nodev,relatime,space_cache=v2,subvolid=5,subvol=/,uhelper=udisks2)
/dev/sdc2 on /media/root/4TB201701 type fuseblk (rw,nosuid,nodev,relatime,user_id=0,group_id=0,default_permissions,allow_other,blksize=4096,uhelper=udisks2)
/dev/sdi2 on /media/root/3TB002 type fuseblk (rw,nosuid,nodev,relatime,user_id=0,group_id=0,default_permissions,allow_other,blksize=4096,uhelper=udisks2)
/dev/sdg1 on /media/root/16TB-linux001 type btrfs (rw,nosuid,nodev,relatime,space_cache=v2,subvolid=5,subvol=/,uhelper=udisks2)
/dev/sdh2 on /media/root/4TB0A1 type fuseblk (rw,nosuid,nodev,relatime,user_id=0,group_id=0,default_permissions,allow_other,blksize=4096,uhelper=udisks2)
```

where the first command is just me testing my `xargs` foo -- it fails sometimes, the foo that is -- before firing for real: the second line.
Once it's done (it takes a while, all the disks spin up, etc.) the `mount` command shows what happened. Good! The `/media/...` mounted entries are new. Which was as we intended.


### Which stuff DID NOT work?

Too much to mention.🤢 

Once I got this far, or at least *close*, it turned out `fdisk -l` is a total bust for the non-root user (that was part of another answer here on SO); while I was fiddling with this I was setting up samba (😖︎what fun😖︎) alongside so as to have unlimited access to everything on the MSwindows analysis box as well.

The fast move there was to run and access the whole blather as `root`. I'm sure that'll raise some hairs and toupets; see my caveat about air-gapped + maximum-access-no-security near the top: I plead 2nd and 5th amendment and I'm not even American.

Added benefit of mounting NTFS drives on Linux boxen: when you have drives that got (mis)treated by MSWindows `chkdsk` (which may have run automagically some day), the resulting `found.000` directories and other cruft, that may be very hard to get into due to buggered ACL records and such: those are all nicely accessible under Linux so that's great if you want/need to *recover hidden data*. (I suggest GetDataBack Pro on the Windows box for when it got really hairy that way and a basic Linux mount + copy-to-fresh-disk when the situation is a little less grave.)


Anyhow, `udiskctl` turns out to be robust enough to leave existing mounts alone *and* not doing duplicate mount points either: see the loud error messages in the run shown above.


### (Bleep??!) *series of question marks* in your `ls` output?! What the (bleep)?!?!

Oh! one thing you may run into and than give a serious scare (it did for me!) is you (*me*!) running `mount -a` and then, when you `ls` the mount points, seeing THIS zip past your console window:

```
root@ger-GA-A75M-UD2H:/home/ger# ls -lra /media/root/16TB-linux001/
ls: cannot access '/media/root/16TB-linux001/USB_B7': Input/output error
ls: cannot access '/media/root/16TB-linux001/.Trash-1000': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_8': Input/output error
ls: cannot access '/media/root/16TB-linux001/W_projects': Input/output error
ls: cannot access '/media/root/16TB-linux001/SSD1TB': Input/output error
ls: cannot access '/media/root/16TB-linux001/HenkBootHD11': Input/output error
ls: cannot access '/media/root/16TB-linux001/SSD01-Q-drv': Input/output error
ls: cannot access '/media/root/16TB-linux001/laptop_C_drv': Input/output error
ls: cannot access '/media/root/16TB-linux001/laptop_D_drv': Input/output error
ls: cannot access '/media/root/16TB-linux001/1TBBLACK': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_E2': Input/output error
ls: cannot access '/media/root/16TB-linux001/1TBSSD01A': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_A7': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_C1': Input/output error
ls: cannot access '/media/root/16TB-linux001/Win7Boot': Input/output error
ls: cannot access '/media/root/16TB-linux001/Win7BtPt2': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_H1': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_E1': Input/output error
ls: cannot access '/media/root/16TB-linux001/EX-Google.Drive-NOT-SYNCED': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_A3': Input/output error
ls: cannot access '/media/root/16TB-linux001/HD700-01': Input/output error
ls: cannot access '/media/root/16TB-linux001/C-SSD-Boot': Input/output error
ls: cannot access '/media/root/16TB-linux001/D-SSD-Projects': Input/output error
ls: cannot access '/media/root/16TB-linux001/github-recovery-codes.txt': Input/output error
ls: cannot access '/media/root/16TB-linux001/GoogleDrive-copy2023': Input/output error
ls: cannot access '/media/root/16TB-linux001/OneDrive-copy2023': Input/output error
ls: cannot access '/media/root/16TB-linux001/O-NVMe': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_A4': Input/output error
ls: cannot access '/media/root/16TB-linux001/4TB00M00': Input/output error
ls: cannot access '/media/root/16TB-linux001/4TB0A1': Input/output error
ls: cannot access '/media/root/16TB-linux001/HenkBootHD1': Input/output error
ls: cannot access '/media/root/16TB-linux001/300GB_1': Input/output error
ls: cannot access '/media/root/16TB-linux001/Qiqqa-Sopkonijn': Input/output error
ls: cannot access '/media/root/16TB-linux001/500GB1': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_B5': Input/output error
ls: cannot access '/media/root/16TB-linux001/J-300GBrecvr2': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_B3': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_B6': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB8': Input/output error
ls: cannot access '/media/root/16TB-linux001/HD200G_I': Input/output error
ls: cannot access '/media/root/16TB-linux001/rdfind-dedup-results1.txt.gz': Input/output error
total 20
d?????????? ? ?    ?       ?            ? W_projects
d?????????? ? ?    ?       ?            ? Win7BtPt2
d?????????? ? ?    ?       ?            ? Win7Boot
d?????????? ? ?    ?       ?            ? USB_H1
d?????????? ? ?    ?       ?            ? USB_E2
d?????????? ? ?    ?       ?            ? USB_E1
d?????????? ? ?    ?       ?            ? USB_C1
drwxrwxrwx  1 ger  ger   120 mei 23  2012 USB_B9
d?????????? ? ?    ?       ?            ? USB_B7
d?????????? ? ?    ?       ?            ? USB_B6
d?????????? ? ?    ?       ?            ? USB_B5
d?????????? ? ?    ?       ?            ? USB_B3
d?????????? ? ?    ?       ?            ? USB_A7
d?????????? ? ?    ?       ?            ? USB_A4
d?????????? ? ?    ?       ?            ? USB_A3
d?????????? ? ?    ?       ?            ? USB8
d?????????? ? ?    ?       ?            ? USB_8
d?????????? ? ?    ?       ?            ? .Trash-1000
d?????????? ? ?    ?       ?            ? SSD1TB
d?????????? ? ?    ?       ?            ? SSD01-Q-drv
-?????????? ? ?    ?       ?            ? rdfind-dedup-results1.txt.gz
d?????????? ? ?    ?       ?            ? Qiqqa-Sopkonijn
d?????????? ? ?    ?       ?            ? O-NVMe
d?????????? ? ?    ?       ?            ? OneDrive-copy2023
d?????????? ? ?    ?       ?            ? laptop_D_drv
d?????????? ? ?    ?       ?            ? laptop_C_drv
d?????????? ? ?    ?       ?            ? J-300GBrecvr2
d?????????? ? ?    ?       ?            ? HenkBootHD11
d?????????? ? ?    ?       ?            ? HenkBootHD1
d?????????? ? ?    ?       ?            ? HD700-01
d?????????? ? ?    ?       ?            ? HD200G_I
d?????????? ? ?    ?       ?            ? GoogleDrive-copy2023
-?????????? ? ?    ?       ?            ? github-recovery-codes.txt
d?????????? ? ?    ?       ?            ? EX-Google.Drive-NOT-SYNCED
d?????????? ? ?    ?       ?            ? D-SSD-Projects
d?????????? ? ?    ?       ?            ? C-SSD-Boot
d?????????? ? ?    ?       ?            ? 500GB1
d?????????? ? ?    ?       ?            ? 4TB0A1
d?????????? ? ?    ?       ?            ? 4TB00M00
d?????????? ? ?    ?       ?            ? 300GB_1
d?????????? ? ?    ?       ?            ? 1TBSSD01A
d?????????? ? ?    ?       ?            ? 1TBBLACK
drwxr-x---+ 8 root root 4096 nov 21 00:19 ..
drwxrwxrwx  1 ger  ger   834 mei 20  2023 .
root@ger-GA-A75M-UD2H:/home/ger# ls -lha /media/root/16TB-linux001/
ls: cannot access '/media/root/16TB-linux001/USB_B7': Input/output error
ls: cannot access '/media/root/16TB-linux001/.Trash-1000': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_8': Input/output error
ls: cannot access '/media/root/16TB-linux001/W_projects': Input/output error
ls: cannot access '/media/root/16TB-linux001/SSD1TB': Input/output error
ls: cannot access '/media/root/16TB-linux001/HenkBootHD11': Input/output error
ls: cannot access '/media/root/16TB-linux001/SSD01-Q-drv': Input/output error
ls: cannot access '/media/root/16TB-linux001/laptop_C_drv': Input/output error
ls: cannot access '/media/root/16TB-linux001/laptop_D_drv': Input/output error
ls: cannot access '/media/root/16TB-linux001/1TBBLACK': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_E2': Input/output error
ls: cannot access '/media/root/16TB-linux001/1TBSSD01A': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_A7': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_C1': Input/output error
ls: cannot access '/media/root/16TB-linux001/Win7Boot': Input/output error
ls: cannot access '/media/root/16TB-linux001/Win7BtPt2': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_H1': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_E1': Input/output error
ls: cannot access '/media/root/16TB-linux001/EX-Google.Drive-NOT-SYNCED': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_A3': Input/output error
ls: cannot access '/media/root/16TB-linux001/HD700-01': Input/output error
ls: cannot access '/media/root/16TB-linux001/C-SSD-Boot': Input/output error
ls: cannot access '/media/root/16TB-linux001/D-SSD-Projects': Input/output error
ls: cannot access '/media/root/16TB-linux001/github-recovery-codes.txt': Input/output error
ls: cannot access '/media/root/16TB-linux001/GoogleDrive-copy2023': Input/output error
ls: cannot access '/media/root/16TB-linux001/OneDrive-copy2023': Input/output error
ls: cannot access '/media/root/16TB-linux001/O-NVMe': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_A4': Input/output error
ls: cannot access '/media/root/16TB-linux001/4TB00M00': Input/output error
ls: cannot access '/media/root/16TB-linux001/4TB0A1': Input/output error
ls: cannot access '/media/root/16TB-linux001/HenkBootHD1': Input/output error
ls: cannot access '/media/root/16TB-linux001/300GB_1': Input/output error
ls: cannot access '/media/root/16TB-linux001/Qiqqa-Sopkonijn': Input/output error
ls: cannot access '/media/root/16TB-linux001/500GB1': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_B5': Input/output error
ls: cannot access '/media/root/16TB-linux001/J-300GBrecvr2': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_B3': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB_B6': Input/output error
ls: cannot access '/media/root/16TB-linux001/USB8': Input/output error
ls: cannot access '/media/root/16TB-linux001/HD200G_I': Input/output error
ls: cannot access '/media/root/16TB-linux001/rdfind-dedup-results1.txt.gz': Input/output error
total 20K
drwxrwxrwx  1 ger  ger   834 mei 20  2023 .
drwxr-x---+ 8 root root 4,0K nov 21 00:19 ..
d?????????? ? ?    ?       ?            ? 1TBBLACK
d?????????? ? ?    ?       ?            ? 1TBSSD01A
d?????????? ? ?    ?       ?            ? 300GB_1
d?????????? ? ?    ?       ?            ? 4TB00M00
d?????????? ? ?    ?       ?            ? 4TB0A1
d?????????? ? ?    ?       ?            ? 500GB1
d?????????? ? ?    ?       ?            ? C-SSD-Boot
d?????????? ? ?    ?       ?            ? D-SSD-Projects
d?????????? ? ?    ?       ?            ? EX-Google.Drive-NOT-SYNCED
-?????????? ? ?    ?       ?            ? github-recovery-codes.txt
d?????????? ? ?    ?       ?            ? GoogleDrive-copy2023
d?????????? ? ?    ?       ?            ? HD200G_I
d?????????? ? ?    ?       ?            ? HD700-01
d?????????? ? ?    ?       ?            ? HenkBootHD1
d?????????? ? ?    ?       ?            ? HenkBootHD11
d?????????? ? ?    ?       ?            ? J-300GBrecvr2
d?????????? ? ?    ?       ?            ? laptop_C_drv
d?????????? ? ?    ?       ?            ? laptop_D_drv
d?????????? ? ?    ?       ?            ? OneDrive-copy2023
d?????????? ? ?    ?       ?            ? O-NVMe
d?????????? ? ?    ?       ?            ? Qiqqa-Sopkonijn
-?????????? ? ?    ?       ?            ? rdfind-dedup-results1.txt.gz
d?????????? ? ?    ?       ?            ? SSD01-Q-drv
d?????????? ? ?    ?       ?            ? SSD1TB
d?????????? ? ?    ?       ?            ? .Trash-1000
d?????????? ? ?    ?       ?            ? USB_8
d?????????? ? ?    ?       ?            ? USB8
d?????????? ? ?    ?       ?            ? USB_A3
d?????????? ? ?    ?       ?            ? USB_A4
d?????????? ? ?    ?       ?            ? USB_A7
d?????????? ? ?    ?       ?            ? USB_B3
d?????????? ? ?    ?       ?            ? USB_B5
d?????????? ? ?    ?       ?            ? USB_B6
d?????????? ? ?    ?       ?            ? USB_B7
drwxrwxrwx  1 ger  ger   120 mei 23  2012 USB_B9
d?????????? ? ?    ?       ?            ? USB_C1
d?????????? ? ?    ?       ?            ? USB_E1
d?????????? ? ?    ?       ?            ? USB_E2
d?????????? ? ?    ?       ?            ? USB_H1
d?????????? ? ?    ?       ?            ? Win7Boot
d?????????? ? ?    ?       ?            ? Win7BtPt2
d?????????? ? ?    ?       ?            ? W_projects
```

*Particularly* when you are in the process of *recovering* faulty disks (file system corruption, etc. have already occurred previously!) this is empathetically NOT what you want to see! The 3 lines *without* question marks, but actual user names and such, make this a truly horrendous WTF!

See also https://serverfault.com/questions/65616/question-marks-showing-in-ls-of-directory-io-errors-too

Turns out this was (koff!koff!) "harmless": it WOULD have been very harmful if I hadn't reacted by UNmounting these, but it shows you cannot trust `mount -a` to do The Right Thing(tm) at all times when you're jacking in "arbitrary" disks. 

*Moral of side-story*: at least CHECK your mount points' directory tree after mounting when you're going through disks of various and unknown consistency.




# Conclusion

Carefully inspect and evaluate the errors that WILL be reported during the execution of this command seqeunce: you may not like what you see there and then appropriate follow-up action is required. This is no fire&forget stuff!


It works over here, for the given conditions, 2023AD, Linux Mint Victoria on vintage hardware -- all I need from this machine is I/O, which is bottlenecked at the other box (MS Windows based), which does the heavy lifting, so the old AMD processor doesn't get to work. Why waste a good machine when you have a ready spare like that?[^1] 😇︎



[^1]: why did I need a vintage hardware rig with latest Linux anyway? Well, it turned out the MS Windows 10 (64 bit) modern one wasn't coping too well with these disks; turns out that, at least for me, using USB harddrive bays is fine, but not when you plug *several* of those little monsters in a MSwin machine: curious hardware crashes and catastrophic cold boots have been my bane. Until I  got really fed up and set up a Linux rig, root-level samba access with no login and have all the disks that need processing mounted by the Linux kit: 🥳🥳 no more fatal accidents in the process.🥳🥳




# TODO

- Automatic unmounting

For now, this suffices for me: it unmounts all `/media/...` mounts for both `root` and the main user account (`ger` in my case, on this box), which MAY have caught one or more mounts due to an earlier `mount -a`:

```
root@ger-GA-A75M-UD2H:/home/ger# umount /media/root/*
root@ger-GA-A75M-UD2H:/home/ger# umount /media/ger/*
```


