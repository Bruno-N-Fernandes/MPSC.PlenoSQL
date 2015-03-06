using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	[TestClass]
	public class TestandoWebClient
	{
		//[TestMethod]
		public void webClient()
		{
			const int Len = 4096;
			var buffer = new Byte[Len];
			var html = String.Empty;
			var webClient = new WebClient();

			var stream = webClient.OpenRead("http://www.google.com.br");
			var read = stream.Read(buffer, 0, Len);
			while (read > 0)
			{
				html += System.Text.Encoding.Default.GetString(buffer, 0, read);
				read = stream.Read(buffer, 0, Len);
			}
			stream.Close();
			stream.Dispose();
			Assert.IsNotNull(html);
		}
	}
}