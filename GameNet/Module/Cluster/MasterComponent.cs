#if Server
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GN
{
    public class MasterComponent : Component
    {
        private static int randIndex = 0;
        private readonly ConcurrentDictionary<int, AppInfo> id2Info = new ConcurrentDictionary<int, AppInfo>();
        private readonly ConcurrentDictionary<AppType, List<AppInfo>> type2Infos = new ConcurrentDictionary<AppType, List<AppInfo>>();
        public bool Add(AppInfo app)
        {
            if(id2Info.TryAdd(app.appId, app))
            {
                List<AppInfo> infos = type2Infos.GetOrAdd(app.appType, new List<AppInfo>());
                infos.Add(app);
                return true;
            }

            return false;
        }

        public AppInfo Get(int appId)
        {
            AppInfo info;
            id2Info.TryGetValue(appId, out info);
            return info;
        }

        public AppInfo Get(AppType type)
        {
            List<AppInfo> infos = new List<AppInfo>();
            foreach (var item in type2Infos)
            {
                if(item.Key.Is(type))
                {
                    infos.AddRange(item.Value);
                }
            }
            if(infos.Count > 0)
            {
                return infos[randIndex++ % infos.Count];
            }
            return null;
        }

        public void Remove(int appId)
        {
            AppInfo info;
            if(id2Info.TryRemove(appId, out info))
            {
                List<AppInfo> infos;
                if(type2Infos.TryGetValue(info.appType, out infos))
                {
                    infos.RemoveAll(c => c.appId == appId);
                }
            }
        }

        public override void Dispose()
        {
            if(IsDisposed)
            {
                return;
            }

            base.Dispose();

            id2Info.Clear();
            type2Infos.Clear();
        }
    }
}
#endif