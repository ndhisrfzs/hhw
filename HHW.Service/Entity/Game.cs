namespace HHW.Service
{
    public static class Game
    {
        private static Scene scene;
        public static Scene Scene
        {
            get
            {
                if(scene == null)
                {
                    scene = new Scene();
                }
                return scene;
            }
        }
    }
}
