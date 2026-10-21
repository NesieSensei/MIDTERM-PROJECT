namespace MIDTERM_PROJECT
{
    public sealed class Room
    {
        public int Number { get; private set; }
        public int SeatCount { get; private set; }

        public Room(int number, int seatCount)
        {
            Number = number;
            SeatCount = seatCount;
        }
    }
}
