using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.System;

using GameEngine.Levels;
using GameEngine.Graphics;
using GameEngine.Inputs;
using GameEngine.Utilities;
using GameEngine.Entities.Mobs;
using GameEngine.Sounds;
using GameEngine.Networking;
using GameEngine.Networking.Packets;
using GameEngine;
using System.Threading;
using GameEngine.Entities;
using GameEngine.UI;
using Microsoft.Graphics.Canvas.Text;

namespace Game2D
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        Stopwatch watch = new Stopwatch();
        Level level = null;
        Screen screen;
        long lastTime;
        int upCount = 0;
        int frameCount = 0;
        bool assests_ready = false;
        bool animated_assests_ready = false;
        bool animated_assests_ready2 = false;
        bool animated_assests_ready3 = false;
        bool animated_assests_ready4 = false;
        Player player;
        PlayerUI ui;
        KeyBoard key;
        ArtificialInput artificial;
        Mouse mouse;
        bool test = false;
        bool gotpong = true;
        GameType type;
        int seed;

        Server server = null;
        Client client = null;
        ClientState clientState = ClientState.CONNECTING;
        ServerState serverState = ServerState.WAITING;

        enum GameType
        {
            Single,
            Host,
            Client
        }

        enum ClientState
        {
            CONNECTING,
            ENTITY_EXCHANGE,
            READY
        }

        enum ServerState
        {
            WAITING,
            ENTITY_EXCHANGE,
            READY
        }

        public GamePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string param = (string)e.Parameter;

            if (param == "single") type = GameType.Single;
            else if (param == "host") type = GameType.Host;
            else if (param == "client") type = GameType.Client;
            else type = GameType.Client;

            Init();

            base.OnNavigatedTo(e);
        }

        void Init()
        {
            //TODO: change it to a loadable property
            ApplicationView.PreferredLaunchViewSize = new Size(1366, 768);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            //Create screen
            screen = new Screen(1366, 768);

            //Create KeyBoard instance
            key = new KeyBoard();

            //Create Mouse instance
            mouse = new Mouse();

            //Init sounds
            Task.Run(async () => await Sound.InitSound());

            if (type != GameType.Client)
            {
                //Create level
                Random random = new Random();
                seed = random.Next(100000000);

                level = new LevelGenerator(seed, 500, true);

                Entity.GenID = true;

                //Create Player
                player = new Player(0, 0, key);

                //Create UI
                ui = new PlayerUI(player);

                //Add Player
                level.AddEntity(player);

                //Init level
                level.Init();

                //Time measurement
                watch.Start();
                lastTime = watch.ElapsedMilliseconds;

                if (type == GameType.Host)
                {
                    artificial = new ArtificialInput();

                    server = new Server("25000");
                    server.StartServer();
                }
            } 
            else
            {
                //Time measurement
                watch.Start();
                lastTime = watch.ElapsedMilliseconds;

                artificial = new ArtificialInput();

                Debug.WriteLine(Config.IP);
                client = new Client(Config.IP, "25000");
                client.StartClient();

                Entity.GenID = false;
            }
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Register for keyboard events
            Window.Current.CoreWindow.KeyDown += KeyDown_UIThread;
            Window.Current.CoreWindow.KeyUp += KeyUp_UIThread;
            
        }

        // The KeyDown handler runs on the UI thread...
        private void KeyDown_UIThread(CoreWindow sender, KeyEventArgs args)
        {
            args.Handled = true;

            // extract the data from the args before marshaling it to the
            // game loop thread
            var virtualKey = args.VirtualKey;

            var action = canvas.RunOnGameLoopThreadAsync(() => KeyDown_GameLoopThread(virtualKey));
        }

        // The KeyUp handler runs on the UI thread...
        private void KeyUp_UIThread(CoreWindow sender, KeyEventArgs args)
        {
            args.Handled = true;

            // extract the data from the args before marshaling it to the
            // game loop thread
            var virtualKey = args.VirtualKey;

            var action = canvas.RunOnGameLoopThreadAsync(() => KeyUp_GameLoopThread(virtualKey));
        }

        void KeyDown_GameLoopThread(VirtualKey vk)
        {
            key.Update(vk, true);
        }

        void KeyUp_GameLoopThread(VirtualKey vk)
        {
            key.Update(vk, false);
        }
        void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            // extract data
            var position = e.GetCurrentPoint(canvas).Position;
            
            var action = canvas.RunOnGameLoopThreadAsync(() => mouse.MouseMoved(position));
        }
        void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                // extract data
                var pointer = e.GetCurrentPoint(canvas);

                mouse.MousePressed(pointer);
            }
        }

        void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            var action = canvas.RunOnGameLoopThreadAsync(() => mouse.MouseReleased());
        }

        /// <summary>
        /// Reasource loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void Canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            //Setup
            Sprite.Init(sender);

            //Load spritesheets
            assests_ready = await Sprite.LoadSheet(@"\resources\spritesheets\tiles_sheet_data.txt", @"\resources\spritesheets\tiles.png");
            if (!Sprite.CreateSpriteFromColor("particle_normal", 100, 4, 4, 0xAA, 0xAA, 0xAA)) 
                throw new Exception("Cannot create sprite from color");

            if (!Sprite.CreateSpriteFromColor("particle_red", 101, 4, 4, 0xFF, 0x00, 0x00))
                throw new Exception("Cannot create sprite from color");

            //AnimatedSprite Setup
            AnimatedSprite.Init(sender);
            
            //Load AnimatedSpriteSheets
            animated_assests_ready = await AnimatedSprite.LoadSheet(@"\resources\spritesheets\player_sheet_data.txt", @"\resources\spritesheets\player_sprites.png");
            animated_assests_ready2 = await AnimatedSprite.LoadSheet(@"\resources\spritesheets\mobs_sheet_data.txt", @"\resources\spritesheets\mobs_sprites.png");
            animated_assests_ready3 = await AnimatedSprite.LoadSheet(@"\resources\spritesheets\ui_sheet_data.txt", @"\resources\spritesheets\ui_sprites.png");
            animated_assests_ready4 = await AnimatedSprite.LoadSheet(@"\resources\spritesheets\object_sheet_data.txt", @"\resources\spritesheets\object_sprites.png");

            //LoadSounds
            await Sound.LoadSound("shot.mp3");
        }

        /// <summary>
        /// Canvas render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            frameCount++;
            if (!assests_ready) return;
            if (!animated_assests_ready) return;
            if (!animated_assests_ready2) return;
            if (!animated_assests_ready3) return;
            if (!animated_assests_ready4) return;
            args.DrawingSession.Antialiasing = CanvasAntialiasing.Aliased;
            screen.SetCDS(args.DrawingSession);
            //In client state can be null
            if (level != null) level.Render(player.GetXY(), screen);
            if (ui != null) ui.Render(screen);
            if (player != null && player.IsRemoved()) 
            {
                CanvasTextFormat format = new CanvasTextFormat();
                format.FontSize = 100;
                format.HorizontalAlignment = CanvasHorizontalAlignment.Center;
                format.VerticalAlignment = CanvasVerticalAlignment.Center;
                screen.RenderText(screen.GetWidth() / 2, screen.GetHeight() / 2, "GAME OVER\n\nPress Esc to close game", format);
            }
        }

        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            upCount++;
            if (assests_ready && !AStar.IsInitialized() && level != null)
            {
                level.InitAStar();
            }
            if ((watch.ElapsedMilliseconds - lastTime) > 1000)
            {
                lastTime = watch.ElapsedMilliseconds;
                Debug.WriteLine("UPS: " + upCount + " , FPS: " + frameCount);
                upCount = 0;
                frameCount = 0;
                if (type == GameType.Client)
                {
                    if (gotpong && clientState == ClientState.READY)
                    {
                        Debug.WriteLine("Sending ping");
                        client.Send(new Ping(lastTime));
                        gotpong = false;
                    }
                }
            }
            //If server of client make special update calls
            if (type == GameType.Client) ClientUpdate();
            else if (type == GameType.Host) ServerUpdate();

            mouse.SetOffset(screen.GetOffset());
            //Can be null in case of Client
            if (level != null) level.Update();
            if (ui != null) ui.Update();

            // Terminate application on Escape press
            if (key.escape)
            {
                Application.Current.Exit();
            }


            if (animated_assests_ready && animated_assests_ready2 && animated_assests_ready3 && animated_assests_ready4)
                AnimatedSprite.GetUpdateables().ForEach(e => e.Update());

            if (type == GameType.Client) client.Update();

            if (type == GameType.Host) server.Update();
        }

        void ClientUpdate()
        {
            Packet p = client.GetNextReceived();
            switch (clientState)
            {
                case ClientState.CONNECTING:
                    {
                        if (!client.Connected)
                        {
                            Debug.WriteLine("Sending connecting");
                            client.Send(new Connecting());
                        }
                        else
                        {
                            if (p != null)
                            {
                                if (p.Code == Code.LevelGenerationData)
                                {
                                    LevelGenData data = (LevelGenData)p;
                                    seed = data.Seed;

                                    level = new LevelGenerator(seed, data.Size, false);

                                    //Create Player
                                    player = new Player(0, 0, key);

                                    //Create UI
                                    ui = new PlayerUI(player);

                                    //Add Player
                                    level.AddEntity(player);

                                    //Init level
                                    level.Init();

                                    client.Send(new AddOtherPlayer(player.GetX(), player.GetY()));

                                    clientState = ClientState.ENTITY_EXCHANGE;
                                }
                                else
                                {
                                    Debug.WriteLine("Failed connection!");
                                    //Go back to menu
                                    if (Frame.CanGoBack)
                                    {
                                        Frame.GoBack();
                                    }
                                }
                            }
                        }
                        break;
                    }
                case ClientState.ENTITY_EXCHANGE:
                    {
                        while (p != null)
                        {
                            if (p.Code == Code.OtherPlayerCreationData)
                            {
                                AddOtherPlayer otherPlayer = (AddOtherPlayer)p;

                                OtherPlayer other = new OtherPlayer(otherPlayer.X, otherPlayer.Y, artificial);

                                level.AddEntity(other);

                                p = client.GetNextReceived();
                                if (p.Code != Code.OtherPlayerID)
                                {
                                    Debug.WriteLine("Something is extremely wrong");
                                }
                                else
                                {
                                    other.AddID(((OtherPlayerID)p).ID);
                                }
                            }
                            else if (p.Code == Code.OtherPlayerID)
                            {
                                OtherPlayerID playerID = (OtherPlayerID)p;
                                player.AddID(playerID.ID);

                                clientState = ClientState.READY;
                            }
                            p = client.GetNextReceived();
                        }
                        break;
                    }
                case ClientState.READY:
                    {
                        while (p != null)
                        {
                            if (p.Code == Code.Pong)
                            {
                                gotpong = true;
                                Pong po = (Pong)p;
                                Debug.WriteLine("Last ping: " + (watch.ElapsedMilliseconds - po.Time) + " ms");
                            }
                            else if (p.Code == Code.EntityXYCorrection)
                            {
                                level.AddCorrection((EntityCorrection)p);
                            }
                            else if (p.Code == Code.Input)
                            {
                                artificial.Update((Input)p);
                            }
                            p = client.GetNextReceived();
                        }
                        client.Send(new Input(key.up, key.down, key.left, key.right));
                        break;
                    }
            } 
        }

        void ServerUpdate()
        {
            Packet p = server.GetNextReceived();
            switch (serverState)
            {
                case ServerState.WAITING:
                    {
                        if (p != null)
                        {
                            if (p.Code == Code.Acknowledge)
                            {
                                Acknowledge ack = (Acknowledge)p;
                                if (ack.Ack == Code.LevelGenerationData)
                                {
                                    server.Send(new AddOtherPlayer(player.GetX(), player.GetY()));
                                    server.Send(new OtherPlayerID(player.ID));

                                    serverState = ServerState.ENTITY_EXCHANGE;
                                    break;
                                }
                            }
                        }
                        if (server.Connected)
                        {
                            //Client connected send level data
                            server.Send(new LevelGenData(seed, 500));
                        }
                        break;
                    }
                case ServerState.ENTITY_EXCHANGE:
                    {
                        while (p != null)
                        {
                            if (p.Code == Code.OtherPlayerCreationData)
                            {
                                AddOtherPlayer otherPlayer = (AddOtherPlayer)p;

                                OtherPlayer other = new OtherPlayer(otherPlayer.X, otherPlayer.Y, artificial);

                                level.AddEntity(other);

                                server.Send(new OtherPlayerID(other.ID));

                                serverState = ServerState.READY;
                            }
                            p = server.GetNextReceived();
                        }
                        break;
                    }
                case ServerState.READY:
                    {
                        while (p != null)
                        {
                            if (p.Code == Code.Ping)
                            {
                                Ping po = (Ping)p;
                                server.Send(new Pong(po.Time));
                            }
                            else if (p.Code == Code.Input)
                            {
                                artificial.Update((Input)p);
                            }
                            p = server.GetNextReceived();
                        }
                        server.Send(new EntityCorrection(player.ID, player.GetX(), player.GetY()));
                        server.Send(new Input(key.up, key.down, key.left, key.right));
                        break;
                    }
            }
        }

        void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= KeyDown_UIThread;
            Window.Current.CoreWindow.KeyUp -= KeyUp_UIThread;

            canvas.RemoveFromVisualTree();
            canvas = null;
        }
    }
}
