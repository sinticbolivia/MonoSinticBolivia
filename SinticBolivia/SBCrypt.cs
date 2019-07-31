using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SinticBolivia
{
	public class SBCrypt
	{
		public SBCrypt ()
		{
		}
		public static string GetMD5(string cadena)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(cadena));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
	}
}

