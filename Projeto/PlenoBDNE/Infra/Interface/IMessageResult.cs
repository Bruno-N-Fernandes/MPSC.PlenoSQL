using System;

namespace MP.PlenoBDNE.AppWin.Infra.Interface
{
	public interface IMessageResult
	{
		void Processar(String message, String tipo);
	}
}