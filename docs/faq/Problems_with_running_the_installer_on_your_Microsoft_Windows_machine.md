<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    <p># Problems with running the installer on your Microsoft Windows machine</p>
<blockquote>
<hr>
<p>The info below applies to all executable content that you download from the Internet. These issues are <em>not</em> Qiqqa specific.</p>
<hr>
</blockquote>
<ul>
<li>
<p>when you encounter this:</p>
<blockquote>
<p>Windows protected your PC</p>
<p>Microsoft Defender SmartScreen prevented an unrecognized app from starting. Running this app might put your PC at risk.</p>
<p>App:
setup-v82.0.7357.40407.exe
Publisher:
Unknown publisher</p>
</blockquote>
</li>
<li>
<p>or when the Windows App Store pops up when you double-click the installer</p>
</li>
<li>
<p>or for some hard to diagnose reason the installer <strong>will not run despite the download apparently having succeeded without any errors, using your Chrome browser</strong></p>
</li>
</ul>
<p>… then it might very well be your Windows machine attempting to protect you from ‘unknown executable content downloaded from the Internet’.</p>
<p>This is not magic, but is signaled internally via <a href="https://docs.microsoft.com/en-us/archive/blogs/askcore/alternate-data-streams-in-ntfs">ADS (Alternative Data Streams)</a> <a href="https://blog.malwarebytes.com/101/2015/07/introduction-to-alternate-data-streams/">[Ref2]</a> and you should then be able to remove this blockade by using a tool to remove the <em>Zone 3</em> ADS: <a href="http://www.merijn.nu/programs.php">ADSSpy by Merijn Bellekom</a> <a href="https://github.com/mrbellek/ADSspy">(ADSSpy github repo at this link)</a></p>
<h2>Removing the ADS info (invisibly) attached to your downloaded file(s)</h2>
<p>Here’s an example of some <em>Zone 3</em> ADS content for an image which I downloaded:</p>
<p><img src="../assets/ADSSpy_Zone3_image_example.jpg" alt=""></p>
<p>The same type of information is attached to your downloaded installer / executable and that is recognized by Windows internally and making it reject your double-click-to-execute that file waiting in your Downloads directory (or elsewhere).</p>
<h3>First you need to fetch the ADS Spy tool</h3>
<p>You can <a href="http://www.merijn.nu/programs.php">download ADSSpy from Merijn’s website</a> as a ZIP archive file, which you can extract locally to a directory, resulting in something like this:</p>
<p><img src="../assets/explorer_ADS_in_dir.jpg" alt=""></p>
<blockquote>
<p>Aside: I have several ADS-related tools in that directory on my D: drive, hence the longer list in the picture above.</p>
</blockquote>
<p>As the ADS info is not copied onto the extracted ZIP contents, you can freely run the <code>ADSSpy.exe</code> tool by double-clicking it.</p>
<h3>Set up the ADS Spy tool to find the setup / installer executable you’re having trouble with</h3>
<p><img src="../assets/ADSSpy_dialog.png" alt=""></p>
<p>When you double-click ADS Spy to run the tool, this dialog will show up.</p>
<p>Make sure to chnage these items in the dialog:</p>
<ul>
<li>
<p><strong>turn off = UNcheck the tick box</strong> “Ignore safe system data streams” – if you don’t the <em>Zone 3</em> ADS streams will <em>not</em> be found and you will be unable to remove them.</p>
</li>
<li>
<p><strong>select “Scan only this folder”</strong> and click the little [..] button to the right to select your Downloads directory (where you have your offending setup / installer executable stored on disk)</p>
</li>
</ul>
<p>Now you’re ready to <strong>click the “Scan the system for alternate data streams” button</strong>: ADS Spy will now scan the selected directory and every directory in it and show all files with ADS attached in the list, like this:</p>
<p><img src="../assets/ADSSpy_select_exe.jpg" alt=""></p>
<p>In the picture above, we’ve highlighted the downloaded Qiqqa installer already and <strong>ticked the checkbox</strong>: <em>only the ticked(=selected) entries will be processed when we hit the “Remove selected streams” button</em> next:</p>
<p><img src="../assets/ADSSpy_remove_selected.jpg" alt=""></p>
<p>ADS Spy will then pop up a “Are you sure?” dialog to check:</p>
<p><img src="../assets/ADSSpy_sure_check.jpg" alt=""></p>
<p>where you click “Yes” to remove the ADS data.</p>
<h3>The result of this activity</h3>
<p><em>You won’t see anything different in your Windows Explorer</em> but you should now be able to double-click the setup/installer executable you downloaded from the Qiqqa/GitHub releases web page and proceed with the install process, which is described in the <a href="Installing%20Qiqqa%20-%20Updating%20Qiqqa.md">Install Qiqqa / Update Qiqqa</a> web page.</p>

  </head>
  <body>

    <p># Problems with running the installer on your Microsoft Windows machine</p>
<blockquote>
<hr>
<p>The info below applies to all executable content that you download from the Internet. These issues are <em>not</em> Qiqqa specific.</p>
<hr>
</blockquote>
<ul>
<li>
<p>when you encounter this:</p>
<blockquote>
<p>Windows protected your PC</p>
<p>Microsoft Defender SmartScreen prevented an unrecognized app from starting. Running this app might put your PC at risk.</p>
<p>App:
setup-v82.0.7357.40407.exe
Publisher:
Unknown publisher</p>
</blockquote>
</li>
<li>
<p>or when the Windows App Store pops up when you double-click the installer</p>
</li>
<li>
<p>or for some hard to diagnose reason the installer <strong>will not run despite the download apparently having succeeded without any errors, using your Chrome browser</strong></p>
</li>
</ul>
<p>… then it might very well be your Windows machine attempting to protect you from ‘unknown executable content downloaded from the Internet’.</p>
<p>This is not magic, but is signaled internally via <a href="https://docs.microsoft.com/en-us/archive/blogs/askcore/alternate-data-streams-in-ntfs">ADS (Alternative Data Streams)</a> <a href="https://blog.malwarebytes.com/101/2015/07/introduction-to-alternate-data-streams/">[Ref2]</a> and you should then be able to remove this blockade by using a tool to remove the <em>Zone 3</em> ADS: <a href="http://www.merijn.nu/programs.php">ADSSpy by Merijn Bellekom</a> <a href="https://github.com/mrbellek/ADSspy">(ADSSpy github repo at this link)</a></p>
<h2>Removing the ADS info (invisibly) attached to your downloaded file(s)</h2>
<p>Here’s an example of some <em>Zone 3</em> ADS content for an image which I downloaded:</p>
<p><img src="../assets/ADSSpy_Zone3_image_example.jpg" alt=""></p>
<p>The same type of information is attached to your downloaded installer / executable and that is recognized by Windows internally and making it reject your double-click-to-execute that file waiting in your Downloads directory (or elsewhere).</p>
<h3>First you need to fetch the ADS Spy tool</h3>
<p>You can <a href="http://www.merijn.nu/programs.php">download ADSSpy from Merijn’s website</a> as a ZIP archive file, which you can extract locally to a directory, resulting in something like this:</p>
<p><img src="../assets/explorer_ADS_in_dir.jpg" alt=""></p>
<blockquote>
<p>Aside: I have several ADS-related tools in that directory on my D: drive, hence the longer list in the picture above.</p>
</blockquote>
<p>As the ADS info is not copied onto the extracted ZIP contents, you can freely run the <code>ADSSpy.exe</code> tool by double-clicking it.</p>
<h3>Set up the ADS Spy tool to find the setup / installer executable you’re having trouble with</h3>
<p><img src="../assets/ADSSpy_dialog.png" alt=""></p>
<p>When you double-click ADS Spy to run the tool, this dialog will show up.</p>
<p>Make sure to chnage these items in the dialog:</p>
<ul>
<li>
<p><strong>turn off = UNcheck the tick box</strong> “Ignore safe system data streams” – if you don’t the <em>Zone 3</em> ADS streams will <em>not</em> be found and you will be unable to remove them.</p>
</li>
<li>
<p><strong>select “Scan only this folder”</strong> and click the little [..] button to the right to select your Downloads directory (where you have your offending setup / installer executable stored on disk)</p>
</li>
</ul>
<p>Now you’re ready to <strong>click the “Scan the system for alternate data streams” button</strong>: ADS Spy will now scan the selected directory and every directory in it and show all files with ADS attached in the list, like this:</p>
<p><img src="../assets/ADSSpy_select_exe.jpg" alt=""></p>
<p>In the picture above, we’ve highlighted the downloaded Qiqqa installer already and <strong>ticked the checkbox</strong>: <em>only the ticked(=selected) entries will be processed when we hit the “Remove selected streams” button</em> next:</p>
<p><img src="../assets/ADSSpy_remove_selected.jpg" alt=""></p>
<p>ADS Spy will then pop up a “Are you sure?” dialog to check:</p>
<p><img src="../assets/ADSSpy_sure_check.jpg" alt=""></p>
<p>where you click “Yes” to remove the ADS data.</p>
<h3>The result of this activity</h3>
<p><em>You won’t see anything different in your Windows Explorer</em> but you should now be able to double-click the setup/installer executable you downloaded from the Qiqqa/GitHub releases web page and proceed with the install process, which is described in the <a href="Installing%20Qiqqa%20-%20Updating%20Qiqqa.md">Install Qiqqa / Update Qiqqa</a> web page.</p>


    <footer>
      © 2020 Qiqqa Contributors ::
      <a href="https://github.com/GerHobbelt/qiqqa-open-source/blob/docs-src/FAQ/Problems with running the installer on your Microsoft Windows machine.md">Edit this page on GitHub</a>
    </footer>
  </body>
</html>
