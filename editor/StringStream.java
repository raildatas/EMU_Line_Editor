package com.example.editor;

import java.io.IOException;
import java.io.OutputStream;

public class StringStream extends OutputStream {
    StringBuilder sb;

    public StringStream()
    {
        sb = new StringBuilder();
    }

    @Override
    public void write(int b) throws IOException {
        sb.append((char)b);
    }
}
