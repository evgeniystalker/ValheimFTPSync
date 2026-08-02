using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Configuration
{
    internal interface ISettingManager
    {
        /// <summary>
        /// Сохранение всех настроек в файл конфигурации.
        /// </summary>
        public void Save();

        /// <summary>
        /// Загрузка настроек из файла конфигурации.
        /// </summary>
        public void Initialize();

        /// <summary>
        /// Применить настройки по умолчанию.
        /// </summary>
        public void Default();

        /// <summary>
        /// Сброс файла настроек.
        /// </summary>
        public void Reset();
    }
}
