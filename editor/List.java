package com.example.editor;

import java.util.ArrayList;

public class List<T> extends ArrayList<T> {
    public void AddRange(T[] index) {
        for (int i = 0;i<index.length;i++)
        {
            add(index[i]);
        }
    }
    public void Add(T item) {
        add(item);
    }
}