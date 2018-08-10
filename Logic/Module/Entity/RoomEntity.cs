using GN;

namespace Logic
{
    public class RoomEntity : Entity
    {
        public int roomKey { get; set; }
        public void Awake(int roomKey)
        {
            this.roomKey = roomKey;
        }
    }
}
