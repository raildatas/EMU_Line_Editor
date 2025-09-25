package com.example.editor;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Paint;
import android.graphics.Picture;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Environment;
import android.view.View;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ImageView;

import androidx.appcompat.app.AppCompatActivity;

import com.example.myapplication.R;

public class TicketActivity  extends AppCompatActivity{

    public void temp()
    {
        Bitmap bmp = ((BitmapDrawable)((ImageView)findViewById(R.id.img_tkt)).getDrawable()).getBitmap();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_ticket);
    }
    public void SaveButton(View view) {
        if (Environment.isExternalStorageManager()) {
            Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT_TREE);
            startActivityForResult(intent, 7);
        }
    }
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        switch (requestCode){
            case 7:
                try {
                    Bitmap bmp = ((BitmapDrawable)getResources().getDrawable(R.drawable.ticket)).getBitmap().copy(Bitmap.Config.ARGB_8888, true);
                    Graphics g = Graphics.FromImage(bmp);
                    g.cav.drawBitmap(((BitmapDrawable)getResources().getDrawable(R.drawable.ticket)).getBitmap(), 0, 0, new Paint());
                    String astn = ((EditText)findViewById(R.id.et_tka)).getText().toString().split("\\|")[0];
                    int al = astn.length();
                    String bstn = ((EditText)findViewById(R.id.et_tkb)).getText().toString().split("\\|")[0];
                    int bl = bstn.length();
                    float f = 491.7729f;
                    if (astn.length() == 2)
                    {
                        astn = new StringBuffer(astn).insert(1, "    ").toString();
                        al = 3;
                    }
                    if (bstn.length() == 2)
                    {
                        bstn = new StringBuffer(bstn).insert(1, "    ").toString();
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
                    g.DrawStrings(((EditText)findViewById(R.id.et_tkn)).getText().toString().substring(0), new Font("Bahnschrift", 48 ),
                            new SolidBrush(Color.valueOf(Color.rgb(0xf9, 0x2f, 0x10))), 205.5277f - (209.4209f / 2), 112.6888f - (59.4258f / 2));
                    g.DrawStrings(((EditText)findViewById(R.id.et_tkt)).getText().toString().substring(0), new Font("宋体", 55 ),
                            new SolidBrush(Color.rgb(0, 0, 0)), 529.536f - (151.25f / 2 * 0) - ((((EditText)findViewById(R.id.et_tkt)).getText().toString().substring(0)).length() / 4.0f * 55f), 171.1038f - 30.25f);
                    g.DrawStrings(astn, new Font("微软雅黑", 55 ), new SolidBrush(Color.rgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - (al * 55f) - 10f, 168.2542f - (60.5f / 2) - 10f);
                    if (((EditText)findViewById(R.id.et_tka)).getText().toString().contains("|"))
                    {
                        if ((((EditText)findViewById(R.id.et_tka)).getText().toString().split("\\|")[1].length() * 18) >= (al * 55)) //英语比中文长
                            g.DrawStrings(((EditText)findViewById(R.id.et_tka)).getText().toString().split("\\|")[1], new Font("宋体", 36 ), new SolidBrush(Color.rgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - (al * 55f) - 10f, 215.1326f - (39.6006f / 2) - 10f);
                        else //英语比中文短
                            g.DrawStrings(((EditText)findViewById(R.id.et_tka)).getText().toString().split("\\|")[1], new Font("宋体", 36 ), new SolidBrush(Color.rgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - 10f - (((EditText)findViewById(R.id.et_tka)).getText().toString().split("\\|")[1].length() * 18f), 215.1326f - (39.6006f / 2) - 10f);
                    }
                    g.DrawStrings("站", new Font("宋体", 42 ), new SolidBrush(Color.rgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f), 167.9109f - (46.2002f / 2));

                    g.DrawStrings(bstn, new Font("微软雅黑", 55 ), new SolidBrush(Color.rgb(0, 0, 0)), 345.7445f - (46.2002f / 2) - (bl*55f), 168.2542f - (60.5f / 2) - 10f);
                    if (((EditText)findViewById(R.id.et_tka)).getText().toString().contains("|"))
                    {
                        g.DrawStrings(((EditText)findViewById(R.id.et_tkb)).getText().toString().split("\\|")[1], new Font("宋体", 36 ), new SolidBrush(Color.rgb(0, 0, 0)), 345.7445f - (46.2002f / 2) - (((EditText)findViewById(R.id.et_tkb)).getText().toString().split("\\|")[1].length() * 18f), 215.1326f - (39.6006f / 2) - 10f);
                    }
                    g.DrawStrings("站", new Font("宋体", 42 ), new SolidBrush(Color.rgb(0, 0, 0)), 345.7445f - (46.2002f / 2), 167.9109f - (46.2002f / 2));

                    g.DrawStrings(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(0, 4), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            126.1459f - (111.6074f / 2) + (Count(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(3-3, 4), "1") * 0f), 265.1282f - (59.4258f / 2) );
                    g.DrawStrings(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(5, 5+2), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            244.4638f - (56.2285f / 2) , 265.1282f - (59.4258f / 2));
                    g.DrawStrings(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(8, 8+2), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            338.2602f - (44.1377f / 2) -7f, 265.1282f - (59.4258f / 2) );
                    g.DrawStrings(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(11, 2+11), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            434.6693f - (44.6533f / 2) -4f + (Count(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(14-3, 2+14-3), "1") * 0f), 265.1282f - (59.4258f / 2) );
                    g.DrawStrings(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(14, 2+14), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            500.1137f - (55.1973f / 2) - 0f, 265.1282f - (59.4258f / 2));

                    g.DrawStrings(((EditText)findViewById(R.id.et_tkc)).getText().toString(), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            675.7763f - (56.7959f / 2) - 10f + (Count(((EditText)findViewById(R.id.et_tkc)).getText().toString(), "1") * 10f), 265.1282f - (59.4258f / 2) );
                    g.DrawStrings(((EditText)findViewById(R.id.et_tks)).getText().toString(), new Font("宋体", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            781.6835f - (79.2002f / 2) - 7.5f, 265.1282f - (59.4258f / 2));

                    g.DrawStrings(((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[0], new Font("Noto Serif SC", 36 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            96.3871f - (30.334f / 2), 319.9993f - (69.4805f / 2) + 7.5f);
                    g.DrawStrings(((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[1], new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            165.8598f - (95.7256f / 2) + (((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[0].length() * 18f) - 15f, 316.7962f - (54.0234f / 2) - 5f);
                    g.DrawStrings(((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[2], new Font("宋体", 24 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            165.8598f - (95.7256f / 2) + (((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[0].length() * 18f) - 10f + ((((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[1].length()) * 26f), 314.8333f - (26.2002f / 2));

                    g.DrawStrings(((EditText)findViewById(R.id.et_tst)).getText().toString(), new Font("宋体", 42 ), new SolidBrush(Color.rgb(0, 0, 0)), 823.6229f - (((EditText)findViewById(R.id.et_tst)).getText().toString().length() * 21f), 315.7581f - 20);

                    if (((CheckBox)findViewById(R.id.cb_dis)).isChecked()) {
                        g.DrawStrings(((EditText)findViewById(R.id.et_tki)).getText().toString().substring(0,10), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2), 466.2356f - (54.0234f / 2));
                        g.DrawStrings("****", new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2) + 264 + 5f, 466.2356f - (54.0234f / 2));
                        g.DrawStrings(((EditText)findViewById(R.id.et_tki)).getText().toString().substring(14, 4+14), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2) - 36f + 240 + 144 + 5f, 466.2356f - (54.0234f / 2));
                        g.DrawStrings(" " + ((EditText)findViewById(R.id.et_tki)).getText().toString().substring(18), new Font("宋体", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2) - 36f + 240 + 144 + 72 + 24 + 5f, 466.2356f - (54.0234f / 2));
                    }
                    else
                    {
                        g.DrawStrings(((EditText)findViewById(R.id.et_tki)).getText().toString().substring(0, 18), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2), 466.2356f - (54.0234f / 2));
                        g.DrawStrings(" " + ((EditText)findViewById(R.id.et_tki)).getText().toString().substring(18), new Font("宋体", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2) + (18f * 24f) - (Count(((EditText)findViewById(R.id.et_tki)).getText().toString().substring(0, 18), "1") * 0f) + 48f, 466.2356f - (54.0234f / 2));
                    }

                    g.DrawStrings(((EditText)findViewById(R.id.et_tkm)).getText().toString().substring(0) + ((EditText)findViewById(R.id.et_tkn)).getText().toString().substring(0), new Font("Bernard MT Condensed", 36 ), new SolidBrush(Color.rgb(0, 0, 0)), 261.8358f - (361.2305f / 2) - 24f, 616.718f - (44.3359f / 2));

                    g.DrawStrings(((EditText)findViewById(R.id.et_tel)).getText().toString().replace("：", ":"), new Font("宋体", 40 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            804.5585f + (352f / 2) - (((EditText)findViewById(R.id.et_tel)).getText().toString().length() * 40f), 112.6888f - 30f);

                    //491.7729

                    if(((EditText)findViewById(R.id.et_tmk)).getText().toString().length() == 1)
                    {
                        g.DrawStrings(((EditText)findViewById(R.id.et_tmk)).getText().toString(), new Font("华文中宋", 30 ), new SolidBrush(Color.rgb(0, 0, 0)),
                                463.6884f - (33f / 2f) + 5f, 316.7029f - (33.54f / 2f) - 5f);
                        g.DrawEllipse(new Pen(Color.rgb(0, 0, 0), 3), 463.2284f - ((float)Math.sqrt(1800) / 2f) + 4f, 314.0786f - (42.4264f / 2f), (float)Math.sqrt(1800) * 1f, (float)Math.sqrt(1800) * 1f);
                    }
                    else if (((EditText)findViewById(R.id.et_tmk)).getText().toString().length() > 1)
                    {
                        f -= ((EditText)findViewById(R.id.et_tmk)).getText().toString().length() / 2.0f * ((float)Math.sqrt(1800));
                        f -= (((EditText)findViewById(R.id.et_tmk)).getText().toString().length() - 1.0f) / 2.0f * 15.0f;
                        for(int i = 0; i < ((EditText)findViewById(R.id.et_tmk)).getText().toString().length(); i++)
                        {
                            //463.6884(字)-463.2284(圆)
                            //
                            g.DrawEllipse(new Pen(Color.rgb(0, 0, 0), 3), f, 314.0786f - (42.4264f / 2f), (float)Math.sqrt(1800) * 1f, (float)Math.sqrt(1800) * 1f);
                            g.DrawStrings(Character.toString(((EditText)findViewById(R.id.et_tmk)).getText().toString().charAt(i)), new Font("华文中宋", 30 ), new SolidBrush(Color.rgb(0, 0, 0)),
                                    f + 463.6884f - 463.2284f + 5f, 316.7029f - (33.54f / 2f) - 5f);
                            f += 15f;
                            f += (float)Math.sqrt(1800);
                        }
                    }

                    ((ImageView)findViewById(R.id.img_tkt)).setImageBitmap(bmp);
                    File.WriteBitmap(data.getData().getPath().replace("/document/primary:", "/storage/emulated/0/").replace("/tree/primary:", "/storage/emulated/0/") + Character.toString(java.io.File.separatorChar) + ((EditText)findViewById(R.id.et_tkt)).getText().toString().replaceAll("/", "_")+ ".png", bmp);
                    new AlertDialog.Builder(this)
                            .setTitle("提示")
                            .setMessage("保存成功")
                            .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                                @Override
                                public void onClick(DialogInterface dialog, int which) {  }
                            })
                            .create().show();
                }
                catch (Exception ex) {
                    new AlertDialog.Builder(this)
                            .setTitle("错误")
                            .setMessage("保存失败")
                            .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                                @Override
                                public void onClick(DialogInterface dialog, int which) {  }
                            })
                            .create().show();
                }
                break;
        }
    }
    private int Count(String str, String find)
    {
        int i = str.length();
        return i - str.replaceAll(find, "").length();
    }
    public void Preview(View view) {
        try {
            Bitmap bmp = ((BitmapDrawable)getResources().getDrawable(R.drawable.ticket)).getBitmap().copy(Bitmap.Config.ARGB_8888, true);
            Graphics g = Graphics.FromImage(bmp);
            g.cav.drawBitmap(((BitmapDrawable)getResources().getDrawable(R.drawable.ticket)).getBitmap(), 0, 0, new Paint());
            String astn = ((EditText)findViewById(R.id.et_tka)).getText().toString().split("\\|")[0];
            int al = astn.length();
            String bstn = ((EditText)findViewById(R.id.et_tkb)).getText().toString().split("\\|")[0];
            int bl = bstn.length();
            float f = 491.7729f;
            if (astn.length() == 2)
            {
                astn = new StringBuffer(astn).insert(1, "    ").toString();
                al = 3;
            }
            if (bstn.length() == 2)
            {
                bstn = new StringBuffer(bstn).insert(1, "    ").toString();
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
            g.DrawStrings(((EditText)findViewById(R.id.et_tkn)).getText().toString().substring(0), new Font("Bahnschrift", 48 ),
                    new SolidBrush(Color.valueOf(Color.rgb(0xf9, 0x2f, 0x10))), 205.5277f - (209.4209f / 2), 112.6888f - (59.4258f / 2));
            g.DrawStrings(((EditText)findViewById(R.id.et_tkt)).getText().toString().substring(0), new Font("宋体", 55 ),
                    new SolidBrush(Color.rgb(0, 0, 0)), 529.536f - (151.25f / 2 * 0) - ((((EditText)findViewById(R.id.et_tkt)).getText().toString().substring(0)).length() / 4.0f * 55f), 171.1038f - 30.25f);
            g.DrawStrings(astn, new Font("微软雅黑", 55 ), new SolidBrush(Color.rgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - (al * 55f) - 10f, 168.2542f - (60.5f / 2) - 10f);
            if (((EditText)findViewById(R.id.et_tka)).getText().toString().contains("|"))
            {
                if ((((EditText)findViewById(R.id.et_tka)).getText().toString().split("\\|")[1].length() * 18) >= (al * 55)) //英语比中文长
                    g.DrawStrings(((EditText)findViewById(R.id.et_tka)).getText().toString().split("\\|")[1], new Font("宋体", 36 ), new SolidBrush(Color.rgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - (al * 55f) - 10f, 215.1326f - (39.6006f / 2) - 10f);
                else //英语比中文短
                    g.DrawStrings(((EditText)findViewById(R.id.et_tka)).getText().toString().split("\\|")[1], new Font("宋体", 36 ), new SolidBrush(Color.rgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f) - 10f - (((EditText)findViewById(R.id.et_tka)).getText().toString().split("\\|")[1].length() * 18f), 215.1326f - (39.6006f / 2) - 10f);
            }
            g.DrawStrings("站", new Font("宋体", 42 ), new SolidBrush(Color.rgb(0, 0, 0)), 774.6464f + (181.5f / 1) - (46.2002f * 1.5f), 167.9109f - (46.2002f / 2));

            g.DrawStrings(bstn, new Font("微软雅黑", 55 ), new SolidBrush(Color.rgb(0, 0, 0)), 345.7445f - (46.2002f / 2) - (bl*55f), 168.2542f - (60.5f / 2) - 10f);
            if (((EditText)findViewById(R.id.et_tka)).getText().toString().contains("|"))
            {
                g.DrawStrings(((EditText)findViewById(R.id.et_tkb)).getText().toString().split("\\|")[1], new Font("宋体", 36 ), new SolidBrush(Color.rgb(0, 0, 0)), 345.7445f - (46.2002f / 2) - (((EditText)findViewById(R.id.et_tkb)).getText().toString().split("\\|")[1].length() * 18f), 215.1326f - (39.6006f / 2) - 10f);
            }
            g.DrawStrings("站", new Font("宋体", 42 ), new SolidBrush(Color.rgb(0, 0, 0)), 345.7445f - (46.2002f / 2), 167.9109f - (46.2002f / 2));

            g.DrawStrings(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(0, 4), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    126.1459f - (111.6074f / 2) + (Count(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(3-3, 4), "1") * 0f), 265.1282f - (59.4258f / 2) );
            g.DrawStrings(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(5, 5+2), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    244.4638f - (56.2285f / 2) , 265.1282f - (59.4258f / 2));
            g.DrawStrings(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(8, 8+2), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    338.2602f - (44.1377f / 2) -7f, 265.1282f - (59.4258f / 2) );
            g.DrawStrings(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(11, 2+11), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    434.6693f - (44.6533f / 2) -4f + (Count(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(14-3, 2+14-3), "1") * 0f), 265.1282f - (59.4258f / 2) );
            g.DrawStrings(((EditText)findViewById(R.id.et_tm)).getText().toString().substring(14, 2+14), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    500.1137f - (55.1973f / 2) - 0f, 265.1282f - (59.4258f / 2));

            g.DrawStrings(((EditText)findViewById(R.id.et_tkc)).getText().toString(), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    675.7763f - (56.7959f / 2) - 10f + (Count(((EditText)findViewById(R.id.et_tkc)).getText().toString(), "1") * 10f), 265.1282f - (59.4258f / 2) );
            g.DrawStrings(((EditText)findViewById(R.id.et_tks)).getText().toString(), new Font("宋体", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    781.6835f - (79.2002f / 2) - 7.5f, 265.1282f - (59.4258f / 2));

            g.DrawStrings(((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[0], new Font("Noto Serif SC", 36 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    96.3871f - (30.334f / 2), 319.9993f - (69.4805f / 2) + 7.5f);
            g.DrawStrings(((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[1], new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    165.8598f - (95.7256f / 2) + (((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[0].length() * 18f) - 15f, 316.7962f - (54.0234f / 2) - 5f);
            g.DrawStrings(((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[2], new Font("宋体", 24 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    165.8598f - (95.7256f / 2) + (((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[0].length() * 18f) - 10f + ((((EditText)findViewById(R.id.et_tkp)).getText().toString().split("\\|")[1].length()) * 26f), 314.8333f - (26.2002f / 2));

            g.DrawStrings(((EditText)findViewById(R.id.et_tst)).getText().toString(), new Font("宋体", 42 ), new SolidBrush(Color.rgb(0, 0, 0)), 823.6229f - (((EditText)findViewById(R.id.et_tst)).getText().toString().length() * 21f), 315.7581f - 20);

            if (((CheckBox)findViewById(R.id.cb_dis)).isChecked()) {
                g.DrawStrings(((EditText)findViewById(R.id.et_tki)).getText().toString().substring(0,10), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2), 466.2356f - (54.0234f / 2));
                g.DrawStrings("****", new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2) + 264 + 5f, 466.2356f - (54.0234f / 2));
                g.DrawStrings(((EditText)findViewById(R.id.et_tki)).getText().toString().substring(14, 4+14), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2) - 36f + 240 + 144 + 5f, 466.2356f - (54.0234f / 2));
                g.DrawStrings(" " + ((EditText)findViewById(R.id.et_tki)).getText().toString().substring(18), new Font("宋体", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2) - 36f + 240 + 144 + 72 + 24 + 5f, 466.2356f - (54.0234f / 2));
            }
            else
            {
                g.DrawStrings(((EditText)findViewById(R.id.et_tki)).getText().toString().substring(0, 18), new Font("Bahnschrift", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2), 466.2356f - (54.0234f / 2));
                g.DrawStrings(" " + ((EditText)findViewById(R.id.et_tki)).getText().toString().substring(18), new Font("宋体", 48 ), new SolidBrush(Color.rgb(0, 0, 0)), 189.7577f - (240.2109f / 2) + (18f * 24f) - (Count(((EditText)findViewById(R.id.et_tki)).getText().toString().substring(0, 18), "1") * 0f) + 48f, 466.2356f - (54.0234f / 2));
            }

            g.DrawStrings(((EditText)findViewById(R.id.et_tkm)).getText().toString().substring(0) + ((EditText)findViewById(R.id.et_tkn)).getText().toString().substring(0), new Font("Bernard MT Condensed", 36 ), new SolidBrush(Color.rgb(0, 0, 0)), 261.8358f - (361.2305f / 2) - 24f, 616.718f - (44.3359f / 2));

            g.DrawStrings(((EditText)findViewById(R.id.et_tel)).getText().toString().replace("：", ":"), new Font("宋体", 40 ), new SolidBrush(Color.rgb(0, 0, 0)),
                    804.5585f + (352f / 2) - (((EditText)findViewById(R.id.et_tel)).getText().toString().length() * 40f), 112.6888f - 30f);

            //491.7729

            if(((EditText)findViewById(R.id.et_tmk)).getText().toString().length() == 1)
            {
                g.DrawStrings(((EditText)findViewById(R.id.et_tmk)).getText().toString(), new Font("华文中宋", 30 ), new SolidBrush(Color.rgb(0, 0, 0)),
                        463.6884f - (33f / 2f) + 5f, 316.7029f - (33.54f / 2f) - 5f);
                g.DrawEllipse(new Pen(Color.rgb(0, 0, 0), 3), 463.2284f - ((float)Math.sqrt(1800) / 2f) + 4f, 314.0786f - (42.4264f / 2f), (float)Math.sqrt(1800) * 1f, (float)Math.sqrt(1800) * 1f);
            }
            else if (((EditText)findViewById(R.id.et_tmk)).getText().toString().length() > 1)
            {
                f -= ((EditText)findViewById(R.id.et_tmk)).getText().toString().length() / 2.0f * ((float)Math.sqrt(1800));
                f -= (((EditText)findViewById(R.id.et_tmk)).getText().toString().length() - 1.0f) / 2.0f * 15.0f;
                for(int i = 0; i < ((EditText)findViewById(R.id.et_tmk)).getText().toString().length(); i++)
                {
                    //463.6884(字)-463.2284(圆)
                    //
                    g.DrawEllipse(new Pen(Color.rgb(0, 0, 0), 3), f, 314.0786f - (42.4264f / 2f), (float)Math.sqrt(1800) * 1f, (float)Math.sqrt(1800) * 1f);
                    g.DrawStrings(Character.toString(((EditText)findViewById(R.id.et_tmk)).getText().toString().charAt(i)), new Font("华文中宋", 30 ), new SolidBrush(Color.rgb(0, 0, 0)),
                            f + 463.6884f - 463.2284f + 5f, 316.7029f - (33.54f / 2f) - 5f);
                    f += 15f;
                    f += (float)Math.sqrt(1800);
                }
            }

            ((ImageView)findViewById(R.id.img_tkt)).setImageBitmap(bmp);
        }
        catch (Exception ex)
        {
            new AlertDialog.Builder(this)
                    .setTitle("错误")
                    .setMessage("报销凭证生成失败" + ex.getMessage()
                    +((EditText)findViewById(R.id.et_tm)).getText().toString())
                    .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {  }
                    })
                    .create().show();
        }
    }
}
