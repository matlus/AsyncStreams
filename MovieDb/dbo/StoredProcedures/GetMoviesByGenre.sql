CREATE PROCEDURE [dbo].[GetMoviesByGenre]
	@Genre varchar(50)
AS
	SELECT	Title, Genre, Year, ImageUrl FROM dbo.MovieVw
	WHERE	Genre = @Genre

RETURN 0
