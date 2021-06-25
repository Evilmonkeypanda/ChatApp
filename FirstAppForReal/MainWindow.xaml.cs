using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace FirstAppForReal
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Please note this was my very first program
    /// that I created with the intention of publishing
    /// to GitHub. While I made it I made a lot of mistakes
    /// and broke way more coding conventions than should be possible
    /// That being said I am happy with how it turned out for a first attempt
    /// and I will be keeping it in Github if anything just to show 
    /// that my skills have progressed from this point
    /// </summary>
    public partial class MainWindow : Window
    {

        //Stores key and IV recieved from every message
        byte[] key;
        byte[] IV;
        Network network = new Network();

        //Sets and Gets
        // Network
        public object GetNetwork() {
            return network;
        }



        // Key
        public byte[] getKey()
        {
            return key;
        }
        public void setKey(byte[] keyS)
        {
            key = keyS;
        }

        // IV
        public byte[] getIV()
        {
            return IV;
        }
        public void setIV(byte[] IVS)
        {
            IV = IVS;
        }

        // IPADDRESS
        public string getIPADDRESS() {

            if (String.IsNullOrEmpty(IPADDRESS.Content.ToString()))
            {
                throw new ArgumentNullException("IP Address is null or empty");
            }
            else {
                return IPADDRESS.Content.ToString();
            }
        }

        public void setIPADDRESS(string ipaddress)
        {
            if (!String.IsNullOrWhiteSpace(ipaddress))
            {
                IPADDRESS.Content = ipaddress;
            }
            if (String.IsNullOrEmpty(ipaddress)) {
                throw new ArgumentNullException("IP Address is blankspace or empty");
            }
        }



        //Initialize the window
        public MainWindow()
        {


            InitializeComponent();

        }

        // To be called for basically any user input that is wrong
        // IE Null or whitespace or IP address doesnt work
        public void inputErrorPopup(string ErrorText, string Header) {
            MessageBox.Show(ErrorText, Header, MessageBoxButton.OK, MessageBoxImage.Warning);
        }



        
        

        //Sets the username
        //Cant be nothing obviously :)
        //TODO: Replace with login system
        private void ButtonAddName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtName.Text))
                {
                    UserName.Content = txtName.Text;
                }
                else
                {
                    throw new ArgumentNullException("Username cannot be empty");
                }
            }
            catch (ArgumentNullException ANE) {
                inputErrorPopup("Could not set Username: " + ANE.Message, "ArgumentNullException");
            }
        }


        // Adds the text to the messages List
        public void AddMessage(string msg)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(msg))
                {
                    messages.Items.Add(msg);
                }
                else
                {
                    throw new ArgumentNullException("Message cannot be empty");
                }
            }
            catch (ArgumentNullException ANE) {
                inputErrorPopup("Could not send message: " + ANE.Message, "ArgumentNullException");
            }
        }



        // Sends text in textbox for messages
        private void ButtonSendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMessage.Text) && (!String.IsNullOrEmpty(UserName.Content.ToString())))
                {
                    network.SendMessage(UserName.Content.ToString() + " : " + txtMessage.Text);
                }
                else
                {
                    throw new ArgumentNullException("Message or Username is empty!");
                }
            }
            catch (ArgumentNullException ANE) {
                inputErrorPopup("Could not send message: " + ANE.Message, "ArgumentNullException");
            }
        }


   
        // Begins connection to server and initializes everything
        private void CONNECTTEST(object sender, RoutedEventArgs e)
        {
            network.Connect(this);
        }

      
        //Updates IP address to connect to
        private void SET_IP(object sender, RoutedEventArgs e)
        {
            try
            {
                setIPADDRESS(txtName.Text);
            }
            catch (ArgumentNullException ANE) {
                inputErrorPopup("Could not set IP: " + ANE.Message, "ArgumentNullException");
            }
        }

        //Just makes enter send a message
        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ButtonSendMessage_Click(sender, e);

            }
        }
    }
}
