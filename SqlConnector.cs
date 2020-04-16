using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace UnRegisterBCEvent
{
    public class SqlConnector
    {
        private string ConnectionString = string.Empty;
        private string DataWareHouseConnectionString = string.Empty;
        private SqlConnection connection = null;
        public SqlConnector()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["SqlConnection"].ToString();
            DataWareHouseConnectionString = ConfigurationManager.ConnectionStrings["DataWarehouseConnection"].ToString();
        }
        public DataTable GetEstimateEventRegister()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_unregisterevent_GetContracEvent";
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    conn.Close();
                }
            }
            return dt;
        }


        public string UnregisterEventFromDataWarehouse(int contractnumber, int eventnumber)
        {
            string result = "OK";
            string querystring = @"delete ev  
	                              from cdr.CONTRACT_EVENT ev
	                              inner join cdr.CONTRACT c on ev.cntr_evt_cntr_key=c.cntr_key
	                              WHERE c.cntr_number_bk_cde= '" + contractnumber.ToString()+ "' AND ev.cntr_evt_type_cde= '" + eventnumber.ToString()+"'";

            SqlCommand Cmd = null;
            DataSet ds = new DataSet();
            try
            {
                if (DataWareHouseConnectionString != null)
                {
                    connection = new SqlConnection(DataWareHouseConnectionString);
                    connection.Open();
                }
                Cmd = new SqlCommand(querystring, connection);
                Cmd.CommandTimeout = 1200;
                Cmd.CommandType = CommandType.Text;
                SqlDataAdapter Adapter = new SqlDataAdapter(Cmd);
                Adapter.Fill(ds);
                connection.Close();

                //System.Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public DataTable GetCustomerDocumentDetails(int estimateRevisionId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_SalesEstimate_GetCustomerDocumentDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@estimateRevisionId", estimateRevisionId));

                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    conn.Close();
                }
            }
            return dt;
        }

        public void UpdateBcEventProcessingQueue(int queueId, string errorMessage, bool success)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_SalesEstimate_UpdateBcEventProcessingQueue";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@queueId", queueId));
                    cmd.Parameters.Add(new SqlParameter("@errorMessage", errorMessage));
                    cmd.Parameters.Add(new SqlParameter("@success", success));

                    conn.Open();

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public DataTable GetEstimateHeader(int estimateRevisionId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_SalesEstimate_GetEstimateHeaderForEventIntegration";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@estimateRevisionId", estimateRevisionId));

                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    conn.Close();
                }
            }
            return dt;
        }

        public DataTable GetEstimateVariation(int estimateRevisionId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_SalesEstimate_GetEstimateVariationForEventIntegration";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@estimateRevisionId", estimateRevisionId));

                    conn.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    conn.Close();
                }
            }
            return dt;
        }

        public void PopulateBcEventProcessingQueue(int eventRegisterId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "sp_SalesEstimate_PopulateBcEventProcessingQueue";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@eventRegisterId", eventRegisterId));

                    conn.Open();

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }
    }
}
