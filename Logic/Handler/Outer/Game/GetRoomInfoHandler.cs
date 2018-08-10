using GN;
using System.Threading.Tasks;

namespace Logic
{
    [MessageHandler(AppType.Game)]
    public class GetRoomInfoHandler : AMRpcHandler<RoomEntity, GetRoomInfo.Request, GetRoomInfo.Response>
    {
        protected override Task<GetRoomInfo.Response> Run(RoomEntity entity, GetRoomInfo.Request message)
        {
            GetRoomInfo.Response resp = new GetRoomInfo.Response();

            resp.roomInfo = Game.Scene.GetComponent<RoomManagerComponent>().GetRoomInfoByKey(entity.roomKey);

            return Task.FromResult(resp);
        }
    }
}
