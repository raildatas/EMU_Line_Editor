package com.example.editor;

import android.graphics.Bitmap;
import android.net.Uri;

import java.io.*;
import java.net.URI;

public class File {
    public static void WriteAllText(String path, String text) throws IOException {
        java.io.File file = new java.io.File(path);
        if(file.exists())
            file.delete();
        BufferedWriter bw = null;
        try {
            bw = new BufferedWriter(new FileWriter(path));
            bw.write(text);
        }
        catch (IOException e) {
            if(bw != null)
                bw.close();
            throw e;
        }
        bw.close();
    }
    public static String ReadAllText(String path) throws Exception {
        BufferedReader br = null;
        java.io.File file = new java.io.File(path);
        Exception e = null;
        if(!file.exists())
            return "";
        StringBuilder sb = new StringBuilder();
        try {
            br = new BufferedReader(new FileReader(file));
            String line;
            while(true)
            {
                line = br.readLine();
                if(line == null)
                    break;
                sb.append(line);
                sb.append('\n');
            }
        }
        catch (Exception e2) { e = e2; }
        finally {
            if(br != null)
            {
                try { br.close(); }
                catch (Exception e1) { throw e1; }
            }
        }
        if(sb.length()>2)
            return sb.toString().substring(0,sb.length()-1);
        return sb.toString();
    }
    public static boolean Exists(String path) {

        return new java.io.File(path).exists();
    }
    public static void WriteBitmap(String path, Bitmap bitmap) {
        try {
            java.io.File file = new java.io.File(path);
            FileOutputStream fos = new FileOutputStream(file);
            bitmap.compress(Bitmap.CompressFormat.PNG, 100, fos);
            fos.flush();
            fos.close();
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}