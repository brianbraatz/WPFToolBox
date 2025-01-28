using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace MyAdvancedListbox
{
    public class Database
    {
        // -- Enumerations --
        // -- Fields --
        // -- Properties --
        
        private static String ConnectionString
        {
            get { return MyAdvancedListbox.Properties.Settings.Default.ConnectionString; }
        }

        // -- Constructors --
        private Database()
        {

        }

        // -- Methods --
        // Connectie maken en openen
        private static OleDbConnection GetConnection()
        {
            OleDbConnection oCon = new OleDbConnection(ConnectionString);
            oCon.Open();
            return oCon;
        }

        // De connectie sluiten en vrijgeven
        private static void ReleaseConnection(OleDbConnection oCon)
        {
            if (oCon != null)
            {
                oCon.Close();
                oCon.Dispose();
            }
        }

        // Command maken op basis van bestaande connection, SQL en parameters
        private static OleDbCommand BuildCommand(OleDbConnection oCon, String sSQL, params OleDbParameter[] dbParams)
        {
            OleDbCommand oCommand = oCon.CreateCommand();
            oCommand.CommandType = System.Data.CommandType.Text;
            oCommand.CommandText = sSQL;

            if (dbParams != null)
            {
                foreach (OleDbParameter oPar in dbParams)
                {
                    oCommand.Parameters.Add(oPar);
                }
            }
            return oCommand;
        }

        // Command maken op basis van SQL en parameters
        private static OleDbCommand BuildCommand(String sSQL, params OleDbParameter[] dbParams)
        {
            OleDbConnection oCon = GetConnection();
            return BuildCommand(oCon, sSQL, dbParams);
        }

        // Een DataTable ophalen
        public static DataTable GetDT(String sSQL, params OleDbParameter[] dbParams)
        {
            OleDbCommand oCommand = null;
            try
            {
                oCommand = BuildCommand(sSQL, dbParams);

                OleDbDataAdapter oDA = new OleDbDataAdapter();
                oDA.SelectCommand = oCommand;

                DataTable oDT = new DataTable();
                oDA.Fill(oDT);

                return oDT;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oCommand != null)
                {
                    ReleaseConnection(oCommand.Connection);
                }
            }
        }

        // DataReader ophalen
        public static OleDbDataReader GetDR(String sSQL, params OleDbParameter[] dbParams)
        {
            OleDbCommand oCommand = null;
            OleDbDataReader oDR = null;

            try
            {
                oCommand = BuildCommand(sSQL, dbParams);
                oDR = oCommand.ExecuteReader(CommandBehavior.CloseConnection);

                return oDR;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oCommand != null)
                {
                    ReleaseConnection(oCommand.Connection);
                }
            }
        }

        // Slechts 1 resultaat teruggeven
        public static Object ExecuteScalar(String sSQL, params OleDbParameter[] dbParams)
        {
            OleDbCommand oCommand = null;

            try
            {
                oCommand = BuildCommand(sSQL, dbParams);
                Object oObject = oCommand.ExecuteScalar();

                return oObject;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oCommand != null)
                {
                    ReleaseConnection(oCommand.Connection);
                }
            }
        }

        // SQL zonder terugkeer resultaat uitvoeren
        public static void ExcecuteSQL(String sSQL, params OleDbParameter[] dbParams)
        {
            OleDbCommand oCommand = null;

            try
            {
                oCommand = BuildCommand(sSQL, dbParams);
                oCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (oCommand != null)
                {
                    ReleaseConnection(oCommand.Connection);
                }
            }
        }
    }
}

