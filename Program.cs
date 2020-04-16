using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace UnRegisterBCEvent
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnector sqlConn = new SqlConnector();
            DataTable eventTable = sqlConn.GetEstimateEventRegister();
            string warningMessage = string.Empty;
            string errorMessage = string.Empty;
            int bcResult = 0;
            BcConnector.Connect();

            foreach (DataRow dr in eventTable.Rows)
            {
                int contractNumber = int.Parse(dr["contractNumber"].ToString());
                int eventNumber= int.Parse(dr["eventNumber"].ToString());

               // string message = sqlConn.UnregisterEventFromDataWarehouse(contractNumber, eventNumber);
               // Console.Write("Data Warehouse: Unregister contract " + contractNumber.ToString() + " Event " + eventNumber.ToString() + " Result: " + message + Environment.NewLine);

                bcResult = BcConnector.UnRegisterEvent(contractNumber, eventNumber, out errorMessage);
                if (bcResult != 0)
                {
                    if (bcResult == -9010 || bcResult == -9011) //Event does not exist (-9010) or event has already been registered (-9011)
                        warningMessage = errorMessage;
                    else if (bcResult == -9013) // ignore error on unregister but log it. throw new Exception("Cannot unregister event " + eventNumber.ToString() + " due to error.");
                        warningMessage = errorMessage + ", Cannot unregister event " + eventNumber.ToString() + " due to error.";
                    else
                        warningMessage = "Error " + bcResult.ToString() + " " + errorMessage; // throw new Exception("Error " + bcResult.ToString() + " " + errorMessage);

                    Console.Write("BC: Unregister contract " + contractNumber.ToString() + " Event " + eventNumber.ToString() + " Result: " + warningMessage + Environment.NewLine);

                }

            }

        }
    }
}
