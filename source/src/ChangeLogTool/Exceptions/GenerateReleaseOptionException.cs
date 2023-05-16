namespace Buhler.IoT.Environment.ChangeLogTool.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [Serializable]
    public class GenerateReleaseOptionException : Exception
    {
        public GenerateReleaseOptionException(string message) : base(message)
        {
            // Nothing to do.
        }

        [ExcludeFromCodeCoverage]
        protected GenerateReleaseOptionException(SerializationInfo info, StreamingContext context)
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
