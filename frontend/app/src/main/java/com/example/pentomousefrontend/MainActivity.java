package com.example.pentomousefrontend;

import android.os.Bundle;
import android.provider.ContactsContract;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;

import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;

public class MainActivity extends AppCompatActivity {

    private static final int SERVER_PORT = 5000;
    private static final String SERVER_IP = "192.168.3.36"; //replace ip with server ip
    private DatagramSocket socket;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        EdgeToEdge.enable(this);
        setContentView(R.layout.activity_main);
        View mainLayout = findViewById(R.id.main);      //adding for hover event

        //EditText inputField = findViewById(R.id.inputField);  // Add EditText in XML
        //Button sendBtn = findViewById(R.id.sendBtn);          // Add Button in XML, will replace when i figure out how to read stylus input
        /*refactored
        sendBtn.setOnClickListener(v -> {       //will be removing this, was just to establish con
            String message = inputField.getText().toString();
            new Thread(() -> sendMessage(message)).start();
        });*/
        try {
            socket = new DatagramSocket();
        }catch (Exception e){
            e.printStackTrace();
        }

        mainLayout.setOnHoverListener((view, event) -> {        //working
            if(event.getToolType(0) == MotionEvent.TOOL_TYPE_STYLUS){
                /*float x = event.getX();
                float y = event.getY();
                float pressure = event.getPressure();
                new Thread(()-> sendMessage(x + "," + y + "," + pressure)).start();*/
                sendMessage(event.getX(), event.getY(), event.getPressure(), "HOVER");
            }
            return true;    //consume the event
        });
    }

    private void sendMessage(float x, float y, float pressure, String type) {
        /*try {     //refactor to put in 1 socket in oncreate
            DatagramSocket socket = new DatagramSocket();
            InetAddress serverAddr = InetAddress.getByName(SERVER_IP);

            byte[] buf = message.getBytes();
            DatagramPacket packet = new DatagramPacket(buf, buf.length, serverAddr, SERVER_PORT);

            socket.send(packet);
            socket.close();
        } catch (Exception e) {
            e.printStackTrace();
        }*/
        new Thread(() ->{
            try {
                String message = type + ":" + x + "," + y + "," + pressure;
                InetAddress servAddress = InetAddress.getByName(SERVER_IP);
                byte[] buff = message.getBytes();
                DatagramPacket packet = new DatagramPacket(buff, buff.length, servAddress, SERVER_PORT);
                socket.send(packet);
            }catch (Exception e){
                e.printStackTrace();
            }
        }).start();
    }

    public boolean onTouchEvent(MotionEvent event){ //https://developer.android.com/reference/android/view/MotionEvent
        if(event.getToolType(0) == MotionEvent.TOOL_TYPE_STYLUS) {        //if the tool we are using is a stylus
            //we get our variables
            //float x = event.getX();
            //float y = event.getY();
            //float pressure = event.getPressure();
            //we then need to send this via backend
            //new Thread(() -> sendMessage(x + "," + y + "," + pressure)).start();   //start new thread with message data
            //refactored above
            if(event.getAction() == MotionEvent.ACTION_MOVE){
                sendMessage(event.getX(), event.getY(), event.getPressure(), "TOUCH");
            }
        }
        return super.onTouchEvent(event);
    }
    //need to check for hover aswell to track where the stylus is
    // not working atm refactoring
    /*
    public boolean onHoverEvent(MotionEvent event){
        if(event.getToolType(0) == MotionEvent.TOOL_TYPE_STYLUS) {
            //get x and y
            float x = event.getX();
            float y = event.getY();
            //send to backend
            new Thread(() -> sendMessage("Hovering Stylus: " + x + "," + y)).start();
        }
        return  super.onHoverEvent(event);
    }
    */
    //according to gpt need to just set a hover listener


}