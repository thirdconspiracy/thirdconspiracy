using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using thirdconspiracy.Utilities.Models;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class EncodingExtensions
    {

        public static string ToHash<T>(this IEnumerable<T> objectCollection)
        {
            var jsonOptions = new JsonObjectSerializationOptions
            {
                SerializeNullProperties = false,
                IndentOutput = false,
                SerializeEnumsAsStrings = false,
            };
            return objectCollection
                .Select(node => node.SerializeObjectJson(jsonOptions))
                .Aggregate(string.Empty, (finalHash, json) => ToSha1Hash(finalHash + json));
        }

        public static string ToHash<T>(this T obj)
        {
            var jsonOptions = new JsonObjectSerializationOptions
            {
                SerializeNullProperties = false,
                IndentOutput = false,
                SerializeEnumsAsStrings = false,
            };
            var json = obj.SerializeObjectJson(jsonOptions);
            return ToSha1Hash(json);
        }

        public static string ToSha256WithRsa(this string stringToSign, string privateKey)
        {
            return stringToSign.ToSha256WithRsa(Convert.FromBase64String(privateKey));
        }

        public static string ToSha256WithRsa(this string stringToSign, byte[] privateKeyBytes)
        {
            try
            {
                var rsaKeyParameters = (RsaKeyParameters)PrivateKeyFactory.CreateKey(privateKeyBytes);
                var signer = SignerUtilities.GetSigner("SHA-256withRSA");
                signer.Init(true, rsaKeyParameters);
                var stringToSignInBytes = Encoding.UTF8.GetBytes(stringToSign);
                signer.BlockUpdate(stringToSignInBytes, 0, stringToSignInBytes.Length);
                var signature = signer.GenerateSignature();

                return Convert.ToBase64String(signature);
            }
            catch (Exception)
            {
                throw; //TODO Later
            }
            
        }

        private static string ToSha1Hash(this string input)
        {
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash);
            }
        }

        public static string ToSHA256Hash(this string input)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var byteArray = Encoding.UTF8.GetBytes(input);
                var byteHash = sha256Hash.ComputeHash(byteArray);

                var sBuilder = new StringBuilder();
                for (var i = 0; i < byteHash.Length; i++)
                {
                    sBuilder.Append(byteHash[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        public static string ToMD5Hash(this string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes 
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data  
                // and format each one as a hexadecimal string. 
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string. 
                return sBuilder.ToString();
            }
        }

        public static string ToBase64FromString(this string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);

                return Convert.ToBase64String(bytes);
            }
            return string.Empty;
        }

        public static string ToStringFromFromBase64(this string base64Value)
        {
            if (!String.IsNullOrEmpty(base64Value))
            {
                byte[] bytes = Convert.FromBase64String(base64Value);

                return Encoding.UTF8.GetString(bytes);
            }
            return string.Empty;
        }

        public static string ToHex(this IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        public static string ToHex(this string s)
        {
            try
            {
                return BitConverter.ToString(Encoding.Default.GetBytes(s));
            }
            catch (EncoderFallbackException e)
            {
                return $"Failed to convert string to hex: {e.Message}";
            }
        }

        public static IEnumerable<byte> ToBytesFromHex(this string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("Input string must be even in length");
            }
            for (var i = 0; i < hexString.Length; i += 2)
            {
                yield return Convert.ToByte(hexString.Substring(i, 2), 16);
            }
        }
    }
}
