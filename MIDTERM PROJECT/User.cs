namespace MIDTERM_PROJECT
{
    public sealed class User
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public UserRole Role { get; private set; }

        public User(string username, string password, UserRole role)
        {
            Username = username;
            Password = password;
            Role = role;
        }
    }
}
