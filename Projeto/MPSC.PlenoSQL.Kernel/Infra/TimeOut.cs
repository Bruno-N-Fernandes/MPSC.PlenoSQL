using System;
using System.Timers;

namespace MPSC.PlenoSQL.Kernel.Infra
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
			TimeOut.timeOut = timeOut ?? new TimeOut();
			TimeOut.timeOut.Configurar(action, miliSegundos);
			return TimeOut.timeOut;
		}


		private static TimeOut timeOut;
		public static TimeOut SetTimeOut(Action action, Int32 miliSegundos)
		{
			TimeOut.timeOut = TimeOut.timeOut ?? new TimeOut();
			TimeOut.timeOut.Configurar(action, miliSegundos);
			return TimeOut.timeOut;
		}
	}
}