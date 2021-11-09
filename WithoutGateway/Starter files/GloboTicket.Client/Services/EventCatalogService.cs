using GloboTicket.Web.Extensions;
using GloboTicket.Web.Models.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace GloboTicket.Web.Services
{
    public class EventCatalogService : IEventCatalogService
    {
        private readonly HttpClient client;
        private string _accessToken;
        private SemaphoreSlim _locker = new SemaphoreSlim(1,1);

        public EventCatalogService(HttpClient client)
        {
            this.client = client;
        }

        private async Task<string> GetToken()
        {
            await _locker.WaitAsync();
            try
            {
                if (!string.IsNullOrWhiteSpace(_accessToken))
                {
                    return _accessToken;
                }

                var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync("https://localhost:5010/");
                if (discoveryDocumentResponse.IsError)
                {
                    throw new Exception(discoveryDocumentResponse.Error);
                }

                var tokenResponse = await client.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest
                    {
                        Address = discoveryDocumentResponse.TokenEndpoint,
                        ClientId = "globoticket",
                        ClientSecret = "2493918d-bb14-4d89-b35f-d44bbe169620",
                        Scope = "eventcatalog.read"
                    });

                if (tokenResponse.IsError)
                {
                    throw new Exception(tokenResponse.Error);
                }

                _accessToken = tokenResponse.AccessToken;
                return _accessToken;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _locker.Release();
            }
        }
        
        public async Task<IEnumerable<Event>> GetAll()
        {
            client.SetBearerToken(await GetToken());
            var response = await client.GetAsync("/api/events");
            return await response.ReadContentAs<List<Event>>();
        }

      

        public async Task<IEnumerable<Event>> GetByCategoryId(Guid categoryid)
        {
            client.SetBearerToken(await GetToken());
            var response = await client.GetAsync($"/api/events/?categoryId={categoryid}");
            return await response.ReadContentAs<List<Event>>();
        }

        public async Task<Event> GetEvent(Guid id)
        {
            client.SetBearerToken(await GetToken());
            var response = await client.GetAsync($"/api/events/{id}");
            return await response.ReadContentAs<Event>();
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            client.SetBearerToken(await GetToken());
            var response = await client.GetAsync("/api/categories");
            return await response.ReadContentAs<List<Category>>();
        }

    }
}
