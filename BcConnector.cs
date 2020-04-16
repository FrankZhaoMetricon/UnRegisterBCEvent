using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessCraft;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace UnRegisterBCEvent
{
    public static class BcConnector
    {
        private static BusinessCraft.BsnWeb prv_Utils;
        private static BusinessCraft.Xf_authticket prv_AuthTicket;
        private static string prv_UserID;
        private static string prv_UserPW;
        private static string prv_UserInit;
        private static string prv_UserName;
        private static string prv_CompanyCode;
        private static string prv_CompanyPwd;
        private static decimal GstRate = 10m;

        private static bool hasConnected = false;


        public static void Connect()
        {

            string sErrTxt = "";
            string sWkstnName = "localhost";
            string sWkstnAddr = "127.0.0.1";
            int iWkstnSess = 0;
            int iWkstnConn = 0;
            int iSuccess = 0;
            string param = "";
            prv_UserInit = "SQS";


            try
            {
                prv_UserID = ConfigurationManager.AppSettings.Get("BC_USER_NAME");
                prv_UserPW = ConfigurationManager.AppSettings.Get("BC_PASSWORD");
                prv_CompanyCode = ConfigurationManager.AppSettings.Get("BC_COMPANY");
                prv_CompanyPwd = ConfigurationManager.AppSettings.Get("BC_COMPANY_PASSWORD");



                prv_Utils = new BsnWeb();
                prv_AuthTicket = new Xf_authticket();

                iSuccess = prv_Utils.qr_login(prv_UserID,
                    prv_UserPW,
                    ref prv_UserName,
                    ref prv_UserInit,
                    prv_CompanyCode,
                    prv_CompanyPwd,
                    sWkstnName,
                    sWkstnAddr,
                    ref iWkstnSess,
                    ref iWkstnConn,
                    ref prv_AuthTicket,
                    ref sErrTxt,
                    ref param);
                if (iSuccess == 0)
                {

                    hasConnected = true;
                    //Console.Write("Integration starts: Logged in to BC." + Environment.NewLine);
                    //MetriconCommon.LogToDatabase("Open BC connection", "", "Successful", "");
                }
            }
            catch (Exception e)
            {
                //try to disconnect
                Close();
                //MetriconCommon.LogToDatabase("Open BC connection", "", e.Message, "");
            }

        }

        public static bool Connected()
        {
            return hasConnected;
        }

        public static void Close()
        {
            hasConnected = false;
            if (prv_Utils != null)
            {
                string sErrTxt = "";
                try
                {
                    int iSuccess = 0;
                    iSuccess = prv_Utils.qr_logout(prv_AuthTicket, ref sErrTxt);

                    if (iSuccess == 0)
                    {
                        //Console.Write("Integration starts: Successfully logged out: " + Environment.NewLine);
                        //MetriconCommon.LogToDatabase("Close BC connection", "", "Successfully logged out: ", "");
                    }

                    prv_Utils.disconnect();
                }
                catch (Exception e)
                {
                    //Console.Write("Integration starts: Failed and logged out: " + Environment.NewLine);
                    //MetriconCommon.LogToDatabase("Close BC connection", "", e.Message, "");
                }
            }
        }

        public static int RegisterEvent(int contractNumber,
            int eventNumber,
            DateTime eventDate,
            decimal contractAmount,
            string userCode,
            out string errorMsg)
        {
            decimal gstAmount = 0m;
            decimal exGstAmount = 0m;
            int result = 0;
            if (contractAmount > 0)
            {
                //multiply by 100 so that there are 2 decimal points
                exGstAmount = Math.Round(contractAmount / (1 + (GstRate / 100)), 2);
                gstAmount = contractAmount - exGstAmount;
            }

            errorMsg = string.Empty;

            result = prv_Utils.qr_co_event_register(ref prv_AuthTicket,
                contractNumber,
                eventNumber,
                eventDate.ToString(),
                DateTime.Now.ToString(),
                eventDate.ToString(),
                Convert.ToInt64(exGstAmount * 100),
                Convert.ToInt64(gstAmount * 100),
                "",
                userCode,
                "G",
                0,
                ref errorMsg,
                "N");

            return result;
        }

        public static int UnRegisterEvent(int contractNumber,
                                            int eventNumber,
                                            out string errorMsg)
        {
            int result = 0;

            errorMsg = string.Empty;

            result = prv_Utils.qr_co_event_unregister(ref prv_AuthTicket,
                contractNumber,
                eventNumber,
                1,
                ref errorMsg);

            return result;
        }

        public static int ReforecastEvents(int contractNumber, out string errorMsg)
        {
            int result = 0;
            errorMsg = string.Empty;

            result = prv_Utils.qr_co_event_reforecast(ref prv_AuthTicket, contractNumber, ref errorMsg);

            return result;
        }

        public static int UpdateUnit(int contractNumber, int unit, out string errorMsg)
        {
            int result = 0;
            Conhdra c = new Conhdra();
            errorMsg = string.Empty;
            string prt_file = string.Empty;
            string contractNo = contractNumber.ToString();

            //mode 4 is reading the contract
            prv_Utils.qr_get_contract(ref prv_AuthTicket, 4, contractNumber.ToString(), ref c, ref errorMsg);
            c.Co_contract = contractNumber;
            c.Co_num_units = unit;

            result = prv_Utils.qr_co_contract(ref prv_AuthTicket, 2, ref contractNo, ref c, ref errorMsg, string.Empty, ref prt_file, string.Empty);

            return result;
        }

        public static int UpdateHome(int contractNumber, string home, out string errorMsg)
        {
            int result = 0;
            Conhdra c = new Conhdra();
            errorMsg = string.Empty;
            string prt_file = string.Empty;
            string contractNo = contractNumber.ToString();

            //mode 4 is reading the contract
            prv_Utils.qr_get_contract(ref prv_AuthTicket, 4, contractNumber.ToString(), ref c, ref errorMsg);
            c.Co_contract = contractNumber;
            c.Co_house = home;

            result = prv_Utils.qr_co_contract(ref prv_AuthTicket, 2, ref contractNo, ref c, ref errorMsg, string.Empty, ref prt_file, string.Empty);

            return result;
        }

        public static int CreateVariation(int contractNumber,
            ref int sequence,
            string description,
            string reference,
            string type,
            decimal variationAmount,
            string userCode,
            string registerEvents,
            out string errorMsg)
        {
            int result = 0;
            string para = string.Empty;
            string status = string.Empty;

            errorMsg = string.Empty;

            result = prv_Utils.qr_co_add_var(ref prv_AuthTicket,
                contractNumber,
                ref sequence,
                description,
                reference,
                variationAmount.ToString(),
                "Y",
                "0",
                "V",
                userCode,
                registerEvents,
                string.Empty,
                ref para,
                ref status,
                ref errorMsg);

            return result;
        }

        public static int UnregisterVariationEvent(int contractNumber,
            int sequence,
            int eventNumber,
            out string errorMsg)
        {
            int result = 0;

            errorMsg = string.Empty;

            result = prv_Utils.qr_co_var_event_unregister(ref prv_AuthTicket,
                contractNumber,
                sequence,
                eventNumber,
                1,
                ref errorMsg);
            return result;
        }

        public static int RegisterVariationEvent(int contractNumber,
            int sequence,
            int eventNumber,
            DateTime eventDate,
            string userCode,
            long amount,
            out string errorMsg)
        {
            int result = 0;

            errorMsg = string.Empty;

            result = prv_Utils.qr_co_var_event_register(ref prv_AuthTicket,
                contractNumber,
                sequence,
                eventNumber,
                eventDate.ToString(),
                amount,
                0,
                0,
                "",
                userCode,
                "G",
                ref errorMsg);
            return result;
        }
    }
}
