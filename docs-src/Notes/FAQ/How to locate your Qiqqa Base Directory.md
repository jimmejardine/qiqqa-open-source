# How to locate your Qiqqa Base Directory

This is the place where Qiqqa stores all your libraries and works on them. Intranet and Web Libraries are also stored in here (as "local work copies").

> TBD: copyedit this copy/pasta'd article.

---

First things first: what I need is a copy of your *local machine's qiqqa library/libraries*: that is the library your Qiqqa Windows software shows you and operates on when you search the library, add to it, etc.

To find out where your Qiqqa software keeps the library files on your PC, here's the sequence of screenshots to follow and find the directory - in screenshots that is `D:\Qiqqa\base\`. You will need that path to find out where all the Qiqqa data is so you can package it and send it to me (and later get stuff from me and add that to your Qiqqa library set).

In Qiqqa 

![(screenshot)](../assets/htlyqbd1.png)

click on tools button as shown below:

![(screenshot)](../assets/htlyqbd2.png)

to get a dropdown menu, where you click on the Configuration item:

![(screenshot)](../assets/htlyqbd3.png)

to see the Configuration Tab in Qiqqa as below. Scroll down if necessary to find the System item line and 'unfold' it by clicking on the '+' icon to the right:

![(screenshot)](../assets/htlyqbd4.png)

This will show you the system details, including the base path to ALL Qiqqa libraries on your PC: in my case that's the indicated path on the D: drive of my PC:

![(screenshot)](../assets/htlyqbd5.png)

You can copy that path and then paste it into Windows Explorer: you can open a new Windows Explorer window in various ways, e.g. RIGHT-clicking on the icon (1) in the Windows Task Bar and then in the popup menu clicking on the 'File Explorer' line (number 2 in the screenshot below),

![(screenshot)](../assets/htlyqbd6.png)

 after which you get a new Windows Explorer window in which you can either paste that base directory in the bar at the top of that window or browse to that base directory in the usual way, until you end up in that Qiqqa base directory and see something like this:

![(screenshot)](../assets/htlyqbd7.png)


Note the blue-selected directory path at the top there; that's where I pasted that path copied from the Qiqqa Configuration panel.

The slightly more technical bit now is to find out which library is the local copy of that XXXXX library you invited me to on Qiqqa:
with a bit of luck one of your directories in there is also given the cryptic name

    C9F0D079-EE4C-4A15-8547-72164A7A356D

but to make sure (or in case that directory name is NOT available on your PC: I am guessing here) you need to look inside those directories and look for a file named

    Qiqqa.known_web_libraries

In my case, that file is inside the `3A614D8C-0882-4CAB-8FBD-A9E494093283` directory but in your case it'll certainly sit in another directory.

(Note: no need to look deep inside those folders; when you look in them, you'll notice almost all of those cryptic directories have a file called
`Qiqqa.library`
in them and we're only interested in those directories. Most of them will have subdirectories called 'documents' (which is where Qiqqa stores all your collected PDFs) and 'index' (which is where the search index database for Qiqqa is stored), but right now we don't care as we're only looking for that
`Qiqqa.known_web_libraries`
file.

When found, you can open it in a text editor (Notepad) and it will look like some chunks of text, interspersed with unintelligible characters. No worries, just have a look and NEVER select to 'save' if Notepad asks: Qiqqa should be the ONLY application writing to that file! But we can have a look: you'll see something like this:

![(screenshot)](../assets/htlyqbd8.png)

We find the XXXXX library name and description and then pick the corresponding 'hash' which is used by Qiqqa for the directory name: selected blue in the snap below:

![(screenshot)](../assets/htlyqbd9.png)

No need to be super-precise here: all we need this for is a hint which of those cryptic directory names is the one storing the XXXXXX library we are looking for: it's enough to recognize that one of those directory names STARTS with the same "C9" characters as the "XXXXX" line in that "`known_web_libraries`" file: recognizing the first two characters is enough as none of the other directories in that base directory starts with "C9".

Now that we have the location where the entire library is stored on your local PC, you can create a copy or backup of that one and send it/backup it anywhere.

