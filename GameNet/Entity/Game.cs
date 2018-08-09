namespace GN
{
    public static class Game
    {
        private static Scene scene;
        public static Scene Scene
        {
            get
            {
                if (scene == null)
                {
                    scene = new Scene();
                }
                return scene;
            }
        }

       private static EventSystem eventSystem;
        public static EventSystem EventSystem
        {
            get
            {
                if (eventSystem == null)
                {
                    eventSystem = new EventSystem();
                }
                return eventSystem;
            }
        }
    }
}
