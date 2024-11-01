using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace ELE
{
    public partial class Form1 : Form
    {
        Boolean select = false;
        Boolean flag = true;
        int upmin = 6, uphour = 10, downmin = 6, downhour = 10;
        int selectB = -1, selectL = -1, selectA = -1, selectC = -1;
        List<String> emus = new List<string>();
        Dictionary<String, String> dic = new Dictionary<String, String>();
        Dictionary<String, String> dic2 = new Dictionary<String, String>();
        List<String> dk1 = new List<string>();
        List<String> dk2 = new List<string>();
        List<TimeTime> uptimes = new List<TimeTime>();
        List<TimeTime> downtimes = new List<TimeTime>();
        List<List<List<int>>> sss = new List<List<List<int>>>();
        List<int> ssns = new List<int>();
        String train = "";
        String ver = "";
        List<ListItem> lis = new List<ListItem>();
        String upTime = "", downTime = "";
        String[] rcps = new String[0];
        String appdir;
        WebClient wc = new WebClient();
        string file = "";
        private static double EARTH_RADIUS = 6378137;

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }
        public Form1(string file)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.file = file;
        }

        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        public static double GetLength(double lng1, double lat1, double lng2, double lat2)
        {//获取长度
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s / 1000.0;
        }
        SeatType GetSeatType(String str)
        {//String转SeatType
            switch (str.ToUpper())
            {
                case "ZE":
                    return SeatType.ZE;
                case "CA":
                    return SeatType.CA;
                case "ZEC":
                    return SeatType.ZEC;
                case "ZY":
                    return SeatType.ZY;
                case "ZT":
                    return SeatType.ZT;
                case "ZS":
                case "SW":
                    return SeatType.SW;
                case "WG":
                    return SeatType.WG;
                case "WR":
                    return SeatType.WR;
                case "WY":
                    return SeatType.WY;
                case "WE":
                    return SeatType.WE;
                case "WRC":
                    return SeatType.WRC;
                case "DGN":
                    return SeatType.DGN;
                case "UY":
                    return SeatType.UY;
                case "D":
                    return SeatType.D;
                case "ZYC":
                    return SeatType.ZYC;
                case "BZ":
                    return SeatType.BZ;
            }
            return SeatType.ZE;
        }
        public String Gethtml(String url, int delay)
        {
            byte[] buffer = wc.DownloadData(url);
            ;
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }
        public string replace(string str, char ch)
        {
            return str.Replace(ch.ToString(), "");
        }
        private void Form1_Load(object sender, EventArgs e5)
        {
            if (!flag)
                return;
            select = false;
            Share.emus = new List<EMU>();
            Share.rails = new List<Rail>();
            Share.lists = new Dictionary<char, List<int>>();
            List<Byte> persons = new List<Byte>();
            List<SeatType> seatTypes = new List<SeatType>();
            List<List<Train>> trains = new List<List<Train>>();
            List<String> ver = new List<String>();
            String[] emustrs, trainstrs, seatstrs, temp, temp2, tmp3, evens = null, ssstrs = null, spdstrs;
            int tmp4;
            emustrs = new String[1];
            JObject railstr, tmp;
            Station station;
            Boolean b;
            JObject item;
            String[] replaceDatas = null;
            String html;
            double length;
            int test = 0;
            int y = 0;
            appdir = Directory.GetCurrentDirectory() + "\\";
            try
            {
                if (!File.Exists(appdir + "settings.ini"))
                {
                    File.WriteAllText(appdir + "settings.ini", "dataLink=https://raildatas.github.io/");
                }
                html = File.ReadAllText(appdir + "settings.ini");
                for (int i = 0; i < html.Split('=').Length; i += 2)
                    Share.settings.Add(html.Split('=')[i], html.Split('=')[i + 1]);
                if (!File.Exists(appdir + "evnets.dta"))
                    html = Gethtml(Share.settings["dataLink"] + "ver.html", 0);
                else
                {
                    html = Gethtml("Share.settings[\"dataLink\"] + \"ver.html", 10);
                    if (File.ReadAllText(appdir + "ver.dta").Equals(html))
                        throw new Exception();
                }
                File.WriteAllText(appdir + "ver.dta", html);
                html = Gethtml(Share.settings["dataLink"] + "emudata.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                DataFiles.emuDatas = replace(html, '\r').Split('\n');
                File.WriteAllText(appdir + "emudata.dta", html);
                html = Gethtml(Share.settings["dataLink"] + "railwaydata.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                DataFiles.railDatas = replace(html, '\r').Split('\n');
                File.WriteAllText(appdir + "railwaydata.dta", html);
                html = Gethtml(Share.settings["dataLink"] + "replace.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                replaceDatas = replace(html, '\r').Split('\n');
                File.WriteAllText(appdir + "replace.dta", html);
                html = Gethtml(Share.settings["dataLink"] + "rcp.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                rcps = replace(html, '\r').Split('\n');
                File.WriteAllText(appdir + "rcp.dta", html);
                html = Gethtml(Share.settings["dataLink"] + "smallstations.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                ssstrs = replace(html, '\r').Split('\n');
                File.WriteAllText(appdir + "smallstations.dta", html);
                html = Gethtml(Share.settings["dataLink"] + "evnets.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                evens = replace(html, '\r').Split('\n');
                File.WriteAllText(appdir + "evnets.dta", html);
            }
            catch (Exception e)
            {
                //System.exit(1);
                if (!File.Exists(appdir + "evnets.dta"))
                    this.Close();
                else
                {
                    try
                    {
                        html = File.ReadAllText(appdir + "emudata.dta");
                        DataFiles.emuDatas = replace(html, '\r').Split('\n');
                        html = File.ReadAllText(appdir + "railwaydata.dta");
                        DataFiles.railDatas = replace(html, '\r').Split('\n');
                        html = File.ReadAllText(appdir + "replace.dta");
                        replaceDatas = replace(html, '\r').Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        html = File.ReadAllText(appdir + "rcp.dta");
                        rcps = replace(html, '\r').Split('\n');
                        html = File.ReadAllText(appdir + "smallstations.dta");
                        ssstrs = replace(html, '\r').Split('\n');
                        html = File.ReadAllText(appdir + "evnets.dta");
                        evens = replace(html, '\r').Split('\n');
                    }
                    catch (Exception e2)
                    {
                        this.Close();
                    }
                }
            }
            try
            {
                if (rcps.Length > 0)
                {
                    selectC = 0;
                    cbo_rcp.Text = (rcps[0]);
                }
                for (int i = 0; i < evens.Length; i++)
                {
                    tmp3 = evens[i].Split(' ');
                    //es.Add();
                    tmp4 = new Even(int.Parse(tmp3[0]), int.Parse(tmp3[1]), int.Parse(tmp3[2]), tmp3[3]).IsNow();
                    if (tmp4 != -1)
                        lbl_event.Text = ((lbl_event.Text.ToString().Equals("") ? "今天是" : "\n今天是") + tmp3[3] + tmp4.ToString() + "周年");
                }
                for (int i = 0; i < DataFiles.emuDatas.Length; i++)
                {
                    if (i == 11)
                        i = 11;
                    emustrs = DataFiles.emuDatas[i].Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
                    trains = new List<List<Train>>();
                    ver = new List<string>();
                    for (int j = 4, x = 0; j < emustrs.Length; j += 2, x++)
                    {
                        trainstrs = emustrs[j].Split(' ');
                        trains.Add(new List<Train>());
                        ver.Add(emustrs[j - 1]);
                        for (int k = 0; k < trainstrs.Length; k++)
                        {
                            seatstrs = trainstrs[k].Split('-');
                            persons.Clear();
                            seatTypes.Clear();
                            for (int l = 0; l < seatstrs.Length; l += 2)
                            {
                                persons.Add(Byte.Parse(seatstrs[l + 1]));
                                seatTypes.Add(GetSeatType(seatstrs[l]));
                            }
                            trains[x].Add(new Train(persons, seatTypes, (byte)(k + 1)));
                        }
                    }
                    Share.emus.Add(new EMU(Double.Parse(emustrs[1]), emustrs[0], trains, ver, int.Parse(emustrs[2])));
                }
                if (Share.emus.Count > 0)
                {
                    train = Share.emus[0].name;
                    this.ver = Share.emus[0].versions[0];
                    cbo_trainType.Text = (train + "(" + this.ver + ")");
                    if (Share.emus[0].trains[0].Count > 9)
                        cbx_isTwoTrain.Enabled = (false);
                    else
                        cbx_isTwoTrain.Enabled = (true);
                }
                foreach (EMU item2 in Share.emus)
                    emus.Add(item2.name);
                Share.stations = new List<Station>();
                for (int i = 0; i < DataFiles.railDatas.Length; i++, y++)
                {
                    railstr = JObject.Parse(DataFiles.railDatas[i]);
                    Share.rails.Add(new Rail());
                    Share.rails[i].name = (String)railstr["lineName"];
                    Share.rails[i].speeds = (string)((JObject)railstr["serviceTime"])["up"];
                    Share.rails[i].locations = (JArray)((JObject)railstr["route"])[("up")];
                    Share.rails[i].lengths = new List<double>();
                    Share.rails[i].stations = new List<String>();
                    length = 0;
                    for (int j = 0; j < Share.rails[i].locations.Count; j++)
                    {
                        item = (JObject)Share.rails[i].locations[(j)];
                        tmp = item;
                        String str = (string)tmp[("type")];
                        ////Log.w("tag", tmp.getString("type").Equals("station") ? "t" : "f");
                        if (((string)tmp[("type")]).Equals("station"))
                        {
                            Share.rails[i].stations.Add((string)tmp[("name")]);
                            if (j > 0)
                                Share.rails[i].lengths.Add(length);
                            length = 0;
                            if (!((((string)tmp[("name")]).ToLower().StartsWith("x"))))
                            {
                                b = false;
                                int k = 0;
                                foreach (Station item2 in Share.stations)
                                {
                                    if (item2.name.Equals((string)tmp[("name")]))
                                    {
                                        b = true;
                                        Share.stations[k].AddRail(new int[] { i });
                                        break;
                                    }
                                    k++;
                                }
                                if (!b)
                                {
                                    station = new Station((string)tmp[("name")]);
                                    station.AddRail(new int[] { i });
                                    Share.stations.Add(station);
                                }
                                //Share.lists.Add((tmp.getString("name"))[0], new List<int>());
                                //if (!b)
                                //    Share.lists.GetE((tmp.getString("name"))[0]).Add(k);
                            }
                        }
                        if (j < Share.rails[i].locations.Count - 1)
                        {
                            length += GetLength(((double)tmp[("lng")]), ((double)tmp[("lat")]),
                            (double)((JObject)Share.rails[i].locations[(j + 1)])[("lng")],
                            (double)((JObject)Share.rails[i].locations[(j + 1)])[("lat")]);
                        }
                    }
                }
                for (int i = 0; i < ssstrs.Length; i++)
                {
                    temp = ssstrs[i].Split(' ');
                    for (int j = 0; j < Share.stations.Count; j++)
                    {
                        if (Share.stations[j].name.Equals(temp[0]))
                        {
                            ssns.Add(j);
                            break;
                        }
                    }
                    sss.Add(new List<List<int>>());
                    for (int j = 1; j < temp.Length; j++)
                    {
                        temp2 = temp[j].Split('&');
                        sss[i].Add(new List<int>());
                        for (int k = 0; k < temp2.Length; k++)
                        {
                            for (int l = 0; l < Share.rails.Count; l++)
                            {
                                if (Share.rails[l].name.Equals(temp2[k]))
                                {
                                    sss[i][j - 1].Add((l));
                                    break;
                                }
                            }
                        }
                    }
                }
                foreach (String item2 in replaceDatas)
                {
                    if (item2.StartsWith("*"))
                    {
                        dic2.Add(item2.Substring(1).Split(' ')[0], item2.Split(' ')[1]);
                        dk2.Add(item2.Substring(1).Split(' ')[0]);
                    }
                    else
                    {
                        dic.Add(item2.Split(' ')[0], item2.Split(' ')[1]);
                        dk1.Add(item2.Split(' ')[0]);
                    }
                }
                foreach (var rci in rcps)
                {
                    cbo_rcp.Items.Add(rci);
                }
                cbo_rcp.SelectedIndex = 0;
                //StringBuilder sb2 = new StringBuilder();
                for (int i = 0; i < Share.rails.Count; i++)
                {
                    spdstrs = Share.rails[i].speeds.Split(new char[] { '\n' });
                    for (int j = 0; j < spdstrs.Length; j++)
                    {
                        if (spdstrs[j].StartsWith("1") || spdstrs[j].StartsWith("2") || spdstrs[j].StartsWith("3") || spdstrs[j].StartsWith("4") || spdstrs[j].StartsWith("5") ||
                            spdstrs[j].StartsWith("6") || spdstrs[j].StartsWith("7") || spdstrs[j].StartsWith("8") || spdstrs[j].StartsWith("9"))
                        {
                            spdstrs[j] = "atp " + spdstrs[j] + "\n" +
                                "ms " + spdstrs[j] + "\n" +
                                "ss " + spdstrs[j];
                        }
                    }
                    Share.rails[i].speeds = "";
                    for (int j = 0; j < spdstrs.Length; j++)
                    {
                        Share.rails[i].speeds += spdstrs[j];
                        if (j + 1 < spdstrs.Length)
                            Share.rails[i].speeds += "\n";
                    }
                    /*for (int j = 0; j < Share.rails[i].lengths.Count; j++)
                    {
                        sb2.Append(Share.rails[i].name);
                        sb2.Append("    ");
                        sb2.Append(Share.rails[i].stations[j]);
                        sb2.Append("~");
                        sb2.Append(Share.rails[i].stations[j + 1]);
                        sb2.Append("  ");
                        sb2.Append(Share.rails[i].lengths[j]);
                        sb2.Append("km\r\n");
                    }
                    sb2.Append(Share.rails[i].speeds);
                    sb2.Append("\r\n\r\n");*/
                }
                //File.WriteAllText("rds.txt", sb2.ToString());
            }
            catch (Exception e)
            {
                //System.exit(1);
            }
            flag = false;
            foreach (EMU item6 in Share.emus)
            {
                cbo_trainType.Items.Add(item6.name);
            }
            cbo_trainType.SelectedIndex = 0;
            cbo_trainSeat.SelectedIndex = 0;
            if (file != "")
                Open(file);
            /*int t1 = 0; int t2 = 0;
            for (int k = 0; k < Share.stations.Count; k++)
            {
                if (Share.stations[k].name == "广州南")
                {
                    t1 = k;
                    break;
                }
            }
            for (int k = 0; k < Share.stations.Count; k++)
            {
                if (Share.stations[k].name == "哈尔滨")
                {
                    t2 = k;
                    break;
                }
            }
            this.lis = AStar(t1, t2, false);*/
                //textBox1_Click(sender, e);
        }
        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void onFocusChange(bool hasFocus)
        {
            if (!hasFocus)
            {
                if (tbx_before.Text.Length > 0)
                {
                    bool b = false;
                    int i = 0;
                    foreach (Station item in Share.stations)
                    {
                        if (item.name
                                .Equals(tbx_before.Text.ToString())
                                && (!(item.name.ToLower().StartsWith("x"))))
                        {
                            b = true;
                            break;
                        }
                        i++;
                    }
                    if (selectB != i)
                    {
                        selectL = -1;
                        selectA = -1;
                        cbo_line.Text = ("");
                        cbo_after.Text = ("");
                        btn_submit.Enabled = (false);
                    }
                    selectB = i;
                    if (!b)
                    {
                        //tbx_before.Text = ("");
                        //new AlertDialog.Builder(MainActivity.this).setTitle("未找到起点站").create().show();
                    }
                }
                else
                {
                    selectL = -1;
                    selectA = -1;
                    cbo_line.Text = ("");
                    cbo_after.Text = ("");
                    btn_submit.Enabled = (false);
                }
            }
        }
        private void cbo_line_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbo_line.SelectedIndex == -1)
                return;
            List<String> strs = new List<String>();
            bool b = false;
            bool bIsSmall = false;
            bool b1 = false;
            int i = 0;
            if (lis.Count != 0)
            {
                for (i = 0; i < ssns.Count; i++)
                {
                    if (ssns[i] == lis[(lis.Count - 1)].after)
                    {
                        bIsSmall = true;
                        bool zc = false;
                        for (int j = 0; j < sss[i].Count; j++)
                        {
                            for (int k = 0; k < sss[i][j].Count; k++)
                            {
                                if ((zc == false) && (sss[i][j][k] == lis[(lis.Count - 1)].line))
                                {
                                    zc = true;
                                    b = true;
                                    k = 0;
                                    //strs.Add(Share.rails.get(lis[(lis.Count - 1)].line).name);
                                }
                                b1 = false || (strs.Count == 0);
                                for (int l = 0; l < strs.Count; l++)
                                {
                                    b1 = b1 || (!Share.rails[sss[i][j][k]].name.Equals(strs[l]));
                                    if ((!Share.rails[sss[i][j][k]].name.Equals(strs[l])) == false)
                                    {
                                        b1 = false;
                                        break;
                                    }
                                }
                                if (zc && b1)
                                    strs.Add(Share.rails[sss[i][j][k]].name);
                            }
                            zc = false;
                            b1 = false;
                        }
                        break;
                    }
                }
            }
            if ((bIsSmall == false) || (b == false))
            {
                foreach (Station item in Share.stations)
                {
                    if (item.name
                            .Equals(tbx_before.Text.ToString())
                            && (!(item.name.ToLower().StartsWith("x"))) && ((!(item.name.ToLower().EndsWith("线路所"))) || (!tbx_before.Enabled)))
                    {
                        b = true;
                        foreach (int items in item.rails)
                        {
                            strs.Add(Share.rails[(items)].name);
                        }
                    }
                }
            }
            i = 0;
            foreach (Rail rail in Share.rails)
            {
                if (rail.name.Equals(strs[cbo_line.SelectedIndex]))
                    break;
                i++;
            }
            if (selectL != i)
            {
                selectA = -1;
                cbo_after.Text = ("");
                btn_submit.Enabled = (false);
            }
            selectL = i;
            cbo_line.Text = (strs[cbo_line.SelectedIndex]);
            cbo_after.Enabled = (true);
        }
        private void cbo_after_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbx_isAstar.Checked)
            {
                if ((!String.IsNullOrEmpty(cbo_after.Text)) && (!String.IsNullOrEmpty(tbx_before.Text)))
                    btn_submit.Enabled = true;
                return;
            }
            List<String> strs = new List<String>();
            foreach (String items in Share.rails[(selectL)].stations)
            {
                if (!(items.Equals(Share.stations[(selectB)].name)))
                {
                    if ((!items.StartsWith("x")))
                        strs.Add(items);
                }
            }
            int i = 0;
            foreach (Station rail in Share.stations)
            {
                if (rail.name.Equals(strs[cbo_after.SelectedIndex]))
                    break;
                i++;
            }
            selectA = i;
            cbo_after.Text = (strs[cbo_after.SelectedIndex]);
            btn_submit.Enabled = (true);
            if (strs[cbo_after.SelectedIndex].EndsWith("线路所"))
            {
                tbx_afterstop.Text = ("0");
                //tbx_afterstop.Enabled=(false);
            }
            else
            {
                tbx_afterstop.Text = ("2");
                tbx_afterstop.Enabled = (true);
            }
        }
        private void cbo_line_Click(object sender, EventArgs e)
        {
            //onFocusChange(false);
            List<String> strs = new List<String>();
            bool b = false;
            bool bIsSmall = false;
            bool b1 = false;
            if (lis.Count != 0)
            {
                for (int i = 0; i < ssns.Count; i++)
                {
                    if (ssns[i] == lis[(lis.Count - 1)].after)
                    {
                        bIsSmall = true;
                        bool zc = false;
                        for (int j = 0; j < sss[i].Count; j++)
                        {
                            for (int k = 0; k < sss[i][j].Count; k++)
                            {
                                if ((zc == false) && (sss[i][j][k] == lis[(lis.Count - 1)].line))
                                {
                                    zc = true;
                                    b = true;
                                    k = 0;
                                    //strs.Add(Share.rails.get(lis[(lis.Count - 1)].line).name);
                                }
                                b1 = false || (strs.Count == 0);
                                for (int l = 0; l < strs.Count; l++)
                                {
                                    b1 = b1 || (!Share.rails[sss[i][j][k]].name.Equals(strs[l]));
                                    if ((!Share.rails[sss[i][j][k]].name.Equals(strs[l])) == false)
                                    {
                                        b1 = false;
                                        break;
                                    }
                                }
                                if (zc && b1)
                                    strs.Add(Share.rails[sss[i][j][k]].name);
                            }
                            zc = false;
                            b1 = false;
                        }
                        break;
                    }
                }
            }
            if ((bIsSmall == false) || (b == false))
            {
                selectB = 0;
                foreach (Station item in Share.stations)
                {
                    if (item.name
                            .Equals(tbx_before.Text.ToString())
                            && (!(item.name.ToLower().StartsWith("x"))) && ((!(item.name.ToLower().EndsWith("线路所")))))
                    {
                        b = true;
                        foreach (int items in item.rails)
                        {
                            strs.Add(Share.rails[(items)].name);
                        }
                        break;
                    }
                    selectB++;
                }
                if (b == false)
                    selectB = -1;
            }
            if (b == false)
            {
                tbx_before.Text = ("");
                MessageBox.Show("未找到起点站", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                int tmp2 = 0;
                if (selectL > -1)
                {
                    foreach (String i in strs)
                    {
                        if (Share.rails[(selectL)].name.Equals(i))
                            break;
                        tmp2++;
                    }
                }
                cbo_line.Items.Clear();
                cbo_line.Items.AddRange(strs.ToArray());
            }
        }
        private void cbo_after_Click(object sender, EventArgs e)
        {
            if (cbx_isAstar.Checked)
                return;
            List<String> strs = new List<String>();
            bool b = false;
            //Toast.makeText(this, " ", Toast.LENGTH_SHORT).show();
            foreach (String items in Share.rails[(selectL)].stations)
            {
                if (!(items.Equals(Share.stations[(selectB)].name)))
                {
                    if ((!items.StartsWith("x")))
                        strs.Add(items);
                }
            }
            cbo_after.Items.Clear();
            cbo_after.Items.AddRange(strs.ToArray());
            int tmp2 = 0;
            if (selectA > -1)
            {
                foreach (String i in strs)
                {
                    if (Share.stations[(selectA)].name.Equals(i))
                        break;
                    tmp2++;
                }
            }
        }
        private void btn_submit_Click(object sender, EventArgs e)
        {
            RunMode mode = RunMode.Express;
            保存SToolStripMenuItem.Enabled = (true);
            导出EToolStripMenuItem.Enabled = (true);
            if (rb_atp.Checked)
                mode = RunMode.MaxSpeed;
            else if (rb_max.Checked)
                mode = RunMode.MaxSpeed;
            else if (rb_exp.Checked)
                mode = RunMode.Express;
            if (string.IsNullOrEmpty(tbx_beforestop.Text))
                tbx_beforestop.Text = "0";
            if (string.IsNullOrEmpty(tbx_afterstop.Text))
                tbx_afterstop.Text = "0";
            if (string.IsNullOrEmpty(tbx_etim.Text))
                tbx_etim.Text = "0";
            if (cbx_isAstar.Checked)
            {
                List<ListItem> liss;
                int tmp1 = 0;
                int tmp2 = 1;
                int i = 0;
                foreach (Station stat in Share.stations)
                {
                    if (stat.name.Equals(tbx_before.Text))
                        tmp1 = i;
                    if (stat.name.Equals(cbo_after.Text))
                        tmp2 = i;
                    i++;
                }
                if (select)
                {
                    liss = AStar(tmp2, tmp1, int.Parse(tbx_afterstop.Text.ToString()), int.Parse(tbx_beforestop.Text.ToString()), int.Parse(tbx_etim.Text.ToString()), mode, cbx_afterteg.Checked, cbx_beforeteg.Checked);
                }
                else
                {
                    liss = AStar(tmp1, tmp2, int.Parse(tbx_beforestop.Text.ToString()), int.Parse(tbx_afterstop.Text.ToString()), int.Parse(tbx_etim.Text.ToString()), mode, cbx_beforeteg.Checked, cbx_afterteg.Checked);
                }
                if (liss.Count == 0)
                {
                    //MessageBox.Show("未找到路径！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                for (i = 0; i < liss.Count; i++)
                {
                    lis.Add(liss[i]);
                }
                UpdateData();
                btn_submit.Enabled = false;
                tbx_before.Text = cbo_after.Text;
                tbx_before.Enabled = false;
                cbo_after.SelectedIndex = -1;
                SetTime();
                return;
            }
            if (select == true)
            {
                ListItem li = new ListItem(selectA,
                        int.Parse(tbx_afterstop.Text.ToString()),
                        selectL,
                        mode
                        , selectB,
                        int.Parse(tbx_beforestop.Text.ToString()),
                        int.Parse(tbx_etim.Text.ToString()),
                        cbx_afterteg.Checked,
                        cbx_beforeteg.Checked);
                if (lis.Count > 0)
                    lis.Insert(0, li);
                else
                    lis.Add(li);
            }
            else
                lis.Add(new ListItem(selectB,
                    int.Parse(tbx_beforestop.Text.ToString())
                    , selectL,
                    mode
                    , selectA,
                    int.Parse(tbx_afterstop.Text.ToString()),
                    int.Parse(tbx_etim.Text.ToString()),
                    cbx_beforeteg.Checked,
                    cbx_afterteg.Checked));
            tbx_before.Text = (Share.stations[(selectA)].name);
            UpdateData();
            selectB = selectA;
            selectL = -1;
            selectA = -1;
            cbo_line.Text = ("");
            cbo_after.Text = ("");
            btn_submit.Enabled = (false);
            cbo_after.Enabled = (false);
            cbo_after.Items.Clear();
            cbo_line.Items.Clear();
            tbx_before.Enabled = (false);
            (tbx_beforestop).Text = ((select ? lis[0].beforestop : lis[(lis.Count - 1)].afterstop).ToString());
            (tbx_beforestop).Enabled = (false);
            (tbx_afterstop).Text = ("2");
            (tbx_etim).Text = ("5");
            cbx_beforeteg.Checked = cbx_afterteg.Checked;
            cbx_beforeteg.Enabled = (false);
            cbx_afterteg.Checked = (false);
            rb_exp.Checked = (true);
            tbx_afterstop.Enabled = (true);
            SetTime();
        }
        public void Out(bool isshow, string path)
        {
            StringBuilder sb = new StringBuilder();
            JArray jArray, j2;
            String name;
            JObject tmp;
            bool b = false;
            bool b1 = false;
            int j = 0;
            j2 = new JArray();
            //progressDialog.setProgress(//progressDialog.getProgress() + 1);
            sb.Append("{\"fileVersion\":1,\"cityName\":\"中华人民共和国\",\"lineName\":\"");
            sb.Append(tbx_trainNum.Text);
            sb.Append("\",\"lineColor\":\"#");
            EMU e2 = null;
            foreach (EMU e in Share.emus)
            {
                if (e.name.Equals(train))
                {
                    e2 = e;
                    break;
                }
            }
            if (e2.speed > 310)
                sb.Append("CC0000");
            else if (e2.speed > 290)
                sb.Append("FF3300");
            else if (e2.speed > 230)
                sb.Append("FF6600");
            else if (e2.speed > 190)
                sb.Append("FF9900");
            else if (e2.speed > 150)
                sb.Append("CBCB00");
            else
                sb.Append("33CC33");
            sb.Append("\",\"remark\":\"采用");
            sb.Append(train);
            if (!this.ver.Contains("&"))
                sb.Append("(" + this.ver + ")");
            if (cbx_isTwoTrain.Checked)
                sb.Append("重联");
            if (((!this.ver.EndsWith("型")) && (!this.ver.EndsWith("版"))) || (cbx_isTwoTrain.Checked))
                sb.Append("型");
            sb.Append("列车\",\"lineType\":1");
            sb.Append(",\"company\":\"");
            sb.Append(rcps[selectC]);
            sb.Append("\",\"route\":{\"up\":[");
            //progressDialog.setProgress(//progressDialog.getProgress() + 1);
            int i = 0;
            int k = 0;
            int tt = 0;
            try
            {
                for (i = 0; i < lis.Count; i++)
                {
                    b1 = false;
                    jArray = Share.rails[(lis[i].line)].locations;
                    //j = 0;
                    b = false;
                    for (j = 0; j < jArray.Count;)
                    {
                        if (((string)((JObject)jArray[j])["name"]).Equals(Share.stations[(lis[i].before)].name))
                            b = true;
                        tmp = (JObject)jArray[j];
                        if (((string)((JObject)jArray[j])["name"]).Equals(Share.stations[(lis[i].after)].name))
                        {
                            if (b == false)
                            {
                                b1 = true;
                                jArray = new JArray();
                                for (int t = Share.rails[(lis[i].line)].locations.Count - 1; t >= 0; t--)
                                {
                                    jArray.Add((JObject)Share.rails[(lis[i].line)].locations[(t)]);
                                }
                                /*bool c = false;
                                for (k = 0; k < jArray.Length; ) {
                                    if (!jArray.getJObject(k).getString("name").Equals(Share.stations[(lis[i].before)].name))
                                        c = true;
                                    tmp = jArray.getJObject(k);
                                    if (jArray.getJObject(k).getString("name").Equals(Share.stations[(lis[i].after)].name)) {
                                        sb.Append(',');
                                        if (i == lis.Count - 1)
                                            sb.Append(tmp.ToString());
                                        break;
                                    }
                                    if (c) {
                                        if (!jArray.getJObject(k).getString("name").Equals(Share.stations[(lis[i].before)].name))
                                            tmp.put("type", "waypoint");
                                        sb.Append(tmp.ToString());
                                        if (!jArray.getJObject(k+1).getString("name").Equals(Share.stations[(lis[i].after)].name))
                                            sb.Append(",");
                                    }
                                    k++;
                                }*/
                                j = 0;
                                b = false;
                                continue;
                            }
                            sb.Append(',');
                            if (i == lis.Count - 1)
                            {
                                if (isshow)
                                    tmp["name"] = (((string)((JObject)jArray[j])["name"]) + " " + ToTime(uptimes[tt].arriveTime) + "到");
                                j2.Add(tmp);
                                sb.Append(tmp.ToString());
                            }
                            break;
                        }
                        if (b)
                        {
                            if ((!((string)((JObject)jArray[j])["name"]).Equals(Share.stations[(lis[i].before)].name))
                                    || (((string)((JObject)jArray[j])["name"]).Equals(Share.stations[(lis[i].before)].name)
                                    && (lis[i].beforestop == 0)))
                                tmp["type"] = "waypoint";
                            else if (isshow)
                            {
                                if (i == 0)
                                    tmp["name"] = (((string)((JObject)jArray[j])["name"]) + " " + ToTime(uptimes[tt].deparTime) + "开");
                                else
                                    tmp["name"] = (((string)((JObject)jArray[j])["name"]) + " " + ToTime(uptimes[tt].arriveTime) + "到 " + ToTime(uptimes[tt].deparTime) + "开" + (uptimes[tt].teg ? "技停" : ""));
                                tt++;
                            }
                            sb.Append(tmp.ToString());
                            j2.Add(tmp);
                            if (!((string)((JObject)jArray[(j + 1)])[("name")]).Equals(Share.stations[(lis[i].after)].name))
                                sb.Append(",");
                        }
                        j++;
                    }
                    if (b1)
                    {
                        b1 = false;
                    }
                    //progressDialog.setProgress(//progressDialog.getProgress() + 1);
                }
                sb.Append("],\"down\":[");
                tt = 0;
                //if(!isshow) {
                for (int t = j2.Count - 1; t >= 0; t--)
                {
                    if (isshow && ((string)((JObject)j2[t])["type"]).Equals("station"))
                    {
                        name = ((string)((JObject)j2[t])["name"]).Split(' ')[0];
                        if (t == j2.Count - 1)
                        {
                            ((JObject)j2[t]).Add("name", name + " " + ToTime(downtimes[tt].deparTime) + "开");
                        }
                        else if (t == 0)
                        {
                            ((JObject)j2[t]).Add("name", name + " " + ToTime(downtimes[tt].arriveTime) + "到");
                        }
                        else
                        {
                            ((JObject)j2[t]).Add("name", name + " " + ToTime(downtimes[tt].arriveTime) + "到 " + ToTime(downtimes[tt].deparTime) + "开" + (downtimes[tt].teg ? "技停" : ""));
                        }
                        tt++;
                    }
                    sb.Append(j2[t]);
                    if (t > 0)
                        sb.Append(',');
                }
                //}
                sb.Append("]},\"serviceTime\":{\"up\":\"");
                sb.Append(upTime.Replace("\n", "\\n").Replace("\r", ""));
                sb.Append("\",\"down\":\"");
                sb.Append(downTime.Replace("\n", "\\n").Replace("\r", ""));
                sb.Append("\"},\"fare\":{\"strategy\":\"multilevel\",\"enableRing\":\"0\",\"desc\":\"在“票价”面板设置\",\"single\":{\"price\":\"1.00\"},\"multilevel\":{\"startPrice\":\"0.00\",\"startingDistance\":\"0\",\"magnification\":\"0.35\",\"magnificationAttenuation\":\"0.00\",\"increaseBase\":\"0.001\",\"maxPrice\":\"Infinity\"},\"text\":{\"text\":\"\"},\"customize\":{\"formula\":\"0.2*distance\"}}}");
                File.WriteAllText(path
                        + "\\" + tbx_trainNum.Text
                        .ToString().Replace("/", "_") + ".bll", sb.ToString());
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception e)
            {
                MessageBox.Show("导出失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        protected void SetTime()
        {
            upTime = CalcTime(upmin, uphour, true);
            tbx_uptime.Text = upTime;
            downTime = CalcTime(downmin, downhour, false);
            tbx_downtime.Text = downTime;
        }
        private void 删除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lis.Count > 0)
            {
                if (select)
                {
                    lis.RemoveAt((int)0);
                    //lbx_list.Items.RemoveAt(0);
                }
                else
                {
                    lis.RemoveAt(lis.Count - 1);
                    //lbx_list.Items.RemoveAt(lis.Count - 1);
                }
                UpdateData();
                if (lis.Count < 1)
                {
                    selectB = -1;
                    tbx_before.Enabled = (true);
                    保存SToolStripMenuItem.Enabled = (false);
                    导出EToolStripMenuItem.Enabled = (false);
                    tbx_before.Text = ("");
                    (tbx_beforestop).Text = ("2");
                    (tbx_beforestop).Enabled = (true);
                }
                else if (!select)
                {
                    selectB = int.Parse((lis[(lis.Count - 1)].after).ToString());
                    (tbx_beforestop).Text = ((lis[(lis.Count - 1)].afterstop).ToString());
                }
                else
                {
                    selectB = int.Parse((lis[0].before).ToString());
                    (tbx_beforestop).Text = ((lis[0].beforestop).ToString());
                }
                if (lis.Count > 0)
                {
                    tbx_before.Text = (Share.stations[(selectB)].name);
                    cbx_beforeteg.Checked = (select ? lis[0].beforeteg : lis[(lis.Count - 1)].beforeteg);
                }
                selectL = selectA = -1;
                cbo_line.Text = ("");
                cbo_after.Text = ("");
                btn_submit.Enabled = (false);
                cbo_after.Enabled = (false);
                (tbx_afterstop).Text = ("2");
                (tbx_etim).Text = ("5");
                cbx_afterteg.Checked = (false);
                rb_exp.Checked = (true);
                if(cbx_isAstar.Checked)
                    cbo_after.Enabled = true;
                SetTime();
            }
        }
        private void cbo_rcp_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = cbo_rcp.SelectedIndex;
            selectC = i;
            cbo_rcp.Text = (rcps[cbo_rcp.SelectedIndex]);
        }
        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "列车线路文件(*.tlf)|*.tlf";
            ofd.ShowDialog();
            if (string.IsNullOrEmpty(ofd.FileName))
                return;
            Open(ofd.FileName);
        }
        public void Open(String path)
        {
            try
            {
                String s = File.ReadAllText(path);
                String[] str = s
                    .Replace("\r", "").Split('\n');
                String[] strs, sts;
                lis.Clear();
                bool b7;
                ListItem li;
                uptimes = new List<TimeTime>();
                downtimes = new List<TimeTime>();
                for (int i = 0; i < rcps.Length; i++)
                {
                    if (rcps[i].Equals(str[0]))
                    {
                        selectC = i;
                        cbo_rcp.SelectedIndex = i;
                        break;
                    }
                    if (i == rcps.Length - 1)
                        throw new Exception("版本不兼容");
                }
                strs = str[1].Split(' ');
                uphour = int.Parse(strs[0]);
                upmin = int.Parse(strs[1]);
                String tmp1 = upmin < 10 ? "0" + (upmin).ToString() : (upmin).ToString();
                String tmp2 = uphour < 10 ? "0" + (uphour).ToString() : (uphour).ToString();
                tbx_uptime.Text = (tmp2 + ":" + tmp1);
                strs = str[2].Split(' ');
                downhour = int.Parse(strs[0]);
                downmin = int.Parse(strs[1]);
                tmp1 = downmin < 10 ? "0" + (downmin).ToString() : (downmin).ToString();
                tmp2 = downhour < 10 ? "0" + (downhour).ToString() : (downhour).ToString();
                tbx_downtime.Text = (tmp2 + ":" + tmp1);
                train = str[3];
                ver = str[4];
                int tmp5 = 0, which = 0;
                foreach (EMU emu in Share.emus)
                {
                    if (train.Equals(emu.name))
                        break;
                    tmp5++;
                }
                foreach (String v in Share.emus[tmp5].versions)
                {
                    if (v.Equals(ver))
                        break;
                    which++;
                }
                cbo_trainType.SelectedIndex = tmp5;
                cbo_trainSeat.SelectedIndex = which;
                if (Share.emus[tmp5].trains[which].Count > 9)
                {
                    cbx_isTwoTrain.Enabled = (false);
                    cbx_isTwoTrain.Checked = (false);
                }
                else
                    cbx_isTwoTrain.Enabled = (true);
                sts = str[5].Split(' ');
                if (sts.Length == 2)
                {
                    if (sts[0].Equals("t"))
                        cbx_isTwoTrain.Checked = true;
                    else
                        cbx_isTwoTrain.Checked = false;
                    if (sts[1].Equals("t"))
                        cbx_no350mode.Checked = true;
                    else
                        cbx_no350mode.Checked = false;
                }
                else
                {
                    if (str[5].Equals("t"))
                        cbx_isTwoTrain.Checked = (true);
                    else
                        cbx_isTwoTrain.Checked = (false);
                    cbx_no350mode.Checked = false;
                }
                tbx_trainNum.Text = (str[6]);
                for (int i = 7; i < str.Length; i++)
                {
                    strs = str[i].Split(' ');
                    li = new ListItem(0, 0, 0, RunMode.MaxSpeed, 0, 0, 0, false, false);
                    b7 = false;
                    for (int j = 0; j < Share.stations.Count; j++)
                    {
                        if (Share.stations[j].name.Equals(strs[0]))
                        {
                            li.before = j;
                            b7 = true;
                            break;
                        }
                        if (j == Share.stations.Count - 1)
                        {
                            foreach (String stat in dk1)
                            {
                                if (stat.Equals(strs[0]))
                                {
                                    strs[0] = stat;
                                    b7 = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!b7)
                        throw new Exception("版本不兼容");
                    b7 = false;
                    for (int j = 0; j < Share.stations.Count; j++)
                    {
                        if (Share.stations[j].name.Equals(strs[2]))
                        {
                            li.after = j;
                            b7 = true;
                            break;
                        }
                        if (j == Share.stations.Count - 1)
                        {
                            foreach (String stat in dk1)
                            {
                                if (stat.Equals(strs[2]))
                                {
                                    strs[2] = stat;
                                    b7 = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!b7)
                        throw new Exception("版本不兼容");
                    b7 = false;
                    for (int j = 0; j < Share.rails.Count; j++)
                    {
                        if (Share.rails[j].name.Equals(strs[1]))
                        {
                            li.line = j;
                            b7 = true;
                            break;
                        }
                        if (j == Share.rails.Count - 1)
                        {
                            foreach (String stat in dk2)
                            {
                                if (stat.Equals(strs[1]))
                                {
                                    strs[1] = stat;
                                    b7 = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!b7)
                        throw new Exception("版本不兼容");
                    li.afterstop = int.Parse(strs[5]);
                    li.beforestop = int.Parse(strs[3]);
                    li.beforeteg = strs[4].Equals("t") ? true : false;
                    li.afterteg = strs[6].Equals("t") ? true : false;
                    li.earlytime = int.Parse(strs[7]);
                    switch (strs[8])
                    {
                        case "a":
                            li.mode = RunMode.MaxSpeed;
                            break;
                        case "m":
                            li.mode = RunMode.MaxSpeed;
                            break;
                        case "s":
                            li.mode = RunMode.Express;
                            break;
                    }
                    selectB = li.after;
                    lis.Add(li);
                    //progressDialog.setProgress(//progressDialog.getProgress() + 1);
                }
                保存SToolStripMenuItem.Enabled = (true);
                导出EToolStripMenuItem.Enabled = (true);
                selectA = -1;
                selectL = -1;
                if (select)
                {
                    select = false;
                    ((Button)((btn_up))).BackColor = Color.FromArgb(12, 8, 178);
                    ((Button)((btn_down))).BackColor = Color.FromArgb(15, 175, 207);
                }
                UpdateData();
                SetTime();
                tbx_before.Text = (Share.stations[(selectB)].name);
                cbo_line.Text = ("");
                cbo_after.Text = ("");
                btn_submit.Enabled = (false);
                cbo_after.Enabled = (false);
                tbx_before.Enabled = (false);
                (tbx_beforestop).Text = ((lis[(lis.Count - 1)].afterstop).ToString());
                (tbx_beforestop).Enabled = (false);
                (tbx_afterstop).Text = ("2");
                cbx_beforeteg.Checked = (cbx_afterteg.Checked);
                cbx_beforeteg.Enabled = (false);
                cbx_afterteg.Checked = (false);
                rb_exp.Checked = (true);
                //progressDialog.setProgress(MAX_PROGRESS);
                //progressDialog.cancel();
            }
            catch (Exception e2)
            {
                MessageBox.Show("打开失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void UpdateData()
        {
            if (lis.Count > 0)
            {
                lbx_list.Items.Clear();
                lbx_list.Items.Add(Share.stations[lis[0].before].name);
                foreach (ListItem li in lis)
                {
                    lbx_list.Items.Add(Share.stations[(li.after)].name);
                }
            }
            else
                lbx_list.Items.Clear();
        }
        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (string.IsNullOrEmpty(fbd.SelectedPath))
                return;
            String path = fbd.SelectedPath
                    + "\\" + tbx_trainNum.Text
                    .ToString().Replace("/", "_") + ".tlf";
            StringBuilder sb2 = new StringBuilder();
            sb2.Append(rcps[selectC]);
            sb2.Append("\n");
            sb2.Append(uphour);
            sb2.Append(" ");
            sb2.Append(upmin);
            sb2.Append("\n");
            sb2.Append(downhour);
            sb2.Append(" ");
            sb2.Append(downmin);
            sb2.Append("\n");
            sb2.Append(train);
            sb2.Append("\n");
            sb2.Append(ver);
            sb2.Append("\n");
            sb2.Append(cbx_isTwoTrain.Checked ? "t" : "f");
            sb2.Append(" ");
            sb2.Append(cbx_no350mode.Checked ? "t" : "f");
            sb2.Append("\n");
            sb2.Append(tbx_trainNum.Text);
            //progressDialog2.setProgress(//progressDialog.getProgress() + 1);
            for (int i = 0; i < lis.Count; i++)
            {
                sb2.Append("\n");
                sb2.Append(Share.stations[(lis[i].before)].name);
                sb2.Append(" ");
                sb2.Append(Share.rails[(lis[i].line)].name);
                sb2.Append(" ");
                sb2.Append(Share.stations[(lis[i].after)].name);
                sb2.Append(" ");
                sb2.Append(lis[i].beforestop);
                sb2.Append(" ");
                sb2.Append(lis[i].beforeteg ? "t" : "f");
                sb2.Append(" ");
                sb2.Append(lis[i].afterstop);
                sb2.Append(" ");
                sb2.Append(lis[i].afterteg ? "t" : "f");
                sb2.Append(" ");
                sb2.Append(lis[i].earlytime);
                sb2.Append(" ");
                switch (lis[i].mode)
                {
                    case RunMode.ATP:
                        sb2.Append("a");
                        break;
                    case RunMode.MaxSpeed:
                        sb2.Append("m");
                        break;
                    case RunMode.Express:
                        sb2.Append("s");
                        break;
                }
                //progressDialog2.setProgress(//progressDialog.getProgress() + 1);
            }
            try
            {
                File.WriteAllText(path, sb2.ToString());
                MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e2)
            {
                //progressDialog2.cancel();
                MessageBox.Show("保存失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void cbo_trainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbo_trainSeat.Enabled = true;
            cbo_trainSeat.Items.Clear();
            train = cbo_trainType.Items[cbo_trainType.SelectedIndex].ToString();
            foreach (EMU emu in Share.emus)
            {
                if (emu.name == train)
                {
                    foreach (string item in emu.versions)
                    {
                        cbo_trainSeat.Items.Add(item);
                    }
                    break;
                }
            }
            cbo_trainSeat.SelectedIndex = 0;
            SetTime();
        }
        private void cbo_trainType_Click(object sender, EventArgs e)
        {
            cbo_trainSeat.Items.Clear();
            cbo_trainSeat.Enabled = false;
        }
        private void cbo_trainSeat_SelectedIndexChanged(object sender, EventArgs e)
        {
            ver = cbo_trainSeat.Items[cbo_trainSeat.SelectedIndex].ToString();
            foreach (EMU emu in Share.emus)
            {
                if (emu.name == train)
                {
                    if (emu.trains[0].Count > 10)
                    {
                        cbx_isTwoTrain.Checked = false;
                        cbx_isTwoTrain.Enabled = false;
                    }
                    else
                    {
                        cbx_isTwoTrain.Checked = false;
                        cbx_isTwoTrain.Enabled = true;
                    }
                    break;
                }
            }
            SetTime();
        }
        private void cbx_no350mode_CheckedChanged(object sender, EventArgs e)
        {
            SetTime();
        }
        private void bll文件BToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (string.IsNullOrEmpty(fbd.SelectedPath))
                return;
            Out(MessageBox.Show("在地图中是否显示到发时间？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes ? true : false, fbd.SelectedPath);
        }
        private void 车内PIDS文件LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (string.IsNullOrEmpty(fbd.SelectedPath))
                return;
            new Thread(SpwanLCD) { IsBackground = true }.Start(new object[] { fbd.SelectedPath, MessageBox.Show("是否手动设置列车信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes });
        }
        private void SpwanLCD(object obj)
        {
            string path = (obj as object[])[0].ToString();
            bool showdeley = Convert.ToBoolean((obj as object[])[1]);
            try
            {
                Color line_c;
                EMU e2 = null;
                bool isSelect1 = false;
                int sn = 0;
                string cpy = this.cbo_rcp.Items[cbo_rcp.SelectedIndex].ToString();
                string tra = train;
                string tn = tbx_trainNum.Text.Split(' ')[0];
                List<PIDSInfo> stats = new List<PIDSInfo>();
                StringBuilder baseData = new StringBuilder();
                JArray jArray;
                foreach (EMU e in Share.emus)
                {
                    if (e.name.Equals(train))
                    {
                        e2 = e;
                        break;
                    }
                }
                if (e2.speed > 310)
                    line_c = Color.FromArgb(204, 0, 0);
                else if (e2.speed > 290)
                    line_c = Color.FromArgb(255, 51, 0);
                else if (e2.speed > 230)
                    line_c = Color.FromArgb(255, 102, 0);
                else if (e2.speed > 190)
                    line_c = Color.FromArgb(255, 153, 0);
                else if (e2.speed > 150)
                    line_c = Color.FromArgb(203, 203, 0);
                else
                    line_c = Color.FromArgb(51, 204, 51);
                for (int j = 0; j < lis.Count; j++)
                {
                    isSelect1 = false;
                    jArray = Share.rails[lis[j].line].locations;
                    for (int k = 0; k < jArray.Count; k++)
                    {
                        if (((String)jArray[k]["name"]) == Share.stations[lis[j].after].name)
                        {
                            if (!isSelect1)
                            {
                                jArray = new JArray();
                                for (int l = Share.rails[(lis[j].line)].locations.Count - 1; l >= 0; l--)
                                {
                                    jArray.Add((JObject)Share.rails[(lis[j].line)].locations[(l)]);
                                }
                                k = -1;
                                isSelect1 = false;
                                continue;
                            }
                            else
                            {
                                if ((!Share.stations[lis[j].after].name.ToLower().StartsWith("x")) && (!Share.stations[lis[j].after].name.ToLower().EndsWith("线路所")))
                                    baseData.Append(Share.stations[lis[j].after].name);
                                if (lis[j].afterstop > 0)
                                {
                                    baseData.Append('|');
                                    baseData.Append(((uptimes[sn].arriveTime == -1) ? ToLCDTime(uptimes[sn].deparTime) : ToLCDTime(uptimes[sn].arriveTime)));
                                    baseData.Append(':');
                                    if (showdeley)
                                    {
                                        Share.delayformflag = false;
                                        //new Thread(() => { 
                                        new SetDelayForm().ShowDialog(Share.stations[lis[j].after].name);
                                        //}) { IsBackground = false }.Start();
                                        //while (!Share.delayformflag) ;
                                        baseData.Append(Share.delayformdata1.ToString());
                                    }
                                    else
                                    {
                                        baseData.Append('0');
                                    }
                                    sn++;
                                }
                                if (j + 1 < jArray.Count)
                                    baseData.Append(',');
                                break;
                            }
                        }
                        if (isSelect1 && (((String)jArray[k]["type"]) == "station"))
                        {
                            if ((!((String)jArray[k]["name"]).ToLower().Contains("x")) && (!((String)jArray[k]["name"]).Contains("线路所")))
                                baseData.Append(((String)jArray[k]["name"]));
                            if (j + 1 < jArray.Count)
                                baseData.Append(',');
                        }
                        if (((String)jArray[k]["name"]) == Share.stations[lis[j].before].name)
                        {
                            isSelect1 = true;
                            if (j == 0)
                            {
                                baseData.Append(Share.stations[lis[j].before].name);
                                if (lis[j].beforestop > 0)
                                {
                                    baseData.Append('|');
                                    baseData.Append(((uptimes[sn].arriveTime == -1) ? ToLCDTime(uptimes[sn].deparTime) : ToLCDTime(uptimes[sn].arriveTime)));
                                    baseData.Append(':');
                                    if (showdeley)
                                    {
                                        Share.delayformflag = false;
                                        //new Thread(() => { 
                                        new SetDelayForm().ShowDialog(Share.stations[lis[j].before].name);
                                        //}) { IsBackground = false }.Start();
                                        //while (!Share.delayformflag) ;
                                        baseData.Append(Share.delayformdata1.ToString());
                                    }
                                    else
                                    {
                                        baseData.Append('0');
                                    }
                                    sn++;
                                }
                                if (j + 1 < jArray.Count)
                                    baseData.Append(',');
                            }
                        }
                    }
                }
                if (showdeley)
                {
                    tra = tra + "-";
                    new SetDelayForm().ShowDialog("");
                    tra = tra + Share.delayformdata1.ToString();
                    if (cbx_isTwoTrain.Checked)
                    {
                        tra = tra + "&";
                        tra = tra + train;
                        new SetDelayForm().ShowDialog("");
                        tra = tra + Share.delayformdata1.ToString();
                    }
                }
                while (baseData.ToString().Contains(",,"))
                {
                    baseData.Replace(",,", ",");
                }
                foreach (string item in baseData.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    stats.Add(new PIDSInfo(item));
                }
                /*string tmp = "";
                foreach (Station stat in stats)
                {
                    tmp+=(stat.name);
                    tmp += ", ";
                }
                MessageBox.Show(tmp);*/
                StringBuilder sb = new StringBuilder();
                StringBuilder sbd = new StringBuilder();
                StringBuilder sbb = new StringBuilder();
                int i = 0;
                ulong lcdNum = 1;
                int t = 0, t3 = 0;
                bool t2, t4 = false;
                List<string> stations = new List<string>();
                Bitmap bmp = null;
                Graphics g = null;
                Pen afterpen = new Pen(line_c, 20.0f);
                Pen nullpen = new Pen(new SolidBrush(line_c));
                Pen backpen = new Pen(new SolidBrush(Color.White));
                Pen beforepen = new Pen(Color.FromArgb(153, 153, 153), 20.0f);
                Pen astpen = new Pen(line_c, 10.0f);
                Pen bstpen = new Pen(Color.FromArgb(153, 153, 153), 10.0f);
                foreach (PIDSInfo stat in stats)
                {
                    //start x95 y170;end x3750 y170
                    bmp = new Bitmap(3840, 360);
                    g = Graphics.FromImage(bmp);
                    g.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(0, 0), new Size(bmp.Width, bmp.Height)));
                    g.FillRectangle(new SolidBrush(line_c), new Rectangle(new Point(0, 0), new Size(bmp.Width, 90)));
                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point(1280, 18), new Size(27 * 2, 27 * 2)));
                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point(2505, 18), new Size(27 * 2, 27 * 2)));
                    g.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(1307, 18), new Size(2532 - 1307, 27 * 2)));
                    sb.Clear();
                    //车次：G6591/4    揭阳→广州东     本站：广州东       终点站：广州东
                    sb.Append("车次：");
                    sb.Append(tn);
                    sb.Append("    ");
                    sb.Append(stats[0].name);
                    sb.Append("→");
                    sb.Append(stats[stats.Count - 1].name);
                    sb.Append("     本站：");
                    if (stat.isStop)
                        sb.Append(stat.name);
                    else
                        sb.Append("      ");
                    if (i == stats.Count - 1)
                        sb.Append("       终点站");
                    else
                        sb.Append("       下一站：");
                    for (int j = i + 1; j < stats.Count; j++)
                    {
                        if (stats[j].isStop)
                        {
                            sb.Append(stats[j].name);
                            break;
                        }
                    }
                    t = 0;
                    foreach (char item in sb.ToString())
                    {
                        if (item == ' ')
                            t++;
                    }
                    if ((((sb.Length - (t / 4.0)) * 15)) > ((2532 - 1307) / 2.0))
                    {
                        g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point(1920 - (((sb.Length - (t / 4)) * 15)) - 27, 18), new Size(27 * 2, 27 * 2)));
                        g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point(1920 + (((sb.Length - (t / 4)) * 15)) - 27, 18), new Size(27 * 2, 27 * 2)));
                        g.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(1920 - (((sb.Length - (t / 4)) * 15)), 18), new Size((((sb.Length - (t / 4)) * 15)) * 2, 27 * 2)));
                    }
                    g.DrawString(sb.ToString(), new Font("微软雅黑", 30), new SolidBrush(Color.Black), new PointF(1920.0f - ((sb.Length - (t / 4.0f)) * 15), 15.5f));
                    //担当企业 x=655
                    sbd.Clear();
                    sbd.Append("担当企业：");
                    sbd.Append(cpy);
                    g.DrawString(sbd.ToString(), new Font("微软雅黑", 30), new SolidBrush(Color.White), new PointF(655.0f - (sbd.Length * 15.0f), 15.5f));
                    //本务 x=3070
                    sbb.Clear();
                    sbb.Append("本务：");
                    sbb.Append(tra.ToUpper());
                    g.DrawString(sbb.ToString(), new Font("微软雅黑", 30), new SolidBrush(Color.White), new PointF(3070.0f - (sbb.Length * 15.0f), 15.5f));
                    if ((i + 26 >= stats.Count - 1) && (i - 26 <= 0)) //一次性
                    {
                        g.DrawLine(afterpen, new Point(95, 215), new Point(3750, 215));
                        stations.Clear();
                        foreach (PIDSInfo stat2 in stats)
                        {
                            stations.Add(stat2.name);
                        }
                        if (stations.IndexOf(stat.name) > 0)
                            g.DrawLine(beforepen, new Point(95, 215), new Point((3655 / (stats.Count - 1) * (stations.IndexOf(stat.name))) + 95, 215));
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (stat2.name == stat.name)
                                break;
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            }
                            else
                            {
                                g.DrawEllipse(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45, 13, 13);//左上
                                g.DrawEllipse(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 + 26, 150 + 45, 13, 13);//右上
                                g.DrawEllipse(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 + 26, 173 + 45, 13, 13);//右下
                                g.DrawEllipse(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 173 + 45, 13, 13);//左下
                                g.DrawLine(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45 + 6, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45 + 6 + 26);
                                g.DrawLine(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 0 + 45, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                g.DrawLine(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                g.DrawLine(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                g.FillRectangle(new SolidBrush(Color.White), 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95.0f - 17.5f, 170.0f + 28.0f));
                                g.DrawString(stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95.0f - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            }
                        }
                        t2 = false;
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (stat2.name == stat.name)
                                t2 = true;
                            if (t2)
                            {
                                if (stat2.isStop)
                                {
                                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                    g.DrawEllipse(astpen, new Rectangle(new Point((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                }
                                else
                                {
                                    g.DrawEllipse(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45, 13, 13);//左上
                                    g.DrawEllipse(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 + 26, 150 + 45, 13, 13);//右上
                                    g.DrawEllipse(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 + 26, 173 + 45, 13, 13);//右下
                                    g.DrawEllipse(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 173 + 45, 13, 13);//左下
                                    g.DrawLine(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45 + 6, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45 + 6 + 26);
                                    g.DrawLine(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 0 + 45, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                    g.DrawLine(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                    g.DrawLine(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                    g.FillRectangle(new SolidBrush(Color.White), 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                                }
                                if (stat2.isStop)
                                {
                                    g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95.0f - 17.5f, 170.0f + 28.0f));
                                    if (stat2.delayTime > 0)
                                        g.DrawString("+" + stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Red), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95.0f - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                    else if (stat2.delayTime == 0)
                                        g.DrawString(stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95.0f - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                    else if (stat2.delayTime < 0)
                                        g.DrawString(stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.DarkGreen), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95.0f - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                }
                                g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.White), 3655.0f / (stats.Count - 1) * (stations.IndexOf(stat2.name)) + (3655.0f / (stats.Count - 1) / 2.0f) + 70.0f, 170 + 45 - 10);
                            }
                        }
                        for (int k = 0; k < stats.Count; k++)
                        {
                            g.DrawString(stats[k].name, new Font("微软雅黑", 40), new SolidBrush(Color.Black), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stats[k].name))) + 95.0f - ((stats[k].name.Length / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                        }
                    }
                    else if (i >= stats.Count - 1) //终点站
                    {
                        g.DrawLine(afterpen, new Point(0, 215), new Point(3750, 215));
                        g.DrawLine(beforepen, new Point(0, 215), new Point((3655 / 25 * (24)) + 95, 215));
                        t3 = 0;
                        t4 = false;
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (t3 == 25)
                                break;
                            if (stat2.name == stats[i - 25].name)
                                t4 = true;
                            if (t4)
                            {
                                if (stat2.isStop)
                                {
                                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (t3)) + 95 - 27, 188), new Size(54, 54)));
                                    g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / 25 * ((t3))) + 95 - 27, 188), new Size(54, 54)));
                                    g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (t3)) + 95 - 17.5f, 170.0f + 28.0f));
                                    g.DrawString(stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (t3)) + 95 - 7.5f, 170.0f + 42.0f));
                                }
                                else
                                {
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3) + 95 - 26, 150 + 45, 13, 13);//左上
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3) + 95 + 26, 150 + 45, 13, 13);//右上
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3) + 95 + 26, 173 + 45, 13, 13);//右下
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3) + 95 - 26, 173 + 45, 13, 13);//左下
                                    g.DrawLine(bstpen, 3655 / 25 * (t3) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (t3) + 95 - 26, 150 + 45 + 6 + 26);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (t3) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (t3) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (t3) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                    g.FillRectangle(new SolidBrush(Color.White), 3655 / 25 * (t3) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                                }
                            }
                            if (t4)
                                t3++;
                        }
                        g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                        g.DrawEllipse(astpen, new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                        if (stat.isStop)
                        {
                            g.DrawString(stat.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * 25) + 95 - 17.5f, 170.0f + 28.0f));
                            if (stat.delayTime > 0)
                                g.DrawString("+" + stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Red), new PointF((3655 / 25 * (t3)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            else if (stat.delayTime == 0)
                                g.DrawString(stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (t3)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            else if (stat.delayTime < 0)
                                g.DrawString(stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.DarkGreen), new PointF((3655 / 25 * (t3)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.White), 3655.0f + (3655.0f / (stats.Count - 1) / 2.0f) - 17.5f, 170 + 45 - 10);
                        }
                        for (int k = i - 25; k < stats.Count; k++)
                        {
                            g.DrawString(stats[k].name, new Font("微软雅黑", 40), new SolidBrush(Color.Black), new PointF((3655 / 25 * (k - (i - 25))) + 95 - ((stats[k].name.Length / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                        }
                    }
                    else if (i - 25 <= 0) //始发站
                    {
                        g.DrawLine(afterpen, new Point(95, 215), new Point(3840, 215));
                        stations.Clear();
                        foreach (PIDSInfo stat2 in stats)
                        {
                            stations.Add(stat2.name);
                        }
                        if (i > 0)
                            g.DrawLine(beforepen, new Point(95, 215), new Point((3655 / 25 * (stations.IndexOf(stat.name))) + 95, 215));
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (stat2.name == stat.name)
                                break;
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            }
                            else
                            {
                                g.DrawEllipse(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45, 13, 13);//左上
                                g.DrawEllipse(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 + 26, 150 + 45, 13, 13);//右上
                                g.DrawEllipse(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 + 26, 173 + 45, 13, 13);//右下
                                g.DrawEllipse(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 173 + 45, 13, 13);//左下
                                g.DrawLine(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45 + 6 + 26);
                                g.DrawLine(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                g.DrawLine(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                g.DrawLine(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                g.FillRectangle(new SolidBrush(Color.White), 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 17.5f, 170.0f + 28.0f));
                                g.DrawString(stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            }
                        }
                        t2 = false;
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (stat2.name == stat.name)
                                t2 = true;
                            if (t2)
                            {
                                if (stat2.isStop)
                                {
                                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                    g.DrawEllipse(astpen, new Rectangle(new Point((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                }
                                else
                                {
                                    g.DrawEllipse(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45, 13, 13);//左上
                                    g.DrawEllipse(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 + 26, 150 + 45, 13, 13);//右上
                                    g.DrawEllipse(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 + 26, 173 + 45, 13, 13);//右下
                                    g.DrawEllipse(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 173 + 45, 13, 13);//左下
                                    g.DrawLine(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45 + 6 + 26);
                                    g.DrawLine(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                    g.DrawLine(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                    g.DrawLine(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                    g.FillRectangle(new SolidBrush(Color.White), 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                                }
                                g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.White), 3655.0f / 25.0f * (stations.IndexOf(stat2.name) + 1 + 0.05f), 170 + 45 - 10);
                                if (stat2.isStop)
                                {
                                    g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 17.5f, 170.0f + 28.0f));
                                    if (stat2.delayTime > 0)
                                        g.DrawString("+" + stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Red), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                    else if (stat2.delayTime == 0)
                                        g.DrawString(stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                    else if (stat2.delayTime < 0)
                                        g.DrawString(stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.DarkGreen), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));

                                }
                            }
                        }
                        for (int k = 0; k < 26; k++)
                        {
                            g.DrawString(stats[k].name, new Font("微软雅黑", 40), new SolidBrush(Color.Black), new PointF((3655 / 25 * (k)) + 95 - ((stats[k].name.Length / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                        }
                    }
                    else //中途站
                    {
                        g.DrawLine(afterpen, new Point(0, 215), new Point(3840, 215));
                        g.DrawLine(beforepen, new Point(0, 215), new Point((3655 / 25 * (25)) + 95, 215));
                        t3 = 0;
                        t4 = false;
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (t3 == 26)
                                break;
                            if (stat2.name == stats[i - 26].name)
                                t4 = true;
                            if (t4)
                            {
                                if (stat2.isStop)
                                {
                                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (t3 - 1)) + 95 - 27, 188), new Size(54, 54)));
                                    g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / 25 * (t3 - 1)) + 95 - 27, 188), new Size(54, 54)));
                                }
                                else
                                {
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26, 150 + 45, 13, 13);//左上
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3 - 1) + 95 + 26, 150 + 45, 13, 13);//右上
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3 - 1) + 95 + 26, 173 + 45, 13, 13);//右下
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26, 173 + 45, 13, 13);//左下
                                    g.DrawLine(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (t3 - 1) + 95 - 26, 150 + 45 + 6 + 26);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (t3 - 1) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (t3 - 1) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (t3 - 1) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                    g.FillRectangle(new SolidBrush(Color.White), 3655 / 25 * (t3 - 1) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                                }
                                if (stat2.isStop)
                                {
                                    g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * ((t3 - 1))) + 95 - 17.5f, 170.0f + 28.0f));
                                    g.DrawString(stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black),
                                        new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                }
                            }
                            if (t4)
                                t3++;
                        }
                        if (stats[i].isStop)
                        {
                            g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                            g.DrawEllipse(astpen, new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                        }
                        else
                        {
                            g.DrawEllipse(astpen, 3655 / 25 * (25) + 95 - 26, 150 + 45, 13, 13);//左上
                            g.DrawEllipse(astpen, 3655 / 25 * (25) + 95 + 26, 150 + 45, 13, 13);//右上
                            g.DrawEllipse(astpen, 3655 / 25 * (25) + 95 + 26, 173 + 45, 13, 13);//右下
                            g.DrawEllipse(astpen, 3655 / 25 * (25) + 95 - 26, 173 + 45, 13, 13);//左下
                            g.DrawLine(astpen, 3655 / 25 * (25) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (25) + 95 - 26, 150 + 45 + 6 + 26);
                            g.DrawLine(astpen, 3655 / 25 * (25) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (25) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                            g.DrawLine(astpen, 3655 / 25 * (25) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (25) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                            g.DrawLine(astpen, 3655 / 25 * (25) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (25) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                            g.FillRectangle(new SolidBrush(Color.White), 3655 / 25 * (25) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                        }
                        if (stat.isStop)
                        {
                            g.DrawString(stat.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * 25) + 95 - 17.5f, 170.0f + 28.0f));
                            if (stat.delayTime > 0)
                                g.DrawString("+" + stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Red),
                                    new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            else if (stat.delayTime == 0)
                                g.DrawString(stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black),
                                    new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            else if (stat.delayTime < 0)
                                g.DrawString(stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.DarkGreen),
                                    new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                        }
                        g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.White), 3655.0f / 25.0f * (25 + 1 + 0.05f), 170 + 45 - 10);
                        for (int k = i - 25; k < i + 1; k++)
                        {
                            g.DrawString(stats[k].name, new Font("微软雅黑", 40), new SolidBrush(Color.Black), new PointF((3655 / 25 * (k - (i - 25))) + 95 - ((stats[k].name.Length / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                        }
                    }
                    bmp.Save(path + "\\" + tn + "lcd" + lcdNum + ".png");
                    lcdNum++;
                    bmp = new Bitmap(3840, 360);
                    g = Graphics.FromImage(bmp);
                    g.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(0, 0), new Size(bmp.Width, bmp.Height)));
                    g.FillRectangle(new SolidBrush(line_c), new Rectangle(new Point(0, 0), new Size(bmp.Width, 90)));
                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point(1280, 18), new Size(27 * 2, 27 * 2)));
                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point(2505, 18), new Size(27 * 2, 27 * 2)));
                    g.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(1307, 18), new Size(2532 - 1307, 27 * 2)));
                    sb.Clear();
                    //车次：G6591/4    揭阳→广州东     本站：广州东       终点站：广州东
                    sb.Append("车次：");
                    sb.Append(tn);
                    sb.Append("    ");
                    sb.Append(stats[0].name);
                    sb.Append("→");
                    sb.Append(stats[stats.Count - 1].name);
                    sb.Append("     本站：");
                    if (stat.isStop)
                        sb.Append(stat.name);
                    else
                        sb.Append("      ");
                    if (i == stats.Count - 1)
                        sb.Append("       终点站");
                    else
                        sb.Append("       下一站：");
                    for (int j = i + 1; j < stats.Count; j++)
                    {
                        if (stats[j].isStop)
                        {
                            sb.Append(stats[j].name);
                            break;
                        }
                    }
                    t = 0;
                    foreach (char item in sb.ToString())
                    {
                        if (item == ' ')
                            t++;
                    }
                    if ((((sb.Length - (t / 4.0)) * 15)) > ((2532 - 1307) / 2.0))
                    {
                        g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point(1920 - (((sb.Length - (t / 4)) * 15)) - 27, 18), new Size(27 * 2, 27 * 2)));
                        g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point(1920 + (((sb.Length - (t / 4)) * 15)) - 27, 18), new Size(27 * 2, 27 * 2)));
                        g.FillRectangle(new SolidBrush(Color.White), new Rectangle(new Point(1920 - (((sb.Length - (t / 4)) * 15)), 18), new Size((((sb.Length - (t / 4)) * 15)) * 2, 27 * 2)));
                    }
                    g.DrawString(sb.ToString(), new Font("微软雅黑", 30), new SolidBrush(Color.Black), new PointF(1920.0f - ((sb.Length - (t / 4.0f)) * 15), 15.5f));
                    //担当企业 x=655
                    sbd.Clear();
                    sbd.Append("担当企业：");
                    sbd.Append(cpy);
                    g.DrawString(sbd.ToString(), new Font("微软雅黑", 30), new SolidBrush(Color.White), new PointF(655.0f - (sbd.Length * 15.0f), 15.5f));
                    //本务 x=3070
                    sbb.Clear();
                    sbb.Append("本务：");
                    sbb.Append(tra.ToUpper());
                    g.DrawString(sbb.ToString(), new Font("微软雅黑", 30), new SolidBrush(Color.White), new PointF(3070.0f - (sbb.Length * 15.0f), 15.5f));
                    if ((i + 26 >= stats.Count - 1) && (i - 26 <= 0))
                    {
                        g.DrawLine(afterpen, new Point(95, 215), new Point(3750, 215));
                        stations.Clear();
                        foreach (PIDSInfo stat2 in stats)
                        {
                            stations.Add(stat2.name);
                        }
                        if (stations.IndexOf(stat.name) > 0)
                            g.DrawLine(beforepen, new Point(95, 215), new Point((3655 / (stats.Count - 1) * (stations.IndexOf(stat.name))) + 95, 215));
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            }
                            else
                            {
                                g.DrawEllipse(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45, 13, 13);//左上
                                g.DrawEllipse(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 + 26, 150 + 45, 13, 13);//右上
                                g.DrawEllipse(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 + 26, 173 + 45, 13, 13);//右下
                                g.DrawEllipse(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 173 + 45, 13, 13);//左下
                                g.DrawLine(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45 + 6, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45 + 6 + 26);
                                g.DrawLine(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 0 + 45, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                g.DrawLine(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                g.DrawLine(bstpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                g.FillRectangle(new SolidBrush(Color.White), 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 17.5f, 170.0f + 28.0f));
                                g.DrawString(stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            }
                            if (stat2.name == stat.name)
                                break;
                        }
                        t2 = false;
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (t2)
                            {
                                if (stat2.isStop)
                                {
                                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                    g.DrawEllipse(astpen, new Rectangle(new Point((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                }
                                else
                                {
                                    g.DrawEllipse(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45, 13, 13);//左上
                                    g.DrawEllipse(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 + 26, 150 + 45, 13, 13);//右上
                                    g.DrawEllipse(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 + 26, 173 + 45, 13, 13);//右下
                                    g.DrawEllipse(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 173 + 45, 13, 13);//左下
                                    g.DrawLine(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45 + 6, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26, 150 + 45 + 6 + 26);
                                    g.DrawLine(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 0 + 45, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                    g.DrawLine(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                    g.DrawLine(astpen, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                    g.FillRectangle(new SolidBrush(Color.White), 3655 / (stats.Count - 1) * stations.IndexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                                }
                                if (stat2.isStop)
                                {
                                    g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 17.5f, 170.0f + 28.0f));
                                    if (stat2.delayTime > 0)
                                        g.DrawString("+" + stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Red), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                    else if (stat2.delayTime == 0)
                                        g.DrawString(stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                    else if (stat2.delayTime < 0)
                                        g.DrawString(stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.DarkGreen), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                }
                                g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.White), 3655.0f / (stats.Count - 1) * (stations.IndexOf(stat2.name) - 1) + (3655.0f / (stats.Count - 1) / 2.0f) + 70.0f, 170 + 45 - 10);
                            }
                            if (stat2.name == stat.name)
                                t2 = true;
                        }
                        for (int k = 0; k < stats.Count; k++)
                        {
                            g.DrawString(stats[k].name, new Font("微软雅黑", 40), new SolidBrush(Color.Black), new PointF((3655 / (stats.Count - 1) * (stations.IndexOf(stats[k].name))) + 95 - ((stats[k].name.Length / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                        }
                    }
                    else if (i + 0 >= stats.Count - 1)
                    {
                        //g.DrawLine(afterpen, new Point(0, 215), new Point(3750, 215));
                        g.DrawLine(beforepen, new Point(0, 215), new Point((3655 / 25 * 25) + 95, 215));
                        t3 = 0;
                        t4 = false;
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (t3 == 25)
                                break;
                            if (stat2.name == stats[i - 25].name)
                                t4 = true;
                            if (t4)
                            {
                                if (stat2.isStop)
                                {
                                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (t3)) + 95 - 27, 188), new Size(54, 54)));
                                    g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / 25 * (t3)) + 95 - 27, 188), new Size(54, 54)));
                                }
                                else
                                {
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3) + 95 - 26, 150 + 45, 13, 13);//左上
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3) + 95 + 26, 150 + 45, 13, 13);//右上
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3) + 95 + 26, 173 + 45, 13, 13);//右下
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3) + 95 - 26, 173 + 45, 13, 13);//左下
                                    g.DrawLine(bstpen, 3655 / 25 * (t3) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (t3) + 95 - 26, 150 + 45 + 6 + 26);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (t3) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (t3) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (t3) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                    g.FillRectangle(new SolidBrush(Color.White), 3655 / 25 * (t3) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                                }
                                if (stat2.isStop)
                                {
                                    g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * ((t3))) + 95 - 17.5f, 170.0f + 28.0f));
                                    g.DrawString(stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black),
                                        new PointF((3655 / 25 * (t3)) + 95 - 7.5f, 170.0f + 42.0f));
                                }
                            }
                            if (t4)
                                t3++;
                        }
                        g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                        g.DrawEllipse(astpen, new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                        if (stat.isStop)
                        {
                            g.DrawString(stat.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * 25) + 95 - 17.5f, 170.0f + 28.0f));
                            if (stat.delayTime > 0)
                                g.DrawString("+" + stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Red), new PointF((3655 / 25 * (t3 - 1)) + 95 - 17.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            else if (stat.delayTime == 0)
                                g.DrawString(stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (t3 - 1)) + 95 - 17.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            else if (stat.delayTime < 0)
                                g.DrawString(stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.DarkGreen), new PointF((3655 / 25 * (t3 - 1)) + 95 - 17.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                        }
                        for (int k = i - 25; k < stats.Count; k++)
                        {
                            g.DrawString(stats[k].name, new Font("微软雅黑", 40), new SolidBrush(Color.Black), new PointF((3655 / 25 * (k - (i - 25))) + 95 - ((stats[k].name.Length / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                        }
                    }
                    else if (i - 25 <= 0)
                    {
                        g.DrawLine(afterpen, new Point(95, 215), new Point(3840, 215));
                        stations.Clear();
                        foreach (PIDSInfo stat2 in stats)
                        {
                            stations.Add(stat2.name);
                        }
                        if (i > 0)
                            g.DrawLine(beforepen, new Point(95, 215), new Point((3655 / 25 * (stations.IndexOf(stat.name))) + 95, 215));
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            }
                            else
                            {
                                g.DrawEllipse(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45, 13, 13);//左上
                                g.DrawEllipse(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 + 26, 150 + 45, 13, 13);//右上
                                g.DrawEllipse(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 + 26, 173 + 45, 13, 13);//右下
                                g.DrawEllipse(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 173 + 45, 13, 13);//左下
                                g.DrawLine(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45 + 6 + 26);
                                g.DrawLine(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                g.DrawLine(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                g.DrawLine(bstpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                g.FillRectangle(new SolidBrush(Color.White), 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 17.5f, 170.0f + 28.0f));
                                g.DrawString(stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            }
                            if (stat2.name == stat.name)
                                break;
                        }
                        t2 = false;
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (t2)
                            {
                                if (stat2.isStop)
                                {
                                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                    g.DrawEllipse(astpen, new Rectangle(new Point((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                }
                                else
                                {
                                    g.DrawEllipse(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45, 13, 13);//左上
                                    g.DrawEllipse(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 + 26, 150 + 45, 13, 13);//右上
                                    g.DrawEllipse(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 + 26, 173 + 45, 13, 13);//右下
                                    g.DrawEllipse(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 173 + 45, 13, 13);//左下
                                    g.DrawLine(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26, 150 + 45 + 6 + 26);
                                    g.DrawLine(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                    g.DrawLine(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                    g.DrawLine(astpen, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                    g.FillRectangle(new SolidBrush(Color.White), 3655 / 25 * (stations.IndexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                                }
                                g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.White), 3655.0f / 25.0f * (stations.IndexOf(stat2.name) + 1 + 0.05f), 170 + 45 - 10);
                                if (stat2.isStop)
                                {
                                    g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 17.5f, 170.0f + 28.0f));
                                    if (stat2.delayTime > 0)
                                        g.DrawString("+" + stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Red), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                    else if (stat2.delayTime == 0)
                                        g.DrawString(stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                    else if (stat2.delayTime < 0)
                                        g.DrawString(stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.DarkGreen), new PointF((3655 / 25 * (stations.IndexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));

                                }
                            }
                            if (stat2.name == stat.name)
                            {
                                t2 = true;
                                g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.White), 3655.0f / 25.0f * (stations.IndexOf(stat2.name) + 1 + 0.05f), 170 + 45 - 10);
                            }
                        }
                        for (int k = 0; k < 26; k++)
                        {
                            g.DrawString(stats[k].name, new Font("微软雅黑", 40), new SolidBrush(Color.Black), new PointF((3655 / 25 * (k)) + 95 - ((stats[k].name.Length / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                        }
                    }
                    else
                    {
                        g.DrawLine(afterpen, new Point(0, 215), new Point(3840, 215));
                        g.DrawLine(beforepen, new Point(0, 215), new Point((3655 / 25 * (25)) + 95, 215));
                        t3 = 0;
                        t4 = false;
                        foreach (PIDSInfo stat2 in stats)
                        {
                            if (t3 == 26)
                                break;
                            if (stat2.name == stats[i - 26].name)
                                t4 = true;
                            if (t4)
                            {
                                if (stat2.isStop)
                                {
                                    g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (t3 - 1)) + 95 - 27, 188), new Size(54, 54)));
                                    g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / 25 * (t3 - 1)) + 95 - 27, 188), new Size(54, 54)));
                                }
                                else
                                {
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26, 150 + 45, 13, 13);//左上
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3 - 1) + 95 + 26, 150 + 45, 13, 13);//右上
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3 - 1) + 95 + 26, 173 + 45, 13, 13);//右下
                                    g.DrawEllipse(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26, 173 + 45, 13, 13);//左下
                                    g.DrawLine(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (t3 - 1) + 95 - 26, 150 + 45 + 6 + 26);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (t3 - 1) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (t3 - 1) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                    g.DrawLine(bstpen, 3655 / 25 * (t3 - 1) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (t3 - 1) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                    g.FillRectangle(new SolidBrush(Color.White), 3655 / 25 * (t3 - 1) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                                }
                                if (stat2.isStop)
                                {
                                    g.DrawString(stat2.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * ((t3 - 1))) + 95 - 17.5f, 170.0f + 28.0f));
                                    g.DrawString(stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black),
                                        new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + stat2.delayTime.ToString() : stat2.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                                }
                            }
                            if (t4)
                                t3++;
                        }
                        if (stats[i].isStop)
                        {
                            g.FillEllipse(new SolidBrush(Color.White), new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                            g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                        }
                        else
                        {
                            g.DrawEllipse(bstpen, 3655 / 25 * (25) + 95 - 26, 150 + 45, 13, 13);//左上
                            g.DrawEllipse(bstpen, 3655 / 25 * (25) + 95 + 26, 150 + 45, 13, 13);//右上
                            g.DrawEllipse(bstpen, 3655 / 25 * (25) + 95 + 26, 173 + 45, 13, 13);//右下
                            g.DrawEllipse(bstpen, 3655 / 25 * (25) + 95 - 26, 173 + 45, 13, 13);//左下
                            g.DrawLine(bstpen, 3655 / 25 * (25) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (25) + 95 - 26, 150 + 45 + 6 + 26);
                            g.DrawLine(bstpen, 3655 / 25 * (25) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (25) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                            g.DrawLine(bstpen, 3655 / 25 * (25) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (25) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                            g.DrawLine(bstpen, 3655 / 25 * (25) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (25) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                            g.FillRectangle(new SolidBrush(Color.White), 3655 / 25 * (25) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                        }
                        if (stat.isStop)
                        {
                            g.DrawString(stat.arriveTime.ToString("HHmm"), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * 25) + 95 - 17.5f, 170.0f + 28.0f));
                            if (stat.delayTime > 0)
                                g.DrawString("+" + stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Red), new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            else if (stat.delayTime == 0)
                                g.DrawString(stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.Black), new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                            else if (stat.delayTime < 0)
                                g.DrawString(stat.delayTime.ToString(), new Font("微软雅黑", 10), new SolidBrush(Color.DarkGreen), new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + stat.delayTime.ToString() : stat.delayTime.ToString()).Length / 2), 170.0f + 42.0f));
                        }
                        g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.White), 3655.0f / 25.0f * (25 + 1 + 0.05f), 170 + 45 - 10);
                        for (int k = i - 25; k < i + 1; k++)
                        {
                            g.DrawString(stats[k].name, new Font("微软雅黑", 40), new SolidBrush(Color.Black), new PointF((3655 / 25 * (k - (i - 25))) + 95 - ((stats[k].name.Length / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                        }
                    }
                    bmp.Save(path + "\\" + tn + "lcd" + lcdNum + ".png");
                    lcdNum++;
                    //if (i == 10)
                    //    break;
                    i++;
                }
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("导出失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public String ToTime(int min)
        {
            String tmp1, tmp3;
            StringBuilder sb = new StringBuilder();
            int minute, hourOfDay;
            minute = min % 60;
            hourOfDay = min / 60;
            hourOfDay = hourOfDay % 24;
            tmp1 = minute < 10 ? "0" + minute.ToString() : minute.ToString();
            tmp3 = hourOfDay < 10 ? "0" + hourOfDay.ToString() : hourOfDay.ToString();
            sb.Append(tmp3);
            sb.Append(":");
            sb.Append(tmp1);
            return sb.ToString();
        }
        public String ToLCDTime(int min)
        {
            String tmp1, tmp3;
            StringBuilder sb = new StringBuilder();
            int minute, hourOfDay;
            minute = min % 60;
            hourOfDay = min / 60;
            hourOfDay = hourOfDay % 24;
            tmp1 = minute < 10 ? "0" + minute.ToString() : minute.ToString();
            tmp3 = hourOfDay < 10 ? "0" + hourOfDay.ToString() : hourOfDay.ToString();
            sb.Append(tmp3);
            sb.Append(tmp1);
            return sb.ToString();
        }
        private void textbox4_LostFocus(object sender, EventArgs e)
        {
            try
            {
                (tbx_trainNum).Text = ((tbx_trainNum).Text.ToString().ToUpper());
                String tmp = (tbx_trainNum).Text.ToString();
                bool b = true;
                b = b && tmp.Contains(" ");
                b = b && (tmp.StartsWith("G") || tmp.StartsWith("D") || tmp.StartsWith("C") || tmp.StartsWith("S"));
                b = b && (tmp.Length < 22);
                int min = 0;
                if (tmp.StartsWith("D"))
                    min = 300;
                else if (tmp.StartsWith("C"))
                    min = 1000;
                else if (tmp.StartsWith("S"))
                    min = 100;
                if (tmp.Contains(" "))
                {
                    b = b && (tmp.Split(' ')[0][0] == tmp.Split(' ')[1][0]);
                    if (tmp.Contains("/"))
                    {
                        b = b && (int.Parse(tmp.Split(' ')[0].Substring(1).Split('/')[0]) < 9999);
                        b = b && (int.Parse(tmp.Split(' ')[0].Substring(1).Split('/')[0]) > min);
                        b = b && (int.Parse(tmp.Split(' ')[1].Substring(1).Split('/')[0]) < 9999);
                        b = b && (int.Parse(tmp.Split(' ')[1].Substring(1).Split('/')[0]) > min);
                        b = b && (int.Parse(tmp.Split(' ')[0].Substring(1).Split('/')[1]) < 9999);
                        b = b && (int.Parse(tmp.Split(' ')[0].Substring(1).Split('/')[1]) > min);
                        b = b && (int.Parse(tmp.Split(' ')[1].Substring(1).Split('/')[1]) < 9999);
                        b = b && (int.Parse(tmp.Split(' ')[1].Substring(1).Split('/')[1]) > min);
                    }
                    else
                    {
                        b = b && (int.Parse(tmp.Split(' ')[0].Substring(1)) < 9999);
                        b = b && (int.Parse(tmp.Split(' ')[0].Substring(1)) > min);
                        b = b && (int.Parse(tmp.Split(' ')[1].Substring(1)) < 9999);
                        b = b && (int.Parse(tmp.Split(' ')[1].Substring(1)) > min);
                    }
                }
                b = b && (tmp.IndexOf(" ") == tmp.LastIndexOf(" "));
                b = b && (tmp.Replace("G", "").Replace("D", "")
                        .Replace("C", "").Replace("S", "")
                        .Replace("9", "").Replace("0", "")
                        .Replace("8", "").Replace("1", "")
                        .Replace("7", "").Replace("2", "")
                        .Replace("6", "").Replace("3", "")
                        .Replace("5", "").Replace("4", "")
                        .Replace("/", "").Replace(" ", "").Length == 0);
                if (b == false)
                    (tbx_trainNum).Text = ("D301 D302");
            }
            catch { }
        }
        public String CalcTime(int upmin, int uphour, bool up)
        {
            if (this.lis.Count == 0)
                return "";
            List<ListItem> lis = new List<ListItem>();
            double ast;
            int speed, max_length, bspeed = 0, aspeed = 0, _speed = 0, m, minute, hourOfDay, m2;
            double length;
            List<String> jArray = new List<string>();
            String tmp, tmp1, tmp3;
            List<String> speeds = new List<string>();
            List<Double> lengths = new List<double>();
            StringBuilder sb = new StringBuilder();
            String[] tmp2;
            List<String> spd = new List<string>();
            EMU e2 = null;
            int min = upmin + (60 * uphour);
            TimeTime tttmp = new TimeTime(0, 0, false);
            int j = 0;
            bool b, b1, b2, b3, b4;
            foreach (EMU e in Share.emus)
            {
                if (e.name.Equals(train))
                {
                    e2 = e;
                    break;
                }
            }
            if (up)
            {
                lis = this.lis;
                uptimes.Clear();
            }
            else
            {
                downtimes.Clear();
                ListItem tmp4;
                for (int i = this.lis.Count - 1; i >= 0; i--)
                {
                    tmp4 = this.lis[i];
                    lis.Add(new ListItem(tmp4.after, tmp4.afterstop,
                            tmp4.line, tmp4.mode,
                            tmp4.before, tmp4.beforestop, tmp4.earlytime,
                            tmp4.afterteg, tmp4.beforeteg));/*
                tmp4 = lis[i];
                t5 = tmp4.before;
                tmp4.before = tmp4.after;
                tmp4.after = t5;
                t5 = tmp4.beforestop;
                tmp4.beforestop = tmp4.afterstop;
                tmp4.afterstop = t5;
                t6 = tmp4.beforeteg;
                tmp4.beforeteg = tmp4.afterteg;
                tmp4.afterteg = t6;*/
                }
            }
            ast = e2.ast;
            speed = e2.speed;
            max_length = Share.stations[(lis[0].before)].name.Length;
            for (int i = 0; i < lis.Count; i++)
            {
                if ((!Share.stations[(lis[i].before)].name.EndsWith("线路所")) && (lis[i].beforestop != 0) && (!lis[i].beforeteg))
                {
                    if (max_length < Share.stations[(lis[i].before)].name.Length)
                        max_length = Share.stations[(lis[i].before)].name.Length;
                }
            }
            if ((!Share.stations[(lis[(lis.Count - 1)].after)].name.EndsWith("线路所"))
                    && (lis[(lis.Count - 1)].afterstop != 0) && (!lis[(lis.Count - 1)].afterteg))
            {
                if (max_length < Share.stations[(lis[(lis.Count - 1)].after)].name.Length)
                    max_length = Share.stations[(lis[(lis.Count - 1)].after)].name.Length;
            }
            if (lis[0].beforestop != 0)
            {
                if (Share.stations[(lis[0].before)].name.Length < max_length)
                {
                    sb.Append("_");
                    for (int i = 1; i < getLen(max_length, Share.stations[(lis[0].before)].name.Length)[0]; i++)
                        sb.Append(" ");
                    sb.Append(Share.stations[(lis[0].before)].name);
                    for (int i = 0; i <= getLen(max_length, Share.stations[(lis[0].before)].name.Length)[1]; i++)
                        sb.Append(" ");
                }
                else
                    sb.Append(Share.stations[(lis[0].before)].name);
                sb.Append(" --:--  ");
                minute = upmin;
                hourOfDay = uphour % 24;
                tmp1 = minute < 10 ? "0" + minute.ToString() : minute.ToString();
                tmp3 = hourOfDay < 10 ? "0" + hourOfDay.ToString() : hourOfDay.ToString();
                sb.Append(tmp3);
                sb.Append(":");
                sb.Append(tmp1);
                sb.Append(" --\r\n");
                m2 = min;
                tttmp.arriveTime = -1;
                tttmp.deparTime = m2;
                tttmp.teg = false;
                if (up)
                    uptimes.Add(tttmp);
                else
                    downtimes.Add(tttmp);
            }
            //开始计算
            try
            {
                for (int i = 0; i < lis.Count; i++)
                {
                    speeds.Clear();
                    spd.Clear();
                    tmp2 = Share.rails[(lis[i].line)].speeds.Split('\n');
                    for (j = 0; j < tmp2.Length; j++)
                    {
                        if ((!tmp2[j].StartsWith("atp"))
                                && (!tmp2[j].StartsWith("ms"))
                                && (!tmp2[j].StartsWith("ss"))
                                && (!tmp2[j].StartsWith("1")) && (!tmp2[j].StartsWith("2")) && (!tmp2[j].StartsWith("3"))
                                && (!tmp2[j].StartsWith("4")) && (!tmp2[j].StartsWith("5")) && (!tmp2[j].StartsWith("6"))
                                && (!tmp2[j].StartsWith("7")) && (!tmp2[j].StartsWith("8")) && (!tmp2[j].StartsWith("9")))
                            speeds.Add(tmp2[j]);
                        spd.Add(tmp2[j]);
                    }
                    b1 = false;
                    jArray = Share.rails[(lis[i].line)].stations;
                    lengths = Share.rails[(lis[i].line)].lengths;
                    //j = 0;
                    b = false;
                    length = 0;
                    for (j = 0; j < jArray.Count;)
                    {
                        if (jArray[j].Equals(Share.stations[(lis[i].before)].name))
                        {
                            if ((i > 0) && !(lis[i].beforestop == 0))
                            {
                                if (Share.stations[(lis[i].before)].name.Length < max_length)
                                {
                                    for (int v = 0; v < getLen(max_length, Share.stations[(lis[i].before)].name.Length)[0]; v++)
                                        sb.Append(" ");
                                    sb.Append(Share.stations[(lis[i].before)].name);
                                    for (int v = 0; v <= getLen(max_length, Share.stations[(lis[i].before)].name.Length)[1]; v++)
                                        sb.Append(" ");
                                }
                                else
                                    sb.Append(Share.stations[(lis[i].before)].name);
                                tttmp = new TimeTime(0, 0, false);
                                minute = min % 60;
                                hourOfDay = min / 60;
                                hourOfDay = hourOfDay % 24;
                                sb.Append(" ");
                                tmp1 = minute < 10 ? "0" + minute.ToString() : minute.ToString();
                                tmp3 = hourOfDay < 10 ? "0" + hourOfDay.ToString() : hourOfDay.ToString();
                                sb.Append(tmp3);
                                sb.Append(":");
                                sb.Append(tmp1);
                                m2 = min;
                                tttmp.arriveTime = m2;
                                min += lis[i].beforestop;
                                minute = min % 60;
                                hourOfDay = min / 60;
                                hourOfDay = hourOfDay % 24;
                                tmp1 = minute < 10 ? "0" + minute.ToString() : minute.ToString();
                                tmp3 = hourOfDay < 10 ? "0" + hourOfDay.ToString() : hourOfDay.ToString();
                                sb.Append(" ");
                                sb.Append(tmp3);
                                sb.Append(":");
                                sb.Append(tmp1);
                                sb.Append(" ");
                                m2 = min;
                                tttmp.deparTime = m2;
                                tttmp.teg = lis[i].beforeteg;
                                if (lis[i].beforeteg)
                                    sb.Append("技停\r\n");
                                else
                                {
                                    sb.Append(lis[i].beforestop);
                                    sb.Append("分\r\n");
                                }
                                if (up)
                                    uptimes.Add(tttmp);
                                else
                                    downtimes.Add(tttmp);
                            }
                            b = true;
                            length = 0;
                        }
                        tmp = jArray[j];
                        if (jArray[j].Equals(Share.stations[(lis[i].after)].name))
                        {
                            if (b == false)
                            {
                                b1 = true;
                                jArray = new List<string>();
                                spd = new List<string>();
                                lengths = new List<double>();
                                speeds = new List<string>();
                                length = 0;
                                for (int t = Share.rails[(lis[i].line)].stations.Count - 1; t >= 0; t--)
                                    jArray.Add(Share.rails[(lis[i].line)].stations[t]);
                                tmp2 = Share.rails[(lis[i].line)].speeds.Split('\n');
                                for (int t = tmp2.Length - 1; t >= 0; t--)
                                {
                                    if ((!tmp2[t].StartsWith("atp"))
                                            && (!tmp2[t].StartsWith("ms"))
                                            && (!tmp2[t].StartsWith("ss"))
                                            && (!tmp2[t].StartsWith("1")) && (!tmp2[t].StartsWith("2")) && (!tmp2[t].StartsWith("3"))
                                            && (!tmp2[t].StartsWith("4")) && (!tmp2[t].StartsWith("5")) && (!tmp2[t].StartsWith("6"))
                                            && (!tmp2[t].StartsWith("7")) && (!tmp2[t].StartsWith("8")) && (!tmp2[t].StartsWith("9")))
                                        speeds.Add(tmp2[t]);
                                    spd.Add(tmp2[t]);
                                }
                                for (int t = Share.rails[(lis[i].line)].lengths.Count - 1; t >= 0; t--)
                                    lengths.Add(Share.rails[(lis[i].line)].lengths[t]);
                                j = 0;
                                b = false;
                                continue;
                            }
                            foreach (String stat in speeds)
                            {
                                b3 = false;
                                m = -1;
                                for (int l = j; l < jArray.Count; l++)
                                {
                                    if (jArray[l].Equals(stat))
                                    {
                                        m = l;
                                        b3 = true;
                                        break;
                                    }
                                }
                                if (jArray[j].Equals(stat) || b3)
                                {
                                    b2 = false;
                                    aspeed = 0;
                                    for (int k = 0; k < spd.Count; k++)
                                    {
                                        if (spd[k].Equals(stat))
                                            break;
                                        if (spd[k].StartsWith("1") || spd[k].StartsWith("2") ||
                                                spd[k].StartsWith("3") || spd[k].StartsWith("4") ||
                                                spd[k].StartsWith("5") || spd[k].StartsWith("6") ||
                                                spd[k].StartsWith("7") || spd[k].StartsWith("8") ||
                                                spd[k].StartsWith("9") || spd[k].StartsWith("0"))
                                            _speed = int.Parse(spd[k]);
                                        else if (spd[k].StartsWith("ss") && (lis[i].mode == RunMode.Express))
                                            _speed = int.Parse(spd[k].Substring(3));
                                        else if (spd[k].StartsWith("ms") && (lis[i].mode != RunMode.Express) && (b2 != true))
                                            _speed = int.Parse(spd[k].Substring(3));
                                        else if (spd[k].StartsWith("atp") && (lis[i].mode == RunMode.MaxSpeed))
                                        {
                                            _speed = int.Parse(spd[k].Substring(4));
                                            b2 = true;
                                        }
                                    }
                                    b2 = false;
                                    if (_speed > speed)
                                    {
                                        if (speed == 310)
                                            speed += 5;
                                        switch (lis[i].mode)
                                        {
                                            case RunMode.ATP:
                                                _speed = speed - 6;
                                                break;
                                            case RunMode.Express:
                                                _speed = speed - 15;
                                                break;
                                            case RunMode.MaxSpeed:
                                                _speed = speed - 10;
                                                break;
                                        }
                                    }
                                    if (cbx_no350mode.Checked && _speed > 310)
                                        _speed = 305;
                                    //找到下条线路限速
                                    /*if (i < lis.Count - 1) {
                                        if (lis[i].afterstop == 0) {
                                            c1 = false;
                                            c5 = false;
                                            tpd = new List<>();
                                            tpeeds = new List<>();
                                            ump2 = Share.rails.get(lis.get(i + 1).line).speeds.Split('\n');
                                            for (int s = 0; s < ump2.Length; s++) {
                                                if ((!ump2[s].StartsWith("atp"))
                                                        && (!ump2[s].StartsWith("ms"))
                                                        && (!ump2[s].StartsWith("ss"))
                                                        && (!ump2[s].StartsWith("1")) && (!ump2[s].StartsWith("2")) && (!ump2[s].StartsWith("3"))
                                                        && (!ump2[s].StartsWith("4")) && (!ump2[s].StartsWith("5")) && (!ump2[s].StartsWith("6"))
                                                        && (!ump2[s].StartsWith("7")) && (!ump2[s].StartsWith("8")) && (!ump2[s].StartsWith("9")))
                                                    tpeeds.Add(ump2[s]);
                                                tpd.Add(ump2[s]);
                                            }
                                            b1 = false;
                                            c = false;
                                            kArray = Share.rails.get(lis.get(i + 1).line).stations;
                                            mengths = Share.rails.get(lis.get(i + 1).line).lengths;
                                            for (int n = 0; n < Share.rails.get(lis.get(i + 1).line).stations.Count; n++) {
                                                if (kArray.get(n).Equals(Share.stations.get(lis.get(i + 1).after).name)) {
                                                    if (c == false) {
                                                        c1 = true;
                                                        kArray = new List<>();
                                                        tpd = new List<>();
                                                        mengths = new List<>();
                                                        tpeeds = new List<>();
                                                        for (int t = Share.rails.get(lis.get(i + 1).line).stations.Count - 1; t >= 0; t--)
                                                            kArray.Add(Share.rails.get(lis.get(i + 1).line).stations[t]);
                                                        ump2 = Share.rails.get(lis.get(i + 1).line).speeds.Split('\n');
                                                        for (int t = ump2.Length - 1; t >= 0; t--) {
                                                            if ((!ump2[t].StartsWith("atp"))
                                                                    && (!ump2[t].StartsWith("ms"))
                                                                    && (!ump2[t].StartsWith("ss"))
                                                                    && (!ump2[t].StartsWith("1")) && (!ump2[t].StartsWith("2")) && (!ump2[t].StartsWith("3"))
                                                                    && (!ump2[t].StartsWith("4")) && (!ump2[t].StartsWith("5")) && (!ump2[t].StartsWith("6"))
                                                                    && (!ump2[t].StartsWith("7")) && (!ump2[t].StartsWith("8")) && (!ump2[t].StartsWith("9")))
                                                                tpeeds.Add(ump2[t]);
                                                            tpd.Add(ump2[t]);
                                                        }
                                                        for (int t = Share.rails.get(lis.get(i + 1).line).lengths.Count - 1; t >= 0; t--)
                                                            mengths.Add(Share.rails.get(lis.get(i + 1).line).lengths[t]);
                                                        n = 0;
                                                        c = false;
                                                        continue;
                                                    }
                                                    else
                                                        break;
                                                }
                                                if (kArray.get(n).Equals(Share.stations.get(lis.get(i + 1).before).name))
                                                    c = true;
                                                if (c) {
                                                    for (String ttat:tpeeds) {
                                                        c3 = false;
                                                        o = -1;
                                                        for (int l = n; l < kArray.Count; l++) {
                                                            if (kArray[l].Equals(ttat)) {
                                                                o = l;
                                                                c3 = true;
                                                                break;
                                                            }
                                                        }
                                                        if((kArray.get(n).Equals(ttat) || c3) && (!kArray.get(n).Equals(Share.stations.get(lis.get(i + 1).before).name))) {
                                                            c2 = false;
                                                            c4 = false;
                                                            for(int k = 0; k < tpd.Count; k++) {
                                                                if(tpd[k].Equals(ttat))
                                                                    break;
                                                                if(tpd[k].StartsWith("1")||tpd[k].StartsWith("2")||
                                                                        tpd[k].StartsWith("3")||tpd[k].StartsWith("4")||
                                                                        tpd[k].StartsWith("5")||tpd[k].StartsWith("6")||
                                                                        tpd[k].StartsWith("7")||tpd[k].StartsWith("8")||
                                                                        tpd[k].StartsWith("9")||tpd[k].StartsWith("0"))
                                                                    aspeed = int.Parse(tpd[k]);
                                                                else if (tpd[k].StartsWith("ss") && (lis[i].mode == RunMode.Express))
                                                                    aspeed = int.Parse(tpd[k].Substring(3));
                                                                else if (tpd[k].StartsWith("ms") && (lis[i].mode != RunMode.Express) && (c4 != true)) {
                                                                    if((lis[i].mode != RunMode.Express))
                                                                        aspeed = int.Parse(tpd[k].Substring(3));
                                                                }
                                                                else if (tpd[k].StartsWith("atp") && (lis[i].mode == RunMode.MaxSpeed)) {
                                                                    aspeed = int.Parse(tpd[k].Substring(4));
                                                                    c4 = true;
                                                                }
                                                            }
                                                            c2 = false;
                                                            c4 = false;
                                                            if(aspeed > speed) {
                                                                if(speed == 310)
                                                                    speed += 5;
                                                                switch (lis[i].mode) {
                                                                    case RunMode.MaxSpeed:
                                                                        aspeed = speed - 6;
                                                                        break;
                                                                    case RunMode.Express:
                                                                        aspeed = speed - 15;
                                                                    case RunMode.MaxSpeed:
                                                                        aspeed = speed - 10;
                                                                        break;
                                                                }
                                                            }
                                                            c5 = true;
                                                            break;
                                                        }
                                                    }
                                                    if (c5) {
                                                        c5 = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                            aspeed = 0;
                                    }*/
                                    min += Calc(length, ast, bspeed, aspeed, _speed, lis[i].earlytime);
                                    bspeed = _speed;
                                    if (aspeed == 0) bspeed = 0;
                                    length = 0;
                                    break;
                                }
                            }
                            if (i == lis.Count - 1)
                            {
                                if (lis[i].afterstop != 0)
                                {
                                    if (Share.stations[(lis[i].after)].name.Length < max_length)
                                    {
                                        for (int v = 0; v < getLen(max_length, Share.stations[(lis[i].after)].name.Length)[0]; v++)
                                            sb.Append(" ");
                                        sb.Append(Share.stations[(lis[i].after)].name);
                                        for (int v = 0; v <= getLen(max_length, Share.stations[(lis[i].after)].name.Length)[1]; v++)
                                            sb.Append(" ");
                                    }
                                    else
                                        sb.Append(Share.stations[(lis[i].after)].name);
                                    minute = min % 60;
                                    hourOfDay = min / 60;
                                    hourOfDay = hourOfDay % 24;
                                    sb.Append(" ");
                                    tmp1 = minute < 10 ? "0" + minute.ToString() : minute.ToString();
                                    tmp3 = hourOfDay < 10 ? "0" + hourOfDay.ToString() : hourOfDay.ToString();
                                    sb.Append(tmp3);
                                    sb.Append(":");
                                    sb.Append(tmp1);
                                    sb.Append("  --:--   --");
                                    tttmp = new TimeTime(0, 0, false);
                                    m2 = min;
                                    tttmp.teg = false;
                                    tttmp.deparTime = -1;
                                    tttmp.arriveTime = m2;
                                    if (up)
                                        uptimes.Add(tttmp);
                                    else
                                        downtimes.Add(tttmp);
                                }
                                b = true;
                                length = 0;
                            }
                            //else
                            //    sb.Append("\n");
                            break;
                        }
                        if (b)
                        {
                            //if(!b1 && j > 0)
                            //    length += lengths.get(j-1);
                            foreach (String stat in speeds)
                            {
                                if (jArray[j].Equals(stat) && (!jArray[j].Equals(Share.stations[(lis[i].before)].name)))
                                {
                                    b2 = false;
                                    b4 = false;
                                    aspeed = 0;
                                    for (int k = 0; k < spd.Count; k++)
                                    {
                                        if (spd[k].Equals(stat))
                                        {
                                            b2 = true;
                                            b4 = false;
                                        }
                                        if (spd[k].StartsWith("1") || spd[k].StartsWith("2") ||
                                                spd[k].StartsWith("3") || spd[k].StartsWith("4") ||
                                                spd[k].StartsWith("5") || spd[k].StartsWith("6") ||
                                                spd[k].StartsWith("7") || spd[k].StartsWith("8") ||
                                                spd[k].StartsWith("9") || spd[k].StartsWith("0")
                                        )
                                        {
                                            if (b2)
                                            {
                                                aspeed = int.Parse(spd[k]);
                                                break;
                                            }
                                            else
                                                _speed = int.Parse(spd[k]);
                                        }
                                        else if (spd[k].StartsWith("ss") && (lis[i].mode == RunMode.Express))
                                        {
                                            if (b2)
                                            {
                                                aspeed = int.Parse(spd[k].Substring(3));
                                                break;
                                            }
                                            else
                                                _speed = int.Parse(spd[k].Substring(3));
                                        }
                                        else if (spd[k].StartsWith("ms") && (lis[i].mode != RunMode.Express) && (b4 != true))
                                        {
                                            if (b2)
                                            {
                                                aspeed = int.Parse(spd[k].Substring(3));
                                                if ((lis[i].mode == RunMode.MaxSpeed))
                                                    break;
                                            }
                                            else if ((lis[i].mode != RunMode.Express))
                                                _speed = int.Parse(spd[k].Substring(3));
                                        }
                                        else if (spd[k].StartsWith("atp") && (lis[i].mode == RunMode.MaxSpeed))
                                        {
                                            b4 = true;
                                            if (b2)
                                            {
                                                aspeed = int.Parse(spd[k].Substring(4));
                                                break;
                                            }
                                            else
                                                _speed = int.Parse(spd[k].Substring(4));
                                        }
                                        /*else if (lis[i].mode == RunMode.MaxSpeed) {
                                            if (b2)
                                                break;
                                        }*/
                                    }
                                    b2 = false;
                                    b4 = false;
                                    if (_speed > speed)
                                    {
                                        if (speed == 310)
                                            speed += 5;
                                        switch (lis[i].mode)
                                        {
                                            case RunMode.ATP:
                                                _speed = speed - 6;
                                                break;
                                            case RunMode.Express:
                                                _speed = speed - 15;
                                                break;
                                            case RunMode.MaxSpeed:
                                                _speed = speed - 10;
                                                break;
                                        }
                                    }
                                    if (cbx_no350mode.Checked && _speed > 310)
                                        _speed = 305;
                                    min += Calc(length, ast, bspeed, aspeed, _speed, 0);
                                    bspeed = _speed;
                                    if (aspeed == 0) bspeed = 0;
                                    length = 0;
                                    break;
                                }
                            }
                            if (j < lengths.Count)
                                length += lengths[j];
                        }// if b==true
                        j++;
                    }
                    if (b1)
                    {
                        b1 = false;
                    }
                }
            }
            catch (Exception e) { }
            return sb.ToString();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_isAstar.Checked)
            {
                cbo_line.Enabled = false;
                cbo_line.Items.Clear();
                cbo_after.Items.Clear();
                cbo_after.DropDownStyle = ComboBoxStyle.DropDown;
                cbo_after.Enabled = true;
                foreach (Station stats in Share.stations)
                {
                    cbo_after.Items.Add(stats.name);
                }
            }
            else
            {
                if (cbo_line.SelectedIndex == -1)
                    cbo_after.Enabled = false;
                cbo_line.Enabled = true;
                cbo_line.Items.Clear();
                cbo_after.Items.Clear();
                cbo_after.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }
        private void 设置CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingForm().Show();
        }
        private void 新建NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(New) { IsBackground = false, ApartmentState = ApartmentState.STA }.Start();
            this.Close();
        }
        [STAThread] public void New()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        private void mtb_uptime_TextChanged(object sender, EventArgs e)
        {
            if (mtb_uptime.Text.Replace(":", "").Length == 4)
            {
                uphour = int.Parse(mtb_uptime.Text.Replace(":", "").Substring(0, 2));
                upmin = int.Parse(mtb_uptime.Text.Replace(":", "").Substring(2, 2));
                SetTime();
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\isclose.dta", "");
        }
        private void mtb_downtime_TextChanged(object sender, EventArgs e)
        {
            if (mtb_downtime.Text.Replace(":", "").Length == 4)
            {
                downhour = int.Parse(mtb_downtime.Text.Replace(":", "").Substring(0, 2));
                downmin = int.Parse(mtb_downtime.Text.Replace(":", "").Substring(2, 2));
                SetTime();
            }
        }
        private void btn_up_Click(object sender, EventArgs e)
        {
            if (select)
            {
                select = false;
                btn_down.BackColor = Color.FromArgb(12, 8, 178);
                btn_up.BackColor = Color.FromArgb(15, 175, 207);
            }
            else
            {
                select = true;
                btn_up.BackColor = Color.FromArgb(12, 8, 178);
                btn_down.BackColor = Color.FromArgb(15, 175, 207);
            }
            if (lis.Count < 1)
            {
                selectB = -1;
                tbx_before.Enabled = true;
                保存SToolStripMenuItem.Enabled = false;
                导出EToolStripMenuItem.Enabled = false;
                tbx_before.Text = "";
            }
            else if (!select)
            {
                selectB = lis[lis.Count - 1].after;
                tbx_beforestop.Text = lis[lis.Count - 1].afterstop.ToString();
            }
            else
            {
                selectB = lis[0].before;
                tbx_beforestop.Text = lis[0].beforestop.ToString();
            }
            if (lis.Count > 0)
            {
                tbx_before.Text = Share.stations[selectB].name;
                cbx_beforeteg.Checked = select ? lis[0].beforeteg : lis[lis.Count - 1].beforeteg;
            }
            selectL = selectA = -1;
            cbo_line.Items.Clear();
            cbo_after.Items.Clear();
            btn_submit.Enabled = false;
            cbo_after.Enabled = false;
            tbx_afterstop.Text = "2";
            tbx_etim.Text = "5";
            cbx_afterteg.Checked = false;
            rb_exp.Checked = true;
            if (cbx_isAstar.Checked)
                cbo_after.Enabled = true;
        }
        private void 帮助HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://space.bilibili.com/621814881");
        }
        private void 关于动车线路编辑器AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }
        public int[] getLen(int big, int small)
        {
            if ((((big - small) * 3) % 2) == 0)
                return new int[] { ((big - small) * 3) / 2, ((big - small) * 3) / 2 };
            else
                return new int[] { (((big - small) * 3) + 1) / 2, ((((big - small) * 3) + 1) / 2) - 1 };
        }
        public int Calc(double _long, double ast, int bspeed, int aspeed, int speed, int early)
        {
            double h, m, s, d, tmp2 = 0;
            if (bspeed < speed)
                tmp2 = ((speed + 1) * (speed / 2) / ast / 3600) - ((bspeed + 1) * (bspeed / 2) / ast / 3600);
            h = _long - tmp2;
            tmp2 = 0.0;
            if (aspeed < speed)
                tmp2 = ((speed + 1) * (speed / 2) / ast / 3600) - ((aspeed + 1) * (aspeed / 2) / ast / 3600);
            h = h - tmp2;
            if (h < 0)
            {
                speed = (int)Math.Sqrt(Math.Pow(0 - (bspeed * 2) + (aspeed * 2), 2) - (4 * 2 * ((aspeed * aspeed) + (bspeed * bspeed) - (_long * 7200 * ast))));
                tmp2 = 0;
                if (bspeed < speed)
                    tmp2 = ((speed + 1) * (speed / 2) / ast / 3600) - ((bspeed + 1) * (bspeed / 2) / ast / 3600);
                h = _long - tmp2;
                tmp2 = 0.0;
                if (aspeed < speed)
                    tmp2 = ((speed + 1) * (speed / 2) / ast / 3600) - ((aspeed + 1) * (aspeed / 2) / ast / 3600);
                h = h - tmp2;
            }
            h = h / speed;
            tmp2 = 0;
            if (bspeed < speed)
                tmp2 = (speed - bspeed) / ast / 3600.0;
            h = h + tmp2;
            tmp2 = 0;
            if (aspeed < speed)
                tmp2 = (speed - aspeed) / ast / 3600.0;
            h = h + tmp2;
            s = h * 3600.0;
            s = Math.Ceiling(s);
            s += 60 - (s % 60);
            m = Math.Ceiling(s / 60);
            m += early;/*
        s = ((int)s)%60;
        h = floor(m/60);
        m = ((int)m)%60;
        d = floor(h/24);
        h = ((int)h)%24;*/
            return (int)m;
        }
        public List<ListItem> AStar(int start, int end, int bstop, int astop, int delay, RunMode rm, bool bteg, bool ateg)
        {
            List<List<DFSItem>> dfsis = new List<List<DFSItem>>();
            List<ListItem> liss = new List<ListItem>();
            List<ListItem> r = new List<ListItem>();
            double lat, lng = lat = -1;
            //bool[,,] fake = new bool[Share.stations.Count, Share.rails.Count, Share.stations.Count];
            List<int[]> visit = new List<int[]>();
            int tmp1 = -1;
            int tmp2 = -1;
            int tmp3 = -1;
            int num = 0;
            List<int> sns = new List<int>();
            List<double> pos = new List<double>();
            List<int> lin = new List<int>();
            List<int> lin2 = new List<int>();
            bool b = false;
            bool bIsSmall = false;
            bool b1 = false;
            bool b2 = false;
            bool isstovf = false;
            tmp1 = -1;
            tmp2 = -1;
            double sortmp1 = -1;
            int sortmp2 = -1;
            bool zc;
            int i;
            int j;
            int k;
            List<DFSItem> it = new List<DFSItem>();
            foreach (Rail item in Share.rails)
            {
                foreach (JToken i2 in item.locations)
                {
                    if ((!((string)i2["name"]).ToLower().StartsWith("x")) && (((string)i2["type"]) == "station"))
                    {
                        if (((string)i2["name"]).ToLower() == Share.stations[end].name)
                        {
                            lat = (double)i2["lat"];
                            lng = (double)i2["lng"];
                            break;
                        }
                    }
                }
                if (lng != -1)
                    break;
            }
            void astar(int st, int nd, int th)
            {
                try
                {
                    it.Add(new DFSItem(st, -1));
                    num++;
                    if (num > 399)
                        return;
                    if (st == nd)
                    {
                        dfsis.Add(it);
                    }
                    else
                    {
                        sns = new List<int>();
                        pos = new List<double>();
                        lin = new List<int>();
                        lin2 = new List<int>();
                        b = false;
                        bIsSmall = false;
                        b1 = false;
                        b2 = false;
                        tmp1 = -1;
                        tmp2 = -1;
                        sortmp1 = -1;
                        sortmp2 = -1;
                        for (i = 0; i < ssns.Count; i++)
                        {
                            if (ssns[i] == it[it.Count - 1].before)
                            {
                                bIsSmall = true;
                                zc = false;
                                for (j = 0; j < sss[i].Count; j++)
                                {
                                    for (k = 0; k < sss[i][j].Count; k++)
                                    {
                                        if ((it.Count < 2) && (lis.Count == 0))
                                        {
                                            b1 = true;
                                            break;
                                        }
                                        else if ((it.Count > 2) && (zc == false))
                                        {
                                            if ((sss[i][j][k] == it[it.Count - 2].line))
                                            {
                                                zc = true;
                                                b = true;
                                                k = 0;
                                            }
                                        }
                                        else if ((zc == false) && (it.Count < 2))
                                        {
                                            if (sss[i][j][k] == lis[(lis.Count - 1)].line)
                                            {
                                                zc = true;
                                                b = true;
                                                k = 0;
                                            }
                                        }
                                        if (zc)
                                            sns.Add(sss[i][j][k]);
                                    }
                                    if (b1)
                                        break;
                                    zc = false;
                                }
                                break;
                            }
                        }
                        if (bIsSmall == false)
                            sns = Share.stations[it[it.Count - 1].before].rails;
                        foreach (int item in sns)
                        {
                            tmp1 = -1;
                            for (i = 0; i < Share.rails[item].stations.Count; i++)
                            {
                                if (Share.rails[item].stations[i] == Share.stations[st].name)
                                {
                                    tmp1 = i;
                                    break;
                                }
                            }
                            if (tmp1 > 0)
                            {
                                for (i = 0; i < Share.rails[item].locations.Count; i++)
                                {
                                    if (((String)(Share.rails[item].locations[i]["name"])) == (Share.rails[item].stations[tmp1 - 1]))
                                    {
                                        pos.Add(GetLength(lng, lat, ((double)(Share.rails[item].locations[i]["lng"])), ((double)(Share.rails[item].locations[i]["lat"]))));
                                        lin2.Add(item);
                                        for (j = 0; j < Share.stations.Count; j++)
                                        {
                                            for (tmp2 = 1; tmp2 < Share.rails[item].stations.Count; tmp2++)
                                            {
                                                //GC.Collect();
                                                if (!Share.rails[item].stations[tmp1 - tmp2].ToLower().StartsWith("x"))
                                                    break;
                                            }
                                            if (Share.stations[j].name == Share.rails[item].stations[tmp1 - tmp2])
                                            {
                                                lin.Add(j);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            if (tmp1 + 1 < Share.rails[item].stations.Count)
                            {
                                for (i = 0; i < Share.rails[item].locations.Count; i++)
                                {
                                    if (((String)(Share.rails[item].locations[i]["name"])) == (Share.rails[item].stations[tmp1 + 1]))
                                    {
                                        pos.Add(GetLength(lng, lat, ((double)(Share.rails[item].locations[i]["lng"])), ((double)(Share.rails[item].locations[i]["lat"]))));
                                        lin2.Add(item);
                                        for (j = 0; j < Share.stations.Count; j++)
                                        {
                                            for (tmp2 = 1; tmp2 < Share.rails[item].stations.Count; tmp2++)
                                            {
                                                //GC.Collect();
                                                if (!Share.rails[item].stations[tmp1 + tmp2].ToLower().StartsWith("x"))
                                                    break;
                                            }
                                            if (Share.stations[j].name == Share.rails[item].stations[tmp1 + tmp2])
                                            {
                                                lin.Add(j);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        for (i = 0; i < pos.Count - 1; i++)
                        {
                            for (j = 0; j < pos.Count - 1 - i; j++)
                            {
                                if (pos[j] > pos[j + 1])
                                {
                                    sortmp1 = pos[j];
                                    pos[j] = pos[j + 1];
                                    pos[j + 1] = sortmp1;
                                    sortmp2 = lin[j];
                                    lin[j] = lin[j + 1];
                                    lin[j + 1] = sortmp2;
                                    sortmp2 = lin2[j];
                                    lin2[j] = lin2[j + 1];
                                    lin2[j + 1] = sortmp2;
                                }
                            }
                        }
                        if (lin.Count == 0)
                        {
                            it.RemoveAt(it.Count - 1);
                            astar(it[it.Count - 1].before, nd, th);
                            if (num > Share.stations.Count * 2)
                                return;
                            if (num > 400)
                                return;
                        }
                        else if (th - 1 < pos.Count)
                        {
                            it[it.Count - 1].line = lin2[th - 1];
                            //string str = Share.stations[lin[th - 1]].name;
                            foreach (int[] item in visit)
                            {
                                if (((item[0] == st) && (item[1] == lin2[lin2.Count - 1]) && (item[2] == lin[th - 1])) ||
                                    ((item[2] == st) && (item[1] == lin2[lin2.Count - 1]) && (item[0] == lin[th - 1])))
                                {
                                    for (i = 1; i < pos.Count; i++)
                                    {
                                        if (th - 1 + i < pos.Count)
                                        {
                                            b2 = false;
                                            foreach (int[] item2 in visit)
                                            {
                                                b2 = b2 || (((item2[0] == st) && (item2[1] == lin2[lin2.Count - 1]) && (item2[2] == lin[th - 1 + i])) ||
                                                    ((item2[2] == st) && (item2[1] == lin2[lin2.Count - 1]) && (item2[0] == lin[th - 1 + i])));
                                            }
                                            if (!b2)
                                            {
                                                it[it.Count - 1].line = lin2[th - 1 + i];
                                                visit.Add(new int[3] { st, lin2[th - 1 + i], lin[th - 1 + i] });
                                                //visit[st, lin2[th - 1 + i], lin[th - 1 + i]] = true;
                                                //visit[lin[th - 1 + i], lin2[th - 1 + i], st] = true;
                                                //GC.Collect();
                                                astar(lin[th - 1 + i], nd, th);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            it.RemoveAt(it.Count - 1);
                                            //GC.Collect();
                                            astar(it[it.Count - 1].before, nd, th);
                                            return;
                                        }
                                    }
                                }
                            }
                            visit.Add(new int[3] { st, lin2[th - 1], lin[th - 1] });
                            //visit[lin[th - 1], lin2[th - 1], st] = true;
                            astar(lin[th - 1], nd, th);
                            if (num > Share.stations.Count * 2)
                                return;
                            if (num > 400)
                                return;
                        }
                        else
                        {
                            it[it.Count - 1].line = lin2[lin2.Count - 1];
                            //string str = Share.stations[lin[lin.Count - 1]].name;
                            //|| visit[lin[lin.Count - 1], lin2[lin2.Count - 1], st]
                            foreach (int[] item in visit)
                            {
                                if (((item[0] == st) && (item[1] == lin2[lin2.Count - 1]) && (item[2] == lin[lin.Count - 1])) ||
                                    ((item[2] == st) && (item[1] == lin2[lin2.Count - 1]) && (item[0] == lin[lin.Count - 1])))
                                {
                                    it.RemoveAt(it.Count - 1);
                                    astar(it[it.Count - 1].before, nd, th);
                                    return;
                                }
                            }
                            visit.Add(new int[3] { st, lin2[lin2.Count - 1], lin[lin.Count - 1] });
                            //visit[lin[lin.Count - 1], lin2[lin2.Count - 1], st] = true;
                            astar(lin[lin.Count - 1], nd, th);
                            if (num > Share.stations.Count * 2)
                                return;
                            if (num > 400)
                                return;
                        }
                    }
                }
                catch (StackOverflowException ex)
                {
                    num = Share.stations.Count * 3;
                    isstovf = true;
                    return;
                }
            }
            //GC.Collect();
            /*for (int i = 0; i < Share.stations.Count; i++)
                for (int j = 0; j < Share.rails.Count; j++)
                    for (int k = 0; k < Share.stations.Count; k++)
                        visit[i, j, k] = true;
            for (int i = 0; i < Share.stations.Count; i++)
                for (int j = 0; j < Share.rails.Count; j++)
                    for (int k = 0; k < Share.stations.Count; k++)
                        visit[i, j, k] = false;*/
            astar(start, end, 1);
            GC.Collect();
            if (num > 350)
            {
                MessageBox.Show("路径过于复杂！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return new List<ListItem>();
            }
            tmp1 = -1;
            tmp2 = -1;
            tmp3 = -1;
            for (i = 0; i < dfsis[0].Count; i++)
            {
                if (i == 47)
                    i = i;
                if (dfsis[0][i].line == tmp2)
                    liss[liss.Count - 1].after = dfsis[0][i].before;
                else if (i > 0)
                {
                    if (dfsis[0][i - 1].line == tmp2)
                        liss[liss.Count - 1].after = dfsis[0][i].before;
                }
                if ((dfsis[0][i].line > -1) && (dfsis[0][i].line != tmp2))
                    liss.Add(new ListItem(dfsis[0][i].before, 0, dfsis[0][i].line, rm, dfsis[0][i + 1].before, 0, 0, false, false));
                tmp2 = dfsis[0][i].line;
            }
            /*for (int i = 0; i < liss.Count; i++)
            {
                if (tmp1 == -1)
                {
                    tmp1 = liss[i].before;
                    tmp2 = liss[i].line;
                }
                if (liss[i].line == tmp2)
                {
                    tmp3 = liss[i].after;
                }
                else
                {
                    r.Add(new ListItem(tmp1, 0, tmp2, rm, tmp3, 0, 0, false, false));
                    tmp1 = -1;
                    tmp2 = -1; 
                }
                if((liss[i].line == tmp2) && (i + 1 >= liss.Count))
                    r.Add(new ListItem(tmp1, 0, tmp2, rm, tmp3, 0, delay, false, ateg));
                else if(i + 1 >= liss.Count)
                    r.Add(new ListItem(liss[i].before, 0, liss[i].line, rm, liss[i].after, 0, delay, false, ateg));
                if (tmp1 == -1)
                {
                    tmp1 = liss[i].before;
                    tmp2 = liss[i].line;
                }
            }*/
            r = liss;
            //r.Add(new ListItem(tmp1, 0, tmp2, rm, tmp3, 0, delay, false, ateg));
            r[0].beforestop = bstop;
            r[0].beforeteg = bteg;
            r[r.Count - 1].afterstop = astop;
            r[r.Count - 1].earlytime = delay;
            r[r.Count - 1].afterteg = ateg;
            for (i = 0; i < r.Count; i++)
            {
                if (r[i].before == r[i].after)
                {
                    r[i + 1].beforestop = r[i].beforestop;
                    r.RemoveAt(i);
                    i = 0;
                }
            }
            return r;
        }
    }
}