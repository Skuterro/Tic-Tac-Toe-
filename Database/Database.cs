using Microsoft.EntityFrameworkCore;
using System;

namespace database
{
    public class ShipsGameDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=*******;Database=TicTacToe");
        }
    }

    public class Database
    {
        public static ShipsGameDbContext dbContext;

        public static void Update(string name, bool Won)
        {
            var player = dbContext.Players.FirstOrDefault(p => p.Nickname == name);
       
            if (player != null)
            {
                player.GamesPlayed++;
            }
            if (Won)
            {
                player.GamesWon++;
            }

            decimal winRatio = (decimal)player.GamesWon / player.GamesPlayed * 100;
            player.WinRatio = winRatio.ToString("0.00") + "%";

            dbContext.SaveChanges();
        }

        public static void Add(string name)
        {
            var existingPlayer = dbContext.Players.FirstOrDefault(p => p.Nickname == name);

            if (existingPlayer == null)
               {
               var player = new Player()
               {
                   Nickname = name,
               };
               dbContext.Players.Add(player);
               dbContext.SaveChanges();
            }            
        }

        public static List<Player> returnDB()
        {
            List<Player> players = dbContext.Players.ToList();

            return players;
        }

        public static void Main()
        {
            dbContext = new ShipsGameDbContext();
            
            dbContext.Database.EnsureCreated();           
        }
    }
}
