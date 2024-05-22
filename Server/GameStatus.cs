namespace server
{

    class GameStatus
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
}
