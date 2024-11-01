package com.example.editor;

import android.graphics.Color;

public class Pen {

    public Color c;
    public float cx;

    public Pen(Color c, float cx) {
        this.c = c;
        this.cx = cx;
    }
    public Pen(SolidBrush sb) {
        this.c = sb.c;
        this.cx = -1.0f;
    }
}