using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;
using MPSC.PlenoSQL.Kernel.Interface;
using MPSC.PlenoSQL.Kernel.Dados.Base;
using System.Collections.Generic;

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

			Application.DoEvents();
			return obj;
		}

		public static Type CriarTipoVirtual(IDataReader iDataReader, IMessageResult messageResult)
		{
			Type tipo = null;
			if (iDataReader != null)
			{
				var transformador = new Transformador(iDataReader);
				var classeDTO = CriarClasseVirtual("DadosDinamicosDTO", transformador.Propriedade);
				//var classeVO = CriarClasseVirtual("DadosDinamicosVO", transformador.CampoSomenteLeitura, transformador.PropGet, "\t\tpublic DadosDinamicosVO(" + transformador.Parametro + ")", "\t\t{", transformador.Atribuicao, "\t\t}");

				messageResult.ShowLog(transformador.Nomes, "ListaSelect");
				//messageResult.ShowLog(classeDTO, "TipoVirtual");
				//messageResult.ShowLog(classeVO, "TipoVirtual");
	
				tipo = CompilarClasseVirtual(classeDTO, "DadosDinamicosDTO");
			}
			return tipo;
		}

		public class Transformador
		{
			public readonly List<Field> fields = new List<Field>();


			public String Nomes { get { return "\t" + String.Join(",\r\n\t", fields.Select(f => f.Property)); } }
			public String CampoSomenteLeitura { get { return String.Join("\r\n", fields.Select(f => f.CampoSomenteLeitura)); } }
			public String PropGet { get { return String.Join("\r\n", fields.Select(f => f.PropGet)); } }
			public String Propriedade { get { return String.Join("\r\n", fields.Select(f => f.Propriedade)); } }
			public String Parametro { get { return String.Join(", ", fields.Select(f => f.Parametro)); } }
			public String Atribuicao { get { return String.Join("\r\n", fields.Select(f => f.Atribuicao)); } }


			public Transformador(IDataReader iDataReader)
			{
				if (iDataReader != null)
				{
					for (Int32 index = 0; (iDataReader.IsOpen()) && (index < iDataReader.FieldCount); index++)
					{
						var type = iDataReader.GetFieldType(index).Name + (iDataReader.GetFieldType(index).IsValueType ? "?" : "");
						var originalName = iDataReader.GetName(index);

						var field = new Field(type, originalName, index, fields);
						fields.Add(field);
					}
				}
			}

			public class Field
			{
				private readonly String _type;
				private readonly String _originalName;
				public readonly String Property;
				private readonly String _field;
				private readonly String _parameter;


				public String CampoSomenteLeitura { get { return String.Format("\t\tprivate readonly {0} {1};", _type, _field); } }
				public String PropGet { get { return String.Format("\t\tpublic {0} {1} {{ get {{ return this.{2}; }} }}", _type, Property, _field); } }
				public String Propriedade { get { return String.Format("\t\tpublic {0} {1} {{ get; set; }}", _type, Property); } }
				public String Parametro { get { return String.Format("{0} {1}", _type, _parameter); } }
				public String Atribuicao { get { return String.Format("\t\t\tthis.{0} = {1};", _field, _parameter); } }


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


		private static String NomeDoCampo(IDataReader iDataReader, Int32 index)
		{
			var nomeDoCampo = iDataReader.GetName(index);
			nomeDoCampo = String.IsNullOrWhiteSpace(nomeDoCampo) ? "Campo" + index.ToString() : nomeDoCampo.Replace(" ", "_").Replace(".", "_").Replace("\"", "");
			return Cache.Traduzir(Char.IsDigit(nomeDoCampo, 0) ? "C" + nomeDoCampo : nomeDoCampo);
		}

		private static Type CompilarClasseVirtual(String codigoFonte, String nomeClasse)
		{
			CodeDomProvider vCodeCompiler = new CSharpCodeProvider();
			CompilerResults vResults = vCodeCompiler.CompileAssemblyFromSource(CreateCompillerParameters(false, true), codigoFonte);
			return vResults.CompiledAssembly.GetType("Virtual." + nomeClasse, false, true);
		}

		private static String CriarClasseVirtual(String nomeClasse, params String[] corpoDaClasse)
		{
			return String.Format("using System;\r\nnamespace Virtual\r\n{{\r\n\tpublic class {0}\r\n\t{{\r\n{1}\r\n\t}}\r\n}}", nomeClasse, String.Join("\r\n", corpoDaClasse));
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
}