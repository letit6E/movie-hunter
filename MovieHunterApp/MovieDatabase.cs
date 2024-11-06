using System.Collections.Generic;

class MovieDatabase
{
    private Dictionary<string, Movie> movies = new Dictionary<string, Movie>();
    private Dictionary<string, HashSet<Movie>> actorsDirectorsMovies = new Dictionary<string, HashSet<Movie>>();
    private Dictionary<string, HashSet<Movie>> tagsMovies = new Dictionary<string, HashSet<Movie>>();

    private Dictionary<string, string> movieTitles = new Dictionary<string, string>();
    private Dictionary<string, string> actorIds = new Dictionary<string, string>();
    private Dictionary<int, string> tagNames = new Dictionary<int, string>();

    public void UpdateTagName(int tag, string name)
    {
        tagNames[tag] = name;
    }

    public string? GetTagName(int tag)
    {
        return tagNames.ContainsKey(tag) ? tagNames[tag] : null;
    }

    public void AddMovie(Movie movie)
    {
        if (!movies.ContainsKey(movie.Title))
        {
            movies[movie.Title] = movie;
            movieTitles[movie.TitleId] = movie.Title;
        }
    }

    public void AddActorDirectorToMovie(string personId, string titleId, bool isDirector)
    {
        if (!movieTitles.ContainsKey(titleId) || !movies.ContainsKey(movieTitles[titleId]) || !actorIds.ContainsKey(personId))
            return;

        var movie = movies[movieTitles[titleId]];
        var personName = actorIds[personId];

        if (isDirector) movie.Director = personName;
        else movie.Actors.Add(personName);

        if (!actorsDirectorsMovies.ContainsKey(personName))
            actorsDirectorsMovies[personName] = new HashSet<Movie>();
        actorsDirectorsMovies[personName].Add(movie);
    }

    public void AddActorDirector(string personId, string personName)
    {
        actorIds[personId] = personName;
    }

    public void AddTagToMovie(string tag, string movieId)
    {
        if (!movieTitles.ContainsKey(movieId))
            return;

        string movieTitle = movieTitles[movieId];
        if (!movies.ContainsKey(movieTitle))
            return;

        var movie = movies[movieTitles[movieId]];
        movie.Tags.Add(tag);

        if (!tagsMovies.ContainsKey(tag))
            tagsMovies[tag] = new HashSet<Movie>();
        tagsMovies[tag].Add(movie);
    }

    public void SetRatingToMovie(string titleId, double rating)
    {
        if (movieTitles.ContainsKey(titleId) && movies.ContainsKey(movieTitles[titleId]))
        {
            movies[movieTitles[titleId]].Rating = rating;
        }
    }

    public Movie? SearchMovieByTitle(string title) => movies.ContainsKey(title) ? movies[title] : null;

    public HashSet<Movie>? SearchMoviesByPerson(string name) => actorsDirectorsMovies.ContainsKey(name) ? actorsDirectorsMovies[name] : null;

    public HashSet<Movie>? SearchMoviesByTag(string tag) => tagsMovies.ContainsKey(tag) ? tagsMovies[tag] : null;
}
