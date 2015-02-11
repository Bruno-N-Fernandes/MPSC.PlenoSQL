using System;
using System.CodeDom.Compiler;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;
using MPSC.PlenoSQL.AppWin.Interface;
using MPSC.PlenoSQL.AppWin.Dados.Base;

namespace MPSC.PlenoSQL.AppWin.Infra
{
	public static class ClasseDinamica
	{
		public static Object CreateObjetoVirtual(Type tipo, IDataReader iDataReader)
		{
			Object obj = ((tipo == null) ? null : Activator.CreateInstance(tipo));
			for (Int32 i = 0; (obj != null) && (tipo != null) && (iDataReader != null) && (!iDataReader.IsClosed) && (i < iDataReader.FieldCount); i++)
			{
				var property = tipo.GetProperty(NomeDoCampo(iDataReader, i) + i) ?? tipo.GetProperty(NomeDoCampo(iDataReader, i));
				if (property != null)
					property.SetValue(obj, iDataReader.IsDBNull(i) ? null : iDataReader.GetValue(i), null);
			}
			Application.DoEvents();
			return obj;
		}

		public static Type CriarTipoVirtual(IDataReader iDataReader, IMessageResult messageResult)
		{
			Type tipo = null;
			if (iDataReader != null)
			{
				var properties = String.Empty;
				var classeVOf = String.Empty;
				var classeVOp = String.Empty;
				var classeVOc = String.Empty;
				var classeVOs = String.Empty;
				for (Int32 i = 0; (iDataReader.IsOpen()) && (i < iDataReader.FieldCount); i++)
				{
					var type = iDataReader.GetFieldType(i).Name + (iDataReader.GetFieldType(i).IsValueType ? "?" : "");
					var propertyName = NomeDoCampo(iDataReader, i);
					propertyName += properties.Contains(" " + propertyName + " ") ? i.ToString() : String.Empty;
					var field = (propertyName.ToUpper() == propertyName) ? propertyName.ToLower() : (propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1));
					var property = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);

					properties += String.Format("\t\tpublic {0} {1} {{ get; set; }}\r\n", type, propertyName);
					classeVOf += String.Format("\t\tprivate readonly {0} {1};\r\n", type, field);
					classeVOp += String.Format("\t\tpublic {0} {1} {{ get {{ return this.{2}; }} }}\r\n", type, property, field);
					classeVOc += String.Format(", {0} {1}", type, field);
					classeVOs += String.Format("\t\t\tthis.{0} = {0};\r\n", field);
				}
				classeVOc += "  ";
				var classeDTO = CriarClasseVirtual(properties, "DadosDinamicosDTO");
				var classeVO = CriarClasseVirtual(classeVOf + "\r\n" + classeVOp + "\r\n\t\tpublic DadosDinamicosVO(" + classeVOc.Substring(2).Trim() + ")\r\n\t\t{\r\n" + classeVOs + "\t\t}\r\n", "DadosDinamicosVO");
				messageResult.ShowLog(classeDTO, "TipoVirtual");
				messageResult.ShowLog(classeVO, "TipoVirtual");
				tipo = CompilarClasseVirtual(classeDTO, "DadosDinamicosDTO");
			}
			return tipo;
		}

		private static String NomeDoCampo(IDataReader iDataReader, Int32 index)
		{
			var nomeDoCampo = iDataReader.GetName(index);
			nomeDoCampo = String.IsNullOrWhiteSpace(nomeDoCampo) ? "Campo" + index.ToString() : nomeDoCampo.Replace(" ", "_").Replace(".", "_").Replace("\"", "");
			return Char.IsDigit(nomeDoCampo, 0) ? "C" + nomeDoCampo : nomeDoCampo;
		}

		private static Type CompilarClasseVirtual(String codigoFonte, String nomeClasse)
		{
			CodeDomProvider vCodeCompiler = new CSharpCodeProvider();
			CompilerResults vResults = vCodeCompiler.CompileAssemblyFromSource(CreateCompillerParameters(false, true), codigoFonte);
			return vResults.CompiledAssembly.GetType("Virtual." + nomeClasse, false, true);
		}

		private static String CriarClasseVirtual(String corpoDaClasse, String nomeClasse)
		{
			return String.Format("using System;\r\nnamespace Virtual\r\n{{\r\n\tpublic class {0}\r\n\t{{\r\n{1}\t}}\r\n}}", nomeClasse, corpoDaClasse);
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