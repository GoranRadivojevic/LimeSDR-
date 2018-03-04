//=================================================================
// database.cs
//=================================================================

using System;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace DatabaseEditor
{
	public class DB
    {
        #region enum

        public enum Band
        {
            FIRST = 0,
            GEN,
            B160M,
            B80M,
            B60M,
            B40M,
            B30M,
            B20M,
            B17M,
            B15M,
            B12M,
            B10M,
            B6M,
            B2M,
            WWV,
            B07M,
            VHF,
            B600M,
            B2190M,
            BX1,
            BX2,
            BX3,
            BX4,
            BX5,
            BX6,
            BX7,
            BX8,
            BX9,
            BX10,
            BX11,
            BX12,
            LAST,
        }

        public enum Model       // yt7pwr
        {
            GENESIS_G59USB = 1,
            GENESIS_G3020 = 2,
            GENESIS_G40 = 3,
            GENESIS_G80 = 4,
            GENESIS_G160 = 5,
            GENESIS_G59NET = 6,
            QRP2000 = 7,
            GENESIS_G137 = 9,
            GENESIS_G500 = 10,
            GENESIS_G11 = 11,
            GENESIS_G6 = 12,
        }

        #endregion

        #region variable

        public static DataSet ds;
        public static string FileName = "";

        #endregion

        #region Private Member Functions

        private static void AddIARU1BandTextTable()
        {
            #region IARU1

            ds.Tables.Add("IARU1BandText");
            DataTable t = ds.Tables["IARU1BandText"];
            t.Columns.Add("Low", typeof(double));
            t.Columns.Add("High", typeof(double));
            t.Columns.Add("Name", typeof(string));
            t.Columns.Add("TX", typeof(bool));

            object[] data = {
                                0.135700, 0.135999, "Trans atlantic window",    true,
                                0.136000, 0.137399, "CW",                       true,
                                0.137400, 0.137599, "Digital modes",            true,
                                0.137600, 0.137800, "Very slow telegraphy",     true,
                                0.415000, 0.524999, "600m band",                true,
								1.800000, 1.809999, "160M CW/Digital Modes",	true,
								1.810000, 1.810000, "160M CW QRP",				true,
								1.810001, 1.836599, "160M CW",					true,
                                1.836600, 1.836600, "160M WSPR",				true,
                                1.836601, 1.837999, "160M CW",					true,
                                1.838000, 1.838000, "160M JT65HF",  			true,
                                1.838001, 1.842999, "160M CW",					true,
								1.843000, 1.909999, "160M SSB/SSTV/Wide Band",	true,
								1.910000, 1.910000, "160M SSB QRP",				true,
								1.910001, 1.994999, "160M SSB/SSTV/Wide Band",	true,
								1.995000, 1.999999, "160M Experimental",		true,

								3.500000, 3.524999, "80M Extra CW",				true,
								3.525000, 3.575999, "80M CW",					true,
                                3.576000, 3.576000, "80M JT65HF",				true,
                                3.576001, 3.579999, "80M CW",					true,
								3.580000, 3.589999, "80M RTTY",					true,
								3.590000, 3.590000, "80M RTTY DX",				true,
								3.590001, 3.592599, "80M RTTY",					true,
                                3.592600, 3.592600, "80M WSPR",					true,
                                3.592601, 3.599999, "80M RTTY",					true,
								3.600000, 3.699999, "75M Extra SSB",			true,
								3.700000, 3.789999, "75M Ext/Adv SSB",			true,
								3.790000, 3.799999, "75M Ext/Adv DX Window",	true,
								3.800000, 3.844999, "75M SSB",					true,
								3.845000, 3.845000, "75M SSTV",					true,
								3.845001, 3.884999, "75M SSB",					true,
								3.885000, 3.885000, "75M AM Calling Frequency", true,
								3.885001, 3.999999, "75M SSB",					true,

                                5.287200, 5.287200, "60M WSPR",     			true,
								5.330500, 5.330500, "60M Channel 1",			true,
								5.346500, 5.346500, "60M Channel 2",			true,
								5.357000, 5.357000, "60M Channel 3",			true,
								5.371500, 5.371500, "60M Channel 4",			true,
								5.403500, 5.403500, "60M Channel 5",			true,
								
								7.000000, 7.024999, "40M Extra CW",				true,
								7.025000, 7.038599, "40M CW",					true,
                                7.038600, 7.038600, "40M WSPR",					true,
                                7.038601, 7.039999, "40M CW",					true,
								7.040000, 7.040000, "40M RTTY DX",				true,
								7.040001, 7.075999, "40M RTTY",					true,
                                7.076000, 7.076000, "40M JT65HF",				true,
                                7.076001, 7.099999, "40M RTTY",					true,
								7.100000, 7.124999, "40M CW",					true,
								7.125000, 7.170999, "40M Ext/Adv SSB",			true,
								7.171000, 7.171000, "40M SSTV",					true,
								7.171001, 7.174999, "40M Ext/Adv SSB",			true,
								7.175000, 7.289999, "40M SSB",					true,
								7.290000, 7.290000, "40M AM Calling Frequency", true,
								7.290001, 7.299999, "40M SSB",					true,
								
								10.100000, 10.129999, "30M CW",					true,
								10.130000, 10.138699, "30M RTTY",				true,
                                10.138700, 10.138700, "30M WSPR",				true,
                                10.138701, 10.139999, "30M RTTY",				true,
								10.140000, 10.149999, "30M Packet",				true,

								14.000000, 14.024999, "20M Extra CW",			true,
								14.025000, 14.069999, "20M CW",					true,
                                14.070000, 14.070000, "20M PSK",				true,
								14.070001, 14.075999, "20M RTTY",				true,
                                14.076000, 14.076000, "20M JT65HF",				true,
                                14.076001, 14.094999, "20M RTTY",				true,
								14.095000, 14.095599, "20M Packet",				true,
                                14.095600, 14.095600, "20M WSPR",				true,
                                14.095601, 14.099499, "20M Packet",				true,
								14.099500, 14.099999, "20M CW",					true,
								14.100000, 14.100000, "20M NCDXF Beacons",		true,
								14.100001, 14.100499, "20M CW",					true,
								14.100500, 14.111999, "20M Packet",				true,
								14.112000, 14.149999, "20M CW",					true,
								14.150000, 14.174999, "20M Extra SSB",			true,
								14.175000, 14.224999, "20M Ext/Adv SSB",		true,
								14.225000, 14.229999, "20M SSB",				true,
								14.230000, 14.230000, "20M SSTV",				true,
								14.230001, 14.285999, "20M SSB",				true,
								14.286000, 14.286000, "20M AM Calling Frequency", true,
								14.286001, 14.349999, "20M SSB",				true,
								
								18.068000, 18.099999, "17M CW",					true,
								18.100000, 18.104599, "17M RTTY",				true,
                                18.104600, 18.104600, "17M WSPR",				true,
                                18.104601, 18.104999, "17M RTTY",				true,
								18.105000, 18.109999, "17M Packet",				true,
								18.110000, 18.110000, "17M NCDXF Beacons",		true,
								18.110001, 18.167999, "17M SSB",				true,
								
								21.000000, 21.024999, "15M Extra CW",			true,
								21.025000, 21.069999, "15M CW",					true,
                                21.070000, 21.070000, "15M PSK",				true,
								21.070001, 21.075999, "15M RTTY",				true,
                                21.076000, 21.076000, "15M JT65HF",				true,
                                21.076001, 21.094599, "15M RTTY",				true,
                                21.094600, 21.094600, "15M WSPR",				true,
                                21.094601, 21.099999, "15M RTTY",				true,
								21.100000, 21.109999, "15M Packet",				true,
								21.110000, 21.149999, "15M CW",					true,
								21.150000, 21.150000, "15M NCDXF Beacons",		true,
								21.150001, 21.199999, "15M CW",					true,
								21.200000, 21.224999, "15M Extra SSB",			true,
								21.225000, 21.274999, "15M Ext/Adv SSB",		true,
								21.275000, 21.339999, "15M SSB",				true,
								21.340000, 21.340000, "15M SSTV",				true,
								21.340001, 21.449999, "15M SSB",				true,
								
								24.890000, 24.919999, "12M CW",					true,
								24.920000, 24.924599, "12M RTTY",				true,
                                24.924600, 24.924600, "12M WSPR",				true,
                                24.924601, 24.924999, "12M RTTY",				true,
								24.925000, 24.929999, "12M Packet",				true,
								24.930000, 24.930000, "12M NCDXF Beacons",		true,
								24.930001, 24.989999, "12M SSB",				true,
								
								28.000000, 28.069999, "10M CW",					true,
                                28.070000, 28.070000, "10M PSK",				true,
								28.070001, 28.075999, "10M RTTY",				true,
                                28.076000, 28.076000, "10M JT65HF",				true,
                                28.076001, 28.124599, "10M RTTY",				true,
                                28.124600, 28.124600, "10M WSPR",				true,
                                28.124601, 28.149999, "10M RTTY",				true,
								28.150000, 28.199999, "10M CW",					true,
								28.200000, 28.200000, "10M NCDXF Beacons",		true,
								28.200001, 28.299999, "10M Beacons",			true,
								28.300000, 28.679999, "10M SSB",				true,
								28.680000, 28.680000, "10M SSTV",				true,
								28.680001, 28.999999, "10M SSB",				true,
								29.000000, 29.199999, "10M AM",					true,
								29.200000, 29.299999, "10M SSB",				true,
								29.300000, 29.509999, "10M Satellite Downlinks", true,
								29.510000, 29.519999, "10M Deadband",			true,
								29.520000, 29.589999, "10M Repeater Inputs",	true,
								29.590000, 29.599999, "10M Deadband",			true,
								29.600000, 29.600000, "10M FM Simplex",			true,
								29.600001, 29.609999, "10M Deadband",			true,
								29.610000, 29.699999, "10M Repeater Outputs",	true,
								
								50.000000, 50.059999, "6M CW",					true,
								50.060000, 50.079999, "6M Beacon Sub-Band",		true,
								50.080000, 50.099999, "6M CW",					true,
								50.100000, 50.124999, "6M DX Window",			true,
								50.125000, 50.125000, "6M Calling Frequency",	true,
								50.125001, 50.292999, "6M SSB",					true,
                                50.293000, 50.293000, "6M WSPR",				true,
                                50.293001, 50.299999, "6M SSB",					true,
								50.300000, 50.599999, "6M All Modes",			true,
								50.600000, 50.619999, "6M Non Voice",			true,
								50.620000, 50.620000, "6M Digital Packet Calling", true,
								50.620001, 50.799999, "6M Non Voice",			true,
								50.800000, 50.999999, "6M RC",					true,
								51.000000, 51.099999, "6M Pacific DX Window",	true,
								51.100000, 51.119999, "6M Deadband",			true,
								51.120000, 51.179999, "6M Digital Repeater Inputs", true,
								51.180000, 51.479999, "6M Repeater Inputs",		true,
								51.480000, 51.619999, "6M Deadband",			true,
								51.620000, 51.679999, "6M Digital Repeater Outputs", true,
								51.680000, 51.979999, "6M Repeater Outputs",	true,
								51.980000, 51.999999, "6M Deadband",			true,
								52.000000, 52.019999, "6M Repeater Inputs",		true,
								52.020000, 52.020000, "6M FM Simplex",			true,
								52.020001, 52.039999, "6M Repeater Inputs",		true,
								52.040000, 52.040000, "6M FM Simplex",			true,
								52.040001, 52.479999, "6M Repeater Inputs",		true,
								52.480000, 52.499999, "6M Deadband",			true,
								52.500000, 52.524999, "6M Repeater Outputs",	true,
								52.525000, 52.525000, "6M Primary FM Simplex",	true,
								52.525001, 52.539999, "6M Deadband",			true,
								52.540000, 52.540000, "6M Secondary FM Simplex", true,
								52.540001, 52.979999, "6M Repeater Outputs",	true,
								52.980000, 52.999999, "6M Deadbands",			true,
								53.000000, 53.000000, "6M Remote Base FM Spx",	true,
								53.000001, 53.019999, "6M Repeater Inputs",		true,
								53.020000, 53.020000, "6M FM Simplex",			true,
								53.020001, 53.479999, "6M Repeater Inputs",		true,
								53.480000, 53.499999, "6M Deadband",			true,
								53.500000, 53.519999, "6M Repeater Outputs",	true,
								53.520000, 53.520000, "6M FM Simplex",			true,
								53.520001, 53.899999, "6M Repeater Outputs",	true,
								53.900000, 53.900000, "6M FM Simplex",			true,
								53.900010, 53.979999, "6M Repeater Outputs",	true,
								53.980000, 53.999999, "6M Deadband",			true,
								
                                70.000000, 70.089999, "4M CW Telegraphy",		true,
                                70.090000, 70.090499, "4M CW/Beacons",  		true,
                                70.090500, 70.091499, "4M WSPR beacons",  		true,
                                70.091500, 70.099999, "4M CW/Beacons",  		true,
                                70.100000, 70.184999, "4M CW/SSB",      		true,
                                70.185000, 70.199999, "4M Cross band calling", 	true,
                                70.200000, 70.249999, "4M CW/SSB",      		true,
                                70.250000, 70.250000, "4M MS calling",     		true,
                                70.250001, 70.259999, "4M All modes",      		true,
                                70.260000, 70.260000, "4M AM/FM calling",  		true,
                                70.260001, 70.269999, "4M All modes",      		true,
                                70.270000, 70.270000, "4M MGM activity centre",	true,
                                70.270001, 70.299999, "4M All modes",      		true,
                                70.300000, 70.300000, "4M RTTY/FAX",      		true,
                                70.300001, 70.311499, "4M FM 12.5KHz spacing",  true,
                                70.312500, 70.312500, "4M Digital",      		true,
                                70.312501, 70.324999, "4M FM 12.5KHz spacing",	true,
                                70.325000, 70.325000, "4M Digital",      		true,
                                70.322501, 70.449999, "4M FM 12.5KHz spacing",	true,
                                70.450000, 70.450000, "4M FM calling",      	true,
                                70.450001, 70.487499, "4M FM 12.5KHz spacing",	true,
                                70.487500, 70.487500, "4M Digital",         	true,
                                70.487501, 70.499999, "4M FM 12.5KHz spacing",	true,

								144.000000, 144.099999, "2M CW",				true,
								144.100000, 144.199999, "2M CW/SSB",			true,
								144.200000, 144.200000, "2M MS Calling",    	true,
								144.200001, 144.274999, "2M CW/SSB",			true,
								144.275000, 144.299999, "2M SSB Center",    	true,
								144.300000, 144.399999, "2M SSB",   			true,
                                144.400000, 144.487999, "2M Satellite",			true,
                                144.488000, 144.488000, "2M WSPR",  			true,
                                144.488001, 144.499999, "2M Satellite",			true,
								144.500000, 144.599999, "2M Linear Translator Inputs", true,
								144.600000, 144.899999, "2M FM Repeater",		true,
								144.900000, 145.199999, "2M FM Simplex",		true,
								145.200000, 145.499999, "2M FM Repeater",		true,
								145.500000, 145.799999, "2M FM Simplex",		true,
								145.800000, 145.999999, "2M Satellite",			true,
								146.000000, 146.399999, "2M FM Repeater",		true,
								146.400000, 146.609999, "2M FM Simplex",		true,
								146.610000, 147.389999, "2M FM Repeater",		true,
								147.390000, 147.599999, "2M FM Simplex",		true,
								147.600000, 147.999999, "2M FM Repeater",		true,

								222.000000, 222.024999, "1.25M EME/Weak Signal", true,
								222.025000, 222.049999, "1.25M Weak Signal",	true,
								222.050000, 222.059999, "1.25M Propagation Beacons", true,
								222.060000, 222.099999, "1.25M Weak Signal",	true,
								222.100000, 222.100000, "1.25M SSB/CW Calling",	true,
								222.100001, 222.149999, "1.25M Weak Signal CW/SSB", true,
								222.150000, 222.249999, "1.25M Local Option",	true,
								222.250000, 223.380000, "1.25M FM Repeater Inputs", true,
								222.380001, 223.399999, "1.25M General", true,
								223.400000, 223.519999, "1.25M FM Simplex",		true,
								223.520000, 223.639999, "1.25M Digital/Packet",	true,
								223.640000, 223.700000, "1.25M Links/Control",	true,
								223.700001, 223.709999, "1.25M General",	true,
								223.710000, 223.849999, "1.25M Local Option",	true,
								223.850000, 224.980000, "1.25M Repeater Outputs", true,

								420.000000, 425.999999, "70CM ATV Repeater",	true,
								426.000000, 431.999999, "70CM ATV Simplex",		true,
								432.000000, 432.069999, "70CM EME",				true,
								432.070000, 432.099999, "70CM Weak Signal CW",	true,
								432.100000, 432.100000, "70CM Calling Frequency", true,
								432.100001, 432.299999, "70CM Mixed Mode Weak Signal", true,
								432.300000, 432.399999, "70CM Propagation Beacons", true,
								432.400000, 432.999999, "70CM Mixed Mode Weak Signal", true,
								433.000000, 434.999999, "70CM Auxillary/Repeater Links", true,
								435.000000, 437.999999, "70CM Satellite Only",	true,
								438.000000, 441.999999, "70CM ATV Repeater",	true,
								442.000000, 444.999999, "70CM Local Repeaters",	true,
								445.000000, 445.999999, "70CM Local Option",	true,
								446.000000, 446.000000, "70CM Simplex",			true,
								446.000001, 446.999999, "70CM Local Option",	true,
								447.000000, 450.000000, "70CM Local Repeaters", true,

								902.000000, 902.099999, "33CM Weak Signal SSTV/FAX/ACSSB", true,
								902.100000, 902.100000, "33CM Weak Signal Calling", true,
								902.100001, 902.799999, "33CM Weak Signal SSTV/FAX/ACSSB", true,
								902.800000, 902.999999, "33CM Weak Signal EME/CW", true,
								903.000000, 903.099999, "33CM Digital Modes",	true,
								903.100000, 903.100000, "33CM Alternate Calling", true,
								903.100001, 905.999999, "33CM Digital Modes",	true,
								906.000000, 908.999999, "33CM FM Repeater Inputs", true,
								909.000000, 914.999999, "33CM ATV",				true,
								915.000000, 917.999999, "33CM Digital Modes",	true,
								918.000000, 920.999999, "33CM FM Repeater Outputs", true,
								921.000000, 926.999999, "33CM ATV",				true,
								927.000000, 928.000000, "33CM FM Simplex/Links", true,
								
								1240.000000, 1245.999999, "23CM ATV #1",		true,
								1246.000000, 1251.999999, "23CM FMN Point/Links", true,
								1252.000000, 1257.999999, "23CM ATV #2, Digital Modes", true,
								1258.000000, 1259.999999, "23CM FMN Point/Links", true,
								1260.000000, 1269.999999, "23CM Sat Uplinks/Wideband Exp.", true,
								1270.000000, 1275.999999, "23CM Repeater Inputs", true,
								1276.000000, 1281.999999, "23CM ATV #3",		true,
								1282.000000, 1287.999999, "23CM Repeater Outputs",	true,
								1288.000000, 1293.999999, "23CM Simplex ATV/Wideband Exp.", true,
								1294.000000, 1294.499999, "23CM Simplex FMN",		true,
								1294.500000, 1294.500000, "23CM FM Simplex Calling", true,
								1294.500001, 1294.999999, "23CM Simplex FMN",		true,
								1295.000000, 1295.799999, "23CM SSTV/FAX/ACSSB/Exp.", true,
								1295.800000, 1295.999999, "23CM EME/CW Expansion",	true,
								1296.000000, 1296.049999, "23CM EME Exclusive",		true,
								1296.050000, 1296.069999, "23CM Weak Signal",		true,
								1296.070000, 1296.079999, "23CM CW Beacons",		true,
								1296.080000, 1296.099999, "23CM Weak Signal",		true,
								1296.100000, 1296.100000, "23CM CW/SSB Calling",	true,
								1296.100001, 1296.399999, "23CM Weak Signal",		true,
								1296.400000, 1296.599999, "23CM X-Band Translator Input", true,
								1296.600000, 1296.799999, "23CM X-Band Translator Output", true,
								1296.800000, 1296.999999, "23CM Experimental Beacons", true,
								1297.000000, 1300.000000, "23CM Digital Modes",		true,

								2300.000000, 2302.999999, "2.3GHz High Data Rate", true,
								2303.000000, 2303.499999, "2.3GHz Packet",		true,
								2303.500000, 2303.800000, "2.3GHz TTY Packet",	true,
								2303.800001, 2303.899999, "2.3GHz General",	true,
								2303.900000, 2303.900000, "2.3GHz Packet/TTY/CW/EME", true,
								2303.900001, 2304.099999, "2.3GHz CW/EME",		true,
								2304.100000, 2304.100000, "2.3GHz Calling Frequency", true,
								2304.100001, 2304.199999, "2.3GHz CW/EME/SSB",	true,
								2304.200000, 2304.299999, "2.3GHz SSB/SSTV/FAX/Packet AM/Amtor", true,
								2304.300000, 2304.319999, "2.3GHz Propagation Beacon Network", true,
								2304.320000, 2304.399999, "2.3GHz General Propagation Beacons", true,
								2304.400000, 2304.499999, "2.3GHz SSB/SSTV/ACSSB/FAX/Packet AM", true,
								2304.500000, 2304.699999, "2.3GHz X-Band Translator Input", true,
								2304.700000, 2304.899999, "2.3GHz X-Band Translator Output", true,
								2304.900000, 2304.999999, "2.3GHz Experimental Beacons", true,
								2305.000000, 2305.199999, "2.3GHz FM Simplex", true,
								2305.200000, 2305.200000, "2.3GHz FM Simplex Calling", true,
								2305.200001, 2305.999999, "2.3GHz FM Simplex", true,
								2306.000000, 2308.999999, "2.3GHz FM Repeaters", true,
								2309.000000, 2310.000000, "2.3GHz Control/Aux Links", true,
								2390.000000, 2395.999999, "2.3GHz Fast-Scan TV", true,
								2396.000000, 2398.999999, "2.3GHz High Rate Data", true,
								2399.000000, 2399.499999, "2.3GHz Packet", true,
								2399.500000, 2399.999999, "2.3GHz Control/Aux Links", true,
								2400.000000, 2402.999999, "2.4GHz Satellite", true,
								2403.000000, 2407.999999, "2.4GHz Satellite High-Rate Data", true,
								2408.000000, 2409.999999, "2.4GHz Satellite", true,
								2410.000000, 2412.999999, "2.4GHz FM Repeaters", true,
								2413.000000, 2417.999999, "2.4GHz High-Rate Data", true,
								2418.000000, 2429.999999, "2.4GHz Fast-Scan TV", true,
								2430.000000, 2432.999999, "2.4GHz Satellite", true,
								2433.000000, 2437.999999, "2.4GHz Sat. High-Rate Data", true,
								2438.000000, 2450.000000, "2.4GHz Wideband FM/FSTV/FMTV", true,

								3456.000000, 3456.099999, "3.4GHz General", true,
								3456.100000, 3456.100000, "3.4GHz Calling Frequency", true,
								3456.100001, 3456.299999, "3.4GHz General", true,
								3456.300000, 3456.400000, "3.4GHz Propagation Beacons", true,

								5760.000000, 5760.099999, "5.7GHz General", true,
								5760.100000, 5760.100000, "5.7GHz Calling Frequency", true,
								5760.100001, 5760.299999, "5.7GHz General", true,
								5760.300000, 5760.400000, "5.7GHz Propagation Beacons", true,

								10368.000000, 10368.099999, "10GHz General", true,
								10368.100000, 10368.100000, "10GHz Calling Frequency", true,
								10368.100001, 10368.400000, "10GHz General", true,

								24192.000000, 24192.099999, "24GHz General", true,
								24192.100000, 24192.100000, "24GHz Calling Frequency", true,
								24192.100001, 24192.400000, "24GHz General", true,

								47088.000000, 47088.099999, "47GHz General", true,
								47088.100000, 47088.100000, "47GHz Calling Frequency", true,
								47088.100001, 47088.400000, "47GHz General", true,

								2.500000, 2.500000, "WWV",						false,
								5.000000, 5.000000, "WWV",						false,
								10.000000, 10.000000, "WWV",					false,
								15.000000, 15.000000, "WWV",					false,
								20.000000, 20.000000, "WWV",					false,

								0.525000, 1.710000, "Broadcast AM Med Wave",	false,				
								2.300000, 2.495000, "120M Short Wave",			false,
								3.200000, 3.400000, "90M Short Wave",			false,
								4.750000, 4.999999, "60M Short Wave",			false,
								5.000001, 5.060000, "60M Short Wave",			false,
								5.900000, 6.200000, "49M Short Wave",			false,
								7.300000, 7.350000, "41M Short Wave",			false,
								9.400000, 9.900000, "31M Short Wave",			false,
								11.600000, 12.100000, "25M Short Wave",			false,
								13.570000, 13.870000, "22M Short Wave",			false,
								15.100000, 15.800000, "19M Short Wave",			false,
								17.480000, 17.900000, "16M Short Wave",			false,
								18.900000, 19.020000, "15M Short Wave",			false,
								21.450000, 21.850000, "13M Short Wave",			false,
								25.600000, 26.100000, "11M Short Wave",			false,
			};

            for (int i = 0; i < data.Length / 4; i++)
            {
                DataRow dr = t.NewRow();
                dr["Low"] = (double)data[i * 4 + 0];
                dr["High"] = (double)data[i * 4 + 1];
                dr["Name"] = (string)data[i * 4 + 2];
                dr["TX"] = (bool)data[i * 4 + 3];
                t.Rows.Add(dr);
            }

            #endregion
        }

        private static void AddIARU2BandTextTable()
        {
            #region IARU 2

            ds.Tables.Add("IARU2BandText");
            DataTable t = ds.Tables["IARU2BandText"];
            t.Columns.Add("Low", typeof(double));
            t.Columns.Add("High", typeof(double));
            t.Columns.Add("Name", typeof(string));
            t.Columns.Add("TX", typeof(bool));

            object[] data = {
                                0.135700, 0.135999, "Trans atlantic window",    true,
                                0.136000, 0.137399, "CW",                       true,
                                0.137400, 0.137599, "Digital modes",            true,
                                0.137600, 0.137800, "Very slow telegraphy",     true,
                                0.415000, 0.524999, "600m band",                true,
								1.800000, 1.809999, "160M Digital Modes",	    true,
								1.810000, 1.811999, "160M CW QRP",				true,
                                1.812000, 1.812000, "160M CW QRP Center",		true,
								1.812001, 1.829999, "160M CW QRP",  			true,
                                1.830000, 1.836599, "160M CW DX",     			true,
                                1.836600, 1.836600, "160M WSPR",				true,
                                1.836601, 1.837999, "160M CW DX",     			true,
                                1.838000, 1.838000, "160M JT65HF",  			true,
                                1.838001, 1.839999, "160M CW DX",				true,
								1.840000, 1.849999, "160M SSB DX",          	true,
								1.850000, 1.909999, "160M All modes",			true,
								1.910000, 1.910000, "160M SSB QRP",         	true,
								1.910001, 1.999000, "160M All modes",	    	true,
                                1.999001, 1.999999, "160M Beacons",  			true,

								3.500000, 3.509999, "80M CW DX",				true,
                                3.510000, 3.554999, "80M CW Contest",			true,
                                3.555000, 3.555000, "80M CW QRS",				true,
								3.555001, 3.559999, "80M CW",					true,
                                3.560000, 3.560000, "80M CW QRP",				true,
                                3.560001, 3.575999, "80M CW",					true,
                                3.576000, 3.576000, "80M JT65HF",				true,
                                3.576001, 3.579999, "80M CW",					true,
								3.580000, 3.589999, "80M Digi",					true,
								3.590000, 3.592599, "80M All/Digi narrow",		true,
                                3.592600, 3.592600, "80M WSPR",					true,
                                3.592601, 3.599999, "80M All/Digi narrow",		true,
								3.600000, 3.625000, "80M All/Digi wide",		true,
								3.625001, 3.629999, "80M All/SSB contest",		true,
                                3.630000, 3.630000, "80M Digital voice",		true,
                                3.630001, 3.650000, "80M All/SSB contest",		true,
								3.650001, 3.689999, "80M All modes",        	true,
                                3.690000, 3.690000, "80M SSB QRP",          	true,
                                3.690001, 3.699999, "80M All modes",        	true,
								3.700000, 3.734999, "80M All/SSB contest",  	true,
                                3.735000, 3.735000, "80M Emergency",          	true,
                                3.735001, 3.775000, "80M All/SSB contest",  	true,
                                3.775001, 3.779999, "80M All/SSB DX",         	true,
                                3.800000, 3.875000, "80M All modes",          	true,
                                3.875001, 3.884999, "80M All modes",          	true,
                                3.885000, 3.885000, "80M AM center",          	true,
                                3.885001, 3.894999, "80M All modes",          	true,
                                3.895000, 3.895000, "80M Emergency",          	true,
                                3.895001, 3.999999, "80M All modes",          	true,
								
								7.000000, 7.024999, "40M CW DX",				true,
                                7.025000, 7.034999, "40M CW",					true,
                                7.035000, 7.035000, "40M CW QRP",				true,
								7.035001, 7.037999, "40M Digi/Narrow",  		true,
                                7.038000, 7.038599, "40M Digi/Narrow/Unattended",true,
                                7.038600, 7.038600, "40M WSPR",					true,
                                7.038601, 7.039999, "40M All/Digi/Narrow/Unattended",true,
                                7.040000, 7.043000, "40M All/Digi/Unattended",  true,
								7.043001, 7.059999, "40M All modes",			true,
                                7.076000, 7.076000, "40M JT65HF",				true,
                                7.060001, 7.089999, "40M All modes",			true,
                                7.090000, 7.090000, "40M SSB QRP center",   	true,
								7.090001, 7.100000, "40M All modes",			true,
								7.100001, 7.239999, "40M All modes",			true,
								7.240000, 7.240000, "40M emergency center 2",	true,
								7.240001, 7.274999, "40M All modes",			true,
                                7.275000, 7.275000, "40M emergency center 3",	true,
                                7.275001, 7.284999, "40M All modes",			true,
								7.285000, 7.285000, "40M SSB QRP center",   	true,
                                7.285001, 7.289999, "40M All modes",			true,
								7.290000, 7.290000, "40M AM Calling Frequency", true,
								7.290001, 7.299999, "40M All modes",			true,
								
								10.100000, 10.115999, "30M CW",					true,
                                10.116000, 10.116000, "30M CW QRP",				true,
                                10.116001, 10.129999, "30M CW",					true,
                                10.130001, 10.138699, "30M All narrow modes",	true,
                                10.138700, 10.138700, "30M WSPR",				true,
                                10.138701, 10.139999, "30M All narrow modes",	true,
                                10.140000, 10.149999, "30M All/Digi (no Phone)",true,

								14.000000, 14.024999, "20M CW DX",  			true,
								14.025000, 14.054999, "20M CW contest",			true,
                                14.055000, 14.055000, "20M CW QRS",				true,
                                14.055001, 14.059999, "20M CW contest",			true,
                                14.060000, 14.060000, "20M CW QRP",				true,
                                14.060001, 14.069999, "20M CW",					true,
                                14.070000, 14.070000, "20M PSK",				true,
								14.070001, 14.075999, "20M All/narrow/Digi",	true,
                                14.076000, 14.076000, "20M JT65HF",				true,
                                14.076001, 14.095599, "20M All/narrow/Digi",	true,
                                14.095600, 14.095600, "20M WSPR",				true,
                                14.095601, 14.098999, "20M All/narrow/Digi",	true,
                                14.999000, 14.101000, "20M IBP, Beacons",		true,
                                14.101001, 14.111999, "20M All/narrow/Digi",	true,
								14.112000, 14.129999, "20M All modes",			true,
                                14.130000, 14.130000, "20M Digital voice",  	true,
                                14.130001, 14.130000, "20M All modes",			true,
                                14.130001, 14.189999, "20M All modes",         	true,
                                14.112000, 14.189999, "20M All modes",			true,
                                14.190000, 14.200000, "20M SSB DX", 			true,
                                14.200001, 14.229999, "20M All modes",			true,
								14.230000, 14.230000, "20M SSTV",				true,
								14.230001, 14.284999, "20M All modes",  		true,
								14.285000, 14.285000, "20M SSB QRP",            true,
								14.285001, 14.285999, "20M All modes",			true,
                                14.286000, 14.286000, "20M AM Calling",	    	true,
                                14.286001, 14.299999, "20M All modes",			true,
                                14.300000, 14.300000, "20M Global emergency",	true,
                                14.300001, 14.349999, "20M All modes",			true,
								
								18.068000, 18.085999, "17M CW",					true,
                                18.086000, 18.086000, "17M CW QRP",				true,
                                18.086001, 18.094999, "17M CW",					true,
								18.095000, 18.104599, "17M All narrow/Digi",	true,
                                18.104600, 18.104600, "17M WSPR",				true,
                                18.104601, 18.108999, "17M All narrow/Digi",	true,
								18.109000, 18.110999, "17M IBP, Beacons",		true,
								18.111000, 18.119999, "17M All/Digi/unattended",true,
								18.120000, 18.129999, "17M All modes",		    true,
                                18.130000, 18.130000, "17M QRP center",			true,
                                18.130001, 18.159999, "17M All modes",			true,
                                18.160000, 18.160000, "17M Global emergency",   true,
                                18.160001, 18.167999, "17M All modes",			true,
								
								21.000000, 21.024999, "15M CW DX",  			true,
								21.025000, 21.054999, "15M CW",					true,
                                21.055000, 21.055000, "15M CW QRS", 			true,
                                21.055001, 21.059999, "15M CW",					true,
                                21.060000, 21.060000, "15M CW QRP",				true,
                                21.060001, 21.006999, "15M CW",					true,
                                21.070000, 21.070000, "15M PSK",				true,
								21.070001, 21.075999, "15M All narrow/Digi",	true,
                                21.076000, 21.076000, "15M JT65HF",				true,
                                21.076001, 21.094599, "15M All narrow/Digi",	true,
                                21.094600, 21.094600, "15M WSPR",				true,
                                21.094601, 21.110000, "15M All narrow/Digi",	true,
								21.110001, 21.120000, "15M All modes(no SSB)",	true,
								21.120001, 21.148999, "15M All narrow modes",   true,
								21.149000, 21.151000, "15M IBP, Beacons",		true,
								21.151001, 21.179999, "15M All modes",			true,
								21.180000, 21.180000, "15M Digital voice",	    true,
								21.180001, 21.284999, "15M All modes",		    true,
                                21.285000, 21.285000, "15M SSB QRP center",     true,
								21.285001, 21.339999, "15M All modes",			true,
								21.340000, 21.340000, "15M SSTV",				true,
                                21.340001, 21.359999, "15M All modes",		    true,
                                21.360000, 21.360000, "15M Global emergency",   true,
								21.360001, 21.449999, "15M All modes",			true,
								
								24.890000, 24.905999, "12M CW",					true,
                                24.906000, 24.906000, "12M CW QRP",				true,
                                24.906001, 24.914999, "12M CW",					true,
								24.915000, 24.924599, "12M All narrow/Digi",	true,
                                24.924600, 24.924600, "12M WSPR",				true,
                                24.924601, 24.928999, "12M All narrow/Digi",	true,
								24.929000, 24.931000, "12M IBP, Beacons",		true,
								24.931001, 24.939999, "12M All modes/Digi", 	true,
                                24.940000, 24.949999, "12M All modes",       	true,
                                24.950000, 24.950000, "12M SSB QRP",        	true,
                                24.950001, 24.989999, "12M All modes",       	true,
								
								28.000000, 28.054999, "10M CW",					true,
                                28.055000, 28.055000, "10M CW QRS", 			true,
                                28.055001, 28.059999, "10M CW",					true,
                                28.060000, 28.060000, "10M CW QRP",				true,
                                28.060001, 28.069999, "10M CW",					true,
                                28.070000, 28.070000, "10M PSK",				true,
								28.070001, 28.075999, "10M All narrow/Digi",	true,
                                28.076000, 28.076000, "10M JT65HF",				true,
                                28.076001, 28.120000, "10M All narrow/Digi",	true,
                                28.120001, 28.124599, "10M Narrow/Digi/unattended",	true,
                                28.124600, 28.124600, "10M WSPR",				true,
                                28.124601, 28.124999, "10M Narrow/Digi/unattended",	true,
								28.150000, 28.189999, "10M All narrow band modes",  true,
								28.190000, 28.198999, "10M Regional time shared Beacons", true,
								28.199000, 28.201000, "10M IBP, Beacons",		true,
                                28.201001, 28.224599, "10M Continuous duty beacons", true,
								28.225000, 28.300000, "10M All modes, Beacons",	true,
                                28.300001, 28.320000, "10M All/Digi/unattended",true,
                                28.320001, 28.329999, "10M All modes",      	true,
                                28.330000, 28.330000, "10M Digital voice",  	true,
                                28.330001, 28.359999, "10M All modes",      	true,
                                28.360000, 28.360000, "10M SSB QRP",          	true,
                                28.360001, 28.679999, "10M All modes",      	true,
								28.680000, 28.680000, "10M SSTV",				true,
								28.680001, 28.999999, "10M All modes",			true,
								29.000000, 29.199999, "10M All/AM preferred",	true,
								29.200000, 29.299999, "10M All modes wide",		true,
								29.300000, 29.509999, "10M Satellite Downlinks", true,
								29.510000, 29.519999, "10M Guard band",			false,
								29.520000, 29.589999, "10M FM Repeater Inputs",	true,
								29.590000, 29.599999, "10M FM",     			true,
								29.600000, 29.600000, "10M FM calling",			true,
								29.600001, 29.619999, "10M FM",     			true,
								29.620000, 29.699999, "10M Repeater Outputs",	true,
								
								50.000000, 50.059999, "6M CW",					true,
								50.060000, 50.079999, "6M Beacon Sub-Band",		true,
								50.080000, 50.099999, "6M CW",					true,
								50.100000, 50.124999, "6M DX Window",			true,
								50.125000, 50.125000, "6M Calling Frequency",	true,
								50.125001, 50.292999, "6M SSB",					true,
                                50.293000, 50.293000, "6M WSPR",				true,
                                50.293001, 50.299999, "6M SSB",					true,
								50.300000, 50.599999, "6M All Modes",			true,
								50.600000, 50.619999, "6M Non Voice",			true,
								50.620000, 50.620000, "6M Digital Packet Calling", true,
								50.620001, 50.799999, "6M Non Voice",			true,
								50.800000, 50.999999, "6M RC",					true,
								51.000000, 51.099999, "6M Pacific DX Window",	true,
								51.100000, 51.119999, "6M Deadband",			true,
								51.120000, 51.179999, "6M Digital Repeater Inputs", true,
								51.180000, 51.479999, "6M Repeater Inputs",		true,
								51.480000, 51.619999, "6M Deadband",			true,
								51.620000, 51.679999, "6M Digital Repeater Outputs", true,
								51.680000, 51.979999, "6M Repeater Outputs",	true,
								51.980000, 51.999999, "6M Deadband",			true,
								52.000000, 52.019999, "6M Repeater Inputs",		true,
								52.020000, 52.020000, "6M FM Simplex",			true,
								52.020001, 52.039999, "6M Repeater Inputs",		true,
								52.040000, 52.040000, "6M FM Simplex",			true,
								52.040001, 52.479999, "6M Repeater Inputs",		true,
								52.480000, 52.499999, "6M Deadband",			true,
								52.500000, 52.524999, "6M Repeater Outputs",	true,
								52.525000, 52.525000, "6M Primary FM Simplex",	true,
								52.525001, 52.539999, "6M Deadband",			true,
								52.540000, 52.540000, "6M Secondary FM Simplex", true,
								52.540001, 52.979999, "6M Repeater Outputs",	true,
								52.980000, 52.999999, "6M Deadbands",			true,
								53.000000, 53.000000, "6M Remote Base FM Spx",	true,
								53.000001, 53.019999, "6M Repeater Inputs",		true,
								53.020000, 53.020000, "6M FM Simplex",			true,
								53.020001, 53.479999, "6M Repeater Inputs",		true,
								53.480000, 53.499999, "6M Deadband",			true,
								53.500000, 53.519999, "6M Repeater Outputs",	true,
								53.520000, 53.520000, "6M FM Simplex",			true,
								53.520001, 53.899999, "6M Repeater Outputs",	true,
								53.900000, 53.900000, "6M FM Simplex",			true,
								53.900010, 53.979999, "6M Repeater Outputs",	true,
								53.980000, 53.999999, "6M Deadband",			true,
								
                                70.000000, 70.089999, "4M CW Telegraphy",		true,
                                70.090000, 70.090499, "4M CW/Beacons",  		true,
                                70.090500, 70.091499, "4M WSPR beacons",  		true,
                                70.091500, 70.099999, "4M CW/Beacons",  		true,
                                70.100000, 70.184999, "4M CW/SSB",      		true,
                                70.185000, 70.199999, "4M Cross band calling", 	true,
                                70.200000, 70.249999, "4M CW/SSB",      		true,
                                70.250000, 70.250000, "4M MS calling",     		true,
                                70.250001, 70.259999, "4M All modes",      		true,
                                70.260000, 70.260000, "4M AM/FM calling",  		true,
                                70.260001, 70.269999, "4M All modes",      		true,
                                70.270000, 70.270000, "4M MGM activity centre",	true,
                                70.270001, 70.299999, "4M All modes",      		true,
                                70.300000, 70.300000, "4M RTTY/FAX",      		true,
                                70.300001, 70.311499, "4M FM 12.5KHz spacing",  true,
                                70.312500, 70.312500, "4M Digital",      		true,
                                70.312501, 70.324999, "4M FM 12.5KHz spacing",	true,
                                70.325000, 70.325000, "4M Digital",      		true,
                                70.322501, 70.449999, "4M FM 12.5KHz spacing",	true,
                                70.450000, 70.450000, "4M FM calling",      	true,
                                70.450001, 70.487499, "4M FM 12.5KHz spacing",	true,
                                70.487500, 70.487500, "4M Digital",         	true,
                                70.487501, 70.499999, "4M FM 12.5KHz spacing",	true,

								144.000000, 144.099999, "2M CW",				true,
								144.100000, 144.199999, "2M CW/SSB",			true,
								144.200000, 144.200000, "2M MS Calling",    	true,
								144.200001, 144.274999, "2M CW/SSB",			true,
								144.275000, 144.299999, "2M SSB Center",    	true,
								144.300000, 144.399999, "2M SSB",   			true,
                                144.400000, 144.487999, "2M Satellite",			true,
                                144.488000, 144.488000, "2M WSPR",  			true,
                                144.488001, 144.499999, "2M Satellite",			true,
								144.500000, 144.599999, "2M Linear Translator Inputs", true,
								144.600000, 144.899999, "2M FM Repeater",		true,
								144.900000, 145.199999, "2M FM Simplex",		true,
								145.200000, 145.499999, "2M FM Repeater",		true,
								145.500000, 145.799999, "2M FM Simplex",		true,
								145.800000, 145.999999, "2M Satellite",			true,
								146.000000, 146.399999, "2M FM Repeater",		true,
								146.400000, 146.609999, "2M FM Simplex",		true,
								146.610000, 147.389999, "2M FM Repeater",		true,
								147.390000, 147.599999, "2M FM Simplex",		true,
								147.600000, 147.999999, "2M FM Repeater",		true,

								222.000000, 222.024999, "1.25M EME/Weak Signal", true,
								222.025000, 222.049999, "1.25M Weak Signal",	true,
								222.050000, 222.059999, "1.25M Propagation Beacons", true,
								222.060000, 222.099999, "1.25M Weak Signal",	true,
								222.100000, 222.100000, "1.25M SSB/CW Calling",	true,
								222.100001, 222.149999, "1.25M Weak Signal CW/SSB", true,
								222.150000, 222.249999, "1.25M Local Option",	true,
								222.250000, 223.380000, "1.25M FM Repeater Inputs", true,
								222.380001, 223.399999, "1.25M General", true,
								223.400000, 223.519999, "1.25M FM Simplex",		true,
								223.520000, 223.639999, "1.25M Digital/Packet",	true,
								223.640000, 223.700000, "1.25M Links/Control",	true,
								223.700001, 223.709999, "1.25M General",	true,
								223.710000, 223.849999, "1.25M Local Option",	true,
								223.850000, 224.980000, "1.25M Repeater Outputs", true,

								420.000000, 425.999999, "70CM ATV Repeater",	true,
								426.000000, 431.999999, "70CM ATV Simplex",		true,
								432.000000, 432.069999, "70CM EME",				true,
								432.070000, 432.099999, "70CM Weak Signal CW",	true,
								432.100000, 432.100000, "70CM Calling Frequency", true,
								432.100001, 432.299999, "70CM Mixed Mode Weak Signal", true,
								432.300000, 432.399999, "70CM Propagation Beacons", true,
								432.400000, 432.999999, "70CM Mixed Mode Weak Signal", true,
								433.000000, 434.999999, "70CM Auxillary/Repeater Links", true,
								435.000000, 437.999999, "70CM Satellite Only",	true,
								438.000000, 441.999999, "70CM ATV Repeater",	true,
								442.000000, 444.999999, "70CM Local Repeaters",	true,
								445.000000, 445.999999, "70CM Local Option",	true,
								446.000000, 446.000000, "70CM Simplex",			true,
								446.000001, 446.999999, "70CM Local Option",	true,
								447.000000, 450.000000, "70CM Local Repeaters", true,

								902.000000, 902.099999, "33CM Weak Signal SSTV/FAX/ACSSB", true,
								902.100000, 902.100000, "33CM Weak Signal Calling", true,
								902.100001, 902.799999, "33CM Weak Signal SSTV/FAX/ACSSB", true,
								902.800000, 902.999999, "33CM Weak Signal EME/CW", true,
								903.000000, 903.099999, "33CM Digital Modes",	true,
								903.100000, 903.100000, "33CM Alternate Calling", true,
								903.100001, 905.999999, "33CM Digital Modes",	true,
								906.000000, 908.999999, "33CM FM Repeater Inputs", true,
								909.000000, 914.999999, "33CM ATV",				true,
								915.000000, 917.999999, "33CM Digital Modes",	true,
								918.000000, 920.999999, "33CM FM Repeater Outputs", true,
								921.000000, 926.999999, "33CM ATV",				true,
								927.000000, 928.000000, "33CM FM Simplex/Links", true,
								
								1240.000000, 1245.999999, "23CM ATV #1",		true,
								1246.000000, 1251.999999, "23CM FMN Point/Links", true,
								1252.000000, 1257.999999, "23CM ATV #2, Digital Modes", true,
								1258.000000, 1259.999999, "23CM FMN Point/Links", true,
								1260.000000, 1269.999999, "23CM Sat Uplinks/Wideband Exp.", true,
								1270.000000, 1275.999999, "23CM Repeater Inputs", true,
								1276.000000, 1281.999999, "23CM ATV #3",		true,
								1282.000000, 1287.999999, "23CM Repeater Outputs",	true,
								1288.000000, 1293.999999, "23CM Simplex ATV/Wideband Exp.", true,
								1294.000000, 1294.499999, "23CM Simplex FMN",		true,
								1294.500000, 1294.500000, "23CM FM Simplex Calling", true,
								1294.500001, 1294.999999, "23CM Simplex FMN",		true,
								1295.000000, 1295.799999, "23CM SSTV/FAX/ACSSB/Exp.", true,
								1295.800000, 1295.999999, "23CM EME/CW Expansion",	true,
								1296.000000, 1296.049999, "23CM EME Exclusive",		true,
								1296.050000, 1296.069999, "23CM Weak Signal",		true,
								1296.070000, 1296.079999, "23CM CW Beacons",		true,
								1296.080000, 1296.099999, "23CM Weak Signal",		true,
								1296.100000, 1296.100000, "23CM CW/SSB Calling",	true,
								1296.100001, 1296.399999, "23CM Weak Signal",		true,
								1296.400000, 1296.599999, "23CM X-Band Translator Input", true,
								1296.600000, 1296.799999, "23CM X-Band Translator Output", true,
								1296.800000, 1296.999999, "23CM Experimental Beacons", true,
								1297.000000, 1300.000000, "23CM Digital Modes",		true,

								2300.000000, 2302.999999, "2.3GHz High Data Rate", true,
								2303.000000, 2303.499999, "2.3GHz Packet",		true,
								2303.500000, 2303.800000, "2.3GHz TTY Packet",	true,
								2303.800001, 2303.899999, "2.3GHz General",	true,
								2303.900000, 2303.900000, "2.3GHz Packet/TTY/CW/EME", true,
								2303.900001, 2304.099999, "2.3GHz CW/EME",		true,
								2304.100000, 2304.100000, "2.3GHz Calling Frequency", true,
								2304.100001, 2304.199999, "2.3GHz CW/EME/SSB",	true,
								2304.200000, 2304.299999, "2.3GHz SSB/SSTV/FAX/Packet AM/Amtor", true,
								2304.300000, 2304.319999, "2.3GHz Propagation Beacon Network", true,
								2304.320000, 2304.399999, "2.3GHz General Propagation Beacons", true,
								2304.400000, 2304.499999, "2.3GHz SSB/SSTV/ACSSB/FAX/Packet AM", true,
								2304.500000, 2304.699999, "2.3GHz X-Band Translator Input", true,
								2304.700000, 2304.899999, "2.3GHz X-Band Translator Output", true,
								2304.900000, 2304.999999, "2.3GHz Experimental Beacons", true,
								2305.000000, 2305.199999, "2.3GHz FM Simplex", true,
								2305.200000, 2305.200000, "2.3GHz FM Simplex Calling", true,
								2305.200001, 2305.999999, "2.3GHz FM Simplex", true,
								2306.000000, 2308.999999, "2.3GHz FM Repeaters", true,
								2309.000000, 2310.000000, "2.3GHz Control/Aux Links", true,
								2390.000000, 2395.999999, "2.3GHz Fast-Scan TV", true,
								2396.000000, 2398.999999, "2.3GHz High Rate Data", true,
								2399.000000, 2399.499999, "2.3GHz Packet", true,
								2399.500000, 2399.999999, "2.3GHz Control/Aux Links", true,
								2400.000000, 2402.999999, "2.4GHz Satellite", true,
								2403.000000, 2407.999999, "2.4GHz Satellite High-Rate Data", true,
								2408.000000, 2409.999999, "2.4GHz Satellite", true,
								2410.000000, 2412.999999, "2.4GHz FM Repeaters", true,
								2413.000000, 2417.999999, "2.4GHz High-Rate Data", true,
								2418.000000, 2429.999999, "2.4GHz Fast-Scan TV", true,
								2430.000000, 2432.999999, "2.4GHz Satellite", true,
								2433.000000, 2437.999999, "2.4GHz Sat. High-Rate Data", true,
								2438.000000, 2450.000000, "2.4GHz Wideband FM/FSTV/FMTV", true,

								3456.000000, 3456.099999, "3.4GHz General", true,
								3456.100000, 3456.100000, "3.4GHz Calling Frequency", true,
								3456.100001, 3456.299999, "3.4GHz General", true,
								3456.300000, 3456.400000, "3.4GHz Propagation Beacons", true,

								5760.000000, 5760.099999, "5.7GHz General", true,
								5760.100000, 5760.100000, "5.7GHz Calling Frequency", true,
								5760.100001, 5760.299999, "5.7GHz General", true,
								5760.300000, 5760.400000, "5.7GHz Propagation Beacons", true,

								10368.000000, 10368.099999, "10GHz General", true,
								10368.100000, 10368.100000, "10GHz Calling Frequency", true,
								10368.100001, 10368.400000, "10GHz General", true,

								24192.000000, 24192.099999, "24GHz General", true,
								24192.100000, 24192.100000, "24GHz Calling Frequency", true,
								24192.100001, 24192.400000, "24GHz General", true,

								47088.000000, 47088.099999, "47GHz General", true,
								47088.100000, 47088.100000, "47GHz Calling Frequency", true,
								47088.100001, 47088.400000, "47GHz General", true,

								2.500000, 2.500000, "WWV",						false,
								5.000000, 5.000000, "WWV",						false,
								10.000000, 10.000000, "WWV",					false,
								15.000000, 15.000000, "WWV",					false,
								20.000000, 20.000000, "WWV",					false,

								0.525000, 1.710000, "Broadcast AM Med Wave",	false,				
								2.300000, 2.495000, "120M Short Wave",			false,
								3.200000, 3.400000, "90M Short Wave",			false,
								4.750000, 4.999999, "60M Short Wave",			false,
								5.000001, 5.060000, "60M Short Wave",			false,
								5.900000, 6.200000, "49M Short Wave",			false,
								7.300000, 7.350000, "41M Short Wave",			false,
								9.400000, 9.900000, "31M Short Wave",			false,
								11.600000, 12.100000, "25M Short Wave",			false,
								13.570000, 13.870000, "22M Short Wave",			false,
								15.100000, 15.800000, "19M Short Wave",			false,
								17.480000, 17.900000, "16M Short Wave",			false,
								18.900000, 19.020000, "15M Short Wave",			false,
								21.450000, 21.850000, "13M Short Wave",			false,
								25.600000, 26.100000, "11M Short Wave",			false,
			};

            for (int i = 0; i < data.Length / 4; i++)
            {
                DataRow dr = t.NewRow();
                dr["Low"] = (double)data[i * 4 + 0];
                dr["High"] = (double)data[i * 4 + 1];
                dr["Name"] = (string)data[i * 4 + 2];
                dr["TX"] = (bool)data[i * 4 + 3];
                t.Rows.Add(dr);
            }

            #endregion
        }

        private static void AddIARU3BandTextTable()
        {
            #region IARU 3

            ds.Tables.Add("IARU3BandText");
            DataTable t = ds.Tables["IARU3BandText"];
            t.Columns.Add("Low", typeof(double));
            t.Columns.Add("High", typeof(double));
            t.Columns.Add("Name", typeof(string));
            t.Columns.Add("TX", typeof(bool));

            object[] data = {
                                0.135700, 0.135999, "Trans atlantic window",    true,
                                0.136000, 0.137399, "CW",                       true,
                                0.137400, 0.137599, "Digital modes",            true,
                                0.137600, 0.137800, "Very slow telegraphy",     true,
                                0.415000, 0.524999, "600m band",                true,
								1.800000, 1.809999, "160M CW/Digital Modes",	true,
								1.810000, 1.810000, "160M CW QRP",				true,
								1.810001, 1.836599, "160M CW",					true,
                                1.836600, 1.836600, "160M WSPR",				true,
                                1.836601, 1.837999, "160M CW",					true,
                                1.838000, 1.838000, "160M JT65HF",  			true,
                                1.838001, 1.842999, "160M CW",					true,
								1.843000, 1.909999, "160M SSB/SSTV/Wide Band",	true,
								1.910000, 1.910000, "160M SSB QRP",				true,
								1.910001, 1.994999, "160M SSB/SSTV/Wide Band",	true,
								1.995000, 1.999999, "160M Experimental",		true,

								3.500000, 3.524999, "80M Extra CW",				true,
								3.525000, 3.575999, "80M CW",					true,
                                3.576000, 3.576000, "80M JT65HF",				true,
                                3.576001, 3.579999, "80M CW",					true,
								3.580000, 3.589999, "80M RTTY",					true,
								3.590000, 3.590000, "80M RTTY DX",				true,
								3.590001, 3.592599, "80M RTTY",					true,
                                3.592600, 3.592600, "80M WSPR",					true,
                                3.592601, 3.599999, "80M RTTY",					true,
								3.600000, 3.699999, "75M Extra SSB",			true,
								3.700000, 3.789999, "75M Ext/Adv SSB",			true,
								3.790000, 3.799999, "75M Ext/Adv DX Window",	true,
								3.800000, 3.844999, "75M SSB",					true,
								3.845000, 3.845000, "75M SSTV",					true,
								3.845001, 3.884999, "75M SSB",					true,
								3.885000, 3.885000, "75M AM Calling Frequency", true,
								3.885001, 3.999999, "75M SSB",					true,

                                5.287200, 5.287200, "60M WSPR",     			true,
								5.330500, 5.330500, "60M Channel 1",			true,
								5.346500, 5.346500, "60M Channel 2",			true,
								5.357000, 5.357000, "60M Channel 3",			true,
								5.371500, 5.371500, "60M Channel 4",			true,
								5.403500, 5.403500, "60M Channel 5",			true,
								
								7.000000, 7.024999, "40M Extra CW",				true,
								7.025000, 7.038599, "40M CW",					true,
                                7.038600, 7.038600, "40M WSPR",					true,
                                7.038601, 7.039999, "40M CW",					true,
								7.040000, 7.040000, "40M RTTY DX",				true,
								7.040001, 7.075999, "40M RTTY",					true,
                                7.076000, 7.076000, "40M JT65HF",				true,
                                7.076001, 7.099999, "40M RTTY",					true,
								7.100000, 7.124999, "40M CW",					true,
								7.125000, 7.170999, "40M Ext/Adv SSB",			true,
								7.171000, 7.171000, "40M SSTV",					true,
								7.171001, 7.174999, "40M Ext/Adv SSB",			true,
								7.175000, 7.289999, "40M SSB",					true,
								7.290000, 7.290000, "40M AM Calling Frequency", true,
								7.290001, 7.299999, "40M SSB",					true,
								
								10.100000, 10.129999, "30M CW",					true,
								10.130000, 10.138699, "30M RTTY",				true,
                                10.138700, 10.138700, "30M WSPR",				true,
                                10.138701, 10.139999, "30M RTTY",				true,
								10.140000, 10.149999, "30M Packet",				true,

								14.000000, 14.024999, "20M Extra CW",			true,
								14.025000, 14.069999, "20M CW",					true,
                                14.070000, 14.070000, "20M PSK",				true,
								14.070001, 14.075999, "20M RTTY",				true,
                                14.076000, 14.076000, "20M JT65HF",				true,
                                14.076001, 14.094999, "20M RTTY",				true,
								14.095000, 14.095599, "20M Packet",				true,
                                14.095600, 14.095600, "20M WSPR",				true,
                                14.095601, 14.099499, "20M Packet",				true,
								14.099500, 14.099999, "20M CW",					true,
								14.100000, 14.100000, "20M NCDXF Beacons",		true,
								14.100001, 14.100499, "20M CW",					true,
								14.100500, 14.111999, "20M Packet",				true,
								14.112000, 14.149999, "20M CW",					true,
								14.150000, 14.174999, "20M Extra SSB",			true,
								14.175000, 14.224999, "20M Ext/Adv SSB",		true,
								14.225000, 14.229999, "20M SSB",				true,
								14.230000, 14.230000, "20M SSTV",				true,
								14.230001, 14.285999, "20M SSB",				true,
								14.286000, 14.286000, "20M AM Calling Frequency", true,
								14.286001, 14.349999, "20M SSB",				true,
								
								18.068000, 18.099999, "17M CW",					true,
								18.100000, 18.104599, "17M RTTY",				true,
                                18.104600, 18.104600, "17M WSPR",				true,
                                18.104601, 18.104999, "17M RTTY",				true,
								18.105000, 18.109999, "17M Packet",				true,
								18.110000, 18.110000, "17M NCDXF Beacons",		true,
								18.110001, 18.167999, "17M SSB",				true,
								
								21.000000, 21.024999, "15M Extra CW",			true,
								21.025000, 21.069999, "15M CW",					true,
                                21.070000, 21.070000, "15M PSK",				true,
								21.070001, 21.075999, "15M RTTY",				true,
                                21.076000, 21.076000, "15M JT65HF",				true,
                                21.076001, 21.094599, "15M RTTY",				true,
                                21.094600, 21.094600, "15M WSPR",				true,
                                21.094601, 21.099999, "15M RTTY",				true,
								21.100000, 21.109999, "15M Packet",				true,
								21.110000, 21.149999, "15M CW",					true,
								21.150000, 21.150000, "15M NCDXF Beacons",		true,
								21.150001, 21.199999, "15M CW",					true,
								21.200000, 21.224999, "15M Extra SSB",			true,
								21.225000, 21.274999, "15M Ext/Adv SSB",		true,
								21.275000, 21.339999, "15M SSB",				true,
								21.340000, 21.340000, "15M SSTV",				true,
								21.340001, 21.449999, "15M SSB",				true,
								
								24.890000, 24.919999, "12M CW",					true,
								24.920000, 24.924599, "12M RTTY",				true,
                                24.924600, 24.924600, "12M WSPR",				true,
                                24.924601, 24.924999, "12M RTTY",				true,
								24.925000, 24.929999, "12M Packet",				true,
								24.930000, 24.930000, "12M NCDXF Beacons",		true,
								24.930001, 24.989999, "12M SSB",				true,
								
								28.000000, 28.069999, "10M CW",					true,
                                28.070000, 28.070000, "10M PSK",				true,
								28.070001, 28.075999, "10M RTTY",				true,
                                28.076000, 28.076000, "10M JT65HF",				true,
                                28.076001, 28.124599, "10M RTTY",				true,
                                28.124600, 28.124600, "10M WSPR",				true,
                                28.124601, 28.149999, "10M RTTY",				true,
								28.150000, 28.199999, "10M CW",					true,
								28.200000, 28.200000, "10M NCDXF Beacons",		true,
								28.200001, 28.299999, "10M Beacons",			true,
								28.300000, 28.679999, "10M SSB",				true,
								28.680000, 28.680000, "10M SSTV",				true,
								28.680001, 28.999999, "10M SSB",				true,
								29.000000, 29.199999, "10M AM",					true,
								29.200000, 29.299999, "10M SSB",				true,
								29.300000, 29.509999, "10M Satellite Downlinks", true,
								29.510000, 29.519999, "10M Deadband",			true,
								29.520000, 29.589999, "10M Repeater Inputs",	true,
								29.590000, 29.599999, "10M Deadband",			true,
								29.600000, 29.600000, "10M FM Simplex",			true,
								29.600001, 29.609999, "10M Deadband",			true,
								29.610000, 29.699999, "10M Repeater Outputs",	true,
								
								50.000000, 50.059999, "6M CW",					true,
								50.060000, 50.079999, "6M Beacon Sub-Band",		true,
								50.080000, 50.099999, "6M CW",					true,
								50.100000, 50.124999, "6M DX Window",			true,
								50.125000, 50.125000, "6M Calling Frequency",	true,
								50.125001, 50.292999, "6M SSB",					true,
                                50.293000, 50.293000, "6M WSPR",				true,
                                50.293001, 50.299999, "6M SSB",					true,
								50.300000, 50.599999, "6M All Modes",			true,
								50.600000, 50.619999, "6M Non Voice",			true,
								50.620000, 50.620000, "6M Digital Packet Calling", true,
								50.620001, 50.799999, "6M Non Voice",			true,
								50.800000, 50.999999, "6M RC",					true,
								51.000000, 51.099999, "6M Pacific DX Window",	true,
								51.100000, 51.119999, "6M Deadband",			true,
								51.120000, 51.179999, "6M Digital Repeater Inputs", true,
								51.180000, 51.479999, "6M Repeater Inputs",		true,
								51.480000, 51.619999, "6M Deadband",			true,
								51.620000, 51.679999, "6M Digital Repeater Outputs", true,
								51.680000, 51.979999, "6M Repeater Outputs",	true,
								51.980000, 51.999999, "6M Deadband",			true,
								52.000000, 52.019999, "6M Repeater Inputs",		true,
								52.020000, 52.020000, "6M FM Simplex",			true,
								52.020001, 52.039999, "6M Repeater Inputs",		true,
								52.040000, 52.040000, "6M FM Simplex",			true,
								52.040001, 52.479999, "6M Repeater Inputs",		true,
								52.480000, 52.499999, "6M Deadband",			true,
								52.500000, 52.524999, "6M Repeater Outputs",	true,
								52.525000, 52.525000, "6M Primary FM Simplex",	true,
								52.525001, 52.539999, "6M Deadband",			true,
								52.540000, 52.540000, "6M Secondary FM Simplex", true,
								52.540001, 52.979999, "6M Repeater Outputs",	true,
								52.980000, 52.999999, "6M Deadbands",			true,
								53.000000, 53.000000, "6M Remote Base FM Spx",	true,
								53.000001, 53.019999, "6M Repeater Inputs",		true,
								53.020000, 53.020000, "6M FM Simplex",			true,
								53.020001, 53.479999, "6M Repeater Inputs",		true,
								53.480000, 53.499999, "6M Deadband",			true,
								53.500000, 53.519999, "6M Repeater Outputs",	true,
								53.520000, 53.520000, "6M FM Simplex",			true,
								53.520001, 53.899999, "6M Repeater Outputs",	true,
								53.900000, 53.900000, "6M FM Simplex",			true,
								53.900010, 53.979999, "6M Repeater Outputs",	true,
								53.980000, 53.999999, "6M Deadband",			true,
								
                                70.000000, 70.089999, "4M CW Telegraphy",		true,
                                70.090000, 70.090499, "4M CW/Beacons",  		true,
                                70.090500, 70.091499, "4M WSPR beacons",  		true,
                                70.091500, 70.099999, "4M CW/Beacons",  		true,
                                70.100000, 70.184999, "4M CW/SSB",      		true,
                                70.185000, 70.199999, "4M Cross band calling", 	true,
                                70.200000, 70.249999, "4M CW/SSB",      		true,
                                70.250000, 70.250000, "4M MS calling",     		true,
                                70.250001, 70.259999, "4M All modes",      		true,
                                70.260000, 70.260000, "4M AM/FM calling",  		true,
                                70.260001, 70.269999, "4M All modes",      		true,
                                70.270000, 70.270000, "4M MGM activity centre",	true,
                                70.270001, 70.299999, "4M All modes",      		true,
                                70.300000, 70.300000, "4M RTTY/FAX",      		true,
                                70.300001, 70.311499, "4M FM 12.5KHz spacing",  true,
                                70.312500, 70.312500, "4M Digital",      		true,
                                70.312501, 70.324999, "4M FM 12.5KHz spacing",	true,
                                70.325000, 70.325000, "4M Digital",      		true,
                                70.322501, 70.449999, "4M FM 12.5KHz spacing",	true,
                                70.450000, 70.450000, "4M FM calling",      	true,
                                70.450001, 70.487499, "4M FM 12.5KHz spacing",	true,
                                70.487500, 70.487500, "4M Digital",         	true,
                                70.487501, 70.499999, "4M FM 12.5KHz spacing",	true,

								144.000000, 144.099999, "2M CW",				true,
								144.100000, 144.199999, "2M CW/SSB",			true,
								144.200000, 144.200000, "2M MS Calling",    	true,
								144.200001, 144.274999, "2M CW/SSB",			true,
								144.275000, 144.299999, "2M SSB Center",    	true,
								144.300000, 144.399999, "2M SSB",   			true,
                                144.400000, 144.487999, "2M Satellite",			true,
                                144.488000, 144.488000, "2M WSPR",  			true,
                                144.488001, 144.499999, "2M Satellite",			true,
								144.500000, 144.599999, "2M Linear Translator Inputs", true,
								144.600000, 144.899999, "2M FM Repeater",		true,
								144.900000, 145.199999, "2M FM Simplex",		true,
								145.200000, 145.499999, "2M FM Repeater",		true,
								145.500000, 145.799999, "2M FM Simplex",		true,
								145.800000, 145.999999, "2M Satellite",			true,
								146.000000, 146.399999, "2M FM Repeater",		true,
								146.400000, 146.609999, "2M FM Simplex",		true,
								146.610000, 147.389999, "2M FM Repeater",		true,
								147.390000, 147.599999, "2M FM Simplex",		true,
								147.600000, 147.999999, "2M FM Repeater",		true,

								222.000000, 222.024999, "1.25M EME/Weak Signal", true,
								222.025000, 222.049999, "1.25M Weak Signal",	true,
								222.050000, 222.059999, "1.25M Propagation Beacons", true,
								222.060000, 222.099999, "1.25M Weak Signal",	true,
								222.100000, 222.100000, "1.25M SSB/CW Calling",	true,
								222.100001, 222.149999, "1.25M Weak Signal CW/SSB", true,
								222.150000, 222.249999, "1.25M Local Option",	true,
								222.250000, 223.380000, "1.25M FM Repeater Inputs", true,
								222.380001, 223.399999, "1.25M General", true,
								223.400000, 223.519999, "1.25M FM Simplex",		true,
								223.520000, 223.639999, "1.25M Digital/Packet",	true,
								223.640000, 223.700000, "1.25M Links/Control",	true,
								223.700001, 223.709999, "1.25M General",	true,
								223.710000, 223.849999, "1.25M Local Option",	true,
								223.850000, 224.980000, "1.25M Repeater Outputs", true,

								420.000000, 425.999999, "70CM ATV Repeater",	true,
								426.000000, 431.999999, "70CM ATV Simplex",		true,
								432.000000, 432.069999, "70CM EME",				true,
								432.070000, 432.099999, "70CM Weak Signal CW",	true,
								432.100000, 432.100000, "70CM Calling Frequency", true,
								432.100001, 432.299999, "70CM Mixed Mode Weak Signal", true,
								432.300000, 432.399999, "70CM Propagation Beacons", true,
								432.400000, 432.999999, "70CM Mixed Mode Weak Signal", true,
								433.000000, 434.999999, "70CM Auxillary/Repeater Links", true,
								435.000000, 437.999999, "70CM Satellite Only",	true,
								438.000000, 441.999999, "70CM ATV Repeater",	true,
								442.000000, 444.999999, "70CM Local Repeaters",	true,
								445.000000, 445.999999, "70CM Local Option",	true,
								446.000000, 446.000000, "70CM Simplex",			true,
								446.000001, 446.999999, "70CM Local Option",	true,
								447.000000, 450.000000, "70CM Local Repeaters", true,

								902.000000, 902.099999, "33CM Weak Signal SSTV/FAX/ACSSB", true,
								902.100000, 902.100000, "33CM Weak Signal Calling", true,
								902.100001, 902.799999, "33CM Weak Signal SSTV/FAX/ACSSB", true,
								902.800000, 902.999999, "33CM Weak Signal EME/CW", true,
								903.000000, 903.099999, "33CM Digital Modes",	true,
								903.100000, 903.100000, "33CM Alternate Calling", true,
								903.100001, 905.999999, "33CM Digital Modes",	true,
								906.000000, 908.999999, "33CM FM Repeater Inputs", true,
								909.000000, 914.999999, "33CM ATV",				true,
								915.000000, 917.999999, "33CM Digital Modes",	true,
								918.000000, 920.999999, "33CM FM Repeater Outputs", true,
								921.000000, 926.999999, "33CM ATV",				true,
								927.000000, 928.000000, "33CM FM Simplex/Links", true,
								
								1240.000000, 1245.999999, "23CM ATV #1",		true,
								1246.000000, 1251.999999, "23CM FMN Point/Links", true,
								1252.000000, 1257.999999, "23CM ATV #2, Digital Modes", true,
								1258.000000, 1259.999999, "23CM FMN Point/Links", true,
								1260.000000, 1269.999999, "23CM Sat Uplinks/Wideband Exp.", true,
								1270.000000, 1275.999999, "23CM Repeater Inputs", true,
								1276.000000, 1281.999999, "23CM ATV #3",		true,
								1282.000000, 1287.999999, "23CM Repeater Outputs",	true,
								1288.000000, 1293.999999, "23CM Simplex ATV/Wideband Exp.", true,
								1294.000000, 1294.499999, "23CM Simplex FMN",		true,
								1294.500000, 1294.500000, "23CM FM Simplex Calling", true,
								1294.500001, 1294.999999, "23CM Simplex FMN",		true,
								1295.000000, 1295.799999, "23CM SSTV/FAX/ACSSB/Exp.", true,
								1295.800000, 1295.999999, "23CM EME/CW Expansion",	true,
								1296.000000, 1296.049999, "23CM EME Exclusive",		true,
								1296.050000, 1296.069999, "23CM Weak Signal",		true,
								1296.070000, 1296.079999, "23CM CW Beacons",		true,
								1296.080000, 1296.099999, "23CM Weak Signal",		true,
								1296.100000, 1296.100000, "23CM CW/SSB Calling",	true,
								1296.100001, 1296.399999, "23CM Weak Signal",		true,
								1296.400000, 1296.599999, "23CM X-Band Translator Input", true,
								1296.600000, 1296.799999, "23CM X-Band Translator Output", true,
								1296.800000, 1296.999999, "23CM Experimental Beacons", true,
								1297.000000, 1300.000000, "23CM Digital Modes",		true,

								2300.000000, 2302.999999, "2.3GHz High Data Rate", true,
								2303.000000, 2303.499999, "2.3GHz Packet",		true,
								2303.500000, 2303.800000, "2.3GHz TTY Packet",	true,
								2303.800001, 2303.899999, "2.3GHz General",	true,
								2303.900000, 2303.900000, "2.3GHz Packet/TTY/CW/EME", true,
								2303.900001, 2304.099999, "2.3GHz CW/EME",		true,
								2304.100000, 2304.100000, "2.3GHz Calling Frequency", true,
								2304.100001, 2304.199999, "2.3GHz CW/EME/SSB",	true,
								2304.200000, 2304.299999, "2.3GHz SSB/SSTV/FAX/Packet AM/Amtor", true,
								2304.300000, 2304.319999, "2.3GHz Propagation Beacon Network", true,
								2304.320000, 2304.399999, "2.3GHz General Propagation Beacons", true,
								2304.400000, 2304.499999, "2.3GHz SSB/SSTV/ACSSB/FAX/Packet AM", true,
								2304.500000, 2304.699999, "2.3GHz X-Band Translator Input", true,
								2304.700000, 2304.899999, "2.3GHz X-Band Translator Output", true,
								2304.900000, 2304.999999, "2.3GHz Experimental Beacons", true,
								2305.000000, 2305.199999, "2.3GHz FM Simplex", true,
								2305.200000, 2305.200000, "2.3GHz FM Simplex Calling", true,
								2305.200001, 2305.999999, "2.3GHz FM Simplex", true,
								2306.000000, 2308.999999, "2.3GHz FM Repeaters", true,
								2309.000000, 2310.000000, "2.3GHz Control/Aux Links", true,
								2390.000000, 2395.999999, "2.3GHz Fast-Scan TV", true,
								2396.000000, 2398.999999, "2.3GHz High Rate Data", true,
								2399.000000, 2399.499999, "2.3GHz Packet", true,
								2399.500000, 2399.999999, "2.3GHz Control/Aux Links", true,
								2400.000000, 2402.999999, "2.4GHz Satellite", true,
								2403.000000, 2407.999999, "2.4GHz Satellite High-Rate Data", true,
								2408.000000, 2409.999999, "2.4GHz Satellite", true,
								2410.000000, 2412.999999, "2.4GHz FM Repeaters", true,
								2413.000000, 2417.999999, "2.4GHz High-Rate Data", true,
								2418.000000, 2429.999999, "2.4GHz Fast-Scan TV", true,
								2430.000000, 2432.999999, "2.4GHz Satellite", true,
								2433.000000, 2437.999999, "2.4GHz Sat. High-Rate Data", true,
								2438.000000, 2450.000000, "2.4GHz Wideband FM/FSTV/FMTV", true,

								3456.000000, 3456.099999, "3.4GHz General", true,
								3456.100000, 3456.100000, "3.4GHz Calling Frequency", true,
								3456.100001, 3456.299999, "3.4GHz General", true,
								3456.300000, 3456.400000, "3.4GHz Propagation Beacons", true,

								5760.000000, 5760.099999, "5.7GHz General", true,
								5760.100000, 5760.100000, "5.7GHz Calling Frequency", true,
								5760.100001, 5760.299999, "5.7GHz General", true,
								5760.300000, 5760.400000, "5.7GHz Propagation Beacons", true,

								10368.000000, 10368.099999, "10GHz General", true,
								10368.100000, 10368.100000, "10GHz Calling Frequency", true,
								10368.100001, 10368.400000, "10GHz General", true,

								24192.000000, 24192.099999, "24GHz General", true,
								24192.100000, 24192.100000, "24GHz Calling Frequency", true,
								24192.100001, 24192.400000, "24GHz General", true,

								47088.000000, 47088.099999, "47GHz General", true,
								47088.100000, 47088.100000, "47GHz Calling Frequency", true,
								47088.100001, 47088.400000, "47GHz General", true,

								2.500000, 2.500000, "WWV",						false,
								5.000000, 5.000000, "WWV",						false,
								10.000000, 10.000000, "WWV",					false,
								15.000000, 15.000000, "WWV",					false,
								20.000000, 20.000000, "WWV",					false,

								0.525000, 1.710000, "Broadcast AM Med Wave",	false,				
								2.300000, 2.495000, "120M Short Wave",			false,
								3.200000, 3.400000, "90M Short Wave",			false,
								4.750000, 4.999999, "60M Short Wave",			false,
								5.000001, 5.060000, "60M Short Wave",			false,
								5.900000, 6.200000, "49M Short Wave",			false,
								7.300000, 7.350000, "41M Short Wave",			false,
								9.400000, 9.900000, "31M Short Wave",			false,
								11.600000, 12.100000, "25M Short Wave",			false,
								13.570000, 13.870000, "22M Short Wave",			false,
								15.100000, 15.800000, "19M Short Wave",			false,
								17.480000, 17.900000, "16M Short Wave",			false,
								18.900000, 19.020000, "15M Short Wave",			false,
								21.450000, 21.850000, "13M Short Wave",			false,
								25.600000, 26.100000, "11M Short Wave",			false,
			};

            for (int i = 0; i < data.Length / 4; i++)
            {
                DataRow dr = t.NewRow();
                dr["Low"] = (double)data[i * 4 + 0];
                dr["High"] = (double)data[i * 4 + 1];
                dr["Name"] = (string)data[i * 4 + 2];
                dr["TX"] = (bool)data[i * 4 + 3];
                t.Rows.Add(dr);
            }

            #endregion
        }

        private static void AddBandLimitsTable()   // yt7pwr
        {
            ds.Tables.Add("BandLimits");
            DataTable t = ds.Tables["BandLimits"];
            t.Columns.Add("Low", typeof(double));
            t.Columns.Add("High", typeof(double));
            t.Columns.Add("Name", typeof(string));

            object[] data = {
                                0.1, 0.300000, Band.B2190M.ToString(),
                                0.4, 0.600000, Band.B600M.ToString(),
                                1.8, 2.0, Band.B160M.ToString(),
                                3.5, 4.0, Band.B80M.ToString(),
                                5.2872, 5.2872, Band.B60M.ToString(),
                                5.3305, 5.3305, Band.B60M.ToString(),
                                5.3465, 5.3465, Band.B60M.ToString(),
                                5.357, 5.357, Band.B60M.ToString(),
                                5.3715, 5.3715, Band.B60M.ToString(),
                                5.4035, 5.4035, Band.B60M.ToString(),
                                7.0, 7.3, Band.B40M.ToString(),
                                10.1, 10.15, Band.B30M.ToString(),
                                14.0, 14.35, Band.B20M.ToString(),
                                18.068, 18.168, Band.B17M.ToString(),
                                21.0, 21.45, Band.B15M.ToString(),
                                24.89, 24.99, Band.B12M.ToString(),
                                28.0, 29.7, Band.B10M.ToString(),
                                50.0, 54.0, Band.B6M.ToString(),
                                2.5, 2.5, Band.WWV.ToString(),
                                5.0, 5.0, Band.WWV.ToString(),
                                10.0, 10.0, Band.WWV.ToString(),
                                15.0, 15.0, Band.WWV.ToString(),
                                20.0, 20.0, Band.WWV.ToString(),
                                144.0, 148.0, Band.B2M.ToString(),
                             };

            for (int i = 0; i < data.Length / 3; i++)
            {
                DataRow dr = t.NewRow();
                dr["Low"] = (double)data[i * 3 + 0];
                dr["High"] = (double)data[i * 3 + 1];
                dr["Name"] = (string)data[i * 3 + 2];
                t.Rows.Add(dr);
            }
        }

        private static void AddLimeSDRBandFiltersTable()
        {
            ds.Tables.Add("LimeSDRBandFilters");
            DataTable t = ds.Tables["LimeSDRBandFilters"];

            t.Columns.Add("Low", typeof(double));
            t.Columns.Add("High", typeof(double));
            t.Columns.Add("Filter", typeof(string));

            object[] data = {
                                0.1, 0.300000, Band.B2190M.ToString(),
                                0.4, 0.600000, Band.B600M.ToString(),
                                1.0, 2.75, Band.B160M.ToString(),
                                2.7500001, 5.2, Band.B80M.ToString(),
                                5.200001, 5.5, Band.B60M.ToString(),
                                5.500001, 8.7, Band.B40M.ToString(),
                                8.700001, 12.75, Band.B30M.ToString(),
                                12.750001, 16.209, Band.B20M.ToString(),
                                16.209001, 19.584, Band.B17M.ToString(),
                                19.584001, 23.17, Band.B15M.ToString(),
                                23.170001, 26.495, Band.B12M.ToString(),
                                26.495001, 30.0, Band.B10M.ToString(),
                                49.9, 54.1, Band.B6M.ToString(),
                                144.0, 148.0, Band.B2M.ToString(),
                                430.0, 440.0, Band.B07M.ToString(),
                             };

            for (int i = 0; i < data.Length / 3; i++)
            {
                DataRow dr = t.NewRow();
                dr["Low"] = (double)data[i * 3 + 0];
                dr["High"] = (double)data[i * 3 + 1];
                dr["Filter"] = (string)data[i * 3 + 2];
                t.Rows.Add(dr);
            }
        }

        private static void CheckBandTextValid()
        {
            ArrayList bad_rows = new ArrayList();

            if (ds == null) return;

            foreach (DataRow dr in ds.Tables["IARU1BandText"].Rows)
            {
                // check low freq
                string f = ((double)dr["Low"]).ToString("f6");
                f = f.Replace(",", ".");
                DataRow[] rows = ds.Tables["IARU1BandText"].Select(f + ">=Low AND " + f + "<=High");

                if (rows.Length > 1)
                {
                    // handle multiple entries
                    if (!bad_rows.Contains(dr))
                        bad_rows.Add(dr);
                }

                // check high freq
                f = ((double)dr["High"]).ToString("f6");
                f = f.Replace(",", ".");
                rows = ds.Tables["IARU1BandText"].Select(f + ">=Low AND " + f + "<=High");

                if (rows.Length > 1)
                {
                    // handle multiple entries
                    if (!bad_rows.Contains(dr))
                        bad_rows.Add(dr);
                }
            }

            foreach (DataRow dr in bad_rows)
                ds.Tables["IARU1BandText"].Rows.Remove(dr);
            bad_rows.Clear();

            foreach (DataRow dr in ds.Tables["IARU2BandText"].Rows)
            {
                // check low freq
                string f = ((double)dr["Low"]).ToString("f6");
                f = f.Replace(",", ".");
                DataRow[] rows = ds.Tables["IARU2BandText"].Select(f + ">=Low AND " + f + "<=High");

                if (rows.Length > 1)
                {
                    // handle multiple entries
                    if (!bad_rows.Contains(dr))
                        bad_rows.Add(dr);
                }

                // check high freq
                f = ((double)dr["High"]).ToString("f6");
                f = f.Replace(",", ".");
                rows = ds.Tables["IARU2BandText"].Select(f + ">=Low AND " + f + "<=High");

                if (rows.Length > 1)
                {
                    // handle multiple entries
                    if (!bad_rows.Contains(dr))
                        bad_rows.Add(dr);
                }
            }

            foreach (DataRow dr in bad_rows)
                ds.Tables["IARU2BandText"].Rows.Remove(dr);
            bad_rows.Clear();

            foreach (DataRow dr in ds.Tables["IARU3BandText"].Rows)
            {
                // check low freq
                string f = ((double)dr["Low"]).ToString("f6");
                f = f.Replace(",", ".");
                DataRow[] rows = ds.Tables["IARU3BandText"].Select(f + ">=Low AND " + f + "<=High");

                if (rows.Length > 1)
                {
                    // handle multiple entries
                    if (!bad_rows.Contains(dr))
                        bad_rows.Add(dr);
                }

                // check high freq
                f = ((double)dr["High"]).ToString("f6");
                f = f.Replace(",", ".");
                rows = ds.Tables["IARU3BandText"].Select(f + ">=Low AND " + f + "<=High");

                if (rows.Length > 1)
                {
                    // handle multiple entries
                    if (!bad_rows.Contains(dr))
                        bad_rows.Add(dr);
                }
            }

            foreach (DataRow dr in bad_rows)
                ds.Tables["IARU3BandText"].Rows.Remove(dr);

        }

        private static void VerifyTables()
        {
            try
            {
                if (!ds.Tables.Contains("IARU1BandText"))
                    AddIARU1BandTextTable();

                if (!ds.Tables.Contains("IARU2BandText"))
                    AddIARU2BandTextTable();

                if (!ds.Tables.Contains("IARU3BandText"))
                    AddIARU3BandTextTable();

                if (!ds.Tables.Contains("BandLimits"))
                    AddBandLimitsTable();

                if (!ds.Tables.Contains("LimeSDRBandFilters"))
                {
                    AddLimeSDRBandFiltersTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Database!" + ex.ToString());
            }
        }

		#endregion

		#region Public Member Functions

        public static void Init()
        {
            try
            {
                FileName = Application.StartupPath + "\\band_database.xml";
                ds = new DataSet("Data");

                if (File.Exists(FileName))
                    ds.ReadXml(FileName);
                else
                {
                    AddIARU1BandTextTable();
                    AddIARU2BandTextTable();
                    AddIARU3BandTextTable();
                    AddBandLimitsTable();
                    AddLimeSDRBandFiltersTable();
                }

                VerifyTables();
                CheckBandTextValid();
                ds.Tables["IARU1BandText"].DefaultView.Sort = "Low";
                ds.Tables["IARU2BandText"].DefaultView.Sort = "Low";
                ds.Tables["IARU3BandText"].DefaultView.Sort = "Low";
                ds.Tables["BandLimits"].DefaultView.Sort = "Low";
                ds.Tables["LimeSDRBandFilters"].DefaultView.Sort = "Low";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Database Init!\n" + ex.ToString());
            }
        }

        public static void Update()
        {
            try
            {
                ds.WriteXml(FileName, XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB save error!" + ex.ToString());
            }
        }

		public static void Exit()
		{
            try
            {
                Update();
                ds = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB exit error!" + ex.ToString());
            }
		}

        public static bool RemoveTable(string table_name)
        {
            try
            {
                if (ds.Tables.Contains(table_name))
                {
                    ds.Tables.Remove(table_name);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        private static void AddFormTable(string name)
        {
            DataTable tbl;
            tbl = ds.Tables.Add(name);
            ds.Tables[name].Columns.Add("Key", typeof(string));
            ds.Tables[name].Columns.Add("Value", typeof(string));
            ds.Tables[name].PrimaryKey = new DataColumn[] { tbl.Columns["Key"] };
        }

        public static bool SortTable(string table_name)
        {
            try
            {
                if (ds.Tables.Contains(table_name))
                {
                    DataTable tbl;
                    tbl = ds.Tables.Add("temp_table");
                    tbl = ds.Tables[table_name].Copy();
                    string sort = ds.Tables[table_name].Columns[0].ToString();
                    DataRow[] rows = tbl.Select("0 <=" + sort);

                    if (rows.Length > 0)
                    {
                        ds.Tables[table_name].Rows.Clear();

                        foreach (DataRow row in rows)
                        {
                            DataRow r = ds.Tables[table_name].NewRow();

                            for (int i = 0; i < tbl.Columns.Count; i++)
                                r[i] = row[i];

                            ds.Tables[table_name].Rows.Add(r);
                        }

                        ds.Tables.Remove("temp_table");
                        ds.Tables[table_name].DefaultView.Sort = ds.Tables[table_name].Columns[0].ToString();
                        return true;
                    }
                    else
                    {
                        ds.Tables.Remove("temp_table");
                        return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ds.Tables.Remove("temp_table");
                Debug.Write(ex.ToString());
                return false;
            }
        }

        public static void SaveVars(string tableName, ref ArrayList list)
        {
            if (!ds.Tables.Contains(tableName))
                AddFormTable(tableName);
            else
            {
                RemoveTable(tableName);
                AddFormTable(tableName);
            }

            list.Sort();

            foreach (string s in list)
            {
                string[] vals = s.Split('/');

                if (vals.Length > 2)
                {
                    for (int i = 2; i < vals.Length; i++)
                        vals[1] += "/" + vals[i];
                }

                DataRow[] rows = ds.Tables[tableName].Select("Key = '" + vals[0] + "'");

                if (rows.Length == 0)	// name is not in list
                {
                    DataRow newRow = ds.Tables[tableName].NewRow();
                    newRow[0] = vals[0];
                    newRow[1] = vals[1];
                    ds.Tables[tableName].Rows.Add(newRow);
                }
                else if (rows.Length == 1)
                {
                    rows[0][1] = vals[1];
                }
            }
        }

        public static ArrayList GetVars(string tableName)
        {
            ArrayList list = new ArrayList();
            if (!ds.Tables.Contains(tableName))
                return list;

            DataTable t = ds.Tables[tableName];

            for (int i = 0; i < t.Rows.Count; i++)
            {
                list.Add(t.Rows[i][0].ToString() + "/" + t.Rows[i][1].ToString());
            }

            return list;
        }

        public static bool ImportDatabase(string filename)
        {
            if (!File.Exists(filename))
                return false;

            FileName = filename;
            DataSet file = new DataSet();

            try
            {
                file.ReadXml(filename);
                ds = file;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool ExportDatabase(string filename)
        {
            try
            {
                ds.WriteXml(filename, XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool ImportTable(string filename, string table)
        {
            try
            {
                if (!File.Exists(filename))
                    return false;

                DataSet file = new DataSet();

                try
                {
                    file.ReadXml(filename);
                }
                catch (Exception)
                {
                    return false;
                }

                DataTableCollection tb = file.Tables;

                if (tb != null && tb.Contains(table))
                {
                    if (ds.Tables.Contains(table))
                        ds.Tables.Remove(table);

                    DataTable tbl = tb[table].Copy();
                    ds.Tables.Add(tbl);
                }
                else
                {
                    MessageBox.Show("File do not contain table " + table, "Error!");
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        public static bool CheckImportTable(string filename, out string table_name)
        {
            table_name = "";

            try
            {
                if (!File.Exists(filename))
                    return false;

                DataSet file = new DataSet();

                try
                {
                    file.ReadXml(filename);
                }
                catch (Exception)
                {
                    return false;
                }

                DataTableCollection tb = file.Tables;

                if (tb != null && tb.Count == 1)
                {
                    table_name = tb[0].ToString();
                    return true;
                }
                else
                {
                    MessageBox.Show("File do not contain valid table! " + "Error!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        public static bool GetTableNames(out string[] table)
        {
            string[] names = new string[1];
            table = names;

            try
            {
                DataTableCollection tb = ds.Tables;

                if (tb != null && tb.Count > 0)
                {
                    names = new string[tb.Count];

                    for (int i = 0; i < tb.Count; i++)
                    {
                        names[i] = tb[i].TableName;
                    }

                    table = names;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        public static bool ExportTable(string filename, string table)
        {
            DataSet file = new DataSet();
            DataTableCollection tb = ds.Tables;

            try
            {
                if (tb != null && tb.Contains(table))
                {
                    DataTable tbl = tb[table].Copy();
                    file.Tables.Add(tbl);
                    file.WriteXml(filename, XmlWriteMode.WriteSchema);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

		#endregion
	}
}
