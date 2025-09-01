using Grpc.Core;
using PaymentService.Grcp;

namespace Payment.Api.Services;

// {csharp_namespace}.{service}.{service}Base
public class PaymentImplementation : PaymentService.Grcp.Payment.PaymentBase
{
    public override async Task<PaymentResponse> AuthorizePayment(PaymentRequest request, ServerCallContext context)
    {
        // mock: 80% zaakceptowane, 20% odrzucone
        if (Random.Shared.NextDouble() < 0.2)
        {
            return new PaymentResponse { Status = PaymentStatus.Declined, Reason = "Brak środków na koncie" };
        }

        return new PaymentResponse { Status = PaymentStatus.Accepted };
    }
  
}
