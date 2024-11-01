package com.example.editor;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import android.content.DialogInterface;
import android.os.Bundle;
import android.os.Environment;
import android.view.View;
import android.widget.EditText;

import com.example.myapplication.R;

public class SettingsActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);
        ((EditText)findViewById(R.id.et_dataLink)).setText(Share.settings.GetE("dataLink"));
        ((EditText)findViewById(R.id.et_asnr)).setText(Share.settings.GetE("asnr"));
    }
    public void Apply(View view) {
        if (((EditText)findViewById(R.id.et_asnr)).getText().toString().equals(""))
            ((EditText)findViewById(R.id.et_asnr)).setText("3000");
        if (((EditText)findViewById(R.id.et_dataLink)).getText().toString().equals(""))
            ((EditText)findViewById(R.id.et_dataLink)).setText("https://raildatas.github.io/");
        Share.settings.SetE("dataLink", ((EditText)findViewById(R.id.et_dataLink)).getText().toString());
        Share.settings.SetE("asnr", ((EditText)findViewById(R.id.et_asnr)).getText().toString());
        try {
            File.WriteAllText(getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS) + Character.toString(java.io.File.separatorChar) + "settings.ini", "dataLink=" + ((EditText) findViewById(R.id.et_dataLink)).getText().toString()
                    + "=asnr=" + ((EditText) findViewById(R.id.et_asnr)).getText().toString());
        }
        catch (Exception e) {
            new AlertDialog.Builder(this)
                    .setTitle("错误")
                    .setMessage("应用失败，原因：" + e.getMessage())
                    .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {  }
                    }).create().show();
        }
        finish();
    }
    public void Cancel(View view) {
        finish();
    }
}