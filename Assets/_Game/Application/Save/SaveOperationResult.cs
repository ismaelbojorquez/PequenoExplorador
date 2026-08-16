namespace PequenoExplorador.Application.Save
{
    public sealed class SaveOperationResult
    {
        public SaveOperationResult(SaveOperationStatus status, string errorCode)
        {
            Status = status;
            ErrorCode = errorCode ?? string.Empty;
        }

        public SaveOperationStatus Status { get; }
        public string ErrorCode { get; }
        public bool IsSuccess => Status == SaveOperationStatus.Saved;

        public static SaveOperationResult Saved()
        {
            return new SaveOperationResult(SaveOperationStatus.Saved, string.Empty);
        }
    }
}
