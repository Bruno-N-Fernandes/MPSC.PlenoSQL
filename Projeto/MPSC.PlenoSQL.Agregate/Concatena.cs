using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;

//https://www.mssqltips.com/sqlservertip/2022/concat-aggregates-sql-server-clr-function/

[Serializable]
[SqlUserDefinedAggregate(
	Format.UserDefined, // Binary Serialization because of StringBuilder 
	IsInvariantToOrder = false, // order changes the result  
	IsInvariantToNulls = true, // nulls don't change the result  
	IsInvariantToDuplicates = false, // duplicates change the result 
	MaxByteSize = -1)]

public struct Concatena : IBinarySerialize
{
	private List<String> _builder;
	private String _separador;

	public void Init()
	{
		_builder = new List<String>();
		_separador = String.Empty;
	}

	public void Accumulate(SqlString valor, SqlString separador)
	{
		_separador = separador.IsNull ? ";" : separador.Value;

		if (valor.Value != null)
			_builder.Add(valor.Value);
	}

	public void Merge(Concatena group)
	{
		_builder.AddRange(group._builder);
	}

	public SqlString Terminate()
	{
		return new SqlString(String.Join(_separador, _builder));
	}

	public void Read(BinaryReader r)
	{
		_separador = r.ReadString();
		_builder = new List<String> { r.ReadString() };
	}

	public void Write(BinaryWriter w)
	{
		w.Write(_separador);
		w.Write(String.Join(_separador, _builder));
	}
}