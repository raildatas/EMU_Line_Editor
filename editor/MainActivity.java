package com.example.editor;

import static java.lang.Math.ceil;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import android.Manifest;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.TimePickerDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.res.ColorStateList;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.StrictMode;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.EditText;
import android.widget.RadioButton;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.TimePicker;
import com.example.myapplication.R;
import org.json.JSONArray;
import org.json.JSONObject;
import java.io.IOException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicBoolean;
import okhttp3.*;

public class MainActivity extends AppCompatActivity implements View.OnFocusChangeListener {
    boolean select = false;
    boolean flag = true;
    int upmin = 6, uphour = 10, downmin = 6, downhour = 10;
    int selectB = -1, selectL = -1, selectA = -1, selectC = -1;
    List<String> emus = new List<>();
    Dictionary<String, String> dic = new Dictionary<>();
    Dictionary<String, String> dic2 = new Dictionary<>();
    List<String> dk1 = new List<>();
    List<String> dk2 = new List<>();
    List<TimeTime> uptimes = new List<>();
    List<TimeTime> downtimes = new List<>();
    List<List<List<Integer>>> sss = new List<>();//small stations
    List<Integer> ssns = new List<>();//small station names
    String train = "";
    String ver = "";
    List<ListItem> lis = new List<>();
    String upTime = "", downTime = "";
    String[] rcps = new String[0];
    String appdir;
    List<List<DFSItem>> dfsis = new List<List<DFSItem>>();
    List<DFSItem> it;
    double lat, lng = lat = -1;
    int num = 0;
    List<int[]> visit = new List<int[]>();
    private static double EARTH_RADIUS = 6378137;

    private static double rad(double d) {
        return d * Math.PI / 180.0;
    }
    public static double GetLength(double lng1,double lat1,double lng2, double lat2) {//获取长度
        double radLat1 = rad(lat1);
        double radLat2 = rad(lat2);
        double a = radLat1 - radLat2;
        double b = rad(lng1) - rad(lng2);
        double s = 2 * Math.asin(Math.sqrt(Math.pow(Math.sin(a / 2), 2) + Math.cos(radLat1) * Math.cos(radLat2) * Math.pow(Math.sin(b / 2), 2)));
        s = s * EARTH_RADIUS;
        s = Math.round(s * 10000) / 10000;
        return s / 1000.0;
    }
    public String Gethtml(String url, int delay) throws IOException {//爬虫
        OkHttpClient.Builder builder = new OkHttpClient.Builder();
        if(delay > 0)
            builder = builder.connectTimeout(delay, TimeUnit.SECONDS);
        return builder.build().newCall(new Request.Builder().url(url).get().build()).execute().body().string();
    }
    SeatType GetSeatType(String str) {//String转SeatType
        switch (str.toUpperCase()) {
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
    public String replace(String sourceString, char chElemData) {//删除字符
        String tmpString = "";
        tmpString += chElemData;
        StringBuffer StringBuffer = new StringBuffer(sourceString);
        int iFlag = -1;
        do {
            iFlag = StringBuffer.indexOf(tmpString);
            if (iFlag != -1) {
                StringBuffer.deleteCharAt(iFlag);
            }
        } while (iFlag != -1);
        return StringBuffer.toString();
    }
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        if (android.os.Build.VERSION.SDK_INT > 9) {
            StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
            StrictMode.setThreadPolicy(policy);
        }
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        if(!Environment.isExternalStorageManager()) {
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.READ_EXTERNAL_STORAGE,Manifest.permission.WRITE_EXTERNAL_STORAGE}, 4);
            //ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.MANAGE_DOCUMENTS}, 4);
            //ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.REQUEST_COMPANION_RUN_IN_BACKGROUND}, 4);
        }
        ((EditText)(findViewById(R.id.et_trainNumber))).setOnFocusChangeListener((v, hasFocus) -> {
            if(!hasFocus) {
                ((EditText)(findViewById(R.id.et_trainNumber))).setText(((EditText)(findViewById(R.id.et_trainNumber))).getText().toString().toUpperCase());
                String tmp = ((EditText)(findViewById(R.id.et_trainNumber))).getText().toString();
                boolean b = true;
                b = b && tmp.contains(" ");
                b = b && (tmp.startsWith("G")||tmp.startsWith("D")||tmp.startsWith("C")||tmp.startsWith("S"));
                b = b && (tmp.length()<22);
                int min = 0;
                if (tmp.startsWith("D"))
                    min = 300;
                else if (tmp.startsWith("C"))
                    min = 1000;
                else if (tmp.startsWith("S"))
                    min = 100;
                if(tmp.contains(" ")) {
                    b = b && (tmp.split(" ")[0].charAt(0) == tmp.split(" ")[1].charAt(0));
                    if(tmp.contains("/")) {
                        b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1).split("/")[0]) < 9999);
                        b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1).split("/")[0]) > min);
                        b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1).split("/")[0]) < 9999);
                        b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1).split("/")[0]) > min);
                        b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1).split("/")[1]) < 9999);
                        b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1).split("/")[1]) > min);
                        b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1).split("/")[1]) < 9999);
                        b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1).split("/")[1]) > min);
                    }
                    else {
                        b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1)) < 9999);
                        b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1)) > min);
                        b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1)) < 9999);
                        b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1)) > min);
                    }
                }
                b = b && (tmp.indexOf(" ") == tmp.lastIndexOf(" "));
                b = b && (tmp.replace("G","").replace("D","")
                        .replace("C","").replace("S","")
                        .replace("9","").replace("0","")
                        .replace("8","").replace("1","")
                        .replace("7","").replace("2","")
                        .replace("6","").replace("3","")
                        .replace("5","").replace("4","")
                        .replace("/","").replace(" ","").length() == 0);
                if(b == false)
                    ((EditText)(findViewById(R.id.et_trainNumber))).setText("D301 D302".toCharArray(),0,2);
            }
        });
            if(!flag)
                return;
            select = false;
            Share.emus = new List<EMU>();
            Share.rails = new List<Rail>();
            Share.lists = new Dictionary<Character, List<Integer>>();
            List<Byte> persons = new List<Byte>();
            List<SeatType> seatTypes = new List<SeatType>();
            List<List<Train>> trains = new List<List<Train>>();
            List<String> ver = new List<String>();
            String[] emustrs, trainstrs, seatstrs, temp, temp2, tmp3, evens = null, ssstrs = null, spdstrs;
            int tmp4;
            emustrs = new String[1];
            JSONObject railstr, tmp;
            Station station;
            Boolean b;
            JSONObject item;
            String[] replaceDatas = null;
            String html;
            double length;
            appdir = getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS) + Character.toString(java.io.File.separatorChar);
            try {
                if (!File.Exists(appdir + "settings.ini"))
                {
                    File.WriteAllText(appdir + "settings.ini", "dataLink=https://raildatas.github.io/=asnr=399");
                }
                html = File.ReadAllText(appdir + "settings.ini");
                for (int i = 0; i < html.split("=").length; i += 2)
                    Share.settings.Add(html.split("=")[i], html.split("=")[i + 1]);
                if(!File.Exists(appdir+"evnets.dta"))
                    html = Gethtml(Share.settings.GetE("dataLink") + "ver.html", 0);
                else {
                    html = Gethtml(Share.settings.GetE("dataLink") + "ver.html", 10);
                    if(File.ReadAllText(appdir + "ver.dta").equals(html))
                        throw new Exception();
                }
                File.WriteAllText(appdir+"ver.dta",html);
                html = Gethtml(Share.settings.GetE("dataLink") + "emudata.html",(File.Exists(appdir+"evnets.dta")?10:0));
                DataFiles.emuDatas = replace(html, '\r').split("\n");
                File.WriteAllText(appdir+"emudata.dta",html);
                html = Gethtml(Share.settings.GetE("dataLink") + "railwaydata.html",(File.Exists(appdir+"evnets.dta")?10:0));
                DataFiles.railDatas = replace(html, '\r').split("\n");
                File.WriteAllText(appdir+"railwaydata.dta",html);
                html = Gethtml(Share.settings.GetE("dataLink") + "replace.html",(File.Exists(appdir+"evnets.dta")?10:0));
                replaceDatas = replace(html, '\r').split("\n");
                File.WriteAllText(appdir+"replace.dta",html);
                html = Gethtml(Share.settings.GetE("dataLink") + "rcp.html",(File.Exists(appdir+"evnets.dta")?10:0));
                rcps = replace(html, '\r').split("\n");
                File.WriteAllText(appdir+"rcp.dta",html);
                html = Gethtml(Share.settings.GetE("dataLink") + "smallstations.html",(File.Exists(appdir+"evnets.dta")?10:0));
                ssstrs = replace(html, '\r').split("\n");
                File.WriteAllText(appdir+"smallstations.dta",html);
                html = Gethtml(Share.settings.GetE("dataLink") + "evnets.html",(File.Exists(appdir+"evnets.dta")?10:0));
                evens = replace(html, '\r').split("\n");
                File.WriteAllText(appdir+"evnets.dta",html);
            }
            catch (Exception e) {
                //System.exit(1);
                if(!File.Exists(appdir+"evnets.dta"))
                    System.exit(1);
                else {
                    try {
                        html = File.ReadAllText(appdir + "emudata.dta");
                        DataFiles.emuDatas = replace(html, '\r').split("\n");
                        html = File.ReadAllText(appdir + "railwaydata.dta");
                        DataFiles.railDatas = replace(html, '\r').split("\n");
                        html = File.ReadAllText(appdir + "replace.dta");
                        replaceDatas = replace(html, '\r').split("\n");
                        html = File.ReadAllText(appdir + "rcp.dta");
                        rcps = replace(html, '\r').split("\n");
                        html = File.ReadAllText(appdir + "smallstations.dta");
                        ssstrs = replace(html, '\r').split("\n");
                        html = File.ReadAllText(appdir + "evnets.dta");
                        evens = replace(html, '\r').split("\n");
                    } catch (Exception e2) {
                        System.exit(1);
                    }
                }
            }
            try {
                if(rcps.length > 0) {
                    selectC = 0;
                    ((Button)findViewById(R.id.sp_rcp)).setText(rcps[0]);
                }
                for (int i = 0; i < evens.length; i++) {
                    tmp3 = evens[i].split(" ");
                    //es.add();
                    tmp4 = new Even(Integer.parseInt(tmp3[0]),Integer.parseInt(tmp3[1]),Integer.parseInt(tmp3[2]), tmp3[3]).IsNow();
                    if(tmp4 != -1)
                        ((TextView)findViewById(R.id.tv_even)).setText((((TextView)findViewById(R.id.tv_even)).getText().toString().equals("")?"今天是":"\n今天是") + tmp3[3] + Integer.toString(tmp4) + "周年");
                }
                for (int i = 0; i < DataFiles.emuDatas.length; i++) {
                    emustrs = DataFiles.emuDatas[i].split("\\*");
                    if (i == 54)
                        i = 54;
                    trains = new List<>();
                    ver = new List<>();
                    for (int j = 4, x = 0; j < emustrs.length; j += 2, x++) {
                        trainstrs = emustrs[j].split(" ");
                        trains.Add(new List<Train>());
                        ver.Add(emustrs[j - 1]);
                        for (int k = 0; k < trainstrs.length; k++) {
                            seatstrs = trainstrs[k].split("-");
                            persons.clear();
                            seatTypes.clear();
                            for (int l = 0; l < seatstrs.length; l += 2)
                            {
                                persons.Add(Byte.valueOf(seatstrs[l + 1]));
                                seatTypes.Add(GetSeatType(seatstrs[l]));
                            }
                            trains.get(x).Add(new Train(persons, seatTypes, (byte)(k + 1)));
                        }
                    }
                    Share.emus.Add(new EMU(Double.valueOf(emustrs[1]), emustrs[0], trains, ver, Integer.valueOf(emustrs[2])));
                }
                if(Share.emus.size()>0) {
                    train = Share.emus.get(0).name;
                    this.ver = Share.emus.get(0).versions.get(0);
                    ((Button)(findViewById(R.id.sp_emu))).setText(train + "(" + this.ver + ")");
                    if(Share.emus.get(0).trains.get(0).size() > 9)
                        ((Switch)(findViewById(R.id.sw_double))).setEnabled(false);
                    else
                        ((Switch)(findViewById(R.id.sw_double))).setEnabled(true);
                }
                for (EMU item2:Share.emus)
                    emus.add(item2.name);
                Share.stations = new List<Station>();
                for (int i = 0; i < DataFiles.railDatas.length; i++) {
                    railstr = new JSONObject(DataFiles.railDatas[i]);
                    Share.rails.Add(new Rail());
                    Share.rails.get(i).name = (String)railstr.getString("lineName");
                    Share.rails.get(i).speeds = railstr.getJSONObject("serviceTime").getString("up");
                    Share.rails.get(i).locations = railstr.getJSONObject("route").getJSONArray("up");
                    Share.rails.get(i).lengths = new List<>();
                    Share.rails.get(i).stations = new List<String>();
                    length = 0;
                    for (int j = 0;j < Share.rails.get(i).locations.length();j++) {
                        item = Share.rails.get(i).locations.getJSONObject(j);
                        tmp = item;
                        String str = tmp.getString("type");
                        Log.w("tag",tmp.getString("type").equals("station")?"t":"f");
                        if (tmp.getString("type").equals("station")) {
                            Share.rails.get(i).stations.Add(tmp.getString("name"));
                            if (j > 0)
                                Share.rails.get(i).lengths.add(length);
                            length = 0;
                            if (!((tmp.getString("name").toLowerCase().startsWith("x")))) {
                                b = false;
                                int k = 0;
                                for (Station item2 : Share.stations) {
                                    if (item2.name.equals(tmp.getString("name"))) {
                                        b = true;
                                        Share.stations.get(k).AddRail(new Integer[]{Integer.valueOf(i)});
                                        break;
                                    }
                                    k++;
                                }
                                if (!b) {
                                    station = new Station(tmp.getString("name"));
                                    station.AddRail(new Integer[]{Integer.valueOf(i)});
                                    Share.stations.Add(station);
                                }
                                //Share.lists.Add((tmp.getString("name")).charAt(0), new List<Integer>());
                                //if (!b)
                                //    Share.lists.GetE((tmp.getString("name")).charAt(0)).Add(k);
                            }
                        }
                        if(j < Share.rails.get(i).locations.length() - 1) {
                            length += GetLength(tmp.getDouble("lng"), tmp.getDouble("lat"),
                            Share.rails.get(i).locations.getJSONObject(j + 1).getDouble("lng"),
                            Share.rails.get(i).locations.getJSONObject(j + 1).getDouble("lat"));
                        }
                    }
                }
                for(int i = 0;i < ssstrs.length;i++) {
                    temp = ssstrs[i].split(" ");
                    for(int j = 0;j < Share.stations.size();j++) {
                        if(Share.stations.get(j).name.equals(temp[0])) {
                            ssns.add(Integer.valueOf(j));
                            break;
                        }
                    }
                    sss.add(new List<>());
                    for (int j = 1;j < temp.length;j++) {
                        temp2 = temp[j].split("&");
                        sss.get(i).add(new List<>());
                        for (int k = 0;k < temp2.length;k++) {
                            for (int l = 0;l < Share.rails.size();l++) {
                                if(Share.rails.get(l).name.equals(temp2[k])) {
                                    sss.get(i).get(j - 1).add(Integer.valueOf(l));
                                    break;
                                }
                            }
                        }
                    }
                }
                for(String item2:replaceDatas) {
                    if(item2.startsWith("*")) {
                        dic2.Add(item2.substring(1).split(" ")[0], item2.split(" ")[1]);
                        dk2.add(item2.substring(1).split(" ")[0]);
                    }
                    else {
                        dic.Add(item2.split(" ")[0], item2.split(" ")[1]);
                        dk1.add(item2.split(" ")[0]);
                    }
                }
                for(int i = 0;i < Share.rails.size(); i++)
                {
                    spdstrs = Share.rails.get(i).speeds.split("\n");
                    for(int j = 0;j < spdstrs.length;j++)
                    {
                        if (spdstrs[j].startsWith("1") || spdstrs[j].startsWith("2") || spdstrs[j].startsWith("3") || spdstrs[j].startsWith("4") || spdstrs[j].startsWith("5") ||
                                spdstrs[j].startsWith("6") || spdstrs[j].startsWith("7") || spdstrs[j].startsWith("8") || spdstrs[j].startsWith("9"))
                        {
                            spdstrs[j] = "atp " + spdstrs[j] + "\n" +
                                    "ms " + spdstrs[j] + "\n" +
                                    "ss " + spdstrs[j];
                        }
                    }
                    Share.rails.get(i).speeds = "";
                    for (int j = 0;j < spdstrs.length;j++)
                    {
                        Share.rails.get(i).speeds += spdstrs[j];
                        if (j + 1 < spdstrs.length)
                            Share.rails.get(i).speeds += "\n";
                    }
                }
            }
            catch (Exception e) {
                //System.exit(1);
            }
            flag = false;
        ((EditText)(findViewById(R.id.et_before))).setOnFocusChangeListener(this);
        ((EditText)findViewById(R.id.sw_no_after)).addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {    }
            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {    }
            @Override
            public void afterTextChanged(Editable s) {
                if (selectA < 0)
                    return;
                if (((EditText)findViewById(R.id.sw_no_after)).getText().toString().equals(""))
                    return;
                if ((Integer.valueOf(((EditText)findViewById(R.id.sw_no_after)).getText().toString()) > 0) &&
                    Share.stations.get(selectA).name.endsWith("线路所"))
                    ((EditText)findViewById(R.id.sw_no_after)).setText("0");
            }
        });
        ((Switch)findViewById(R.id.sw_no350mode)).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                SetTime();
            }
        });
        ((Switch)findViewById(R.id.sw_isAstar)).setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                if (isChecked)
                {
                    ((Button)findViewById(R.id.sp_lineName)).setEnabled(false);
                    ((Button)findViewById(R.id.sp_lineName)).setText("");
                    ((Button)findViewById(R.id.sp_after)).setText("");
                    ((Button)findViewById(R.id.sp_after)).setEnabled(true);
                }
                else
                {
                    if (((Button)findViewById(R.id.sp_lineName)).getText().toString().equals(""))
                        ((Button)findViewById(R.id.sp_after)).setEnabled(false);
                    ((Button)findViewById(R.id.sp_lineName)).setEnabled(true);
                    ((Button)findViewById(R.id.sp_lineName)).setText("");
                    ((Button)findViewById(R.id.sp_after)).setText("");
                }
            }
        });
        ((Button)findViewById(R.id.btn_set)).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent();
                intent.setClass(MainActivity.this,SettingsActivity.class);
                startActivity(intent);
            }
        });
    }
    @Override
    public void onFocusChange(View v, boolean hasFocus) {
        if(!hasFocus) {
            if(((EditText)(findViewById(R.id.et_before))).length() > 0) {
                boolean b = false;
                int i = 0;
                for (Station item : Share.stations) {
                    if (item.name
                            .equals(((EditText)(findViewById(R.id.et_before))).getText().toString())
                            && (!(item.name.toLowerCase().startsWith("x")))) {
                        b = true;
                        break;
                    }
                    i++;
                }
                if(selectB != i) {
                    selectL = -1;
                    selectA = -1;
                    ((Button)(findViewById(R.id.sp_lineName))).setText("");
                    ((Button)(findViewById(R.id.sp_after))).setText("");
                    ((Button)(findViewById(R.id.btn_add))).setEnabled(false);
                }
                selectB = i;
                if(!b) {
                    //((EditText)(findViewById(R.id.et_before))).setText("");
                    //new AlertDialog.Builder(MainActivity.this).setTitle("未找到起点站").create().show();
                }
            }
            else {
                selectL = -1;
                selectA = -1;
                ((Button)(findViewById(R.id.sp_lineName))).setText("");
                ((Button)(findViewById(R.id.sp_after))).setText("");
                ((Button)(findViewById(R.id.btn_add))).setEnabled(false);
            }
        }
    }
    public void ChangeDire(View view) {
        if(select) {
            select = false;
            ((Button)(findViewById(R.id.btn_down))).setBackgroundTintList(ColorStateList.valueOf(
                    ContextCompat.getColor(this,R.color.CRH)));
            ((Button)(findViewById(R.id.btn_up))).setBackgroundTintList(ColorStateList.valueOf(
                    ContextCompat.getColor(this,R.color.E_Train)));
        }
        else {
            select = true;
            ((Button) (findViewById(R.id.btn_up))).setBackgroundTintList(ColorStateList.valueOf(
                    ContextCompat.getColor(this, R.color.CRH)));
            ((Button) (findViewById(R.id.btn_down))).setBackgroundTintList(ColorStateList.valueOf(
                    ContextCompat.getColor(this, R.color.E_Train)));
        }
        if (lis.size() < 1) {
            selectB = -1;
            ((EditText)(findViewById(R.id.et_before))).setEnabled(true);
            ((Button)(findViewById(R.id.btn_save))).setEnabled(false);
            ((Button)(findViewById(R.id.btn_out))).setEnabled(false);
            ((EditText)(findViewById(R.id.et_before))).setText("");
        }
        else if (!select) {
            selectB = Integer.valueOf(Integer.toString(lis.get(lis.size() - 1).after));
            ((EditText)(findViewById(R.id.sw_no_before))).setText(Integer.toString(lis.get(lis.size() - 1).afterstop));
        }
        else {
            selectB = Integer.valueOf(Integer.toString(lis.get(0).before));
            ((EditText)(findViewById(R.id.sw_no_before))).setText(Integer.toString(lis.get(0).beforestop));
        }
        if(lis.size() > 0) {
            ((EditText) (findViewById(R.id.et_before))).setText(Share.stations.get(selectB).name);
            ((Switch) findViewById(R.id.sw_before_tst)).setChecked(select ? lis.get(0).beforeteg : lis.get(lis.size() - 1).beforeteg);
        }
        selectL = selectA = -1;
        ((Button)(findViewById(R.id.sp_lineName))).setText("");
        ((Button)(findViewById(R.id.sp_after))).setText("");
        ((Button)(findViewById(R.id.btn_add))).setEnabled(false);
        ((Button)(findViewById(R.id.sp_after))).setEnabled(false);
        ((EditText)(findViewById(R.id.sw_no_after))).setText("2");
        ((EditText)(findViewById(R.id.et_early_time))).setText("5");
        ((Switch)findViewById(R.id.sw_after_tst)).setChecked(false);
        ((RadioButton)findViewById(R.id.rb_slow)).setChecked(true);
        ((Button)(findViewById(R.id.sp_lineName))).setText("");
        ((Button)(findViewById(R.id.sp_after))).setText("");
    }
    public void Click_Del(View view) {
        if(lis.size()>0) {
            if (select)
                lis.remove((int)0);
            else
                lis.remove(lis.size() - 1);
            Update();
            if(lis.size() < 1) {
                selectB = -1;
                ((EditText)(findViewById(R.id.et_before))).setEnabled(true);
                ((Button)(findViewById(R.id.btn_save))).setEnabled(false);
                ((Button)(findViewById(R.id.btn_out))).setEnabled(false);
                ((EditText)(findViewById(R.id.et_before))).setText("");
                ((EditText)(findViewById(R.id.sw_no_before))).setText("2");
                ((EditText)(findViewById(R.id.sw_no_before))).setEnabled(true);
            }
            else if (!select) {
                selectB = Integer.valueOf(Integer.toString(lis.get(lis.size() - 1).after));
                ((EditText)(findViewById(R.id.sw_no_before))).setText(Integer.toString(lis.get(lis.size() - 1).afterstop));
            }
            else {
                selectB = Integer.valueOf(Integer.toString(lis.get(0).before));
                ((EditText)(findViewById(R.id.sw_no_before))).setText(Integer.toString(lis.get(0).beforestop));
            }
            if(lis.size() > 0) {
                ((EditText) (findViewById(R.id.et_before))).setText(Share.stations.get(selectB).name);
                ((Switch)findViewById(R.id.sw_before_tst)).setChecked(select?lis.get(0).beforeteg:lis.get(lis.size() - 1).beforeteg);
            }
            selectL = selectA = -1;
            ((Button)(findViewById(R.id.sp_lineName))).setText("");
            ((Button)(findViewById(R.id.sp_after))).setText("");
            ((Button)(findViewById(R.id.btn_add))).setEnabled(false);
            if(!((Switch)findViewById(R.id.sw_isAstar)).isChecked())
                ((Button)(findViewById(R.id.sp_after))).setEnabled(false);
            ((EditText)(findViewById(R.id.sw_no_after))).setText("2");
            ((EditText)(findViewById(R.id.et_early_time))).setText("5");
            ((Switch)findViewById(R.id.sw_after_tst)).setChecked(false);
            ((RadioButton)findViewById(R.id.rb_slow)).setChecked(true);
            SetTime();
        }
    }
    private void Update() {
        StringBuilder sb = new StringBuilder();
        if(lis.size() > 0) {
            sb.append(Share.stations.get(lis.get(0).before).name);
            for (ListItem li : lis) {
                sb.append("\n");
                sb.append(Share.stations.get(li.after).name);
            }
        }
        ((TextView)findViewById(R.id.lv_stationbox)).setText(sb.toString());
    }
    public void SetUpStart(View view) {
        new TimePickerDialog(this, android.R.style.Theme_Holo_Light_Dialog, new TimePickerDialog.OnTimeSetListener() {
            @Override
            public void onTimeSet(TimePicker view, int hourOfDay, int minute) {
                upmin = minute;
                uphour = hourOfDay;
                String tmp1 = minute<10?"0"+Integer.toString(minute):Integer.toString(minute);
                String tmp2 = hourOfDay<10?"0"+Integer.toString(hourOfDay):Integer.toString(hourOfDay);
                ((TextView)(findViewById(R.id.tv_upstart))).setText(tmp2+":"+tmp1);
                SetTime();
            }
        }, uphour, upmin, true).show();
    }
    public void SetDownStart(View view) {
        new TimePickerDialog(this, android.R.style.Theme_Holo_Light_Dialog, (view1, hourOfDay, minute) -> {
            downmin = minute;
            downhour = hourOfDay;
            String tmp1 = minute<10?"0"+Integer.toString(minute):Integer.toString(minute);
            String tmp2 = hourOfDay<10?"0"+Integer.toString(hourOfDay):Integer.toString(hourOfDay);
            ((TextView)(findViewById(R.id.tv_downstart))).setText(tmp2+":"+tmp1);
            SetTime();
        },
                downhour, downmin, true).show();
    }
    public String[] ToArray(List<String> str){
        final String[] tmp = new String[str.size()];
        for (int i=0;i<str.size();i++)
            tmp[i] = str.get(i);
        return tmp;
    }
    public void ChangeTrain(View view) {
        String[] tmp = ToArray(emus);
        int tmp2 = 0;
        for(String i:emus) {
            if(train.equals(i))
                break;
            tmp2++;
        }
        new AlertDialog.Builder(this)
                .setTitle("请选择车型")
                .setSingleChoiceItems(tmp, tmp2, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        train = tmp[which];
                        String[] tmp4 = ToArray(Share.emus.get(which).versions);
                        if (tmp4.length <= 1) {
                            ver = tmp4[0];
                            ((Button)(findViewById(R.id.sp_emu))).setText(train + "(" + ver + ")");
                            if(Share.emus.get(which).trains.get(0).size() > 9) {
                                ((Switch) (findViewById(R.id.sw_double))).setEnabled(false);
                                ((Switch) (findViewById(R.id.sw_double))).setChecked(false);
                            }
                            else
                                ((Switch)(findViewById(R.id.sw_double))).setEnabled(true);
                            SetTime();
                            dialog.cancel();
                            return;
                        }
                        int tmp3 = 0;
                        int tmp5 = which;
                        dialog.cancel();
                        new AlertDialog.Builder(MainActivity.this)
                                .setTitle("请选择车型")
                                .setSingleChoiceItems(tmp4, tmp3, new DialogInterface.OnClickListener() {
                                    @Override
                                    public void onClick(DialogInterface dialog, int which) {
                                        ver = tmp4[which];
                                        ((Button)(findViewById(R.id.sp_emu))).setText(train + "(" + ver + ")");
                                        if(Share.emus.get(tmp5).trains.get(which).size() > 9) {
                                            ((Switch) (findViewById(R.id.sw_double))).setEnabled(false);
                                            ((Switch) (findViewById(R.id.sw_double))).setChecked(false);
                                        }
                                        else
                                            ((Switch)(findViewById(R.id.sw_double))).setEnabled(true);
                                        SetTime();
                                        dialog.cancel();
                                    }
                                }).create().show();
                    }
                }).create().show();
    }
    public void ChangeLine(View view) {
        onFocusChange(view, false);
        List<String> strs = new List<>();
        boolean b = false;
        boolean bIsSmall = false;
        boolean b1 = false;
        if (lis.size() != 0) {
            for(int i = 0;i < ssns.size();i++) {
                if (ssns.get(i) == lis.get(lis.size() - 1).after) {
                    bIsSmall = true;
                    boolean zc = false;
                    for(int j = 0;j < sss.get(i).size();j++) {
                        for(int k = 0;k < sss.get(i).get(j).size();k++) {
                            if((zc == false) && (sss.get(i).get(j).get(k) == lis.get(lis.size() - 1).line)) {
                                zc = true;
                                b = true;
                                k = 0;
                                //strs.add(Share.rails.get(lis.get(lis.size() - 1).line).name);
                            }
                            b1 = false || (strs.size() == 0);
                            for(int l = 0;l < strs.size();l++) {
                                b1 = b1 || (!Share.rails.get(sss.get(i).get(j).get(k)).name.equals(strs.get(l)));
                                if((!Share.rails.get(sss.get(i).get(j).get(k)).name.equals(strs.get(l))) == false) {
                                    b1 = false;
                                    break;
                                }
                            }
                            if (zc && b1)
                                strs.add(Share.rails.get(sss.get(i).get(j).get(k)).name);
                        }
                        zc = false;
                        b1 = false;
                    }
                    break;
                }
            }
        }
        if ((bIsSmall == false)||(b == false)) {
            for (Station item : Share.stations) {
                if (item.name
                        .equals(((EditText) (findViewById(R.id.et_before))).getText().toString())
                        && (!(item.name.toLowerCase().startsWith("x"))) && ((!(item.name.toLowerCase().endsWith("线路所"))) || (!((EditText) findViewById(R.id.et_before)).isEnabled()))) {
                    b = true;
                    for (int items : item.rails) {
                        strs.add(Share.rails.get(items).name);
                    }
                }
            }
        }
        if (b == false) {
            ((EditText)(findViewById(R.id.et_before))).setText("");
            new AlertDialog.Builder(MainActivity.this).setTitle("未找到起点站").setPositiveButton("确认", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {

                }
            }).create().show();
            return;
        }
        else {
            int tmp2 = 0;
            if(selectL > -1) {
                for (String i : strs) {
                    if (Share.rails.get(selectL).name.equals(i))
                        break;
                    tmp2++;
                }
            }
            new AlertDialog.Builder(this)
                    .setTitle("请选择线路")
                    .setSingleChoiceItems(ToArray(strs), tmp2, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            int i = 0;
                            for (Rail rail:Share.rails) {
                                if(rail.name.equals(strs.get(which)))
                                    break;
                                i++;
                            }
                            if(selectL != i) {
                                selectA = -1;
                                ((Button)(findViewById(R.id.sp_after))).setText("");
                                ((Button)(findViewById(R.id.btn_add))).setEnabled(false);
                            }
                            selectL = i;
                            ((Button)(findViewById(R.id.sp_lineName))).setText(strs.get(which));
                            ((Button)(findViewById(R.id.sp_after))).setEnabled(true);
                            dialog.cancel();
                        }
                    }).create().show();
        }
    }
    public void ChangeAfter(View view) {
        if(((Switch)findViewById(R.id.sw_isAstar)).isChecked())
        {
            EditText et = new EditText(this);
            new AlertDialog.Builder(this)
                    .setTitle("请输入车站")
                    .setView(et)
                    .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            for (Station stat:Share.stations) {
                                if (stat.name.equals(et.getText().toString()))
                                {
                                    ((Button)findViewById(R.id.sp_after)).setText(et.getText());
                                    ((Button)findViewById(R.id.btn_add)).setEnabled(true);
                                    return;
                                }
                            }
                            ((Button)findViewById(R.id.btn_add)).setEnabled(false);
                        }
                    })
                    .create().show();
            return;
        }
        List<String> strs = new List<>();
        boolean b = false;
        //Toast.makeText(this, " ", Toast.LENGTH_SHORT).show();
        for (String items : Share.rails.get(selectL).stations) {
            if(!(items.equals(Share.stations.get(selectB).name))) {
                if ((!items.startsWith("x")))
                    strs.add(items);
            }
        }
        int tmp2 = 0;
        if(selectA > -1) {
            for (String i : strs) {
                if (Share.stations.get(selectA).name.equals(i))
                    break;
                tmp2++;
            }
        }
        new AlertDialog.Builder(this)
                .setTitle("请选择车站")
                .setSingleChoiceItems(ToArray(strs), tmp2, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        int i = 0;
                        for (Station rail:Share.stations) {
                            if(rail.name.equals(strs.get(which)))
                                break;
                            i++;
                        }
                        selectA = i;
                        ((Button)(findViewById(R.id.sp_after))).setText(strs.get(which));
                        ((Button)(findViewById(R.id.btn_add))).setEnabled(true);
                        if (strs.get(which).endsWith("线路所")) {
                            ((EditText)findViewById(R.id.sw_no_after)).setText("0");
                            //((EditText)findViewById(R.id.sw_no_after)).setEnabled(false);
                        }
                        else {
                            ((EditText)findViewById(R.id.sw_no_after)).setText("2");
                            ((EditText)findViewById(R.id.sw_no_after)).setEnabled(true);
                        }
                        dialog.cancel();
                    }
                }).create().show();
    }
    public void AddLine(View view) {
        RunMode mode = null;
        ((Button)(findViewById(R.id.btn_save))).setEnabled(true);
        ((Button)(findViewById(R.id.btn_out))).setEnabled(true);
        if (((RadioButton)findViewById(R.id.rb_atp)).isChecked())
            mode = RunMode.MaxSpeed;
        else if (((RadioButton)findViewById(R.id.rb_max)).isChecked())
            mode = RunMode.MaxSpeed;
        else if (((RadioButton)findViewById(R.id.rb_slow)).isChecked())
            mode = RunMode.Express;
        if (((EditText)findViewById(R.id.sw_no_before)).getText().toString().equals(""))
            ((EditText)findViewById(R.id.sw_no_before)).setText("0");
        if (((EditText)findViewById(R.id.sw_no_after)).getText().toString().equals(""))
            ((EditText)findViewById(R.id.sw_no_after)).setText("0");
        if (((EditText)findViewById(R.id.et_early_time)).getText().toString().equals(""))
            ((EditText)findViewById(R.id.et_early_time)).setText("0");
        if (((Switch)findViewById(R.id.sw_isAstar)).isChecked())
        {
            List<ListItem> liss;
            int tmp1 = 0;
            int tmp2 = 1;
            int i = 0;
            for (Station stat : Share.stations)
            {
                if (stat.name.equals(((EditText)findViewById(R.id.et_before)).getText().toString()))
                    tmp1 = i;
                if (stat.name.equals(((Button)findViewById(R.id.sp_after)).getText().toString()))
                    tmp2 = i;
                i++;
            }
            if (select)
            {
                liss = AStar(tmp2, tmp1, Integer.valueOf(((EditText)findViewById(R.id.sw_no_after)).getText().toString()), Integer.valueOf(((EditText)findViewById(R.id.sw_no_before)).getText().toString()), Integer.valueOf(((EditText)findViewById(R.id.et_early_time)).getText().toString()),
                        mode
                        ,((Switch)findViewById(R.id.sw_before_tst)).isChecked(),
                        ((Switch)findViewById(R.id.sw_after_tst)).isChecked());
            }
            else
            {
                liss = AStar(tmp1, tmp2, Integer.valueOf(((EditText)findViewById(R.id.sw_no_before)).getText().toString()), Integer.valueOf(((EditText)findViewById(R.id.sw_no_after)).getText().toString()), Integer.valueOf(((EditText)findViewById(R.id.et_early_time)).getText().toString()),
                        mode
                        ,((Switch)findViewById(R.id.sw_before_tst)).isChecked(),
                        ((Switch)findViewById(R.id.sw_after_tst)).isChecked());
            }
            if (liss.size() == 0)
            {
                //MessageBox.Show("未找到路径！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (lis.size() == 0)
                ((TextView)(findViewById(R.id.lv_stationbox))).setText(Share.stations.get(tmp1).name);
            for (i = 0; i < liss.size(); i++)
            {
                lis.Add(liss.get(i));
            }
            ((Button)(findViewById(R.id.btn_add))).setEnabled(false);
            Update();
            SetTime();
            ((EditText)findViewById(R.id.et_before)).setText(Share.stations.get(tmp2).name);
            ((EditText)findViewById(R.id.et_before)).setEnabled(false);
            ((Button)(findViewById(R.id.sp_after))).setEnabled(true);
            ((Button)(findViewById(R.id.sp_after))).setText("");
            ((EditText)(findViewById(R.id.sw_no_after))).setText("2");
            ((EditText)(findViewById(R.id.et_early_time))).setText("5");
            ((Switch)findViewById(R.id.sw_before_tst)).setChecked(((Switch)findViewById(R.id.sw_after_tst)).isChecked());
            ((Switch)findViewById(R.id.sw_before_tst)).setEnabled(false);
            ((Switch)findViewById(R.id.sw_after_tst)).setChecked(false);
            ((RadioButton)findViewById(R.id.rb_slow)).setChecked(true);
            return;
        }
        if (select == true) {
            ListItem li = new ListItem(selectA,
                    Integer.valueOf(((EditText)findViewById(R.id.sw_no_after)).getText().toString()),
                    selectL,
                    mode
                    , selectB,
                    Integer.valueOf(((EditText)findViewById(R.id.sw_no_before)).getText().toString()),
                    Integer.valueOf(((EditText)findViewById(R.id.et_early_time)).getText().toString()),
                    ((Switch)findViewById(R.id.sw_after_tst)).isChecked(),
                    ((Switch)findViewById(R.id.sw_before_tst)).isChecked());
            if(lis.size() > 0)
                lis.add(0, li);
            else
                lis.add(li);
        }
        else
            lis.add(new ListItem(selectB,
                Integer.valueOf(((EditText)findViewById(R.id.sw_no_before)).getText().toString())
                , selectL,
                mode
                , selectA,
                Integer.valueOf(((EditText)findViewById(R.id.sw_no_after)).getText().toString()),
                Integer.valueOf(((EditText)findViewById(R.id.et_early_time)).getText().toString()),
                ((Switch)findViewById(R.id.sw_before_tst)).isChecked(),
                ((Switch)findViewById(R.id.sw_after_tst)).isChecked()));
        ((EditText)(findViewById(R.id.et_before))).setText(Share.stations.get(selectA).name);
        Update();
        selectB = selectA;
        selectL = -1;
        selectA = -1;
        ((Button)(findViewById(R.id.sp_lineName))).setText("");
        ((Button)(findViewById(R.id.sp_after))).setText("");
        ((Button)(findViewById(R.id.btn_add))).setEnabled(false);
        ((Button)(findViewById(R.id.sp_after))).setEnabled(false);
        ((EditText)(findViewById(R.id.et_before))).setEnabled(false);
        ((EditText)(findViewById(R.id.sw_no_before))).setText(Integer.toString(select?lis.get(0).beforestop:lis.get(lis.size()-1).afterstop));
        ((EditText)(findViewById(R.id.sw_no_before))).setEnabled(false);
        ((EditText)(findViewById(R.id.sw_no_after))).setText("2");
        ((EditText)(findViewById(R.id.et_early_time))).setText("5");
        ((Switch)findViewById(R.id.sw_before_tst)).setChecked(((Switch)findViewById(R.id.sw_after_tst)).isChecked());
        ((Switch)findViewById(R.id.sw_before_tst)).setEnabled(false);
        ((Switch)findViewById(R.id.sw_after_tst)).setChecked(false);
        ((RadioButton)findViewById(R.id.rb_slow)).setChecked(true);
        ((EditText)findViewById(R.id.sw_no_after)).setEnabled(true);
        SetTime();
    }
    protected void SetTime() {
        upTime = CalcTime(upmin, uphour, true);
        ((TextView)findViewById(R.id.tv_uptime)).setText(upTime);
        downTime = CalcTime(downmin, downhour, false);
        ((TextView)findViewById(R.id.tv_downtime)).setText(downTime);
    }
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == 1 && resultCode == Activity.RESULT_OK) { //out
            if (!Environment.isExternalStorageManager())
                ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.READ_EXTERNAL_STORAGE, Manifest.permission.WRITE_EXTERNAL_STORAGE}, 4);
            //int MAX_PROGRESS = lis.size() + 3;
            //progressDialog //progressDialog =
                    //new //progressDialog(MainActivity.this);
            //progressDialog.setProgress(0);
            //progressDialog.setTitle("正在导出中……");
            //progressDialog.setProgressStyle(//progressDialog.STYLE_HORIZONTAL);
            //progressDialog.setMax(MAX_PROGRESS);
            //progressDialog.show();
            AtomicBoolean isshow = new AtomicBoolean(false);
            new AlertDialog.Builder(this)
                    .setTitle("是否在地图中显示到发时间？")
                    .setMessage("")
                    .setPositiveButton("是", (dialog, which) -> { isshow.set(true); Out(isshow, data); })
                    .setNegativeButton("否", (dialog, which) -> { isshow.set(false); Out(isshow, data); })
                    .create().show();
            //Out(isshow, data);
        }
        else if((requestCode == 2 &&(!Environment.isExternalStorageManager()))) {
            new AlertDialog.Builder(this)
                    .setTitle("提示")
                    .setMessage("你没有允许访问文件，无法打开、保存")
                    .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {  }
                    }).create().show();
        }
        else if (requestCode == 3 && resultCode == Activity.RESULT_OK) {
            /*new AlertDialog.Builder(this)
                    .setTitle("是否手动设置列车信息？")
                    .setPositiveButton("是", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            dialog.cancel();
                            SpwanLCD(data.getData().getPath().replace("/tree/primary:","/storage/emulated/0/"), true);
                        }
                    })
                    .setNegativeButton("否", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            dialog.cancel();
                            SpwanLCD(data.getData().getPath().replace("/tree/primary:","/storage/emulated/0/"), false);
                        }
                    })
                    .create().show();*/
            SpwanLCD(data.getData().getPath().replace("/tree/primary:","/storage/emulated/0/"), false);
        }
        else if(requestCode == 5 && resultCode == Activity.RESULT_OK) { //save
            String path = data.getData().getPath().replace("/tree/primary:","/storage/emulated/0/")
                    +java.io.File.separatorChar+((EditText)findViewById(R.id.et_trainNumber)).getText()
                    .toString().replace("/","_")+".tlf";
            //int MAX_PROGRESS = lis.size() + 3;
            //progressDialog //progressDialog =
                    //new //progressDialog(MainActivity.this);
            //progressDialog.setProgress(0);
            //progressDialog.setTitle("正在保存中……");
            //progressDialog.setProgressStyle(//progressDialog.STYLE_HORIZONTAL);
            //progressDialog.setMax(MAX_PROGRESS);
            //progressDialog.show();
            //int MAX_PROGRESS2 = lis.size() + 2;
            //progressDialog //progressDialog2 =
                    //new //progressDialog(MainActivity.this);
            //progressDialog2.setProgress(0);
            //progressDialog2.setTitle("正在导出中……");
            //progressDialog2.setProgressStyle(//progressDialog.STYLE_HORIZONTAL);
            //progressDialog2.setMax(MAX_PROGRESS2);
            //progressDialog2.show();

            StringBuilder sb2 = new StringBuilder();
            sb2.append(rcps[selectC]);
            sb2.append("\n");
            sb2.append(uphour);
            sb2.append(" ");
            sb2.append(upmin);
            sb2.append("\n");
            sb2.append(downhour);
            sb2.append(" ");
            sb2.append(downmin);
            sb2.append("\n");
            sb2.append(train);
            sb2.append("\n");
            sb2.append(ver);
            sb2.append("\n");
            sb2.append(((Switch)findViewById(R.id.sw_double)).isChecked() ? "t" : "f");
            sb2.append(" ");
            sb2.append(((Switch)findViewById(R.id.sw_no350mode)).isChecked() ? "t" : "f");
            sb2.append("\n");
            sb2.append(((EditText)findViewById(R.id.et_trainNumber)).getText());
            //progressDialog2.setProgress(//progressDialog.getProgress() + 1);
            for (int i = 0; i < lis.size();i++) {
                sb2.append("\n");
                sb2.append(Share.stations.get(lis.get(i).before).name);
                sb2.append(" ");
                sb2.append(Share.rails.get(lis.get(i).line).name);
                sb2.append(" ");
                sb2.append(Share.stations.get(lis.get(i).after).name);
                sb2.append(" ");
                sb2.append(lis.get(i).beforestop);
                sb2.append(" ");
                sb2.append(lis.get(i).beforeteg ? "t" : "f");
                sb2.append(" ");
                sb2.append(lis.get(i).afterstop);
                sb2.append(" ");
                sb2.append(lis.get(i).afterteg ? "t" : "f");
                sb2.append(" ");
                sb2.append(lis.get(i).earlytime);
                sb2.append(" ");
                switch (lis.get(i).mode) {
                    case ATP:
                        sb2.append("a");
                        break;
                    case MaxSpeed:
                        sb2.append("m");
                        break;
                    case Express:
                        sb2.append("s");
                        break;
                }
                //progressDialog2.setProgress(//progressDialog.getProgress() + 1);
            }
            try {
                File.WriteAllText(path, sb2.toString());
                new AlertDialog.Builder(this)
                        .setTitle("提示")
                        .setMessage("保存成功")
                        .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {  }
                        }).create().show();
            }
            catch (Exception e) {
                //progressDialog2.cancel();
                new AlertDialog.Builder(this)
                        .setTitle("提示")
                        .setMessage("保存失败")
                        .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {  }
                        }).create().show();
            }
            //progressDialog2.setProgress(MAX_PROGRESS2);
            //progressDialog2.cancel();
        }
        else if(requestCode == 6 && resultCode == Activity.RESULT_OK) { //open
            try {
                    String s = File.ReadAllText(data.getData().getPath().replace("/document/primary:", "/storage/emulated/0/"));
                    String[] str = s
                        .replace("\r", "").split("\n");
                    String[] strs, sts;
                    int MAX_PROGRESS = str.length + 2;
                    //progressDialog //progressDialog =
                        //new //progressDialog(MainActivity.this);
                    //progressDialog.setProgress(0);
                    //progressDialog.setTitle("正在读取中……");
                    //progressDialog.setProgressStyle(//progressDialog.STYLE_HORIZONTAL);
                    //progressDialog.setMax(MAX_PROGRESS);
                    //progressDialog.show();
                    lis.clear();
                    boolean b7;
                    ListItem li;
                    uptimes = new List<>();
                    downtimes = new List<>();
                    for(int i = 0; i < rcps.length;i++) {
                        if(rcps[i].equals(str[0])) {
                            selectC = i;
                            ((Button)findViewById(R.id.sp_rcp)).setText(rcps[i]);
                            break;
                        }
                        if (i == rcps.length - 1)
                            throw new Exception("版本不兼容");
                    }
                    strs = str[1].split(" ");
                    uphour = Integer.valueOf(strs[0]);
                    upmin = Integer.valueOf(strs[1]);
                    String tmp1 = upmin<10?"0"+Integer.toString(upmin):Integer.toString(upmin);
                    String tmp2 = uphour<10?"0"+Integer.toString(uphour):Integer.toString(uphour);
                    ((TextView)(findViewById(R.id.tv_upstart))).setText(tmp2+":"+tmp1);
                    strs = str[2].split(" ");
                    downhour = Integer.valueOf(strs[0]);
                    downmin = Integer.valueOf(strs[1]);
                    tmp1 = downmin<10?"0"+Integer.toString(downmin):Integer.toString(downmin);
                    tmp2 = downhour<10?"0"+Integer.toString(downhour):Integer.toString(downhour);
                    ((TextView)(findViewById(R.id.tv_downstart))).setText(tmp2+":"+tmp1);
                    //progressDialog.setProgress(//progressDialog.getProgress() + 1);
                    train = str[3];
                    ver = str[4];
                    ((Button)findViewById(R.id.sp_emu)).setText(train + "(" + this.ver + ")");
                    int tmp5 = 0, which = 0;
                    for(EMU emu:Share.emus) {
                        if(train.equals(emu.name))
                            break;
                        tmp5++;
                    }
                    for(String v:Share.emus.get(tmp5).versions) {
                        if(v.equals(ver))
                            break;
                        which++;
                    }
                    if(Share.emus.get(tmp5).trains.get(which).size() > 9) {
                        ((Switch) (findViewById(R.id.sw_double))).setEnabled(false);
                        ((Switch) (findViewById(R.id.sw_double))).setChecked(false);
                    }
                    else
                        ((Switch)(findViewById(R.id.sw_double))).setEnabled(true);
                    sts = str[5].split(" ");
                    if (sts.length == 2) {
                        if(sts[0].equals("t"))
                            ((Switch)findViewById(R.id.sw_double)).setChecked(true);
                        else
                            ((Switch)findViewById(R.id.sw_double)).setChecked(false);
                        if(sts[1].equals("t"))
                            ((Switch)findViewById(R.id.sw_no350mode)).setChecked(true);
                        else
                            ((Switch)findViewById(R.id.sw_no350mode)).setChecked(false);
                    }
                    else
                    {
                        if(str[5].equals("t"))
                            ((Switch)findViewById(R.id.sw_double)).setChecked(true);
                        else
                            ((Switch)findViewById(R.id.sw_double)).setChecked(false);
                        ((Switch)findViewById(R.id.sw_no350mode)).setChecked(false);
                    }
                    ((EditText)findViewById(R.id.et_trainNumber)).setText(str[6]);
                    for (int i = 7;i < str.length;i++) {
                        strs = str[i].split(" ");
                        li = new ListItem(0,0,0,RunMode.MaxSpeed,0,0,0,false, false);
                        b7 = false;
                        for (int j = 0; j < Share.stations.size();j++) {
                        if(Share.stations.get(j).name.equals(strs[0])) {
                            li.before = j;
                            b7 = true;
                            break;
                        }
                        if(j == Share.stations.size() - 1) {
                            for (String stat:dk1) {
                                if(stat.equals(strs[0])) {
                                    strs[0] = stat;
                                    b7 = true;
                                    break;
                                }
                            }
                        }
                    }
                        if(!b7)
                            throw new Exception("版本不兼容");
                        b7 = false;
                        for (int j = 0; j < Share.stations.size();j++) {
                        if(Share.stations.get(j).name.equals(strs[2])) {
                            li.after = j;
                            b7 = true;
                            break;
                        }
                        if(j == Share.stations.size() - 1) {
                            for (String stat:dk1) {
                                if(stat.equals(strs[2])) {
                                    strs[2] = stat;
                                    b7 = true;
                                    break;
                                }
                            }
                        }
                    }
                        if(!b7)
                            throw new Exception("版本不兼容");
                        b7 = false;
                        for (int j = 0; j < Share.rails.size();j++) {
                        if(Share.rails.get(j).name.equals(strs[1])) {
                            li.line = j;
                            b7 = true;
                            break;
                        }
                        if(j == Share.rails.size() - 1) {
                            for (String stat:dk2) {
                                if(stat.equals(strs[1])) {
                                    strs[1] = stat;
                                    b7 = true;
                                    break;
                                }
                            }
                        }
                    }
                        if(!b7)
                            throw new Exception("版本不兼容");
                        li.afterstop = Integer.valueOf(strs[5]);
                        li.beforestop = Integer.valueOf(strs[3]);
                        li.beforeteg = strs[4].equals("t")?true:false;
                        li.afterteg = strs[6].equals("t")?true:false;
                        li.earlytime = Integer.valueOf(strs[7]);
                        switch (strs[8]) {
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
                        lis.add(li);
                        //progressDialog.setProgress(//progressDialog.getProgress() + 1);
                    }
                    ((Button)(findViewById(R.id.btn_save))).setEnabled(true);
                    ((Button)(findViewById(R.id.btn_out))).setEnabled(true);
                    selectA = -1;
                    selectL = -1;
                    if(select)
                    {
                        select = false;
                        ((Button)(findViewById(R.id.btn_up))).setBackgroundTintList(ColorStateList.valueOf(
                                ContextCompat.getColor(this,R.color.CRH)));
                        ((Button)(findViewById(R.id.btn_down))).setBackgroundTintList(ColorStateList.valueOf(
                                ContextCompat.getColor(this,R.color.E_Train)));
                    }
                    Update();
                    SetTime();
                    ((EditText)(findViewById(R.id.et_before))).setText(Share.stations.get(selectB).name);
                    ((Button)(findViewById(R.id.sp_lineName))).setText("");
                    ((Button)(findViewById(R.id.sp_after))).setText("");
                    ((Button)(findViewById(R.id.btn_add))).setEnabled(false);
                    ((Button)(findViewById(R.id.sp_after))).setEnabled(false);
                    ((EditText)(findViewById(R.id.et_before))).setEnabled(false);
                    ((EditText)(findViewById(R.id.sw_no_before))).setText(Integer.toString(lis.get(lis.size() - 1).afterstop));
                    ((EditText)(findViewById(R.id.sw_no_before))).setEnabled(false);
                    ((EditText)(findViewById(R.id.sw_no_after))).setText("2");
                    ((Switch)findViewById(R.id.sw_before_tst)).setChecked(((Switch)findViewById(R.id.sw_after_tst)).isChecked());
                    ((Switch)findViewById(R.id.sw_before_tst)).setEnabled(false);
                    ((Switch)findViewById(R.id.sw_after_tst)).setChecked(false);
                    ((RadioButton)findViewById(R.id.rb_slow)).setChecked(true);
                    //progressDialog.setProgress(MAX_PROGRESS);
                    //progressDialog.cancel();
            }
            catch (Exception e) {
                new AlertDialog.Builder(this)
                        .setTitle("提示")
                        .setMessage("打开失败")
                        .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {  }
                        }).create().show();
            }
        }
    }
    public String ToTime(int min) {
        String tmp1,tmp3;
        StringBuilder sb = new StringBuilder();
        int minute,hourOfDay;
        minute = min % 60;
        hourOfDay = min / 60;
        hourOfDay = hourOfDay % 24;
        tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
        tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
        sb.append(tmp3);
        sb.append(":");
        sb.append(tmp1);
        return sb.toString();
    }
    public String ToLCDTime(int min) {
        String tmp1, tmp3;
        StringBuilder sb = new StringBuilder();
        int minute, hourOfDay;
        minute = min % 60;
        hourOfDay = min / 60;
        hourOfDay = hourOfDay % 24;
        tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
        tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
        sb.append(tmp3);
        sb.append(tmp1);
        return sb.toString();
    }
    public void Out(AtomicBoolean isshow, Intent data) {
        Uri uri = data.getData();
        StringBuilder sb = new StringBuilder();
        JSONArray jArray,j2;
        String name;
        JSONObject tmp;
        boolean b = false;
        boolean b1 = false;
        int j = 0;
        j2 = new JSONArray();
        //progressDialog.setProgress(//progressDialog.getProgress() + 1);
        sb.append("{\"fileVersion\":1,\"cityName\":\"中华人民共和国\",\"lineName\":\"");
        sb.append(((EditText)findViewById(R.id.et_trainNumber)).getText());
        sb.append("\",\"lineColor\":\"#");
        EMU e2 = null;
        for(EMU e : Share.emus) {
            if(e.name.equals(train)) {
                e2 = e;
                break;
            }
        }
        if(e2.speed > 310)
            sb.append("CC0000");
        else if(e2.speed > 290)
            sb.append("FF3300");
        else if(e2.speed > 230)
            sb.append("FF6600");
        else if(e2.speed > 190)
            sb.append("FF9900");
        else if(e2.speed > 150)
            sb.append("CBCB00");
        else
            sb.append("33CC33");
        sb.append("\",\"remark\":\"采用");
        sb.append(train);
        if(!this.ver.contains("&"))
            sb.append("(" + this.ver + ")");
        if(((Switch)findViewById(R.id.sw_double)).isChecked())
            sb.append("重联");
        if(((!this.ver.endsWith("型"))&&(!this.ver.endsWith("版"))) || (((Switch)findViewById(R.id.sw_double)).isChecked()))
            sb.append("型");
        sb.append("列车\",\"lineType\":1");
        sb.append(",\"company\":\"");
        sb.append(rcps[selectC]);
        sb.append("\",\"route\":{\"up\":[");
        //progressDialog.setProgress(//progressDialog.getProgress() + 1);
        int i = 0;
        int k = 0;
        int tt = 0;
        try {
            for (i = 0; i < lis.size(); i++) {
                b1 = false;
                jArray = Share.rails.get(lis.get(i).line).locations;
                //j = 0;
                b = false;
                for (j = 0; j < jArray.length(); ) {
                    if (jArray.getJSONObject(j).getString("name").equals(Share.stations.get(lis.get(i).before).name))
                        b = true;
                    tmp = jArray.getJSONObject(j);
                    if (jArray.getJSONObject(j).getString("name").equals(Share.stations.get(lis.get(i).after).name)) {
                        if (b == false) {
                            b1 = true;
                            jArray = new JSONArray();
                            for (int t =  Share.rails.get(lis.get(i).line).locations.length()-1;t >= 0;t--) {
                                jArray.put(Share.rails.get(lis.get(i).line).locations.getJSONObject(t));
                            }
                                /*boolean c = false;
                                for (k = 0; k < jArray.length(); ) {
                                    if (!jArray.getJSONObject(k).getString("name").equals(Share.stations.get(lis.get(i).before).name))
                                        c = true;
                                    tmp = jArray.getJSONObject(k);
                                    if (jArray.getJSONObject(k).getString("name").equals(Share.stations.get(lis.get(i).after).name)) {
                                        sb.append(',');
                                        if (i == lis.size() - 1)
                                            sb.append(tmp.toString());
                                        break;
                                    }
                                    if (c) {
                                        if (!jArray.getJSONObject(k).getString("name").equals(Share.stations.get(lis.get(i).before).name))
                                            tmp.put("type", "waypoint");
                                        sb.append(tmp.toString());
                                        if (!jArray.getJSONObject(k+1).getString("name").equals(Share.stations.get(lis.get(i).after).name))
                                            sb.append(",");
                                    }
                                    k++;
                                }*/
                            j = 0;
                            b = false;
                            continue;
                        }
                        sb.append(',');
                        if (i == lis.size() - 1) {
                            if(isshow.get())
                                tmp.put("name", jArray.getJSONObject(j).getString("name") + " " + ToTime(uptimes.get(tt).arriveTime) + "到");
                            j2.put(tmp);
                            sb.append(tmp.toString());
                        }
                        break;
                    }
                    if (b) {
                        if ((!jArray.getJSONObject(j).getString("name").equals(Share.stations.get(lis.get(i).before).name))
                                || (jArray.getJSONObject(j).getString("name").equals(Share.stations.get(lis.get(i).before).name)
                                && (lis.get(i).beforestop == 0)))
                            tmp.put("type", "waypoint");
                        else if(isshow.get()) {
                            if (i == 0)
                                tmp.put("name", jArray.getJSONObject(j).getString("name") + " " + ToTime(uptimes.get(tt).deparTime) + "开");
                            else
                                tmp.put("name", jArray.getJSONObject(j).getString("name") + " " + ToTime(uptimes.get(tt).arriveTime) + "到 " + ToTime(uptimes.get(tt).deparTime) + "开" + (uptimes.get(tt).teg?"技停":""));
                            tt++;
                        }
                        sb.append(tmp.toString());
                        j2.put(tmp);
                        if (!jArray.getJSONObject(j+1).getString("name").equals(Share.stations.get(lis.get(i).after).name))
                            sb.append(",");
                    }
                    j++;
                }
                if (b1) {
                    b1 = false;
                }
                //progressDialog.setProgress(//progressDialog.getProgress() + 1);
            }
            sb.append("],\"down\":[");
            tt = 0;
            //if(!isshow.get()) {
            for (int t = j2.length() - 1; t >= 0; t--) {
                if(isshow.get() && j2.getJSONObject(t).getString("type").equals("station")) {
                    name = j2.getJSONObject(t).getString("name").split(" ")[0];
                    if (t == j2.length() - 1) {
                        j2.getJSONObject(t).put("name", name + " " + ToTime(downtimes.get(tt).deparTime) + "开");
                    }
                    else if (t == 0) {
                        j2.getJSONObject(t).put("name", name + " " + ToTime(downtimes.get(tt).arriveTime) + "到");
                    }
                    else {
                        j2.getJSONObject(t).put("name", name + " " + ToTime(downtimes.get(tt).arriveTime) + "到 " + ToTime(downtimes.get(tt).deparTime) + "开" + (downtimes.get(tt).teg?"技停":""));
                    }
                    tt++;
                }
                sb.append(j2.get(t));
                if (t > 0)
                    sb.append(',');
            }
            //}
            sb.append("]},\"serviceTime\":{\"up\":\"");
            sb.append(upTime.replace("\n", "\\n"));
            sb.append("\",\"down\":\"");
            sb.append(downTime.replace("\n", "\\n"));
            sb.append("\"},\"fare\":{\"strategy\":\"multilevel\",\"enableRing\":\"0\",\"desc\":\"在“票价”面板设置\",\"single\":{\"price\":\"1.00\"},\"multilevel\":{\"startPrice\":\"0.00\",\"startingDistance\":\"0\",\"magnification\":\"0.35\",\"magnificationAttenuation\":\"0.00\",\"increaseBase\":\"0.001\",\"maxPrice\":\"Infinity\"},\"text\":{\"text\":\"\"},\"customize\":{\"formula\":\"0.2*distance\"}}}");
            //progressDialog.setProgress(MAX_PROGRESS);
            //java.io.File.separatorChar+"storage"+java.io.File.separatorChar+"Download"
            File.WriteAllText(uri.getPath().replace("/tree/primary:","/storage/emulated/0/")
                    +java.io.File.separatorChar+((EditText)findViewById(R.id.et_trainNumber)).getText()
                    .toString().replace("/","_")+".bll", sb.toString());
            //progressDialog.cancel();
            new AlertDialog.Builder(this)
                    .setTitle("提示")
                    .setMessage("导出成功")
                    .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {  }
                    }).create().show();
        }
        catch (Exception e) {
            //progressDialog.cancel();
            new AlertDialog.Builder(this)
                    .setTitle("提示")
                    .setMessage("导出失败")
                    .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {  }
                    }).create().show();
        }
    }
    public void OutFile(View view) {
        if (!Environment.isExternalStorageManager())
            return;
        final Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT_TREE);
        new AlertDialog.Builder(this)
                .setTitle("请选择导出格式")
                .setSingleChoiceItems(new String[] {"bll文件", "车内PIDS文件"}, 0, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        dialog.cancel();
                        if (which == 0)
                            startActivityForResult(intent, 1);
                        else if(which == 1)
                            startActivityForResult(intent, 3);
                    }
                }).create().show();
    }
    public void ChangeRailC(View view) {
        int tmp2 = 0;
        if(selectC > -1) {
            for (String i : rcps) {
                if (rcps[selectC].equals(i))
                    break;
                tmp2++;
            }
        }
        new AlertDialog.Builder(this)
                .setTitle("请选择路局")
                .setSingleChoiceItems(rcps, tmp2, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        int i = which;
                        selectC = i;
                        ((Button)findViewById(R.id.sp_rcp)).setText(rcps[which]);
                        dialog.cancel();
                    }
                }).create().show();
    }
    public String CalcTime(int upmin, int uphour, boolean up) {
        if(this.lis.size() == 0)
            return "";
        List<ListItem> lis = new List<>();
        double ast;
        int speed, max_length, bspeed = 0, aspeed = 0, _speed = 0, m, minute, hourOfDay, m2;
        double length;
        List<String> jArray = new List<>();
        String tmp, tmp1, tmp3;
        List<String> speeds = new List<>();
        List<Double> lengths = new List<>();
        StringBuilder sb = new StringBuilder();
        String[] tmp2;
        List<String> spd = new List<>();
        EMU e2 = null;
        int min = upmin + (60 * uphour);
        TimeTime tttmp = new TimeTime(0,0, false);
        int j = 0;
        boolean b,b1,b2,b3,b4;
        for(EMU e : Share.emus) {
            if(e.name.equals(train)) {
                e2 = e;
                break;
            }
        }
        if(up) {
            lis = this.lis;
            uptimes.clear();
        }
        else {
            ListItem tmp4;
            for (int i = this.lis.size() - 1;i >= 0;i--) {
                tmp4 = this.lis.get(i);
                lis.add(new ListItem(tmp4.after, tmp4.afterstop,
                        tmp4.line, tmp4.mode,
                        tmp4.before, tmp4.beforestop, tmp4.earlytime,
                        tmp4.afterteg, tmp4.beforeteg));/*
                tmp4 = lis.get(i);
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
            downtimes.clear();
        }
        ast = e2.ast;
        speed = e2.speed;
        max_length = Share.stations.get(lis.get(0).before).name.length();
        for(int i = 0; i < lis.size(); i++) {
            if ((!Share.stations.get(lis.get(i).before).name.endsWith("线路所")) && (lis.get(i).beforestop != 0) && (!lis.get(i).beforeteg)) {
                if (max_length < Share.stations.get(lis.get(i).before).name.length())
                    max_length = Share.stations.get(lis.get(i).before).name.length();
            }
        }
        if ((!Share.stations.get(lis.get(lis.size() - 1).after).name.endsWith("线路所"))
                && (lis.get(lis.size() - 1).afterstop != 0) && (!lis.get(lis.size() - 1).afterteg)) {
            if (max_length < Share.stations.get(lis.get(lis.size() - 1).after).name.length())
                max_length = Share.stations.get(lis.get(lis.size() - 1).after).name.length();
        }
        if (lis.get(0).beforestop != 0) {
            if (Share.stations.get(lis.get(0).before).name.length() < max_length) {
                sb.append("_");
                for (int i = 1; i < getLen(max_length, Share.stations.get(lis.get(0).before).name.length())[0]; i++)
                    sb.append(" ");
                sb.append(Share.stations.get(lis.get(0).before).name);
                for (int i = 0; i <= getLen(max_length, Share.stations.get(lis.get(0).before).name.length())[1]; i++)
                    sb.append(" ");
            }
            else
                sb.append(Share.stations.get(lis.get(0).before).name);
            sb.append(" --:--  ");
            minute = upmin;
            hourOfDay = uphour%24;
            tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
            tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
            sb.append(tmp3);
            sb.append(":");
            sb.append(tmp1);
            sb.append(" --\n");
            m2 = min;
            tttmp.arriveTime = -1;
            tttmp.deparTime = m2;
            tttmp.teg = false;
            if(up)
                uptimes.add(tttmp);
            else
                downtimes.add(tttmp);
        }
        //开始计算
        try {
            for (int i = 0; i < lis.size(); i++) {
                speeds.clear();
                spd.clear();
                tmp2 = Share.rails.get(lis.get(i).line).speeds.split("\n");
                for (j = 0; j < tmp2.length; j++) {
                    if ((!tmp2[j].startsWith("atp"))
                            && (!tmp2[j].startsWith("ms"))
                            && (!tmp2[j].startsWith("ss"))
                            && (!tmp2[j].startsWith("1")) && (!tmp2[j].startsWith("2")) && (!tmp2[j].startsWith("3"))
                            && (!tmp2[j].startsWith("4")) && (!tmp2[j].startsWith("5")) && (!tmp2[j].startsWith("6"))
                            && (!tmp2[j].startsWith("7")) && (!tmp2[j].startsWith("8")) && (!tmp2[j].startsWith("9")))
                        speeds.add(tmp2[j]);
                    spd.add(tmp2[j]);
                }
                b1 = false;
                jArray = Share.rails.get(lis.get(i).line).stations;
                lengths = Share.rails.get(lis.get(i).line).lengths;
                //j = 0;
                b = false;
                length = 0;
                for (j = 0; j < jArray.size(); ) {
                    if (jArray.get(j).equals(Share.stations.get(lis.get(i).before).name)) {
                        if ((i > 0) && !(lis.get(i).beforestop == 0)) {
                            if (Share.stations.get(lis.get(i).before).name.length() < max_length) {
                                for (int v = 0; v < getLen(max_length, Share.stations.get(lis.get(i).before).name.length())[0]; v++)
                                    sb.append(" ");
                                sb.append(Share.stations.get(lis.get(i).before).name);
                                for (int v = 0; v <= getLen(max_length, Share.stations.get(lis.get(i).before).name.length())[1]; v++)
                                    sb.append(" ");
                            }
                            else
                                sb.append(Share.stations.get(lis.get(i).before).name);
                            tttmp = new TimeTime(0,0,false);
                            minute = min % 60;
                            hourOfDay = min / 60;
                            hourOfDay = hourOfDay % 24;
                            sb.append(" ");
                            tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
                            tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
                            sb.append(tmp3);
                            sb.append(":");
                            sb.append(tmp1);
                            m2 = min;
                            tttmp.arriveTime = m2;
                            min += lis.get(i).beforestop;
                            minute = min % 60;
                            hourOfDay = min / 60;
                            hourOfDay = hourOfDay % 24;
                            tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
                            tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
                            sb.append(" ");
                            sb.append(tmp3);
                            sb.append(":");
                            sb.append(tmp1);
                            sb.append(" ");
                            m2 = min;
                            tttmp.deparTime = m2;
                            tttmp.teg = lis.get(i).beforeteg;
                            if(lis.get(i).beforeteg)
                                sb.append("技停\n");
                            else {
                                sb.append(lis.get(i).beforestop);
                                sb.append("分\n");
                            }
                            if(up)
                                uptimes.add(tttmp);
                            else
                                downtimes.add(tttmp);
                        }
                        b = true;
                        length = 0;
                    }
                    tmp = jArray.get(j);
                    if (jArray.get(j).equals(Share.stations.get(lis.get(i).after).name)) {
                        if (b == false) {
                            b1 = true;
                            jArray = new List<>();
                            spd = new List<>();
                            lengths = new List<>();
                            speeds = new List<>();
                            length = 0;
                            for (int t = Share.rails.get(lis.get(i).line).stations.size() - 1; t >= 0; t--)
                                jArray.add(Share.rails.get(lis.get(i).line).stations.get(t));
                            tmp2 = Share.rails.get(lis.get(i).line).speeds.split("\n");
                            for (int t = tmp2.length - 1; t >= 0; t--) {
                                if ((!tmp2[t].startsWith("atp"))
                                        && (!tmp2[t].startsWith("ms"))
                                        && (!tmp2[t].startsWith("ss"))
                                        && (!tmp2[t].startsWith("1")) && (!tmp2[t].startsWith("2")) && (!tmp2[t].startsWith("3"))
                                        && (!tmp2[t].startsWith("4")) && (!tmp2[t].startsWith("5")) && (!tmp2[t].startsWith("6"))
                                        && (!tmp2[t].startsWith("7")) && (!tmp2[t].startsWith("8")) && (!tmp2[t].startsWith("9")))
                                    speeds.add(tmp2[t]);
                                spd.add(tmp2[t]);
                            }
                            for (int t = Share.rails.get(lis.get(i).line).lengths.size() - 1; t >= 0; t--)
                                lengths.add(Share.rails.get(lis.get(i).line).lengths.get(t));
                            j = 0;
                            b = false;
                            continue;
                        }
                        for (String stat:speeds) {
                            b3 = false;
                            m = -1;
                            for (int l = j; l < jArray.size(); l++) {
                                if (jArray.get(l).equals(stat)) {
                                    m = l;
                                    b3 = true;
                                    break;
                                }
                            }
                            if(jArray.get(j).equals(stat) || b3) {
                                b2 = false;
                                aspeed = 0;
                                for(int k = 0; k < spd.size(); k++) {
                                    if(spd.get(k).equals(stat))
                                        break;
                                    if(spd.get(k).startsWith("1")||spd.get(k).startsWith("2")||
                                            spd.get(k).startsWith("3")||spd.get(k).startsWith("4")||
                                            spd.get(k).startsWith("5")||spd.get(k).startsWith("6")||
                                            spd.get(k).startsWith("7")||spd.get(k).startsWith("8")||
                                            spd.get(k).startsWith("9")||spd.get(k).startsWith("0"))
                                        _speed = Integer.valueOf(spd.get(k));
                                    else if (spd.get(k).startsWith("ss") && (lis.get(i).mode == RunMode.Express))
                                        _speed = Integer.valueOf(spd.get(k).substring(3));
                                    else if (spd.get(k).startsWith("ms") && (lis.get(i).mode != RunMode.Express) && (b2 != true))
                                        _speed = Integer.valueOf(spd.get(k).substring(3));
                                    else if (spd.get(k).startsWith("atp") && (lis.get(i).mode == RunMode.MaxSpeed)) {
                                        _speed = Integer.valueOf(spd.get(k).substring(4));
                                        b2 = true;
                                    }
                                }
                                b2 = false;
                                if(_speed > speed) {
                                    if(speed == 310)
                                        speed += 5;
                                    switch (lis.get(i).mode) {
                                        case ATP:
                                            _speed = speed - 6;
                                            break;
                                        case Express:
                                            _speed = speed - 15; break;
                                        case MaxSpeed:
                                            _speed = speed - 10;
                                            break;
                                    }
                                }
                                if ((((Switch)findViewById(R.id.sw_no350mode)).isChecked()) && (_speed > 310))
                                    _speed = 305;
                                //找到下条线路限速
                                /*if (i < lis.size() - 1) {
                                    if (lis.get(i).afterstop == 0) {
                                        c1 = false;
                                        c5 = false;
                                        tpd = new List<>();
                                        tpeeds = new List<>();
                                        ump2 = Share.rails.get(lis.get(i + 1).line).speeds.split("\n");
                                        for (int s = 0; s < ump2.length; s++) {
                                            if ((!ump2[s].startsWith("atp"))
                                                    && (!ump2[s].startsWith("ms"))
                                                    && (!ump2[s].startsWith("ss"))
                                                    && (!ump2[s].startsWith("1")) && (!ump2[s].startsWith("2")) && (!ump2[s].startsWith("3"))
                                                    && (!ump2[s].startsWith("4")) && (!ump2[s].startsWith("5")) && (!ump2[s].startsWith("6"))
                                                    && (!ump2[s].startsWith("7")) && (!ump2[s].startsWith("8")) && (!ump2[s].startsWith("9")))
                                                tpeeds.add(ump2[s]);
                                            tpd.add(ump2[s]);
                                        }
                                        b1 = false;
                                        c = false;
                                        kArray = Share.rails.get(lis.get(i + 1).line).stations;
                                        mengths = Share.rails.get(lis.get(i + 1).line).lengths;
                                        for (int n = 0; n < Share.rails.get(lis.get(i + 1).line).stations.size(); n++) {
                                            if (kArray.get(n).equals(Share.stations.get(lis.get(i + 1).after).name)) {
                                                if (c == false) {
                                                    c1 = true;
                                                    kArray = new List<>();
                                                    tpd = new List<>();
                                                    mengths = new List<>();
                                                    tpeeds = new List<>();
                                                    for (int t = Share.rails.get(lis.get(i + 1).line).stations.size() - 1; t >= 0; t--)
                                                        kArray.add(Share.rails.get(lis.get(i + 1).line).stations.get(t));
                                                    ump2 = Share.rails.get(lis.get(i + 1).line).speeds.split("\n");
                                                    for (int t = ump2.length - 1; t >= 0; t--) {
                                                        if ((!ump2[t].startsWith("atp"))
                                                                && (!ump2[t].startsWith("ms"))
                                                                && (!ump2[t].startsWith("ss"))
                                                                && (!ump2[t].startsWith("1")) && (!ump2[t].startsWith("2")) && (!ump2[t].startsWith("3"))
                                                                && (!ump2[t].startsWith("4")) && (!ump2[t].startsWith("5")) && (!ump2[t].startsWith("6"))
                                                                && (!ump2[t].startsWith("7")) && (!ump2[t].startsWith("8")) && (!ump2[t].startsWith("9")))
                                                            tpeeds.add(ump2[t]);
                                                        tpd.add(ump2[t]);
                                                    }
                                                    for (int t = Share.rails.get(lis.get(i + 1).line).lengths.size() - 1; t >= 0; t--)
                                                        mengths.add(Share.rails.get(lis.get(i + 1).line).lengths.get(t));
                                                    n = 0;
                                                    c = false;
                                                    continue;
                                                }
                                                else
                                                    break;
                                            }
                                            if (kArray.get(n).equals(Share.stations.get(lis.get(i + 1).before).name))
                                                c = true;
                                            if (c) {
                                                for (String ttat:tpeeds) {
                                                    c3 = false;
                                                    o = -1;
                                                    for (int l = n; l < kArray.size(); l++) {
                                                        if (kArray.get(l).equals(ttat)) {
                                                            o = l;
                                                            c3 = true;
                                                            break;
                                                        }
                                                    }
                                                    if((kArray.get(n).equals(ttat) || c3) && (!kArray.get(n).equals(Share.stations.get(lis.get(i + 1).before).name))) {
                                                        c2 = false;
                                                        c4 = false;
                                                        for(int k = 0; k < tpd.size(); k++) {
                                                            if(tpd.get(k).equals(ttat))
                                                                break;
                                                            if(tpd.get(k).startsWith("1")||tpd.get(k).startsWith("2")||
                                                                    tpd.get(k).startsWith("3")||tpd.get(k).startsWith("4")||
                                                                    tpd.get(k).startsWith("5")||tpd.get(k).startsWith("6")||
                                                                    tpd.get(k).startsWith("7")||tpd.get(k).startsWith("8")||
                                                                    tpd.get(k).startsWith("9")||tpd.get(k).startsWith("0"))
                                                                aspeed = Integer.valueOf(tpd.get(k));
                                                            else if (tpd.get(k).startsWith("ss") && (lis.get(i).mode == RunMode.Express))
                                                                aspeed = Integer.valueOf(tpd.get(k).substring(3));
                                                            else if (tpd.get(k).startsWith("ms") && (lis.get(i).mode != RunMode.Express) && (c4 != true)) {
                                                                if((lis.get(i).mode != RunMode.Express))
                                                                    aspeed = Integer.valueOf(tpd.get(k).substring(3));
                                                            }
                                                            else if (tpd.get(k).startsWith("atp") && (lis.get(i).mode == RunMode.MaxSpeed)) {
                                                                aspeed = Integer.valueOf(tpd.get(k).substring(4));
                                                                c4 = true;
                                                            }
                                                        }
                                                        c2 = false;
                                                        c4 = false;
                                                        if(aspeed > speed) {
                                                            if(speed == 310)
                                                                speed += 5;
                                                            switch (lis.get(i).mode) {
                                                                case ATP:
                                                                    aspeed = speed - 6;
                                                                    break;
                                                                case Express:
                                                                    aspeed = speed - 15;
                                                                case MaxSpeed:
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
                                min += Calc(length, ast, bspeed, aspeed, _speed, lis.get(i).earlytime);
                                bspeed = _speed;
                                if (aspeed == 0) bspeed = 0;
                                length = 0;
                                break;
                            }
                        }
                        if (i == lis.size() - 1) {
                            if(lis.get(i).afterstop != 0) {
                                if (Share.stations.get(lis.get(i).after).name.length() < max_length) {
                                    for (int v = 0; v < getLen(max_length, Share.stations.get(lis.get(i).after).name.length())[0]; v++)
                                        sb.append(" ");
                                    sb.append(Share.stations.get(lis.get(i).after).name);
                                    for (int v = 0; v <= getLen(max_length, Share.stations.get(lis.get(i).after).name.length())[1]; v++)
                                        sb.append(" ");
                                }
                                else
                                    sb.append(Share.stations.get(lis.get(i).after).name);
                                minute = min % 60;
                                hourOfDay = min / 60;
                                hourOfDay = hourOfDay % 24;
                                sb.append(" ");
                                tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
                                tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
                                sb.append(tmp3);
                                sb.append(":");
                                sb.append(tmp1);
                                sb.append("  --:--   --");
                                tttmp = new TimeTime(0,0,false);
                                m2 = min;
                                tttmp.teg = false;
                                tttmp.deparTime = -1;
                                tttmp.arriveTime = m2;
                                if(up)
                                    uptimes.add(tttmp);
                                else
                                    downtimes.add(tttmp);
                            }
                            b = true;
                            length = 0;
                        }
                        //else
                        //    sb.append("\n");
                        break;
                    }
                    if (b) {
                        //if(!b1 && j > 0)
                        //    length += lengths.get(j-1);
                        for (String stat:speeds) {
                            if(jArray.get(j).equals(stat) && (!jArray.get(j).equals(Share.stations.get(lis.get(i).before).name))) {
                                b2 = false;
                                b4 = false;
                                aspeed = 0;
                                for(int k = 0; k < spd.size(); k++) {
                                    if(spd.get(k).equals(stat)) {
                                        b2 = true;
                                        b4 = false;
                                    }
                                    if(spd.get(k).startsWith("1")||spd.get(k).startsWith("2")||
                                            spd.get(k).startsWith("3")||spd.get(k).startsWith("4")||
                                            spd.get(k).startsWith("5")||spd.get(k).startsWith("6")||
                                            spd.get(k).startsWith("7")||spd.get(k).startsWith("8")||
                                            spd.get(k).startsWith("9")||spd.get(k).startsWith("0")
                                    ) {
                                        if (b2) {
                                            aspeed = Integer.valueOf(spd.get(k));
                                            break;
                                        }
                                        else
                                            _speed = Integer.valueOf(spd.get(k));
                                    }
                                    else if (spd.get(k).startsWith("ss") && (lis.get(i).mode == RunMode.Express)) {
                                        if (b2) {
                                            aspeed = Integer.valueOf(spd.get(k).substring(3));
                                            break;
                                        }
                                        else
                                            _speed = Integer.valueOf(spd.get(k).substring(3));
                                    }
                                    else if (spd.get(k).startsWith("ms") && (lis.get(i).mode != RunMode.Express) && (b4 != true)) {
                                        if (b2) {
                                            aspeed = Integer.valueOf(spd.get(k).substring(3));
                                            if((lis.get(i).mode == RunMode.MaxSpeed))
                                                break;
                                        }
                                        else if((lis.get(i).mode != RunMode.Express))
                                            _speed = Integer.valueOf(spd.get(k).substring(3));
                                    }
                                    else if (spd.get(k).startsWith("atp") && (lis.get(i).mode == RunMode.MaxSpeed)) {
                                        b4 = true;
                                        if (b2) {
                                            aspeed = Integer.valueOf(spd.get(k).substring(4));
                                            break;
                                        }
                                        else
                                            _speed = Integer.valueOf(spd.get(k).substring(4));
                                    }
                                    /*else if (lis.get(i).mode == RunMode.MaxSpeed) {
                                        if (b2)
                                            break;
                                    }*/
                                }
                                b2 = false;
                                b4 = false;
                                if(_speed > speed) {
                                    if(speed == 310)
                                        speed += 5;
                                    switch (lis.get(i).mode) {
                                        case ATP:
                                            _speed = speed - 6;
                                            break;
                                        case Express:
                                            _speed = speed - 15;
                                        case MaxSpeed:
                                            _speed = speed - 10;
                                            break;
                                    }
                                }
                                if ((((Switch)findViewById(R.id.sw_no350mode)).isChecked()) && (_speed > 310))
                                    _speed = 305;
                                min += Calc(length, ast, bspeed, aspeed, _speed, 0);
                                bspeed = _speed;
                                if (aspeed == 0) bspeed = 0;
                                length = 0;
                                break;
                            }
                        }
                        if(//b1 &&
                           j < lengths.size())
                            length += lengths.get(j);
                    }// if b==true
                    j++;
                }
                if (b1) {
                    b1 = false;
                }
            }
        }
        catch (Exception e) {  }
        return sb.toString();
    }
    public int Calc(double _long, double ast, int bspeed, int aspeed, int speed, int early) {
        double h,m,s,d,tmp2=0;
        if(bspeed<speed)
            tmp2 = ((speed+1)*(speed/2)/ast/3600)-((bspeed+1)*(bspeed/2)/ast/3600);
        h = _long - tmp2;
        tmp2 = 0.0;
        if(aspeed<speed)
            tmp2 = ((speed+1)*(speed/2)/ast/3600)-((aspeed+1)*(aspeed/2)/ast/3600);
        h = h - tmp2;
        if (h<0) {
            speed = (int)Math.sqrt(Math.pow(0-(bspeed*2)+(aspeed*2), 2)-(4*2*((aspeed*aspeed)+(bspeed*bspeed)-(_long*7200*ast))));
            tmp2 = 0;
            if(bspeed<speed)
                tmp2 = ((speed+1)*(speed/2)/ast/3600)-((bspeed+1)*(bspeed/2)/ast/3600);
            h = _long - tmp2;
            tmp2 = 0.0;
            if(aspeed<speed)
                tmp2 = ((speed+1)*(speed/2)/ast/3600)-((aspeed+1)*(aspeed/2)/ast/3600);
            h = h - tmp2;
        }
        h = h / speed;
        tmp2 = 0;
        if(bspeed<speed)
            tmp2 = (speed-bspeed)/ast/3600.0;
        h = h + tmp2;
        tmp2 = 0;
        if(aspeed<speed)
            tmp2 = (speed-aspeed)/ast/3600.0;
        h = h + tmp2;
        s = h*3600.0;
        s = ceil(s);
        s += 60-(s%60);
        m = ceil(s/60);
        m += early;/*
        s = ((int)s)%60;
        h = floor(m/60);
        m = ((int)m)%60;
        d = floor(h/24);
        h = ((int)h)%24;*/
        return (int)m;
    }
    public int[] getLen(int big, int small) {
        if((((big-small)*3)%2) == 0)
            return new int[]{((big-small)*3)/2, ((big-small)*3)/2};
        else
            return new int[]{(((big-small)*3)+1)/2,((((big-small)*3)+1)/2)-1};
    }
    public void OpenFile(View view) {
        if (Environment.isExternalStorageManager()) {
            Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
            intent.setType("*/*");
            intent.addCategory(Intent.CATEGORY_OPENABLE);
            startActivityForResult(Intent.createChooser(intent, "选择一个文件"), 6);
        }
    }
    public void SaveFile(View view) {
        if (Environment.isExternalStorageManager()) {
            Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT_TREE);
            startActivityForResult(intent, 5);
        }
    }
    public void NewFile(View view) {
        Intent intent = getIntent();
        finish();
        startActivity(intent);
    }
    public List<ListItem> AStar(int start, int end, int bstop, int astop, int delay, RunMode rm, boolean bteg, boolean ateg) {
        List<ListItem> liss = new List<ListItem>();
        dfsis = new List<List<DFSItem>>();
        List<ListItem> r = new List<ListItem>();
        //bool[,,] fake = new bool[Share.stations.size(), Share.rails.size(), Share.stations.size()];
        visit = new List<int[]>();
        int tmp1 = -1;
        int tmp2 = -1;
        int tmp3 = -1;
        num = 0;
        List<Integer> sns = new List<Integer>();
        List<Double> pos = new List<Double>();
        List<Integer> lin = new List<Integer>();
        List<Integer> lin2 = new List<Integer>();
        boolean b = false;
        boolean bIsSmall = false;
        boolean b1 = false;
        boolean b2 = false;
        boolean isstovf = false;
        tmp1 = -1;
        tmp2 = -1;
        double sortmp1 = -1;
        int sortmp2 = -1;
        boolean zc;
        int i;
        int j;
        int k;
        it = new List<DFSItem>();
        try {
            lng = -1;
            lat = -1;
            for (Rail item : Share.rails) {
                for (int i2 = 0; i2 < item.locations.length(); i2++) {
                    if ((!(item.locations.getJSONObject(i2).getString("name")).toLowerCase().startsWith("x")) && ((item.locations.getJSONObject(i2).getString("type")).equals("station"))) {
                        if (((String) item.locations.getJSONObject(i2).getString("name")).toLowerCase().equals(Share.stations.get(end).name)) {
                            lat = item.locations.getJSONObject(i2).getDouble("lat");
                            lng = item.locations.getJSONObject(i2).getDouble("lng");
                            break;
                        }
                    }
                }
                if (lng != -1)
                    break;
            }
            //GC.Collect();
            /*for (int i = 0; i < Share.stations.size(); i++)
                for (int j = 0; j < Share.rails.size(); j++)
                    for (int k = 0; k < Share.stations.size(); k++)
                        visit[i, j, k] = true;
            for (int i = 0; i < Share.stations.size(); i++)
                for (int j = 0; j < Share.rails.size(); j++)
                    for (int k = 0; k < Share.stations.size(); k++)
                        visit[i, j, k] = false;*/
            astar(start, end, 1);
            if (Integer.valueOf(num) > Integer.valueOf(Share.settings.GetE("asnr"))) {
                new AlertDialog.Builder(this)
                        .setTitle("错误")
                        .setMessage("路径过于复杂！")
                        .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {  }
                        }).create().show();
                return new List<ListItem>();
            }
            tmp1 = -1;
            tmp2 = -1;
            tmp3 = -1;
            for (i = 0; i < dfsis.get(0).size(); i++) {
                if (dfsis.get(0).get(i).line == tmp2)
                    liss.get(liss.size() - 1).after = dfsis.get(0).get(i).before;
                else if (i > 0) {
                    if (dfsis.get(0).get(i - 1).line == tmp2)
                        liss.get(liss.size() - 1).after = dfsis.get(0).get(i).before;
                }
                if ((dfsis.get(0).get(i).line > -1) && (dfsis.get(0).get(i).line != tmp2))
                    liss.Add(new ListItem(dfsis.get(0).get(i).before, 0, dfsis.get(0).get(i).line, rm, dfsis.get(0).get(i + 1).before, 0, 0, false, false));
                tmp2 = dfsis.get(0).get(i).line;
            }
            /*for (int i = 0; i < liss.size(); i++)
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
                if((liss[i].line == tmp2) && (i + 1 >= liss.size()))
                    r.Add(new ListItem(tmp1, 0, tmp2, rm, tmp3, 0, delay, false, ateg));
                else if(i + 1 >= liss.size())
                    r.Add(new ListItem(liss[i].before, 0, liss[i].line, rm, liss[i].after, 0, delay, false, ateg));
                if (tmp1 == -1)
                {
                    tmp1 = liss[i].before;
                    tmp2 = liss[i].line;
                }
            }*/
            r = liss;
            //r.Add(new ListItem(tmp1, 0, tmp2, rm, tmp3, 0, delay, false, ateg));
            r.get(0).beforestop = bstop;
            r.get(0).beforeteg = bteg;
            r.get(r.size() - 1).afterstop = astop;
            r.get(r.size() - 1).earlytime = delay;
            r.get(r.size() - 1).afterteg = ateg;
            for (i = 0; i < r.size(); i++)
            {
                if (r.get(i).before == r.get(i).after)
                {
                    r.get(i + 1).beforestop = r.get(i).beforestop;
                    r.remove(i);
                    i = 0;
                }
            }
        }
        catch (Exception e){   }
        return r;
    }
    public void astar(int st, int nd, int th) {
        int tmp1 = -1;
        int tmp2 = -1;
        int tmp3 = -1;
        List<Integer> sns = new List<Integer>();
        List<Double> pos = new List<Double>();
        List<Integer> lin = new List<Integer>();
        List<Integer> lin2 = new List<Integer>();
        boolean b = false;
        boolean bIsSmall = false;
        boolean b1 = false;
        boolean b2 = false;
        boolean isstovf = false;
        tmp1 = -1;
        tmp2 = -1;
        double sortmp1 = -1;
        int sortmp2 = -1;
        boolean zc;
        int i;
        int j;
        int k;
        try
        {
            it.Add(new DFSItem(st, -1));
            if (Integer.valueOf(num) > Integer.valueOf(Share.settings.GetE("asnr")))
                return;
            num++;
            if (st == nd)
            {
                dfsis.Add(it);
            }
            else
            {
                sns = new List<Integer>();
                pos = new List<Double>();
                lin = new List<Integer>();
                lin2 = new List<Integer>();
                b = false;
                bIsSmall = false;
                b1 = false;
                b2 = false;
                tmp1 = -1;
                tmp2 = -1;
                sortmp1 = -1;
                sortmp2 = -1;
                for (i = 0; i < ssns.size(); i++)
                {
                    if (ssns.get(i) == it.get(it.size() - 1).before)
                    {
                        bIsSmall = true;
                        zc = false;
                        for (j = 0; j < sss.get(i).size(); j++)
                        {
                            for (k = 0; k < sss.get(i).get(j).size(); k++)
                            {
                                if ((it.size() < 2) && (lis.size() == 0))
                                {
                                    b1 = true;
                                    break;
                                }
                                else if ((it.size() > 2) && (zc == false))
                                {
                                    if ((sss.get(i).get(j).get(k) == it.get(it.size() - 2).line))
                                    {
                                        zc = true;
                                        b = true;
                                        k = 0;
                                    }
                                }
                                else if ((zc == false) && (it.size() < 2))
                                {
                                    if (sss.get(i).get(j).get(k) == lis.get(lis.size() - 1).line)
                                    {
                                        zc = true;
                                        b = true;
                                        k = 0;
                                    }
                                }
                                if (zc)
                                    sns.Add(sss.get(i).get(j).get(k));
                            }
                            if (b1)
                                break;
                            zc = false;
                        }
                        break;
                    }
                }
                if (bIsSmall == false)
                    sns = Share.stations.get(it.get(it.size() - 1).before).rails;
                for (int item : sns)
                {
                    tmp1 = -1;
                    for (i = 0; i < Share.rails.get(item).stations.size(); i++)
                    {
                        if (Share.rails.get(item).stations.get(i).equals(Share.stations.get(st).name))
                        {
                            tmp1 = i;
                            break;
                        }
                    }
                    if (tmp1 > 0)
                    {
                        for (i = 0; i < Share.rails.get(item).locations.length(); i++)
                        {
                            if (((String)(Share.rails.get(item).locations.getJSONObject(i).getString("name"))).equals(Share.rails.get(item).stations.get(tmp1 - 1)))
                            {
                                pos.Add(GetLength(lng, lat, ((double)(Share.rails.get(item).locations.getJSONObject(i).getDouble("lng"))), ((double)(Share.rails.get(item).locations.getJSONObject(i).getDouble("lat")))));
                                lin2.Add(item);
                                for (j = 0; j < Share.stations.size(); j++)
                                {
                                    for (tmp2 = 1; tmp2 < Share.rails.get(item).stations.size(); tmp2++)
                                    {
                                        //GC.Collect();
                                        if (!Share.rails.get(item).stations.get(tmp1 - tmp2).toLowerCase().startsWith("x"))
                                            break;
                                    }
                                    if (Share.stations.get(j).name.equals(Share.rails.get(item).stations.get(tmp1 - tmp2)))
                                    {
                                        lin.Add(j);
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    if (tmp1 + 1 < Share.rails.get(item).stations.size())
                    {
                        for (i = 0; i < Share.rails.get(item).locations.length(); i++)
                        {
                            if (((String)(Share.rails.get(item).locations.getJSONObject(i).getString("name"))).equals(Share.rails.get(item).stations.get(tmp1 + 1)))
                            {
                                pos.Add(GetLength(lng, lat, ((double)(Share.rails.get(item).locations.getJSONObject(i).getDouble("lng"))), ((double)(Share.rails.get(item).locations.getJSONObject(i).getDouble("lat")))));
                                lin2.Add(item);
                                for (j = 0; j < Share.stations.size(); j++)
                                {
                                    for (tmp2 = 1; tmp2 < Share.rails.get(item).stations.size(); tmp2++)
                                    {
                                        //GC.Collect();
                                        if (!Share.rails.get(item).stations.get(tmp1 + tmp2).toLowerCase().startsWith("x"))
                                            break;
                                    }
                                    if (Share.stations.get(j).name.equals(Share.rails.get(item).stations.get(tmp1 + tmp2)))
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
                for (i = 0; i < pos.size() - 1; i++)
                {
                    for (j = 0; j < pos.size() - 1 - i; j++)
                    {
                        if (pos.get(j) > pos.get(j+1))
                        {
                            sortmp1 = pos.get(j);
                            pos.set(j,pos.get(j+1));
                            pos.set(j+1,sortmp1);
                            sortmp2 = lin.get(j);
                            lin.set(j,lin.get(j+1));
                            lin.set((j+1),sortmp2);
                            sortmp2 = lin2.get(j);
                            lin2.set(j,lin2.get(j+1));
                            lin2.set(j+1,sortmp2);
                        }
                    }
                }
                if (lin.size() == 0)
                {
                    it.remove(it.size() - 1);
                    astar(it.get(it.size() - 1).before, nd, th);
                    if (num > Share.stations.size() * 2)
                        return;
                    if (num > 400)
                        return;
                }
                else if (th - 1 < pos.size())
                {
                    it.get(it.size() - 1).line = lin2.get(th - 1);
                    //String str = Share.stations[lin.get(th - 1)].name;
                    for (int[] item : visit)
                    {
                        if (((item[0] == st) && (item[1] == lin2.get(lin2.size() - 1)) && (item[2] == lin.get(th - 1))) ||
                                ((item[2] == st) && (item[1] == lin2.get(lin2.size() - 1)) && (item[0] == lin.get(th - 1))))
                        {
                            for (i = 1; i < pos.size(); i++)
                            {
                                if (th - 1 + i < pos.size())
                                {
                                    b2 = false;
                                    for (int[] item2 : visit)
                                    {
                                        b2 = b2 || (((item2[0] == st) && (item2[1] == lin2.get(lin2.size() - 1)) && (item2[2] == lin.get(th - 1 + i))) ||
                                                ((item2[2] == st) && (item2[1] == lin2.get(lin2.size() - 1)) && (item2[0] == lin.get(th - 1 + i))));
                                    }
                                    if (!b2)
                                    {
                                        it.get(it.size() - 1).line = lin2.get(th - 1 + i);
                                        visit.Add(new int[]{ st, lin2.get(th - 1 + i), lin.get(th - 1 + i) });
                                        //visit[st, lin2.get(th - 1 + i), lin.get(th - 1 + i)] = true;
                                        //visit[lin.get(th - 1 + i), lin2.get(th - 1 + i), st] = true;
                                        //GC.Collect();
                                        astar(lin.get(th - 1 + i), nd, th);
                                        return;
                                    }
                                }
                                else
                                {
                                    it.remove(it.size() - 1);
                                    //GC.Collect();
                                    astar(it.get(it.size() - 1).before, nd, th);
                                    return;
                                }
                            }
                        }
                    }
                    visit.Add(new int[] { st, lin2.get(th - 1), lin.get(th - 1) });
                    //visit[lin.get(th - 1), lin2.get(th - 1), st] = true;
                    astar(lin.get(th - 1), nd, th);
                    if (num > Share.stations.size() * 2)
                        return;
                    if (num > 400)
                        return;
                }
                else
                {
                    it.get(it.size() - 1).line = lin2.get(lin2.size() - 1);
                    //String str = Share.stations[lin[lin.size() - 1]].name;
                    //|| visit[lin[lin.size() - 1], lin2[lin2.size() - 1], st]
                    for (int[] item : visit)
                    {
                        if (((item[0] == st) && (item[1] == lin2.get(lin2.size() - 1)) && (item[2] == lin.get(lin.size() - 1))) ||
                                ((item[2] == st) && (item[1] == lin2.get(lin2.size() - 1)) && (item[0] == lin.get(lin.size() - 1))))
                        {
                            it.remove(it.size() - 1);
                            astar(it.get(it.size() - 1).before, nd, th);
                            return;
                        }
                    }
                    visit.Add(new int[]{ st, lin2.get(lin2.size() - 1), lin.get(lin.size() - 1) });
                    //visit[lin[lin.size() - 1], lin2[lin2.size() - 1], st] = true;
                    astar(lin.get(lin.size() - 1), nd, th);
                    if (num > Share.stations.size() * 2)
                        return;
                    if (num > 400)
                        return;
                }
            }
        }
        catch (Exception e){  }
    }
    public void SpwanLCD(String path, boolean showdeley) {
        try
        {
            Color line_c;
            EMU e2 = null;
            boolean isSelect1 = false;
            int sn = 0;
            String cpy = ((Button)findViewById(R.id.sp_rcp)).getText().toString();
            String tra = train;
            String[] dats;
            String tn = ((EditText)findViewById(R.id.et_trainNumber)).getText().toString().split(" ")[0];
            List<PIDSInfo> stats = new List<PIDSInfo>();
            final StringBuilder baseData = new StringBuilder();
            JSONArray jArray;
            String tmp114;
            AtomicBoolean isget = new AtomicBoolean(false);
            Handler h = null;
            for (EMU e : Share.emus)
            {
                if (e.name.equals(train))
                {
                    e2 = e;
                    break;
                }
            }
            if (e2.speed > 310)
                line_c = Color.valueOf(Color.argb(255, 204, 0, 0));
            else if (e2.speed > 290)
                line_c = Color.valueOf(Color.argb(255, 255, 51, 0));
            else if (e2.speed > 230)
                line_c = Color.valueOf(Color.argb(255, 255, 102, 0));
            else if (e2.speed > 190)
                line_c = Color.valueOf(Color.argb(255, 255, 153, 0));
            else if (e2.speed > 150)
                line_c = Color.valueOf(Color.argb(255, 203, 203, 0));
            else
                line_c = Color.valueOf(Color.argb(255, 51, 204, 51));
            for (int j = 0; j < lis.size(); j++)
            {
                isSelect1 = false;
                jArray = Share.rails.get(lis.get(j).line).locations;
                for (int k = 0; k < jArray.length(); k++) {
                    if ((jArray.getJSONObject(k).getString("name")).equals(Share.stations.get(lis.get(j).after).name))
                    {
                        if (!isSelect1)
                        {
                            jArray = new JSONArray();
                            for (int l = Share.rails.get(lis.get(j).line).locations.length() - 1; l >= 0; l--)
                            {
                                jArray.put(Share.rails.get(lis.get(j).line).locations.getJSONObject(l));
                            }
                            k = -1;
                            isSelect1 = false;
                            continue;
                        }
                        else
                        {
                            if((!Share.stations.get(lis.get(j).after).name.toLowerCase().startsWith("x")) && (!Share.stations.get(lis.get(j).after).name.toLowerCase().endsWith("线路所")))
                                baseData.append(Share.stations.get(lis.get(j).after).name);
                            if (lis.get(j).afterstop > 0)
                            {
                                baseData.append('|');
                                baseData.append(((uptimes.get(sn).arriveTime == -1) ? ToLCDTime(uptimes.get(sn).deparTime) : ToLCDTime(uptimes.get(sn).arriveTime)));
                                baseData.append(':');
                                if (showdeley)
                                {
                                    isget.set(false);
                                    EditText et = new EditText(this);
                                    et.setInputType(((EditText)findViewById(R.id.sw_no_after)).getInputType());
                                    new AlertDialog.Builder(this)
                                            .setTitle("请输入\"" + Share.stations.get(lis.get(j).after).name + "\"站的正晚点信息\\r\\n早点为负，正点为0，晚点为正")
                                            .setView(et)
                                            .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                                                @Override
                                                public void onClick(DialogInterface dialog, int which) {
                                                    if (et.getText().toString().equals(""))
                                                        et.setText("0");
                                                    baseData.append(et.getText().toString());
                                                    dialog.cancel();
                                                    isget.set(true);
                                                }
                                            })
                                            .create().show();
                                }
                                else
                                {
                                    baseData.append('0');
                                }
                                sn++;
                            }
                            if (j + 1 < jArray.length())
                                baseData.append(',');
                            break;
                        }
                    }
                    if (isSelect1 && ((jArray.getJSONObject(k).getString("type")).equals("station")))
                    {
                        if ((!(jArray.getJSONObject(k).getString("name")).toLowerCase().contains("x")) && (!(jArray.getJSONObject(k).getString("name")).contains("线路所")))
                            baseData.append((jArray.getJSONObject(k).getString("name")));
                        if (j + 1 < jArray.length())
                            baseData.append(',');
                    }
                    if ((jArray.getJSONObject(k).getString("name")).equals(Share.stations.get(lis.get(j).before).name))
                    {
                        isSelect1 = true;
                        if (j == 0)
                        {
                            baseData.append(Share.stations.get(lis.get(j).before).name);
                            if (lis.get(j).beforestop > 0)
                            {
                                baseData.append('|');
                                baseData.append(((uptimes.get(sn).arriveTime == -1) ? ToLCDTime(uptimes.get(sn).deparTime) : ToLCDTime(uptimes.get(sn).arriveTime)));
                                baseData.append(':');
                                if (showdeley)
                                {
                                    isget.set(false);
                                    EditText et = new EditText(this);
                                    et.setInputType(((EditText)findViewById(R.id.sw_no_after)).getInputType());
                                    new AlertDialog.Builder(this)
                                            .setTitle("请输入\"" + Share.stations.get(lis.get(j).before).name + "\"站的正晚点信息\\r\\n早点为负，正点为0，晚点为正")
                                            .setView(et)
                                            .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                                                @Override
                                                public void onClick(DialogInterface dialog, int which) {
                                                    if (et.getText().toString().equals(""))
                                                        et.setText("0");
                                                    baseData.append(et.getText().toString());
                                                    dialog.cancel();
                                                    isget.set(true);
                                                }
                                            })
                                            .create().show();
                                }
                                else
                                {
                                    baseData.append('0');
                                }
                                sn++;
                            }
                            if (j + 1 < jArray.length())
                                baseData.append(',');
                        }
                    }
                }
            }
            if (showdeley) {
                tra = tra + "-";
                isget.set(false);
                EditText et = new EditText(this);
                et.setInputType(((EditText)findViewById(R.id.sw_no_after)).getInputType());
                new AlertDialog.Builder(this)
                        .setTitle("请输入列车编号")
                        .setView(et)
                        .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                            @Override
                            public void onClick(DialogInterface dialog, int which) {
                                if (et.getText().toString().equals(""))
                                    et.setText("5033");
                                dialog.cancel();
                                isget.set(true);
                            }
                        })
                        .create().show();
                tra = tra + (et.getText().toString());
                if(((Switch)findViewById(R.id.sw_double)).isChecked())
                {
                    tra = tra + "&";
                    tra = tra + train;
                    tra = tra + "-";
                    isget.set(false);
                    EditText et2 = new EditText(this);
                    et.setInputType(((EditText)findViewById(R.id.sw_no_after)).getInputType());
                    new AlertDialog.Builder(this)
                            .setTitle("请输入列车编号")
                            .setView(et)
                            .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                                @Override
                                public void onClick(DialogInterface dialog, int which) {
                                    if (et2.getText().toString().equals(""))
                                        et2.setText("5033");
                                    dialog.cancel();
                                    isget.set(true);
                                }
                            })
                            .create().show();
                    tra = tra + (et2.getText().toString());
                }
            }
            tmp114 = baseData.toString();
            baseData.delete(0, baseData.length() - 1);
            while (tmp114.contains(",,"))
            {
                tmp114 = tmp114.replace(",,", ",");
            }
            for (String item : tmp114.split(","))
            {
                if(item.equals(""))
                    continue;
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
            long lcdNum = 1;
            int t = 0, t3 = 0;
            boolean t2, t4 = false;
            List<String> stations = new List<String>();
            Bitmap bmp = null;
            Graphics g = null;
            Pen afterpen = new Pen(line_c, 20.0f);
            Pen nullpen = new Pen(new SolidBrush(line_c));
            Pen backpen = new Pen(new SolidBrush(Color.valueOf(Color.WHITE)));
            Pen beforepen = new Pen(Color.valueOf(Color.argb(255, 153, 153, 153)), 20.0f);
            Pen astpen = new Pen(line_c, 10.0f);
            Pen bstpen = new Pen(Color.valueOf(Color.argb(255, 153, 153, 153)), 10.0f);
            for (PIDSInfo stat : stats)
            {
                //start x95 y170;end x3750 y170
                bmp = Bitmap.createBitmap(3840, 360, Bitmap.Config.ARGB_8888);
                g = Graphics.FromImage(bmp);
                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(0, 0), new Size(bmp.getWidth(), bmp.getHeight())));
                g.FillRectangle(new SolidBrush(line_c), new Rectangle(new Point(0, 0), new Size(bmp.getWidth(), 90)));
                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(1280, 18), new Size(27 * 2, 27 * 2)));
                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(2505, 18), new Size(27 * 2, 27 * 2)));
                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(1307, 18), new Size(2532 - 1307, 27 * 2)));
                sb = new StringBuilder();
                //车次：G6591/4    揭阳→广州东     本站：广州东       终点站：广州东
                sb.append("车次：");
                sb.append(tn);
                sb.append("    ");
                sb.append(stats.get(0).name);
                sb.append("→");
                sb.append(stats.get(stats.size() - 1).name);
                sb.append("     本站：");
                if (stat.isStop)
                    sb.append(stat.name);
                else
                    sb.append("      ");
                if (i == stats.size() - 1)
                    sb.append("       终点站");
                else
                    sb.append("       下一站：");
                for (int j = i + 1; j < stats.size(); j++)
                {
                    if (stats.get(j).isStop)
                    {
                        sb.append(stats.get(j).name);
                        break;
                    }
                }
                t = 0;
                for (int l = 0; l < sb.length(); l++)
                {
                    if (sb.charAt(l) == ' ')
                        t++;
                }
                if ((((sb.length() - (t / 4.0)) * 15)) > ((2532 - 1307) / 2.0))
                {
                    g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(1920 - (((sb.length() - (t / 4)) * 15)) - 27, 18), new Size(27 * 2, 27 * 2)));
                    g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(1920 + (((sb.length() - (t / 4)) * 15)) - 27, 18), new Size(27 * 2, 27 * 2)));
                    g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(1920 - (((sb.length() - (t / 4)) * 15)), 18), new Size((((sb.length() - (t / 4)) * 15)) * 2, 27 * 2)));
                }
                g.DrawString(sb.toString(), new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF(1900.0f - ((sb.length() - (t / 4.0f)) * 20), 10.5f));
                //担当企业 x=655
                sbd = new StringBuilder();
                sbd.append("担当企业：");
                sbd.append(cpy);
                g.DrawString(sbd.toString(), new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.WHITE)), new PointF(655.0f - (sbd.length() * 20.0f), 10.5f));
                //本务 x=3070
                sbb = new StringBuilder();
                sbb.append("本务：");
                sbb.append(tra.toUpperCase());
                g.DrawString(sbb.toString(), new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.WHITE)), new PointF(3070.0f - (sbb.length() * 20.0f), 10.5f));
                if ((i + 26 >= stats.size() - 1) && (i - 26 <= 0)) {
                    g.DrawLine(afterpen, new Point(95, 215), new Point(3750, 215));
                    stations.clear();
                    for (PIDSInfo stat2 : stats)
                    {
                        stations.Add(stat2.name);
                    }
                    if (stations.indexOf(stat.name) > 0)
                        g.DrawLine(beforepen, new Point(95, 215), new Point((3655 / (stats.size() - 1) * (stations.indexOf(stat.name))) + 95, 215));
                    for (PIDSInfo stat2 : stats)
                    {
                        if (stat2.name.equals(stat.name))
                            break;
                        if (stat2.isStop)
                        {
                            g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                        }
                        else
                        {
                            g.DrawEllipse(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45, 13, 13);//左上
                            g.DrawEllipse(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 + 26, 150 + 45, 13, 13);//右上
                            g.DrawEllipse(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 + 26, 173 + 45, 13, 13);//右下
                            g.DrawEllipse(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 173 + 45, 13, 13);//左下
                            g.DrawLine(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45 + 6, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45 + 6 + 26);
                            g.DrawLine(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 0 + 45, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                            g.DrawLine(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                            g.DrawLine(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                            g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                        }
                        if (stat2.isStop)
                        {
                            g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95.0f - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                            g.DrawString(stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95.0f - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                        }
                    }
                    t2 = false;
                    for (PIDSInfo stat2 : stats)
                    {
                        if (stat2.name.equals(stat.name))
                            t2 = true;
                        if (t2)
                        {
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                g.DrawEllipse(astpen, new Rectangle(new Point((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            }
                            else
                            {
                                g.DrawEllipse(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45, 13, 13);//左上
                                g.DrawEllipse(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 + 26, 150 + 45, 13, 13);//右上
                                g.DrawEllipse(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 + 26, 173 + 45, 13, 13);//右下
                                g.DrawEllipse(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 173 + 45, 13, 13);//左下
                                g.DrawLine(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45 + 6, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45 + 6 + 26);
                                g.DrawLine(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 0 + 45, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                g.DrawLine(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                g.DrawLine(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95.0f - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                                if (stat2.delayTime > 0)
                                    g.DrawString("+" + Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.RED)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95.0f - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                                else if (stat2.delayTime == 0)
                                    g.DrawString(Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95.0f - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                                else if (stat2.delayTime < 0)
                                    g.DrawString(Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.argb(255,0,100,0))), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95.0f - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                            }
                            g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.WHITE)), 3655.0f / (stats.size() - 1) * (stations.indexOf(stat2.name)) + (3655.0f / (stats.size() - 1) / 2.0f) + 70.0f, 170 + 45 - 10);
                        }
                    }
                    for (int k = 0; k < stats.size(); k++)
                    {
                        g.DrawString(stats.get(k).name, new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stats.get(k).name))) + 95.0f - ((stats.get(k).name.length() / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                    }
                }
                else if (i >= stats.size() - 1) {
                    g.DrawLine(afterpen, new Point(0, 215), new Point(3750, 215));
                    g.DrawLine(beforepen, new Point(0, 215), new Point((3655 / 25 * (24)) + 95, 215));
                    t3 = 0;
                    t4 = false;
                    for (PIDSInfo stat2 : stats)
                    {
                        if (t3 == 25)
                            break;
                        if (stat2.name.equals(stats.get(i - 25).name))
                            t4 = true;
                        if (t4)
                        {
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (t3)) + 95 - 27, 188), new Size(54, 54)));
                                g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / 25 * ((t3))) + 95 - 27, 188), new Size(54, 54)));
                                g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (t3)) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                                g.DrawString(stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (t3)) + 95 - 7.5f, 170.0f + 42.0f));
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
                                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / 25 * (t3) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                        }
                        if (t4)
                            t3++;
                    }
                    g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                    g.DrawEllipse(astpen, new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                    if (stat.isStop)
                    {
                        g.DrawString(stat.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * 25) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                        if (stat.delayTime > 0)
                            g.DrawString("+" + Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.RED)), new PointF((3655 / 25 * (t3)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                        else if (stat.delayTime == 0)
                            g.DrawString(Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (t3)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                        else if (stat.delayTime < 0)
                            g.DrawString(Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.argb(255,0,100,0))), new PointF((3655 / 25 * (t3)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                        g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.WHITE)), 3655.0f + (3655.0f / (stats.size() - 1) / 2.0f) - 17.5f, 170 + 45 - 10);
                    }
                    for (int k = i - 25; k < stats.size(); k++)
                    {
                        g.DrawString(stats.get(k).name, new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (k - (i - 25))) + 95 - ((stats.get(k).name.length() / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                    }
                }
                else if (i - 25 <= 0) {
                    g.DrawLine(afterpen, new Point(95, 215), new Point(3840, 215));
                    stations.clear();
                    for (PIDSInfo stat2 : stats)
                    {
                        stations.Add(stat2.name);
                    }
                    if (i > 0)
                        g.DrawLine(beforepen, new Point(95, 215), new Point((3655 / 25 * (stations.indexOf(stat.name))) + 95, 215));
                    for (PIDSInfo stat2 : stats)
                    {
                        if (stat2.name.equals(stat.name))
                            break;
                        if (stat2.isStop)
                        {
                            g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                        }
                        else
                        {
                            g.DrawEllipse(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45, 13, 13);//左上
                            g.DrawEllipse(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 + 26, 150 + 45, 13, 13);//右上
                            g.DrawEllipse(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 + 26, 173 + 45, 13, 13);//右下
                            g.DrawEllipse(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 173 + 45, 13, 13);//左下
                            g.DrawLine(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45 + 6 + 26);
                            g.DrawLine(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                            g.DrawLine(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                            g.DrawLine(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                            g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                        }
                        if (stat2.isStop)
                        {
                            g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                            g.DrawString(stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                        }
                    }
                    t2 = false;
                    for (PIDSInfo stat2 : stats)
                    {
                        if (stat2.name.equals(stat.name))
                            t2 = true;
                        if (t2)
                        {
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                g.DrawEllipse(astpen, new Rectangle(new Point((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            }
                            else
                            {
                                g.DrawEllipse(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45, 13, 13);//左上
                                g.DrawEllipse(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 + 26, 150 + 45, 13, 13);//右上
                                g.DrawEllipse(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 + 26, 173 + 45, 13, 13);//右下
                                g.DrawEllipse(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 173 + 45, 13, 13);//左下
                                g.DrawLine(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45 + 6 + 26);
                                g.DrawLine(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                g.DrawLine(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                g.DrawLine(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.WHITE)), 3655.0f / 25.0f * (stations.indexOf(stat2.name) + 1 + 0.05f), 170 + 45 - 10);
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                                if (stat2.delayTime > 0)
                                    g.DrawString("+" + Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.RED)), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                                else if (stat2.delayTime == 0)
                                    g.DrawString(Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                                else if (stat2.delayTime < 0)
                                    g.DrawString(Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.argb(255,0,100,0))), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));

                            }
                        }
                    }
                    for (int k = 0; k < 26; k++)
                    {
                        g.DrawString(stats.get(k).name, new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (k)) + 95 - ((stats.get(k).name.length() / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                    }
                }
                else {
                    g.DrawLine(afterpen, new Point(0, 215), new Point(3840, 215));
                    g.DrawLine(beforepen, new Point(0, 215), new Point((3655 / 25 * (25)) + 95, 215));
                    t3 = 0;
                    t4 = false;
                    for (PIDSInfo stat2 : stats)
                    {
                        if (t3 == 26)
                            break;
                        if (stat2.name.equals(stats.get(i - 26).name))
                            t4 = true;
                        if (t4)
                        {
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (t3 - 1)) + 95 - 27, 188), new Size(54, 54)));
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
                                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / 25 * (t3 - 1) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * ((t3 - 1))) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                                g.DrawString(stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)),
                                        new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                            }
                        }
                        if (t4)
                            t3++;
                    }
                    if (stats.get(i).isStop)
                    {
                        g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
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
                        g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / 25 * (25) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                    }
                    if (stat.isStop)
                    {
                        g.DrawString(stat.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * 25) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                        if (stat.delayTime > 0)
                            g.DrawString("+" + Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.RED)),
                                    new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                        else if (stat.delayTime == 0)
                            g.DrawString(Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)),
                                    new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                        else if (stat.delayTime < 0)
                            g.DrawString(Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.argb(255,0,100,0))),
                                    new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                    }
                    g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.WHITE)), 3655.0f / 25.0f * (25 + 1 + 0.05f), 170 + 45 - 10);
                    for (int k = i - 25; k < i + 1; k++)
                    {
                        g.DrawString(stats.get(k).name, new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (k - (i - 25))) + 95 - ((stats.get(k).name.length() / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                    }
                }
                File.WriteBitmap(path + Character.toString(java.io.File.separatorChar) + tn.replace("/", "_") + "lcd" + lcdNum + ".png", bmp);
                lcdNum++;
                bmp = Bitmap.createBitmap(3840, 360, Bitmap.Config.ARGB_8888);
                g = Graphics.FromImage(bmp);
                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(0, 0), new Size(bmp.getWidth(), bmp.getHeight())));
                g.FillRectangle(new SolidBrush(line_c), new Rectangle(new Point(0, 0), new Size(bmp.getWidth(), 90)));
                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(1280, 18), new Size(27 * 2, 27 * 2)));
                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(2505, 18), new Size(27 * 2, 27 * 2)));
                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(1307, 18), new Size(2532 - 1307, 27 * 2)));
                sb = new StringBuilder();
                //车次：G6591/4    揭阳→广州东     本站：广州东       终点站：广州东
                sb.append("车次：");
                sb.append(tn);
                sb.append("    ");
                sb.append(stats.get(0).name);
                sb.append("→");
                sb.append(stats.get(stats.size() - 1).name);
                sb.append("     本站：");
                if (stat.isStop)
                    sb.append(stat.name);
                else
                    sb.append("      ");
                if (i == stats.size() - 1)
                    sb.append("       终点站");
                else
                    sb.append("       下一站：");
                for (int j = i + 1; j < stats.size(); j++)
                {
                    if (stats.get(j).isStop)
                    {
                        sb.append(stats.get(j).name);
                        break;
                    }
                }
                t = 0;
                for (int l = 0; l < sb.length(); l++)
                {
                    if (sb.charAt(l) == ' ')
                        t++;
                }
                if((((sb.length() - (t / 4.0)) * 15)) > ((2532 - 1307)/2.0))
                {
                    g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(1920 - (((sb.length() - (t / 4)) * 15)) - 27, 18), new Size(27 * 2, 27 * 2)));
                    g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(1920 + (((sb.length() - (t / 4)) * 15)) - 27, 18), new Size(27 * 2, 27 * 2)));
                    g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point(1920 - (((sb.length() - (t / 4)) * 15)), 18), new Size((((sb.length() - (t / 4)) * 15)) * 2, 27 * 2)));
                }
                g.DrawString(sb.toString(), new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF(1900.0f - ((sb.length() - (t / 4.0f)) * 20), 10.5f));
                //担当企业 x=655
                sbd = new StringBuilder();
                sbd.append("担当企业：");
                sbd.append(cpy);
                g.DrawString(sbd.toString(), new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.WHITE)), new PointF(655.0f - (sbd.length() * 20.0f), 10.5f));
                //本务 x=3070
                sbb = new StringBuilder();
                sbb.append("本务：");
                sbb.append(tra.toUpperCase());
                g.DrawString(sbb.toString(), new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.WHITE)), new PointF(3070.0f - (sbb.length() * 20.0f), 10.5f));
                if ((i + 26 >= stats.size() - 1) && (i - 26 <= 0)) {
                    g.DrawLine(afterpen, new Point(95, 215), new Point(3750, 215));
                    stations.clear();
                    for (PIDSInfo stat2 : stats)
                    {
                        stations.Add(stat2.name);
                    }
                    if(stations.indexOf(stat.name) > 0)
                        g.DrawLine(beforepen, new Point(95, 215), new Point((3655 / (stats.size() - 1) * (stations.indexOf(stat.name))) + 95, 215));
                    for (PIDSInfo stat2 : stats)
                    {
                        if (stat2.isStop)
                        {
                            g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                        }
                        else
                        {
                            g.DrawEllipse(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45, 13, 13);//左上
                            g.DrawEllipse(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 + 26, 150 + 45, 13, 13);//右上
                            g.DrawEllipse(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 + 26, 173 + 45, 13, 13);//右下
                            g.DrawEllipse(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 173 + 45, 13, 13);//左下
                            g.DrawLine(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45 + 6, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45 + 6 + 26);
                            g.DrawLine(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 0 + 45, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                            g.DrawLine(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                            g.DrawLine(bstpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                            g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                        }
                        if (stat2.isStop)
                        {
                            g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                            g.DrawString(stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                        }
                        if (stat2.name.equals(stat.name))
                            break;
                    }
                    t2 = false;
                    for (PIDSInfo stat2 : stats)
                    {
                        if (t2)
                        {
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                g.DrawEllipse(astpen, new Rectangle(new Point((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            }
                            else
                            {
                                g.DrawEllipse(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45, 13, 13);//左上
                                g.DrawEllipse(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 + 26, 150 + 45, 13, 13);//右上
                                g.DrawEllipse(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 + 26, 173 + 45, 13, 13);//右下
                                g.DrawEllipse(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 173 + 45, 13, 13);//左下
                                g.DrawLine(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45 + 6, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26, 150 + 45 + 6 + 26);
                                g.DrawLine(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 0 + 45, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                g.DrawLine(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                g.DrawLine(astpen, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / (stats.size() - 1) * stations.indexOf(stat2.name) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                                if (stat2.delayTime > 0)
                                    g.DrawString("+" + Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.RED)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                                else if (stat2.delayTime == 0)
                                    g.DrawString(Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                                else if (stat2.delayTime < 0)
                                    g.DrawString(Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.argb(255,0,100,0))), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                            }
                            g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.WHITE)), 3655.0f / (stats.size() - 1) * (stations.indexOf(stat2.name) - 1) + (3655.0f / (stats.size() - 1) / 2.0f) + 70.0f, 170 + 45 - 10);
                        }
                        if (stat2.name.equals(stat.name))
                            t2 = true;
                    }
                    for (int k = 0; k < stats.size(); k++)
                    {
                        g.DrawString(stats.get(k).name, new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / (stats.size() - 1) * (stations.indexOf(stats.get(k).name))) + 95 - ((stats.get(k).name.length() / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                    }
                }
                else if (i >= stats.size() - 1) {
                    //g.DrawLine(afterpen, new Point(0, 215), new Point(3750, 215));
                    g.DrawLine(beforepen, new Point(0, 215), new Point((3655 / 25 * 25) + 95, 215));
                    t3 = 0;
                    t4 = false;
                    for (PIDSInfo stat2 : stats)
                    {
                        if (t3 == 25)
                            break;
                        if (stat2.name.equals(stats.get(i - 25).name))
                            t4 = true;
                        if (t4)
                        {
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (t3)) + 95 - 27, 188), new Size(54, 54)));
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
                                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / 25 * (t3) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * ((t3))) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                                g.DrawString(stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)),
                                        new PointF((3655 / 25 * (t3)) + 95 - 7.5f, 170.0f + 42.0f));
                            }
                        }
                        if (t4)
                            t3++;
                    }
                    g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                    g.DrawEllipse(astpen, new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
                    if (stat.isStop)
                    {
                        g.DrawString(stat.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * 25) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                        if (stat.delayTime > 0)
                            g.DrawString("+" + Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.RED)), new PointF((3655 / 25 * 25) + 95 - 17.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                        else if (stat.delayTime == 0)
                            g.DrawString(Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * 25) + 95 - 17.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                        else if (stat.delayTime < 0)
                            g.DrawString(Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.argb(255,0,100,0))), new PointF((3655 / 25 * 25) + 95 - 17.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                    }
                    for (int k = i - 25; k < stats.size(); k++)
                    {
                        g.DrawString(stats.get(k).name, new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (k - (i - 25))) + 95 - ((stats.get(k).name.length() / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                    }
                }
                else if (i - 25 <= 0) {
                    g.DrawLine(afterpen, new Point(95, 215), new Point(3840, 215));
                    stations.clear();
                    for (PIDSInfo stat2 : stats)
                    {
                        stations.Add(stat2.name);
                    }
                    if (i > 0)
                        g.DrawLine(beforepen, new Point(95, 215), new Point((3655 / 25 * (stations.indexOf(stat.name))) + 95, 215));
                    for (PIDSInfo stat2 : stats)
                    {
                        if (stat2.isStop)
                        {
                            g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            g.DrawEllipse(bstpen, new Rectangle(new Point((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                        }
                        else
                        {
                            g.DrawEllipse(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45, 13, 13);//左上
                            g.DrawEllipse(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 + 26, 150 + 45, 13, 13);//右上
                            g.DrawEllipse(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 + 26, 173 + 45, 13, 13);//右下
                            g.DrawEllipse(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 173 + 45, 13, 13);//左下
                            g.DrawLine(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45 + 6 + 26);
                            g.DrawLine(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                            g.DrawLine(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                            g.DrawLine(bstpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                            g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                        }
                        if (stat2.isStop)
                        {
                            g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                            g.DrawString(stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                        }
                        if (stat2.name.equals(stat.name))
                            break;
                    }
                    t2 = false;
                    for (PIDSInfo stat2 : stats)
                    {
                        if (t2)
                        {
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                                g.DrawEllipse(astpen, new Rectangle(new Point((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 27, 188), new Size(54, 54)));
                            }
                            else
                            {
                                g.DrawEllipse(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45, 13, 13);//左上
                                g.DrawEllipse(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 + 26, 150 + 45, 13, 13);//右上
                                g.DrawEllipse(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 + 26, 173 + 45, 13, 13);//右下
                                g.DrawEllipse(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 173 + 45, 13, 13);//左下
                                g.DrawLine(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45 + 6, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26, 150 + 45 + 6 + 26);
                                g.DrawLine(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 0 + 45, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 0);
                                g.DrawLine(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 12 + 26, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 26 + 25 + 3, 150 + 45 + 12 + 26);
                                g.DrawLine(astpen, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6, 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6 + 26 + 26 + 3, 150 + 45 + 6 + 26);
                                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / 25 * (stations.indexOf(stat2.name)) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.WHITE)), 3655.0f / 25.0f * (stations.indexOf(stat2.name) + 1 + 0.05f), 170 + 45 - 10);
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                                if (stat2.delayTime > 0)
                                    g.DrawString("+" + Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.RED)), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                                else if (stat2.delayTime == 0)
                                    g.DrawString(Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                                else if (stat2.delayTime < 0)
                                    g.DrawString(Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.argb(255,0,100,0))), new PointF((3655 / 25 * (stations.indexOf(stat2.name))) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));

                            }
                        }
                        if (stat2.name.equals(stat.name))
                        {
                            t2 = true;
                            g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.WHITE)), 3655.0f / 25.0f * (stations.indexOf(stat2.name) + 1 + 0.05f), 170 + 45 - 10);
                        }
                    }
                    for (int k = 0; k < 26; k++)
                    {
                        g.DrawString(stats.get(k).name, new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (k)) + 95 - ((stats.get(k).name.length() / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                    }
                }
                else {
                    g.DrawLine(afterpen, new Point(0, 215), new Point(3840, 215));
                    g.DrawLine(beforepen, new Point(0, 215), new Point((3655 / 25 * (25)) + 95, 215));
                    t3 = 0;
                    t4 = false;
                    for (PIDSInfo stat2 : stats)
                    {
                        if (t3 == 26)
                            break;
                        if (stat2.name.equals(stats.get(i - 26).name))
                            t4 = true;
                        if (t4)
                        {
                            if (stat2.isStop)
                            {
                                g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (t3 - 1)) + 95 - 27, 188), new Size(54, 54)));
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
                                g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / 25 * (t3 - 1) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                            }
                            if (stat2.isStop)
                            {
                                g.DrawString(stat2.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * ((t3 - 1))) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                                g.DrawString(stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)),
                                        new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat2.delayTime > 0 ? "+" + Short.toString(stat2.delayTime) : Short.toString(stat2.delayTime)).length() / 2), 170.0f + 42.0f));
                            }
                        }
                        if (t4)
                            t3++;
                    }
                    if (stats.get(i).isStop)
                    {
                        g.FillEllipse(new SolidBrush(Color.valueOf(Color.WHITE)), new Rectangle(new Point((3655 / 25 * (25)) + 95 - 27, 188), new Size(54, 54)));
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
                        g.FillRectangle(new SolidBrush(Color.valueOf(Color.WHITE)), 3655 / 25 * (25) + 95 - 26 + 6, 150 + 45 + 6, 26 * 2 + 3, 26);
                    }
                    if (stat.isStop)
                    {
                        g.DrawString(stat.arriveTime, new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * 25) + 95 - 17.5f - 8.25f + 2.5f, 170.0f + 28.0f));
                        if (stat.delayTime > 0)
                            g.DrawString("+" + Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.RED)), new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                        else if (stat.delayTime == 0)
                            g.DrawString(Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                        else if (stat.delayTime < 0)
                            g.DrawString(Short.toString(stat.delayTime), new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.argb(255,0,100,0))), new PointF((3655 / 25 * (t3 - 1)) + 95 - 7.5f - ((stat.delayTime > 0 ? "+" + Short.toString(stat.delayTime) : Short.toString(stat.delayTime)).length() / 2), 170.0f + 42.0f));
                    }
                    g.DrawString("》》》", new Font("微软雅黑", 10), new SolidBrush(Color.valueOf(Color.WHITE)), 3655.0f / 25.0f * (25 + 1 + 0.05f), 170 + 45 - 10);
                    for (int k = i - 25; k < i + 1; k++)
                    {
                        g.DrawString(stats.get(k).name, new Font("微软雅黑", 40), new SolidBrush(Color.valueOf(Color.BLACK)), new PointF((3655 / 25 * (k - (i - 25))) + 95 - ((stats.get(k).name.length() / 2.0f * 58.0f)), ((k % 2) == 0) ? 115.0f : 240.0f));
                    }
                }
                File.WriteBitmap(path + Character.toString(java.io.File.separatorChar) + tn.replace("/", "_") + "lcd" + lcdNum + ".png", bmp);
                lcdNum++;
                //if (i == 10)
                //    break;
                i++;
            }
            new AlertDialog.Builder(this)
                    .setTitle("提示")
                    .setMessage("导出成功")
                    .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {  }
                    })
                    .create().show();
        }
        catch (Exception ex)
        {
            new AlertDialog.Builder(this)
                    .setTitle("提示")
                    .setMessage("导出失败")
                    .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {  }
                    })
                    .create().show();
        }
    }
}