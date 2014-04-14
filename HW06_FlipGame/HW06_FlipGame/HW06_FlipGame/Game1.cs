using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HW06_FlipGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>


    public class Position
    {
        public int x;
        public int y;
        public Position(int x, int y) { this.x = x; this.y = y; }
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState mouseState;
        MouseState lastMouseState;

        int x = 0;

        Texture2D[] spriteCollection = new Texture2D[4];
        Texture2D[,] spriteTable = new Texture2D[4,4];

        bool[,] boolTable = new bool[4, 4];
        bool[,] startTable = new bool[4, 4];

        Table currentTable;
        int elapsedTime = 0;
        int timer = 0;

        List<Table> solutionSet = new List<Table>();
        bool displaySolution = false;
        bool reset = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 400;
            graphics.PreferredBackBufferHeight = 400;
            IsMouseVisible = true;
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
            spriteCollection[0] = Content.Load<Texture2D>(@"image\black");
            spriteCollection[1] = Content.Load<Texture2D>(@"image\white");
            spriteCollection[2] = Content.Load<Texture2D>(@"image\blackHint");
            spriteCollection[3] = Content.Load<Texture2D>(@"image\whiteHint");
            Random random = new Random();
            int choice = random.Next(2);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (choice == 0)
                    {
                        boolTable[j, i] = false;
                    }
                    else
                    {
                        boolTable[j, i] = true;
                    }
                }
            }

            generatePattern();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (!boolTable[j, i])
                        spriteTable[j, i] = spriteCollection[0];
                    else
                        spriteTable[j, i] = spriteCollection[1];
                }
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // TODO: Add your update logic here
            if (reset)
            {
                timer += gameTime.ElapsedGameTime.Milliseconds;
                if (timer > 1000)
                {
                    generatePattern();

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (!boolTable[j, i])
                                spriteTable[j, i] = spriteCollection[0];
                            else
                                spriteTable[j, i] = spriteCollection[1];
                        }
                    }
                    timer = 0;
                    reset = false;
                }
                displaySolution = false;
            }
            if (displaySolution)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime > 1000)
                {
                    if (solutionSet.Count != 0)
                    {
                        Table temp = solutionSet.Last();
                        solutionSet.RemoveAt(solutionSet.Count - 1);
                        copyTable(boolTable, temp.table);
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if (!boolTable[j, i])
                                    spriteTable[j, i] = spriteCollection[0];
                                else
                                    spriteTable[j, i] = spriteCollection[1];
                            }
                        }
                        elapsedTime = 0;
                    }
                    else
                    {
                        displaySolution = false;
                        elapsedTime = 0;
                    }
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                reset = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (checkSprite(boolTable))
                {
                    return;
                }
                searchSolution();
                Table temp = currentTable;
                while (!compareTable(boolTable, temp.table))
                {
                    solutionSet.Add(temp);
                    temp = temp.previousTable;
                }
                displaySolution = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                if (checkSprite(boolTable))
                {
                    return;
                }
                searchSolution();
                Table temp = currentTable.previousTable;
                while (!compareTable(boolTable, temp.table))
                {
                    currentTable = currentTable.previousTable;
                    temp = temp.previousTable;
                }
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (!boolTable[j, i])
                            spriteTable[j, i] = spriteCollection[0];
                        else
                            spriteTable[j, i] = spriteCollection[1];
                    }
                }
                if (spriteTable[currentTable.x, currentTable.y] == spriteCollection[0])
                    spriteTable[currentTable.x, currentTable.y] = spriteCollection[2];
                else
                    spriteTable[currentTable.x, currentTable.y] = spriteCollection[3];
                displaySolution = false;
            }

            lastMouseState = mouseState;

            mouseState = Mouse.GetState();


            if (IsActive && mouseState.LeftButton == ButtonState.Pressed)
            {
                Console.WriteLine(++x);
                if (lastMouseState.LeftButton == ButtonState.Released)
                {
                    Position pos = new Position(0, 0);
                    overSprite(pos);
                    gameRule(boolTable, pos.x, pos.y);
                    copyTable(startTable, boolTable);
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (!boolTable[j, i])
                                spriteTable[j, i] = spriteCollection[0];
                            else
                                spriteTable[j, i] = spriteCollection[1];
                        }
                    }
                }
                displaySolution = false;
            }

            base.Update(gameTime);
            }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    spriteBatch.Draw(spriteTable[j, i], new Vector2(j * 100, i * 100), Color.White);
                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void generatePattern()
        {
            Random random = new Random();
            int state = 20;
            int randomNumI;
            int randomNumJ;
            for (int i = 0; i < state; i++)
            {
                randomNumI = random.Next(4);
                randomNumJ = random.Next(4);
                gameRule(boolTable, randomNumJ, randomNumI);
            }
            copyTable(startTable, boolTable);
        }

        protected void searchSolution()
        {
            if (checkSprite(startTable))
                return;

            Queue<Table> checkSet = new Queue<Table>();
            HashSet<Table> storeSet = new HashSet<Table>();

            checkSet.Enqueue(new Table(startTable));
        
            while (checkSet.Count != 0)
            {
                bool[,] tempTable = new bool[4, 4];
                currentTable = checkSet.Dequeue();
                tempTable = currentTable.table;

                storeSet.Add(currentTable);

                if (checkSprite(tempTable))
                {
                    return;
                }

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        bool[,] testTable = new bool[4, 4];
                        copyTable(testTable, tempTable);
                        gameRule(testTable, j, i);
                        Table newTable = new Table(testTable, currentTable);
                        newTable.y = j;
                        newTable.x = i;
                        if (!storeSet.Contains(newTable))
                        {
                            checkSet.Enqueue(newTable);
                        }
                    }
                }
            }
        }

        protected void gameRule(bool[,] boolTable, int x, int y)
        {
            boolTable[y, x] = !boolTable[y, x];
            if(y > 0)
                boolTable[y - 1, x] = !boolTable[y - 1, x];
            if(y < 3)
                boolTable[y + 1, x] = !boolTable[y + 1, x];
            if(x > 0)
                boolTable[y, x - 1] = !boolTable[y, x - 1];
            if(x < 3)
                boolTable[y, x + 1] = !boolTable[y, x + 1];
        }

        protected bool checkSprite(bool[,] tempArray)
        { 
            bool isUniform01 = true;
            bool isUniform02 = true;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (tempArray[j, i])
                        isUniform01 = false;
                    if (!tempArray[j, i])
                        isUniform02 = false;
                }
            }
            if (isUniform01 || isUniform02)
                return true;
            else
                return false;
        }

        protected bool compareTable(bool[,] first, bool[,] second)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (first[j, i] != second[j, i])
                        return false;
                }
            }
            return true;
        }

        protected void copyTable(bool[,] first, bool[,] second)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (second[j, i])
                        first[j, i] = true;
                    else
                        first[j, i] = false;
                }
            }
        }

        protected void overSprite(Position pos)
        {
            float xPos = Mouse.GetState().X;
            float yPos = Mouse.GetState().Y;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (xPos > i * 100 && xPos < (i + 1) * 100
                        && yPos > j * 100 && yPos < (j + 1) * 100)
                    {
                        pos.x = j;
                        pos.y = i;
                    }   
                }
            }
        }

    }
}
