package com.example.editor;

public class Train {
    public Dictionary<SeatType, Byte> dic;
    public byte number;

    public Train(List<Byte> person, List<SeatType> seatTypes, Byte number) {
        dic = new Dictionary<SeatType, Byte>();
        if (seatTypes.size() != person.size())
        {
            //throw new Exception("对应的席位无法匹配到对应的人数。");
        }
        for (int i = 0; i < seatTypes.size(); i++)
        {
            dic.Add(seatTypes.get(i), person.get(i));
        }
        this.number = number;
    }
}