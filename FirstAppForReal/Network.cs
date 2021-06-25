using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FirstAppForReal
{
    class Network
    {

        //create socket to use as client
        private Socket senderSocket;

        // Allows  use of EncryptionLib
        readonly EncryptionLib encrypt = new EncryptionLib();

        // Stores link to the MainWindow
        MainWindow mainWindow;

        //Stores key and IV used for decryption
        private byte[] key;
        private byte[] IV;

        //Getters and Setters for key and IV
        public byte[] GetKey() { return key; }
        public void SetKey(byte[] newKey) { key = newKey; }

        public byte[] GetIV() { return IV; }
        public void SetIV(byte[] newIV) { IV = newIV; }

        // Loaded first to create and initialize client socket
        public Socket CreateClient(MainWindow main)
        {
            mainWindow = main;
            try
            {
                //Set IP address for the socket creation.
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];

                //Initialization for the encryption class
                encrypt.CreationFunc();

                //Create and initialize the socket then return it
                Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                return socket;
            }
            //Error "handling"
            catch (ArgumentNullException ANE) { mainWindow.inputErrorPopup("Could not create Socket: " + ANE.Message, "ArgumentNullException"); }
            catch (ArgumentOutOfRangeException AOORE) { mainWindow.inputErrorPopup("Could not create Socket: " + AOORE.Message, "ArgumentOutOfRangeException"); }
            catch (ArgumentException AE) { mainWindow.inputErrorPopup("Could not create Socket: " + AE.Message, "ArgumentException"); }
            catch (SocketException SE) { mainWindow.inputErrorPopup("Could not create Socket: " + SE.Message, "SocketException"); }
            return null;
        }


        // Encrypts message and sends it to the server
        public void SendMessage(string message) {
            try
            {
                senderSocket.Send(encrypt.MakeMessage(message, mainWindow));
            }
            catch (ArgumentNullException ANE) { mainWindow.inputErrorPopup("Could not send message: " + ANE.Message, "ArgumentNullException"); }
            catch (SocketException SE) { mainWindow.inputErrorPopup("Could not send message: " + SE.Message, "SocketException"); }
            catch (ObjectDisposedException ODE) { mainWindow.inputErrorPopup("Could not send message: " + ODE.Message, "ObjectDisposedException");  }


        }

        // Created in its own thread and left to hang until a message comes through
        private void WaitForMessages()
        {
            try
            {
                while (true)
                {

                    byte[] bytes = new byte[senderSocket.Available];
                    senderSocket.Receive(bytes, 0, senderSocket.Available, SocketFlags.None);
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine("Recieved Message: " + Encoding.ASCII.GetString(bytes));
                        if (bytes.Length > 0)
                        {
                            mainWindow.AddMessage(encrypt.Decrypt(encrypt.GetMessage(bytes), key, IV));
                        }
                    });


                }
            }
            catch (ArgumentNullException ANE) { mainWindow.inputErrorPopup("Could not create Socket: " + ANE.Message, "ArgumentNullException"); }
            catch (ArgumentOutOfRangeException AOORE) { mainWindow.inputErrorPopup("Could not create Socket: " + AOORE.Message, "ArgumentOutOfRangeException"); }
            catch (ArgumentException AE) { mainWindow.inputErrorPopup("Could not create Socket: " + AE.Message, "ArgumentException"); }
            catch (SocketException SE) { mainWindow.inputErrorPopup("Could not create Socket: " + SE.Message, "SocketException");
                senderSocket = null;
            }
        }

        // Creates the client socket and connects to the IP Address selected
        public void Connect(MainWindow main) {

            if (senderSocket == null)
            {
                try
                {
                    senderSocket = CreateClient(main);
                    IPHostEntry host = Dns.GetHostEntry(mainWindow.getIPADDRESS());
                    IPAddress ipAddress = host.AddressList[0];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
                    senderSocket.Connect(remoteEP);
                }
                catch (ArgumentNullException ANE) { mainWindow.inputErrorPopup("Could not connect: " + ANE.Message, "ArgumentNullException"); return; }
                catch (SocketException SE) { mainWindow.inputErrorPopup("Could not connect: " + SE.Message, "SocketException"); return; }
                catch (ObjectDisposedException ODE) { mainWindow.inputErrorPopup("Could not connect: " + ODE.Message, "ObjectDisposedException"); return; }
                catch (ArgumentOutOfRangeException AOORE) { mainWindow.inputErrorPopup("Could not connect: " + AOORE.Message, "ArgumentOutOfRangeException"); return; }
                catch (ArgumentException AE) { mainWindow.inputErrorPopup("Could not connect: " + AE.Message, "ArgumentException"); return; }
                catch (InvalidOperationException IOE) { mainWindow.inputErrorPopup("Could not connect: " + IOE.Message, "InvalidOperationException"); return; }
                catch (System.Security.SecurityException SE) { mainWindow.inputErrorPopup("Could not connect: " + SE.Message, "SecurityException"); return; }

                Thread t = new Thread(new ThreadStart(WaitForMessages));
                t.Start();
                t.IsBackground = true;

            }
            else
            {
                mainWindow.messages.Items.Add("Already Connected to server.");
            }




        }

    }
}
