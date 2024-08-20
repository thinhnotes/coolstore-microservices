using Duende.IdentityServer.Models;

namespace IdentityService
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
                new ApiScope("scope1"),
                new ApiScope("scope2"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("inventory")
                {
                    Scopes = { "scope1" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "scope1" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:44300/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "scope2" }
                },
                new Client
                {
                    ClientId = "coolstore.web",
                    ClientName = "React Web",
                    ClientUri = "http://cool-store.ml:3000",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris =
                    {
                        "http://localhost:3000/auth/callback",
                        "http://localhost:3000/auth/silent-renew",
                        "http://cool-store.ml:3000/auth/callback",
                        "http://cool-store.ml:3000/auth/silent-renew",
                        "http://cool-store.ml/auth/callback",
                        "http://cool-store.ml/auth/silent-renew"
                    },
                    PostLogoutRedirectUris =
                    {
                        "http://cool-store.ml",
                        "http://cool-store.ml:3000",
                        "http://localhost:3000"
                    },
                    AllowedCorsOrigins =
                    {
                        "http://cool-store.ml",
                        "http://cool-store.ml:3000",
                        "http://localhost:3000"
                    },
                    AllowedScopes = { "openid", "profile", "scope2" }
                },

                // code flow
                new Client
                {
                    ClientId = "store.code",
                    ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = {"https://www.getpostman.com/oauth2/callback", "https://app.getpostman.com/oauth2/callback"},
                    PostLogoutRedirectUris = {"https://app.getpostman.com", "https://www.getpostman.com"},

                    AllowedScopes = {"openid", "profile", "scope1", "scope2"}
                },
                new Client
                {
                    ClientId = "inventory_api_swagger",
                    ClientName = "Swagger UI for Inventory API",
                    ClientSecrets = {new Secret("secret".Sha256())}, // change me!

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = false,
                    RequireClientSecret = false,

                    RedirectUris = {"https://localhost:5002/swagger/oauth2-redirect.html"},
                    AllowedCorsOrigins = {"https://localhost:5002"},
                    AllowedScopes = { "scope1" }
                },
                // password flow
                new Client
                {
                    ClientId = "store.password",
                    ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = {"openid", "profile", "scope1", "scope2"}
                }
            };
    }
}
