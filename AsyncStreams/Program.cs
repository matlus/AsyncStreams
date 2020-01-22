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

        private readonly MovieDataManager _movieDataManager;

        static void Main()
        {
            /*
             * Do this only the first time when you need
             * to initilize the database with data.
             * Also modify the Main() method's signature
             * to be async and returning a Task instead of void
             */
            ////await InitializeDataInDatabase(10000);

            BenchmarkRunner.Run<Program>();
        }

        public Program()
        {
            _movieDataManager = new MovieDataManager();
        }

        [Benchmark]
        public async Task<int> GetAllMoviesAsyncStreaming()
        {
            var total = 0;

            await foreach (var movie in _movieDataManager.GetAllMoviesAsyncStreaming().ConfigureAwait(false))
            {
                total += movie.Year;
            }

            return total;
        }

        [Benchmark]
        public async Task<int> GetAllMovies()
        {
            var total = 0;
            var allMovies = await _movieDataManager.GetAllMovies().ConfigureAwait(false);

            foreach (var movie in allMovies)
            {
                total += movie.Year;
            }
            
            return total;
        }

        private static async Task InitializeDataInDatabase(int numberOfRecords)
        {
            var movieDataManager = new MovieDataManager();
            var uniqueMovieTitles = Randomizer.GenerateUniqueAsciiStrings(numberOfRecords);
            var movies = GetRandomMovies(numberOfRecords, uniqueMovieTitles);
            await movieDataManager.CreateMoviesTvpMergeInsertInto(movies);
        }

        private static IEnumerable<Movie> GetRandomMovies(int requiredCount, string[] uniqueMovieTitles)
        {
            var count = 0;
            do
            {
                yield return CreateRandomMovie(uniqueMovieTitles[count]);
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
                imageUrl: "http://www." + Randomizer.GetRandomAciiStringNoPunctuations(20) + ".com");
        }
    }
}
