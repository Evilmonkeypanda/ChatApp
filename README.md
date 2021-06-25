# "Small" Project I wanted to make to learn Sockets and Encryption

Theres two parts to this, first is the ChatApp and the second is the ServerHost.
Server host runs on port 11000 and so do the ChatApps.
You can connect as many ChatApp's as you want to a server however I have not stress tested it.

ChatApp has two part encryption where it will first encrypt the message contents using Asymmetric encryption
and adds the key and IV as well as the keysize and the IVsize to the encrypted message then encrypts it again Symmetrically.
When the other clients recieve the message they decrypt it and use the keysize and IV size to get the key and IV to decrypt the original message.

I'm not entirely sure if this is the best way to do it and in the future might add a way to set a custom key for the symmetric encryption but I'm happy with where the project is now

Since this is my first project I assume theres a few inefficient methods in there that can be optimized but I plan on leaving this as is to at least show if theres been any growth in
my coding capabilities. 

Also no unit testing because whats life without a little excitement
