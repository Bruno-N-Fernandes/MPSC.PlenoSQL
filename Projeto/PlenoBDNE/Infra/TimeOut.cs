using System;
using System.Timers;

namespace MP.PlenoBDNE.AppWin.Infra
{
	public class TimeOut
	{
		private Timer timer;

		public void Configurar(Action action, int miliSegundos)
		{
			if (timer != null)
			{
				timer.Stop();
				timer.Enabled = false;
				timer.Close();
				timer.Dispose();
			}
			timer = new Timer(miliSegundos);
			timer.Elapsed += (o, e) => action();
			timer.Start();
		}

		public static TimeOut SetTimeOut(TimeOut timeOut, Action action, Int32 miliSegundos)
		{
			timeOut = timeOut ?? new TimeOut();
			timeOut.Configurar(action, miliSegundos);
			return timeOut;
		}


		private static TimeOut timeOut;
		public static TimeOut SetTimeOut(Action action, Int32 miliSegundos)
		{
			timeOut = timeOut ?? new TimeOut();
			timeOut.Configurar(action, miliSegundos);
			return timeOut;
		}
	}
}