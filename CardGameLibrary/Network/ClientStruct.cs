using System;
using System.IO;
using System.Net.Sockets;

namespace CardGameLibrary.Network
{
    /// <summary>
    /// Defines a common client structure for data parmaeters
    /// </summary>
    public class ClientStruct
    {
        /// <summary>
        /// Stores the TcpClient
        /// </summary>
        public TcpClient client { get; private set; }

        /// <summary>
        /// Stores the reference to the network stream
        /// </summary>
        public Stream stream { get; private set; }

        /// <summary>
        /// Constructs the client structure from a provided TcpClient and sets the
        /// stream parameters
        /// </summary>
        /// <param name="tcpClient">The client to setup the struct with</param>
        public ClientStruct(TcpClient tcpClient)
        {
            // Store the client and stream
            client = tcpClient;
            stream = client.GetStream();

            // Set stream parameter
            stream.ReadTimeout = 1000;
            stream.WriteTimeout = 1000;
        }

        /// <summary>
        /// Close the client
        /// </summary>
        public void Close()
        {
            if (client != null)
            {
                // Close the stream and client
                stream.Close();
                client.Close();

                // Reset the client and stream to null
                client = null;
                stream = null;
            }
        }

        /// <summary>
        /// Replaces the current stream with the stream provided
        /// </summary>
        /// <param name="stream">The stream to replace the current stream with</param>
        public void SetStream(Stream stream)
        {
            this.stream = stream;
        }
    }
}
