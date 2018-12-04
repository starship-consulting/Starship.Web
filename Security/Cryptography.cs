using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Starship.Core.Extensions;

namespace Starship.Web.Security
{
    public static class Cryptography {
        
        public static bool Validate(string secretKey, string encryptedPassword, string requestedPassword) {
            if (requestedPassword.IsEmpty()) {
                return false;
            }

            var hashedPassword = HashSHA1(secretKey, requestedPassword);

            return hashedPassword == encryptedPassword;
        }

        public static string HashSHA1(string secretKey, string toHash) {
            return Convert.ToBase64String(SHA.ComputeHash(Encoding.UTF8.GetBytes(secretKey + toHash)));
        }

        public static string HmacSha1(byte[] secretKey, Stream input) {
            return HashStream(() => new HMACSHA1 { Key = secretKey }, input);
        }

        public static string HashStream<T>(Func<T> algorithm, Stream input) where T : HashAlgorithm {
            string result;
            using (T t = algorithm()) {
                using (var cryptoStream = new CryptoStream(Stream.Null, t, CryptoStreamMode.Write)) {
                    BoundedCopyTo(input, cryptoStream, 9223372036854775807L);
                    cryptoStream.FlushFinalBlock();
                    IEnumerable<byte> bytes = t.Hash;
                    Func<byte, string> func = ((b) => b.ToString("X2"));
                    result = string.Join("", bytes.Select(func).ToArray());
                }
            }
            return result;
        }

        public static long BoundedCopyTo(Stream source, Stream destination, long length) {
            byte[] buffer = new byte[1024];
            long num = 0L;

            do {
                int num2 = 1024;
                if (num + (long)num2 > length) {
                    num2 = (int)(length - num);
                }
                int num3 = source.Read(buffer, 0, num2);
                if (num3 <= 0) {
                    break;
                }
                num += (long)num3;
                destination.Write(buffer, 0, num3);
            }
            while (num != length);

            return num;
        }

        public static string HmacSha1(string secretKey, string message) {
            return HmacSha1(secretKey, message, Encoding.UTF8);
        }

        public static string HmacSha1(string secretKey, string message, Encoding encoding) {
            return HmacSha1(encoding.GetBytes(secretKey), new MemoryStream(encoding.GetBytes(message)));
        }

        private static readonly SHA1CryptoServiceProvider SHA = new SHA1CryptoServiceProvider();
    }
}
