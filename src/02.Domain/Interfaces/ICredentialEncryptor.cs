namespace TicketManagement.Domain.Interfaces
{
    public interface ICredentialEncryptor
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
