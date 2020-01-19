using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace AsyncStreams
{
    internal static class CommandFactoryMovies
    {
        private static readonly SqlMetaData[] s_sqlMetaDataCreateMovies = new SqlMetaData[]
        {
            new SqlMetaData("Title", SqlDbType.VarChar, 50),
            new SqlMetaData("Genre", SqlDbType.VarChar, 50),
            new SqlMetaData("Year", SqlDbType.Int),
            new SqlMetaData("ImageUrl", SqlDbType.VarChar, 200)
        };

        private static void AddReturnValueParameter(DbCommand dbCommand)
        {
            AddCommandParameter(dbCommand, "@Return", ParameterDirection.ReturnValue, DbType.Int32, null);
        }

        private static void AddCommandParameter(DbCommand dbCommand, string parameterName, ParameterDirection parameterDirection, DbType dbType, object value)
        {
            var dbParameter = dbCommand.CreateParameter();
            dbParameter.ParameterName = parameterName;
            dbParameter.Direction = parameterDirection;
            dbParameter.DbType = dbType;
            dbParameter.Value = value;
            dbCommand.Parameters.Add(dbParameter);
        }

        private static void AddCommandParameterTvp(DbCommand dbCommand, string parameterName, object value)
        {
            var dbParameter = (SqlParameter)dbCommand.CreateParameter();
            dbParameter.ParameterName = parameterName;
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.SqlDbType = SqlDbType.Structured;
            dbParameter.Value = value;
            dbCommand.Parameters.Add(dbParameter);
        }

        public static DbCommand CreateCommandForGetAllMovies(DbConnection dbConnection)
        {
            var dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.StoredProcedure;
            dbCommand.CommandText = "dbo.GetAllMovies";
            AddReturnValueParameter(dbCommand);
            return dbCommand;
        }

        public static DbCommand CreateCommandForGetMoviesByGenre(DbConnection dbConnection, Genre genre)
        {
            var dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.StoredProcedure;
            dbCommand.CommandText = "dbo.GetMoviesByGenre";
            AddReturnValueParameter(dbCommand);
            AddCommandParameter(dbCommand, "@Genre", ParameterDirection.Input, DbType.String, GenreParser.ToString(genre));
            return dbCommand;
        }

        public static DbCommand CreateCommandForGetMoviesByYear(DbConnection dbConnection, int year)
        {
            var dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.StoredProcedure;
            dbCommand.CommandText = "dbo.GetMoviesByYear";
            AddReturnValueParameter(dbCommand);
            AddCommandParameter(dbCommand, "@Year", ParameterDirection.Input, DbType.Int32, year);
            return dbCommand;
        }

        public static DbCommand CreateCommandForCreateMoviesTvpMergeInsertInto(DbConnection dbConnection, DbTransaction dbTransaction, IEnumerable<Movie> movies)
        {
            return CreateCommandForCreateMoviesUsingTvp(dbConnection, dbTransaction, movies, "dbo.CreateMoviesTvpMergeInsertInto");
        }

        private static DbCommand CreateCommandForCreateMoviesUsingTvp(DbConnection dbConnection, DbTransaction dbTransaction, IEnumerable<Movie> movies, string storedProcedureName)
        {
            var dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = storedProcedureName;
            dbCommand.CommandType = CommandType.StoredProcedure;
            dbCommand.Transaction = dbTransaction;
            AddReturnValueParameter(dbCommand);
            AddCommandParameterTvp(dbCommand, "@MovieTvp", ConvertToSqlDataRecord(movies));
            return dbCommand;
        }

        public static DbCommand CreateCommandForCreateMovie(DbConnection dbConnection, DbTransaction dbTransaction, Movie movie)
        {
            var dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "dbo.CreateMovie";
            dbCommand.CommandType = CommandType.StoredProcedure;
            dbCommand.Transaction = dbTransaction;
            AddReturnValueParameter(dbCommand);
            AddCommandParameter(dbCommand, "@Title", ParameterDirection.Input, DbType.String, movie.Title);
            AddCommandParameter(dbCommand, "@Genre", ParameterDirection.Input, DbType.String, movie.Genre);
            AddCommandParameter(dbCommand, "@Year", ParameterDirection.Input, DbType.Int32, movie.Year);
            AddCommandParameter(dbCommand, "@ImageUrl", ParameterDirection.Input, DbType.String, movie.ImageUrl);
            return dbCommand;
        }

        private static IEnumerable<SqlDataRecord> ConvertToSqlDataRecord(IEnumerable<Movie> movies)
        {
            var sqlDataRecord = new SqlDataRecord(s_sqlMetaDataCreateMovies);

            foreach (var movie in movies)
            {
                sqlDataRecord.SetString(0, movie.Title);
                sqlDataRecord.SetString(1, GenreParser.ToString(movie.Genre));
                sqlDataRecord.SetInt32(2, movie.Year);
                sqlDataRecord.SetString(3, movie.ImageUrl);
                yield return sqlDataRecord;
            }
        }
    }
}
