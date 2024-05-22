using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using database;

namespace Ships_Game
{
    public partial class MainWindow : Window
    {
        public TcpClient client;
        public NetworkStream stream;
        public string serverAddr = "127.0.0.1";
        public int port = 13000;
        public string nickname;
        public string gameRoomsString;
        public int player;
        public GameStatus gameStatus;
        public List<database.Player> playerList;

        public MainWindow()
        {
            InitializeComponent();          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            nickname = PlayersNickname.Text;
            PlayersNickname.Text = "";

            InitializeClient();

            string msg = "ENTER:" + nickname;

            Byte[] buffer = System.Text.Encoding.ASCII.GetBytes(msg);

            NetworkStream stream = client.GetStream();

            stream.Write(buffer, 0, buffer.Length);

            UserControl userControl = new UserControl1(this);
            this.Content = userControl;
            
        }

        private void InitializeClient()
        {
            try
            {
                client = new TcpClient(serverAddr, port);
                stream = client.GetStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public class GameStatus
        {
            public string P1;
            public string P2;

            public int[,] board = new int[3, 3] { { 0, 0, 0 },
                                                  { 0, 0, 0 },
                                                  { 0, 0, 0 }
            };

            public int turn = 1;

            public int winner = 0;
        }

        public void CreateRoom()
        {
            string s = "CREATE:" + nickname;

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(s);

            try
            {
                stream.Write(msg, 0, msg.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }

            WaitingFor2ndPlayer waiting = new WaitingFor2ndPlayer();
            this.Content = waiting;

            GameHandle();
        }

        public async Task Leaderboard()
        {
            string s = "LEADERBOARD:" + nickname;

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(s);

            try
            {
                stream.Write(msg, 0, msg.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }

            try
            {
                byte[] data = new byte[1024];
                int bytesRead = await stream.ReadAsync(data, 0, data.Length);

                string json = Encoding.UTF8.GetString(data, 0, bytesRead);
                playerList = JsonConvert.DeserializeObject<List<database.Player>>(json);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }

            Leaderboard leaderboard = new Leaderboard(this);
            this.Content = leaderboard;
        }

        public async Task GameHandle()
        {
            gameStatus = new GameStatus();

            try
            {
                byte[] data = new byte[1024];
                int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                string json = Encoding.UTF8.GetString(data, 0, bytesRead);
                gameStatus = JsonConvert.DeserializeObject<GameStatus>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
                return;
            }

            if(gameStatus.P1 == nickname){ player = 1; }else{ player = 2; }

            Board board = new Board(this);
            this.Content = board;

            try { 
                while (true)
                {
                    byte[] data = new byte[1024];
                    int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                    string json = Encoding.UTF8.GetString(data, 0, bytesRead);
                    gameStatus = JsonConvert.DeserializeObject<GameStatus>(json);

                    if (gameStatus.winner == player)
                    {
                        Winner winner = new Winner(this);
                        this.Content = winner;
                    }
                    else if (gameStatus.winner == 0)
                    {
                        board = new Board(this);
                        this.Content = board;
                    }
                    else if(gameStatus.winner == 3) 
                    {
                        Draw draw = new Draw(this);
                        this.Content = draw;
                    }
                    else
                    {
                        Loser loser = new Loser(this);
                        this.Content = loser;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }

        public void MakeMove(int field)
        {
            switch (field)
            {
                case 1:
                    gameStatus.board[0, 0] = player;
                    break;
                case 2:
                    gameStatus.board[0, 1] = player;
                    break;
                case 3:
                    gameStatus.board[0, 2] = player;
                    break;
                case 4:
                    gameStatus.board[1, 0] = player;
                    break;
                case 5:
                    gameStatus.board[1, 1] = player;
                    break;
                case 6:
                    gameStatus.board[1, 2] = player;
                    break;
                case 7:
                    gameStatus.board[2, 0] = player;
                    break;
                case 8:
                    gameStatus.board[2, 1] = player;
                    break;
                case 9:
                    gameStatus.board[2, 2] = player;
                    break;
                default:
                    break;
            }

            try
            {
                string json = JsonConvert.SerializeObject(gameStatus);
                byte[] data = Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);

                //stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }

        }

        public void JoinRoom(int roomid)//JOIN:nick:ID
        {
            string s = "JOIN:" + nickname + ":" + roomid.ToString();

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(s);

            try
            {
                stream.Write(msg, 0, msg.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }

            GameHandle();
        }

        public void ShowRooms()
        {
            string s = "SHOW:" + nickname;

            Byte[] buffer = new byte[1024];
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(s);

            String data = null;

            try
            {
                stream.Write(msg, 0, msg.Length);
                int bytes = stream.Read(buffer, 0, buffer.Length);
                data = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);

                gameRoomsString = data;                           
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string s = "EXIT:" + nickname;

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(s);

            try
            {
                stream.Write(msg, 0, msg.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }

            stream.Close();
            client.Close();
        }
    }
}