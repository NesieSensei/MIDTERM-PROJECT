using System;
using System.Collections.Generic;

namespace MIDTERM_PROJECT
{
    public sealed class Showing
    {
        public int Id { get; private set; }
        public Movie Movie { get; private set; }
        public Room Room { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime End { get { return Start.AddMinutes(Movie.Runtime); } }
        public decimal Price { get; private set; }
        public List<int> ReservedSeats { get; private set; }

        public Showing(int id, Movie movie, Room room, DateTime start, decimal price)
        {
            Id = id;
            Movie = movie;
            Room = room;
            Start = start;
            Price = price;
            ReservedSeats = new List<int>();
        }
    }
}
