﻿using IdentityServer.EntityFramework.Entities;
using System.Security.Claims;

namespace IdentityServer.Models
{
    public class AuthorizationCodeEntity : Entity
    {
        public string Code { get; set; } = default!;
        public int Lifetime { get; set; }
        public string State { get; set; } = default!;
        public string Scope { get; set; } = default!;
        public string None { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string RedirectUri { get; set; } = default!;
        public string ResponseType { get; set; } = default!;
        public string? ResponseMode { get; set; }
        public string? CodeChallenge { get; }
        public string? CodeChallengeMethod { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime CreationTime { get; set; }
        public IEnumerable<ClaimEntity> Claims { get; set; } = new List<ClaimEntity>();

        protected AuthorizationCodeEntity()
        {

        }

        public AuthorizationCodeEntity(
             string code,
             string none,
             string state,
             string scope,
             ICollection<ClaimEntity> claims,
             string clientId,
             string redirectUri,
             string responseType,
             string? responseMode,
             int lifetime,
             DateTime expirationTime,
             DateTime creationTime,
             string? codeChallenge,
             string? codeChallengeMethod)
        {
            Code = code;
            None = none;
            State = state;
            Scope = scope;
            Claims = claims;
            ClientId = clientId;
            RedirectUri = redirectUri;
            ResponseType = responseType;
            ResponseMode = responseMode;
            Lifetime = lifetime;
            CreationTime = creationTime;
            ExpirationTime = expirationTime;
            CodeChallenge = codeChallenge;
            CodeChallengeMethod = codeChallengeMethod;
        }

        public AuthorizationCode Cast()
        {
            var claims = Claims
                .Select(s => new Claim(s.Type, s.Value, s.ValueType, s.Issuer))
                .ToArray();
            return new AuthorizationCode(
                code: Code,
                none: None,
                state: State,
                scope: Scope,
                claims: claims,
                lifetime: Lifetime,
                expirationTime: ExpirationTime,
                creationTime: CreationTime,
                clientId: ClientId,
                redirectUri: RedirectUri,
                responseType: ResponseType,
                responseMode: ResponseMode,
                codeChallenge: CodeChallenge,
                codeChallengeMethod: CodeChallengeMethod);
        }

        public static implicit operator AuthorizationCodeEntity(AuthorizationCode entity)
        {
            var claims = entity.Claims
                .Select(s => new ClaimEntity(s.Type, s.Value, s.ValueType, s.Issuer))
                .ToArray();
            return new AuthorizationCodeEntity(
               code: entity.Code,
               none: entity.None,
               state: entity.State,
               scope: entity.Scope,
               claims: claims,
               lifetime: entity.Lifetime,
               expirationTime: entity.ExpirationTime,
               creationTime: entity.CreationTime,
               clientId: entity.ClientId,
               redirectUri: entity.RedirectUri,
               responseType: entity.ResponseType,
               responseMode: entity.ResponseMode,
               codeChallenge: entity.CodeChallenge,
               codeChallengeMethod: entity.CodeChallengeMethod);
        }
    }
}
