using System;
using System.Collections.Generic;
using System.Linq;

namespace MIDTERM_PROJECT
{
    public sealed class CinemaSystem
    {
        public const int CleaningMinutes = 20;
        public List<User> Users { get; private set; }
        public List<Movie> Movies { get; private set; }
        public List<Room> Rooms { get; private set; }
        public List<Showing> Showings { get; private set; }
        public List<Reservation> Reservations { get; private set; }

        private int nextShowingId;
        private int nextReservationId;

        public CinemaSystem()
        {
            Users = new List<User>();
            Movies = new List<Movie>();
            Rooms = new List<Room>();
            Showings = new List<Showing>();
            Reservations = new List<Reservation>();
            nextShowingId = 1;
            nextReservationId = 1;
        }

        public void Seed()
        {
            Users.Add(new User("manager@gmail.com", "manager123", UserRole.Manager));
            Users.Add(new User("staff@gmail.com", "staff123", UserRole.Staff));
            Rooms.Add(new Room(1, 60));
            Rooms.Add(new Room(2, 40));

            Movie movie = new Movie("The Grand Adventure", 120, "Adventure", "A journey beyond the ordinary.", "PG-13");
            Movies.Add(movie);

            string error;
            TryAddShowing(movie, 1, DateTime.Today.AddHours(10), 12m, out error);
            TryAddShowing(movie, 1, DateTime.Today.AddHours(13).AddMinutes(20), 12m, out error);
        }

        public User Authenticate(string username, string password)
        {
            return Users.FirstOrDefault(user => user.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && user.Password == password);
        }

        public bool TryAddShowing(Movie movie, int roomNumber, DateTime start, decimal price, out string error)
        {
            Room room = Rooms.FirstOrDefault(item => item.Number == roomNumber);
            if (room == null)
            {
                error = "Room does not exist.";
                return false;
            }

            Showing candidate = new Showing(0, movie, room, start, price);
            bool conflicts = Showings.Any(showing => showing.Room.Number == roomNumber
                && candidate.Start < showing.End.AddMinutes(CleaningMinutes)
                && showing.Start < candidate.End.AddMinutes(CleaningMinutes));

            if (conflicts)
            {
                error = "The room requires a 20-minute cleaning gap between showings.";
                return false;
            }

            Showings.Add(new Showing(nextShowingId++, movie, room, start, price));
            error = null;
            return true;
        }

        public bool TryReserve(User customer, Showing showing, IEnumerable<int> seats, out Reservation reservation, out string error)
        {
            List<int> selected = seats.ToList();
            if (selected.Count == 0)
            {
                reservation = null;
                error = "Select at least one seat.";
                return false;
            }

            if (selected.Count != selected.Distinct().Count())
            {
                reservation = null;
                error = "A seat cannot be selected more than once.";
                return false;
            }

            if (selected.Any(seat => seat < 1 || seat > showing.Room.SeatCount))
            {
                reservation = null;
                error = "Seat selection is outside this room's range.";
                return false;
            }

            if (selected.Any(seat => showing.ReservedSeats.Contains(seat)))
            {
                reservation = null;
                error = "One or more seats are already reserved.";
                return false;
            }

            showing.ReservedSeats.AddRange(selected);
            reservation = new Reservation(nextReservationId++, customer, showing, selected);
            Reservations.Add(reservation);
            error = null;
            return true;
        }

        public void PrintShowings(DateTime date)
        {
            Console.WriteLine("\nMOVIE SHOWINGS FOR " + date.ToString("yyyy-MM-dd"));
            IEnumerable<IGrouping<Movie, Showing>> groups = Showings
                .Where(showing => showing.Start.Date == date.Date)
                .GroupBy(showing => showing.Movie);

            foreach (IGrouping<Movie, Showing> group in groups)
            {
                List<Showing> dayShowings = group.OrderBy(showing => showing.Start).ToList();
                Console.WriteLine(group.Key.Name + " | Runtime: " + group.Key.Runtime + " min | Genre: " + group.Key.Genre + " | Rating: " + group.Key.Rating);
                Console.WriteLine("Description: " + group.Key.Description);
                Console.WriteLine("First: " + dayShowings.First().Start.ToString("HH:mm") + " | Last: " + dayShowings.Last().Start.ToString("HH:mm"));
                foreach (Showing showing in dayShowings)
                {
                    Console.WriteLine("  #" + showing.Id + " " + showing.Start.ToString("HH:mm") + " - Room " + showing.Room.Number + " - " + showing.Price.ToString("C") + " (" + showing.ReservedSeats.Count + "/" + showing.Room.SeatCount + " seats)");
                }
            }

            if (!Showings.Any(showing => showing.Start.Date == date.Date))
            {
                Console.WriteLine("No showings scheduled.");
            }
        }

        public void PrintRooms()
        {
            Console.WriteLine("\nROOMS");
            foreach (Room room in Rooms.OrderBy(item => item.Number))
            {
                Console.WriteLine("Room " + room.Number + " - " + room.SeatCount + " seats");
            }
        }

        public void AddRoom(int number, int seats)
        {
            if (Rooms.Any(room => room.Number == number))
            {
                Console.WriteLine("Room number already exists.");
                return;
            }

            Rooms.Add(new Room(number, seats));
            Console.WriteLine("Room added.");
        }

        public bool RemoveRoom(int number)
        {
            if (Showings.Any(showing => showing.Room.Number == number) || Reservations.Any(reservation => reservation.Showing.Room.Number == number)) return false;
            Room room = Rooms.FirstOrDefault(item => item.Number == number);
            if (room == null) return false;
            Rooms.Remove(room);
            return true;
        }

        public void PrintStatistics()
        {
            Console.WriteLine("\nTICKET STATISTICS");
            Console.WriteLine("Reservations: " + Reservations.Count);
            Console.WriteLine("Tickets sold: " + Reservations.Sum(reservation => reservation.Seats.Count));
            Console.WriteLine("Revenue: " + Reservations.Sum(reservation => reservation.Total).ToString("C"));
        }
    }
}
