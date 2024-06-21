# Testing :: retry connections after a while as user may be running application firewall

I myself run MalwareBytes Windows Firewall Control and that one pops up an okay-to-go-forward-or-not dialog when a yet-unknown-to-him application on my machine attempts to connect to the internet anywhere. That's great!

However, whenever we use cUrl or other means to grab web pages or anything else net-access-y we SHOULD test for this and be okay with initially *failed* socket connections: the remedy here in the application itself is to retry the connection attempt after a while as the user may not be very swift in answering that dialog (I know I am one of those) because they wish to check some stuff or for any other reason and meanwhile your application gets a timeout.

Seen it happen with the MS Visual Studio update process, which clearly has retry attempts coded at various spots as I only "broke" it once while a WFC dialog was waiting for my answer and I wasn't in a hurry to answer it for I had other things on my mind, the MSVC Updater running kinda *in the background*  for me then.

