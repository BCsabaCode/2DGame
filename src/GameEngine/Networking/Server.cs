using System;
using System.Collections.Generic;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Diagnostics;

using GameEngine.Networking.Packets;
using System.IO;
using Windows.Networking;

namespace GameEngine.Networking
{
    class Server
    {
        //Check later: https://docs.microsoft.com/en-us/archive/msdn-magazine/2005/august/get-closer-to-the-wire-with-high-performance-sockets-in-net
        //and : https://www.codeproject.com/Articles/22918/How-To-Use-the-SocketAsyncEventArgs-Class
        //and : https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.receivefromasync?view=netcore-3.1
        //and : https://csharp.hotexamples.com/examples/-/SocketAsyncEventArgs/-/php-socketasynceventargs-class-examples.html
        string Port { get; set; }
        bool running = false;
        public bool Connected { get; private set; }
        List<Packet> send_buffer = new List<Packet>();
        List<Packet> receive_buffer = new List<Packet>();
        DatagramSocket socket;
        HostName hostName = null;
        BinaryWriter writer;

        public Server(string port)
        {
            Port = port;
            Connected = false;
        }

        public async void StartServer()
        {
            if (!running)
            {
                try
                {
                    socket = new DatagramSocket();

                    //Set Low Latency mode for the socket
                    socket.Control.QualityOfService = SocketQualityOfService.LowLatency;

                    //Set don't fragment
                    socket.Control.DontFragment = true;

                    //The ConnectionReceived event is raised when connections are received.
                    socket.MessageReceived += MessageReceived;

                    Debug.WriteLine("Server is about to bind...");

                    //Start listening for incoming UDP connections on the specified port. You can specify any port that's not currently in use.
                    await socket.BindServiceNameAsync(Port);

                    Debug.WriteLine("Server is bound to port number " + Port);
                    running = true;
                }
                catch (Exception e)
                {
                    SocketErrorStatus status = SocketError.GetStatus(e.GetBaseException().HResult);
                    Debug.WriteLine(status.ToString() + " : " + e.Message);
                }
            }
            else
            {
                throw new ArgumentException("Server already running!");
            }
        }

        public void Update()
        {
            if (running && Connected && writer != null && send_buffer.Count != 0)
            {
                while (send_buffer.Count != 0)
                {
                    send_buffer[0].WriteData(writer);
                    send_buffer.RemoveAt(0);
                }

                writer.Flush();
            }
        }

        async void MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            using (DataReader dataReader = args.GetDataReader())
            {
                dataReader.ByteOrder = ByteOrder.LittleEndian;
                Debug.WriteLine("buffer lntgh: " + dataReader.UnconsumedBufferLength);
                while (dataReader.UnconsumedBufferLength != 0)
                {
                    Code code = (Code)dataReader.ReadInt32();
                    Packet p;
                    switch (code) {
                        case Code.Connecting:
                            {
                                Connected = true;
                                hostName = args.RemoteAddress;
                                Debug.WriteLine("Connected to: " + hostName);
                                try
                                {
                                    Stream output = (await socket.GetOutputStreamAsync(hostName, Port)).AsStreamForWrite();
                                    writer = new BinaryWriter(output);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message);
                                }
                                send_buffer.Add(new Connected());
                                p = null;
                                break;
                            }
                        case Code.Ping:
                            {
                                p = Ping.ConstructPacket(dataReader);
                                break;
                            }
                        case Code.Pong:
                            {
                                p = Pong.ConstructPacket(dataReader);
                                break;
                            }
                        case Code.Acknowledge:
                            {
                                p = Acknowledge.ConstructPacket(dataReader);
                                break;
                            }
                        case Code.OtherPlayerCreationData:
                            {
                                p = AddOtherPlayer.ConstructPacket(dataReader);
                                send_buffer.Add(new Acknowledge(p.Code));
                                break;
                            }
                        case Code.Input:
                            {
                                p = Input.ConstructPacket(dataReader);
                                break;
                            }
                        case Code.EntityXYCorrection:
                            {
                                p = EntityCorrection.ConstructPacket(dataReader);
                                break;
                            }
                        default: p = null; break;
                    }

                    if (p != null)
                    {
                        receive_buffer.Add(p);
                    }
                    else
                    {
                        Debug.WriteLine("Unrecognized packet " + code);
                        break;
                    }
                }
            }
        }

        public void Send(Packet packet)
        {
            if (running)
            {
                send_buffer.Add(packet);
            }
        }

        public Packet GetNextReceived()
        {
            if (!running) return null;
            if (receive_buffer.Count == 0) return null;

            Packet p = receive_buffer[0];
            receive_buffer.Remove(p);

            return p;
        }
    }
}
