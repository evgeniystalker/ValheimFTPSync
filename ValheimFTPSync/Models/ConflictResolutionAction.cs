using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    /// <summary>
    /// Определяет действие, которое должно быть предпринято при обнаружении конфликта данных.
    /// </summary>
    enum ConflictResolutionAction
    {
        /// <summary>
        /// Перезаписать конфликтное значение новым значением, игнорируя старое.
        /// </summary>
        Overwrite,
        /// <summary>
        /// Пропустить обработку текущего конфликтующего элемента, сохранив существующее значение.
        /// </summary>
        Skip,
        /// <summary>
        /// Перезаписать все конфликтующие значения в рамках текущей операции.
        /// </summary>
        OverwriteAll,
        /// <summary>
        /// Пропустить обработку всех конфликтующих элементов в рамках текущей операции.
        /// </summary>
        SkipAll,
        /// <summary>
        /// Отменить всю операцию из-за конфликта.
        /// </summary>
        Cancel
    }


}