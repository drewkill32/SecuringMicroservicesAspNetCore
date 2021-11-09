// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace GloboTicket.Services.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("eventcatalog.fullaccess"),
                new ApiScope("shoppingbasket.fullaccess"),
                new ApiScope("eventcatalog.read"),
                new ApiScope("eventcatalog.write"),
                new ApiScope("discount.fullaccess"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("eventcatalog", "Event catalog API")
                {
                    Scopes = { "eventcatalog.read", "eventcatalog.write" }
                },
                new ApiResource("shoppingbasket", "Shopping basket API")
                {
                    Scopes = { "shoppingbasket.fullaccess" }
                },
                new ApiResource("discount", "Discount API")
                {
                    Scopes = { "discount.fullaccess" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientName = "GloboTicket Machine 2 Machine Client",
                    ClientId = "globoticketm2m",
                    ClientSecrets = { new Secret("adde1735-8116-4736-87fe-a0e5a2917d4e".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "eventcatalog.fullaccess" }
                },
                new Client
                {
                    ClientName = "GloboTicket Interactive Client",
                    ClientId = "globoticketinteractive",
                    ClientSecrets = { new Secret("2493918d-bb14-4d89-b35f-d44bbe169620".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:5000/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5000/signout-callback-oidc" },
                    AllowedScopes = { "openid", "profile", "shoppingbasket.fullaccess" }
                },
                new Client
                {
                    ClientName = "GloboTicket Client",
                    ClientId = "globoticket",
                    ClientSecrets = { new Secret("2493918d-bb14-4d89-b35f-d44bbe169620".Sha256()) },
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RedirectUris = { "https://localhost:5000/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5000/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        "openid", "profile",
                        "shoppingbasket.fullaccess",
                        "eventcatalog.read",
                        "eventcatalog.write"
                    }
                },
                new Client
                {
                    ClientName = "Shopping Basket Token Exchange Client",
                    ClientId = "shoppingbaskettodownstreamtokenexchangeclient",
                    AllowedGrantTypes = new[] { "urn:ietf:params:oauth:grant-type:token-exchange"},
                    RedirectUris = { "https://localhost:5000/signin-oidc" },
                    ClientSecrets = { new Secret("93b20402-0270-4a86-9c5f-cec8eb860df9".Sha256()) },
                    PostLogoutRedirectUris = { "https://localhost:5000/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        "openid", "profile",
                        "discount.fullaccess"
                    }
                }
            };
    }
}