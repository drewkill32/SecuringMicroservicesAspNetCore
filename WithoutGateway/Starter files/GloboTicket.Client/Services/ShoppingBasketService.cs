﻿using GloboTicket.Web.Extensions;
using GloboTicket.Web.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace GloboTicket.Web.Services
{
    public class ShoppingBasketService : IShoppingBasketService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingBasketService(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BasketLine> AddToBasket(Guid basketId, BasketLineForCreation basketLine)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            if (basketId == Guid.Empty)
            {
                _client.SetBearerToken(token);
                var basketResponse = await _client.PostAsJson("/api/baskets", new BasketForCreation
                {
                    UserId = Guid.Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x =>
                        x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value)
                });
                
                var basket = await basketResponse.ReadContentAs<Basket>();
                basketId = basket.BasketId;
            }
            _client.SetBearerToken(token);
            var response = await _client.PostAsJson($"api/baskets/{basketId}/basketlines", basketLine);
            return await response.ReadContentAs<BasketLine>();
        }

        public async Task<Basket> GetBasket(Guid basketId)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            _client.SetBearerToken(token);
            if (basketId == Guid.Empty)
                return null;
            var response = await _client.GetAsync($"/api/baskets/{basketId}");
            return await response.ReadContentAs<Basket>();
        }

        public async Task<IEnumerable<BasketLine>> GetLinesForBasket(Guid basketId)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            _client.SetBearerToken(token);
            if (basketId == Guid.Empty)
                return new BasketLine[0];
            var response = await _client.GetAsync($"/api/baskets/{basketId}/basketLines");
            return await response.ReadContentAs<BasketLine[]>();
        }

        public async Task UpdateLine(Guid basketId, BasketLineForUpdate basketLineForUpdate)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            _client.SetBearerToken(token);
            await _client.PutAsJson($"/api/baskets/{basketId}/basketLines/{basketLineForUpdate.LineId}",
                basketLineForUpdate);
        }

        public async Task RemoveLine(Guid basketId, Guid lineId)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            _client.SetBearerToken(token);
            await _client.DeleteAsync($"/api/baskets/{basketId}/basketLines/{lineId}");
        }

        public async Task<BasketForCheckout> Checkout(Guid basketId, BasketForCheckout basketForCheckout)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            _client.SetBearerToken(token);
            var response = await _client.PostAsJson($"api/baskets/checkout", basketForCheckout);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<BasketForCheckout>();
            else
            {
                throw new Exception("Something went wrong placing your order. Please try again.");
            }
        }
    }
}