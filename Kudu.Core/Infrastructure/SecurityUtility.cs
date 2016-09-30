﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.Web.DataProtection;
using System;
using System.Security.Cryptography;

namespace Kudu.Core.Infrastructure
{
    public static class SecurityUtility
    {
        private static string GenerateSecretString()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[40];
                rng.GetBytes(data);
                string secret = Convert.ToBase64String(data);
                // Replace pluses as they are problematic as URL values
                return secret.Replace('+', 'a');
            }
        }

        public static Tuple<string, string>[] GenerateSecretStringsKeyPair(int number)
        {
            var unencryptedToEncryptedKeyPair = new Tuple<string, string>[number];
            var protector = DataProtectionProvider.CreateAzureDataProtector().CreateProtector(_defaultProtectorStr);
            for (int i = 0; i < number; i++)
            {
                string unencryptedKey = GenerateSecretString();
                unencryptedToEncryptedKeyPair[i] = new Tuple<string, string>(unencryptedKey, protector.Protect(unencryptedKey));
            }
            return unencryptedToEncryptedKeyPair;
        }

        private const string _defaultProtectorStr = "function-secrets";

        public static string DecryptSecretString(string content)
        {
            return DataProtectionProvider.CreateAzureDataProtector().CreateProtector(_defaultProtectorStr).Unprotect(content);
        }

    }
}

