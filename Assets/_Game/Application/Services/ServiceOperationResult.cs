namespace PequenoExplorador.Application.Services
{
    public enum ServiceAvailability
    {
        Available,
        Disabled,
        Unavailable
    }

    public enum ServiceOperationStatus
    {
        Simulated,
        Disabled,
        Unavailable
    }

    public sealed class ServiceOperationResult
    {
        private ServiceOperationResult(ServiceOperationStatus status, string code)
        {
            Status = status;
            Code = code;
        }

        public ServiceOperationStatus Status { get; }
        public string Code { get; }

        public static ServiceOperationResult Simulated(string code)
        {
            return new ServiceOperationResult(ServiceOperationStatus.Simulated, code);
        }

        public static ServiceOperationResult Disabled(string code)
        {
            return new ServiceOperationResult(ServiceOperationStatus.Disabled, code);
        }

        public static ServiceOperationResult Unavailable(string code)
        {
            return new ServiceOperationResult(ServiceOperationStatus.Unavailable, code);
        }
    }
}
