using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FirstAppForReal
{
    class EncryptionLib
    {
        string keyName = "BenjaminFranklin";
        readonly CspParameters cspp = new CspParameters();
        Network network;
        readonly Aes aes = Aes.Create();

        RSACryptoServiceProvider rsa;


        public void CreationFunc()
        {
            cspp.KeyContainerName = keyName;
            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;
            Console.WriteLine("Keys Created: " + cspp.KeyContainerName);

        }
        public byte[] GetKey()
        {
            //Console.WriteLine(string.Join(", ", aes.Key));
            Console.WriteLine(string.Join(", ", aes.IV));
            return aes.Key;

        }


        public byte[] MakeMessage(string message, MainWindow main)
        {
            byte[] encryptedMessage;
            byte[] key = GetKey();
            byte[] IV = GetIV();

            // For some reason public Network getNetwork() 
            // Wasnt working so I returned an object and just
            // Cast it into a Network. Seems to work?
            network = (Network)main.GetNetwork();
            if (network.GetKey() != null)
            {
                key = network.GetKey();
                IV = network.GetIV();
                encryptedMessage = Encrypt(message, key, IV);
            }
            else
            {
                Console.WriteLine("main.getKey() returned null on line 36");
                encryptedMessage = Encrypt(message, GetKey(), GetIV());
            }

            int keyLength = key.Length;
            int IVLength = IV.Length;

            int length = keyLength + IVLength + encryptedMessage.Length + 8;
            byte[] output = new byte[length];
            byte[] tempByte = new byte[4];
            tempByte = BitConverter.GetBytes(keyLength);

            //Adding the keyLength and IVLength
            for (int i = 0; i <= 3; i++)
            {
                output[i] = tempByte[i];
            }

            
            tempByte = BitConverter.GetBytes(IVLength);
            for (int i = 0; i <= 3; i++)
            {
                output[i + 4] = tempByte[i];
            }

            //Adding the key
            for (int i = 0; i < keyLength; i++)
            {
                output[i + 8] = key[i];
            }

            //Adding the IV
            for (int i = 0; i < IVLength; i++)
            {
                output[i + 8 + keyLength] = IV[i];
            }

            //Adding Message
            for (int i = 0; i < encryptedMessage.Length; i++)
            {
                output[i + 8 + keyLength + IVLength] = encryptedMessage[i];
            }
            
            // Adding second encryption
            byte[] output2 = rsa.Encrypt(output, false);
           
            // Shouldnt happen
            if (output == null)
            {
                Console.WriteLine("ouput is null");
            }
            return output2;
        }
        public byte[] GetMessage(byte[] incomingMessage)
        {
            Console.WriteLine(cspp.KeyContainerName);
            incomingMessage = rsa.Decrypt(incomingMessage, false);
            byte[] keySize = new byte[4];
            byte[] IVSize = new byte[4];


            for (int i = 0; i <= 3; i++)
            {
                keySize[i] = incomingMessage[i];
            }
            for (int i = 0; i <= 3; i++)
            {
                IVSize[i] = incomingMessage[i + 4];
            }
            int keySizeInt = BitConverter.ToInt32(keySize);
            int IVSizeInt = BitConverter.ToInt32(IVSize);
            byte[] tempKey = new byte[keySizeInt];
            byte[] tempIV = new byte[IVSizeInt];

            for (int i = 0; i < keySizeInt; i++)
            {
                tempKey[i] = incomingMessage[i + 8];
            }
            for (int i = 0; i < IVSizeInt; i++)
            {
                tempIV[i] = incomingMessage[i + 8 + keySizeInt];
            }

            network.SetKey(tempKey);
            network.SetIV(tempIV);
            int messageSize = incomingMessage.Length - 8 - keySizeInt - IVSizeInt;
            byte[] cutMessage = new byte[messageSize];
            for (int i = 0; i < messageSize; i++)
            {
                Console.WriteLine(i + " : {0} : {1}", i + incomingMessage.Length - keySizeInt - IVSizeInt - 8, messageSize);
                cutMessage[i] = incomingMessage[8 + keySizeInt + IVSizeInt + i];
                Console.WriteLine(Encoding.ASCII.GetString(cutMessage));
            }
            return cutMessage;
        }


        public byte[] GetIV()
        {
            return aes.IV;
        }
        public byte[] Encrypt(string plainText, byte[] key, byte[] IV)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException("PlainText");
            }
            if (key == null)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV == null)
            {
                throw new ArgumentNullException("IV");
            }
            byte[] encrypted;
            Console.WriteLine("Encrypting: " + plainText);
            using (Aes aesAlg = Aes.Create())
            {

                aesAlg.Key = key;
                aesAlg.IV = IV;
                aesAlg.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);

                        }
                        encrypted = msEncrypt.ToArray();
                        Console.WriteLine(Encoding.ASCII.GetString(encrypted));
                    }
                }

            }

            return encrypted;
        }


        public string Decrypt(byte[] cypher, byte[] key, byte[] IV)
        {
            if (cypher == null || cypher.Length <= 0)
            {
                throw new ArgumentNullException("PlainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            string plainText = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cypher))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrpyt = new StreamReader(csDecrypt))
                        {
                            //Console.WriteLine(Encoding.ASCII.GetString(cypher));
                            //Console.WriteLine(string.Join(", ", aes.Key));
                            plainText = srDecrpyt.ReadToEnd();
                        }
                    }

                }

            }
            return plainText;
        }

    }
}
