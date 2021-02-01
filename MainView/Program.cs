﻿using MainView.Mock;
using System;
using System.Windows.Forms;

namespace MainView
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            Presenter presenter = new Presenter(new Model(), form);
            Application.Run(form);
        }
    }
}
