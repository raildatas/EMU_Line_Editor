package com.example.editor;

import android.graphics.Color;

public class SolidBrush {
    public Color c;

    public SolidBrush(Color c) {
        this.c = c;
    }
    public SolidBrush(int c) { this.c = Color.valueOf(c); }
}