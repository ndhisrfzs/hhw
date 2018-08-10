namespace GN
{
    public enum AppType
    {
        None = 0,
        Master = 1,
        Login = 1 << 1,
        Gate = 1 << 2,
        DB = 1 << 3,
        Location = 1 << 4,
        Game = 1 << 5,
        Match = 1 << 6,

        Client = 1 << 20,

        AllServer = Master | Login | Gate | DB | Location | Game | Match
    }

    public static class AppTypeHelper
    {
        public static bool Is(this AppType a, AppType b)
        {
            return (a & b) != 0;
        }
    }
}
