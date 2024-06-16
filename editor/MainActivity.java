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
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.os.StrictMode;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RadioButton;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.TimePicker;
import com.example.myapplication.R;
import org.json.JSONArray;
import org.json.JSONObject;
import java.io.IOException;
import okhttp3.*;

//17:30 OK
public class MainActivity extends AppCompatActivity implements View.OnFocusChangeListener {
    boolean select = false;
    boolean flag = true;
    int upmin = 6, uphour = 10, downmin = 6, downhour = 10;
    int selectB = -1, selectL = -1, selectA = -1, selectC = -1;
    List<String> emus = new List<String>();
    Dictionary<String, String> dic = new Dictionary<>();
    Dictionary<String, String> dic2 = new Dictionary<>();
    List<String> dk1 = new List<>();
    List<String> dk2 = new List<>();
    String train = "";
    List<ListItem> lis = new List<>();
    String upTime = "", downTime = "";
    String[] rcps = new String[0];
    private static double EARTH_RADIUS = 6378137;

    private static double rad(double d) {
        double e = d * Math.PI / 180.0;
        return e;
    }
    public static double GetLength(double lng1,double lat1,double lng2, double lat2) {
        double radLat1 = rad(lat1);
        double radLat2 = rad(lat2);
        double a = radLat1 - radLat2;
        double b = rad(lng1) - rad(lng2);
        double s = 2 * Math.asin(Math.sqrt(Math.pow(Math.sin(a / 2), 2) + Math.cos(radLat1) * Math.cos(radLat2) * Math.pow(Math.sin(b / 2), 2)));
        s = s * EARTH_RADIUS;
        s = Math.round(s * 10000) / 10000;
        return s / 1000.0;
    }
    public String Gethtml(String url) throws IOException {
        return new OkHttpClient().newCall(new Request.Builder().url(url).get().build()).execute().body().string();
    }
    SeatType GetSeatType(String str) {
        switch (str) {
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
        }
        return SeatType.ZE;
    }
    public String replace(String sourceString, char chElemData) {
        String tmpString = "";
        tmpString += chElemData;
        StringBuffer stringBuffer = new StringBuffer(sourceString);
        int iFlag = -1;
        do {
            iFlag = stringBuffer.indexOf(tmpString);
            if (iFlag != -1) {
                stringBuffer.deleteCharAt(iFlag);
            }
        } while (iFlag != -1);
        return stringBuffer.toString();
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
        ((EditText)(findViewById(R.id.et_trainNumber))).setOnFocusChangeListener(new android.view.View.OnFocusChangeListener() {
            @Override
            public void onFocusChange(View v, boolean hasFocus) {
                if(!hasFocus) {
                    ((EditText)(findViewById(R.id.et_trainNumber))).setText(((EditText)(findViewById(R.id.et_trainNumber))).getText().toString().toUpperCase());
                    String tmp = ((EditText)(findViewById(R.id.et_trainNumber))).getText().toString();
                    boolean b = true;
                    b = b && tmp.contains(" ");
                    b = b && (tmp.startsWith("G")||tmp.startsWith("D")||tmp.startsWith("C")||tmp.startsWith("S"));
                    b = b && (tmp.length()<22);
                    if(tmp.contains(" ")) {
                        b = b && (tmp.split(" ")[0].charAt(0) == tmp.split(" ")[1].charAt(0));
                        if(tmp.contains("/")) {
                            b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1).split("/")[0]) < 9999);
                            b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1).split("/")[0]) > 0);
                            b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1).split("/")[0]) < 9999);
                            b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1).split("/")[0]) > 0);
                            b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1).split("/")[1]) < 9999);
                            b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1).split("/")[1]) > 0);
                            b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1).split("/")[1]) < 9999);
                            b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1).split("/")[1]) > 0);
                        }
                        else {
                            b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1)) < 9999);
                            b = b && (Integer.valueOf(tmp.split(" ")[0].substring(1)) > 0);
                            b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1)) < 9999);
                            b = b && (Integer.valueOf(tmp.split(" ")[1].substring(1)) > 0);
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
                        ((EditText)(findViewById(R.id.et_trainNumber))).setText("C1".toCharArray(),0,2);
                }
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
            String[] emustrs, trainstrs, seatstrs;
            emustrs = new String[1];
            JSONObject railstr, tmp;
            Station station;
            Boolean b;
            JSONObject item;
            String[] replaceDatas = null;
            String html;
            double length;
            try {
                html = Gethtml("https://raildatas.github.io/emudata.html");
                DataFiles.emuDatas = replace(html, '\r').split("\n");
                File.WriteAllText(getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS)+Character.toString(java.io.File.separatorChar)
                        +"emudata.dta",html);
                html = Gethtml("https://raildatas.github.io/railwaydata.html");
                DataFiles.railDatas = replace(html, '\r').split("\n");
                File.WriteAllText(getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS)+Character.toString(java.io.File.separatorChar)
                        +"railwaydata.dta",html);
                html = Gethtml("https://raildatas.github.io/replace.html");
                replaceDatas = replace(html, '\r').split("\n");
                File.WriteAllText(getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS)+Character.toString(java.io.File.separatorChar)
                        +"replace.dta",html);
                html = Gethtml("https://raildatas.github.io/rcp.html");
                rcps = replace(html, '\r').split("\n");
                File.WriteAllText(getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS)+Character.toString(java.io.File.separatorChar)
                        +"rcp.dta",html);
            }
            catch (Exception e) {
                System.exit(1);
                try {
                    html = File.ReadAllText(getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS)+Character.toString(java.io.File.separatorChar)
                            +"emudata.dta");
                    DataFiles.emuDatas = replace(html, '\r').split("\n");
                    html = File.ReadAllText(getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS)+Character.toString(java.io.File.separatorChar)
                            +"railwaydata.dta");
                    DataFiles.railDatas = replace(html, '\r').split("\n");
                    html = File.ReadAllText(getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS)+Character.toString(java.io.File.separatorChar)
                            +"replace.dta");
                    replaceDatas = replace(html, '\r').split("\n");
                    html = File.ReadAllText(getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS)+Character.toString(java.io.File.separatorChar)
                            +"rcp.dta");
                    rcps = replace(html, '\r').split("\n");
                }
                catch (Exception e2) {
                    System.exit(1);
                }
            }
            try {
                if(rcps.length > 0) {
                    selectC = 0;
                    ((Button)findViewById(R.id.sp_rcp)).setText(rcps[0]);
                }
                for (int i = 0; i < DataFiles.emuDatas.length; i++) {
                    emustrs = DataFiles.emuDatas[i].split("\\*");
                    trains.clear();
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
                    ((Button)(findViewById(R.id.sp_emu))).setText(train);
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
            }
            catch (Exception e) {
                Log.i("tag",e.toString());
            }
            flag = false;
        ((EditText)(findViewById(R.id.et_before))).setOnFocusChangeListener(this);
    }
    @Override
    public void onFocusChange(View v, boolean hasFocus) {
        if(!hasFocus)
        {
            if(((EditText)(findViewById(R.id.et_before))).length() > 0)
            {
                boolean b = false;
                int i = 0;
                for (Station item : Share.stations)
                {
                    if (item.name
                            .equals(((EditText)(findViewById(R.id.et_before))).getText().toString())
                            && (!(item.name.toLowerCase().startsWith("x")))) {
                        b = true;
                        break;
                    }
                    //Log.i("tag", item.name+"+"+((EditText) (findViewById(R.id.et_before))).getText().toString());
                    i++;
                }
                if(selectB != i)
                {
                    selectL = -1;
                    selectA = -1;
                    ((Button)(findViewById(R.id.sp_lineName))).setText("");
                    ((Button)(findViewById(R.id.sp_after))).setText("");
                    ((Button)(findViewById(R.id.btn_add))).setEnabled(false);
                }
                selectB = i;
                //if(b)
                // ((Button)(findViewById(R.id.sp_lineName))).setEnabled(true);
                if(!b)
                {
                    ((EditText)(findViewById(R.id.et_before))).setText("");
                    new AlertDialog.Builder(MainActivity.this).setTitle("未找到起点站").create().show();
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
            if (lis.size() > 0) {
                selectB = lis.get(lis.size() - 1).after;
                ((EditText)(findViewById(R.id.et_before))).setEnabled(false);
                ((EditText)(findViewById(R.id.et_before))).setText(Share.stations.get(selectB).name);
            }
            else {
                selectB = -1;
                ((EditText)(findViewById(R.id.et_before))).setEnabled(true);
                ((Button)(findViewById(R.id.btn_save))).setEnabled(false);
                ((Button)(findViewById(R.id.btn_out))).setEnabled(false);
                ((EditText)(findViewById(R.id.et_before))).setText("");
            }
        }
        else {
            select = true;
            ((Button)(findViewById(R.id.btn_up))).setBackgroundTintList(ColorStateList.valueOf(
                    ContextCompat.getColor(this,R.color.CRH)));
            ((Button)(findViewById(R.id.btn_down))).setBackgroundTintList(ColorStateList.valueOf(
                    ContextCompat.getColor(this,R.color.E_Train)));
            if (lis.size() > 0) {
                selectB = lis.get(0).before;
                ((EditText)(findViewById(R.id.et_before))).setEnabled(false);
                ((EditText)(findViewById(R.id.et_before))).setText(Share.stations.get(selectB).name);
            }
            else {
                selectB = -1;
                ((EditText)(findViewById(R.id.et_before))).setEnabled(true);
                ((Button)(findViewById(R.id.btn_save))).setEnabled(false);
                ((Button)(findViewById(R.id.btn_out))).setEnabled(false);
                ((EditText)(findViewById(R.id.et_before))).setText("");
            }
        }
        selectL = -1;
        selectA = -1;
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
                ((EditText)(findViewById(R.id.sw_no_before))).setText("");
            }
            else {
                selectB = Integer.valueOf(Integer.toString(lis.get(lis.size() - 1).after));
                ((EditText)(findViewById(R.id.et_before))).setEnabled(false);
                ((EditText)(findViewById(R.id.et_before))).setText(Share.stations.get(selectB).name);
                ((EditText)(findViewById(R.id.sw_no_before))).setText(Integer.toString(lis.get(lis.size() - 1).afterstop));
            }
            selectL = -1;
            selectA = -1;
            ((Button)(findViewById(R.id.sp_lineName))).setText("");
            ((Button)(findViewById(R.id.sp_after))).setText("");
            ((Button)(findViewById(R.id.btn_add))).setEnabled(false);
            ((Button)(findViewById(R.id.sp_after))).setEnabled(false);
            ((EditText)(findViewById(R.id.sw_no_before))).setEnabled(false);
            ((EditText)(findViewById(R.id.sw_no_after))).setText("2");
            ((Switch)findViewById(R.id.sw_before_tst)).setChecked(((Switch)findViewById(R.id.sw_after_tst)).isChecked());
            ((Switch)findViewById(R.id.sw_before_tst)).setEnabled(false);
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
        new TimePickerDialog(this, android.R.style.Theme_Holo_Light_Dialog, new TimePickerDialog.OnTimeSetListener() {
            @Override
            public void onTimeSet(TimePicker view, int hourOfDay, int minute) {
                downmin = minute;
                downhour = hourOfDay;
                String tmp1 = minute<10?"0"+Integer.toString(minute):Integer.toString(minute);
                String tmp2 = hourOfDay<10?"0"+Integer.toString(hourOfDay):Integer.toString(hourOfDay);
                ((TextView)(findViewById(R.id.tv_downstart))).setText(tmp2+":"+tmp1);
                SetTime();
            }
        }, downhour, downmin, true).show();
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
                        ((Button)(findViewById(R.id.sp_emu))).setText(train);
                        if(Share.emus.get(which).trains.get(0).size() > 9)
                            ((Switch)(findViewById(R.id.sw_double))).setEnabled(false);
                        else
                            ((Switch)(findViewById(R.id.sw_double))).setEnabled(true);
                        dialog.cancel();
                    }
                }).create().show();
    }
    public void ChangeLine(View view) {
        onFocusChange(view, false);
        List<String> strs = new List<>();
        boolean b = false;
        for (Station item : Share.stations) {
            if (item.name
                    .equals(((EditText) (findViewById(R.id.et_before))).getText().toString())
                    && (!(item.name.toLowerCase().startsWith("x"))) && ((!(item.name.toLowerCase().endsWith("线路所"))) || (!((EditText)findViewById(R.id.et_before)).isEnabled()))) {
                b = true;
                for (int items : item.rails) {
                    strs.add(Share.rails.get(items).name);
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
            mode = RunMode.ATP;
        else if (((RadioButton)findViewById(R.id.rb_max)).isChecked())
            mode = RunMode.MaxSpeed;
        else if (((RadioButton)findViewById(R.id.rb_slow)).isChecked())
            mode = RunMode.Express;
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
        ((EditText)(findViewById(R.id.sw_no_before))).setText(Integer.toString(lis.get(lis.size()-1).afterstop));
        ((EditText)(findViewById(R.id.sw_no_before))).setEnabled(false);
        ((EditText)(findViewById(R.id.sw_no_after))).setText("2");
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
        if (requestCode == 1 && resultCode == Activity.RESULT_OK) {
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

            Uri uri = data.getData();
            StringBuilder sb = new StringBuilder();
            StringBuilder buffer = new StringBuilder();
            JSONArray jArray,j2;
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
            for(EMU e : Share.emus)
            {
                if(e.name.equals(train)) {
                    e2 = e;
                    break;
                }
            }
            switch (e2.speed)
            {
                case 360:
                    sb.append("CC0000");
                    break;
                case 310:
                case 300:
                    sb.append("FF3300");
                    break;
                case 260:
                    sb.append("FF6600");
                    break;
                case 210:
                    sb.append("FF9900");
                    break;
                case 170:
                    sb.append("032F94");
                    break;
                default:
                    sb.append("00D3FC");
                    break;
            }
            sb.append("\",\"remark\":\"采用");
            sb.append(train);
            if(((Switch)findViewById(R.id.sw_double)).isChecked())
                sb.append("重联");
            sb.append("型列车\",\"lineType\":1,\"company\":\"");
            sb.append(rcps[selectC]);
            sb.append("\",\"route\":{\"up\":[");
            //progressDialog.setProgress(//progressDialog.getProgress() + 1);
            int i = 0;
            int k = 0;
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
                            sb.append(tmp.toString());
                            j2.put(tmp);
                            if (!jArray.getJSONObject(j+1).getString("name").equals(Share.stations.get(lis.get(i).after).name))
                                sb.append(",");
                        }
                        j++;
                    }
                    if (b1) {
                        //Share.rails[lis.get(i).Rail].locations.Reverse();
                        b1 = false;
                    }
                    //progressDialog.setProgress(//progressDialog.getProgress() + 1);
                }
                sb.append("],\"down\":[");
                for (int t = j2.length() - 1; t >= 0; t--) {
                    sb.append(j2.get(t));
                    if (t > 0)
                        sb.append(',');
                }
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
        else if((requestCode == 2 &&(!Environment.isExternalStorageManager()))) {
            new AlertDialog.Builder(this)
                    .setTitle("提示")
                    .setMessage("你没有允许访问文件，无法打开、保存")
                    .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {  }
                    }).create().show();
        }
        else if(requestCode == 5 && resultCode == Activity.RESULT_OK) {
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
        else if(requestCode == 6 && resultCode == Activity.RESULT_OK) {
            try {
                    String s = File.ReadAllText(data.getData().getPath().replace("/document/primary:", "/storage/emulated/0/"));
                    String[] str = s
                        .replace("\r", "").split("\n");
                    String[] strs;
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
                    for(int i = 0; i < rcps.length;i++) {
                        if(rcps[i].equals(str[0])) {
                            selectC = i;
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
                    for (int i = 3;i < str.length;i++) {
                        strs = str[i].split(" ");
                        li = new ListItem(0,0,0,RunMode.ATP,0,0,0,false, false);
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
                                li.mode = RunMode.ATP;
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
    public void OutFile(View view) {
        if (!Environment.isExternalStorageManager())
            return;
        Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT_TREE);
        startActivityForResult(intent, 1);
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
        int speed, max_length, bspeed = 0, aspeed = 0, _speed = 0, m, minute, hourOfDay;
        double length;
        List<String> jArray = new List<>();
        String tmp, tmp1, tmp3;
        List<String> speeds = new List<>();
        List<Double> lengths = new List<>();
        List<Integer> ints = new List<>();
        StringBuilder sb = new StringBuilder();
        String[] tmp2;
        List<String> spd = new List<>();
        EMU e2 = null;
        int min = upmin + (60 * uphour);
        int j = 0;
        boolean b,b1,b2,b3;
        for(EMU e : Share.emus) {
            if(e.name.equals(train)) {
                e2 = e;
                break;
            }
        }
        if(up)
            lis = this.lis;
        else {
            ListItem tmp4;
            int t5;
            boolean t6;
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
                //Toast.makeText(this, Integer.toString(lis.get(0).after), Toast.LENGTH_SHORT).show();
            }
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
            hourOfDay = uphour;
            tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
            tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
            sb.append(tmp3);
            sb.append(":");
            sb.append(tmp1);
            sb.append(" --\n");
        }
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
                            minute = min % 60;
                            hourOfDay = min / 60;
                            sb.append(" ");
                            tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
                            tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
                            sb.append(tmp3);
                            sb.append(":");
                            sb.append(tmp1);
                            min += lis.get(i).beforestop;
                            minute = min % 60;
                            hourOfDay = min / 60;
                            tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
                            tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
                            sb.append(" ");
                            sb.append(tmp3);
                            sb.append(":");
                            sb.append(tmp1);
                            sb.append(" ");
                            if(lis.get(i).beforeteg)
                                sb.append("技停\n");
                            else {
                                sb.append(lis.get(i).beforestop);
                                sb.append("分\n");
                            }
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
                            for (int t = Share.rails.get(lis.get(i).line).stations.size() - 1; t >= 0; t--) {
                                jArray.add(Share.rails.get(lis.get(i).line).stations.get(t));
                            }
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
                            for (int t = Share.rails.get(lis.get(i).line).lengths.size() - 1; t >= 0; t--) {
                                lengths.add(Share.rails.get(lis.get(i).line).lengths.get(t));
                            }
                            j = 0;
                            b = false;
                            continue;
                        }
                        if(!b1 && j > 0)
                            length += lengths.get(j-1);
                        if(b1 && j < lengths.size())
                            length += lengths.get(j);
                        for (String stat:speeds) {
                            b3 = false;
                            m = -1;
                            if (up == false) {
                                up = false;
                            }
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
                                    else if (spd.get(k).startsWith("ms") && (lis.get(i).mode == RunMode.MaxSpeed))
                                        _speed = Integer.valueOf(spd.get(k).substring(3));
                                    else if (spd.get(k).startsWith("atp") && (lis.get(i).mode == RunMode.ATP))
                                        _speed = Integer.valueOf(spd.get(k).substring(4));
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
                                            _speed = speed - 15;
                                        case MaxSpeed:
                                            _speed = speed - 10;
                                            break;
                                    }
                                }
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
                                sb.append(" ");
                                tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
                                tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
                                sb.append(tmp3);
                                sb.append(":");
                                sb.append(tmp1);
                                sb.append("  --:--   --");
                            }
                            b = true;
                            length = 0;
                        }
                        //else
                        //    sb.append("\n");
                        break;
                    }
                    if (b) {
                        if(!b1 && j > 0)
                            length += lengths.get(j-1);
                        for (String stat:speeds) {
                            if(jArray.get(j).equals(stat) && (!jArray.get(j).equals(Share.stations.get(lis.get(i).before).name))) {
                                b2 = false;
                                aspeed = 0;
                                for(int k = 0; k < spd.size(); k++) {
                                    if(spd.get(k).equals(stat))
                                        b2 = true;
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
                                    else if (spd.get(k).startsWith("ms") && (lis.get(i).mode != RunMode.Express)) {
                                        if (b2) {
                                            aspeed = Integer.valueOf(spd.get(k).substring(3));
                                            if((lis.get(i).mode == RunMode.MaxSpeed))
                                                break;
                                        }
                                        else if(lis.get(i).mode != RunMode.Express)
                                            _speed = Integer.valueOf(spd.get(k).substring(3));
                                    }
                                    else if (spd.get(k).startsWith("atp") && (lis.get(i).mode == RunMode.ATP)) {
                                        if (b2) {
                                            aspeed = Integer.valueOf(spd.get(k).substring(4));
                                            break;
                                        }
                                        else
                                            _speed = Integer.valueOf(spd.get(k).substring(4));
                                    }
                                    else if (lis.get(i).mode == RunMode.ATP) {
                                        if (b2)
                                            break;
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
                                            _speed = speed - 15;
                                        case MaxSpeed:
                                            _speed = speed - 10;
                                            break;
                                    }
                                }
                                min += Calc(length, ast, bspeed, aspeed, _speed, 0);
                                bspeed = _speed;
                                if (aspeed == 0) bspeed = 0;
                                length = 0;
                                break;
                            }
                        }
                        if(b1 && j < lengths.size())
                            length += lengths.get(j);
                    }// if b==true
                    j++;
                }
                if (b1) {
                    //Share.rails[lis.get(i).Rail].locations.Reverse();
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

    public enum RunMode {
        ATP,
        MaxSpeed,
        Express
    }
    public static class ListItem {
        public int before;
        public int after;
        public int line;
        public int beforestop;
        public int afterstop;
        public int earlytime;
        public RunMode mode;
        public boolean beforeteg;
        public boolean afterteg;
        public ListItem(int before, int beforestop, int line, RunMode mode, int after, int afterstop, int earlytime, boolean beforeteg, boolean afterteg) {
            this.before = before;
            this.line = line;
            this.after = after;
            this.mode = mode;
            this.beforestop = beforestop;
            this.afterstop = afterstop;
            this.earlytime = earlytime;
            this.beforeteg = beforeteg;
            this.afterteg = afterteg;
        }
    }
}