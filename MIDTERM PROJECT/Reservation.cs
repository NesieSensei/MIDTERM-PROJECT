using System.Collections.Generic;
using System.Linq;

namespace MIDTERM_PROJECT
{
    public sealed class Reservation
    {
        public int Id { get; private set; }
        public User Customer { get; private set; }
        public Showing Showing { get; private set; }
        public List<int> Seats { get; private set; }
        public decimal Total { get { return Seats.Count * Showing.Price; } }

        public Reservation(int id, User customer, Showing showing, IEnumerable<int> seats)
        {
            Id = id;
            Customer = customer;
            Showing = showing;
            Seats = seats.OrderBy(seat => seat).ToList();
        }
    }
}
