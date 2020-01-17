CREATE TYPE dbo.MovieTvp AS TABLE 
(
	Title varchar(50) NOT NULL, 
	Genre varchar(50) NOT NULL, 
	Year int NOT NULL,
	ImageUrl varchar(200) NOT NULL 
    PRIMARY KEY (Title)
)
GO