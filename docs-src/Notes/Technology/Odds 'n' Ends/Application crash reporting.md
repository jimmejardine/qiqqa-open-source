# Application crash reporting

See google breakpad et al for others' implementations.

TODO.

Save any crash info locally to disk and have a *separate* application / background service pick these up and transmit to anywhere (target repo / server / mailing list) on the Internet.

The advantage of that approach is that:
- we don't have / need any 'call home' logic in any application that may crash. This gives users users more control on what goes out and comes in, even the application is granted internet access through the firewall. 
- Meanwhile the crash report transmitter has only one purpose: to send crash reports, if any new ones show up in local storage and possibly retransmit this when (a) the transmission failed earlier, or (2) the user requests a retransmit. This crash reporter app can then be application-firewalled individually for maximum user control over internet access/usage.
- The probably complicated transmit logic is available in a single place, while everybody else can use a simplified dump-to-local-disk crash reporting library (I suggest a patched breakpad library.)

