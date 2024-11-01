package com.example.editor;

import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Rect;

public class Graphics {
    Canvas cav = null;

    private Graphics(Bitmap bmp) {
        cav = new Canvas(bmp);
        cav.drawColor(Color.WHITE);
    }

    public static Graphics FromImage(Bitmap bmp) {
        return new Graphics(bmp);
    }
    public void FillRectangle(SolidBrush sb, Rectangle rc) {
        Paint paint = new Paint();
        paint.setColor(sb.c.toArgb());
        paint.setStyle(Paint.Style.FILL);
        //paint.setStrokeJoin(Paint.Join.MITER);
        cav.drawRect(new Rect(rc.p.x, rc.p.y, rc.p.x + rc.s.w, rc.p.y + rc.s.h), paint);
    }
    public void FillRectangle(SolidBrush sb, int x, int y, int w, int h) {
        this.FillRectangle(sb, new Rectangle(new Point(x, y), new Size(w, h)));
    }
    public void FillEllipse(SolidBrush sb, Rectangle rc) {
        Paint paint = new Paint();
        paint.setColor(sb.c.toArgb());
        paint.setStyle(Paint.Style.FILL);
        cav.drawCircle(rc.p.x + (rc.s.w / 2.0f),rc.p.y + (rc.s.h / 2.0f), (rc.s.h / 2.0f), paint);
    }
    public void FillEllipse(SolidBrush sb, int x, int y, int w, int h) {
        this.FillEllipse(sb, new Rectangle(new Point(x, y), new Size(w, h)));
    }
    public void DrawLine(Pen pen, Point p1, Point p2) {
        Paint paint = new Paint();
        paint.setColor(pen.c.toArgb());
        paint.setStyle(Paint.Style.STROKE);
        paint.setStrokeWidth(pen.cx);
        cav.drawLine(p1.x, p1.y, p2.x, p2.y, paint);
    }
    public void DrawLine(Pen pen, int x1, int y1, int x2, int y2) {
        this.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
    }
    public void DrawString(String text, Font font, SolidBrush sb, PointF p) {
        Paint paint = new Paint();
        paint.setColor(sb.c.toArgb());
        paint.setStyle(Paint.Style.FILL);
        paint.setTextSize(font.ft);
        if(!text.startsWith("车次"))
            cav.drawText(text, p.x + (text.length() / 4.0f * font.ft), p.y + (font.ft * 1.25f), paint);
        else
            cav.drawText(text, p.x + (text.length() / 8.0f * font.ft), p.y + (font.ft * 1.25f), paint);
    }
    public void DrawString(String text, Font font, SolidBrush sb, float x, float y) {
        this.DrawString(text, font, sb, new PointF(x, y));
    }
    public void DrawEllipse(Pen pen, Rectangle rc) {
        Paint paint = new Paint();
        paint.setColor(pen.c.toArgb());
        paint.setStrokeWidth(pen.cx);
        paint.setStyle(Paint.Style.STROKE);
        cav.drawCircle(rc.p.x + (rc.s.w / 2.0f),rc.p.y + (rc.s.h / 2.0f), (rc.s.h / 2.0f), paint);
    }
    public void DrawEllipse(Pen pen, int x, int y, int w, int h) {
        this.DrawEllipse(pen, new Rectangle(new Point(x, y), new Size(w, h)));
    }
    public void DrawRectangle(Pen pen, Rectangle rc) {
        Paint paint = new Paint();
        paint.setColor(pen.c.toArgb());
        paint.setStrokeWidth(pen.cx);
        paint.setStyle(Paint.Style.STROKE);
        cav.drawRect(new Rect(rc.p.x, rc.p.y, rc.p.x + rc.s.w, rc.p.y + rc.s.h), paint);
    }
    public void DrawRectangle(Pen pen, int x, int y, int w, int h) {
        this.DrawRectangle(pen, new Rectangle(new Point(x, y), new Size(w, h)));
    }
}