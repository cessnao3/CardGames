using System;
using System.IO;
using System.Net.Sockets;

namespace CardGameLibrary.Network
{
    /// <summary>
    /// Defines a common client structure for data parmaeters
    /// </summary>
    public class ClientStruct : IDisposable
    {
        /// <summary>
        /// Stores the TcpClient
        /// </summary>
        public TcpClient Client { get; private set; }

        /// <summary>
        /// Stores the reference to the network stream
        /// </summary>
        public Stream NetworkStream { get; private set; }

        /// <summary>
        /// Constructs the client structure from a provided TcpClient and sets the
        /// stream parameters
        /// </summary>
        /// <param name="tcpClient">The client to setup the struct with</param>
        public ClientStruct(TcpClient tcpClient, Stream? stream = null)
        {
            // Store the client and stream
            Client = tcpClient;
            NetworkStream = stream ?? tcpClient.GetStream();

            // Set stream parameter
            NetworkStream.ReadTimeout = 1000;
            NetworkStream.WriteTimeout = 1000;
        }

        /// <summary>
        /// Replaces the current stream with the stream provided
        /// </summary>
        /// <param name="stream">The stream to replace the current stream with</param>
        public void SetStream(Stream stream)
        {
            NetworkStream = stream;
        }

        /// <summary>
        /// Close the client
        /// </summary>
        public void Close()
        {
            // Close the stream and client
            NetworkStream?.Close();
            Client?.Close();

            NetworkStream?.Dispose();
            Client?.Dispose();
        }

        /// <summary>
        /// Implement the disposable interface
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Inner helper class for the disposable interface
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ClientStruct()
        {
            Dispose(false);
        }
    }
}
