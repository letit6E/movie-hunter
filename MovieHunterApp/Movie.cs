using System.Collections.Generic;

class Movie
{
    public string TitleId { get; set; }
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
