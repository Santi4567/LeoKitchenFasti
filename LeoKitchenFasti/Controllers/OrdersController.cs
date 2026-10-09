using LeoKitchenFasti.DTOs;
using LeoKitchenFasti.Models;
using LeoKitchenFasti.Services;
using LeoKitchenFasti.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeoKitchenFasti.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Exige JWT
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin,Mesero")]
        public async Task<ActionResult<ApiResponse<object>>> CreateOrder([FromBody] Order nuevaOrden)
        {
            // Extraer el ID del usuario del Token
            int meseroId = User.GetUserId();

            // Delegar el procesamiento al Servicio
            var ordenGenerada = await _orderService.CreateOrderAsync(nuevaOrden, meseroId);

            return Ok(ApiResponse<object>.Exito(ordenGenerada, "Orden enviada a cocina exitosamente"));
        }
    }
}