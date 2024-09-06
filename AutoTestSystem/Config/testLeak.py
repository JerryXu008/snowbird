#!/usr/bin/python
#coding:utf-8
import binascii
import serial  
import time
import argparse


def test(portname, cmd, delay):
            t = serial.Serial(portname, 115200)  
            hex_string = cmd.replace(" ", "")
            byte_data = ''.join(chr(int(hex_string[i:i+2], len(hex_string))) for i in range(0, len(hex_string), 2))
            sendData= byte_data
            sendData =  bytes(sendData)

            n =t.write(sendData)
            time.sleep(delay)    
            num=t.inWaiting()

            if num: 
                    try:   
                        data = binascii.b2a_hex(t.read(num))
                        print("result:["+data+"]")
                    except:
                        str = t.read(num)  
                        print(str)  
                        
            serial.Serial.close(t)

if __name__ == '__main__':
    
    parser = argparse.ArgumentParser(description='Audio Test.')
    parser.add_argument('-p', '--serialport', help='Serial port name.', type=str, default=False, required=True)
    parser.add_argument('-cmd', '--cmd', help='cmd.', type=str, default="", required=True)
    parser.add_argument('-d', '--delay', help='Delay time.', type=float, default=1, required=False)
   
    args = parser.parse_args()
    test(portname=args.serialport, cmd=args.cmd, delay=args.delay)



 