using GN;
using System.Threading.Tasks;

namespace Logic
{
    [MessageHandler(AppType.Game)]
    public class GetRoomInfoHandler : AMRpcHandler<GamePlayer, GetRoomInfo.Request, GetRoomInfo.Response>
    {
        protected override async Task<GetRoomInfo.Response> Run(GamePlayer entity, GetRoomInfo.Request message)
        {
            GetRoomInfo.Response resp = new GetRoomInfo.Response();
            resp.roomInfo = await Game.Scene.GetComponent<RoomManagerComponent>().WaitForRoomInfoUpdate(entity.uid);

            return resp;
        }
    }
}
