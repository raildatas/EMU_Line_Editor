package com.example.editor;

public class Dictionary<K,V> {
    private List<K> key;
    private List<V> value;

    public Dictionary() {
        key = new List<K>();
        value = new List<V>();
    }
    public void Add(K k, V v) {
        key.add(k);
        value.add(v);
    }
    public V GetE(K k) {
        for (int i = 0;i<key.size();i++)
        {
            if(k == key.get(i))
                return value.get(i);
        }
        return null;
    }
}