using Grpc.Core;
using PaymentService.Grcp;

namespace Payment.Api.Services;

// {csharp_namespace}.{service}.{service}Base
public class PaymentImplementation : PaymentService.Grcp.Payment.PaymentBase
{
    public override async Task<PaymentResponse> AuthorizePayment(PaymentRequest request, ServerCallContext context)
    {
        await Task.Delay(10_000);

        // mock: 80% zaakceptowane, 20% odrzucone
        if (Random.Shared.NextDouble() < 0.2)
        {
            return new PaymentResponse { Status = PaymentStatus.Declined, Reason = "Brak środków na koncie" };
        }

        return new PaymentResponse { Status = PaymentStatus.Accepted };
    }

    public override async Task AuthorizePaymentStream(PaymentRequest request, IServerStreamWriter<PaymentStageResponse> responseStream, ServerCallContext context)
    {
        // Stage 1
        await Task.Delay(3000);
        await responseStream.WriteAsync(new PaymentStageResponse {  Stage = PaymentStage.Initializing,
            Response = new PaymentResponse() });

        // Stage 2
        await Task.Delay(5000);
        await responseStream.WriteAsync(new PaymentStageResponse
        {
            Stage = PaymentStage.Authorizing,
            Response = new PaymentResponse()
        });

        // Stage 3
        await Task.Delay(2000);

        PaymentResponse paymentResponse;

        if (Random.Shared.NextDouble() < 0.2)
        {
            paymentResponse = new PaymentResponse { Status = PaymentStatus.Declined, Reason = "Brak środków na koncie" };
        }
        else 
        {
            paymentResponse = new PaymentResponse { Status = PaymentStatus.Accepted };
        }

        await responseStream.WriteAsync(new PaymentStageResponse { Stage = PaymentStage.Processing, 
            Response = paymentResponse });
    }

  

}
