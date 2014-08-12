using System;
using System.CodeDom.Compiler;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;
using MP.PlenoBDNE.AppWin.Infra.Interface;

namespace MP.PlenoBDNE.AppWin.Infra
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
			var properties = String.Empty;
			for (Int32 i = 0; (iDataReader != null) && (!iDataReader.IsClosed) && (i < iDataReader.FieldCount); i++)
			{
				var propertyName = NomeDoCampo(iDataReader, i);
				properties += String.Format("\t\tpublic {0}{1} {2}{3} {{ get; set; }}\r\n", iDataReader.GetFieldType(i).Name, iDataReader.GetFieldType(i).IsValueType ? "?" : "", propertyName, properties.Contains(" " + propertyName + " ") ? i.ToString() : String.Empty);
			}

			var classe = CriarClasseVirtual(properties, "DadosDinamicos");
			messageResult.Processar(classe, "TipoVirtual");
			return CompilarClasseVirtual(classe, "DadosDinamicos");
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