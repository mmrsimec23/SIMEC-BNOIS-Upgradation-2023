﻿using System;
using System.Collections.Generic;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;

namespace Infinity.Bnois.Api.Web.Config
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Clients'
    public class Clients
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Clients'
    {
        private static int identityTokenLifetime = 3600;
        private static int accessTokenLifetime = 3600;
        private static bool requireConsent = false;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Clients.Get()'
        public static List<Client> Get()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Clients.Get()'
        {
            //identityTokenLifetime = Convert.ToInt32(ConfigurationManager.AppSettings["IdentityTokenLifetime"]);
            //accessTokenLifetime = Convert.ToInt32(ConfigurationManager.AppSettings["AccessTokenLifetime"]);
            //requireConsent = Convert.ToBoolean(ConfigurationManager.AppSettings["RequireConsent"]);

            return new List<Client>
            {
                new Client
                {
                    ClientId = "infinity-bnois-api-client",
                    ClientName = "Bangladesh Naval Officers Information System",
                    Flow = Flows.Implicit,
                    IdentityTokenLifetime =identityTokenLifetime,
                    AccessTokenLifetime = accessTokenLifetime,
                    RequireConsent = requireConsent,
                    AccessTokenType = AccessTokenType.Jwt,

                    AbsoluteRefreshTokenLifetime = accessTokenLifetime,
                    SlidingRefreshTokenLifetime = accessTokenLifetime,

                      RefreshTokenUsage = TokenUsage.OneTimeOnly,
                        RefreshTokenExpiration = TokenExpiration.Sliding,
                    //RefreshTokenExpiration = TokenExpiration.Absolute,
                   // RefreshTokenUsage = TokenUsage.OneTimeOnly,
                  //  AbsoluteRefreshTokenLifetime = TimeSpan.FromDays(2).Seconds,
                    
                         // redirect = URI of the Angular application
                          //"http://192.168.1.200:8091/callback.html",
                          //"http://192.168.1.200:8091/callback.html",

                        //Local Pc
                    RedirectUris = new List<string>
                    {

                        "http://192.168.1.200:8091/callback.html",
                        // for silent refresh
                        "http://192.168.1.200:8091/silent-renew.html",
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "http://192.168.1.200:8091/index.html",
                    },


                    //    //Server PC
                    //    RedirectUris = new List<string>
                    //{

                    //    "http://192.168.1.200:8091/callback.html",
                    //    // for silent refresh
                    //    "http://192.168.1.200:8091/silent-renew.html",
                    //},
                    //PostLogoutRedirectUris = new List<string>()
                    //{
                    //    "http://192.168.1.200:8091/index.html",
                    //},





                    AllowedScopes = new List<string> {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.Address,
                        Constants.StandardScopes.Email,
                        Constants.StandardScopes.Roles,
                        Constants.StandardScopes.OfflineAccess,
                        Constants.StandardScopes.AllClaims,
                        "infinity-bnois-api-scope",
                        "infinity-bnois-identity-api-scope"
                    },
                    AllowAccessToAllScopes = true

                }
            };
        }
    }
}