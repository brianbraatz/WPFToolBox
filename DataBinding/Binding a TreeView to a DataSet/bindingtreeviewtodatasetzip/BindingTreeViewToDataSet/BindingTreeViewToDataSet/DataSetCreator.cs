using System;
using System.Data;

namespace BindingTreeViewToDataSet
{
	public static class DataSetCreator
	{
		public static DataSet CreateDataSet()
		{
			DataSet ds = new DataSet();

			// Create the parent table.
			// ***************************************
			DataTable tbl = new DataTable( "Master" );
			tbl.Columns.Add( "ID", typeof( int ) );
			tbl.Columns.Add( "Name" );
			for( int i = 0; i < 3; ++i )
			{
				DataRow row = tbl.NewRow();
				row["ID"] = i;
				row["Name"] = "Master #" + i;
				tbl.Rows.Add( row );
			}
			ds.Tables.Add( tbl );

			// Create the child table.
			// ***************************************
			tbl = new DataTable( "Detail" );
			tbl.Columns.Add( "MasterID", typeof( int ) );
			tbl.Columns.Add( "Info" );
			for( int i = 0; i < 9; ++i )
			{
				DataRow row = tbl.NewRow();
				row["MasterID"] = i % 3;
				row["Info"] = String.Format(
					"Detail Info #{0} for Master #{1}",
					(i / 3), (i % 3) );
				tbl.Rows.Add( row );
			}
			ds.Tables.Add( tbl );

			// Associate the tables.
			// ***************************************
			ds.Relations.Add(
				"Master2Detail",
				ds.Tables["Master"].Columns["ID"],
				ds.Tables["Detail"].Columns["MasterID"] );

			return ds;
		}
	}
}
