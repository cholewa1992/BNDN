using System.Data.Entity;
using EntityFrameworkStorage;

namespace EntityFrameworkStorageUnitTest
{
    /// <summary>
    /// Helper class to test Linq-to-sql
    /// </summary>
    /// <author>
    /// Jacob Cholewa (jbec@itu.dk)
    /// </author>
    class MockFakeImdbContext : DbContext, IDbContext
    {
        public MockFakeImdbContext()
        {
            Database.ExecuteSqlCommand("DELETE FROM UserAccs");
        }

        public IDbSet<UserAcc> UserAcc { set; get; }
        public IDbSet<FavouritedMovie> FavouritedMovie { set; get; }
        public IDbSet<FavouriteList> FavouriteList { set; get; }
        public IDbSet<Movies> Movies { set; get; }
        public IDbSet<People> People { set; get; }
        public IDbSet<PersonInfo> PersonInfo { set; get; }
        public IDbSet<MovieInfo> MovieInfo { set; get; }
        public IDbSet<Participate> Participate { set; get; }
        public IDbSet<InfoType> InfoType { set; get; }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }
    }
}