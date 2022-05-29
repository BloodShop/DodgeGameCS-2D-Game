using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
namespace DodgeGameAlonKolyakov
{
    public class GameBoard
    {
        [JsonIgnore]
        public List<Rectangle> rectImage = new List<Rectangle>();
        public TextBlock tbScore = new TextBlock() { Text = $"Score: {MainPage.myScore}", Height = 76 * MainPage._scaleBoardHeight, Width = 250 * MainPage._scaleBoardWidth, FontSize = 45 * MainPage._scaleBoardHeight, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("/Assets/Fonts1/SpringInMay.ttf#Spring in May") };

       // Random generators
        Random randomSize = new Random(); // property size (width, height) set tot the enemy
        Random speedGenerator = new Random(); // generates random speed to the enemy property
        Random generateXandYlocation = new Random(); // usefull in the RandomizeXandY func to set enemy's spwan location

        public List<Character> characters { get; set; }
        public Player Boni { get; set; }
        public List<Enemy> enemies { get; set; }
        public List<Coin> coins { get; set; }

        public int _boardWidth, _boardHeight, characterWidth, characterHeight, playerSpeed;
        [JsonIgnore]
        private int countDebug = 0;
        private double distanceBetweenRectangles { get; set; }
        private int xSum, ySum;
        public int health, xLifeProperty = (int)MainPage.windowRectangle.Width;
        [JsonIgnore]
        public int numOfEnemies = 10;
        [JsonIgnore]
        private int numOfCoins = 15;

        public string PrintCharactersToSave()
        {
            var temp = new Character();
            string str = $"{{ \'numOfEnemies\' : {numOfEnemies}, \'health\' : {health}, \'score\' : {MainPage.myScore}, \'characters\' : [\n";
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] is Player)
                    temp = (Player)characters[i];
                else if (characters[i] is Enemy)
                    temp = (Enemy)characters[i];
                else if (characters[i] is Coin)
                    temp = (Coin)characters[i];
                // {{ \'{temp}\' :         }}
                str += $" {{ \'_x\' : {characters[i]._x}, \'_y\' : {characters[i]._y}, \'isAlive\' : \'{characters[i].isAlive}\', \'_width\' : \'{characters[i]._width}\'," +
                          $" \'_height\' : \'{characters[i]._height}\', \'_radius\' : \'{characters[i]._radius}\', \'_speed\' : \'{characters[i]._speed}\', \'type\' : {temp.type} }} ";
                if(i != characters.Count - 1)
                    str += ",\n";
            }
            str += $"\n] }}";
            Debug.WriteLine(str);
            return str;
        } // Characters stringJSON form
        public string PrintSeperatlyToSave()
        {
            string str = $"{{ \'numOfEnemies\' : {numOfEnemies}, \'health\' : {health}, \'score\' : {MainPage.myScore}," +
                $" \'enemySpawned\' : {MainPage.enemySpawned}, \'enemies\' : [\n";
            for (int i = 0; i < enemies.Count; i++)
            {
                str += $" {{ \'_x\' : {enemies[i]._x}, \'_y\' : {enemies[i]._y}, \'isAlive\' : \'{enemies[i].isAlive}\', \'_width\' : \'{enemies[i]._width}\'," +
                   $" \'_height\' : \'{enemies[i]._height}\', \'_radius\' : \'{enemies[i]._radius}\', \'_speed\' : \'{enemies[i]._speed}\' }}";
                if (i != enemies.Count - 1)
                    str += ",\n";
            }
            str += $"\n], \'Boni\' : {{ \'_x\' : {Boni._x}, \'_y\' : {Boni._y}, \'isAlive\' : \'{Boni.isAlive}\', \'_width\' : \'{Boni._width}\'," +
                          $" \'_height\' : \'{Boni._height}\', \'_radius\' : \'{Boni._radius}\', \'_speed\' : \'{Boni._speed}\' }} }}";
            Debug.WriteLine(str);
            return str;
        }  // Enemies and player stringJSON form

        #region Constructors
        [JsonConstructor] // Enemies and player FORM
        public GameBoard(int numOfEnemies, int health, int score, int enemySpawned, List<Enemy> enemies, Player Boni)
        {
            this.numOfEnemies = numOfEnemies;
            this.health = health;
            MainPage.myScore = score;
            MainPage.enemySpawned = enemySpawned;

            //MainPage.rectangleEnemy.Clear();
            this.Boni = Boni;
            this.enemies = enemies;

            for (int i = 0; i < health; i++)
                rectImage.Add(CreateLifeImg());
            UserLifeIndex();

            Canvas.SetLeft(tbScore, MainPage.windowRectangle.Width / 2 - tbScore.Width / 2);
            Canvas.SetTop(tbScore, 15);
        }
        //[JsonConstructor] // Characters FORM
        //public GameBoard(int numOfEnemies, int health, int score, int enemySpawned, List<Character> characters)
        //{
        //    this.numOfEnemies = numOfEnemies;
        //    this.health = health;
        //    MainPage.myScore = score;
        //    MainPage.enemySpawned = enemySpawned;
        //    this.characters = characters;

        //    for (int i = 0; i < characters.Count; i++)
        //        if (characters[i] is Player)
        //            Boni = (Player)characters[i];
        //        else if (characters[i] is Enemy)
        //            enemies.Add((Enemy)characters[i]);
        //        else if (characters[i] is Coin)
        //            coins.Add((Coin)characters[i]);
            
        //    for (int i = 0; i < health; i++)
        //        rectImage.Add(CreateLifeImg());

        //    Canvas.SetLeft(tbScore, MainPage.windowRectangle.Width / 2 - tbScore.Width / 2);
        //    Canvas.SetTop(tbScore, 15);
        //}
        public GameBoard(int boardWidth, int boardHeight)
        {
            _boardHeight = boardHeight;
            _boardWidth = boardWidth;
            characterHeight = 55; characterWidth = 55; playerSpeed = 11;
            numOfEnemies += 5 * MainPage.countOfVictories;

            characters = new List<Character>();
            enemies = new List<Enemy>();
            coins = new List<Coin>();

            // Add PLAYER
            AddCharacter(characterWidth, characterHeight, (int)MainPage.windowRectangle.Width / 2, (int)MainPage.windowRectangle.Height / 2, playerSpeed, 1);

            // Add ENEMIES
            for (int i = 0; i < numOfEnemies; i++)
            {
                RandomizeXandYEnemies(out int x, out int y);
                int radius = randomSize.Next(22, 32); // widht and height
                AddCharacter((int)(radius * MainPage._scaleBoardWidth), (int)(radius * MainPage._scaleBoardHeight), x , y , speedGenerator.Next(1, 2) + i / 2, 2);
            }

            // Add COINS
            for (int i = 0; i < numOfCoins; i++)
            {
                RandomizeXandYCoins(out int x, out int y);
                int radius = randomSize.Next(20, 30);
                AddCharacter((int)(radius * MainPage._scaleBoardWidth), (int)(radius * MainPage._scaleBoardHeight), x, y, 0, 3);
            }

            health = 3;
            for (int i = 0; i < health; i++)
                rectImage.Add(CreateLifeImg());

            Canvas.SetLeft(tbScore, MainPage.windowRectangle.Width / 2 - tbScore.Width / 2);
            Canvas.SetTop(tbScore, 15);
        }
        #endregion
        public void AddCharacter(int characterWidth, int characterHeight, int xLocation, int yLocation, int speedCharacter, int type)
        {
            if (type == 1)
            {
                Character player = new Player(characterWidth, characterHeight, xLocation, yLocation, speedCharacter);
                characters.Add((Player)player);
                Boni = (Player)player;
            }
            else if (type == 2)
            {
                Character enemy = new Enemy(characterWidth, characterHeight, xLocation, yLocation, speedCharacter);
                characters.Add(enemy);        // characters list
                enemies.Add((Enemy)enemy);    // ENEMIES LIST
            }
            else if (type == 3)
            {
                Character coin = new Coin(characterWidth, characterHeight, xLocation, yLocation);
                characters.Add(coin);
                coins.Add((Coin)coin);      // COIN LIST
            }
        }

        #region life/health set and Follow
        public Rectangle CreateLifeImg()
        {
            xLifeProperty -= (int)(70 * MainPage._scaleBoardWidth);
            Rectangle life = new Rectangle() { Height = 50 * MainPage._scaleBoardHeight, Width = 50 * MainPage._scaleBoardWidth, Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/redHeart.jpg")) } };
            Canvas.SetLeft(life, xLifeProperty);
            Canvas.SetTop(life, 25);
            return life;
        } 
        public void UserLifeIndex()
        {   
            if (health == 2)
                rectImage[0].Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/greyHeart.jpg")) };
            else if (health == 1)
                rectImage[1].Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/greyHeart.jpg")) };
            else if (health == 0)
            {
                rectImage[2].Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/greyHeart.jpg")) };
                MainPage.lvlEnded = true;
            }
        } // Life counter 
        #endregion
        private void RandomizeXandYCoins(out int x, out int y)
        {
            x= generateXandYlocation.Next(0, (int)(_boardWidth * MainPage._scaleBoardWidth));
            y = generateXandYlocation.Next(0, (int)(_boardHeight * MainPage._scaleBoardHeight));
        }     // coins Randomly on board
        private void RandomizeXandYEnemies(out int x, out int y)
        { // generetes the enemies on the boundries of the board randomly
            x = 0; y = 0;
            int setScreenBorder = randomSize.Next(1, 5); // randomize num to set which border to spwan the enemy
            switch (setScreenBorder)
            {
                case 1: // left borderScreen
                    x = 0;
                    y = generateXandYlocation.Next(0, (int)(_boardHeight * MainPage._scaleBoardHeight));
                    break;
                case 2: // top borderScreen
                    y = 0;
                    x = generateXandYlocation.Next(0, (int)(_boardWidth * MainPage._scaleBoardWidth));
                    break;
                case 3: // right borderScreen
                    x = _boardWidth;
                    y = generateXandYlocation.Next(0, (int)(_boardHeight * MainPage._scaleBoardHeight));
                    break;
                case 4: // bottom boardScreen
                    y = _boardHeight;
                    x = generateXandYlocation.Next(0, (int)(_boardWidth * MainPage._scaleBoardWidth));
                    break;
            }
            Debug.WriteLine(x + ", " + y, $"EnemyS Coordinates {countDebug}: "); countDebug++;
        }   // enemies at the edges
        public void EnemyAttraction()
        { // enemy "Move" - attraction to the player while they are alive (importante)
            foreach (Enemy enemy in enemies)
                if(enemy.isAlive)
                    enemy.Move(Boni);
        }

        #region Colllsion
        public bool PlayerCollisionWith_Coin(out int indexY)
        {
            for (int i = 0; i < MainPage.rectangleCoin.Count; i++)
                if (coins[i].isAlive == true && MainPage.rectangleCoin[i] != null)
                {
                    xSum = (coins[i]._x - Boni._x);
                    ySum = (coins[i]._y - Boni._y);
                    distanceBetweenRectangles = Math.Sqrt(Math.Pow(xSum, 2) + Math.Pow(ySum, 2));

                    if (distanceBetweenRectangles <= coins[i]._radius + Boni._radius)
                    {
                        coins[i].isAlive = false;
                        MainPage.myScore += 55;
                        SetScore();
                        indexY = i;
                        return true;
                    }
                }
            indexY = -1;
            return false;
        }
        public bool EnemyCollisionWith_EachOther(out int indexI/*, out int indexJ*/)
        {
            for (int i = 0; i < MainPage.rectangleEnemy.Count; i++)
                for (int j = 0; j < MainPage.rectangleEnemy.Count; j++)
                    if (enemies[i].isAlive && enemies[j].isAlive && j != i)
                    {
                        xSum = (enemies[j]._x - enemies[i]._x);
                        ySum = (enemies[j]._y - enemies[i]._y);
                        distanceBetweenRectangles = Math.Sqrt(Math.Pow(xSum, 2) + Math.Pow(ySum, 2));

                        if (distanceBetweenRectangles <= enemies[i]._radius + enemies[j]._radius)
                        {
                            MainPage.myScore += 100;
                            SetScore();
                            enemies[i].isAlive = false;
                            //enemies[j].isAlive = false;
                            indexI = i;
                            //indexJ = j;
                            return true;
                        }
                    }
            indexI = -1;
            //indexJ = -1;
            return false;
        }
        public Rectangle EnemyCollisionWith_Player(out int indexK, out TextBlock tbHighScore)
        {  // Distance Between two circle centers <= sum of radiuses
           // (x1 - x2)^2 + (y1 - y2)^2 = distance^2
           // if (distance <= r + R) => collision
            for (int i = 0; i < MainPage.rectangleEnemy.Count; i++)
            {
                if (enemies[i].isAlive == true && MainPage.rectangleEnemy[i] != null)
                {
                    xSum = (enemies[i]._x - Boni._x);
                    ySum = (enemies[i]._y - Boni._y);
                    distanceBetweenRectangles = Math.Sqrt(Math.Pow(xSum, 2) + Math.Pow(ySum, 2));

                    if (distanceBetweenRectangles <= enemies[i]._radius + Boni._radius)
                    {
                        enemies[i].isAlive = false;
                        health--;
                        MainPage.myScore -= 75;
                        SetScore();
                        UserLifeIndex();
                        if (health == 0)
                        {
                            // creating a gameOver image and hishScore textBlock 
                            Rectangle gameOver = new Rectangle() { Height = 450 * MainPage._scaleBoardHeight, Width = 645 * MainPage._scaleBoardWidth, RadiusX = 10, RadiusY = 10, Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/pressEnterToRestart.jpg")) }};

                            CheckHighScore();
                            tbHighScore = new TextBlock() { Text = $"Last High-Score: {MainPage.strHighScore}", Height = 76 * MainPage._scaleBoardHeight, Width = 550 * MainPage._scaleBoardWidth, FontSize = 45 * MainPage._scaleBoardHeight, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Windows.UI.Colors.Yellow), FontFamily = new FontFamily("/Assets/Fonts1/SpringInMay.ttf#Spring in May") };
                            
                            // settin their loaction on board
                            Canvas.SetLeft(tbHighScore, MainPage.windowRectangle.Width / 2 - tbHighScore.Width / 2); Canvas.SetTop(tbHighScore, MainPage.windowRectangle.Height - tbHighScore.Height);
                            Canvas.SetTop(gameOver, MainPage.windowRectangle.Height / 2 - gameOver.Height / 2); Canvas.SetLeft(gameOver, MainPage.windowRectangle.Width / 2 - gameOver.Width / 2);

                            MainPage.gameStarted = false;
                            MainPage.lvlEnded = true;
                            MainPage.gameOver = true;
                            MainPage.myScore = 0;
                            MainPage.countOfVictories = 0;

                            indexK = i;
                            return gameOver;
                        }
                        tbHighScore = null;
                        indexK = i;
                        return null;
                    }
                }
            }
            tbHighScore = null;
            indexK = -1;
            return null;

        } // collision between to enemies
        #endregion
        public bool Victory(out Rectangle tbVictory)
        {
            int countOfDeadEnemies = 0;
            //foreach (Rectangle rect in MainPage.rectangleEnemy)
            //    if (rect != null)
            //        countOfEnemies++;
            foreach (Enemy enemyAlive in enemies)
                if (!enemyAlive.isAlive && MainPage.enemySpawned == numOfEnemies)
                    countOfDeadEnemies++;
            if (countOfDeadEnemies == numOfEnemies || countOfDeadEnemies == numOfEnemies - 1)
            {
                tbVictory = new Rectangle()
                {
                    Height = MainPage.windowRectangle.Height,
                    Width = MainPage.windowRectangle.Width,
                    Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/victory.jpg")) }
                };
                Canvas.SetTop(tbVictory,0); Canvas.SetLeft(tbVictory,0);
                MainPage.lvlEnded = true;
                MainPage.gameStarted = false;
                MainPage.countOfVictories += 1;
                return true;
            }
            tbVictory = null;
            return false;
        }
        public void CheckHighScore()
        {
            if (MainPage.myScore > Convert.ToInt16(MainPage.strHighScore))
                /*await Task.Run(() =>*/ HighScore.UpdateScore(MainPage.myScore);
        }
        private void SetScore()
        {
            if (!MainPage.gameOver)
                tbScore.Text = $"Score: {MainPage.myScore}";
            else
               MainPage.myScore = 0;
        }
    }
}