﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Verwaltet die Anwendungseinstellungen.
    /// TODO: In den Client verschieben
    /// </summary>
    public class Settings : ISettings
    {
        /// <summary>
        /// Bei UnitTests ist Assembly.GetEntryAssembly null, Gründe dazu gibts auf StackOverflow.
        /// Um Schmerzen zu vermeiden wurde eine Variable eingeführt, die unabhängig testet.
        /// </summary>
        public bool DEBUG = false;

        private Configuration _config;

        public Settings()
        {
            if (DEBUG)
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap {ExeConfigFilename = "EXECONFIG_PATH"};
                _config = ConfigurationManager.OpenMappedExeConfiguration(map,
                    ConfigurationUserLevel.None);
            }
            else
            {
                _config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            }
        }

        /// <summary>
        /// Gibt den Wert einer Einstellung zurück.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <returns>Der Wert der Einstellung.</returns>
        public T Get<T>(string key)
        {
            // var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);

            
            var valueConfig = _config.AppSettings.Settings[key].Value;

            return (T) Convert.ChangeType(valueConfig, typeof(T));
        }

        /// <summary>
        /// Gibt das Array einer Einstellung zurück
        /// </summary>
        /// <typeparam name="T">Art der Einstellung</typeparam>
        /// <param name="key">Schlüssel der Einstellung</param>
        /// <returns>Das Array der Einstellung</returns>
        public T[] GetArray<T>(string key)
        {
            var valueConfig = _config.AppSettings.Settings[key].Value;

            return DeserializeArray<T>(valueConfig);
        }

        /// <summary>
        /// Überprüft, ob die angegebene Einstellung existeiert.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
          

            return _config.AppSettings.Settings.AllKeys.Contains(key);
        }

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="value">Der Wert der Einstellung.</param>
        public void Set(string key, string value)
        {
        

            if (_config.AppSettings.Settings.AllKeys.Contains(key))
                _config.AppSettings.Settings[key].Value = value;
            else
                _config.AppSettings.Settings.Add(key, value);
            _config.Save(ConfigurationSaveMode.Modified, false);
        }

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="value">Der Wert der Einstellung.</param>
        public void Set(string key, int value)
        {
            Set(key, Convert.ToString(value));
        }

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="value">Der Wert der Einstellung.</param>
        public void Set(string key, bool value)
        {
            Set(key, Convert.ToString(value));
        }

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="values">Der Wert der Einstellung.</param>
        public void Set(string key, string[] values)
        {
            /* Wir bauen das Array in eine Art serialisierten String um.
             * Wenn eine Zeichenkette, die wir aus den Einstellugen lesen mit einer
             * eckigen Klammer anfängt, ist es ein Array.
             * [value1, value2, value3]
             * 
             */

            string writeString = "[" + String.Join(",", values) + "]";
            Set(key, writeString);
        }

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="values">Der Wert der Einstellung.</param>
        public void Set(string key, int[] values)
        {
            string[] strValues = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                strValues[i] = Convert.ToString(values[i]);
            }
            Set(key, strValues);
        }

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="values">Der Wert der Einstellung.</param>
        public void Set(string key, bool[] values)
        {
            string[] stringValues = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                stringValues[i] = Convert.ToString(values[i]);
            }
            Set(key, stringValues);
        }


        private T[] DeserializeArray<T>(string arrayString)
        {
            // Wir müssten, um beide Klammern zu entfernen, - 3 rechnen. Ich lasse die letzte Klammer stellvertretend für das Komma, was folgen würde, stehen.
            // Das wird in der for-Schleife auseinander gepflückt.


            arrayString = arrayString.Substring(1, arrayString.Length - 2 /*- 1*/);

            string[] partsString = arrayString.Split(',');
            T[] tArray = new T[partsString.Length];
            for (int i = 0; i < partsString.Length; i++)
            {
                tArray[i] = (T) Convert.ChangeType(partsString[i], typeof(T));
            }


            return tArray;
        }
    }
}