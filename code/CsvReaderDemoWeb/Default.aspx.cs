using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using LumenWorks.Framework.IO.Csv;
using LumenWorks.Framework.Tests.Unit.IO.Csv;

namespace CsvReaderDemoWeb
{
	public partial class _Default 
		: System.Web.UI.Page
	{
		protected void btnBind_Click(object sender, EventArgs e)
		{
			using (CsvReader csv = new CsvReader(new StringReader(CsvReaderSampleData.SampleData1), true))
			{
				rptSampleData.DataSource = csv;
				rptSampleData.DataBind();
			}
		}

		protected void rptSampleData_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			string[] dataItem = (string[]) e.Item.DataItem;

			((ITextControl) e.Item.FindControl("lblFirstName")).Text = dataItem[0];
			((ITextControl) e.Item.FindControl("lblLastName")).Text = dataItem[1];
			((ITextControl) e.Item.FindControl("lblAddress")).Text = dataItem[2];
			((ITextControl) e.Item.FindControl("lblCity")).Text = dataItem[3];
			((ITextControl) e.Item.FindControl("lblState")).Text = dataItem[4];
			((ITextControl) e.Item.FindControl("lblZipCode")).Text = dataItem[5];
		}
	}
}