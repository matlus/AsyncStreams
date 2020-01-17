using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AsyncStreams
{
    internal sealed class MovieDataManager
    {
        private static readonly SqlClientFactory sqlClientFactory = SqlClientFactory.Instance;        

        private static DbConnection CreateDbConnection()
        {
            var dbConnection = sqlClientFactory.CreateConnection();
            dbConnection.ConnectionString = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=MovieDb;Integrated Security=True;TrustServerCertificate=True;";
            return dbConnection;
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            var dbConnection = CreateDbConnection();
            ////DbCommand dbCommand = null;
            try
            {
                await dbConnection.OpenAsync().ConfigureAwait(false);
                var dbCommand = CommandFactoryMovies.CreateCommandForGetAllMovies(dbConnection);
                var dbDataReader = await dbCommand.ExecuteReaderAsync().ConfigureAwait(false);
                return MapToMovies(dbDataReader);
            }
            finally
            {
                ////dbCommand?.Dispose();
                dbConnection.Dispose();
            }
        }

        private static IEnumerable<Movie> MapToMovies(DbDataReader dbDataReader)
        {
            var movies = new List<Movie>();

            while (dbDataReader.Read())
            {
                movies.Add(new Movie(
                    title: (string)dbDataReader[0],
                    genre: GenreParser.Parse((string)dbDataReader[1]),
                    year: (int)dbDataReader[2],
                    imageUrl: (string)dbDataReader[3]));
            }

            return movies;
        }

        public async IAsyncEnumerable<Movie> GetAllMoviesAsyncStreaming()
        {
            var dbConnection = CreateDbConnection();
            ////DbCommand dbCommand = null;
            DbDataReader dbDataReader = null;
            try
            {
                await dbConnection.OpenAsync().ConfigureAwait(false);
                var dbCommand = CommandFactoryMovies.CreateCommandForGetAllMovies(dbConnection);
                dbDataReader = await dbCommand.ExecuteReaderAsync().ConfigureAwait(false);

                var movie = new Movie();
                while (dbDataReader.Read())
                {
                    movie.Title = (string)dbDataReader[0];
                    movie.Genre = GenreParser.Parse((string)dbDataReader[1]);
                    movie.Year = (int)dbDataReader[2];
                    movie.ImageUrl = (string)dbDataReader[3];
                    yield return movie;
                }
            }
            finally
            {
                dbDataReader?.Dispose();
                ////dbCommand?.Dispose();
                dbConnection.Dispose();
            }
        }

        public async Task CreateMoviesTvpMergeInsertInto(IEnumerable<Movie> movies)
        {
            DbConnection dbConnection = null;
            DbTransaction dbTransaction = null;
            DbCommand dbCommand = null;
            try
            {
                dbConnection = CreateDbConnection();
                await dbConnection.OpenAsync().ConfigureAwait(false);
                dbTransaction = dbConnection.BeginTransaction();
                dbCommand = CommandFactoryMovies.CreateCommandForCreateMoviesTvpMergeInsertInto(dbConnection, dbTransaction, movies);
                await dbCommand.ExecuteNonQueryAsync();
                dbTransaction.Commit();
            }
            catch (DbException)
            {
                dbTransaction?.Rollback();
                throw;
            }
            finally
            {
                dbCommand?.Dispose();
                dbTransaction?.Dispose();
                dbConnection.Dispose();
            }
        }
    }
}
