using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PequenoExplorador.Application.Lifecycle;

namespace PequenoExplorador.Tests.EditMode.Fixtures
{
    internal sealed class RecordingApplicationService : IApplicationService
    {
        private readonly IList<string> _trace;

        public RecordingApplicationService(string serviceId, IList<string> trace)
        {
            ServiceId = serviceId;
            _trace = trace;
        }

        public string ServiceId { get; }

        public Func<CancellationToken, Task> InitializeBehavior { get; set; } =
            cancellationToken => Task.CompletedTask;

        public int InitializeCount { get; private set; }

        public int ShutdownCount { get; private set; }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            InitializeCount++;
            _trace.Add("initialize:" + ServiceId);
            await InitializeBehavior(cancellationToken);
        }

        public void Shutdown()
        {
            ShutdownCount++;
            _trace.Add("shutdown:" + ServiceId);
        }
    }
}
