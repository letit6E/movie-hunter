using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class XSVParser<T>
{
    public static List<T> Parse(string filePath, char sep, Func<string[], T> mapFunction)
    {
        var lines = File.ReadAllLines(filePath).Skip(1);
        return lines.Select(line => mapFunction(line.Split(sep))).ToList();
    }
}

class Movie
{
    public string TitleId { get; set; }  // IMDb TitleId
    public string Title { get; set; }
    public HashSet<string> Actors { get; set; } = new HashSet<string>();
    public string Director { get; set; }
    public HashSet<string> Tags { get; set; } = new HashSet<string>();
    public double Rating { get; set; }

    public override string ToString()
    {
        return $"Title: {Title}, Director: {Director}, Rating: {Rating}, Actors: {string.Join(", ", Actors)}, Tags: {string.Join(", ", Tags)}";
    }
}

class MovieDatabase
{
    private Dictionary<string, Movie> movies = new Dictionary<string, Movie>();
    private Dictionary<string, HashSet<Movie>> actorsDirectorsMovies = new Dictionary<string, HashSet<Movie>>();
    private Dictionary<string, HashSet<Movie>> tagsMovies = new Dictionary<string, HashSet<Movie>>();
    private Dictionary<string, string> movieIdToTitleId = new Dictionary<string, string>();

    public void AddMovie(Movie movie)
    {
        if (!movies.ContainsKey(movie.TitleId))
        {
            movies[movie.TitleId] = movie;
        }
    }

    public void AddActorDirectorToMovie(string personName, string titleId, bool isDirector)
    {
        if (movies.ContainsKey(titleId))
        {
            var movie = movies[titleId];

            if (isDirector)
            {
                movie.Director = personName;
            }
            else
            {
                movie.Actors.Add(personName);
            }

            if (!actorsDirectorsMovies.ContainsKey(personName))
            {
                actorsDirectorsMovies[personName] = new HashSet<Movie>();
            }
            actorsDirectorsMovies[personName].Add(movie);
        }
    }

    public void AddTagToMovie(string tag, string movieId)
    {
        if (movieIdToTitleId.ContainsKey(movieId))
        {
            string titleId = movieIdToTitleId[movieId];

            if (movies.ContainsKey(titleId))
            {
                var movie = movies[titleId];
                movie.Tags.Add(tag);
            }
        }
    }

    public void SetRatingToMovie(string titleId, double rating)
    {
        if (movies.ContainsKey(titleId))
        {
            movies[titleId].Rating = rating;
        }
    }

    public void AddLinkBetweenMovieIdAndTitleId(string movieId, string imdbId)
    {
        string titleId = "tt" + imdbId.PadLeft(7, '0');
        movieIdToTitleId[movieId] = titleId;
    }


    public Movie SearchMovieByTitle(string title)
    {
        return movies.Values.FirstOrDefault(m => m.Title == title);
    }

    public HashSet<Movie> SearchMoviesByPerson(string name)
    {
        return actorsDirectorsMovies.ContainsKey(name) ? actorsDirectorsMovies[name] : null;
    }

    public HashSet<Movie> SearchMoviesByTag(string tag)
    {
        return tagsMovies.ContainsKey(tag) ? tagsMovies[tag] : null;
    }
}

class Program
{
    static void Main(string[] args)
    {
        MovieDatabase movieDb = new MovieDatabase();

        var movieList = XSVParser
    <Movie>.Parse("resources/ml-latest/MovieCodes_IMDB.tsv", '\t', fields =>
        {
            string titleId = fields[0];
            string title = fields[2];
            string language = fields[3];

            if (language == "EN" || language == "RU")
            {
                return new Movie { TitleId = titleId, Title = title };
            }

            return null;
        }).Where(m => m != null).ToList();

        foreach (var movie in movieList)
        {
            movieDb.AddMovie(movie);
        }

        var actorsDirectorsList = XSVParser
    <string[]>.Parse("resources/ml-latest/ActorsDirectorsCodes_IMDB.tsv", '\t', fields => fields);
        foreach (var fields in actorsDirectorsList)
        {
            string titleId = fields[0];
            string personName = fields[2];
            string category = fields[3];

            movieDb.AddActorDirectorToMovie(personName, titleId, category == "director");
        }

        var linksList = XSVParser
    <string[]>.Parse("resources/ml-latest/links_IMDB_MovieLens.csv", ',', fields =>
        {
            string movieId = fields[0];
            string imdbId = fields[1];
            return new string[] { movieId, imdbId };
        });

        foreach (var link in linksList)
        {
            string movieId = link[0];
            string imdbId = link[1];
            movieDb.AddLinkBetweenMovieIdAndTitleId(movieId, imdbId);
        }


        var ratingsList = XSVParser
    <string[]>.Parse("resources/ml-latest/Ratings_IMDB.tsv", '\t', fields => fields);
        foreach (var fields in ratingsList)
        {
            string titleId = fields[0];
            double rating = double.Parse(fields[1]);

            movieDb.SetRatingToMovie(titleId, rating);
        }



        var tagList = XSVParser
    <string[]>.Parse("resources/ml-latest/TagCodes_MovieLens.csv", ',', tagFields => tagFields);
        Dictionary<int, string> tagsToNames = new Dictionary<int, string>();
        foreach (var fields in tagList)
        {
            int id = int.Parse(fields[0]);
            string name = fields[1];

            tagsToNames[id] = name;
        }

        using (var reader = new StreamReader("resources/ml-latest/TagScores_MovieLens.csv"))
        {

            reader.ReadLine();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var fields = line.Split(',');
                string movieId = fields[0];
                int tagId = int.Parse(fields[1]);
                double relevance = double.Parse(fields[2]);

                if (relevance > 0.5)
                {
                    if (tagsToNames.ContainsKey(tagId))
                    {
                        movieDb.AddTagToMovie(tagsToNames[tagId], movieId);
                    }
                }
            }
        }

        string movieTitle = "Это случилось однажды ночью";
        var movieSearch = movieDb.SearchMovieByTitle(movieTitle);
        if (movieSearch != null)
        {
            Console.WriteLine(movieSearch);
        }
        else
        {
            Console.WriteLine("Фильм не найден.");
        }
    }
}
