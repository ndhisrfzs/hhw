namespace Logic
{
    public enum PlayerState
    {
        Main,
        Matching,
        Room
    }

    public enum Games : byte
    {
        Classical = 1,
        Metallic = 2
    }

    public enum GameType : byte
    {
        Friend = 1,
        Random = 2 
    }

    public enum ModelType : byte
    {
        BombScore = 1,
        BombDouble = 2,
        ChangePartner = 4
    }

    public enum GameState : byte
    {
        Ready = 1,          //准备阶段
        DealCard = 2,       //发牌
        Discard = 3,        //出牌
        GameOver = 4,       //游戏结束
    }
}
