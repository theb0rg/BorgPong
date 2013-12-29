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

#endregion

namespace BorgPong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        private static bool Shutdown = false;
        SpriteBatch spriteBatch;

        private Texture2D background;
        private Texture2D earth;
        private Texture2D RedLine;
        private Texture2D BlueLine;
        private Texture2D BorgCube;

        private SpriteFont BorgSpriteFont;

        Brick LeftPlayer;
        Brick RightPlayer;

        BorgNetLib.User user;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Exiting += Game1_Exiting;
            this.Activated += Game1_Activated;
            this.Deactivated += Game1_Deactivated;
            this.Window.Title = "BorgPong - Resistance is futile";
           
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
            
            LeftPlayer = new Brick(new Vector2(0,0));
            RightPlayer = new Brick(new Vector2(780,0));

            BorgNetLib.ConnectionSetting connection = new BorgNetLib.ConnectionSetting("85.230.218.187", "1234");
            user = new BorgNetLib.User();
            user.Net = new BorgNetLib.NetService(connection);

            user.Login(Guid.NewGuid().ToString(), "");

            base.Initialize();

            Thread ctThread = new Thread(new ThreadStart(this.SyncThread));
            ctThread.Start();
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



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

               // if (previousPos != (int)LeftPlayer.Y)
               // {

            if (currentUpdate >= numOfUpdatesBeforeSend)
            {
                //PongUpdateMessage msg = new PongUpdateMessage(0, 0, LeftPlayer.Y, gameTime.ElapsedGameTime, user);
                //msg.LastMovements = LeftPlayer.LastMovements;
                //user.Net.Send(msg);
                //previousPos = LeftPlayer.Y;
               // LeftPlayer.LastMovements.Clear();

                user.Net.Send("$" + LeftPlayer.Y);
                currentUpdate = 0;
            }

            if (IsActive)
            {
                //LeftPlayer.Y = gameTime.ElapsedGameTime.Milliseconds * Mouse.GetState().Y;
                LeftPlayer.Y = Mouse.GetState().Y;
                //LeftPlayer.LastMovements.Add(LeftPlayer.Y);
                currentUpdate++;
            }
               // }

            base.Update(gameTime);
        }
       // int previousPos = 0;
        short currentUpdate = 0;
        short numOfUpdatesBeforeSend = 1;


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);

            spriteBatch.Draw(earth,    new Vector2(400, 240),null, Color.White, 0F, new Vector2(), 0.5F, SpriteEffects.None, 0F);
            spriteBatch.Draw(BorgCube, new Vector2(200, 100),null, Color.White, 0F, new Vector2(), 0.5F, SpriteEffects.None, 0F);

            spriteBatch.Draw(BlueLine, LeftPlayer.Vector, Color.White);
           // GraphicsDevice.DrawPrimitives(PrimitiveType.

            if (RightPlayer.Y.IsDifference(10))
            {

            }

            DrawText("CONNECTED TO SERVER: " + user.IsConnected);

            DrawText("UPDATES BEFORE SEND: " + numOfUpdatesBeforeSend);
            DrawText("CURRENT UPDATE: " + currentUpdate);

            if (currentUpdate >= numOfUpdatesBeforeSend)
            {
                DrawText("SENDING");
            }
           /* if(){

            }*/
            DrawText("Movement count: " + RightPlayer.LastMovements.Count);
            if (RightPlayer.LastMovements.Any())
            {
                spriteBatch.Draw(RedLine, new Vector2(RightPlayer.X, RightPlayer.LastMovements[0]), Color.White);
                RightPlayer.LastMovements.RemoveAt(0);
                DrawText("SENDING");
            }
            else
            {
                spriteBatch.Draw(RedLine, new Vector2(RightPlayer.X, RightPlayer.Y), Color.White);
            }


            spriteBatch.End();
           
           // foreach (int yPos in RightPlayer.LastMovements)
          //  {
           // spriteBatch.Begin();
           //     spriteBatch.Draw(RedLine, new Vector2(RightPlayer.X,yPos), Color.White);
           // spriteBatch.End();
          //  }
            numOfText = 0;



            base.Draw(gameTime);
        }

        int numOfText = 0;
        private void DrawText(String text){
            spriteBatch.DrawString(BorgSpriteFont, text, new Vector2(0, numOfText * 25), Color.Green, 0F, Vector2.Zero, 0.7F, SpriteEffects.None, 0F);
            numOfText++;
        }
        
        internal void SyncThread()
        {



            while (true)
            {

                try
                {

                    if (user.Net.Connected)
                    {
                        String dataFromClient = user.Net.Recieve();
                        //dataFromClient.ToygJson<>
                        if (dataFromClient.Length != 0)
                        {
                            if (dataFromClient[0] == '$')
                            {
                                int pos = 0;
                                if (Int32.TryParse(dataFromClient.Substring(1), out pos))
                                {
                                RightPlayer.Y = pos;
                                }
                            }
                        }
                        //Identify packets here
                        if (dataFromClient.IsSerializable<PongUpdateMessage>())
                        {
                    PongUpdateMessage message = (PongUpdateMessage)dataFromClient.XmlDeserialize(typeof(PongUpdateMessage));
                            RightPlayer.Y = (int)message.Y;
                            //RightPlayer.LastMovements = message.LastMovements;
                            RightPlayer.LastMovements.Add(message.Y);
                           

                        }

                    }

                }

                catch (Exception e)
                {

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
