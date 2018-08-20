using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Diagnostics;

public class MoveCube : MonoBehaviour {

    public Vector3 cameraFollowOffset = new Vector3(0, 8, -5);
    public Camera followCamera;
    // Use this for initialization
    void Start ()
    {
        if (followCamera == null)
            followCamera = Camera.main;
    }
    // Update is called once per frame
    void Update ()
    {
        followCamera.transform.position = transform.position + cameraFollowOffset;
        // Compute positions of rotors
        Vector3 D = GetComponent<Rigidbody>().transform.TransformPoint(new Vector3(1, 1, -1));
        Vector3 A = GetComponent<Rigidbody>().transform.TransformPoint(new Vector3(1, 1, 1));
        Vector3 B = GetComponent<Rigidbody>().transform.TransformPoint(new Vector3(-1, 1, 1));
        Vector3 C = GetComponent<Rigidbody>().transform.TransformPoint(new Vector3(-1, 1, -1));
        
        // Gravity
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(0, -8.0f, 0);

        // Get force direction relative to each rotor angle.
        var fdir = GetComponent<Rigidbody>().transform.up;

        // Get x, y and z of drone.
        float xpos = GetComponent<Rigidbody>().transform.position.x;
        float ypos = GetComponent<Rigidbody>().transform.position.y;
        float zpos = GetComponent<Rigidbody>().transform.position.z;

        var destobj = GameObject.Find("Cube (2)");
        float xdestpos = destobj.transform.position.x;
        float ydestpos = destobj.transform.position.y;
        float zdestpos = destobj.transform.position.z;

        var rotX = GetComponent<Rigidbody>().transform.rotation.eulerAngles.x;
        var rotZ = GetComponent<Rigidbody>().transform.rotation.eulerAngles.z;

        if (rotX > 180)
        {
            rotX = rotX - 360;
        }
        if (rotZ > 180)
        {
            rotZ = rotZ - 360;
        }

        // FOR GETTING TRAINING DATA.
        /*
        // Force on each rotor.
        float rotorA = 2.0f;
        float rotorB = 2.0f;
        float rotorC = 2.0f;
        float rotorD = 2.0f;

        

        // Increase altitude
        // UP
        if (Input.GetKey(KeyCode.Keypad8))
        {
            rotorA = 6.0f;
            rotorB = 6.0f;
            rotorC = 6.0f;
            rotorD = 6.0f;
            
        }
        // DOWN
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            rotorA = 0.0f;
            rotorB = 0.0f;
            rotorC = 0.0f;
            rotorD = 0.0f;
        }
        // Forward
        else if (Input.GetKey("up"))
        {
            rotorA = 1.8f;
            rotorB = 1.8f;
            rotorC = 2.2f;
            rotorD = 2.2f;
        }
        // Backward
        else if (Input.GetKey("down"))
        {
            rotorA = 2.2f;
            rotorB = 2.2f;
            rotorC = 1.8f;
            rotorD = 1.8f;
        }
        // Left
        else if (Input.GetKey("left"))
        {
            rotorA = 2.2f;
            rotorB = 1.8f;
            rotorC = 1.8f;
            rotorD = 2.2f;
        }
        // Right
        else if (Input.GetKey("right"))
        {
            rotorA = 1.8f;
            rotorB = 2.2f;
            rotorC = 2.2f;
            rotorD = 1.8f;
        }
        
        // For drone stabilization.
        if (!Input.anyKey)
        {
            if (rotX > 5)
            {
                rotorA = rotorA + 0.1f;
                rotorB = rotorB + 0.1f;
                rotorC = rotorC - 0.1f;
                rotorD = rotorD - 0.1f;
            }
            if (rotX < -5)
            {
                rotorA = rotorA - 0.1f;
                rotorB = rotorB - 0.1f;
                rotorC = rotorC + 0.1f;
                rotorD = rotorD + 0.1f;
            }
            if (rotZ > 5)
            {
                rotorA = rotorA - 0.1f;
                rotorB = rotorB + 0.1f;
                rotorC = rotorC + 0.1f;
                rotorD = rotorD - 0.1f;
            }
            if (rotZ < -5)
            {
                rotorA = rotorA + 0.1f;
                rotorB = rotorB - 0.1f;
                rotorC = rotorC - 0.1f;
                rotorD = rotorD + 0.1f;
            }
        }

        // FOR GETTING TRAINING DATA.
        var csv = new StringBuilder();

        csv.Append(xpos.ToString() + ",");
        csv.Append(ypos.ToString() + ",");
        csv.Append(zpos.ToString() + ",");
        csv.Append(xdestpos.ToString() + ",");
        csv.Append(ydestpos.ToString() + ",");
        csv.Append(zdestpos.ToString() + ",");
        csv.Append(rotorA.ToString() + ",");
        csv.Append(rotorB.ToString() + ",");
        csv.Append(rotorC.ToString() + ",");
        csv.Append(rotorD.ToString());
        csv.AppendLine();

        File.AppendAllText("Train.csv", csv.ToString());
        */
        
        // TESTING USING TRAINED NETWORK.

        string python = @"C:\Program Files (x86)\Python\python.exe";
        ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);
        myProcessStartInfo.UseShellExecute = false;
        myProcessStartInfo.RedirectStandardOutput = true;
        myProcessStartInfo.CreateNoWindow = true;

        string pythonCmd = "-c \"import pickle; import neuralnetworks as nn; trainedClass = pickle.load(open('9.trainedNet_5_100', 'rb')) ;rotorVals = trainedClass.use([[" + xpos+", "+ypos+", "+zpos+", "+xdestpos+", "+ydestpos+", "+zdestpos+"]]); print(str(rotorVals[0][0]) + ',' + str(rotorVals[0][1]) + ',' + str(rotorVals[0][2]) + ',' + str(rotorVals[0][3]))\"";
        myProcessStartInfo.Arguments = pythonCmd;
        Process myProcess = new Process();
        myProcess.StartInfo = myProcessStartInfo;
        myProcess.Start();
        StreamReader myStreamReader = myProcess.StandardOutput;
        string myString = myStreamReader.ReadToEnd();
        myProcess.WaitForExit();
        myProcess.Close();

        // String array with four rotor speeds.
        string[] words = myString.Split(',');
        float rotorA = float.Parse(words[0]);
        float rotorB = float.Parse(words[1]);
        float rotorC = float.Parse(words[2]);
        float rotorD = float.Parse(words[3]);
        
        if(rotX >= 150 || rotZ >= 150)
        {
            Application.Quit();
        }
        
        /*
        var sampletext1 = new StringBuilder();
        string[] words = myString.Split(',');
        foreach (string s in words)
        {
            sampletext1.Append(s);
        }
        File.AppendAllText("test.txt", sampletext1.ToString());
        */

        // Add forces.
        GetComponent<Rigidbody>().AddForceAtPosition(fdir * rotorA, A);
        GetComponent<Rigidbody>().AddForceAtPosition(fdir * rotorC, C);
        GetComponent<Rigidbody>().AddForceAtPosition(fdir * rotorB, B);
        GetComponent<Rigidbody>().AddForceAtPosition(fdir * rotorD, D);
    }

    private void OnApplicationQuit()
    {
    }







}

   
