using System.Collections;
using System.Collections.Specialized;

namespace MPSC.LanguageEditor
{
	/// <summary>
	/// Summary description for AutoCompleteForm.
	/// </summary>
	public partial class AutoCompleteForm : System.Windows.Forms.Form
	{

		public StringCollection Items 
		{
			get 
			{
				return mItems;
			}
		}

		internal int ItemHeight 
		{
			get  
			{
				return 18;
			}
		}



		public AutoCompleteForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		public string SelectedItem 
		{
			get
			{
				if (lstCompleteItems.SelectedItems.Count == 0) return null;
				return (string)lstCompleteItems.SelectedItems[0].Text;
			}
		}


		private void lstCompleteItems_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		}

		internal int SelectedIndex 
		{
			get 
			{
				if (lstCompleteItems.SelectedIndices.Count == 0)
				{
					return -1;
				}
				return lstCompleteItems.SelectedIndices[0];
			}
			set
			{
				lstCompleteItems.Items[value].Selected = true;
			}
		}
		private void AutoCompleteForm_Resize(object sender, System.EventArgs e)
		{
			//System.Diagnostics.Debug.WriteLine(string.Format("Size x:{0} y:{1}\r\n {2}", Size.Width , Size.Height, Environment.StackTrace));
		}

		internal void UpdateView()
		{
			lstCompleteItems.Items.Clear();
			foreach (string item in mItems)
			{
				lstCompleteItems.Items.Add(item);
			}
		}

		private void AutoCompleteForm_VisibleChanged(object sender, System.EventArgs e)
		{
			ArrayList items = new ArrayList(mItems);
			items.Sort(new CaseInsensitiveComparer());
			mItems = new StringCollection();
			mItems.AddRange((string[])items.ToArray(typeof(string)));
			columnHeader1.Width = lstCompleteItems.Width - 20;

		}

		private void lstCompleteItems_Resize(object sender, System.EventArgs e)
		{
			if (this.Size != lstCompleteItems.Size)
			{
				
			}
		}
	}
}
