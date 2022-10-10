using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    public Text debugText;
    public InputField portNameInput;
    public InputField baudRateInput;
    public InputField dataBitsInput;
    public Dropdown parityDd;
    public Dropdown stopBitsDd;

    public string portName = "COM3";//������
    public int baudRate = 9600;//������
    public Parity parity = Parity.None;//Ч��λ
    public int dataBits = 8;//����λ
    public StopBits stopBits = StopBits.One;//ֹͣλ
    SerialPort sp = null;
    Thread dataReceiveThread;
    public List<byte> listReceive = new List<byte>();
    char[] strchar = new char[100];//���յ��ַ���Ϣת��Ϊ�ַ�������Ϣ
    string str;

    void Start()
    {
        portNameInput.text = "COM3";
        baudRateInput.text = "9600";
        dataBitsInput.text = "8";
        parityDd.value = 0;
        stopBitsDd.value = 1;
       
        dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));
        dataReceiveThread.Start();
    }
    void Update()
    {
        
    }


    public void BtnClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        if (btn != null)
        {
            if (btn.name == "�򿪰�ť")
            {
                debugText.text = "";
                ClosePort();
                OpenPort();              

            }
            else if (btn.name == "�رհ�ť")
            {
                ClosePort();
            }
        }
    }

    public void OpenPort()
    {

        portName = portNameInput.text;
        baudRate = int.Parse(baudRateInput.text);
        dataBits = int.Parse(dataBitsInput.text);

        if (parityDd.value == 0)
            parity = Parity.None;
        else if (parityDd.value == 1)
            parity = Parity.Odd;
        else if (parityDd.value == 2)
            parity = Parity.Even;
        else if (parityDd.value == 3)
            parity = Parity.Mark;
        else if (parityDd.value == 4)
            parity = Parity.Space;

        if (stopBitsDd.value == 0)
            stopBits = StopBits.None;
        else if (stopBitsDd.value == 1)
            stopBits = StopBits.One;
        else if (stopBitsDd.value == 2)
            stopBits = StopBits.Two;
        else if (stopBitsDd.value == 3)
            stopBits = StopBits.OnePointFive;

        debugText.text +="���ڴ򿪴���\n";
        //��������
        sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        sp.ReadTimeout = 10;
        try
        {
            sp.Open();
            debugText.text += "�򿪳ɹ�\n";
        }
        catch (Exception ex)
        {
            debugText.text += ex.Message + "\n";
            Debug.Log(ex.Message);
        }
    }




  
    void OnApplicationQuit()
    {
        if (sp != null)
        {
            try
            {
                sp.Close();
                dataReceiveThread.Abort();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);

            }
        }
    }
    public void ClosePort()
    {
        if (sp != null)
        {
            try
            {
                sp.Close();
                //  dataReceiveThread.Abort();
                debugText.text += "�رմ���\n";
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                debugText.text += ex.Message + "\n";
            }
        }
    }
    void DataReceiveFunction()
    {   
        byte[] buffer = new byte[1024];
        int bytes = 0;
        while (true)
        {       
            if (sp != null && sp.IsOpen)
            {
                try
                {
                    bytes = sp.Read(buffer, 0, buffer.Length);//�����ֽ�
                    if (bytes == 0)
                        continue;
                    else
                    {
                        string strbytes = Encoding.Default.GetString(buffer);                     
                        debugText.text += strbytes + "\n";
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ThreadAbortException))
                    {
                    }
                }
            }
            Thread.Sleep(10);
        }
       
    }


    public string HexToStr(byte[] data)
    {
        StringBuilder sb = new StringBuilder(data.Length * 3);
        foreach (byte b in data)
        {
            sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            if (sb.Length == 18)//�Լ�ѡ����Ҫ�ĳ��� ��Ȼ���кܶ�00000
                break;
        }

        return sb.ToString();
    }


}