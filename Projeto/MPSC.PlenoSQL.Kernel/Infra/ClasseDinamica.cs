using Microsoft.CSharp;
using MPSC.PlenoSQL.Kernel.Dados.Base;
using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MPSC.PlenoSQL.Kernel.Infra
{
	public static class ClasseDinamica
	{
		public static Object CriarObjetoVirtual(Type tipo, IDataReader iDataReader, PropertyInfo[] properties)
		{
			var obj = Activator.CreateInstance(tipo);
			if ((iDataReader != null) && (properties.Length == iDataReader.FieldCount))
				for (var i = 0; i < iDataReader.FieldCount; i++)
					properties[i].SetValue(obj, iDataReader.IsDBNull(i) ? null : iDataReader.GetValue(i), null);

			return obj;
		}

		public static Type CriarTipoVirtual(IDataReader iDataReader, IMessageResult messageResult)
		{
			Type tipo = null;
			if (iDataReader != null)
			{
				var transformador = new Transformador(iDataReader);
				var classeDTO = transformador.CriarClasseDTO("DadosDinamicosDTO");
				tipo = CompilarClasseVirtual(classeDTO, "DadosDinamicosDTO");

				messageResult.ShowLog(classeDTO, "TipoVirtual");
				messageResult.ShowLog(transformador.Nomes, "ListaSelect");
				messageResult.ShowLog(transformador.CriarClasseVO_Privado("DadosDinamicosVO"), "TipoVirtual");
				messageResult.ShowLog(transformador.CriarClasseVO_Publico("DadosDinamicosVO"), "TipoVirtual");
			}
			return tipo;
		}

		private static Type CompilarClasseVirtual(String codigoFonte, String nomeClasse)
		{
			var vCodeCompiler = new CSharpCodeProvider();
			var vResults = vCodeCompiler.CompileAssemblyFromSource(CreateCompillerParameters(false, true), codigoFonte);
			return vResults.CompiledAssembly.GetType("Virtual." + nomeClasse, false, true);
		}

		private static CompilerParameters CreateCompillerParameters(Boolean generateExecutable, Boolean includeDebugInformation)
		{
			return new CompilerParameters(new String[] { Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase) })
			{
				GenerateInMemory = !generateExecutable,
				GenerateExecutable = generateExecutable,
				IncludeDebugInformation = includeDebugInformation
			};
		}
	}

	public class Transformador
	{
		public readonly List<Field> fields = new List<Field>();

		public String Nomes { get { return "\t" + String.Join(",\r\n\t", fields.Select(f => f.Property)); } }
		public String CampoPublicSomenteLeitura { get { return String.Join("\r\n", fields.Select(f => f.CampoPublicSomenteLeitura)); } }
		public String CampoPrivateSomenteLeitura { get { return String.Join("\r\n", fields.Select(f => f.CampoPrivateSomenteLeitura)); } }
		public String PropGet { get { return String.Join("\r\n", fields.Select(f => f.PropGet)); } }
		public String Propriedade { get { return String.Join("\r\n", fields.Select(f => f.Propriedade)); } }
		public String Parametro { get { return String.Join(", ", fields.Select(f => f.Parametro)); } }
		public String AtribuicaoPrivate { get { return String.Join("\r\n", fields.Select(f => f.AtribuicaoPrivate)); } }
		public String AtribuicaoPublic { get { return String.Join("\r\n", fields.Select(f => f.AtribuicaoPublic)); } }


		public Transformador(IDataReader iDataReader)
		{
			for (Int32 index = 0; (iDataReader.IsOpen()) && (index < iDataReader.FieldCount); index++)
			{
				var type = iDataReader.GetFieldType(index).Name + (iDataReader.GetFieldType(index).IsValueType ? "?" : "");
				var originalName = iDataReader.GetName(index);

				fields.Add(new Field(type, originalName, index, fields));
			}
		}

		public String CriarClasseDTO(String nomeClasse)
		{
			return CriarClasseVirtual(nomeClasse, Propriedade);
		}

		public String CriarClasseVO_Privado(String nomeClasse)
		{
			return CriarClasseVirtual(nomeClasse, CampoPrivateSomenteLeitura, PropGet, "\t\tpublic DadosDinamicosVO(" + Parametro + ")", "\t\t{", AtribuicaoPrivate, "\t\t}");
		}
		public String CriarClasseVO_Publico(String nomeClasse)
		{
			return CriarClasseVirtual(nomeClasse, CampoPublicSomenteLeitura, "\t\tpublic DadosDinamicosVO(" + Parametro + ")", "\t\t{", AtribuicaoPublic, "\t\t}");
		}

		private static String CriarClasseVirtual(String nomeClasse, params String[] corpoDaClasse)
		{
			return String.Format("using System;\r\nnamespace Virtual\r\n{{\r\n\tpublic class {0}\r\n\t{{\r\n{1}\r\n\t}}\r\n}}", nomeClasse, String.Join("\r\n", corpoDaClasse));
		}

		public class Field
		{
			private readonly String _type;
			private readonly String _originalName;
			public readonly String Property;
			private readonly String _field;
			private readonly String _parameter;


			public String CampoPublicSomenteLeitura { get { return String.Format("\t\tpublic readonly {0} {1};", Property, _field); } }
			public String CampoPrivateSomenteLeitura { get { return String.Format("\t\tprivate readonly {0} {1};", _type, _field); } }
			public String PropGet { get { return String.Format("\t\tpublic {0} {1} {{ get {{ return this.{2}; }} }}", _type, Property, _field); } }
			public String Propriedade { get { return String.Format("\t\tpublic {0} {1} {{ get; set; }}", _type, Property); } }
			public String Parametro { get { return String.Format("{0} {1}", _type, _parameter); } }
			public String AtribuicaoPublic { get { return String.Format("\t\t\t{0} = {1};", Property, _parameter); } }
			public String AtribuicaoPrivate { get { return String.Format("\t\t\tthis.{0} = {1};", _field, _parameter); } }


			public Field(String type, String originalName, Int32 index, IEnumerable<Field> fields)
			{
				_type = type;
				_originalName = originalName;
				Property = NomeDoCampo(_originalName, index);
				Property += fields.Any(f => f.Property == Property) ? index.ToString() : String.Empty;
				_parameter = (Property.ToUpper() == Property) ? Property.ToLower() : FirstLower(Property);
				_field = (Property.ToUpper() == Property) ? Property.ToLower() : ((Property.ToLower() == Property) ? "_" + Property : FirstLower(Property));
				Property = FirstUpper(Property);
			}

			public String NomeDoCampo(String nomeDoCampo, Int32 index)
			{
				nomeDoCampo = String.IsNullOrWhiteSpace(nomeDoCampo) ? "Campo" + index.ToString() : ReplaceIllegalChars(nomeDoCampo);
				return Cache.Traduzir(Char.IsDigit(nomeDoCampo, 0) ? "C" + nomeDoCampo : nomeDoCampo);
			}

			public static String FirstLower(String texto)
			{
				return texto.Substring(0, 1).ToLower() + texto.Substring(1);
			}

			public static String FirstUpper(String texto)
			{
				return texto.Substring(0, 1).ToUpper() + texto.Substring(1);
			}

			public static String ReplaceIllegalChars(String nomeDoCampo)
			{
				return nomeDoCampo
					.Replace(" ", "_")
					.Replace(".", "_")
					.Replace("+", "_")
					.Replace("-", "_")
					.Replace("*", "_")
					.Replace("/", "_")
					.Replace("\"", "");
			}
		}
	}
}