using System;
using System.Collections.Generic;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Networking;
using System.Diagnostics;
using System.IO;

using GameEngine.Networking.Packets;

namespace GameEngine.Networking
{
    class Client
    {
        string IP { get; set; }
        string Port { get; set; }
        bool running = false;
        public bool Connected { get; private set; }
        List<Packet> send_buffer = new List<Packet>();
        List<Packet> receive_buffer = new List<Packet>();
        DatagramSocket socket;
        HostName hostName;
        BinaryWriter writer;

        public Client(string ip, string port)
        {
            IP = ip;
            Port = port;
            Connected = false;
        }

        public async void StartClient()
        {
            if (!running)
            {
                try
                {
                    //Create the DatagramSocket and establish a connection to the server.
                    socket = new DatagramSocket();

                    //Set Low Latency mode for the socket
                    socket.Control.QualityOfService = SocketQualityOfService.LowLatency;

                    //Set don't fragment
                    socket.Control.DontFragment = true;

                    //The ConnectionReceived event is raised when connections are received.
                    socket.MessageReceived += MessageReceived;

                    // The server hostname that we will be establishing a connection to.
                    hostName = new HostName(IP);

                    Debug.WriteLine("Client is about to bind...");

                    await socket.BindServiceNameAsync(Port);

                    Debug.WriteLine("Client is bound to port number " + Port);

                    try
                    {
                        Stream output = (await socket.GetOutputStreamAsync(hostName, Port)).AsStreamForWrite();
                        writer = new BinaryWriter(output);

                        new Connecting().WriteData(writer);
                        //await writer.FlushAsync();
                        writer.Flush();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }

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
                throw new ArgumentException("Client already running!");
            }
        }

        void MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            using (DataReader dataReader = args.GetDataReader())
            {
                dataReader.ByteOrder = ByteOrder.LittleEndian;
                while (dataReader.UnconsumedBufferLength != 0)
                {
                    Code code = (Code)dataReader.ReadInt32();
                    Packet p;
                    switch (code)
                    {
                        case Code.Connected:
                            {
                                Connected = true;
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
                        case Code.LevelGenerationData:
                            {
                                p = LevelGenData.ConstructPacket(dataReader);
                                send_buffer.Add(new Acknowledge(p.Code));
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
                                break;
                            }
                        case Code.OtherPlayerID:
                            {
                                p = OtherPlayerID.ConstructPacket(dataReader);
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

        public void Update()
        {
            if (running && writer != null && send_buffer.Count != 0)
            {
                while (send_buffer.Count != 0)
                {
                    send_buffer[0].WriteData(writer);
                    send_buffer.RemoveAt(0);
                }

                writer.Flush();
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
