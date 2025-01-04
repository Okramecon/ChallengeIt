using System.Security.Cryptography;
using ChallengeIt.Application.Security;

namespace ChallengeIt.Infrastructure.Security.Identity;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int Iterations = 100000;
    private const int HashSize = 32;
    
    private readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA512;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _algorithm, HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
            var split = hashedPassword.Split('-');
        var hash = Convert.FromHexString(split[0]);
        var salt = Convert.FromHexString(split[1]);
        
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(providedPassword, salt, Iterations, _algorithm, HashSize);
        
        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }
}