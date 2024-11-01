package com.example.editor;

public class DateTime {
    int h;
    int m;

    public DateTime(int h, int m) {
        this.h = h;
        this.m = m;
    }

    public String toString(String format) {
        String tmp1,tmp3;
        StringBuilder sb = new StringBuilder();
        int minute,hourOfDay;
        minute = m % 60;
        hourOfDay = m / 60;
        hourOfDay = hourOfDay % 24;
        tmp1 = minute < 10 ? "0" + Integer.toString(minute) : Integer.toString(minute);
        tmp3 = hourOfDay < 10 ? "0" + Integer.toString(hourOfDay) : Integer.toString(hourOfDay);
        sb.append(tmp3);
        sb.append(tmp1);
        return sb.toString();
    }
}