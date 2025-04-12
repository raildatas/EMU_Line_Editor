package com.example.editor;

public class Ticket extends Dictionary<SeatType, Integer>
{
    public Ticket(SeatType[] st)
    {
        key = new List<SeatType>();
        value = new List<Integer>();
        for (SeatType s:st
             ) {
            key.add(s);
            value.add(Integer.valueOf(0));
        }
    }
}