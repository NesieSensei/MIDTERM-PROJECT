using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MIDTERM_PROJECT
{
    public static class Program
    {
        private static readonly CinemaSystem Cinema = new CinemaSystem();

        private static void Main(string[] args)
        {
            Cinema.Seed();
            Console.WriteLine("========================================");
            Console.WriteLine("       GRAND SCREEN CINEMA POS");
            Console.WriteLine("========================================");
            Console.WriteLine("Demo manager login: manager@gmail.com / manager123");

            while (true)
            {
                Console.WriteLine("\n1. Browse movie showings\n2. Login\n3. Register customer\n0. Exit");
                switch (ReadRequired("Select: "))
                {
                    case "1": Cinema.PrintShowings(DateTime.Today); break;
                    case "2": Login(); break;
                    case "3": RegisterCustomer(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid selection."); break;
                }
            }
        }

        private static void Login()
        {
            User user = Cinema.Authenticate(ReadRequired("Username/email: "), ReadRequired("Password: "));
            if (user == null) { Console.WriteLine("Invalid username or password."); return; }
            Console.WriteLine("Logged in as " + user.Role + ".");
            if (user.Role == UserRole.Manager) ManagerMenu();
            else if (user.Role == UserRole.Staff) StaffMenu();
            else CustomerMenu(user);
        }

        private static void RegisterCustomer()
        {
            string email = ReadEmail();
            if (Cinema.Users.Any(user => user.Username.Equals(email, StringComparison.OrdinalIgnoreCase))) { Console.WriteLine("That username is already registered."); return; }
            Cinema.Users.Add(new User(email, ReadRequired("Password: "), UserRole.Customer));
            Console.WriteLine("Customer account created.");
        }

        private static void ManagerMenu()
        {
            while (true)
            {
                Console.WriteLine("\nMANAGER\n1. Register staff\n2. Room management\n3. Add movie showing\n4. Movie showing output\n5. Ticket statistics\n0. Logout");
                switch (ReadRequired("Select: "))
                {
                    case "1": RegisterStaff(); break;
                    case "2": RoomManagement(); break;
                    case "3": AddShowing(); break;
                    case "4": Cinema.PrintShowings(ReadDate()); break;
                    case "5": Cinema.PrintStatistics(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid selection."); break;
                }
            }
        }

        private static void StaffMenu()
        {
            while (true)
            {
                Console.WriteLine("\nSTAFF\n1. Ticket/reservation\n2. Movie showing output\n3. Room management output\n0. Logout");
                switch (ReadRequired("Select: "))
                {
                    case "1": MakeReservation(); break;
                    case "2": Cinema.PrintShowings(ReadDate()); break;
                    case "3": Cinema.PrintRooms(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid selection."); break;
                }
            }
        }

        private static void CustomerMenu(User customer)
        {
            while (true)
            {
                Console.WriteLine("\nCUSTOMER\n1. Browse movie showings\n2. Reserve seats\n0. Logout");
                switch (ReadRequired("Select: "))
                {
                    case "1": Cinema.PrintShowings(ReadDate()); break;
                    case "2": MakeReservation(customer); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid selection."); break;
                }
            }
        }

        private static void RegisterStaff()
        {
            string email = ReadEmail();
            if (Cinema.Users.Any(user => user.Username.Equals(email, StringComparison.OrdinalIgnoreCase))) { Console.WriteLine("That username is already registered."); return; }
            Cinema.Users.Add(new User(email, ReadRequired("Password: "), UserRole.Staff));
            Console.WriteLine("Staff account created.");
        }

        private static void RoomManagement()
        {
            Cinema.PrintRooms();
            Console.WriteLine("1. Increase rooms\n2. Decrease rooms\n0. Back");
            string choice = ReadRequired("Select: ");
            if (choice == "1") Cinema.AddRoom(ReadInt("New room number: ", 1, 999), ReadInt("Seats (1-60): ", 1, 60));
            else if (choice == "2") Console.WriteLine(Cinema.RemoveRoom(ReadInt("Room number to remove: ", 1, 999)) ? "Room removed." : "Room cannot be removed or does not exist.");
        }

        private static void AddShowing()
        {
            string name = ReadRequired("Movie name: ");
            int runtime = ReadInt("Runtime in minutes: ", 1, 600);
            string genre = ReadRequired("Genre: ");
            string description = ReadRequired("Description: ");
            string rating = ReadRequired("Rating: ");
            Movie movie = Cinema.Movies.FirstOrDefault(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (movie == null) { movie = new Movie(name, runtime, genre, description, rating); Cinema.Movies.Add(movie); }
            int roomNumber = ReadInt("Room number: ", 1, 999);
            DateTime start = ReadDateTime("Start time (yyyy-MM-dd HH:mm): ");
            decimal price = ReadDecimal("Price: ", 0.01m, 10000m);
            string error;
            Console.WriteLine(Cinema.TryAddShowing(movie, roomNumber, start, price, out error) ? "Showing added." : "Showing rejected: " + error);
        }

        private static void MakeReservation(User customer = null)
        {
            Cinema.PrintShowings(ReadDate());
            Showing showing = Cinema.Showings.FirstOrDefault(item => item.Id == ReadInt("Showing ID: ", 1, int.MaxValue));
            if (showing == null) { Console.WriteLine("Showing not found."); return; }
            List<int> seats = new List<int>();
            foreach (string part in ReadRequired("Seat numbers (comma separated): ").Split(','))
            {
                int seat;
                if (!int.TryParse(part.Trim(), out seat)) { Console.WriteLine("Invalid seat selection."); return; }
                seats.Add(seat);
            }
            string error;
            Reservation reservation;
            if (!Cinema.TryReserve(customer, showing, seats, out reservation, out error)) { Console.WriteLine("Reservation rejected: " + error); return; }
            PrintTicket(reservation);
        }

        private static void PrintTicket(Reservation reservation)
        {
            Showing showing = reservation.Showing;
            Console.WriteLine("\n========== SIMPLE TICKET ==========");
            Console.WriteLine("Cinema: Grand Screen Cinema\nMovie: " + showing.Movie.Name + " (" + showing.Movie.Rating + ")\nDate: " + showing.Start.ToString("yyyy-MM-dd HH:mm") + "\nRoom: " + showing.Room.Number + "\nSeats: " + string.Join(", ", reservation.Seats) + "\nPrice: " + reservation.Total.ToString("C"));
            Console.WriteLine("========== RECEIPT ================\nReservation #: " + reservation.Id + "\nThank you for your purchase.");
        }

        private static string ReadRequired(string prompt)
        {
            string value;
            do { Console.Write(prompt); value = Console.ReadLine(); } while (string.IsNullOrWhiteSpace(value));
            return value.Trim();
        }

        private static string ReadEmail()
        {
            while (true) { string email = ReadRequired("Username/email (@gmail.com): "); if (email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase)) return email; Console.WriteLine("Username must end with @gmail.com."); }
        }

        private static int ReadInt(string prompt, int min, int max)
        {
            int value;
            while (!int.TryParse(ReadRequired(prompt), out value) || value < min || value > max) Console.WriteLine("Enter a number from " + min + " to " + max + ".");
            return value;
        }

        private static decimal ReadDecimal(string prompt, decimal min, decimal max)
        {
            decimal value;
            while (!decimal.TryParse(ReadRequired(prompt), out value) || value < min || value > max) Console.WriteLine("Enter a valid amount.");
            return value;
        }

        private static DateTime ReadDate()
        {
            DateTime date;
            while (!DateTime.TryParseExact(ReadRequired("Date (yyyy-MM-dd): "), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) Console.WriteLine("Use yyyy-MM-dd.");
            return date.Date;
        }

        private static DateTime ReadDateTime(string prompt)
        {
            DateTime value;
            while (!DateTime.TryParseExact(ReadRequired(prompt), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out value)) Console.WriteLine("Use yyyy-MM-dd HH:mm.");
            return value;
        }
    }
}
