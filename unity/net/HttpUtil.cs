using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Threading;

public class HttpUtil : MonoBehaviour {

    private bool _cut = false;          //是否断开，测试用

    private string tempPath;            //下载临时存储路径

    private string savePath;            //解压后存储路径

    private string downPath;            //下载地址

    private int maxData = 0;            //需要下载多大文件

    private int currentData = 0;        //已下载多少

    private bool success = false;       //是否下载成功

    private bool connect = true;        //是否正在连接

    private int openingZipState = 1;    //解压状态 1没有解压，2正在解压，3解压完成

    private int _newResVersion = 1;

    public void DeownloadFile(string http,int newResVersion )
    {
        //http://192.168.0.157/xxx
        downPath = "http://"+ http;
        _newResVersion = newResVersion;
        //记录路径
        tempPath = Application.temporaryCachePath;
        savePath = Application.persistentDataPath;

        //重置变量
        openingZipState = 1;
        _cut = false;
        success = false;


        //开辟新线程
        thread = new Thread(threadstart);
        thread.Start();
    }

    ///
    /// 下载文件方法
    ///
    /// 文件保存路径和文件名
    /// 返回服务器文件名
    ///
    public bool DeownloadFile(string http,string strFileName,string type)
    {
        connect = true;
        bool flag = false;
        //打开上次下载的文件
        long SPosition = 0;
        //实例化流对象
        FileStream FStream;
        //判断要下载的文件夹是否存在

        if (File.Exists(tempPath + "/" + strFileName + type))
        {
            //打开要下载的文件
            FStream = File.OpenWrite(tempPath + "/" + strFileName + type);
            //获取已经下载的长度
            SPosition = FStream.Length;
            FStream.Seek(SPosition, SeekOrigin.Current);
        }
        else
        {
            //文件不保存创建一个文件
            FStream = new FileStream(tempPath + "/" + strFileName + type, FileMode.Create);
            SPosition = 0;
        }
        try
        {
            ////获取文件大小
            HttpWebRequest myRequestTest = (HttpWebRequest)HttpWebRequest.Create(http);
            maxData = (int)((myRequestTest.GetResponse().ContentLength) / 1024);
            Debug.Log("maxData="+maxData);
            if (SPosition >= myRequestTest.GetResponse().ContentLength)
            {
                success = true;
                FStream.Close();
                myRequestTest.Abort();
                return true;
            }
            myRequestTest.Abort();

            //打开网络连接
            HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(http);
            if (SPosition > 0)
                myRequest.AddRange((int)SPosition);             //设置Range值

            //向服务器请求,获得服务器的回应数据流
            Stream myStream = myRequest.GetResponse().GetResponseStream();
            //maxData = (int)((myRequest.GetResponse().ContentLength + (int)SPosition) / 1024);

            ////定义一个字节数据
            byte[] btContent = new byte[512];
            int intSize = 0;
            intSize = myStream.Read(btContent, 0, 512);

            while (intSize > 0)
            {
                if (_cut)
                {
                    myRequest.Abort();
                    break;
                }
                FStream.Write(btContent, 0, intSize);
                intSize = myStream.Read(btContent, 0, 512);
                currentData = (int)(FStream.Length/1024);
            }


            //关闭流
            FStream.Close();
            myStream.Close();
            flag = true;        //返回true下载成功

            if (currentData >= maxData)
            {
                openingZipState = 2;
                success = true;
                if (type == ".zip")//判断如果是zip的话就解压到指定文件夹,并删除原文件
                {
                    if(File.Exists(tempPath + "/" + strFileName + type))
                    {
                        ZipHelper.UnZip(tempPath + "/" + strFileName + type, savePath + "/" + "config/" , "", true);
                        File.Delete(tempPath + "/" + strFileName + type);
                    }
                }
                else//一般文件则直接拷贝
	            {
                    if (File.Exists(tempPath + "/" + strFileName + type))
                    {
                        File.Copy(tempPath + "/" + strFileName + type, savePath + "/" + "config/", true);
                        File.Delete(tempPath + "/" + strFileName + type);
                    }
	            }
                openingZipState = 3;
            }
            else
            {
                connect = false;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            FStream.Close();
            flag = false;       //返回false下载失败
            connect = false;
        }
        return flag;
    }
}

/// <summary>
/// 方便查看路径
//pc_win7:
//{
//    dataPath            : ${项目路径}/Assets
//    persistentDataPath  : %userprofile%/AppData/LocalLow/${用户名}/${appname}
//    temporaryCachePath  : %userprofile%/AppData/Local/Temp/${用户名}/${appname}
//}

//Android:
//{
//    dataPath            : /data/app/xxx.apk(只读)
//    persistentDataPath  : mnt/sdcard/Android/data/com.company.xxx/files (读写，存放持久化数据)
//    temporaryCachePath  : mnt/sdcard/Android/data/com.company.xxx/cache
//}

//IOS:
//{
//    dataPath            :/var/mobile/Applications/xx-x-x-x-xx/${AppName.app}/Data(只读)
//    persistentDataPath     :/var/mobile/Applications/xx-x-x-x-xx/${AppName.app}/Documents (读写，存放持久化数据)
//    temporaryCachePath  :/var/mobile/Applications/xx-x-x-x-xx/${AppName.app}/Library/Caches
//}
/// </summary>