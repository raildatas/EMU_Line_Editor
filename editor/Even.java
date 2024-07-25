package com.example.editor;

import java.util.Calendar;

public class Even {
    public Date date;
    public int year;
    public String content;

    public Even(int year, int month, int day, String content) {
        date = new Date(day, month);
        this.year = year;
        this.content = content;
    }

    public int IsNow() {
        Calendar calendar = Calendar.getInstance();
        int y = calendar.get(Calendar.YEAR);
        int m = calendar.get(Calendar.MONTH)+1;
        int d = calendar.get(Calendar.DAY_OF_MONTH);
        if((m == date.month) && (d== date.day))
            return y - year;
        else
            return -1;
    }
}