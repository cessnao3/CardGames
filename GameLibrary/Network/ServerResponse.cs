using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Network
{
    /// <summary>
    /// Defines default server response codes
    /// </summary>
    public enum ResponseCodes
    {
        Fail,
        Unauthorized,
        OK
    };

    /// <summary>
    /// Defines the default server response messages
    /// </summary>
    public class ServerResponse
    {
        /// <summary>
        /// The server response
        /// </summary>
        public ResponseCodes code;
    }
}
