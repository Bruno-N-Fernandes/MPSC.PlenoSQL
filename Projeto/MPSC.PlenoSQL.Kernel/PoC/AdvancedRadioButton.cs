using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.Kernel.PoC
{
	public class AdvancedRadioButton : CheckBox
	{
		public enum Level { Parent, Form };

		[Category("AdvancedRadioButton"),
		Description("Gets or sets the level that specifies which RadioButton controls are affected."),
		DefaultValue(Level.Parent)]
		public Level GroupNameLevel { get; set; }

		[Category("AdvancedRadioButton"),
		Description("Gets or sets the name that specifies which RadioButton controls are mutually exclusive.")]
		public string GroupName { get; set; }

		protected override void OnCheckedChanged(EventArgs e)
		{
			base.OnCheckedChanged(e);

			if (Checked)
			{
				var arbControls = (dynamic)null;
				switch (GroupNameLevel)
				{
					case Level.Parent:
						if (this.Parent != null)
							arbControls = GetAll(this.Parent);
						break;
					case Level.Form:
						Form form = this.FindForm();
						if (form != null)
							arbControls = GetAll(form);
						break;
				}
				if (arbControls != null)
					foreach (var control in arbControls)
						if (control != this && control.GroupName == this.GroupName)
							control.Checked = false;
			}
		}

		protected override void OnClick(EventArgs e)
		{
			if (!Checked)
				base.OnClick(e);
		}

		private IEnumerable<AdvancedRadioButton> GetAll(Control control)
		{
			return control.Controls.Cast<Control>().Where(c => c is AdvancedRadioButton).Cast<AdvancedRadioButton>();

		}
	}
}