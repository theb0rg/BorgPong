#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BorgNetLib.Packages;
using BorgNetLib;
using System.Threading;
using System.Linq;
using ServiceStack.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;

#endregion

namespace BorgPong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : FixedTimestepGame
    {
        GraphicsDeviceManager graphics;
        private static bool Shutdown = false;
        SpriteBatch spriteBatch;

        private Texture2D background;
        private Texture2D earth;
        private Texture2D RedLine;
        private Texture2D BlueLine;
        private Texture2D BorgCube;
        private Texture2D BorgBall;

        KeyboardState oldState;

        GameObject ball;

        private SpriteFont BorgSpriteFont;

        Brick LeftPlayer;
        Brick RightPlayer;

        BorgNetLib.User user;
        bool bShowHitBoxes = true;
        bool RealRunningSlowly;

        readonly float[] TestFpsRates = { 60.0f, 120.0f, 30.0f, 90.0f, 15.0f, 20.0f, 40.0f };
        int FpsIndex = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Exiting += Game1_Exiting;
            this.Activated += Game1_Activated;
            this.Deactivated += Game1_Deactivated;
            this.Window.Title = "BorgPong - Resistance is futile";
            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromTicks((long)(10000000.0f / TestFpsRates[FpsIndex] + 0.5f));  
        }
        void NewStepRate()
        {
            if (++FpsIndex > 6)
                FpsIndex = 0;
            TargetElapsedTime = TimeSpan.FromTicks((long)(10000000.0f / TestFpsRates[FpsIndex] + 0.5f));
        }  

        private void Game1_Deactivated(object sender, EventArgs e)
        {
            
        }

        private void Game1_Activated(object sender, EventArgs e)
        {
   
        }

        void Game1_Exiting(Object sender, EventArgs args)
        {
            user.Net.Disconnect();
            Shutdown = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = false;
            
            BorgNetLib.ConnectionSetting connection = new BorgNetLib.ConnectionSetting("85.230.222.162", "1234");
            user = new BorgNetLib.User();
            user.Net = new BorgNetLib.NetService(connection);

            user.Login(Guid.NewGuid().ToString(), "");
            oldState = Keyboard.GetState();

            Thread ctThread = new Thread(new ThreadStart(this.SyncThread));
            ctThread.Start();

            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {


            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("stars");
            earth = Content.Load<Texture2D>("earth");

            BlueLine = Content.Load<Texture2D>("BlueLine");
            RedLine = Content.Load<Texture2D>("RedLine");
            BorgCube = Content.Load<Texture2D>("borgcube");
            BorgSpriteFont = Content.Load<SpriteFont>("BorgSpriteFont");
            BorgBall = Content.Load<Texture2D>("BorgBallSmall");


            LeftPlayer = new Brick(BlueLine,new Vector2(0, 0));
            RightPlayer = new Brick(RedLine, new Vector2(780, 0));



            Vector2 position = new Vector2(
                LeftPlayer.BoundingBox.Right + 1,
                (Window.ClientBounds.Height - earth.Height) / 2);

            ball = new GameObject(BorgBall, position, new Vector2(3, 0));
            ball.SetBoundingBoxSize(0.5F);
            ResetBall();
        }

        private void ResetBall()
        {
            ball.Position.X = LeftPlayer.BoundingBox.Right;
            ball.Position.Y = LeftPlayer.BoundingBox.Center.Y - (ball.BoundingBox.Height / 2);
            ball.Velocity = new Vector2(100F, 0);         
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Shutdown = true;
        }

        private void CheckBallCollision()
        {
         /*   if (ball.BoundingBox.Intersects(this.Window.ClientBounds))
            {
                ball.Velocity.Y *= -1;
                ball.Position += ball.Velocity;
            }

            if (ball.BoundingBox.Intersects(new Rectangle(this.Window.ClientBounds.X))
            {
                ball.Velocity.Y *= -1;
                ball.Position += ball.Velocity;
            }*/

            if (ball.BoundingBox.Intersects(LeftPlayer.BoundingBox))
            {
                ball.Velocity.X *= -1;
                //ball.Position += ball.Velocity;
            }

            if (ball.BoundingBox.Intersects(RightPlayer.BoundingBox))
            {
                ball.Velocity.X *= -1;
               // ball.Position += ball.Velocity;
            }

            if ((ball.Position.X < -ball.BoundingBox.Width)
                || (ball.Position.X > Window.ClientBounds.Width))
                ResetBall();

        }

        private void UpdateInput()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            

            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Space) &&
        oldState.IsKeyUp(Keys.Space))
                NewStepRate();

            // Is the SPACE key down?
            if (newState.IsKeyDown(Keys.F1))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.F1))
                {
                 //   backColor =
                   //     new Color(backColor.R, backColor.G, (byte)~backColor.B);
                }
            }
            else if (oldState.IsKeyDown(Keys.F1))
            {
                nSmoothingSteps++;
                // Key was down last update, but not down now, so
                // it has just been released.
            }

            // Is the SPACE key down?
            if (newState.IsKeyDown(Keys.F2))
            {
                // If not down last update, key has just been pressed.
                if (!oldState.IsKeyDown(Keys.F2))
                {
                    //   backColor =
                    //     new Color(backColor.R, backColor.G, (byte)~backColor.B);
                }
            }
            else if (oldState.IsKeyDown(Keys.F2))
            {
                // Key was down last update, but not down now, so
                // it has just been released.
                if (nSmoothingSteps != 0)
                    nSmoothingSteps--;
            }

            // Update saved state.
            oldState = newState;
        }

        protected override void Update2(GameTime gameTime)
        {
            // The gameTime passed to Draw is the bad XNA copy  
            RealRunningSlowly = gameTime.IsRunningSlowly;
            UpdateInput();
            ball.Position.X += ball.Velocity.X * (float)TargetElapsedTime.TotalSeconds;
            if (!user.IsConnected && (int)gameTime.TotalGameTime.TotalSeconds % 10 == 0 && LastConnectionTrySecond != (int)gameTime.TotalGameTime.TotalSeconds)
            {
                //TODO: Put login/Connect on thread
                user.Login(Guid.NewGuid().ToString(), "");
                LastConnectionTrySecond = (int)gameTime.TotalGameTime.TotalSeconds;
            }

            if (currentUpdate >= numOfUpdatesBeforeSend)
            {
              //  using (var config = JsConfig.BeginScope()) {

               // config.ExcludePropertyReferences = new[] { "Exclude.Id", "IncludeExclude.Id", "IncludeExclude.Name" }
                //user.Net.Send("$" + LeftPlayer.Y);
                //throwOnDeserializationError = false,

               // using (JsConfig.With(excludePropertyReferences: (new[] { "SenderUser.NetService", "user.netService" })))
               // {
                PongUpdateMessage msg = new PongUpdateMessage(0, 0, (int)LeftPlayer.Y, gameTime.TotalGameTime, user);
                    //using (var config = JsConfig.BeginScope()) {
                String message = msg.ToJson<PongUpdateMessage>();
                user.Net.Send(message);
               // }
                currentUpdate = 0;
            }

            if (IsActive)
            {
                LeftPlayer.Y = Mouse.GetState().Y;
                currentUpdate++;
            }

            if (this.Window.ClientBounds.Bottom <= LeftPlayer.HitBox.Bottom)
            {
                LeftPlayer.Y = this.Window.ClientBounds.Bottom - LeftPlayer.Height;
                Mouse.SetPosition(Mouse.GetState().X, (int)LeftPlayer.Y);
            }
            CheckBallCollision();
        }  

       // int previousPos = 0;
        short currentUpdate = 0;
        short numOfUpdatesBeforeSend = 1;
        private FrameCounter _frameCounter = new FrameCounter();

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);
            spriteBatch.Draw(BorgCube, new Vector2(200, 100), null, Color.White, 0F, new Vector2(), 0.5F, SpriteEffects.None, 0F);
            ball.Draw(spriteBatch, 0.5F);

           // new Vector2(400, 240)
            //spriteBatch.Draw(earth,    ball.Position,null, Color.White, 0F, new Vector2(), 0.5F, SpriteEffects.None, 0F);

            spriteBatch.Draw(BlueLine, LeftPlayer.Vector, Color.White);
           // GraphicsDevice.DrawPrimitives(PrimitiveType.
            DrawText("");
            DrawText("BOTTOM: " + this.Window.ClientBounds.Bottom + " " + (LeftPlayer.Y + LeftPlayer.Height));

            DrawText("CONNECTED TO SERVER: " + user.IsConnected);
            DrawText("Smothingsteps: " + nSmoothingSteps);

           // DrawText("UPDATES BEFORE SEND: " + numOfUpdatesBeforeSend);
          //  DrawText("CURRENT UPDATE: " + currentUpdate);

            DrawScores();
           DrawText("Programmed FPS=" + TestFpsRates[FpsIndex].ToString() + ", Locked=" +
                                   LockStep.ToString() + ", Locked FPS=" + StepRate.ToString() + ", Monitor FPS=" +
                                   RefreshRate.ToString() + ", IsRunningSlow=" + RealRunningSlowly.ToString());
           DrawText("(press [Space] to change FPS rate)");         
            if (currentUpdate >= numOfUpdatesBeforeSend)
            {
              //  DrawText("SENDING");
            }
           /* if(){

            }*/
          //  DrawText("Movement count: " + RightPlayer.LastMovements.Count);
            //_frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            //DrawText("FPS: " + (int)_frameCounter.AverageFramesPerSecond + " PosRecieved: " + posRecived);
            if (RightPlayer.LastMovements.Any())
            {
                spriteBatch.Draw(RedLine, new Vector2(RightPlayer.X, RightPlayer.LastMovements[0]), Color.White);
                RightPlayer.LastMovements.RemoveAt(0);
                //DrawText("SENDING");
            }
            else
            {
                spriteBatch.Draw(RedLine, new Vector2(RightPlayer.X, RightPlayer.Y), Color.White);
            }


            spriteBatch.End();
           
            numOfText = 0;



            base.Draw(gameTime);
        }

        private void DrawScores()
        {
            DrawText("HITS: " + LeftPlayer.Score,30,0);
            DrawText("HITS: " + RightPlayer.Score,this.Window.ClientBounds.Right-150,0);
        }

        int numOfText = 0;
        private int LastConnectionTrySecond;
        private int posRecived;
        private int nSmoothingSteps = 2;
        private void DrawText(String text){
            spriteBatch.DrawString(BorgSpriteFont, text, new Vector2(this.Window.ClientBounds.Center.X - 200, (this.Window.ClientBounds.Bottom - numOfText * 25)), Color.Green, 0F, Vector2.Zero, 0.7F, SpriteEffects.None, 0F);
            numOfText++;
        }

        private void DrawText(String text, int x, int y)
        {
            spriteBatch.DrawString(BorgSpriteFont, text, new Vector2(x, y), Color.Green, 0F, Vector2.Zero, 0.7F, SpriteEffects.None, 0F);
        }

        public static IEnumerable<int> Range(int steps, int y1, int y2)
        {
            return Enumerable.Range(0, steps)
                             .Select(i => (int)(y1 + (y2 - y1) * ((double)i / (steps - 1)))).Distinct().ToList();
        }

        public void ProcessResponse(Byte[] data, int index, Int32 count)
        {
            using (var stream = new MemoryStream(data, index, count))
            {
                //var serializer = new XmlSerializer(typeof(ClientPacket));

                //var packet = serializer.Deserialize(stream);

                // Do something with the packet
                //serverStream.CopyTo(stream);
               // String asd = System.Text.Encoding.ASCII.GetString(data).Trim();
                try
                {
                    JsonObject message = JsonSerializer.DeserializeFromStream<JsonObject>(stream);
                    if (message == null || message.Keys.First().ToLower().Contains("xml")) return;

                    PackageType type = (PackageType)Enum.Parse(typeof(PackageType), message["PackageType"]);

                    switch (type)
                    {
                        case PackageType.PaddleUpdate:
                            LeftPlayer.Y = int.Parse(message["Y"]);
                            Debug.Print("Recieved " + message.Dump());
                           // PongUpdateMessage a1 = message.Cast<PongUpdateMessage>().First();
                            break;
                    }
                   
                }
                catch(Exception ex){
                    Debug.Print("ProcessResponse:"+ex.Message);
                }
            }

         /*   if (dataFromClient.Length != 0)
            {
                if (dataFromClient[0] == '$')
                {
                    int pos = 0;
                    if (Int32.TryParse(dataFromClient.Substring(1), out pos))
                    {
                        List<int> l1 = (List<int>)Range(nSmoothingSteps, (int)RightPlayer.Y, pos);
                        if (l1.Any()) l1.RemoveAt(0);
                        //Debug.WriteLine(l1.Dump());
                        RightPlayer.LastMovements.Clear();
                        RightPlayer.LastMovements.AddRange(l1);
                        posRecived++;

                        RightPlayer.Y = pos;
                    }
                }
                else if (dataFromClient.IsSerializable<PongUpdateMessage>())
                {
                    PongUpdateMessage message = (PongUpdateMessage)dataFromClient.XmlDeserialize(typeof(PongUpdateMessage));
                    RightPlayer.Y = (int)message.Y;
                    //RightPlayer.LastMovements = message.LastMovements;
                    RightPlayer.LastMovements.Add(message.Y);


                }
            }*/
        }
        public void SocketReceive(Object sender, SocketAsyncEventArgs e)
        {
            try
            {
                ProcessResponse(e.Buffer, 0, e.BytesTransferred);

                NetService.ResetBuffer(e);

                e.AcceptSocket.ReceiveAsync(e);
            }
            catch(Exception ex){
                Debug.Print("SocketRecieve:" + ex.Message);
            }
        }
        internal void SyncThread()
        {

            while (true)
            {

                try
                {

                    if (user.Net.Connected)
                    {

                        //String dataFromClient = user.Net.Recieve();
                        var socketAsyncEventArgs = new SocketAsyncEventArgs();
                        socketAsyncEventArgs.Completed += SocketReceive;
                        NetService.StartListening(socketAsyncEventArgs,user.Net.Socket.Client);
                        break;
                       
                        //dataFromClient.ToygJson<>
                        string dataFromClient = "";
                       

                    }

                }

                catch (Exception e)
                {
                    int asd = 0;
                    //Log.Error(e);

                    //break;

                }



                if (Game1.Shutdown)
                {

                    break;

                }

            }

        }
    }
}
