using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;
using Newtonsoft.Json;
using Windows.Media.Playback;
using Windows.Media.Core;
namespace DodgeGameAlonKolyakov
{
    public sealed partial class MainPage : Page
    {
        List<MediaPlayer> allMedia = new List<MediaPlayer>();
        MediaPlayer mediaPlayer, coinSound, loseSound, winCheers;

        private static List<DispatcherTimer> timers;
        private static DispatcherTimer enemySpawnTmr, checkerTmr, coinTmr;

        GameBoard gb;
        Random random = new Random();

        public static List<Rectangle> rectangleEnemy;
        public static List<Rectangle> rectangleCoin;
        private static Rectangle funinStartBG;

        public static Rect windowRectangle;
        public static int designWidth = 1280, designHeight = 720;
        public static double _scaleBoardWidth, _scaleBoardHeight;

        public static string strHighScore = string.Empty;
        public static int myScore, enemySpawned, coinSpawned, countOfVictories;
        public static bool lvlEnded = false, gameStarted = false, gamePaused = false, gameOver = false;

        public MainPage()
        {
            this.InitializeComponent();
            //ApplicationView.PreferredLaunchViewSize = new Size(designWidth, designHeight);
            windowRectangle = ApplicationView.GetForCurrentView().VisibleBounds;

            HighScore.CreateFile();
            HighScore.ReadFile();
            SetScale();
            CreateMediaPlayer();

            Window.Current.CoreWindow.SizeChanged += Current_SizeChanged;
            Window.Current.CoreWindow.KeyDown += SettingBtns_KeyDown;

            #region windowSize change debug
            LayoutRoot.SizeChanged += (sender, args) => { Debug.WriteLine($"{DateTime.Now.Ticks} Grid size changed, layout update triggered."); };
            Window.Current.SizeChanged += (sender, args) => { Debug.WriteLine($"{DateTime.Now.Ticks} Window size changed."); };
            #endregion

            StartingBackGround();
        }
        private void SetTimers()
        {
            timers = new List<DispatcherTimer>();
            // Enemy spawn timer
            enemySpawnTmr = new DispatcherTimer();
            timers.Add(enemySpawnTmr);
            enemySpawnTmr.Tick -= EnemySpawnTmr_Tick;
            enemySpawnTmr.Tick += EnemySpawnTmr_Tick;
            enemySpawnTmr.Interval = new TimeSpan(0, 0, 0, 0, random.Next(750, 2000));
            enemySpawnTmr.Start();

            // Checker timer
            checkerTmr = new DispatcherTimer();
            timers.Add(checkerTmr);
            checkerTmr.Tick += ManageTmr_Tick;
            checkerTmr.Interval = new TimeSpan(0, 0, 0, 0, 10);
            checkerTmr.Start();

            // Coins Spawner
            coinTmr = new DispatcherTimer();
            timers.Add(coinTmr);
            coinTmr.Tick += CoinSpawnTmr_Tick; ;
            coinTmr.Interval = new TimeSpan(0, 0, 0, 5, 0);
            coinTmr.Start();
        } // setting up timers
        public static void SetScale()
        {
           // Display information
            _scaleBoardWidth = windowRectangle.Width / (double)designWidth;
            _scaleBoardHeight = windowRectangle.Height / (double)designHeight;
            Debug.WriteLine($" Width: {windowRectangle.Width}, Height: {windowRectangle.Height}", "Window parameters:");

           // for the next SizeChange that we would compare it to the previous size
            designWidth = (int)windowRectangle.Width;
            designHeight = (int)windowRectangle.Height;
        }
        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            windowRectangle = ApplicationView.GetForCurrentView().VisibleBounds; 
            SetScale(); // gets the width and height parameters to the set scale func

            foreach (UIElement item in this.LayoutRoot.Children)
            {
                if (item is Rectangle)
                {
                    Rectangle rect = (Rectangle)item;
                    rect.Height *= _scaleBoardHeight;
                    rect.Width *= _scaleBoardWidth;
                    rect.RadiusX *= _scaleBoardWidth;
                    rect.RadiusY *= _scaleBoardHeight;
                }
                if(item is TextBlock)
                {
                    TextBlock tb = (TextBlock)item;
                    tb.Height *= _scaleBoardHeight;
                    tb.Width *= _scaleBoardWidth;
                }
                double x = Canvas.GetLeft(item);
                double y = Canvas.GetTop(item);
                Canvas.SetLeft(item, x * _scaleBoardWidth);
                Canvas.SetTop(item, y * _scaleBoardHeight);
            }
        }
        // window changes and also the other UIelemnets by the scaling of the board
        #region Timers
        private void ManageTmr_Tick(object sender, object e)
        {
            if (gb.Victory(out Rectangle tbVictory))
            {
                mediaPlayer.Volume = 0.03; 
                winCheers.Play();
                LayoutRoot.Children.Add(tbVictory);
                foreach (DispatcherTimer dt in timers)
                    dt.Stop();
            }
            gb.EnemyAttraction();

            if (gb.EnemyCollisionWith_EachOther(out int indexI/*, out int indexJ*/))
            { // if i would like to remove both if they intersect
                LayoutRoot.Children.Remove(rectangleEnemy[indexI]);
                //LayoutRoot.Children.Remove(rectangleEnemy[indexJ]);
                rectangleEnemy[indexI] = null;
                //rectangleEnemy[indexJ] = null;
            }

            if(gb.PlayerCollisionWith_Coin(out int indexY))
            {
                coinSound.Play();
                LayoutRoot.Children.Remove(rectangleCoin[indexY]);
                rectangleCoin[indexY] = null;
            }

            Rectangle gameOverRect = gb.EnemyCollisionWith_Player(out int indexK, out TextBlock tbHighScore);
            if (gameOverRect != null && indexK >= 0) // if there is an enemy that intersects with the player
            {                                        // and textBlock has return it means there is no lives left
                ResumePauseBtn.Visibility = Visibility.Collapsed;
                RestartBtn.Visibility = Visibility.Visible;
                LoadBtn.Visibility = Visibility.Visible;
                ExitBtn.Visibility = Visibility.Visible;
                LayoutRoot.Children.Add(gameOverRect);
                LayoutRoot.Children.Add(tbHighScore);
                gameOver = true;
                foreach (DispatcherTimer dt in timers)
                    dt.Stop();
                loseSound.Play();
            }
            else if (gameOverRect == null && indexK >= 0) // if there is an enemy that intersects with the player 
            {                                             // and index HAS NOT BEEN return it means there are more lives left 1 / 2
                LayoutRoot.Children.Remove(rectangleEnemy[indexK]);
                        rectangleEnemy[indexK] = null;
            }
        }
        private void CoinSpawnTmr_Tick(object sender, object e)
        {
            int i = coinSpawned;
            if (i < gb.coins.Count && rectangleCoin[i] != null) // spawns coins every 5 seconds
            {
                LayoutRoot.Children.Add(gb.coins[i].rect);
                gb.coins[i].isAlive = true;

                Debug.WriteLine(Canvas.GetLeft(gb.coins[i].rect) + ", " + Canvas.GetTop(gb.coins[i].rect), "coin's Coordinates");
                    // Coins coordiantes on the output Debug
                coinSpawned++;
            }
        }
        private void EnemySpawnTmr_Tick(object sender, object e)
        {
            int i = enemySpawned;
            if (i < gb.enemies.Count && rectangleEnemy[i] != null) // spawns enemy every 0.75 - 2 seconds
            {
                LayoutRoot.Children.Add(gb.enemies[i].rect);
                gb.enemies[i].isAlive = true;

                Debug.WriteLine(Canvas.GetLeft(gb.enemies[i].rect) + ", " + Canvas.GetTop(gb.enemies[i].rect), "enemy's Coordinates");
                // Enemies coordiantes on the output Debug
                enemySpawned++;
            }
        } // enemy spawns randomly
        #endregion
        private void SettingBtns_KeyDown(CoreWindow sender, KeyEventArgs e)
        {
            switch (e.VirtualKey)
            {
                case VirtualKey.Escape: // escape to pause and set the settingBar at the top left corner
                    if (!lvlEnded && !gamePaused && !gameStarted)
                        return;
                    if (!lvlEnded)
                        if (!gamePaused)
                            Pause();
                        else if (gamePaused)
                            Resume();
                    break;
                case VirtualKey.Enter:
                    if (!gameStarted && !lvlEnded)
                    {
                        funinStartBG.Visibility = Visibility.Collapsed;
                        StartGame();
                        gameStarted = true;
                        lvlEnded = false;
                    }
                    else if (lvlEnded)
                        ReStart();
                    break;
                case VirtualKey.Left:
                case VirtualKey.Right:
                case VirtualKey.Up:
                case VirtualKey.Down:
                    if (gameOver)
                        break;
                    else if ((gameStarted && !gamePaused) || lvlEnded) // gameEnded deosnt work
                        gb.Boni.Move(e);
                    break;
            }
        }

        // startGame, resume/pause, restart..
        #region Game Setting options
        private void StartGame()
        {
            rectangleEnemy = new List<Rectangle>();
            rectangleCoin = new List<Rectangle>();

            gb = new GameBoard(designWidth, designHeight);
            LayoutRoot.Children.Add(gb.tbScore);
            LayoutSettingsDesign();

            enemySpawned = 0; coinSpawned = 0;

            for(int i = 0; i < gb.characters.Count; i++)
                if(gb.characters[i] is Player)
                    LayoutRoot.Children.Add(gb.characters[i].rect);
                else if(gb.characters[i] is Enemy)
                    rectangleEnemy.Add(gb.characters[i].rect);
                else if(gb.characters[i] is Coin)
                    rectangleCoin.Add(gb.characters[i].rect);
            
            foreach (Rectangle imgHealth in gb.rectImage) // creates the health images at the right top corner
                LayoutRoot.Children.Add(imgHealth);

            gameStarted = true;
            SetTimers();
            Save(); // autoSave after finishing a lvl
        }
        private void StartLoadGameCharacters(GameBoard DodgeGameFromJson)
        {
            List<Character> charactersLoad = DodgeGameFromJson.characters;
            LayoutRoot.Children.Clear();
            rectangleEnemy.Clear();
            rectangleCoin.Clear();
            gb.enemies.Clear();
            gb.coins.Clear();

            LayoutSettingsDesign();
            foreach (Rectangle imgHealth in gb.rectImage) // creates the health images at the right top corner
                LayoutRoot.Children.Add(imgHealth);

            for (int i = 0; i < charactersLoad.Count; i++)
            {
                if(charactersLoad[i].type == 1)
                {
                    gb.Boni = (Player)charactersLoad[i];
                    LayoutRoot.Children.Add(gb.Boni.rect);
                }
                else if(charactersLoad[i].type == 2)
                {
                    gb.enemies.Add((Enemy)charactersLoad[i]);
                    rectangleEnemy.Add(charactersLoad[i].rect);
                }
                else if(charactersLoad[i].type == 3)
                {
                    gb.coins.Add((Coin)charactersLoad[i]);
                    rectangleCoin.Add(charactersLoad[i].rect);
                }
            }
            #region magic doesn't work
            //for (int i = 0; i < charactersLoad.Count; i++)
            //    if (charactersLoad[i] is Player)
            //    {
            //        gb.Boni = (Player)charactersLoad[i];
            //        LayoutRoot.Children.Add(gb.Boni.rect);
            //    }
            //    else if (charactersLoad[i] is Enemy)
            //    {
            //        gb.enemies.Add((Enemy)charactersLoad[i]);
            //        rectangleEnemy.Add(charactersLoad[i].rect);
            //    }
            //    else if (DodgeGameFromJson.characters[i] is Coin)
            //    {
            //        gb.coins.Add((Coin)charactersLoad[i]);
            //        rectangleCoin.Add(charactersLoad[i].rect);
            //    }
            #endregion
            LayoutRoot.Children.Add(gb.tbScore);
            gb.UserLifeIndex();
            
            foreach (Character character in charactersLoad)
                if (character.isAlive)
                    LayoutRoot.Children.Add(character.rect);

            Pause();
            lvlEnded = false;
            gameOver = false;
            gameStarted = true;
            SetTimers();
        } // don't forget to ask
        private void StartLoadGameEnemiesAndPlayer(GameBoard DodgeGameFromJson)
        {
            //List<Character> characters1 = DodgeGameFromJson.characters;
            List<Enemy> enemiesLoad = DodgeGameFromJson.enemies;
            Player BoniLoad = DodgeGameFromJson.Boni;
            LayoutRoot.Children.Clear();
            rectangleEnemy.Clear();

            // gb = new GameBoard(DodgeGameFromJson.numOfEnemies, DodgeGameFromJson.health, myScore, enemySpawned, enemiesLoad, BoniLoad);
            LayoutSettingsDesign();
            gb.UserLifeIndex();
            gb.Boni = BoniLoad;
            LayoutRoot.Children.Add(gb.Boni.rect);

            for (int i = 0; i < gb.enemies.Count; i++)
            {
                gb.enemies[i] = enemiesLoad[i];
                rectangleEnemy.Add(gb.enemies[i].rect);
            }
            LayoutRoot.Children.Add(gb.tbScore);

            foreach (Rectangle imgHealth in gb.rectImage) // creates the health images at the right top corner
                LayoutRoot.Children.Add(imgHealth);

            foreach (Enemy enemyCheck in gb.enemies)
                if (enemyCheck.isAlive)
                    LayoutRoot.Children.Add(enemyCheck.rect);
            Pause();
            gamePaused = true;
            lvlEnded = false;
            gameOver = false;
            gameStarted = true;
            //SetTimers();
        }
        private void CloseApp() => CoreApplication.Exit();
        private void ReStart()
        { 
            lvlEnded = false;
            gameStarted = false;
            gameOver = false;
            gamePaused = false;

            #region clear·ance
            LayoutRoot.Children.Clear();
            gb.enemies.Clear();
            rectangleEnemy.Clear();
            gb.coins.Clear();
            rectangleCoin.Clear();
            gb.characters.Clear();
#endregion

            StartGame();
            mediaPlayer.Volume = 0.2;
        }
        private void Pause()
        {
            ResumePauseBtn.Content = "Resume";
            #region Visible buttons
            RestartBtn.Visibility = Visibility.Visible;
            LoadBtn.Visibility = Visibility.Visible;
            SaveBtn.Visibility = Visibility.Visible;
            ExitBtn.Visibility = Visibility.Visible;
            #endregion
            foreach(DispatcherTimer dt in timers)
                dt.Stop();
            gamePaused = true;
            mediaPlayer.Pause();
        }
        private void Resume()
        {
            ResumePauseBtn.Content = "Pause";
            #region Collapse buttons
            RestartBtn.Visibility = Visibility.Collapsed;
            LoadBtn.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;
            ExitBtn.Visibility = Visibility.Collapsed;
            readTextJson.Visibility = Visibility.Collapsed;
            #endregion
            foreach (DispatcherTimer dt in timers)
                dt.Start();
            gamePaused = false;
            mediaPlayer.Play();
        }
        #endregion
        private void StartingBackGround()
        {
            funinStartBG = new Rectangle() { Height = windowRectangle.Height, Width = windowRectangle.Width, Visibility = Visibility.Visible, Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Images/backGround.jpg")) } };
            funinStartBG.PointerPressed += FuninStartBG_PointerPressed;
            Canvas.SetLeft(funinStartBG, 0);
            Canvas.SetTop(funinStartBG, 0);
            LayoutRoot.Children.Add(funinStartBG);
        }
        private void FuninStartBG_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            funinStartBG.Visibility = Visibility.Collapsed;
            StartGame();
            gameStarted = true;
            lvlEnded = false;
        }
        private void CreateMediaPlayer()
        {
            mediaPlayer = new MediaPlayer() { AutoPlay = true, Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/dodgeGameSong.mp3")), Volume = 0.2, IsLoopingEnabled = true };
            coinSound = new MediaPlayer() { AutoPlay = false, Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/coinSound.wav")), Volume = 0.4, IsLoopingEnabled = false };
            loseSound = new MediaPlayer() { AutoPlay = false, Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/loseSound.wav")), Volume = 0.5, IsLoopingEnabled = false };
            winCheers = new MediaPlayer() { AutoPlay = false, Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/maleVoiceCheer.wav")), Volume = 1.0, IsLoopingEnabled = false };
            allMedia.Add(mediaPlayer); allMedia.Add(coinSound); allMedia.Add(loseSound); allMedia.Add(winCheers);
        }

        // LayOut stackPanel properties
        #region LayoutSettingsDesign Props
        private TextBlock readTextJson;
        private StackPanel ButtonsStackPanel;
        private Button ResumePauseBtn, RestartBtn, ExitBtn, SaveBtn, LoadBtn;
        private SolidColorBrush inBouns = new SolidColorBrush(Color.FromArgb(155, 160, 120, 20)), outBouns = new SolidColorBrush(Color.FromArgb(65, 220, 100, 100));
        #endregion
        private void LayoutSettingsDesign()
        {
            //Create StackPanel for buttons
            ButtonsStackPanel = new StackPanel() {
                Margin = new Thickness(5),
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                BorderThickness = new Thickness(0),
            };
            Grid.SetColumn(ButtonsStackPanel, 0);
            Grid.SetRow(ButtonsStackPanel, 2);

            LayoutRoot.Children.Add(ButtonsStackPanel);
            ButtonsStackPanel.PointerEntered += ButtonsStackPanel_PointerEntered;
            ButtonsStackPanel.PointerExited += ButtonsStackPanel_PointerExited;

            ResumePauseBtn = new Button() { Content = "Pause", Width = 100, BorderBrush = inBouns, Background = outBouns, Visibility = Visibility.Visible };
            ResumePauseBtn.Click += ResumePauseBtn_Click;

            RestartBtn = new Button() { Content = "New Game", Width = 100, BorderBrush = inBouns, Background = outBouns, Visibility = Visibility.Collapsed };
            RestartBtn.Click += NewGameBtn_Click;

            SaveBtn = new Button() { Content = "Save Game", Width = 100, BorderBrush = inBouns, Background = outBouns, Visibility = Visibility.Collapsed };
            SaveBtn.Click += SaveBtn_Click;

            LoadBtn = new Button() { Content = "Load Game", Width = 100, BorderBrush = inBouns, Background = outBouns, Visibility = Visibility.Collapsed };
            LoadBtn.Click += LoadBtn_Click;

            ExitBtn = new Button() { Content = "Exit", Width = 100, BorderBrush = inBouns, Background = outBouns, Visibility = Visibility.Collapsed };
            ExitBtn.Click += ExitBtn_Click;

            readTextJson = new TextBlock() { Visibility = Visibility.Collapsed, Width = 750, Height = 400 };

            ButtonsStackPanel.Children.Add(ResumePauseBtn);
            ButtonsStackPanel.Children.Add(RestartBtn);
            ButtonsStackPanel.Children.Add(SaveBtn);
            ButtonsStackPanel.Children.Add(LoadBtn);
            ButtonsStackPanel.Children.Add(ExitBtn);
            ButtonsStackPanel.Children.Add(readTextJson);
        }
        #region Buttons Stack Panel Custom
        private void ButtonsStackPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button tempBtn;
            foreach (UIElement uIElement in ButtonsStackPanel.Children)
                if (uIElement is Button)
                {
                    tempBtn = (Button)uIElement;
                    tempBtn.Background = outBouns;
                    tempBtn.BorderBrush = inBouns;
                }
        }
        private void ButtonsStackPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button tempBtn;
            foreach (UIElement uIElement in ButtonsStackPanel.Children)
                if (uIElement is Button)
                {
                    tempBtn = (Button)uIElement;
                    tempBtn.Background = inBouns;
                    tempBtn.BorderBrush = outBouns;
                }
        }
        #endregion

        #region Click EXIT / NEW GAME / RESUME-PAUSE
        private void ExitBtn_Click(object sender, RoutedEventArgs e) => CloseApp();
        private void NewGameBtn_Click(object sender, RoutedEventArgs e) 
        {
            myScore = 0;
            countOfVictories = 0;
            ReStart();
            mediaPlayer.Play();
        }
        private void ResumePauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!gamePaused)
                Pause();
            else if (gamePaused)
                Resume();
        }
        #endregion
        public async void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            readTextJson.Visibility = Visibility.Visible;
            try
            {  // Read File path
                string jsonFromFile = null;
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await storageFolder.GetFileAsync("exposure.txt");
                jsonFromFile = readTextJson.Text = await FileIO.ReadTextAsync(sampleFile);
                var DodgeGameFromJson = JsonConvert.DeserializeObject<GameBoard>(jsonFromFile);
                //StartLoadGameCharacters(DodgeGameFromJson);
                StartLoadGameEnemiesAndPlayer(DodgeGameFromJson);
            }
                catch (Exception ex) { readTextJson.Text = $"Couldn't find any saved tedails"; }
        }
        public /*async*/ void SaveBtn_Click(object sender, RoutedEventArgs e) => Save();
        public void Save()
        {
            var jsonFromString = gb.PrintSeperatlyToSave();
            AddTextToFile(jsonFromString);
        }
        private async Task AddTextToFile(String textToSave)
        {
            var appFolder = ApplicationData.Current.LocalFolder;
            var file = await appFolder.CreateFileAsync("exposure.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, textToSave);
           // Track where the file is located
            Debug.WriteLine(String.Format($"File is located at {file.Path}"));
        }
    }
}