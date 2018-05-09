using System.Reflection;

namespace HHW.Service
{
    public class DynamicFieldInfo
    {
        public FieldInfo TheField;

        public string FieldName;

        public uint FieldNameHash;

        public ushort FieldDataLength;

#if Dynamic
        public GameLib.BetterSerialize.DynamicHelper.DynamicFieldGetHandler GetField;

        public GameLib.BetterSerialize.DynamicHelper.DynamicFieldSetHandler SetField;
#endif
    }
}
