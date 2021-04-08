// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Hosting.DynamicProviders;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Add extension methods for configuring OIDC dynamic providers.
    /// </summary>
    public static class IdentityServerBuilderOidcExtensions
    {
        /// <summary>
        /// Adds the OIDC dynamic provider feature.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddOidcDynamicProvider(this IIdentityServerBuilder builder)
        {
            builder.Services.Configure<IdentityServerOptions>(options =>
            {
                // this associates the OIDC auth handler (OpenIdConnectHandler) and options (OpenIdConnectOptions) classes
                // to the idp class (OidcProvider) and type value ("oidc") from the identity provider store
                options.DynamicProviders.AddProviderType<OpenIdConnectHandler, OpenIdConnectOptions, OidcProvider>("oidc");
            });

            // this registers the OidcConfigureOptions to build the OpenIdConnectOptions from the OidcProvider data
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OidcConfigureOptions>();

            // these are services from ASP.NET Core and are added manually since we're not using the 
            // AddOpenIdConnect helper that we'd normally use statically on the AddAuthentication.
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OpenIdConnectOptions>, OpenIdConnectPostConfigureOptions>());
            builder.Services.TryAddTransient<OpenIdConnectHandler>();

            return builder;
        }

        /// <summary>
        /// Adds the in memory OIDC provider store.
        /// This API is for testing only.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="providers"></param>
        /// <returns></returns>
        internal static IIdentityServerBuilder AddInMemoryOidcProviders(this IIdentityServerBuilder builder, IEnumerable<OidcProvider> providers)
        {
            builder.Services.AddSingleton(providers);
            builder.AddIdentityProviderStore<InMemoryOidcProviderStore>();
            builder.Services.AddTransientDecorator<IIdentityProviderStore, CachingIdentityProviderStore>();
            builder.Services.AddSingleton<IdentityProviderCache>();
            return builder;
        }
    }
}
