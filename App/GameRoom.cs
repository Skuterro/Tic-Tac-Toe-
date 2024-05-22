//using client;
namespace Ships_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class GameRoom
    {
        public GameRoom(int i)
        {
            id = i;
        }

        public int id;

        public List<string> players = new List<string>();
    }
}