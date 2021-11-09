using GloboTicket.Services.ShoppingBasket.Extensions;
using GloboTicket.Services.ShoppingBasket.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace GloboTicket.Services.ShoppingBasket.Services
{
    public class DiscountService: IDiscountService
    {
        private readonly HttpClient client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _accessToken;
        private SemaphoreSlim _locker = new SemaphoreSlim(1, 1);
        public DiscountService(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetToken()
        {
            
            try
            {
                await _locker.WaitAsync();
                if (!string.IsNullOrEmpty(_accessToken))
                {
                    return _accessToken;
                }

                var discoveryDocumentResponse = await client
                    .GetDiscoveryDocumentAsync("https://localhost:5010");
                if (discoveryDocumentResponse.IsError)
                {
                    throw new Exception(discoveryDocumentResponse.Error);
                }

                var customParams = new Dictionary<string, string>
                {
                    { "subject_token_type", "urn:ietf:params:oauth:grant-type:access_token" },
                    { "subject_token", await _httpContextAccessor.HttpContext.GetTokenAsync("access_token") },
                    { "scope", "openid profile discount.fullaccess" }
                };

                var tokenResponse = await client.RequestTokenAsync(new TokenRequest()
                {
                    Address = discoveryDocumentResponse.TokenEndpoint,
                    GrantType = "urn:ietf:params:oauth:grant-type:token-exchange",
                    Parameters = customParams,
                    ClientId = "shoppingbaskettodownstreamtokenexchangeclient",
                    ClientSecret = "93b20402-0270-4a86-9c5f-cec8eb860df9"
                });

                if (tokenResponse.IsError)
                {
                    throw new Exception(tokenResponse.Error);
                }

                _accessToken = tokenResponse.AccessToken;
                return _accessToken;

            }
            finally
            {
                _locker.Release();
            }
        }
       
        public async Task<Coupon> GetCoupon(Guid userId)
        {
            client.SetBearerToken(await GetToken());
            var response = await client.GetAsync($"/api/discount/user/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.ReadContentAs<Coupon>();
        }
    }
}
