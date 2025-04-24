namespace TransactionCore.Common.Helpers
{
    public static class AuthorizationPolicies
    {
        public static void AddCustomPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("MicroserviceOnly", policy =>
                {
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("client_id", "TransactionCore") ||
                        context.User.IsInRole("Admin")
                    );
                });
            });
        }
    }
}
