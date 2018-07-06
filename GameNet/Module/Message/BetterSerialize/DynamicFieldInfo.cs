using System.Reflection;

namespace GN
{
    public class DynamicFieldInfo
    {
        public PropertyInfo TheField;

        public string FieldName;

        public uint FieldNameHash;

        public ushort FieldDataLength;

#if Dynamic
        public GameLib.BetterSerialize.DynamicHelper.DynamicFieldGetHandler GetField;

        public GameLib.BetterSerialize.DynamicHelper.DynamicFieldSetHandler SetField;
#endif
    }
}
