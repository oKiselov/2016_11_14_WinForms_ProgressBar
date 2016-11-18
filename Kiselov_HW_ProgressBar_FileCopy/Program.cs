using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
Написать программу копирования файла. 
Пользователь должен указать путь к копируемому файлу и путь куда копировать. 
Во время копирования файла должен отображаться прогресс копирования 
( и окно должно реагировать на действия пользователя желательно но не обязательно ).
Обязательно проверки и комментарии.

Апргейдить предыдущее ДЗ по копированию файла. 
В момент копирования файла еще должна отображаться скорость копирования по выбору пользователя:
1) Мб/сек
2) Байт/сек
Обязательное требование проверки и комментарии.
 */

namespace Kiselov_HW_ProgressBar_FileCopy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
