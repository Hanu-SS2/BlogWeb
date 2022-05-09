using System.Security.Cryptography;
using System.Text;
namespace BlogWeb.Extensions
{
    public static class HashMD5
    {
        public static string ToMD5(this string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] sbHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in sbHash)
                sb.Append(String.Format("0:x2", b));
            return sb.ToString();
        }
    }
}
