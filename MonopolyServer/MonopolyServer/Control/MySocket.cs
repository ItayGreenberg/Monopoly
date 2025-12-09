using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MonopolyServer
{
    public static class MySocket
    {
        public static void Send(Socket socket, byte[] data)
        {
            if (data.Length > 0 && socket.Connected)
            {
                try
                {
                    int len = data.Length;
                    byte[] sizeArray = BitConverter.GetBytes(len);
                    byte[] buffer = new byte[4 + data.Length];
                    Array.Copy(sizeArray, 0, buffer, 0, 4);
                    Array.Copy(data, 0, buffer, 4, data.Length);

                    // Send the data
                    int bytesSent = socket.Send(buffer);

                    // Check if all bytes were sent
                    if (bytesSent != buffer.Length)
                    {
                        // Not all bytes were sent
                        ServerForm.s_instance.UpdateDebugOutput("Not all bytes were sent.");
                    }
                    else
                    {
                        // All bytes were sent successfully
                        ServerForm.s_instance.UpdateDebugOutput("Data sent successfully.");
                    }
                }
                catch (SocketException ex)
                {
                    // Handle socket exception
                    ServerForm.s_instance.UpdateDebugOutput($"SocketException: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    ServerForm.s_instance.UpdateDebugOutput($"Exception: {ex.Message}");
                }
            }
        }

        public static byte[] Receive(Socket socket)
        {
            try
            {
                byte[] sizeBuffer = new byte[4];
                int bytesRead = socket.Receive(sizeBuffer);
                if (bytesRead == 0)
                {
                    // If no bytes are received, the connection has been closed
                    return null;
                }

                int messageSize = BitConverter.ToInt32(sizeBuffer, 0);
                byte[] buffer = new byte[messageSize];
                int totalBytesRead = 0;

                while (totalBytesRead < messageSize)
                {
                    int bytesReceived = socket.Receive(buffer, totalBytesRead, messageSize - totalBytesRead, SocketFlags.None);
                    if (bytesReceived == 0)
                    {
                        // If no bytes are received, the connection has been closed
                        return null;
                    }
                    totalBytesRead += bytesReceived;
                }

                return buffer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
