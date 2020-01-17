
namespace AsyncStreams
{
    public sealed class Movie
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public Genre Genre { get; set; }
        public int Year { get; set; }

        public Movie()
        {
        }

        public Movie(string title, string imageUrl, Genre genre, int year)
        {
            Title = title;
            ImageUrl = imageUrl;
            Genre = genre;
            Year = year;
        }
    }
}
