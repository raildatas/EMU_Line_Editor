using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace ELE
{
    public partial class Form1 : Form
    {
        string FTPCONSTR = "ftp://012.3vftp.cn:3535/";
        string FTPUSERNAME = "crh380b";
        string FTPPASSWORD = "142410";
        string de1 = null;
        bool open = false;
        Boolean select = false;
        Boolean flag = true;
        int upmin = 6, uphour = 10, downmin = 6, downhour = 10;
        int selectB = -1, selectL = -1, selectA = -1, selectC = -1;
        List<String> emus = new List<string>();
        Dictionary<String, String> dic = new Dictionary<String, String>();
        Dictionary<String, String> dic2 = new Dictionary<String, String>();
        Dictionary<String, Ticket> tkl = new Dictionary<String, Ticket>();
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
        static double EARTH_RADIUS = 6378137;

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
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s / 1000.0;
        }
        public SeatType GetSeatType(String str)
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
                default:
                    return SeatType.UK;
            }
        }
        public String ToString(SeatType st)
        {
            switch (st)
            {
                case SeatType.ZE:
                    return "二等座";
                case SeatType.D:
                    return "动力车";
                case SeatType.CA:
                    return "餐车";
                case SeatType.WG:
                    return "高级动卧";
                case SeatType.WR:
                    return "动卧";
                case SeatType.WY:
                    return "一等卧";
                case SeatType.WE:
                    return "二等卧";
                case SeatType.ZEC:
                    return "二等座";
                case SeatType.SW:
                    return "商务座";
                case SeatType.DGN:
                    return "多功能座";
                case SeatType.UY:
                    return "优选一等座";
                case SeatType.ZY:
                    return "一等座";
                case SeatType.ZT:
                    return "特等座";
                case SeatType.ZYC:
                    return "一等座";
                case SeatType.WRC:
                    return "动卧";
                case SeatType.BZ:
                    return "标准座";
                default:
                    return "未知席别";
            }
        }
        public String Gethtml(String url, int delay)
        {
            byte[] buffer = wc.DownloadData(url);
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
            Control.CheckForIllegalCrossThreadCalls = false;
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
            String[] ntfs;
            String html;
            double length;
            int test = 0;
            int y = 0;
            appdir = Directory.GetCurrentDirectory() + "\\";
            bool ei()
            {
                return File.Exists(appdir + "evnets.dta") && File.Exists(appdir + "ver.dta") && File.Exists(appdir + "emudata.dta") &&
                    File.Exists(appdir + "railwaydata.dta") && File.Exists(appdir + "replace.dta") && File.Exists(appdir + "rcp.dta") && File.Exists(appdir + "smallstations.dta");
            }
            if ((DateTime.Now.Year == 2025) && (DateTime.Now.Month == 4) && (DateTime.Now.Day <= 12))
            {
                if ((DateTime.Now.Day == 12) && (DateTime.Now.Hour >= 18))
                    goto conti;
                tabControl1.TabPages.RemoveAt(2);
            }
     conti: try
            {
                html = Gethtml("https://raildatas.github.io/2^9+1.html", 0).Replace("\r", "");
                ntfs = html.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ntfs.Length; i++)
                {
                    lbx_ntfs.Items.Add("UID: " + ntfs[i]);
                }
            }
            catch
            {
                lbx_ntfs.Items.Clear();
                lbx_ntfs.Items.Add("反火车迷攻击数据网站导致数据网站崩溃力~");
            }
            try
            {
                if (!ei())
                {
                    File.WriteAllText(appdir + "settings.ini", "dataLink=offical");
                }
                html = File.ReadAllText(appdir + "settings.ini");
                for (int i = 0; i < html.Split('=').Length; i += 2)
                    Share.settings.Add(html.Split('=')[i], html.Split('=')[i + 1]);
                string dl = Share.settings["dataLink"];
                if (dl.Equals("offical"))
                {
                    dl = "https://raildatas.github.io/";
                    try
                    {
                        if (!ei())
                            html = Download("ver.html");
                        else
                        {
                            html = Download("ver.html");
                            if (File.ReadAllText(appdir + "ver.dta").Equals(html))
                                throw new NullException();
                        }
                        File.WriteAllText(appdir + "ver.dta", html);
                        html = Download("emudata.html");
                        DataFiles.emuDatas = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "emudata.dta", html);
                        html = Download("railwaydata.html");
                        DataFiles.railDatas = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "railwaydata.dta", html);
                        html = Download("replace.html");
                        replaceDatas = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "replace.dta", html);
                        html = Download("rcp.html");
                        rcps = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "rcp.dta", html);
                        html = Download("smallstations.html");
                        ssstrs = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "smallstations.dta", html);
                        html = Download("evnets.html");
                        evens = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "evnets.dta", html);
                    }
                    catch (Exception ex)
                    {
                        if (ex is NullException)
                            throw new NullException();
                        if (!ei())
                            html = Gethtml(dl + "ver.html", 0);
                        else
                        {
                            html = Gethtml(Share.settings["dataLink"] + "ver.html", 10);
                            if (File.ReadAllText(appdir + "ver.dta").Equals(html))
                                throw new NullException();
                        }
                        File.WriteAllText(appdir + "ver.dta", html);
                        html = Gethtml(dl + "emudata.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                        DataFiles.emuDatas = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "emudata.dta", html);
                        html = Gethtml(dl + "railwaydata.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                        DataFiles.railDatas = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "railwaydata.dta", html);
                        html = Gethtml(dl + "replace.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                        replaceDatas = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "replace.dta", html);
                        html = Gethtml(dl + "rcp.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                        rcps = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "rcp.dta", html);
                        html = Gethtml(dl + "smallstations.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                        ssstrs = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "smallstations.dta", html);
                        html = Gethtml(dl + "evnets.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                        evens = replace(html, '\r').Split('\n');
                        File.WriteAllText(appdir + "evnets.dta", html);
                    }
                }
                else
                {
                    if (!ei())
                        html = Gethtml(dl + "ver.html", 0);
                    else
                    {
                        html = Gethtml(Share.settings["dataLink"] + "ver.html", 10);
                        if (File.ReadAllText(appdir + "ver.dta").Equals(html))
                            throw new NullException();
                    }
                    File.WriteAllText(appdir + "ver.dta", html);
                    html = Gethtml(dl + "emudata.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                    DataFiles.emuDatas = replace(html, '\r').Split('\n');
                    File.WriteAllText(appdir + "emudata.dta", html);
                    html = Gethtml(dl + "railwaydata.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                    DataFiles.railDatas = replace(html, '\r').Split('\n');
                    File.WriteAllText(appdir + "railwaydata.dta", html);
                    html = Gethtml(dl + "replace.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                    replaceDatas = replace(html, '\r').Split('\n');
                    File.WriteAllText(appdir + "replace.dta", html);
                    html = Gethtml(dl + "rcp.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                    rcps = replace(html, '\r').Split('\n');
                    File.WriteAllText(appdir + "rcp.dta", html);
                    html = Gethtml(dl + "smallstations.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                    ssstrs = replace(html, '\r').Split('\n');
                    File.WriteAllText(appdir + "smallstations.dta", html);
                    html = Gethtml(dl + "evnets.html", (File.Exists(appdir + "evnets.dta") ? 10 : 0));
                    evens = replace(html, '\r').Split('\n');
                    File.WriteAllText(appdir + "evnets.dta", html);
                }
            }
            catch (Exception e)
            {
                /*if(!(e is NullException))
                    this.Close();*/
                //System.exit(1);
                if ((!ei()) && false)
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
                cbo_tkn.SelectedIndex = 0;
                cbo_tkt.SelectedIndex = 30;
                if (rcps.Length > 0)
                {
                    selectC = 0;
                    cbo_rcp.Text = (rcps[0]);
                }
                //pbx_ticket.Image = Pic.GetBmp();
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
                try
                {
                    if (!replaceDatas[0].Equals(""))
                    {
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
                    }
                }
                catch { }
                foreach (var rci in rcps)
                {
                    cbo_rcp.Items.Add(rci);
                }
                cbo_rcp.SelectedIndex = 0;
                //StringBuilder sb2 = new StringBuilder();
                /*for (int i = 0; i < Share.rails.Count; i++)
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
                    for (int j = 0; j < Share.rails[i].lengths.Count; j++)
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
                    sb2.Append("\r\n\r\n");
                }*/
                //File.WriteAllText("rds.txt", sb2.ToString());
            }
            catch (Exception e)
            {
                //System.exit(1);
            }
            try
            {
                flag = false;
                foreach (EMU item6 in Share.emus)
                {
                    cbo_trainType.Items.Add(item6.name);
                }
                cbo_trainType.SelectedIndex = 0;
                cbo_trainSeat.SelectedIndex = 0;
            }
            catch { }
            try
            {
                if (file != "")
                    Open(file);
            }
            catch { MessageBox.Show("打开失败","错误", MessageBoxButtons.OK, MessageBoxIcon.Stop); }
            try
            {
                Bitmap bmp = new Bitmap(global::ELE.Properties.Resources.红前_01);
                Graphics g = Graphics.FromImage(bmp);
                String astn = tbx_tka.Text.Split('|')[0];
                int al = astn.Length;
                String bstn = tbx_tkb.Text.Split('|')[0];
                int bl = bstn.Length;
                float f = 491.7729f;
                if (astn.Length == 2)
                {
                    astn = astn.Insert(1, "    ");
                    al = 3;
                }
                if (bstn.Length == 2)
                {
                    bstn = bstn.Insert(1, "    ");
                    bl = 3;
                }
                //御坂|Misaka Mikoto
                //贴白|Tiebai
                //韧穿普|Wren Trumpull
                //咸淑娜|Xian Shuna
                //香港西九龙|Hongkongwestkowloon
                //忽任|Head of the Strategic Misinfomation Office (h)
                //北京城市副中心|Beijingchengshifuzhongxin
                //卡特洛斯|Kateluos
                g.DrawString(cbo_tkn.Items[cbo_tkn.SelectedIndex].ToString() + tbx_tkn.Text.Substring(3), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular),
                    new SolidBrush(Color.FromArgb(0xf9, 0x2f, 0x10)), 205.5277f - (209.4209f / 2), 112.6888f - (59.4258f / 2));
                g.DrawString(cbo_tkt.Items[cbo_tkt.SelectedIndex].ToString() + tbx_tkt.Text.Substring(3), new Font("宋体", 55 * 0.75f, FontStyle.Regular),
                    new SolidBrush(Color.FromArgb(0, 0, 0)), 529.536f - (151.25f / 2 * 0) - ((cbo_tkt.Items[cbo_tkt.SelectedIndex].ToString() + tbx_tkt.Text.Substring(3)).Length / 4.0f * 55f * 0.75f), 171.1038f - 30.25f);
                g.DrawString(astn, new Font("微软雅黑", 55 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - (al * 55f) - 10f, 168.2542f - (60.5f / 2) - 20);
                if (tbx_tka.Text.Contains("|"))
                {
                    if ((tbx_tka.Text.Split('|')[1].Length * 18) >= (al * 55)) //英语比中文长
                        g.DrawString(tbx_tka.Text.Split('|')[1], new Font("宋体", 36 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - (al * 55f) - 10f, 215.1326f - (39.6006f / 2) - 10f);
                    else //英语比中文短
                        g.DrawString(tbx_tka.Text.Split('|')[1], new Font("宋体", 36 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - 10f - (tbx_tka.Text.Split('|')[1].Length * 18f), 215.1326f - (39.6006f / 2) - 10f);
                }
                g.DrawString("站", new Font("宋体", 42 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f), 167.9109f - (46.2002f / 2));

                g.DrawString(bstn, new Font("微软雅黑", 55 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 345.7445f - (46.2002f / 2) - (bl * 55f), 168.2542f - (60.5f / 2) - 20);
                if (tbx_tka.Text.Contains("|"))
                {
                    g.DrawString(tbx_tkb.Text.Split('|')[1], new Font("宋体", 36 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 345.7445f - (46.2002f / 2) - (tbx_tkb.Text.Split('|')[1].Length * 18f), 215.1326f - (39.6006f / 2) - 10f);
                }
                g.DrawString("站", new Font("宋体", 42 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 345.7445f - (46.2002f / 2), 167.9109f - (46.2002f / 2));

                g.DrawString(tbx_tm.Text.Substring(3, 4), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    126.1459f - (111.6074f / 2) - 10f + (Count(tbx_tm.Text.Substring(3, 4), "1") * 10f), 265.1282f - (59.4258f / 2) + 5f);
                g.DrawString(tbx_tm.Text.Substring(8, 2), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    244.4638f - (56.2285f / 2) - 10f, 265.1282f - (59.4258f / 2) + 5f);
                g.DrawString(tbx_tm.Text.Substring(11, 2), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    338.2602f - (44.1377f / 2) - 15f, 265.1282f - (59.4258f / 2) + 5f);
                g.DrawString(tbx_tm.Text.Substring(14, 2), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    434.6693f - (44.6533f / 2) - 13f + (Count(tbx_tm.Text.Substring(14, 2), "1") * 8f), 265.1282f - (59.4258f / 2) + 5f);
                g.DrawString(tbx_tm.Text.Substring(17, 2), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    500.1137f - (55.1973f / 2) - 10f, 265.1282f - (59.4258f / 2) + 5f);

                g.DrawString(tbx_tkc.Text, new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    675.7763f - (56.7959f / 2) - 10f + (Count(tbx_tkc.Text, "1") * 10f), 265.1282f - (59.4258f / 2) + 5f);
                g.DrawString(tbx_tks.Text, new Font("宋体", 48 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    781.6835f - (79.2002f / 2) - 15f, 265.1282f - (59.4258f / 2));

                g.DrawString(tbx_tkp.Text.Split('|')[0], new Font("Noto Serif SC", 36 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    96.3871f - (30.334f / 2), 319.9993f - (69.4805f / 2));
                g.DrawString(tbx_tkp.Text.Split('|')[1], new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    165.8598f - (95.7256f / 2) + (tbx_tkp.Text.Split('|')[0].Length * 18f) - 15f, 316.7962f - (54.0234f / 2));
                g.DrawString(tbx_tkp.Text.Split('|')[2], new Font("宋体", 24 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    165.8598f - (95.7256f / 2) + (tbx_tkp.Text.Split('|')[0].Length * 18f) - 15f + ((tbx_tkp.Text.Split('|')[1].Length) * 24f), 314.8333f - (26.2002f / 2));

                g.DrawString(tbx_tst.Text, new Font("宋体", 42 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 823.6229f - (tbx_tst.Text.Length * 21f), 315.7581f - 20);

                if (cbx_dis.Checked)
                {
                    g.DrawString(tbx_tki.Text.Substring(0, 10), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 189.7577f - (240.2109f / 2), 466.2356f - (54.0234f / 2));
                    g.DrawString("****", new Font("Bahnschrift", 48 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 356.7472f - (90.48f / 2) - (Count(tbx_tki.Text.Substring(0, 10), "1") * 6f) + 5f, 466.2356f - (54.0234f / 2));
                    g.DrawString(tbx_tki.Text.Substring(14, 4), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 457.7636f - (84.0234f / 2) - 24f - (Count(tbx_tki.Text.Substring(0, 10), "1") * 6f) + 5f, 466.2356f - (54.0234f / 2));
                    g.DrawString(" " + tbx_tki.Text.Substring(18), new Font("宋体", 48 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 457.7636f + (84.0234f / 2) - 42f, 466.2356f - (54.0234f / 2));
                }
                else
                {
                    g.DrawString(tbx_tki.Text.Substring(0, 18), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 189.7577f - (240.2109f / 2), 466.2356f - (54.0234f / 2));
                    g.DrawString(" " + tbx_tki.Text.Substring(18), new Font("宋体", 42 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 189.7577f - (240.2109f / 2) + (18f * 24f) - (Count(tbx_tki.Text.Substring(0, 18), "1") * 6f) + 5f, 466.2356f - (54.0234f / 2));
                }

                g.DrawString(tbx_tkm.Text.Substring(9) + cbo_tkn.Items[cbo_tkn.SelectedIndex].ToString() + tbx_tkn.Text.Substring(3), new Font("Bernard MT Condensed", 36 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 261.8358f - (361.2305f / 2), 616.718f - (44.3359f / 2));

                g.DrawString(tbx_tel.Text.Replace('：', ':'), new Font("宋体", 40 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    804.5585f + (352f / 2) - (tbx_tel.Text.Length * 40f), 112.6888f - 30f);

                //491.7729

                if (tbx_tmk.Text.Length == 1)
                {
                    g.DrawString(tbx_tmk.Text, new Font("华文中宋", 30 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)),
                        463.6884f - (33f / 2f), 316.7029f - (33.54f / 2f) - 3f);
                    g.DrawEllipse(new Pen(Color.FromArgb(0, 0, 0), 3), 463.2284f - ((float)Math.Sqrt(1800) / 2f) + 4f, 314.0786f - (42.4264f / 2f), (float)Math.Sqrt(1800) * 1f, (float)Math.Sqrt(1800) * 1f);
                }
                else if (tbx_tmk.Text.Length > 1)
                {
                    f -= tbx_tmk.Text.Length / 2.0f * ((float)Math.Sqrt(1800));
                    f -= (tbx_tmk.Text.Length - 1.0f) / 2.0f * 15.0f;
                    for (int i = 0; i < tbx_tmk.Text.Length; i++)
                    {
                        //463.6884(字)-463.2284(圆)
                        //
                        g.DrawEllipse(new Pen(Color.FromArgb(0, 0, 0), 3), f, 314.0786f - (42.4264f / 2f), (float)Math.Sqrt(1800) * 1f, (float)Math.Sqrt(1800) * 1f);
                        g.DrawString(tbx_tmk.Text[i].ToString(), new Font("华文中宋", 30 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)),
                        f + 463.6884f - 463.2284f, 316.7029f - (33.54f / 2f) - 3f);
                        f += 15f;
                        f += (float)Math.Sqrt(1800);
                    }
                }

                pbx_ticket.Image = bmp;
            }
            catch
            {
            }
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
                    if (item.name.Equals(tbx_before.Text.ToString()) && (!(item.name.ToLower().StartsWith("x"))) && ((!(item.name.ToLower().EndsWith("线路所"))) || (!tbx_before.Enabled)))
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
            List<string> stas = new List<string>();
            for (int i = 0; i < lis.Count; i++)
                if (!((lis[i].beforestop == 0) || lis[i].beforeteg))
                    stas.Add(Share.stations[lis[i].before].name);
            if (!((lis[lis.Count - 1].afterstop == 0) || lis[lis.Count - 1].afterteg))
                stas.Add(Share.stations[lis[lis.Count - 1].after].name);
            lbx_lis.Items.Clear();
            for (int i = 0; i < stas.Count; i++)
                for (int j = 0; j < stas.Count; j++)
                    if (!stas[i].Equals(stas[j]))
                        lbx_lis.Items.Add(stas[i] + " → " + stas[j]);
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
        public void Out(object o)
        {
            object[] os = o as object[];
            bool isshow = Convert.ToBoolean(os[0]);
            string path = os[1] as string;
            StringBuilder sb = new StringBuilder();
            JArray jArray, j2;
            String name;
            JObject tmp = new JObject();
            JObject tmp2 = new JObject();
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
            jArray = new JArray();
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
                        ReloadRail(lis[i].line);
                        jArray = Share.rails[(lis[i].line)].locations;
                        if(b1)
                        {
                            jArray = new JArray();
                            for (int t = Share.rails[(lis[i].line)].locations.Count - 1; t >= 0; t--)
                            {
                                jArray.Add((JObject)Share.rails[(lis[i].line)].locations[(t)]);
                            }
                        }
                        if (((string)((JObject)jArray[j])["name"]).Equals(Share.stations[(lis[i].before)].name))
                            b = true;
                        tmp = (JObject)jArray[j];
                        tmp2 = tmp;
                        tmp = null;
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
                                    tmp2["name"] = (((string)((JObject)jArray[j])["name"]) + " " + ToTime(uptimes[tt].arriveTime) + "到");
                                j2.Add(tmp2);
                                sb.Append(tmp2.ToString());
                            }
                            break;
                        }
                        if (b)
                        {
                            if ((!((string)((JObject)jArray[j])["name"]).Equals(Share.stations[(lis[i].before)].name))
                                    || (((string)((JObject)jArray[j])["name"]).Equals(Share.stations[(lis[i].before)].name)
                                    && (lis[i].beforestop == 0)))
                                tmp2["type"] = "waypoint";
                            else if (isshow)
                            {
                                if (i == 0)
                                    tmp2["name"] = (((string)((JObject)jArray[j])["name"]) + " " + ToTime(uptimes[tt].deparTime) + "开");
                                else
                                    tmp2["name"] = (((string)((JObject)jArray[j])["name"]) + " " + ToTime(uptimes[tt].arriveTime) + "到 " + ToTime(uptimes[tt].deparTime) + "开" + (uptimes[tt].teg ? "技停" : ""));
                                tt++;
                            }
                            sb.Append(tmp2.ToString());
                            j2.Add(tmp2);
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
                            ((JObject)j2[t])["name"] = name + " " + ToTime(downtimes[tt].deparTime) + "开";
                        }
                        else if (t == 0)
                        {
                            ((JObject)j2[t])["name"] = (name + " " + ToTime(downtimes[tt].arriveTime) + "到");
                        }
                        else
                        {
                            ((JObject)j2[t])["name"] = (name + " " + ToTime(downtimes[tt].arriveTime) + "到 " + ToTime(downtimes[tt].deparTime) + "开" + (downtimes[tt].teg ? "技停" : ""));
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
                if (cbx_isAstar.Checked)
                    cbo_after.Enabled = true;
                SetTime();
                List<string> stas = new List<string>();
                try
                {
                    for (int i = 0; i < lis.Count; i++)
                        if (!((lis[i].beforestop == 0) || lis[i].beforeteg))
                            stas.Add(Share.stations[lis[i].before].name);
                    if (!((lis[lis.Count - 1].afterstop == 0) || lis[lis.Count - 1].afterteg))
                        stas.Add(Share.stations[lis[lis.Count - 1].after].name);
                }
                catch { }
                lbx_lis.Items.Clear();
                for (int i = 0; i < stas.Count; i++)
                    for (int j = 0; j < stas.Count; j++)
                        if (!stas[i].Equals(stas[j]))
                            lbx_lis.Items.Add(stas[i] + " → " + stas[j]);
                try
                {
                    if (lbx_lis.SelectedIndex == -1)
                        lbx_lis.SelectedIndex = 0;
                }
                catch { }
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
                Ticket tk = null;
                int x = 6;
                List<string> stass = new List<string>();
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
                mtb_uptime.Text = (tmp2 + tmp1);
                strs = str[2].Split(' ');
                downhour = int.Parse(strs[0]);
                downmin = int.Parse(strs[1]);
                tmp1 = downmin < 10 ? "0" + (downmin).ToString() : (downmin).ToString();
                tmp2 = downhour < 10 ? "0" + (downhour).ToString() : (downhour).ToString();
                mtb_downtime.Text = (tmp2 + tmp1);
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
                tbx_trainNum.Text = (str[6]); EMU f = null;
                foreach (EMU emu in Share.emus)
                {
                    if (emu.name == train)
                    {
                        f = emu;
                        if (emu.trains[cbo_trainSeat.SelectedIndex].Count > 10)
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
                List<SeatType> st = new List<SeatType>();
                bool b = false;
                for (int i = 0; i < f.trains[cbo_trainSeat.SelectedIndex].Count; i++)
                {
                    for (int j = 0; j < f.trains[cbo_trainSeat.SelectedIndex][i].st.Count; j++)
                    {
                        b = false;
                        for (int k = 0; k < st.Count; k++)
                            b = b || ToString(st[k]).Equals(ToString(f.trains[cbo_trainSeat.SelectedIndex][i].st[j]));
                        if (!b)
                            st.Add(f.trains[cbo_trainSeat.SelectedIndex][i].st[j]);
                    }
                }
                st.Remove(SeatType.D);
                st.Remove(SeatType.UK);
                st.Remove(SeatType.CA);
                st.Remove(SeatType.ZEC);
                st.Remove(SeatType.ZYC);
                st.Remove(SeatType.WRC);
                for (int i = 7; i < str.Length; i++)
                {
                    if (str[i].Equals("--END--"))
                    {
                        x = ++i;
                        break;
                    }
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
                    stass = new List<string>();
                    for (int j = 0; j < lis.Count; j++)
                        if (!((lis[j].beforestop == 0) || lis[j].beforeteg))
                            stass.Add(Share.stations[lis[j].before].name);
                    if (!((lis[lis.Count - 1].afterstop == 0) || lis[lis.Count - 1].afterteg))
                        stass.Add(Share.stations[lis[lis.Count - 1].after].name);
                    lbx_lis.Items.Clear();
                    tkl.Clear();
                    for (int j = 0; j < stass.Count; j++)
                        for (int k = 0; k < stass.Count; k++)
                            if (!stass[j].Equals(stass[k]))
                            {
                                lbx_lis.Items.Add(stass[j] + " → " + stass[k]);
                                tkl.Add(stass[j] + " → " + stass[k], new Ticket(st.ToArray()));
                            }
                    if (lbx_lis.SelectedIndex == -1)
                        lbx_lis.SelectedIndex = 0;
                    //progressDialog.setProgress(//progressDialog.getProgress() + 1);
                }
                if (x != 6)
                {
                    try
                    {
                        for (int i = x; i < str.Length; i++)
                        {
                            strs = str[i].Split(' ');
                            tk = new Ticket();
                            for (int j = 2; j < strs.Length; j++)
                            {
                                tk.Add((SeatType)Enum.Parse(typeof(SeatType), strs[j].Split('-')[0]), int.Parse(strs[j].Split('-')[1]));
                            }
                            tkl[strs[0] + " → " + strs[1]] = new Ticket(tk);
                        }
                    }
                    catch { }
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
                List<string> stas = new List<string>();
                for (int i = 0; i < lis.Count; i++)
                    if (!((lis[i].beforestop == 0) || lis[i].beforeteg))
                        stas.Add(Share.stations[lis[i].before].name);
                if (!((lis[lis.Count - 1].afterstop == 0) || lis[lis.Count - 1].afterteg))
                    stas.Add(Share.stations[lis[lis.Count - 1].after].name);
                lbx_lis.Items.Clear();
                for (int i = 0; i < stas.Count; i++)
                    for (int j = 0; j < stas.Count; j++)
                        if (!stas[i].Equals(stas[j]))
                            lbx_lis.Items.Add(stas[i] + " → " + stas[j]);
                try
                {
                    open = true;
                    if (lbx_lis.SelectedIndex == -1)
                        lbx_lis.SelectedIndex = 0;
                }
                catch { }
                //progressDialog.setProgress(MAX_PROGRESS);
                //progressDialog.cancel();
                try
                {
                    tk = tkl[lbx_lis.Items[0].ToString()];
                    for (int i = 0; i < tk.k.Count; i++)
                        dgv_tickets.Rows[i].Cells[1].Value = tk[tk.k[i]];
                }
                catch {  }
                open = false;
                MessageBox.Show("打开成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e2)
            {
                MessageBox.Show("打开失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ReloadRail(int i)
        {
            JObject railstr = JObject.Parse(DataFiles.railDatas[i]);
            Share.rails[i].locations = (JArray)((JObject)railstr["route"])[("up")];
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
            Ticket tk = null;
            sb2.Append("\n--END--");
            for (int i = 0; i < lbx_lis.Items.Count; i++)
            {
                sb2.Append("\n");
                try
                {
                    tk = tkl[lbx_lis.Items[i].ToString()];
                }
                catch
                {
                    EMU f = null;
                    foreach (EMU emu in Share.emus)
                    {
                        if (emu.name == train)
                        {
                            f = emu;
                            if (emu.trains[cbo_trainSeat.SelectedIndex].Count > 10)
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
                    List<SeatType> st = new List<SeatType>();
                    SeatType s;
                    bool b = false;
                    for (int j = 0; j < f.trains[cbo_trainSeat.SelectedIndex].Count; j++)
                    {
                        for (int k = 0; k < f.trains[cbo_trainSeat.SelectedIndex][j].st.Count; k++)
                        {
                            b = false;
                            for (int l = 0; l < st.Count; l++)
                                b = b || ToString(st[l]).Equals(ToString(f.trains[cbo_trainSeat.SelectedIndex][j].st[k]));
                            if (!b)
                                st.Add(f.trains[cbo_trainSeat.SelectedIndex][j].st[k]);
                        }
                    }
                    st.Remove(SeatType.D);
                    st.Remove(SeatType.UK);
                    st.Remove(SeatType.CA);
                    st.Remove(SeatType.ZEC);
                    st.Remove(SeatType.ZYC);
                    st.Remove(SeatType.WRC);
                    tkl.Add(lbx_lis.Items[i].ToString(), new Ticket(st.ToArray()));
                    tk = tkl[lbx_lis.Items[i].ToString()];
                }
                sb2.Append(lbx_lis.Items[i].ToString().Split(' ')[0]);
                sb2.Append(" ");
                sb2.Append(lbx_lis.Items[i].ToString().Split(' ')[2]);
                sb2.Append(" ");
                for (int j = 0; j < tk.k.Count; j++)
                {
                    sb2.Append(tk.k[j].ToString());
                    sb2.Append("-");
                    sb2.Append(tk[tk.k[j]].ToString());
                    if (j < (tk.k.Count - 1))
                        sb2.Append(" ");
                }
            }
            try
            {
                File.WriteAllText(path, sb2.ToString());
                MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e2)
            {
                //progressDialog2.cancel();
                MessageBox.Show("保存失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            EMU f = null;
            foreach (EMU emu in Share.emus)
            {
                if (emu.name == train)
                {
                    f = emu;
                    if (emu.trains[cbo_trainSeat.SelectedIndex].Count > 10)
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
            List<SeatType> st = new List<SeatType>();
            SeatType s;
            bool b = false;
            for (int i = 0; i < f.trains[cbo_trainSeat.SelectedIndex].Count; i++)
            {
                for (int j = 0; j < f.trains[cbo_trainSeat.SelectedIndex][i].st.Count; j++)
                {
                    b = false;
                    for (int k = 0; k < st.Count; k++)
                        b = b || ToString(st[k]).Equals(ToString(f.trains[cbo_trainSeat.SelectedIndex][i].st[j]));
                    if (!b)
                        st.Add(f.trains[cbo_trainSeat.SelectedIndex][i].st[j]);
                }
            }
            st.Remove(SeatType.D);
            st.Remove(SeatType.UK);
            st.Remove(SeatType.CA);
            st.Remove(SeatType.ZEC);
            st.Remove(SeatType.ZYC);
            st.Remove(SeatType.WRC);
            tkl = new Dictionary<string, Ticket>();
            foreach (object item in lbx_lis.Items)
            {
                tkl.Add(item.ToString(), new Ticket(st.ToArray()));
            }
            dgv_tickets.Rows.Clear();
            for (int i = 0; i < st.Count; i++)
            {
                dgv_tickets.Rows.Add(ToString(st[i]), 0);
            }
            try
            {
                lbx_lis.SelectedIndex = 0;
                lbx_lis.SelectedIndex = lbx_lis.SelectedIndex;
            }
            catch { }
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
            new Thread(Out) { IsBackground = true }
            .Start(new object[] { MessageBox.Show("在地图中是否显示到发时间？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes ? true : false, fbd.SelectedPath });
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
            //try
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
                    if (26 >= stats.Count) //一次性
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
                    bmp.Save(path + "\\" + tn.Replace('/', '_') + "lcd" + lcdNum + ".png");
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
                    if (26 >= stats.Count)
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
                    bmp.Save(path + "\\" + tn.Replace('/', '_') + "lcd" + lcdNum + ".png");
                    lcdNum++;
                    //if (i == 10)
                    //    break;
                    i++;
                }
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //catch
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
                    (tbx_trainNum).Text = ("D303 D304");
            }
            catch { }
        }
        public string Download(string ftpfilepath)
        {
            ftpfilepath = ftpfilepath.Replace("\\", "/");
            string url = FTPCONSTR + ftpfilepath;
            FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            reqFtp.UseBinary = true;
            reqFtp.Credentials = new NetworkCredential(FTPUSERNAME, FTPPASSWORD);
            FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
            Stream ftpStream = response.GetResponseStream();
            long cl = response.ContentLength;
            int bufferSize = 1024 * 1024 * 16;
            int readCount;
            byte[] buffer = new byte[bufferSize];
            StringBuilder sb = new StringBuilder();
            readCount = ftpStream.Read(buffer, 0, bufferSize);
            while (readCount > 0)
            {
                if ((buffer[0] == 0xEF) && (buffer[1] == 0xBB) && (buffer[2] == 0xBF))
                    sb.Append(Encoding.UTF8.GetString(buffer, 3, readCount - 3));
                else
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, readCount));
                readCount = ftpStream.Read(buffer, 0, bufferSize);
            }
            ftpStream.Close();
            response.Close();
            return sb.ToString();
        }
        public String OldCalcTime(int upmin, int uphour, bool up)
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
        public String CalcTime(int upmin, int uphour, bool up)
        {
            if (this.lis.Count == 0)
                return "";
            StringBuilder sb = new StringBuilder();
            List<ListItem> lis = new List<ListItem>();
            String tmp1, tmp3;
            int minute = 0, hourOfDay = 0, m2;
            int min = upmin + (60 * uphour);
            TimeTime tttmp = new TimeTime(0, 0, false);
            if (up)
            {
                lis = this.lis;
                uptimes.Clear();
            }
            else
            {
                ListItem tmp4;
                for (int i = this.lis.Count - 1; i >= 0; i--)
                {
                    tmp4 = this.lis[i];
                    lis.Add(new ListItem(tmp4.after, tmp4.afterstop,
                            tmp4.line, tmp4.mode,
                            tmp4.before, tmp4.beforestop, tmp4.earlytime,
                            tmp4.afterteg, tmp4.beforeteg));
                }
                downtimes.Clear();
            }
            int max_length = Share.stations[lis[0].before].name.Length;
            for (int i = 0; i < lis.Count; i++)
            {
                if ((!Share.stations[lis[i].before].name.EndsWith("线路所")) && (lis[i].beforestop != 0) && (!lis[i].beforeteg))
                {
                    if (max_length < Share.stations[lis[i].before].name.Length)
                        max_length = Share.stations[lis[i].before].name.Length;
                }
            }
            if ((!Share.stations[lis[lis.Count - 1].after].name.EndsWith("线路所"))
                    && (lis[lis.Count - 1].afterstop != 0) && (!lis[lis.Count - 1].afterteg))
            {
                if (max_length < Share.stations[lis[lis.Count - 1].after].name.Length)
                    max_length = Share.stations[lis[lis.Count - 1].after].name.Length;
            }
            if (lis[0].beforestop != 0)
            {
                if (Share.stations[lis[0].before].name.Length < max_length)
                {
                    sb.Append("_");
                    for (int i = 1; i < getLen(max_length, Share.stations[lis[0].before].name.Length)[0]; i++)
                        sb.Append(" ");
                    sb.Append(Share.stations[lis[0].before].name);
                    for (int i = 0; i <= getLen(max_length, Share.stations[lis[0].before].name.Length)[1]; i++)
                        sb.Append(" ");
                }
                else
                    sb.Append(Share.stations[lis[0].before].name);
                sb.Append(" --:--  ");
                minute = upmin;
                hourOfDay = uphour % 24;
                tmp1 = minute < 10 ? "0" + Convert.ToString(minute) : Convert.ToString(minute);
                tmp3 = hourOfDay < 10 ? "0" + Convert.ToString(hourOfDay) : Convert.ToString(hourOfDay);
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
            for (int i = 0; i < lis.Count; i++)
            {
                min += CalcLit(lis[i].before, lis[i].line, lis[i].after, lis[i].mode) + lis[i].earlytime;
                if (lis[i].afterstop > 0)
                {
                    if (Share.stations[lis[i].after].name.Length < max_length)
                    {
                        for (int v = 0; v < getLen(max_length, Share.stations[lis[i].after].name.Length)[0]; v++)
                            sb.Append(" ");
                        sb.Append(Share.stations[lis[i].after].name);
                        for (int v = 0; v <= getLen(max_length, Share.stations[lis[i].after].name.Length)[1]; v++)
                            sb.Append(" ");
                    }
                    else
                        sb.Append(Share.stations[lis[i].after].name);
                    sb.Append(" ");
                    tttmp = new TimeTime(0, 0, false);
                    m2 = min;
                    sb.Append(ToTime(min));
                    tttmp.arriveTime = m2;
                    if (i == (lis.Count - 1))
                    {
                        sb.Append("  --:--   --");
                        tttmp.teg = false;
                        tttmp.deparTime = -1;
                        if (up)
                            uptimes.Add(tttmp);
                        else
                            downtimes.Add(tttmp);
                        return sb.ToString();
                    }
                    min += lis[i].afterstop;
                    sb.Append(" ");
                    sb.Append(ToTime(min));
                    m2 = min;
                    tttmp.deparTime = m2;
                    sb.Append(" ");
                    if (lis[i].afterteg)
                    {
                        tttmp.teg = true;
                        sb.Append("技停\r\n");
                    }
                    else
                    {
                        sb.Append(lis[i].afterstop);
                        sb.Append("分\r\n");
                    }
                    if (up)
                        uptimes.Add(tttmp);
                    else
                        downtimes.Add(tttmp);
                }
            }
            return sb.ToString();
        }
        public int CalcLit(int before, int line, int after, RunMode rm)
        {
            int r = 0, spd = 0, bspd = 0, aspd = 0, j = 0, ms = 2147483647, k = 0;
            double tmp = 0.0, len = 0.0;
            double ast = 0.0;
            bool b = false;
            String nowStn = "", bStn = "";
            String[] limitSpeeds = Share.rails[line].speeds.Replace("\r", "").Split('\n');
            foreach (EMU e in Share.emus)
            {
                if (e.name.Equals(train))
                {
                    ast = e.ast;
                    ms = e.speed;
                    break;
                }
            }
            if (cbx_no350mode.Checked)
                ms = 310;
            for (int i = 0; i < Share.rails[line].stations.Count; i++)
            {
                //读取 j
                if (limitSpeeds[j].Equals(Share.rails[line].stations[i]))
                {
                    nowStn = limitSpeeds[j];
                    j++;
                }
                else if (limitSpeeds[j].StartsWith("atp"))
                {
                    if (rm == RunMode.ATP)
                        spd = int.Parse(limitSpeeds[j].Split(' ')[1]);
                    j++;
                    i--;
                    continue;
                }
                else if (limitSpeeds[j].StartsWith("ms"))
                {
                    if (rm != RunMode.Express)
                        spd = int.Parse(limitSpeeds[j].Split(' ')[1]);
                    j++;
                    i--;
                    continue;
                }
                else if (limitSpeeds[j].StartsWith("ss"))
                {
                    if (rm == RunMode.Express)
                        spd = int.Parse(limitSpeeds[j].Split(' ')[1]);
                    j++;
                    i--;
                    continue;
                }
                else if (limitSpeeds[j].StartsWith("1") || limitSpeeds[j].StartsWith("2") || limitSpeeds[j].StartsWith("3") || limitSpeeds[j].StartsWith("4") || limitSpeeds[j].StartsWith("5") || limitSpeeds[j].StartsWith("6") || limitSpeeds[j].StartsWith("7") || limitSpeeds[j].StartsWith("8") || limitSpeeds[j].StartsWith("9"))
                {
                    spd = int.Parse(limitSpeeds[j]);
                    j++;
                    i--;
                    continue;
                }
                if (spd > ms)
                {
                    if (ms == 310)
                        ms += 5;
                    switch (rm)
                    {
                        case RunMode.ATP:
                            spd = ms - 6;
                            break;
                        case RunMode.Express:
                            spd = ms - 15;
                            break;
                        case RunMode.MaxSpeed:
                            spd = ms - 10;
                            break;
                    }
                }
                if (Share.rails[line].stations[i].Equals(Share.stations[after].name))
                {
                    if (b == false)
                        return CalcLit(after, line, before, rm);
                    else
                    {
                        r += Calc(len, ast, bspd, 0, spd, 0);
                        return r;
                    }
                }
                if (Share.rails[line].stations[i].Equals(Share.stations[before].name))
                {
                    b = true;
                    bStn = nowStn;
                }
                if (!bStn.Equals(nowStn))
                {
                    k = 0;
                    if (limitSpeeds[j].StartsWith("atp"))
                    {
                        if (rm == RunMode.ATP)
                            aspd = int.Parse(limitSpeeds[j].Split(' ')[1]);
                        j++;
                        k++;
                    }
                    if (limitSpeeds[j].StartsWith("ms"))
                    {
                        if (rm != RunMode.Express)
                            aspd = int.Parse(limitSpeeds[j].Split(' ')[1]);
                        j++;
                        k++;
                    }
                    if (limitSpeeds[j].StartsWith("ss"))
                    {
                        if (rm == RunMode.Express)
                            aspd = int.Parse(limitSpeeds[j].Split(' ')[1]);
                        j++;
                        k++;
                    }
                    if (limitSpeeds[j].StartsWith("1") || limitSpeeds[j].StartsWith("2") || limitSpeeds[j].StartsWith("3") || limitSpeeds[j].StartsWith("4") || limitSpeeds[j].StartsWith("5") || limitSpeeds[j].StartsWith("6") || limitSpeeds[j].StartsWith("7") || limitSpeeds[j].StartsWith("8") || limitSpeeds[j].StartsWith("9"))
                    {
                        aspd = int.Parse(limitSpeeds[j]);
                        j++;
                        k++;
                    }
                    j -= k;
                    r += Calc(len, ast, bspd, aspd, spd, 0);
                    len = 0.0;
                    bStn = nowStn;
                }
                bspd = spd;
                if (b)
                {
                    len += Share.rails[line].lengths[i];
                }
            }
            return r;
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
        [STAThread]
        public void New()
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
        private void lbx_lis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbx_lis.SelectedIndex == -1)
                return;
            if (open)
                return;
            Ticket tk = null;
            if (de1 != null)
            {
                tk = tkl[de1];
                int b = 0, c = 0;
                EMU f = null;
                foreach (EMU emu in Share.emus)
                {
                    if (emu.name == train)
                    {
                        f = emu;
                        if (emu.trains[cbo_trainSeat.SelectedIndex].Count > 10)
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
                for (int i = 0; i < tk.k.Count; i++)
                {
                    c = 0;
                    for (int j = 0; j < f.trains[cbo_trainSeat.SelectedIndex].Count; j++)
                    {
                        for (int k = 0; k < f.trains[cbo_trainSeat.SelectedIndex][j].st.Count; k++)
                        {
                            try
                            {
                                if (ToString(tk.k[i]).Equals(ToString(f.trains[cbo_trainSeat.SelectedIndex][j].st[k])))
                                    c += f.trains[cbo_trainSeat.SelectedIndex][j].dic[tk.k[i]];
                            }
                            catch
                            {
                                if (ToString(tk.k[i]).Equals(ToString(f.trains[cbo_trainSeat.SelectedIndex][j].st[k])))
                                    c += f.trains[cbo_trainSeat.SelectedIndex][j].dic[(SeatType)Enum.Parse(typeof(SeatType), tk.k[i].ToString() + "C")];
                            }
                        }
                    }
                    try
                    {
                        b = int.Parse(dgv_tickets.Rows[i].Cells[1].Value.ToString());
                    }
                    catch
                    {
                        b = c;
                    }
                    if (b > c)
                        b = c;
                    tkl[de1][tk.k[i]] = b;
                }
            }
            try
            {
                tk = tkl[lbx_lis.Items[lbx_lis.SelectedIndex].ToString()];
            }
            catch
            {
                EMU f = null;
                foreach (EMU emu in Share.emus)
                {
                    if (emu.name == train)
                    {
                        f = emu;
                        if (emu.trains[cbo_trainSeat.SelectedIndex].Count > 10)
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
                List<SeatType> st = new List<SeatType>();
                SeatType s;
                bool b = false;
                for (int i = 0; i < f.trains[cbo_trainSeat.SelectedIndex].Count; i++)
                {
                    for (int j = 0; j < f.trains[cbo_trainSeat.SelectedIndex][i].st.Count; j++)
                    {
                        b = false;
                        for (int k = 0; k < st.Count; k++)
                            b = b || ToString(st[k]).Equals(ToString(f.trains[cbo_trainSeat.SelectedIndex][i].st[j]));
                        if (!b)
                            st.Add(f.trains[cbo_trainSeat.SelectedIndex][i].st[j]);
                    }
                }
                st.Remove(SeatType.D);
                st.Remove(SeatType.UK);
                st.Remove(SeatType.CA);
                st.Remove(SeatType.ZEC);
                st.Remove(SeatType.ZYC);
                st.Remove(SeatType.WRC);
                tkl.Add(lbx_lis.Items[lbx_lis.SelectedIndex].ToString(), new Ticket(st.ToArray()));
                tk = tkl[lbx_lis.Items[lbx_lis.SelectedIndex].ToString()];
            }
            for (int i = 0; i < tk.k.Count; i++)
                dgv_tickets.Rows[i].Cells[1].Value = tk[tk.k[i]];
            de1 = lbx_lis.Items[lbx_lis.SelectedIndex].ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (de1 == null) return;
            Ticket tk = tkl[de1];
            int b = 0, c = 0;
            EMU f = null;
            foreach (EMU emu in Share.emus)
            {
                if (emu.name == train)
                {
                    f = emu;
                    if (emu.trains[cbo_trainSeat.SelectedIndex].Count > 10)
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
            for (int i = 0; i < tk.k.Count; i++)
            {
                c = 0;
                for (int j = 0; j < f.trains[cbo_trainSeat.SelectedIndex].Count; j++)
                {
                    for (int k = 0; k < f.trains[cbo_trainSeat.SelectedIndex][j].st.Count; k++)
                    {
                        try
                        {
                            if (ToString(tk.k[i]).Equals(ToString(f.trains[cbo_trainSeat.SelectedIndex][j].st[k])))
                                c += f.trains[cbo_trainSeat.SelectedIndex][j].dic[tk.k[i]];
                        }
                        catch
                        {
                            if (ToString(tk.k[i]).Equals(ToString(f.trains[cbo_trainSeat.SelectedIndex][j].st[k])))
                                c += f.trains[cbo_trainSeat.SelectedIndex][j].dic[(SeatType)Enum.Parse(typeof(SeatType), tk.k[i].ToString() + "C")];
                        }
                    }
                }
                try
                {
                    b = int.Parse(dgv_tickets.Rows[i].Cells[1].Value.ToString());
                }
                catch
                {
                    b = c;
                }
                if (b > c)
                    b = c;
                tkl[de1][tk.k[i]] = b;
            }
            for (int i = 0; i < tk.k.Count; i++)
                dgv_tickets.Rows[i].Cells[1].Value = tk[tk.k[i]];
        }
        private void btn_findrab_Click(object sender, EventArgs e)
        {
            Process.Start("https://space.bilibili.com/621814881");
        }
        private void btn_fuckntf_Click(object sender, EventArgs e)
        {
            if (lbx_ntfs.SelectedIndex != -1)
            {
                if (lbx_ntfs.Items[0].ToString() == "反火车迷攻击数据网站导致数据网站崩溃力~")
                    return;
                Process.Start("https://space.bilibili.com/" + lbx_ntfs.Items[lbx_ntfs.SelectedIndex].ToString().Substring(5));
            }
        }
        private void btn_refresh_Click(object sender, EventArgs e)
        {
            string html;
            string[] ntfs;
            lbx_ntfs.Items.Clear();
            try
            {
                html = Gethtml("https://raildatas.github.io/2^9+1.html", 0).Replace("\r", "");
                ntfs = html.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ntfs.Length; i++)
                {
                    lbx_ntfs.Items.Add("UID: " + ntfs[i]);
                }
            }
            catch
            {
                lbx_ntfs.Items.Clear();
                lbx_ntfs.Items.Add("反火车迷攻击数据网站导致数据网站崩溃力~");
            }
        }
        private int Count(String str, String find)
        {
            int j = 0;
            while (str.Contains(find))
            {
                str = str.Remove(str.IndexOf(find), find.Length);
                j++;
            }
            return j;
        }
        private void tabPage4_Click(object sender, EventArgs e)
        {
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmp = new Bitmap(global::ELE.Properties.Resources.红前_01);
                Graphics g = Graphics.FromImage(bmp);
                String astn = tbx_tka.Text.Split('|')[0];
                int al = astn.Length;
                String bstn = tbx_tkb.Text.Split('|')[0];
                int bl = bstn.Length;
                float f = 491.7729f;
                if (astn.Length == 2)
                {
                    astn = astn.Insert(1, "    ");
                    al = 3;
                }
                if (bstn.Length == 2)
                {
                    bstn = bstn.Insert(1, "    ");
                    bl = 3;
                }
                //御坂|Misaka Mikoto
                //贴白|Tiebai
                //韧穿普|Wren Trumpull
                //咸淑娜|Xian Shuna
                //香港西九龙|Hongkongwestkowloon
                //忽任|Head of the Strategic Misinfomation Office (h)
                //北京城市副中心|Beijingchengshifuzhongxin
                //卡特洛斯|Kateluos
                g.DrawString(cbo_tkn.Items[cbo_tkn.SelectedIndex].ToString() + tbx_tkn.Text.Substring(3), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), 
                    new SolidBrush(Color.FromArgb(0xf9, 0x2f, 0x10)), 205.5277f - (209.4209f / 2), 112.6888f - (59.4258f / 2));
                g.DrawString(cbo_tkt.Items[cbo_tkt.SelectedIndex].ToString() + tbx_tkt.Text.Substring(3), new Font("宋体", 55 * 0.75f, FontStyle.Regular), 
                    new SolidBrush(Color.FromArgb(0, 0, 0)), 529.536f - (151.25f / 2 * 0) - ((cbo_tkt.Items[cbo_tkt.SelectedIndex].ToString() + tbx_tkt.Text.Substring(3)).Length / 4.0f * 55f*0.75f), 171.1038f - 30.25f);
                g.DrawString(astn, new Font("微软雅黑", 55 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - (al * 55f) - 10f, 168.2542f - (60.5f / 2) - 20);
                if (tbx_tka.Text.Contains("|"))
                {
                    if ((tbx_tka.Text.Split('|')[1].Length * 18) >= (al * 55)) //英语比中文长
                        g.DrawString(tbx_tka.Text.Split('|')[1], new Font("宋体", 36 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - (al * 55f) - 10f, 215.1326f - (39.6006f / 2) - 10f);
                    else //英语比中文短
                        g.DrawString(tbx_tka.Text.Split('|')[1], new Font("宋体", 36 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - 10f - (tbx_tka.Text.Split('|')[1].Length * 18f), 215.1326f - (39.6006f / 2) - 10f);
                }
                g.DrawString("站", new Font("宋体", 42 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f), 167.9109f - (46.2002f / 2));

                g.DrawString(bstn, new Font("微软雅黑", 55 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 345.7445f - (46.2002f / 2) - (bl*55f), 168.2542f - (60.5f / 2) - 20);
                if (tbx_tka.Text.Contains("|"))
                {
                    g.DrawString(tbx_tkb.Text.Split('|')[1], new Font("宋体", 36 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 345.7445f - (46.2002f / 2) - (tbx_tkb.Text.Split('|')[1].Length * 18f), 215.1326f - (39.6006f / 2) - 10f);
                }
                g.DrawString("站", new Font("宋体", 42 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 345.7445f - (46.2002f / 2), 167.9109f - (46.2002f / 2));

                g.DrawString(tbx_tm.Text.Substring(3, 4), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 
                    126.1459f - (111.6074f / 2) - 10f + (Count(tbx_tm.Text.Substring(3, 4), "1") * 10f), 265.1282f - (59.4258f / 2) + 5f);
                g.DrawString(tbx_tm.Text.Substring(8, 2), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    244.4638f - (56.2285f / 2) - 10f, 265.1282f - (59.4258f / 2) + 5f);
                g.DrawString(tbx_tm.Text.Substring(11, 2), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    338.2602f - (44.1377f / 2) - 15f, 265.1282f - (59.4258f / 2) + 5f);
                g.DrawString(tbx_tm.Text.Substring(14, 2), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    434.6693f - (44.6533f / 2) - 13f + (Count(tbx_tm.Text.Substring(14, 2), "1") * 8f), 265.1282f - (59.4258f / 2) + 5f);
                g.DrawString(tbx_tm.Text.Substring(17, 2), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    500.1137f - (55.1973f / 2) - 10f, 265.1282f - (59.4258f / 2) + 5f);

                g.DrawString(tbx_tkc.Text, new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    675.7763f - (56.7959f / 2) - 10f + (Count(tbx_tkc.Text, "1") * 10f), 265.1282f - (59.4258f / 2) + 5f);
                g.DrawString(tbx_tks.Text, new Font("宋体", 48 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    781.6835f - (79.2002f / 2) - 15f, 265.1282f - (59.4258f / 2));

                g.DrawString(tbx_tkp.Text.Split('|')[0], new Font("Noto Serif SC", 36 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    96.3871f - (30.334f / 2), 319.9993f - (69.4805f / 2));
                g.DrawString(tbx_tkp.Text.Split('|')[1], new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    165.8598f - (95.7256f / 2) + (tbx_tkp.Text.Split('|')[0].Length * 18f) - 15f, 316.7962f - (54.0234f / 2));
                g.DrawString(tbx_tkp.Text.Split('|')[2], new Font("宋体", 24 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    165.8598f - (95.7256f / 2) + (tbx_tkp.Text.Split('|')[0].Length * 18f) - 15f + ((tbx_tkp.Text.Split('|')[1].Length) * 24f), 314.8333f - (26.2002f / 2));

                g.DrawString(tbx_tst.Text, new Font("宋体", 42 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 823.6229f - (tbx_tst.Text.Length * 21f), 315.7581f - 20);

                if (cbx_dis.Checked) {
                    g.DrawString(tbx_tki.Text.Substring(0,10), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 189.7577f - (240.2109f / 2), 466.2356f - (54.0234f / 2));
                    g.DrawString("****", new Font("Bahnschrift", 48 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 356.7472f - (90.48f / 2) - (Count(tbx_tki.Text.Substring(0, 10), "1") * 6f) + 5f, 466.2356f - (54.0234f / 2));
                    g.DrawString(tbx_tki.Text.Substring(14, 4), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 457.7636f - (84.0234f / 2) - 24f - (Count(tbx_tki.Text.Substring(0, 10), "1") * 6f) + 5f, 466.2356f - (54.0234f / 2));
                    g.DrawString(" " + tbx_tki.Text.Substring(18), new Font("宋体", 48 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 457.7636f + (84.0234f / 2) - 42f, 466.2356f - (54.0234f / 2));
                }
                else
                {
                    g.DrawString(tbx_tki.Text.Substring(0, 18), new Font("Bahnschrift", 48 * 0.75f, FontStyle.Regular), new SolidBrush(Color.FromArgb(0, 0, 0)), 189.7577f - (240.2109f / 2), 466.2356f - (54.0234f / 2));
                    g.DrawString(" " + tbx_tki.Text.Substring(18), new Font("宋体", 42 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 189.7577f - (240.2109f / 2) + (18f * 24f) - (Count(tbx_tki.Text.Substring(0, 18), "1") * 6f) + 5f, 466.2356f - (54.0234f / 2));
                }

                g.DrawString(tbx_tkm.Text.Substring(9) + cbo_tkn.Items[cbo_tkn.SelectedIndex].ToString() + tbx_tkn.Text.Substring(3), new Font("Bernard MT Condensed", 36 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)), 261.8358f - (361.2305f / 2), 616.718f - (44.3359f / 2));

                g.DrawString(tbx_tel.Text.Replace('：', ':'), new Font("宋体", 40 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)),
                    804.5585f + (352f / 2) - (tbx_tel.Text.Length * 40f), 112.6888f - 30f);

                //491.7729

                if(tbx_tmk.Text.Length == 1)
                {
                    g.DrawString(tbx_tmk.Text, new Font("华文中宋", 30 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)),
                        463.6884f - (33f / 2f), 316.7029f - (33.54f / 2f) - 3f);
                    g.DrawEllipse(new Pen(Color.FromArgb(0, 0, 0), 3), 463.2284f - ((float)Math.Sqrt(1800) / 2f) + 4f, 314.0786f - (42.4264f / 2f), (float)Math.Sqrt(1800) * 1f, (float)Math.Sqrt(1800) * 1f);
                }
                else if (tbx_tmk.Text.Length > 1)
                {
                    f -= tbx_tmk.Text.Length / 2.0f * ((float)Math.Sqrt(1800));
                    f -= (tbx_tmk.Text.Length - 1.0f) / 2.0f * 15.0f;
                    for(int i = 0; i < tbx_tmk.Text.Length; i++)
                    {
                        //463.6884(字)-463.2284(圆)
                        //
                        g.DrawEllipse(new Pen(Color.FromArgb(0, 0, 0), 3), f, 314.0786f - (42.4264f / 2f), (float)Math.Sqrt(1800) * 1f, (float)Math.Sqrt(1800) * 1f);
                        g.DrawString(tbx_tmk.Text[i].ToString(), new Font("华文中宋", 30 * 0.75f, FontStyle.Bold), new SolidBrush(Color.FromArgb(0, 0, 0)),
                        f + 463.6884f - 463.2284f, 316.7029f - (33.54f / 2f) - 3f);
                        f += 15f;
                        f += (float)Math.Sqrt(1800);
                    }
                }

                pbx_ticket.Image = bmp;
            }
            catch
            {
                MessageBox.Show("报销凭证生成失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == 2)
                MessageBox.Show("该功能仅供娱乐，禁止用于其它用途", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PNG文件(*.png)|*.png|位图文件(*.bmp)|*.bmp";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    new Bitmap(pbx_ticket.Image).Save(sfd.FileName);
                    MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show("保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            try
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
            catch
            {
                return new List<ListItem>();
            }
        }
    }
}