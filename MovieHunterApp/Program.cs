using System;

class Program
{
    static void Main(string[] args)
    {
        var movieDb = new MovieDatabase();
        LoadData(movieDb);

        while (true)
        {
            Console.WriteLine("\n================ MENU ================\n");
            Console.WriteLine("Enter a mode:");
            Console.WriteLine("1. Search by Movie Title");
            Console.WriteLine("2. Search by Actor/Director");
            Console.WriteLine("3. Search by Tag");
            Console.WriteLine("Type 'exit' to quit.");
            Console.WriteLine("======================================\n");

            string? mode = Console.ReadLine()?.ToLower();

            if (mode == "exit" || mode == null)
            {
                break;
            }

            switch (mode)
            {
                case "1":
                    Console.Write("Enter the movie title: ");
                    string? title = Console.ReadLine();
                    if (title != null)
                    {
                        var movie = movieDb.SearchMovieByTitle(title);
                        Console.WriteLine("\n============== RESULT ==============");
                        Console.WriteLine(movie != null ? movie.ToString() : "Movie not found.");
                        Console.WriteLine("====================================\n");
                    }
                    break;

                case "2":
                    Console.Write("Enter the actor/director name: ");
                    string? name = Console.ReadLine();
                    if (name != null)
                    {
                        var movies = movieDb.SearchMoviesByPerson(name);
                        Console.WriteLine("\n============== RESULTS ==============");
                        if (movies != null && movies.Count > 0)
                        {
                            foreach (var movie in movies)
                            {
                                Console.WriteLine(movie);
                                Console.WriteLine("------------------------------------");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No movies found for this person.");
                        }
                        Console.WriteLine("====================================\n");
                    }
                    break;

                case "3":
                    Console.Write("Enter the tag: ");
                    string? tag = Console.ReadLine();
                    if (tag != null)
                    {
                        var movies = movieDb.SearchMoviesByTag(tag);
                        Console.WriteLine("\n============== RESULTS ==============");
                        if (movies != null && movies.Count > 0)
                        {
                            foreach (var movie in movies)
                            {
                                Console.WriteLine(movie);
                                Console.WriteLine("------------------------------------");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No movies found with this tag.");
                        }
                        Console.WriteLine("====================================\n");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid mode. Please try again.");
                    break;
            }
        }
    }

    static void LoadData(MovieDatabase movieDb)
    {
        SeparatedFileProcessor.Process("resources/MovieCodes_IMDB.tsv", '\t', fields =>
        {
            string titleId = fields[0];
            string title = fields[2];
            string region = fields[3];

            if (region == "EN" || region == "RU")
            {
                var movie = new Movie { TitleId = titleId, Title = title };
                movieDb.AddMovie(movie);
            }
        });

        SeparatedFileProcessor.Process("resources/ActorsDirectorsNames_IMDB.txt", '\t', fields =>
        {
            string personId = fields[0];
            string personName = fields[1];

            movieDb.AddActorDirector(personId, personName);
        });

        SeparatedFileProcessor.Process("resources/ActorsDirectorsCodes_IMDB.tsv", '\t', fields =>
        {
            string titleId = fields[0];
            string personId = fields[2];
            string category = fields[3];

            movieDb.AddActorDirectorToMovie(personId, titleId, category == "director");
        });

        SeparatedFileProcessor.Process("resources/Ratings_IMDB.tsv", '\t', fields =>
        {
            string titleId = fields[0];
            double rating = double.Parse(fields[1]);

            movieDb.SetRatingToMovie(titleId, rating);
        });

        SeparatedFileProcessor.Process("resources/TagCodes_MovieLens.csv", ',', fields =>
        {
            int id = int.Parse(fields[0]);
            string name = fields[1];

            movieDb.UpdateTagName(id, name);
        });

        SeparatedFileProcessor.Process("resources/TagScores_MovieLens.csv", ',', fields =>
        {
            string movieId = "tt" + fields[0].PadLeft(7, '0');
            int tagId = int.Parse(fields[1]);
            double relevance = double.Parse(fields[2]);

            if (relevance > 0.5)
            {
                var name = movieDb.GetTagName(tagId);
                if (name != null)
                {
                    movieDb.AddTagToMovie(name, movieId);
                }
            }
        });
    }
}
