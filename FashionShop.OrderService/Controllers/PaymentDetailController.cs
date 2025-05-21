using FashionShop.OrderService.Model;
using FashionShop.OrderService.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FashionShop.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentDetailController : ControllerBase
    {
        private readonly IPaymentDetailService _paymentDetailService;

        public PaymentDetailController(IPaymentDetailService paymentDetailService)
        {
            _paymentDetailService = paymentDetailService;
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<PaymentDetail>> GetPaymentDetailByOrderId(Guid orderId)
        {
            var payment = await _paymentDetailService.GetPaymentDetailByOrderIdAsync(orderId);
            if (payment == null)
                return NotFound();
            return Ok(payment);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> ProcessPayment(PaymentDetail paymentDetail)
        {
            var result = await _paymentDetailService.ProcessPaymentAsync(paymentDetail);
            if (!result)
                return BadRequest();
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<bool>> UpdatePaymentStatus(Guid id, [FromBody] string status)
        {
            var result = await _paymentDetailService.UpdatePaymentStatusAsync(id, status);
            if (!result)
                return BadRequest();
            return Ok(result);
        }
    }
}
