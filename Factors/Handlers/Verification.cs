﻿using Factors.Models.Interfaces;
using Factors.Models.UserAccount;
using System.Threading.Tasks;

namespace Factors
{
    public partial class FactorsApplication
    {
        /// <summary>
        /// Verifies that the token passed for the specified user is correct,
        /// and also updates the associated credential in the database as verified
        /// (assuming the token is correct)
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        public FactorsCredentialCreationVerificationResult VerifyCredentialRegistration<tt>(string tokenValue) where tt : IFactorsFeatureType, new()
        {
            var featureType = new tt();
            return Configuration.StorageDatabase.VerifyToken(UserAccount, featureType, tokenValue);
        }

        /// <summary>
        /// Verifies that the token passed for the specified user is correct,
        /// and also updates the associated credential in the database as verified
        /// (assuming the token is correct)
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        public Task<FactorsCredentialCreationVerificationResult> VerifyCredentialRegistrationAsync<tt>(string tokenValue) where tt : IFactorsFeatureType, new()
        {
            var featureType = new tt();
            return Configuration.StorageDatabase.VerifyTokenAsync(UserAccount, featureType, tokenValue);
        }
    }
}