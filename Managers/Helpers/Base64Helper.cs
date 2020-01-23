using System;
using System.Text;

namespace HCR
{
	/// <summary>
	/// Класс - для кодирования и раскодирования строк Base64-преобразованием
	/// </summary>

	public static class Base64Helper
	{



		// INETRFACES

		/// <summary>
		/// Закодировать строку
		/// </summary>
		public static string Encode(string plainText)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

			return Convert.ToBase64String(plainTextBytes);
		}

		/// <summary>
		/// Раскодировать строку
		/// </summary>
		public static string Decode(string base64EncodedData)
		{
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

			return Encoding.UTF8.GetString(base64EncodedBytes);
		}



	}
}