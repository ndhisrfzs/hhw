namespace HHW.Service
{
    public enum AppType
    {
        None = 0,
        Manager = 1,
        Login = 1 << 1,
        Gate = 1 << 2,
        DB = 1 << 3,
        Location = 1 << 4,
        Game = 1 << 5,

        AllServer = Manager | Login | Gate | DB | Location | Game
    }

    public static class AppTypeHelper
    {
        public static bool Is(this AppType a, AppType b)
        {
            return (a & b) != 0;
        }
    }
}
