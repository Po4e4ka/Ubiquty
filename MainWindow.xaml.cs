using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Net.NetworkInformation;
using System.Windows.Media;
using System.Threading;

namespace Ubiquty
{
    public partial class MainWindow : Window
    {
        //Дополнительные параметры
        bool flag_auto = true;
        string mask_IP = "255.255.248.0";
        string gateway_IP = "192.168.0.1";
        string password = "Request1106";
        string host = "192.168.1.20";
        string name = "ubnt";
        string pass = "ubnt";
        bool radio = false;
        private readonly Timer timer;
        StreamWriter Log_File;        

        public MainWindow()
        {
            InitializeComponent();
            radiobutton.IsChecked = true;
            Log_File = new StreamWriter("Log_File.txt", true);
            Log_File.WriteLine("\n/----------Запуск {0}----------/", DateTime.Now);
            Log_File.Close();

            button_ping.Visibility = Visibility.Hidden;

            button_fw.IsEnabled = false;
            button_prog.IsEnabled = false;
            timer = new Timer(new TimerCallback(Count), null, 0, 3000);
        }

        int Test_IP_and_Mac()
        {
            List<TextBox> textbox = new List<TextBox> { textbox1, textbox3, textbox4 };

            string[] split;

            for (int i = 0; i <= 2; i++)
            {
                split = textbox[i].Text.Split('.');

                if (split.Length != 4)
                {
                    System.Windows.MessageBox.Show("Некорректный ввод IP адрессов", "Сообщение", MessageBoxButton.OK,
                                      MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                    return 1;
                }

                if (Convert.ToInt16(split[0]) > 255 || Convert.ToInt16(split[1]) > 255 || Convert.ToInt16(split[2]) > 255 || Convert.ToInt16(split[3]) > 255 ||
                    Convert.ToInt16(split[0]) < 0 || Convert.ToInt16(split[1]) < 0 || Convert.ToInt16(split[2]) < 0 || Convert.ToInt16(split[3]) < 0)
                {
                    System.Windows.MessageBox.Show("Октет больше 255", "Сообщение", MessageBoxButton.OK,
                                      MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                    return 1;
                }
            }

            if (radio == false)
            {
                if (!textbox2.IsMaskCompleted)
                {
                    System.Windows.MessageBox.Show("Заполните MacAP", "Сообщение", MessageBoxButton.OK,
                                           MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                    return 2;
                }


                split = textbox2.Text.Split(':');
                foreach (string elem in split)
                {
                    try { uint a = Convert.ToUInt32(elem, 16); }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        System.Windows.MessageBox.Show("Введите корректный MacAP", "Сообщение", MessageBoxButton.OK,
                                      MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                        return 2;
                    }
                }
            }
            return 0;
        }

        private void Button_Click_FW(object sender, RoutedEventArgs e)
        {
            APTest.fwupdate(host, name, pass);
            System.Windows.MessageBox.Show("Залив прошивки успешный. Дождитесь обнаружения устройства", "Сообщение", MessageBoxButton.OK,
                                      MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
        }
        
        private void Button_Click_PING(object sender, RoutedEventArgs e)
        {
            Ping ping = new Ping();
            PingReply pr = ping.Send(host);
            if (pr.Status == IPStatus.Success)
            {
                label_ping.Foreground = Brushes.Green;
                label_ping.Content = "Устройство \nобнаружено";
            }
            else
            {
                label_ping.Foreground = Brushes.Red;
                label_ping.Content = "Устройство \nне обнаружено";
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Log_File = new StreamWriter("Log_File.txt", true);
            try
            {
                // Проверка на корректность вводимых данных
                if (Test_IP_and_Mac() > 0)
                {
                    Log_File.Close();
                    return;
                }

                if (radio == false)
                {
                    // ST
                    APTest Test = new APTest(textbox4.Text, textbox3.Text, textbox2.Text, password, gateway_IP, mask_IP);
                    //Test.fwupdate(host, name, pass);
                    Test.conf_update(host, name, pass);
                }

                if (radio == true)
                {
                    // AP
                    textbox2.Text = APTest.get_mac(host, name, pass);
                    APTest Test = new APTest(textbox1.Text, textbox3.Text, password, gateway_IP, mask_IP);
                    //Test.fwupdate(host, name, pass);
                    Test.conf_update(host, name, pass);
                }


                System.Windows.MessageBox.Show("Программирование прошло успешно", "Сообщение", MessageBoxButton.OK,
                                MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

                if (radio) Log_File.WriteLine("Успешное программирование Access Point. \n\tIP:" + textbox1.Text);
                else Log_File.WriteLine("Успешное программирование Station. \n\tIP:" + textbox4.Text);
                Log_File.Close();
            }
            catch(Exception el)
            {
                string er = el.ToString();
                MessageBox.Show(er);
                Log_File.WriteLine("Исключение: " + er);
                System.Windows.MessageBox.Show("Повторите попытку" +
                                               "ещё раз", "Инфо", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information,
                                               System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                //Close();
                Log_File.Close();
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            textbox2.Visibility = Visibility.Visible;
            label2.Visibility = Visibility.Visible;
            radio = false;
        }
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {

            textbox2.Visibility = Visibility.Hidden;
            label2.Visibility = Visibility.Hidden;
            radio = true;
        }
        private void textbox1_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (flag_auto == false) return;
                string temp = textbox1.Text;
                string[] mass;
                mass = temp.Split('.');
                int octet = Convert.ToInt32(mass[3]);
                octet++;
                textbox4.Text = String.Format("{0}.{1}.{2}.{3}", mass[0], mass[1], mass[2], octet);
                octet++;
                textbox3.Text = String.Format("{0}.{1}.{2}.{3}", mass[0], mass[1], mass[2], octet);
            }
            catch
            {
                return;
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            textbox2.IsEnabled = true;
            textbox3.IsEnabled = true;
            textbox4.IsEnabled = true;
            flag_auto = false;
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            textbox2.IsEnabled = false;
            textbox3.IsEnabled = false;
            textbox4.IsEnabled = false;
            flag_auto = true;
            textbox1_LostFocus(sender, e);
        }
        public void Count(object obj)
        {
            this.Dispatcher.Invoke(() =>
            {               
            Ping ping = new Ping();
            PingReply pr = ping.Send(host, 20);
            if (pr.Status == IPStatus.Success)
            {
                    if (!button_fw.IsEnabled)
                    {
                        button_fw.IsEnabled = true;
                        button_prog.IsEnabled = true;
                        label_ping.Foreground = Brushes.Green;
                        label_ping.Content = "Устройство \nобнаружено";
                        Console.Beep();
                    }                
            }
            else
            {
                    if (button_fw.IsEnabled)
                    {
                        button_fw.IsEnabled = false;
                        button_prog.IsEnabled = false;
                        label_ping.Foreground = Brushes.Red;
                        label_ping.Content = "Устройство \nне обнаружено";
                    }
            }
            });
        } //Ебанная асинхронность

        private void info_Click(object sender, RoutedEventArgs e)
        {
            StreamReader info = new StreamReader("info.txt");
            MessageBox.Show(info.ReadToEnd(), "Инструкция", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information,
                                               System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
        }               
    }
}
