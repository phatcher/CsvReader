using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using LumenWorks.Framework.IO.Csv;
using LumenWorks.Framework.Tests.Unit.IO.Csv;

namespace CsvReaderDemo
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			using (CachedCsvReader csv = new CachedCsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				dataGrid1.DataSource = csv;
			}

			button2.Enabled = true;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			ITypedList tList = (ITypedList) ((IListSource) dataGrid1.DataSource).GetList();
			IBindingList bList = (IBindingList) tList;

			int index = bList.Find(tList.GetItemProperties(null)[0], "Stephen");

			dataGrid1.Rows[index].Selected = true;
		}
	}
}