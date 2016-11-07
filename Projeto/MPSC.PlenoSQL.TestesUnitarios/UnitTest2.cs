using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml.Serialization;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	[TestClass]
	public class UnitTest
	{
		[TestMethod]
		public void TestMethod()
		{
			var myWriter = new StreamWriter("d:\\myFileName1.xml");

			var myObject0 = new Classe { ExibeLogo = true, Property1 = DateTime.Today, Property3 = 5 };
			var mySerializer0 = new XmlSerializer(myObject0.GetType());
			mySerializer0.Serialize(myWriter, myObject0);

			myWriter.WriteLine("\r\n\r\n");

			var myObject1 = new ClasseQueNaoSerializa3 { ExibeLogo = true, Property1 = DateTime.Today, Property3 = 4 };
			var mySerializer1 = new XmlSerializer(myObject1.GetType());
			mySerializer1.Serialize(myWriter, myObject1);

			myWriter.Close();
		}
	}

	[Serializable]
	public class Classe
	{
		[XmlElement(Order = 4)]
		public virtual Boolean ExibeLogo { get; set; }

		[XmlElement(Order = 3)]
		public virtual Int64 Property3 { get; set; }

		[XmlElement(Order = 1)]
		public virtual DateTime Property1 { get; set; }

	}

	[Serializable, XmlRoot("Classe")]
	public class ClasseQueNaoSerializa3 : Classe
	{
		[XmlElement(Order = 3)]
		public override Int64 Property3 { get; set; }

		[XmlElement(Order = 2)]
		public virtual Int64 Property2 { get; set; }

		[XmlElement(Order = 5)]
		public virtual Int64 Property5 { get; set; }
	}

}