#!/usr/bin/python
#coding:utf-8
import binascii
import serial  
import time
import argparse


def test(portname,cmd):
            t = serial.Serial(portname,115200)  
            #print(t.portstr)  
            
            #strInput= b'\x01\x03\x00\x0C\x00\x01\x44\x09'

            hex_string = cmd.replace(" ", "")
            byte_data = ''.join(chr(int(hex_string[i:i+2], len(hex_string))) for i in range(0, len(hex_string), 2))
            sendData= byte_data
            sendData =  bytes(sendData)

            n =t.write(sendData)
            time.sleep(1)     #sleep() 与 inWaiting() 最好配对使用
            num=t.inWaiting()
            print(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>byte num:",num)

            if num: 
                    try:   #如果读取的不是十六进制数据--
                        
                        data = binascii.b2a_hex(t.read(num))
                        #data= str(binascii.b2a_hex(t.read(num))) [2:-1] #十六进制显示方法2
                        
                        print("result:["+data+"]")
                    except: #--则将其作为字符串读取
                        str = t.read(num)  
                        print(str)  
                        
            serial.Serial.close(t)
            #print(portname+" close") 

if __name__ == '__main__':
    
    parser = argparse.ArgumentParser(description='Audio Test.')
    parser.add_argument('-p', '--serialport', help='Serial port name.', type=str, default=False, required=True)
    parser.add_argument('-cmd', '--cmd', help='cmd.', type=str, default="", required=True)
   
    args = parser.parse_args()
    
    #print (args)
    test(portname=args.serialport,cmd=args.cmd)
