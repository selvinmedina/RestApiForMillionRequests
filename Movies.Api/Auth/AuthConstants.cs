﻿namespace Movies.Api.Auth
{
    public static class AuthConstants
    {
        public const string AdminPolicyName = "Admin";
        public const string AdminUserClaimName = "admin";

        public const string TruestedMemberName = "Trusted";
        public const string TruestedMemberClaimName = "trusted_member";

        public const string ApiKeyHeaderName = "X-Api-Key";

    }
}
