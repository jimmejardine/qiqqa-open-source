# Linux :: mount all drives at once

## Why?

To quickly mount all USB bay drives on the Linux disk I/O box with the absolute minimum of fuss.

## How?

--> commandline, **run as user: `root`**:

```sh
blkid | grep /dev/sd | sed -e 's/:.*$//' | xargs -n 1  udisksctl  mount --no-user-interaction -b
```

`blkid` lists all devices recognized by the system, e.g.:

```
root@ger-GA-A75M-UD2H:/home/ger# blkid
/dev/sdf2: LABEL="4TB0A1" BLOCK_SIZE="512" UUID="F07ABFBA7ABF7C42" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="aff0d461-a492-4ce9-8203-395e9be9009c"
/dev/sdb2: LABEL="4TB201701" BLOCK_SIZE="512" UUID="863EC5FD3EC5E671" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="69b817eb-0614-47de-9266-6c1d3de5a6fa"
/dev/sdk2: LABEL="16TB-PAUL" BLOCK_SIZE="512" UUID="8A76FF6376FF4E87" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="fab8917a-2ed2-4b07-8e3b-0f465f6cdd2b"
/dev/sdi1: LABEL="16GB_DSK05" UUID="867a387e-c89f-456c-9400-f9d6c72ea706" UUID_SUB="220d455b-d9e1-4753-b7f3-f2e6f56cdfbb" BLOCK_SIZE="4096" TYPE="btrfs" PARTLABEL="primary" PARTUUID="51a1cb68-3683-40e3-be31-18f9793d6d39"
/dev/sdg2: LABEL="3TB002" BLOCK_SIZE="512" UUID="2ED8898CD889534F" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="29fafbfb-94a7-4c3d-baae-abdc46c663d4"
/dev/sde2: UUID="BFCB-C886" BLOCK_SIZE="512" TYPE="vfat" PARTLABEL="EFI System Partition" PARTUUID="ad819fa6-308b-4302-b476-205f58ab23aa"
/dev/sde3: UUID="cefb8307-2722-4b89-b66f-692251cb9ce7" BLOCK_SIZE="4096" TYPE="ext4" PARTUUID="afc3f6dc-afc7-42d2-8eb2-21603d6b4b37"
/dev/sdc2: LABEL="16TB03CX" BLOCK_SIZE="512" UUID="76B8C967B8C92689" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="9565f4ef-1d4f-45f4-b994-56be2009f12f"
/dev/sdl1: LABEL="USB8" BLOCK_SIZE="512" UUID="2AC8D135C8D1004D" TYPE="ntfs" PARTUUID="6540671e-01"
/dev/sda1: LABEL="16TB-btrfs02" UUID="70f2d37d-2534-4dca-ac06-ce31a3f9c56b" UUID_SUB="8bc3e3f3-cdc9-4b79-8506-a763fed2c8f0" BLOCK_SIZE="4096" TYPE="btrfs" PARTLABEL="16TB-btrfs2" PARTUUID="71c59478-0527-42e0-ab31-df549cf3cf8f"
/dev/sdj2: LABEL="6TBN005" BLOCK_SIZE="512" UUID="EE70714170711217" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="e9e1e06f-663b-4ad4-a774-03e5de9b2b56"
/dev/sdh1: LABEL="18GB_DSK01" UUID="9618e951-37c6-4763-b2d0-0ba10cb90732" UUID_SUB="ddd4c4da-1cf5-4c3d-af83-1bcb6c0a5132" BLOCK_SIZE="4096" TYPE="btrfs" PARTLABEL="primary" PARTUUID="32d4b52c-a7b9-4d45-b27c-28cf2b272e24"
/dev/sdd1: LABEL="16TB-linux001" UUID="171163e9-539a-4369-b273-e186f5509b2d" UUID_SUB="a0b8c894-3b6d-49c5-a378-4fd25b963e0c" BLOCK_SIZE="4096" TYPE="btrfs" PARTUUID="fc303633-6b39-db46-9355-0f1be2f3ff81"
/dev/sdm5: BLOCK_SIZE="512" UUID="8E2CCD8E2CCD71AF" TYPE="ntfs" PARTUUID="11781178-05"
/dev/sdm1: BLOCK_SIZE="512" UUID="2498C10B98C0DD04" TYPE="ntfs" PARTUUID="11781178-01"
/dev/sdf1: PARTLABEL="Microsoft reserved partition" PARTUUID="f86513b7-0fa5-4b2e-9a78-c835752247ac"
/dev/sdb1: PARTLABEL="Microsoft reserved partition" PARTUUID="e6b4be16-b5f7-4fc5-a995-f9e81a6914f7"
/dev/sdk1: PARTLABEL="Microsoft reserved partition" PARTUUID="7a610397-1cd1-424b-a636-64d2a8d05e3e"
/dev/sdg1: PARTLABEL="Microsoft reserved partition" PARTUUID="ad9896cc-50f9-4183-bda8-3d8e2348f511"
/dev/sde1: PARTUUID="c65d2d74-e6c9-416f-b6a5-cc1bd4783646"
/dev/sdc1: PARTLABEL="Microsoft reserved partition" PARTUUID="46ad23fb-070f-4c42-b04b-2eb2a7c2bfa9"
/dev/sdj1: PARTLABEL="Microsoft reserved partition" PARTUUID="d808a869-73a2-41ea-8b83-77f820a4d79e"
```

`grep` is there to filter out the fixed disks and non-interesting devices, producing:

```
root@ger-GA-A75M-UD2H:/home/ger# blkid | grep /dev/sd
/dev/sdf2: LABEL="4TB0A1" BLOCK_SIZE="512" UUID="F07ABFBA7ABF7C42" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="aff0d461-a492-4ce9-8203-395e9be9009c"
/dev/sdb2: LABEL="4TB201701" BLOCK_SIZE="512" UUID="863EC5FD3EC5E671" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="69b817eb-0614-47de-9266-6c1d3de5a6fa"
/dev/sdk2: LABEL="16TB-PAUL" BLOCK_SIZE="512" UUID="8A76FF6376FF4E87" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="fab8917a-2ed2-4b07-8e3b-0f465f6cdd2b"
/dev/sdi1: LABEL="16GB_DSK05" UUID="867a387e-c89f-456c-9400-f9d6c72ea706" UUID_SUB="220d455b-d9e1-4753-b7f3-f2e6f56cdfbb" BLOCK_SIZE="4096" TYPE="btrfs" PARTLABEL="primary" PARTUUID="51a1cb68-3683-40e3-be31-18f9793d6d39"
/dev/sdg2: LABEL="3TB002" BLOCK_SIZE="512" UUID="2ED8898CD889534F" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="29fafbfb-94a7-4c3d-baae-abdc46c663d4"
/dev/sde2: UUID="BFCB-C886" BLOCK_SIZE="512" TYPE="vfat" PARTLABEL="EFI System Partition" PARTUUID="ad819fa6-308b-4302-b476-205f58ab23aa"
/dev/sde3: UUID="cefb8307-2722-4b89-b66f-692251cb9ce7" BLOCK_SIZE="4096" TYPE="ext4" PARTUUID="afc3f6dc-afc7-42d2-8eb2-21603d6b4b37"
/dev/sdc2: LABEL="16TB03CX" BLOCK_SIZE="512" UUID="76B8C967B8C92689" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="9565f4ef-1d4f-45f4-b994-56be2009f12f"
/dev/sdl1: LABEL="USB8" BLOCK_SIZE="512" UUID="2AC8D135C8D1004D" TYPE="ntfs" PARTUUID="6540671e-01"
/dev/sda1: LABEL="16TB-btrfs02" UUID="70f2d37d-2534-4dca-ac06-ce31a3f9c56b" UUID_SUB="8bc3e3f3-cdc9-4b79-8506-a763fed2c8f0" BLOCK_SIZE="4096" TYPE="btrfs" PARTLABEL="16TB-btrfs2" PARTUUID="71c59478-0527-42e0-ab31-df549cf3cf8f"
/dev/sdj2: LABEL="6TBN005" BLOCK_SIZE="512" UUID="EE70714170711217" TYPE="ntfs" PARTLABEL="Basic data partition" PARTUUID="e9e1e06f-663b-4ad4-a774-03e5de9b2b56"
/dev/sdh1: LABEL="18GB_DSK01" UUID="9618e951-37c6-4763-b2d0-0ba10cb90732" UUID_SUB="ddd4c4da-1cf5-4c3d-af83-1bcb6c0a5132" BLOCK_SIZE="4096" TYPE="btrfs" PARTLABEL="primary" PARTUUID="32d4b52c-a7b9-4d45-b27c-28cf2b272e24"
/dev/sdd1: LABEL="16TB-linux001" UUID="171163e9-539a-4369-b273-e186f5509b2d" UUID_SUB="a0b8c894-3b6d-49c5-a378-4fd25b963e0c" BLOCK_SIZE="4096" TYPE="btrfs" PARTUUID="fc303633-6b39-db46-9355-0f1be2f3ff81"
/dev/sdm5: BLOCK_SIZE="512" UUID="8E2CCD8E2CCD71AF" TYPE="ntfs" PARTUUID="11781178-05"
/dev/sdm1: BLOCK_SIZE="512" UUID="2498C10B98C0DD04" TYPE="ntfs" PARTUUID="11781178-01"
/dev/sdf1: PARTLABEL="Microsoft reserved partition" PARTUUID="f86513b7-0fa5-4b2e-9a78-c835752247ac"
/dev/sdb1: PARTLABEL="Microsoft reserved partition" PARTUUID="e6b4be16-b5f7-4fc5-a995-f9e81a6914f7"
/dev/sdk1: PARTLABEL="Microsoft reserved partition" PARTUUID="7a610397-1cd1-424b-a636-64d2a8d05e3e"
/dev/sdg1: PARTLABEL="Microsoft reserved partition" PARTUUID="ad9896cc-50f9-4183-bda8-3d8e2348f511"
/dev/sde1: PARTUUID="c65d2d74-e6c9-416f-b6a5-cc1bd4783646"
/dev/sdc1: PARTLABEL="Microsoft reserved partition" PARTUUID="46ad23fb-070f-4c42-b04b-2eb2a7c2bfa9"
/dev/sdj1: PARTLABEL="Microsoft reserved partition" PARTUUID="d808a869-73a2-41ea-8b83-77f820a4d79e"
```

which is close but not good enough for the mount command yet, hence the `sed` operation:

```
root@ger-GA-A75M-UD2H:/home/ger# blkid | grep /dev/sd | sed -e 's/:.*$//'
/dev/sdf2
/dev/sdb2
/dev/sdk2
/dev/sdi1
/dev/sdg2
/dev/sde2
/dev/sde3
/dev/sdc2
/dev/sdl1
/dev/sda1
/dev/sdj2
/dev/sdh1
/dev/sdd1
/dev/sdm5
/dev/sdm1
/dev/sdf1
/dev/sdb1
/dev/sdk1
/dev/sdg1
/dev/sde1
/dev/sdc1
/dev/sdj1
```

which is very nice to feed the mount operation via `xargs -n 1`; the commandline can be executed without any worries about existing mounts being impacted in any way, so it is rather "fire & forget", which I *like*. When everyone is mounted already, you might then see something like this:

```
root@ger-GA-A75M-UD2H:/home/ger# blkid | grep /dev/sd | sed -e 's/:.*$//' | xargs -n 1  udisksctl  mount --no-user-interaction -b
Error mounting /dev/sdf2: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdf2 is already mounted at `/media/root/4TB0A1'.

Error mounting /dev/sdb2: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdb2 is already mounted at `/media/root/4TB201701'.

Error mounting /dev/sdk2: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdk2 is already mounted at `/media/root/16TB-PAUL'.

Error mounting /dev/sdi1: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdi1 is already mounted at `/media/root/16GB_DSK05'.

Error mounting /dev/sdg2: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdg2 is already mounted at `/media/root/3TB002'.

Error mounting /dev/sde2: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sde2 is already mounted at `/boot/efi'.

Error mounting /dev/sde3: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sde3 is already mounted at `/'.

Error mounting /dev/sdc2: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdc2 is already mounted at `/media/root/16TB03CX1'.

Error mounting /dev/sdl1: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdl1 is already mounted at `/media/root/USB8'.

Error mounting /dev/sda1: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sda1 is already mounted at `/media/root/16TB-btrfs02'.

Error mounting /dev/sdj2: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdj2 is already mounted at `/media/root/6TBN005'.

Error mounting /dev/sdh1: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdh1 is already mounted at `/media/root/18GB_DSK01'.

Error mounting /dev/sdd1: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdd1 is already mounted at `/media/root/16TB-linux001'.

Error mounting /dev/sdm5: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdm5 is already mounted at `/media/root/8E2CCD8E2CCD71AF'.

Error mounting /dev/sdm1: GDBus.Error:org.freedesktop.UDisks2.Error.AlreadyMounted: Device /dev/sdm1 is already mounted at `/media/root/2498C10B98C0DD04'.

Object /org/freedesktop/UDisks2/block_devices/sdf1 is not a mountable filesystem.
Object /org/freedesktop/UDisks2/block_devices/sdb1 is not a mountable filesystem.
Object /org/freedesktop/UDisks2/block_devices/sdk1 is not a mountable filesystem.
Object /org/freedesktop/UDisks2/block_devices/sdg1 is not a mountable filesystem.
Object /org/freedesktop/UDisks2/block_devices/sde1 is not a mountable filesystem.
Object /org/freedesktop/UDisks2/block_devices/sdc1 is not a mountable filesystem.
Object /org/freedesktop/UDisks2/block_devices/sdj1 is not a mountable filesystem.
```

which is perfectly fine. Let's check the drives' info quickly using `df` which should print sane, easy to read values, hence `-BG` for print-in-Gigabyte-sizes:

```
root@ger-GA-A75M-UD2H:/home/ger# df -BG
Filesystem     1G-blocks   Used Available Use% Mounted on
tmpfs                 2G     1G        2G   2% /run
/dev/sde3           457G   305G      130G  71% /
tmpfs                 8G     1G        8G   1% /dev/shm
tmpfs                 1G     1G        1G   1% /run/lock
tmpfs                 8G     0G        8G   0% /run/qemu
/dev/sde2             1G     1G        1G   2% /boot/efi
tmpfs                 2G     1G        2G   1% /run/user/112
tmpfs                 2G     1G        2G   1% /run/user/1000
/dev/sdf2          3727G  3679G       48G  99% /media/root/4TB0A1
/dev/sdb2          3726G  3560G      167G  96% /media/root/4TB201701
/dev/sdk2         14902G 10815G     4088G  73% /media/root/16TB-PAUL
/dev/sdi1         14902G 10240G     4662G  69% /media/root/16GB_DSK05
/dev/sdg2          2795G  2789G        6G 100% /media/root/3TB002
/dev/sda1         14902G  9508G     5393G  64% /media/root/16TB-btrfs02
/dev/sdh1         16764G 13511G     3251G  81% /media/root/18GB_DSK01
/dev/sdl1           932G   384G      549G  42% /media/root/USB8
/dev/sdj2          5590G  4640G      950G  84% /media/root/6TBN005
/dev/sdc2         14902G 13647G     1256G  92% /media/root/16TB03CX1
/dev/sdd1         14902G 14817G       83G 100% /media/root/16TB-linux001
/dev/sdm5             4G     1G        4G   2% /media/root/8E2CCD8E2CCD71AF
/dev/sdm1             4G     1G        4G  10% /media/root/2498C10B98C0DD04
```

which shows the entire machine, including all the USB mounted drives (`/dev/sd*`), which are the ones I'm concerned with/about.

>
> By the way, all those "`is not a mountable filesystem`" lines is due to most of these being old Windows NT/2000/7/8/10 NTFS drives partitioned and formatted on such systems; they tend to place a minimal 4G or there-abouts scratch/recovery/*whatever*? partition before (or was it after?) the real deal. Ah well... *fire & forget* is the operative word here.
>
> **When disks/partitions are damaged, they need to be recovered elsewhere as my recovery software is Win64/10 based.**
>


## When you have problems, i.e. this doesn't work as you might expect

The `blkid...` commandline above ASSUMES all your partitions have LABELS -- as do all my external drives as that's a habit I developed and kept while using these with MS Windows-based machines.

Today, this assumption *failed*. 

I got a new drive out of its packaging and placed it in one of the bays, did the GPT partitioning and make filesystem little dance and then the `blkid...` commandline failed to deliver: it didn't mount the new partition.
What took care of it is assigning a new **label** to the fresh partition: then everything was hunky-dory and I could proceed as planned.

The commandline elements used today (new BTRFS single partition on a new 16GB harddisk):

The fix and subsequent remount-all attempt:

```
btrfs filesystem label /dev/sdn1 16TB_DSK06

blkid | grep /dev/sd | sed -e 's/:.*$//' | xargs -n 1  udisksctl  mount --no-user-interaction -b

df -BG | sort   
# ^^^ so the /dev/sdxyz mount points are shown in an easy-to-check
# 'have I got them all?' sequence.
```

What went before:

```
ls /dev/sd*
# quick check to see which drives are 'seen' by the system 
# and which ones have partitions: /dev/sdn has none, so that's
# the new, straight-from-packaging harddrive we have to init:
> /dev/sda   /dev/sdb1  /dev/sdc1  /dev/sdd1  /dev/sde2  /dev/sdf1  /dev/sdg1  /dev/sdh1  /dev/sdj   /dev/sdk   /dev/sdl
> /dev/sda1  /dev/sdb2  /dev/sdc2  /dev/sde   /dev/sde3  /dev/sdf2  /dev/sdg2  /dev/sdi   /dev/sdj1  /dev/sdk1  /dev/sdl1
> /dev/sdb   /dev/sdc   /dev/sdd   /dev/sde1  /dev/sdf   /dev/sdg   /dev/sdh   /dev/sdi1  /dev/sdj2  /dev/sdk2  /dev/sdn

fdisk /dev/sdn
# g: create GPT parition table
# n: add new partition 1, spanning full disk

mkfs -t btrfs /dev/sdn1
```





## The End / *Finis*! 🥳





