//=================================================================
// CATParser.cs
//=================================================================
// Copyright (C) 2005  Bob Tracy
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// You may contact the author via email at: k5kdn@arrl.net
//=================================================================


/*
 *  Changes for GenesisRadio
 *  Copyright (C)2008,2009,2010,2011,2012 YT7PWR Goran Radivojevic
 *  contact via email at: yt7pwr@ptt.rs or yt7pwr2002@yahoo.com
*/


using System;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;

namespace PowerSDR
{
    /// <summary>
    /// Summary description for CATParser.
    /// </summary>

    #region CATParser Class

    public class CATParser
    {

        #region Variable declarations

        private string current_cat;
        private string prefix;
        private string suffix;
        private string extension;
        private char[] term = new char[1] { ';' };
        public int nSet;
        public int nGet;
        public int nAns;
        public bool IsActive;
        private XmlDocument doc;
        private CATCommands cmdlist;
        private Console console;
        public string Error1 = "?;";
        public string Error2 = "E;";
        public string Error3 = "O;";
        private bool IsExtended;
        private ASCIIEncoding AE = new ASCIIEncoding();

        #endregion Variable declarations

        public CATParser(Console c)
        {
            console = c;
            cmdlist = new CATCommands(console, this);
            GetCATData();
        }

        private void GetCATData()
        {
            string file = "CATStructs.xml";
            doc = new XmlDocument();
            try
            {
                doc.Load(Application.StartupPath + "\\" + file);
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw (e);
            }
        }

        // Overloaded Get method accepts either byte or string
        public byte[] Get(byte[] pCmdString)
        {
            byte[] answer = new byte[16];

            switch (pCmdString[4])
            {
                case 0:
                    answer = cmdlist.CMD_00(pCmdString);
                    break;

                case 1:
                    answer = cmdlist.CMD_01(pCmdString);
                    break;

                case 2:
                    answer = cmdlist.CMD_02(pCmdString);
                    break;

                case 3:
                    answer = cmdlist.CMD_03(pCmdString);
                    break;

                case 4:
                    answer = cmdlist.CMD_04(pCmdString);
                    break;

                case 5:
                    answer = cmdlist.CMD_05(pCmdString);
                    break;

                case 6:
                    answer = cmdlist.CMD_06(pCmdString);
                    break;

                case 7:
                    answer = cmdlist.CMD_07(pCmdString);
                    break;

                case 0x0f:
                    answer = cmdlist.CMD_0F(pCmdString);
                    break;

                case 0x11:
                    answer = cmdlist.CMD_11(pCmdString);
                    break;

                case 0x14:
                    answer = cmdlist.CMD_14(pCmdString);                // Various Level Settings
                    break;

                case 0x15:
                    answer = cmdlist.CMD_15(pCmdString);                // S meter
                    break;

                case 0x16:
                    answer = cmdlist.CMD_16(pCmdString);
                    break;

                case 0x19:
                    answer = cmdlist.CMD_19(pCmdString);
                    break;

                case 0x1A:
                    switch (pCmdString[5])
                    {
                        case 0x00:
                            answer = cmdlist.CMD_1A_00(pCmdString);
                            break;

                        case 0x03:
                            answer = cmdlist.CMD_1A_03(pCmdString);
                            break;

                        case 0x05:
                            answer = cmdlist.CMD_1A_03(pCmdString);
                            break;

                        default:
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = pCmdString[3];
                            answer[3] = pCmdString[2];
                            answer[4] = 0xfa;           // error code
                            answer[6] = 0xfd;
                            break;
                    }
                    break;

                case 0x1b:
                    answer = cmdlist.CMD_1B(pCmdString);
                    break;

                case 0x1C:
                    switch (pCmdString[5])
                    {
                        case 0x00:
                            answer = cmdlist.CMD_1C_00(pCmdString);
                            break;

                        case 0x01:
                            answer = cmdlist.CMD_1C_01(pCmdString);
                            break;

                        default:
                            answer[0] = 0xfe;
                            answer[1] = 0xfe;
                            answer[2] = pCmdString[3];
                            answer[3] = pCmdString[2];
                            answer[4] = 0xfa;           // error code
                            answer[6] = 0xfd;
                            break;
                    }
                    break;

                default:
                    answer[0] = 0xfe;
                    answer[1] = 0xfe;
                    answer[2] = pCmdString[3];
                    answer[3] = pCmdString[2];
                    answer[4] = 0xfa;           // error code
                    answer[6] = 0xfd;
                    break;
            }

            return answer;
        }

        public string Get(string pCmdString)
        {
            current_cat = pCmdString;
            string rtncmd = "";
            bool goodcmd = false;

            // Abort if the overall string length is less than 3 (aa;)
            if (current_cat.Length < 3)
                return ""; // Error1;

            goodcmd = CheckFormat();

            if (goodcmd)
            {
                switch (prefix)
                {
                    case "AC":
                        break;
                    case "AG":
                        rtncmd = cmdlist.AG(suffix);
                        break;
                    case "AI":
                        rtncmd = cmdlist.AI(suffix);
                        break;
                    case "AL":
                        break;
                    case "AM":
                        break;
                    case "AN":
                        break;
                    case "AR":
                        break;
                    case "AS":
                        break;
                    case "BC":
                        break;
                    case "BD":
                        rtncmd = cmdlist.BD();
                        break;
                    case "BP":
                        break;
                    case "BU":
                        rtncmd = cmdlist.BU();
                        break;
                    case "BY":
                        break;
                    case "CA":
                        break;
                    case "CG":
                        break;
                    case "CH":
                        break;
                    case "CI":
                        break;
                    case "CM":
                        break;
                    case "CN":
                        break;
                    case "CT":
                        break;
                    case "DC":
                        break;
                    case "DN":
                        rtncmd = cmdlist.DN();
                        break;
                    case "DQ":
                        break;
                    case "EX":
                        break;
                    case "F1":
                        rtncmd = cmdlist.F1(suffix);
                        break;
                    case "F2":
                        rtncmd = cmdlist.F2(suffix);
                        break;
                    case "F3":
                        rtncmd = cmdlist.F3(suffix);
                        break;
                    case "F4":
                        rtncmd = cmdlist.F4(suffix);
                        break;
                    case "F5":
                        rtncmd = cmdlist.F5(suffix);
                        break;
                    case "F6":
                        rtncmd = cmdlist.F6(suffix);
                        break;
                    case "FA":
                        rtncmd = cmdlist.FA(suffix);
                        break;
                    case "FB":
                        rtncmd = cmdlist.FB(suffix);
                        break;
                    case "FC":
                        break;
                    case "FD":
                        rtncmd = cmdlist.FD(suffix);
                        break;
                    case "FL":
                        rtncmd = cmdlist.FL(suffix);
                        break;
                    case "FR":
                        rtncmd = cmdlist.FR(suffix);
                        break;
                    case "FS":
                        break;
                    case "FT":
                        rtncmd = cmdlist.FT(suffix);
                        break;
                    case "FU":
                        rtncmd = cmdlist.FU(suffix);
                        break;
                    case "FW":
                        rtncmd = cmdlist.FW(suffix);
                        break;
                    case "GT":
                        rtncmd = cmdlist.GT(suffix);
                        break;
                    case "ID":
                        rtncmd = cmdlist.ID();
                        break;
                    case "IF":
                        rtncmd = cmdlist.IF();
                        break;
                    case "IS":
                        break;
                    case "KS":
                        rtncmd = cmdlist.KS(suffix);
                        break;
                    case "KY":
                        rtncmd = cmdlist.KY(suffix);
                        break;
                    case "LK":
                        break;
                    case "LM":
                        break;
                    case "LT":
                        break;
                    case "MC":
                        break;
                    case "MD":
                        rtncmd = cmdlist.MD(suffix);
                        break;
                    case "MF":
                        break;
                    case "MG":
                        rtncmd = cmdlist.MG(suffix);
                        break;
                    case "ML":
                        break;
                    case "MO":
                        rtncmd = cmdlist.MO(suffix);
                        break;
                    case "MR":
                        break;
                    case "MU":
                        break;
                    case "MW":
                        break;
                    case "NB":
                        rtncmd = cmdlist.NB(suffix);
                        break;
                    case "NL":
                        break;
                    case "NR":
                        break;
                    case "NT":
                        rtncmd = cmdlist.NT(suffix);
                        break;
                    case "OF":
                        break;
                    case "OI":
                        break;
                    case "OS":
                        break;
                    case "PA":
                        rtncmd = cmdlist.PA(suffix);
                        break;
                    case "PB":
                        break;
                    case "PC":
                        rtncmd = cmdlist.PC(suffix);
                        break;
                    case "PI":
                        break;
                    case "PK":
                        break;
                    case "PL":
                        break;
                    case "PM":
                        break;
                    case "PR":
                        rtncmd = cmdlist.PR(suffix);
                        break;
                    case "PS":
                        rtncmd = cmdlist.PS(suffix);
                        break;
                    case "PT":
                        rtncmd = cmdlist.PT(suffix);
                        break;
                    case "QC":
                        break;
                    case "QI":
                        rtncmd = cmdlist.QI();
                        break;
                    case "QR":
                        break;
                    case "RA":
                        rtncmd = cmdlist.RA(suffix);
                        break;
                    case "RC":
                        rtncmd = cmdlist.RC();
                        break;
                    case "RD":
                        rtncmd = cmdlist.RD(suffix);
                        break;
                    case "RI":
                        rtncmd = cmdlist.RI(suffix);
                        break;
                    case "RG":
                        rtncmd = cmdlist.RG(suffix);
                        break;
                    case "RL":
                        break;
                    case "RM":
                        break;
                    case "RT":
                        rtncmd = cmdlist.RT(suffix);
                        break;
                    case "RU":
                        rtncmd = cmdlist.RU(suffix);
                        break;
                    case "RX":
                        rtncmd = cmdlist.RX(suffix);
                        break;
                    case "SA":
                        break;
                    case "SB":
                        break;
                    case "SC":
                        break;
                    case "SD":
                        break;
                    case "SH":
                        rtncmd = cmdlist.SH(suffix);
                        break;
                    case "SI":
                        break;
                    case "SL":
                        rtncmd = cmdlist.SL(suffix);
                        break;
                    case "SM":
                        rtncmd = cmdlist.SM(suffix);
                        break;
                    case "SQ":
                        rtncmd = cmdlist.SQ(suffix);
                        break;
                    case "SR":
                        break;
                    case "SS":
                        break;
                    case "ST":
                        break;
                    case "SU":
                        break;
                    case "SV":
                        break;
                    case "TC":
                        break;
                    case "TD":
                        break;
                    case "TI":
                        break;
                    case "TN":
                        break;
                    case "TO":
                        break;
                    case "TS":
                        break;
                    case "TX":
                        rtncmd = cmdlist.TX(suffix);
                        break;
                    case "TY":
                        break;
                    case "UL":
                        break;
                    case "UP":
                        rtncmd = cmdlist.UP();
                        break;
                    case "VD":
                        break;
                    case "VG":
                        break;
                    case "VR":
                        break;
                    case "VX":
                        break;
                    case "XT":
                        rtncmd = cmdlist.XT(suffix);
                        break;
                    case "ZZ":
                        rtncmd = ParseExtended();
                        break;
                }

                if (prefix != "ZZ")	                                    // if this is a standard command
                {
                    // and it's not an error
                    if (rtncmd != Error1 && rtncmd != Error2 && rtncmd != Error3)
                    {
                        // if it has the correct length
                        if (rtncmd.Length == nAns && nAns > 0)
                            rtncmd = prefix + rtncmd + ";";	            // return the formatted CAT answer
                        else if (nAns == -1 || rtncmd == "")	        // no answer is required
                            rtncmd = "";
                        else
                            rtncmd = Error3;	                        // processing incomplete for some reason
                    }
                }
            }
            else
                rtncmd = Error1;	// this was a bad command

            return rtncmd;	        // Read successfully executed
        }

        private bool CheckFormat()
        {
            if (console.CATRigType == 1)        // ICOM IC-7000
            {
                return true;
            }
            else
            {
                bool goodprefix, goodsuffix;
                // If there is no terminator, or the prefix or suffix
                // is invalid, abort.

                // If the command has a leading terminator(s) (like sent by WriteLog)
                // dump it and check the rest of the command.
                if (current_cat.StartsWith(";"))
                    current_cat = current_cat.TrimStart(term);

                // If there is no terminator, or the prefix
                // is invalid, abort.
                if (current_cat.IndexOfAny(term) < 2)
                    return false;

                // Now check to see if it's an extended command
                if (current_cat.Substring(0, 2).ToUpper() == "ZZ")
                    IsExtended = true;
                else
                    IsExtended = false;

                // Check the prefix
                goodprefix = FindPrefix();
                if (!goodprefix)
                    return false;

                // Check the suffix
                goodsuffix = FindSuffix();
                if (!goodsuffix)
                    return false;

                return true;
            }
        }

        private bool FindPrefix()
        {
            string pfx = "";

            // Extract the prefix from the command string
            if (IsExtended)
                pfx = current_cat.Substring(0, 4).ToUpper();
            else
                pfx = current_cat.Substring(0, 2).ToUpper();

            try
            // Find the prefix in the xml document and get the parameter
            // values.
            {
                XmlNode struc;
                XmlElement root = doc.DocumentElement;
                string search = "descendant::catstruct[@code='" + pfx + "']";
                struc = root.SelectSingleNode(search);

                if (struc != null)
                {
                    foreach (XmlNode x in struc)
                    {
                        switch (x.Name)
                        {
                            case "active":
                                IsActive = Convert.ToBoolean(x.InnerXml);
                                break;
                            case "nsetparms":
                                nSet = Convert.ToInt16(x.InnerXml);
                                break;
                            case "ngetparms":
                                nGet = Convert.ToInt16(x.InnerXml);
                                break;
                            case "nansparms":
                                nAns = Convert.ToInt16(x.InnerXml);
                                break;
                        }
                    }
                    //					prefix = pfx;
                    // If this is not an active command there is no use continuing.
                    if (IsActive)
                    {
                        if (IsExtended)
                        {
                            prefix = pfx.Substring(0, 2);
                            extension = pfx.Substring(2, 2);
                        }
                        else
                            prefix = pfx;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
            return false;
        }

        private bool FindSuffix()
        {
            string sfx;
            int len = 3;
            int start = 2;
            int end = 2;

            if (IsExtended)
            {
                len = 5;
                start = 4;
                end = 4;
            }

            // Define the suffix as everything after the prefix and before
            // the first terminator.
            if (current_cat.Length > len)
            {
                sfx = current_cat.Substring(start, current_cat.IndexOf(";") - end);
                if (prefix != "KY" && prefix + extension != "ZZKY" &&
                    (prefix + extension != "ZZEA" && prefix + extension != "ZZEB") &&
                    (prefix + extension != "ZZFX" && prefix + extension != "ZZFY") &&
                    (prefix + extension != "ZZFV" && prefix + extension != "ZZFW"))
                {
                    Regex sfxpattern = new Regex("^[+-]?[Vv0-9]*$");
                    if (!sfxpattern.IsMatch(sfx))
                        return false;
                }
                //modified 3/17/07 BT to correct bug in reading parameters with plus or minus sign
                // Check the suffix for illegal characters
                // [^0-9] = match any non-numeric character
                //Regex sfxpattern = new Regex("[^0-9]");
                //if(sfxpattern.IsMatch(sfx))
                //	return false;
            }
            else
            {
                sfx = "";
            }

            // Check the length against the struct requirements
            if (sfx.Length == nSet || sfx.Length == nGet)
            {
                suffix = sfx;
                return true;
            }
            return false;
        }

        private string ParseExtended()
        {
            string rtncmd = Error1;
            string extended = prefix + extension;

            switch (extended)
            {
                case "ZZAC":
                    rtncmd = cmdlist.ZZAC(suffix);
                    break;
                case "ZZAD":
                    rtncmd = cmdlist.ZZAD(suffix);
                    break;
                case "ZZAG":
                    rtncmd = cmdlist.ZZAG(suffix);
                    break;
                case "ZZAI":
                    rtncmd = cmdlist.ZZAI(suffix);
                    break;
                case "ZZAR":
                    rtncmd = cmdlist.ZZAR(suffix);
                    break;
                case "ZZAU":
                    rtncmd = cmdlist.ZZAU(suffix);
                    break;
                case "ZZBD":
                    rtncmd = cmdlist.ZZBD();
                    break;
                case "ZZBI":
                    rtncmd = cmdlist.ZZBI(suffix);
                    break;
                case "ZZBG":
                    rtncmd = cmdlist.ZZBG(suffix);
                    break;
                case "ZZBM":
                    rtncmd = cmdlist.ZZBM(suffix);
                    break;
                case "ZZBP":
                    rtncmd = cmdlist.ZZBP(suffix);
                    break;
                case "ZZBS":
                    rtncmd = cmdlist.ZZBS(suffix);
                    break;
                case "ZZBU":
                    rtncmd = cmdlist.ZZBU();
                    break;
                case "ZZBY":
                    rtncmd = cmdlist.ZZBY();
                    break;
                case "ZZCB":
                    rtncmd = cmdlist.ZZCB(suffix);
                    break;
                case "ZZCD":
                    rtncmd = cmdlist.ZZCD(suffix);
                    break;
                case "ZZCF":
                    rtncmd = cmdlist.ZZCF(suffix);
                    break;
                case "ZZCI":
                    rtncmd = cmdlist.ZZCI(suffix);
                    break;
                case "ZZCL":
                    rtncmd = cmdlist.ZZCL(suffix);
                    break;
                case "ZZCM":
                    rtncmd = cmdlist.ZZCM(suffix);
                    break;
                case "ZZCP":
                    rtncmd = cmdlist.ZZCP(suffix);
                    break;
                case "ZZCS":
                    rtncmd = cmdlist.ZZCS(suffix);
                    break;
                case "ZZCT":
                    rtncmd = cmdlist.ZZCT(suffix);
                    break;
                case "ZZCU":
                    rtncmd = cmdlist.ZZCU();
                    break;
                case "ZZDA":
                    rtncmd = cmdlist.ZZDA(suffix);
                    break;
                case "ZZDM":
                    rtncmd = cmdlist.ZZDM(suffix);
                    break;
                case "ZZDU":
                    rtncmd = cmdlist.ZZDU(suffix);
                    break;
                case "ZZDX":
                    rtncmd = cmdlist.ZZDX(suffix);
                    break;
                case "ZZER":
                    rtncmd = cmdlist.ZZER(suffix);
                    break;
                case "ZZEA":
                    rtncmd = cmdlist.ZZEA(suffix);
                    break;
                case "ZZEB":
                    rtncmd = cmdlist.ZZEB(suffix);
                    break;
                case "ZZET":
                    rtncmd = cmdlist.ZZET(suffix);
                    break;
                case "ZZF1":
                    rtncmd = cmdlist.ZZF1(suffix);
                    break;
                case "ZZF2":
                    rtncmd = cmdlist.ZZF2(suffix);
                    break;
                case "ZZF3":
                    rtncmd = cmdlist.ZZF3(suffix);
                    break;
                case "ZZF4":
                    rtncmd = cmdlist.ZZF4(suffix);
                    break;
                case "ZZF5":
                    rtncmd = cmdlist.ZZF5(suffix);
                    break;
                case "ZZF6":
                    rtncmd = cmdlist.ZZF6(suffix);
                    break;
                case "ZZFA":
                    rtncmd = cmdlist.ZZFA(suffix);
                    break;
                case "ZZFB":
                    rtncmd = cmdlist.ZZFB(suffix);
                    break;
                case "ZZFO":
                    rtncmd = cmdlist.ZZFO(suffix);
                    break;
                case "ZZFI":
                    rtncmd = cmdlist.ZZFI(suffix);
                    break;
                case "ZZFJ":
                    rtncmd = cmdlist.ZZFJ(suffix);
                    break;
                case "ZZFL":
                    rtncmd = cmdlist.ZZFL(suffix);
                    break;
                case "ZZFH":
                    rtncmd = cmdlist.ZZFH(suffix);
                    break;
                case "ZZFM":
                    rtncmd = cmdlist.ZZFM();
                    break;
                case "ZZGA":
                    rtncmd = cmdlist.ZZGA(suffix);
                    break;
                case "ZZGE":
                    rtncmd = cmdlist.ZZGE(suffix);
                    break;
                case "ZZGL":
                    rtncmd = cmdlist.ZZGL(suffix);
                    break;
                case "ZZGN":
                    rtncmd = cmdlist.ZZGN(suffix);
                    break;
                case "ZZGR":
                    rtncmd = cmdlist.ZZGR(suffix);
                    break;
                case "ZZGT":
                    rtncmd = cmdlist.ZZGT(suffix);
                    break;
                case "ZZHA":
                    rtncmd = cmdlist.ZZHA(suffix);
                    break;
                case "ZZHR":
                    rtncmd = cmdlist.ZZHR(suffix);
                    break;
                case "ZZHT":
                    rtncmd = cmdlist.ZZHT(suffix);
                    break;
                case "ZZHU":
                    rtncmd = cmdlist.ZZHU(suffix);
                    break;
                case "ZZHV":
                    rtncmd = cmdlist.ZZHV(suffix);
                    break;
                case "ZZHW":
                    rtncmd = cmdlist.ZZHW(suffix);
                    break;
                case "ZZHX":
                    rtncmd = cmdlist.ZZHX(suffix);
                    break;
                case "ZZID":
                    rtncmd = cmdlist.ZZID();
                    break;
                case "ZZIF":
                    rtncmd = cmdlist.ZZIF(suffix);
                    break;
                case "ZZIS":
                    rtncmd = cmdlist.ZZIS(suffix);
                    break;
                case "ZZIT":
                    rtncmd = cmdlist.ZZIT(suffix);
                    break;
                case "ZZIU":
                    rtncmd = cmdlist.ZZIU();
                    break;
                case "ZZKM":
                    rtncmd = cmdlist.ZZKM(suffix);
                    break;
                case "ZZKS":
                    rtncmd = cmdlist.ZZKS(suffix);
                    break;
                case "ZZKY":
                    rtncmd = cmdlist.ZZKY(suffix);
                    break;
                case "ZZMA":
                    rtncmd = cmdlist.ZZMA(suffix);
                    break;
                case "ZZMD":
                    rtncmd = cmdlist.ZZMD(suffix);
                    break;
                case "ZZME":
                    rtncmd = cmdlist.ZZME(suffix);
                    break;
                case "ZZMG":
                    rtncmd = cmdlist.ZZMG(suffix);
                    break;
                case "ZZMO":
                    rtncmd = cmdlist.ZZMO(suffix);
                    break;
                case "ZZMR":
                    rtncmd = cmdlist.ZZMR(suffix);
                    break;
                case "ZZMS":
                    //rtncmd = cmdlist.ZZMS(suffix);
                    rtncmd = "?";
                    break;
                case "ZZMT":
                    rtncmd = cmdlist.ZZMT(suffix);
                    break;
                case "ZZMU":
                    rtncmd = cmdlist.ZZMU(suffix);
                    break;
                case "ZZNA":
                    rtncmd = cmdlist.ZZNA(suffix);
                    break;
                case "ZZNB":
                    rtncmd = cmdlist.ZZNB(suffix);
                    break;
                case "ZZNL":
                    rtncmd = cmdlist.ZZNL(suffix);
                    break;
                case "ZZNM":
                    rtncmd = cmdlist.ZZNM(suffix);
                    break;
                case "ZZNR":
                    rtncmd = cmdlist.ZZNR(suffix);
                    break;
                case "ZZNT":
                    rtncmd = cmdlist.ZZNT(suffix);
                    break;
                case "ZZPA":
                    rtncmd = cmdlist.ZZPA(suffix);
                    break;
                case "ZZPC":
                    rtncmd = cmdlist.ZZPC(suffix);
                    break;
                case "ZZPD":
                    rtncmd = cmdlist.ZZPD();
                    break;
                case "ZZPK":
                    rtncmd = cmdlist.ZZPK(suffix);
                    break;
                case "ZZPL":
                    rtncmd = cmdlist.ZZPL(suffix);
                    break;
                case "ZZPO":
                    rtncmd = cmdlist.ZZPO(suffix);
                    break;
                case "ZZPS":
                    rtncmd = cmdlist.ZZPS(suffix);
                    break;
                case "ZZPZ":
                    rtncmd = cmdlist.ZZPZ(suffix);
                    break;
                case "ZZQM":
                    rtncmd = cmdlist.ZZQM();
                    break;
                case "ZZQR":
                    rtncmd = cmdlist.ZZQR();
                    break;
                case "ZZQS":
                    rtncmd = cmdlist.ZZQS();
                    break;
                case "ZZRA":
                    rtncmd = cmdlist.ZZRA(suffix);
                    break;
                case "ZZRB":
                    rtncmd = cmdlist.ZZRB(suffix);
                    break;
                case "ZZRC":
                    rtncmd = cmdlist.ZZRC();
                    break;
                case "ZZRD":
                    rtncmd = cmdlist.ZZRD(suffix);
                    break;
                case "ZZRF":
                    rtncmd = cmdlist.ZZRF(suffix);
                    break;
                case "ZZRH":
                    rtncmd = cmdlist.ZZRH(suffix);
                    break;
                case "ZZRL":
                    rtncmd = cmdlist.ZZRL(suffix);
                    break;
                case "ZZRM":
                    rtncmd = cmdlist.ZZRM(suffix);
                    break;
                case "ZZRS":
                    rtncmd = cmdlist.ZZRS(suffix);
                    break;
                case "ZZRT":
                    rtncmd = cmdlist.ZZRT(suffix);
                    break;
                case "ZZRU":
                    rtncmd = cmdlist.ZZRU(suffix);
                    break;
                case "ZZSA":
                    rtncmd = cmdlist.ZZSA();
                    break;
                case "ZZSB":
                    rtncmd = cmdlist.ZZSB();
                    break;
                case "ZZSD":
                    rtncmd = cmdlist.ZZSD();
                    break;
                case "ZZSF":
                    rtncmd = cmdlist.ZZSF(suffix);
                    break;
                case "ZZSG":
                    rtncmd = cmdlist.ZZSG();
                    break;
                case "ZZSH":
                    rtncmd = cmdlist.ZZSH();
                    break;
                case "ZZSM":
                    rtncmd = cmdlist.ZZSM(suffix);
                    break;
                case "ZZSO":
                    rtncmd = cmdlist.ZZSO(suffix);
                    break;
                case "ZZSP":
                    rtncmd = cmdlist.ZZSP(suffix);
                    break;
                case "ZZSQ":
                    rtncmd = cmdlist.ZZSQ(suffix);
                    break;
                case "ZZS0":
                    rtncmd = cmdlist.ZZS0(suffix);
                    break;
                case "ZZS1":
                    rtncmd = cmdlist.ZZS1(suffix);
                    break;
                case "ZZSS":
                    rtncmd = cmdlist.ZZSS();
                    break;
                case "ZZST":
                    rtncmd = cmdlist.ZZST(suffix);
                    break;
                case "ZZSU":
                    rtncmd = cmdlist.ZZSU();
                    break;
                case "ZZSV":
                    rtncmd = cmdlist.ZZSV(suffix);
                    break;
                case "ZZSW":
                    rtncmd = cmdlist.ZZSW(suffix);
                    break;
                case "ZZTH":
                    rtncmd = cmdlist.ZZTH(suffix);
                    break;
                case "ZZTI":
                    rtncmd = cmdlist.ZZTI(suffix);
                    break;
                case "ZZTP":
                    rtncmd = cmdlist.ZZTP(suffix);
                    break;
                case "ZZTL":
                    rtncmd = cmdlist.ZZTL(suffix);
                    break;
                case "ZZTO":
                    rtncmd = cmdlist.ZZTO(suffix);
                    break;
                case "ZZTU":
                    rtncmd = cmdlist.ZZTU(suffix);
                    break;
                case "ZZTX":
                    rtncmd = cmdlist.ZZTX(suffix);
                    break;
                case "ZZVA":
                    rtncmd = cmdlist.ZZVA(suffix);
                    break;
                case "ZZVB":
                    rtncmd = cmdlist.ZZVB(suffix);
                    break;
                case "ZZVC":
                    rtncmd = cmdlist.ZZVC(suffix);
                    break;
                case "ZZVD":
                    rtncmd = cmdlist.ZZVD(suffix);
                    break;
                case "ZZVE":
                    rtncmd = cmdlist.ZZVE(suffix);
                    break;
                case "ZZVF":
                    rtncmd = cmdlist.ZZVF(suffix);
                    break;
                case "ZZVG":
                    rtncmd = cmdlist.ZZVG(suffix);
                    break;
                case "ZZVL":
                    rtncmd = cmdlist.ZZVL(suffix);
                    break;
                case "ZZVN":
                    rtncmd = cmdlist.ZZVN();
                    break;
                case "ZZVS":
                    rtncmd = cmdlist.ZZVS(suffix);
                    break;
                case "ZZXC":
                    rtncmd = cmdlist.ZZXC();
                    break;
                case "ZZXF":
                    rtncmd = cmdlist.ZZXF(suffix);
                    break;
                case "ZZXS":
                    rtncmd = cmdlist.ZZXS(suffix);
                    break;
                case "ZZXX":
                    rtncmd = cmdlist.ZZXX(suffix);
                    break;
                case "ZZZZ":
                    rtncmd = cmdlist.ZZZZ();
                    break;
                case "ZZUB":                    // USB status
                    rtncmd = cmdlist.ZZUB(suffix);
                    break;
            }

            if (rtncmd != Error1 && rtncmd != Error2 && rtncmd != Error3)
            {
                if (rtncmd.Length == nAns && nAns > 0)
                {
                    if (rtncmd.StartsWith(" ") && extension != "MN")  //Don't trim filter name string
                    {												// Fix in next generation.
                        rtncmd = rtncmd.Trim();
                    }
                    rtncmd = prefix + extension + suffix + rtncmd + ";";
                }
            }
            else
                rtncmd = Error1;

            return rtncmd;
        }
    }

    #endregion CATParser Class

}
