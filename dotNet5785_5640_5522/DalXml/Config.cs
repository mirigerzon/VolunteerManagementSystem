namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
static internal class Config
{
    // Defines the XML file name for configuration data.
    internal const string s_data_config_xml = "data-config.xml";
    // Defines the XML file name for assignments data.
    internal const string s_Assignments_xml = "Assignments.xml";
    // Defines the XML file name for calls data.
    internal const string s_Calls_xml = "Calls.xml";
    // Defines the XML file name for volunteers data.
    internal const string s_Volunteers_xml = "Volunteers.xml";
    // Gets and increments the next available Assignment ID from the configuration.
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    // Gets and increments the next available Call ID from the configuration.
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }
    // Gets or sets the global clock value from the configuration.
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    // Gets or sets the risk range as a TimeSpan.
    internal static TimeSpan RiskRange
    {
        get;
        set;
    }
    // Resets the configuration values to their initial states.
    internal static void Reset()
    {
        NextAssignmentId = 0;
        NextCallId = 0;
        Clock = DateTime.Now;
    }
}