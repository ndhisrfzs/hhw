using System;
namespace GN
{
    public class SerializeException : Exception
    {
        public static SerializeException OverSizeException = new SerializeException("Size too big");

        public static SerializeException SerializeFailException = new SerializeException("Data bytes and type not match");

        public SerializeException()
            : base()
        { }

        public SerializeException(string message)
            :base(message)
        { }
    }
}
