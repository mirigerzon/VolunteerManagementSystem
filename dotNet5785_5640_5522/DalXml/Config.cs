namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
static internal class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_Assignments_xml = "Assignments.xml";
    internal const string s_Calls_xml = "Calls.xml";
    internal const string s_Volunteers_xml = "Volunteers.xml";
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    internal static TimeSpan RiskRange
    {
        get;
        set;
    }
    internal static void Reset()
    {
        NextAssignmentId = 0;
        NextCallId = 0;
        Clock = DateTime.Now;
    }
}