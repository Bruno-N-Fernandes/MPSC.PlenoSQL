using System;

namespace MPSC.PlenoSQL.Kernel.Interface
{
	public interface IMessageResult
	{
		void ShowLog(String message, String tipo);
		void ProcessarEventos();
	}
}