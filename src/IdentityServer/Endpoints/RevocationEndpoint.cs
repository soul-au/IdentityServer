﻿using Microsoft.AspNetCore.Http;
using System.Net;

namespace IdentityServer.Endpoints
{
    internal class RevocationEndpoint : EndpointBase
    {
        private readonly ITokenStore _tokens;
        private readonly IdentityServerOptions _options;
        private readonly ILoggerFormater _loggerFormater;
        private readonly IClientSecretValidator _clientSecretValidator;

        public RevocationEndpoint(
            ITokenStore tokens,
            IdentityServerOptions options,
            ILoggerFormater loggerFormater,
            IClientSecretValidator clientSecretValidator)
        {
            _tokens = tokens;
            _options = options;
            _loggerFormater = loggerFormater;
            _clientSecretValidator = clientSecretValidator;
        }

        public override async Task<IEndpointResult> HandleAsync(HttpContext context)
        {
            #region Logger Request
            await _loggerFormater.LogRequestAsync();
            #endregion
            
            #region Validate Method
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                return MethodNotAllowed();
            }
            if (!context.Request.HasFormContentType)
            {
                return BadRequest(ValidationErrors.InvalidRequest, "Invalid contextType");
            }
            #endregion

            #region Validate Client
            await _clientSecretValidator.ValidateAsync(context);
            #endregion

            #region Read Form
            var body = await context.Request.ReadFormAsync();
            var form = body.AsNameValueCollection();
            #endregion

            #region Parse Token
            var token = form.Get("token");
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(ValidationErrors.InvalidRequest, "token is missing");
            }
            if (token.Length > _options.InputLengthRestrictions.Jwt)
            {
                return BadRequest(ValidationErrors.InvalidRequest, "Grant type is too long");
            }
            #endregion

            #region RevomeToken
            var accessToken = await _tokens.FindAccessTokenAsync(token);
            if (accessToken != null)
            {
                await _tokens.RevomeTokenAsync(accessToken);
            }
            #endregion

            return StatusCode(HttpStatusCode.OK);
        }
    }
}
