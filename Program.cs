using System;
using System.Collections.Generic;
//using System.Linq;
using System.Windows.Forms;

namespace MyFarm
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>        
        public static FrmLogin formLogin = null;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formLogin = new FrmLogin();
            Application.Run(formLogin);
        }
    }
}
