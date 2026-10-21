namespace MIDTERM_PROJECT
{
    public sealed class Movie
    {
        public string Name { get; private set; }
        public int Runtime { get; private set; }
        public string Genre { get; private set; }
        public string Description { get; private set; }
        public string Rating { get; private set; }

        public Movie(string name, int runtime, string genre, string description, string rating)
        {
            Name = name;
            Runtime = runtime;
            Genre = genre;
            Description = description;
            Rating = rating;
        }
    }
}
