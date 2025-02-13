using System.Security.Cryptography;
using System.Text;

namespace TabbyCat.App.Tools;

public class CertificateGenerator
{
    public static string GenerateHMACSignatureForMessage(string message, string key)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToBase64String(hash);
        }
    }

    public static string GenerateHMACSignatureForFile(string filePath, string key)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            var fileBytes = File.ReadAllBytes(filePath);
            var hash = hmac.ComputeHash(fileBytes);
            return Convert.ToBase64String(hash);
        }
    }

    public static bool VerifyHMACSignatureForMessage(string message, string signature, string key)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            var computedSignature = Convert.ToBase64String(computedHash);
            return computedSignature == signature; // 验证签名
        }
    }

    public static bool VerifyHMACSignatureForFile(string filePath, string signature, string key)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            var fileBytes = File.ReadAllBytes(filePath);
            var computedHash = hmac.ComputeHash(fileBytes);
            var computedSignature = Convert.ToBase64String(computedHash);
            return computedSignature == signature; // 验证签名
        }
    }
}