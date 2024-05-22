using database;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace server
{
    internal class Server
    {
        static List<ConnectedClient> connectedPlayers = new List<ConnectedClient>();

        static List<GameRoom> gameRooms = new List<GameRoom>();

        static int id = 1;

        static void CreateRoom(string player1)
        {
            GameRoom gm = new GameRoom(id);
            id++;

            gm.players.Add(player1);

            gameRooms.Add(gm);

            Console.WriteLine($"Room {gm.id} created by {player1}");
        }

        static string ShowRooms()
        {
            string roomsInfo = "";

            foreach (var room in gameRooms)
            {
                string id = room.id.ToString();
                roomsInfo += id + ":" + room.players[0] + "|";
            }

            roomsInfo = roomsInfo.Remove(roomsInfo.Length - 1);

            return roomsInfo.ToString(); ;
        }

        static void HandleClient(Object obj)
        {
            TcpClient client = obj as TcpClient;

            if (client == null)
            {
                Console.WriteLine("TCP client is null...");

                client.Close();
            }

            String data = null;
            Byte[] buffer = new Byte[1024];

            NetworkStream stream = client.GetStream();

            int i;

            try
            {
                while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    data = System.Text.Encoding.ASCII.GetString(buffer, 0, i); //ACTION:nick or ACTION:nick:roomID

                    string[] parts = data.Split(':');
                    string playerName = parts[1];

                    if (parts.Length >= 2)
                    {
                        if (parts[0] == "CREATE")
                        {
                            CreateRoom(playerName);
                        }
                        else if (parts[0] == "JOIN")
                        {
                            int roomID = int.Parse(parts[2]);

                            foreach (var room in gameRooms)
                            {
                                if (room.id == roomID)
                                {
                                    if (room.players.Count < 2)
                                    {
                                        room.players.Add(playerName);
                                    }

                                    Console.WriteLine($"Player {playerName} joined room {roomID}");

                                    StartGame(room);
                                }
                            }
                        }
                        else if (parts[0] == "ENTER")
                        {
                            Console.WriteLine($"Player {playerName} joined!");
                            ConnectedClient CC = new ConnectedClient
                            {
                                PlayerName = playerName,
                                Client = client
                            };

                            connectedPlayers.Add(CC);

                            database.Database.Add(playerName);
                        }
                        else if (parts[0] == "SHOW")
                        {
                            string s = ShowRooms();
                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(s);
                            stream.Write(msg, 0, msg.Length);
                        }
                        else if (parts[0] == "EXIT")
                        {
                            string playerNameToRemove = playerName;

                            connectedPlayers.RemoveAll(x => x.PlayerName == playerName);

                            stream.Close();
                            client.Close();

                            Console.WriteLine($"Player {playerName} left the server");

                            return;
                        }
                        else if (parts[0] == "LEADERBOARD")
                        {
                            List<database.Player> players = database.Database.returnDB();
                            string json = JsonConvert.SerializeObject(players);
                            byte[] jsondata = Encoding.UTF8.GetBytes(json);
                            stream.Write(jsondata, 0, jsondata.Length);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException: {0}", e.Message);
            }
        }

        public static void Main(string[] args)
        {
            TcpListener server = null;

            database.Database.Main();

            try
            {
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                server.Start();

                Console.WriteLine("Server is working...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Client connected!");

                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
                Console.WriteLine("Server stopped...");
            }
        }

        static void StartGame(GameRoom room)
        {
            Game game = new Game(room);
            game.Start();
        }

        internal class Game
        {
            public GameRoom room;

            GameStatus gamestatus;

            public Game(GameRoom room)
            {
                this.room = room;
                this.gamestatus = new GameStatus();
                this.gamestatus.P1 = room.players[0];
                this.gamestatus.P2 = room.players[1];
            }

            public async Task Start()
            {
                Console.WriteLine($"Game started in room {room.id} between players {room.players[0]} and {room.players[1]}");



                while (true)
                {
                    foreach (var playerName in room.players)
                    {
                        SendToClient(playerName, gamestatus);
                    }

                    try
                    {
                        ConnectedClient currentClient;
                        NetworkStream stream = null;
                        byte[] data = new byte[1024];

                        if (gamestatus.turn == 1)
                        {
                            currentClient = connectedPlayers.FirstOrDefault(x => x.PlayerName == gamestatus.P1);
                            stream = currentClient.Client.GetStream();

                            int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                            string json = Encoding.UTF8.GetString(data, 0, bytesRead);
                            gamestatus = JsonConvert.DeserializeObject<GameStatus>(json);
                        }
                        else if(gamestatus.turn == 2) 
                        {
                            currentClient = connectedPlayers.FirstOrDefault(x => x.PlayerName == gamestatus.P2);
                            stream = currentClient.Client.GetStream();

                            int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                            string json = Encoding.UTF8.GetString(data, 0, bytesRead);
                            gamestatus = JsonConvert.DeserializeObject<GameStatus>(json);
                        }                      
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("IOException: {0}", ex);
                        return;
                    }

                    if(IsResolved(gamestatus.board) == true)
                    {
                        gamestatus.winner = gamestatus.turn;

                        if (gamestatus.winner == 1) 
                        {
                            database.Database.Update(gamestatus.P1, true);
                            database.Database.Update(gamestatus.P2, false);
                        }
                        else if(gamestatus.winner == 2)
                        {
                            database.Database.Update(gamestatus.P1, false);
                            database.Database.Update(gamestatus.P2, true);
                        }
                        foreach (var playerName in room.players)
                        {
                            SendToClient(playerName, gamestatus);
                        }

                        gameRooms.Remove(room);

                        return;
                    }
                    else
                    {
                        if(IsDraw(gamestatus.board) == true)
                        {
                            gamestatus.winner = 3;

                            database.Database.Update(gamestatus.P1, false);
                            database.Database.Update(gamestatus.P2, false);

                            foreach (var playerName in room.players)
                            {
                                SendToClient(playerName, gamestatus);
                            }

                            gameRooms.Remove(room);

                            return;
                        }
                    }

                    if(gamestatus.turn == 1)
                    {
                        gamestatus.turn = 2;
                    }
                    else
                    {
                        gamestatus.turn = 1;
                    }
                }
            }

            static void SendToClient(string playerName, GameStatus gs)
            {
                ConnectedClient connectedClient = connectedPlayers.FirstOrDefault(x => x.PlayerName == playerName);

                if (connectedClient != null && connectedClient.Client.Connected)
                {
                    try
                    {
                        NetworkStream stream = connectedClient.Client.GetStream();
                        string json = JsonConvert.SerializeObject(gs);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        stream.Write(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message to client {playerName}: {ex.Message}");
                    }
                }
            }

            static bool IsResolved(int[,] board)
            {
                if (board[0, 0] == board[0, 1] && board[0, 1] == board[0, 2] && board[0, 0] != 0)
                {
                    return true;
                }
                else if (board[1, 0] == board[1, 1] && board[1, 1] == board[1, 2] && board[1, 0] != 0)
                {
                    return true;
                }
                else if (board[2, 0] == board[2, 1] && board[2, 1] == board[2, 2] && board[2, 0] != 0)
                {
                    return true;
                }
                else if (board[0, 0] == board[1, 0] && board[1, 0] == board[2, 0] && board[0, 0] != 0)
                {
                    return true;
                }
                else if (board[0, 1] == board[1, 1] && board[1, 1] == board[2, 1] && board[0, 1] != 0)
                {
                    return true;
                }
                else if (board[0, 2] == board[1, 2] && board[1, 2] == board[2, 2] && board[0, 2] != 0)
                {
                    return true;
                }
                else if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[0, 0] != 0)
                {
                    return true;
                }
                else if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] && board[0, 2] != 0)
                {
                    return true;
                }
                return false;
            }
            static bool IsDraw(int[,] board)
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (board[row, col] == 0)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}
