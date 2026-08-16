using System;

namespace PequenoExplorador.Infrastructure.Save
{
    internal sealed class SaveDataException : Exception
    {
        public SaveDataException(string errorCode)
            : base(errorCode)
        {
            ErrorCode = errorCode;
        }

        public SaveDataException(string errorCode, Exception innerException)
            : base(errorCode, innerException)
        {
            ErrorCode = errorCode;
        }

        public string ErrorCode { get; }
    }
}
