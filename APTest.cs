using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using Renci;
using Renci.SshNet;

public class APTest {

    public APTest(string SelfIP, string CamIP, string AP_MAc, string pass, string gateway, string mask)
    {
        this.SelfIP = SelfIP;
        this.CamIP = CamIP;
        this.AP_MAc = AP_MAc;
        this.pass = pass;
        this.gateway = gateway;
        this.mask = mask;
        AP = false;
    }
    public APTest(string SelfIP, string CamIP, string pass, string gateway, string mask)
    {
        this.SelfIP = SelfIP;
        this.CamIP = CamIP;
        this.pass = pass;
        this.gateway = gateway;
        this.mask = mask;
        AP = true;
    }
    bool AP;
    string SelfIP;
    string CamIP; 
    string AP_MAc;
    string pass;
    string gateway;
    string mask;

    public static string get_mac(string host, string name, string pass)
    {
        ScpClient scp = new ScpClient(host, 22, name, pass);
        scp.Connect();
        FileInfo file = new FileInfo("C:\\fw\\Mac.txt");
        scp.Download("/etc/board.info", file);
        scp.Disconnect();
        FileStream file_s = file.OpenRead();
        string data;
        Regex re = new Regex(@"hwaddr=(\w+)\n");
        
        using (StreamReader sr = new StreamReader(file_s))
        {
            data = sr.ReadToEnd();
        }
        var match = re.Match(data);
        return match.Groups[1].Value;
    }
    public static void fwupdate(string host, string name, string pass)
    {
        SshClient ssh = new SshClient(host, 22, name, pass);
        ScpClient scp = new ScpClient(host, 22, name, pass);
        scp.Connect();
        FileInfo file = new FileInfo("C:\\fw\\fwupdate.bin");
        scp.Upload(file, "/tmp/fwupdate.bin");
        scp.Disconnect();
        ssh.Connect();
        ssh.RunCommand("/sbin/fwupdate -m");
        ssh.Disconnect();
        ssh.Dispose();
    }
    public void conf_update(string host, string name, string pass)
    {
        SshClient ssh = new SshClient(host, 22, name, pass);
        ScpClient scp = new ScpClient(host, 22, name, pass);
        string file_reader;
        if (AP)
        {
            // file = new FileInfo("C:\\fw\\AP_conf.cfg");
            file_reader = File.ReadAllText("C:\\fw\\AP_conf.cfg");
            file_reader += "netconf.3.ip=" + SelfIP + '\n' + "pwdog.host =" + CamIP;
            StreamWriter sw = new StreamWriter("C:\\fw\\system.cfg");
            sw.WriteLine(file_reader);
            sw.Close();
        }
        else
        {
            file_reader = File.ReadAllText("C:\\fw\\ST_conf.cfg");
            file_reader += "netconf.3.ip=" + SelfIP + '\n' + 
                            "pwdog.host =" + CamIP + '\n' + 
                            "wpasupplicant.profile.1.network.1.bssid=" + AP_MAc + '\n' + 
                            "wireless.1.ap=" + AP_MAc;
            StreamWriter sw = new StreamWriter("C:\\fw\\system.cfg");
            sw.WriteLine(file_reader);
            sw.Close();
        }
        FileInfo file = new FileInfo("C:\\fw\\system.cfg");

        scp.Connect();
        scp.Upload(file, "/tmp/system.cfg");
        scp.Disconnect();
        ssh.Connect();
        ssh.RunCommand("cfgmtd -f /tmp/system.cfg -w");
    try
    {
        ssh.RunCommand("/usr/etc/rc.d/rc.softrestart save");
    }
    catch
    {
    }
        ssh.Disconnect();
        ssh.Dispose();
    }

    
}
