# Multi-user, Multi-node, Sync-Across and Remote-Backup Considerations

What if you don't just want to work on a single machine? There are several scenarios where you cannot be restricted to a single machine for your (research) activities:

- You work both at the office and at home: you'll need some mechanism to synchronize your several machines where you've installed Qiqqa.

  That would require some form of network synchronization, or, when that's not possible due to office security restraints or other causes, a way to synchronize to a portable drive that you carry around. This includes options such as using generic Cloud Storage, e.g. Google Drive or DropBox. We call this: **Multi-Node**.
  
- You work as part of a team, where you want one or more other folks to have full (read/write/edit) access to a particular Qiqqa Library you've agreed to use amongst yourselves.

  Again, that would require some form of network synchronization, or, when that's not possible due to office security restraints or other causes, a way to synchronize to a portable drive that you carry around. This includes options such as using generic Cloud Storage, e.g. Google Drive, OneDrive or DropBox. We call this: **Multi-User**.

- You want your work backed up. For that purpose you have installed a NAS (Network Attached Storage) device in your network and/or have some form of Cloud Storage (e.g. Google Drive or Microsoft OneDrive) available on your Qiqqa machine -- you use that storage device/facility as a generic backup system, and you want Qiqqa to follow along.

  This implies your backup process is more advanced than copying a bunch of data files to an inserted thumbdrive and involves copying the data files across some computer network onto other hardware. Technically, this involves having "*network shares*" / "*network drives*" accessible on your Qiqqa work machine. We call this: **Remote-Backup**.
  
  Some folks will have automated full-system backup or replication mechanisms up & running on their machines: this is where *all your files are automatically backed-up to the other (remote) machine/storage, without you needing to lift a finger*. That would be a **Automatic Remote Backup Process**.
  
  Anyone else (the rest of us) who wants your Qiqqa libraries backed up will have to execute some (minimal) action to make it happen. Either scheduled or hand-triggered, all these actions and their underlying activities to make that (remote) backup a reality we call: **Sync-Across**.
  
---

Anything you do involving replicating/duplicating/backing-up your Qiqqa library, while continuing working in Qiqqa, would be considered a **Sync-Across** action. Whether it's to a second drive in your machine, a thumbdrive you plugged in or a storage space you've connected through the network, it would involve Qiqqa executing your Sync command.

> Qiqqa also offers **Backup and Restore** facilities, but these are different in that you have a choice to backup or restore the local Qiqqa rig, **all the libraries you have listed**, at the start of the application, *instead of executing the Qiqqa application*.
> 
> Here, we are more interested in all *live* Sync activity that's triggered by either the user or through a configured schedule (*scheduling is not available yet in Qiqqa v80-90 series*), while *Qiqqa keeps on running and being used by you*. Any such **Sync** activity would involve various modes of network access and 'remote file locking'. And that's where the trouble starts...





  

## Whiskey and network bugs: 12 year old, cask strength. Still capped.

Old SMB had a nasty network bug around file locking.
NFS, much older still, also had a “known issue” re network file locking.
Bottom line: it doesn’t work.

Empirical data / personal experience: nobody has fixed that issue. 
That is discounting _specific network **monocultures**_ where the software involved is kept up to date and, *more importantly*, kept in sync with all the other network nodes. 
Any **multicultural** network to date has failed to provide trustworthy network file locking. That is: any mix of Windows, UNIX boxen of any denomination, not even bothering with Macs.

Database servers you say? Redundant / fail-over servers you say?
Did you ever take a **real close look** there? 
We’re talking **network file locking** here (as SQLite is an ISAM database, for starters) and none of the systems employed successfully in the wild employ **network file locking**: all of them use other technologies to distribute and *synchronize* tasks. All use network *communications* of some kind, mostly TCP/IP based, but *none* bet their bottom on the premise of **file locking**.
Meanwhile, the enterprise solutions that **do work** are not available to me here as Qiqqa will run on an unknown gamut of machines and operating system versions (even while it currently only supports MS Windows, there’s the question of version, service packs and patch level there) while the “*Sync Directories*”, i.e. the file system slots where Qiqqa will attempt to *synchronize its database and all other relevant data* to, MAY reside on **anything** that’s able to connect to a Windows platform and offer some kind of filesystem interaction. This is includes cloud-based systems such as DropBox, OneDrive and Google Drive. This also includes any other efforts, such as hardware NAS systems (which often run trimmed-down, older, Linux OSes), “software NAS” (anyone who repurposed their older machines as a “free” NAS, really) and god knows what else out there.
Across-the-network locking is simply **not available** when you happen to interact with such a wide gamut of devices.

So we need to come up with “something else”. Or at least *something that might work across the board and not suffer from the current “oddities”*.



  