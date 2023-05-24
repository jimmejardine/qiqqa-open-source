# vcopy + sqlite + sync + rclone + rsync + ... tests: logbook notes


## Test run notes at the bleeding edge

**Big Caveat: these notes were valid at the time of writing, but MAY be obsolete or even contradicting current behaviour at any later moment, sometimes even *seconds* away from the original event.**

This is about the things we observe when applying our tools at the bleeding edge of development. This is the lump sum notes (logbook) of these test runs' *odd observations*.

**The Table Of Contents / Overview Index is at [[PDF `bulktest` test run notes at the bleeding edge]].**

-------------------------------

(This logbook section was started in 2023.)

*Here goes -- lower is later ==> updates are appended at the bottom.*

-------------------------------

### Item ♯00001 - Linux `cp` and Unicode / long filename surprises

When copying NTFS to BTRFS on Linux Mint, I got these error messages from `cp`:  DID NOT expect these. Apparently he Linux FUSE NTFS driver also suffers from MS Windows' long filename/path syndrome, but here I don't know how to fix that. On MS Windows you can get around it by using universal paths, like `\\.\drive\absolute-path\filename`, but there's no such thing on Linuxes. 🤔

```
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/!QIQQA-pdf-watch-dir/!5/docs/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/!QIQQA-pdf-watch-dir/!5/docs_original/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/!QIQQA-pdf-watch-dir/2018-10-09/Phased Locked Loop - PLL/74HCT9046A Philips Semiconductor PDFs ����������� ������������ 74HCT9046A �������� ������� 74HCT9046A.pdf Philips Semiconductor PDFs datasheets datasheet data sheets 74HCT9046A Philips Semiconductor PDFs.html': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/Qiqqa-exports-dir/docs/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/Qiqqa-exports-dir/docs_original/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
```

No idea how we're going to resolve this in our sync tool (named `vcopy`).



### Item ♯00002

