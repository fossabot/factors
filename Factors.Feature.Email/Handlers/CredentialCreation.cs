﻿using Factors.Models.Interfaces;
using Factors.Models.UserAccount;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Factors.Feature.Email
{
    public partial class EmailInstance : IFactorFeature
    {
        /// <summary>
        /// Creates a new email credential in the database and sends out
        /// an email with a verification token which will be used to verify
        /// the email address is legitimate
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="credentialKey"></param>
        /// <returns></returns>
        public Task<FactorCredentialCreationResult> CreateCredentialAsync(IFactorInstance instance, string credentialKey)
        {
            return CreateEmailCredentialAsync(instance, credentialKey, true);
        }

        /// <summary>
        /// Creates a new email credential in the database and sends out
        /// an email with a verification token which will be used to verify
        /// the email address is legitimate
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="credentialKey"></param>
        /// <returns></returns>
        public FactorCredentialCreationResult CreateCredential(IFactorInstance instance, string credentialKey)
        {
            return CreateEmailCredentialAsync(instance, credentialKey, false).GetAwaiter().GetResult();
        }

        private async Task<FactorCredentialCreationResult> CreateEmailCredentialAsync(IFactorInstance instance, string credentialKey, bool runAsAsync)
        {
            //
            // Sets up our return model on the event of a successful
            // credential creation
            //
            var credentialResult = new FactorCredentialCreationResult
            {
                IsSuccess = true,
                VerificationMessageSent = true,
                Message = "Account registered as pending verification, validation token sent",
            };

            //
            // Attempts to parse the email address. Will throw an exception if
            // it fails to.
            //
            new MailAddress(credentialKey);

            //
            // Creates the new "pending verification" credential
            //
            var credentailDetails = new FactorCredential
            {
                UserAccountId = instance.UserAccount,
                FeatureTypeGuid = _featureType.FeatureGuid,
                CredentialKey = credentialKey
            };

            try
            {
                credentialResult.CredentailDetails = runAsAsync
                    ? await instance.Configuration.StorageDatabase.CreateCredentialAsync(credentailDetails).ConfigureAwait(false)
                    : instance.Configuration.StorageDatabase.CreateCredential(credentailDetails);
            }
            catch (Exception ex)
            {
                return new FactorCredentialCreationResult
                {
                    IsSuccess = false,
                    VerificationMessageSent = false,
                    Message = $"There was an issue when creating the credential: {ex.Message}"
                };
            }

            //
            // Generates a new token to be used to verify the credential
            //
            var newToken = instance.Configuration.TokenProvider.GenerateToken();

            //
            // Stores the new token in the database
            //
            var tokenDetails = new FactorGeneratedToken
            {
                UserAccountId = instance.UserAccount,
                VerificationToken = newToken,
                CredentialType = _featureType.FeatureGuid,
                ExpirationDateUtc = DateTime.UtcNow.Add(_configuration.TokenExpirationTime),
                CredentialKey = credentialKey
            };

            credentialResult.TokenDetails = runAsAsync
                ? await instance.Configuration.StorageDatabase.StoreTokenAsync(tokenDetails).ConfigureAwait(false)
                : instance.Configuration.StorageDatabase.StoreToken(tokenDetails);

            //
            // Send out the verification token to the user's email address
            //
            var messageSendResult = runAsAsync
                ? await SendTokenMessageAsync(credentialKey, newToken).ConfigureAwait(false)
                : SendTokenMessage(credentialKey, newToken);

            if (!messageSendResult.IsSuccess)
            {
                return new FactorCredentialCreationResult
                {
                    IsSuccess = false,
                    Message = messageSendResult.Message
                };
            }

            //
            // And finally return our result
            //
            return credentialResult;
        }
    }
}