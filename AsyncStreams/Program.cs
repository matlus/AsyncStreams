using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncStreams
{    
    [MemoryDiagnoser]
    public class Program
    {
        private static readonly int[] s_validYears = new int[] { 2000, 2001, 2002, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019 };
        private static readonly string[] s_validGenres = GenreParser.GenreValues.ToArray();
        private static string[] s_uniqueMovieTitles;

        private MovieDataManager _movieDataManager;

        static void Main()
        {
            /*
             * Do this only the first time when you need
             * to initilize the database with data.
             * Also modify the Main() method's signature
             * to be async and returning a Task instead of void
             */
            ////await InitializeDataInDatabase(50000);

            ////_movieDataManager = new MovieDataManager();
            ////var program = new Program();
            ////await program.GetAllMoviesAsyncStreaming();
            ////await program.GetAllMoviesAsyncStreaming();


            BenchmarkRunner.Run<Program>();
        }

        private static async Task InitializeDataInDatabase(int numberOfRecords)
        {
            var movieDataManager = new MovieDataManager();
            s_uniqueMovieTitles = Randomizer.GenerateUniqueAsciiStrings(numberOfRecords);
            var movies = GetRandomMovies(numberOfRecords);
            await movieDataManager.CreateMoviesTvpMergeInsertInto(movies);
        }

        [GlobalSetup]
        public void Initialize()
        {
            _movieDataManager = new MovieDataManager();
        }

        [Benchmark]
        public async ValueTask<int> GetAllMoviesAsyncStreaming()
        {
            var total = 0;

            await foreach (var movie in _movieDataManager.GetAllMoviesAsyncStreaming())
            {
                total += movie.Year;
            }

            return total;
        }

        [Benchmark]
        public async ValueTask<int> GetAllMovies()
        {
            var total = 0;
            var allMovies = await _movieDataManager.GetAllMovies();

            foreach (var movie in allMovies)
            {
                total += movie.Year;
            }
            
            return total;
        }

        private static IEnumerable<Movie> GetRandomMovies(int requiredCount)
        {
            var count = 0;
            do
            {
                yield return CreateRandomMovie(s_uniqueMovieTitles[count]);
                count++;
            } while (count < requiredCount);
        }

        private static Movie CreateRandomMovie(string title)
        {
            var shuffledYears = Randomizer.ShuffleArray(s_validYears);
            var shuffledGenres = Randomizer.ShuffleArray(s_validGenres);

            return new Movie(
                title: title,
                genre: GenreParser.Parse(shuffledGenres[0]),
                year: shuffledYears[0],
                imageUrl: Randomizer.GetRandomAciiString(50));
        }
    }
}
