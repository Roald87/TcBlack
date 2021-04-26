using System;
using System.Runtime.Serialization;

namespace TcBlackCLI
{
    [Serializable]
    public class ProjectBuildFailedException : Exception
    {
        public ProjectBuildFailedException()
        {
        }

        public ProjectBuildFailedException(string message) : base(message)
        {
        }

        public ProjectBuildFailedException(
            string message, Exception innerException
        ) : base(message, innerException)
        {
        }

        protected ProjectBuildFailedException(
            SerializationInfo info, StreamingContext context
        ) : base(info, context)
        {
        }
    }
}
