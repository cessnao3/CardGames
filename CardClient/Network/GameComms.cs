using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using CardGameLibrary.GameParameters;
using CardGameLibrary.Messages;
using CardGameLibrary.Network;
using System.IO;

namespace CardClient.Network
{
    public sealed class GameComms
    {
        private ClientStruct? ClientStruct { get; set; }

        private static GameComms CommsInstance { get; } = new();

        private GamePlayer? Player { get; set; }

        public bool Failed { get; private set; } = false;

        IPAddress Host { get; set; } = IPAddress.Loopback;

        public static bool SetHost(string hostname)
        {
            IPAddress[] addrs = Dns.GetHostAddresses(hostname);

            if (addrs.Length > 0)
            {
                CommsInstance.Host = addrs[0];
                return true;
            }
            else
            {
                return false;
            }
        }

        private GameComms()
        {
            // Do Nothing
        }

        static public void SetPlayer(GamePlayer p)
        {
            CommsInstance.Player = p;
        }

        static public bool SetupSSL()
        {
            if (CommsInstance.ClientStruct == null)
            {
                return false;
            }

            SslStream ssl_stream = new(
                CommsInstance.ClientStruct.NetworkStream,
                leaveInnerStreamOpen: false,
                userCertificateValidationCallback: new RemoteCertificateValidationCallback(ValidateServerCertificate),
                userCertificateSelectionCallback: null);

            try
            {
                ssl_stream.AuthenticateAsClient("cardgameserver");
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                if (e.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {e.InnerException.Message}");
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                CommsInstance.ClientStruct.Close();
                return false;
            }
            catch (IOException)
            {
                Console.WriteLine("Timeout waiting for server");
                CommsInstance.ClientStruct.Close();
                return false;
            }

            CommsInstance.ClientStruct.SetStream(ssl_stream);
            return true;
        }

        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate? certificate,
              X509Chain? chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            else if (
                chain != null && 
                sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors &&
                chain.ChainStatus.Length == 1 &&
                chain.ChainStatus.First().Status == X509ChainStatusFlags.UntrustedRoot)
            {
                // Chain is valid and only doesn't contain the untrusted root certificate
                return true;
            }
            else
            {
                // Print out error messages
                Console.WriteLine($"Certificate error: {sslPolicyErrors}");

                // Do not allow this client to communicate with unauthenticated servers.
                return false;
            }
        }

        static public GamePlayer? GetPlayer()
        {
            return CommsInstance.Player;
        }

        static public void ResetSocket()
        {
            if (CommsInstance.ClientStruct != null)
            {
                CommsInstance.ClientStruct.Close();
                CommsInstance.ClientStruct = null;
            }

            TcpClient client = new();
            client.Connect(CommsInstance.Host, 8088);

            CommsInstance.ClientStruct = new ClientStruct(client);
        }

        static public void SendMessage(MsgBase msg)
        {
            if (CommsInstance.ClientStruct == null) return;

            try
            {
                MessageReader.SendMessage(CommsInstance.ClientStruct, msg);
            }
            catch (IOException)
            {
                CommsInstance.Failed = true;
            }
        }

        static public bool HasFailed()
        {
            return CommsInstance.Failed;
        }

        static public MsgBase? ReceiveMessage()
        {
            if (CommsInstance.ClientStruct == null) return null;

            try
            {
                return MessageReader.ReadMessage(CommsInstance.ClientStruct);
            }
            catch (IOException)
            {
                CommsInstance.Failed = true;
            }

            return null;
        }
    }
}
