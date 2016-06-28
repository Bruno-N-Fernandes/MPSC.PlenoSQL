using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace MPSC.PlenoSQL.TestesUnitarios.MDA
{
	public class MDD
	{
		public class Modelo
		{
			public String Nome { get; set; }
		}

		public class Classe : Modelo
		{
			public List<Propriedade> Propriedades { get; set; }
			public Classe(String texto)
			{
				Propriedades = new List<Propriedade>();
				var linhas = texto.Split("\r\n;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				foreach (var linha in linhas)
				{
					Propriedades.Add(new Propriedade(linha));
				}
			}
		}

		public class Propriedade : Modelo
		{
			private static readonly Type[] types = Assembly.GetAssembly(typeof(String)).GetTypes().Where(t => t.IsVisible && t.IsPublic && t.IsSealed && !t.IsEnum && !t.IsGenericType && !t.IsGenericTypeDefinition && (t.FullName == "System." + t.Name)).OrderBy(t => t.Name).ToArray();
			public Type Tipo { get; set; }

			public Propriedade(String linha)
			{
				var prop = linha.Split(' ');
				Tipo = Obter(prop[0]);
				Nome = prop[1];
			}
			private Type Obter(String tipo)
			{
				return types.FirstOrDefault(t => t.Name == tipo)
					?? types.FirstOrDefault(t => t.Name.ToUpper() == tipo.ToUpper())
					?? types.FirstOrDefault(t => t.Name.StartsWith(tipo))
					?? types.FirstOrDefault(t => t.Name.ToUpper().StartsWith(tipo.ToUpper()))
					?? types.FirstOrDefault(t => t.Name.EndsWith(tipo))
					?? types.FirstOrDefault(t => t.Name.ToUpper().EndsWith(tipo.ToUpper()))
					?? types.FirstOrDefault(t => t.Name.Contains(tipo))
					?? types.FirstOrDefault(t => t.Name.ToUpper().Contains(tipo.ToUpper()))

					;
			}
		}

		public static Classe Load(String texto)
		{
			return new Classe(texto);
		}
	}
}