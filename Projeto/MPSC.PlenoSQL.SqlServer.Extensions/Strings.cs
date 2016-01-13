using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace MPSC.PlenoSQL.SqlServer.Extensions
{
	public class Strings
	{
		[SqlFunction]
		public static SqlString RegExpReplace(SqlString input, SqlString expressaoRegular, SqlString substituicao)
		{
			return new SqlString(Regex.Replace(input.Value, expressaoRegular.Value, substituicao.Value));
		}
	}
}