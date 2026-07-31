using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.Domain.Interfaces;

namespace TicketManagement.Infrastructure.DataProtection
{
    public class CredentialEncryptor(IDataProtectionProvider provider) : ICredentialEncryptor
    {
        private readonly IDataProtector _protector = provider.CreateProtector("TicketManagement.Credentials");

        public string Encrypt(string plainText) => _protector.Protect(plainText);

        public string Decrypt(string cipherText) => _protector.Unprotect(cipherText);
    }
}
