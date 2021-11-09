using System;
using System.Linq;
using System.Security.Claims;
using GloboTicket.Web.Models;
using GloboTicket.Web.Models.View;
using GloboTicket.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GloboTicket.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly Settings settings;

        public OrderController(Settings settings, IOrderService orderService)
        {
            this.settings = settings;
            this.orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await orderService.GetOrdersForUser(
                Guid.Parse(User.Claims.FirstOrDefault(x =>
                    x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value));

            return View(new OrderViewModel { Orders = orders });
        }
    }
}