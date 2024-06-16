package com.example.editor;

//import java.util.List;

public class Station {
    public List<Integer> rails = new List<Integer>();
    public String name;

    public Station(String name)
    {
        this.name = name;
        rails = new List<Integer>();
    }

    public void AddRail(Integer[] index)
    {
        rails.AddRange(index);
    }
}
