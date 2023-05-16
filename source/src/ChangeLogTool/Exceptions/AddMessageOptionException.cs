namespace Buhler.IoT.Environment.ChangeLogTool.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [Serializable]
    public class AddMessageOptionException : Exception
    {
        public AddMessageOptionException(string message) : base(message)
        {
            // Nothing to do.
        }

        [ExcludeFromCodeCoverage]
        protected AddMessageOptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [ExcludeFromCodeCoverage]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
