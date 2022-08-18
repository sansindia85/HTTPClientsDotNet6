using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client
{
    internal class TimeOutDelegatingHandler : DelegatingHandler
    {
        //Default timeout of HTTP Client is 100 secs
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(100);

        public TimeOutDelegatingHandler(TimeSpan timeout)
        : base()
        {
            _timeout = timeout;
        }

        public TimeOutDelegatingHandler(HttpMessageHandler innerHandler,
            TimeSpan timeout)
            : base()
        {
            _timeout = timeout;
        }

        protected override async Task<HttpResponseMessage> 
            SendAsync(HttpRequestMessage request, 
                CancellationToken cancellationToken)
        {
            using (var linkedCancellationTokenSource =
                   CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                linkedCancellationTokenSource.CancelAfter(_timeout);

                try
                {
                    return await base.SendAsync(request, linkedCancellationTokenSource.Token);

                }
                catch (OperationCanceledException ex )
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        throw new TimeoutException("The request timed out.", ex);
                    }

                    throw;
                }
            }
        }
    }
}
