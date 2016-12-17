namespace Mbk.Dal.Repositories
{
    public abstract class BaseRepository
    {
        protected string ConnectionString { get; private set; }

        public BaseRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
