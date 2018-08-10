using GN;
using System.Threading.Tasks;

namespace Logic
{
    [MessageHandler(AppType.Game)]
    public class M2G_InitRoomHandler : AMRpcHandler<Session, M2G_InitRoom.Request, M2G_InitRoom.Response>
    {
        protected override Task<M2G_InitRoom.Response> Run(Session entity, M2G_InitRoom.Request message)
        {
            M2G_InitRoom.Response resp = new M2G_InitRoom.Response();
            var roomManagerComponent = Game.Scene.GetComponent<RoomManagerComponent>();
            var key = roomManagerComponent.InitRoom(message.players);
            var room = ObjectFactory.Create<RoomEntity, int>(key);
            room.AddComponent<ActorComponent>();
            resp.actorId = room.id;

            return Task.FromResult(resp);
        }
    }
}
