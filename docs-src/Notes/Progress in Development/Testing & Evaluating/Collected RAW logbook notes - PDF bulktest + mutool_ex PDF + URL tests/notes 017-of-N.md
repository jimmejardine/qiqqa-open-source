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


#### Tests to set up & run (performance checks):

- SQLite performance test with 100K+ documents:
  1. column storage via multiple tables and JOINs: how does this fare as a metadata *variable column count* storage approach vs. ...
  2. *one* metatable which stores all (label, value) pairs for all metadata item labels for all documents, and ...
  3. one table which has columns for the most usual / regular metadata labels (title, author, ...) *mixed* via JOIN (+ flatten) of the above metadata table which now stores all the other, remaining, (label, value) metadata items for all documents, and ...
  4. is there any kind of noticeable *optimum* there, with respect to which and *how many* columns we designate as "*regular*" and thus park in the first, *wide*, table, vs. the second *gathering* table?

  And then the question also involves the question whether we'ld benefit from different \[types of\] *indexes* on the different tables. Plus: can we otherwise *optimize* such metadata gathering SQL queries, when, say, we want the metadata record to be gathered at query level, i.e. reported as a *very wide set of columns*, vs. leaving such *pivoting* to the requesting software?
  > 
  > Initially I thought it might be handy such a "*flattened*" SQL output for third-party users, but anyone can easily gather & merge multiple rows *by themselves* when they want this type of output, while folks who are only interested in certain *columns* for feeding into follow-up queries, he *flattening* is only detrimental to *lump sum consolidated* performance of the Qiqqa database query system... 🤔 so everyone might be better off with sticking with the *mostly RAW* output, which might be a bit chaotic when we land on database design/layout *numeros* 2 or 3 rom the list above.
  
  




#### Tooling to create:

* [[../../Considering the Way Forward/Tools To Be Developed/vcopy - copy, move, synchronize files safely|vcopy :: copy, move, synchronize files safely]]
* [[../../Considering the Way Forward/Tools To Be Developed/ingest - import other databases|ingest :: import other databases]]
* [[../../Considering the Way Forward/Tools To Be Developed/chop-shop - eschew documents and produce content in usable forms|chop-shop :: eschew documents and produce content in usable forms]]
* [[../../Considering the Way Forward/Tools To Be Developed/hog aka hound - fetch the document you seek using maximum effort|hog a.k.a. hound :: fetch the document you seek using maximum effort]]
* [[../../Considering the Way Forward/Tools To Be Developed/snarfl - snarfling all the metadata you need off The Net|snarfl :: snarfling all the metadata you need off The Net]]
* [[../../Considering the Way Forward/Tools To Be Developed/pappy - hola, pappi! - let's play doctor with your database|pappy :: hola, pappi! ... Let's play doctor with your database]]
* [[../../Considering the Way Forward/Tools To Be Developed/bezoar - OCR and related document page prep|bezoar :: OCR and related document page prep]]
* [[../../Considering the Way Forward/Tools To Be Developed/libelle - compare PDF page renders and images|libelle :: compare PDF page renders / images]]
-      
- 


----


Cool names for tools:

- `bezoar`  (Wikipedia: Bezoarsteine (DE))
- `auerhuhn` (Wikipedia: bird (DE))
- `jackdaw` (NL: "kauwtje" (family of blackbird))  (inspired by music: Blackbird & Crow)
- `troglodytes` (birds living in caves, e.g. Dutch "winterkoninkje") -- for extra fun compare `troglodytae` ("[cave dwellers](https://en.wikipedia.org/wiki/Troglodytae)") to `trogodytae` ([people living in Ethiopia and environs](https://oxfordre.com/classics/display/10.1093/acrefore/9780199381135.001.0001/acrefore-9780199381135-e-6581;jsessionid=1D13817E2D77A1285548CDFE2476E972)) --> `spelunking` --> `speleologica`
- `mela` ([Pomponius Mela](https://en.wikipedia.org/wiki/Pomponius_Mela) was the earliest *cartographer* whose name and output has persisted through history; many have preceded him, e.g. [Strabo](https://en.wikipedia.org/wiki/Strabo), and Roman legions, when traveling, measured their routes, thus producing the actual "graphing" of the world as they knew it, but none of those predecessors are referred as *cartographers*, rather *geographers*, *historians*, etc.)
* `statio`, `stationarii` (originally army barracks and their inhabitants, later the transportation management stations spread throughout the Roman Empire. See also [Stationarius (Roman military)](https://en.wikipedia.org/wiki/Stationarius_(Roman_military)), "[The Milites Stationarii considered in relation to the Hundred and Tithing of England](https://sci-hub.ru/10.1017/S0261340900006767)", ...)
* 






### Item ♯00003










### Item ♯00004







