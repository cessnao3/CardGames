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
    public class GameComms
    {
        protected ClientStruct client_struct;

        protected static GameComms gc_instance = new GameComms();

        protected GamePlayer player;

        bool failed = false;

        IPAddress host;

        public static bool SetHost(string hostname)
        {
            IPAddress[] addrs = Dns.GetHostAddresses(hostname);

            if (addrs.Length > 0)
            {
                gc_instance.host = addrs[0];
                return true;
            }
            else
            {
                return false;
            }
        }

        protected GameComms()
        {
            // Do Nothing
        }

        static public void SetPlayer(GamePlayer p)
        {
            gc_instance.player = p;
        }

        static public bool SetupSSL()
        {
            SslStream ssl_stream = new SslStream(
                gc_instance.client_struct.stream,
                leaveInnerStreamOpen: false,
                userCertificateValidationCallback: new RemoteCertificateValidationCallback(ValidateServerCertificate),
                userCertificateSelectionCallback: null);

            try
            {
                ssl_stream.AuthenticateAsClient("cardgameserver");
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                gc_instance.client_struct.Close();
                return false;
            }
            catch (IOException)
            {
                Console.WriteLine("Timeout waiting for server");
                gc_instance.client_struct.Close();
                return false;
            }

            gc_instance.client_struct.SetStream(ssl_stream);
            return true;
        }

        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            else if (
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
                Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

                // Do not allow this client to communicate with unauthenticated servers.
                return false;
            }
        }

        static public GamePlayer GetPlayer()
        {
            return gc_instance.player;
        }

        static public void ResetSocket()
        {
            if (gc_instance.client_struct != null)
            {
                gc_instance.client_struct.Close();
                gc_instance.client_struct = null;
            }

            TcpClient client = new TcpClient();
            client.Connect(gc_instance.host, 8088);

            gc_instance.client_struct = new ClientStruct(client);
        }

        static public bool Failed()
        {
            return gc_instance.failed;
        }

        static public void SendMessage(MsgBase msg)
        {
            if (gc_instance.client_struct == null) return;

            try
            {
                MessageReader.SendMessage(gc_instance.client_struct, msg);
            }
            catch (System.IO.IOException)
            {
                gc_instance.failed = true;
            }
        }

        static public MsgBase ReceiveMessage()
        {
            if (gc_instance.client_struct == null) return null;

            try
            {
                return MessageReader.ReadMessage(gc_instance.client_struct);
            }
            catch (System.IO.IOException)
            {
                gc_instance.failed = true;
            }

            return null;
        }
    }
}
