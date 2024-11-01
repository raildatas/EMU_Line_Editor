package com.example.editor;

public class PIDSInfo {
    public boolean isStop;
    public String arriveTime;
    public short delayTime;
    public String name;

    public PIDSInfo(String str)
    {
        this.isStop = str.contains("|");
        String[] strs = str.split("\\|");
        this.name = strs[0];
        if (isStop)
        {
            arriveTime = strs[1].substring(0, 4);
            if (strs[1].contains(":"))
                this.delayTime = Short.valueOf(strs[1].split(":")[1]);
            else
                this.delayTime = 0;
        }
    }
}