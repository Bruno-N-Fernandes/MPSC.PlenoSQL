using System;
using System.CodeDom.Compiler;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace LBJC.NavegadorDeDados
{
	public static class ClasseDinamica
	{
		public static Object CreateObjetoVirtual(Type tipo, IDataReader iDataReader)
		{
			Object obj = ((tipo == null) ? null : Activator.CreateInstance(tipo));
			for (Int32 i = 0; (iDataReader != null) && (!iDataReader.IsClosed) && (i < iDataReader.FieldCount); i++)
			{
				var property = tipo.GetProperty(NomeDoCampo(iDataReader, i));
				if (property != null)
					property.SetValue(obj, iDataReader.IsDBNull(i) ? null : iDataReader.GetValue(i), null);
			}
			Application.DoEvents();
			return obj;
		}

		public static Type CriarTipoVirtual(IDataReader iDataReader)
		{
			var properties = String.Empty;
			for (Int32 i = 0; (iDataReader != null) && (!iDataReader.IsClosed) && (i < iDataReader.FieldCount); i++)
			{
				var propertyName = NomeDoCampo(iDataReader, i);
				if (!properties.Contains(" " + propertyName + " "))
					properties += String.Format("\t\tpublic {0}{1} {2} {{ get; set; }}\r\n", iDataReader.GetFieldType(i).Name, iDataReader.GetFieldType(i).IsValueType ? "?" : "", propertyName);
			}
			return CriarClasseVirtual("DadosDinamicos", properties);
		}

		private static String NomeDoCampo(IDataReader iDataReader, Int32 index)
		{
			var nomeDoCampo = iDataReader.GetName(index);
			nomeDoCampo = String.IsNullOrWhiteSpace(nomeDoCampo) ? "Campo" + index.ToString() : nomeDoCampo.Replace(" ", "_").Replace(".", "_").Replace("\"", "");
			return Char.IsDigit(nomeDoCampo, 0) ? "C" + nomeDoCampo : nomeDoCampo;
		}

		private static Type CriarClasseVirtual(String nomeClasse, String codigo)
		{
			var classe = String.Format("using System;\nnamespace Virtual\n{{\n\tpublic class {0}\n\t{{\n{1}\t}}\n}}", nomeClasse, codigo);
			CodeDomProvider vCodeCompiler = new CSharpCodeProvider();
			CompilerResults vResults = vCodeCompiler.CompileAssemblyFromSource(CreateCompillerParameters(false, true), classe);
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
}